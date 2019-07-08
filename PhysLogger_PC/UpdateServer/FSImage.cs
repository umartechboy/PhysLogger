using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace UpdateServer
{
    public class FSEntry
    {
        public string AbsolutePath { get; set; }
        public string RootDirectory
        {
            get
            {
                return Top.AbsolutePath;
            }
        }
        public void RebuildHash()
        {
            hash_ = null;
            var dummy = Hash;
            if (Parent != null)
                Parent.RebuildHash();
        }
        public void FindParent(string top)
        {
            if (top.ToLower() == AbsolutePath.ToLower())
                Parent = null;
            else
            {
                Parent = new DirectoryItem(Path.GetDirectoryName(AbsolutePath));
                Parent.FindParent(top);
            }
        }
        protected string forcedRelativePath = null;
        public string RelativePath
        {
            get
            {
                if (forcedRelativePath != null)
                    return forcedRelativePath;
                string p = AbsolutePath;
                string r = RootDirectory;
                try
                {
                    return p.Substring(r.Length).TrimStart(new char[] { '\\' });
                }
                catch { return ""; }

            }
        }
        public virtual string Name { get; set; }
        
        public DirectoryItem Parent { get; set; }
        public DirectoryItem Top
        {
            get
            {
                if (Parent != null)
                    return Parent.Top;
                else
                    return (DirectoryItem)this;
            }
        }
        protected byte[] hash_ = null;
        public virtual byte[] Hash { get { return hash_; } }
        public string MD5
        { get { return BitConverter.ToString(Hash).Replace("-", "").ToLowerInvariant(); } }        
    }
    public class FileItem: FSEntry
    {
        public FileItem(string file)
        {
            AbsolutePath = file;
        }
        public FileItem(string file, string forcedRelativePath)
        {
            AbsolutePath = file;
            this.forcedRelativePath = forcedRelativePath;
        }
        //public FileItem(string file, string top)
        //{
        //    AbsolutePath = file;
        //    FindParent(top);
        //    RebuildHash();
        //}
        public override string ToString()
        {
            return "File > " + Name + " " + MD5;
        }
        public override byte[] Hash
        {
            get
            {
                if (hash_ != null)
                    return hash_;
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = File.OpenRead(AbsolutePath))
                    {
                        var hash = md5.ComputeHash(stream);
                        hash_ = hash;
                        return hash_;
                    }
                }
            }
        }
        public override string Name
        {
            get { return System.IO.Path.GetFileName(AbsolutePath); }
            set { File.Move(AbsolutePath, System.IO.Path.Combine(Parent.AbsolutePath, value)); }
        }
    }
    public class DirectoryItem : FSEntry
    {
        public DirectoryItem(string dir)
        {
            AbsolutePath = dir;
        }
        public DirectoryItem(string dir, string forcedRelativePath)
        {
            AbsolutePath = dir;
            this.forcedRelativePath = forcedRelativePath;
        }
        //public DirectoryItem(string dir, string top)
        //{
        //    AbsolutePath = dir;
        //    FindParent(top);
        //    RebuildHash();
        //}
        public List<FSEntry> Children { get; set; } = new List<FSEntry>();
        public List<FSEntry> AllChildren
        {
            get
            {
                var cs = Flatten();
                cs.Remove(this);
                return cs;
            }
        }
        public static DirectoryItem FromDirectoryScan(string dir, string[] topLevelIgnore = null)
        {
            if (topLevelIgnore == null)
                topLevelIgnore = new string[] { };
            DirectoryItem dThis = new DirectoryItem(dir);
            foreach (var d in Directory.GetDirectories(dir))
            {
                bool cont = false;
                foreach (var item in topLevelIgnore)
                    if (Path.GetFileName(d.ToLower()) == item.ToLower())
                        cont = true;
                if (cont) continue;
                var dDown = FromDirectoryScan(d, new string[] { });
                dDown.Parent = dThis;
                dThis.Children.Add(dDown);
            }
            foreach (var f in Directory.GetFiles(dir))
            {
                bool cont = false;
                foreach (var item in topLevelIgnore)
                    if (Path.GetFileName(f.ToLower()) == item.ToLower())
                        cont = true;
                if (cont) continue;
                var fDown = new FileItem(f);
                fDown.Parent = dThis;
                dThis.Children.Add(fDown);
            }
            return dThis;
        }
        public override byte[] Hash
        {
            get
            {
                if (hash_ != null)
                    return hash_;
                hash_ = new byte[16];
                foreach (var c in Children)
                {
                    for (int i = 0; i < c.Hash.Length; i++)
                        hash_[i] ^= c.Hash[i];
                }
                return hash_;
            }
        }
        public override string ToString()
        {
            return "Dir > " + Name + " " + MD5;
        }

        public void AppendFileStructureUpdateScript(StringBuilder sb)
        { }
        FSEntry Find(FSEntry entry)
        {
            if (this.AbsolutePath == entry.AbsolutePath && this.GetType() == entry.GetType())
                return this;
            foreach (var c in Children)
            {
                if (c is FileItem)
                {
                    if (c.AbsolutePath == entry.AbsolutePath && c.GetType() == entry.GetType())
                        return c;
                }
                else
                {
                    var f = ((DirectoryItem)c).Find(entry);
                    if (f != null)
                        return f;
                }
            }
            return null;
        }
        public List<FSEntry> Flatten(List<FSEntry> list = null)
        {
            if (list == null)
                list = new List<UpdateServer.FSEntry>();
            else
                list.Add(this);
            foreach (var c in Children)
            {
                if (c is FileItem)
                    list.Add(c);
                else
                    ((DirectoryItem)c).Flatten(list);
            }
            return list;
        }
        public override string Name
        {
            get { return System.IO.Path.GetFileName(AbsolutePath); }
            set { Directory.Move(AbsolutePath, System.IO.Path.Combine(Parent.AbsolutePath, value)); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServer
{
    public class UpdateCommand
    {
        internal static UpdateCommand Parse(string line, string rootDir)
        {
            var parts = line.Split(new char[] { ' ', '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts[0].ToLower() == "rmdir")
                return new DeleteDirectoryCommand(new FileItem(System.IO.Path.Combine(rootDir,parts[1]), parts[1]));
            else if (parts[0].ToLower() == "rmfile")
                return new DeleteFileCommand(new FileItem(System.IO.Path.Combine(rootDir, parts[1]), parts[1]));
            else if (parts[0].ToLower() == "copy")
                return new UpdateOrCopyCommand(new FileItem(System.IO.Path.Combine(rootDir, parts[1]), parts[1]));
            else if (parts[0].ToLower() == "mkdir")
                return new MakeDirectoryCommand(new FileItem(System.IO.Path.Combine(rootDir, parts[1]), parts[1]));
            else if (parts[0].ToLower() == "version")
                return new VersionSetCommand(int.Parse(parts[1]));
            else
                throw new FormatException();
        }
        public string Serialize()
        { return ToString(); }
    }
    public class VersionSetCommand : UpdateCommand
    {
        public int Version { get; protected set; }
        public VersionSetCommand(int version)
        {
            Version = version;
        }
        public override string ToString()
        {
            return "Version = " + Version;
        }
    }
    public class DeleteDirectoryCommand: UpdateCommand
    {
        public FSEntry Target { get; set; }
        public DeleteDirectoryCommand(FSEntry entry)
        {
            Target = entry;
        }
        public override string ToString()
        {
            return "rmdir " + Target.RelativePath;
        }
    }
    public class DeleteFileCommand: UpdateCommand
    {
        public FSEntry Target { get; set; }
        public DeleteFileCommand(FSEntry entry)
        {
            Target = entry;
        }
        public override string ToString()
        {
            return "rmfile " + Target.RelativePath;
        }
    }
    public class UpdateOrCopyCommand : UpdateCommand
    {
        public FSEntry Target { get; set; }
        public UpdateOrCopyCommand(FSEntry target)
        {
            Target = target;
        }
        public override string ToString()
        {
            return "copy " + Target.RelativePath;
        }
    }

    public class MakeDirectoryCommand : UpdateCommand
    {
        public FSEntry Target { get; set; }
        public MakeDirectoryCommand(FSEntry target)
        {
            Target = target;
        }
        public override string ToString()
        {
            return "mkdir " + Target.RelativePath;
        }
    }
}

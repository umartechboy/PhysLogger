using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UpdateServer
{
    class UpdateProcessor
    {
        public static void MakeUpdateStructure(string source)
        {
            if (!Directory.Exists(Path.Combine(source, "ApplicationUpdates")))
                Directory.CreateDirectory(Path.Combine(source, "ApplicationUpdates"));
            if (!Directory.Exists(Path.Combine(source, "ApplicationUpdates\\Release")))
                Directory.CreateDirectory(Path.Combine(source, "ApplicationUpdates\\Release"));
            if (!File.Exists(Path.Combine(source, "ApplicationUpdates\\Release\\UpdateScript.txt")))
                File.WriteAllText(Path.Combine(source, "ApplicationUpdates\\Release\\UpdateScript.txt"), "version=0\r\n");
        }
        public static List<FSEntry> AssumeUpdates(List<FSEntry> target, UpdateScript script)
        {
            foreach (var com in script.Commands)
            {
                if (com is DeleteDirectoryCommand)
                {
                    var dc = (DeleteDirectoryCommand)com;
                    var found = target.Find(fe => fe.RelativePath == dc.Target.RelativePath);
                    if (found != null)
                        target.Remove(found);
                }
                else if (com is DeleteFileCommand)
                {
                    var dc = (DeleteFileCommand)com;
                    var found = target.Find(fe => fe.RelativePath == dc.Target.RelativePath);
                    if (found != null)
                        target.Remove(found);
                }
                else if (com is UpdateOrCopyCommand)
                {
                    var sample = (UpdateOrCopyCommand)com;
                    var found = target.Find(fe => fe.RelativePath == sample.Target.RelativePath);
                    if (found != null)
                        target.Remove(found);
                    target.Add(sample.Target);
                }
                else if (com is MakeDirectoryCommand)
                {
                    var sample = (MakeDirectoryCommand)com;
                    var found = target.Find(fe => fe.RelativePath == sample.Target.RelativePath);
                    if (found != null)
                        target.Remove(found);
                    target.Add(sample.Target);
                }
            }
            return target;
        }
        public static UpdateScript CreateComparisonScript(string sourceDirectory, string targetDirectory)
        {
            var source = DirectoryItem.FromDirectoryScan(sourceDirectory).Flatten();
            var target = DirectoryItem.FromDirectoryScan(targetDirectory).Flatten();
            return CreateComparisonScript(source, target);
        }
        public static UpdateScript CreateComparisonScript(List<FSEntry> source, List<FSEntry> target)
        {
            var coms = new List<UpdateCommand>();
            foreach (var fet in target)
            {
                if (source.Find(fe =>
                    fet.GetType() == fe.GetType()
                    && fet.RelativePath == fe.RelativePath) == null)
                {
                    if (fet is DirectoryItem)
                        coms.Add(new DeleteDirectoryCommand(fet));
                    else
                        coms.Add(new DeleteFileCommand(fet));
                }
            }
            foreach (var fe in source)
            {
                if (target.Find(fet =>
                    fet.GetType() == fe.GetType()
                    && fet.RelativePath == fe.RelativePath) == null)
                {
                    if (fe is DirectoryItem)
                        coms.Add(new MakeDirectoryCommand(fe));
                }
            }
            foreach (var fe in source)
            {
                if (target.Find(fet =>
                    fet.MD5 == fe.MD5
                    && fet.GetType() == fe.GetType()
                    && fet.RelativePath == fe.RelativePath) == null)
                {
                    if (fe is FileItem)
                        coms.Add(new UpdateOrCopyCommand(fe));
                }
            }
            return new UpdateScript(coms);
        }
    }

}

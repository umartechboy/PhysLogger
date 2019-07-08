using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ionic.Zip;

namespace UpdateServer
{
    public partial class UpdateCreator : Form
    {
        string WorkingDirectory = "";
        public UpdateCreator()
        {
            InitializeComponent();
            WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            UpdateProcessor.MakeUpdateStructure(WorkingDirectory);
        }

        UpdateScript newUpdateScript;
        DirectoryItem latest;
        int newVersion = 0;

        private void scanChangesB_Click(object sender, EventArgs e)
        {
            var updateStructure = new string[] { "ApplicationUpdates", "UpdateScript.txt", "UpdatePackage.zip" };

            latest = DirectoryItem.FromDirectoryScan(WorkingDirectory, updateStructure);
            DirectoryItem release = DirectoryItem.FromDirectoryScan(Path.Combine(WorkingDirectory, "ApplicationUpdates\\Release"), updateStructure);
            List<DirectoryItem> updateSources = new List<DirectoryItem>();
            List<UpdateScript> updateScripts = new List<UpdateScript>();
            foreach (var updateDir in Directory.GetDirectories(Path.Combine(WorkingDirectory, "ApplicationUpdates"), "update*"))
            {
                updateSources.Add(DirectoryItem.FromDirectoryScan(updateDir, updateStructure));
                updateScripts.Add(UpdateScript.Read(updateDir));
            }
            List<FSEntry> finalExisting = release.Flatten();
            foreach (var updateScript in updateScripts)
                UpdateProcessor.AssumeUpdates(finalExisting, updateScript);
            newUpdateScript = UpdateProcessor.CreateComparisonScript(latest.Flatten(), finalExisting);
            newVersion = 0;
            if (updateScripts.Count > 0)
                newVersion = updateScripts.Max(us => us.Version) + 1;
            else
            {
                if (release.Flatten().Count == 0) // its the first release
                    newVersion = 1;
                else
                    newVersion = 2;
            }
            createUpdateB.Enabled = newUpdateScript.Commands.Count > 0;
            newUpdateScript.Version = newVersion;
            totalComsL.Text = newUpdateScript.Commands.Count.ToString();
            additionsL.Text = newUpdateScript.Commands.Count(com => com is UpdateOrCopyCommand).ToString();
            newDiresL.Text = newUpdateScript.Commands.Count(com => com is MakeDirectoryCommand).ToString();
            removalsL.Text = newUpdateScript.Commands.Count(com => com is DeleteDirectoryCommand || com is DeleteFileCommand).ToString();
            versionL.Text = createUpdateB.Enabled ? newVersion.ToString() : "--";
            
        }

        void CreateFile(string source, string target)
        {
            CreateDir(Path.GetDirectoryName(target));
            File.Copy(source, target);
        }
        void CreateDir(string path)
        {
            if (Directory.Exists(path))
                return;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                CreateDir(Path.GetDirectoryName(path));
            Directory.CreateDirectory(path);
        }
        private void createUpdateB_Click(object sender, EventArgs e)
        {
            string updateDir = "Update " + (newVersion - 1);
            if (newVersion == 1) // its a release
                updateDir = "Release";
            updateDir = Path.Combine(WorkingDirectory, "ApplicationUpdates\\" + updateDir);
            if (Directory.Exists(updateDir))
                Directory.Delete(updateDir, true);
            Directory.CreateDirectory(updateDir);
            File.WriteAllLines(Path.Combine(updateDir, "UpdateScript.txt"), newUpdateScript.Commands.Select(com => com.Serialize()).ToArray());
            foreach (var com_ in newUpdateScript.Commands)
            {
                if (com_ is UpdateOrCopyCommand)
                {
                    var com = (UpdateOrCopyCommand)com_;
                    if (com.Target is FileItem)
                        CreateFile(com.Target.AbsolutePath, Path.Combine(updateDir, com.Target.RelativePath));
                    else
                        CreateDir(Path.Combine(updateDir, com.Target.RelativePath));
                }
                else if (com_ is MakeDirectoryCommand)
                {
                    var com = (MakeDirectoryCommand)com_;
                    CreateDir(Path.Combine(updateDir, com.Target.RelativePath));
                }
            }

            ZipFile zipped = new ZipFile();
            foreach (var com in newUpdateScript.Commands)
                if (com is UpdateOrCopyCommand)
                {
                    zipped.AddFile(Path.Combine(updateDir, ((UpdateOrCopyCommand)com).Target.RelativePath), Path.GetDirectoryName(((UpdateOrCopyCommand)com).Target.RelativePath));
                }
            zipped.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
            zipped.Save(Path.Combine(updateDir, "UpdatePackage.zip"));
            foreach (var com_ in newUpdateScript.Commands)
            {
                if (com_ is UpdateOrCopyCommand)
                {
                    var com = (UpdateOrCopyCommand)com_;
                    File.Delete(Path.Combine(updateDir, com.Target.RelativePath));
                }
                else if (com_ is MakeDirectoryCommand)
                {
                    var com = (MakeDirectoryCommand)com_;
                    Directory.Delete(Path.Combine(updateDir, com.Target.RelativePath));
                }
            }
        }
    }
}

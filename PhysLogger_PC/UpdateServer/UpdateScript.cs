using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UpdateServer
{
    class UpdateScript
    {
        public UpdateScript()
        { }
        public UpdateScript(List<UpdateCommand> coms)
        {
            Commands = coms;
        }
        public List<string> Script { get; protected set; } = new List<string>();
        public List<UpdateCommand> Commands { get; protected set; } = new List<UpdateCommand>();
        public int Version
        {
            get { return ((VersionSetCommand)(Commands.Find(c => c is VersionSetCommand))).Version; }
            set { Commands.RemoveAll(c => c is VersionSetCommand); Commands.Add(new VersionSetCommand(value)); }
        }

        public static UpdateScript Read(string updateDir)
        {
            var script = new UpdateScript();
            script.Script = File.ReadAllLines(Path.Combine(updateDir, "UpdateScript.txt")).ToList();

            foreach (var line in script.Script)
            {
                script.Commands.Add(UpdateCommand.Parse(line, updateDir));
            }
            return script;
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UpdateServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            //{
            //    Application.Run(new UpdateClient());
            //})).Start();
            Application.Run(new UpdateCreator());
            
        }
    }
}

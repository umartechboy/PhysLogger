using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysLogger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //LoggerSimulator.Program.Main(null);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //FivePointNine.LicenseManager.ManagerBase mb = new FivePointNine.LicenseManager.ManagerBase();
            //mb.GUIResourceRequired += Mb_GUIResourceRequired;
            //mb.Initialize("Qosain PhysLogger 1.0");
            //var lic = mb.GetLicense();

            //if (lic != null)
            //{
            //    mb.CleanUp();
            //    Application.Run(new MainForm());
            //}
            Form splash = new Form();
            splash.BackgroundImage = Properties.Resources.Splash;
            splash.Size = Properties.Resources.Splash.Size;
            splash.StartPosition = FormStartPosition.CenterScreen;
            splash.FormBorderStyle = FormBorderStyle.None;
            splash.Show();
            System.Threading.Thread.Sleep(3000);
            splash.Hide();
            Application.Run(new MainForm());
        }

        private static object Mb_GUIResourceRequired(FivePointNine.LicenseManager.ResourceKind resourceType, Type dataTypeToReturn, string requirements)
        {
            if (resourceType == FivePointNine.LicenseManager.ResourceKind.Splash)
                return Properties.Resources.Splash;
            else if (resourceType == FivePointNine.LicenseManager.ResourceKind.FormTitle)
                return "Qosain Scientific PhysLogger";
            else if (resourceType == FivePointNine.LicenseManager.ResourceKind.RegisterFormHead)
                return Properties.Resources.FormHead;
            else if (resourceType == FivePointNine.LicenseManager.ResourceKind.HowToRegister)
                return "You need a valid license file to run Qosain PhysLogger. Kindly note down the below mentioned machine code and contact Qosain Scientific to acquire a license file (*.lic) against it.";
            else
                return null;
        }
    }
}

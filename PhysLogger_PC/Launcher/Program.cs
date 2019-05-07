using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Launcher
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
            var e = DotNetIsInstalled();
            if (!e)
            {
                if (MessageBox.Show("It looks like Microsoft .Net Framework 4.5.2 or higher is required to run PhysLogger. Do you want to install .Net Framework now?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return;
                var p = System.Diagnostics.Process.Start("Redist\\.Net Framework 4.5.2 Offline Installer.exe");
                p.WaitForExit();
            }
            System.Diagnostics.Process.Start("PhysLogger.exe");
        }
        static string VerInfo()
        {
            string s = "";
            // Opens the registry key for the .NET Framework entry. 
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                // As an alternative, if you know the computers you will query are running .NET Framework 4.5  
                // or later, you can use: 
                // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,  
                // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {

                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                            s+=(versionKeyName + "  " + name) + "\r\n";
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                s+=(versionKeyName + "  " + name + "  SP" + sp) + "\r\n";
                            }

                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                s+=(versionKeyName + "  " + name) + "\r\n";
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    s+=("  " + subKeyName + "  " + name + "  SP" + sp) + "\r\n";
                                }
                                else if (install == "1")
                                {
                                    s+=("  " + subKeyName + "  " + name) + "\r\n";
                                }

                            }

                        }

                    }
                }
            }
            return s;
        }
        static bool DotNetIsInstalled()
        {
            try
            {// Opens the registry key for the .NET Framework entry. 
                using (RegistryKey ndpKey =
                    RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                    OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                {
                    foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                    {
                        if (versionKeyName.StartsWith("v4"))
                        {

                            RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                            string name = (string)versionKey.GetValue("Version", "");
                            string sp = versionKey.GetValue("SP", "").ToString();
                            string install = versionKey.GetValue("Install", "").ToString();
                            if (name != "")
                            {
                                continue;
                            }
                            foreach (string subKeyName in versionKey.GetSubKeyNames())
                            {
                                RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                                var subVer = ((string)subKey.GetValue("Version", "")).Split(new char[] { '.' });
                                if (subVer[0] == "4" && Convert.ToInt32(subVer[1]) >= 5 && Convert.ToInt32(subVer[2]) >= 5)
                                    return true;
                            }

                        }
                    }
                }

                return false;
            }
            catch (Exception ex){ MessageBox.Show(ex.ToString()); return false; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysLogger.Forms
{
    public partial class PhysLoggerUpdaterConsole : Form
    {
        public PhysLoggerUpdaterConsole()
        {
            InitializeComponent();
        }
        public bool IsActive { get; protected set; }
        bool canExit = false;
        
        private void PhysLoggerUpdaterConsole_KeyDown(object sender, KeyEventArgs e)
        {

        }
        
        public void SafeExit()
        {
            WriteLine("\r\n\r\nPress any key to exit...");
            ReadKey();
            canExit = true;
            Close();
        }
        string toAppend = "";
        public void Write(string str)
        {
            if (str == null)
                return;
            toAppend += str;
            Application.DoEvents();
        }
        public void WriteLine(string str = "")
        {
            if (str == null)
                return;
            Write(str + "\r\n");
        }
        int TypedLength = 0;
        int typeCursor = 0;
        bool EnterDown = false;
        bool awaitingKey = false;
        char KeyRead = ' ';
        bool userCanType = false;
        public string ReadKey()
        {
            awaitingKey = true;
            KeyRead = (char)0;
            while (KeyRead == (char)0)
            {
                Application.DoEvents();
            }
            if (KeyRead == 0)
                return "";
            var s = KeyRead.ToString();
            return s;
        }
        public string ReadLine()
        {
            userCanType = true;
            TypedLength = 0;
            typeCursor = 0;
            EnterDown = false;
            while (!EnterDown)
            {
                Application.DoEvents();
            }
            string s = consoleTB.Text.Substring(consoleTB.Text.Length - TypedLength - 2);
            s = s.Substring(0, s.Length - 2);
            userCanType = false;
            return s;
        }
        private void consoleTB_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void consoleTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (!awaitingKey)
            {
                if (e.KeyCode == Keys.Up)
                    e.SuppressKeyPress = true;
                else if (e.KeyCode == Keys.Enter)
                    EnterDown = true;
                else if (e.KeyCode == Keys.Delete)
                { if (typeCursor < TypedLength) TypedLength--; }
                else if (e.KeyCode == Keys.Right)
                { if (typeCursor < TypedLength) typeCursor++; }
                else if (e.KeyCode == Keys.Left)
                { if (typeCursor > 0) typeCursor--; else e.SuppressKeyPress = true; }
                else if (e.KeyCode == Keys.Back)
                { if (typeCursor > 0 && TypedLength > 0) { typeCursor--; TypedLength--; } else e.SuppressKeyPress = true; }
                else if (e.KeyCode == Keys.End)
                    typeCursor = TypedLength;
                else if (e.KeyCode == Keys.Home)
                {
                    if (e.Shift == true)
                    { e.SuppressKeyPress = true; return; }
                    consoleTB.SelectionStart = consoleTB.Text.Length - TypedLength;
                    typeCursor = 0;
                    e.SuppressKeyPress = true;
                }
                else
                {
                    char c = (char)e.KeyCode;
                    if (!char.IsControl(c))
                    {
                        typeCursor++; TypedLength++;
                    }
                    else
                        e.SuppressKeyPress = true;
                }
            }
            else
            {
                char c = (char)e.KeyCode;

                if (!char.IsControl(c))
                {
                    KeyRead = c;
                }
                else
                    KeyRead = ' ';
            }
            if (!userCanType)
                e.SuppressKeyPress = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (toAppend != "")
            {
                consoleTB.AppendText(toAppend);
                toAppend = "";
                Application.DoEvents();
            }
        }

        private void PhysLoggerUpdaterConsole_Shown(object sender, EventArgs e)
        {
            IsActive = true;
            canExit = false;
        }

        private void PhysLoggerUpdaterConsole_Load(object sender, EventArgs e)
        {
            IsActive = true;
            canExit = false;
        }

        private void PhysLoggerUpdaterConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!canExit)
                e.Cancel = true;
            else
                IsActive = false;

        }
    }
}

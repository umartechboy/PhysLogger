using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Web;
using System.Net.Sockets;

namespace UpdateServer
{
    public partial class Form1 : Form
    {
        Socket listener;
        public Form1()
        {
            InitializeComponent();
        }

        System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
        private void button1_Click(object sender, EventArgs e)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 5095));
            listener.Listen(100);
            listener.BeginAccept(new AsyncCallback(Listener), listener);
            while (true)
            {   
                if (mre.WaitOne(30))
                {
                    listener.BeginAccept(new AsyncCallback(Listener), listener);
                    mre.Reset();
                }
                Application.DoEvents();
            }
        }
        
        void Listener(IAsyncResult ir)
        {
        }
    }
}

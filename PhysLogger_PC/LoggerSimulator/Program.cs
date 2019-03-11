using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using FivePointNine.Windows.IO;

namespace LoggerSimulator
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        public static void Main(string[] args)
        {
            AllocConsole();
            Program p = new LoggerSimulator.Program();
            Thread t = new Thread(p.Main2);
            t.Start();
        }
        
        void Main2()
        {         
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 5093));
            server.Listen(1);
            while (true)
            {
                allDone.Reset();
                server.BeginAccept(AcceptReceived, server);
                Console.WriteLine("Listening");
                allDone.WaitOne(); 
            }
        }

        ManualResetEvent allDone = new ManualResetEvent(false);
        void AcceptReceived(IAsyncResult ar)
        {
            var s = (Socket)ar.AsyncState;
            s.EndAccept(ar);   
            SerialDataChannel dc = new SocketChannel(s);

            allDone.Set();
            Console.WriteLine("Got Client");
            Thread.Sleep(1000); // simulate reset time
            //new PacketCommandMini(1, "ver=PhysLogger1_0", 0, 0).SendCommand(dc);
            while (true)
            { 
                Thread.Sleep(10);
            //    var command = new PacketCommand();
            //    command.PayLoadLength = 9;
            //    var t = DateTime.Now.Millisecond / 1000.0F;
            //    writeBytes(command.PayLoad,BitConverter.GetBytes((UInt32)DateTime.Now.Millisecond), 0);
            //    Int16 a0 = Math.
            //    writeBytes(command.PayLoad, BitConverter.GetBytes((UInt32)DateTime.Now.Millisecond), 0);
            //    Int16 a0 = command.PayLoad[4];
            //    Int16 a1 = command.PayLoad[5];
            //    Int16 a2 = command.PayLoad[6];
            //    Int16 a3 = command.PayLoad[7];
            //    a0 += (Int16)(((command.PayLoad[8] >> 0) & 0x3) << 8);
            //    a1 += (Int16)(((command.PayLoad[8] >> 2) & 0x3) << 8);
            //    a2 += (Int16)(((command.PayLoad[8] >> 4) & 0x3) << 8);
            //    a3 += (Int16)(((command.PayLoad[8] >> 6) & 0x3) << 8);
            //    new PacketCommand(4, new byte[] { }, 0, 0).SendCommand(dc);

            }
        }
        void writeBytes(byte[] buffer, byte[] values, int start)
        {

        }
    }
}

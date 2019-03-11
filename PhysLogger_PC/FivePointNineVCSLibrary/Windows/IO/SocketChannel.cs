using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.IO.Ports;
using System.Net;

namespace FivePointNine.Windows.IO
{
    public class SocketChannel  :SerialDataChannel
    {
        public Socket socket { get; set; }
        public override int ReadBufferSize { get; set; }
        public override bool DtrEnable
        {
            get; set;
        }
        string ipAddress = "";
        public SocketChannel(string ip)
        {
            ipAddress = ip;
        }
        public SocketChannel(Socket soc)
        {
            try
            {
                ipAddress = ((IPEndPoint)soc.RemoteEndPoint).ToString();
            }
            catch { }
            socket = soc;
        }
        public override void Open()
        {
            IPEndPoint ipep =
                new IPEndPoint(IPAddress.Parse(ipAddress), 5093);
            Socket server = new Socket(AddressFamily.InterNetwork,
                              SocketType.Stream, ProtocolType.Tcp);
            try
            {                     
                server.Connect(ipep);
                socket = server;
            }
            catch { socket = null; throw new Exception("Server not available."); }
        }
        public override void Close()
        {
            socket.Close();
        }
        public override string Address
        {
            get { return ipAddress; }

            set { ipAddress = value; }
        }
        public override int BytesToRead
        {
            get
            {
                if (socket.Available > 0)
                    ;
                return socket.Available;
            }
        }
        public override bool IsOpen
        {
            get
            {
                return socket != null;
            }
        }
        public override int Read(byte[] data, int offset, int length)
        {
            return socket.Receive(data, offset, length, SocketFlags.None);
        }
        public override int ReadByte()
        {
            byte[] buf = new byte[1];
            if (Read(buf, 0, 1) == 1)
                return buf[0];
            else return -1;
        }
        public override void Write(byte[] data, int offset, int length)
        {
            try
            {
                socket.Send(data, offset, length, SocketFlags.None);
            }
            catch (Exception ex)
            { }
            socket.Send(data, offset, length, SocketFlags.None);
        }
    }
}

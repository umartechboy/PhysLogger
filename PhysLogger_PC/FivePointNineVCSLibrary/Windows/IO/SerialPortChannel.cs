using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace FivePointNine.Windows.IO
{
    public class SerialPortChannel : SerialDataChannel
    {
        SerialPort sp;
        public SerialPortChannel(string com, int baudrate)
        {
            sp = new SerialPort(com, baudrate);
        }
        public override void Open() { sp.Open(); }
        public override void Close()
        { sp.Close(); }
        public override void Write(byte[] data, int offset, int length)
        { sp.Write(data, offset, length); }
        public override int Read(byte[] data, int offset, int length)
        {
            return sp.Read(data, offset, length);
        }
        public override int BytesToRead { get { return sp.BytesToRead; } }
        public override int ReadByte()
        {
            return sp.ReadByte();
        }
        public override bool IsOpen { get { return sp.IsOpen; } }
        public override bool DtrEnable { get { return sp.DtrEnable; } set { sp.DtrEnable = value; } }
        public override int ReadBufferSize { get { return sp.ReadBufferSize; } set { sp.ReadBufferSize = value; } }
        public override string Address { get { return sp.PortName; } set { sp.PortName = value; } }
    }
}

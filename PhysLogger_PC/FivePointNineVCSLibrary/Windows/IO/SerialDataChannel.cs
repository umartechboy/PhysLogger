using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivePointNine.Windows.IO
{
    public abstract class SerialDataChannel
    {
        public abstract void Open();
        public abstract void Close();
        public abstract void Write(byte[] data, int offset, int length);
        public abstract int Read(byte[] data, int offset, int length);
        public abstract int BytesToRead { get; }
        public abstract int ReadByte();
        public abstract bool IsOpen { get; }
        public abstract bool DtrEnable { get; set; }
        public abstract int ReadBufferSize { get; set; }
        public abstract string Address { get; set; }
        public bool WaitForBytes(int numberOfBytes, int delayms)
        {
            int count = 0;
            while (BytesToRead < numberOfBytes && count < delayms)
            {
                System.Threading.Thread.Sleep(20);
                count += 20;
                System.Windows.Forms.Application.DoEvents();
            }
            return BytesToRead >= numberOfBytes;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

//              0,   1,           2,       3,    4,    5
// 0xAA, 0x55,  pID, PLL.LoW, PLL.High, SrcID, TgtID, TCS
namespace FivePointNine.Windows.IO
{
    public class PacketCommandID
    {
        byte value = 0;
        public const byte Debug = 90;
        public const byte Ping = 94;
        public PacketCommandID(byte value)
        {
            this.value = value;
        }
        protected PacketCommandID()
        { }
        public static implicit operator PacketCommandID(byte value)
        {
            return new PacketCommandID(value);
        }
        public static implicit operator byte(PacketCommandID value)
        {
            return value.value;
        }
    }

    public enum ProtocolError : byte
    {
        Unknown = 0,
        None = 4,
        ReadTimeout = 7,
        CheckSumMismatch = 10,
        BufferOverFlow = 15,
    }
    public class PacketCommandMini
    {
        public byte[] data_ = new byte[0];
        byte[] comData_ = new byte[2];
        static void DoEvents()
        {
            //System.Windows.Forms.Application.DoEvents();
        }
        public static ProtocolError FromStream(ref PacketCommandMini command, SerialDataChannel serial, int timeOut, System.Windows.Forms.TextBox flush = null)
        {
            long start = DateTime.Now.Millisecond;

            bool hasAA = false;
            while (!hasAA && (DateTime.Now.Millisecond - start) <= timeOut)
            {
                if (serial.BytesToRead < 3)
                    continue;
                if (serial.ReadByte() == 0xAA)
                {
                    hasAA = true;
                    break;
                }
            }
            if (!hasAA)
                return ProtocolError.ReadTimeout;
            serial.Read(command.comData_,0, 2);
            command.PayLoad = new byte[command.PayLoadLength];
            int dlen = command.PayLoadLength;
            int i = 0;
            while (i < dlen && (DateTime.Now.Millisecond - start) <= timeOut)
            {
                if (serial.BytesToRead > 0)
                    command.PayLoad[i++] = (byte)serial.ReadByte();
            }
            if (i < dlen) // not all of the bytes were received
            {
                return ProtocolError.ReadTimeout;
            }
            // calculate checksum
            if (command.TrueCheckSum != command.ActualCheckSum)
            {
                return ProtocolError.CheckSumMismatch;
            }

            // we got the command. letes return
            return ProtocolError.None;
        }
        public void SendCommand(SerialDataChannel serial_)
        {
            try
            {
                serial_.Write(new byte[] { 0xAA }, 0, 1);
                TrueCheckSum = ActualCheckSum;
                serial_.Write(comData_, 0, 2);
                if (PayLoadLength > 0)
                    serial_.Write(data_, 0, PayLoadLength);
            }
            catch
            {
            }
        }
        public string PayLoadString
        {
            get { return new UTF8Encoding().GetString(PayLoad); }
            set { PayLoad = new UTF8Encoding().GetBytes(value); }
        }
        public byte[] PayLoad
        {
            get
            {
                return data_;
            }
            set
            {
                PayLoadLength = (byte)value.Length;
                data_ = new byte[PayLoadLength];
                if (value.Length > 0)
                    Buffer.BlockCopy(value, 0, data_, 0, data_.Length);

            }
        }
        
        public PacketCommandID PacketID
        {
            get
            {
                return (byte)(comData_[0] & 0x07);
            }
            set
            {
                comData_[0] &= 0xF8;
                comData_[0] |= (byte)(value & 0x07);
            }
        }
        public PacketCommandMini()
        {
            PayLoad = new byte[0];
            PacketID = new PacketCommandID(0);
        }
        public PacketCommandMini(PacketCommandID commandID)
        {
            PayLoad = new byte[0];
            PacketID = commandID;
        }
        
        public PacketCommandMini(PacketCommandID commandID, byte[] payload)
        {
            PacketID = commandID;
            PayLoad = payload;
        }
        byte TrueCheckSum
        {
            get
            {
                return comData_[1];
            }
            set
            {
                comData_[1] = value;
            }
        }
        public byte PayLoadLength
        {
            get
            {
                return (byte)(comData_[0] >> 3);
            }
            set
            {
                if (value > 30)
                    throw new Exception("Overflow");
                comData_[0] &= 0x07;
                comData_[0] |= (byte)(value << 3);
            }
        }
        byte ActualCheckSum
        {
            get
            {
                byte sum = (byte)(0x55 ^ comData_[0]);
                for (int i = 0; i < PayLoadLength; i++)
                    sum ^= data_[i];
                return sum;
            }
        }

    }
}

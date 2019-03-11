using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.IO;

namespace PhysLogger
{
    public delegate void SetPointHandler(float x, float [] y);
    public delegate void GainChangedHandler(int index, int gain);
    
    public class PhysLoggerHardware
    {
        public event EventHandler OnSignatureUpdate;
        public PhysLoggerHWProperties HWProps { get; set; } = new PhysLoggerHWProperties(PhysLoggerHWSignature.Unknown);
        public event GainChangedHandler OnNewGainValues;
        public event SetPointHandler NewPointReceived;

        public int[] SupportedGains { get { return HWProps.SupportedGains; } }
        public int TotalChannels { get { return HWProps.TotalChannels; } }
        public void ParseCommand(PacketCommand command, SerialDataChannel channel)
        {
            if (command.PacketID == PhysLoggerPacketCommandID.GetHWSignatures)
            {
                var parts = command.PayLoadString.Split(new char[] { '=' });
                if (parts[0] == "ver")
                {
                    if (parts[1] == PhysLoggerHWSignature.PhysLogger1_0.ToString())
                    {
                        HWProps = new PhysLogger1_0HWProperties();
                        HWProps.OnNewGainValues += HWProps_OnNewGainValues;
                        new PacketCommand(PhysLoggerPacketCommandID.BeginFire, new byte[] { 1 }).SendCommand(channel);
                    }
                    else
                        return;
                    OnSignatureUpdate(this, new EventArgs());
                }
                else
                {
                    if (HWProps != null)
                    {
                        if (HWProps.UpdateProp(parts))
                        { OnSignatureUpdate?.Invoke(this, new EventArgs()); }
                    }
                }
            }
            if (HWProps.Signature == PhysLoggerHWSignature.Unknown)
            {
                new PacketCommand(PhysLoggerPacketCommandID.GetHWSignatures).SendCommand(channel);
                return;
            }
            if (command.PacketID == PhysLoggerPacketCommandID.DataPacket)
            {
                float t = 0;
                float[] values = null;
                HWProps.HandleDataPacket(command,ref t, ref values);
                if (values != null)
                    NewPointReceived(t, values);
            }
        }

        private void HWProps_OnNewGainValues(int index, int gain)
        {
            OnNewGainValues(index, gain);
        }

        public bool ChangeChannelGain(int index, int gain, SerialDataChannel channel)
        {
            new PacketCommand(PhysLoggerPacketCommandID.ChangeChannelGain, new byte[] { (byte)index, (byte)gain }).SendCommand(channel);
            return true;
        }
        public bool SetChannelType(int index, ChannelType cType, SerialDataChannel channel)
        {
            new PacketCommand(PhysLoggerPacketCommandID.ChangeChannelGain, new byte[] { (byte)index, (byte)cType}).SendCommand(channel);
            return true;
        }
    }
    public class PhysLoggerPacketCommandID : FivePointNine.Windows.IO.PacketCommandID
    {
        public const byte GetHWSignatures = 1;
        public const byte ChangeChannelType = 2;
        public const byte ChangeChannelGain = 3;
        public const byte DataPacket = 4;
        public const byte BeginFire = 5;
    }
    public class PhysLoggerHWProperties
    {
        public virtual event GainChangedHandler OnNewGainValues;
        public PhysLoggerHWProperties(PhysLoggerHWSignature signature)
        { Signature = signature; }
        public PhysLoggerHWSignature Signature { get; protected set; } = PhysLoggerHWSignature.Unknown;
        public virtual void HandleDataPacket(PacketCommand command, ref float f, ref float[] values)
        { new NotImplementedException(); }

        internal virtual bool UpdateProp(string[] parts)
        {
            return false;
            //throw new NotImplementedException();
        }

        public int[] SupportedGains { get; protected set; }
        public int TotalChannels { get; protected set; }
    }
    public class PhysLogger1_0HWProperties : PhysLoggerHWProperties
    {
        public override event GainChangedHandler OnNewGainValues;
        public PhysLogger1_0HWProperties() : base(PhysLoggerHWSignature.PhysLogger1_0)
        { }
        public float MaxADC { get; set; }
        public float MaxVoltage { get; set; }
        public override void HandleDataPacket(PacketCommand command, ref float t, ref float[] values)
        {
            t = BitConverter.ToUInt32(command.PayLoad, 0) / 1000.0F;
            Int16 a0 = command.PayLoad[4];
            Int16 a1 = command.PayLoad[5];
            Int16 a2 = command.PayLoad[6];
            Int16 a3 = command.PayLoad[7];
            a0 += (Int16)(((command.PayLoad[8] >> 0) & 0x3) << 8);
            a1 += (Int16)(((command.PayLoad[8] >> 2) & 0x3) << 8);
            a2 += (Int16)(((command.PayLoad[8] >> 4) & 0x3) << 8);
            a3 += (Int16)(((command.PayLoad[8] >> 6) & 0x3) << 8);

            byte gains = command.PayLoad[9];
            int gain0 = SupportedGains[(gains >> 0) & 3];
            int gain1 = SupportedGains[(gains >> 2) & 3];
            int gain2 = SupportedGains[(gains >> 4) & 3];
            int gain3 = SupportedGains[(gains >> 6) & 3];
            values = new float[] {
                    (a0 / MaxADC) * MaxVoltage / gain0,
                    (a1 / MaxADC) * MaxVoltage / gain1,
                    (a2 / MaxADC) * MaxVoltage / gain2,
                    (a3 / MaxADC) * MaxVoltage / gain3
                    };
            OnNewGainValues(0, gain0);
            OnNewGainValues(1, gain1);
            OnNewGainValues(2, gain2);
            OnNewGainValues(3, gain3);
        }
        internal override bool UpdateProp(string[] parts)
        {
            if (parts[0] == "MaxVoltage")
                MaxVoltage = Convert.ToSingle(parts[1]);
            else if (parts[0] == "MaxADC")
                MaxADC = Convert.ToSingle(parts[1]);
            else if (parts[0] == "tChannels")
            {
                TotalChannels = Convert.ToInt16(parts[1]);
                return true;
            }
            else if (parts[0] == "sgains")
            {
                var gains = parts[1].Split(new char[] { ',' });
                List<int> gainL = new List<int>();
                foreach (var gain in gains)
                {
                    gainL.Add(Convert.ToInt16(gain));
                }
                SupportedGains = gainL.ToArray();
                return true;
            }
            return false;
        }
    }
    public enum ChannelType:byte
    {
        DifferentialADCWithInternalGain = 0,
        DifferentialADCWithExternalGain = 1,
        Thermister = 2
    }
    public enum PhysLoggerHWSignature
    {
        Unknown,
        PhysLogger1_0
    }
}

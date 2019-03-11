using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.IO;
using PhysLogger.LogControls;

namespace PhysLogger.Hardware
{
    public delegate void SetPointHandler(float x, float [] y, PlotLabel [] labels);
    public delegate void GainChangedHandler(int index, int gain);
    public delegate void SendCommandRequest(PacketCommandMini pc);
    public class LoggerHardware
    {
        protected List<ChannelOption> ChannelOptions;
        public virtual event EventHandler OnSignatureUpdate;
        public virtual event SendCommandRequest OnCommandSendRequest;
        public virtual event valueChangeHandler OnSamplingRateChanged;
        public PhysLoggerHWSignature Signature { get; protected set; } = PhysLoggerHWSignature.Unknown;
        public virtual void HandleDataPacket(PacketCommandMini command, ref float f, ref float[] values, ref PlotLabel[] labels)
        { new NotImplementedException(); }
        public virtual event SetPointHandler NewPointReceived;
        public void AttachVirtualLogger1()
        {
            Signature = PhysLoggerHWSignature.PhysLogger1_0;
            OnSignatureUpdate(this, new EventArgs());
        }
        public void ParseCommand(PacketCommandMini command, SerialDataChannel channel)
        {
            if (command.PacketID == PhysLoggerPacketCommandID.GetHWSignatures)
            {
                var parts = command.PayLoadString.Split(new char[] { '=' });
                if (parts[0] == "ver")
                {
                    if (parts[1] == PhysLoggerHWSignature.PhysLogger1_0.ToString())
                    {
                        Signature = PhysLoggerHWSignature.PhysLogger1_0;
                        new PacketCommandMini(PhysLoggerPacketCommandID.BeginFire, new byte[] { 1 }).SendCommand(channel);
                    }
                    else
                        return;
                    OnSignatureUpdate(this, new EventArgs());
                }      
            }
            if (Signature == PhysLoggerHWSignature.Unknown)
            {
                new PacketCommandMini(PhysLoggerPacketCommandID.GetHWSignatures).SendCommand(channel);
                return;
            }
            if (command.PacketID == PhysLoggerPacketCommandID.DataPacket)
            {
                float t = 0;
                float[] values = null;
                PlotLabel[] labels = null;
                HandleDataPacket(command, ref t, ref values, ref labels);
                if (values != null)
                    NewPointReceived(t, values, labels);
            }
            else if (command.PacketID == PhysLoggerPacketCommandID.ChangeSamplingFrequency)
            {
                var ts = BitConverter.ToUInt32(command.PayLoad, 0);
                OnSamplingRateChanged.Invoke(1 / (float)ts * 1000);
            }
        }
        public virtual void ChangeSamplingRate(float frequencyInHz)
        {
            throw new NotImplementedException();
        }

        internal virtual ChannelOption UpdateChannelOptions(ChannelOption[] allOptions, int id)
        {
            throw new NotImplementedException();
        }
        public virtual List<ChannelOption> EnumerateChannelOptions()
        {
            throw new NotImplementedException();
        }
        public List<float> SupportedFrequencies { get; protected set; }
    }
    public class PhysLoggerPacketCommandID : FivePointNine.Windows.IO.PacketCommandID
    {
        public const byte GetHWSignatures = 1;
        public const byte ChangeChannelType = 2;
        public const byte ChangeChannelRange = 3;
        public const byte DataPacket = 4;
        public const byte BeginFire = 5;
        public const byte ChangeSamplingFrequency = 6;
    }                                     
    public class PhysLogger1_0HW : LoggerHardware
    {
        public override event SendCommandRequest OnCommandSendRequest;
        public PhysLogger1_0HW()
        {
            Signature = PhysLoggerHWSignature.PhysLogger1_0;
            SupportedFrequencies = new List<float>();
            SupportedFrequencies.Add(1000);
            SupportedFrequencies.Add(500);
            SupportedFrequencies.Add(250);
            SupportedFrequencies.Add(200);
            SupportedFrequencies.Add(100);
            SupportedFrequencies.Add(50);
            SupportedFrequencies.Add(40);
            SupportedFrequencies.Add(25);
            SupportedFrequencies.Add(20);
            SupportedFrequencies.Add(10);
            SupportedFrequencies.Add(5);
            SupportedFrequencies.Add(2);
            SupportedFrequencies.Add(1);
            SupportedFrequencies.Add(0.5F);
            SupportedFrequencies.Add(0.25F);
            SupportedFrequencies.Add(0.2F);
            SupportedFrequencies.Add(0.1F);
        }
        public int[] SupportedGains = new int[] { 1, 10, 200 };
        protected float MaxInputVoltage = 6.0F;
        protected float Vref = 2.56F;
        protected float InputVoltageDivider = 3.0F;
        public override void HandleDataPacket(PacketCommandMini command, ref float t, ref float[] values, ref PlotLabel[] labels)
        {
            t = BitConverter.ToUInt32(new byte[] {
            command.PayLoad[0],
            command.PayLoad[1],
            command.PayLoad[2],
            (byte)0 }, 0) / 1000.0F;
            Int16 a0 = command.PayLoad[3];
            Int16 a1 = command.PayLoad[4];
            Int16 a2 = command.PayLoad[5];
            Int16 a3 = command.PayLoad[6];
            a0 += (((command.PayLoad[7] >> 0) & 1) == 1) ? (Int16)256 : (Int16)0;
            a1 += (((command.PayLoad[7] >> 2) & 1) == 1) ? (Int16)256 : (Int16)0;
            a2 += (((command.PayLoad[7] >> 4) & 1) == 1) ? (Int16)256 : (Int16)0;
            a3 += (((command.PayLoad[7] >> 6) & 1) == 1) ? (Int16)256 : (Int16)0;
            a0 *= (((command.PayLoad[7] >> 1) & 1) == 0) ? (Int16)1 : (Int16)(-1);
            a1 *= (((command.PayLoad[7] >> 3) & 1) == 0) ? (Int16)1 : (Int16)(-1);
            a2 *= (((command.PayLoad[7] >> 5) & 1) == 0) ? (Int16)1 : (Int16)(-1);
            a3 *= (((command.PayLoad[7] >> 7) & 1) == 0) ? (Int16)1 : (Int16)(-1);
            byte gains = command.PayLoad[8];
            int gain0 = SupportedGains[(gains >> 0) & 3];
            int gain1 = SupportedGains[(gains >> 2) & 3];
            int gain2 = SupportedGains[(gains >> 4) & 3];
            int gain3 = SupportedGains[(gains >> 6) & 3];
            values = new float[] {
                    analogToOutValue((a0 / 512.0F) * Vref * InputVoltageDivider / gain0, 0),
                    analogToOutValue((a1 / 512.0F) * Vref * InputVoltageDivider / gain1, 1),
                    analogToOutValue((a2 / 512.0F) * Vref * InputVoltageDivider / gain2, 2),
                    analogToOutValue((a3 / 512.0F) * Vref * InputVoltageDivider / gain3, 3)
                    };
            //values = new float[] { a0, a1, a2, a3 };
            labels = new PlotLabel []
                {
                    SelectedInstruments[0] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[0].UnitConversions.Current.Label,
                    SelectedInstruments[1] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[1].UnitConversions.Current.Label,
                    SelectedInstruments[2] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[2].UnitConversions.Current.Label,
                    SelectedInstruments[3] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[3].UnitConversions.Current.Label,
                };
        }
        protected virtual float analogToOutValue(float voltage, int cID)
        {
            if (SelectedInstruments[cID] != null)
                return SelectedInstruments[cID].TF(voltage);
            else
                return voltage;
        }

        protected List<GroupHeadOption> typeOp = new List<GroupHeadOption>();
        //var sourceOp = new GroupHeadOption("Source");
        protected List<GroupHeadOption> differentialGainOps = new List<GroupHeadOption>();
        protected List<GroupHeadOption> RSEGainOps = new List<GroupHeadOption>();
        //List<GroupHeadOption> instrumentTypeOps = new List<GroupHeadOption>();
        protected List<GroupHeadOption> resetPhysLabLoadCellOps = new List<GroupHeadOption>();
        protected List<Instrument> SelectedInstruments = new List<Instrument>();
        protected List<Instrument[]> AvailableInstruments = new List<Instrument[]>();
        public override List<ChannelOption> EnumerateChannelOptions()
        {
            ChannelOptions = new List<ChannelOption>();
            for (int index = 0; index < 4; index++)
            {
                ChannelOption ops = new GroupHeadOption("Channel Options");
                typeOp.Add(new GroupHeadOption("Type"));
                //var sourceOp = new GroupHeadOption("Source");
                differentialGainOps.Add(new GroupHeadOption("Input Range"));
                typeOp[index].Parent = ops;
                //sourceOp.Parent = ops;
                differentialGainOps[index].Parent = ops;
                typeOp[index].SubOptions.Add(new ChannelTypeOption(ChannelType.AnalogInDifferential));
                typeOp[index].SubOptions.Add(new ChannelTypeOption(ChannelType.AnalogInRSE));
                typeOp[index].SubOptions[0].Checked = true;

                for (int i = 0; i < 2; i++)
                {
                    typeOp[index].SubOptions[i].MenuItem.Click += TypeChange_Click;
                    typeOp[index].SubOptions[i].Parent = typeOp[index];
                }

                //for (int i = 0; i < 4; i++)
                //{
                //    var op = new AnalogInDifferentialSourceOption(
                //      new BoardPin(i * 2, ((char)(i + 'A')).ToString() + "+"),
                //      new BoardPin(i * 2 + 1, ((char)(i + 'A')).ToString() + "-")
                //      );
                //    op.Checked = index == i;
                //    op.MenuItem.Click += SourceChange_Click;
                //    op.Parent = sourceOp;
                //    sourceOp.SubOptions.Add(op);
                //}
                for (int i = 0; i < 3; i++)
                {
                    var op = new RangeSelectionOption(MaxInputVoltage / SupportedGains[i], true);
                    op.Checked = i == 0;
                    op.MenuItem.Click += RangeChange_Click;
                    op.Parent = differentialGainOps[index];
                    differentialGainOps[index].SubOptions.Add(op);
                }
                
                // RSE gain (only one option)
                var rsegOp = new RangeSelectionOption(MaxInputVoltage / SupportedGains[0], false);
                rsegOp.Checked = true;
                RSEGainOps.Add(new GroupHeadOption("Input Range"));
                rsegOp.Parent = RSEGainOps[index];
                RSEGainOps[index].SubOptions.Add(rsegOp);
                var allInstruments = Instrument.GetInstruments();
                AvailableInstruments.Add(allInstruments.Items.ToArray());
                //instrumentTypeOps.Add(new GroupHeadOption("Instrument Type"));
                foreach (var iIns in allInstruments.Items)
                {
                    var iOp = iIns.TitleItem;
                    iOp.MenuItem.Click += InstrumentTypeChange_Click;
                    iOp.Parent = typeOp[index];
                    typeOp[index].SubOptions.Add(iOp);
                }
                // at least, add null.
                SelectedInstruments.Add(null);

                ops.SubOptions.Add(typeOp[index]);
                //ops.SubOptions.Add(sourceOp);
                ops.SubOptions.Add(differentialGainOps[index]);
                ChannelOptions.Add(ops);
                //instrumentTypeOps[index].Parent = ChannelOptions[index];
                
            }
            return ChannelOptions;
        }

        protected virtual void InstrumentTypeChange_Click(object sender, EventArgs e)
        {
            var op = ((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption;
            var selectedInstrument = ((InstrumentTypeOption)((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption).Instrument;
            var cID = op.TopParent.ID;
            ChannelOptions[cID].SubOptions.Remove(differentialGainOps[cID]);
            ChannelOptions[cID].SubOptions.Remove(RSEGainOps[cID]);
            if (SelectedInstruments[cID] != null)
            {
                foreach (var item in SelectedInstruments[cID].MenuItem.SubOptions)
                    ChannelOptions[cID].SubOptions.Remove(item);
                foreach (var item in SelectedInstruments[cID].MenuItem.Actions)
                    ChannelOptions[cID].SubOptions.Remove(item);
            }
            AttatchInstrument(cID, AvailableInstruments[cID].ToList().IndexOf(selectedInstrument));
        }
        void RangeChange_Click(object sender, EventArgs e)
        {
            var op = ((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption;
            var selectedOp = ((RangeSelectionOption)((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption);
            var selectedRange = selectedOp.Range;
            var selectedGain = (int)Math.Round(MaxInputVoltage / selectedRange);
            var cID = op.TopParent.ID;
            foreach (var dOp in differentialGainOps[cID].SubOptions)
                dOp.Checked = selectedOp == dOp;
            RangeChangeCommandSend(cID, (byte)SupportedGains.ToList().IndexOf(selectedGain));
        }
        public override void ChangeSamplingRate(float frequencyInHz)
        {
            if (SupportedFrequencies.Contains(frequencyInHz))
                SendFreqChangeCommand(frequencyInHz);
        }
        public virtual void SendFreqChangeCommand(float frequencyInHz)
        {
            var com = new PacketCommandMini(PhysLoggerPacketCommandID.ChangeSamplingFrequency,
             BitConverter.GetBytes((UInt32)(Math.Round(1.0F / frequencyInHz * 1000.0F)))
         );
            OnCommandSendRequest?.Invoke(com);
        }

        //private void SourceChange_Click(object sender, EventArgs e)
        //{
        //    var op = ((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption;
        //}

        void TypeChange_Click(object sender, EventArgs e)
        {
            var op = ((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption;
            var selectedType = ((ChannelTypeOption)((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption).Type;
            int typeInd = selectedType == ChannelType.AnalogInDifferential ? 0 : 1;
            var cID = op.TopParent.ID;
            ChannelOptions[cID].SubOptions.Remove(differentialGainOps[cID]);
            ChannelOptions[cID].SubOptions.Remove(RSEGainOps[cID]);            
            //ChannelOptions[cID].SubOptions.Remove(instrumentTypeOps[cID]);
            for (int i = 0; i < typeOp[cID].SubOptions.Count; i++)
                typeOp[cID].SubOptions[i].Checked = typeOp[cID].SubOptions[i] == op;
            if (SelectedInstruments[cID] != null) // an instrument is attached to this channel
            {
                foreach (var item in SelectedInstruments[cID].MenuItem.SubOptions)
                    ChannelOptions[cID].SubOptions.Remove(item);
            }
            if (selectedType == ChannelType.AnalogInRSE)
            {
                ChannelOptions[cID].SubOptions.Add(RSEGainOps[cID]);
            }
            else if (selectedType == ChannelType.AnalogInDifferential)
            {
                ChannelOptions[cID].SubOptions.Add(differentialGainOps[cID]);
            }
            TypeChangeCommandSend(cID, selectedType);
            SelectedInstruments[cID] = null; // confirm that no instrument is attached
        }
        void AttatchInstrument(int cID, int listID)
        {
            SelectedInstruments[cID] = AvailableInstruments[cID][listID];
            foreach (var item in SelectedInstruments[cID].MenuItem.SubOptions)
                ChannelOptions[cID].SubOptions.Add(item);
            foreach (var item in SelectedInstruments[cID].MenuItem.Actions)
                ChannelOptions[cID].SubOptions.Add(item);
            foreach (ChannelOption itOp in typeOp[cID].SubOptions)
            {
                if (!(itOp is InstrumentTypeOption))
                    itOp.Checked = false;
                else
                    itOp.Checked = ((InstrumentTypeOption)itOp).Instrument == SelectedInstruments[cID];
            }
            //ChannelOptions[cID].SubOptions.Add(SelectedInstruments[cID].MenuItem);
            int nearestGainInd = -1;
            float smallestGainError = float.PositiveInfinity;
            for (int i = 0; i < SupportedGains.Length; i++)
            {
                if (Math.Abs(SupportedGains[i] - SelectedInstruments[cID].RequiredGain) < smallestGainError)
                {
                    smallestGainError = Math.Abs(SupportedGains[i] - SelectedInstruments[cID].RequiredGain);
                    nearestGainInd = i;
                }
            }
            SelectedInstruments[cID].ActualGain = SupportedGains[nearestGainInd];
            TypeChangeCommandSend(cID, SelectedInstruments[cID].ChannelType);
            RangeChangeCommandSend(cID, nearestGainInd);
        }
        protected virtual void TypeChangeCommandSend(int cID, ChannelType selectedType)
        {
            var com = new PacketCommandMini(PhysLoggerPacketCommandID.ChangeChannelType, new byte[] {
                (byte)cID,
                (byte)(selectedType == ChannelType.AnalogInDifferential ? 0 : 1)
            });
            OnCommandSendRequest?.Invoke(com);
        }
        protected virtual void RangeChangeCommandSend(int cID, int nearestGainInd)
        {
            var gainCom = new PacketCommandMini(PhysLoggerPacketCommandID.ChangeChannelRange, new byte[] {
                (byte)cID,
                (byte)nearestGainInd
            });
            OnCommandSendRequest?.Invoke(gainCom);
        }
    }           
    public enum PhysLoggerHWSignature
    {
        Unknown,
        PhysLogger1_0,
        PhysLogger1_0_Virtual
    }
}

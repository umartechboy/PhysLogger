using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.IO;
using PhysLogger.LogControls;
using System.Diagnostics;
using System.IO;

namespace PhysLogger.Hardware
{
    public class PhysLogger1_0HW : LoggerHardware
    {
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
        public int[] SupportedGains = new int[] { 1, 10, 200};
        protected virtual float MaxInputVoltage { get; set; } = 6;
        protected float Vref = 2.56F;
        protected virtual float InputVoltageDivider { get; set; } = 3F; 
        public override void HandleDataPacket(PacketCommandMini command, ref float t, ref float[] values, ref PlotLabel[] labels)
        {
            if (command.PayLoadLength < 9)
                return;
            t = BitConverter.ToUInt32(new byte[] {
            command.PayLoad[0],
            command.PayLoad[1],
            command.PayLoad[2],
            (byte)0 }, 0) / 1000.0F;
            byte gains = command.PayLoad[8];
            int gainInd0 = (gains >> 0) & 3;
            int gainInd1 = (gains >> 2) & 3;
            int gainInd2 = (gains >> 4) & 3;
            int gainInd3 = (gains >> 6) & 3;
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
            values = new float[] {
                    analogToOutValue(RawValueToVoltage(a0, gainInd0, 0), 0),
                    analogToOutValue(RawValueToVoltage(a1, gainInd1, 1), 1),
                    analogToOutValue(RawValueToVoltage(a2, gainInd2, 2), 2),
                    analogToOutValue(RawValueToVoltage(a3, gainInd3, 3), 3),
                    };
            if (float.IsInfinity(values[0]))
                ;
            //values = new float[] { a0, a1, a2, a3 };
            labels = new PlotLabel[]
                {
                    SelectedInstruments[0] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[0].UnitConversions.Current.Label,
                    SelectedInstruments[1] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[1].UnitConversions.Current.Label,
                    SelectedInstruments[2] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[2].UnitConversions.Current.Label,
                    SelectedInstruments[3] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[3].UnitConversions.Current.Label,
                };
        }
        protected virtual float RawValueToVoltage(float raw, int gainInd, int cID)
        {
            if (gainInd == 3) // 0-1023 encoding
                return (raw / 1023.0F) * Vref * InputVoltageDivider;
            else
                return (raw / 512.0F) * Vref * InputVoltageDivider / SupportedGains[gainInd];
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
        protected List<List<Instrument>> AvailableInstruments = new List<List<Instrument>>();

        public override List<ChannelOption> EnumerateChannelOptions()
        {
            ChannelOptions = new List<ChannelOption>();
            ChannelDataSourceIndices = new int[4];
            for (int index = 0; index < 4; index++)
            {
                ChannelDataSourceIndices[index] = index;
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
                AvailableInstruments.Add(allInstruments.Items);
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

        public override void AddI2CInstrument(int i2cAddress, int instrumentID)
        {
            // do nothing
        }

        protected virtual void TypeChange_Click(object sender, EventArgs e)
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
        protected virtual void RangeChange_Click(object sender, EventArgs e)
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
        public override void ChangeSamplingRate(float frequencyInHz)
        {
            if (SupportedFrequencies.Contains(frequencyInHz))
                SendFreqChangeCommand(frequencyInHz);
        }
        public virtual void SendFreqChangeCommand(float frequencyInHz)
        {
            var com = new PacketCommandMini(PhysLoggerPacketCommandID.ChangeSamplingTime,
             BitConverter.GetBytes((UInt32)(Math.Round(1.0F / frequencyInHz * 1000.0F)))
         );
            CommandSendRequest(com);
        }
        
        protected virtual void AttatchInstrument(int cID, int listID)
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
            if (SelectedInstruments[cID].ChannelType == ChannelType.AnalogInDifferential || SelectedInstruments[cID].ChannelType == ChannelType.AnalogInRSE)
            {
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
        }
        protected virtual void TypeChangeCommandSend(int cID, ChannelType selectedType)
        {
            var com = new PacketCommandMini(PhysLoggerPacketCommandID.ChangeChannelType, new byte[] {
                (byte)cID,
                (byte)(selectedType == ChannelType.AnalogInDifferential ? 0 : 1)
            });
            CommandSendRequest(com);
        }
        protected virtual void RangeChangeCommandSend(int cID, int nearestGainInd)
        {
            var gainCom = new PacketCommandMini(PhysLoggerPacketCommandID.ChangeChannelRange, new byte[] {
                (byte)cID,
                (byte)nearestGainInd
            });
            CommandSendRequest(gainCom);
        }
        public override void UpdateFirmware(PhysLogger.Forms.PhysLoggerUpdaterConsole ucf, string comPort, string forcedSource)
        {
            if (forcedSource != null)
            {
                if (forcedSource != "")
                {
                    base.UpdateFirmware(ucf, comPort, forcedSource);
                    return;
                }
            }
            RequestDisconnect();
            ucf.WriteLine("PhysLogger Update Console");
            string fNameSeed = "";
            string verString = "";
            double maxVer = -1;

            foreach (var file in System.IO.Directory.GetFiles("firmware", "PhysLoggerV1*.hex"))
            {
                try
                {
                    string verS = System.IO.Path.GetFileNameWithoutExtension(file);
                    verS = verS.Substring(11);
                    double ver = double.Parse(verS);
                    if (ver > maxVer)
                        maxVer = ver;
                }
                catch { }
            }
            if (maxVer >= 0)
            {
                fNameSeed = "firmware\\PhysLoggerV" + maxVer;
                verString = maxVer.ToString();
            }
            if (fNameSeed != "")
            {
                if (System.IO.File.Exists(fNameSeed + ".hex"))
                {
                    ucf.WriteLine();
                    ucf.Write("You are about to update the PhysLogger firmare from v"+Signature+" to v");
                    ucf.Write(verString);
                    ucf.WriteLine(". This will update the firmare on the PhysLogger hardware which might raise conflicts if not done properly.");
                    ucf.WriteLine("If you are not sure what you are doing, you might want to quit it now.\r\n");
                    if (System.IO.File.Exists(fNameSeed + "_preInstall.txt"))
                    {
                        ucf.WriteLine();
                        ucf.Write(System.IO.File.ReadAllText(fNameSeed + "_preInstall.txt"));
                        ucf.WriteLine();
                    }
                    ucf.Write("\r\nDo you still want to continue with the update (Y/n)? ");
                    if (ucf.ReadLine() != "Y")
                    {
                        return;
                    }
                    string root = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\avrdude";


                    ArduinoSketchUploader.UploaderMain uploader = new ArduinoSketchUploader.UploaderMain();
                    uploader.OnProgressUpdate += delegate (string a)
                    {
                        ucf.Write(a);
                    }
                    ;
                    for (int i = 1; i <= 3; i++)
                    {
                        if (uploader.Upload(fNameSeed + ".hex", ArduinoUploader.Hardware.ArduinoModel.Mega2560, comPort))
                            break;
                        ucf.WriteLine("Upload failed. Retrying (" + i + "/3)...");
                        System.Threading.Thread.Sleep(3000);
                    }
                    return;
                    //p.OutputDataReceived += (s, ea) => ucf.Write(ea.Data);
                }
            }
        }

        private void P_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
        }
        public override void SignatureReceived(PhysLoggerHWSignature newSignature)
        {

        }
    }
}

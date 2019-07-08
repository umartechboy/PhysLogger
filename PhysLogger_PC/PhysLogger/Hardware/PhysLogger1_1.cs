using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.IO;
using PhysLogger.LogControls;
using PhysLogger.Maths;

namespace PhysLogger.Hardware
{
    
    public class PhysLogger1_1HW : PhysLogger1_0HW
    {
        protected override float InputVoltageDivider { get; set; } = 4.030303F; // 10k  &&  (3.3k || 10k internal resistance)
        public PhysLogger1_1HW()
        {
            Signature = PhysLoggerHWSignature.PhysLogger1_1;
            foreach (var instrument in AvailableInstruments)
            {
            }
        }
        public override void AddI2CInstrument(int instrumentID, int i2cAddress)
        {
            for (int index = 0; index < 4; index++)
            {
                var iIns = Instrument.FromI2C(i2cAddress, instrumentID);
                iIns.OnRangeChanged += I2CInstruments_OnRangeChanged;
                iIns.OnCommandRequest += IIns_OnCommandRequest;
                AvailableInstruments[index].Add(iIns);
                var iOp = iIns.TitleItem;
                iOp.MenuItem.Click += InstrumentTypeChange_Click;
                iOp.Parent = typeOp[index];
                typeOp[index].SubOptions.Add(iOp);
            }
        }

        private void IIns_OnCommandRequest(I2CInstrument instrument, InstrumentCommand command)
        {
            // Send the range change command
            // Get = Set in reverse

            //#define GetFloatTBC 0
            //#define GetFloat 1
            //#define GetKeyChar 2
            //#define ChangeI2CInstrumentRange 3
            //#define SetI2CInstrumentParameter 4
            byte[] bytes = new byte[4 + command.DataBytes.Length];
            bytes[0] = 4; // SetI2CInstrumentParameter
            bytes[1] = (byte)instrument.InstrumentAddress; // instrument address
            bytes[2] = command.ID; // param id
            bytes[3] = 1; // dataType from instrument types
            Buffer.BlockCopy(command.DataBytes, 0, bytes, 4, command.DataBytes.Length);
            PacketCommandMini rChangeCom = new PacketCommandMini(PhysLoggerPacketCommandID.GetValue, bytes);
            CommandSendRequest(rChangeCom);
        }

        private void I2CInstruments_OnRangeChanged(I2CInstrument instrument, InstrumentRange range)
        {
            // Send the range change command
            // Get = Set in reverse

            //#define GetFloatTBC 0
            //#define GetFloat 1
            //#define GetKeyChar 2
            //#define ChangeI2CInstrumentRange 3
            //#define SetI2CInstrumentParameter 4
            PacketCommandMini rChangeCom = new PacketCommandMini(
                PhysLoggerPacketCommandID.GetValue,
                new byte[] {
                    3,
                    (byte)instrument.InstrumentAddress,
                    range.Code }
                );
            CommandSendRequest(rChangeCom);            
        }

        protected override void AttatchInstrument(int cID, int listID)
        {
            SelectedInstruments[cID] = AvailableInstruments[cID][listID];
            base.AttatchInstrument(cID, listID);
            if (SelectedInstruments[cID] is I2CInstrument)
            {
                TypeChangeCommandSend(cID, ((I2CInstrument)SelectedInstruments[cID]).InstrumentAddress);
                SelectedInstruments[cID].ActualGain = SelectedInstruments[cID].RequiredGain;
            }
        }
        public override void HandleMiscCommand(PacketCommandMini command)
        {
            if (command.PacketID == PhysLoggerPacketCommandID.SetValue)
            {
                if (command.PayLoad[0] == 0) // calibration data
                {
                    int channel = command.PayLoad[1];
                    int gain = command.PayLoad[2];
                    float inl = BitConverter.ToSingle(command.PayLoad, 3);
                    float dnl = BitConverter.ToSingle(command.PayLoad, 7);
                    INLCorrectionTable[channel, gain] = inl;
                    DNLCorrectionTable[channel, gain] = dnl;
                }
                else
                { }
            }
            else if (command.PacketID == PhysLoggerPacketCommandID.GetValue)
            {
                if (command.PayLoad[0] == 3) // Range Change Feedback
                {
                    var relatedIns = (SelectedInstruments.FindAll(ins => ins is I2CInstrument)
                        .FindAll(iIns => ((I2CInstrument)iIns).InstrumentAddress == command.PayLoad[1])).Cast<I2CInstrument>().ToList();
                    foreach (var relIns in relatedIns)
                    {
                        var actualRange = relIns.Ranges.Items.Find(r => r.Code == command.PayLoad[2]);
                        relIns.Ranges.Current = actualRange;
                    }
                }
            }
        }
        float[,] INLCorrectionTable = new float[8, 3];
        float[,] DNLCorrectionTable = new float[8, 3];
        protected override float RawValueToVoltage(float raw, int gainInd, int cID)
        {
            // the value is -512to511 encoded for Diff Channels and 1023 for RSE.
            // for i2c, its -512to511
            if (SelectedInstruments[cID] == null) // normal channels
                if (gainInd == 3) // 0-1023 encoding
                    return (raw / 1023.0F) * Vref * InputVoltageDivider * DNLCorrectionTable[cID + 4, 0] - INLCorrectionTable[cID + 4, 0];
                else
                    return (raw / 512.0F) * Vref * InputVoltageDivider / SupportedGains[gainInd] * DNLCorrectionTable[cID, gainInd] - INLCorrectionTable[cID, gainInd];
            else // i2c instruments TF musts be designed to work with the raw values.
            {
                if (SelectedInstruments[cID] is I2CInstrument)
                    return raw; // Raw is in -512to511 format already. don't change it
                else // simple instrument
                {
                    // same as above
                    if (gainInd == 3) // 0-1023 encoding
                        return (raw / 1023.0F) * Vref * InputVoltageDivider * DNLCorrectionTable[cID + 4, 0] - INLCorrectionTable[cID + 4, 0];
                    else
                        return (raw / 512.0F) * Vref * InputVoltageDivider / SupportedGains[gainInd] * DNLCorrectionTable[cID, gainInd] - INLCorrectionTable[cID, gainInd];
                }
            }
        }
        protected void TypeChangeCommandSend(int cID, int i2cAddress)
        {
            var com = new PacketCommandMini(PhysLoggerPacketCommandID.ChangeChannelType, new byte[] {
                (byte)cID,
                (byte)(128 + i2cAddress)
            });
            CommandSendRequest(com);
        }
        public override void SignatureReceived(PhysLoggerHWSignature newSignature)
        {
            // just wait for another 5 seconds and the controller will resume the last state
        }
    }
}

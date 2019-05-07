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
    public class PhysLogger1_1HW : PhysLogger1_0HW
    {
        protected override float InputVoltageDivider { get; set; } = 4.030303F; // 10k  &&  (3.3k || 10k internal resistance)
        public PhysLogger1_1HW()
        {
            Signature = PhysLoggerHWSignature.PhysLogger1_1;
        }
        public override void AddI2CInstrument(int i2cAddress, int instrumentID)
        {
            for (int index = 0; index < 4; index++)
            {
                Instrument iIns = Instrument.FromI2C(i2cAddress, instrumentID);
                AvailableInstruments[index].Add(iIns);
                var iOp = iIns.TitleItem;
                iOp.MenuItem.Click += InstrumentTypeChange_Click;
                iOp.Parent = typeOp[index];
                typeOp[index].SubOptions.Add(iOp);
            }
        }
        protected override void AttatchInstrument(int cID, int listID)
        {
            SelectedInstruments[cID] = AvailableInstruments[cID][listID];
            base.AttatchInstrument(cID, listID);
            if (SelectedInstruments[cID].ChannelType == ChannelType.I2C)
            {
                TypeChangeCommandSend(cID, SelectedInstruments[cID].InstrumentAddress);
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
                if (SelectedInstruments[cID].ChannelType == ChannelType.I2C)
                    return raw; // Raw is in -512to511 format already. don't change it
                else
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

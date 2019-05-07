using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FivePointNine.Windows.IO;
using PhysLogger.LogControls;

namespace PhysLogger.Hardware
{
    public class PhysLogger1_0Virtual:PhysLogger1_0HW
    {
        public override event SetPointHandler NewPointReceived;
        System.Windows.Forms.Timer virtualController;
        public PhysLogger1_0Virtual()
        {
            virtualController = new System.Windows.Forms.Timer();
            virtualController.Interval = 1;
            virtualController.Tick += VirtualController_Tick;
            virtualController.Enabled = true;
            Signature = PhysLoggerHWSignature.PhysLogger1_0_Virtual;
        }

        bool first = true;

        private void VirtualController_Tick(object sender, EventArgs e)
        {
            if (first)
            {
                SignatureUpdates(PhysLoggerHWSignature.PhysLogger1_0_Virtual, PhysLoggerHWSignature.Unknown);
                SamplingRateChanged(1 / (float)virtualController.Interval * 1000.0F);
                first = false;
                return;
            }
            var values = new float[] {
                    analogToOutValue(Math.Max((float)Math.Sin(2 * Math.PI * 10 * t) * 1F, types[0] == ChannelType.AnalogInRSE?0:-1F), 0),
                    analogToOutValue(Math.Max((float)Math.Sin(2 * Math.PI * 10 * t) * 0.6F + 0.6F, types[1] == ChannelType.AnalogInRSE?0:0F), 1),
                    analogToOutValue(Math.Max((float)Math.Cos(2 * Math.PI * 10 * t) * 1F, types[2] == ChannelType.AnalogInRSE ? 0:-0.5F), 2),
                    analogToOutValue(Math.Max((float)Math.Cos(2 * Math.PI * 10 * t) * 0.6F + 0.6F, types[3] == ChannelType.AnalogInRSE ? 0:-0.5F), 3),
                    };
            //values = new float[] { a0, a1, a2, a3 };
            var labels = new PlotLabel[]
                {
                    SelectedInstruments[0] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[0].UnitConversions.Current.Label,
                    SelectedInstruments[1] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[1].UnitConversions.Current.Label,
                    SelectedInstruments[2] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[2].UnitConversions.Current.Label,
                    SelectedInstruments[3] == null?new PlotLabel("Voltage", "V"):SelectedInstruments[3].UnitConversions.Current.Label,
                };
            t += (float)virtualController.Interval / 1000.0F;
            NewPointReceived(t, values, labels);
        }

        float t = 0;

        ChannelType[] types = new ChannelType[] { 0, 0, 0, 0 };
        int[] gains = new int[] { 0, 0, 0, 0 };
        protected override void RangeChangeCommandSend(int cID, int selectedGain)
        {
            gains[cID] = (byte)SupportedGains.ToList().IndexOf(selectedGain);
        }
        public override void SendFreqChangeCommand(float frequencyInHz)
        {
            virtualController.Interval = (int)(Math.Round(1 / frequencyInHz * 1000));
        }

        protected override void TypeChangeCommandSend(int cID, ChannelType selectedType)
        {
            types[cID] = selectedType;
        }

        internal void Stop()
        {
            virtualController.Enabled = false;
        }
    }
}

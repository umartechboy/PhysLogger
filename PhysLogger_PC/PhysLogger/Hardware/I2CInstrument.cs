using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PhysLogger.Maths;

namespace PhysLogger.Hardware
{
    public delegate void InstrumentCommandRequestHandler(I2CInstrument instrument, InstrumentCommand command);
    public class I2CInstrument : GenericInstrument
    {
        public event RangeChangeHandler OnRangeChanged;
        public event InstrumentCommandRequestHandler OnCommandRequest;
        public InstrumentRangeCollection Ranges { get; protected set; }
        public int InstrumentTypeIndex { get; protected set; } = -1;

        public int InstrumentAddress { get; set; } = -1;
        public I2CInstrument(
            string title,
            int instrumentTypeIndex,
            UnitConversionCollection units,
            InstrumentRangeCollection ranges,
            List<InstrumentCommand> commands,
            bool enableResetToZero,
            bool enableResetToOffset,
            string resetToOffsetString,
            string resetToOffsetDescription,
            CalibrationFunctionsCollection func,
            int preferredGain,
            ChannelType channelType
            ) : base(title, units, enableResetToZero, enableResetToOffset, resetToOffsetString, resetToOffsetDescription, func, preferredGain, channelType)
        {
            Ranges = ranges;
            if (Ranges.Count > 1)
            {
                MenuItem.SubOptions.Add(Ranges.MenuItem);
                Ranges.OnRangeChanged += Ranges_OnRangeChanged;
            }
            foreach (var com in commands)
                MenuItem.Actions.Add(new InstrumentCommandActionOption(com, ComAction));
            this.InstrumentTypeIndex = instrumentTypeIndex;
        }
        private bool ComAction(object parameters)
        {
            var com = ((InstrumentCommandActionOption)parameters).Command;
            OnCommandRequest?.Invoke(this, com);
            return true;
        }
        private void Ranges_OnRangeChanged(Instrument instrument, InstrumentRange range)
        {
            OnRangeChanged?.Invoke(this, range);
        }
        public override float TF(float voltage)
        {
            float x = voltage;
            if (resetOffset) // reset to default
            {
                outputOffset = 0;
                resetOffset = false;
            }
            else
            {
                if (forceNextValue)
                {
                    forceNextValue = false;
                    outputOffset = 0;
                    outputOffset = -TF(voltage) + valueToForce;
                }
            }
            float y = 0;
            y = calibTF.Selected.TF.Evaluate(x);
            y = Ranges.Current.TF.Evaluate(UnitConversions.Current.TF.Evaluate(y)) + outputOffset;
            return y;
        }
    }
    public class InstrumentCommand
    {
        public string Title { get; protected set; } = "";
        public byte ID { get; protected set; } = 0;
        public byte DataType { get; protected set; } = 0;
        public byte[] DataBytes {
            get {
                if (DataType == 1)
                    return new byte[] { (byte)Value };
                else //
                    // we need to implement value to byte conversions for all the instrument data types here.
                    throw new NotImplementedException(); } }
        public object Value { get; protected set; }
        public static InstrumentCommand Parse(string valueString)
        {
            var valuePairs = valueString.Split(new char[] { '&' });
            try
            {
                InstrumentCommand com = new Hardware.InstrumentCommand();
                foreach (var pair in valuePairs)
                {
                    var parts = pair.Split(new char[] { ':' }, 2);
                    parts[0] = parts[0].ToLower();
                    if (parts[0] == "title")
                        com.Title = parts[1];
                    else if (parts[0] == "code")
                        com.ID = (byte)int.Parse(parts[1]);
                    else if (parts[0] == "datatype")
                    {
                        // we need to implement value to byte conversions for all the instrument data types here.
                        if (parts[1] == "uint8")
                            com.DataType = 1;
                    }
                    else if (parts[0] == "value")
                    {
                        // we need to implement value to byte conversions for all the instrument data types here.
                        if (com.DataType == 1)
                            com.Value = byte.Parse(parts[1]);
                    }
                    // need to do parsing of value yet.
                }
                return com;
            }
            catch { return null; }
        }
    }
}

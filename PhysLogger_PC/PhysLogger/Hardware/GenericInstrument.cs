using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PhysLogger.Maths;

namespace PhysLogger.Hardware
{
    public class GenericInstrument : Instrument
    {
        protected float outputOffset = 0;
        protected string resetToOffsetDescription = "";
        protected bool forceNextValue = false, resetOffset = false;
        protected float valueToForce = 0;
        public GenericInstrument(
            string title,
            UnitConversionCollection units,
            bool enableResetToZero,
            bool enableResetToOffset,
            string resetToOffsetString,
            string resetToOffsetDescription,
            CalibrationFunctionsCollection func,
            int preferredGain,
            ChannelType channelType
            ) : base(title, units, func, preferredGain, channelType)
        {
            if (enableResetToZero)
                MenuItem.Actions.Add(new ActionOption("Reset to Zero", resetToZero));
            if (enableResetToOffset)
                MenuItem.Actions.Add(new ActionOption(resetToOffsetString, resetToOffset));
            if (enableResetToOffset || enableResetToZero)
                MenuItem.Actions.Add(new ActionOption("Reset to default offset", resetToDefaultOffset));


            this.resetToOffsetDescription = resetToOffsetDescription;
        }

        public override float TF(float voltage)
        {
            float x = voltage * ActualGain / RequiredGain;
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
            y = UnitConversions.Current.TF.Evaluate(y) + outputOffset;
            return y;
        }
        private bool resetToZero(object parameters)
        {
            forceNextValue = true;
            valueToForce = 0;
            return true;
        }
        private bool resetToOffset(object parameters)
        {
            var result = PhysLogger.Forms.AskFloat.ShowDialog(ParseStringVariables(resetToOffsetDescription), 25);
            if (result.dr == System.Windows.Forms.DialogResult.OK)
            {
                forceNextValue = true;
                valueToForce = result.Value;
            }
            return true;
        }
        private bool resetToDefaultOffset(object parameters)
        {
            resetOffset = true;
            return true;
        }
        public override string ToString()
        {
            return Title;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PhysLogger.Maths;

namespace PhysLogger.Hardware
{
    public class InstrumentCollection
    {
        public InstrumentCollection()
        {
            menuItem = new GroupHeadOption("Instrument Type");
        }
        public List<Instrument> Items { get; protected set;} = new List<Instrument>();
        public Instrument SelectedInstrument
        {
            get { return Items.Find(ii => ii.MenuItem.Checked); }
            set
            { foreach (var ins in Items) ins.MenuItem.Checked = ins == value; }
        }

        GroupHeadOption menuItem;
        public GroupHeadOption TitleItem
        {
            get
            {
                menuItem.SubOptions.Clear();
                foreach (var item in Items)
                    menuItem.SubOptions.Add(item.TitleItem);
                return menuItem;
            }
            protected set { menuItem = value; }
        }
    }
    public abstract class Instrument
    {
        protected Instrument(
            string title,
            UnitConversionCollection units,
            CalibrationFunctionsCollection func,
            int preferredGain,
            ChannelType channelType)
        {
            //Label = label;
            titleItem = new InstrumentTypeOption(this);
            menuItem = new InstrumentTypeOption(this);
            menuItem.SubOptions.Add(units.MenuItem);
            if (func.Count > 0)
                menuItem.SubOptions.Add(func.MenuItem);

            this.UnitConversions = units;
            this.Title = title;
            this.calibTF = func;
            this.RequiredGain = preferredGain;
            this.ChannelType = channelType;
        }

        InstrumentTypeOption menuItem;
        InstrumentTypeOption titleItem;
        // Must be set
        protected CalibrationFunctionsCollection calibTF = new CalibrationFunctionsCollection();
        public UnitConversionCollection UnitConversions { get; protected set; }
        public InstrumentTypeOption TitleItem
        {
            get { if (titleItem == null) titleItem = new InstrumentTypeOption(this); return titleItem; }
        }
        public InstrumentTypeOption MenuItem
        {
            get { if (menuItem == null) menuItem = new InstrumentTypeOption(this); return menuItem; }
            protected set { menuItem = value; }
        }
        public string Title { get; protected set; } = "";


        public virtual float TF(float voltage)
        {
            return UnitConversions.Current.TF.Evaluate(calibTF.Selected.TF.Evaluate(voltage * ActualGain / RequiredGain));
        }
        public int RequiredGain { get; protected set; }
        public int ActualGain { get; set; }
        public ChannelType ChannelType { get; protected set; }
        public static InstrumentCollection GetInstruments()
        {
            // Add to list any instrument that has its own class structure
            InstrumentCollection list = new InstrumentCollection();
            foreach (var insFile in Directory.GetFiles("Instruments", "*.plf"))
            {
                var ins = Instrument.FromFile(insFile);
                if (ins != null)
                    list.Items.Add(ins);
            }
            return list;
        }
        public static Instrument FromFile(string iFile)
        {
            try
            {
                string[] iLines = File.ReadAllLines(iFile);
                string title = Path.GetFileNameWithoutExtension(iFile); ChannelType icType = ChannelType.None;
                UnitConversionCollection units = null;
                CalibrationFunctionsCollection calibTFCollection = new CalibrationFunctionsCollection(); ;
                int iGain = 1;
                bool resetToZeroEnabled = false, resetInput = false;
                units = new UnitConversionCollection();
                foreach (var iLine in iLines)
                {
                    var parts = iLine.Split(new char[] { '=' });
                    parts[0] = parts[0].ToLower().Replace(" ", "");
                    if (parts[0] == "title")
                        title = parts[1];
                    else if (parts[0] == "gain")
                        iGain = Convert.ToInt32(parts[1]);
                    else if (parts[0] == "channeltype")
                        icType = (ChannelType)Convert.ToByte(parts[1]);
                    else if (parts[0] == "resettozero")
                        resetToZeroEnabled = parts[1].ToLower() == "true" || parts[1].ToLower() == "1" || parts[1].ToLower().StartsWith("enable") || parts[1].ToLower().StartsWith("yes");
                    else if (parts[0] == "resetinput")
                        resetInput = parts[1].ToLower() == "true" || parts[1].ToLower() == "1" || parts[1].ToLower().StartsWith("enable") || parts[1].ToLower().StartsWith("yes");
                    else if (parts[0] == "unit")
                    {
                        var u = UnitConversion.Parse(parts[1]);
                        if (u != null)
                            units.Add(u);
                    }
                }
                if (units.Count == 0)
                {
                    units.Add(new UnitConversion("untitled", "", new G1Function()));
                }

                var calibFiles = Directory.GetFiles("CalibrationData", title + "*.plf");

                foreach (var cFile in calibFiles)
                {
                    var cLines = File.ReadAllLines(cFile);
                    string cID = (units.Count + 1).ToString();
                    Function itf = null;
                    foreach (var cLine in cLines)
                    {
                        var parts = cLine.Split(new char[] { '=' }, 2);
                        parts[0] = parts[0].ToLower();
                        if (parts[0] == "instrumentid")
                            cID = parts[1];
                        else if (parts[0] == "tf")
                            itf = Function.Parse(parts[1]);
                    }
                    if (itf != null)
                        calibTFCollection.Add(itf, cID);
                }

                if (calibTFCollection.Count == 0)
                {
                    units.Items.Clear();
                    units.Add(new UnitConversion("Voltage", "", new G1Function()));
                    //calibTFCollection.Add(new G1Function(), "");
                }

                if (units.Count == 0 || icType == ChannelType.None)
                    return null;
                else
                    return new GenericInstrument(title, units, resetToZeroEnabled, resetInput, calibTFCollection, iGain, icType);
            }
            catch (Exception ex) { return null; }
        }

    }
    public class GenericInstrument : Instrument
    {
        protected float inLastOffset = 0, inZeroPoint = 0, outZeroPoint = 0, outLastOffset;
        bool resetInput = false;
        public GenericInstrument(
            string title,
            UnitConversionCollection units,
            bool enableResetToZero, 
            bool resetInput, 
            CalibrationFunctionsCollection func,
            int preferredGain,
            ChannelType channelType
            ):base(title, units, func, preferredGain, channelType)
        {
            this.resetInput = resetInput;
            if (enableResetToZero)
                MenuItem.Actions.Add(new ActionOption("Reset to Zero", resetToZero));
        }

        public override float TF(float voltage)
        {
            float x = voltage * ActualGain / RequiredGain ;
            if (forceNextZero)
            {
                forceNextZero = false;
                outZeroPoint = calibTF.Selected.TF.Evaluate(x - inZeroPoint);
            }
            float y = 0;
            y = calibTF.Selected.TF.Evaluate(x - inZeroPoint);


            inLastOffset = x;
            outLastOffset = y;

            y = UnitConversions.Current.TF.Evaluate(y - outZeroPoint);
            return y;
        }
        bool forceNextZero = false;
        private bool resetToZero(object parameters)
        {
            inZeroPoint = inLastOffset;
            forceNextZero = true;
            return true;
        }
        public override string ToString()
        {
            return Title;
        }
    }
}

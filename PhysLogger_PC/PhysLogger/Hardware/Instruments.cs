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
            CalibrationFunctionsCollection calibFunc,
            int preferredGain,
            ChannelType channelType)
        {
            //Label = label;
            menuItem = new InstrumentTypeOption(this);
            menuItem.SubOptions.Add(units.MenuItem);
            if (calibFunc.Count > 0)
                menuItem.SubOptions.Add(calibFunc.MenuItem);

            this.UnitConversions = units;
            this.Title = title;
            if (calibFunc.Count == 0)
                calibFunc.Add(new G1Function(), "");
            this.calibTF = calibFunc;
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
                {
                    if (ins.ChannelType == ChannelType.I2C)
                        continue;
                    list.Items.Add(ins);
                }
            }
            return list;
        }

        public static I2CInstrument FromI2C(int i2cAddress, int instrumentID)
        {
            InstrumentCollection list = new InstrumentCollection();
            foreach (var insFile in Directory.GetFiles("Instruments", "*.plf"))
            {
                var ins_ = FromFile(insFile);
                if (ins_ != null)
                {
                    I2CInstrument ins = null;
                    if (ins_ is I2CInstrument)
                    {
                        ins = (I2CInstrument)ins_;
                        if (ins.InstrumentTypeIndex == instrumentID)
                        {
                            ins.InstrumentAddress = i2cAddress;
                            return ins;
                        }
                    }
                }
            }
            return null;
        }
        
        public static Instrument FromFile(string iFile)
        {
            try
            {
                string[] iLines = File.ReadAllLines(iFile);
                string title = Path.GetFileNameWithoutExtension(iFile);
                string resetToOffsetString = "Reset to Offset", resetToOffsetDescription = "Enter a value";
                
                ChannelType icType = ChannelType.None;
                int i2cInsType = -1;
                bool iHasCalib = true;
                UnitConversionCollection units = null;
                InstrumentRangeCollection ranges = null;
                List<InstrumentCommand> iCommands = new List<InstrumentCommand>();
                CalibrationFunctionsCollection calibTFCollection = new CalibrationFunctionsCollection(); ;
                int iGain = 1;
                bool resetToZeroEnabled = false, resetToOffsetEnabled = false;
                units = new UnitConversionCollection();
                ranges = new InstrumentRangeCollection();
                foreach (var iLine in iLines)
                {
                    var parts = iLine.Split(new char[] { '=' }, 2);
                    for (int i = 0; i < parts.Length; i++)
                        parts[i] = parts[i].Trim();
                    if (parts.Length == 1)
                    {
                        if (parts[0].StartsWith("//") || parts[0].StartsWith("%")) // its a comment
                            continue;
                    }
                    
                    parts[0] = parts[0].ToLower().Replace(" ", "");
                    if (parts[0] == "title")
                        title = parts[1];
                    else if (parts[0] == "gain")
                        iGain = Convert.ToInt32(parts[1]);
                    else if (parts[0] == "channeltype")
                        icType = (ChannelType)Convert.ToByte(parts[1]);
                    else if (parts[0] == "instrumentid")
                        i2cInsType = Convert.ToInt16(parts[1]);
                    else if (parts[0] == "hascalibration")
                        iHasCalib = parts[1].ToLower() == "true" || parts[1].ToLower() == "1" || parts[1].ToLower().StartsWith("enable") || parts[1].ToLower().StartsWith("yes");
                    else if (parts[0] == "resettozero")
                        resetToZeroEnabled = parts[1].ToLower() == "true" || parts[1].ToLower() == "1" || parts[1].ToLower().StartsWith("enable") || parts[1].ToLower().StartsWith("yes");
                    else if (parts[0] == "resettooffset")
                        resetToOffsetEnabled = parts[1].ToLower() == "true" || parts[1].ToLower() == "1" || parts[1].ToLower().StartsWith("enable") || parts[1].ToLower().StartsWith("yes");
                    else if (parts[0] == "resettooffsetstring")
                        resetToOffsetString = parts[1];
                    else if (parts[0] == "resettooffsetdescription")
                        resetToOffsetDescription = parts[1];
                    else if (parts[0] == "command") // only in i2c Instruments
                    {
                        var com = InstrumentCommand.Parse(parts[1]);
                        if (com != null)
                            iCommands.Add(com);
                    }
                    else if (parts[0] == "unit")
                    {
                        var u = UnitConversion.Parse(parts[1]);
                        if (u != null)
                            units.Add(u);
                    }
                    else if (parts[0] == "range")
                    {
                        var r = InstrumentRange.Parse(parts[1]);
                        if (r != null)
                            ranges.Add(r);
                    }
                    else
                    { }
                }
                if (units.Count == 0)
                {
                    units.Add(new UnitConversion("untitled", "", new G1Function()));
                }

                if (ranges.Count == 0)
                {
                    ranges.Add(new InstrumentRange("untitled", new G1Function(), 0));
                }

                var calibFiles = Directory.GetFiles("CalibrationData", title + "*.plf");
                if (iHasCalib)
                {
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
                }

                if (units.Count == 0 || icType == ChannelType.None)
                    return null;
                else
                {
                    if (i2cInsType < 0)
                    {
                        return new GenericInstrument(title, units, resetToZeroEnabled,
                          resetToOffsetEnabled,
                          resetToOffsetString,
                          resetToOffsetDescription,
                          calibTFCollection, iGain, icType);
                    }
                    else
                    {
                        return new I2CInstrument(
                            title, i2cInsType, units, ranges, iCommands, resetToZeroEnabled,
                             resetToOffsetEnabled,
                             resetToOffsetString,
                             resetToOffsetDescription,
                             calibTFCollection, iGain, icType);
                    }
                }
            }
            catch (Exception ex)
            { return null; }
        }
        public static string[] GetStringParsableVariables(string text)
        {
            List<string> keys = new List<string>();
            bool awaitingEnd = false;
            string key = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '%')
                {
                    if (awaitingEnd)
                        key += text[i];
                    continue;
                }
                if (!awaitingEnd)
                {
                    awaitingEnd = true;
                    key = "";
                }
                else
                {
                    keys.Add(key);
                    awaitingEnd = false;
                }
            }
            return keys.ToArray();
        }
        public virtual string ParseStringVariables(string text)
        {
            var parsables = GetStringParsableVariables(text);
            foreach (var parsable in parsables)
            {
                if (parsable == "SelectedUnit")
                    text = text.Replace("%" + parsable + "%", UnitConversions.Current.Unit);
            }
            return text;
        }
    }
}

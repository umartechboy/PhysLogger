using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PhysLogger.Maths;

namespace PhysLogger.Hardware
{
    public class ChannelOptionsCollection
    {
        /// <summary>
        /// An option can either be checkable or have subitems
        /// Self explnatory subitems
        /// </summary>
        public ChannelOption SoftwareOptions { get; set; }
        public ChannelOption HardwareOptions { get; set; }
        ContextMenuStrip strip = new ContextMenuStrip();
        public int ID { get; set; }
        public ChannelOptionsCollection(int id)
        {
            this.ID = id;
            SoftwareOptions = new Hardware.GroupHeadOption("Display Options");
            SoftwareOptions.TopParent = this;
            HardwareOptions = new Hardware.GroupHeadOption("Channel Configuration");
            HardwareOptions.TopParent = this;
        }
        public ContextMenuStrip MenuStrip
        {
            get
            {
                strip.Items.Clear();
                strip.Items.Add(SoftwareOptions.MenuItem);
                strip.Items.Add(HardwareOptions.MenuItem);
                return strip;
            }
        }
    }
    /// <summary>
    /// Base class to define the channel options. Options can be: a Visual setting, source selector, gain selector, ClickActionOnly etc
    /// </summary>
    public class ChannelOption
    {
        protected ChannelOption()
        { }

        /// <summary>
        /// Every Option must have a text. This gives the channel the ability to dynamically change menu text.
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// An option can either be checkable or have subitems
        /// Self explnatory subitems
        /// </summary>
        public List<ChannelOption> SubOptions { get; protected set; } = new List<ChannelOption>();
        /// <summary>
        /// This list contains all actions that don't have  subitems or checked state. 
        /// They are one time fired events that may or may not do anything
        /// </summary>
        public List<ActionOption> Actions { get; protected set; } = new List<ActionOption>();
        /// <summary>
        /// Gets the checked state of this item.
        /// </summary>
        public bool Checked { get; set; }
        /// <summary>
        /// When disabled, the option will appear diabled with all the sub options hidden
        /// </summary>
        public bool Enabled { get; internal set; } = true;
        /// <summary>
        /// Leave the possibility of a configuration object: like, a file, dataobject, calibration variable etc.
        /// </summary>
        protected ChannelConfigurationObject ConfigurationObject { get; set; } = null;
        ToolStripMenuItemWithChannelOption menuItem = new ToolStripMenuItemWithChannelOption();
        /// <summary>
        /// Gets the associated menuitem
        /// </summary>
        public ToolStripMenuItemWithChannelOption MenuItem
        {
            get
            {
                menuItem.AssociatedChannelOption = this;
                menuItem.DropDownItems.Clear();
                menuItem.Text = Title;
                if (!Enabled)
                {
                    menuItem.Enabled = false;
                    return menuItem;
                }
                menuItem.Checked = SubOptions.Count == 0 ? Checked : false;

                foreach (var option in SubOptions)
                    menuItem.DropDownItems.Add(option.MenuItem);
                foreach (var action in Actions)
                    menuItem.DropDownItems.Add(action.MenuItem);
                return menuItem;
            }
        }
        public ChannelOption Parent { get; set; }
        public ChannelOptionsCollection topParent = null;
        public ChannelOptionsCollection TopParent
        {
            get
            {
                if (topParent == null) return Parent.TopParent;
                else return topParent;
            }
            set { topParent = value; }
        }
        public override string ToString()
        {
            return Title;
        }
    }
    public class GroupHeadOption:ChannelOption
    {
        public GroupHeadOption(string Title)
        {
            this.Title = Title;
        }
    }
    public class CalibrationFunctionOption:ChannelOption
    {
        public CalibrationFunction TF { get; protected set; }
        public CalibrationFunctionOption(CalibrationFunction func)
        {
            TF = func;
        }
        public override string Title
        {
            get
            {
                return TF.InstrumentID;
            }

            set
            {
                base.Title = value;
            }
        }
    }
    public class UnitOption : ChannelOption
    {
        public UnitConversion Unit { get; protected set; }
        public UnitOption(UnitConversion units)
        {
            Unit = units;
        }
        public override string Title
        {
            get
            {
                return Unit.Unit;
            }
        }
    }
    public class ChannelTypeOption : ChannelOption
    {
        public ChannelTypeOption(ChannelType Type)
        {
            this.Type = Type;
        }
        public ChannelType Type { get; protected set; } = ChannelType.AnalogInRSE;
        public override string Title
        {
            get
            {
                return Type.ToString();
            }
            set
            {
                foreach (ChannelType s in Enum.GetValues(typeof(ChannelType)))
                {
                    if (s.ToString() == value)
                    {
                        Type = s;
                        return;
                    }
                }
                throw new FormatException();
            }
        }
    }
    //public class AnalogInRSESourceOption : ChannelOption
    //{
    //    public BoardPin BoardPin { get; protected set; }
    //    public override string Title
    //    {
    //        get
    //        {
    //            return BoardPin.Name;
    //        }

    //        set
    //        {
    //            base.Title = value;
    //        }
    //    }
    //}

    //public class AnalogInDifferentialSourceOption : ChannelOption
    //{
    //    public AnalogInDifferentialSourceOption(BoardPin P, BoardPin N)
    //    {
    //        BoardPinN = N;
    //        BoardPinP = P;
    //    }
    //    public BoardPin BoardPinN { get; protected set; }
    //    public BoardPin BoardPinP { get; protected set; }
    //    public override string Title
    //    {
    //        get
    //        {
    //            return BoardPinP.Name + " (+) -- " + BoardPinN.Name + " (-)";
    //        }

    //        set
    //        {
    //            base.Title = value;
    //        }
    //    }
    //}
    public class RangeSelectionOption : ChannelOption
    {
        public bool CanBeNegative { get; set; }
        public RangeSelectionOption(float range, bool CanBeNegative)
        {
            this.CanBeNegative = CanBeNegative;
            this.Range = range;
        }
        public float Range { get; set; } = 3;
        public override string Title
        {
            get
            {
                if (Range < 0.1)
                    return (CanBeNegative ? "±" : "0-") + Math.Round(Range * 1000).ToString() + "mV";
                return (CanBeNegative ? "±" : "0-") + Math.Round(Range, 1).ToString() + "V";
            }
        }
    }
    public class ActionOption : ChannelOption
    {
        public Func<ActionOption, bool> Function { get; protected set; }
        public ActionOption(string title, Func<ActionOption, bool> actionToCall)
        {
            this.Function = actionToCall;
            Title = title;
            MenuItem.Click += MenuItem_Click;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Function(this);
        }
    }
    public class LineOpacityOption : ChannelOption
    {
        public LineOpacityOption(float opacity)
        {
            Value = opacity;
        }
        public float Value { get; set; } = 1;
        public override string Title
        {
            get
            {
                return (Value * 100) + "%";
            }
        }
    }
    public class LineThicknessOption : ChannelOption
    {
        public LineThicknessOption(int thickness)
        { this.Value = thickness; }
        public int Value { get; set; } = 1;
        public override string Title
        {
            get
            {
                return Value.ToString();
            }
        }
    }
    public class LineColorOption : ChannelOption
    {
        public LineColorOption(System.Drawing.Color color)
        {
            this.Color = color;
        }

        System.Drawing.Color color = System.Drawing.Color.Black;
        public System.Drawing.Color Color { get { return color; }  set { color = value; MenuItem.BackColor = value; } }
        public override string Title
        {
            get
            {
                return "";
            }
        }

    }
    public class InstrumentTypeOption : ChannelOption
    {
        public InstrumentTypeOption(Instrument instrument)
        { this.Instrument = instrument; }
        public Instrument Instrument { get; set; }
        public override string Title
        {
            get
            {
                return Instrument.Title;
            }
            
        }
    }
    //public class BoardPin
    //{
    //    public BoardPin(int index, string name)
    //    {
    //        this.Index = index;
    //        this.Name = name;
    //    }
    //    public int Index { get; set; } = -1;
    //    public string Name { get; set; } = "";
    //}
    public class AlwaysDisabledOption : ChannelOption
    {
        public AlwaysDisabledOption(string text)
        {
            Title = text;
            MenuItem.Enabled = false;
        }
    }
    public class ChannelConfigurationObject
    {
    }
    public class ToolStripMenuItemWithChannelOption : ToolStripMenuItem
    {
        public ToolStripMenuItemWithChannelOption()
        {
            Font = new System.Drawing.Font("Arial", 16);
        }
        public ChannelOption AssociatedChannelOption { get; set; }
    }
}

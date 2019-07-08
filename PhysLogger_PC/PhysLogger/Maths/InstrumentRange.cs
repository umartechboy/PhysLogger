using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysLogger.LogControls;
using PhysLogger.Hardware;

namespace PhysLogger.Maths
{
    public delegate void RangeChangeHandler(I2CInstrument instrument, InstrumentRange range);
    public class InstrumentRange
    {
        public InstrumentRange(string Title, Function TF, byte code)
        {
            this.Title = Title;
            this.TF = TF;
            this.Code = code;
        }
        public byte Code { get; protected set; }
        public string Title { get; set; } = "";
        public Function TF { get; set; }
        public static InstrumentRange Parse(string str)
        {
            string title = "";
            Function tf = null;
            byte code = 0;
            var pairs = str.Split(new char[] { '&' });
            foreach (var pair in pairs)
            {
                var parts = pair.Split(new char[] { ':' });
                parts[0] = parts[0].ToLower();
                if (parts[0] == "title")
                    title = parts[1];
                else if (parts[0] == "func")
                    tf = Function.Parse(parts[1]);
                else if (parts[0] == "code")
                    code = byte.Parse(parts[1]);
            }
            if (tf == null || title == "")
                return null;
            return new InstrumentRange(title, tf, code);
        }
        public override string ToString()
        {
            return "Title = " + Title + ", " + "Code = " + Code + ", TF = " + TF.ToString();
        }
    }
    public class InstrumentRangeCollection
    {
        public event RangeChangeHandler OnRangeChanged;
        public List<InstrumentRange> Items { get; private set; } = new List<InstrumentRange>();

        public InstrumentRange Current
        {
            get
            {
                return ((InstrumentRangeOption)MenuItem.SubOptions.Find(uOp => uOp.Checked)).Range;
            }
            set
            {
                foreach (InstrumentRangeOption item in menuItem.SubOptions)
                    item.Checked = item.Range == value;
            }
        }
        public void Add(InstrumentRange range)
        {
            var uOp = new InstrumentRangeOption(range);
            uOp.MenuItem.AssociatedChannelOption = uOp;
            uOp.MenuItem.Click += MenuItem_Click;
            menuItem.SubOptions.Add(uOp);
            Items.Add(range);
            if (Items.Count == 1)
                Current = range;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var uOp = ((InstrumentRangeOption)(((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption));
            // Current = uOp.Range;
            // Current will be set via feedback
            OnRangeChanged?.Invoke(null, uOp.Range);
        }

        public float Convert(float value)
        {
            return Current.TF.Evaluate(value);
        }
        public InstrumentRange this[int index]
        {
            get { return Items[index]; }
        }
        public int Count { get { return Items.Count; } }
        ChannelOption menuItem = new GroupHeadOption("Range");
        public ChannelOption MenuItem { get { return menuItem; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysLogger;
using PhysLogger.LogControls;
using PhysLogger.Hardware;

namespace PhysLogger.Maths
{

    public class UnitConversion
    {
        public UnitConversion(string Title, string unitSymbol, Function TF)
        {
            this.Title = Title;
            this.Unit = unitSymbol;
            this.TF = TF;
        }
        public string Title { get; set; } = "";
        public string Unit { get; set; } = "";
        public Function TF { get; set; }
        public PlotLabel Label { get { return new PlotLabel(Title, Unit); } set { Title = value.Name; Unit = value.Unit; } }
        public static UnitConversion Parse(string str)
        {
            string title = "";
            string sym = "";
            Function tf = null;
            var pairs = str.Split(new char[] { '&' });
            foreach (var pair in pairs)
            {
                var parts = pair.Split(new char[] { ':' });
                parts[0] = parts[0].ToLower();
                if (parts[0] == "title")
                    title = parts[1];
                else if (parts[0] == "func")
                    tf = Function.Parse(parts[1]);
                else if (parts[0] == "unit")
                    sym = parts[1];
            }
            if (tf == null || title == "")
                return null;
            return new UnitConversion(title, sym, tf);
        }
        public override string ToString()
        {
            return Label.ToString() + ", " + TF.ToString();
        }
    }
    public class UnitConversionCollection
    {
        
        public List<UnitConversion> Items { get; private set; } = new List<UnitConversion>();

        public UnitConversion Current
        {
            get
            {
                return ((UnitOption)MenuItem.SubOptions.Find(uOp => uOp.Checked)).Unit;
            }
            set
            {
                foreach(UnitOption item in menuItem.SubOptions)
                    item.Checked = item.Unit == value;
            }
        }
        public void Add(UnitConversion unit)
        {
            var uOp = new UnitOption(unit);
            uOp.MenuItem.AssociatedChannelOption = uOp;
            uOp.MenuItem.Click += MenuItem_Click;
            menuItem.SubOptions.Add(uOp);
            Items.Add(unit);
            if (Items.Count == 1)
                Current = unit;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var uOp = ((UnitOption)(((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption));
            Current = uOp.Unit;
        }

        public float Convert(float value)
        {
            return Current.TF.Evaluate(value);
        }
        public UnitConversion this[int index]
        {
            get { return Items[index]; }
        }
        public int Count { get { return Items.Count; } }
        ChannelOption menuItem = new GroupHeadOption("Unit");
        public ChannelOption MenuItem{ get { return menuItem; } }
    }
}

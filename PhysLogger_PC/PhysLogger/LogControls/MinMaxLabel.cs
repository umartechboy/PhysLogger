using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysLogger
{
    public partial class MinMaxLabel : UserControl
    {
        public MinMaxLabel()
        {
            InitializeComponent();
        }
        public Font MinMaxFont
        {
            get
            {
                return minL.Font;
            }

            set
            {
                minL.Font = value;
                maxL.Font = value;
            }
        }
        public Font MainFont
        {
            get
            {
                return ValueL.Font;
            }

            set
            {
                ValueL.Font = value;
            }
        }
        public override string Text
        {
            get
            {
                return ValueL.Text;
            }

            set
            {
                ValueL.Text = value;
            }
        }
        int rounding = 2;
        public int Rounding
        {
            get { return rounding; }
            set { rounding = value; Value = Value; Max = Max; Min = Min; }
        }
        public string Suffix { get { return sufL.Text; } set { if (sufL.Text != value) sufL.Text = value; } }
        public double Value
        {
            get { return Convert.ToDouble(Text); }
            set
            {
                Text = Math.Round(value, Rounding).ToString();
                if (value < Min)
                    Min = value;
                if (value > Max)
                    Max = value;
            }
        }
        public double Max { get { if (maxL.Text == "") maxL.Text = "0"; return Convert.ToDouble(maxL.Text); } set { maxL.Text = Math.Round(value, Rounding).ToString(); } }
        public double Min
        {
            get
            {
                if (minL.Text == "") minL.Text = "0";
                return Convert.ToDouble(minL.Text);
            }
            set { minL.Text = Math.Round(value, Rounding).ToString(); }
        }

        private void resetMaxB_Click(object sender, EventArgs e)
        {
            Max = Value;
        }

        private void resetMinB_Click(object sender, EventArgs e)
        {
            Min = Value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FivePointNine.Windows.Controls
{
    public class FlatComboBox: UserControl
    {
        public event EventHandler ValueChanged;
        private Label valueLable;
        int delayedValueInd = 0;
        public string Unit { get; set; } = "";

        public FlatComboBox()
        {
            InitializeComponent();
            MouseMove += FlatNumericUpDown_MouseMove;
            Click += FlatNumericUpDown_Click;
            MouseLeave += FlatNumericUpDown_MouseLeave;
            SizeChanged += FlatNumericUpDown_SizeChanged;
            MouseDown += FlatNumericUpDown_MouseDown;
            DoubleBuffered = true;
            MouseUp += FlatNumericUpDown_MouseUp;
            valueLable.MouseDown += Label1_MouseDown;
            valueLable.MouseUp += Label1_MouseUp;
            valueLable.MouseMove += Label1_MouseMove;
            valueLable.Cursor = Cursors.SizeNS;
            MouseEnter += FlatNumericUpDown_MouseEnter;
        }
        bool active = false;

        private void FlatNumericUpDown_MouseEnter(object sender, EventArgs e)
        {
            active = true;
            Invalidate();
        }


        public int Increment { get; set; } = 1;
        Point lastLabelMouse = new Point();
        private void Label1_MouseMove(object sender, MouseEventArgs e)
        {
            int change = (e.Y - lastLabelMouse.Y) / 10;
            if (clickState)
                IncValue(change * Increment);
            if (change != 0)
                lastLabelMouse = e.Location;
        }

        private void Label1_MouseUp(object sender, MouseEventArgs e)
        {
            valueLable.BackColor = BackColor;
            clickState = false;
        }

        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            valueLable.BackColor = Color.LightGray;
            clickState = true;
        }
        
        int dir = 0;
        private void FlatNumericUpDown_MouseMove(object sender, MouseEventArgs e)
        {
            int bkp = dir;
            if (e.Y < 2 * arrowM + arrowH)
                dir = 1;
            else if (e.Y > Height - (2 * arrowM + arrowH))
                dir = -1;
            else
                dir = 0;
            if (dir == 0)
                Cursor = Cursors.SizeNS;
            if (bkp != dir)
                Invalidate();
        }
        private void FlatNumericUpDown_MouseUp(object sender, MouseEventArgs e)
        {
            clickState = false;
            active = false;
            Invalidate();
        }

        private void FlatNumericUpDown_MouseDown(object sender, MouseEventArgs e)
        {
            clickState = true;
            Invalidate();
        }

        private void FlatNumericUpDown_MouseLeave(object sender, EventArgs e)
        {
            dir = 0;
            Invalidate();
        }

        private void FlatNumericUpDown_SizeChanged(object sender, EventArgs e)
        {
            valueLable.Height = (int)Math.Floor(Height - arrowH * 2 - arrowM * 4 - 5);
            valueLable.Width = Width;
            valueLable.Left = 2;
        }

        int shownInd = 0;
        List<string> items = new List<string>();
        public void IncValue(int fac)
        {
            int value = shownInd + fac;
            var bkp = shownInd;
            if (value >= items.Count) value = items.Count - 1;
            if (value < 0)
                value = 0;
            shownInd = value;
            if (shownInd == bkp) return;
            valueLable.Text = items[shownInd] + Unit;
            ValueChanged?.Invoke(this, new EventArgs());
        }
        public void ForceValue(string V)
        {
            shownInd = items.IndexOf(V);
            if (shownInd < 0)
            {
                valueLable.Text = items[shownInd].ToString() + Unit;
                return;
            }
            IncValue(0);
            valueLable.Text = items[shownInd].ToString() + Unit;
        }
        Timer td = new Timer();
        //public void ForceDelayedValue(string V, int delayMs)
        //{
        //    td.Stop();
        //    if (items.Contains(V))
        //    {
        //        td.Tick += Td_Tick;
        //        td.Interval = delayMs;
        //        td.Start();
        //        delayedValueInd = items.IndexOf(V);
        //    }
        //    else
        //    { valueLable.Text = "--"; }
        //}

        private void Td_Tick(object sender, EventArgs e)
        {
            td.Stop();
            ForceValue(items[delayedValueInd]);
        }

        public string Value
        {
            get { return items[shownInd]; }

        }
        private void FlatNumericUpDown_Click(object sender, EventArgs e)
        {
            IncValue(-dir);
        }


        float arrowW = 40;
        float arrowH = 5;
        float arrowM = 4;
        bool clickState = false;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.Clear(BackColor);
            if (dir == 1)
            {
                g.FillRectangle(clickState ? Brushes.DarkGray : Brushes.LightGray, 0, 0, Width, arrowH + 2 * arrowM);
            }
            else if (dir == -1)
            {
                g.FillRectangle(clickState ? Brushes.DarkGray : Brushes.LightGray, 0, Height - arrowH - 2 * arrowM, Width, arrowH + 2 * arrowM);
            }

            PointF[] topPoints = new PointF[] { new PointF(Width / 2 - arrowW / 2, arrowM + arrowH), new PointF(Width / 2 + arrowW / 2, arrowM + arrowH), new PointF(Width / 2 + 1, arrowM) };
            PointF[] bottomPoints = new PointF[] { new PointF(Width / 2 - arrowW / 2, Height - (arrowM + arrowH)), new PointF(Width / 2 + arrowW / 2, Height - (arrowM + arrowH)), new PointF(Width / 2 + 1, Height - arrowM) };
            g.FillPolygon(dir == 1 ? Brushes.Black : new SolidBrush(active ? Color.LightGray : BackColor), topPoints);
            g.FillPolygon(dir == -1 ? Brushes.Black : new SolidBrush(active ? Color.LightGray : BackColor), bottomPoints);
        }
        private void InitializeComponent()
        {
            this.valueLable = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.valueLable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueLable.Location = new System.Drawing.Point(0, 14);
            this.valueLable.Name = "label1";
            this.valueLable.Size = new System.Drawing.Size(150, 23);
            this.valueLable.TabIndex = 0;
            this.valueLable.Text = "--";
            this.valueLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FlatNumericUpDown
            // 
            this.Controls.Add(this.valueLable);
            this.Name = "FlatNumericUpDown";
            this.Size = new System.Drawing.Size(150, 49);
            this.ResumeLayout(false);

        }
        
        public void SetFloatList(List<float> floatValues, int rounding)
        {
            items.Clear();
            foreach (var val in floatValues)
            {
                items.Add(Math.Round(val, rounding).ToString());
            }
            shownInd = 0;
            valueLable.Text = items[shownInd];
        }
    }
}

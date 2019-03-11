using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FivePointNine.Windows.Controls
{
    public class FlatCheckBox:CheckBox
    {
        Color cc = Color.Black;
        Color ucc = Color.Black;
        public Color CheckedColor { get { return cc; } set { cc = value; Invalidate();Application.DoEvents(); } }
        public Color UncheckedColor { get { return ucc; } set { ucc = value; Invalidate(); Application.DoEvents(); } }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.Clear(BackColor);
            float wid = Height*0.6F;
            Brush b = new SolidBrush(cc);
            if (!Checked)
                b = new SolidBrush(ucc);
            g.FillRectangle(b, 0.0F, Height / 2 - wid / 2, wid, wid);
            var sz = g.MeasureString(Text, Font);
            g.DrawString(Text, Font, b, wid + 5, Height / 2 - sz.Height / 2);
        }

    }
}

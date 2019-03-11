using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using FivePointNine.Graphics;
using FivePointNine.Numbers;

namespace PhysLogger
{
    public class legendLinePanel : Panel
    {
        public bool ShowLine { get; set; } = true;
        Pen _pen = Pens.Black;
        public Pen Pen { get { return _pen; } set { _pen = value; Invalidate(); } }
        public legendLinePanel()
        {
            DoubleBuffered = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            if (ShowLine)
                e.Graphics.DrawLine(Pen, 0, Height / 2, Width, Height / 2);
        }
    }
}

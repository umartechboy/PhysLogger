using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FivePointNine.Windows.Controls
{
    public class ColoredCheckBox:CheckBox
    {
        public ColoredCheckBox()
        {
            AutoSize = false;
            Cursor = Cursors.Hand;
        }
        Color _textColorUnchecked = Color.White, _textColorChecked = Color.White, _checkColor = Color.White, _uncheckedColor = Color.Gray;
        bool useLighterColorsC = false, useLighterColorsUC = false;
        public Color CheckedTextColor { get { return _textColorChecked; } set { _textColorChecked = value; Invalidate(); } }
        public Color UncheckedTextColor { get { return _textColorUnchecked; } set { _textColorUnchecked = value; Invalidate(); } }
        public Color CheckedColor { get { return _checkColor; } set { _checkColor = value; Invalidate(); } }
        public Color UncheckedColor { get { return _uncheckedColor; } set { _uncheckedColor = value; Invalidate(); } }
        public bool CheckedColorIsLight { get { return useLighterColorsC; } set { useLighterColorsC = value; Invalidate(); } }
        public bool UncheckedColorIsLight { get { return useLighterColorsUC; } set { useLighterColorsUC = value; Invalidate(); } }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            System.Drawing.Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(BackColor);
            int dif = 60;
            Color DarkerChecked = Color.FromArgb(Math.Max(CheckedColor.R - dif, 0), Math.Max(CheckedColor.G - dif, 0), Math.Max(CheckedColor.B - dif, 0));
            Color DarkerUnchecked = Color.FromArgb(Math.Max(UncheckedColor.R - dif, 0), Math.Max(UncheckedColor.G - dif, 0), Math.Max(UncheckedColor.B - dif, 0));
            Color LighterChecked = Color.FromArgb(
                Math.Min(CheckedColor.R + dif, 255),
                Math.Min(CheckedColor.G + dif, 255),
                Math.Min(CheckedColor.B + dif, 255)
                );
            Color LighterUnchecked = Color.FromArgb(
                Math.Min(UncheckedColor.R + dif, 255),
                Math.Min(UncheckedColor.G + dif, 255),
                Math.Min(UncheckedColor.B + dif, 255)
                );
            var m = g.MeasureString(Text, Font);
            if (Checked)
            {
                if (CheckedColorIsLight)
                {
                    g.FillRoundedRectangle(new SolidBrush(CheckedColor), new RectangleF(0, 0, Width - 1, Height - 1), 10);
                    g.DrawRoundedRectangle(new Pen(DarkerChecked, 3), new RectangleF(1.5F, 1.5F, Width - 4, Height - 4), 10);
                }
                else
                {
                    g.FillRoundedRectangle(new SolidBrush(LighterChecked), new RectangleF(0, 0, Width - 1, Height - 1), 10);
                    g.DrawRoundedRectangle(new Pen(CheckedColor, 3), new RectangleF(1.5F, 1.5F, Width - 4, Height - 4), 10);
                }
                    g.DrawString(Text, Font, new SolidBrush(CheckedTextColor), (Width - m.Width) / 2, (Height - m.Height) / 2);
            }
            else
            {
                if (UncheckedColorIsLight)
                    g.FillRoundedRectangle(new SolidBrush(DarkerUnchecked), new RectangleF(0, 0, Width - 1, Height - 1), 10);
                else
                    g.FillRoundedRectangle(new SolidBrush(LighterUnchecked), new RectangleF(0, 0, Width - 1, Height - 1), 10);
                g.DrawRoundedRectangle(new Pen(UncheckedColor, 3), new RectangleF(1.5F, 1.5F, Width - 4, Height - 4), 10);
                g.DrawString(Text, Font, new SolidBrush(UncheckedTextColor), (Width - m.Width) / 2, (Height - m.Height) / 2);
            }
        }
    }
}
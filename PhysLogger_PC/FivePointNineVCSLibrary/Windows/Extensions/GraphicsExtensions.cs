using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FivePointNine.Graphics
{
    public static class GraphicsUtils
    {
        public static Color GetContrast(this Color source, bool preserveOpacity = true)
        {
            Color inputColor = source;
            //if RGB values are close to each other by a diff less than 10%, then if RGB values are lighter side, decrease the blue by 50% (eventually it will increase in conversion below), if RBB values are on darker side, decrease yellow by about 50% (it will increase in conversion)
            byte avgColorValue = (byte)((source.R + source.G + source.B) / 3);
            if (avgColorValue < 110 || avgColorValue > 144) //The color is a shade of gray
            {
                if (avgColorValue < 123) //color is dark
                {
                    inputColor = Color.FromArgb(221, 57, 138);
                }
                else
                {
                    inputColor = Color.FromArgb(34, 198, 117);
                }
            }
            byte sourceAlphaValue = source.A;
            if (!preserveOpacity)
            {
                sourceAlphaValue = Math.Max(source.A, (byte)127); //We don't want contrast color to be more than 50% transparent ever.
            }
            RGB rgb = new RGB { R = inputColor.R, G = inputColor.G, B = inputColor.B };
            HSB hsb = ConvertToHSB(rgb);
            hsb.H = hsb.H < 180 ? hsb.H + 180 : hsb.H - 180;
            //hsb.B = isColorDark ? 240 : 50; //Added to create dark on light, and light on dark
            rgb = ConvertToRGB(hsb);
            return Color.FromArgb(sourceAlphaValue, (byte)rgb.R, (byte)rgb.G, (byte)rgb.B); ;
        }
        internal static RGB ConvertToRGB(HSB hsb)
        {
            // By: <a href="http://blogs.msdn.com/b/codefx/archive/2012/02/09/create-a-color-picker-for-windows-phone.aspx" title="MSDN" target="_blank">Yi-Lun Luo</a>
            double chroma = hsb.S * hsb.B;
            double hue2 = hsb.H / 60;
            double x = chroma * (1 - Math.Abs(hue2 % 2 - 1));
            double r1 = 0d;
            double g1 = 0d;
            double b1 = 0d;
            if (hue2 >= 0 && hue2 < 1)
            {
                r1 = chroma;
                g1 = x;
            }
            else if (hue2 >= 1 && hue2 < 2)
            {
                r1 = x;
                g1 = chroma;
            }
            else if (hue2 >= 2 && hue2 < 3)
            {
                g1 = chroma;
                b1 = x;
            }
            else if (hue2 >= 3 && hue2 < 4)
            {
                g1 = x;
                b1 = chroma;
            }
            else if (hue2 >= 4 && hue2 < 5)
            {
                r1 = x;
                b1 = chroma;
            }
            else if (hue2 >= 5 && hue2 <= 6)
            {
                r1 = chroma;
                b1 = x;
            }
            double m = hsb.B - chroma;
            return new RGB()
            {
                R = r1 + m,
                G = g1 + m,
                B = b1 + m
            };
        }
        internal static HSB ConvertToHSB(RGB rgb)
        {
            // By: <a href="http://blogs.msdn.com/b/codefx/archive/2012/02/09/create-a-color-picker-for-windows-phone.aspx" title="MSDN" target="_blank">Yi-Lun Luo</a>
            double r = rgb.R;
            double g = rgb.G;
            double b = rgb.B;

            double max = Max(r, g, b);
            double min = Min(r, g, b);
            double chroma = max - min;
            double hue2 = 0d;
            if (chroma != 0)
            {
                if (max == r)
                {
                    hue2 = (g - b) / chroma;
                }
                else if (max == g)
                {
                    hue2 = (b - r) / chroma + 2;
                }
                else
                {
                    hue2 = (r - g) / chroma + 4;
                }
            }
            double hue = hue2 * 60;
            if (hue < 0)
            {
                hue += 360;
            }
            double brightness = max;
            double saturation = 0;
            if (chroma != 0)
            {
                saturation = chroma / brightness;
            }
            return new HSB()
            {
                H = hue,
                S = saturation,
                B = brightness
            };
        }
        private static double Max(double d1, double d2, double d3)
        {
            if (d1 > d2)
            {
                return Math.Max(d1, d3);
            }
            return Math.Max(d2, d3);
        }
        private static double Min(double d1, double d2, double d3)
        {
            if (d1 < d2)
            {
                return Math.Min(d1, d3);
            }
            return Math.Min(d2, d3);
        }
        internal struct RGB
        {
            internal double R;
            internal double G;
            internal double B;
        }
        internal struct HSB
        {
            internal double H;
            internal double S;
            internal double B;
        }
        public static Color[] CommonColors()
        {
            UInt32[] vs = new UInt32[] { 0x408000, 0x7E4001, 0x290075, 0x0, 0xC0C0C0, 0x808080, 0xFFFFFF, 0x800000, 0xFF0000, 0x800080, 0xFF00FF, 0x8000, 0x0FF00, 0x808000, 0xFFFF00, 0x80, 0x0000FF, 0x8080, 0x00FFFF };

            List<Color> colorsList = new List<Color>();
            foreach (var v in vs)
            {
                colorsList.Add(Color.FromArgb((int)(v % 256), (int)((v >> 8) % 256), (int)((v >> 16) % 256)));
            }
            //var colors = Enum.GetValues(typeof(KnownColor));
            //foreach (KnownColor knowColor in colors)
            //{
            //    var c = Color.FromKnownColor(knowColor);
            //    if (c.A != 255)
            //        continue;
            //    colorsList.Add(c);
            //}
            return colorsList.ToArray();
        }
    }
}

namespace System.Drawing
{
    public static class GraphicsExtensions
    {
        public static void DrawString(this Graphics g, string str, Font font, Brush brush, float x, float y, float angle)
        {
            // Save the graphics state.
            GraphicsState state = g.Save();
            g.ResetTransform();

            // Rotate.
            g.RotateTransform(angle);

            // Translate to desired position. Be sure to append
            // the rotation so it occurs after the rotation.
            g.TranslateTransform(x, y, MatrixOrder.Append);
            
            // Draw the text at the origin.
            g.DrawString(str, font, brush, 0, 0);

            // Restore the graphics state.
            g.Restore(state);
        }
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, RectangleF bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }
        public static GraphicsPath RoundedRect(RectangleF bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            RectangleF arc = new RectangleF(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }
    }
}

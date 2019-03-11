using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivePointNine.Windows.Graphics
{
    public class Graphics2:IDisposable
    {
        System.Drawing.Graphics gdi;
        Direct3D9Graphics d3d;
        Graphics2() { }
        bool isD3D = false;
        public static Graphics2 FromGDI(System.Drawing.Graphics g)
        {
            var obj = new Graphics2();
            obj.gdi = g;
            obj.isD3D = false;
            return obj;
        }
        public static Graphics2 AttachD3D(System.Windows.Forms.Control control)
        {
            var obj = new Graphics2();
            obj.d3d = new Direct3D9Graphics(control);
            obj.isD3D = true;
            return obj;
        }
        public void RotateTransform(float angleInDegrees)
        {
            if (isD3D)
                d3d.RotateTransform(angleInDegrees);
            else
                gdi.RotateTransform(angleInDegrees);
        }
        public void TranslateTransform(float x, float y)
        {
            if (isD3D)
                d3d.TranslateTransform(x, y);
            else
                gdi.TranslateTransform(x, y);
            
        }
        public void ScaleTransform(float xscale, float yscale)
        {
            if (isD3D)
                d3d.ScaleTransform(xscale, yscale);
            else
                gdi.ScaleTransform(xscale, yscale);
        }
        public void DrawString(string s, System.Drawing.Font f, System.Drawing.Color c, float x, float y)
        {
            if (isD3D)
                d3d.DrawString(s, f, c, x, y);
            else
                gdi.DrawString(s, f, new System.Drawing.SolidBrush(c), x, y);
        }
        public void DrawString(string s, System.Drawing.Font f, System.Drawing.Color c, System.Drawing.PointF position)
        {
            if (isD3D)
                d3d.DrawString(s, f, c, position.X, position.Y);
            else
                gdi.DrawString(s, f, new System.Drawing.SolidBrush(c), position);
        }
        public void DrawString(string s, Font f, Color c, float x, float y, float rotation)
        {
            if (isD3D)
                d3d.DrawString(s, f, c, x, y, rotation);
            else
                gdi.DrawString(s, f, new System.Drawing.SolidBrush(c), x, y, rotation);
        }
        public void FillRectangle(System.Drawing.Color c, float x, float y, float width, float height)
        {
            if (isD3D)
                d3d.FillRectangle(c, x, y, width, height);
            else
                gdi.FillRectangle(new SolidBrush(c), x, y, width, height);
        }
        public void DrawRectangle(System.Drawing.Pen p, float x, float y, float width, float height)
        {
            if (isD3D)
                d3d.DrawRectangle(p, x, y, width, height);
            else
                gdi.DrawRectangle(p, x, y, width, height);
        }
        public void DrawRectangle(System.Drawing.Pen p, RectangleF drawingRect)
        {
            if (isD3D)
                d3d.DrawRectangle(p, drawingRect.X, drawingRect.Y, drawingRect.Width, drawingRect.Height);
            else
                gdi.DrawRectangle(p, drawingRect.X, drawingRect.Y, drawingRect.Width, drawingRect.Height);
        }
        public void DrawLine(System.Drawing.Pen p, System.Drawing.PointF p1, System.Drawing.PointF p2)
        {
            if (isD3D)
                d3d.DrawLine(p, p1, p2);
            else
                gdi.DrawLine(p, p1, p2);
        }
        public void DrawLine(System.Drawing.Pen p, float x1, float y1, float x2, float y2)
        {
            if (isD3D)
                d3d.DrawLine(p, x1, y1, x2, y2);
            else
                gdi.DrawLine(p, x1, y1, x2, y2);
        }
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
        {
            get
            {
                if (isD3D)
                    return d3d.AntiAlias ? System.Drawing.Drawing2D.SmoothingMode.AntiAlias : System.Drawing.Drawing2D.SmoothingMode.None;
                else
                    return gdi.SmoothingMode;
            }
            set
            {
                if (isD3D)
                    d3d.AntiAlias = value == System.Drawing.Drawing2D.SmoothingMode.AntiAlias || value == System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                else
                    gdi.SmoothingMode = value;
            }
        }

        public void ResetTransform()
        {
            if (isD3D)
                d3d.ResetTransform();
            else
                gdi.ResetTransform();
        }

        public Region Clip
        {
            get
            {
                return isD3D ? new Region() : gdi.Clip;
            }
            set
            {
                if (!isD3D)
                    gdi.Clip = value;
            }
        }

        public int D3DPendingElements { get { return d3d.PendingElements; } }
        public void Clear(System.Drawing.Color color)
        {
            if (isD3D)
                d3d.Clear(color);
            else
                gdi.Clear(color);
        }
        public void Flush()
        {
            if (isD3D)
                d3d.Flush();
            else
                gdi.Flush();
        }
        public System.Drawing.RectangleF MeasureString(string s, System.Drawing.Font f)
        {
            if (isD3D)
                return d3d.MeasureString(s, f);
            else
                return new System.Drawing.RectangleF(new System.Drawing.PointF(), gdi.MeasureString(s, f));
        }
        
        public void DrawLines(Pen p, PointF[] list)
        {
            if (isD3D)
                d3d.DrawLines(p, list);
            else
                gdi.DrawLines(p, list);
        }

        public void Dispose()
        {
            d3d.Dispose();
        }

        public void DrawImage(Image i, float x, int y)
        {
            if (isD3D)
                throw new NotImplementedException();
            else
                gdi.DrawImage(i, x, y);
        }
    }
    public class Direct3D9Graphics : IDisposable
    {
        public SlimDX.Direct3D9.Direct3D d3d;
        public SlimDX.Direct3D9.Device device;
        System.Windows.Forms.Control c;

        TransformationMatrix transform;
        SlimDX.Direct3D9.Font font;
        public SlimDX.Direct3D9.Line line;
        
        System.Drawing.Font fCurrent;
        public Direct3D9Graphics(System.Windows.Forms.Control control)
        {
            
            c = control;
            d3d = new SlimDX.Direct3D9.Direct3D();
            SlimDX.Direct3D9.PresentParameters pp = new SlimDX.Direct3D9.PresentParameters();
            pp.SwapEffect = SlimDX.Direct3D9.SwapEffect.Discard;
            pp.DeviceWindowHandle = control.Handle;
            pp.Windowed = true;

            pp.BackBufferWidth = control.Width;
            pp.BackBufferHeight = control.Height;
            pp.BackBufferFormat = SlimDX.Direct3D9.Format.A8R8G8B8;
            device = new SlimDX.Direct3D9.Device(
                d3d, 
                0, 
                SlimDX.Direct3D9.DeviceType.Hardware, 
                control.Handle, 
                SlimDX.Direct3D9.CreateFlags.HardwareVertexProcessing, 
                pp);
            line = new SlimDX.Direct3D9.Line(device);
            fCurrent = new System.Drawing.Font("ARIAL", 10);
            font = new SlimDX.Direct3D9.Font(device, fCurrent);
            transform = new TransformationMatrix();
        }
        public void RotateTransform(float angleInDegrees)
        {
            transform.AddRotation(angleInDegrees);
        }
        public void TranslateTransform(float x, float y)
        {
            transform.AddTranslation(x, y);
        }
        public void ScaleTransform(float xscale, float yscale)
        {
            transform.AddScale(xscale, yscale);
        }
        public void DrawString(string s, System.Drawing.Font f, System.Drawing.Color c, float x, float y)
        {
            Begin();
            var v = transform.Transform(new SlimDX.Vector2(x, y));
            if (fCurrent.Height != f.Height || fCurrent.FontFamily != f.FontFamily)
            {
                font.Dispose();
                font = new SlimDX.Direct3D9.Font(device, f);
                fCurrent = f;
            }
            font.DrawString(null, s, (int)v.X, (int)v.Y, new SlimDX.Color4(c));
        }
        public void DrawString(string s, System.Drawing.Font f, System.Drawing.Color c, System.Drawing.PointF position)
        {
            DrawString(s, f, c, position.X, position.Y);
        }
        public void DrawString(string s, Font f, Color c, float x, float y, float rotation)
        {
            transform.AddRotation(rotation);
            DrawString(s, f, c, x, y);
            transform.AddRotation(-rotation);
        }
        public void DrawRectangle(System.Drawing.Pen p, float x, float y, float width, float height)
        {
            Begin();
            line.Antialias = AntiAlias;
            line.Width = p.Width;
            SlimDX.Vector2[] v = new SlimDX.Vector2[] {
                transform.Transform(new SlimDX.Vector2(x, y)),
                transform.Transform(new SlimDX.Vector2(x + width, y)),
                transform.Transform(new SlimDX.Vector2(x + width, y + height)),
                transform.Transform(new SlimDX.Vector2(x, y + height)),
                transform.Transform(new SlimDX.Vector2(x, y))
            };
            line.Draw(v, new SlimDX.Color4(p.Color));
        }
        public void DrawLines(Pen p, PointF[] list)
        {
            Begin();
            line.Antialias = AntiAlias;
            line.Width = p.Width;
            SlimDX.Vector2[] v = new SlimDX.Vector2[list.Length];
            for (int i = 0; i < list.Length; i++)
                v[i] = transform.Transform(new SlimDX.Vector2(list[i].X, list[i].Y));
            line.Draw(v, new SlimDX.Color4(p.Color));
        }
        public void DrawLine(System.Drawing.Pen p, System.Drawing.PointF p1, System.Drawing.PointF p2)
        {
            DrawLine(p, p1.X, p1.Y, p2.X, p2.Y);
        }
        public void DrawLine(System.Drawing.Pen p, float x1, float y1, float x2, float y2)
        {
            Begin();
            line.Antialias = AntiAlias;
            line.Width = p.Width;
            SlimDX.Vector2[] v = new SlimDX.Vector2[] {
                transform.Transform(new SlimDX.Vector2(x1, y1)),
                transform.Transform(new SlimDX.Vector2(x2, y2)) };
            line.Draw(v, new SlimDX.Color4(p.Color));
        }
        internal void FillRectangle(Color c, float x, float y, float width, float height)
        {
            throw new NotImplementedException("This method isn't yet implemented");
        }
        bool hasBegun = false;
        public bool AntiAlias { get; set; } = true;
        public int PendingElements { get; private set; } = 0;
        void Begin()
        {
            if (!hasBegun)
            {
                device.BeginScene();
            }
            hasBegun = true;
            PendingElements++;
        }
        public void Dispose()
        {
            device.Dispose();
            d3d.Dispose();
        }
        public void Clear(System.Drawing.Color color)
        {
            Begin();
            device.Clear(SlimDX.Direct3D9.ClearFlags.Target, color.ToArgb(), 0f, 1);
        }
        public void Flush()
        {
            if (PendingElements == 0)
                return;
            device.EndScene();
            device.Present();
            PendingElements = 0;
            hasBegun = false;
        }
        public System.Drawing.RectangleF MeasureString(string s, System.Drawing.Font f)
        {
            if (fCurrent.Height != f.Height || fCurrent.FontFamily != f.FontFamily)
            {
                font.Dispose();
                font = new SlimDX.Direct3D9.Font(device, f);
                fCurrent = f;
            }
            return font.MeasureString(null, s, SlimDX.Direct3D9.DrawTextFormat.Center);
        }

        public void ResetTransform()
        {
            transform.Reset();
        }

    }
    public class TransformationMatrix
    {
        MathNet.Numerics.LinearAlgebra.Matrix<float> matrix;
        public TransformationMatrix()
        {
            matrix = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseIdentity(3);
        }
        public void AddRotation(float angleInDegrees)
        {
            float r = (float)(angleInDegrees * Math.PI / 180.0F);

            var m = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Dense(3, 3);
            m[0, 0] = (float)Math.Cos(r);
            m[0, 1] = (float)-Math.Sin(r);
            m[0, 2] = 0;
            m[1, 0] = (float)Math.Sin(r);
            m[1, 1] = (float)Math.Cos(r);
            m[1, 2] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = 1;
            matrix = matrix * m;
        }
        public SlimDX.Vector2 Transform(SlimDX.Vector2 v2)
        {
            var v = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Dense(3, 1);
            v[0, 0] = v2.X;
            v[1, 0] = v2.Y;
            v[2, 0] = 1;
            var t = matrix * v;
            var vt = new SlimDX.Vector2(t[0, 0], t[1, 0]);
            return vt;
        }
        public void AddTranslation(float x, float y)
        {
            var m = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseIdentity(3);
            m[0, 2] = x;
            m[1, 2] = y;
            matrix = matrix * m;
        }
        public void AddScale(float xscale, float yscale)
        {
            var m = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseIdentity(3);
            m[0, 0] = xscale;
            m[1, 1] = yscale;
            matrix = matrix * m;
        }

        public void Reset()
        {
            matrix = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.DenseIdentity(3);
        }
    }
}

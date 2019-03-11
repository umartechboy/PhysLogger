using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FivePointNine.Numbers;

namespace PhysLogger
{
    public class LogControlBasic:Panel
    {
        public PlotOverLayObject UpLimitImage;
        public PlotOverLayObject DownLimitImage;
        public PlotOverLayObject RightLimitImage;

        protected FivePointNine.Windows.Graphics.Graphics2 g2;
        bool _DontScrollPlotOnReSize_ = false;
        public bool DontScrollPlotOnReSize
        {
            get { return _DontScrollPlotOnReSize_; }
            set
            {
                _DontScrollPlotOnReSize_ = value;
            }
        }
        public virtual List<float> TimeStamps { get { return ts; } }
        public float[] xOffsetG_ref = new float[1];
        public float[] xppu_ref = new float[1];
        public bool[] needsRefresh_ref = new bool[1];
        //public string YUnit { get; set; } = "Voltage (Volts)";
        public string XUnit { get; set; } = "Time (seconds)";
        ContextMenuStrip yScaleConextMenuStrip;
        ToolStripMenuItem YScaleAutoScale_MI;

        public LogControlBasic()
        {
            DoubleBuffered = true;
            SizeChanged += TimePlot_SizeChanged;
            MouseDown += TimePlot_MouseDown;
            MouseUp += TimePlot_MouseUp;
            MouseMove += TimePlot_MouseMove;
            AutoScroll = true;
            yScaleConextMenuStrip = new ContextMenuStrip();
            yScaleConextMenuStrip.Items.Add("Autoscale");
            YScaleAutoScale_MI = (ToolStripMenuItem)(yScaleConextMenuStrip.Items[0]);
            YScaleAutoScale_MI.Click += YScaleAutoScale_MI_Click;
            YScaleAutoScale_MI.Checked = AutoScale;
            FontChanged += LogControlBasic_FontChanged;
            LogControlBasic_FontChanged(null, null);
        }

        private void LogControlBasic_FontChanged(object sender, EventArgs e)
        {
            var g = Graphics.FromImage(new Bitmap(1, 1));
            var yLabelSz = g.MeasureString("Ay", Font);
            var xLabelSz = g.MeasureString("Ay", Font);
            YLabelWidth = yLabelSz.Height * 2;
            XLabelHeight = xLabelSz.Height * 2;
        }

        List<float> ts;
        public virtual void dsCollectionUpdated(TimeSeriesCollection dsCol)
        {
            ts = dsCol.TimeStamps;
        }

        private void YScaleAutoScale_MI_Click(object sender, EventArgs e)
        {
            AutoScale = !YScaleAutoScale_MI.Checked;
        }
        public void AttachD3D()
        {
            g2 = FivePointNine.Windows.Graphics.Graphics2.AttachD3D(this);
            needsRefresh = true;
        }
        public void Initialize(bool enableD3D)
        {
            XPPU = Width; // 1 sec per width
            YPPU = 0.5F; // 1 degree per pixel
            yOffsetG = Height / 2; // center of the screen
            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip_bkp = ContextMenuStrip;
        }
        public void resetXtoZero()
        {
            xOffsetG = DrawPlotArea.Width;
        }
        private void TimePlot_SizeChanged(object sender, EventArgs e)
        {
            if (!DontScrollPlotOnReSize)
            {
                resetXtoZero();
            }
            if (UpLimitImage != null)
            {
                UpLimitImage.Position = new Point(Width - (int)YLabelWidth - UpLimitImage.Width - 10, 10);
                DownLimitImage.Position = new Point(Width - (int)YLabelWidth - UpLimitImage.Width - 10, Height - DownLimitImage.Height - 10 - (int)XLabelHeight);
                RightLimitImage.Position = new Point(Width - RightLimitImage.Width - 10, Height - RightLimitImage.Height - (int)XLabelHeight);
            }
            needsRefresh = true;
        }
        MoveOp CurrentMoveOp = MoveOp.None;
        protected MoveOp TentativeOp = MoveOp.None;
        Point LastMouse = new Point();
        //protected PointF CursorV = new PointF();
        protected PointF CursorG = new PointF();
        protected bool showCursorX = false;
        protected bool showCursorY = false;
        Point MouseDownAt = new Point();
        protected TimeSeries HoverOver;
        ContextMenuStrip ContextMenuStrip_bkp;
        protected float YLabelWidth = 200, XLabelHeight = 100;
        protected RectangleF DrawPlotArea = new RectangleF();
        bool needsRefresh { get { return needsRefresh_ref[0]; } set { needsRefresh_ref[0] = value; } }

        //Size LastSize;
        protected float YPPU, yOffsetG;
        protected float xOffsetG
        {
            get { return xOffsetG_ref[0]; }
            set
            {
                xOffsetG_ref[0] = value;
            }
        }
        protected float XPPU { get { return xppu_ref[0]; } set { xppu_ref[0] = value; } }
        protected float xOffsetV { get { return xOffsetG / XPPU; } set { xOffsetG = value * XPPU; } }
        protected float yOffsetV { get { return yOffsetG / YPPU; } set { yOffsetG = value * YPPU; } }

        public virtual void Invalidate2()
        {
            if (g2 != null)
            {
                g2.ResetTransform();
                OnPaintD3D();
            }
            else
                Invalidate();
        }
        bool isInD3DPaint = false;
        public void OnPaintD3D()
        {
            if (isInD3DPaint)
                return;
            isInD3DPaint = true;
            g2.Clear(BackColor);
            DrawSeriesAndAxis(g2);
            g2.Flush();
            isInD3DPaint = false;
        }
        bool _ascale = true;
        public bool AutoScale
        {
            get { return _ascale; }
            internal set
            {
                _ascale = value;
                if (value) AutoSetScale(); Invalidate2();
                if (YScaleAutoScale_MI != null)
                    YScaleAutoScale_MI.Checked = value;
            }
        }

        public virtual void MinMaxAutoSetScaleMinMaxY(ref float minY, ref float maxY)
        { throw new NotImplementedException(); }
        protected float minYInDisplay = 0; float maxYInDisplay = 0;
        public void AutoSetScale()
        {
            float minY = 0, maxY = 0;
            MinMaxAutoSetScaleMinMaxY(ref minY, ref maxY);
            minYInDisplay = minYInDisplay * 0.9F + minY * 0.1F;
            maxYInDisplay = maxYInDisplay * 0.9F + maxY * 0.1F;
            if (minY == float.PositiveInfinity || maxY == float.NegativeInfinity || minY == maxY)
                return;
            float cover = 0.4F;
            yOffsetV = -(maxYInDisplay + minYInDisplay) / 2;
            yOffsetG += DrawPlotArea.Height / 2;
            if (maxYInDisplay - minYInDisplay <= 0)
                return;
            YPPU = DrawPlotArea.Height / (float)(maxYInDisplay - minYInDisplay) * cover;
        }

        public void DrawFullLengthExistingScale(Graphics g, int width, int height)
        {
            var g2 = FivePointNine.Windows.Graphics.Graphics2.FromGDI(g);
            var bkp = this.g2;
            this.g2 = null;
            DrawSeriesAndAxis(g2);
            this.g2 = bkp;
        }
        public void DrawTasksBeforeSeriesPlot(FivePointNine.Windows.Graphics.Graphics2 g)
        {
            if (AutoScale)
                AutoSetScale();
            DrawPlotArea = drawXYAxis(g, Width - YLabelWidth, Height - XLabelHeight, XPPU, YPPU, xOffsetG, yOffsetG, true, BackColor, Font);
        }
        protected virtual bool MaxValueOvershootInDisplay()
        { return false; }
        protected virtual bool MinValueOvershootInDisplay()
        { return false; }
        protected virtual bool TimeUndershootInDisplay()
        { return false; }
        protected virtual float MaxT()
        { throw new NotImplementedException(); }
        public void DrawTasksAfterSeriesPlot(FivePointNine.Windows.Graphics.Graphics2 g)
        {
            if (showCursorX)
            {
                var f = Font;
                var CursorV = GtoV(CursorG);
                var strSz = g.MeasureString(roundedFrac(CursorV.X, 3), f);
                g.FillRectangle(BackColor, CursorG.X, Height - XLabelHeight - strSz.Height, strSz.Width, strSz.Height);
                g.DrawString(
                    roundedFrac(CursorV.X, 3), 
                    f, 
                    Color.Black, 
                    new PointF(CursorG.X, Height - XLabelHeight - strSz.Height));
                g.DrawLine(Pens.Black, CursorG.X, 0, CursorG.X, Height - 20);
            }
            if (showCursorY)
            {
                var f = Font;
                var CursorV = GtoV(CursorG);
                var strSz = g.MeasureString(roundedFrac(CursorV.Y, 3), f);
                g.FillRectangle(BackColor, Width - strSz.Width - XLabelHeight, CursorG.Y, strSz.Width, strSz.Height);
                g.DrawString(
                    roundedFrac(CursorV.Y, 3), 
                    f, 
                    Color.Black,
                    new PointF(Width - strSz.Width - XLabelHeight, CursorG.Y));
                g.DrawLine(Pens.Black, 0, CursorG.Y, Width, CursorG.Y);
            }
            if (TimeUndershootInDisplay())
                RightLimitImage.Draw(g, TentativeOp == MoveOp.goToZero);
            if (MinValueOvershootInDisplay())
                DownLimitImage.Draw(g, TentativeOp == MoveOp.resetScale);
            if (MaxValueOvershootInDisplay())
                UpLimitImage.Draw(g, TentativeOp == MoveOp.resetScale);
        }
        public virtual void DrawSeriesAndAxis(FivePointNine.Windows.Graphics.Graphics2 g)
        {
            throw new NotImplementedException();
        }

        internal void NeedsInvalidate()
        {
            
        }

        public static RectangleF drawXYAxis(FivePointNine.Windows.Graphics.Graphics2 g, float w, float h, float xs, float ys, float xog, float yog, bool grid, Color backColor, Font Font)
        {
            RectangleF drawingRect = new RectangleF(0, 0, w, h);
            // X Axis
            var axisP = new Pen(Color.DarkGray, 1.5F);
            var majLine = new Pen(Color.FromArgb(155, 155, 155), 1F);
            var minLine = new Pen(Color.FromArgb(200, 200, 200), 1F);

            float unitX = 1e-8F;
            float multF = 5;
            // determine scale first
            while (unitX * xs < Font.Height * 2.2F)
            {
                unitX *= multF;
                multF = multF == 2 ? 5 : 2;
            }
            if (unitX < 1e-8 || unitX > 1e8)
                return drawingRect;
            
            float minX = 0, maxX = 0;
            while (minX * xs < -xog)
                minX += unitX;
            while (minX * xs > -xog)
                minX -= unitX;

            while (maxX * xs > w - xog)
                maxX -= unitX;
            while (maxX * xs < w - xog)
                maxX += unitX;

            Font f = new Font("ARIAL", Font.Height * 0.5F);
            int yaWid = f.Height * 4;
            int xaHei = (f.Height * 15 / 10);
            drawingRect = new RectangleF(0, 0, w - yaWid, h - xaHei);
            bool isMinLine = false;

            var xSigFiguresAfterD = 0;
            var totalFigs = (unitX/2 - Math.Floor(unitX / 2)).ToString().Length - 2;

            while (Math.Round(unitX, xSigFiguresAfterD) == Math.Round(unitX / 2, xSigFiguresAfterD) 
                && xSigFiguresAfterD <= totalFigs)
                xSigFiguresAfterD++;
            for (float i = minX; i <= maxX; i += unitX / 2)
            {
                PointF drawableMid = VtoG(new PointF(i, 0), xog / xs, xs, yog / ys, ys, h);
                drawableMid = new PointF(drawableMid.X, h);

                if (!isMinLine)
                {
                    PointF drawable1 = new PointF(drawableMid.X, drawableMid.Y - 1.5F);
                    PointF drawable2 = new PointF(drawableMid.X, drawableMid.Y + 1.5F);
                    if (grid) drawable1 = new PointF(drawable1.X, 0);
                    if (grid) drawable2 = new PointF(drawable2.X, h - xaHei);
                    string s = roundedFrac(i, xSigFiguresAfterD);
                    var xyo = g.MeasureString(s, f);
                    PointF drawableStrPos = new PointF(drawableMid.X - xyo.Width / 2, drawableMid.Y - xyo.Height - 2);
                    if (drawable1.X < w - yaWid && drawable1.X > 0)
                    {
                        g.DrawLine(majLine, drawable1, drawable2);
                        g.DrawString(s, f, Color.Gray, drawableStrPos);
                    }
                }
                else
                {
                    //PointF drawable1 = new PointF(drawableMid.X, drawableMid.Y - 1);
                    //PointF drawable2 = new PointF(drawableMid.X, drawableMid.Y + 1);

                    //if (grid) drawable1 = new PointF(drawable1.X, 0);
                    //if (grid) drawable2 = new PointF(drawable2.X, h - 15);

                    //if (drawable1.X < w - 30 && drawable1.X > 0)
                    //    g.DrawLine(minLine, drawable1, drawable2);
                }
                isMinLine = !isMinLine;
            }
            if (xog < w - yaWid && xog > 0)
                g.DrawLine(axisP, xog, 0, xog, h - 15);
            // Y Axis
            float unitY = 1e-8F; ;
            multF = 5;
            // determine scale first
            while (unitY * ys < Font.Height * 1.5F)
            {
                unitY *= multF;
                multF = multF == 2 ? 5 : 2;
            }
            if (unitY < 1e-7 || unitY > 1e7)
                return drawingRect;

            float minY = 0, maxY = 0;
            while (minY * ys < -yog)
                minY += unitY;
            while (minY * ys > -yog)
                minY -= unitY;

            while (maxY * ys > h - yog)
                maxY -= unitX;
            while (maxY * ys < h - yog)
                maxY += unitY;

            isMinLine = false;
            var ySigFiguresAfterD = 0;
            totalFigs = (unitY / 2 - Math.Floor(unitY / 2)).ToString().Length - 2;

            while (Math.Round(unitY, ySigFiguresAfterD) == Math.Round(unitY / 2, xSigFiguresAfterD)
                && ySigFiguresAfterD <= totalFigs)
                ySigFiguresAfterD++;
            for (float i = minY; i <= maxY; i += unitY / 2)
            {
                PointF drawableMid = VtoG(new PointF(0, i), xog / xs, xs, yog / ys, ys, h);
                drawableMid = new PointF(w, drawableMid.Y);

                if (!isMinLine)
                {
                    PointF drawable1 = new PointF(drawableMid.X - 1.5F, drawableMid.Y);
                    PointF drawable2 = new PointF(drawableMid.X + 1.5F, drawableMid.Y);
                    if (grid) drawable1 = new PointF(0, drawable1.Y);
                    if (grid) drawable2 = new PointF(w - yaWid, drawable2.Y);
                    string s = roundedFrac(i, ySigFiguresAfterD);
                    var xyo = g.MeasureString(s, f);
                    PointF drawableStrPos = new PointF(drawableMid.X - xyo.Width, drawableMid.Y - xyo.Height / 2 - 2);
                    if (drawable2.Y < h - xaHei && drawable2.Y > 0)
                    {
                        g.DrawLine(majLine, drawable1, drawable2);
                        g.DrawString(s, f, Color.Gray, drawableStrPos);
                    }
                }
                else
                {
                    //PointF drawable1 = new PointF(drawableMid.X - 1F, drawableMid.Y);
                    //PointF drawable2 = new PointF(drawableMid.X + 1F, drawableMid.Y);
                    //if (grid) drawable1 = new PointF(0, drawable1.Y);
                    //if (grid) drawable2 = new PointF(w - 30, drawable2.Y);
                    //if (drawable2.Y < h - 15 && drawable2.Y > 0)
                    //    g.DrawLine(minLine, drawable1, drawable2);
                }
                isMinLine = !isMinLine;
            }
            g.DrawLine(axisP, 0, h - yog, w - yaWid, h - yog);
            g.DrawRectangle(axisP, drawingRect);

            return drawingRect;
        }

        protected static float XVtoXG(float xV, float xov, float xs)
        {
            return (xV + xov) * xs;
        }
        protected static float YVtoYG(float yV, float yov, float ys, float h)
        {
            return h - (yV + yov) * ys;
        }

        protected static PointF VtoG(PointF pV, float xov, float xs, float yov, float ys, float h)
        {
            return new PointF(XVtoXG(pV.X, xov, xs), YVtoYG(pV.Y, yov, ys, h));
        }

        protected float XGtoXV(float xG, float xog, float xs)
        {
            return (xG - xog) / xs;
        }
        protected float YGtoYV(float yG, float yog, float ys, float h)
        {
            return ((h - yG) - yog) / ys;
        }
        protected PointF GtoV(PointF pG, float xog, float xs, float yog, float ys, float h)
        {
            return new PointF(XGtoXV(pG.X, xog, XPPU), YGtoYV(pG.Y, yog, YPPU, h));
        }
        protected PointF GtoV(PointF pG)
        {
            return GtoV(pG, xOffsetG, XPPU, yOffsetG, YPPU, Height - XLabelHeight);
        }
        protected static string roundedFrac(float frac, int significantFigures = 0, bool addPrefix = true)
        {
            if (frac == float.PositiveInfinity)
                return "Inf";
            else if (frac == float.NegativeInfinity)
                return "-Inf";
            float fracBkp = frac;
            if (frac < 1e-8 && frac > -1e-8)
                return "0";
            float lc = 1 / (float)Math.Pow(10, significantFigures);
            double thisFrac = bringAbove1(frac, significantFigures);
            double nextFrac = bringAbove1(frac + lc, significantFigures);
            int sigFigure = 1;
            while (thisFrac == nextFrac)
            {
                if (sigFigure > 5)
                    break;
                sigFigure++;
                thisFrac = bringAbove1(frac, sigFigure);
                nextFrac = bringAbove1(frac + lc, sigFigure);
            }
            string toReturn = "";
            if (addPrefix)
                toReturn =  NumberUtils.AddPrefix(thisFrac, "");
            else
                toReturn = thisFrac.ToString();
            return toReturn;
        }
        static double bringAbove1(float frac, int sigFigures = 1)
        {
            if (frac == float.PositiveInfinity || frac == float.NegativeInfinity)
                return 0;
            if (frac == 0)
                return 0;
            bool isNeg = frac < 0;
            if (isNeg) frac *= -1;
            int mp = 0;
            while (frac < 1)
            {
                frac *= 10;
                mp++;
            }
            if (Math.Floor(frac) == 9)
            {
                mp--;
                frac /= 10;
            }
            float mult = 1;
            while (mp-- > 0)
                mult *= 10;
            return (double)(Math.Round(frac * (isNeg ? -1 : 1) / mult, sigFigures));
        }

        PointF GAtMouseDown = new PointF();
        PointF VAtMouseDown = new PointF();
        PointF ScreenLoopOffset = new PointF();
        private void TimePlot_MouseMove(object sender, MouseEventArgs e)
        {
            CursorG = e.Location;
            Point eLocForSaving = e.Location;
            Point eLoc = new Point(e.Location.X + (int)ScreenLoopOffset.X, e.Location.Y + (int)ScreenLoopOffset.Y);
            showCursorX = false;
            showCursorY = false;
            if (CurrentMoveOp == MoveOp.None)
            {
                if (
                    (MaxValueOvershootInDisplay() && UpLimitImage.Contains(eLoc)) ||
                    (MinValueOvershootInDisplay() && DownLimitImage.Contains(eLoc))
                   )
                {
                    Cursor = Cursors.Hand;
                    TentativeOp = MoveOp.resetScale;
                    needsRefresh = true;
                    HoverOver = null;
                }
                else if (TimeUndershootInDisplay() && RightLimitImage.Contains(eLoc))
                {
                    Cursor = Cursors.Hand;
                    TentativeOp = MoveOp.goToZero;
                    needsRefresh = true;
                    HoverOver = null;
                }
                else if (eLoc.Y > DrawPlotArea.Height)
                {
                    Cursor = Cursors.SizeWE;
                    TentativeOp = MoveOp.xZoom;
                    needsRefresh = true;
                    showCursorX = true;
                    HoverOver = null;
                }
                else if (eLoc.X > DrawPlotArea.Width)
                {
                    Cursor = Cursors.SizeNS;
                    TentativeOp = MoveOp.yZoom;
                    needsRefresh = true;
                    showCursorY = true;
                    HoverOver = null;
                }
                else
                {
                    Cursor = Cursors.Default;
                    TentativeOp = MoveOp.xyPan;
                    var bkp = HoverOver;
                    HoverOver = CheckHover(GtoV(eLoc, xOffsetG, XPPU, yOffsetG, YPPU, Height - XLabelHeight), 10 / XPPU, 10 / YPPU);
                    if (HoverOver != null)
                        TentativeOp = MoveOp.selectSeries;
                    if (HoverOver != bkp)
                    {
                        needsRefresh = true;
                        if (HoverOver != null)
                        {
                            Cursor = Cursors.Help;
                        }
                    }
                }
            }
            else if (CurrentMoveOp == MoveOp.xZoom)
            {
                float totalShownV = Width / XPPU;
                float changeV = -(eLoc.X - LastMouse.X) / XPPU;
                float newTotalV = totalShownV + changeV;
                if (newTotalV < 0)
                    return;
                XPPU = Width / newTotalV;
                xOffsetG = GAtMouseDown.X - VAtMouseDown.X * XPPU;
                needsRefresh = true;
            }
            else if (CurrentMoveOp == MoveOp.yZoom)
            {
                float totalShownV = (Height - XLabelHeight) / YPPU;
                float changeV = (eLoc.Y - LastMouse.Y) / YPPU;
                float newTotalV = totalShownV + changeV;
                YPPU = (Height - XLabelHeight) / newTotalV;
                yOffsetG = ((Height - XLabelHeight) - GAtMouseDown.Y) - VAtMouseDown.Y * YPPU;
                needsRefresh = true;
            }
            else if (CurrentMoveOp == MoveOp.xyPan || CurrentMoveOp == MoveOp.selectSeries)
            {
                xOffsetG += (eLoc.X - LastMouse.X);
                yOffsetG += -(eLoc.Y - LastMouse.Y);

                needsRefresh = true;
            }

            if (CurrentMoveOp == MoveOp.xyPan || CurrentMoveOp == MoveOp.yZoom || CurrentMoveOp == MoveOp.xZoom || CurrentMoveOp == MoveOp.selectSeries)
            {
                int x = Cursor.Position.X;
                if (x + 1 >= Screen.PrimaryScreen.Bounds.Width)
                {
                    Cursor.Position = new Point(1, MousePosition.Y);
                    ScreenLoopOffset = new PointF(ScreenLoopOffset.X + Screen.PrimaryScreen.Bounds.Width, ScreenLoopOffset.Y);
                    eLocForSaving.X -= Screen.PrimaryScreen.Bounds.Width;
                }
                else if (x == 0)
                {
                    Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width - 2, MousePosition.Y);
                    ScreenLoopOffset = new PointF(ScreenLoopOffset.X - Screen.PrimaryScreen.Bounds.Width, ScreenLoopOffset.Y);
                    eLocForSaving.X += Screen.PrimaryScreen.Bounds.Width;
                }
                int y = Cursor.Position.Y;
                if (y + 1 >= Screen.PrimaryScreen.Bounds.Height)
                {
                    Cursor.Position = new Point(MousePosition.X, 1);
                    ScreenLoopOffset = new PointF(ScreenLoopOffset.X, ScreenLoopOffset.Y + Screen.PrimaryScreen.Bounds.Height);
                    eLocForSaving.Y -= Screen.PrimaryScreen.Bounds.Height;
                }
                else if (y == 0)
                {
                    Cursor.Position = new Point(MousePosition.X, Screen.PrimaryScreen.Bounds.Height - 2);
                    ScreenLoopOffset = new PointF(ScreenLoopOffset.Y, ScreenLoopOffset.Y - Screen.PrimaryScreen.Bounds.Height);
                    eLocForSaving.Y += Screen.PrimaryScreen.Bounds.Height;
                }
            }
            LastMouse = new Point(eLocForSaving.X + (int)ScreenLoopOffset.X, eLocForSaving.Y + (int)ScreenLoopOffset.Y);
        }
        public virtual TimeSeries CheckHover(PointF v, float xTol, float yTol)
        { throw new NotImplementedException(); }
        public void ForceHighlight(TimeSeries series)
        {
            HoverOver = series;
            TentativeOp = series == null ? MoveOp.None : MoveOp.selectSeries;
            MenuStripIsShowing = series;
        }
        protected TimeSeries MenuStripIsShowing = null;
        private void TimePlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CurrentMoveOp = TentativeOp;
                if (CurrentMoveOp == MoveOp.resetScale)
                    AutoSetScale();
                if (CurrentMoveOp == MoveOp.goToZero)
                {
                    if (TimeStamps.Count == 0)
                        xOffsetG = DrawPlotArea.Width;
                    else
                        xOffsetG = DrawPlotArea.Width - TimeStamps.Max() * XPPU;
                    needsRefresh = true;
                }
                if (CurrentMoveOp == MoveOp.xyPan)
                    Cursor = Cursors.NoMove2D;
                else if (CurrentMoveOp == MoveOp.yZoom)
                    Cursor = Cursors.NoMove2D;


                GAtMouseDown = e.Location;
                VAtMouseDown = GtoV(e.Location);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (TentativeOp == MoveOp.selectSeries)
                {
                    ContextMenuStrip = HoverOver.ContextMenuStrip;
                    ContextMenuStrip.Closed += ContextMenuStrip_Closed;
                    MenuStripIsShowing = HoverOver;
                }
                else if (TentativeOp == MoveOp.yZoom)
                    ContextMenuStrip = yScaleConextMenuStrip;
                else
                {
                    ContextMenuStrip_bkp = ContextMenuStrip;
                    ContextMenuStrip = null;
                }
            }
            MouseDownAt = LastMouse;
        }

        private void ContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            MenuStripIsShowing = null;
        }

        private void TimePlot_MouseUp(object sender, MouseEventArgs e)
        {
            CurrentMoveOp = MoveOp.None;
            TentativeOp = MoveOp.None;
            if (MenuStripIsShowing == null)
                HoverOver = null;
            ScreenLoopOffset = new PointF();
        }
        public void SetOverlayImages(Image upLimitWarning, Image downLimitWarning, Image rightLimitWarning, Image upLimitWarningLight, Image downLimitWarningLight, Image rightLimitWarningLight)
        {
            UpLimitImage = new PlotOverLayObject(upLimitWarning, upLimitWarningLight, 0, 0);
            DownLimitImage = new PlotOverLayObject(downLimitWarning, downLimitWarningLight, 0, 0);
            RightLimitImage = new PlotOverLayObject(rightLimitWarning, rightLimitWarningLight, 0, 0);
        }

    }
    public class PlotOverLayObject
    {
        Image img, imgLight;
        Rectangle r;

        public Point Position { set { r = new Rectangle(value.X, value.Y, img.Width, img.Height); } }
        public int Width { get { return img.Width; } }

        public int Height { get { return img.Height; } }

        public PlotOverLayObject(Image img, Image imgLight, int x, int y)
        {
            this.img = img;
            this.imgLight = imgLight;
            r = new Rectangle(x, y, img.Width, img.Height);
        }
        public void Draw(FivePointNine.Windows.Graphics.Graphics2 g, bool drawHighlighted)
        {
            g.DrawImage(drawHighlighted ? img : imgLight, r.X, r.Y);
        }

        public bool Contains(Point p)
        { return r.Contains(p); }
    }
    public enum MoveOp
    {
        None,
        xZoom,
        yZoom,
        xPan,
        yPan,
        xyPan,
        resetScale,
        goToZero,
        selectSeries

    }
}

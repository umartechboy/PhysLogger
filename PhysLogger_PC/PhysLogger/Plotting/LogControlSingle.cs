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
    public class LogControlSingle : LogControlBasic
    {
        public event EventHandler OnForcedInvalidate;
        public int ID { get; set; } = -1;
        public TimeSeries DataSeries { get { if (dsCollection_ != null) return dsCollection_[ID]; return null; } }
        TimeSeriesCollection dsCollection_;
        public LogControlSingle()
        {
        }

        public override void dsCollectionUpdated(TimeSeriesCollection dsCol)
        {
            base.dsCollectionUpdated(dsCol);
            dsCollection_ = dsCol;
        }
        public override void Invalidate2()
        {
            base.Invalidate2();
            //OnForcedInvalidate?.Invoke(this, new EventArgs());
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            //g.Clear(BackColor);
            var g2 = FivePointNine.Windows.Graphics.Graphics2.FromGDI(g);
            DrawTasksBeforeSeriesPlot(g2);
            DrawSeriesAndAxis(g2);
            DrawTasksAfterSeriesPlot(FivePointNine.Windows.Graphics.Graphics2.FromGDI(g));
        }
        public override void MinMaxAutoSetScaleMinMaxY(ref float minY, ref float maxY)
        {
            if (DataSeries == null)
                return;
            minY = DataSeries.MinMaxVinDisplay(true);
            maxY = DataSeries.MinMaxVinDisplay(false);
        }
        /// <summary>
        /// Assumes full sized sheet as input. Draws grid, points and axis
        /// </summary>
        public override void DrawSeriesAndAxis(FivePointNine.Windows.Graphics.Graphics2 g)
        {
            if (DataSeries == null) // only in the designer
                return;
            if (!DataSeries.Enabled)
                return;
            string YUnit = DataSeries.YUnits.ToString();
            var yLabelSz = g.MeasureString(YUnit, Font);
            var xLabelSz = g.MeasureString(XUnit, Font);
            YLabelWidth = yLabelSz.Height * 2;
            XLabelHeight = xLabelSz.Height * 2;
            g.DrawString(YUnit, Font, Color.Black, Width - YLabelWidth / 2 * 1.5F, (Height - XLabelHeight) / 2 + yLabelSz.Width / 2, -90);
            g.DrawString(XUnit, Font, Color.Black, (Width - YLabelWidth) / 2, Height - XLabelHeight * 1.5F / 2.0F);
            if (DataSeries != null)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clip = new Region(DrawPlotArea);
                DataSeries.Draw(g, (int)DrawPlotArea.Width, (int)DrawPlotArea.Height, xOffsetG, yOffsetG, XPPU, YPPU, false);
            }

            g.Clip = new Region(new RectangleF(0,0,Width, Height));
        }


        protected override bool TimeUndershootInDisplay()
        {
            if (dsCollection_ == null)
                return false;
            return dsCollection_.TimeStampsMax() * XPPU + xOffsetG > DrawPlotArea.Width + 5
                || dsCollection_.TimeStampsMax() * XPPU + xOffsetG < 0;
        }
        protected override bool MaxValueOvershootInDisplay()
        {
            if (dsCollection_ == null)
                return false;
            return DataSeries.MaxValueOvershootInDisplay(); }
        protected override bool MinValueOvershootInDisplay()
        {
            if (dsCollection_ == null)
                return false;
            return DataSeries.MinValueOvershootInDisplay(); }
        public override TimeSeries CheckHover(PointF v, float xTol, float yTol)
        {
            return DataSeries;
        }
    }
}

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
    public class LogControlOverlapping : LogControlBasic
    {
        public TimeSeriesCollection dsCollection
        {
            get { return dsCollection_; }
        }
        TimeSeriesCollection dsCollection_;
        public override void dsCollectionUpdated(TimeSeriesCollection dsCol)
        {
            base.dsCollectionUpdated(dsCol);
            dsCollection_ = dsCol;
        }
        public override void Invalidate2()
        {
            base.Invalidate2();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);

            if (dsCollection == null)
                return;
            foreach (var DataSeries in dsCollection.SeriesList)
                if (DataSeries == null)
                    return;

            if (AutoScale)
                AutoSetScale();
            DrawTasksBeforeSeriesPlot(FivePointNine.Windows.Graphics.Graphics2.FromGDI(g));
            DrawSeriesAndAxis(FivePointNine.Windows.Graphics.Graphics2.FromGDI(g));
            DrawTasksAfterSeriesPlot(FivePointNine.Windows.Graphics.Graphics2.FromGDI(g));
        }
        protected override bool MaxValueOvershootInDisplay()
        {
            foreach (var DataSeries in dsCollection.SeriesList.FindAll(s => s.Enabled))
                if (DataSeries.MaxValueOvershootInDisplay())
                    return true;
            return false;
        }
        protected override bool MinValueOvershootInDisplay()
        {
            foreach (var DataSeries in dsCollection.SeriesList.FindAll(s => s.Enabled))
                if (DataSeries.MinValueOvershootInDisplay())
                    return true;
            return false;
        }
        public override void MinMaxAutoSetScaleMinMaxY(ref float minY, ref float maxY)
        {
            if (dsCollection == null)
                return;
            minY = 0;
            maxY = 0;
            bool firstSeries = false;
            foreach (var DataSeries in dsCollection.SeriesList)
            {
                if (DataSeries == null)
                    return;
                if (DataSeries.Enabled)
                {
                    float min = DataSeries.MinMaxVinDisplay(true);
                    float max = DataSeries.MinMaxVinDisplay(false);
                    if (max > maxY || firstSeries)
                        maxY = max;
                    if (min < minY || firstSeries)
                        minY = min;
                    firstSeries = false;
                }
            }
        }

        public override void DrawSeriesAndAxis(FivePointNine.Windows.Graphics.Graphics2 g)
        {
            if (dsCollection == null)
                return;

            string YUnit = dsCollection[0].YUnits.Name;
            string unit = dsCollection[0].YUnits.Unit;
            bool noUnit = false;
            for (int i = 1; i < dsCollection.Count; i++)
            {
                if (!dsCollection[i].Enabled)
                    continue;
                if (unit != dsCollection[i].YUnits.Unit)
                {
                    noUnit = true;
                    break;
                }
                if (!YUnit.Contains(dsCollection[i].YUnits.Name))
                    YUnit += ", " + dsCollection[i].YUnits.Name;
            }
            if (noUnit)
                YUnit = "Error -- Incompatible units";
            else
                YUnit += " (" + dsCollection[0].YUnits.Unit + ")";


            var yLabelSz = g.MeasureString(YUnit, Font);
            var xLabelSz = g.MeasureString(XUnit, Font);
            YLabelWidth = yLabelSz.Height * 2;
            XLabelHeight = xLabelSz.Height * 2;
            g.Clip = new Region(DrawPlotArea);
            g.DrawString(YUnit, Font, Color.Black, Width - YLabelWidth / 2 * 1.5F, (Height - XLabelHeight) / 2 + yLabelSz.Width / 2, -90);
            g.DrawString(XUnit, Font, Color.Black, (Width - YLabelWidth) / 2, Height - XLabelHeight * 1.5F / 2.0F);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (var DataSeries in dsCollection.SeriesList)
            {
                if (DataSeries != null)
                {
                    if (DataSeries.Enabled)
                    {
                        if (DataSeries != HoverOver)
                        {
                            DataSeries.Draw(g, (int)DrawPlotArea.Width, (int)(Height - XLabelHeight), xOffsetG, yOffsetG, XPPU, YPPU, false);
                        }
                    }
                }
            }
            if (HoverOver != null)
                HoverOver.Draw(g, (int)DrawPlotArea.Width, (int)DrawPlotArea.Height, xOffsetG, yOffsetG, XPPU, YPPU, false);

            g.Clip = new Region(new RectangleF(0, 0, Width, Height));
        }

        protected override bool TimeUndershootInDisplay()
        {
            if (dsCollection_ == null)
                return false;
            return dsCollection_.TimeStampsMax() * XPPU + xOffsetG > DrawPlotArea.Width + 5
                || dsCollection_.TimeStampsMax() * XPPU + xOffsetG < 0;
        }
        public override TimeSeries CheckHover(PointF v, float xTol, float yTol)
        {
            if (MenuStripIsShowing != null)
                return MenuStripIsShowing;
            float[] scores = new float[dsCollection.Count];
            for (int ti = 0; ti < TimeStamps.Count; ti++)
            {
                if (TimeStamps[ti] >= v.X - xTol && TimeStamps[ti] <= v.X + xTol)
                {
                    int i = -1;
                    bool found = false;
                    foreach (var DataSeries in dsCollection.SeriesList)
                    {
                        i++;
                        scores[i] = float.PositiveInfinity;
                        if (DataSeries.Values[ti] > v.Y - yTol && DataSeries.Values[ti] < v.Y + yTol)
                        {
                            if (!DataSeries.Enabled)
                                continue;
                            found = true;
                            scores[i] = Math.Abs(DataSeries.Values[ti] - v.Y);
                        }
                    }
                    if (found)
                        return dsCollection[scores.ToList().IndexOf(scores.Min())];
                }
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PhysLogger.Hardware;

namespace PhysLogger
{

    public partial class LogPlotCascade4XUC : UserControl
    { 
        LogControlSingle[] SingleLogs;
        LogControlOverlapping OverLappingLogs;
        System.Timers.Timer d3DRefreshTimer;
        float[] xOffsetG_ref = new float[1];
        float[] XPPU_ref = new float[1];
        bool[] needsRefresh_ref = new bool[1];
        public bool needsResetLayout = true;
        bool d3DEnabled = false;
        public bool DontScrollPlotOnReSize
        {
            get { return OverLappingLogs.DontScrollPlotOnReSize; }
            set
            {
                foreach (var p in SingleLogs)
                    p.DontScrollPlotOnReSize = value;
                OverLappingLogs.DontScrollPlotOnReSize = value;
            }
        }
        public LogLayout LogLayout
        {
            get { return modeCascadeRB.Checked ? LogLayout.Cascade : LogLayout.Overlap; }
            set
            {
                modeCascadeRB.CheckedChanged -= modeRB_CheckedChanged;
                modeOverlapRB.CheckedChanged -= modeRB_CheckedChanged;
                modeCascadeRB.Checked = value == LogLayout.Cascade;
                modeOverlapRB.Checked = !modeCascadeRB.Checked;
                modeCascadeRB.CheckedChanged += modeRB_CheckedChanged;
                modeOverlapRB.CheckedChanged += modeRB_CheckedChanged;
            }
        }
        public TimeSeriesCollection dsCollection
        {
            get { return dsCollection_; }
        }
        TimeSeriesCollection dsCollection_;
        public void dsEnabledUpdated(int seriesIndex, bool enabled)
        {
            dsCollection_[seriesIndex].Enabled = enabled;
            ResetLogLayout();
            NeedsInvalidate();
        }
        public void dsCollectionUpdated(TimeSeriesCollection dsCol)
        {
            dsCollection_ = dsCol;
            foreach (var log in SingleLogs)
                log.dsCollectionUpdated(dsCol);
            OverLappingLogs.dsCollectionUpdated(dsCol);

        }
        bool needsRefresh { get { return needsRefresh_ref[0]; } set { needsRefresh_ref[0] = value; } }
        float xOffsetG { get { return xOffsetG_ref[0]; } set { xOffsetG_ref[0] = value; } }
        float XPPU { get { return XPPU_ref[0]; } }

        public LogPlotCascade4XUC()
        {
            InitializeComponent();
            modeOverlapRB.Left = Width - modeOverlapRB.Width - 10;
            modeCascadeRB.Left = modeOverlapRB.Left - modeCascadeRB.Width - 10;
            modeOverlapRB.Top = Height - modeOverlapRB.Height;
            modeCascadeRB.Top = Height - modeOverlapRB.Height;
            OverLappingLogs = new LogControlOverlapping();
            OverLappingLogs.Dock = DockStyle.Fill;

            SingleLogs = new LogControlSingle[] { log0, log1, log2, log3 };
            log0.ID = 0;
            log1.ID = 1;
            log2.ID = 2;
            log3.ID = 3;
            foreach (var log in SingleLogs)
            {
                log.xOffsetG_ref = xOffsetG_ref;
                log.xppu_ref = XPPU_ref;
                log.OnForcedInvalidate += Log_OnForcedInvalidate;
                log.SetOverlayImages(
                    Properties.Resources.UpLimitWarning, Properties.Resources.DownLimitWarning, Properties.Resources.RightLimitWarning,
                    Properties.Resources.UpLimitWarningLight, Properties.Resources.DownLimitWarningLight, Properties.Resources.RightLimitWarningLight);
                log.needsRefresh_ref = needsRefresh_ref;
                log.Initialize(d3DEnabled);
            }
            OverLappingLogs.xOffsetG_ref = xOffsetG_ref;
            OverLappingLogs.xppu_ref = XPPU_ref;
            OverLappingLogs.needsRefresh_ref = needsRefresh_ref;
            OverLappingLogs.Initialize(d3DEnabled);
            OverLappingLogs.SetOverlayImages(
                Properties.Resources.UpLimitWarning, Properties.Resources.DownLimitWarning, Properties.Resources.RightLimitWarning,
                Properties.Resources.UpLimitWarningLight, Properties.Resources.DownLimitWarningLight, Properties.Resources.RightLimitWarningLight);

            AutoScroll = true;
            Timer t = new Timer();
            t.Tick += T_Tick;
            t.Interval = 30;
            t.Enabled = true;
        }

        //This function needs to be called at only one time. No control reseizing is allowed afterwards if d3d is enabled
        public void AttemptToAttachD3D()
        {
            if (d3DEnabled)
                return;
            d3DEnabled = true;

            d3DRefreshTimer = new System.Timers.Timer();
            d3DRefreshTimer.Elapsed += T_Elapsed;
            d3DRefreshTimer.Interval = 10;
            d3DRefreshTimer.Enabled = true;
            foreach (var log in SingleLogs)
                log.AttachD3D();
            OverLappingLogs.AttachD3D();
        }
        public void NeedsInvalidate()
        {
            return;
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!needsRefresh)
                return;
            needsRefresh = false;
            if (LogLayout == LogLayout.Cascade)
                foreach (var log in SingleLogs)
                    log.Invalidate2();
            else
                OverLappingLogs.Invalidate2();
        }
        TimeSpan now = new TimeSpan(System.DateTime.Now.Ticks);
        bool isInTimer = false;
        private void T_Tick(object sender, EventArgs e)
        {
            if (isInTimer)
                return;
            isInTimer = true;
            if (needsResetLayout)
            {
                needsResetLayout = false;
                ResetLogLayout();
            }
            if (!d3DEnabled && needsRefresh)
            {
                var now2 = new TimeSpan(System.DateTime.Now.Ticks);
                if (LogLayout == LogLayout.Cascade)
                    foreach (var log in SingleLogs)
                        log.Invalidate2();
                else
                    OverLappingLogs.Invalidate2();
                //Application.DoEvents();
                var then = new TimeSpan(System.DateTime.Now.Ticks);
                var diff2 = (then - now2).TotalMilliseconds;
                var diff = (then - now).TotalMilliseconds;
            }
                now = new TimeSpan(System.DateTime.Now.Ticks);
            needsRefresh = false;
            isInTimer = false;
        }

        internal void SeriesChannelEditerHoverStart(int channelID)
        {
            OverLappingLogs.ForceHighlight(dsCollection[channelID]);
        }

        internal void SeriesChannelEditerHoverStop()
        {
            OverLappingLogs.ForceHighlight(null);
        }

        private void Log_OnForcedInvalidate(object sender, EventArgs e)
        {
            foreach (var log in SingleLogs)
            {
                if (sender != log)
                    log.Invalidate2();
                else
                    ;
            }
        }
        internal string GetSaveableString(bool addHeader)
        {
            return dsCollection.GetSaveableString(addHeader);
        }
        public Image GetImage(string title, string xl, LogLayout layout, List<TimeSeries> seriesList = null)
        {
            if (seriesList == null)
                seriesList = dsCollection.SeriesList;
            Font f = new Font("ARIAL", 12);
            int rangeXG = 0;
            if (dsCollection.TimeStamps.Count > 0)
                rangeXG = (int)Math.Round(dsCollection.TimeStamps.Max() * XPPU, 0);
            int rangeYG = Height;
            int p = 20;
            int yLabelMargin = 0,
                xLabelMargin = (int)measureString(xl, f).Width + p * 2;
            int legendLength = 0;
            float titleWidth = 0;
            Bitmap bmp;
            if (layout == LogLayout.Overlap)
            {
                int heightPerPlotWithTitleAndLabel = rangeYG + xLabelMargin;
                titleWidth = measureString(title, f).Width;
                foreach (TimeSeries s in seriesList)
                {
                    if (!s.Enabled) continue;
                    int len = ((int)measureString(s.Name, f).Height + p * 2) * 2;
                    if (len > yLabelMargin)
                        yLabelMargin = len;
                }
                legendLength = 100;
                yLabelMargin += legendLength;
                bmp = new Bitmap((int)Math.Max(rangeXG + yLabelMargin + legendLength + 2 * p, titleWidth), heightPerPlotWithTitleAndLabel);
                Graphics g = Graphics.FromImage(bmp);

                g.Clear(OverLappingLogs.BackColor);
                g.TranslateTransform(p, measureString(title, f).Height + 2 * p);
                OverLappingLogs.DrawFullLengthExistingScale(g, rangeXG, rangeYG);
                g.TranslateTransform(-p, -measureString(title, f).Height - 2 * p);
                g.Clip = new Region(new RectangleF(0, 0, bmp.Width, bmp.Height));
                drawCenterString(g, title, f, bmp.Width, new PointF(bmp.Width / 2, p));
                var xLabelLength = measureString(xl, f).Width;
                drawCenterString(g, xl, f, bmp.Width, new PointF(Math.Max((bmp.Width - yLabelMargin) / 2, p + xLabelLength / 2), bmp.Height - p - measureString(xl, f).Height));
                int total = seriesList.Sum(se => se.Enabled ? 1 : 0);
                int ind = 0;
                foreach (TimeSeries s in seriesList)
                {
                    if (!s.Enabled) continue;
                    drawCenterString(g, s.Name, f, yLabelMargin, new PointF(bmp.Width - yLabelMargin / 2, (bmp.Height - xLabelMargin) / 2 - total / 2 * 30 + ind * 30));
                    float h = measureString(s.Name, f).Height;
                    g.DrawLine(s.Pen,
                        new PointF(bmp.Width - yLabelMargin - legendLength + p, (bmp.Height - xLabelMargin) / 2 - total / 2 * 30 + ind * 30 + h / 2),
                        new PointF(bmp.Width - yLabelMargin + p, (bmp.Height - xLabelMargin) / 2 - total / 2 * 30 + ind * 30 + h / 2));
                    ind++;
                }
            }
            else
            {
                int totalEnabled = seriesList.Count(s => s.Enabled);
                rangeYG = Height / totalEnabled;
                int heightPerPlotWithTitleAndLabel = rangeYG + xLabelMargin;
                bmp = new Bitmap((int)Math.Max(rangeXG + yLabelMargin + legendLength + 2 * p, titleWidth), heightPerPlotWithTitleAndLabel * totalEnabled);

                Graphics g = Graphics.FromImage(bmp);

                g.Clear(BackColor);
                int yInd = 0;
                foreach (var log in SingleLogs)
                {
                    if (!log.DataSeries.Enabled) continue;
                    float Y = heightPerPlotWithTitleAndLabel * yInd;
                    g.TranslateTransform(0, Y);
                    // g is at 0,Y
                    g.TranslateTransform(p, measureString(log.DataSeries.Name, f).Height + 2 * p);
                    log.DrawFullLengthExistingScale(g, rangeXG, rangeYG);
                    g.TranslateTransform(-p, -measureString(log.DataSeries.Name, f).Height - 2 * p);
                    // g is at 0,Y

                    g.Clip = new Region(new RectangleF(0, 0, bmp.Width, heightPerPlotWithTitleAndLabel));
                    drawCenterString(g, log.DataSeries.Name, f, bmp.Width, new PointF(bmp.Width / 2, p));
                    var xLabelLength = measureString(xl, f).Width;
                    drawCenterString(g, xl, f, bmp.Width, new PointF(Math.Max((bmp.Width - yLabelMargin) / 2, p + xLabelLength / 2), heightPerPlotWithTitleAndLabel - p - measureString(xl, f).Height));

                    g.TranslateTransform(0, -Y);
                    yInd++;
                }
            }
            return bmp;
        }
        public static SizeF measureString(string s, Font f)
        {
            return Graphics.FromImage(new Bitmap(1, 1)).MeasureString(s, f);
        }
        public static void drawCenterString(Graphics g, string s, Font f, int w, PointF p)
        {
            var sz = g.MeasureString(s, f);
            g.DrawString(s, f, Brushes.Black, p.X - sz.Width / 2, p.Y);
        }
        DateTime lastDataPoint = DateTime.Now;
        public void AppendLog(float time, float[] dataPoints, bool ClearIfTimeSuspicious)
        {
            var changeX = dsCollection.AppendLog(
                time, dataPoints, 
                (float)(DateTime.Now - lastDataPoint).TotalMilliseconds / 1000.0F,
                ClearIfTimeSuspicious);
            if (AutoScroll)
            {
                if (dsCollection.TimeStamps.Count == 1)
                {
                    if (!modeCascadeRB.Checked) OverLappingLogs.resetXtoZero();
                    else SingleLogs.ToList().Find(s => s.Enabled).resetXtoZero();
                }
                xOffsetG -= changeX * XPPU;
            }
            needsRefresh = true;
            lastDataPoint = DateTime.Now;
        }
        public void ClearAll()
        {
            xOffsetG = Width - 50;
            Invalidate();
        }
        public void ResetLogLayout()
        {
            if (SingleLogs == null)
                return;

            var totalNull = SingleLogs.Sum(p => p.DataSeries == null ? 1 : 0);
            if (totalNull > 0)
                return;
            panel1.Controls.Clear();
            if (LogLayout == LogLayout.Cascade)
            {
                panel1.Controls.Add(scMain);
                var total = SingleLogs.Sum(p => p.DataSeries.Enabled ? 1 : 0);
                var totalA = (log0.DataSeries.Enabled ? 1 : 0) + (log1.DataSeries.Enabled ? 1 : 0);
                var totalB = (log2.DataSeries.Enabled ? 1 : 0) + (log3.DataSeries.Enabled ? 1 : 0);
                int aHeight = (panel1.Height * totalA) / total;
                int bHeight = (panel1.Height * totalA) / total;
                // set collapsing first
                scMain.Panel1Collapsed = totalA == 0;
                scMain.Panel2Collapsed = totalB == 0;

                scA.Panel1Collapsed = !log0.DataSeries.Enabled;
                scA.Panel2Collapsed = !log1.DataSeries.Enabled;
                scB.Panel1Collapsed = !log2.DataSeries.Enabled;
                scB.Panel2Collapsed = !log3.DataSeries.Enabled;

                // set sizes
                if (totalA > 0 && totalB > 0)
                    scMain.SplitterDistance = aHeight;
                if (!scMain.Panel1Collapsed)
                    scA.SplitterDistance = aHeight / 2;
                if (!scMain.Panel2Collapsed)
                    scB.SplitterDistance = bHeight / 2;
            }
            else
            {
                panel1.Controls.Add(OverLappingLogs);
                OverLappingLogs.Dock = DockStyle.Fill;
                scMain.Panel1Collapsed = false;
                scMain.Panel2Collapsed = true;
                scA.Panel1Collapsed = false;
                scA.Panel2Collapsed = true;
            }
        }

        private void modeRB_CheckedChanged(object sender, EventArgs e)
        {
            ResetLogLayout();
            needsRefresh = true;
        }

        private void autoScrollCB_CheckedChanged(object sender, EventArgs e)
        {
            AutoScroll = autoScrollCB.Checked;
        }
    }
    public enum LogLayout
    {
        Cascade,
        Overlap
    }
}

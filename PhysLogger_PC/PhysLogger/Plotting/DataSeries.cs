using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using FivePointNine.Graphics;
using FivePointNine.Numbers;
using PhysLogger.Hardware;
using PhysLogger.LogControls;

namespace PhysLogger
{
    public delegate void LegendChangeHandler(TimeSeries ts);
    public delegate Color GetColorHandler(object sender);

    public delegate List<float> GetTimeStampsHandler();
    public class TimeSeries
    {
        public ChannelOptionsCollection ChannelOptions { get; set; }

        public event GetTimeStampsHandler GetTimeStampsRequest;
        public event EventHandler InvalidateRequest;
        public event LegendChangeHandler OnVisualsChanged;
        AlwaysDisabledOption nameSubMenu;
        ChannelOption lineThicknessSubMenu;
        ChannelOption lineColorSubMenu;
        ChannelOption lineOpacitySubMenu;          
        public int ID { get; private set; }
        public ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return ChannelOptions.MenuStrip;
            }
        }
        string _ln = "";
        public string Name { get { return _ln; } set { _ln = value; if (nameSubMenu != null) nameSubMenu.Title = value; } }
        public List<float> Values { get; set; } = new List<float>();
        public List<float> TimeStamps { get { return GetTimeStampsRequest?.Invoke(); } }
        Color _lc = Color.Black;
        public Pen Pen
        {
            get
            {
                Color c = Color.FromArgb((int)(LineOpacity * 255), LineColor);
                return new Pen(c, LineThickness);
            }
            set
            {
                LineThickness = value.Width;
                LineColor = value.Color;
            }
        }
        public Color LineColor
        {
            get { return _lc; }
            set
            {
                _lc = value;
                foreach (LineColorOption item in lineColorSubMenu.SubOptions)
                {
                    var col = item.Color;
                    item.Checked = col.R == LineColor.R && col.G == LineColor.G && col.B == LineColor.B;
                }
                OnVisualsChanged?.Invoke(this);
            }
        }
        float _lt = 3;
        public TimeSeries(string name, int ID)
        {
            this.ID = ID;
            Name = name;
            ChannelOptions = new Hardware.ChannelOptionsCollection(ID);
            ChannelOptions.HardwareOptions.Enabled = false;
            PopulateChannelMenuOptions();
        }
        internal string Serialize()
        {
            return Serialize(this);
        }
        internal static string Serialize(TimeSeries s)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(s.Name);
            sb.AppendLine(s.Values.Count.ToString());
            foreach (var v in s.Values)
                sb.AppendLine(v.ToString());
            sb.AppendLine(s.LineColor.A.ToString() + "," + s.LineColor.R.ToString() + "," + s.LineColor.G.ToString() + "," + s.LineColor.B.ToString());
            sb.AppendLine(s.LineThickness.ToString());
            sb.AppendLine(s.LineOpacity.ToString());
            sb.AppendLine(s.LastPointHighlight.ToString());
            sb.AppendLine(s.Enabled.ToString());
            sb.AppendLine(s.minDispYCached.ToString());
            sb.AppendLine(s.maxDispYCached.ToString());
            return sb.ToString();
        }
        public static TimeSeries Deserialize(string [] str, ref int offset, int ID)
        {
            string n = str[offset]; offset++;
            TimeSeries s = new TimeSeries(n, ID);
            int c = Convert.ToInt32(str[offset]); offset++;
            s.Values = new List<float>();
            for (int i = 0; i < c; i++)
            {
                s.Values.Add(Convert.ToSingle(str[offset]));
                offset++;
            }
            var part = str[offset].Split(new char[] { ',' }); offset++;
            s.LineColor = Color.FromArgb(Convert.ToByte(part[0]), Convert.ToByte(part[1]), Convert.ToByte(part[2]), Convert.ToByte(part[3]));
            s.LineThickness = Convert.ToSingle(str[offset]); offset++;
            s.LineOpacity = Convert.ToSingle(str[offset]); offset++;
            s.LastPointHighlight = Convert.ToBoolean(str[offset]); offset++;
            s.Enabled = Convert.ToBoolean(str[offset]); offset++;
            try
            {
                s.minDispYCached = Convert.ToSingle(str[offset]);
            }
            catch { }
            offset++;
            try { s.maxDispYCached = Convert.ToSingle(str[offset]); }
            catch { } offset++;
            return s;
        }
        public void Adapt(TimeSeries series)
        {
            _lt = series.LineThickness;
            _lo = series.LineOpacity;
            _ln = series.Name;
            _lc = series.LineColor;
            _se = series.Enabled;
        }
        public float LineThickness
        {
            get { return _lt; }
            private set
            {
                _lt = value;
                foreach (LineThicknessOption item in lineThicknessSubMenu.SubOptions)
                {
                    item.Checked = item.Value == _lt;
                }
                OnVisualsChanged?.Invoke(this);
            }
        }
        float _lo = 0.9F;
        public float LineOpacity
        {
            get { return _lo; }
            set
            {
                _lo = value;

                foreach (LineOpacityOption item in lineOpacitySubMenu.SubOptions)
                {
                    item.Checked = item.Value == value;
                }
                OnVisualsChanged?.Invoke(this);
            }
        }
        public bool LastPointHighlight { get; set; } = false;
        bool _se = true;
        public bool Enabled
        {
            get { return _se; }
            set
            {
                _se = value;
                OnVisualsChanged?.Invoke(this);
            }
        }

        public PlotLabel YUnits { get; set; } = new PlotLabel("Voltage", "V");

        public void Invalidate()
        {
            InvalidateRequest?.Invoke(this, new EventArgs());   
        }
        /// <summary>
        /// This returns a cached copy of the min/max computed during the last Draw.
        /// </summary>
        /// <param name="giveMin"></param>
        /// <returns></returns>
        public float MinMaxVinDisplay(bool giveMin)
        {
            if (minDispYCached == float.PositiveInfinity || minDispYCached == float.NegativeInfinity ||
                maxDispYCached == float.PositiveInfinity || maxDispYCached == float.NegativeInfinity)
                return 0;
            if (giveMin)
                return minDispYCached;
            else
                return maxDispYCached;
        }
        public bool MaxValueOvershootInDisplay()
        { return maxDispValueOvershoot; }
        public bool MinValueOvershootInDisplay()
        {
            return minDispValueOvershoot;
        }
        float minDispYCached = float.PositiveInfinity;
        float maxDispYCached = float.NegativeInfinity;
        bool minDispValueOvershoot = false;
        bool maxDispValueOvershoot = false;
        internal void Draw(FivePointNine.Windows.Graphics.Graphics2 g, int width, int height, float xog, float yog, float xs, float ys, bool showHighlighted)
        {
            //minDispCached = float.PositiveInfinity;
            //maxDispCached = float.NegativeInfinity;
            if (!Enabled)
                return;
            try
            {
                minDispValueOvershoot = false;
                maxDispValueOvershoot = false;
                List<PointF> ps = new List<PointF>();
                if (Values.Count > 2)
                {
                    minDispYCached = Values[0];
                    maxDispYCached = Values[0];
                    float sumOfValues = 0;
                    float sumOfTimes = 0;
                    float summedValues = 0;
                    int lastAddedX = (int)Math.Round(TimeStamps[0] * xs) - 1;
                    for (int i = 0; i < Values.Count; i++)
                    {
                        var xToAdd = (int)Math.Round(TimeStamps[i] * xs);
                        summedValues++;
                        sumOfValues += Values[i];
                        sumOfTimes += TimeStamps[i];
                        if (xToAdd > lastAddedX)
                        {
                            float t = sumOfTimes / summedValues;
                            float v = sumOfValues / summedValues;

                            if (double.IsPositiveInfinity(v))
                                continue;
                            float vG = height - (v * ys + yog);
                            float tG = t * xs + xog;
                            if (tG >= 0 && tG <= width)
                            {
                                if (vG < 0)
                                    maxDispValueOvershoot = true;
                                if (vG > height)
                                    minDispValueOvershoot = true;

                                if (v < minDispYCached)
                                    minDispYCached = v;
                                if (v > maxDispYCached)
                                    maxDispYCached = v;
                                ps.Add(new PointF(tG, vG));
                            }

                            summedValues = 0;
                            sumOfTimes = 0;
                            sumOfValues = 0;
                            lastAddedX = xToAdd;
                        }
                    }
                }


                Color c = Color.FromArgb((int)(LineOpacity * 255), LineColor);
                Pen PlotP = new Pen(c, LineThickness);
                if (ps.Count > 1)
                {
                    if (showHighlighted)
                    {
                        g.DrawLines(new Pen(GraphicsUtils.GetContrast(PlotP.Color), PlotP.Width + 3), ps.ToArray());
                        g.DrawLines(new Pen(Color.FromArgb(255, PlotP.Color), LineThickness), ps.ToArray());
                    }
                    else
                    {
                        g.DrawLines(PlotP, ps.ToArray());
                    }
                }
                if (LastPointHighlight && ps.Count > 0)
                {
                    var p = ps[ps.Count - 1];
                    g.DrawRectangle(Pen, (int)(p.X - LineThickness * 2), (int)(p.Y - LineThickness * 2), LineThickness * 4, LineThickness * 4);
                }
            }
            catch (Exception ex)
            { }
        }

        private void LineOpacityItemClick(object sender, EventArgs e)
        {
            LineOpacity = Convert.ToSingle(((ToolStripItem)sender).Text.TrimEnd(new char[] { '%' })) / 100;
            Invalidate();
        }

        private void LineColorItemClick(object sender, EventArgs e)
        {
            LineColor = ((ToolStripItem)sender).BackColor;
            Invalidate();
        }

        private void LineThicknessItemClick(object sender, EventArgs e)
        {
            LineThickness = Convert.ToInt16(((ToolStripItem)sender).Text);
            Invalidate();
        }
        void PopulateChannelMenuOptions()
        {
            nameSubMenu = new AlwaysDisabledOption(Name);
            lineColorSubMenu = new GroupHeadOption("Color");
            lineThicknessSubMenu = new GroupHeadOption("Thickness");
            lineOpacitySubMenu = new GroupHeadOption("Opacity");
            ChannelOptions.SoftwareOptions.SubOptions.Add(nameSubMenu);
            ChannelOptions.SoftwareOptions.SubOptions.Add(lineColorSubMenu);
            ChannelOptions.SoftwareOptions.SubOptions.Add(lineThicknessSubMenu);
            ChannelOptions.SoftwareOptions.SubOptions.Add(lineOpacitySubMenu);
            ChannelOptions.SoftwareOptions.Enabled = true;
            for (int i = 1; i <= 8; i++)
            {
                var op = new LineThicknessOption(i);
                op.Checked = i == LineThickness;
                lineThicknessSubMenu.SubOptions.Add(op);
                op.MenuItem.Click += LineThicknessItemClick;
            }
            foreach (var col in GraphicsUtils.CommonColors())
            {
                var op = new LineColorOption(col);
                op.Checked = col.R == LineColor.R && col.G == LineColor.G && col.B == LineColor.B;
                lineColorSubMenu.SubOptions.Add(op);
                op.MenuItem.Click += LineColorItemClick;
            }
            for (int i = 10; i <= 100; i += 10)
            {
                var op = new LineOpacityOption(i / 100.0F);
                int t = (int)Math.Round(LineOpacity * 100);
                op.Checked = i == t;
                lineOpacitySubMenu.SubOptions.Add(op);
                op.MenuItem.Click += LineOpacityItemClick;
            }
        }

    }
    public class TimeSeriesCollection
    {
        public event GetColorHandler GetBackColorRequest;
        public event EventHandler InvalidateRequest;
        public List<float> TimeStamps { get; set; } = new List<float>();
        public Color BackColor { get { return GetBackColorRequest(this); } } 
        public bool UniqueXAxisStamps { get; set; } = false;
        public void Invalidate()
        { InvalidateRequest(this, new EventArgs()); }
        private List<TimeSeries> seriesList = new List<TimeSeries>();
        public List<TimeSeries> SeriesList { get { return seriesList; } }

        
        public TimeSeriesCollection()
        {
        }
        /// <summary>
        /// Initializes a collection with NoS number of new channels. 
        /// Each channel is given a unique name and an ID.
        /// </summary>
        /// <param name="NoS"></param>
        public TimeSeriesCollection(int NoS)
        {
            for (int i = 0; i < NoS; i++)
            {
                var ds = new TimeSeries("Channel " + (seriesList.Count + 1), seriesList.Count);
                ds.InvalidateRequest += Ds_InvalidateRequest;
                ds.GetTimeStampsRequest += Ds_GetTimeStampsRequest;
                ds.LineColor = GraphicsUtils.CommonColors()[seriesList.Count];
                if (seriesList.Count > 0)
                {
                    foreach (var dp in seriesList[0].Values)
                    {
                        ds.Values.Add(0);
                    }
                }
                seriesList.Add(ds);
            }
        }

        private List<float> Ds_GetTimeStampsRequest()
        {
            return TimeStamps;
        }

        private void Ds_InvalidateRequest(object sender, EventArgs e)
        {
            Invalidate();
        }

        float lastTime = -0.000001F;
        float controllerOffset = 0;
        bool resetControllerOffset = true;
        public virtual float AppendLog(float time, float[] values, float dt, bool ClearIfTimeSuspicious)
        {
            if (tMax < time)
                tMax = time;
            if (resetControllerOffset)
            {
                controllerOffset = time;
                resetControllerOffset = false;
                tMax = 0;
            }

            time -= controllerOffset;
            if (time < 0 || time < lastTime) // controller has reset
            {
                if (ClearIfTimeSuspicious)
                {
                    ClearAll();
                    return 0;
                }
                else
                {
                    time = lastTime + dt - controllerOffset;
                    tMax = time;
                }
            }
            if (seriesList.Count != values.Length)
                throw new Exception("Data size mismatch");
            for (int i = 0; i < values.Length; i++)
                seriesList[i].Values.Add(values[i]);
            if (TimeStamps.Count == 0)
                TimeStamps.Add(time);
            else
                TimeStamps.Add(time);
            var bkp = lastTime;
            lastTime = time;
            return time - bkp;
        }
       
        public string Serialize()
        {
            return Serialize(this);
        }
        public static string Serialize(TimeSeriesCollection ts)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ts.UniqueXAxisStamps.ToString());
            sb.AppendLine(ts.TimeStamps.Count.ToString());
            foreach (var t in ts.TimeStamps)
                sb.AppendLine(t.ToString());
            sb.AppendLine(ts.seriesList.Count.ToString());
            foreach (var s in ts.seriesList)
                sb.Append(s.Serialize());
            return sb.ToString();
        }
        public static TimeSeriesCollection Deserialize(string[] str)
        {
            TimeSeriesCollection ts = new TimeSeriesCollection();
            int offset = 0;
            ts.UniqueXAxisStamps = Convert.ToBoolean(str[offset]); offset++;
            var tsCount = Convert.ToInt32(str[offset]); offset++;
            ts.TimeStamps = new List<float>();
            for (int i = 0; i < tsCount; i++)
            {
                ts.TimeStamps.Add(Convert.ToSingle(str[offset]));
                offset++;
            }
            var seriesCount = Convert.ToInt32(str[offset]); offset++;
            ts.seriesList = new List<TimeSeries>();
            for (int i = 0; i < seriesCount; i++)
            {
                var s = TimeSeries.Deserialize(str, ref offset, ts.seriesList.Count);
                s.GetTimeStampsRequest += ts.Ds_GetTimeStampsRequest;
                ts.seriesList.Add(s);
            }
            return ts;
        }
        public void ClearAll()
        {
            foreach (var ds in seriesList)
            {
                ds.Values.Clear();
                lastTime = 0;
            }
            TimeStamps.Clear();

            resetControllerOffset = true;
        }
        public string GetSaveableString(bool addHeader)
        {
            StringBuilder sw = new StringBuilder();
            if (addHeader)
            {
                sw.Append("Sr_No_\ttime_s");
                foreach (var s in seriesList)
                {
                    if (s.Enabled)
                        sw.Append("\t" + s.Name.Replace(" ", "_").Replace(".", "_"));
                }
                sw.AppendLine();
            }
            int srNo = 1;
            int startInd = -1;
            for (int i = 0; i < TimeStamps.Count; i++)
            {
                if (addHeader)
                    sw.Append(srNo + "\t");
                if (startInd < 0)
                    startInd = i;

                sw.Append(Math.Round(TimeStamps[i], 3) + "\t");
                foreach (var s in seriesList)
                    if (s.Enabled)
                        sw.Append(Math.Round(s.Values[i], 2) + "\t");
                sw.AppendLine();
                srNo++;
            }
            return sw.ToString();
        }
        float tMax = 0;
        internal void TimeStampsMax(float v)
        {
            tMax = v;
        }
        internal float TimeStampsMax()
        {
            return tMax - controllerOffset;
        }

        public int Count { get { return seriesList.Count; } }

        /// <summary>
        /// returns the indexed time series.
        /// </summary>
        /// <param name="seriesInd"></param>
        /// <returns></returns>
        public TimeSeries this[int seriesInd]
        {
            get { return seriesList[seriesInd]; }
        }
    }
    public class HoverPoint
    {
        public HoverPoint(int arrayIndex, PointF value, TimeSeries series)
        {
            ArrayIndex = arrayIndex;
            Value = value;
            Series = series;
        }
        public int ArrayIndex;
        public PointF Value;
        public TimeSeries Series;
    }
}

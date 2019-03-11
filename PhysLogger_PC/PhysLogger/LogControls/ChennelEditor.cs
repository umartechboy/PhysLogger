using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysLogger
{
    public delegate ContextMenuStrip GetContextMenuStripHandler(ChannelEditor sender);
    public delegate void ChannelEditorChangeHandler(ChannelEditor sender);
    public delegate bool ChannelEnableChangeHandler(ChannelEditor sender, bool enable);
    public delegate void ChannelSeriesMouseEvent(int channelID);

    /// <summary>
    /// This control adapts to a given time series for one time. Doesn't automatically change to match series unless requseted to.
    /// </summary>
    public partial class ChannelEditor : UserControl
    {
        public event GetContextMenuStripHandler GetContextMenuRequest;
        public event ChannelSeriesMouseEvent MouseEntered;
        public event ChannelSeriesMouseEvent MouseLeft;
        public TimeSeries TimeSeries { get; set; }
        public event EventHandler TextUpdated;
        public event ChannelEnableChangeHandler EnableChanged;
        public int ID { get; private set; }
        public override ContextMenuStrip ContextMenuStrip
        {
            set
            {
                base.ContextMenuStrip = value;
                legendLinePanel1.ContextMenuStrip = value;
            }
        }

        /// <summary>
        /// Applies the characteristics of the series to this channel editor
        /// </summary>
        /// <param name="series"></param>
        internal void Adapt(TimeSeries series)
        {
            TimeSeries = series;
            textBox1.TextChanged -= textBox1_TextChanged;
            textBox1.Text = series.Name;
            textBox1.TextChanged += textBox1_TextChanged;
            checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
            checkBox1.Checked = series.Enabled;
            textBox1.Enabled = series.Enabled;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            legendLinePanel1.Pen = series.Pen;
            ContextMenuStrip = series.ContextMenuStrip;
            minMaxLabel1.Value = 0;
            if (series.Values.Count > 0)
            {
                minMaxLabel1.Min = series.Values.Min();
                minMaxLabel1.Max = series.Values.Max();
            }
            else
            {
                minMaxLabel1.Min = 0;
                minMaxLabel1.Max = 0;
            }
        }

        public ChannelEditor(int ID)
        {
            this.ID = ID;
            InitializeComponent();
            textBox1.Top = 0;
            BackColorChanged += EditableCheckBox_BackColorChanged;
            Content = Name;
            legendLinePanel1.Cursor = Cursors.Hand;
            legendLinePanel1.Click += LegendLinePanel1_Click;
        }

        private void LegendLinePanel1_Click(object sender, EventArgs e)
        {
            legendLinePanel1.ContextMenuStrip.Show(legendLinePanel1, 0, 0);
        }

        private void EditableCheckBox_BackColorChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = BackColor;
        }

        public bool Checked { get { return checkBox1.Checked; } }
        public string Content
        {
            get
            { return textBox1.Text; }
            set
            {
                textBox1.Text = value;
            }
        }

        float _minMaxLabel1Value_ = 0;
        public float minMaxLabel1Value
        {
            get { return _minMaxLabel1Value_; }
            set
            {
                _minMaxLabel1Value_ = value;
            }
        }
        public string minMaxLabel1Suffix { get; internal set; }
        public bool NeedsUpdate = true;
        private void EditableCheckBox_SizeChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextUpdated?.Invoke(this, e);
        }

        ContextMenuStrip contextBKP = null;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!EnableChanged.Invoke(this, checkBox1.Checked))
            {
                checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                checkBox1.Checked = true;
                checkBox1.CheckedChanged += checkBox1_CheckedChanged;
                return;
            }
            textBox1.Enabled = checkBox1.Checked;
            minMaxLabel1.Visible = checkBox1.Checked;
            timer1.Stop();
            timer1.Start();
            if (checkBox1.Checked)
                ContextMenuStrip = contextBKP;
            else
            {
                contextBKP = ContextMenuStrip;
                ContextMenuStrip = null;
            }
        }


        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            timer1.Stop();
            timer1.Start();
            checkBox1.Visible = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            checkBox1.Visible = false;
        }

        private void ChannelEditor_Load(object sender, EventArgs e)
        {
            Timer t = new Timer();
            t.Interval = 100;
            t.Tick += T_Tick;
            t.Enabled = true;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (NeedsUpdate)
            {
                minMaxLabel1.Value = minMaxLabel1Value;
                minMaxLabel1.Suffix = minMaxLabel1Suffix;
                NeedsUpdate = false;
            }
        }

        private void ChannelEditor_MouseEnter(object sender, EventArgs e)
        {
            ContextMenuStrip = GetContextMenuRequest(this);
            MouseEntered(ID);
        }
        
        private void ChannelEditor_MouseLeave(object sender, EventArgs e)
        {
            MouseLeft(ID);
        }
    }
}

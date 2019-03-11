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
    public partial class ChannelSelectCheckBox : UserControl
    {
        public TimeSeriesCollection dsCollection;
        public List<FivePointNine.Windows.Controls.ColoredCheckBox> cbList = new List<FivePointNine.Windows.Controls.ColoredCheckBox>();
        public ChannelSelectCheckBox()
        {
            InitializeComponent();
        }
        public TimeSeries[] Selected
        {
            get
            {
                List<TimeSeries> answer = new List<TimeSeries>();
                int ind = 0;
                foreach (var cb in cbList)
                {
                    if (!cb.Enabled) continue;
                    if (cb.Checked)
                        answer.Add(dsCollection.SeriesList[ind]);
                    ind++;
                }
                return answer.ToArray();
            }
        }
        void Reset()
        {
            foreach (var c in cbList)
                Controls.Remove(c);
            cbList.Clear();
            foreach (var series in dsCollection.SeriesList)
            {
                if (series.Enabled)
                {
                    int i = cbList.Count();
                    FivePointNine.Windows.Controls.ColoredCheckBox cb = new FivePointNine.Windows.Controls.ColoredCheckBox();
                    cb.BackColor = BackColor;

                    cb.CheckedColor = series.Pen.Color;
                    cb.CheckedColorIsLight = allCB.CheckedColorIsLight;
                    cb.CheckedTextColor = allCB.CheckedTextColor;

                    cb.UncheckedColor = allCB.UncheckedColor;
                    cb.UncheckedColorIsLight = allCB.UncheckedColorIsLight;
                    cb.UncheckedTextColor = allCB.UncheckedTextColor;
                    cb.Width = allCB.Width;
                    cb.Height = allCB.Height;
                    cb.Left = 0;
                    cb.Top = i * (allCB.Height + 3) + allCB.Height;
                    cbList.Add(cb);
                    Controls.Add(cb);
                }
            }
        }
        
    }
}

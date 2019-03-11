using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace PhysLogger
{
    public delegate void RecordingStateChangeHandler(bool state);
    public class RecordButton:Button
    {
        int maxt = 0;
        public void SetAutoTimerMS(int value)
        {
            maxt = value;
        }
        internal void Reset()
        {
            RecordingState = false;
            blinker.Enabled = false;
            Text = "Record";
            showRed = false;
            timeOutTimer.Stop();
            Invalidate();
        }
        public event RecordingStateChangeHandler RecordingStateChanged;
        public bool RecordingState { get; set; } = false;
        public bool showRed = true;
        Timer blinker;
        public RecordButton()
        {
            blinker = new Timer();
            blinker.Interval = 1000;
            blinker.Tick += Blinker_Tick;
            Click += RecordButton_Click;
            Text = "Record";
            FlatStyle = FlatStyle.Flat;
            TextAlign = ContentAlignment.MiddleRight;
        }
        

        public override string Text
        {
            get
            {
                return RecordingState ? "Stop" : "Record";
            }

            set
            {
            }
        }

        Timer timeOutTimer = new Timer();
        private void RecordButton_Click(object sender, EventArgs e)
        {
            RecordingStateChanged?.Invoke(!RecordingState);
            if (RecordingState == false)
            {
                RecordingState = true;
                blinker.Enabled = true;
                showRed = true;
                Text = "Stop";
                timeOutTimer.Interval = maxt;
                timeOutTimer.Tick += T_Tick;
                timeOutTimer.Start();
            }
            else
            {
                RecordingState = false;
                blinker.Enabled = false;
                Text = "Record";
            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (RecordingState)
                ((Timer)sender).Stop();

            RecordingStateChanged?.Invoke(!RecordingState);
            RecordingState = false;
            blinker.Enabled = false;
            Text = "Record";
        }

        private void Blinker_Tick(object sender, EventArgs e)
        {
            showRed = !showRed;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (RecordingState)
                pevent.Graphics.FillEllipse(showRed ? Brushes.Red : new SolidBrush(Color.Gray), 6, 6, Height - 12, Height - 12);
            else
                pevent.Graphics.FillEllipse(new SolidBrush(Color.Gray), 6, 6, Height - 12, Height - 12);
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysLogger
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            completeLogControl.DataPort = dataPort;
            dataPort.PacketReceived += completeLogControl.serialChannelControl1_PacketReceived;
        }
        protected override void OnLoad(EventArgs args)
        {
            Application.Idle += new EventHandler(OnLoaded);
        }

        public void OnLoaded(object sender, EventArgs args)
        {
            Application.Idle -= new EventHandler(OnLoaded);
            // rest of your code 
            // we need to work with resetting D3D FrameBuffer size, which couldn't be found in SlimDX.
            // Once this issue is resolved, we can use D3D.
            //completeLogControl.AttemptToAttachD3D();
            string f = "LastSession.plprj";

            if (!System.IO.File.Exists(f))
                f = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LastSession.plprj");

            if (System.IO.File.Exists(f))
            {
                var data = System.IO.File.ReadAllLines(f);
                TimeSeriesCollection ts = null;
                try
                {
                    ts = TimeSeriesCollection.Deserialize(data);
                    completeLogControl.UpdateChannelVisuals(TimeSeriesCollection.Deserialize(data));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("PhysLogger has met an unexpected error and must be restarted. \r\n\r\nAdditional Information: " + ex.ToString());
                    System.IO.File.Delete(f);
                    Close();
                }
            }

            completeLogControl.DontScrollPlotOnReSize = true;
            //completeLogControl.HW.AttachVirtualLogger1();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        
        private void closeB_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void minimizeB_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void windowedModeB_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }
        

        private void clearAllB_Click(object sender, EventArgs e)
        {
            completeLogControl.ClearAll();
        }

        private void dataPort_Connected(object sender, EventArgs e)
        {
            completeLogControl.ClearAll();
        }
        private void dataPort_Disconnected(object sender, EventArgs e)
        {
        }

        #region MenuStripCode
        OpenFileDialog ofd = new OpenFileDialog();
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Filter = "PhysLogger Project Files (*.plprj)|*.plprj";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            var data = System.IO.File.ReadAllLines(ofd.FileName);

            TimeSeriesCollection ts = null;
            try
            {
                completeLogControl.DataPort.Disconnect();
                ts = TimeSeriesCollection.Deserialize(data);
                try
                {
                    completeLogControl.dsCollectionForceSet = TimeSeriesCollection.Deserialize(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("PhysLogger has met an unexpected error. It will exit now. \r\n\r\nAdditional Information: " + ex.ToString());
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("The selected file could not be parsed as a valid PhysLogger Project File.\r\n\r\nAdditional Information: " + ex.Message);
            }
        }

        SaveFileDialog sfd = new SaveFileDialog();
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "PhysLogger Project Files (*.plprj)|*.plprj";
            var serialized = completeLogControl.dsCollection.Serialize();
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            System.IO.File.WriteAllText(sfd.FileName, serialized);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void allOverlappingImagesItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png";
            var img = completeLogControl.logControl.GetImage("Time Log of the enabled channels", "Time (s)", LogLayout.Overlap);
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            img.Save(sfd.FileName);
        }

        private void screenShotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScreenCapture sc = new ScreenCapture();
            // capture entire screen, and save it to a file
            sfd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png";
            Image img = sc.CaptureWindow(completeLogControl.Handle);
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            img.Save(sfd.FileName);
        }
        private void channelAOverlappingImagesItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png";
            int ind = ((ToolStripMenuItem)sender).Name.Substring(7, 1)[0] - 'A';
            var s = completeLogControl.dsCollection.SeriesList.FindAll(ps => ps.Enabled);
            var img = completeLogControl.logControl.GetImage("Time Log of the enabled channels", "Time (s)", LogLayout.Overlap, s);
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            img.Save(sfd.FileName);
        }
        
        private void allCascadingImagesItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png";
            var img = completeLogControl.logControl.GetImage("Time Log of the enabled channels", "Time (s)", LogLayout.Cascade);
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            img.Save(sfd.FileName);
        }

        private void channelsIndividualImagesItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png";
            int ind = ((ToolStripMenuItem)sender).Name.Substring(0, 1)[0] - 'A';
            var s = (new TimeSeries[] { completeLogControl.dsCollection.SeriesList[ind] }).ToList();
            var img = completeLogControl.logControl.GetImage("Time Log of the enabled channels", "Time (s)", LogLayout.Cascade, s);
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            img.Save(sfd.FileName);
        }

        private void individualImagesItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.bmp;*.jpg;*.png";
            
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            foreach (var s in completeLogControl.dsCollection.SeriesList)
            {
                if (!s.Enabled)
                    continue;
                var img = completeLogControl.logControl.GetImage("Time Log of the enabled channels", "Time (s)", LogLayout.Cascade, new TimeSeries[] { s }.ToList());

                var ext = System.IO.Path.GetExtension(sfd.FileName);
                var name = System.IO.Path.GetFileNameWithoutExtension(sfd.FileName) + "_" + s.Name + ext;
                var dir = System.IO.Path.GetDirectoryName(sfd.FileName);
                img.Save(System.IO.Path.Combine(dir,name));
            }
        }
        private void rawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "Text File(*.txt)|*.txt|All files(*.*)|*.*";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            System.IO.File.WriteAllText(sfd.FileName, completeLogControl.logControl.GetSaveableString(false));
        }

        private void withHeadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfd.Filter = "Text File(*.txt)|*.txt|All files(*.*)|*.*";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            System.IO.File.WriteAllText(sfd.FileName, completeLogControl.logControl.GetSaveableString(true));
        }

        private void rawToolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(completeLogControl.logControl.GetSaveableString(false));
        }

        private void withHeadersToolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(completeLogControl.logControl.GetSaveableString(true));
        }

        private void usingArduinoIDEToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void arduinoMega2560ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void arduinoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cH340SerialToUSBConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void wikiToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void recordB_Click(object sender, EventArgs e)
        {

        }
        #endregion

        private void completeLogControl_OnSessionLifeUpdated(float value)
        {
            sessionLifeL.Text = (int)value + "." + Math.Round((value - Math.Floor(value)) * 10, 0).ToString().Substring(0, 1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var serialized = completeLogControl.dsCollection.Serialize();
            try
            {
                System.IO.File.WriteAllText("LastSession.plprj", serialized);
            }
            catch
            {
                try { System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LastSession.plprj"), serialized); } catch { }
            }
        }

        private void completeLogControl_OnHWSignatureUpdate(object sender, EventArgs e)
        {
            flatNumericUpDown1.SetFloatList(completeLogControl.GetSupportedFrequencies(), 1);
            flatNumericUpDown1.ValueChanged += FlatNumericUpDown1_ValueChanged;
        }

        bool dontChangeSampleFrequency = false;
        private void FlatNumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!dontChangeSampleFrequency)
                completeLogControl.ChangeHWSamplingRate(Convert.ToSingle(flatNumericUpDown1.Value));
        }

        private void completeLogControl_OnHWSamplingRateChanged(float frequency)
        {
            dontChangeSampleFrequency = true;
            flatNumericUpDown1.ForceValue(frequency.ToString());
            flatNumericUpDown1.Unit = "Hz";
            dontChangeSampleFrequency = false;
        }

        private void dataPort_DevicesRefreshed(object sender, EventArgs e)
        {
            dataPort.AddAddress("Virtual PhysLogger 1.0");
        }

        private bool dataPort_OnVirtualChannelCheck(string address)
        {
            if (address.ToLower() == "Virtual PhysLogger 1.0".ToLower())
                return true;
            return false;
        }

        private bool dataPort_VirutualChannelConnected(string address)
        {
            if (address.ToLower() == "Virtual PhysLogger 1.0".ToLower())
                completeLogControl.AttachVirtualLogger1_0();
            return true;
        }
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;
        
        protected override void WndProc(ref Message m)
        {
            if (WindowState == FormWindowState.Normal)
            {
                if (m.Msg == 0x84)
                {  // Trap WM_NCHITTEST
                    Point pos = new Point(m.LParam.ToInt32());
                    pos = this.PointToClient(pos);
                    if (pos.Y < cCaption)
                    {
                        m.Result = (IntPtr)2;  // HTCAPTION
                        return;
                    }
                    if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                    {
                        m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                        return;
                    }
                }
            }
            base.WndProc(ref m);
        }
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
        }


        Point lastDragPoint = new Point();
        private void label8_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragginWindows)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    int wid = Width, hei = Height;
                    WindowState = FormWindowState.Normal;
                    Top = 0;
                    Width = wid - 100;
                    Height = hei - 100;
                    Left = lastDragPoint.X - Width / 2;
                    lastDragPoint = new Point(Width / 2, lastDragPoint.Y);
                }
                else if (WindowState == FormWindowState.Normal)
                {
                    if (Top + lastDragPoint.Y < 1)
                    {
                        WindowState = FormWindowState.Maximized;
                        isDragginWindows = false;
                    }
                }
                Left += e.X - lastDragPoint.X;
                Top += e.Y - lastDragPoint.Y;
            }
            else
                lastDragPoint = e.Location;
        }

        bool isDragginWindows = false;
        private void label8_MouseDown(object sender, MouseEventArgs e)
        {
            isDragginWindows = true;
        }

        private void label8_MouseUp(object sender, MouseEventArgs e)
        {
            isDragginWindows = false;
        }

        private void label8_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;
        }

        private bool dataPort_VirutualChannelDisconnected(string address)
        {
            ((PhysLogger.Hardware.PhysLogger1_0Virtual)completeLogControl.HW).Stop();
            return true;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PhysLogger.Hardware;
using PhysLogger.LogControls;

namespace PhysLogger
{
    public delegate void valueChangeHandler(float value);
    public partial class CompleteLogControl : UserControl
    {
        // system initialization param.
        int NoS = 4;

        //Shared resources
        /// <summary>
        /// HW is the hardware reflection. It is cleared at every disconnect. 
        /// The Logger should send a new HW def at the start or the system will ask for one at the recepetion of first packaet.
        /// </summary>
        public LoggerHardware HW { get; private set; }

        ChannelOptionsCollection[] ChannelOptions;
        public virtual event valueChangeHandler OnHWSamplingRateChanged;
        public event EventHandler OnHWSignatureUpdate;
        public event valueChangeHandler OnSessionLifeUpdated;

        internal void AttemptToAttachD3D()
        {
            logControl.AttemptToAttachD3D();
        }

        /// <summary>
        /// List of visual channel editors
        /// </summary>
        ChannelEditor[] ChannelEditors;
        TimeSeriesCollection dsCol;
        /// <summary>
        /// original dsCollection. Setting this will also reset the channel editors.
        /// This is called while initialization and loading from the saved file.
        /// </summary>
        public TimeSeriesCollection dsCollection
        {
            get
            {
                if (dsCol == null)
                    throw new Exception("dsCol called before init.");
                return dsCol;
            }
            set
            {
                SetDsCollection(value, false);
            }
        }
        public TimeSeriesCollection dsCollectionForceSet
        {
            set { SetDsCollection(value, true); }
        }
        void SetDsCollection(TimeSeriesCollection value, bool force)
        {
            if (dsCol != null && !force)
                return;
            dsCol = value;
            dsCol.GetBackColorRequest += DsCol_GetBackColorRequest;
            dsCol.InvalidateRequest += DsCol_InvalidateRequest;
            for (int i = 0; i < value.Count; i++)
            {
                ChannelEditors[i].Adapt(value[i]);
                // these two events are called by the context menu strip, whether on the plot or the channel editor.
                value[i].OnVisualsChanged += CompleteLogControl_OnVisualsChanged;
            }
            // this will automatically look for disabled series and hide the respective panels.
            resetChannelEditorTops();
            if (dsCollection.TimeStamps.Count > 0)
            {
                dsCol.TimeStampsMax(value.TimeStamps.Max());
                OnSessionLifeUpdated?.Invoke(value.TimeStampsMax());
            }
            else
            {
                value.TimeStampsMax(0);
                OnSessionLifeUpdated?.Invoke(0);
            }
            logControl.dsCollectionUpdated(dsCol);
            logControl.Invalidate();
        }
        public void UpdateChannelVisuals(TimeSeriesCollection value)
        {
            for (int i = 0; i < value.Count; i++)
            {
                ChannelEditors[i].Adapt(value[i]);
                dsCol[i].Adapt(value[i]);
            }
            // this will automatically look for disabled series and hide the respective panels.
            resetChannelEditorTops();
            logControl.Invalidate();
        }
        private void DsCol_InvalidateRequest(object sender, EventArgs e)
        {
            logControl.Invalidate();
        }

        private Color DsCol_GetBackColorRequest(object sender)
        {
            return BackColor;
        }

        /// <summary>
        /// Initializes the control with a fixed number of maximum supported channels. 
        /// The channels can later be disabled but NOT removed.
        /// </summary>
        public CompleteLogControl()
        {
            InitializeComponent();
            HW = new LoggerHardware();
            HW.OnSignatureUpdate += HW_OnSignatureUpdate;
            HW.OnDisconnectRequested += HW_OnDisconnectRequested;
            ChannelEditors = new ChannelEditor[NoS];
            for (int i = 0; i < NoS; i++)
            {
                ChannelEditor ecb = new ChannelEditor(i);
                // Do one time setup
                ecb.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                ecb.Width = Width - logControl.Width - 5;
                ecb.Height = 80;
                ecb.Left = Width - ecb.Width;
                ecb.Top = 5 + i * 100;
                ecb.TextUpdated += Ecb_TextUpdated;
                ecb.EnableChanged += Ecb_EnableChanged;
                ecb.GetContextMenuRequest += Ecb_GetContextMenuRequest;
                ecb.MouseEntered += Ecb_MouseEntered;
                ecb.MouseLeft += Ecb_MouseLeft;
                ChannelEditors[i] = ecb;
                this.Controls.Add(ecb);
            }
            dsCollection = new TimeSeriesCollection(NoS);
            // enable only the first two channels by default
            for (int i = 0; i < NoS; i++)
                dsCollection[i].Enabled = i < 2;
            ChannelOptions = new ChannelOptionsCollection[NoS];
            for (int i = 0; i < NoS; i++)
                ChannelOptions[i] = new ChannelOptionsCollection(i);
        }

        private void Ecb_MouseLeft(int channelID)
        {
            logControl.SeriesChannelEditerHoverStop();
        }

        private void Ecb_MouseEntered(int channelID)
        {
            logControl.SeriesChannelEditerHoverStart(channelID);
        }

        private ContextMenuStrip Ecb_GetContextMenuRequest(ChannelEditor sender)
        {
            for (int i = 0; i < ChannelEditors.Length; i++)
            {
                if (ChannelEditors[i] == sender)
                    return dsCollection.SeriesList[i].ContextMenuStrip;
            }
            return null;
        }

        private TimeSeriesCollection LogControl_GetdsCollectionRequest()
        {
            return dsCol;
        }

        public void AttachVirtualLogger1_0()
        {
            HW = new PhysLogger1_0Virtual();
            Logger_1_0Attached();
            OnHWSignatureUpdate.Invoke(HW, new EventArgs());
        }
        private void HW_OnSignatureUpdate(PhysLoggerHWSignature oldSignature, PhysLoggerHWSignature newSignature)
        {
            if (oldSignature == PhysLoggerHWSignature.Unknown)// new connection signature update
            {
                if (newSignature == PhysLoggerHWSignature.PhysLogger1_0)
                {
                    HW = new PhysLogger1_0HW();
                    Logger_1_0Attached();
                }
                else if (newSignature == PhysLoggerHWSignature.PhysLogger1_1)
                {
                    HW = new PhysLogger1_1HW();
                    Logger_1_1Attached();
                }
                else if (newSignature == PhysLoggerHWSignature.PhysLogger1_2)
                {
                    HW = new PhysLogger1_2HW();
                    Logger_1_2Attached();
                }
                OnHWSignatureUpdate.Invoke(HW, new EventArgs());
            }
            else
            {
                HW.SignatureReceived(newSignature);
            }
        }
        void Logger_1_0Attached()
        {
            HW.NewPointReceived += HW_NewPointReceived;
            HW.OnSignatureUpdate += HW_OnSignatureUpdate;
            HW.OnCommandSendRequest += HW_OnCommandSendRequest;
            HW.OnSamplingRateChanged += HW_OnSamplingRateChanged;
            HW.OnDisconnectRequested += HW_OnDisconnectRequested;
            var channelHWOpsHW = HW.EnumerateChannelOptions();
            // if (dsCollection.Count != channelHWOpsHW.Count)
            //    // This version doesn't support this loggerHW
            for (int i = 0; i < 4; i++)
            {
                dsCollection[i].ChannelOptions.HardwareOptions = channelHWOpsHW[i];
                dsCollection[i].ChannelOptions.HardwareOptions.TopParent = dsCollection[i].ChannelOptions;
                dsCollection[i].ChannelOptions.SoftwareOptions.TopParent = dsCollection[i].ChannelOptions;
            }
        }

        private void HW_OnDisconnectRequested(object sender, EventArgs e)
        {
            DataPort.Disconnect();
            HW = new LoggerHardware();
            HW.OnSignatureUpdate += HW_OnSignatureUpdate;
            HW.OnDisconnectRequested += HW_OnDisconnectRequested;
        }

        void Logger_1_2Attached()
        {
            Logger_1_1Attached();
        }
        void Logger_1_1Attached()
        {
            HW.NewPointReceived += HW_NewPointReceived;
            HW.OnSignatureUpdate += HW_OnSignatureUpdate;
            HW.OnCommandSendRequest += HW_OnCommandSendRequest;
            HW.OnSamplingRateChanged += HW_OnSamplingRateChanged;
            HW.OnDisconnectRequested += HW_OnDisconnectRequested;
            var channelHWOpsHW = HW.EnumerateChannelOptions();
            // if (dsCollection.Count != channelHWOpsHW.Count)
            //    // This version doesn't support this loggerHW
            for (int i = 0; i < 4; i++)
            {
                dsCollection[i].ChannelOptions.HardwareOptions = channelHWOpsHW[i];
                dsCollection[i].ChannelOptions.HardwareOptions.TopParent = dsCollection[i].ChannelOptions;
                dsCollection[i].ChannelOptions.SoftwareOptions.TopParent = dsCollection[i].ChannelOptions;
            }
        }
        public List<float> GetSupportedFrequencies()
        {
            return HW.SupportedFrequencies;
        }
        public void ChangeHWSamplingRate(float frequencyInHz)
        {
            HW.ChangeSamplingRate(frequencyInHz);
        }
        private void HW_OnSamplingRateChanged(float rate)
        {
            OnHWSamplingRateChanged(rate);
        }

        private void HW_OnCommandSendRequest(FivePointNine.Windows.IO.PacketCommandMini pc)
        {
            pc.SendCommand(DataPort.Channel);
        }

        public bool DontScrollPlotOnReSize
        {
            get { return logControl.DontScrollPlotOnReSize; }
            set { logControl.DontScrollPlotOnReSize = value; }
        }
        private void HW_NewPointReceived(float x, float[] values, PlotLabel [] units)
        {
            logControl.AppendLog(x, values, HW.Signature == PhysLoggerHWSignature.PhysLogger1_0 ? true : false);
            if (dsCollection.TimeStamps.Count > 0)
                OnSessionLifeUpdated?.Invoke(dsCollection.TimeStampsMax());
            for (int i = 0; i < values.Length; i++)
            {
                ChannelEditors[i].minMaxLabel1Value = values[i];
                ChannelEditors[i].minMaxLabel1Suffix = units[i].Unit.ToString();
                dsCollection[i].YUnits = units[i];
                ChannelEditors[i].NeedsUpdate = true;
            }
        }


        // this is called when a time series visual is changed (Context Menu Strip). 
        // We need to do two things,
        // update individual channel editors, change editor layouts.
        private void CompleteLogControl_OnVisualsChanged(TimeSeries ts)
        {
            ChannelEditors[ts.ID].Adapt(ts);
            resetChannelEditorTops();
        }
        

        int[] SupportedGains = new int[] { 1, 10, 200 };
        public FivePointNine.Windows.Controls.SerialChannelControl DataPort { get; set; }
        public void serialChannelControl1_PacketReceived(FivePointNine.Windows.IO.PacketCommandMini command)
        {
            HW.ParseCommand(command, DataPort.Channel);
        }
        public void serialChannelControl1_Disconnected(object sender, EventArgs e)
        { HW.Signature = PhysLoggerHWSignature.Unknown; }
        private ChannelEditor Ds_GetEditorRequest(TimeSeries sender)
        {
            return ChannelEditors[sender.ID];
        }
        
        private bool Ecb_EnableChanged(ChannelEditor channelEditor, bool enable)
        {
            if (ChannelEditors.ToList().Count(ce => ce.Checked) == 0)
                return false;
            channelEditor.TimeSeries.Enabled = enable;
            logControl.dsEnabledUpdated(ChannelEditors.ToList().IndexOf(channelEditor), enable);
            resetChannelEditorTops();
            return true;
        }

        private void resetChannelEditorTops()
        {
            ChannelEditors[0].Height = ChannelEditors[0].TimeSeries.Enabled ? 80 : 20;
            for (int i = 1; i < NoS; i++)
            {
                var last = ChannelEditors[i - 1];
                var current = ChannelEditors[i];
                if (current == null || last == null)
                    break;
                current.Top = last.Top + last.Height + 20;
                current.Height = current.TimeSeries.Enabled ? 80 : 20;
            }
            logControl.needsResetLayout = true;
        }

        private void Ecb_TextUpdated(object sender, EventArgs e)
        {
            dsCol[((ChannelEditor)sender).ID].Name = ((ChannelEditor)sender).Content;
        }        

        public void ClearAll()
        {
            dsCollection.ClearAll();
            logControl.ClearAll();
            OnSessionLifeUpdated?.Invoke(0);
            dsCol.TimeStampsMax(0);
        }
    }
}


namespace PhysLogger
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PhysLogger.TimeSeriesCollection timeSeriesCollection1 = new PhysLogger.TimeSeriesCollection();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.clearAllB = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withHeadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rawToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.withHeadersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.screenShotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installArduinoDrivresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateFirmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sessionLifeL = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.flatNumericUpDown1 = new FivePointNine.Windows.Controls.FlatComboBox();
            this.windowedModeB = new MagneticPendulum.Button2();
            this.minimizeB = new MagneticPendulum.Button2();
            this.dataPort = new FivePointNine.Windows.Controls.SerialChannelControl();
            this.closeB = new MagneticPendulum.Button2();
            this.completeLogControl = new PhysLogger.CompleteLogControl();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.panel2.Controls.Add(this.label8);
            this.panel2.Location = new System.Drawing.Point(228, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(654, 22);
            this.panel2.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(0, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(630, 23);
            this.label8.TabIndex = 0;
            this.label8.Text = "PhysLogger";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label8.DoubleClick += new System.EventHandler(this.label8_DoubleClick);
            this.label8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label8_MouseDown);
            this.label8.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label8_MouseMove);
            this.label8.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label8_MouseUp);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(169, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "Time Trajectory";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(214, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 20);
            this.label5.TabIndex = 2;
            this.label5.Text = "Phase Portrait";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // clearAllB
            // 
            this.clearAllB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearAllB.BackColor = System.Drawing.Color.Gainsboro;
            this.clearAllB.FlatAppearance.BorderSize = 0;
            this.clearAllB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearAllB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearAllB.Location = new System.Drawing.Point(742, 480);
            this.clearAllB.Name = "clearAllB";
            this.clearAllB.Size = new System.Drawing.Size(150, 32);
            this.clearAllB.TabIndex = 21;
            this.clearAllB.Text = "Clear All";
            this.clearAllB.UseVisualStyleBackColor = false;
            this.clearAllB.Click += new System.EventHandler(this.clearAllB_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(925, 24);
            this.menuStrip1.TabIndex = 24;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.fileToolStripMenuItem.Text = "Project";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawDataToolStripMenuItem,
            this.copyToClipboardToolStripMenuItem1,
            this.screenShotToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // rawDataToolStripMenuItem
            // 
            this.rawDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawToolStripMenuItem,
            this.withHeadersToolStripMenuItem});
            this.rawDataToolStripMenuItem.Name = "rawDataToolStripMenuItem";
            this.rawDataToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.rawDataToolStripMenuItem.Text = "Data File";
            // 
            // rawToolStripMenuItem
            // 
            this.rawToolStripMenuItem.Name = "rawToolStripMenuItem";
            this.rawToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.rawToolStripMenuItem.Text = "Raw";
            this.rawToolStripMenuItem.Click += new System.EventHandler(this.rawToolStripMenuItem_Click);
            // 
            // withHeadersToolStripMenuItem
            // 
            this.withHeadersToolStripMenuItem.Name = "withHeadersToolStripMenuItem";
            this.withHeadersToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.withHeadersToolStripMenuItem.Text = "With Headers";
            this.withHeadersToolStripMenuItem.Click += new System.EventHandler(this.withHeadersToolStripMenuItem_Click);
            // 
            // copyToClipboardToolStripMenuItem1
            // 
            this.copyToClipboardToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawToolStripMenuItem1,
            this.withHeadersToolStripMenuItem1});
            this.copyToClipboardToolStripMenuItem1.Name = "copyToClipboardToolStripMenuItem1";
            this.copyToClipboardToolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
            this.copyToClipboardToolStripMenuItem1.Text = "Copy to Clipboard";
            // 
            // rawToolStripMenuItem1
            // 
            this.rawToolStripMenuItem1.Name = "rawToolStripMenuItem1";
            this.rawToolStripMenuItem1.Size = new System.Drawing.Size(145, 22);
            this.rawToolStripMenuItem1.Text = "Raw";
            this.rawToolStripMenuItem1.Click += new System.EventHandler(this.rawToolStripMenuItemCopy_Click);
            // 
            // withHeadersToolStripMenuItem1
            // 
            this.withHeadersToolStripMenuItem1.Name = "withHeadersToolStripMenuItem1";
            this.withHeadersToolStripMenuItem1.Size = new System.Drawing.Size(145, 22);
            this.withHeadersToolStripMenuItem1.Text = "With Headers";
            this.withHeadersToolStripMenuItem1.Click += new System.EventHandler(this.withHeadersToolStripMenuItemCopy_Click);
            // 
            // screenShotToolStripMenuItem
            // 
            this.screenShotToolStripMenuItem.Name = "screenShotToolStripMenuItem";
            this.screenShotToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.screenShotToolStripMenuItem.Text = "Take Screen Shot";
            this.screenShotToolStripMenuItem.Click += new System.EventHandler(this.screenShotToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installArduinoDrivresToolStripMenuItem,
            this.updateFirmwareToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // installArduinoDrivresToolStripMenuItem
            // 
            this.installArduinoDrivresToolStripMenuItem.Name = "installArduinoDrivresToolStripMenuItem";
            this.installArduinoDrivresToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.installArduinoDrivresToolStripMenuItem.Text = "Install Arduino Drivers";
            this.installArduinoDrivresToolStripMenuItem.Click += new System.EventHandler(this.installArduinoDrivresToolStripMenuItem_Click);
            // 
            // updateFirmwareToolStripMenuItem
            // 
            this.updateFirmwareToolStripMenuItem.Name = "updateFirmwareToolStripMenuItem";
            this.updateFirmwareToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.updateFirmwareToolStripMenuItem.Text = "Update Firmware";
            this.updateFirmwareToolStripMenuItem.Click += new System.EventHandler(this.updateFirmwareToolStripMenuItem_Click);
            // 
            // sessionLifeL
            // 
            this.sessionLifeL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sessionLifeL.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sessionLifeL.Location = new System.Drawing.Point(761, 424);
            this.sessionLifeL.Name = "sessionLifeL";
            this.sessionLifeL.Size = new System.Drawing.Size(131, 37);
            this.sessionLifeL.TabIndex = 25;
            this.sessionLifeL.Text = "0";
            this.sessionLifeL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(639, 543);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(195, 32);
            this.label2.TabIndex = 22;
            this.label2.Text = "Sampling Frequency";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(893, 426);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 37);
            this.label3.TabIndex = 25;
            this.label3.Text = "s";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(644, 427);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 32);
            this.label4.TabIndex = 22;
            this.label4.Text = "Session Life";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flatNumericUpDown1
            // 
            this.flatNumericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flatNumericUpDown1.Increment = 1;
            this.flatNumericUpDown1.Location = new System.Drawing.Point(840, 535);
            this.flatNumericUpDown1.Name = "flatNumericUpDown1";
            this.flatNumericUpDown1.Size = new System.Drawing.Size(70, 49);
            this.flatNumericUpDown1.TabIndex = 27;
            this.flatNumericUpDown1.Unit = "";
            // 
            // windowedModeB
            // 
            this.windowedModeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowedModeB.BackgroundImage = global::PhysLogger.Properties.Resources.NormalDim;
            this.windowedModeB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.windowedModeB.FlatAppearance.BorderSize = 0;
            this.windowedModeB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.windowedModeB.HoverBackgroundImage = global::PhysLogger.Properties.Resources.NormalHighlight;
            this.windowedModeB.Location = new System.Drawing.Point(881, 1);
            this.windowedModeB.Name = "windowedModeB";
            this.windowedModeB.Size = new System.Drawing.Size(22, 22);
            this.windowedModeB.TabIndex = 17;
            this.windowedModeB.UseVisualStyleBackColor = true;
            this.windowedModeB.Click += new System.EventHandler(this.windowedModeB_Click);
            // 
            // minimizeB
            // 
            this.minimizeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeB.BackgroundImage = global::PhysLogger.Properties.Resources.MinimizeDim;
            this.minimizeB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.minimizeB.FlatAppearance.BorderSize = 0;
            this.minimizeB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeB.HoverBackgroundImage = global::PhysLogger.Properties.Resources.CloseHighlighte;
            this.minimizeB.Location = new System.Drawing.Point(859, 1);
            this.minimizeB.Name = "minimizeB";
            this.minimizeB.Size = new System.Drawing.Size(22, 22);
            this.minimizeB.TabIndex = 17;
            this.minimizeB.UseVisualStyleBackColor = true;
            this.minimizeB.Click += new System.EventHandler(this.minimizeB_Click);
            // 
            // dataPort
            // 
            this.dataPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dataPort.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.dataPort.DefaultDTR = true;
            this.dataPort.ID = ((byte)(0));
            this.dataPort.Location = new System.Drawing.Point(728, 595);
            this.dataPort.MinimumSize = new System.Drawing.Size(170, 51);
            this.dataPort.Name = "dataPort";
            this.dataPort.PingDuration = 1000;
            this.dataPort.PingEnabled = false;
            this.dataPort.ReceiveEnabled = true;
            this.dataPort.ShowDTR = false;
            this.dataPort.Size = new System.Drawing.Size(185, 51);
            this.dataPort.TabIndex = 23;
            this.dataPort.Connected += new System.EventHandler(this.dataPort_Connected);
            this.dataPort.VirutualChannelConnected += new FivePointNine.Windows.Controls.CheckIfAddressIsVirtual(this.dataPort_VirutualChannelConnected);
            this.dataPort.VirutualChannelDisconnected += new FivePointNine.Windows.Controls.CheckIfAddressIsVirtual(this.dataPort_VirutualChannelDisconnected);
            this.dataPort.Disconnected += new System.EventHandler(this.dataPort_Disconnected);
            this.dataPort.DevicesRefreshed += new System.EventHandler(this.dataPort_DevicesRefreshed);
            this.dataPort.OnVirtualChannelCheck += new FivePointNine.Windows.Controls.CheckIfAddressIsVirtual(this.dataPort_OnVirtualChannelCheck);
            // 
            // closeB
            // 
            this.closeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeB.BackgroundImage = global::PhysLogger.Properties.Resources.CloseDim;
            this.closeB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.closeB.FlatAppearance.BorderSize = 0;
            this.closeB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeB.HoverBackgroundImage = global::PhysLogger.Properties.Resources.CloseHighlighted;
            this.closeB.Location = new System.Drawing.Point(903, 0);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(22, 22);
            this.closeB.TabIndex = 16;
            this.closeB.UseVisualStyleBackColor = true;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // completeLogControl
            // 
            this.completeLogControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.completeLogControl.DataPort = null;
            this.completeLogControl.DontScrollPlotOnReSize = false;
            timeSeriesCollection1.TimeStamps = ((System.Collections.Generic.List<float>)(resources.GetObject("timeSeriesCollection1.TimeStamps")));
            timeSeriesCollection1.UniqueXAxisStamps = false;
            this.completeLogControl.dsCollection = timeSeriesCollection1;
            this.completeLogControl.Location = new System.Drawing.Point(12, 28);
            this.completeLogControl.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.completeLogControl.Name = "completeLogControl";
            this.completeLogControl.Size = new System.Drawing.Size(901, 618);
            this.completeLogControl.TabIndex = 15;
            this.completeLogControl.OnHWSamplingRateChanged += new PhysLogger.valueChangeHandler(this.completeLogControl_OnHWSamplingRateChanged);
            this.completeLogControl.OnHWSignatureUpdate += new System.EventHandler(this.completeLogControl_OnHWSignatureUpdate);
            this.completeLogControl.OnSessionLifeUpdated += new PhysLogger.valueChangeHandler(this.completeLogControl_OnSessionLifeUpdated);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.ClientSize = new System.Drawing.Size(925, 658);
            this.Controls.Add(this.flatNumericUpDown1);
            this.Controls.Add(this.windowedModeB);
            this.Controls.Add(this.minimizeB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sessionLifeL);
            this.Controls.Add(this.dataPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clearAllB);
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.completeLogControl);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(792, 658);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseMove);
            this.panel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        private CompleteLogControl completeLogControl;
        private MagneticPendulum.Button2 closeB;
        private MagneticPendulum.Button2 minimizeB;
        private FivePointNine.Windows.Controls.SerialChannelControl dataPort;
        private System.Windows.Forms.Button clearAllB;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withHeadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem rawToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem withHeadersToolStripMenuItem1;
        private System.Windows.Forms.Label sessionLifeL;
        private System.Windows.Forms.Label label2;
        private MagneticPendulum.Button2 windowedModeB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private FivePointNine.Windows.Controls.FlatComboBox flatNumericUpDown1;
        private System.Windows.Forms.ToolStripMenuItem screenShotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installArduinoDrivresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateFirmwareToolStripMenuItem;
    }
}


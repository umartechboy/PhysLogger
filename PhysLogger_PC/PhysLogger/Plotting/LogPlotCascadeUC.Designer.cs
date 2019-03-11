namespace PhysLogger
{
    partial class LogPlotCascade4XUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.scMain = new FivePointNine.Windows.Controls.SplitContainer2();
            this.scA = new FivePointNine.Windows.Controls.SplitContainer2();
            this.log0 = new PhysLogger.LogControlSingle();
            this.log1 = new PhysLogger.LogControlSingle();
            this.scB = new FivePointNine.Windows.Controls.SplitContainer2();
            this.log2 = new PhysLogger.LogControlSingle();
            this.log3 = new PhysLogger.LogControlSingle();
            this.modeCascadeRB = new System.Windows.Forms.RadioButton();
            this.modeOverlapRB = new System.Windows.Forms.RadioButton();
            this.autoScrollCB = new FivePointNine.Windows.Controls.FlatCheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scA)).BeginInit();
            this.scA.Panel1.SuspendLayout();
            this.scA.Panel2.SuspendLayout();
            this.scA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scB)).BeginInit();
            this.scB.Panel1.SuspendLayout();
            this.scB.Panel2.SuspendLayout();
            this.scB.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.scMain);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(596, 520);
            this.panel1.TabIndex = 0;
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 0);
            this.scMain.Name = "scMain";
            this.scMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.scA);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.scB);
            this.scMain.Size = new System.Drawing.Size(596, 520);
            this.scMain.SplitterDistance = 221;
            this.scMain.TabIndex = 0;
            // 
            // scA
            // 
            this.scA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scA.Location = new System.Drawing.Point(0, 0);
            this.scA.Name = "scA";
            this.scA.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scA.Panel1
            // 
            this.scA.Panel1.Controls.Add(this.log0);
            // 
            // scA.Panel2
            // 
            this.scA.Panel2.Controls.Add(this.log1);
            this.scA.Size = new System.Drawing.Size(596, 221);
            this.scA.SplitterDistance = 94;
            this.scA.TabIndex = 0;
            // 
            // log0
            // 
            this.log0.AutoScroll = true;
            this.log0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log0.DontScrollPlotOnReSize = false;
            this.log0.ID = -1;
            this.log0.Location = new System.Drawing.Point(0, 0);
            this.log0.Name = "log0";
            this.log0.Size = new System.Drawing.Size(596, 94);
            this.log0.TabIndex = 0;
            this.log0.XUnit = "Time (seconds)";
            // 
            // log1
            // 
            this.log1.AutoScroll = true;
            this.log1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log1.DontScrollPlotOnReSize = false;
            this.log1.ID = -1;
            this.log1.Location = new System.Drawing.Point(0, 0);
            this.log1.Name = "log1";
            this.log1.Size = new System.Drawing.Size(596, 123);
            this.log1.TabIndex = 0;
            this.log1.XUnit = "Time (seconds)";
            // 
            // scB
            // 
            this.scB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scB.Location = new System.Drawing.Point(0, 0);
            this.scB.Name = "scB";
            this.scB.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scB.Panel1
            // 
            this.scB.Panel1.Controls.Add(this.log2);
            // 
            // scB.Panel2
            // 
            this.scB.Panel2.Controls.Add(this.log3);
            this.scB.Size = new System.Drawing.Size(596, 295);
            this.scB.SplitterDistance = 124;
            this.scB.TabIndex = 0;
            // 
            // log2
            // 
            this.log2.AutoScroll = true;
            this.log2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log2.DontScrollPlotOnReSize = false;
            this.log2.ID = -1;
            this.log2.Location = new System.Drawing.Point(0, 0);
            this.log2.Name = "log2";
            this.log2.Size = new System.Drawing.Size(596, 124);
            this.log2.TabIndex = 0;
            this.log2.XUnit = "Time (seconds)";
            // 
            // log3
            // 
            this.log3.AutoScroll = true;
            this.log3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log3.DontScrollPlotOnReSize = false;
            this.log3.ID = -1;
            this.log3.Location = new System.Drawing.Point(0, 0);
            this.log3.Name = "log3";
            this.log3.Size = new System.Drawing.Size(596, 167);
            this.log3.TabIndex = 0;
            this.log3.XUnit = "Time (seconds)";
            // 
            // modeCascadeRB
            // 
            this.modeCascadeRB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modeCascadeRB.AutoSize = true;
            this.modeCascadeRB.Checked = true;
            this.modeCascadeRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modeCascadeRB.Location = new System.Drawing.Point(361, 506);
            this.modeCascadeRB.Name = "modeCascadeRB";
            this.modeCascadeRB.Size = new System.Drawing.Size(102, 28);
            this.modeCascadeRB.TabIndex = 2;
            this.modeCascadeRB.TabStop = true;
            this.modeCascadeRB.Text = "Cascade";
            this.modeCascadeRB.UseVisualStyleBackColor = true;
            this.modeCascadeRB.CheckedChanged += new System.EventHandler(this.modeRB_CheckedChanged);
            // 
            // modeOverlapRB
            // 
            this.modeOverlapRB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modeOverlapRB.AutoSize = true;
            this.modeOverlapRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modeOverlapRB.Location = new System.Drawing.Point(488, 506);
            this.modeOverlapRB.Name = "modeOverlapRB";
            this.modeOverlapRB.Size = new System.Drawing.Size(94, 28);
            this.modeOverlapRB.TabIndex = 2;
            this.modeOverlapRB.Text = "Overlap";
            this.modeOverlapRB.UseVisualStyleBackColor = true;
            this.modeOverlapRB.CheckedChanged += new System.EventHandler(this.modeRB_CheckedChanged);
            // 
            // autoScrollCB
            // 
            this.autoScrollCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.autoScrollCB.AutoSize = true;
            this.autoScrollCB.Checked = true;
            this.autoScrollCB.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.autoScrollCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoScrollCB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoScrollCB.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.autoScrollCB.Location = new System.Drawing.Point(3, 505);
            this.autoScrollCB.Name = "autoScrollCB";
            this.autoScrollCB.Size = new System.Drawing.Size(120, 28);
            this.autoScrollCB.TabIndex = 8;
            this.autoScrollCB.Text = "Auto Scroll";
            this.autoScrollCB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.autoScrollCB.UncheckedColor = System.Drawing.Color.DarkGray;
            this.autoScrollCB.UseVisualStyleBackColor = true;
            this.autoScrollCB.CheckedChanged += new System.EventHandler(this.autoScrollCB_CheckedChanged);
            // 
            // LogPlotCascade4XUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.autoScrollCB);
            this.Controls.Add(this.modeOverlapRB);
            this.Controls.Add(this.modeCascadeRB);
            this.Controls.Add(this.panel1);
            this.Name = "LogPlotCascade4XUC";
            this.Size = new System.Drawing.Size(596, 538);
            this.panel1.ResumeLayout(false);
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            this.scA.Panel1.ResumeLayout(false);
            this.scA.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scA)).EndInit();
            this.scA.ResumeLayout(false);
            this.scB.Panel1.ResumeLayout(false);
            this.scB.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scB)).EndInit();
            this.scB.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private FivePointNine.Windows.Controls.SplitContainer2 scMain;
        private FivePointNine.Windows.Controls.SplitContainer2 scA;
        private LogControlSingle log0;
        private LogControlSingle log1;
        private FivePointNine.Windows.Controls.SplitContainer2 scB;
        private LogControlSingle log2;
        private LogControlSingle log3;
        private System.Windows.Forms.RadioButton modeCascadeRB;
        private System.Windows.Forms.RadioButton modeOverlapRB;
        private FivePointNine.Windows.Controls.FlatCheckBox autoScrollCB;
    }
}

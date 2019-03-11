namespace PhysLogger
{
    partial class CompleteLogControl
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
            this.logControl = new PhysLogger.LogPlotCascade4XUC();
            this.SuspendLayout();
            // 
            // logControl
            // 
            this.logControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logControl.AutoScroll = true;
            this.logControl.DontScrollPlotOnReSize = false;
            this.logControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logControl.Location = new System.Drawing.Point(0, 0);
            this.logControl.LogLayout = PhysLogger.LogLayout.Cascade;
            this.logControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.logControl.Name = "logControl";
            this.logControl.Size = new System.Drawing.Size(631, 580);
            this.logControl.TabIndex = 8;
            // 
            // CompleteLogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logControl);
            this.Name = "CompleteLogControl";
            this.Size = new System.Drawing.Size(960, 580);
            this.ResumeLayout(false);

        }

        #endregion

        public LogPlotCascade4XUC logControl;
    }
}

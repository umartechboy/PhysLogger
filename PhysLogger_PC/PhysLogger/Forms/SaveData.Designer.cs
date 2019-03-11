namespace PhysLogger
{
    partial class ChannelSelectCheckBox
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
            this.allCB = new FivePointNine.Windows.Controls.ColoredCheckBox();
            this.SuspendLayout();
            // 
            // allCB
            // 
            this.allCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.allCB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.allCB.CheckedColor = System.Drawing.Color.DimGray;
            this.allCB.CheckedColorIsLight = true;
            this.allCB.CheckedTextColor = System.Drawing.Color.White;
            this.allCB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.allCB.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allCB.ForeColor = System.Drawing.Color.White;
            this.allCB.Location = new System.Drawing.Point(0, 0);
            this.allCB.Name = "allCB";
            this.allCB.Size = new System.Drawing.Size(144, 39);
            this.allCB.TabIndex = 0;
            this.allCB.Text = "All";
            this.allCB.UncheckedColor = System.Drawing.Color.Silver;
            this.allCB.UncheckedColorIsLight = false;
            this.allCB.UncheckedTextColor = System.Drawing.Color.Silver;
            this.allCB.UseVisualStyleBackColor = false;
            // 
            // ChannelSelectCheckBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.allCB);
            this.Name = "ChannelSelectCheckBox";
            this.Size = new System.Drawing.Size(144, 39);
            this.ResumeLayout(false);

        }

        #endregion

        private FivePointNine.Windows.Controls.ColoredCheckBox allCB;
    }
}

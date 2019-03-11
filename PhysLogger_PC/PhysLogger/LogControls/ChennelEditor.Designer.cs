namespace PhysLogger
{
    partial class ChannelEditor
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
            this.components = new System.ComponentModel.Container();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.minMaxLabel1 = new PhysLogger.MinMaxLabel();
            this.legendLinePanel1 = new PhysLogger.legendLinePanel();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(0, 5);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.checkBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(58, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(248, 24);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // minMaxLabel1
            // 
            this.minMaxLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.minMaxLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minMaxLabel1.Location = new System.Drawing.Point(0, 27);
            this.minMaxLabel1.MainFont = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minMaxLabel1.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.minMaxLabel1.Max = 0D;
            this.minMaxLabel1.Min = 0D;
            this.minMaxLabel1.MinMaxFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minMaxLabel1.Name = "minMaxLabel1";
            this.minMaxLabel1.Rounding = 2;
            this.minMaxLabel1.Size = new System.Drawing.Size(305, 44);
            this.minMaxLabel1.Suffix = "v";
            this.minMaxLabel1.TabIndex = 2;
            this.minMaxLabel1.Value = 0D;
            this.minMaxLabel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // legendLinePanel1
            // 
            this.legendLinePanel1.Location = new System.Drawing.Point(17, 0);
            this.legendLinePanel1.Name = "legendLinePanel1";
            this.legendLinePanel1.ShowLine = true;
            this.legendLinePanel1.Size = new System.Drawing.Size(35, 26);
            this.legendLinePanel1.TabIndex = 1;
            this.legendLinePanel1.MouseEnter += new System.EventHandler(this.ChannelEditor_MouseEnter);
            this.legendLinePanel1.MouseLeave += new System.EventHandler(this.ChannelEditor_MouseLeave);
            this.legendLinePanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // ChannelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.minMaxLabel1);
            this.Controls.Add(this.legendLinePanel1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.checkBox1);
            this.Name = "ChannelEditor";
            this.Size = new System.Drawing.Size(308, 71);
            this.Load += new System.EventHandler(this.ChannelEditor_Load);
            this.SizeChanged += new System.EventHandler(this.EditableCheckBox_SizeChanged);
            this.MouseEnter += new System.EventHandler(this.ChannelEditor_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ChannelEditor_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private legendLinePanel legendLinePanel1;
        private MinMaxLabel minMaxLabel1;
    }
}

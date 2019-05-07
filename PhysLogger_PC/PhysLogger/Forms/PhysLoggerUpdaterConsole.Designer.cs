namespace PhysLogger.Forms
{
    partial class PhysLoggerUpdaterConsole
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.consoleTB = new PhysLogger.Forms.ConsoleStyleTextBox();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // consoleTB
            // 
            this.consoleTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.consoleTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consoleTB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleTB.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.consoleTB.ForeColor = System.Drawing.Color.White;
            this.consoleTB.Location = new System.Drawing.Point(0, 0);
            this.consoleTB.Multiline = true;
            this.consoleTB.Name = "consoleTB";
            this.consoleTB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.consoleTB.Size = new System.Drawing.Size(1039, 588);
            this.consoleTB.TabIndex = 0;
            this.consoleTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.consoleTB_KeyDown);
            this.consoleTB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.consoleTB_MouseDown);
            // 
            // PhysLoggerUpdaterConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(1039, 588);
            this.Controls.Add(this.consoleTB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PhysLoggerUpdaterConsole";
            this.Text = "PhysLoggerUpdaterConsole";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PhysLoggerUpdaterConsole_FormClosing);
            this.Load += new System.EventHandler(this.PhysLoggerUpdaterConsole_Load);
            this.Shown += new System.EventHandler(this.PhysLoggerUpdaterConsole_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PhysLoggerUpdaterConsole_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ConsoleStyleTextBox consoleTB;
        private System.Windows.Forms.Timer timer1;
    }
}
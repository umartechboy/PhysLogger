namespace UpdateServer
{
    partial class UpdateCreator
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
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.newDiresL = new System.Windows.Forms.Label();
            this.versionL = new System.Windows.Forms.Label();
            this.removalsL = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.additionsL = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.totalComsL = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.createUpdateB = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Scan Changes";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.scanChangesB_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.newDiresL);
            this.groupBox1.Controls.Add(this.versionL);
            this.groupBox1.Controls.Add(this.removalsL);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.additionsL);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.totalComsL);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(177, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(159, 135);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Summary";
            // 
            // newDiresL
            // 
            this.newDiresL.AutoSize = true;
            this.newDiresL.Location = new System.Drawing.Point(108, 68);
            this.newDiresL.Name = "newDiresL";
            this.newDiresL.Size = new System.Drawing.Size(13, 13);
            this.newDiresL.TabIndex = 0;
            this.newDiresL.Text = "--";
            // 
            // versionL
            // 
            this.versionL.AutoSize = true;
            this.versionL.Location = new System.Drawing.Point(108, 108);
            this.versionL.Name = "versionL";
            this.versionL.Size = new System.Drawing.Size(13, 13);
            this.versionL.TabIndex = 0;
            this.versionL.Text = "--";
            // 
            // removalsL
            // 
            this.removalsL.AutoSize = true;
            this.removalsL.Location = new System.Drawing.Point(108, 89);
            this.removalsL.Name = "removalsL";
            this.removalsL.Size = new System.Drawing.Size(13, 13);
            this.removalsL.TabIndex = 0;
            this.removalsL.Text = "--";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "New Version:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "New Directories:";
            // 
            // additionsL
            // 
            this.additionsL.AutoSize = true;
            this.additionsL.Location = new System.Drawing.Point(108, 47);
            this.additionsL.Name = "additionsL";
            this.additionsL.Size = new System.Drawing.Size(13, 13);
            this.additionsL.TabIndex = 0;
            this.additionsL.Text = "--";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Removed Entries:";
            // 
            // totalComsL
            // 
            this.totalComsL.AutoSize = true;
            this.totalComsL.Location = new System.Drawing.Point(108, 26);
            this.totalComsL.Name = "totalComsL";
            this.totalComsL.Size = new System.Drawing.Size(13, 13);
            this.totalComsL.TabIndex = 0;
            this.totalComsL.Text = "--";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Additions:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total Commands:";
            // 
            // createUpdateB
            // 
            this.createUpdateB.Enabled = false;
            this.createUpdateB.Location = new System.Drawing.Point(342, 62);
            this.createUpdateB.Name = "createUpdateB";
            this.createUpdateB.Size = new System.Drawing.Size(159, 23);
            this.createUpdateB.TabIndex = 0;
            this.createUpdateB.Text = "Create and Upload";
            this.createUpdateB.UseVisualStyleBackColor = true;
            this.createUpdateB.Click += new System.EventHandler(this.createUpdateB_Click);
            // 
            // UpdateCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 151);
            this.Controls.Add(this.createUpdateB);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Name = "UpdateCreator";
            this.Text = "UpdateCreator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label versionL;
        private System.Windows.Forms.Label removalsL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label additionsL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label totalComsL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button createUpdateB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label newDiresL;
    }
}
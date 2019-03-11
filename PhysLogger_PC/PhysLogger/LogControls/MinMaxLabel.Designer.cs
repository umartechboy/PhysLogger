namespace PhysLogger
{
    partial class MinMaxLabel
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
            this.ValueL = new System.Windows.Forms.Label();
            this.maxL = new System.Windows.Forms.Label();
            this.minL = new System.Windows.Forms.Label();
            this.resetMaxB = new System.Windows.Forms.Button();
            this.resetMinB = new System.Windows.Forms.Button();
            this.sufL = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ValueL
            // 
            this.ValueL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ValueL.Location = new System.Drawing.Point(3, 3);
            this.ValueL.Margin = new System.Windows.Forms.Padding(0);
            this.ValueL.Name = "ValueL";
            this.ValueL.Size = new System.Drawing.Size(101, 39);
            this.ValueL.TabIndex = 0;
            this.ValueL.Text = "0";
            this.ValueL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // maxL
            // 
            this.maxL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxL.Location = new System.Drawing.Point(240, 0);
            this.maxL.Name = "maxL";
            this.maxL.Size = new System.Drawing.Size(51, 20);
            this.maxL.TabIndex = 1;
            this.maxL.Text = "0";
            this.maxL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // minL
            // 
            this.minL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.minL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minL.Location = new System.Drawing.Point(240, 22);
            this.minL.Name = "minL";
            this.minL.Size = new System.Drawing.Size(51, 20);
            this.minL.TabIndex = 1;
            this.minL.Text = "0";
            this.minL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // resetMaxB
            // 
            this.resetMaxB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetMaxB.BackColor = System.Drawing.Color.Silver;
            this.resetMaxB.FlatAppearance.BorderSize = 0;
            this.resetMaxB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.resetMaxB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetMaxB.Location = new System.Drawing.Point(293, 0);
            this.resetMaxB.Name = "resetMaxB";
            this.resetMaxB.Size = new System.Drawing.Size(12, 20);
            this.resetMaxB.TabIndex = 2;
            this.resetMaxB.UseVisualStyleBackColor = false;
            this.resetMaxB.Click += new System.EventHandler(this.resetMaxB_Click);
            // 
            // resetMinB
            // 
            this.resetMinB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetMinB.BackColor = System.Drawing.Color.Silver;
            this.resetMinB.FlatAppearance.BorderSize = 0;
            this.resetMinB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.resetMinB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetMinB.Location = new System.Drawing.Point(293, 23);
            this.resetMinB.Name = "resetMinB";
            this.resetMinB.Size = new System.Drawing.Size(12, 20);
            this.resetMinB.TabIndex = 2;
            this.resetMinB.UseVisualStyleBackColor = false;
            this.resetMinB.Click += new System.EventHandler(this.resetMinB_Click);
            // 
            // sufL
            // 
            this.sufL.AutoSize = true;
            this.sufL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sufL.Location = new System.Drawing.Point(110, 12);
            this.sufL.Margin = new System.Windows.Forms.Padding(0);
            this.sufL.Name = "sufL";
            this.sufL.Size = new System.Drawing.Size(19, 20);
            this.sufL.TabIndex = 3;
            this.sufL.Text = "--";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(180, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Maximum";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(181, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Minimum";
            // 
            // MinMaxLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(20F, 39F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sufL);
            this.Controls.Add(this.resetMinB);
            this.Controls.Add(this.resetMaxB);
            this.Controls.Add(this.minL);
            this.Controls.Add(this.maxL);
            this.Controls.Add(this.ValueL);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.Name = "MinMaxLabel";
            this.Size = new System.Drawing.Size(305, 44);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ValueL;
        private System.Windows.Forms.Label maxL;
        private System.Windows.Forms.Label minL;
        private System.Windows.Forms.Button resetMaxB;
        private System.Windows.Forms.Button resetMinB;
        private System.Windows.Forms.Label sufL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

namespace ARKBreedingStats
{
    partial class TamingFoodControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PanelColorIndicator = new System.Windows.Forms.Panel();
            this.labelDuration = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.labelFoodUsed = new System.Windows.Forms.Label();
            this.numericUpDown1 = new ARKBreedingStats.uiControls.Nud();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PanelColorIndicator);
            this.groupBox1.Controls.Add(this.labelDuration);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.labelFoodUsed);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 45);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // PanelColorIndicator
            // 
            this.PanelColorIndicator.Location = new System.Drawing.Point(6, 19);
            this.PanelColorIndicator.Name = "PanelColorIndicator";
            this.PanelColorIndicator.Size = new System.Drawing.Size(20, 20);
            this.PanelColorIndicator.TabIndex = 4;
            // 
            // labelDuration
            // 
            this.labelDuration.AutoSize = true;
            this.labelDuration.Location = new System.Drawing.Point(235, 21);
            this.labelDuration.Name = "labelDuration";
            this.labelDuration.Size = new System.Drawing.Size(58, 13);
            this.labelDuration.TabIndex = 3;
            this.labelDuration.Text = "0:00:00:00";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(114, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "only";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelFoodUsed
            // 
            this.labelFoodUsed.Location = new System.Drawing.Point(173, 21);
            this.labelFoodUsed.Name = "labelFoodUsed";
            this.labelFoodUsed.Size = new System.Drawing.Size(56, 13);
            this.labelFoodUsed.TabIndex = 1;
            this.labelFoodUsed.Text = "used";
            this.labelFoodUsed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.numericUpDown1.Location = new System.Drawing.Point(32, 19);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDown1.Size = new System.Drawing.Size(76, 20);
            this.numericUpDown1.TabIndex = 0;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // TamingFoodControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "TamingFoodControl";
            this.Size = new System.Drawing.Size(307, 45);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private uiControls.Nud numericUpDown1;
        private System.Windows.Forms.Label labelFoodUsed;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelDuration;
        private System.Windows.Forms.Panel PanelColorIndicator;
    }
}

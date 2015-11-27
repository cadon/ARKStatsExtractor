namespace ARKBreedingStats
{
    partial class StatIO
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelBar = new System.Windows.Forms.Panel();
            this.labelBValue = new System.Windows.Forms.Label();
            this.labelLvD = new System.Windows.Forms.Label();
            this.labelLvW = new System.Windows.Forms.Label();
            this.numericUpDownInput = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelBar);
            this.groupBox1.Controls.Add(this.labelBValue);
            this.groupBox1.Controls.Add(this.labelLvD);
            this.groupBox1.Controls.Add(this.labelLvW);
            this.groupBox1.Controls.Add(this.numericUpDownInput);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 43);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // panelBar
            // 
            this.panelBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBar.Location = new System.Drawing.Point(6, 34);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(283, 7);
            this.panelBar.TabIndex = 4;
            // 
            // labelBValue
            // 
            this.labelBValue.AutoSize = true;
            this.labelBValue.Location = new System.Drawing.Point(204, 14);
            this.labelBValue.Name = "labelBValue";
            this.labelBValue.Size = new System.Drawing.Size(35, 13);
            this.labelBValue.TabIndex = 3;
            this.labelBValue.Text = "label1";
            this.labelBValue.Click += new System.EventHandler(this.labelBValue_Click);
            // 
            // labelLvD
            // 
            this.labelLvD.AutoSize = true;
            this.labelLvD.Location = new System.Drawing.Point(163, 14);
            this.labelLvD.Name = "labelLvD";
            this.labelLvD.Size = new System.Drawing.Size(35, 13);
            this.labelLvD.TabIndex = 2;
            this.labelLvD.Text = "label1";
            this.labelLvD.Click += new System.EventHandler(this.labelLvD_Click);
            // 
            // labelLvW
            // 
            this.labelLvW.AutoSize = true;
            this.labelLvW.Location = new System.Drawing.Point(122, 14);
            this.labelLvW.Name = "labelLvW";
            this.labelLvW.Size = new System.Drawing.Size(35, 13);
            this.labelLvW.TabIndex = 1;
            this.labelLvW.Text = "label1";
            this.labelLvW.Click += new System.EventHandler(this.labelLvW_Click);
            // 
            // numericUpDownInput
            // 
            this.numericUpDownInput.DecimalPlaces = 1;
            this.numericUpDownInput.Location = new System.Drawing.Point(6, 12);
            this.numericUpDownInput.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDownInput.Name = "numericUpDownInput";
            this.numericUpDownInput.Size = new System.Drawing.Size(110, 20);
            this.numericUpDownInput.TabIndex = 0;
            this.numericUpDownInput.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownInput.Enter += new System.EventHandler(this.numericUpDownInput_Enter);
            // 
            // StatIO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "StatIO";
            this.Size = new System.Drawing.Size(295, 43);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelBValue;
        private System.Windows.Forms.Label labelLvD;
        private System.Windows.Forms.Label labelLvW;
        private System.Windows.Forms.NumericUpDown numericUpDownInput;
        private System.Windows.Forms.Panel panelBar;
    }
}

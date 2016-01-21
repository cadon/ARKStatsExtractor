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
            this.panelSettings = new System.Windows.Forms.Panel();
            this.labelMultLevel = new System.Windows.Forms.Label();
            this.numericUpDownMultLevel = new System.Windows.Forms.NumericUpDown();
            this.labelMultAff = new System.Windows.Forms.Label();
            this.numericUpDownMultAff = new System.Windows.Forms.NumericUpDown();
            this.labelAdd = new System.Windows.Forms.Label();
            this.numericUpDownMultAdd = new System.Windows.Forms.NumericUpDown();
            this.panelBar = new System.Windows.Forms.Panel();
            this.labelBValue = new System.Windows.Forms.Label();
            this.labelLvD = new System.Windows.Forms.Label();
            this.labelLvW = new System.Windows.Forms.Label();
            this.numericUpDownInput = new System.Windows.Forms.NumericUpDown();
            this.labelPercent = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panelSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultAff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelSettings);
            this.groupBox1.Controls.Add(this.panelBar);
            this.groupBox1.Controls.Add(this.labelBValue);
            this.groupBox1.Controls.Add(this.labelLvD);
            this.groupBox1.Controls.Add(this.labelLvW);
            this.groupBox1.Controls.Add(this.numericUpDownInput);
            this.groupBox1.Controls.Add(this.labelPercent);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 43);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // panelSettings
            // 
            this.panelSettings.Controls.Add(this.labelMultLevel);
            this.panelSettings.Controls.Add(this.numericUpDownMultLevel);
            this.panelSettings.Controls.Add(this.labelMultAff);
            this.panelSettings.Controls.Add(this.numericUpDownMultAff);
            this.panelSettings.Controls.Add(this.labelAdd);
            this.panelSettings.Controls.Add(this.numericUpDownMultAdd);
            this.panelSettings.Location = new System.Drawing.Point(5, 12);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(285, 29);
            this.panelSettings.TabIndex = 5;
            this.panelSettings.Visible = false;
            // 
            // labelMultLevel
            // 
            this.labelMultLevel.AutoSize = true;
            this.labelMultLevel.Location = new System.Drawing.Point(243, 10);
            this.labelMultLevel.Name = "labelMultLevel";
            this.labelMultLevel.Size = new System.Drawing.Size(33, 13);
            this.labelMultLevel.TabIndex = 5;
            this.labelMultLevel.Text = "Level";
            // 
            // numericUpDownMultLevel
            // 
            this.numericUpDownMultLevel.DecimalPlaces = 3;
            this.numericUpDownMultLevel.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownMultLevel.Location = new System.Drawing.Point(194, 8);
            this.numericUpDownMultLevel.Name = "numericUpDownMultLevel";
            this.numericUpDownMultLevel.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownMultLevel.TabIndex = 4;
            this.numericUpDownMultLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMultLevel.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // labelMultAff
            // 
            this.labelMultAff.AutoSize = true;
            this.labelMultAff.Location = new System.Drawing.Point(144, 10);
            this.labelMultAff.Name = "labelMultAff";
            this.labelMultAff.Size = new System.Drawing.Size(38, 13);
            this.labelMultAff.TabIndex = 3;
            this.labelMultAff.Text = "Affinity";
            // 
            // numericUpDownMultAff
            // 
            this.numericUpDownMultAff.DecimalPlaces = 3;
            this.numericUpDownMultAff.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownMultAff.Location = new System.Drawing.Point(95, 8);
            this.numericUpDownMultAff.Name = "numericUpDownMultAff";
            this.numericUpDownMultAff.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownMultAff.TabIndex = 2;
            this.numericUpDownMultAff.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMultAff.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // labelAdd
            // 
            this.labelAdd.AutoSize = true;
            this.labelAdd.Location = new System.Drawing.Point(58, 10);
            this.labelAdd.Name = "labelAdd";
            this.labelAdd.Size = new System.Drawing.Size(26, 13);
            this.labelAdd.TabIndex = 1;
            this.labelAdd.Text = "Add";
            // 
            // numericUpDownMultAdd
            // 
            this.numericUpDownMultAdd.DecimalPlaces = 3;
            this.numericUpDownMultAdd.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownMultAdd.Location = new System.Drawing.Point(9, 8);
            this.numericUpDownMultAdd.Name = "numericUpDownMultAdd";
            this.numericUpDownMultAdd.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownMultAdd.TabIndex = 0;
            this.numericUpDownMultAdd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMultAdd.Enter += new System.EventHandler(this.numericUpDown_Enter);
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
            this.numericUpDownInput.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // labelPercent
            // 
            this.labelPercent.AutoSize = true;
            this.labelPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPercent.Location = new System.Drawing.Point(114, 25);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(12, 9);
            this.labelPercent.TabIndex = 6;
            this.labelPercent.Text = "%";
            this.labelPercent.Visible = false;
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
            this.panelSettings.ResumeLayout(false);
            this.panelSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultAff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultAdd)).EndInit();
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
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.Label labelMultLevel;
        private System.Windows.Forms.NumericUpDown numericUpDownMultLevel;
        private System.Windows.Forms.Label labelMultAff;
        private System.Windows.Forms.NumericUpDown numericUpDownMultAff;
        private System.Windows.Forms.Label labelAdd;
        private System.Windows.Forms.NumericUpDown numericUpDownMultAdd;
        private System.Windows.Forms.Label labelPercent;
    }
}

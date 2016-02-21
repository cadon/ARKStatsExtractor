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
            this.inputPanel = new System.Windows.Forms.Panel();
            this.numericUpDownInput = new System.Windows.Forms.NumericUpDown();
            this.numLvD = new System.Windows.Forms.NumericUpDown();
            this.numLvW = new System.Windows.Forms.NumericUpDown();
            this.labelBValue = new System.Windows.Forms.Label();
            this.panelFinalValue = new System.Windows.Forms.Panel();
            this.labelFinalValue = new System.Windows.Forms.Label();
            this.labelWildLevel = new System.Windows.Forms.Label();
            this.labelDomLevel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.inputPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLvD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLvW)).BeginInit();
            this.panelFinalValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelFinalValue);
            this.groupBox1.Controls.Add(this.panelBar);
            this.groupBox1.Controls.Add(this.inputPanel);
            this.groupBox1.Controls.Add(this.labelBValue);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // panelBar
            // 
            this.panelBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBar.Location = new System.Drawing.Point(6, 36);
            this.panelBar.Name = "panelBar";
            this.panelBar.Size = new System.Drawing.Size(2, 6);
            this.panelBar.TabIndex = 4;
            // 
            // inputPanel
            // 
            this.inputPanel.Controls.Add(this.labelFinalValue);
            this.inputPanel.Controls.Add(this.numLvD);
            this.inputPanel.Controls.Add(this.numLvW);
            this.inputPanel.Location = new System.Drawing.Point(6, 11);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(217, 25);
            this.inputPanel.TabIndex = 8;
            this.inputPanel.Click += new System.EventHandler(this.inputPanel_Click);
            // 
            // numericUpDownInput
            // 
            this.numericUpDownInput.DecimalPlaces = 1;
            this.numericUpDownInput.Location = new System.Drawing.Point(3, 3);
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
            // numLvD
            // 
            this.numLvD.Location = new System.Drawing.Point(171, 3);
            this.numLvD.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numLvD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numLvD.Name = "numLvD";
            this.numLvD.Size = new System.Drawing.Size(46, 20);
            this.numLvD.TabIndex = 7;
            // 
            // numLvW
            // 
            this.numLvW.Location = new System.Drawing.Point(119, 3);
            this.numLvW.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numLvW.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numLvW.Name = "numLvW";
            this.numLvW.Size = new System.Drawing.Size(46, 20);
            this.numLvW.TabIndex = 6;
            this.numLvW.ValueChanged += new System.EventHandler(this.numLvW_ValueChanged);
            // 
            // labelBValue
            // 
            this.labelBValue.AutoSize = true;
            this.labelBValue.Location = new System.Drawing.Point(229, 16);
            this.labelBValue.Name = "labelBValue";
            this.labelBValue.Size = new System.Drawing.Size(50, 13);
            this.labelBValue.TabIndex = 3;
            this.labelBValue.Text = "BreedVal";
            this.labelBValue.Click += new System.EventHandler(this.labelBValue_Click);
            // 
            // panelFinalValue
            // 
            this.panelFinalValue.Controls.Add(this.labelDomLevel);
            this.panelFinalValue.Controls.Add(this.labelWildLevel);
            this.panelFinalValue.Controls.Add(this.numericUpDownInput);
            this.panelFinalValue.Location = new System.Drawing.Point(6, 11);
            this.panelFinalValue.Name = "panelFinalValue";
            this.panelFinalValue.Size = new System.Drawing.Size(217, 25);
            this.panelFinalValue.TabIndex = 9;
            // 
            // labelFinalValue
            // 
            this.labelFinalValue.AutoSize = true;
            this.labelFinalValue.Location = new System.Drawing.Point(3, 5);
            this.labelFinalValue.Name = "labelFinalValue";
            this.labelFinalValue.Size = new System.Drawing.Size(13, 13);
            this.labelFinalValue.TabIndex = 10;
            this.labelFinalValue.Text = "0";
            // 
            // labelWildLevel
            // 
            this.labelWildLevel.AutoSize = true;
            this.labelWildLevel.Location = new System.Drawing.Point(116, 5);
            this.labelWildLevel.Name = "labelWildLevel";
            this.labelWildLevel.Size = new System.Drawing.Size(13, 13);
            this.labelWildLevel.TabIndex = 10;
            this.labelWildLevel.Text = "0";
            this.labelWildLevel.Click += new System.EventHandler(this.labelWildLevel_Click);
            // 
            // labelDomLevel
            // 
            this.labelDomLevel.AutoSize = true;
            this.labelDomLevel.Location = new System.Drawing.Point(168, 5);
            this.labelDomLevel.Name = "labelDomLevel";
            this.labelDomLevel.Size = new System.Drawing.Size(13, 13);
            this.labelDomLevel.TabIndex = 11;
            this.labelDomLevel.Text = "0";
            this.labelDomLevel.Click += new System.EventHandler(this.labelDomLevel_Click);
            // 
            // StatIO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "StatIO";
            this.Size = new System.Drawing.Size(295, 50);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLvD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLvW)).EndInit();
            this.panelFinalValue.ResumeLayout(false);
            this.panelFinalValue.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelBValue;
        private System.Windows.Forms.NumericUpDown numericUpDownInput;
        private System.Windows.Forms.Panel panelBar;
        private System.Windows.Forms.NumericUpDown numLvD;
        private System.Windows.Forms.NumericUpDown numLvW;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Panel panelFinalValue;
        private System.Windows.Forms.Label labelDomLevel;
        private System.Windows.Forms.Label labelWildLevel;
        private System.Windows.Forms.Label labelFinalValue;
    }
}

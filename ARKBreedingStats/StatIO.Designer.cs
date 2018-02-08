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
            this.panelBarDomLevels = new System.Windows.Forms.Panel();
            this.panelFinalValue = new System.Windows.Forms.Panel();
            this.checkBoxFixDomZero = new System.Windows.Forms.CheckBox();
            this.labelDomLevel = new System.Windows.Forms.Label();
            this.labelWildLevel = new System.Windows.Forms.Label();
            this.numericUpDownInput = new ARKBreedingStats.uiControls.Nud();
            this.panelBarWildLevels = new System.Windows.Forms.Panel();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.labelFinalValue = new System.Windows.Forms.Label();
            this.numLvD = new ARKBreedingStats.uiControls.Nud();
            this.numLvW = new ARKBreedingStats.uiControls.Nud();
            this.labelBValue = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panelFinalValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).BeginInit();
            this.inputPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLvD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLvW)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelBarDomLevels);
            this.groupBox1.Controls.Add(this.panelFinalValue);
            this.groupBox1.Controls.Add(this.panelBarWildLevels);
            this.groupBox1.Controls.Add(this.inputPanel);
            this.groupBox1.Controls.Add(this.labelBValue);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // panelBarDomLevels
            // 
            this.panelBarDomLevels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelBarDomLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBarDomLevels.Location = new System.Drawing.Point(6, 43);
            this.panelBarDomLevels.Name = "panelBarDomLevels";
            this.panelBarDomLevels.Size = new System.Drawing.Size(2, 3);
            this.panelBarDomLevels.TabIndex = 5;
            // 
            // panelFinalValue
            // 
            this.panelFinalValue.Controls.Add(this.checkBoxFixDomZero);
            this.panelFinalValue.Controls.Add(this.labelDomLevel);
            this.panelFinalValue.Controls.Add(this.labelWildLevel);
            this.panelFinalValue.Controls.Add(this.numericUpDownInput);
            this.panelFinalValue.Location = new System.Drawing.Point(6, 14);
            this.panelFinalValue.Name = "panelFinalValue";
            this.panelFinalValue.Size = new System.Drawing.Size(217, 25);
            this.panelFinalValue.TabIndex = 9;
            this.panelFinalValue.Click += new System.EventHandler(this.panelFinalValue_Click);
            // 
            // checkBoxFixDomZero
            // 
            this.checkBoxFixDomZero.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxFixDomZero.Image = global::ARKBreedingStats.Properties.Resources.unlocked;
            this.checkBoxFixDomZero.Location = new System.Drawing.Point(157, 9);
            this.checkBoxFixDomZero.Name = "checkBoxFixDomZero";
            this.checkBoxFixDomZero.Size = new System.Drawing.Size(14, 17);
            this.checkBoxFixDomZero.TabIndex = 12;
            this.checkBoxFixDomZero.TabStop = false;
            this.checkBoxFixDomZero.UseVisualStyleBackColor = true;
            this.checkBoxFixDomZero.CheckedChanged += new System.EventHandler(this.checkBoxFixDomZero_CheckedChanged);
            // 
            // labelDomLevel
            // 
            this.labelDomLevel.AutoSize = true;
            this.labelDomLevel.Location = new System.Drawing.Point(168, 5);
            this.labelDomLevel.Name = "labelDomLevel";
            this.labelDomLevel.Size = new System.Drawing.Size(13, 13);
            this.labelDomLevel.TabIndex = 11;
            this.labelDomLevel.Text = "0";
            this.labelDomLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDomLevel.Click += new System.EventHandler(this.labelDomLevel_Click);
            // 
            // labelWildLevel
            // 
            this.labelWildLevel.AutoSize = true;
            this.labelWildLevel.Location = new System.Drawing.Point(116, 5);
            this.labelWildLevel.Name = "labelWildLevel";
            this.labelWildLevel.Size = new System.Drawing.Size(13, 13);
            this.labelWildLevel.TabIndex = 10;
            this.labelWildLevel.Text = "0";
            this.labelWildLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelWildLevel.Click += new System.EventHandler(this.labelWildLevel_Click);
            // 
            // numericUpDownInput
            // 
            this.numericUpDownInput.DecimalPlaces = 1;
            this.numericUpDownInput.ForeColor = System.Drawing.SystemColors.WindowText;
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
            this.numericUpDownInput.ValueChanged += new System.EventHandler(this.numericUpDownInput_ValueChanged);
            this.numericUpDownInput.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // panelBarWildLevels
            // 
            this.panelBarWildLevels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelBarWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBarWildLevels.Location = new System.Drawing.Point(6, 39);
            this.panelBarWildLevels.Name = "panelBarWildLevels";
            this.panelBarWildLevels.Size = new System.Drawing.Size(2, 6);
            this.panelBarWildLevels.TabIndex = 4;
            this.panelBarWildLevels.Click += new System.EventHandler(this.panelBar_Click);
            // 
            // inputPanel
            // 
            this.inputPanel.Controls.Add(this.labelFinalValue);
            this.inputPanel.Controls.Add(this.numLvD);
            this.inputPanel.Controls.Add(this.numLvW);
            this.inputPanel.Location = new System.Drawing.Point(6, 14);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(217, 25);
            this.inputPanel.TabIndex = 8;
            // 
            // labelFinalValue
            // 
            this.labelFinalValue.AutoSize = true;
            this.labelFinalValue.Location = new System.Drawing.Point(122, 5);
            this.labelFinalValue.Name = "labelFinalValue";
            this.labelFinalValue.Size = new System.Drawing.Size(13, 13);
            this.labelFinalValue.TabIndex = 10;
            this.labelFinalValue.Text = "0";
            // 
            // numLvD
            // 
            this.numLvD.ForeColor = System.Drawing.SystemColors.GrayText;
            this.numLvD.Location = new System.Drawing.Point(55, 3);
            this.numLvD.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numLvD.Name = "numLvD";
            this.numLvD.Size = new System.Drawing.Size(46, 20);
            this.numLvD.TabIndex = 7;
            this.numLvD.ValueChanged += new System.EventHandler(this.numLvD_ValueChanged);
            this.numLvD.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // numLvW
            // 
            this.numLvW.ForeColor = System.Drawing.SystemColors.GrayText;
            this.numLvW.Location = new System.Drawing.Point(3, 3);
            this.numLvW.Maximum = new decimal(new int[] {
            9999,
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
            this.numLvW.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // labelBValue
            // 
            this.labelBValue.AutoSize = true;
            this.labelBValue.Location = new System.Drawing.Point(229, 19);
            this.labelBValue.Name = "labelBValue";
            this.labelBValue.Size = new System.Drawing.Size(50, 13);
            this.labelBValue.TabIndex = 3;
            this.labelBValue.Text = "BreedVal";
            this.labelBValue.Click += new System.EventHandler(this.labelBValue_Click);
            // 
            // StatIO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.groupBox1);
            this.Name = "StatIO";
            this.Size = new System.Drawing.Size(295, 50);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelFinalValue.ResumeLayout(false);
            this.panelFinalValue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).EndInit();
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLvD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLvW)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelBValue;
        private uiControls.Nud numericUpDownInput;
        private System.Windows.Forms.Panel panelBarWildLevels;
        private uiControls.Nud numLvD;
        private uiControls.Nud numLvW;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Panel panelFinalValue;
        private System.Windows.Forms.Label labelDomLevel;
        private System.Windows.Forms.Label labelWildLevel;
        private System.Windows.Forms.Label labelFinalValue;
        private System.Windows.Forms.CheckBox checkBoxFixDomZero;
        private System.Windows.Forms.Panel panelBarDomLevels;
    }
}

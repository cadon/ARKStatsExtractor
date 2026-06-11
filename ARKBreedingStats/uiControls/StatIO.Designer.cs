namespace ARKBreedingStats.uiControls
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
            groupBox1 = new GroupBoxC();
            panelBarDomLevels = new System.Windows.Forms.Panel();
            panelBarMutLevels = new System.Windows.Forms.Panel();
            panelBarWildLevels = new System.Windows.Forms.Panel();
            panelFinalValue = new System.Windows.Forms.Panel();
            labelMutatedLevel = new System.Windows.Forms.Label();
            checkBoxFixDomZero = new System.Windows.Forms.CheckBox();
            labelDomLevel = new System.Windows.Forms.Label();
            labelWildLevel = new System.Windows.Forms.Label();
            numericUpDownInput = new Nud();
            inputPanel = new System.Windows.Forms.Panel();
            nudLvM = new Nud();
            labelFinalValue = new System.Windows.Forms.Label();
            nudLvD = new Nud();
            nudLvW = new Nud();
            labelBValue = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            panelFinalValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInput).BeginInit();
            inputPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudLvM).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLvD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLvW).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = System.Drawing.Color.Transparent;
            groupBox1.Controls.Add(panelBarDomLevels);
            groupBox1.Controls.Add(panelBarMutLevels);
            groupBox1.Controls.Add(panelBarWildLevels);
            groupBox1.Controls.Add(panelFinalValue);
            groupBox1.Controls.Add(inputPanel);
            groupBox1.Controls.Add(labelBValue);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(405, 58);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            // 
            // panelBarDomLevels
            // 
            panelBarDomLevels.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
            panelBarDomLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelBarDomLevels.Location = new System.Drawing.Point(7, 51);
            panelBarDomLevels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelBarDomLevels.Name = "panelBarDomLevels";
            panelBarDomLevels.Size = new System.Drawing.Size(2, 3);
            panelBarDomLevels.TabIndex = 5;
            // 
            // panelBarMutLevels
            // 
            panelBarMutLevels.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
            panelBarMutLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelBarMutLevels.Location = new System.Drawing.Point(7, 47);
            panelBarMutLevels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelBarMutLevels.Name = "panelBarMutLevels";
            panelBarMutLevels.Size = new System.Drawing.Size(2, 4);
            panelBarMutLevels.TabIndex = 6;
            // 
            // panelBarWildLevels
            // 
            panelBarWildLevels.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
            panelBarWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelBarWildLevels.Location = new System.Drawing.Point(7, 44);
            panelBarWildLevels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelBarWildLevels.Name = "panelBarWildLevels";
            panelBarWildLevels.Size = new System.Drawing.Size(2, 7);
            panelBarWildLevels.TabIndex = 4;
            panelBarWildLevels.Click += panelBar_Click;
            // 
            // panelFinalValue
            // 
            panelFinalValue.Controls.Add(labelMutatedLevel);
            panelFinalValue.Controls.Add(checkBoxFixDomZero);
            panelFinalValue.Controls.Add(labelDomLevel);
            panelFinalValue.Controls.Add(labelWildLevel);
            panelFinalValue.Controls.Add(numericUpDownInput);
            panelFinalValue.Location = new System.Drawing.Point(7, 16);
            panelFinalValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelFinalValue.Name = "panelFinalValue";
            panelFinalValue.Size = new System.Drawing.Size(314, 29);
            panelFinalValue.TabIndex = 9;
            panelFinalValue.Click += panelFinalValue_Click;
            // 
            // labelMutatedLevel
            // 
            labelMutatedLevel.Location = new System.Drawing.Point(200, 6);
            labelMutatedLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelMutatedLevel.Name = "labelMutatedLevel";
            labelMutatedLevel.Size = new System.Drawing.Size(41, 15);
            labelMutatedLevel.TabIndex = 13;
            labelMutatedLevel.Text = "0";
            labelMutatedLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            labelMutatedLevel.Click += labelMutatedLevel_Click;
            // 
            // checkBoxFixDomZero
            // 
            checkBoxFixDomZero.Appearance = System.Windows.Forms.Appearance.Button;
            checkBoxFixDomZero.BackColor = System.Drawing.SystemColors.Control;
            checkBoxFixDomZero.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            checkBoxFixDomZero.Image = Properties.Resources.unlocked;
            checkBoxFixDomZero.Location = new System.Drawing.Point(293, 6);
            checkBoxFixDomZero.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxFixDomZero.Name = "checkBoxFixDomZero";
            checkBoxFixDomZero.Size = new System.Drawing.Size(16, 20);
            checkBoxFixDomZero.TabIndex = 12;
            checkBoxFixDomZero.TabStop = false;
            checkBoxFixDomZero.UseVisualStyleBackColor = false;
            checkBoxFixDomZero.CheckedChanged += checkBoxFixDomZero_CheckedChanged;
            // 
            // labelDomLevel
            // 
            labelDomLevel.Location = new System.Drawing.Point(250, 6);
            labelDomLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelDomLevel.Name = "labelDomLevel";
            labelDomLevel.Size = new System.Drawing.Size(41, 15);
            labelDomLevel.TabIndex = 11;
            labelDomLevel.Text = "0";
            labelDomLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            labelDomLevel.Click += labelDomLevel_Click;
            // 
            // labelWildLevel
            // 
            labelWildLevel.Location = new System.Drawing.Point(139, 6);
            labelWildLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelWildLevel.Name = "labelWildLevel";
            labelWildLevel.Size = new System.Drawing.Size(41, 15);
            labelWildLevel.TabIndex = 10;
            labelWildLevel.Text = "0";
            labelWildLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            labelWildLevel.Click += labelWildLevel_Click;
            // 
            // numericUpDownInput
            // 
            numericUpDownInput.DecimalPlaces = 1;
            numericUpDownInput.ForeColor = System.Drawing.SystemColors.WindowText;
            numericUpDownInput.Location = new System.Drawing.Point(4, 3);
            numericUpDownInput.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            numericUpDownInput.Maximum = new decimal(new int[] { 1661992959, 1808227885, 5, 0 });
            numericUpDownInput.Name = "numericUpDownInput";
            numericUpDownInput.Size = new System.Drawing.Size(128, 23);
            numericUpDownInput.TabIndex = 0;
            numericUpDownInput.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDownInput.ValueChanged += numericUpDownInput_ValueChanged;
            numericUpDownInput.Enter += numericUpDown_Enter;
            // 
            // inputPanel
            // 
            inputPanel.Controls.Add(nudLvM);
            inputPanel.Controls.Add(labelFinalValue);
            inputPanel.Controls.Add(nudLvD);
            inputPanel.Controls.Add(nudLvW);
            inputPanel.Location = new System.Drawing.Point(7, 16);
            inputPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            inputPanel.Name = "inputPanel";
            inputPanel.Size = new System.Drawing.Size(314, 29);
            inputPanel.TabIndex = 8;
            // 
            // nudLvM
            // 
            nudLvM.ForeColor = System.Drawing.SystemColors.GrayText;
            nudLvM.Location = new System.Drawing.Point(64, 3);
            nudLvM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nudLvM.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            nudLvM.Name = "nudLvM";
            nudLvM.Size = new System.Drawing.Size(54, 23);
            nudLvM.TabIndex = 1;
            nudLvM.ValueChanged += nudLvM_ValueChanged;
            nudLvM.Enter += numericUpDown_Enter;
            // 
            // labelFinalValue
            // 
            labelFinalValue.Location = new System.Drawing.Point(200, 6);
            labelFinalValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelFinalValue.Name = "labelFinalValue";
            labelFinalValue.Size = new System.Drawing.Size(82, 15);
            labelFinalValue.TabIndex = 10;
            labelFinalValue.Text = "0";
            labelFinalValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // nudLvD
            // 
            nudLvD.ForeColor = System.Drawing.SystemColors.GrayText;
            nudLvD.Location = new System.Drawing.Point(125, 3);
            nudLvD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nudLvD.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            nudLvD.Name = "nudLvD";
            nudLvD.Size = new System.Drawing.Size(54, 23);
            nudLvD.TabIndex = 2;
            nudLvD.ValueChanged += numLvD_ValueChanged;
            nudLvD.Enter += numericUpDown_Enter;
            // 
            // nudLvW
            // 
            nudLvW.ForeColor = System.Drawing.SystemColors.GrayText;
            nudLvW.Location = new System.Drawing.Point(4, 3);
            nudLvW.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nudLvW.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            nudLvW.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            nudLvW.Name = "nudLvW";
            nudLvW.Size = new System.Drawing.Size(54, 23);
            nudLvW.TabIndex = 0;
            nudLvW.ValueChanged += numLvW_ValueChanged;
            nudLvW.Enter += numericUpDown_Enter;
            // 
            // labelBValue
            // 
            labelBValue.Location = new System.Drawing.Point(317, 22);
            labelBValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelBValue.Name = "labelBValue";
            labelBValue.Size = new System.Drawing.Size(82, 15);
            labelBValue.TabIndex = 3;
            labelBValue.Text = "BreedVal";
            labelBValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            labelBValue.Click += labelBValue_Click;
            // 
            // StatIO
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "StatIO";
            Size = new System.Drawing.Size(405, 58);
            groupBox1.ResumeLayout(false);
            panelFinalValue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDownInput).EndInit();
            inputPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudLvM).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLvD).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLvW).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private GroupBoxC groupBox1;
        private System.Windows.Forms.Label labelBValue;
        private uiControls.Nud numericUpDownInput;
        private System.Windows.Forms.Panel panelBarWildLevels;
        private uiControls.Nud nudLvD;
        private uiControls.Nud nudLvW;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Panel panelFinalValue;
        private System.Windows.Forms.Label labelDomLevel;
        private System.Windows.Forms.Label labelWildLevel;
        private System.Windows.Forms.Label labelFinalValue;
        private System.Windows.Forms.CheckBox checkBoxFixDomZero;
        private System.Windows.Forms.Panel panelBarDomLevels;
        private System.Windows.Forms.Label labelMutatedLevel;
        private uiControls.Nud nudLvM;
        private System.Windows.Forms.Panel panelBarMutLevels;
    }
}

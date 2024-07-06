﻿namespace ARKBreedingStats.uiControls
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
            this.panelBarMutLevels = new System.Windows.Forms.Panel();
            this.panelBarWildLevels = new System.Windows.Forms.Panel();
            this.panelFinalValue = new System.Windows.Forms.Panel();
            this.labelMutatedLevel = new System.Windows.Forms.Label();
            this.checkBoxFixDomZero = new System.Windows.Forms.CheckBox();
            this.labelDomLevel = new System.Windows.Forms.Label();
            this.labelWildLevel = new System.Windows.Forms.Label();
            this.numericUpDownInput = new ARKBreedingStats.uiControls.Nud();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.nudLvM = new ARKBreedingStats.uiControls.Nud();
            this.labelFinalValue = new System.Windows.Forms.Label();
            this.nudLvD = new ARKBreedingStats.uiControls.Nud();
            this.nudLvW = new ARKBreedingStats.uiControls.Nud();
            this.labelBValue = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panelFinalValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).BeginInit();
            this.inputPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLvM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLvD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLvW)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelBarDomLevels);
            this.groupBox1.Controls.Add(this.panelBarMutLevels);
            this.groupBox1.Controls.Add(this.panelBarWildLevels);
            this.groupBox1.Controls.Add(this.panelFinalValue);
            this.groupBox1.Controls.Add(this.inputPanel);
            this.groupBox1.Controls.Add(this.labelBValue);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(347, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // panelBarDomLevels
            // 
            this.panelBarDomLevels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelBarDomLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBarDomLevels.Location = new System.Drawing.Point(6, 44);
            this.panelBarDomLevels.Name = "panelBarDomLevels";
            this.panelBarDomLevels.Size = new System.Drawing.Size(2, 3);
            this.panelBarDomLevels.TabIndex = 5;
            // 
            // panelBarMutLevels
            // 
            this.panelBarMutLevels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelBarMutLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBarMutLevels.Location = new System.Drawing.Point(6, 41);
            this.panelBarMutLevels.Name = "panelBarMutLevels";
            this.panelBarMutLevels.Size = new System.Drawing.Size(2, 4);
            this.panelBarMutLevels.TabIndex = 6;
            // 
            // panelBarWildLevels
            // 
            this.panelBarWildLevels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelBarWildLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBarWildLevels.Location = new System.Drawing.Point(6, 38);
            this.panelBarWildLevels.Name = "panelBarWildLevels";
            this.panelBarWildLevels.Size = new System.Drawing.Size(2, 6);
            this.panelBarWildLevels.TabIndex = 4;
            this.panelBarWildLevels.Click += new System.EventHandler(this.panelBar_Click);
            // 
            // panelFinalValue
            // 
            this.panelFinalValue.Controls.Add(this.labelMutatedLevel);
            this.panelFinalValue.Controls.Add(this.checkBoxFixDomZero);
            this.panelFinalValue.Controls.Add(this.labelDomLevel);
            this.panelFinalValue.Controls.Add(this.labelWildLevel);
            this.panelFinalValue.Controls.Add(this.numericUpDownInput);
            this.panelFinalValue.Location = new System.Drawing.Point(6, 14);
            this.panelFinalValue.Name = "panelFinalValue";
            this.panelFinalValue.Size = new System.Drawing.Size(269, 25);
            this.panelFinalValue.TabIndex = 9;
            this.panelFinalValue.Click += new System.EventHandler(this.panelFinalValue_Click);
            // 
            // labelMutatedLevel
            // 
            this.labelMutatedLevel.Location = new System.Drawing.Point(171, 5);
            this.labelMutatedLevel.Name = "labelMutatedLevel";
            this.labelMutatedLevel.Size = new System.Drawing.Size(35, 13);
            this.labelMutatedLevel.TabIndex = 13;
            this.labelMutatedLevel.Text = "0";
            this.labelMutatedLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelMutatedLevel.Click += new System.EventHandler(this.labelMutatedLevel_Click);
            // 
            // checkBoxFixDomZero
            // 
            this.checkBoxFixDomZero.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxFixDomZero.Image = global::ARKBreedingStats.Properties.Resources.unlocked;
            this.checkBoxFixDomZero.Location = new System.Drawing.Point(251, 9);
            this.checkBoxFixDomZero.Name = "checkBoxFixDomZero";
            this.checkBoxFixDomZero.Size = new System.Drawing.Size(14, 17);
            this.checkBoxFixDomZero.TabIndex = 12;
            this.checkBoxFixDomZero.TabStop = false;
            this.checkBoxFixDomZero.UseVisualStyleBackColor = true;
            this.checkBoxFixDomZero.CheckedChanged += new System.EventHandler(this.checkBoxFixDomZero_CheckedChanged);
            // 
            // labelDomLevel
            // 
            this.labelDomLevel.Location = new System.Drawing.Point(214, 5);
            this.labelDomLevel.Name = "labelDomLevel";
            this.labelDomLevel.Size = new System.Drawing.Size(35, 13);
            this.labelDomLevel.TabIndex = 11;
            this.labelDomLevel.Text = "0";
            this.labelDomLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDomLevel.Click += new System.EventHandler(this.labelDomLevel_Click);
            // 
            // labelWildLevel
            // 
            this.labelWildLevel.Location = new System.Drawing.Point(119, 5);
            this.labelWildLevel.Name = "labelWildLevel";
            this.labelWildLevel.Size = new System.Drawing.Size(35, 13);
            this.labelWildLevel.TabIndex = 10;
            this.labelWildLevel.Text = "0";
            this.labelWildLevel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelWildLevel.Click += new System.EventHandler(this.labelWildLevel_Click);
            // 
            // numericUpDownInput
            // 
            this.numericUpDownInput.DecimalPlaces = 1;
            this.numericUpDownInput.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDownInput.Location = new System.Drawing.Point(3, 3);
            this.numericUpDownInput.Maximum = new decimal(new int[] {
            1661992959,
            1808227885,
            5,
            0});
            this.numericUpDownInput.Name = "numericUpDownInput";
            this.numericUpDownInput.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
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
            // inputPanel
            // 
            this.inputPanel.Controls.Add(this.nudLvM);
            this.inputPanel.Controls.Add(this.labelFinalValue);
            this.inputPanel.Controls.Add(this.nudLvD);
            this.inputPanel.Controls.Add(this.nudLvW);
            this.inputPanel.Location = new System.Drawing.Point(6, 14);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(269, 25);
            this.inputPanel.TabIndex = 8;
            // 
            // nudLvM
            // 
            this.nudLvM.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudLvM.Location = new System.Drawing.Point(55, 3);
            this.nudLvM.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudLvM.Name = "nudLvM";
            this.nudLvM.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudLvM.Size = new System.Drawing.Size(46, 20);
            this.nudLvM.TabIndex = 1;
            this.nudLvM.ValueChanged += new System.EventHandler(this.nudLvM_ValueChanged);
            this.nudLvM.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // labelFinalValue
            // 
            this.labelFinalValue.Location = new System.Drawing.Point(171, 5);
            this.labelFinalValue.Name = "labelFinalValue";
            this.labelFinalValue.Size = new System.Drawing.Size(70, 13);
            this.labelFinalValue.TabIndex = 10;
            this.labelFinalValue.Text = "0";
            this.labelFinalValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // numLvD
            // 
            this.nudLvD.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudLvD.Location = new System.Drawing.Point(107, 3);
            this.nudLvD.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudLvD.Name = "nudLvD";
            this.nudLvD.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudLvD.Size = new System.Drawing.Size(46, 20);
            this.nudLvD.TabIndex = 2;
            this.nudLvD.ValueChanged += new System.EventHandler(this.numLvD_ValueChanged);
            this.nudLvD.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // numLvW
            // 
            this.nudLvW.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudLvW.Location = new System.Drawing.Point(3, 3);
            this.nudLvW.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudLvW.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.nudLvW.Name = "nudLvW";
            this.nudLvW.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudLvW.Size = new System.Drawing.Size(46, 20);
            this.nudLvW.TabIndex = 0;
            this.nudLvW.ValueChanged += new System.EventHandler(this.numLvW_ValueChanged);
            this.nudLvW.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // labelBValue
            // 
            this.labelBValue.Location = new System.Drawing.Point(272, 19);
            this.labelBValue.Name = "labelBValue";
            this.labelBValue.Size = new System.Drawing.Size(70, 13);
            this.labelBValue.TabIndex = 3;
            this.labelBValue.Text = "BreedVal";
            this.labelBValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelBValue.Click += new System.EventHandler(this.labelBValue_Click);
            // 
            // StatIO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StatIO";
            this.Size = new System.Drawing.Size(347, 50);
            this.groupBox1.ResumeLayout(false);
            this.panelFinalValue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInput)).EndInit();
            this.inputPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudLvM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLvD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLvW)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
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

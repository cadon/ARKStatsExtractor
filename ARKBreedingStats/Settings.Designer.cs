namespace ARKBreedingStats
{
    partial class Settings
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
            this.groupBoxMultiplier = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAllToOne = new System.Windows.Forms.Button();
            this.buttonSetToOfficial = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.multiplierSettingTo = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingSp = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingDm = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingWe = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingFo = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingOx = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingSt = new ARKBreedingStats.MultiplierSetting();
            this.multiplierSettingHP = new ARKBreedingStats.MultiplierSetting();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxAutoSave = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkExperimentalOCR = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownAutosaveMinutes = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownMaturation = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownHatching = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDownMaxWildLevel = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownDomLevelNr = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownMaxBreedingSug = new System.Windows.Forms.NumericUpDown();
            this.groupBoxMultiplier.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutosaveMinutes)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHatching)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxWildLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDomLevelNr)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxBreedingSug)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxMultiplier
            // 
            this.groupBoxMultiplier.Controls.Add(this.label7);
            this.groupBoxMultiplier.Controls.Add(this.labelInfo);
            this.groupBoxMultiplier.Controls.Add(this.label4);
            this.groupBoxMultiplier.Controls.Add(this.label3);
            this.groupBoxMultiplier.Controls.Add(this.label2);
            this.groupBoxMultiplier.Controls.Add(this.buttonAllToOne);
            this.groupBoxMultiplier.Controls.Add(this.buttonSetToOfficial);
            this.groupBoxMultiplier.Controls.Add(this.label1);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingTo);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingSp);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingDm);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingWe);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingFo);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingOx);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingSt);
            this.groupBoxMultiplier.Controls.Add(this.multiplierSettingHP);
            this.groupBoxMultiplier.Location = new System.Drawing.Point(12, 12);
            this.groupBoxMultiplier.Name = "groupBoxMultiplier";
            this.groupBoxMultiplier.Size = new System.Drawing.Size(304, 348);
            this.groupBoxMultiplier.TabIndex = 0;
            this.groupBoxMultiplier.TabStop = false;
            this.groupBoxMultiplier.Text = "Stat-Multipliers";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 314);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(290, 32);
            this.label7.TabIndex = 15;
            this.label7.Text = "To set the multipliers to the official values, load the file multipliers.txt via " +
    "the File - Load Multiplier-file";
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(6, 16);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(288, 31);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "The multipliers are saved with each library. If the server you play on changes it" +
    "s multipliers, you can adjust them here.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(234, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "DomLevel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "WildLevel";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "TameAff";
            // 
            // buttonAllToOne
            // 
            this.buttonAllToOne.Location = new System.Drawing.Point(58, 288);
            this.buttonAllToOne.Name = "buttonAllToOne";
            this.buttonAllToOne.Size = new System.Drawing.Size(87, 23);
            this.buttonAllToOne.TabIndex = 13;
            this.buttonAllToOne.Text = "Set all to 1";
            this.buttonAllToOne.UseVisualStyleBackColor = true;
            this.buttonAllToOne.Click += new System.EventHandler(this.buttonAllToOne_Click);
            // 
            // buttonSetToOfficial
            // 
            this.buttonSetToOfficial.Location = new System.Drawing.Point(151, 288);
            this.buttonSetToOfficial.Name = "buttonSetToOfficial";
            this.buttonSetToOfficial.Size = new System.Drawing.Size(87, 23);
            this.buttonSetToOfficial.TabIndex = 14;
            this.buttonSetToOfficial.Text = "Set to official";
            this.buttonSetToOfficial.UseVisualStyleBackColor = true;
            this.buttonSetToOfficial.Visible = false;
            this.buttonSetToOfficial.Click += new System.EventHandler(this.buttonSetToOfficial_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "TameAdd";
            // 
            // multiplierSettingTo
            // 
            this.multiplierSettingTo.Location = new System.Drawing.Point(6, 256);
            this.multiplierSettingTo.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingTo.Name = "multiplierSettingTo";
            this.multiplierSettingTo.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingTo.TabIndex = 12;
            // 
            // multiplierSettingSp
            // 
            this.multiplierSettingSp.Location = new System.Drawing.Point(6, 230);
            this.multiplierSettingSp.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingSp.Name = "multiplierSettingSp";
            this.multiplierSettingSp.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingSp.TabIndex = 11;
            // 
            // multiplierSettingDm
            // 
            this.multiplierSettingDm.Location = new System.Drawing.Point(6, 204);
            this.multiplierSettingDm.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingDm.Name = "multiplierSettingDm";
            this.multiplierSettingDm.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingDm.TabIndex = 10;
            // 
            // multiplierSettingWe
            // 
            this.multiplierSettingWe.Location = new System.Drawing.Point(6, 178);
            this.multiplierSettingWe.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingWe.Name = "multiplierSettingWe";
            this.multiplierSettingWe.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingWe.TabIndex = 9;
            // 
            // multiplierSettingFo
            // 
            this.multiplierSettingFo.Location = new System.Drawing.Point(6, 152);
            this.multiplierSettingFo.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingFo.Name = "multiplierSettingFo";
            this.multiplierSettingFo.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingFo.TabIndex = 8;
            // 
            // multiplierSettingOx
            // 
            this.multiplierSettingOx.Location = new System.Drawing.Point(6, 126);
            this.multiplierSettingOx.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingOx.Name = "multiplierSettingOx";
            this.multiplierSettingOx.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingOx.TabIndex = 7;
            // 
            // multiplierSettingSt
            // 
            this.multiplierSettingSt.Location = new System.Drawing.Point(6, 100);
            this.multiplierSettingSt.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingSt.Name = "multiplierSettingSt";
            this.multiplierSettingSt.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingSt.TabIndex = 6;
            // 
            // multiplierSettingHP
            // 
            this.multiplierSettingHP.Location = new System.Drawing.Point(6, 74);
            this.multiplierSettingHP.Multipliers = new double[] {
        1D,
        1D,
        1D,
        1D};
            this.multiplierSettingHP.Name = "multiplierSettingHP";
            this.multiplierSettingHP.Size = new System.Drawing.Size(288, 26);
            this.multiplierSettingHP.TabIndex = 5;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(484, 373);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(403, 373);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoSave
            // 
            this.checkBoxAutoSave.AutoSize = true;
            this.checkBoxAutoSave.Location = new System.Drawing.Point(6, 19);
            this.checkBoxAutoSave.Name = "checkBoxAutoSave";
            this.checkBoxAutoSave.Size = new System.Drawing.Size(71, 17);
            this.checkBoxAutoSave.TabIndex = 0;
            this.checkBoxAutoSave.Text = "Autosave";
            this.checkBoxAutoSave.UseVisualStyleBackColor = true;
            this.checkBoxAutoSave.CheckedChanged += new System.EventHandler(this.checkBoxAutoSave_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkExperimentalOCR);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDownAutosaveMinutes);
            this.groupBox1.Controls.Add(this.checkBoxAutoSave);
            this.groupBox1.Location = new System.Drawing.Point(322, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 114);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // chkExperimentalOCR
            // 
            this.chkExperimentalOCR.AutoSize = true;
            this.chkExperimentalOCR.Location = new System.Drawing.Point(6, 83);
            this.chkExperimentalOCR.Name = "chkExperimentalOCR";
            this.chkExperimentalOCR.Size = new System.Drawing.Size(182, 17);
            this.chkExperimentalOCR.TabIndex = 6;
            this.chkExperimentalOCR.Text = "Experimental OCR (needs restart)";
            this.chkExperimentalOCR.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Create Backupfile every";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(181, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Minutes";
            // 
            // numericUpDownAutosaveMinutes
            // 
            this.numericUpDownAutosaveMinutes.Enabled = false;
            this.numericUpDownAutosaveMinutes.Location = new System.Drawing.Point(132, 42);
            this.numericUpDownAutosaveMinutes.Name = "numericUpDownAutosaveMinutes";
            this.numericUpDownAutosaveMinutes.Size = new System.Drawing.Size(43, 20);
            this.numericUpDownAutosaveMinutes.TabIndex = 1;
            this.numericUpDownAutosaveMinutes.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.numericUpDownMaturation);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericUpDownHatching);
            this.groupBox2.Location = new System.Drawing.Point(322, 132);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 81);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Breeding-Speed-Multiplier";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Maturation";
            // 
            // numericUpDownMaturation
            // 
            this.numericUpDownMaturation.DecimalPlaces = 2;
            this.numericUpDownMaturation.Location = new System.Drawing.Point(167, 45);
            this.numericUpDownMaturation.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownMaturation.Name = "numericUpDownMaturation";
            this.numericUpDownMaturation.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownMaturation.TabIndex = 3;
            this.numericUpDownMaturation.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaturation.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Hatching";
            // 
            // numericUpDownHatching
            // 
            this.numericUpDownHatching.DecimalPlaces = 2;
            this.numericUpDownHatching.Location = new System.Drawing.Point(167, 19);
            this.numericUpDownHatching.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownHatching.Name = "numericUpDownHatching";
            this.numericUpDownHatching.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownHatching.TabIndex = 1;
            this.numericUpDownHatching.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownHatching.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.numericUpDownMaxWildLevel);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.numericUpDownDomLevelNr);
            this.groupBox3.Location = new System.Drawing.Point(322, 219);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 84);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Maximal Levels on Server";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Max Wild Level";
            // 
            // numericUpDownMaxWildLevel
            // 
            this.numericUpDownMaxWildLevel.Location = new System.Drawing.Point(167, 19);
            this.numericUpDownMaxWildLevel.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownMaxWildLevel.Name = "numericUpDownMaxWildLevel";
            this.numericUpDownMaxWildLevel.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownMaxWildLevel.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Max Dom Levelups";
            // 
            // numericUpDownDomLevelNr
            // 
            this.numericUpDownDomLevelNr.Location = new System.Drawing.Point(167, 45);
            this.numericUpDownDomLevelNr.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownDomLevelNr.Name = "numericUpDownDomLevelNr";
            this.numericUpDownDomLevelNr.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownDomLevelNr.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.numericUpDownMaxBreedingSug);
            this.groupBox4.Location = new System.Drawing.Point(322, 309);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(230, 51);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Breeding Planner";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(154, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Max Breeding Pair Suggestions";
            // 
            // numericUpDownMaxBreedingSug
            // 
            this.numericUpDownMaxBreedingSug.Location = new System.Drawing.Point(167, 19);
            this.numericUpDownMaxBreedingSug.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownMaxBreedingSug.Name = "numericUpDownMaxBreedingSug";
            this.numericUpDownMaxBreedingSug.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownMaxBreedingSug.TabIndex = 5;
            // 
            // Settings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(571, 408);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxMultiplier);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.groupBoxMultiplier.ResumeLayout(false);
            this.groupBoxMultiplier.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAutosaveMinutes)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHatching)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxWildLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDomLevelNr)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxBreedingSug)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMultiplier;
        private MultiplierSetting multiplierSettingTo;
        private MultiplierSetting multiplierSettingSp;
        private MultiplierSetting multiplierSettingDm;
        private MultiplierSetting multiplierSettingWe;
        private MultiplierSetting multiplierSettingFo;
        private MultiplierSetting multiplierSettingOx;
        private MultiplierSetting multiplierSettingSt;
        private MultiplierSetting multiplierSettingHP;
        private System.Windows.Forms.Button buttonAllToOne;
        private System.Windows.Forms.Button buttonSetToOfficial;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.CheckBox checkBoxAutoSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownAutosaveMinutes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDownHatching;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDownMaturation;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownDomLevelNr;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxWildLevel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxBreedingSug;
        private System.Windows.Forms.CheckBox chkExperimentalOCR;
    }
}
namespace ARKBreedingStats
{
    partial class TamingControl
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
            this.labelResult = new System.Windows.Forms.Label();
            this.nudLevel = new ARKBreedingStats.uiControls.Nud();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.buttonAddTorporTimer = new System.Windows.Forms.Button();
            this.labelTimeUntilWakingUp = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownCurrentTorpor = new ARKBreedingStats.uiControls.Nud();
            this.label4 = new System.Windows.Forms.Label();
            this.nudWDmLongneck = new ARKBreedingStats.uiControls.Nud();
            this.gbWeaponDamage = new System.Windows.Forms.GroupBox();
            this.rbBoneDamageDefault = new System.Windows.Forms.RadioButton();
            this.nudWDmProd = new ARKBreedingStats.uiControls.Nud();
            this.chkbDmCrossbow = new System.Windows.Forms.CheckBox();
            this.chkbDmBow = new System.Windows.Forms.CheckBox();
            this.chkbDmSlingshot = new System.Windows.Forms.CheckBox();
            this.chkbDmClub = new System.Windows.Forms.CheckBox();
            this.chkbDmLongneck = new System.Windows.Forms.CheckBox();
            this.nudWDmSlingshot = new ARKBreedingStats.uiControls.Nud();
            this.chkbDmProd = new System.Windows.Forms.CheckBox();
            this.nudWDmClub = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmBow = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmCrossbow = new ARKBreedingStats.uiControls.Nud();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelKOCount = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnAddStarvingTimer = new System.Windows.Forms.Button();
            this.lblTimeUntilStarving = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).BeginInit();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCurrentTorpor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmLongneck)).BeginInit();
            this.gbWeaponDamage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmProd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmSlingshot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmClub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmBow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmCrossbow)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelResult
            // 
            this.labelResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelResult.Location = new System.Drawing.Point(3, 16);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(340, 385);
            this.labelResult.TabIndex = 6;
            this.labelResult.Text = "Results";
            // 
            // nudLevel
            // 
            this.nudLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudLevel.Location = new System.Drawing.Point(42, 1);
            this.nudLevel.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLevel.Name = "nudLevel";
            this.nudLevel.Size = new System.Drawing.Size(56, 20);
            this.nudLevel.TabIndex = 2;
            this.nudLevel.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudLevel.ValueChanged += new System.EventHandler(this.nudLevel_ValueChanged);
            this.nudLevel.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Level";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(131, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "max";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(194, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "used";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.buttonAddTorporTimer);
            this.groupBox9.Controls.Add(this.labelTimeUntilWakingUp);
            this.groupBox9.Controls.Add(this.label9);
            this.groupBox9.Controls.Add(this.numericUpDownCurrentTorpor);
            this.groupBox9.Location = new System.Drawing.Point(339, 3);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(170, 94);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Torpor-Time";
            // 
            // buttonAddTorporTimer
            // 
            this.buttonAddTorporTimer.Location = new System.Drawing.Point(6, 65);
            this.buttonAddTorporTimer.Name = "buttonAddTorporTimer";
            this.buttonAddTorporTimer.Size = new System.Drawing.Size(158, 23);
            this.buttonAddTorporTimer.TabIndex = 3;
            this.buttonAddTorporTimer.Text = "add Wake-up-Timer";
            this.buttonAddTorporTimer.UseVisualStyleBackColor = true;
            this.buttonAddTorporTimer.Click += new System.EventHandler(this.buttonAddTorporTimer_Click);
            // 
            // labelTimeUntilWakingUp
            // 
            this.labelTimeUntilWakingUp.AutoSize = true;
            this.labelTimeUntilWakingUp.Location = new System.Drawing.Point(6, 37);
            this.labelTimeUntilWakingUp.Name = "labelTimeUntilWakingUp";
            this.labelTimeUntilWakingUp.Size = new System.Drawing.Size(99, 13);
            this.labelTimeUntilWakingUp.TabIndex = 2;
            this.labelTimeUntilWakingUp.Text = "Time until wake-up:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Current Torpor";
            // 
            // numericUpDownCurrentTorpor
            // 
            this.numericUpDownCurrentTorpor.DecimalPlaces = 1;
            this.numericUpDownCurrentTorpor.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownCurrentTorpor.Location = new System.Drawing.Point(87, 14);
            this.numericUpDownCurrentTorpor.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownCurrentTorpor.Name = "numericUpDownCurrentTorpor";
            this.numericUpDownCurrentTorpor.Size = new System.Drawing.Size(77, 20);
            this.numericUpDownCurrentTorpor.TabIndex = 0;
            this.numericUpDownCurrentTorpor.ValueChanged += new System.EventHandler(this.numericUpDownCurrentTorpor_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(239, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "taming time";
            // 
            // nudWDmLongneck
            // 
            this.nudWDmLongneck.DecimalPlaces = 1;
            this.nudWDmLongneck.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudWDmLongneck.Location = new System.Drawing.Point(147, 45);
            this.nudWDmLongneck.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWDmLongneck.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmLongneck.Name = "nudWDmLongneck";
            this.nudWDmLongneck.Size = new System.Drawing.Size(53, 20);
            this.nudWDmLongneck.TabIndex = 3;
            this.nudWDmLongneck.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmLongneck.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // gbWeaponDamage
            // 
            this.gbWeaponDamage.Controls.Add(this.rbBoneDamageDefault);
            this.gbWeaponDamage.Controls.Add(this.nudWDmProd);
            this.gbWeaponDamage.Controls.Add(this.chkbDmCrossbow);
            this.gbWeaponDamage.Controls.Add(this.chkbDmBow);
            this.gbWeaponDamage.Controls.Add(this.chkbDmSlingshot);
            this.gbWeaponDamage.Controls.Add(this.chkbDmClub);
            this.gbWeaponDamage.Controls.Add(this.chkbDmLongneck);
            this.gbWeaponDamage.Controls.Add(this.nudWDmSlingshot);
            this.gbWeaponDamage.Controls.Add(this.chkbDmProd);
            this.gbWeaponDamage.Controls.Add(this.nudWDmClub);
            this.gbWeaponDamage.Controls.Add(this.nudWDmBow);
            this.gbWeaponDamage.Controls.Add(this.nudWDmCrossbow);
            this.gbWeaponDamage.Controls.Add(this.nudWDmLongneck);
            this.gbWeaponDamage.Location = new System.Drawing.Point(691, 3);
            this.gbWeaponDamage.Name = "gbWeaponDamage";
            this.gbWeaponDamage.Size = new System.Drawing.Size(206, 251);
            this.gbWeaponDamage.TabIndex = 7;
            this.gbWeaponDamage.TabStop = false;
            this.gbWeaponDamage.Text = "Weapon-Damage [%]";
            // 
            // rbBoneDamageDefault
            // 
            this.rbBoneDamageDefault.Location = new System.Drawing.Point(6, 173);
            this.rbBoneDamageDefault.Name = "rbBoneDamageDefault";
            this.rbBoneDamageDefault.Size = new System.Drawing.Size(194, 17);
            this.rbBoneDamageDefault.TabIndex = 12;
            this.rbBoneDamageDefault.TabStop = true;
            this.rbBoneDamageDefault.Text = "Default (1×)";
            this.rbBoneDamageDefault.UseVisualStyleBackColor = true;
            this.rbBoneDamageDefault.CheckedChanged += new System.EventHandler(this.rbBoneDamage_CheckedChanged);
            // 
            // nudWDmProd
            // 
            this.nudWDmProd.DecimalPlaces = 1;
            this.nudWDmProd.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudWDmProd.Location = new System.Drawing.Point(147, 19);
            this.nudWDmProd.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWDmProd.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmProd.Name = "nudWDmProd";
            this.nudWDmProd.Size = new System.Drawing.Size(53, 20);
            this.nudWDmProd.TabIndex = 1;
            this.nudWDmProd.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmProd.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // chkbDmCrossbow
            // 
            this.chkbDmCrossbow.AutoSize = true;
            this.chkbDmCrossbow.Checked = true;
            this.chkbDmCrossbow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbDmCrossbow.Location = new System.Drawing.Point(6, 72);
            this.chkbDmCrossbow.Name = "chkbDmCrossbow";
            this.chkbDmCrossbow.Size = new System.Drawing.Size(72, 17);
            this.chkbDmCrossbow.TabIndex = 4;
            this.chkbDmCrossbow.Text = "Crossbow";
            this.chkbDmCrossbow.UseVisualStyleBackColor = true;
            this.chkbDmCrossbow.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmBow
            // 
            this.chkbDmBow.AutoSize = true;
            this.chkbDmBow.Location = new System.Drawing.Point(6, 98);
            this.chkbDmBow.Name = "chkbDmBow";
            this.chkbDmBow.Size = new System.Drawing.Size(47, 17);
            this.chkbDmBow.TabIndex = 6;
            this.chkbDmBow.Text = "Bow";
            this.chkbDmBow.UseVisualStyleBackColor = true;
            this.chkbDmBow.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmSlingshot
            // 
            this.chkbDmSlingshot.AutoSize = true;
            this.chkbDmSlingshot.Location = new System.Drawing.Point(6, 124);
            this.chkbDmSlingshot.Name = "chkbDmSlingshot";
            this.chkbDmSlingshot.Size = new System.Drawing.Size(69, 17);
            this.chkbDmSlingshot.TabIndex = 8;
            this.chkbDmSlingshot.Text = "Slingshot";
            this.chkbDmSlingshot.UseVisualStyleBackColor = true;
            this.chkbDmSlingshot.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmClub
            // 
            this.chkbDmClub.AutoSize = true;
            this.chkbDmClub.Location = new System.Drawing.Point(6, 150);
            this.chkbDmClub.Name = "chkbDmClub";
            this.chkbDmClub.Size = new System.Drawing.Size(47, 17);
            this.chkbDmClub.TabIndex = 10;
            this.chkbDmClub.Text = "Club";
            this.chkbDmClub.UseVisualStyleBackColor = true;
            this.chkbDmClub.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmLongneck
            // 
            this.chkbDmLongneck.AutoSize = true;
            this.chkbDmLongneck.Checked = true;
            this.chkbDmLongneck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbDmLongneck.Location = new System.Drawing.Point(6, 46);
            this.chkbDmLongneck.Name = "chkbDmLongneck";
            this.chkbDmLongneck.Size = new System.Drawing.Size(74, 17);
            this.chkbDmLongneck.TabIndex = 2;
            this.chkbDmLongneck.Text = "Longneck";
            this.chkbDmLongneck.UseVisualStyleBackColor = true;
            this.chkbDmLongneck.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // nudWDmSlingshot
            // 
            this.nudWDmSlingshot.DecimalPlaces = 1;
            this.nudWDmSlingshot.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudWDmSlingshot.Location = new System.Drawing.Point(147, 123);
            this.nudWDmSlingshot.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWDmSlingshot.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmSlingshot.Name = "nudWDmSlingshot";
            this.nudWDmSlingshot.Size = new System.Drawing.Size(53, 20);
            this.nudWDmSlingshot.TabIndex = 9;
            this.nudWDmSlingshot.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmSlingshot.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // chkbDmProd
            // 
            this.chkbDmProd.AutoSize = true;
            this.chkbDmProd.Location = new System.Drawing.Point(6, 20);
            this.chkbDmProd.Name = "chkbDmProd";
            this.chkbDmProd.Size = new System.Drawing.Size(86, 17);
            this.chkbDmProd.TabIndex = 0;
            this.chkbDmProd.Text = "Electric Prod";
            this.chkbDmProd.UseVisualStyleBackColor = true;
            this.chkbDmProd.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // nudWDmClub
            // 
            this.nudWDmClub.DecimalPlaces = 1;
            this.nudWDmClub.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudWDmClub.Location = new System.Drawing.Point(147, 149);
            this.nudWDmClub.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWDmClub.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmClub.Name = "nudWDmClub";
            this.nudWDmClub.Size = new System.Drawing.Size(53, 20);
            this.nudWDmClub.TabIndex = 11;
            this.nudWDmClub.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmClub.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // nudWDmBow
            // 
            this.nudWDmBow.DecimalPlaces = 1;
            this.nudWDmBow.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudWDmBow.Location = new System.Drawing.Point(147, 97);
            this.nudWDmBow.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWDmBow.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmBow.Name = "nudWDmBow";
            this.nudWDmBow.Size = new System.Drawing.Size(53, 20);
            this.nudWDmBow.TabIndex = 7;
            this.nudWDmBow.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmBow.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // nudWDmCrossbow
            // 
            this.nudWDmCrossbow.DecimalPlaces = 1;
            this.nudWDmCrossbow.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudWDmCrossbow.Location = new System.Drawing.Point(147, 71);
            this.nudWDmCrossbow.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWDmCrossbow.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmCrossbow.Name = "nudWDmCrossbow";
            this.nudWDmCrossbow.Size = new System.Drawing.Size(53, 20);
            this.nudWDmCrossbow.TabIndex = 5;
            this.nudWDmCrossbow.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmCrossbow.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelKOCount);
            this.groupBox2.Location = new System.Drawing.Point(691, 260);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(206, 296);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "KO-Counting";
            // 
            // labelKOCount
            // 
            this.labelKOCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKOCount.Location = new System.Drawing.Point(3, 16);
            this.labelKOCount.Name = "labelKOCount";
            this.labelKOCount.Size = new System.Drawing.Size(200, 277);
            this.labelKOCount.TabIndex = 0;
            this.labelKOCount.Text = "KO";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelResult);
            this.groupBox3.Location = new System.Drawing.Point(339, 152);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(346, 404);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Taming-Information";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnAddStarvingTimer);
            this.groupBox4.Controls.Add(this.lblTimeUntilStarving);
            this.groupBox4.Location = new System.Drawing.Point(515, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(170, 94);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Starving-Time";
            // 
            // btnAddStarvingTimer
            // 
            this.btnAddStarvingTimer.Location = new System.Drawing.Point(6, 65);
            this.btnAddStarvingTimer.Name = "btnAddStarvingTimer";
            this.btnAddStarvingTimer.Size = new System.Drawing.Size(158, 23);
            this.btnAddStarvingTimer.TabIndex = 3;
            this.btnAddStarvingTimer.Text = "add Starving-Timer";
            this.btnAddStarvingTimer.UseVisualStyleBackColor = true;
            this.btnAddStarvingTimer.Click += new System.EventHandler(this.btnAddStarvingTimer_Click);
            // 
            // lblTimeUntilStarving
            // 
            this.lblTimeUntilStarving.Location = new System.Drawing.Point(6, 16);
            this.lblTimeUntilStarving.Name = "lblTimeUntilStarving";
            this.lblTimeUntilStarving.Size = new System.Drawing.Size(158, 46);
            this.lblTimeUntilStarving.TabIndex = 2;
            this.lblTimeUntilStarving.Text = "Time until starving:";
            // 
            // TamingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbWeaponDamage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudLevel);
            this.Name = "TamingControl";
            this.Size = new System.Drawing.Size(933, 617);
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCurrentTorpor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmLongneck)).EndInit();
            this.gbWeaponDamage.ResumeLayout(false);
            this.gbWeaponDamage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmProd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmSlingshot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmClub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmBow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmCrossbow)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelResult;
        private uiControls.Nud nudLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label labelTimeUntilWakingUp;
        private System.Windows.Forms.Label label9;
        private uiControls.Nud numericUpDownCurrentTorpor;
        private System.Windows.Forms.Label label4;
        private uiControls.Nud nudWDmLongneck;
        private System.Windows.Forms.GroupBox gbWeaponDamage;
        private uiControls.Nud nudWDmClub;
        private uiControls.Nud nudWDmBow;
        private uiControls.Nud nudWDmCrossbow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelKOCount;
        private uiControls.Nud nudWDmSlingshot;
        private System.Windows.Forms.Button buttonAddTorporTimer;
        private uiControls.Nud nudWDmProd;
        private System.Windows.Forms.CheckBox chkbDmCrossbow;
        private System.Windows.Forms.CheckBox chkbDmBow;
        private System.Windows.Forms.CheckBox chkbDmSlingshot;
        private System.Windows.Forms.CheckBox chkbDmClub;
        private System.Windows.Forms.CheckBox chkbDmProd;
        private System.Windows.Forms.CheckBox chkbDmLongneck;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnAddStarvingTimer;
        private System.Windows.Forms.Label lblTimeUntilStarving;
        private System.Windows.Forms.RadioButton rbBoneDamageDefault;
    }
}

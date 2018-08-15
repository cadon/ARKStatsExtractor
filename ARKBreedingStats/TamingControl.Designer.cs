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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TamingControl));
            this.labelResult = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbMax = new System.Windows.Forms.Label();
            this.lbUsed = new System.Windows.Forms.Label();
            this.gpTorporTime = new System.Windows.Forms.GroupBox();
            this.btAddWakeUpTimer = new System.Windows.Forms.Button();
            this.lbTimeUntilWakingUp = new System.Windows.Forms.Label();
            this.lbCurrentTorpor = new System.Windows.Forms.Label();
            this.lbTamingTime = new System.Windows.Forms.Label();
            this.gbWeaponDamage = new System.Windows.Forms.GroupBox();
            this.chkbDmHarpoon = new System.Windows.Forms.CheckBox();
            this.rbBoneDamageDefault = new System.Windows.Forms.RadioButton();
            this.chkbDmCrossbow = new System.Windows.Forms.CheckBox();
            this.chkbDmBow = new System.Windows.Forms.CheckBox();
            this.chkbDmSlingshot = new System.Windows.Forms.CheckBox();
            this.chkbDmClub = new System.Windows.Forms.CheckBox();
            this.chkbDmLongneck = new System.Windows.Forms.CheckBox();
            this.chkbDmProd = new System.Windows.Forms.CheckBox();
            this.gbKOInfo = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gpStarvingTime = new System.Windows.Forms.GroupBox();
            this.btnAddStarvingTimer = new System.Windows.Forms.Button();
            this.lbTimeUntilStarving = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.nudLevel = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmHarpoon = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmProd = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmSlingshot = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmClub = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmBow = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmCrossbow = new ARKBreedingStats.uiControls.Nud();
            this.nudWDmLongneck = new ARKBreedingStats.uiControls.Nud();
            this.numericUpDownCurrentTorpor = new ARKBreedingStats.uiControls.Nud();
            this.lbKOInfo = new System.Windows.Forms.Label();
            this.gpTorporTime.SuspendLayout();
            this.gbWeaponDamage.SuspendLayout();
            this.gbKOInfo.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gpStarvingTime.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmHarpoon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmProd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmSlingshot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmClub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmBow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmCrossbow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmLongneck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCurrentTorpor)).BeginInit();
            this.SuspendLayout();
            // 
            // labelResult
            // 
            resources.ApplyResources(this.labelResult, "labelResult");
            this.labelResult.Name = "labelResult";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lbMax
            // 
            resources.ApplyResources(this.lbMax, "lbMax");
            this.lbMax.Name = "lbMax";
            // 
            // lbUsed
            // 
            resources.ApplyResources(this.lbUsed, "lbUsed");
            this.lbUsed.Name = "lbUsed";
            // 
            // gpTorporTime
            // 
            this.gpTorporTime.Controls.Add(this.numericUpDownCurrentTorpor);
            this.gpTorporTime.Controls.Add(this.btAddWakeUpTimer);
            this.gpTorporTime.Controls.Add(this.lbTimeUntilWakingUp);
            this.gpTorporTime.Controls.Add(this.lbCurrentTorpor);
            resources.ApplyResources(this.gpTorporTime, "gpTorporTime");
            this.gpTorporTime.Name = "gpTorporTime";
            this.gpTorporTime.TabStop = false;
            // 
            // btAddWakeUpTimer
            // 
            resources.ApplyResources(this.btAddWakeUpTimer, "btAddWakeUpTimer");
            this.btAddWakeUpTimer.Name = "btAddWakeUpTimer";
            this.btAddWakeUpTimer.UseVisualStyleBackColor = true;
            this.btAddWakeUpTimer.Click += new System.EventHandler(this.buttonAddTorporTimer_Click);
            // 
            // lbTimeUntilWakingUp
            // 
            resources.ApplyResources(this.lbTimeUntilWakingUp, "lbTimeUntilWakingUp");
            this.lbTimeUntilWakingUp.Name = "lbTimeUntilWakingUp";
            // 
            // lbCurrentTorpor
            // 
            resources.ApplyResources(this.lbCurrentTorpor, "lbCurrentTorpor");
            this.lbCurrentTorpor.Name = "lbCurrentTorpor";
            // 
            // lbTamingTime
            // 
            resources.ApplyResources(this.lbTamingTime, "lbTamingTime");
            this.lbTamingTime.Name = "lbTamingTime";
            // 
            // gbWeaponDamage
            // 
            this.gbWeaponDamage.Controls.Add(this.nudWDmHarpoon);
            this.gbWeaponDamage.Controls.Add(this.chkbDmHarpoon);
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
            resources.ApplyResources(this.gbWeaponDamage, "gbWeaponDamage");
            this.gbWeaponDamage.Name = "gbWeaponDamage";
            this.tableLayoutPanel1.SetRowSpan(this.gbWeaponDamage, 2);
            this.gbWeaponDamage.TabStop = false;
            // 
            // chkbDmHarpoon
            // 
            resources.ApplyResources(this.chkbDmHarpoon, "chkbDmHarpoon");
            this.chkbDmHarpoon.Name = "chkbDmHarpoon";
            this.chkbDmHarpoon.UseVisualStyleBackColor = true;
            this.chkbDmHarpoon.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // rbBoneDamageDefault
            // 
            resources.ApplyResources(this.rbBoneDamageDefault, "rbBoneDamageDefault");
            this.rbBoneDamageDefault.Name = "rbBoneDamageDefault";
            this.rbBoneDamageDefault.TabStop = true;
            this.rbBoneDamageDefault.UseVisualStyleBackColor = true;
            this.rbBoneDamageDefault.CheckedChanged += new System.EventHandler(this.rbBoneDamage_CheckedChanged);
            // 
            // chkbDmCrossbow
            // 
            resources.ApplyResources(this.chkbDmCrossbow, "chkbDmCrossbow");
            this.chkbDmCrossbow.Checked = true;
            this.chkbDmCrossbow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbDmCrossbow.Name = "chkbDmCrossbow";
            this.chkbDmCrossbow.UseVisualStyleBackColor = true;
            this.chkbDmCrossbow.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmBow
            // 
            resources.ApplyResources(this.chkbDmBow, "chkbDmBow");
            this.chkbDmBow.Name = "chkbDmBow";
            this.chkbDmBow.UseVisualStyleBackColor = true;
            this.chkbDmBow.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmSlingshot
            // 
            resources.ApplyResources(this.chkbDmSlingshot, "chkbDmSlingshot");
            this.chkbDmSlingshot.Name = "chkbDmSlingshot";
            this.chkbDmSlingshot.UseVisualStyleBackColor = true;
            this.chkbDmSlingshot.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmClub
            // 
            resources.ApplyResources(this.chkbDmClub, "chkbDmClub");
            this.chkbDmClub.Name = "chkbDmClub";
            this.chkbDmClub.UseVisualStyleBackColor = true;
            this.chkbDmClub.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmLongneck
            // 
            resources.ApplyResources(this.chkbDmLongneck, "chkbDmLongneck");
            this.chkbDmLongneck.Checked = true;
            this.chkbDmLongneck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbDmLongneck.Name = "chkbDmLongneck";
            this.chkbDmLongneck.UseVisualStyleBackColor = true;
            this.chkbDmLongneck.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // chkbDmProd
            // 
            resources.ApplyResources(this.chkbDmProd, "chkbDmProd");
            this.chkbDmProd.Name = "chkbDmProd";
            this.chkbDmProd.UseVisualStyleBackColor = true;
            this.chkbDmProd.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
            // 
            // gbKOInfo
            // 
            this.gbKOInfo.Controls.Add(this.lbKOInfo);
            resources.ApplyResources(this.gbKOInfo, "gbKOInfo");
            this.gbKOInfo.Name = "gbKOInfo";
            this.gbKOInfo.TabStop = false;
            // 
            // groupBox3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.labelResult);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.tableLayoutPanel1.SetRowSpan(this.groupBox3, 2);
            this.groupBox3.TabStop = false;
            // 
            // gpStarvingTime
            // 
            this.gpStarvingTime.Controls.Add(this.btnAddStarvingTimer);
            this.gpStarvingTime.Controls.Add(this.lbTimeUntilStarving);
            resources.ApplyResources(this.gpStarvingTime, "gpStarvingTime");
            this.gpStarvingTime.Name = "gpStarvingTime";
            this.gpStarvingTime.TabStop = false;
            // 
            // btnAddStarvingTimer
            // 
            resources.ApplyResources(this.btnAddStarvingTimer, "btnAddStarvingTimer");
            this.btnAddStarvingTimer.Name = "btnAddStarvingTimer";
            this.btnAddStarvingTimer.UseVisualStyleBackColor = true;
            this.btnAddStarvingTimer.Click += new System.EventHandler(this.btnAddStarvingTimer_Click);
            // 
            // lbTimeUntilStarving
            // 
            resources.ApplyResources(this.lbTimeUntilStarving, "lbTimeUntilStarving");
            this.lbTimeUntilStarving.Name = "lbTimeUntilStarving";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.gpTorporTime, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gbKOInfo, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gbWeaponDamage, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.gpStarvingTime, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.lbMax);
            this.panel1.Controls.Add(this.lbUsed);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.nudLevel);
            this.panel1.Controls.Add(this.lbTamingTime);
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // nudLevel
            // 
            this.nudLevel.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.nudLevel, "nudLevel");
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
            this.nudLevel.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudLevel.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudLevel.ValueChanged += new System.EventHandler(this.nudLevel_ValueChanged);
            // 
            // nudWDmHarpoon
            // 
            this.nudWDmHarpoon.DecimalPlaces = 1;
            this.nudWDmHarpoon.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.nudWDmHarpoon, "nudWDmHarpoon");
            this.nudWDmHarpoon.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudWDmHarpoon.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmHarpoon.Name = "nudWDmHarpoon";
            this.nudWDmHarpoon.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWDmHarpoon.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmHarpoon.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // nudWDmProd
            // 
            this.nudWDmProd.DecimalPlaces = 1;
            this.nudWDmProd.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.nudWDmProd, "nudWDmProd");
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
            this.nudWDmProd.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWDmProd.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmProd.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // nudWDmSlingshot
            // 
            this.nudWDmSlingshot.DecimalPlaces = 1;
            this.nudWDmSlingshot.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.nudWDmSlingshot, "nudWDmSlingshot");
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
            this.nudWDmSlingshot.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWDmSlingshot.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmSlingshot.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // nudWDmClub
            // 
            this.nudWDmClub.DecimalPlaces = 1;
            this.nudWDmClub.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.nudWDmClub, "nudWDmClub");
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
            this.nudWDmClub.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
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
            resources.ApplyResources(this.nudWDmBow, "nudWDmBow");
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
            this.nudWDmBow.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
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
            resources.ApplyResources(this.nudWDmCrossbow, "nudWDmCrossbow");
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
            this.nudWDmCrossbow.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWDmCrossbow.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmCrossbow.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // nudWDmLongneck
            // 
            this.nudWDmLongneck.DecimalPlaces = 1;
            this.nudWDmLongneck.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.nudWDmLongneck, "nudWDmLongneck");
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
            this.nudWDmLongneck.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudWDmLongneck.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWDmLongneck.ValueChanged += new System.EventHandler(this.nudWDm_ValueChanged);
            // 
            // numericUpDownCurrentTorpor
            // 
            this.numericUpDownCurrentTorpor.DecimalPlaces = 1;
            this.numericUpDownCurrentTorpor.ForeColor = System.Drawing.SystemColors.GrayText;
            this.numericUpDownCurrentTorpor.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.numericUpDownCurrentTorpor, "numericUpDownCurrentTorpor");
            this.numericUpDownCurrentTorpor.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownCurrentTorpor.Name = "numericUpDownCurrentTorpor";
            this.numericUpDownCurrentTorpor.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownCurrentTorpor.ValueChanged += new System.EventHandler(this.numericUpDownCurrentTorpor_ValueChanged);
            // 
            // lbKOInfo
            // 
            resources.ApplyResources(this.lbKOInfo, "lbKOInfo");
            this.lbKOInfo.Name = "lbKOInfo";
            // 
            // TamingControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "TamingControl";
            this.gpTorporTime.ResumeLayout(false);
            this.gpTorporTime.PerformLayout();
            this.gbWeaponDamage.ResumeLayout(false);
            this.gbWeaponDamage.PerformLayout();
            this.gbKOInfo.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.gpStarvingTime.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmHarpoon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmProd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmSlingshot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmClub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmBow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmCrossbow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmLongneck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCurrentTorpor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelResult;
        private uiControls.Nud nudLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbMax;
        private System.Windows.Forms.Label lbUsed;
        private System.Windows.Forms.GroupBox gpTorporTime;
        private System.Windows.Forms.Label lbTimeUntilWakingUp;
        private System.Windows.Forms.Label lbCurrentTorpor;
        private uiControls.Nud numericUpDownCurrentTorpor;
        private System.Windows.Forms.Label lbTamingTime;
        private uiControls.Nud nudWDmLongneck;
        private System.Windows.Forms.GroupBox gbWeaponDamage;
        private uiControls.Nud nudWDmClub;
        private uiControls.Nud nudWDmBow;
        private uiControls.Nud nudWDmCrossbow;
        private System.Windows.Forms.GroupBox gbKOInfo;
        private uiControls.Nud nudWDmSlingshot;
        private System.Windows.Forms.Button btAddWakeUpTimer;
        private uiControls.Nud nudWDmProd;
        private System.Windows.Forms.CheckBox chkbDmCrossbow;
        private System.Windows.Forms.CheckBox chkbDmBow;
        private System.Windows.Forms.CheckBox chkbDmSlingshot;
        private System.Windows.Forms.CheckBox chkbDmClub;
        private System.Windows.Forms.CheckBox chkbDmProd;
        private System.Windows.Forms.CheckBox chkbDmLongneck;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox gpStarvingTime;
        private System.Windows.Forms.Button btnAddStarvingTimer;
        private System.Windows.Forms.Label lbTimeUntilStarving;
        private System.Windows.Forms.RadioButton rbBoneDamageDefault;
        private uiControls.Nud nudWDmHarpoon;
        private System.Windows.Forms.CheckBox chkbDmHarpoon;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lbKOInfo;
    }
}

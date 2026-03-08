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
            this.numericUpDownCurrentTorpor = new ARKBreedingStats.uiControls.Nud();
            this.btAddWakeUpTimer = new System.Windows.Forms.Button();
            this.lbTimeUntilWakingUp = new System.Windows.Forms.Label();
            this.lbCurrentTorpor = new System.Windows.Forms.Label();
            this.lbTamingTime = new System.Windows.Forms.Label();
            this.gbWeaponDamage = new System.Windows.Forms.GroupBox();
            this.flcBodyDamageMultipliers = new System.Windows.Forms.FlowLayoutPanel();
            this.rbBoneDamageDefault = new System.Windows.Forms.RadioButton();
            this.nudWDmHarpoon = new ARKBreedingStats.uiControls.Nud();
            this.chkbDmHarpoon = new System.Windows.Forms.CheckBox();
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
            this.nudWDmLongneck = new ARKBreedingStats.uiControls.Nud();
            this.gbKOInfo = new System.Windows.Forms.GroupBox();
            this.lbKOInfo = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gpStarvingTime = new System.Windows.Forms.GroupBox();
            this.nudTotalFood = new ARKBreedingStats.uiControls.Nud();
            this.label3 = new System.Windows.Forms.Label();
            this.nudCurrentFood = new ARKBreedingStats.uiControls.Nud();
            this.btnAddStarvingTimer = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbTimeUntilStarving = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flpTamingFood = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CbSanguineElixir = new System.Windows.Forms.CheckBox();
            this.checkBoxAugmented = new System.Windows.Forms.CheckBox();
            this.linkLabelWikiPage = new System.Windows.Forms.LinkLabel();
            this.nudLevel = new ARKBreedingStats.uiControls.Nud();
            this.gpTorporTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCurrentTorpor)).BeginInit();
            this.gbWeaponDamage.SuspendLayout();
            this.flcBodyDamageMultipliers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmHarpoon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmProd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmSlingshot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmClub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmBow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmCrossbow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmLongneck)).BeginInit();
            this.gbKOInfo.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.gpStarvingTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotalFood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentFood)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).BeginInit();
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
            this.lbMax.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbMax.Name = "lbMax";
            this.lbMax.Click += new System.EventHandler(this.lbMax_Click);
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
            100000000,
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
            this.lbTamingTime.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbTamingTime.Name = "lbTamingTime";
            this.lbTamingTime.Click += new System.EventHandler(this.lbTamingTime_Click);
            // 
            // gbWeaponDamage
            // 
            this.gbWeaponDamage.Controls.Add(this.flcBodyDamageMultipliers);
            this.gbWeaponDamage.Controls.Add(this.nudWDmHarpoon);
            this.gbWeaponDamage.Controls.Add(this.chkbDmHarpoon);
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
            // flcBodyDamageMultipliers
            // 
            this.flcBodyDamageMultipliers.Controls.Add(this.rbBoneDamageDefault);
            resources.ApplyResources(this.flcBodyDamageMultipliers, "flcBodyDamageMultipliers");
            this.flcBodyDamageMultipliers.Name = "flcBodyDamageMultipliers";
            // 
            // rbBoneDamageDefault
            // 
            resources.ApplyResources(this.rbBoneDamageDefault, "rbBoneDamageDefault");
            this.rbBoneDamageDefault.Name = "rbBoneDamageDefault";
            this.rbBoneDamageDefault.TabStop = true;
            this.rbBoneDamageDefault.UseVisualStyleBackColor = true;
            this.rbBoneDamageDefault.CheckedChanged += new System.EventHandler(this.rbBoneDamage_CheckedChanged);
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
            // chkbDmHarpoon
            // 
            resources.ApplyResources(this.chkbDmHarpoon, "chkbDmHarpoon");
            this.chkbDmHarpoon.Name = "chkbDmHarpoon";
            this.chkbDmHarpoon.UseVisualStyleBackColor = true;
            this.chkbDmHarpoon.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
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
            // chkbDmProd
            // 
            resources.ApplyResources(this.chkbDmProd, "chkbDmProd");
            this.chkbDmProd.Name = "chkbDmProd";
            this.chkbDmProd.UseVisualStyleBackColor = true;
            this.chkbDmProd.CheckedChanged += new System.EventHandler(this.chkbDm_CheckedChanged);
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
            // gbKOInfo
            // 
            this.gbKOInfo.Controls.Add(this.lbKOInfo);
            resources.ApplyResources(this.gbKOInfo, "gbKOInfo");
            this.gbKOInfo.Name = "gbKOInfo";
            this.gbKOInfo.TabStop = false;
            // 
            // lbKOInfo
            // 
            resources.ApplyResources(this.lbKOInfo, "lbKOInfo");
            this.lbKOInfo.Name = "lbKOInfo";
            // 
            // groupBox3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.panel3);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.tableLayoutPanel1.SetRowSpan(this.groupBox3, 2);
            this.groupBox3.TabStop = false;
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.labelResult);
            this.panel3.Name = "panel3";
            // 
            // gpStarvingTime
            // 
            this.gpStarvingTime.Controls.Add(this.nudTotalFood);
            this.gpStarvingTime.Controls.Add(this.label3);
            this.gpStarvingTime.Controls.Add(this.nudCurrentFood);
            this.gpStarvingTime.Controls.Add(this.btnAddStarvingTimer);
            this.gpStarvingTime.Controls.Add(this.label2);
            this.gpStarvingTime.Controls.Add(this.lbTimeUntilStarving);
            resources.ApplyResources(this.gpStarvingTime, "gpStarvingTime");
            this.gpStarvingTime.Name = "gpStarvingTime";
            this.gpStarvingTime.TabStop = false;
            // 
            // nudTotalFood
            // 
            this.nudTotalFood.DecimalPlaces = 1;
            this.nudTotalFood.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudTotalFood.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nudTotalFood, "nudTotalFood");
            this.nudTotalFood.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nudTotalFood.Name = "nudTotalFood";
            this.nudTotalFood.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // nudCurrentFood
            // 
            this.nudCurrentFood.DecimalPlaces = 1;
            this.nudCurrentFood.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudCurrentFood.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nudCurrentFood, "nudCurrentFood");
            this.nudCurrentFood.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nudCurrentFood.Name = "nudCurrentFood";
            this.nudCurrentFood.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudCurrentFood.ValueChanged += new System.EventHandler(this.nudCurrentFood_ValueChanged);
            // 
            // btnAddStarvingTimer
            // 
            resources.ApplyResources(this.btnAddStarvingTimer, "btnAddStarvingTimer");
            this.btnAddStarvingTimer.Name = "btnAddStarvingTimer";
            this.btnAddStarvingTimer.UseVisualStyleBackColor = true;
            this.btnAddStarvingTimer.Click += new System.EventHandler(this.btnAddStarvingTimer_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            this.panel1.Controls.Add(this.lbMax);
            this.panel1.Controls.Add(this.lbUsed);
            this.panel1.Controls.Add(this.lbTamingTime);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flpTamingFood, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // flpTamingFood
            // 
            resources.ApplyResources(this.flpTamingFood, "flpTamingFood");
            this.flpTamingFood.Name = "flpTamingFood";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CbSanguineElixir);
            this.panel2.Controls.Add(this.checkBoxAugmented);
            this.panel2.Controls.Add(this.linkLabelWikiPage);
            this.panel2.Controls.Add(this.nudLevel);
            this.panel2.Controls.Add(this.label1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // CbSanguineElixir
            // 
            resources.ApplyResources(this.CbSanguineElixir, "CbSanguineElixir");
            this.CbSanguineElixir.Name = "CbSanguineElixir";
            this.CbSanguineElixir.UseVisualStyleBackColor = true;
            this.CbSanguineElixir.CheckedChanged += new System.EventHandler(this.CbSanguineElixir_CheckedChanged);
            // 
            // checkBoxAugmented
            // 
            resources.ApplyResources(this.checkBoxAugmented, "checkBoxAugmented");
            this.checkBoxAugmented.Name = "checkBoxAugmented";
            this.checkBoxAugmented.UseVisualStyleBackColor = true;
            this.checkBoxAugmented.CheckedChanged += new System.EventHandler(this.checkBoxAugmented_CheckedChanged);
            // 
            // linkLabelWikiPage
            // 
            resources.ApplyResources(this.linkLabelWikiPage, "linkLabelWikiPage");
            this.linkLabelWikiPage.Name = "linkLabelWikiPage";
            this.linkLabelWikiPage.TabStop = true;
            this.linkLabelWikiPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWikiPage_LinkClicked);
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
            // TamingControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "TamingControl";
            this.gpTorporTime.ResumeLayout(false);
            this.gpTorporTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCurrentTorpor)).EndInit();
            this.gbWeaponDamage.ResumeLayout(false);
            this.gbWeaponDamage.PerformLayout();
            this.flcBodyDamageMultipliers.ResumeLayout(false);
            this.flcBodyDamageMultipliers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmHarpoon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmProd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmSlingshot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmClub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmBow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmCrossbow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWDmLongneck)).EndInit();
            this.gbKOInfo.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.gpStarvingTime.ResumeLayout(false);
            this.gpStarvingTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTotalFood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurrentFood)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).EndInit();
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flpTamingFood;
        private System.Windows.Forms.Panel panel2;
        private uiControls.Nud nudCurrentFood;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabelWikiPage;
        private uiControls.Nud nudTotalFood;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel flcBodyDamageMultipliers;
        private System.Windows.Forms.CheckBox checkBoxAugmented;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox CbSanguineElixir;
    }
}

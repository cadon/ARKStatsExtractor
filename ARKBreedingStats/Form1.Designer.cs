namespace ARKBreedingStats
{
    partial class Form1
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.labelHeaderW = new System.Windows.Forms.Label();
            this.labelHeaderD = new System.Windows.Forms.Label();
            this.numericUpDownLowerTEffBound = new System.Windows.Forms.NumericUpDown();
            this.groupBoxTE = new System.Windows.Forms.GroupBox();
            this.labelTE = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownUpperTEffBound = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.labelHBV = new System.Windows.Forms.Label();
            this.numericUpDownLevel = new System.Windows.Forms.NumericUpDown();
            this.comboBoxCreatures = new System.Windows.Forms.ComboBox();
            this.listBoxPossibilities = new System.Windows.Forms.ListBox();
            this.groupBoxPossibilities = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelDoc = new System.Windows.Forms.Label();
            this.buttonCopyClipboard = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.labelFootnote = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxAlreadyBred = new System.Windows.Forms.CheckBox();
            this.radioButtonOutputTable = new System.Windows.Forms.RadioButton();
            this.radioButtonOutputRow = new System.Windows.Forms.RadioButton();
            this.checkBoxOutputRowHeader = new System.Windows.Forms.CheckBox();
            this.checkBoxJustTamed = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.labelSum = new System.Windows.Forms.Label();
            this.labelSumWild = new System.Windows.Forms.Label();
            this.labelSumDom = new System.Windows.Forms.Label();
            this.labelSumDomSB = new System.Windows.Forms.Label();
            this.labelSumWildSB = new System.Windows.Forms.Label();
            this.labelSumSB = new System.Windows.Forms.Label();
            this.panelSums = new System.Windows.Forms.Panel();
            this.checkBoxSettings = new System.Windows.Forms.CheckBox();
            this.statIOSpeed = new ARKBreedingStats.StatIO();
            this.statIOTorpor = new ARKBreedingStats.StatIO();
            this.statIODamage = new ARKBreedingStats.StatIO();
            this.statIOWeight = new ARKBreedingStats.StatIO();
            this.statIOFood = new ARKBreedingStats.StatIO();
            this.statIOHealth = new ARKBreedingStats.StatIO();
            this.statIOOxygen = new ARKBreedingStats.StatIO();
            this.statIOStamina = new ARKBreedingStats.StatIO();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).BeginInit();
            this.groupBoxTE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
            this.groupBoxPossibilities.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelSums.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Location = new System.Drawing.Point(313, 159);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.Size = new System.Drawing.Size(161, 48);
            this.buttonCalculate.TabIndex = 14;
            this.buttonCalculate.Text = "Extract Level Distribution";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Click += new System.EventHandler(this.buttonCalculate_Click);
            // 
            // labelHeaderW
            // 
            this.labelHeaderW.AutoSize = true;
            this.labelHeaderW.Location = new System.Drawing.Point(133, 41);
            this.labelHeaderW.Name = "labelHeaderW";
            this.labelHeaderW.Size = new System.Drawing.Size(28, 13);
            this.labelHeaderW.TabIndex = 25;
            this.labelHeaderW.Text = "Wild";
            // 
            // labelHeaderD
            // 
            this.labelHeaderD.AutoSize = true;
            this.labelHeaderD.Location = new System.Drawing.Point(174, 41);
            this.labelHeaderD.Name = "labelHeaderD";
            this.labelHeaderD.Size = new System.Drawing.Size(29, 13);
            this.labelHeaderD.TabIndex = 26;
            this.labelHeaderD.Text = "Dom";
            // 
            // numericUpDownLowerTEffBound
            // 
            this.numericUpDownLowerTEffBound.Location = new System.Drawing.Point(6, 19);
            this.numericUpDownLowerTEffBound.Name = "numericUpDownLowerTEffBound";
            this.numericUpDownLowerTEffBound.Size = new System.Drawing.Size(45, 20);
            this.numericUpDownLowerTEffBound.TabIndex = 0;
            this.numericUpDownLowerTEffBound.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numericUpDownLowerTEffBound.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // groupBoxTE
            // 
            this.groupBoxTE.Controls.Add(this.labelTE);
            this.groupBoxTE.Controls.Add(this.label3);
            this.groupBoxTE.Controls.Add(this.numericUpDownUpperTEffBound);
            this.groupBoxTE.Controls.Add(this.label1);
            this.groupBoxTE.Controls.Add(this.numericUpDownLowerTEffBound);
            this.groupBoxTE.Location = new System.Drawing.Point(316, 12);
            this.groupBoxTE.Name = "groupBoxTE";
            this.groupBoxTE.Size = new System.Drawing.Size(158, 76);
            this.groupBoxTE.TabIndex = 11;
            this.groupBoxTE.TabStop = false;
            this.groupBoxTE.Text = "TamingEfficiency Range";
            // 
            // labelTE
            // 
            this.labelTE.Location = new System.Drawing.Point(6, 42);
            this.labelTE.Name = "labelTE";
            this.labelTE.Size = new System.Drawing.Size(142, 31);
            this.labelTE.TabIndex = 4;
            this.labelTE.Text = "TE differs in chosen possibilities";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "-";
            // 
            // numericUpDownUpperTEffBound
            // 
            this.numericUpDownUpperTEffBound.Location = new System.Drawing.Point(73, 19);
            this.numericUpDownUpperTEffBound.Name = "numericUpDownUpperTEffBound";
            this.numericUpDownUpperTEffBound.Size = new System.Drawing.Size(45, 20);
            this.numericUpDownUpperTEffBound.TabIndex = 2;
            this.numericUpDownUpperTEffBound.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownUpperTEffBound.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(124, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "%";
            // 
            // labelHBV
            // 
            this.labelHBV.AutoSize = true;
            this.labelHBV.Location = new System.Drawing.Point(213, 41);
            this.labelHBV.Name = "labelHBV";
            this.labelHBV.Size = new System.Drawing.Size(79, 13);
            this.labelHBV.TabIndex = 27;
            this.labelHBV.Text = "Breeding Value";
            // 
            // numericUpDownLevel
            // 
            this.numericUpDownLevel.Location = new System.Drawing.Point(206, 13);
            this.numericUpDownLevel.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownLevel.Name = "numericUpDownLevel";
            this.numericUpDownLevel.Size = new System.Drawing.Size(56, 20);
            this.numericUpDownLevel.TabIndex = 1;
            this.numericUpDownLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLevel.Enter += new System.EventHandler(this.numericUpDown_Enter);
            // 
            // comboBoxCreatures
            // 
            this.comboBoxCreatures.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxCreatures.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxCreatures.FormattingEnabled = true;
            this.comboBoxCreatures.Location = new System.Drawing.Point(12, 12);
            this.comboBoxCreatures.Name = "comboBoxCreatures";
            this.comboBoxCreatures.Size = new System.Drawing.Size(149, 21);
            this.comboBoxCreatures.TabIndex = 0;
            this.comboBoxCreatures.SelectedIndexChanged += new System.EventHandler(this.comboBoxCreatures_SelectedIndexChanged);
            // 
            // listBoxPossibilities
            // 
            this.listBoxPossibilities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPossibilities.FormattingEnabled = true;
            this.listBoxPossibilities.Location = new System.Drawing.Point(3, 16);
            this.listBoxPossibilities.Name = "listBoxPossibilities";
            this.listBoxPossibilities.Size = new System.Drawing.Size(162, 432);
            this.listBoxPossibilities.TabIndex = 0;
            this.listBoxPossibilities.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBoxPossibilities_MouseClick);
            // 
            // groupBoxPossibilities
            // 
            this.groupBoxPossibilities.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxPossibilities.Location = new System.Drawing.Point(480, 12);
            this.groupBoxPossibilities.Name = "groupBoxPossibilities";
            this.groupBoxPossibilities.Size = new System.Drawing.Size(174, 470);
            this.groupBoxPossibilities.TabIndex = 16;
            this.groupBoxPossibilities.TabStop = false;
            this.groupBoxPossibilities.Text = "Possible Levels";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.listBoxPossibilities, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(168, 451);
            this.tableLayoutPanel1.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Wild        Dom        TEfficiency";
            // 
            // labelDoc
            // 
            this.labelDoc.Location = new System.Drawing.Point(316, 210);
            this.labelDoc.Name = "labelDoc";
            this.labelDoc.Size = new System.Drawing.Size(158, 198);
            this.labelDoc.TabIndex = 24;
            this.labelDoc.Text = resources.GetString("labelDoc.Text");
            // 
            // buttonCopyClipboard
            // 
            this.buttonCopyClipboard.Enabled = false;
            this.buttonCopyClipboard.Location = new System.Drawing.Point(313, 446);
            this.buttonCopyClipboard.Name = "buttonCopyClipboard";
            this.buttonCopyClipboard.Size = new System.Drawing.Size(161, 56);
            this.buttonCopyClipboard.TabIndex = 15;
            this.buttonCopyClipboard.Text = "Copy retrieved Values to Clipboard for Spreadsheet";
            this.buttonCopyClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyClipboard.Click += new System.EventHandler(this.buttonCopyClipboard_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(555, 489);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(108, 13);
            this.linkLabel1.TabIndex = 20;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "v0.14.1, by cad 2016";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // labelFootnote
            // 
            this.labelFootnote.Location = new System.Drawing.Point(12, 495);
            this.labelFootnote.Name = "labelFootnote";
            this.labelFootnote.Size = new System.Drawing.Size(295, 16);
            this.labelFootnote.TabIndex = 18;
            this.labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(167, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Level";
            // 
            // checkBoxAlreadyBred
            // 
            this.checkBoxAlreadyBred.AutoSize = true;
            this.checkBoxAlreadyBred.Location = new System.Drawing.Point(313, 94);
            this.checkBoxAlreadyBred.Name = "checkBoxAlreadyBred";
            this.checkBoxAlreadyBred.Size = new System.Drawing.Size(91, 17);
            this.checkBoxAlreadyBred.TabIndex = 12;
            this.checkBoxAlreadyBred.Text = "Bred Creature";
            this.checkBoxAlreadyBred.UseVisualStyleBackColor = false;
            this.checkBoxAlreadyBred.CheckedChanged += new System.EventHandler(this.checkBoxAlreadyBred_CheckedChanged);
            // 
            // radioButtonOutputTable
            // 
            this.radioButtonOutputTable.AutoSize = true;
            this.radioButtonOutputTable.Location = new System.Drawing.Point(316, 423);
            this.radioButtonOutputTable.Name = "radioButtonOutputTable";
            this.radioButtonOutputTable.Size = new System.Drawing.Size(52, 17);
            this.radioButtonOutputTable.TabIndex = 21;
            this.radioButtonOutputTable.Text = "Table";
            this.radioButtonOutputTable.UseVisualStyleBackColor = true;
            // 
            // radioButtonOutputRow
            // 
            this.radioButtonOutputRow.AutoSize = true;
            this.radioButtonOutputRow.Checked = true;
            this.radioButtonOutputRow.Location = new System.Drawing.Point(374, 423);
            this.radioButtonOutputRow.Name = "radioButtonOutputRow";
            this.radioButtonOutputRow.Size = new System.Drawing.Size(47, 17);
            this.radioButtonOutputRow.TabIndex = 22;
            this.radioButtonOutputRow.TabStop = true;
            this.radioButtonOutputRow.Text = "Row";
            this.radioButtonOutputRow.UseVisualStyleBackColor = true;
            this.radioButtonOutputRow.CheckedChanged += new System.EventHandler(this.radioButtonOutputRow_CheckedChanged);
            // 
            // checkBoxOutputRowHeader
            // 
            this.checkBoxOutputRowHeader.AutoSize = true;
            this.checkBoxOutputRowHeader.Location = new System.Drawing.Point(427, 424);
            this.checkBoxOutputRowHeader.Name = "checkBoxOutputRowHeader";
            this.checkBoxOutputRowHeader.Size = new System.Drawing.Size(40, 17);
            this.checkBoxOutputRowHeader.TabIndex = 23;
            this.checkBoxOutputRowHeader.Text = "Hd";
            this.checkBoxOutputRowHeader.UseVisualStyleBackColor = true;
            // 
            // checkBoxJustTamed
            // 
            this.checkBoxJustTamed.Location = new System.Drawing.Point(313, 117);
            this.checkBoxJustTamed.Name = "checkBoxJustTamed";
            this.checkBoxJustTamed.Size = new System.Drawing.Size(161, 36);
            this.checkBoxJustTamed.TabIndex = 13;
            this.checkBoxJustTamed.Text = "Since Taming no Server-Restart";
            this.checkBoxJustTamed.UseVisualStyleBackColor = true;
            this.checkBoxJustTamed.CheckedChanged += new System.EventHandler(this.checkBoxJustTamed_CheckedChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(11, 34);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(57, 23);
            this.buttonClear.TabIndex = 28;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // labelSum
            // 
            this.labelSum.AutoSize = true;
            this.labelSum.Location = new System.Drawing.Point(76, 3);
            this.labelSum.Name = "labelSum";
            this.labelSum.Size = new System.Drawing.Size(28, 13);
            this.labelSum.TabIndex = 29;
            this.labelSum.Text = "Sum";
            // 
            // labelSumWild
            // 
            this.labelSumWild.AutoSize = true;
            this.labelSumWild.Location = new System.Drawing.Point(121, 3);
            this.labelSumWild.Name = "labelSumWild";
            this.labelSumWild.Size = new System.Drawing.Size(25, 13);
            this.labelSumWild.TabIndex = 30;
            this.labelSumWild.Text = "100";
            // 
            // labelSumDom
            // 
            this.labelSumDom.AutoSize = true;
            this.labelSumDom.Location = new System.Drawing.Point(162, 3);
            this.labelSumDom.Name = "labelSumDom";
            this.labelSumDom.Size = new System.Drawing.Size(25, 13);
            this.labelSumDom.TabIndex = 31;
            this.labelSumDom.Text = "100";
            // 
            // labelSumDomSB
            // 
            this.labelSumDomSB.AutoSize = true;
            this.labelSumDomSB.Location = new System.Drawing.Point(162, 17);
            this.labelSumDomSB.Name = "labelSumDomSB";
            this.labelSumDomSB.Size = new System.Drawing.Size(25, 13);
            this.labelSumDomSB.TabIndex = 34;
            this.labelSumDomSB.Text = "100";
            // 
            // labelSumWildSB
            // 
            this.labelSumWildSB.AutoSize = true;
            this.labelSumWildSB.Location = new System.Drawing.Point(121, 17);
            this.labelSumWildSB.Name = "labelSumWildSB";
            this.labelSumWildSB.Size = new System.Drawing.Size(25, 13);
            this.labelSumWildSB.TabIndex = 33;
            this.labelSumWildSB.Text = "100";
            // 
            // labelSumSB
            // 
            this.labelSumSB.AutoSize = true;
            this.labelSumSB.Location = new System.Drawing.Point(49, 17);
            this.labelSumSB.Name = "labelSumSB";
            this.labelSumSB.Size = new System.Drawing.Size(55, 13);
            this.labelSumSB.TabIndex = 32;
            this.labelSumSB.Text = "Should be";
            // 
            // panelSums
            // 
            this.panelSums.Controls.Add(this.labelSum);
            this.panelSums.Controls.Add(this.labelSumDomSB);
            this.panelSums.Controls.Add(this.labelSumWild);
            this.panelSums.Controls.Add(this.labelSumWildSB);
            this.panelSums.Controls.Add(this.labelSumDom);
            this.panelSums.Controls.Add(this.labelSumSB);
            this.panelSums.Location = new System.Drawing.Point(12, 460);
            this.panelSums.Name = "panelSums";
            this.panelSums.Size = new System.Drawing.Size(295, 32);
            this.panelSums.TabIndex = 35;
            // 
            // checkBoxSettings
            // 
            this.checkBoxSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxSettings.AutoSize = true;
            this.checkBoxSettings.Location = new System.Drawing.Point(74, 34);
            this.checkBoxSettings.Name = "checkBoxSettings";
            this.checkBoxSettings.Size = new System.Drawing.Size(55, 23);
            this.checkBoxSettings.TabIndex = 36;
            this.checkBoxSettings.Text = "Settings";
            this.checkBoxSettings.UseVisualStyleBackColor = true;
            this.checkBoxSettings.CheckedChanged += new System.EventHandler(this.checkBoxSettings_CheckedChanged);
            // 
            // statIOSpeed
            // 
            this.statIOSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.statIOSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOSpeed.Input = 100D;
            this.statIOSpeed.LevelWild = "";
            this.statIOSpeed.Location = new System.Drawing.Point(12, 363);
            this.statIOSpeed.MultAdd = 1D;
            this.statIOSpeed.MultAff = 1D;
            this.statIOSpeed.MultLevel = 1D;
            this.statIOSpeed.Name = "statIOSpeed";
            this.statIOSpeed.Percent = false;
            this.statIOSpeed.PostTame = true;
            this.statIOSpeed.Size = new System.Drawing.Size(295, 45);
            this.statIOSpeed.Status = 0;
            this.statIOSpeed.TabIndex = 8;
            this.statIOSpeed.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOTorpor
            // 
            this.statIOTorpor.BackColor = System.Drawing.SystemColors.Control;
            this.statIOTorpor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOTorpor.Input = 100D;
            this.statIOTorpor.LevelWild = "";
            this.statIOTorpor.Location = new System.Drawing.Point(12, 414);
            this.statIOTorpor.MultAdd = 1D;
            this.statIOTorpor.MultAff = 1D;
            this.statIOTorpor.MultLevel = 1D;
            this.statIOTorpor.Name = "statIOTorpor";
            this.statIOTorpor.Percent = false;
            this.statIOTorpor.PostTame = true;
            this.statIOTorpor.Size = new System.Drawing.Size(295, 45);
            this.statIOTorpor.Status = 0;
            this.statIOTorpor.TabIndex = 9;
            // 
            // statIODamage
            // 
            this.statIODamage.BackColor = System.Drawing.SystemColors.Control;
            this.statIODamage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIODamage.Input = 100D;
            this.statIODamage.LevelWild = "";
            this.statIODamage.Location = new System.Drawing.Point(12, 312);
            this.statIODamage.MultAdd = 1D;
            this.statIODamage.MultAff = 1D;
            this.statIODamage.MultLevel = 1D;
            this.statIODamage.Name = "statIODamage";
            this.statIODamage.Percent = false;
            this.statIODamage.PostTame = true;
            this.statIODamage.Size = new System.Drawing.Size(295, 45);
            this.statIODamage.Status = 0;
            this.statIODamage.TabIndex = 7;
            this.statIODamage.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOWeight
            // 
            this.statIOWeight.BackColor = System.Drawing.SystemColors.Control;
            this.statIOWeight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOWeight.Input = 100D;
            this.statIOWeight.LevelWild = "";
            this.statIOWeight.Location = new System.Drawing.Point(12, 261);
            this.statIOWeight.MultAdd = 1D;
            this.statIOWeight.MultAff = 1D;
            this.statIOWeight.MultLevel = 1D;
            this.statIOWeight.Name = "statIOWeight";
            this.statIOWeight.Percent = false;
            this.statIOWeight.PostTame = true;
            this.statIOWeight.Size = new System.Drawing.Size(295, 45);
            this.statIOWeight.Status = 0;
            this.statIOWeight.TabIndex = 6;
            this.statIOWeight.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOFood
            // 
            this.statIOFood.BackColor = System.Drawing.SystemColors.Control;
            this.statIOFood.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOFood.Input = 100D;
            this.statIOFood.LevelWild = "";
            this.statIOFood.Location = new System.Drawing.Point(12, 210);
            this.statIOFood.MultAdd = 1D;
            this.statIOFood.MultAff = 1D;
            this.statIOFood.MultLevel = 1D;
            this.statIOFood.Name = "statIOFood";
            this.statIOFood.Percent = false;
            this.statIOFood.PostTame = true;
            this.statIOFood.Size = new System.Drawing.Size(295, 45);
            this.statIOFood.Status = 0;
            this.statIOFood.TabIndex = 5;
            this.statIOFood.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOHealth
            // 
            this.statIOHealth.BackColor = System.Drawing.SystemColors.Control;
            this.statIOHealth.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOHealth.Input = 100D;
            this.statIOHealth.LevelWild = "";
            this.statIOHealth.Location = new System.Drawing.Point(12, 57);
            this.statIOHealth.MultAdd = 1D;
            this.statIOHealth.MultAff = 1D;
            this.statIOHealth.MultLevel = 1D;
            this.statIOHealth.Name = "statIOHealth";
            this.statIOHealth.Percent = false;
            this.statIOHealth.PostTame = true;
            this.statIOHealth.Size = new System.Drawing.Size(295, 45);
            this.statIOHealth.Status = 0;
            this.statIOHealth.TabIndex = 2;
            this.statIOHealth.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOOxygen
            // 
            this.statIOOxygen.BackColor = System.Drawing.SystemColors.Control;
            this.statIOOxygen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOOxygen.Input = 100D;
            this.statIOOxygen.LevelWild = "";
            this.statIOOxygen.Location = new System.Drawing.Point(12, 159);
            this.statIOOxygen.MultAdd = 1D;
            this.statIOOxygen.MultAff = 1D;
            this.statIOOxygen.MultLevel = 1D;
            this.statIOOxygen.Name = "statIOOxygen";
            this.statIOOxygen.Percent = false;
            this.statIOOxygen.PostTame = true;
            this.statIOOxygen.Size = new System.Drawing.Size(295, 45);
            this.statIOOxygen.Status = 0;
            this.statIOOxygen.TabIndex = 4;
            this.statIOOxygen.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOStamina
            // 
            this.statIOStamina.BackColor = System.Drawing.SystemColors.Control;
            this.statIOStamina.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOStamina.Input = 100D;
            this.statIOStamina.LevelWild = "";
            this.statIOStamina.Location = new System.Drawing.Point(12, 108);
            this.statIOStamina.MultAdd = 1D;
            this.statIOStamina.MultAff = 1D;
            this.statIOStamina.MultLevel = 1D;
            this.statIOStamina.Name = "statIOStamina";
            this.statIOStamina.Percent = false;
            this.statIOStamina.PostTame = true;
            this.statIOStamina.Size = new System.Drawing.Size(295, 45);
            this.statIOStamina.Status = 0;
            this.statIOStamina.TabIndex = 3;
            this.statIOStamina.Click += new System.EventHandler(this.statIO_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonCalculate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 511);
            this.Controls.Add(this.checkBoxSettings);
            this.Controls.Add(this.panelSums);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.checkBoxJustTamed);
            this.Controls.Add(this.checkBoxOutputRowHeader);
            this.Controls.Add(this.radioButtonOutputRow);
            this.Controls.Add(this.radioButtonOutputTable);
            this.Controls.Add(this.comboBoxCreatures);
            this.Controls.Add(this.checkBoxAlreadyBred);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.buttonCalculate);
            this.Controls.Add(this.statIOSpeed);
            this.Controls.Add(this.labelHeaderW);
            this.Controls.Add(this.statIOTorpor);
            this.Controls.Add(this.numericUpDownLevel);
            this.Controls.Add(this.statIODamage);
            this.Controls.Add(this.labelHeaderD);
            this.Controls.Add(this.statIOWeight);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBoxPossibilities);
            this.Controls.Add(this.groupBoxTE);
            this.Controls.Add(this.statIOFood);
            this.Controls.Add(this.statIOHealth);
            this.Controls.Add(this.labelDoc);
            this.Controls.Add(this.labelFootnote);
            this.Controls.Add(this.statIOOxygen);
            this.Controls.Add(this.labelHBV);
            this.Controls.Add(this.buttonCopyClipboard);
            this.Controls.Add(this.statIOStamina);
            this.Name = "Form1";
            this.Text = "ARK Breeding Stat Extractor";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).EndInit();
            this.groupBoxTE.ResumeLayout(false);
            this.groupBoxTE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).EndInit();
            this.groupBoxPossibilities.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelSums.ResumeLayout(false);
            this.panelSums.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonCalculate;
        private System.Windows.Forms.Label labelHeaderW;
        private System.Windows.Forms.Label labelHeaderD;
        private System.Windows.Forms.NumericUpDown numericUpDownLowerTEffBound;
        private System.Windows.Forms.GroupBox groupBoxTE;
        private System.Windows.Forms.Label label1;
        private StatIO statIOHealth;
        private System.Windows.Forms.Label labelHBV;
        private StatIO statIOStamina;
        private StatIO statIOOxygen;
        private StatIO statIOFood;
        private StatIO statIOWeight;
        private StatIO statIODamage;
        private StatIO statIOSpeed;
        private StatIO statIOTorpor;
        private System.Windows.Forms.NumericUpDown numericUpDownLevel;
        private System.Windows.Forms.ComboBox comboBoxCreatures;
        private System.Windows.Forms.ListBox listBoxPossibilities;
        private System.Windows.Forms.GroupBox groupBoxPossibilities;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelDoc;
        private System.Windows.Forms.Button buttonCopyClipboard;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label labelFootnote;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownUpperTEffBound;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxAlreadyBred;
        private System.Windows.Forms.RadioButton radioButtonOutputTable;
        private System.Windows.Forms.RadioButton radioButtonOutputRow;
        private System.Windows.Forms.CheckBox checkBoxOutputRowHeader;
        private System.Windows.Forms.CheckBox checkBoxJustTamed;
        private System.Windows.Forms.Label labelTE;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label labelSum;
        private System.Windows.Forms.Label labelSumWild;
        private System.Windows.Forms.Label labelSumDom;
        private System.Windows.Forms.Label labelSumDomSB;
        private System.Windows.Forms.Label labelSumWildSB;
        private System.Windows.Forms.Label labelSumSB;
        private System.Windows.Forms.Panel panelSums;
        private System.Windows.Forms.CheckBox checkBoxSettings;
    }
}


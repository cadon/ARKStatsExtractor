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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statTestingTamingEfficiency = new System.Windows.Forms.NumericUpDown();
            this.btnRegularTame = new System.Windows.Forms.Button();
            this.statTestingDinoLevel = new System.Windows.Forms.NumericUpDown();
            this.btnPrimeTame = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnPerfectKibbleTame = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.cbbStatTestingRace = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnStatTestingCompute = new System.Windows.Forms.Button();
            this.statIOStamina = new ARKBreedingStats.StatIO();
            this.statIOOxygen = new ARKBreedingStats.StatIO();
            this.statIOHealth = new ARKBreedingStats.StatIO();
            this.statIOFood = new ARKBreedingStats.StatIO();
            this.statIOSpeed = new ARKBreedingStats.StatIO();
            this.statIOWeight = new ARKBreedingStats.StatIO();
            this.statIOTorpor = new ARKBreedingStats.StatIO();
            this.statIODamage = new ARKBreedingStats.StatIO();
            this.statTestingSpeed = new ARKBreedingStats.StatIO();
            this.statTestingDamage = new ARKBreedingStats.StatIO();
            this.statTestingWeight = new ARKBreedingStats.StatIO();
            this.statTestingFood = new ARKBreedingStats.StatIO();
            this.statTestingOxygen = new ARKBreedingStats.StatIO();
            this.statTestingStamina = new ARKBreedingStats.StatIO();
            this.statTestingHealth = new ARKBreedingStats.StatIO();
            this.statTestingTorpor = new ARKBreedingStats.StatIO();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).BeginInit();
            this.groupBoxTE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
            this.groupBoxPossibilities.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelSums.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingTamingEfficiency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingDinoLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Location = new System.Drawing.Point(307, 153);
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
            this.labelHeaderW.Location = new System.Drawing.Point(127, 35);
            this.labelHeaderW.Name = "labelHeaderW";
            this.labelHeaderW.Size = new System.Drawing.Size(28, 13);
            this.labelHeaderW.TabIndex = 25;
            this.labelHeaderW.Text = "Wild";
            // 
            // labelHeaderD
            // 
            this.labelHeaderD.AutoSize = true;
            this.labelHeaderD.Location = new System.Drawing.Point(187, 35);
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
            this.groupBoxTE.Location = new System.Drawing.Point(310, 6);
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
            this.labelHBV.Location = new System.Drawing.Point(222, 35);
            this.labelHBV.Name = "labelHBV";
            this.labelHBV.Size = new System.Drawing.Size(79, 13);
            this.labelHBV.TabIndex = 27;
            this.labelHBV.Text = "Breeding Value";
            // 
            // numericUpDownLevel
            // 
            this.numericUpDownLevel.Location = new System.Drawing.Point(200, 7);
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
            this.comboBoxCreatures.Location = new System.Drawing.Point(6, 6);
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
            this.groupBoxPossibilities.Location = new System.Drawing.Point(474, 6);
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
            this.labelDoc.Location = new System.Drawing.Point(310, 204);
            this.labelDoc.Name = "labelDoc";
            this.labelDoc.Size = new System.Drawing.Size(158, 198);
            this.labelDoc.TabIndex = 24;
            this.labelDoc.Text = resources.GetString("labelDoc.Text");
            // 
            // buttonCopyClipboard
            // 
            this.buttonCopyClipboard.Enabled = false;
            this.buttonCopyClipboard.Location = new System.Drawing.Point(307, 440);
            this.buttonCopyClipboard.Name = "buttonCopyClipboard";
            this.buttonCopyClipboard.Size = new System.Drawing.Size(161, 56);
            this.buttonCopyClipboard.TabIndex = 15;
            this.buttonCopyClipboard.Text = "Copy retrieved Values to Clipboard for Spreadsheet";
            this.buttonCopyClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyClipboard.Click += new System.EventHandler(this.buttonCopyClipboard_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(537, 489);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(108, 13);
            this.linkLabel1.TabIndex = 20;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "v0.14.1, by cad 2016";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // labelFootnote
            // 
            this.labelFootnote.Location = new System.Drawing.Point(6, 489);
            this.labelFootnote.Name = "labelFootnote";
            this.labelFootnote.Size = new System.Drawing.Size(295, 16);
            this.labelFootnote.TabIndex = 18;
            this.labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Level";
            // 
            // checkBoxAlreadyBred
            // 
            this.checkBoxAlreadyBred.AutoSize = true;
            this.checkBoxAlreadyBred.Location = new System.Drawing.Point(307, 88);
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
            this.radioButtonOutputTable.Location = new System.Drawing.Point(310, 417);
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
            this.radioButtonOutputRow.Location = new System.Drawing.Point(368, 417);
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
            this.checkBoxOutputRowHeader.Location = new System.Drawing.Point(421, 418);
            this.checkBoxOutputRowHeader.Name = "checkBoxOutputRowHeader";
            this.checkBoxOutputRowHeader.Size = new System.Drawing.Size(40, 17);
            this.checkBoxOutputRowHeader.TabIndex = 23;
            this.checkBoxOutputRowHeader.Text = "Hd";
            this.checkBoxOutputRowHeader.UseVisualStyleBackColor = true;
            // 
            // checkBoxJustTamed
            // 
            this.checkBoxJustTamed.Location = new System.Drawing.Point(307, 111);
            this.checkBoxJustTamed.Name = "checkBoxJustTamed";
            this.checkBoxJustTamed.Size = new System.Drawing.Size(161, 36);
            this.checkBoxJustTamed.TabIndex = 13;
            this.checkBoxJustTamed.Text = "Since Taming no Server-Restart";
            this.checkBoxJustTamed.UseVisualStyleBackColor = true;
            this.checkBoxJustTamed.CheckedChanged += new System.EventHandler(this.checkBoxJustTamed_CheckedChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(5, 28);
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
            this.panelSums.Location = new System.Drawing.Point(6, 454);
            this.panelSums.Name = "panelSums";
            this.panelSums.Size = new System.Drawing.Size(295, 32);
            this.panelSums.TabIndex = 35;
            // 
            // checkBoxSettings
            // 
            this.checkBoxSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxSettings.AutoSize = true;
            this.checkBoxSettings.Location = new System.Drawing.Point(68, 28);
            this.checkBoxSettings.Name = "checkBoxSettings";
            this.checkBoxSettings.Size = new System.Drawing.Size(55, 23);
            this.checkBoxSettings.TabIndex = 36;
            this.checkBoxSettings.Text = "Settings";
            this.checkBoxSettings.UseVisualStyleBackColor = true;
            this.checkBoxSettings.CheckedChanged += new System.EventHandler(this.checkBoxSettings_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(668, 543);
            this.tabControl1.TabIndex = 37;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.comboBoxCreatures);
            this.tabPage1.Controls.Add(this.linkLabel1);
            this.tabPage1.Controls.Add(this.checkBoxSettings);
            this.tabPage1.Controls.Add(this.statIOStamina);
            this.tabPage1.Controls.Add(this.panelSums);
            this.tabPage1.Controls.Add(this.buttonCopyClipboard);
            this.tabPage1.Controls.Add(this.buttonClear);
            this.tabPage1.Controls.Add(this.labelHBV);
            this.tabPage1.Controls.Add(this.checkBoxJustTamed);
            this.tabPage1.Controls.Add(this.statIOOxygen);
            this.tabPage1.Controls.Add(this.checkBoxOutputRowHeader);
            this.tabPage1.Controls.Add(this.labelFootnote);
            this.tabPage1.Controls.Add(this.radioButtonOutputRow);
            this.tabPage1.Controls.Add(this.labelDoc);
            this.tabPage1.Controls.Add(this.radioButtonOutputTable);
            this.tabPage1.Controls.Add(this.statIOHealth);
            this.tabPage1.Controls.Add(this.statIOFood);
            this.tabPage1.Controls.Add(this.checkBoxAlreadyBred);
            this.tabPage1.Controls.Add(this.groupBoxTE);
            this.tabPage1.Controls.Add(this.groupBoxPossibilities);
            this.tabPage1.Controls.Add(this.buttonCalculate);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.statIOSpeed);
            this.tabPage1.Controls.Add(this.statIOWeight);
            this.tabPage1.Controls.Add(this.labelHeaderW);
            this.tabPage1.Controls.Add(this.labelHeaderD);
            this.tabPage1.Controls.Add(this.statIOTorpor);
            this.tabPage1.Controls.Add(this.statIODamage);
            this.tabPage1.Controls.Add(this.numericUpDownLevel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(660, 517);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tamed Stats Determination";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.statTestingTorpor);
            this.tabPage2.Controls.Add(this.btnStatTestingCompute);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.cbbStatTestingRace);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.statTestingSpeed);
            this.tabPage2.Controls.Add(this.statTestingDamage);
            this.tabPage2.Controls.Add(this.statTestingWeight);
            this.tabPage2.Controls.Add(this.statTestingFood);
            this.tabPage2.Controls.Add(this.statTestingOxygen);
            this.tabPage2.Controls.Add(this.statTestingStamina);
            this.tabPage2.Controls.Add(this.statTestingHealth);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(660, 517);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Stat Testing";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.statTestingTamingEfficiency);
            this.groupBox1.Controls.Add(this.btnRegularTame);
            this.groupBox1.Controls.Add(this.statTestingDinoLevel);
            this.groupBox1.Controls.Add(this.btnPrimeTame);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnPerfectKibbleTame);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(307, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 162);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Taming Efficiency";
            // 
            // statTestingTamingEfficiency
            // 
            this.statTestingTamingEfficiency.DecimalPlaces = 2;
            this.statTestingTamingEfficiency.Location = new System.Drawing.Point(103, 19);
            this.statTestingTamingEfficiency.Name = "statTestingTamingEfficiency";
            this.statTestingTamingEfficiency.Size = new System.Drawing.Size(60, 20);
            this.statTestingTamingEfficiency.TabIndex = 0;
            this.statTestingTamingEfficiency.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // btnRegularTame
            // 
            this.btnRegularTame.Location = new System.Drawing.Point(9, 129);
            this.btnRegularTame.Name = "btnRegularTame";
            this.btnRegularTame.Size = new System.Drawing.Size(154, 23);
            this.btnRegularTame.TabIndex = 5;
            this.btnRegularTame.Text = "Regular Tame";
            this.btnRegularTame.UseVisualStyleBackColor = true;
            // 
            // statTestingDinoLevel
            // 
            this.statTestingDinoLevel.Location = new System.Drawing.Point(107, 45);
            this.statTestingDinoLevel.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.statTestingDinoLevel.Name = "statTestingDinoLevel";
            this.statTestingDinoLevel.Size = new System.Drawing.Size(56, 20);
            this.statTestingDinoLevel.TabIndex = 2;
            this.statTestingDinoLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnPrimeTame
            // 
            this.btnPrimeTame.Location = new System.Drawing.Point(9, 100);
            this.btnPrimeTame.Name = "btnPrimeTame";
            this.btnPrimeTame.Size = new System.Drawing.Size(154, 23);
            this.btnPrimeTame.TabIndex = 4;
            this.btnPrimeTame.Text = "Prime Tame";
            this.btnPrimeTame.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(68, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Level";
            // 
            // btnPerfectKibbleTame
            // 
            this.btnPerfectKibbleTame.Location = new System.Drawing.Point(9, 71);
            this.btnPerfectKibbleTame.Name = "btnPerfectKibbleTame";
            this.btnPerfectKibbleTame.Size = new System.Drawing.Size(154, 23);
            this.btnPerfectKibbleTame.TabIndex = 3;
            this.btnPerfectKibbleTame.Text = "Perfect Kibble Tame";
            this.btnPerfectKibbleTame.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 13);
            this.label9.TabIndex = 42;
            this.label9.Text = "Taming Efficiency";
            // 
            // cbbStatTestingRace
            // 
            this.cbbStatTestingRace.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbbStatTestingRace.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbbStatTestingRace.FormattingEnabled = true;
            this.cbbStatTestingRace.Location = new System.Drawing.Point(6, 6);
            this.cbbStatTestingRace.Name = "cbbStatTestingRace";
            this.cbbStatTestingRace.Size = new System.Drawing.Size(149, 21);
            this.cbbStatTestingRace.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(222, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Breeding Value";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(127, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Wild";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(187, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Dom";
            // 
            // btnStatTestingCompute
            // 
            this.btnStatTestingCompute.Location = new System.Drawing.Point(307, 174);
            this.btnStatTestingCompute.Name = "btnStatTestingCompute";
            this.btnStatTestingCompute.Size = new System.Drawing.Size(174, 75);
            this.btnStatTestingCompute.TabIndex = 34;
            this.btnStatTestingCompute.Text = "Compute";
            this.btnStatTestingCompute.UseVisualStyleBackColor = true;
            this.btnStatTestingCompute.Click += new System.EventHandler(this.btnStatTestingCompute_Click);
            // 
            // statIOStamina
            // 
            this.statIOStamina.BackColor = System.Drawing.SystemColors.Control;
            this.statIOStamina.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOStamina.Input = 100D;
            this.statIOStamina.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOStamina.LevelDom = 0;
            this.statIOStamina.LevelWild = -1;
            this.statIOStamina.Location = new System.Drawing.Point(6, 102);
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
            // statIOOxygen
            // 
            this.statIOOxygen.BackColor = System.Drawing.SystemColors.Control;
            this.statIOOxygen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOOxygen.Input = 100D;
            this.statIOOxygen.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOOxygen.LevelDom = 0;
            this.statIOOxygen.LevelWild = -1;
            this.statIOOxygen.Location = new System.Drawing.Point(6, 153);
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
            // statIOHealth
            // 
            this.statIOHealth.BackColor = System.Drawing.SystemColors.Control;
            this.statIOHealth.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOHealth.Input = 100D;
            this.statIOHealth.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOHealth.LevelDom = 0;
            this.statIOHealth.LevelWild = -1;
            this.statIOHealth.Location = new System.Drawing.Point(6, 51);
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
            // statIOFood
            // 
            this.statIOFood.BackColor = System.Drawing.SystemColors.Control;
            this.statIOFood.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOFood.Input = 100D;
            this.statIOFood.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOFood.LevelDom = 0;
            this.statIOFood.LevelWild = -1;
            this.statIOFood.Location = new System.Drawing.Point(6, 204);
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
            // statIOSpeed
            // 
            this.statIOSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.statIOSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOSpeed.Input = 100D;
            this.statIOSpeed.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOSpeed.LevelDom = 0;
            this.statIOSpeed.LevelWild = -1;
            this.statIOSpeed.Location = new System.Drawing.Point(6, 357);
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
            // statIOWeight
            // 
            this.statIOWeight.BackColor = System.Drawing.SystemColors.Control;
            this.statIOWeight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOWeight.Input = 100D;
            this.statIOWeight.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOWeight.LevelDom = 0;
            this.statIOWeight.LevelWild = -1;
            this.statIOWeight.Location = new System.Drawing.Point(6, 255);
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
            // statIOTorpor
            // 
            this.statIOTorpor.BackColor = System.Drawing.SystemColors.Control;
            this.statIOTorpor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOTorpor.Input = 100D;
            this.statIOTorpor.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOTorpor.LevelDom = 0;
            this.statIOTorpor.LevelWild = -1;
            this.statIOTorpor.Location = new System.Drawing.Point(6, 408);
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
            this.statIODamage.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIODamage.LevelDom = 0;
            this.statIODamage.LevelWild = -1;
            this.statIODamage.Location = new System.Drawing.Point(6, 306);
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
            // statTestingSpeed
            // 
            this.statTestingSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingSpeed.Input = 100D;
            this.statTestingSpeed.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingSpeed.LevelDom = 0;
            this.statTestingSpeed.LevelWild = -1;
            this.statTestingSpeed.Location = new System.Drawing.Point(6, 357);
            this.statTestingSpeed.MultAdd = 1D;
            this.statTestingSpeed.MultAff = 1D;
            this.statTestingSpeed.MultLevel = 1D;
            this.statTestingSpeed.Name = "statTestingSpeed";
            this.statTestingSpeed.Percent = false;
            this.statTestingSpeed.PostTame = true;
            this.statTestingSpeed.Size = new System.Drawing.Size(295, 45);
            this.statTestingSpeed.Status = 0;
            this.statTestingSpeed.TabIndex = 7;
            // 
            // statTestingDamage
            // 
            this.statTestingDamage.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingDamage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingDamage.Input = 100D;
            this.statTestingDamage.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingDamage.LevelDom = 0;
            this.statTestingDamage.LevelWild = -1;
            this.statTestingDamage.Location = new System.Drawing.Point(6, 306);
            this.statTestingDamage.MultAdd = 1D;
            this.statTestingDamage.MultAff = 1D;
            this.statTestingDamage.MultLevel = 1D;
            this.statTestingDamage.Name = "statTestingDamage";
            this.statTestingDamage.Percent = false;
            this.statTestingDamage.PostTame = true;
            this.statTestingDamage.Size = new System.Drawing.Size(295, 45);
            this.statTestingDamage.Status = 0;
            this.statTestingDamage.TabIndex = 6;
            // 
            // statTestingWeight
            // 
            this.statTestingWeight.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingWeight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingWeight.Input = 100D;
            this.statTestingWeight.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingWeight.LevelDom = 0;
            this.statTestingWeight.LevelWild = -1;
            this.statTestingWeight.Location = new System.Drawing.Point(6, 255);
            this.statTestingWeight.MultAdd = 1D;
            this.statTestingWeight.MultAff = 1D;
            this.statTestingWeight.MultLevel = 1D;
            this.statTestingWeight.Name = "statTestingWeight";
            this.statTestingWeight.Percent = false;
            this.statTestingWeight.PostTame = true;
            this.statTestingWeight.Size = new System.Drawing.Size(295, 45);
            this.statTestingWeight.Status = 0;
            this.statTestingWeight.TabIndex = 5;
            // 
            // statTestingFood
            // 
            this.statTestingFood.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingFood.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingFood.Input = 100D;
            this.statTestingFood.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingFood.LevelDom = 0;
            this.statTestingFood.LevelWild = -1;
            this.statTestingFood.Location = new System.Drawing.Point(6, 204);
            this.statTestingFood.MultAdd = 1D;
            this.statTestingFood.MultAff = 1D;
            this.statTestingFood.MultLevel = 1D;
            this.statTestingFood.Name = "statTestingFood";
            this.statTestingFood.Percent = false;
            this.statTestingFood.PostTame = true;
            this.statTestingFood.Size = new System.Drawing.Size(295, 45);
            this.statTestingFood.Status = 0;
            this.statTestingFood.TabIndex = 4;
            // 
            // statTestingOxygen
            // 
            this.statTestingOxygen.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingOxygen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingOxygen.Input = 100D;
            this.statTestingOxygen.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingOxygen.LevelDom = 0;
            this.statTestingOxygen.LevelWild = -1;
            this.statTestingOxygen.Location = new System.Drawing.Point(6, 153);
            this.statTestingOxygen.MultAdd = 1D;
            this.statTestingOxygen.MultAff = 1D;
            this.statTestingOxygen.MultLevel = 1D;
            this.statTestingOxygen.Name = "statTestingOxygen";
            this.statTestingOxygen.Percent = false;
            this.statTestingOxygen.PostTame = true;
            this.statTestingOxygen.Size = new System.Drawing.Size(295, 45);
            this.statTestingOxygen.Status = 0;
            this.statTestingOxygen.TabIndex = 3;
            // 
            // statTestingStamina
            // 
            this.statTestingStamina.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingStamina.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingStamina.Input = 100D;
            this.statTestingStamina.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingStamina.LevelDom = 0;
            this.statTestingStamina.LevelWild = -1;
            this.statTestingStamina.Location = new System.Drawing.Point(6, 102);
            this.statTestingStamina.MultAdd = 1D;
            this.statTestingStamina.MultAff = 1D;
            this.statTestingStamina.MultLevel = 1D;
            this.statTestingStamina.Name = "statTestingStamina";
            this.statTestingStamina.Percent = false;
            this.statTestingStamina.PostTame = true;
            this.statTestingStamina.Size = new System.Drawing.Size(295, 45);
            this.statTestingStamina.Status = 0;
            this.statTestingStamina.TabIndex = 2;
            // 
            // statTestingHealth
            // 
            this.statTestingHealth.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingHealth.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingHealth.Input = 100D;
            this.statTestingHealth.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingHealth.LevelDom = 0;
            this.statTestingHealth.LevelWild = -1;
            this.statTestingHealth.Location = new System.Drawing.Point(6, 51);
            this.statTestingHealth.MultAdd = 1D;
            this.statTestingHealth.MultAff = 1D;
            this.statTestingHealth.MultLevel = 1D;
            this.statTestingHealth.Name = "statTestingHealth";
            this.statTestingHealth.Percent = false;
            this.statTestingHealth.PostTame = true;
            this.statTestingHealth.Size = new System.Drawing.Size(295, 45);
            this.statTestingHealth.Status = 0;
            this.statTestingHealth.TabIndex = 1;
            // 
            // statTestingTorpor
            // 
            this.statTestingTorpor.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingTorpor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingTorpor.Input = 100D;
            this.statTestingTorpor.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingTorpor.LevelDom = 0;
            this.statTestingTorpor.LevelWild = -1;
            this.statTestingTorpor.Location = new System.Drawing.Point(6, 408);
            this.statTestingTorpor.MultAdd = 1D;
            this.statTestingTorpor.MultAff = 1D;
            this.statTestingTorpor.MultLevel = 1D;
            this.statTestingTorpor.Name = "statTestingTorpor";
            this.statTestingTorpor.Percent = false;
            this.statTestingTorpor.PostTame = true;
            this.statTestingTorpor.Size = new System.Drawing.Size(295, 45);
            this.statTestingTorpor.Status = 0;
            this.statTestingTorpor.TabIndex = 35;
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonCalculate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 570);
            this.Controls.Add(this.tabControl1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingTamingEfficiency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingDinoLevel)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown statTestingTamingEfficiency;
        private System.Windows.Forms.Button btnRegularTame;
        private System.Windows.Forms.NumericUpDown statTestingDinoLevel;
        private System.Windows.Forms.Button btnPrimeTame;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnPerfectKibbleTame;
        private System.Windows.Forms.Label label9;
        private StatIO statTestingSpeed;
        private StatIO statTestingDamage;
        private StatIO statTestingWeight;
        private StatIO statTestingFood;
        private StatIO statTestingOxygen;
        private StatIO statTestingStamina;
        private System.Windows.Forms.ComboBox cbbStatTestingRace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private StatIO statTestingHealth;
        private System.Windows.Forms.Button btnStatTestingCompute;
        private StatIO statTestingTorpor;
    }
}


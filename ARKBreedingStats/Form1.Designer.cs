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
            this.radioButtonWild = new System.Windows.Forms.RadioButton();
            this.radioButtonTamed = new System.Windows.Forms.RadioButton();
            this.checkBoxWildTamedAuto = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelWildTamedAuto = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonAdd2Library = new System.Windows.Forms.Button();
            this.statIOStamina = new ARKBreedingStats.StatIO();
            this.statIOOxygen = new ARKBreedingStats.StatIO();
            this.statIOHealth = new ARKBreedingStats.StatIO();
            this.statIOFood = new ARKBreedingStats.StatIO();
            this.statIOSpeed = new ARKBreedingStats.StatIO();
            this.statIOWeight = new ARKBreedingStats.StatIO();
            this.statIOTorpor = new ARKBreedingStats.StatIO();
            this.statIODamage = new ARKBreedingStats.StatIO();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanelCreatures = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatedStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).BeginInit();
            this.groupBoxTE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
            this.groupBoxPossibilities.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelSums.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelWildTamedAuto.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Location = new System.Drawing.Point(307, 179);
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
            this.labelHeaderD.Location = new System.Drawing.Point(168, 35);
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
            this.labelHBV.Location = new System.Drawing.Point(207, 35);
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
            this.labelDoc.Location = new System.Drawing.Point(307, 230);
            this.labelDoc.Name = "labelDoc";
            this.labelDoc.Size = new System.Drawing.Size(158, 198);
            this.labelDoc.TabIndex = 24;
            this.labelDoc.Text = resources.GetString("labelDoc.Text");
            // 
            // buttonCopyClipboard
            // 
            this.buttonCopyClipboard.Enabled = false;
            this.buttonCopyClipboard.Location = new System.Drawing.Point(307, 454);
            this.buttonCopyClipboard.Name = "buttonCopyClipboard";
            this.buttonCopyClipboard.Size = new System.Drawing.Size(161, 42);
            this.buttonCopyClipboard.TabIndex = 15;
            this.buttonCopyClipboard.Text = "Copy retrieved Values to Clipboard for Spreadsheet";
            this.buttonCopyClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyClipboard.Click += new System.EventHandler(this.buttonCopyClipboard_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(550, 490);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(114, 13);
            this.linkLabel1.TabIndex = 20;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "v0.15, (c) by cad 2016";
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
            this.radioButtonOutputTable.Location = new System.Drawing.Point(3, 3);
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
            this.radioButtonOutputRow.Location = new System.Drawing.Point(61, 3);
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
            this.checkBoxOutputRowHeader.Location = new System.Drawing.Point(114, 3);
            this.checkBoxOutputRowHeader.Name = "checkBoxOutputRowHeader";
            this.checkBoxOutputRowHeader.Size = new System.Drawing.Size(40, 17);
            this.checkBoxOutputRowHeader.TabIndex = 23;
            this.checkBoxOutputRowHeader.Text = "Hd";
            this.checkBoxOutputRowHeader.UseVisualStyleBackColor = true;
            // 
            // checkBoxJustTamed
            // 
            this.checkBoxJustTamed.Location = new System.Drawing.Point(307, 138);
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
            // radioButtonWild
            // 
            this.radioButtonWild.AutoSize = true;
            this.radioButtonWild.Checked = true;
            this.radioButtonWild.Enabled = false;
            this.radioButtonWild.Location = new System.Drawing.Point(52, 4);
            this.radioButtonWild.Name = "radioButtonWild";
            this.radioButtonWild.Size = new System.Drawing.Size(46, 17);
            this.radioButtonWild.TabIndex = 37;
            this.radioButtonWild.TabStop = true;
            this.radioButtonWild.Text = "Wild";
            this.radioButtonWild.UseVisualStyleBackColor = true;
            // 
            // radioButtonTamed
            // 
            this.radioButtonTamed.AutoSize = true;
            this.radioButtonTamed.Enabled = false;
            this.radioButtonTamed.Location = new System.Drawing.Point(100, 4);
            this.radioButtonTamed.Name = "radioButtonTamed";
            this.radioButtonTamed.Size = new System.Drawing.Size(58, 17);
            this.radioButtonTamed.TabIndex = 38;
            this.radioButtonTamed.Text = "Tamed";
            this.radioButtonTamed.UseVisualStyleBackColor = true;
            // 
            // checkBoxWildTamedAuto
            // 
            this.checkBoxWildTamedAuto.AutoSize = true;
            this.checkBoxWildTamedAuto.Checked = true;
            this.checkBoxWildTamedAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWildTamedAuto.Location = new System.Drawing.Point(3, 4);
            this.checkBoxWildTamedAuto.Name = "checkBoxWildTamedAuto";
            this.checkBoxWildTamedAuto.Size = new System.Drawing.Size(48, 17);
            this.checkBoxWildTamedAuto.TabIndex = 39;
            this.checkBoxWildTamedAuto.Text = "Auto";
            this.checkBoxWildTamedAuto.UseVisualStyleBackColor = true;
            this.checkBoxWildTamedAuto.CheckedChanged += new System.EventHandler(this.checkBoxWildTamedAuto_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButtonOutputTable);
            this.panel1.Controls.Add(this.radioButtonOutputRow);
            this.panel1.Controls.Add(this.checkBoxOutputRowHeader);
            this.panel1.Location = new System.Drawing.Point(307, 429);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(158, 22);
            this.panel1.TabIndex = 40;
            // 
            // panelWildTamedAuto
            // 
            this.panelWildTamedAuto.Controls.Add(this.radioButtonTamed);
            this.panelWildTamedAuto.Controls.Add(this.radioButtonWild);
            this.panelWildTamedAuto.Controls.Add(this.checkBoxWildTamedAuto);
            this.panelWildTamedAuto.Location = new System.Drawing.Point(304, 113);
            this.panelWildTamedAuto.Name = "panelWildTamedAuto";
            this.panelWildTamedAuto.Size = new System.Drawing.Size(161, 25);
            this.panelWildTamedAuto.TabIndex = 41;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(678, 534);
            this.tabControl1.TabIndex = 42;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonAdd2Library);
            this.tabPage1.Controls.Add(this.linkLabel1);
            this.tabPage1.Controls.Add(this.comboBoxCreatures);
            this.tabPage1.Controls.Add(this.panelWildTamedAuto);
            this.tabPage1.Controls.Add(this.statIOStamina);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.buttonCopyClipboard);
            this.tabPage1.Controls.Add(this.checkBoxSettings);
            this.tabPage1.Controls.Add(this.labelHBV);
            this.tabPage1.Controls.Add(this.panelSums);
            this.tabPage1.Controls.Add(this.statIOOxygen);
            this.tabPage1.Controls.Add(this.buttonClear);
            this.tabPage1.Controls.Add(this.labelFootnote);
            this.tabPage1.Controls.Add(this.checkBoxJustTamed);
            this.tabPage1.Controls.Add(this.labelDoc);
            this.tabPage1.Controls.Add(this.statIOHealth);
            this.tabPage1.Controls.Add(this.checkBoxAlreadyBred);
            this.tabPage1.Controls.Add(this.statIOFood);
            this.tabPage1.Controls.Add(this.groupBoxTE);
            this.tabPage1.Controls.Add(this.buttonCalculate);
            this.tabPage1.Controls.Add(this.groupBoxPossibilities);
            this.tabPage1.Controls.Add(this.statIOSpeed);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.labelHeaderW);
            this.tabPage1.Controls.Add(this.statIOWeight);
            this.tabPage1.Controls.Add(this.statIOTorpor);
            this.tabPage1.Controls.Add(this.labelHeaderD);
            this.tabPage1.Controls.Add(this.numericUpDownLevel);
            this.tabPage1.Controls.Add(this.statIODamage);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(670, 508);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Extractor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonAdd2Library
            // 
            this.buttonAdd2Library.Location = new System.Drawing.Point(474, 482);
            this.buttonAdd2Library.Name = "buttonAdd2Library";
            this.buttonAdd2Library.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd2Library.TabIndex = 42;
            this.buttonAdd2Library.Text = "Add 2 Lib";
            this.buttonAdd2Library.UseVisualStyleBackColor = true;
            this.buttonAdd2Library.Click += new System.EventHandler(this.buttonAdd2Library_Click);
            // 
            // statIOStamina
            // 
            this.statIOStamina.BackColor = System.Drawing.SystemColors.Control;
            this.statIOStamina.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOStamina.Input = 100D;
            this.statIOStamina.LevelWild = "";
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
            this.statIOOxygen.LevelWild = "";
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
            this.statIOHealth.LevelWild = "";
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
            this.statIOFood.LevelWild = "";
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
            this.statIOSpeed.LevelWild = "";
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
            this.statIOWeight.LevelWild = "";
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
            this.statIOTorpor.LevelWild = "";
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
            this.statIODamage.LevelWild = "";
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.flowLayoutPanelCreatures);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(670, 508);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Library";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelCreatures
            // 
            this.flowLayoutPanelCreatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelCreatures.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanelCreatures.Name = "flowLayoutPanelCreatures";
            this.flowLayoutPanelCreatures.Size = new System.Drawing.Size(664, 502);
            this.flowLayoutPanelCreatures.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(678, 24);
            this.menuStrip1.TabIndex = 43;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl  + O";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loadToolStripMenuItem.Text = "Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.checkForUpdatedStatsToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem1.Text = "?";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.aboutToolStripMenuItem.Text = "about...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // checkForUpdatedStatsToolStripMenuItem
            // 
            this.checkForUpdatedStatsToolStripMenuItem.Name = "checkForUpdatedStatsToolStripMenuItem";
            this.checkForUpdatedStatsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.checkForUpdatedStatsToolStripMenuItem.Text = "check for Updated stats...";
            this.checkForUpdatedStatsToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatedStatsToolStripMenuItem_Click_1);
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonCalculate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 558);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
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
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelWildTamedAuto.ResumeLayout(false);
            this.panelWildTamedAuto.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.RadioButton radioButtonWild;
        private System.Windows.Forms.RadioButton radioButtonTamed;
        private System.Windows.Forms.CheckBox checkBoxWildTamedAuto;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelWildTamedAuto;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button buttonAdd2Library;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCreatures;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatedStatsToolStripMenuItem;
    }
}


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
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPerfectKibbleTame = new System.Windows.Forms.Button();
            this.btnPrimeTame = new System.Windows.Forms.Button();
            this.btnRegularTame = new System.Windows.Forms.Button();
            this.buttonAdd2Library = new System.Windows.Forms.Button();
            this.buttonExtract = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonCopyClipboard = new System.Windows.Forms.Button();
            this.cbbStatTestingRace = new System.Windows.Forms.ComboBox();
            this.checkBoxAlreadyBred = new System.Windows.Forms.CheckBox();
            this.checkBoxJustTamed = new System.Windows.Forms.CheckBox();
            this.checkBoxWildTamedAuto = new System.Windows.Forms.CheckBox();
            this.comboBoxCreatures = new System.Windows.Forms.ComboBox();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAndAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxStatTestingTamed = new System.Windows.Forms.CheckBox();
            this.statTestingTamingEfficiency = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.statTestingDinoLevel = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxPossibilities = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listBoxPossibilities = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxTE = new System.Windows.Forms.GroupBox();
            this.labelTE = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownUpperTEffBound = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownLowerTEffBound = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelDoc = new System.Windows.Forms.Label();
            this.labelFootnote = new System.Windows.Forms.Label();
            this.labelHBV = new System.Windows.Forms.Label();
            this.labelHeaderD = new System.Windows.Forms.Label();
            this.labelHeaderW = new System.Windows.Forms.Label();
            this.labelSum = new System.Windows.Forms.Label();
            this.labelSumDom = new System.Windows.Forms.Label();
            this.labelSumDomSB = new System.Windows.Forms.Label();
            this.labelSumSB = new System.Windows.Forms.Label();
            this.labelSumWild = new System.Windows.Forms.Label();
            this.labelSumWildSB = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.creatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatedStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numericUpDownLevel = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxOutputRowHeader = new System.Windows.Forms.CheckBox();
            this.radioButtonOutputRow = new System.Windows.Forms.RadioButton();
            this.radioButtonOutputTable = new System.Windows.Forms.RadioButton();
            this.panelSums = new System.Windows.Forms.Panel();
            this.panelWildTamedAuto = new System.Windows.Forms.Panel();
            this.radioButtonTamed = new System.Windows.Forms.RadioButton();
            this.radioButtonWild = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageStatTesting = new System.Windows.Forms.TabPage();
            this.groupBoxTestingName = new System.Windows.Forms.GroupBox();
            this.textBoxTestingName = new System.Windows.Forms.TextBox();
            this.buttonAddTest2Lib = new System.Windows.Forms.Button();
            this.labelTestingInfo = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.statTestingTorpor = new ARKBreedingStats.StatIO();
            this.statTestingSpeed = new ARKBreedingStats.StatIO();
            this.statTestingDamage = new ARKBreedingStats.StatIO();
            this.statTestingWeight = new ARKBreedingStats.StatIO();
            this.statTestingFood = new ARKBreedingStats.StatIO();
            this.statTestingOxygen = new ARKBreedingStats.StatIO();
            this.statTestingStamina = new ARKBreedingStats.StatIO();
            this.statTestingHealth = new ARKBreedingStats.StatIO();
            this.tabPageExtractor = new System.Windows.Forms.TabPage();
            this.groupBoxNameExtractor = new System.Windows.Forms.GroupBox();
            this.textBoxExtractorName = new System.Windows.Forms.TextBox();
            this.statIOStamina = new ARKBreedingStats.StatIO();
            this.statIOOxygen = new ARKBreedingStats.StatIO();
            this.statIOHealth = new ARKBreedingStats.StatIO();
            this.statIOFood = new ARKBreedingStats.StatIO();
            this.statIOSpeed = new ARKBreedingStats.StatIO();
            this.statIOWeight = new ARKBreedingStats.StatIO();
            this.statIOTorpor = new ARKBreedingStats.StatIO();
            this.statIODamage = new ARKBreedingStats.StatIO();
            this.tabPageLibrary = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelLibrary = new System.Windows.Forms.TableLayoutPanel();
            this.treeViewCreatureLib = new System.Windows.Forms.TreeView();
            this.listViewLibrary = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOwner = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGender = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTopStatsNr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.creatureBoxListView = new ARKBreedingStats.CreatureBox();
            this.tabPagePedigree = new System.Windows.Forms.TabPage();
            this.pedigree1 = new ARKBreedingStats.Pedigree();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingTamingEfficiency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingDinoLevel)).BeginInit();
            this.groupBoxPossibilities.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxTE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).BeginInit();
            this.panel1.SuspendLayout();
            this.panelSums.SuspendLayout();
            this.panelWildTamedAuto.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageStatTesting.SuspendLayout();
            this.groupBoxTestingName.SuspendLayout();
            this.tabPageExtractor.SuspendLayout();
            this.groupBoxNameExtractor.SuspendLayout();
            this.tabPageLibrary.SuspendLayout();
            this.tableLayoutPanelLibrary.SuspendLayout();
            this.tabPagePedigree.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.aboutToolStripMenuItem.Text = "about...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // btnPerfectKibbleTame
            // 
            this.btnPerfectKibbleTame.Location = new System.Drawing.Point(487, 6);
            this.btnPerfectKibbleTame.Name = "btnPerfectKibbleTame";
            this.btnPerfectKibbleTame.Size = new System.Drawing.Size(154, 23);
            this.btnPerfectKibbleTame.TabIndex = 3;
            this.btnPerfectKibbleTame.Text = "Perfect Kibble Tame";
            this.btnPerfectKibbleTame.UseVisualStyleBackColor = true;
            this.btnPerfectKibbleTame.Visible = false;
            this.btnPerfectKibbleTame.Click += new System.EventHandler(this.btnPerfectKibbleTame_Click);
            // 
            // btnPrimeTame
            // 
            this.btnPrimeTame.Location = new System.Drawing.Point(487, 35);
            this.btnPrimeTame.Name = "btnPrimeTame";
            this.btnPrimeTame.Size = new System.Drawing.Size(154, 23);
            this.btnPrimeTame.TabIndex = 4;
            this.btnPrimeTame.Text = "Prime Tame";
            this.btnPrimeTame.UseVisualStyleBackColor = true;
            this.btnPrimeTame.Visible = false;
            // 
            // btnRegularTame
            // 
            this.btnRegularTame.Location = new System.Drawing.Point(487, 64);
            this.btnRegularTame.Name = "btnRegularTame";
            this.btnRegularTame.Size = new System.Drawing.Size(154, 23);
            this.btnRegularTame.TabIndex = 5;
            this.btnRegularTame.Text = "Regular Tame";
            this.btnRegularTame.UseVisualStyleBackColor = true;
            this.btnRegularTame.Visible = false;
            // 
            // buttonAdd2Library
            // 
            this.buttonAdd2Library.Location = new System.Drawing.Point(474, 479);
            this.buttonAdd2Library.Name = "buttonAdd2Library";
            this.buttonAdd2Library.Size = new System.Drawing.Size(174, 31);
            this.buttonAdd2Library.TabIndex = 15;
            this.buttonAdd2Library.Text = "Add to Library";
            this.buttonAdd2Library.UseVisualStyleBackColor = true;
            this.buttonAdd2Library.Click += new System.EventHandler(this.buttonAdd2Library_Click);
            // 
            // buttonExtract
            // 
            this.buttonExtract.Location = new System.Drawing.Point(307, 179);
            this.buttonExtract.Name = "buttonExtract";
            this.buttonExtract.Size = new System.Drawing.Size(161, 48);
            this.buttonExtract.TabIndex = 10;
            this.buttonExtract.Text = "Extract Level Distribution";
            this.buttonExtract.UseVisualStyleBackColor = true;
            this.buttonExtract.Click += new System.EventHandler(this.buttonExtract_Click);
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
            // buttonCopyClipboard
            // 
            this.buttonCopyClipboard.Enabled = false;
            this.buttonCopyClipboard.Location = new System.Drawing.Point(307, 454);
            this.buttonCopyClipboard.Name = "buttonCopyClipboard";
            this.buttonCopyClipboard.Size = new System.Drawing.Size(161, 56);
            this.buttonCopyClipboard.TabIndex = 14;
            this.buttonCopyClipboard.Text = "Copy retrieved Values to Clipboard for Spreadsheet";
            this.buttonCopyClipboard.UseVisualStyleBackColor = true;
            this.buttonCopyClipboard.Click += new System.EventHandler(this.buttonCopyClipboard_Click);
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
            // checkBoxAlreadyBred
            // 
            this.checkBoxAlreadyBred.AutoSize = true;
            this.checkBoxAlreadyBred.Location = new System.Drawing.Point(307, 88);
            this.checkBoxAlreadyBred.Name = "checkBoxAlreadyBred";
            this.checkBoxAlreadyBred.Size = new System.Drawing.Size(91, 17);
            this.checkBoxAlreadyBred.TabIndex = 11;
            this.checkBoxAlreadyBred.Text = "Bred Creature";
            this.checkBoxAlreadyBred.UseVisualStyleBackColor = false;
            this.checkBoxAlreadyBred.CheckedChanged += new System.EventHandler(this.checkBoxAlreadyBred_CheckedChanged);
            // 
            // checkBoxJustTamed
            // 
            this.checkBoxJustTamed.Location = new System.Drawing.Point(307, 142);
            this.checkBoxJustTamed.Name = "checkBoxJustTamed";
            this.checkBoxJustTamed.Size = new System.Drawing.Size(161, 31);
            this.checkBoxJustTamed.TabIndex = 13;
            this.checkBoxJustTamed.Text = "Since Taming no Server-Restart";
            this.checkBoxJustTamed.UseVisualStyleBackColor = true;
            this.checkBoxJustTamed.CheckedChanged += new System.EventHandler(this.checkBoxJustTamed_CheckedChanged);
            // 
            // checkBoxWildTamedAuto
            // 
            this.checkBoxWildTamedAuto.AutoSize = true;
            this.checkBoxWildTamedAuto.Checked = true;
            this.checkBoxWildTamedAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWildTamedAuto.Location = new System.Drawing.Point(3, 4);
            this.checkBoxWildTamedAuto.Name = "checkBoxWildTamedAuto";
            this.checkBoxWildTamedAuto.Size = new System.Drawing.Size(48, 17);
            this.checkBoxWildTamedAuto.TabIndex = 0;
            this.checkBoxWildTamedAuto.Text = "Auto";
            this.checkBoxWildTamedAuto.UseVisualStyleBackColor = true;
            this.checkBoxWildTamedAuto.CheckedChanged += new System.EventHandler(this.checkBoxWildTamedAuto_CheckedChanged);
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
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.loadAndAddToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.loadToolStripMenuItem.Text = "Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // loadAndAddToolStripMenuItem
            // 
            this.loadAndAddToolStripMenuItem.Name = "loadAndAddToolStripMenuItem";
            this.loadAndAddToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.loadAndAddToolStripMenuItem.Text = "Load and Add...";
            this.loadAndAddToolStripMenuItem.Click += new System.EventHandler(this.loadAndAddToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(243, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxStatTestingTamed);
            this.groupBox1.Controls.Add(this.statTestingTamingEfficiency);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(307, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 67);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Taming-Efficiency";
            // 
            // checkBoxStatTestingTamed
            // 
            this.checkBoxStatTestingTamed.AutoSize = true;
            this.checkBoxStatTestingTamed.Checked = true;
            this.checkBoxStatTestingTamed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStatTestingTamed.Location = new System.Drawing.Point(103, 45);
            this.checkBoxStatTestingTamed.Name = "checkBoxStatTestingTamed";
            this.checkBoxStatTestingTamed.Size = new System.Drawing.Size(59, 17);
            this.checkBoxStatTestingTamed.TabIndex = 41;
            this.checkBoxStatTestingTamed.Text = "Tamed";
            this.checkBoxStatTestingTamed.UseVisualStyleBackColor = true;
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
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 13);
            this.label9.TabIndex = 42;
            this.label9.Text = "Taming Efficiency";
            // 
            // statTestingDinoLevel
            // 
            this.statTestingDinoLevel.Location = new System.Drawing.Point(584, 100);
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
            this.statTestingDinoLevel.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(545, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Level";
            this.label6.Visible = false;
            // 
            // groupBoxPossibilities
            // 
            this.groupBoxPossibilities.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxPossibilities.Location = new System.Drawing.Point(474, 6);
            this.groupBoxPossibilities.Name = "groupBoxPossibilities";
            this.groupBoxPossibilities.Size = new System.Drawing.Size(174, 415);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(168, 396);
            this.tableLayoutPanel1.TabIndex = 25;
            // 
            // listBoxPossibilities
            // 
            this.listBoxPossibilities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPossibilities.FormattingEnabled = true;
            this.listBoxPossibilities.Location = new System.Drawing.Point(3, 16);
            this.listBoxPossibilities.Name = "listBoxPossibilities";
            this.listBoxPossibilities.Size = new System.Drawing.Size(162, 377);
            this.listBoxPossibilities.TabIndex = 0;
            this.listBoxPossibilities.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBoxPossibilities_MouseClick);
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
            this.groupBoxTE.Text = "Taming-Efficiency Range";
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Level";
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
            this.label7.Location = new System.Drawing.Point(18, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Wild";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(73, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Dom";
            // 
            // labelDoc
            // 
            this.labelDoc.Location = new System.Drawing.Point(307, 230);
            this.labelDoc.Name = "labelDoc";
            this.labelDoc.Size = new System.Drawing.Size(158, 198);
            this.labelDoc.TabIndex = 24;
            this.labelDoc.Text = resources.GetString("labelDoc.Text");
            // 
            // labelFootnote
            // 
            this.labelFootnote.Location = new System.Drawing.Point(6, 489);
            this.labelFootnote.Name = "labelFootnote";
            this.labelFootnote.Size = new System.Drawing.Size(295, 16);
            this.labelFootnote.TabIndex = 18;
            this.labelFootnote.Text = "*Creature is not yet tamed and may get better values then.";
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
            // labelHeaderD
            // 
            this.labelHeaderD.AutoSize = true;
            this.labelHeaderD.Location = new System.Drawing.Point(178, 35);
            this.labelHeaderD.Name = "labelHeaderD";
            this.labelHeaderD.Size = new System.Drawing.Size(29, 13);
            this.labelHeaderD.TabIndex = 26;
            this.labelHeaderD.Text = "Dom";
            // 
            // labelHeaderW
            // 
            this.labelHeaderW.AutoSize = true;
            this.labelHeaderW.Location = new System.Drawing.Point(129, 35);
            this.labelHeaderW.Name = "labelHeaderW";
            this.labelHeaderW.Size = new System.Drawing.Size(28, 13);
            this.labelHeaderW.TabIndex = 25;
            this.labelHeaderW.Text = "Wild";
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
            // labelSumDom
            // 
            this.labelSumDom.AutoSize = true;
            this.labelSumDom.Location = new System.Drawing.Point(163, 3);
            this.labelSumDom.Name = "labelSumDom";
            this.labelSumDom.Size = new System.Drawing.Size(25, 13);
            this.labelSumDom.TabIndex = 31;
            this.labelSumDom.Text = "100";
            // 
            // labelSumDomSB
            // 
            this.labelSumDomSB.AutoSize = true;
            this.labelSumDomSB.Location = new System.Drawing.Point(163, 17);
            this.labelSumDomSB.Name = "labelSumDomSB";
            this.labelSumDomSB.Size = new System.Drawing.Size(25, 13);
            this.labelSumDomSB.TabIndex = 34;
            this.labelSumDomSB.Text = "100";
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
            // labelSumWild
            // 
            this.labelSumWild.AutoSize = true;
            this.labelSumWild.Location = new System.Drawing.Point(121, 3);
            this.labelSumWild.Name = "labelSumWild";
            this.labelSumWild.Size = new System.Drawing.Size(25, 13);
            this.labelSumWild.TabIndex = 30;
            this.labelSumWild.Text = "100";
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
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.creatureToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(731, 24);
            this.menuStrip1.TabIndex = 43;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // creatureToolStripMenuItem
            // 
            this.creatureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteSelectedToolStripMenuItem});
            this.creatureToolStripMenuItem.Name = "creatureToolStripMenuItem";
            this.creatureToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.creatureToolStripMenuItem.Text = "Creature";
            // 
            // deleteSelectedToolStripMenuItem
            // 
            this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.deleteSelectedToolStripMenuItem.Text = "Delete Selected";
            this.deleteSelectedToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedToolStripMenuItem_Click);
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
            // checkForUpdatedStatsToolStripMenuItem
            // 
            this.checkForUpdatedStatsToolStripMenuItem.Name = "checkForUpdatedStatsToolStripMenuItem";
            this.checkForUpdatedStatsToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.checkForUpdatedStatsToolStripMenuItem.Text = "Check for Updated Stats...";
            this.checkForUpdatedStatsToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatedStatsToolStripMenuItem_Click_1);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxOutputRowHeader);
            this.panel1.Controls.Add(this.radioButtonOutputRow);
            this.panel1.Controls.Add(this.radioButtonOutputTable);
            this.panel1.Location = new System.Drawing.Point(307, 429);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(158, 22);
            this.panel1.TabIndex = 40;
            // 
            // checkBoxOutputRowHeader
            // 
            this.checkBoxOutputRowHeader.AutoSize = true;
            this.checkBoxOutputRowHeader.Location = new System.Drawing.Point(115, 3);
            this.checkBoxOutputRowHeader.Name = "checkBoxOutputRowHeader";
            this.checkBoxOutputRowHeader.Size = new System.Drawing.Size(40, 17);
            this.checkBoxOutputRowHeader.TabIndex = 2;
            this.checkBoxOutputRowHeader.Text = "Hd";
            this.checkBoxOutputRowHeader.UseVisualStyleBackColor = true;
            // 
            // radioButtonOutputRow
            // 
            this.radioButtonOutputRow.AutoSize = true;
            this.radioButtonOutputRow.Checked = true;
            this.radioButtonOutputRow.Location = new System.Drawing.Point(62, 2);
            this.radioButtonOutputRow.Name = "radioButtonOutputRow";
            this.radioButtonOutputRow.Size = new System.Drawing.Size(47, 17);
            this.radioButtonOutputRow.TabIndex = 1;
            this.radioButtonOutputRow.TabStop = true;
            this.radioButtonOutputRow.Text = "Row";
            this.radioButtonOutputRow.UseVisualStyleBackColor = true;
            // 
            // radioButtonOutputTable
            // 
            this.radioButtonOutputTable.AutoSize = true;
            this.radioButtonOutputTable.Location = new System.Drawing.Point(4, 2);
            this.radioButtonOutputTable.Name = "radioButtonOutputTable";
            this.radioButtonOutputTable.Size = new System.Drawing.Size(52, 17);
            this.radioButtonOutputTable.TabIndex = 0;
            this.radioButtonOutputTable.Text = "Table";
            this.radioButtonOutputTable.UseVisualStyleBackColor = true;
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
            // panelWildTamedAuto
            // 
            this.panelWildTamedAuto.Controls.Add(this.radioButtonTamed);
            this.panelWildTamedAuto.Controls.Add(this.radioButtonWild);
            this.panelWildTamedAuto.Controls.Add(this.checkBoxWildTamedAuto);
            this.panelWildTamedAuto.Location = new System.Drawing.Point(307, 111);
            this.panelWildTamedAuto.Name = "panelWildTamedAuto";
            this.panelWildTamedAuto.Size = new System.Drawing.Size(161, 25);
            this.panelWildTamedAuto.TabIndex = 12;
            // 
            // radioButtonTamed
            // 
            this.radioButtonTamed.AutoSize = true;
            this.radioButtonTamed.Enabled = false;
            this.radioButtonTamed.Location = new System.Drawing.Point(100, 4);
            this.radioButtonTamed.Name = "radioButtonTamed";
            this.radioButtonTamed.Size = new System.Drawing.Size(58, 17);
            this.radioButtonTamed.TabIndex = 2;
            this.radioButtonTamed.Text = "Tamed";
            this.radioButtonTamed.UseVisualStyleBackColor = true;
            // 
            // radioButtonWild
            // 
            this.radioButtonWild.AutoSize = true;
            this.radioButtonWild.Checked = true;
            this.radioButtonWild.Enabled = false;
            this.radioButtonWild.Location = new System.Drawing.Point(52, 4);
            this.radioButtonWild.Name = "radioButtonWild";
            this.radioButtonWild.Size = new System.Drawing.Size(46, 17);
            this.radioButtonWild.TabIndex = 1;
            this.radioButtonWild.TabStop = true;
            this.radioButtonWild.Text = "Wild";
            this.radioButtonWild.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageStatTesting);
            this.tabControl1.Controls.Add(this.tabPageExtractor);
            this.tabControl1.Controls.Add(this.tabPageLibrary);
            this.tabControl1.Controls.Add(this.tabPagePedigree);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 1;
            this.tabControl1.Size = new System.Drawing.Size(731, 542);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageStatTesting
            // 
            this.tabPageStatTesting.Controls.Add(this.groupBoxTestingName);
            this.tabPageStatTesting.Controls.Add(this.buttonAddTest2Lib);
            this.tabPageStatTesting.Controls.Add(this.statTestingDinoLevel);
            this.tabPageStatTesting.Controls.Add(this.btnRegularTame);
            this.tabPageStatTesting.Controls.Add(this.label6);
            this.tabPageStatTesting.Controls.Add(this.labelTestingInfo);
            this.tabPageStatTesting.Controls.Add(this.label10);
            this.tabPageStatTesting.Controls.Add(this.btnPrimeTame);
            this.tabPageStatTesting.Controls.Add(this.statTestingTorpor);
            this.tabPageStatTesting.Controls.Add(this.groupBox1);
            this.tabPageStatTesting.Controls.Add(this.btnPerfectKibbleTame);
            this.tabPageStatTesting.Controls.Add(this.cbbStatTestingRace);
            this.tabPageStatTesting.Controls.Add(this.label5);
            this.tabPageStatTesting.Controls.Add(this.label7);
            this.tabPageStatTesting.Controls.Add(this.label8);
            this.tabPageStatTesting.Controls.Add(this.statTestingSpeed);
            this.tabPageStatTesting.Controls.Add(this.statTestingDamage);
            this.tabPageStatTesting.Controls.Add(this.statTestingWeight);
            this.tabPageStatTesting.Controls.Add(this.statTestingFood);
            this.tabPageStatTesting.Controls.Add(this.statTestingOxygen);
            this.tabPageStatTesting.Controls.Add(this.statTestingStamina);
            this.tabPageStatTesting.Controls.Add(this.statTestingHealth);
            this.tabPageStatTesting.Location = new System.Drawing.Point(4, 22);
            this.tabPageStatTesting.Name = "tabPageStatTesting";
            this.tabPageStatTesting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStatTesting.Size = new System.Drawing.Size(723, 516);
            this.tabPageStatTesting.TabIndex = 1;
            this.tabPageStatTesting.Text = "Stat Testing";
            this.tabPageStatTesting.UseVisualStyleBackColor = true;
            // 
            // groupBoxTestingName
            // 
            this.groupBoxTestingName.Controls.Add(this.textBoxTestingName);
            this.groupBoxTestingName.Location = new System.Drawing.Point(307, 354);
            this.groupBoxTestingName.Name = "groupBoxTestingName";
            this.groupBoxTestingName.Size = new System.Drawing.Size(174, 46);
            this.groupBoxTestingName.TabIndex = 40;
            this.groupBoxTestingName.TabStop = false;
            this.groupBoxTestingName.Text = "Name";
            // 
            // textBoxTestingName
            // 
            this.textBoxTestingName.Location = new System.Drawing.Point(6, 19);
            this.textBoxTestingName.Name = "textBoxTestingName";
            this.textBoxTestingName.Size = new System.Drawing.Size(162, 20);
            this.textBoxTestingName.TabIndex = 39;
            // 
            // buttonAddTest2Lib
            // 
            this.buttonAddTest2Lib.Location = new System.Drawing.Point(307, 406);
            this.buttonAddTest2Lib.Name = "buttonAddTest2Lib";
            this.buttonAddTest2Lib.Size = new System.Drawing.Size(174, 48);
            this.buttonAddTest2Lib.TabIndex = 38;
            this.buttonAddTest2Lib.Text = "Add as new creature to Library";
            this.buttonAddTest2Lib.UseVisualStyleBackColor = true;
            this.buttonAddTest2Lib.Click += new System.EventHandler(this.buttonAddTest2Lib_Click);
            // 
            // labelTestingInfo
            // 
            this.labelTestingInfo.Location = new System.Drawing.Point(307, 204);
            this.labelTestingInfo.Name = "labelTestingInfo";
            this.labelTestingInfo.Size = new System.Drawing.Size(174, 78);
            this.labelTestingInfo.TabIndex = 37;
            this.labelTestingInfo.Text = "Insert Numbers to see what value the creature will have.\r\nRight-click on a creatu" +
    "re in the library to insert its numbers here.";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(126, 35);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 36;
            this.label10.Text = "Current Value";
            // 
            // statTestingTorpor
            // 
            this.statTestingTorpor.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingTorpor.BreedingValue = 0D;
            this.statTestingTorpor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingTorpor.Input = 100D;
            this.statTestingTorpor.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingTorpor.LevelDom = 0;
            this.statTestingTorpor.LevelWild = 0;
            this.statTestingTorpor.Location = new System.Drawing.Point(6, 408);
            this.statTestingTorpor.Name = "statTestingTorpor";
            this.statTestingTorpor.Percent = false;
            this.statTestingTorpor.Size = new System.Drawing.Size(295, 45);
            this.statTestingTorpor.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingTorpor.TabIndex = 35;
            // 
            // statTestingSpeed
            // 
            this.statTestingSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingSpeed.BreedingValue = 0D;
            this.statTestingSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingSpeed.Input = 100D;
            this.statTestingSpeed.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingSpeed.LevelDom = 0;
            this.statTestingSpeed.LevelWild = 0;
            this.statTestingSpeed.Location = new System.Drawing.Point(6, 357);
            this.statTestingSpeed.Name = "statTestingSpeed";
            this.statTestingSpeed.Percent = false;
            this.statTestingSpeed.Size = new System.Drawing.Size(295, 45);
            this.statTestingSpeed.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingSpeed.TabIndex = 7;
            // 
            // statTestingDamage
            // 
            this.statTestingDamage.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingDamage.BreedingValue = 0D;
            this.statTestingDamage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingDamage.Input = 100D;
            this.statTestingDamage.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingDamage.LevelDom = 0;
            this.statTestingDamage.LevelWild = 0;
            this.statTestingDamage.Location = new System.Drawing.Point(6, 306);
            this.statTestingDamage.Name = "statTestingDamage";
            this.statTestingDamage.Percent = false;
            this.statTestingDamage.Size = new System.Drawing.Size(295, 45);
            this.statTestingDamage.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingDamage.TabIndex = 6;
            // 
            // statTestingWeight
            // 
            this.statTestingWeight.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingWeight.BreedingValue = 0D;
            this.statTestingWeight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingWeight.Input = 100D;
            this.statTestingWeight.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingWeight.LevelDom = 0;
            this.statTestingWeight.LevelWild = 0;
            this.statTestingWeight.Location = new System.Drawing.Point(6, 255);
            this.statTestingWeight.Name = "statTestingWeight";
            this.statTestingWeight.Percent = false;
            this.statTestingWeight.Size = new System.Drawing.Size(295, 45);
            this.statTestingWeight.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingWeight.TabIndex = 5;
            // 
            // statTestingFood
            // 
            this.statTestingFood.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingFood.BreedingValue = 0D;
            this.statTestingFood.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingFood.Input = 100D;
            this.statTestingFood.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingFood.LevelDom = 0;
            this.statTestingFood.LevelWild = 0;
            this.statTestingFood.Location = new System.Drawing.Point(6, 204);
            this.statTestingFood.Name = "statTestingFood";
            this.statTestingFood.Percent = false;
            this.statTestingFood.Size = new System.Drawing.Size(295, 45);
            this.statTestingFood.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingFood.TabIndex = 4;
            // 
            // statTestingOxygen
            // 
            this.statTestingOxygen.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingOxygen.BreedingValue = 0D;
            this.statTestingOxygen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingOxygen.Input = 100D;
            this.statTestingOxygen.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingOxygen.LevelDom = 0;
            this.statTestingOxygen.LevelWild = 0;
            this.statTestingOxygen.Location = new System.Drawing.Point(6, 153);
            this.statTestingOxygen.Name = "statTestingOxygen";
            this.statTestingOxygen.Percent = false;
            this.statTestingOxygen.Size = new System.Drawing.Size(295, 45);
            this.statTestingOxygen.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingOxygen.TabIndex = 3;
            // 
            // statTestingStamina
            // 
            this.statTestingStamina.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingStamina.BreedingValue = 0D;
            this.statTestingStamina.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingStamina.Input = 100D;
            this.statTestingStamina.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingStamina.LevelDom = 0;
            this.statTestingStamina.LevelWild = 0;
            this.statTestingStamina.Location = new System.Drawing.Point(6, 102);
            this.statTestingStamina.Name = "statTestingStamina";
            this.statTestingStamina.Percent = false;
            this.statTestingStamina.Size = new System.Drawing.Size(295, 45);
            this.statTestingStamina.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingStamina.TabIndex = 2;
            // 
            // statTestingHealth
            // 
            this.statTestingHealth.BackColor = System.Drawing.SystemColors.Control;
            this.statTestingHealth.BreedingValue = 0D;
            this.statTestingHealth.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statTestingHealth.Input = 100D;
            this.statTestingHealth.InputType = ARKBreedingStats.StatIOInputType.LevelsInputType;
            this.statTestingHealth.LevelDom = 0;
            this.statTestingHealth.LevelWild = 0;
            this.statTestingHealth.Location = new System.Drawing.Point(6, 51);
            this.statTestingHealth.Name = "statTestingHealth";
            this.statTestingHealth.Percent = false;
            this.statTestingHealth.Size = new System.Drawing.Size(295, 45);
            this.statTestingHealth.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statTestingHealth.TabIndex = 1;
            // 
            // tabPageExtractor
            // 
            this.tabPageExtractor.Controls.Add(this.groupBoxNameExtractor);
            this.tabPageExtractor.Controls.Add(this.buttonAdd2Library);
            this.tabPageExtractor.Controls.Add(this.comboBoxCreatures);
            this.tabPageExtractor.Controls.Add(this.panelWildTamedAuto);
            this.tabPageExtractor.Controls.Add(this.panel1);
            this.tabPageExtractor.Controls.Add(this.buttonCopyClipboard);
            this.tabPageExtractor.Controls.Add(this.labelHBV);
            this.tabPageExtractor.Controls.Add(this.panelSums);
            this.tabPageExtractor.Controls.Add(this.buttonClear);
            this.tabPageExtractor.Controls.Add(this.labelFootnote);
            this.tabPageExtractor.Controls.Add(this.checkBoxJustTamed);
            this.tabPageExtractor.Controls.Add(this.labelDoc);
            this.tabPageExtractor.Controls.Add(this.checkBoxAlreadyBred);
            this.tabPageExtractor.Controls.Add(this.groupBoxTE);
            this.tabPageExtractor.Controls.Add(this.buttonExtract);
            this.tabPageExtractor.Controls.Add(this.groupBoxPossibilities);
            this.tabPageExtractor.Controls.Add(this.label4);
            this.tabPageExtractor.Controls.Add(this.labelHeaderW);
            this.tabPageExtractor.Controls.Add(this.labelHeaderD);
            this.tabPageExtractor.Controls.Add(this.numericUpDownLevel);
            this.tabPageExtractor.Controls.Add(this.statIOStamina);
            this.tabPageExtractor.Controls.Add(this.statIOOxygen);
            this.tabPageExtractor.Controls.Add(this.statIOHealth);
            this.tabPageExtractor.Controls.Add(this.statIOFood);
            this.tabPageExtractor.Controls.Add(this.statIOSpeed);
            this.tabPageExtractor.Controls.Add(this.statIOWeight);
            this.tabPageExtractor.Controls.Add(this.statIOTorpor);
            this.tabPageExtractor.Controls.Add(this.statIODamage);
            this.tabPageExtractor.Location = new System.Drawing.Point(4, 22);
            this.tabPageExtractor.Name = "tabPageExtractor";
            this.tabPageExtractor.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExtractor.Size = new System.Drawing.Size(723, 516);
            this.tabPageExtractor.TabIndex = 0;
            this.tabPageExtractor.Text = "Extractor";
            this.tabPageExtractor.UseVisualStyleBackColor = true;
            // 
            // groupBoxNameExtractor
            // 
            this.groupBoxNameExtractor.Controls.Add(this.textBoxExtractorName);
            this.groupBoxNameExtractor.Location = new System.Drawing.Point(474, 427);
            this.groupBoxNameExtractor.Name = "groupBoxNameExtractor";
            this.groupBoxNameExtractor.Size = new System.Drawing.Size(174, 46);
            this.groupBoxNameExtractor.TabIndex = 41;
            this.groupBoxNameExtractor.TabStop = false;
            this.groupBoxNameExtractor.Text = "Name";
            // 
            // textBoxExtractorName
            // 
            this.textBoxExtractorName.Location = new System.Drawing.Point(6, 19);
            this.textBoxExtractorName.Name = "textBoxExtractorName";
            this.textBoxExtractorName.Size = new System.Drawing.Size(162, 20);
            this.textBoxExtractorName.TabIndex = 39;
            // 
            // statIOStamina
            // 
            this.statIOStamina.BackColor = System.Drawing.SystemColors.Control;
            this.statIOStamina.BreedingValue = 0D;
            this.statIOStamina.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOStamina.Input = 100D;
            this.statIOStamina.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOStamina.LevelDom = 0;
            this.statIOStamina.LevelWild = 0;
            this.statIOStamina.Location = new System.Drawing.Point(6, 102);
            this.statIOStamina.Name = "statIOStamina";
            this.statIOStamina.Percent = false;
            this.statIOStamina.Size = new System.Drawing.Size(295, 45);
            this.statIOStamina.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIOStamina.TabIndex = 3;
            this.statIOStamina.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOOxygen
            // 
            this.statIOOxygen.BackColor = System.Drawing.SystemColors.Control;
            this.statIOOxygen.BreedingValue = 0D;
            this.statIOOxygen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOOxygen.Input = 100D;
            this.statIOOxygen.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOOxygen.LevelDom = 0;
            this.statIOOxygen.LevelWild = 0;
            this.statIOOxygen.Location = new System.Drawing.Point(6, 153);
            this.statIOOxygen.Name = "statIOOxygen";
            this.statIOOxygen.Percent = false;
            this.statIOOxygen.Size = new System.Drawing.Size(295, 45);
            this.statIOOxygen.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIOOxygen.TabIndex = 4;
            this.statIOOxygen.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOHealth
            // 
            this.statIOHealth.BackColor = System.Drawing.SystemColors.Control;
            this.statIOHealth.BreedingValue = 0D;
            this.statIOHealth.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOHealth.Input = 100D;
            this.statIOHealth.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOHealth.LevelDom = 0;
            this.statIOHealth.LevelWild = 0;
            this.statIOHealth.Location = new System.Drawing.Point(6, 51);
            this.statIOHealth.Name = "statIOHealth";
            this.statIOHealth.Percent = false;
            this.statIOHealth.Size = new System.Drawing.Size(295, 45);
            this.statIOHealth.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIOHealth.TabIndex = 2;
            this.statIOHealth.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOFood
            // 
            this.statIOFood.BackColor = System.Drawing.SystemColors.Control;
            this.statIOFood.BreedingValue = 0D;
            this.statIOFood.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOFood.Input = 100D;
            this.statIOFood.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOFood.LevelDom = 0;
            this.statIOFood.LevelWild = 0;
            this.statIOFood.Location = new System.Drawing.Point(6, 204);
            this.statIOFood.Name = "statIOFood";
            this.statIOFood.Percent = false;
            this.statIOFood.Size = new System.Drawing.Size(295, 45);
            this.statIOFood.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIOFood.TabIndex = 5;
            this.statIOFood.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOSpeed
            // 
            this.statIOSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.statIOSpeed.BreedingValue = 0D;
            this.statIOSpeed.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOSpeed.Input = 100D;
            this.statIOSpeed.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOSpeed.LevelDom = 0;
            this.statIOSpeed.LevelWild = 0;
            this.statIOSpeed.Location = new System.Drawing.Point(6, 357);
            this.statIOSpeed.Name = "statIOSpeed";
            this.statIOSpeed.Percent = false;
            this.statIOSpeed.Size = new System.Drawing.Size(295, 45);
            this.statIOSpeed.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIOSpeed.TabIndex = 8;
            this.statIOSpeed.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOWeight
            // 
            this.statIOWeight.BackColor = System.Drawing.SystemColors.Control;
            this.statIOWeight.BreedingValue = 0D;
            this.statIOWeight.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOWeight.Input = 100D;
            this.statIOWeight.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOWeight.LevelDom = 0;
            this.statIOWeight.LevelWild = 0;
            this.statIOWeight.Location = new System.Drawing.Point(6, 255);
            this.statIOWeight.Name = "statIOWeight";
            this.statIOWeight.Percent = false;
            this.statIOWeight.Size = new System.Drawing.Size(295, 45);
            this.statIOWeight.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIOWeight.TabIndex = 6;
            this.statIOWeight.Click += new System.EventHandler(this.statIO_Click);
            // 
            // statIOTorpor
            // 
            this.statIOTorpor.BackColor = System.Drawing.SystemColors.Control;
            this.statIOTorpor.BreedingValue = 0D;
            this.statIOTorpor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIOTorpor.Input = 100D;
            this.statIOTorpor.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIOTorpor.LevelDom = 0;
            this.statIOTorpor.LevelWild = 0;
            this.statIOTorpor.Location = new System.Drawing.Point(6, 408);
            this.statIOTorpor.Name = "statIOTorpor";
            this.statIOTorpor.Percent = false;
            this.statIOTorpor.Size = new System.Drawing.Size(295, 45);
            this.statIOTorpor.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIOTorpor.TabIndex = 9;
            // 
            // statIODamage
            // 
            this.statIODamage.BackColor = System.Drawing.SystemColors.Control;
            this.statIODamage.BreedingValue = 0D;
            this.statIODamage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statIODamage.Input = 100D;
            this.statIODamage.InputType = ARKBreedingStats.StatIOInputType.FinalValueInputType;
            this.statIODamage.LevelDom = 0;
            this.statIODamage.LevelWild = 0;
            this.statIODamage.Location = new System.Drawing.Point(6, 306);
            this.statIODamage.Name = "statIODamage";
            this.statIODamage.Percent = false;
            this.statIODamage.Size = new System.Drawing.Size(295, 45);
            this.statIODamage.Status = ARKBreedingStats.StatIOStatus.Neutral;
            this.statIODamage.TabIndex = 7;
            this.statIODamage.Click += new System.EventHandler(this.statIO_Click);
            // 
            // tabPageLibrary
            // 
            this.tabPageLibrary.Controls.Add(this.tableLayoutPanelLibrary);
            this.tabPageLibrary.Location = new System.Drawing.Point(4, 22);
            this.tabPageLibrary.Name = "tabPageLibrary";
            this.tabPageLibrary.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLibrary.Size = new System.Drawing.Size(723, 516);
            this.tabPageLibrary.TabIndex = 2;
            this.tabPageLibrary.Text = "Library";
            this.tabPageLibrary.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelLibrary
            // 
            this.tableLayoutPanelLibrary.ColumnCount = 2;
            this.tableLayoutPanelLibrary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 201F));
            this.tableLayoutPanelLibrary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLibrary.Controls.Add(this.treeViewCreatureLib, 0, 1);
            this.tableLayoutPanelLibrary.Controls.Add(this.listViewLibrary, 1, 0);
            this.tableLayoutPanelLibrary.Controls.Add(this.creatureBoxListView, 0, 0);
            this.tableLayoutPanelLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLibrary.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelLibrary.Name = "tableLayoutPanelLibrary";
            this.tableLayoutPanelLibrary.RowCount = 2;
            this.tableLayoutPanelLibrary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanelLibrary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLibrary.Size = new System.Drawing.Size(717, 510);
            this.tableLayoutPanelLibrary.TabIndex = 4;
            // 
            // treeViewCreatureLib
            // 
            this.treeViewCreatureLib.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCreatureLib.Location = new System.Drawing.Point(3, 253);
            this.treeViewCreatureLib.Name = "treeViewCreatureLib";
            this.treeViewCreatureLib.Size = new System.Drawing.Size(195, 254);
            this.treeViewCreatureLib.TabIndex = 1;
            this.treeViewCreatureLib.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCreatureLib_AfterSelect);
            // 
            // listViewLibrary
            // 
            this.listViewLibrary.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderOwner,
            this.columnHeaderGender,
            this.columnHeaderTopStatsNr,
            this.columnHeaderGen,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            this.listViewLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLibrary.FullRowSelect = true;
            this.listViewLibrary.Location = new System.Drawing.Point(204, 3);
            this.listViewLibrary.Name = "listViewLibrary";
            this.tableLayoutPanelLibrary.SetRowSpan(this.listViewLibrary, 2);
            this.listViewLibrary.Size = new System.Drawing.Size(510, 504);
            this.listViewLibrary.TabIndex = 2;
            this.listViewLibrary.UseCompatibleStateImageBehavior = false;
            this.listViewLibrary.View = System.Windows.Forms.View.Details;
            this.listViewLibrary.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewLibrary_ColumnClick);
            this.listViewLibrary.SelectedIndexChanged += new System.EventHandler(this.listViewLibrary_SelectedIndexChanged);
            this.listViewLibrary.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewLibrary_MouseClick);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 97;
            // 
            // columnHeaderOwner
            // 
            this.columnHeaderOwner.Text = "Owner";
            this.columnHeaderOwner.Width = 48;
            // 
            // columnHeaderGender
            // 
            this.columnHeaderGender.Text = "Ge";
            this.columnHeaderGender.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderGender.Width = 35;
            // 
            // columnHeaderTopStatsNr
            // 
            this.columnHeaderTopStatsNr.DisplayIndex = 11;
            this.columnHeaderTopStatsNr.Text = "Top";
            this.columnHeaderTopStatsNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderTopStatsNr.Width = 31;
            // 
            // columnHeaderGen
            // 
            this.columnHeaderGen.DisplayIndex = 12;
            this.columnHeaderGen.Text = "Gen";
            this.columnHeaderGen.Width = 34;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 3;
            this.columnHeader3.Text = "HP";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 30;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 4;
            this.columnHeader4.Text = "St";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 30;
            // 
            // columnHeader5
            // 
            this.columnHeader5.DisplayIndex = 5;
            this.columnHeader5.Text = "Ox";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 30;
            // 
            // columnHeader6
            // 
            this.columnHeader6.DisplayIndex = 6;
            this.columnHeader6.Text = "Fo";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader6.Width = 30;
            // 
            // columnHeader7
            // 
            this.columnHeader7.DisplayIndex = 7;
            this.columnHeader7.Text = "We";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader7.Width = 30;
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 8;
            this.columnHeader8.Text = "Dm";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader8.Width = 30;
            // 
            // columnHeader9
            // 
            this.columnHeader9.DisplayIndex = 9;
            this.columnHeader9.Text = "Sp";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader9.Width = 30;
            // 
            // columnHeader10
            // 
            this.columnHeader10.DisplayIndex = 10;
            this.columnHeader10.Text = "To";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader10.Width = 30;
            // 
            // creatureBoxListView
            // 
            this.creatureBoxListView.Location = new System.Drawing.Point(3, 3);
            this.creatureBoxListView.Name = "creatureBoxListView";
            this.creatureBoxListView.Size = new System.Drawing.Size(195, 244);
            this.creatureBoxListView.TabIndex = 0;
            this.creatureBoxListView.Changed += new ARKBreedingStats.CreatureBox.ChangedEventHandler(this.creatureBoxListView_Changed);
            this.creatureBoxListView.EnterSettings += new ARKBreedingStats.CreatureBox.EventHandler(this.creatureBoxListView_EnterSettings);
            // 
            // tabPagePedigree
            // 
            this.tabPagePedigree.Controls.Add(this.pedigree1);
            this.tabPagePedigree.Location = new System.Drawing.Point(4, 22);
            this.tabPagePedigree.Name = "tabPagePedigree";
            this.tabPagePedigree.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePedigree.Size = new System.Drawing.Size(723, 516);
            this.tabPagePedigree.TabIndex = 3;
            this.tabPagePedigree.Text = "Pedigree";
            this.tabPagePedigree.UseVisualStyleBackColor = true;
            // 
            // pedigree1
            // 
            this.pedigree1.AutoScroll = true;
            this.pedigree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pedigree1.Location = new System.Drawing.Point(3, 3);
            this.pedigree1.Name = "pedigree1";
            this.pedigree1.Size = new System.Drawing.Size(717, 510);
            this.pedigree1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 566);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(731, 22);
            this.statusStrip1.TabIndex = 44;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(120, 17);
            this.toolStripStatusLabel1.Text = "ToolStripStatusLabel1";
            // 
            // Form1
            // 
            this.AcceptButton = this.buttonExtract;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 588);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "ARK Breeding Stat Extractor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingTamingEfficiency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statTestingDinoLevel)).EndInit();
            this.groupBoxPossibilities.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxTE.ResumeLayout(false);
            this.groupBoxTE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpperTEffBound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLowerTEffBound)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLevel)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelSums.ResumeLayout(false);
            this.panelSums.PerformLayout();
            this.panelWildTamedAuto.ResumeLayout(false);
            this.panelWildTamedAuto.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageStatTesting.ResumeLayout(false);
            this.tabPageStatTesting.PerformLayout();
            this.groupBoxTestingName.ResumeLayout(false);
            this.groupBoxTestingName.PerformLayout();
            this.tabPageExtractor.ResumeLayout(false);
            this.tabPageExtractor.PerformLayout();
            this.groupBoxNameExtractor.ResumeLayout(false);
            this.groupBoxNameExtractor.PerformLayout();
            this.tabPageLibrary.ResumeLayout(false);
            this.tableLayoutPanelLibrary.ResumeLayout(false);
            this.tabPagePedigree.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonExtract;
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
        private System.Windows.Forms.Label labelFootnote;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownUpperTEffBound;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxAlreadyBred;
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageStatTesting;
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
        private StatIO statTestingTorpor;
        private System.Windows.Forms.RadioButton radioButtonWild;
        private System.Windows.Forms.RadioButton radioButtonTamed;
        private System.Windows.Forms.CheckBox checkBoxWildTamedAuto;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelWildTamedAuto;
        private System.Windows.Forms.TabPage tabPageExtractor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button buttonAdd2Library;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatedStatsToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxOutputRowHeader;
        private System.Windows.Forms.RadioButton radioButtonOutputRow;
        private System.Windows.Forms.RadioButton radioButtonOutputTable;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageLibrary;
        private System.Windows.Forms.TreeView treeViewCreatureLib;
        private System.Windows.Forms.ListView listViewLibrary;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderGender;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeaderOwner;
        private System.Windows.Forms.ToolStripMenuItem loadAndAddToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private CreatureBox creatureBoxListView;
        private System.Windows.Forms.ToolStripMenuItem creatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeaderTopStatsNr;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPagePedigree;
        private Pedigree pedigree1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLibrary;
        private System.Windows.Forms.Label labelTestingInfo;
        private System.Windows.Forms.Button buttonAddTest2Lib;
        private System.Windows.Forms.ColumnHeader columnHeaderGen;
        private System.Windows.Forms.GroupBox groupBoxTestingName;
        private System.Windows.Forms.TextBox textBoxTestingName;
        private System.Windows.Forms.GroupBox groupBoxNameExtractor;
        private System.Windows.Forms.TextBox textBoxExtractorName;
        private System.Windows.Forms.CheckBox checkBoxStatTestingTamed;
    }
}

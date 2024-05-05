using ARKBreedingStats.Pedigree;
using ARKBreedingStats.uiControls;
using static ARKBreedingStats.uiControls.StatWeighting;

namespace ARKBreedingStats.BreedingPlanning
{
    partial class BreedingPlan
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BreedingPlan));
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.gbBPBreedingMode = new System.Windows.Forms.GroupBox();
            this.CbIgnoreSexInPlanning = new System.Windows.Forms.CheckBox();
            this.CbDontSuggestOverLimitOffspring = new System.Windows.Forms.CheckBox();
            this.cbBPMutationLimitOnlyOnePartner = new System.Windows.Forms.CheckBox();
            this.cbBPOnlyOneSuggestionForFemales = new System.Windows.Forms.CheckBox();
            this.cbBPIncludeCryoCreatures = new System.Windows.Forms.CheckBox();
            this.nudBPMutationLimit = new ARKBreedingStats.uiControls.Nud();
            this.label2 = new System.Windows.Forms.Label();
            this.cbBPIncludeCooldowneds = new System.Windows.Forms.CheckBox();
            this.rbBPTopStatsCn = new System.Windows.Forms.RadioButton();
            this.rbBPHighStats = new System.Windows.Forms.RadioButton();
            this.rbBPTopStats = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageBreedableSpecies = new System.Windows.Forms.TabPage();
            this.listViewSpeciesBP = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageTags = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cbTribeFilterLibrary = new System.Windows.Forms.CheckBox();
            this.cbOwnerFilterLibrary = new System.Windows.Forms.CheckBox();
            this.tagSelectorList1 = new ARKBreedingStats.uiControls.TagSelectorList();
            this.cbBPTagExcludeDefault = new System.Windows.Forms.CheckBox();
            this.cbServerFilterLibrary = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statWeighting1 = new ARKBreedingStats.uiControls.StatWeighting();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbBreedingPlanHeader = new System.Windows.Forms.Label();
            this.pedigreeCreatureBestPossibleInSpecies = new ARKBreedingStats.Pedigree.PedigreeCreature();
            this.btShowAllCreatures = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pedigreeCreatureBestPossibleInSpeciesFiltered = new ARKBreedingStats.Pedigree.PedigreeCreature();
            this.pedigreeCreature1 = new ARKBreedingStats.Pedigree.PedigreeCreature();
            this.lbBPBreedingScore = new System.Windows.Forms.Label();
            this.pedigreeCreature2 = new ARKBreedingStats.Pedigree.PedigreeCreature();
            this.gbBPOffspring = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.labelBreedingInfos = new System.Windows.Forms.Label();
            this.listViewRaisingTimes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbBPBreedingTimes = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbBPProbabilityBest = new System.Windows.Forms.Label();
            this.pedigreeCreatureBest = new ARKBreedingStats.Pedigree.PedigreeCreature();
            this.pedigreeCreatureWorst = new ARKBreedingStats.Pedigree.PedigreeCreature();
            this.lbMutationProbability = new System.Windows.Forms.Label();
            this.btBPJustMated = new System.Windows.Forms.Button();
            this.offspringPossibilities1 = new ARKBreedingStats.OffspringPossibilities();
            this.panelCombinations = new System.Windows.Forms.Panel();
            this.lbBreedingPlanInfo = new System.Windows.Forms.Label();
            this.flowLayoutPanelPairs = new System.Windows.Forms.FlowLayoutPanel();
            this.CbConsiderMutationLevels = new System.Windows.Forms.CheckBox();
            this.tableLayoutMain.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.gbBPBreedingMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBPMutationLimit)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageBreedableSpecies.SuspendLayout();
            this.tabPageTags.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.gbBPOffspring.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panelCombinations.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutMain.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMain.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 1;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Size = new System.Drawing.Size(3464, 1967);
            this.tableLayoutMain.TabIndex = 5;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoScroll = true;
            this.tableLayoutPanel5.AutoScrollMinSize = new System.Drawing.Size(0, 700);
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.gbBPBreedingMode, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.statWeighting1, 0, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(488, 1955);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // gbBPBreedingMode
            // 
            this.gbBPBreedingMode.Controls.Add(this.CbConsiderMutationLevels);
            this.gbBPBreedingMode.Controls.Add(this.CbIgnoreSexInPlanning);
            this.gbBPBreedingMode.Controls.Add(this.CbDontSuggestOverLimitOffspring);
            this.gbBPBreedingMode.Controls.Add(this.cbBPMutationLimitOnlyOnePartner);
            this.gbBPBreedingMode.Controls.Add(this.cbBPOnlyOneSuggestionForFemales);
            this.gbBPBreedingMode.Controls.Add(this.cbBPIncludeCryoCreatures);
            this.gbBPBreedingMode.Controls.Add(this.nudBPMutationLimit);
            this.gbBPBreedingMode.Controls.Add(this.label2);
            this.gbBPBreedingMode.Controls.Add(this.cbBPIncludeCooldowneds);
            this.gbBPBreedingMode.Controls.Add(this.rbBPTopStatsCn);
            this.gbBPBreedingMode.Controls.Add(this.rbBPHighStats);
            this.gbBPBreedingMode.Controls.Add(this.rbBPTopStats);
            this.gbBPBreedingMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbBPBreedingMode.Location = new System.Drawing.Point(6, 925);
            this.gbBPBreedingMode.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbBPBreedingMode.Name = "gbBPBreedingMode";
            this.gbBPBreedingMode.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbBPBreedingMode.Size = new System.Drawing.Size(476, 513);
            this.gbBPBreedingMode.TabIndex = 6;
            this.gbBPBreedingMode.TabStop = false;
            this.gbBPBreedingMode.Text = "Breeding-Mode";
            // 
            // CbIgnoreSexInPlanning
            // 
            this.CbIgnoreSexInPlanning.AutoSize = true;
            this.CbIgnoreSexInPlanning.Location = new System.Drawing.Point(12, 384);
            this.CbIgnoreSexInPlanning.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.CbIgnoreSexInPlanning.Name = "CbIgnoreSexInPlanning";
            this.CbIgnoreSexInPlanning.Size = new System.Drawing.Size(283, 29);
            this.CbIgnoreSexInPlanning.TabIndex = 11;
            this.CbIgnoreSexInPlanning.Text = "Ignore sex for all species";
            this.CbIgnoreSexInPlanning.UseVisualStyleBackColor = true;
            this.CbIgnoreSexInPlanning.CheckedChanged += new System.EventHandler(this.CbIgnoreSexInPlanning_CheckedChanged);
            // 
            // CbDontSuggestOverLimitOffspring
            // 
            this.CbDontSuggestOverLimitOffspring.AutoSize = true;
            this.CbDontSuggestOverLimitOffspring.Location = new System.Drawing.Point(12, 466);
            this.CbDontSuggestOverLimitOffspring.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.CbDontSuggestOverLimitOffspring.Name = "CbDontSuggestOverLimitOffspring";
            this.CbDontSuggestOverLimitOffspring.Size = new System.Drawing.Size(356, 29);
            this.CbDontSuggestOverLimitOffspring.TabIndex = 10;
            this.CbDontSuggestOverLimitOffspring.Text = "Don\'t suggest over limit offspring";
            this.CbDontSuggestOverLimitOffspring.UseVisualStyleBackColor = true;
            this.CbDontSuggestOverLimitOffspring.CheckedChanged += new System.EventHandler(this.CbDontSuggestOverLimitOffspring_CheckedChanged);
            // 
            // cbBPMutationLimitOnlyOnePartner
            // 
            this.cbBPMutationLimitOnlyOnePartner.AutoSize = true;
            this.cbBPMutationLimitOnlyOnePartner.Location = new System.Drawing.Point(58, 308);
            this.cbBPMutationLimitOnlyOnePartner.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbBPMutationLimitOnlyOnePartner.Name = "cbBPMutationLimitOnlyOnePartner";
            this.cbBPMutationLimitOnlyOnePartner.Size = new System.Drawing.Size(410, 29);
            this.cbBPMutationLimitOnlyOnePartner.TabIndex = 8;
            this.cbBPMutationLimitOnlyOnePartner.Text = "One partner may have more mutations";
            this.cbBPMutationLimitOnlyOnePartner.UseVisualStyleBackColor = true;
            this.cbBPMutationLimitOnlyOnePartner.CheckedChanged += new System.EventHandler(this.cbMutationLimitOnlyOnePartner_CheckedChanged);
            // 
            // cbBPOnlyOneSuggestionForFemales
            // 
            this.cbBPOnlyOneSuggestionForFemales.AutoSize = true;
            this.cbBPOnlyOneSuggestionForFemales.Location = new System.Drawing.Point(12, 425);
            this.cbBPOnlyOneSuggestionForFemales.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbBPOnlyOneSuggestionForFemales.Name = "cbBPOnlyOneSuggestionForFemales";
            this.cbBPOnlyOneSuggestionForFemales.Size = new System.Drawing.Size(358, 29);
            this.cbBPOnlyOneSuggestionForFemales.TabIndex = 7;
            this.cbBPOnlyOneSuggestionForFemales.Text = "Only best suggestion for females";
            this.cbBPOnlyOneSuggestionForFemales.UseVisualStyleBackColor = true;
            this.cbBPOnlyOneSuggestionForFemales.CheckedChanged += new System.EventHandler(this.cbOnlyOneSuggestionForFemales_CheckedChanged);
            // 
            // cbBPIncludeCryoCreatures
            // 
            this.cbBPIncludeCryoCreatures.AutoSize = true;
            this.cbBPIncludeCryoCreatures.Location = new System.Drawing.Point(12, 213);
            this.cbBPIncludeCryoCreatures.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbBPIncludeCryoCreatures.Name = "cbBPIncludeCryoCreatures";
            this.cbBPIncludeCryoCreatures.Size = new System.Drawing.Size(334, 29);
            this.cbBPIncludeCryoCreatures.TabIndex = 6;
            this.cbBPIncludeCryoCreatures.Text = "Include Creatures in Cryopods";
            this.cbBPIncludeCryoCreatures.UseVisualStyleBackColor = true;
            this.cbBPIncludeCryoCreatures.CheckedChanged += new System.EventHandler(this.cbBPIncludeCryoCreatures_CheckedChanged);
            // 
            // nudBPMutationLimit
            // 
            this.nudBPMutationLimit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudBPMutationLimit.Location = new System.Drawing.Point(324, 258);
            this.nudBPMutationLimit.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.nudBPMutationLimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudBPMutationLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.nudBPMutationLimit.Name = "nudBPMutationLimit";
            this.nudBPMutationLimit.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudBPMutationLimit.Size = new System.Drawing.Size(100, 31);
            this.nudBPMutationLimit.TabIndex = 4;
            this.nudBPMutationLimit.ValueChanged += new System.EventHandler(this.nudMutationLimit_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 262);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Creatures with Mutations up to";
            // 
            // cbBPIncludeCooldowneds
            // 
            this.cbBPIncludeCooldowneds.AutoSize = true;
            this.cbBPIncludeCooldowneds.Location = new System.Drawing.Point(12, 169);
            this.cbBPIncludeCooldowneds.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbBPIncludeCooldowneds.Name = "cbBPIncludeCooldowneds";
            this.cbBPIncludeCooldowneds.Size = new System.Drawing.Size(358, 29);
            this.cbBPIncludeCooldowneds.TabIndex = 3;
            this.cbBPIncludeCooldowneds.Text = "Include Creatures with Cooldown";
            this.cbBPIncludeCooldowneds.UseVisualStyleBackColor = true;
            this.cbBPIncludeCooldowneds.CheckedChanged += new System.EventHandler(this.checkBoxIncludeCooldowneds_CheckedChanged);
            // 
            // rbBPTopStatsCn
            // 
            this.rbBPTopStatsCn.AutoSize = true;
            this.rbBPTopStatsCn.Checked = true;
            this.rbBPTopStatsCn.Location = new System.Drawing.Point(12, 37);
            this.rbBPTopStatsCn.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbBPTopStatsCn.Name = "rbBPTopStatsCn";
            this.rbBPTopStatsCn.Size = new System.Drawing.Size(226, 29);
            this.rbBPTopStatsCn.TabIndex = 2;
            this.rbBPTopStatsCn.TabStop = true;
            this.rbBPTopStatsCn.Text = "Combine Top Stats";
            this.rbBPTopStatsCn.UseVisualStyleBackColor = true;
            this.rbBPTopStatsCn.CheckedChanged += new System.EventHandler(this.radioButtonBPTopStatsCn_CheckedChanged);
            // 
            // rbBPHighStats
            // 
            this.rbBPHighStats.AutoSize = true;
            this.rbBPHighStats.Location = new System.Drawing.Point(12, 125);
            this.rbBPHighStats.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbBPHighStats.Name = "rbBPHighStats";
            this.rbBPHighStats.Size = new System.Drawing.Size(248, 29);
            this.rbBPHighStats.TabIndex = 1;
            this.rbBPHighStats.Text = "Best Next Generation";
            this.rbBPHighStats.UseVisualStyleBackColor = true;
            this.rbBPHighStats.CheckedChanged += new System.EventHandler(this.radioButtonBPHighStats_CheckedChanged);
            // 
            // rbBPTopStats
            // 
            this.rbBPTopStats.AutoSize = true;
            this.rbBPTopStats.Location = new System.Drawing.Point(12, 81);
            this.rbBPTopStats.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.rbBPTopStats.Name = "rbBPTopStats";
            this.rbBPTopStats.Size = new System.Drawing.Size(164, 29);
            this.rbBPTopStats.TabIndex = 0;
            this.rbBPTopStats.Text = "Top Stats Lc";
            this.rbBPTopStats.UseVisualStyleBackColor = true;
            this.rbBPTopStats.CheckedChanged += new System.EventHandler(this.radioButtonBPTopStats_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageBreedableSpecies);
            this.tabControl1.Controls.Add(this.tabPageTags);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(6, 6);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabControl1.MinimumSize = new System.Drawing.Size(0, 385);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(476, 907);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageBreedableSpecies
            // 
            this.tabPageBreedableSpecies.Controls.Add(this.listViewSpeciesBP);
            this.tabPageBreedableSpecies.Location = new System.Drawing.Point(8, 39);
            this.tabPageBreedableSpecies.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageBreedableSpecies.Name = "tabPageBreedableSpecies";
            this.tabPageBreedableSpecies.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageBreedableSpecies.Size = new System.Drawing.Size(460, 860);
            this.tabPageBreedableSpecies.TabIndex = 0;
            this.tabPageBreedableSpecies.Text = "Breedable Species";
            this.tabPageBreedableSpecies.UseVisualStyleBackColor = true;
            // 
            // listViewSpeciesBP
            // 
            this.listViewSpeciesBP.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
            this.listViewSpeciesBP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSpeciesBP.FullRowSelect = true;
            this.listViewSpeciesBP.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewSpeciesBP.HideSelection = false;
            this.listViewSpeciesBP.Location = new System.Drawing.Point(6, 6);
            this.listViewSpeciesBP.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.listViewSpeciesBP.MultiSelect = false;
            this.listViewSpeciesBP.Name = "listViewSpeciesBP";
            this.listViewSpeciesBP.Size = new System.Drawing.Size(448, 848);
            this.listViewSpeciesBP.TabIndex = 3;
            this.listViewSpeciesBP.UseCompatibleStateImageBehavior = false;
            this.listViewSpeciesBP.View = System.Windows.Forms.View.Details;
            this.listViewSpeciesBP.SelectedIndexChanged += new System.EventHandler(this.listViewSpeciesBP_SelectedIndexChanged);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Species";
            this.columnHeader5.Width = 178;
            // 
            // tabPageTags
            // 
            this.tabPageTags.Controls.Add(this.tableLayoutPanel3);
            this.tabPageTags.Location = new System.Drawing.Point(8, 39);
            this.tabPageTags.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageTags.Name = "tabPageTags";
            this.tabPageTags.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tabPageTags.Size = new System.Drawing.Size(460, 891);
            this.tabPageTags.TabIndex = 1;
            this.tabPageTags.Text = "Filters / Tags";
            this.tabPageTags.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.cbTribeFilterLibrary, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.cbOwnerFilterLibrary, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tagSelectorList1, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.cbBPTagExcludeDefault, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.cbServerFilterLibrary, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(448, 879);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // cbTribeFilterLibrary
            // 
            this.cbTribeFilterLibrary.AutoSize = true;
            this.cbTribeFilterLibrary.Location = new System.Drawing.Point(6, 47);
            this.cbTribeFilterLibrary.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbTribeFilterLibrary.Name = "cbTribeFilterLibrary";
            this.cbTribeFilterLibrary.Size = new System.Drawing.Size(260, 29);
            this.cbTribeFilterLibrary.TabIndex = 7;
            this.cbTribeFilterLibrary.Text = "Tribe filter from Library";
            this.cbTribeFilterLibrary.UseVisualStyleBackColor = true;
            this.cbTribeFilterLibrary.CheckedChanged += new System.EventHandler(this.cbTribeFilterLibrary_CheckedChanged);
            // 
            // cbOwnerFilterLibrary
            // 
            this.cbOwnerFilterLibrary.AutoSize = true;
            this.cbOwnerFilterLibrary.Location = new System.Drawing.Point(6, 6);
            this.cbOwnerFilterLibrary.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbOwnerFilterLibrary.Name = "cbOwnerFilterLibrary";
            this.cbOwnerFilterLibrary.Size = new System.Drawing.Size(273, 29);
            this.cbOwnerFilterLibrary.TabIndex = 6;
            this.cbOwnerFilterLibrary.Text = "Owner filter from Library";
            this.cbOwnerFilterLibrary.UseVisualStyleBackColor = true;
            this.cbOwnerFilterLibrary.CheckedChanged += new System.EventHandler(this.cbOwnerFilterLibrary_CheckedChanged);
            // 
            // tagSelectorList1
            // 
            this.tagSelectorList1.AutoScroll = true;
            this.tagSelectorList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tagSelectorList1.Location = new System.Drawing.Point(12, 309);
            this.tagSelectorList1.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.tagSelectorList1.Name = "tagSelectorList1";
            this.tagSelectorList1.Size = new System.Drawing.Size(424, 558);
            this.tagSelectorList1.TabIndex = 3;
            // 
            // cbBPTagExcludeDefault
            // 
            this.cbBPTagExcludeDefault.AutoSize = true;
            this.cbBPTagExcludeDefault.Location = new System.Drawing.Point(6, 262);
            this.cbBPTagExcludeDefault.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbBPTagExcludeDefault.Name = "cbBPTagExcludeDefault";
            this.cbBPTagExcludeDefault.Size = new System.Drawing.Size(317, 29);
            this.cbBPTagExcludeDefault.TabIndex = 4;
            this.cbBPTagExcludeDefault.Text = "Exclude creatures by default";
            this.cbBPTagExcludeDefault.UseVisualStyleBackColor = true;
            this.cbBPTagExcludeDefault.CheckedChanged += new System.EventHandler(this.cbTagExcludeDefault_CheckedChanged);
            // 
            // cbServerFilterLibrary
            // 
            this.cbServerFilterLibrary.AutoSize = true;
            this.cbServerFilterLibrary.Location = new System.Drawing.Point(6, 88);
            this.cbServerFilterLibrary.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbServerFilterLibrary.Name = "cbServerFilterLibrary";
            this.cbServerFilterLibrary.Size = new System.Drawing.Size(274, 29);
            this.cbServerFilterLibrary.TabIndex = 5;
            this.cbServerFilterLibrary.Text = "Server filter from Library";
            this.cbServerFilterLibrary.UseVisualStyleBackColor = true;
            this.cbServerFilterLibrary.CheckedChanged += new System.EventHandler(this.cbServerFilterLibrary_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 123);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 133);
            this.label1.TabIndex = 2;
            this.label1.Text = "Consider creatures by tag. \r\n✕ excludes creatures, ✓ includes creatures (even if " +
    "they have an exclusive tag). Add tags in the library with F3.";
            // 
            // statWeighting1
            // 
            this.statWeighting1.AnyOddEven = new ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd[] {
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent,
        ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd.Indifferent};
            this.statWeighting1.CustomWeightings = ((System.Collections.Generic.Dictionary<string, System.ValueTuple<double[], ARKBreedingStats.uiControls.StatWeighting.StatValueEvenOdd[]>>)(resources.GetObject("statWeighting1.CustomWeightings")));
            this.statWeighting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statWeighting1.Location = new System.Drawing.Point(12, 1456);
            this.statWeighting1.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.statWeighting1.Name = "statWeighting1";
            this.statWeighting1.Size = new System.Drawing.Size(464, 487);
            this.statWeighting1.TabIndex = 7;
            this.statWeighting1.WeightValues = new double[] {
        1D,
        1D,
        1D,
        1D,
        1D,
        1D,
        1D,
        1D,
        1D,
        1D,
        1D,
        1D};
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gbBPOffspring, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panelCombinations, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(506, 6);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(2952, 1955);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lbBreedingPlanHeader);
            this.flowLayoutPanel1.Controls.Add(this.pedigreeCreatureBestPossibleInSpecies);
            this.flowLayoutPanel1.Controls.Add(this.btShowAllCreatures);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.pedigreeCreatureBestPossibleInSpeciesFiltered);
            this.flowLayoutPanel1.Controls.Add(this.pedigreeCreature1);
            this.flowLayoutPanel1.Controls.Add(this.lbBPBreedingScore);
            this.flowLayoutPanel1.Controls.Add(this.pedigreeCreature2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(2940, 310);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // lbBreedingPlanHeader
            // 
            this.lbBreedingPlanHeader.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.lbBreedingPlanHeader, true);
            this.lbBreedingPlanHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBreedingPlanHeader.Location = new System.Drawing.Point(6, 0);
            this.lbBreedingPlanHeader.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbBreedingPlanHeader.MinimumSize = new System.Drawing.Size(1400, 0);
            this.lbBreedingPlanHeader.Name = "lbBreedingPlanHeader";
            this.lbBreedingPlanHeader.Size = new System.Drawing.Size(1400, 37);
            this.lbBreedingPlanHeader.TabIndex = 1;
            this.lbBreedingPlanHeader.Text = "Select a species and click on \"Determine Best Breeding\" to see suggestions";
            this.lbBreedingPlanHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pedigreeCreatureBestPossibleInSpecies
            // 
            this.pedigreeCreatureBestPossibleInSpecies.Creature = null;
            this.pedigreeCreatureBestPossibleInSpecies.Location = new System.Drawing.Point(12, 103);
            this.pedigreeCreatureBestPossibleInSpecies.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.pedigreeCreatureBestPossibleInSpecies.Name = "pedigreeCreatureBestPossibleInSpecies";
            this.pedigreeCreatureBestPossibleInSpecies.OnlyLevels = false;
            this.pedigreeCreatureBestPossibleInSpecies.Size = new System.Drawing.Size(650, 67);
            this.pedigreeCreatureBestPossibleInSpecies.TabIndex = 5;
            this.pedigreeCreatureBestPossibleInSpecies.TotalLevelUnknown = false;
            // 
            // btShowAllCreatures
            // 
            this.btShowAllCreatures.Location = new System.Drawing.Point(864, 97);
            this.btShowAllCreatures.Margin = new System.Windows.Forms.Padding(190, 6, 6, 6);
            this.btShowAllCreatures.Name = "btShowAllCreatures";
            this.btShowAllCreatures.Size = new System.Drawing.Size(594, 67);
            this.btShowAllCreatures.TabIndex = 6;
            this.btShowAllCreatures.Text = "Unset restriction to …";
            this.btShowAllCreatures.UseVisualStyleBackColor = true;
            this.btShowAllCreatures.Click += new System.EventHandler(this.btShowAllCreatures_Click);
            // 
            // panel1
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.panel1, true);
            this.panel1.Location = new System.Drawing.Point(1470, 97);
            this.panel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(20, 62);
            this.panel1.TabIndex = 7;
            // 
            // pedigreeCreatureBestPossibleInSpeciesFiltered
            // 
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.Creature = null;
            this.flowLayoutPanel1.SetFlowBreak(this.pedigreeCreatureBestPossibleInSpeciesFiltered, true);
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.Location = new System.Drawing.Point(12, 194);
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.Name = "pedigreeCreatureBestPossibleInSpeciesFiltered";
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.OnlyLevels = false;
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.Size = new System.Drawing.Size(650, 67);
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.TabIndex = 8;
            this.pedigreeCreatureBestPossibleInSpeciesFiltered.TotalLevelUnknown = false;
            // 
            // pedigreeCreature1
            // 
            this.pedigreeCreature1.Creature = null;
            this.pedigreeCreature1.Location = new System.Drawing.Point(6, 273);
            this.pedigreeCreature1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 6);
            this.pedigreeCreature1.Name = "pedigreeCreature1";
            this.pedigreeCreature1.OnlyLevels = false;
            this.pedigreeCreature1.Size = new System.Drawing.Size(650, 67);
            this.pedigreeCreature1.TabIndex = 2;
            this.pedigreeCreature1.TotalLevelUnknown = false;
            // 
            // lbBPBreedingScore
            // 
            this.lbBPBreedingScore.Location = new System.Drawing.Point(668, 302);
            this.lbBPBreedingScore.Margin = new System.Windows.Forms.Padding(6, 29, 6, 0);
            this.lbBPBreedingScore.Name = "lbBPBreedingScore";
            this.lbBPBreedingScore.Size = new System.Drawing.Size(174, 38);
            this.lbBPBreedingScore.TabIndex = 4;
            this.lbBPBreedingScore.Text = "Breeding-Score";
            // 
            // pedigreeCreature2
            // 
            this.pedigreeCreature2.Creature = null;
            this.pedigreeCreature2.Location = new System.Drawing.Point(854, 273);
            this.pedigreeCreature2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 6);
            this.pedigreeCreature2.Name = "pedigreeCreature2";
            this.pedigreeCreature2.OnlyLevels = false;
            this.pedigreeCreature2.Size = new System.Drawing.Size(650, 67);
            this.pedigreeCreature2.TabIndex = 3;
            this.pedigreeCreature2.TotalLevelUnknown = false;
            // 
            // gbBPOffspring
            // 
            this.gbBPOffspring.Controls.Add(this.tableLayoutPanel2);
            this.gbBPOffspring.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbBPOffspring.Location = new System.Drawing.Point(6, 1611);
            this.gbBPOffspring.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbBPOffspring.Name = "gbBPOffspring";
            this.gbBPOffspring.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbBPOffspring.Size = new System.Drawing.Size(2940, 338);
            this.gbBPOffspring.TabIndex = 2;
            this.gbBPOffspring.TabStop = false;
            this.gbBPOffspring.Text = "Offspring";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.offspringPossibilities1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 30);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(2928, 302);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.labelBreedingInfos, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.listViewRaisingTimes, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.lbBPBreedingTimes, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(1240, 6);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1694, 290);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // labelBreedingInfos
            // 
            this.labelBreedingInfos.AutoSize = true;
            this.labelBreedingInfos.Location = new System.Drawing.Point(6, 228);
            this.labelBreedingInfos.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelBreedingInfos.Name = "labelBreedingInfos";
            this.labelBreedingInfos.Size = new System.Drawing.Size(150, 25);
            this.labelBreedingInfos.TabIndex = 7;
            this.labelBreedingInfos.Text = "Breeding Infos";
            // 
            // listViewRaisingTimes
            // 
            this.listViewRaisingTimes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewRaisingTimes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewRaisingTimes.HideSelection = false;
            this.listViewRaisingTimes.Location = new System.Drawing.Point(6, 37);
            this.listViewRaisingTimes.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.listViewRaisingTimes.Name = "listViewRaisingTimes";
            this.listViewRaisingTimes.ShowGroups = false;
            this.listViewRaisingTimes.Size = new System.Drawing.Size(698, 185);
            this.listViewRaisingTimes.TabIndex = 4;
            this.listViewRaisingTimes.UseCompatibleStateImageBehavior = false;
            this.listViewRaisingTimes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Time";
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Total Time";
            this.columnHeader3.Width = 70;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Finished at";
            this.columnHeader4.Width = 103;
            // 
            // lbBPBreedingTimes
            // 
            this.lbBPBreedingTimes.AutoSize = true;
            this.lbBPBreedingTimes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBPBreedingTimes.Location = new System.Drawing.Point(6, 0);
            this.lbBPBreedingTimes.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbBPBreedingTimes.Name = "lbBPBreedingTimes";
            this.lbBPBreedingTimes.Size = new System.Drawing.Size(217, 31);
            this.lbBPBreedingTimes.TabIndex = 3;
            this.lbBPBreedingTimes.Text = "Breeding Times";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.lbBPProbabilityBest);
            this.flowLayoutPanel2.Controls.Add(this.pedigreeCreatureBest);
            this.flowLayoutPanel2.Controls.Add(this.pedigreeCreatureWorst);
            this.flowLayoutPanel2.Controls.Add(this.lbMutationProbability);
            this.flowLayoutPanel2.Controls.Add(this.btBPJustMated);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(6, 6);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(704, 290);
            this.flowLayoutPanel2.TabIndex = 8;
            // 
            // lbBPProbabilityBest
            // 
            this.lbBPProbabilityBest.AutoSize = true;
            this.flowLayoutPanel2.SetFlowBreak(this.lbBPProbabilityBest, true);
            this.lbBPProbabilityBest.Location = new System.Drawing.Point(6, 0);
            this.lbBPProbabilityBest.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbBPProbabilityBest.Name = "lbBPProbabilityBest";
            this.lbBPProbabilityBest.Size = new System.Drawing.Size(415, 25);
            this.lbBPProbabilityBest.TabIndex = 6;
            this.lbBPProbabilityBest.Text = "Probability for this Best Possible outcome:";
            // 
            // pedigreeCreatureBest
            // 
            this.pedigreeCreatureBest.Creature = null;
            this.pedigreeCreatureBest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flowLayoutPanel2.SetFlowBreak(this.pedigreeCreatureBest, true);
            this.pedigreeCreatureBest.Location = new System.Drawing.Point(12, 37);
            this.pedigreeCreatureBest.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.pedigreeCreatureBest.Name = "pedigreeCreatureBest";
            this.pedigreeCreatureBest.OnlyLevels = false;
            this.pedigreeCreatureBest.Size = new System.Drawing.Size(650, 67);
            this.pedigreeCreatureBest.TabIndex = 1;
            this.pedigreeCreatureBest.TotalLevelUnknown = false;
            // 
            // pedigreeCreatureWorst
            // 
            this.pedigreeCreatureWorst.Creature = null;
            this.pedigreeCreatureWorst.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flowLayoutPanel2.SetFlowBreak(this.pedigreeCreatureWorst, true);
            this.pedigreeCreatureWorst.Location = new System.Drawing.Point(12, 128);
            this.pedigreeCreatureWorst.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.pedigreeCreatureWorst.Name = "pedigreeCreatureWorst";
            this.pedigreeCreatureWorst.OnlyLevels = false;
            this.pedigreeCreatureWorst.Size = new System.Drawing.Size(650, 67);
            this.pedigreeCreatureWorst.TabIndex = 2;
            this.pedigreeCreatureWorst.TotalLevelUnknown = false;
            // 
            // lbMutationProbability
            // 
            this.lbMutationProbability.AutoSize = true;
            this.flowLayoutPanel2.SetFlowBreak(this.lbMutationProbability, true);
            this.lbMutationProbability.Location = new System.Drawing.Point(6, 207);
            this.lbMutationProbability.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbMutationProbability.Name = "lbMutationProbability";
            this.lbMutationProbability.Size = new System.Drawing.Size(236, 25);
            this.lbMutationProbability.TabIndex = 7;
            this.lbMutationProbability.Text = "Probability of mutations";
            // 
            // btBPJustMated
            // 
            this.btBPJustMated.Location = new System.Drawing.Point(6, 238);
            this.btBPJustMated.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btBPJustMated.Name = "btBPJustMated";
            this.btBPJustMated.Size = new System.Drawing.Size(650, 56);
            this.btBPJustMated.TabIndex = 0;
            this.btBPJustMated.Text = "These Parents just mated";
            this.btBPJustMated.UseVisualStyleBackColor = true;
            this.btBPJustMated.Click += new System.EventHandler(this.buttonJustMated_Click);
            // 
            // offspringPossibilities1
            // 
            this.offspringPossibilities1.Location = new System.Drawing.Point(728, 12);
            this.offspringPossibilities1.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.offspringPossibilities1.Name = "offspringPossibilities1";
            this.offspringPossibilities1.Size = new System.Drawing.Size(494, 258);
            this.offspringPossibilities1.TabIndex = 1;
            // 
            // panelCombinations
            // 
            this.panelCombinations.Controls.Add(this.lbBreedingPlanInfo);
            this.panelCombinations.Controls.Add(this.flowLayoutPanelPairs);
            this.panelCombinations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCombinations.Location = new System.Drawing.Point(6, 328);
            this.panelCombinations.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panelCombinations.Name = "panelCombinations";
            this.panelCombinations.Size = new System.Drawing.Size(2940, 1271);
            this.panelCombinations.TabIndex = 3;
            // 
            // lbBreedingPlanInfo
            // 
            this.lbBreedingPlanInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBreedingPlanInfo.Location = new System.Drawing.Point(20, 144);
            this.lbBreedingPlanInfo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbBreedingPlanInfo.Name = "lbBreedingPlanInfo";
            this.lbBreedingPlanInfo.Size = new System.Drawing.Size(1366, 371);
            this.lbBreedingPlanInfo.TabIndex = 0;
            this.lbBreedingPlanInfo.Text = "Infotext";
            this.lbBreedingPlanInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbBreedingPlanInfo.Visible = false;
            // 
            // flowLayoutPanelPairs
            // 
            this.flowLayoutPanelPairs.AutoScroll = true;
            this.flowLayoutPanelPairs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelPairs.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelPairs.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.flowLayoutPanelPairs.Name = "flowLayoutPanelPairs";
            this.flowLayoutPanelPairs.Size = new System.Drawing.Size(2940, 1271);
            this.flowLayoutPanelPairs.TabIndex = 1;
            // 
            // CbConsiderMutationLevels
            // 
            this.CbConsiderMutationLevels.AutoSize = true;
            this.CbConsiderMutationLevels.Location = new System.Drawing.Point(12, 346);
            this.CbConsiderMutationLevels.Name = "CbConsiderMutationLevels";
            this.CbConsiderMutationLevels.Size = new System.Drawing.Size(280, 29);
            this.CbConsiderMutationLevels.TabIndex = 12;
            this.CbConsiderMutationLevels.Text = "Consider mutation levels";
            this.CbConsiderMutationLevels.UseVisualStyleBackColor = true;
            this.CbConsiderMutationLevels.CheckedChanged += new System.EventHandler(this.CbConsiderMutationLevels_CheckedChanged);
            // 
            // BreedingPlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutMain);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "BreedingPlan";
            this.Size = new System.Drawing.Size(3464, 1967);
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.gbBPBreedingMode.ResumeLayout(false);
            this.gbBPBreedingMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBPMutationLimit)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageBreedableSpecies.ResumeLayout(false);
            this.tabPageTags.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.gbBPOffspring.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panelCombinations.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.ListView listViewSpeciesBP;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.GroupBox gbBPBreedingMode;
        private System.Windows.Forms.CheckBox cbBPIncludeCooldowneds;
        private System.Windows.Forms.RadioButton rbBPTopStatsCn;
        private System.Windows.Forms.RadioButton rbBPHighStats;
        private System.Windows.Forms.RadioButton rbBPTopStats;
        private StatWeighting statWeighting1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageBreedableSpecies;
        private System.Windows.Forms.TabPage tabPageTags;
        private System.Windows.Forms.Label label1;
        private uiControls.TagSelectorList tagSelectorList1;
        private uiControls.Nud nudBPMutationLimit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbBPTagExcludeDefault;
        private System.Windows.Forms.CheckBox cbServerFilterLibrary;
        private System.Windows.Forms.CheckBox cbOwnerFilterLibrary;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lbBreedingPlanHeader;
        private PedigreeCreature pedigreeCreatureBestPossibleInSpecies;
        private System.Windows.Forms.Button btShowAllCreatures;
        private System.Windows.Forms.Panel panel1;
        private PedigreeCreature pedigreeCreature1;
        private System.Windows.Forms.Label lbBPBreedingScore;
        private PedigreeCreature pedigreeCreature2;
        private System.Windows.Forms.GroupBox gbBPOffspring;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label labelBreedingInfos;
        private System.Windows.Forms.ListView listViewRaisingTimes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label lbBPBreedingTimes;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lbBPProbabilityBest;
        private PedigreeCreature pedigreeCreatureBest;
        private PedigreeCreature pedigreeCreatureWorst;
        private System.Windows.Forms.Button btBPJustMated;
        private OffspringPossibilities offspringPossibilities1;
        private System.Windows.Forms.Panel panelCombinations;
        private System.Windows.Forms.Label lbBreedingPlanInfo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelPairs;
        private System.Windows.Forms.CheckBox cbBPIncludeCryoCreatures;
        private System.Windows.Forms.CheckBox cbBPOnlyOneSuggestionForFemales;
        private System.Windows.Forms.CheckBox cbBPMutationLimitOnlyOnePartner;
        private System.Windows.Forms.Label lbMutationProbability;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox cbTribeFilterLibrary;
        private PedigreeCreature pedigreeCreatureBestPossibleInSpeciesFiltered;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.CheckBox CbDontSuggestOverLimitOffspring;
        private System.Windows.Forms.CheckBox CbIgnoreSexInPlanning;
        private System.Windows.Forms.CheckBox CbConsiderMutationLevels;
    }
}

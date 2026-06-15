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
            components = new System.ComponentModel.Container();
            tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            gbBPBreedingMode = new GroupBoxC();
            CbOnlySameSpecies = new System.Windows.Forms.CheckBox();
            CbConsiderMutationLevels = new System.Windows.Forms.CheckBox();
            CbIgnoreSexInPlanning = new System.Windows.Forms.CheckBox();
            CbDontSuggestOverLimitOffspring = new System.Windows.Forms.CheckBox();
            cbBPMutationLimitOnlyOnePartner = new System.Windows.Forms.CheckBox();
            cbBPOnlyOneSuggestionForFemales = new System.Windows.Forms.CheckBox();
            cbBPIncludeCryoCreatures = new System.Windows.Forms.CheckBox();
            nudBPMutationLimit = new Nud();
            label2 = new System.Windows.Forms.Label();
            cbBPIncludeCooldowneds = new System.Windows.Forms.CheckBox();
            rbBPTopStatsCn = new System.Windows.Forms.RadioButton();
            rbBPHighStats = new System.Windows.Forms.RadioButton();
            rbBPTopStats = new System.Windows.Forms.RadioButton();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPageBreedableSpecies = new System.Windows.Forms.TabPage();
            listViewSpeciesBP = new System.Windows.Forms.ListView();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            tabPageTags = new System.Windows.Forms.TabPage();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            cbTribeFilterLibrary = new System.Windows.Forms.CheckBox();
            cbOwnerFilterLibrary = new System.Windows.Forms.CheckBox();
            tagSelectorList1 = new TagSelectorList();
            cbBPTagExcludeDefault = new System.Windows.Forms.CheckBox();
            cbServerFilterLibrary = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            statWeighting1 = new StatWeighting();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            lbBreedingPlanHeader = new System.Windows.Forms.Label();
            pedigreeCreatureBestPossibleInSpecies = new PedigreeCreature();
            btShowAllCreatures = new System.Windows.Forms.Button();
            BtRecalculatePlan = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            pedigreeCreatureBestPossibleInSpeciesFiltered = new PedigreeCreature();
            pedigreeCreature1 = new PedigreeCreature();
            lbBPBreedingScore = new System.Windows.Forms.Label();
            pedigreeCreature2 = new PedigreeCreature();
            gbBPOffspring = new GroupBoxC();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            labelBreedingInfos = new System.Windows.Forms.Label();
            listViewRaisingTimes = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            lbBPBreedingTimes = new System.Windows.Forms.Label();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            lbBPProbabilityBest = new System.Windows.Forms.Label();
            pedigreeCreatureBest = new PedigreeCreature();
            pedigreeCreatureWorst = new PedigreeCreature();
            lbMutationProbability = new System.Windows.Forms.Label();
            btBPJustMated = new System.Windows.Forms.Button();
            tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            offspringPossibilities1 = new OffspringPossibilities();
            LbMinTotalLevelTopStats = new System.Windows.Forms.Label();
            panelCombinations = new System.Windows.Forms.Panel();
            lbBreedingPlanInfo = new System.Windows.Forms.Label();
            flowLayoutPanelPairs = new System.Windows.Forms.FlowLayoutPanel();
            tableLayoutMain.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            gbBPBreedingMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudBPMutationLimit).BeginInit();
            tabControl1.SuspendLayout();
            tabPageBreedableSpecies.SuspendLayout();
            tabPageTags.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            gbBPOffspring.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            panelCombinations.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutMain
            // 
            tableLayoutMain.ColumnCount = 2;
            tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutMain.Controls.Add(tableLayoutPanel5, 0, 0);
            tableLayoutMain.Controls.Add(tableLayoutPanel1, 1, 0);
            tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutMain.Location = new System.Drawing.Point(0, 0);
            tableLayoutMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutMain.Name = "tableLayoutMain";
            tableLayoutMain.RowCount = 1;
            tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutMain.Size = new System.Drawing.Size(2021, 1180);
            tableLayoutMain.TabIndex = 5;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.AutoScroll = true;
            tableLayoutPanel5.AutoScrollMinSize = new System.Drawing.Size(0, 700);
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel5.Controls.Add(gbBPBreedingMode, 0, 1);
            tableLayoutPanel5.Controls.Add(tabControl1, 0, 0);
            tableLayoutPanel5.Controls.Add(statWeighting1, 0, 2);
            tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel5.Location = new System.Drawing.Point(4, 3);
            tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 3;
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel5.Size = new System.Drawing.Size(285, 1174);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // gbBPBreedingMode
            // 
            gbBPBreedingMode.Controls.Add(CbOnlySameSpecies);
            gbBPBreedingMode.Controls.Add(CbConsiderMutationLevels);
            gbBPBreedingMode.Controls.Add(CbIgnoreSexInPlanning);
            gbBPBreedingMode.Controls.Add(CbDontSuggestOverLimitOffspring);
            gbBPBreedingMode.Controls.Add(cbBPMutationLimitOnlyOnePartner);
            gbBPBreedingMode.Controls.Add(cbBPOnlyOneSuggestionForFemales);
            gbBPBreedingMode.Controls.Add(cbBPIncludeCryoCreatures);
            gbBPBreedingMode.Controls.Add(nudBPMutationLimit);
            gbBPBreedingMode.Controls.Add(label2);
            gbBPBreedingMode.Controls.Add(cbBPIncludeCooldowneds);
            gbBPBreedingMode.Controls.Add(rbBPTopStatsCn);
            gbBPBreedingMode.Controls.Add(rbBPHighStats);
            gbBPBreedingMode.Controls.Add(rbBPTopStats);
            gbBPBreedingMode.Dock = System.Windows.Forms.DockStyle.Fill;
            gbBPBreedingMode.Location = new System.Drawing.Point(4, 541);
            gbBPBreedingMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbBPBreedingMode.Name = "gbBPBreedingMode";
            gbBPBreedingMode.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbBPBreedingMode.Size = new System.Drawing.Size(277, 324);
            gbBPBreedingMode.TabIndex = 6;
            gbBPBreedingMode.TabStop = false;
            gbBPBreedingMode.Text = "Breeding-Mode";
            // 
            // CbOnlySameSpecies
            // 
            CbOnlySameSpecies.AutoSize = true;
            CbOnlySameSpecies.Location = new System.Drawing.Point(7, 302);
            CbOnlySameSpecies.Margin = new System.Windows.Forms.Padding(2);
            CbOnlySameSpecies.Name = "CbOnlySameSpecies";
            CbOnlySameSpecies.Size = new System.Drawing.Size(201, 19);
            CbOnlySameSpecies.TabIndex = 13;
            CbOnlySameSpecies.Text = "Exclude other compatible species";
            CbOnlySameSpecies.UseVisualStyleBackColor = true;
            CbOnlySameSpecies.CheckedChanged += CbOnlySameSpecies_CheckedChanged;
            // 
            // CbConsiderMutationLevels
            // 
            CbConsiderMutationLevels.AutoSize = true;
            CbConsiderMutationLevels.Location = new System.Drawing.Point(7, 208);
            CbConsiderMutationLevels.Margin = new System.Windows.Forms.Padding(2);
            CbConsiderMutationLevels.Name = "CbConsiderMutationLevels";
            CbConsiderMutationLevels.Size = new System.Drawing.Size(157, 19);
            CbConsiderMutationLevels.TabIndex = 12;
            CbConsiderMutationLevels.Text = "Consider mutation levels";
            CbConsiderMutationLevels.UseVisualStyleBackColor = true;
            CbConsiderMutationLevels.CheckedChanged += CbConsiderMutationLevels_CheckedChanged;
            // 
            // CbIgnoreSexInPlanning
            // 
            CbIgnoreSexInPlanning.AutoSize = true;
            CbIgnoreSexInPlanning.Location = new System.Drawing.Point(7, 231);
            CbIgnoreSexInPlanning.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbIgnoreSexInPlanning.Name = "CbIgnoreSexInPlanning";
            CbIgnoreSexInPlanning.Size = new System.Drawing.Size(153, 19);
            CbIgnoreSexInPlanning.TabIndex = 11;
            CbIgnoreSexInPlanning.Text = "Ignore sex for all species";
            CbIgnoreSexInPlanning.UseVisualStyleBackColor = true;
            CbIgnoreSexInPlanning.CheckedChanged += CbIgnoreSexInPlanning_CheckedChanged;
            // 
            // CbDontSuggestOverLimitOffspring
            // 
            CbDontSuggestOverLimitOffspring.AutoSize = true;
            CbDontSuggestOverLimitOffspring.Location = new System.Drawing.Point(7, 279);
            CbDontSuggestOverLimitOffspring.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbDontSuggestOverLimitOffspring.Name = "CbDontSuggestOverLimitOffspring";
            CbDontSuggestOverLimitOffspring.Size = new System.Drawing.Size(203, 19);
            CbDontSuggestOverLimitOffspring.TabIndex = 10;
            CbDontSuggestOverLimitOffspring.Text = "Don't suggest over limit offspring";
            CbDontSuggestOverLimitOffspring.UseVisualStyleBackColor = true;
            CbDontSuggestOverLimitOffspring.CheckedChanged += CbDontSuggestOverLimitOffspring_CheckedChanged;
            // 
            // cbBPMutationLimitOnlyOnePartner
            // 
            cbBPMutationLimitOnlyOnePartner.AutoSize = true;
            cbBPMutationLimitOnlyOnePartner.Location = new System.Drawing.Point(34, 185);
            cbBPMutationLimitOnlyOnePartner.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBPMutationLimitOnlyOnePartner.Name = "cbBPMutationLimitOnlyOnePartner";
            cbBPMutationLimitOnlyOnePartner.Size = new System.Drawing.Size(231, 19);
            cbBPMutationLimitOnlyOnePartner.TabIndex = 8;
            cbBPMutationLimitOnlyOnePartner.Text = "One partner may have more mutations";
            cbBPMutationLimitOnlyOnePartner.UseVisualStyleBackColor = true;
            cbBPMutationLimitOnlyOnePartner.CheckedChanged += cbMutationLimitOnlyOnePartner_CheckedChanged;
            // 
            // cbBPOnlyOneSuggestionForFemales
            // 
            cbBPOnlyOneSuggestionForFemales.AutoSize = true;
            cbBPOnlyOneSuggestionForFemales.Location = new System.Drawing.Point(7, 255);
            cbBPOnlyOneSuggestionForFemales.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBPOnlyOneSuggestionForFemales.Name = "cbBPOnlyOneSuggestionForFemales";
            cbBPOnlyOneSuggestionForFemales.Size = new System.Drawing.Size(199, 19);
            cbBPOnlyOneSuggestionForFemales.TabIndex = 7;
            cbBPOnlyOneSuggestionForFemales.Text = "Only best suggestion for females";
            cbBPOnlyOneSuggestionForFemales.UseVisualStyleBackColor = true;
            cbBPOnlyOneSuggestionForFemales.CheckedChanged += cbOnlyOneSuggestionForFemales_CheckedChanged;
            // 
            // cbBPIncludeCryoCreatures
            // 
            cbBPIncludeCryoCreatures.AutoSize = true;
            cbBPIncludeCryoCreatures.Location = new System.Drawing.Point(7, 128);
            cbBPIncludeCryoCreatures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBPIncludeCryoCreatures.Name = "cbBPIncludeCryoCreatures";
            cbBPIncludeCryoCreatures.Size = new System.Drawing.Size(185, 19);
            cbBPIncludeCryoCreatures.TabIndex = 6;
            cbBPIncludeCryoCreatures.Text = "Include Creatures in Cryopods";
            cbBPIncludeCryoCreatures.UseVisualStyleBackColor = true;
            cbBPIncludeCryoCreatures.CheckedChanged += cbBPIncludeCryoCreatures_CheckedChanged;
            // 
            // nudBPMutationLimit
            // 
            nudBPMutationLimit.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudBPMutationLimit.Location = new System.Drawing.Point(189, 155);
            nudBPMutationLimit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nudBPMutationLimit.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudBPMutationLimit.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            nudBPMutationLimit.Name = "nudBPMutationLimit";
            nudBPMutationLimit.Size = new System.Drawing.Size(58, 23);
            nudBPMutationLimit.TabIndex = 4;
            nudBPMutationLimit.ValueChanged += nudMutationLimit_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 157);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(171, 15);
            label2.TabIndex = 5;
            label2.Text = "Creatures with Mutations up to";
            // 
            // cbBPIncludeCooldowneds
            // 
            cbBPIncludeCooldowneds.AutoSize = true;
            cbBPIncludeCooldowneds.Location = new System.Drawing.Point(7, 102);
            cbBPIncludeCooldowneds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBPIncludeCooldowneds.Name = "cbBPIncludeCooldowneds";
            cbBPIncludeCooldowneds.Size = new System.Drawing.Size(202, 19);
            cbBPIncludeCooldowneds.TabIndex = 3;
            cbBPIncludeCooldowneds.Text = "Include Creatures with Cooldown";
            cbBPIncludeCooldowneds.UseVisualStyleBackColor = true;
            cbBPIncludeCooldowneds.CheckedChanged += checkBoxIncludeCooldowneds_CheckedChanged;
            // 
            // rbBPTopStatsCn
            // 
            rbBPTopStatsCn.AutoSize = true;
            rbBPTopStatsCn.Checked = true;
            rbBPTopStatsCn.Location = new System.Drawing.Point(7, 22);
            rbBPTopStatsCn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbBPTopStatsCn.Name = "rbBPTopStatsCn";
            rbBPTopStatsCn.Size = new System.Drawing.Size(125, 19);
            rbBPTopStatsCn.TabIndex = 2;
            rbBPTopStatsCn.TabStop = true;
            rbBPTopStatsCn.Text = "Combine Top Stats";
            rbBPTopStatsCn.UseVisualStyleBackColor = true;
            rbBPTopStatsCn.CheckedChanged += radioButtonBPTopStatsCn_CheckedChanged;
            // 
            // rbBPHighStats
            // 
            rbBPHighStats.AutoSize = true;
            rbBPHighStats.Location = new System.Drawing.Point(7, 75);
            rbBPHighStats.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbBPHighStats.Name = "rbBPHighStats";
            rbBPHighStats.Size = new System.Drawing.Size(135, 19);
            rbBPHighStats.TabIndex = 1;
            rbBPHighStats.Text = "Best Next Generation";
            rbBPHighStats.UseVisualStyleBackColor = true;
            rbBPHighStats.CheckedChanged += radioButtonBPHighStats_CheckedChanged;
            // 
            // rbBPTopStats
            // 
            rbBPTopStats.AutoSize = true;
            rbBPTopStats.Location = new System.Drawing.Point(7, 48);
            rbBPTopStats.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rbBPTopStats.Name = "rbBPTopStats";
            rbBPTopStats.Size = new System.Drawing.Size(88, 19);
            rbBPTopStats.TabIndex = 0;
            rbBPTopStats.Text = "Top Stats Lc";
            rbBPTopStats.UseVisualStyleBackColor = true;
            rbBPTopStats.CheckedChanged += radioButtonBPTopStats_CheckedChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageBreedableSpecies);
            tabControl1.Controls.Add(tabPageTags);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(4, 3);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl1.MinimumSize = new System.Drawing.Size(0, 231);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(277, 532);
            tabControl1.TabIndex = 8;
            // 
            // tabPageBreedableSpecies
            // 
            tabPageBreedableSpecies.Controls.Add(listViewSpeciesBP);
            tabPageBreedableSpecies.Location = new System.Drawing.Point(4, 24);
            tabPageBreedableSpecies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageBreedableSpecies.Name = "tabPageBreedableSpecies";
            tabPageBreedableSpecies.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageBreedableSpecies.Size = new System.Drawing.Size(269, 504);
            tabPageBreedableSpecies.TabIndex = 0;
            tabPageBreedableSpecies.Text = "Breedable Species";
            // 
            // listViewSpeciesBP
            // 
            listViewSpeciesBP.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader5 });
            listViewSpeciesBP.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewSpeciesBP.FullRowSelect = true;
            listViewSpeciesBP.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewSpeciesBP.Location = new System.Drawing.Point(4, 3);
            listViewSpeciesBP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listViewSpeciesBP.MultiSelect = false;
            listViewSpeciesBP.Name = "listViewSpeciesBP";
            listViewSpeciesBP.Size = new System.Drawing.Size(261, 498);
            listViewSpeciesBP.TabIndex = 3;
            listViewSpeciesBP.UseCompatibleStateImageBehavior = false;
            listViewSpeciesBP.View = System.Windows.Forms.View.Details;
            listViewSpeciesBP.SelectedIndexChanged += listViewSpeciesBP_SelectedIndexChanged;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Species";
            columnHeader5.Width = 240;
            // 
            // tabPageTags
            // 
            tabPageTags.Controls.Add(tableLayoutPanel3);
            tabPageTags.Location = new System.Drawing.Point(4, 24);
            tabPageTags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageTags.Name = "tabPageTags";
            tabPageTags.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPageTags.Size = new System.Drawing.Size(269, 504);
            tabPageTags.TabIndex = 1;
            tabPageTags.Text = "Filters / Tags";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(cbTribeFilterLibrary, 0, 1);
            tableLayoutPanel3.Controls.Add(cbOwnerFilterLibrary, 0, 0);
            tableLayoutPanel3.Controls.Add(tagSelectorList1, 0, 5);
            tableLayoutPanel3.Controls.Add(cbBPTagExcludeDefault, 0, 4);
            tableLayoutPanel3.Controls.Add(cbServerFilterLibrary, 0, 2);
            tableLayoutPanel3.Controls.Add(label1, 0, 3);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(4, 3);
            tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 6;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new System.Drawing.Size(261, 498);
            tableLayoutPanel3.TabIndex = 7;
            // 
            // cbTribeFilterLibrary
            // 
            cbTribeFilterLibrary.AutoSize = true;
            cbTribeFilterLibrary.Location = new System.Drawing.Point(4, 28);
            cbTribeFilterLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbTribeFilterLibrary.Name = "cbTribeFilterLibrary";
            cbTribeFilterLibrary.Size = new System.Drawing.Size(147, 19);
            cbTribeFilterLibrary.TabIndex = 7;
            cbTribeFilterLibrary.Text = "Tribe filter from Library";
            cbTribeFilterLibrary.UseVisualStyleBackColor = true;
            cbTribeFilterLibrary.CheckedChanged += cbTribeFilterLibrary_CheckedChanged;
            // 
            // cbOwnerFilterLibrary
            // 
            cbOwnerFilterLibrary.AutoSize = true;
            cbOwnerFilterLibrary.Location = new System.Drawing.Point(4, 3);
            cbOwnerFilterLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbOwnerFilterLibrary.Name = "cbOwnerFilterLibrary";
            cbOwnerFilterLibrary.Size = new System.Drawing.Size(156, 19);
            cbOwnerFilterLibrary.TabIndex = 6;
            cbOwnerFilterLibrary.Text = "Owner filter from Library";
            cbOwnerFilterLibrary.UseVisualStyleBackColor = true;
            cbOwnerFilterLibrary.CheckedChanged += cbOwnerFilterLibrary_CheckedChanged;
            // 
            // tagSelectorList1
            // 
            tagSelectorList1.AutoScroll = true;
            tagSelectorList1.Dock = System.Windows.Forms.DockStyle.Fill;
            tagSelectorList1.Location = new System.Drawing.Point(7, 187);
            tagSelectorList1.Margin = new System.Windows.Forms.Padding(7);
            tagSelectorList1.Name = "tagSelectorList1";
            tagSelectorList1.Size = new System.Drawing.Size(247, 304);
            tagSelectorList1.TabIndex = 3;
            // 
            // cbBPTagExcludeDefault
            // 
            cbBPTagExcludeDefault.AutoSize = true;
            cbBPTagExcludeDefault.Location = new System.Drawing.Point(4, 158);
            cbBPTagExcludeDefault.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbBPTagExcludeDefault.Name = "cbBPTagExcludeDefault";
            cbBPTagExcludeDefault.Size = new System.Drawing.Size(173, 19);
            cbBPTagExcludeDefault.TabIndex = 4;
            cbBPTagExcludeDefault.Text = "Exclude creatures by default";
            cbBPTagExcludeDefault.UseVisualStyleBackColor = true;
            cbBPTagExcludeDefault.CheckedChanged += cbTagExcludeDefault_CheckedChanged;
            // 
            // cbServerFilterLibrary
            // 
            cbServerFilterLibrary.AutoSize = true;
            cbServerFilterLibrary.Location = new System.Drawing.Point(4, 53);
            cbServerFilterLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbServerFilterLibrary.Name = "cbServerFilterLibrary";
            cbServerFilterLibrary.Size = new System.Drawing.Size(153, 19);
            cbServerFilterLibrary.TabIndex = 5;
            cbServerFilterLibrary.Text = "Server filter from Library";
            cbServerFilterLibrary.UseVisualStyleBackColor = true;
            cbServerFilterLibrary.CheckedChanged += cbServerFilterLibrary_CheckedChanged;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(4, 75);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(203, 80);
            label1.TabIndex = 2;
            label1.Text = "Consider creatures by tag. \r\n✕ excludes creatures, ✓ includes creatures (even if they have an exclusive tag). Add tags in the library with F3.";
            // 
            // statWeighting1
            // 
            statWeighting1.Dock = System.Windows.Forms.DockStyle.Fill;
            statWeighting1.Location = new System.Drawing.Point(7, 875);
            statWeighting1.Margin = new System.Windows.Forms.Padding(7);
            statWeighting1.Name = "statWeighting1";
            statWeighting1.Size = new System.Drawing.Size(271, 292);
            statWeighting1.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 0);
            tableLayoutPanel1.Controls.Add(gbBPOffspring, 0, 2);
            tableLayoutPanel1.Controls.Add(panelCombinations, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(297, 3);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(1720, 1174);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(lbBreedingPlanHeader);
            flowLayoutPanel1.Controls.Add(pedigreeCreatureBestPossibleInSpecies);
            flowLayoutPanel1.Controls.Add(btShowAllCreatures);
            flowLayoutPanel1.Controls.Add(BtRecalculatePlan);
            flowLayoutPanel1.Controls.Add(panel1);
            flowLayoutPanel1.Controls.Add(pedigreeCreatureBestPossibleInSpeciesFiltered);
            flowLayoutPanel1.Controls.Add(pedigreeCreature1);
            flowLayoutPanel1.Controls.Add(lbBPBreedingScore);
            flowLayoutPanel1.Controls.Add(pedigreeCreature2);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(4, 3);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(1712, 205);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // lbBreedingPlanHeader
            // 
            lbBreedingPlanHeader.AutoSize = true;
            flowLayoutPanel1.SetFlowBreak(lbBreedingPlanHeader, true);
            lbBreedingPlanHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lbBreedingPlanHeader.Location = new System.Drawing.Point(4, 0);
            lbBreedingPlanHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbBreedingPlanHeader.MinimumSize = new System.Drawing.Size(817, 0);
            lbBreedingPlanHeader.Name = "lbBreedingPlanHeader";
            lbBreedingPlanHeader.Size = new System.Drawing.Size(817, 21);
            lbBreedingPlanHeader.TabIndex = 1;
            lbBreedingPlanHeader.Text = "Select a species and click on \"Determine Best Breeding\" to see suggestions";
            lbBreedingPlanHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pedigreeCreatureBestPossibleInSpecies
            // 
            pedigreeCreatureBestPossibleInSpecies.Location = new System.Drawing.Point(7, 61);
            pedigreeCreatureBestPossibleInSpecies.Margin = new System.Windows.Forms.Padding(7);
            pedigreeCreatureBestPossibleInSpecies.Name = "pedigreeCreatureBestPossibleInSpecies";
            pedigreeCreatureBestPossibleInSpecies.Size = new System.Drawing.Size(379, 40);
            pedigreeCreatureBestPossibleInSpecies.TabIndex = 5;
            // 
            // btShowAllCreatures
            // 
            btShowAllCreatures.Location = new System.Drawing.Point(504, 57);
            btShowAllCreatures.Margin = new System.Windows.Forms.Padding(111, 3, 4, 3);
            btShowAllCreatures.Name = "btShowAllCreatures";
            btShowAllCreatures.Size = new System.Drawing.Size(346, 40);
            btShowAllCreatures.TabIndex = 6;
            btShowAllCreatures.Text = "Unset restriction to …";
            btShowAllCreatures.UseVisualStyleBackColor = true;
            btShowAllCreatures.Click += btShowAllCreatures_Click;
            // 
            // BtRecalculatePlan
            // 
            BtRecalculatePlan.Location = new System.Drawing.Point(858, 57);
            BtRecalculatePlan.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtRecalculatePlan.Name = "BtRecalculatePlan";
            BtRecalculatePlan.Size = new System.Drawing.Size(208, 40);
            BtRecalculatePlan.TabIndex = 9;
            BtRecalculatePlan.Text = "Library changed, recalculate plan";
            BtRecalculatePlan.UseVisualStyleBackColor = true;
            BtRecalculatePlan.Click += BtRecalculatePlan_Click;
            // 
            // panel1
            // 
            flowLayoutPanel1.SetFlowBreak(panel1, true);
            panel1.Location = new System.Drawing.Point(1074, 57);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(12, 37);
            panel1.TabIndex = 7;
            // 
            // pedigreeCreatureBestPossibleInSpeciesFiltered
            // 
            flowLayoutPanel1.SetFlowBreak(pedigreeCreatureBestPossibleInSpeciesFiltered, true);
            pedigreeCreatureBestPossibleInSpeciesFiltered.Location = new System.Drawing.Point(7, 115);
            pedigreeCreatureBestPossibleInSpeciesFiltered.Margin = new System.Windows.Forms.Padding(7);
            pedigreeCreatureBestPossibleInSpeciesFiltered.Name = "pedigreeCreatureBestPossibleInSpeciesFiltered";
            pedigreeCreatureBestPossibleInSpeciesFiltered.Size = new System.Drawing.Size(379, 40);
            pedigreeCreatureBestPossibleInSpeciesFiltered.TabIndex = 8;
            // 
            // pedigreeCreature1
            // 
            pedigreeCreature1.Location = new System.Drawing.Point(4, 162);
            pedigreeCreature1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 3);
            pedigreeCreature1.Name = "pedigreeCreature1";
            pedigreeCreature1.Size = new System.Drawing.Size(379, 40);
            pedigreeCreature1.TabIndex = 2;
            // 
            // lbBPBreedingScore
            // 
            lbBPBreedingScore.Location = new System.Drawing.Point(391, 179);
            lbBPBreedingScore.Margin = new System.Windows.Forms.Padding(4, 17, 4, 0);
            lbBPBreedingScore.Name = "lbBPBreedingScore";
            lbBPBreedingScore.Size = new System.Drawing.Size(92, 23);
            lbBPBreedingScore.TabIndex = 4;
            lbBPBreedingScore.Text = "Breeding-Score";
            // 
            // pedigreeCreature2
            // 
            pedigreeCreature2.Location = new System.Drawing.Point(491, 162);
            pedigreeCreature2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 3);
            pedigreeCreature2.Name = "pedigreeCreature2";
            pedigreeCreature2.Size = new System.Drawing.Size(379, 40);
            pedigreeCreature2.TabIndex = 3;
            // 
            // gbBPOffspring
            // 
            gbBPOffspring.Controls.Add(tableLayoutPanel2);
            gbBPOffspring.Dock = System.Windows.Forms.DockStyle.Fill;
            gbBPOffspring.Location = new System.Drawing.Point(4, 933);
            gbBPOffspring.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbBPOffspring.Name = "gbBPOffspring";
            gbBPOffspring.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gbBPOffspring.Size = new System.Drawing.Size(1712, 238);
            gbBPOffspring.TabIndex = 2;
            gbBPOffspring.TabStop = false;
            gbBPOffspring.Text = "Offspring";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.Controls.Add(tableLayoutPanel4, 2, 0);
            tableLayoutPanel2.Controls.Add(flowLayoutPanel2, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel6, 1, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(4, 19);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(1704, 216);
            tableLayoutPanel2.TabIndex = 9;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(labelBreedingInfos, 0, 2);
            tableLayoutPanel4.Controls.Add(listViewRaisingTimes, 0, 1);
            tableLayoutPanel4.Controls.Add(lbBPBreedingTimes, 0, 0);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(733, 3);
            tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 3;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel4.Size = new System.Drawing.Size(988, 210);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // labelBreedingInfos
            // 
            labelBreedingInfos.AutoSize = true;
            labelBreedingInfos.Location = new System.Drawing.Point(4, 137);
            labelBreedingInfos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelBreedingInfos.Name = "labelBreedingInfos";
            labelBreedingInfos.Size = new System.Drawing.Size(83, 15);
            labelBreedingInfos.TabIndex = 7;
            labelBreedingInfos.Text = "Breeding Infos";
            // 
            // listViewRaisingTimes
            // 
            listViewRaisingTimes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            listViewRaisingTimes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listViewRaisingTimes.Location = new System.Drawing.Point(4, 22);
            listViewRaisingTimes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listViewRaisingTimes.Name = "listViewRaisingTimes";
            listViewRaisingTimes.ShowGroups = false;
            listViewRaisingTimes.Size = new System.Drawing.Size(409, 112);
            listViewRaisingTimes.TabIndex = 4;
            listViewRaisingTimes.UseCompatibleStateImageBehavior = false;
            listViewRaisingTimes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "";
            columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Time";
            columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Total Time";
            columnHeader3.Width = 70;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Finished at";
            columnHeader4.Width = 108;
            // 
            // lbBPBreedingTimes
            // 
            lbBPBreedingTimes.AutoSize = true;
            lbBPBreedingTimes.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lbBPBreedingTimes.Location = new System.Drawing.Point(4, 0);
            lbBPBreedingTimes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbBPBreedingTimes.Name = "lbBPBreedingTimes";
            lbBPBreedingTimes.Size = new System.Drawing.Size(113, 19);
            lbBPBreedingTimes.TabIndex = 3;
            lbBPBreedingTimes.Text = "Breeding Times";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(lbBPProbabilityBest);
            flowLayoutPanel2.Controls.Add(pedigreeCreatureBest);
            flowLayoutPanel2.Controls.Add(pedigreeCreatureWorst);
            flowLayoutPanel2.Controls.Add(lbMutationProbability);
            flowLayoutPanel2.Controls.Add(btBPJustMated);
            flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel2.Location = new System.Drawing.Point(4, 3);
            flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(411, 210);
            flowLayoutPanel2.TabIndex = 8;
            // 
            // lbBPProbabilityBest
            // 
            lbBPProbabilityBest.AutoSize = true;
            flowLayoutPanel2.SetFlowBreak(lbBPProbabilityBest, true);
            lbBPProbabilityBest.Location = new System.Drawing.Point(4, 0);
            lbBPProbabilityBest.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbBPProbabilityBest.Name = "lbBPProbabilityBest";
            lbBPProbabilityBest.Size = new System.Drawing.Size(229, 15);
            lbBPProbabilityBest.TabIndex = 6;
            lbBPProbabilityBest.Text = "Probability for this Best Possible outcome:";
            // 
            // pedigreeCreatureBest
            // 
            pedigreeCreatureBest.Cursor = System.Windows.Forms.Cursors.Hand;
            flowLayoutPanel2.SetFlowBreak(pedigreeCreatureBest, true);
            pedigreeCreatureBest.Location = new System.Drawing.Point(7, 22);
            pedigreeCreatureBest.Margin = new System.Windows.Forms.Padding(7);
            pedigreeCreatureBest.Name = "pedigreeCreatureBest";
            pedigreeCreatureBest.Size = new System.Drawing.Size(379, 55);
            pedigreeCreatureBest.TabIndex = 1;
            // 
            // pedigreeCreatureWorst
            // 
            pedigreeCreatureWorst.Cursor = System.Windows.Forms.Cursors.Hand;
            flowLayoutPanel2.SetFlowBreak(pedigreeCreatureWorst, true);
            pedigreeCreatureWorst.Location = new System.Drawing.Point(7, 91);
            pedigreeCreatureWorst.Margin = new System.Windows.Forms.Padding(7);
            pedigreeCreatureWorst.Name = "pedigreeCreatureWorst";
            pedigreeCreatureWorst.Size = new System.Drawing.Size(379, 55);
            pedigreeCreatureWorst.TabIndex = 2;
            // 
            // lbMutationProbability
            // 
            lbMutationProbability.AutoSize = true;
            flowLayoutPanel2.SetFlowBreak(lbMutationProbability, true);
            lbMutationProbability.Location = new System.Drawing.Point(4, 153);
            lbMutationProbability.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbMutationProbability.Name = "lbMutationProbability";
            lbMutationProbability.Size = new System.Drawing.Size(135, 15);
            lbMutationProbability.TabIndex = 7;
            lbMutationProbability.Text = "Probability of mutations";
            // 
            // btBPJustMated
            // 
            btBPJustMated.Location = new System.Drawing.Point(4, 171);
            btBPJustMated.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btBPJustMated.Name = "btBPJustMated";
            btBPJustMated.Size = new System.Drawing.Size(379, 33);
            btBPJustMated.TabIndex = 0;
            btBPJustMated.Text = "These Parents just mated";
            btBPJustMated.UseVisualStyleBackColor = true;
            btBPJustMated.Click += buttonJustMated_Click;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.AutoSize = true;
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel6.Controls.Add(offspringPossibilities1, 0, 0);
            tableLayoutPanel6.Controls.Add(LbMinTotalLevelTopStats, 0, 1);
            tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel6.Location = new System.Drawing.Point(423, 3);
            tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel6.Size = new System.Drawing.Size(302, 210);
            tableLayoutPanel6.TabIndex = 8;
            // 
            // offspringPossibilities1
            // 
            offspringPossibilities1.Location = new System.Drawing.Point(7, 7);
            offspringPossibilities1.Margin = new System.Windows.Forms.Padding(7);
            offspringPossibilities1.Name = "offspringPossibilities1";
            offspringPossibilities1.Size = new System.Drawing.Size(288, 155);
            offspringPossibilities1.TabIndex = 1;
            // 
            // LbMinTotalLevelTopStats
            // 
            LbMinTotalLevelTopStats.AutoSize = true;
            LbMinTotalLevelTopStats.Location = new System.Drawing.Point(4, 169);
            LbMinTotalLevelTopStats.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbMinTotalLevelTopStats.Name = "LbMinTotalLevelTopStats";
            LbMinTotalLevelTopStats.Size = new System.Drawing.Size(0, 15);
            LbMinTotalLevelTopStats.TabIndex = 2;
            // 
            // panelCombinations
            // 
            panelCombinations.Controls.Add(lbBreedingPlanInfo);
            panelCombinations.Controls.Add(flowLayoutPanelPairs);
            panelCombinations.Dock = System.Windows.Forms.DockStyle.Fill;
            panelCombinations.Location = new System.Drawing.Point(4, 214);
            panelCombinations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelCombinations.Name = "panelCombinations";
            panelCombinations.Size = new System.Drawing.Size(1712, 713);
            panelCombinations.TabIndex = 3;
            // 
            // lbBreedingPlanInfo
            // 
            lbBreedingPlanInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lbBreedingPlanInfo.Location = new System.Drawing.Point(12, 87);
            lbBreedingPlanInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbBreedingPlanInfo.Name = "lbBreedingPlanInfo";
            lbBreedingPlanInfo.Size = new System.Drawing.Size(797, 223);
            lbBreedingPlanInfo.TabIndex = 0;
            lbBreedingPlanInfo.Text = "Infotext";
            lbBreedingPlanInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbBreedingPlanInfo.Visible = false;
            // 
            // flowLayoutPanelPairs
            // 
            flowLayoutPanelPairs.AutoScroll = true;
            flowLayoutPanelPairs.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanelPairs.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanelPairs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanelPairs.Name = "flowLayoutPanelPairs";
            flowLayoutPanelPairs.Size = new System.Drawing.Size(1712, 713);
            flowLayoutPanelPairs.TabIndex = 1;
            // 
            // BreedingPlan
            // 
            AutoScroll = true;
            Controls.Add(tableLayoutMain);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "BreedingPlan";
            Size = new System.Drawing.Size(2021, 1180);
            tableLayoutMain.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            gbBPBreedingMode.ResumeLayout(false);
            gbBPBreedingMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudBPMutationLimit).EndInit();
            tabControl1.ResumeLayout(false);
            tabPageBreedableSpecies.ResumeLayout(false);
            tabPageTags.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            gbBPOffspring.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            panelCombinations.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.ListView listViewSpeciesBP;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private GroupBoxC gbBPBreedingMode;
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
        private GroupBoxC gbBPOffspring;
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
        private System.Windows.Forms.CheckBox CbOnlySameSpecies;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label LbMinTotalLevelTopStats;
        private System.Windows.Forms.Button BtRecalculatePlan;
    }
}

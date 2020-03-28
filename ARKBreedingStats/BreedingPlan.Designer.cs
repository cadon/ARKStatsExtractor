﻿using ARKBreedingStats.uiControls;

namespace ARKBreedingStats
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageBreedableSpecies = new System.Windows.Forms.TabPage();
            this.listViewSpeciesBP = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageTags = new System.Windows.Forms.TabPage();
            this.cbOwnerFilterLibrary = new System.Windows.Forms.CheckBox();
            this.cbServerFilterLibrary = new System.Windows.Forms.CheckBox();
            this.cbBPTagExcludeDefault = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbBreedingPlanHeader = new System.Windows.Forms.Label();
            this.btShowAllCreatures = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbBPBreedingScore = new System.Windows.Forms.Label();
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
            this.btBPJustMated = new System.Windows.Forms.Button();
            this.panelCombinations = new System.Windows.Forms.Panel();
            this.lbBreedingPlanInfo = new System.Windows.Forms.Label();
            this.flowLayoutPanelPairs = new System.Windows.Forms.FlowLayoutPanel();
            this.gbBPBreedingMode = new System.Windows.Forms.GroupBox();
            this.cbBPOnlyOneSuggestionForFemales = new System.Windows.Forms.CheckBox();
            this.cbBPIncludeCryoCreatures = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbBPIncludeCooldowneds = new System.Windows.Forms.CheckBox();
            this.rbBPTopStatsCn = new System.Windows.Forms.RadioButton();
            this.rbBPHighStats = new System.Windows.Forms.RadioButton();
            this.rbBPTopStats = new System.Windows.Forms.RadioButton();
            this.cbBPMutationLimitOnlyOnePartner = new System.Windows.Forms.CheckBox();
            this.tagSelectorList1 = new ARKBreedingStats.uiControls.TagSelectorList();
            this.pedigreeCreatureBestPossibleInSpecies = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreature1 = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreature2 = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreatureBest = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreatureWorst = new ARKBreedingStats.PedigreeCreature();
            this.offspringPossibilities1 = new ARKBreedingStats.OffspringPossibilities();
            this.nudBPMutationLimit = new ARKBreedingStats.uiControls.Nud();
            this.statWeighting1 = new ARKBreedingStats.uiControls.StatWeighting();
            this.tableLayoutMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageBreedableSpecies.SuspendLayout();
            this.tabPageTags.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.gbBPOffspring.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panelCombinations.SuspendLayout();
            this.gbBPBreedingMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBPMutationLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutMain.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutMain.Controls.Add(this.gbBPBreedingMode, 0, 1);
            this.tableLayoutMain.Controls.Add(this.statWeighting1, 0, 2);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 3;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 215F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.tableLayoutMain.Size = new System.Drawing.Size(1732, 1023);
            this.tableLayoutMain.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageBreedableSpecies);
            this.tabControl1.Controls.Add(this.tabPageTags);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(244, 542);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPageBreedableSpecies
            // 
            this.tabPageBreedableSpecies.Controls.Add(this.listViewSpeciesBP);
            this.tabPageBreedableSpecies.Location = new System.Drawing.Point(4, 22);
            this.tabPageBreedableSpecies.Name = "tabPageBreedableSpecies";
            this.tabPageBreedableSpecies.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBreedableSpecies.Size = new System.Drawing.Size(236, 516);
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
            this.listViewSpeciesBP.Location = new System.Drawing.Point(3, 3);
            this.listViewSpeciesBP.MultiSelect = false;
            this.listViewSpeciesBP.Name = "listViewSpeciesBP";
            this.listViewSpeciesBP.Size = new System.Drawing.Size(230, 510);
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
            this.tabPageTags.Controls.Add(this.cbOwnerFilterLibrary);
            this.tabPageTags.Controls.Add(this.cbServerFilterLibrary);
            this.tabPageTags.Controls.Add(this.cbBPTagExcludeDefault);
            this.tabPageTags.Controls.Add(this.label1);
            this.tabPageTags.Controls.Add(this.tagSelectorList1);
            this.tabPageTags.Location = new System.Drawing.Point(4, 22);
            this.tabPageTags.Name = "tabPageTags";
            this.tabPageTags.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTags.Size = new System.Drawing.Size(236, 516);
            this.tabPageTags.TabIndex = 1;
            this.tabPageTags.Text = "Tags";
            this.tabPageTags.UseVisualStyleBackColor = true;
            // 
            // cbOwnerFilterLibrary
            // 
            this.cbOwnerFilterLibrary.AutoSize = true;
            this.cbOwnerFilterLibrary.Location = new System.Drawing.Point(3, 29);
            this.cbOwnerFilterLibrary.Name = "cbOwnerFilterLibrary";
            this.cbOwnerFilterLibrary.Size = new System.Drawing.Size(139, 17);
            this.cbOwnerFilterLibrary.TabIndex = 6;
            this.cbOwnerFilterLibrary.Text = "Owner Filter from Library";
            this.cbOwnerFilterLibrary.UseVisualStyleBackColor = true;
            this.cbOwnerFilterLibrary.CheckedChanged += new System.EventHandler(this.cbOwnerFilterLibrary_CheckedChanged);
            // 
            // cbServerFilterLibrary
            // 
            this.cbServerFilterLibrary.AutoSize = true;
            this.cbServerFilterLibrary.Location = new System.Drawing.Point(3, 6);
            this.cbServerFilterLibrary.Name = "cbServerFilterLibrary";
            this.cbServerFilterLibrary.Size = new System.Drawing.Size(139, 17);
            this.cbServerFilterLibrary.TabIndex = 5;
            this.cbServerFilterLibrary.Text = "Server Filter from Library";
            this.cbServerFilterLibrary.UseVisualStyleBackColor = true;
            this.cbServerFilterLibrary.CheckedChanged += new System.EventHandler(this.cbServerFilterLibrary_CheckedChanged);
            // 
            // cbBPTagExcludeDefault
            // 
            this.cbBPTagExcludeDefault.AutoSize = true;
            this.cbBPTagExcludeDefault.Location = new System.Drawing.Point(6, 121);
            this.cbBPTagExcludeDefault.Name = "cbBPTagExcludeDefault";
            this.cbBPTagExcludeDefault.Size = new System.Drawing.Size(160, 17);
            this.cbBPTagExcludeDefault.TabIndex = 4;
            this.cbBPTagExcludeDefault.Text = "Exclude creatures by default";
            this.cbBPTagExcludeDefault.UseVisualStyleBackColor = true;
            this.cbBPTagExcludeDefault.CheckedChanged += new System.EventHandler(this.cbTagExcludeDefault_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 69);
            this.label1.TabIndex = 2;
            this.label1.Text = "Consider creatures by tag. \r\n✕ excludes creatures, ✓ includes creatures (even if " +
    "they have an exclusive tag). Add tags in the library with F3.";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gbBPOffspring, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panelCombinations, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(253, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutMain.SetRowSpan(this.tableLayoutPanel1, 3);
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 182F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1476, 1017);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lbBreedingPlanHeader);
            this.flowLayoutPanel1.Controls.Add(this.pedigreeCreatureBestPossibleInSpecies);
            this.flowLayoutPanel1.Controls.Add(this.btShowAllCreatures);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.pedigreeCreature1);
            this.flowLayoutPanel1.Controls.Add(this.lbBPBreedingScore);
            this.flowLayoutPanel1.Controls.Add(this.pedigreeCreature2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1470, 122);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // lbBreedingPlanHeader
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.lbBreedingPlanHeader, true);
            this.lbBreedingPlanHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBreedingPlanHeader.Location = new System.Drawing.Point(3, 0);
            this.lbBreedingPlanHeader.Name = "lbBreedingPlanHeader";
            this.lbBreedingPlanHeader.Size = new System.Drawing.Size(599, 27);
            this.lbBreedingPlanHeader.TabIndex = 1;
            this.lbBreedingPlanHeader.Text = "Select a species and click on \"Determine Best Breeding\" to see suggestions";
            this.lbBreedingPlanHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btShowAllCreatures
            // 
            this.btShowAllCreatures.Location = new System.Drawing.Point(426, 44);
            this.btShowAllCreatures.Margin = new System.Windows.Forms.Padding(95, 3, 3, 3);
            this.btShowAllCreatures.Name = "btShowAllCreatures";
            this.btShowAllCreatures.Size = new System.Drawing.Size(297, 35);
            this.btShowAllCreatures.TabIndex = 6;
            this.btShowAllCreatures.Text = "Unset restriction to …";
            this.btShowAllCreatures.UseVisualStyleBackColor = true;
            this.btShowAllCreatures.Click += new System.EventHandler(this.btShowAllCreatures_Click);
            // 
            // panel1
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.panel1, true);
            this.panel1.Location = new System.Drawing.Point(729, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 32);
            this.panel1.TabIndex = 7;
            // 
            // lbBPBreedingScore
            // 
            this.lbBPBreedingScore.Location = new System.Drawing.Point(334, 97);
            this.lbBPBreedingScore.Margin = new System.Windows.Forms.Padding(3, 15, 3, 0);
            this.lbBPBreedingScore.Name = "lbBPBreedingScore";
            this.lbBPBreedingScore.Size = new System.Drawing.Size(87, 20);
            this.lbBPBreedingScore.TabIndex = 4;
            this.lbBPBreedingScore.Text = "Breeding-Score";
            // 
            // gbBPOffspring
            // 
            this.gbBPOffspring.Controls.Add(this.tableLayoutPanel2);
            this.gbBPOffspring.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbBPOffspring.Location = new System.Drawing.Point(3, 838);
            this.gbBPOffspring.Name = "gbBPOffspring";
            this.gbBPOffspring.Size = new System.Drawing.Size(1470, 176);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1464, 157);
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
            this.tableLayoutPanel4.Location = new System.Drawing.Point(614, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(847, 151);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // labelBreedingInfos
            // 
            this.labelBreedingInfos.Location = new System.Drawing.Point(3, 109);
            this.labelBreedingInfos.Name = "labelBreedingInfos";
            this.labelBreedingInfos.Size = new System.Drawing.Size(322, 48);
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
            this.listViewRaisingTimes.Location = new System.Drawing.Point(3, 20);
            this.listViewRaisingTimes.Name = "listViewRaisingTimes";
            this.listViewRaisingTimes.ShowGroups = false;
            this.listViewRaisingTimes.Size = new System.Drawing.Size(317, 86);
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
            this.lbBPBreedingTimes.Location = new System.Drawing.Point(3, 0);
            this.lbBPBreedingTimes.Name = "lbBPBreedingTimes";
            this.lbBPBreedingTimes.Size = new System.Drawing.Size(121, 17);
            this.lbBPBreedingTimes.TabIndex = 3;
            this.lbBPBreedingTimes.Text = "Breeding Times";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.lbBPProbabilityBest);
            this.flowLayoutPanel2.Controls.Add(this.pedigreeCreatureBest);
            this.flowLayoutPanel2.Controls.Add(this.pedigreeCreatureWorst);
            this.flowLayoutPanel2.Controls.Add(this.btBPJustMated);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(352, 151);
            this.flowLayoutPanel2.TabIndex = 8;
            // 
            // lbBPProbabilityBest
            // 
            this.lbBPProbabilityBest.AutoSize = true;
            this.flowLayoutPanel2.SetFlowBreak(this.lbBPProbabilityBest, true);
            this.lbBPProbabilityBest.Location = new System.Drawing.Point(3, 0);
            this.lbBPProbabilityBest.Name = "lbBPProbabilityBest";
            this.lbBPProbabilityBest.Size = new System.Drawing.Size(202, 13);
            this.lbBPProbabilityBest.TabIndex = 6;
            this.lbBPProbabilityBest.Text = "Probability for this Best Possible outcome:";
            // 
            // btBPJustMated
            // 
            this.btBPJustMated.Location = new System.Drawing.Point(3, 98);
            this.btBPJustMated.Name = "btBPJustMated";
            this.btBPJustMated.Size = new System.Drawing.Size(325, 33);
            this.btBPJustMated.TabIndex = 0;
            this.btBPJustMated.Text = "These Parents just mated";
            this.btBPJustMated.UseVisualStyleBackColor = true;
            this.btBPJustMated.Click += new System.EventHandler(this.buttonJustMated_Click);
            // 
            // panelCombinations
            // 
            this.panelCombinations.Controls.Add(this.lbBreedingPlanInfo);
            this.panelCombinations.Controls.Add(this.flowLayoutPanelPairs);
            this.panelCombinations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCombinations.Location = new System.Drawing.Point(3, 131);
            this.panelCombinations.Name = "panelCombinations";
            this.panelCombinations.Size = new System.Drawing.Size(1470, 701);
            this.panelCombinations.TabIndex = 3;
            // 
            // lbBreedingPlanInfo
            // 
            this.lbBreedingPlanInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBreedingPlanInfo.Location = new System.Drawing.Point(10, 75);
            this.lbBreedingPlanInfo.Name = "lbBreedingPlanInfo";
            this.lbBreedingPlanInfo.Size = new System.Drawing.Size(683, 193);
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
            this.flowLayoutPanelPairs.Name = "flowLayoutPanelPairs";
            this.flowLayoutPanelPairs.Size = new System.Drawing.Size(1470, 701);
            this.flowLayoutPanelPairs.TabIndex = 1;
            // 
            // gbBPBreedingMode
            // 
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
            this.gbBPBreedingMode.Location = new System.Drawing.Point(3, 551);
            this.gbBPBreedingMode.Name = "gbBPBreedingMode";
            this.gbBPBreedingMode.Size = new System.Drawing.Size(244, 209);
            this.gbBPBreedingMode.TabIndex = 6;
            this.gbBPBreedingMode.TabStop = false;
            this.gbBPBreedingMode.Text = "Breeding-Mode";
            // 
            // cbBPOnlyOneSuggestionForFemales
            // 
            this.cbBPOnlyOneSuggestionForFemales.AutoSize = true;
            this.cbBPOnlyOneSuggestionForFemales.Location = new System.Drawing.Point(6, 183);
            this.cbBPOnlyOneSuggestionForFemales.Name = "cbBPOnlyOneSuggestionForFemales";
            this.cbBPOnlyOneSuggestionForFemales.Size = new System.Drawing.Size(178, 17);
            this.cbBPOnlyOneSuggestionForFemales.TabIndex = 7;
            this.cbBPOnlyOneSuggestionForFemales.Text = "Only best suggestion for females";
            this.cbBPOnlyOneSuggestionForFemales.UseVisualStyleBackColor = true;
            this.cbBPOnlyOneSuggestionForFemales.CheckedChanged += new System.EventHandler(this.cbOnlyOneSuggestionForFemales_CheckedChanged);
            // 
            // cbBPIncludeCryoCreatures
            // 
            this.cbBPIncludeCryoCreatures.AutoSize = true;
            this.cbBPIncludeCryoCreatures.Location = new System.Drawing.Point(6, 111);
            this.cbBPIncludeCryoCreatures.Name = "cbBPIncludeCryoCreatures";
            this.cbBPIncludeCryoCreatures.Size = new System.Drawing.Size(167, 17);
            this.cbBPIncludeCryoCreatures.TabIndex = 6;
            this.cbBPIncludeCryoCreatures.Text = "Include Creatures in Cryopods";
            this.cbBPIncludeCryoCreatures.UseVisualStyleBackColor = true;
            this.cbBPIncludeCryoCreatures.CheckedChanged += new System.EventHandler(this.cbBPIncludeCryoCreatures_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Creatures with Mutations up to";
            // 
            // cbBPIncludeCooldowneds
            // 
            this.cbBPIncludeCooldowneds.AutoSize = true;
            this.cbBPIncludeCooldowneds.Location = new System.Drawing.Point(6, 88);
            this.cbBPIncludeCooldowneds.Name = "cbBPIncludeCooldowneds";
            this.cbBPIncludeCooldowneds.Size = new System.Drawing.Size(181, 17);
            this.cbBPIncludeCooldowneds.TabIndex = 3;
            this.cbBPIncludeCooldowneds.Text = "Include Creatures with Cooldown";
            this.cbBPIncludeCooldowneds.UseVisualStyleBackColor = true;
            this.cbBPIncludeCooldowneds.CheckedChanged += new System.EventHandler(this.checkBoxIncludeCooldowneds_CheckedChanged);
            // 
            // rbBPTopStatsCn
            // 
            this.rbBPTopStatsCn.AutoSize = true;
            this.rbBPTopStatsCn.Checked = true;
            this.rbBPTopStatsCn.Location = new System.Drawing.Point(6, 19);
            this.rbBPTopStatsCn.Name = "rbBPTopStatsCn";
            this.rbBPTopStatsCn.Size = new System.Drawing.Size(115, 17);
            this.rbBPTopStatsCn.TabIndex = 2;
            this.rbBPTopStatsCn.TabStop = true;
            this.rbBPTopStatsCn.Text = "Combine Top Stats";
            this.rbBPTopStatsCn.UseVisualStyleBackColor = true;
            this.rbBPTopStatsCn.CheckedChanged += new System.EventHandler(this.radioButtonBPTopStatsCn_CheckedChanged);
            // 
            // rbBPHighStats
            // 
            this.rbBPHighStats.AutoSize = true;
            this.rbBPHighStats.Location = new System.Drawing.Point(6, 65);
            this.rbBPHighStats.Name = "rbBPHighStats";
            this.rbBPHighStats.Size = new System.Drawing.Size(126, 17);
            this.rbBPHighStats.TabIndex = 1;
            this.rbBPHighStats.Text = "Best Next Generation";
            this.rbBPHighStats.UseVisualStyleBackColor = true;
            this.rbBPHighStats.CheckedChanged += new System.EventHandler(this.radioButtonBPHighStats_CheckedChanged);
            // 
            // rbBPTopStats
            // 
            this.rbBPTopStats.AutoSize = true;
            this.rbBPTopStats.Location = new System.Drawing.Point(6, 42);
            this.rbBPTopStats.Name = "rbBPTopStats";
            this.rbBPTopStats.Size = new System.Drawing.Size(86, 17);
            this.rbBPTopStats.TabIndex = 0;
            this.rbBPTopStats.Text = "Top Stats Lc";
            this.rbBPTopStats.UseVisualStyleBackColor = true;
            this.rbBPTopStats.CheckedChanged += new System.EventHandler(this.radioButtonBPTopStats_CheckedChanged);
            // 
            // cbBPMutationLimitOnlyOnePartner
            // 
            this.cbBPMutationLimitOnlyOnePartner.AutoSize = true;
            this.cbBPMutationLimitOnlyOnePartner.Location = new System.Drawing.Point(29, 160);
            this.cbBPMutationLimitOnlyOnePartner.Name = "cbBPMutationLimitOnlyOnePartner";
            this.cbBPMutationLimitOnlyOnePartner.Size = new System.Drawing.Size(205, 17);
            this.cbBPMutationLimitOnlyOnePartner.TabIndex = 8;
            this.cbBPMutationLimitOnlyOnePartner.Text = "One partner may have more mutations";
            this.cbBPMutationLimitOnlyOnePartner.UseVisualStyleBackColor = true;
            this.cbBPMutationLimitOnlyOnePartner.CheckedChanged += new System.EventHandler(this.cbMutationLimitOnlyOnePartner_CheckedChanged);
            // 
            // tagSelectorList1
            // 
            this.tagSelectorList1.AutoScroll = true;
            this.tagSelectorList1.Location = new System.Drawing.Point(6, 144);
            this.tagSelectorList1.Name = "tagSelectorList1";
            this.tagSelectorList1.Size = new System.Drawing.Size(174, 263);
            this.tagSelectorList1.TabIndex = 3;
            // 
            // pedigreeCreatureBestPossibleInSpecies
            // 
            this.pedigreeCreatureBestPossibleInSpecies.Creature = null;
            this.pedigreeCreatureBestPossibleInSpecies.IsVirtual = false;
            this.pedigreeCreatureBestPossibleInSpecies.Location = new System.Drawing.Point(3, 44);
            this.pedigreeCreatureBestPossibleInSpecies.Name = "pedigreeCreatureBestPossibleInSpecies";
            this.pedigreeCreatureBestPossibleInSpecies.Size = new System.Drawing.Size(325, 35);
            this.pedigreeCreatureBestPossibleInSpecies.TabIndex = 5;
            // 
            // pedigreeCreature1
            // 
            this.pedigreeCreature1.Creature = null;
            this.pedigreeCreature1.IsVirtual = false;
            this.pedigreeCreature1.Location = new System.Drawing.Point(3, 82);
            this.pedigreeCreature1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.pedigreeCreature1.Name = "pedigreeCreature1";
            this.pedigreeCreature1.Size = new System.Drawing.Size(325, 35);
            this.pedigreeCreature1.TabIndex = 2;
            // 
            // pedigreeCreature2
            // 
            this.pedigreeCreature2.Creature = null;
            this.pedigreeCreature2.IsVirtual = false;
            this.pedigreeCreature2.Location = new System.Drawing.Point(427, 82);
            this.pedigreeCreature2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.pedigreeCreature2.Name = "pedigreeCreature2";
            this.pedigreeCreature2.Size = new System.Drawing.Size(325, 35);
            this.pedigreeCreature2.TabIndex = 3;
            // 
            // pedigreeCreatureBest
            // 
            this.pedigreeCreatureBest.Creature = null;
            this.pedigreeCreatureBest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flowLayoutPanel2.SetFlowBreak(this.pedigreeCreatureBest, true);
            this.pedigreeCreatureBest.IsVirtual = false;
            this.pedigreeCreatureBest.Location = new System.Drawing.Point(3, 16);
            this.pedigreeCreatureBest.Name = "pedigreeCreatureBest";
            this.pedigreeCreatureBest.Size = new System.Drawing.Size(325, 35);
            this.pedigreeCreatureBest.TabIndex = 1;
            // 
            // pedigreeCreatureWorst
            // 
            this.pedigreeCreatureWorst.Creature = null;
            this.pedigreeCreatureWorst.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flowLayoutPanel2.SetFlowBreak(this.pedigreeCreatureWorst, true);
            this.pedigreeCreatureWorst.IsVirtual = false;
            this.pedigreeCreatureWorst.Location = new System.Drawing.Point(3, 57);
            this.pedigreeCreatureWorst.Name = "pedigreeCreatureWorst";
            this.pedigreeCreatureWorst.Size = new System.Drawing.Size(325, 35);
            this.pedigreeCreatureWorst.TabIndex = 2;
            // 
            // offspringPossibilities1
            // 
            this.offspringPossibilities1.Location = new System.Drawing.Point(361, 3);
            this.offspringPossibilities1.Name = "offspringPossibilities1";
            this.offspringPossibilities1.Size = new System.Drawing.Size(247, 151);
            this.offspringPossibilities1.TabIndex = 1;
            // 
            // nudBPMutationLimit
            // 
            this.nudBPMutationLimit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudBPMutationLimit.Location = new System.Drawing.Point(162, 134);
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
            this.nudBPMutationLimit.Size = new System.Drawing.Size(50, 20);
            this.nudBPMutationLimit.TabIndex = 4;
            this.nudBPMutationLimit.ValueChanged += new System.EventHandler(this.nudMutationLimit_ValueChanged);
            // 
            // statWeighting1
            // 
            this.statWeighting1.CustomWeightings = ((System.Collections.Generic.Dictionary<string, double[]>)(resources.GetObject("statWeighting1.CustomWeightings")));
            this.statWeighting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statWeighting1.Location = new System.Drawing.Point(3, 766);
            this.statWeighting1.Name = "statWeighting1";
            this.statWeighting1.Size = new System.Drawing.Size(244, 254);
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
            // BreedingPlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutMain);
            this.Name = "BreedingPlan";
            this.Size = new System.Drawing.Size(1732, 1023);
            this.tableLayoutMain.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageBreedableSpecies.ResumeLayout(false);
            this.tabPageTags.ResumeLayout(false);
            this.tabPageTags.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.gbBPOffspring.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panelCombinations.ResumeLayout(false);
            this.gbBPBreedingMode.ResumeLayout(false);
            this.gbBPBreedingMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBPMutationLimit)).EndInit();
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
    }
}

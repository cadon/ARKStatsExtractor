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
            this.panelCombinations = new System.Windows.Forms.Panel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.offspringPossibilities1 = new ARKBreedingStats.OffspringPossibilities();
            this.buttonJustMated = new System.Windows.Forms.Button();
            this.labelBreedingInfos = new System.Windows.Forms.Label();
            this.labelProbabilityBest = new System.Windows.Forms.Label();
            this.listViewRaisingTimes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelBreedingDataTitle = new System.Windows.Forms.Label();
            this.pedigreeCreatureBest = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreatureWorst = new ARKBreedingStats.PedigreeCreature();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.labelBreedingScore = new System.Windows.Forms.Label();
            this.pedigreeCreature2 = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreature1 = new ARKBreedingStats.PedigreeCreature();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listViewSpeciesBP = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonApplyNewWeights = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxIncludeCooldowneds = new System.Windows.Forms.CheckBox();
            this.radioButtonBPTopStatsCn = new System.Windows.Forms.RadioButton();
            this.radioButtonBPHighStats = new System.Windows.Forms.RadioButton();
            this.radioButtonBPTopStats = new System.Windows.Forms.RadioButton();
            this.statWeighting1 = new ARKBreedingStats.StatWeighting();
            this.panelCombinations.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCombinations
            // 
            this.panelCombinations.AutoScroll = true;
            this.panelCombinations.Controls.Add(this.labelInfo);
            this.panelCombinations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCombinations.Location = new System.Drawing.Point(3, 73);
            this.panelCombinations.Name = "panelCombinations";
            this.panelCombinations.Size = new System.Drawing.Size(915, 418);
            this.panelCombinations.TabIndex = 3;
            // 
            // labelInfo
            // 
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(10, 75);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(683, 193);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "Infotext";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelInfo.Visible = false;
            // 
            // labelTitle
            // 
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(6, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(599, 20);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "Select a species and click on \"Determine Best Breeding\" to see suggestions";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panelCombinations, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelHeader, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(203, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel3.SetRowSpan(this.tableLayoutPanel1, 3);
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 182F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(921, 676);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.offspringPossibilities1);
            this.groupBox1.Controls.Add(this.buttonJustMated);
            this.groupBox1.Controls.Add(this.labelBreedingInfos);
            this.groupBox1.Controls.Add(this.labelProbabilityBest);
            this.groupBox1.Controls.Add(this.listViewRaisingTimes);
            this.groupBox1.Controls.Add(this.labelBreedingDataTitle);
            this.groupBox1.Controls.Add(this.pedigreeCreatureBest);
            this.groupBox1.Controls.Add(this.pedigreeCreatureWorst);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 497);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(915, 176);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Offspring";
            // 
            // offspringPossibilities1
            // 
            this.offspringPossibilities1.Location = new System.Drawing.Point(315, 19);
            this.offspringPossibilities1.Name = "offspringPossibilities1";
            this.offspringPossibilities1.Size = new System.Drawing.Size(247, 151);
            this.offspringPossibilities1.TabIndex = 1;
            // 
            // buttonJustMated
            // 
            this.buttonJustMated.Location = new System.Drawing.Point(6, 129);
            this.buttonJustMated.Name = "buttonJustMated";
            this.buttonJustMated.Size = new System.Drawing.Size(296, 33);
            this.buttonJustMated.TabIndex = 0;
            this.buttonJustMated.Text = "These Parents just mated";
            this.buttonJustMated.UseVisualStyleBackColor = true;
            this.buttonJustMated.Click += new System.EventHandler(this.buttonJustMated_Click);
            // 
            // labelBreedingInfos
            // 
            this.labelBreedingInfos.Location = new System.Drawing.Point(573, 125);
            this.labelBreedingInfos.Name = "labelBreedingInfos";
            this.labelBreedingInfos.Size = new System.Drawing.Size(347, 48);
            this.labelBreedingInfos.TabIndex = 7;
            this.labelBreedingInfos.Text = "Breeding Infos";
            // 
            // labelProbabilityBest
            // 
            this.labelProbabilityBest.AutoSize = true;
            this.labelProbabilityBest.Location = new System.Drawing.Point(6, 24);
            this.labelProbabilityBest.Name = "labelProbabilityBest";
            this.labelProbabilityBest.Size = new System.Drawing.Size(202, 13);
            this.labelProbabilityBest.TabIndex = 6;
            this.labelProbabilityBest.Text = "Probability for this Best Possible outcome:";
            // 
            // listViewRaisingTimes
            // 
            this.listViewRaisingTimes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewRaisingTimes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewRaisingTimes.Location = new System.Drawing.Point(576, 36);
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
            // labelBreedingDataTitle
            // 
            this.labelBreedingDataTitle.AutoSize = true;
            this.labelBreedingDataTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBreedingDataTitle.Location = new System.Drawing.Point(573, 16);
            this.labelBreedingDataTitle.Name = "labelBreedingDataTitle";
            this.labelBreedingDataTitle.Size = new System.Drawing.Size(121, 17);
            this.labelBreedingDataTitle.TabIndex = 3;
            this.labelBreedingDataTitle.Text = "Breeding Times";
            // 
            // pedigreeCreatureBest
            // 
            this.pedigreeCreatureBest.Creature = null;
            this.pedigreeCreatureBest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pedigreeCreatureBest.IsVirtual = false;
            this.pedigreeCreatureBest.Location = new System.Drawing.Point(6, 46);
            this.pedigreeCreatureBest.Name = "pedigreeCreatureBest";
            this.pedigreeCreatureBest.Size = new System.Drawing.Size(296, 35);
            this.pedigreeCreatureBest.TabIndex = 1;
            // 
            // pedigreeCreatureWorst
            // 
            this.pedigreeCreatureWorst.Creature = null;
            this.pedigreeCreatureWorst.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pedigreeCreatureWorst.IsVirtual = false;
            this.pedigreeCreatureWorst.Location = new System.Drawing.Point(6, 88);
            this.pedigreeCreatureWorst.Name = "pedigreeCreatureWorst";
            this.pedigreeCreatureWorst.Size = new System.Drawing.Size(296, 35);
            this.pedigreeCreatureWorst.TabIndex = 2;
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Controls.Add(this.labelBreedingScore);
            this.panelHeader.Controls.Add(this.pedigreeCreature2);
            this.panelHeader.Controls.Add(this.pedigreeCreature1);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHeader.Location = new System.Drawing.Point(3, 3);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(915, 64);
            this.panelHeader.TabIndex = 4;
            // 
            // labelBreedingScore
            // 
            this.labelBreedingScore.AutoSize = true;
            this.labelBreedingScore.Location = new System.Drawing.Point(312, 50);
            this.labelBreedingScore.Name = "labelBreedingScore";
            this.labelBreedingScore.Size = new System.Drawing.Size(80, 13);
            this.labelBreedingScore.TabIndex = 4;
            this.labelBreedingScore.Text = "Breeding-Score";
            // 
            // pedigreeCreature2
            // 
            this.pedigreeCreature2.Creature = null;
            this.pedigreeCreature2.IsVirtual = false;
            this.pedigreeCreature2.Location = new System.Drawing.Point(10, 28);
            this.pedigreeCreature2.Name = "pedigreeCreature2";
            this.pedigreeCreature2.Size = new System.Drawing.Size(296, 35);
            this.pedigreeCreature2.TabIndex = 3;
            // 
            // pedigreeCreature1
            // 
            this.pedigreeCreature1.Creature = null;
            this.pedigreeCreature1.IsVirtual = false;
            this.pedigreeCreature1.Location = new System.Drawing.Point(397, 28);
            this.pedigreeCreature1.Name = "pedigreeCreature1";
            this.pedigreeCreature1.Size = new System.Drawing.Size(296, 35);
            this.pedigreeCreature1.TabIndex = 2;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.buttonApplyNewWeights, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupBox4, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.statWeighting1, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 207F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1127, 682);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // groupBox3
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.listViewSpeciesBP);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(194, 439);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Breedable Species";
            // 
            // listViewSpeciesBP
            // 
            this.listViewSpeciesBP.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
            this.listViewSpeciesBP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSpeciesBP.FullRowSelect = true;
            this.listViewSpeciesBP.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewSpeciesBP.HideSelection = false;
            this.listViewSpeciesBP.Location = new System.Drawing.Point(3, 16);
            this.listViewSpeciesBP.MultiSelect = false;
            this.listViewSpeciesBP.Name = "listViewSpeciesBP";
            this.listViewSpeciesBP.Size = new System.Drawing.Size(188, 420);
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
            // buttonApplyNewWeights
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.buttonApplyNewWeights, 2);
            this.buttonApplyNewWeights.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonApplyNewWeights.Location = new System.Drawing.Point(3, 655);
            this.buttonApplyNewWeights.Name = "buttonApplyNewWeights";
            this.buttonApplyNewWeights.Size = new System.Drawing.Size(194, 24);
            this.buttonApplyNewWeights.TabIndex = 5;
            this.buttonApplyNewWeights.Text = "Apply new Weightings";
            this.buttonApplyNewWeights.UseVisualStyleBackColor = true;
            this.buttonApplyNewWeights.Click += new System.EventHandler(this.buttonApplyNewWeights_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxIncludeCooldowneds);
            this.groupBox4.Controls.Add(this.radioButtonBPTopStatsCn);
            this.groupBox4.Controls.Add(this.radioButtonBPHighStats);
            this.groupBox4.Controls.Add(this.radioButtonBPTopStats);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(103, 448);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(94, 201);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Breeding-Mode";
            // 
            // checkBoxIncludeCooldowneds
            // 
            this.checkBoxIncludeCooldowneds.Location = new System.Drawing.Point(6, 88);
            this.checkBoxIncludeCooldowneds.Name = "checkBoxIncludeCooldowneds";
            this.checkBoxIncludeCooldowneds.Size = new System.Drawing.Size(82, 63);
            this.checkBoxIncludeCooldowneds.TabIndex = 3;
            this.checkBoxIncludeCooldowneds.Text = "Include Creatures with Cooldown";
            this.checkBoxIncludeCooldowneds.UseVisualStyleBackColor = true;
            this.checkBoxIncludeCooldowneds.CheckedChanged += new System.EventHandler(this.checkBoxIncludeCooldowneds_CheckedChanged);
            // 
            // radioButtonBPTopStatsCn
            // 
            this.radioButtonBPTopStatsCn.AutoSize = true;
            this.radioButtonBPTopStatsCn.Checked = true;
            this.radioButtonBPTopStatsCn.Location = new System.Drawing.Point(6, 19);
            this.radioButtonBPTopStatsCn.Name = "radioButtonBPTopStatsCn";
            this.radioButtonBPTopStatsCn.Size = new System.Drawing.Size(87, 17);
            this.radioButtonBPTopStatsCn.TabIndex = 2;
            this.radioButtonBPTopStatsCn.TabStop = true;
            this.radioButtonBPTopStatsCn.Text = "Top Stats Cn";
            this.radioButtonBPTopStatsCn.UseVisualStyleBackColor = true;
            this.radioButtonBPTopStatsCn.CheckedChanged += new System.EventHandler(this.radioButtonBPTopStatsCn_CheckedChanged);
            // 
            // radioButtonBPHighStats
            // 
            this.radioButtonBPHighStats.AutoSize = true;
            this.radioButtonBPHighStats.Location = new System.Drawing.Point(6, 65);
            this.radioButtonBPHighStats.Name = "radioButtonBPHighStats";
            this.radioButtonBPHighStats.Size = new System.Drawing.Size(74, 17);
            this.radioButtonBPHighStats.TabIndex = 1;
            this.radioButtonBPHighStats.Text = "High Stats";
            this.radioButtonBPHighStats.UseVisualStyleBackColor = true;
            this.radioButtonBPHighStats.CheckedChanged += new System.EventHandler(this.radioButtonBPHighStats_CheckedChanged);
            // 
            // radioButtonBPTopStats
            // 
            this.radioButtonBPTopStats.AutoSize = true;
            this.radioButtonBPTopStats.Location = new System.Drawing.Point(6, 42);
            this.radioButtonBPTopStats.Name = "radioButtonBPTopStats";
            this.radioButtonBPTopStats.Size = new System.Drawing.Size(86, 17);
            this.radioButtonBPTopStats.TabIndex = 0;
            this.radioButtonBPTopStats.Text = "Top Stats Lc";
            this.radioButtonBPTopStats.UseVisualStyleBackColor = true;
            this.radioButtonBPTopStats.CheckedChanged += new System.EventHandler(this.radioButtonBPTopStats_CheckedChanged);
            // 
            // statWeighting1
            // 
            this.statWeighting1.CustomWeightings = ((System.Collections.Generic.Dictionary<string, double[]>)(resources.GetObject("statWeighting1.CustomWeightings")));
            this.statWeighting1.Location = new System.Drawing.Point(3, 448);
            this.statWeighting1.Name = "statWeighting1";
            this.statWeighting1.Size = new System.Drawing.Size(94, 201);
            this.statWeighting1.TabIndex = 7;
            this.statWeighting1.Values = new double[] {
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
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "BreedingPlan";
            this.Size = new System.Drawing.Size(1127, 682);
            this.panelCombinations.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private PedigreeCreature pedigreeCreatureBest;
        private PedigreeCreature pedigreeCreatureWorst;
        private System.Windows.Forms.Panel panelCombinations;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelBreedingDataTitle;
        private System.Windows.Forms.ListView listViewRaisingTimes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private PedigreeCreature pedigreeCreature1;
        private PedigreeCreature pedigreeCreature2;
        private System.Windows.Forms.Button buttonJustMated;
        private System.Windows.Forms.Label labelProbabilityBest;
        private System.Windows.Forms.Label labelBreedingScore;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelBreedingInfos;
        private OffspringPossibilities offspringPossibilities1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView listViewSpeciesBP;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button buttonApplyNewWeights;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxIncludeCooldowneds;
        private System.Windows.Forms.RadioButton radioButtonBPTopStatsCn;
        private System.Windows.Forms.RadioButton radioButtonBPHighStats;
        private System.Windows.Forms.RadioButton radioButtonBPTopStats;
        private StatWeighting statWeighting1;
    }
}

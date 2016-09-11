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
            this.panelCombinations = new System.Windows.Forms.Panel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelBreedingInfos = new System.Windows.Forms.Label();
            this.labelProbabilityBest = new System.Windows.Forms.Label();
            this.groupBoxTimer = new System.Windows.Forms.GroupBox();
            this.buttonBabyPhase = new System.Windows.Forms.Button();
            this.buttonHatching = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelBreedingDataTitle = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.labelBreedingScore = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pedigreeCreatureBest = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreatureWorst = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreature2 = new ARKBreedingStats.PedigreeCreature();
            this.pedigreeCreature1 = new ARKBreedingStats.PedigreeCreature();
            this.panelCombinations.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxTimer.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCombinations
            // 
            this.panelCombinations.AutoScroll = true;
            this.panelCombinations.Controls.Add(this.labelInfo);
            this.panelCombinations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCombinations.Location = new System.Drawing.Point(3, 73);
            this.panelCombinations.Name = "panelCombinations";
            this.panelCombinations.Size = new System.Drawing.Size(883, 449);
            this.panelCombinations.TabIndex = 3;
            // 
            // labelInfo
            // 
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(10, 75);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(589, 193);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(889, 675);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.labelProbabilityBest);
            this.groupBox1.Controls.Add(this.groupBoxTimer);
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.labelBreedingDataTitle);
            this.groupBox1.Controls.Add(this.pedigreeCreatureBest);
            this.groupBox1.Controls.Add(this.pedigreeCreatureWorst);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 528);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(883, 144);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Offspring";
            // 
            // labelBreedingInfos
            // 
            this.labelBreedingInfos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelBreedingInfos.Location = new System.Drawing.Point(3, 16);
            this.labelBreedingInfos.Name = "labelBreedingInfos";
            this.labelBreedingInfos.Size = new System.Drawing.Size(137, 84);
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
            // groupBoxTimer
            // 
            this.groupBoxTimer.Controls.Add(this.buttonBabyPhase);
            this.groupBoxTimer.Controls.Add(this.buttonHatching);
            this.groupBoxTimer.Location = new System.Drawing.Point(584, 19);
            this.groupBoxTimer.Name = "groupBoxTimer";
            this.groupBoxTimer.Size = new System.Drawing.Size(112, 103);
            this.groupBoxTimer.TabIndex = 5;
            this.groupBoxTimer.TabStop = false;
            this.groupBoxTimer.Text = "Add Timer";
            // 
            // buttonBabyPhase
            // 
            this.buttonBabyPhase.Location = new System.Drawing.Point(6, 48);
            this.buttonBabyPhase.Name = "buttonBabyPhase";
            this.buttonBabyPhase.Size = new System.Drawing.Size(95, 23);
            this.buttonBabyPhase.TabIndex = 1;
            this.buttonBabyPhase.Text = "Baby-Phase";
            this.buttonBabyPhase.UseVisualStyleBackColor = true;
            this.buttonBabyPhase.Click += new System.EventHandler(this.buttonBabyPhase_Click);
            // 
            // buttonHatching
            // 
            this.buttonHatching.Location = new System.Drawing.Point(6, 19);
            this.buttonHatching.Name = "buttonHatching";
            this.buttonHatching.Size = new System.Drawing.Size(95, 23);
            this.buttonHatching.TabIndex = 0;
            this.buttonHatching.Text = "Pregnancy";
            this.buttonHatching.UseVisualStyleBackColor = true;
            this.buttonHatching.Click += new System.EventHandler(this.buttonHatching_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(261, 36);
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(317, 86);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
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
            this.labelBreedingDataTitle.Location = new System.Drawing.Point(258, 16);
            this.labelBreedingDataTitle.Name = "labelBreedingDataTitle";
            this.labelBreedingDataTitle.Size = new System.Drawing.Size(121, 17);
            this.labelBreedingDataTitle.TabIndex = 3;
            this.labelBreedingDataTitle.Text = "Breeding Times";
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
            this.panelHeader.Size = new System.Drawing.Size(883, 64);
            this.panelHeader.TabIndex = 4;
            // 
            // labelBreedingScore
            // 
            this.labelBreedingScore.AutoSize = true;
            this.labelBreedingScore.Location = new System.Drawing.Point(265, 50);
            this.labelBreedingScore.Name = "labelBreedingScore";
            this.labelBreedingScore.Size = new System.Drawing.Size(80, 13);
            this.labelBreedingScore.TabIndex = 4;
            this.labelBreedingScore.Text = "Breeding-Score";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelBreedingInfos);
            this.groupBox2.Location = new System.Drawing.Point(702, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(143, 103);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Breeding Infos";
            // 
            // pedigreeCreatureBest
            // 
            this.pedigreeCreatureBest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pedigreeCreatureBest.IsVirtual = false;
            this.pedigreeCreatureBest.Location = new System.Drawing.Point(6, 46);
            this.pedigreeCreatureBest.Name = "pedigreeCreatureBest";
            this.pedigreeCreatureBest.Size = new System.Drawing.Size(249, 35);
            this.pedigreeCreatureBest.TabIndex = 1;
            // 
            // pedigreeCreatureWorst
            // 
            this.pedigreeCreatureWorst.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pedigreeCreatureWorst.IsVirtual = false;
            this.pedigreeCreatureWorst.Location = new System.Drawing.Point(6, 87);
            this.pedigreeCreatureWorst.Name = "pedigreeCreatureWorst";
            this.pedigreeCreatureWorst.Size = new System.Drawing.Size(249, 35);
            this.pedigreeCreatureWorst.TabIndex = 2;
            // 
            // pedigreeCreature2
            // 
            this.pedigreeCreature2.IsVirtual = false;
            this.pedigreeCreature2.Location = new System.Drawing.Point(10, 28);
            this.pedigreeCreature2.Name = "pedigreeCreature2";
            this.pedigreeCreature2.Size = new System.Drawing.Size(249, 35);
            this.pedigreeCreature2.TabIndex = 3;
            // 
            // pedigreeCreature1
            // 
            this.pedigreeCreature1.IsVirtual = false;
            this.pedigreeCreature1.Location = new System.Drawing.Point(350, 28);
            this.pedigreeCreature1.Name = "pedigreeCreature1";
            this.pedigreeCreature1.Size = new System.Drawing.Size(249, 35);
            this.pedigreeCreature1.TabIndex = 2;
            // 
            // BreedingPlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BreedingPlan";
            this.Size = new System.Drawing.Size(889, 675);
            this.panelCombinations.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxTimer.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private PedigreeCreature pedigreeCreature1;
        private PedigreeCreature pedigreeCreature2;
        private System.Windows.Forms.GroupBox groupBoxTimer;
        private System.Windows.Forms.Button buttonHatching;
        private System.Windows.Forms.Label labelProbabilityBest;
        private System.Windows.Forms.Label labelBreedingScore;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonBabyPhase;
        private System.Windows.Forms.Label labelBreedingInfos;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

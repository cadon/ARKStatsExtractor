
namespace ARKBreedingStats.library
{
    partial class AddDummyCreaturesSettings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtOk = new System.Windows.Forms.Button();
            this.BtCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RbMultipleRandomSpecies = new System.Windows.Forms.RadioButton();
            this.RbOnlySelectedSpecies = new System.Windows.Forms.RadioButton();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.CbSetOwner = new System.Windows.Forms.CheckBox();
            this.CbSetServer = new System.Windows.Forms.CheckBox();
            this.NudMutationChance = new ARKBreedingStats.uiControls.Nud();
            this.NudProbabilityInheritingHigherStat = new ARKBreedingStats.uiControls.Nud();
            this.NudUsePairsPerGeneration = new ARKBreedingStats.uiControls.Nud();
            this.NudBreedForGenerations = new ARKBreedingStats.uiControls.Nud();
            this.NudSpeciesAmount = new ARKBreedingStats.uiControls.Nud();
            this.nudMaxWildLevel = new ARKBreedingStats.uiControls.Nud();
            this.NudAmount = new ARKBreedingStats.uiControls.Nud();
            this.CbSetTribe = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudMutationChance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudProbabilityInheritingHigherStat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudUsePairsPerGeneration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudBreedForGenerations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSpeciesAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxWildLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // BtOk
            // 
            this.BtOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtOk.Location = new System.Drawing.Point(253, 331);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(104, 23);
            this.BtOk.TabIndex = 0;
            this.BtOk.Text = "Add creatures";
            this.BtOk.UseVisualStyleBackColor = true;
            this.BtOk.Click += new System.EventHandler(this.BtOk_Click);
            // 
            // BtCancel
            // 
            this.BtCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtCancel.Location = new System.Drawing.Point(143, 331);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(104, 23);
            this.BtCancel.TabIndex = 1;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            this.BtCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Amount of creatures";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Breed for generations";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nudMaxWildLevel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NudAmount);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(347, 49);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tamed Creatures";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "max wild level";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RbMultipleRandomSpecies);
            this.groupBox2.Controls.Add(this.RbOnlySelectedSpecies);
            this.groupBox2.Controls.Add(this.NudSpeciesAmount);
            this.groupBox2.Location = new System.Drawing.Point(12, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(347, 71);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Species";
            // 
            // RbMultipleRandomSpecies
            // 
            this.RbMultipleRandomSpecies.AutoSize = true;
            this.RbMultipleRandomSpecies.Location = new System.Drawing.Point(6, 42);
            this.RbMultipleRandomSpecies.Name = "RbMultipleRandomSpecies";
            this.RbMultipleRandomSpecies.Size = new System.Drawing.Size(138, 17);
            this.RbMultipleRandomSpecies.TabIndex = 8;
            this.RbMultipleRandomSpecies.Text = "Multiple random species";
            this.RbMultipleRandomSpecies.UseVisualStyleBackColor = true;
            // 
            // RbOnlySelectedSpecies
            // 
            this.RbOnlySelectedSpecies.AutoSize = true;
            this.RbOnlySelectedSpecies.Location = new System.Drawing.Point(6, 19);
            this.RbOnlySelectedSpecies.Name = "RbOnlySelectedSpecies";
            this.RbOnlySelectedSpecies.Size = new System.Drawing.Size(128, 17);
            this.RbOnlySelectedSpecies.TabIndex = 7;
            this.RbOnlySelectedSpecies.Text = "Only selected species";
            this.RbOnlySelectedSpecies.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.NudMutationChance);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.NudProbabilityInheritingHigherStat);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.NudUsePairsPerGeneration);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.NudBreedForGenerations);
            this.groupBox3.Location = new System.Drawing.Point(10, 198);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(347, 127);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Breed the tamed creatures";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(204, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Probability in % of mutation (ingame 2.5 %)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(258, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Probability in % of inheriting better stats (ingame 55 %)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(184, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Use the best pairs of each generation";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.CbSetTribe);
            this.groupBox4.Controls.Add(this.CbSetServer);
            this.groupBox4.Controls.Add(this.CbSetOwner);
            this.groupBox4.Location = new System.Drawing.Point(12, 144);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(345, 48);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Details";
            // 
            // CbSetOwner
            // 
            this.CbSetOwner.AutoSize = true;
            this.CbSetOwner.Location = new System.Drawing.Point(6, 19);
            this.CbSetOwner.Name = "CbSetOwner";
            this.CbSetOwner.Size = new System.Drawing.Size(72, 17);
            this.CbSetOwner.TabIndex = 0;
            this.CbSetOwner.Text = "set owner";
            this.CbSetOwner.UseVisualStyleBackColor = true;
            // 
            // CbSetServer
            // 
            this.CbSetServer.AutoSize = true;
            this.CbSetServer.Location = new System.Drawing.Point(162, 19);
            this.CbSetServer.Name = "CbSetServer";
            this.CbSetServer.Size = new System.Drawing.Size(72, 17);
            this.CbSetServer.TabIndex = 1;
            this.CbSetServer.Text = "set server";
            this.CbSetServer.UseVisualStyleBackColor = true;
            // 
            // NudMutationChance
            // 
            this.NudMutationChance.DecimalPlaces = 2;
            this.NudMutationChance.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudMutationChance.Location = new System.Drawing.Point(270, 97);
            this.NudMutationChance.Name = "NudMutationChance";
            this.NudMutationChance.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudMutationChance.Size = new System.Drawing.Size(67, 20);
            this.NudMutationChance.TabIndex = 17;
            // 
            // NudProbabilityInheritingHigherStat
            // 
            this.NudProbabilityInheritingHigherStat.DecimalPlaces = 2;
            this.NudProbabilityInheritingHigherStat.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudProbabilityInheritingHigherStat.Location = new System.Drawing.Point(270, 71);
            this.NudProbabilityInheritingHigherStat.Name = "NudProbabilityInheritingHigherStat";
            this.NudProbabilityInheritingHigherStat.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudProbabilityInheritingHigherStat.Size = new System.Drawing.Size(67, 20);
            this.NudProbabilityInheritingHigherStat.TabIndex = 15;
            // 
            // NudUsePairsPerGeneration
            // 
            this.NudUsePairsPerGeneration.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudUsePairsPerGeneration.Location = new System.Drawing.Point(270, 45);
            this.NudUsePairsPerGeneration.Name = "NudUsePairsPerGeneration";
            this.NudUsePairsPerGeneration.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudUsePairsPerGeneration.Size = new System.Drawing.Size(67, 20);
            this.NudUsePairsPerGeneration.TabIndex = 13;
            // 
            // NudBreedForGenerations
            // 
            this.NudBreedForGenerations.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudBreedForGenerations.Location = new System.Drawing.Point(270, 19);
            this.NudBreedForGenerations.Name = "NudBreedForGenerations";
            this.NudBreedForGenerations.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudBreedForGenerations.Size = new System.Drawing.Size(67, 20);
            this.NudBreedForGenerations.TabIndex = 8;
            // 
            // NudSpeciesAmount
            // 
            this.NudSpeciesAmount.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudSpeciesAmount.Location = new System.Drawing.Point(150, 42);
            this.NudSpeciesAmount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudSpeciesAmount.Name = "NudSpeciesAmount";
            this.NudSpeciesAmount.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudSpeciesAmount.Size = new System.Drawing.Size(67, 20);
            this.NudSpeciesAmount.TabIndex = 6;
            // 
            // nudMaxWildLevel
            // 
            this.nudMaxWildLevel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.nudMaxWildLevel.Location = new System.Drawing.Point(274, 19);
            this.nudMaxWildLevel.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudMaxWildLevel.Name = "nudMaxWildLevel";
            this.nudMaxWildLevel.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMaxWildLevel.Size = new System.Drawing.Size(67, 20);
            this.nudMaxWildLevel.TabIndex = 5;
            // 
            // NudAmount
            // 
            this.NudAmount.ForeColor = System.Drawing.SystemColors.GrayText;
            this.NudAmount.Location = new System.Drawing.Point(114, 19);
            this.NudAmount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NudAmount.Name = "NudAmount";
            this.NudAmount.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudAmount.Size = new System.Drawing.Size(67, 20);
            this.NudAmount.TabIndex = 3;
            // 
            // CbSetTribe
            // 
            this.CbSetTribe.AutoSize = true;
            this.CbSetTribe.Location = new System.Drawing.Point(84, 19);
            this.CbSetTribe.Name = "CbSetTribe";
            this.CbSetTribe.Size = new System.Drawing.Size(63, 17);
            this.CbSetTribe.TabIndex = 2;
            this.CbSetTribe.Text = "set tribe";
            this.CbSetTribe.UseVisualStyleBackColor = true;
            // 
            // AddDummyCreaturesSettings
            // 
            this.AcceptButton = this.BtOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtCancel;
            this.ClientSize = new System.Drawing.Size(369, 366);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtCancel);
            this.Controls.Add(this.BtOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddDummyCreaturesSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add random creatures";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudMutationChance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudProbabilityInheritingHigherStat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudUsePairsPerGeneration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudBreedForGenerations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSpeciesAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxWildLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudAmount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.Button BtCancel;
        private uiControls.Nud NudAmount;
        private System.Windows.Forms.Label label1;
        private uiControls.Nud NudSpeciesAmount;
        private uiControls.Nud NudBreedForGenerations;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.GroupBox groupBox3;
        private uiControls.Nud NudMutationChance;
        private System.Windows.Forms.Label label6;
        private uiControls.Nud NudProbabilityInheritingHigherStat;
        private System.Windows.Forms.Label label5;
        private uiControls.Nud NudUsePairsPerGeneration;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton RbMultipleRandomSpecies;
        private System.Windows.Forms.RadioButton RbOnlySelectedSpecies;
        private System.Windows.Forms.Label label2;
        private uiControls.Nud nudMaxWildLevel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox CbSetServer;
        private System.Windows.Forms.CheckBox CbSetOwner;
        private System.Windows.Forms.CheckBox CbSetTribe;
    }
}
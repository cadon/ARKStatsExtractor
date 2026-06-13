
using ARKBreedingStats.uiControls;

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
            BtOk = new System.Windows.Forms.Button();
            BtCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            groupBox1 = new GroupBoxC();
            CbTameCreatures = new System.Windows.Forms.CheckBox();
            label7 = new System.Windows.Forms.Label();
            NudMaxStatLevel = new Nud();
            label2 = new System.Windows.Forms.Label();
            nudMaxWildLevel = new Nud();
            NudAmount = new Nud();
            groupBox2 = new GroupBoxC();
            RbMultipleRandomSpecies = new System.Windows.Forms.RadioButton();
            RbOnlySelectedSpecies = new System.Windows.Forms.RadioButton();
            NudSpeciesAmount = new Nud();
            fontDialog1 = new System.Windows.Forms.FontDialog();
            groupBox3 = new GroupBoxC();
            NudMutationChance = new Nud();
            label6 = new System.Windows.Forms.Label();
            NudProbabilityInheritingHigherStat = new Nud();
            label5 = new System.Windows.Forms.Label();
            NudUsePairsPerGeneration = new Nud();
            label4 = new System.Windows.Forms.Label();
            NudBreedForGenerations = new Nud();
            groupBox4 = new GroupBoxC();
            CbSetTribe = new System.Windows.Forms.CheckBox();
            CbSetServer = new System.Windows.Forms.CheckBox();
            CbSetOwner = new System.Windows.Forms.CheckBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NudMaxStatLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWildLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NudAmount).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NudSpeciesAmount).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NudMutationChance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NudProbabilityInheritingHigherStat).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NudUsePairsPerGeneration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NudBreedForGenerations).BeginInit();
            groupBox4.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // BtOk
            // 
            BtOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtOk.Location = new System.Drawing.Point(290, 421);
            BtOk.Name = "BtOk";
            BtOk.Size = new System.Drawing.Size(104, 23);
            BtOk.TabIndex = 0;
            BtOk.Text = "Add creatures";
            BtOk.UseVisualStyleBackColor = true;
            BtOk.Click += BtOk_Click;
            // 
            // BtCancel
            // 
            BtCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BtCancel.Location = new System.Drawing.Point(180, 421);
            BtCancel.Name = "BtCancel";
            BtCancel.Size = new System.Drawing.Size(104, 23);
            BtCancel.TabIndex = 1;
            BtCancel.Text = "Cancel";
            BtCancel.UseVisualStyleBackColor = true;
            BtCancel.Click += BtCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(116, 15);
            label1.TabIndex = 4;
            label1.Text = "Amount of creatures";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(120, 15);
            label3.TabIndex = 7;
            label3.Text = "Breed for generations";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tableLayoutPanel1);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(406, 141);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "Creatures";
            // 
            // CbTameCreatures
            // 
            CbTameCreatures.AutoSize = true;
            CbTameCreatures.Location = new System.Drawing.Point(3, 32);
            CbTameCreatures.Name = "CbTameCreatures";
            CbTameCreatures.Size = new System.Drawing.Size(62, 19);
            CbTameCreatures.TabIndex = 9;
            CbTameCreatures.Text = "Tamed";
            CbTameCreatures.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(3, 83);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(78, 15);
            label7.TabIndex = 8;
            label7.Text = "max stat level";
            // 
            // NudMaxStatLevel
            // 
            NudMaxStatLevel.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudMaxStatLevel.Location = new System.Drawing.Point(125, 86);
            NudMaxStatLevel.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            NudMaxStatLevel.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            NudMaxStatLevel.Name = "NudMaxStatLevel";
            NudMaxStatLevel.Size = new System.Drawing.Size(67, 23);
            NudMaxStatLevel.TabIndex = 7;
            NudMaxStatLevel.Value = new decimal(new int[] { 1, 0, 0, int.MinValue });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 54);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(81, 15);
            label2.TabIndex = 6;
            label2.Text = "max wild level";
            // 
            // nudMaxWildLevel
            // 
            nudMaxWildLevel.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            nudMaxWildLevel.Location = new System.Drawing.Point(125, 57);
            nudMaxWildLevel.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudMaxWildLevel.Name = "nudMaxWildLevel";
            nudMaxWildLevel.Size = new System.Drawing.Size(67, 23);
            nudMaxWildLevel.TabIndex = 5;
            // 
            // NudAmount
            // 
            NudAmount.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudAmount.Location = new System.Drawing.Point(125, 3);
            NudAmount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            NudAmount.Name = "NudAmount";
            NudAmount.Size = new System.Drawing.Size(67, 23);
            NudAmount.TabIndex = 3;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(tableLayoutPanel2);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox2.Location = new System.Drawing.Point(0, 141);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(406, 81);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "Species";
            // 
            // RbMultipleRandomSpecies
            // 
            RbMultipleRandomSpecies.AutoSize = true;
            RbMultipleRandomSpecies.Location = new System.Drawing.Point(3, 28);
            RbMultipleRandomSpecies.Name = "RbMultipleRandomSpecies";
            RbMultipleRandomSpecies.Size = new System.Drawing.Size(155, 19);
            RbMultipleRandomSpecies.TabIndex = 8;
            RbMultipleRandomSpecies.Text = "Multiple random species";
            RbMultipleRandomSpecies.UseVisualStyleBackColor = true;
            // 
            // RbOnlySelectedSpecies
            // 
            RbOnlySelectedSpecies.AutoSize = true;
            RbOnlySelectedSpecies.Location = new System.Drawing.Point(3, 3);
            RbOnlySelectedSpecies.Name = "RbOnlySelectedSpecies";
            RbOnlySelectedSpecies.Size = new System.Drawing.Size(137, 19);
            RbOnlySelectedSpecies.TabIndex = 7;
            RbOnlySelectedSpecies.Text = "Only selected species";
            RbOnlySelectedSpecies.UseVisualStyleBackColor = true;
            // 
            // NudSpeciesAmount
            // 
            NudSpeciesAmount.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudSpeciesAmount.Location = new System.Drawing.Point(164, 28);
            NudSpeciesAmount.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            NudSpeciesAmount.Name = "NudSpeciesAmount";
            NudSpeciesAmount.Size = new System.Drawing.Size(67, 23);
            NudSpeciesAmount.TabIndex = 6;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(tableLayoutPanel3);
            groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox3.Location = new System.Drawing.Point(0, 270);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(406, 141);
            groupBox3.TabIndex = 11;
            groupBox3.TabStop = false;
            groupBox3.Text = "Breed the tamed creatures";
            // 
            // NudMutationChance
            // 
            NudMutationChance.DecimalPlaces = 2;
            NudMutationChance.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudMutationChance.Location = new System.Drawing.Point(307, 90);
            NudMutationChance.Name = "NudMutationChance";
            NudMutationChance.Size = new System.Drawing.Size(67, 23);
            NudMutationChance.TabIndex = 17;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(3, 87);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(238, 15);
            label6.TabIndex = 16;
            label6.Text = "Probability in % of mutation (ingame 2.5 %)";
            // 
            // NudProbabilityInheritingHigherStat
            // 
            NudProbabilityInheritingHigherStat.DecimalPlaces = 2;
            NudProbabilityInheritingHigherStat.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudProbabilityInheritingHigherStat.Location = new System.Drawing.Point(307, 61);
            NudProbabilityInheritingHigherStat.Name = "NudProbabilityInheritingHigherStat";
            NudProbabilityInheritingHigherStat.Size = new System.Drawing.Size(67, 23);
            NudProbabilityInheritingHigherStat.TabIndex = 15;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(3, 58);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(298, 15);
            label5.TabIndex = 14;
            label5.Text = "Probability in % of inheriting better stats (ingame 55 %)";
            // 
            // NudUsePairsPerGeneration
            // 
            NudUsePairsPerGeneration.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudUsePairsPerGeneration.Location = new System.Drawing.Point(307, 32);
            NudUsePairsPerGeneration.Name = "NudUsePairsPerGeneration";
            NudUsePairsPerGeneration.Size = new System.Drawing.Size(67, 23);
            NudUsePairsPerGeneration.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 29);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(201, 15);
            label4.TabIndex = 12;
            label4.Text = "Use the best pairs of each generation";
            // 
            // NudBreedForGenerations
            // 
            NudBreedForGenerations.ForeColor = System.Drawing.Color.FromArgb(44, 44, 44);
            NudBreedForGenerations.Location = new System.Drawing.Point(307, 3);
            NudBreedForGenerations.Name = "NudBreedForGenerations";
            NudBreedForGenerations.Size = new System.Drawing.Size(67, 23);
            NudBreedForGenerations.TabIndex = 8;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(CbSetServer);
            groupBox4.Controls.Add(CbSetTribe);
            groupBox4.Controls.Add(CbSetOwner);
            groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            groupBox4.Location = new System.Drawing.Point(0, 222);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(406, 48);
            groupBox4.TabIndex = 12;
            groupBox4.TabStop = false;
            groupBox4.Text = "Details";
            // 
            // CbSetTribe
            // 
            CbSetTribe.AutoSize = true;
            CbSetTribe.Dock = System.Windows.Forms.DockStyle.Left;
            CbSetTribe.Location = new System.Drawing.Point(80, 19);
            CbSetTribe.Name = "CbSetTribe";
            CbSetTribe.Size = new System.Drawing.Size(68, 26);
            CbSetTribe.TabIndex = 2;
            CbSetTribe.Text = "set tribe";
            CbSetTribe.UseVisualStyleBackColor = true;
            // 
            // CbSetServer
            // 
            CbSetServer.AutoSize = true;
            CbSetServer.Dock = System.Windows.Forms.DockStyle.Left;
            CbSetServer.Location = new System.Drawing.Point(148, 19);
            CbSetServer.Name = "CbSetServer";
            CbSetServer.Size = new System.Drawing.Size(75, 26);
            CbSetServer.TabIndex = 1;
            CbSetServer.Text = "set server";
            CbSetServer.UseVisualStyleBackColor = true;
            // 
            // CbSetOwner
            // 
            CbSetOwner.AutoSize = true;
            CbSetOwner.Dock = System.Windows.Forms.DockStyle.Left;
            CbSetOwner.Location = new System.Drawing.Point(3, 19);
            CbSetOwner.Name = "CbSetOwner";
            CbSetOwner.Size = new System.Drawing.Size(77, 26);
            CbSetOwner.TabIndex = 0;
            CbSetOwner.Text = "set owner";
            CbSetOwner.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(NudMaxStatLevel, 1, 3);
            tableLayoutPanel1.Controls.Add(label7, 0, 3);
            tableLayoutPanel1.Controls.Add(CbTameCreatures, 0, 1);
            tableLayoutPanel1.Controls.Add(NudAmount, 1, 0);
            tableLayoutPanel1.Controls.Add(nudMaxWildLevel, 1, 2);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(400, 119);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.Controls.Add(RbOnlySelectedSpecies, 0, 0);
            tableLayoutPanel2.Controls.Add(NudSpeciesAmount, 1, 1);
            tableLayoutPanel2.Controls.Add(RbMultipleRandomSpecies, 0, 1);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.Size = new System.Drawing.Size(400, 59);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.Controls.Add(label3, 0, 0);
            tableLayoutPanel3.Controls.Add(NudMutationChance, 1, 3);
            tableLayoutPanel3.Controls.Add(label4, 0, 1);
            tableLayoutPanel3.Controls.Add(NudProbabilityInheritingHigherStat, 1, 2);
            tableLayoutPanel3.Controls.Add(label6, 0, 3);
            tableLayoutPanel3.Controls.Add(NudUsePairsPerGeneration, 1, 1);
            tableLayoutPanel3.Controls.Add(label5, 0, 2);
            tableLayoutPanel3.Controls.Add(NudBreedForGenerations, 1, 0);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(3, 19);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 4;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.Size = new System.Drawing.Size(400, 119);
            tableLayoutPanel3.TabIndex = 18;
            // 
            // AddDummyCreaturesSettings
            // 
            AcceptButton = BtOk;
            CancelButton = BtCancel;
            ClientSize = new System.Drawing.Size(406, 456);
            Controls.Add(groupBox3);
            Controls.Add(groupBox4);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(BtCancel);
            Controls.Add(BtOk);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "AddDummyCreaturesSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Add random creatures";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NudMaxStatLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxWildLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)NudAmount).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NudSpeciesAmount).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NudMutationChance).EndInit();
            ((System.ComponentModel.ISupportInitialize)NudProbabilityInheritingHigherStat).EndInit();
            ((System.ComponentModel.ISupportInitialize)NudUsePairsPerGeneration).EndInit();
            ((System.ComponentModel.ISupportInitialize)NudBreedForGenerations).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.Button BtCancel;
        private uiControls.Nud NudAmount;
        private System.Windows.Forms.Label label1;
        private uiControls.Nud NudSpeciesAmount;
        private uiControls.Nud NudBreedForGenerations;
        private System.Windows.Forms.Label label3;
        private GroupBoxC groupBox1;
        private GroupBoxC groupBox2;
        private System.Windows.Forms.FontDialog fontDialog1;
        private GroupBoxC groupBox3;
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
        private GroupBoxC groupBox4;
        private System.Windows.Forms.CheckBox CbSetServer;
        private System.Windows.Forms.CheckBox CbSetOwner;
        private System.Windows.Forms.CheckBox CbSetTribe;
        private System.Windows.Forms.CheckBox CbTameCreatures;
        private System.Windows.Forms.Label label7;
        private uiControls.Nud NudMaxStatLevel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
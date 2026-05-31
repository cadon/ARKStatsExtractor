using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.Pedigree
{
    partial class PedigreeControl
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
            PbRegionColors = new System.Windows.Forms.PictureBox();
            lbPedigreeEmpty = new System.Windows.Forms.Label();
            listViewCreatures = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            RbViewH = new System.Windows.Forms.RadioButton();
            RbViewCompact = new System.Windows.Forms.RadioButton();
            RbViewClassic = new System.Windows.Forms.RadioButton();
            TextBoxFilter = new System.Windows.Forms.TextBox();
            ButtonClearFilter = new System.Windows.Forms.Button();
            TbZoom = new System.Windows.Forms.TrackBar();
            PbKeyExplanations = new System.Windows.Forms.PictureBox();
            statSelector1 = new StatSelector();
            LbCreatureName = new System.Windows.Forms.Label();
            nudGenerations = new Nud();
            ((System.ComponentModel.ISupportInitialize)PbRegionColors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TbZoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PbKeyExplanations).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGenerations).BeginInit();
            SuspendLayout();
            // 
            // PbRegionColors
            // 
            PbRegionColors.Location = new System.Drawing.Point(460, 208);
            PbRegionColors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PbRegionColors.Name = "PbRegionColors";
            PbRegionColors.Size = new System.Drawing.Size(299, 295);
            PbRegionColors.TabIndex = 0;
            PbRegionColors.TabStop = false;
            PbRegionColors.Click += pictureBox_Click;
            // 
            // lbPedigreeEmpty
            // 
            lbPedigreeEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            lbPedigreeEmpty.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lbPedigreeEmpty.Location = new System.Drawing.Point(0, 0);
            lbPedigreeEmpty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbPedigreeEmpty.Name = "lbPedigreeEmpty";
            lbPedigreeEmpty.Size = new System.Drawing.Size(975, 600);
            lbPedigreeEmpty.TabIndex = 1;
            lbPedigreeEmpty.Text = "Select a creature in the Library to see its pedigree here.";
            lbPedigreeEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listViewCreatures
            // 
            listViewCreatures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2 });
            tableLayoutPanel1.SetColumnSpan(listViewCreatures, 2);
            listViewCreatures.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewCreatures.FullRowSelect = true;
            listViewCreatures.Location = new System.Drawing.Point(4, 71);
            listViewCreatures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listViewCreatures.MultiSelect = false;
            listViewCreatures.Name = "listViewCreatures";
            listViewCreatures.Size = new System.Drawing.Size(229, 526);
            listViewCreatures.TabIndex = 3;
            listViewCreatures.UseCompatibleStateImageBehavior = false;
            listViewCreatures.View = System.Windows.Forms.View.Details;
            listViewCreatures.ColumnClick += listViewCreatures_ColumnClick;
            listViewCreatures.SelectedIndexChanged += listViewCreatures_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Lvl";
            columnHeader2.Width = 31;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.Controls.Add(lbPedigreeEmpty);
            splitContainer1.Panel2.Controls.Add(TbZoom);
            splitContainer1.Panel2.Controls.Add(PbKeyExplanations);
            splitContainer1.Panel2.Controls.Add(statSelector1);
            splitContainer1.Panel2.Controls.Add(LbCreatureName);
            splitContainer1.Panel2.Controls.Add(nudGenerations);
            splitContainer1.Panel2.Controls.Add(PbRegionColors);
            splitContainer1.Size = new System.Drawing.Size(1217, 600);
            splitContainer1.SplitterDistance = 237;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(listViewCreatures, 0, 2);
            tableLayoutPanel1.Controls.Add(TextBoxFilter, 0, 1);
            tableLayoutPanel1.Controls.Add(ButtonClearFilter, 1, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(237, 600);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            tableLayoutPanel1.SetColumnSpan(panel1, 2);
            panel1.Controls.Add(RbViewH);
            panel1.Controls.Add(RbViewCompact);
            panel1.Controls.Add(RbViewClassic);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(4, 3);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(229, 29);
            panel1.TabIndex = 6;
            // 
            // RbViewH
            // 
            RbViewH.AutoSize = true;
            RbViewH.Location = new System.Drawing.Point(163, 3);
            RbViewH.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbViewH.Name = "RbViewH";
            RbViewH.Size = new System.Drawing.Size(34, 19);
            RbViewH.TabIndex = 2;
            RbViewH.TabStop = true;
            RbViewH.Text = "H";
            RbViewH.UseVisualStyleBackColor = true;
            RbViewH.CheckedChanged += RbViewH_CheckedChanged;
            // 
            // RbViewCompact
            // 
            RbViewCompact.AutoSize = true;
            RbViewCompact.Location = new System.Drawing.Point(78, 3);
            RbViewCompact.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbViewCompact.Name = "RbViewCompact";
            RbViewCompact.Size = new System.Drawing.Size(74, 19);
            RbViewCompact.TabIndex = 1;
            RbViewCompact.TabStop = true;
            RbViewCompact.Text = "Compact";
            RbViewCompact.UseVisualStyleBackColor = true;
            RbViewCompact.CheckedChanged += RbViewCompact_CheckedChanged;
            // 
            // RbViewClassic
            // 
            RbViewClassic.AutoSize = true;
            RbViewClassic.Location = new System.Drawing.Point(4, 3);
            RbViewClassic.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbViewClassic.Name = "RbViewClassic";
            RbViewClassic.Size = new System.Drawing.Size(61, 19);
            RbViewClassic.TabIndex = 0;
            RbViewClassic.TabStop = true;
            RbViewClassic.Text = "Classic";
            RbViewClassic.UseVisualStyleBackColor = true;
            RbViewClassic.CheckedChanged += RbViewClassic_CheckedChanged;
            // 
            // TextBoxFilter
            // 
            TextBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            TextBoxFilter.Location = new System.Drawing.Point(4, 38);
            TextBoxFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TextBoxFilter.Name = "TextBoxFilter";
            TextBoxFilter.Size = new System.Drawing.Size(202, 23);
            TextBoxFilter.TabIndex = 4;
            TextBoxFilter.TextChanged += TextBoxFilterTextChanged;
            // 
            // ButtonClearFilter
            // 
            ButtonClearFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            ButtonClearFilter.Location = new System.Drawing.Point(211, 36);
            ButtonClearFilter.Margin = new System.Windows.Forms.Padding(1);
            ButtonClearFilter.Name = "ButtonClearFilter";
            ButtonClearFilter.Size = new System.Drawing.Size(25, 31);
            ButtonClearFilter.TabIndex = 5;
            ButtonClearFilter.Text = "×";
            ButtonClearFilter.UseVisualStyleBackColor = true;
            ButtonClearFilter.Click += ButtonClearFilter_Click;
            // 
            // TbZoom
            // 
            TbZoom.AutoSize = false;
            TbZoom.Location = new System.Drawing.Point(453, 3);
            TbZoom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TbZoom.Maximum = 30;
            TbZoom.Minimum = 5;
            TbZoom.Name = "TbZoom";
            TbZoom.Size = new System.Drawing.Size(259, 32);
            TbZoom.TabIndex = 7;
            TbZoom.TickFrequency = 5;
            TbZoom.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            TbZoom.Value = 10;
            TbZoom.Scroll += TbZoom_Scroll;
            // 
            // PbKeyExplanations
            // 
            PbKeyExplanations.Location = new System.Drawing.Point(769, 208);
            PbKeyExplanations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PbKeyExplanations.Name = "PbKeyExplanations";
            PbKeyExplanations.Size = new System.Drawing.Size(188, 317);
            PbKeyExplanations.TabIndex = 6;
            PbKeyExplanations.TabStop = false;
            // 
            // statSelector1
            // 
            statSelector1.AutoSize = true;
            statSelector1.Location = new System.Drawing.Point(54, 7);
            statSelector1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            statSelector1.Name = "statSelector1";
            statSelector1.Size = new System.Drawing.Size(202, 36);
            statSelector1.TabIndex = 5;
            // 
            // LbCreatureName
            // 
            LbCreatureName.Location = new System.Drawing.Point(460, 189);
            LbCreatureName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbCreatureName.Name = "LbCreatureName";
            LbCreatureName.Size = new System.Drawing.Size(299, 15);
            LbCreatureName.TabIndex = 4;
            LbCreatureName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nudGenerations
            // 
            nudGenerations.ForeColor = System.Drawing.Color.Black;
            nudGenerations.Location = new System.Drawing.Point(4, 13);
            nudGenerations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nudGenerations.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            nudGenerations.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            nudGenerations.Name = "nudGenerations";
            nudGenerations.Size = new System.Drawing.Size(43, 23);
            nudGenerations.TabIndex = 3;
            nudGenerations.Value = new decimal(new int[] { 2, 0, 0, 0 });
            nudGenerations.ValueChanged += nudGenerations_ValueChanged;
            // 
            // PedigreeControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(splitContainer1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "PedigreeControl";
            Size = new System.Drawing.Size(1217, 600);
            ((System.ComponentModel.ISupportInitialize)PbRegionColors).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TbZoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)PbKeyExplanations).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGenerations).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PbRegionColors;
        private System.Windows.Forms.Label lbPedigreeEmpty;
        private System.Windows.Forms.ListView listViewCreatures;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox TextBoxFilter;
        private System.Windows.Forms.Button ButtonClearFilter;
        private Nud nudGenerations;
        private System.Windows.Forms.Label LbCreatureName;
        private StatSelector statSelector1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton RbViewH;
        private System.Windows.Forms.RadioButton RbViewCompact;
        private System.Windows.Forms.RadioButton RbViewClassic;
        private System.Windows.Forms.PictureBox PbKeyExplanations;
        private System.Windows.Forms.TrackBar TbZoom;
    }
}

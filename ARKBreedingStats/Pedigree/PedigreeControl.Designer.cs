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
            this.PbRegionColors = new System.Windows.Forms.PictureBox();
            this.lbPedigreeEmpty = new System.Windows.Forms.Label();
            this.listViewCreatures = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RbViewH = new System.Windows.Forms.RadioButton();
            this.RbViewCompact = new System.Windows.Forms.RadioButton();
            this.RbViewClassic = new System.Windows.Forms.RadioButton();
            this.TextBoxFilter = new System.Windows.Forms.TextBox();
            this.ButtonClearFilter = new System.Windows.Forms.Button();
            this.TbZoom = new System.Windows.Forms.TrackBar();
            this.PbKeyExplanations = new System.Windows.Forms.PictureBox();
            this.statSelector1 = new ARKBreedingStats.uiControls.StatSelector();
            this.LbCreatureName = new System.Windows.Forms.Label();
            this.nudGenerations = new ARKBreedingStats.uiControls.Nud();
            ((System.ComponentModel.ISupportInitialize)(this.PbRegionColors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TbZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PbKeyExplanations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGenerations)).BeginInit();
            this.SuspendLayout();
            // 
            // PbRegionColors
            // 
            this.PbRegionColors.Location = new System.Drawing.Point(394, 180);
            this.PbRegionColors.Name = "PbRegionColors";
            this.PbRegionColors.Size = new System.Drawing.Size(256, 256);
            this.PbRegionColors.TabIndex = 0;
            this.PbRegionColors.TabStop = false;
            this.PbRegionColors.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // lbPedigreeEmpty
            // 
            this.lbPedigreeEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPedigreeEmpty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPedigreeEmpty.Location = new System.Drawing.Point(0, 0);
            this.lbPedigreeEmpty.Name = "lbPedigreeEmpty";
            this.lbPedigreeEmpty.Size = new System.Drawing.Size(836, 520);
            this.lbPedigreeEmpty.TabIndex = 1;
            this.lbPedigreeEmpty.Text = "Select a creature in the Library to see its pedigree here.";
            this.lbPedigreeEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listViewCreatures
            // 
            this.listViewCreatures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.tableLayoutPanel1.SetColumnSpan(this.listViewCreatures, 2);
            this.listViewCreatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCreatures.FullRowSelect = true;
            this.listViewCreatures.HideSelection = false;
            this.listViewCreatures.Location = new System.Drawing.Point(3, 60);
            this.listViewCreatures.MultiSelect = false;
            this.listViewCreatures.Name = "listViewCreatures";
            this.listViewCreatures.Size = new System.Drawing.Size(197, 457);
            this.listViewCreatures.TabIndex = 3;
            this.listViewCreatures.UseCompatibleStateImageBehavior = false;
            this.listViewCreatures.View = System.Windows.Forms.View.Details;
            this.listViewCreatures.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewCreatures_ColumnClick);
            this.listViewCreatures.SelectedIndexChanged += new System.EventHandler(this.listViewCreatures_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Lvl";
            this.columnHeader2.Width = 31;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.lbPedigreeEmpty);
            this.splitContainer1.Panel2.Controls.Add(this.TbZoom);
            this.splitContainer1.Panel2.Controls.Add(this.PbKeyExplanations);
            this.splitContainer1.Panel2.Controls.Add(this.statSelector1);
            this.splitContainer1.Panel2.Controls.Add(this.LbCreatureName);
            this.splitContainer1.Panel2.Controls.Add(this.nudGenerations);
            this.splitContainer1.Panel2.Controls.Add(this.PbRegionColors);
            this.splitContainer1.Size = new System.Drawing.Size(1043, 520);
            this.splitContainer1.SplitterDistance = 203;
            this.splitContainer1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listViewCreatures, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.TextBoxFilter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ButtonClearFilter, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(203, 520);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.RbViewH);
            this.panel1.Controls.Add(this.RbViewCompact);
            this.panel1.Controls.Add(this.RbViewClassic);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(197, 25);
            this.panel1.TabIndex = 6;
            // 
            // RbViewH
            // 
            this.RbViewH.AutoSize = true;
            this.RbViewH.Location = new System.Drawing.Point(140, 3);
            this.RbViewH.Name = "RbViewH";
            this.RbViewH.Size = new System.Drawing.Size(33, 17);
            this.RbViewH.TabIndex = 2;
            this.RbViewH.TabStop = true;
            this.RbViewH.Text = "H";
            this.RbViewH.UseVisualStyleBackColor = true;
            this.RbViewH.CheckedChanged += new System.EventHandler(this.RbViewH_CheckedChanged);
            // 
            // RbViewCompact
            // 
            this.RbViewCompact.AutoSize = true;
            this.RbViewCompact.Location = new System.Drawing.Point(67, 3);
            this.RbViewCompact.Name = "RbViewCompact";
            this.RbViewCompact.Size = new System.Drawing.Size(67, 17);
            this.RbViewCompact.TabIndex = 1;
            this.RbViewCompact.TabStop = true;
            this.RbViewCompact.Text = "Compact";
            this.RbViewCompact.UseVisualStyleBackColor = true;
            this.RbViewCompact.CheckedChanged += new System.EventHandler(this.RbViewCompact_CheckedChanged);
            // 
            // RbViewClassic
            // 
            this.RbViewClassic.AutoSize = true;
            this.RbViewClassic.Location = new System.Drawing.Point(3, 3);
            this.RbViewClassic.Name = "RbViewClassic";
            this.RbViewClassic.Size = new System.Drawing.Size(58, 17);
            this.RbViewClassic.TabIndex = 0;
            this.RbViewClassic.TabStop = true;
            this.RbViewClassic.Text = "Classic";
            this.RbViewClassic.UseVisualStyleBackColor = true;
            this.RbViewClassic.CheckedChanged += new System.EventHandler(this.RbViewClassic_CheckedChanged);
            // 
            // TextBoxFilter
            // 
            this.TextBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxFilter.Location = new System.Drawing.Point(3, 34);
            this.TextBoxFilter.Name = "TextBoxFilter";
            this.TextBoxFilter.Size = new System.Drawing.Size(174, 20);
            this.TextBoxFilter.TabIndex = 4;
            this.TextBoxFilter.TextChanged += new System.EventHandler(this.TextBoxFilterTextChanged);
            // 
            // ButtonClearFilter
            // 
            this.ButtonClearFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonClearFilter.Location = new System.Drawing.Point(181, 32);
            this.ButtonClearFilter.Margin = new System.Windows.Forms.Padding(1);
            this.ButtonClearFilter.Name = "ButtonClearFilter";
            this.ButtonClearFilter.Size = new System.Drawing.Size(21, 24);
            this.ButtonClearFilter.TabIndex = 5;
            this.ButtonClearFilter.Text = "×";
            this.ButtonClearFilter.UseVisualStyleBackColor = true;
            this.ButtonClearFilter.Click += new System.EventHandler(this.ButtonClearFilter_Click);
            // 
            // TbZoom
            // 
            this.TbZoom.AutoSize = false;
            this.TbZoom.Location = new System.Drawing.Point(388, 3);
            this.TbZoom.Maximum = 30;
            this.TbZoom.Minimum = 5;
            this.TbZoom.Name = "TbZoom";
            this.TbZoom.Size = new System.Drawing.Size(222, 28);
            this.TbZoom.TabIndex = 7;
            this.TbZoom.TickFrequency = 5;
            this.TbZoom.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.TbZoom.Value = 10;
            this.TbZoom.Scroll += new System.EventHandler(this.TbZoom_Scroll);
            // 
            // PbKeyExplanations
            // 
            this.PbKeyExplanations.Location = new System.Drawing.Point(659, 180);
            this.PbKeyExplanations.Name = "PbKeyExplanations";
            this.PbKeyExplanations.Size = new System.Drawing.Size(161, 275);
            this.PbKeyExplanations.TabIndex = 6;
            this.PbKeyExplanations.TabStop = false;
            // 
            // statSelector1
            // 
            this.statSelector1.AutoSize = true;
            this.statSelector1.Location = new System.Drawing.Point(46, 6);
            this.statSelector1.Name = "statSelector1";
            this.statSelector1.Size = new System.Drawing.Size(173, 29);
            this.statSelector1.TabIndex = 5;
            // 
            // LbCreatureName
            // 
            this.LbCreatureName.Location = new System.Drawing.Point(394, 164);
            this.LbCreatureName.Name = "LbCreatureName";
            this.LbCreatureName.Size = new System.Drawing.Size(256, 13);
            this.LbCreatureName.TabIndex = 4;
            this.LbCreatureName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nudGenerations
            // 
            this.nudGenerations.ForeColor = System.Drawing.SystemColors.WindowText;
            this.nudGenerations.Location = new System.Drawing.Point(3, 11);
            this.nudGenerations.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.nudGenerations.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudGenerations.Name = "nudGenerations";
            this.nudGenerations.NeutralNumber = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudGenerations.Size = new System.Drawing.Size(37, 20);
            this.nudGenerations.TabIndex = 3;
            this.nudGenerations.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudGenerations.ValueChanged += new System.EventHandler(this.nudGenerations_ValueChanged);
            // 
            // PedigreeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PedigreeControl";
            this.Size = new System.Drawing.Size(1043, 520);
            ((System.ComponentModel.ISupportInitialize)(this.PbRegionColors)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TbZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PbKeyExplanations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGenerations)).EndInit();
            this.ResumeLayout(false);

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

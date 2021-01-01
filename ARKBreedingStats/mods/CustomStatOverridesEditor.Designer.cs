namespace ARKBreedingStats.mods
{
    partial class CustomStatOverridesEditor
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
            this.lvSpecies = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbFilterSpecies = new System.Windows.Forms.TextBox();
            this.btClearFilter = new System.Windows.Forms.Button();
            this.cbOnlyDisplayOverriddenSpecies = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelOverrideEdits = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btSaveOverride = new System.Windows.Forms.Button();
            this.btRemoveOverride = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadOverrideFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addOverrideFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportOverrideFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanelOverrideEdits.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvSpecies
            // 
            this.lvSpecies.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvSpecies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSpecies.FullRowSelect = true;
            this.lvSpecies.HideSelection = false;
            this.lvSpecies.Location = new System.Drawing.Point(3, 48);
            this.lvSpecies.MultiSelect = false;
            this.lvSpecies.Name = "lvSpecies";
            this.lvSpecies.Size = new System.Drawing.Size(604, 655);
            this.lvSpecies.TabIndex = 0;
            this.lvSpecies.UseCompatibleStateImageBehavior = false;
            this.lvSpecies.View = System.Windows.Forms.View.Details;
            this.lvSpecies.SelectedIndexChanged += new System.EventHandler(this.lvSpecies_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Species";
            this.columnHeader1.Width = 166;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Blueprint path";
            this.columnHeader2.Width = 364;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvSpecies);
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(610, 706);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Species";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.tbFilterSpecies);
            this.flowLayoutPanel1.Controls.Add(this.btClearFilter);
            this.flowLayoutPanel1.Controls.Add(this.cbOnlyDisplayOverriddenSpecies);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(604, 32);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // tbFilterSpecies
            // 
            this.tbFilterSpecies.Location = new System.Drawing.Point(3, 3);
            this.tbFilterSpecies.Name = "tbFilterSpecies";
            this.tbFilterSpecies.Size = new System.Drawing.Size(376, 20);
            this.tbFilterSpecies.TabIndex = 0;
            this.tbFilterSpecies.TextChanged += new System.EventHandler(this.tbFilterSpecies_TextChanged);
            // 
            // btClearFilter
            // 
            this.btClearFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btClearFilter.Location = new System.Drawing.Point(385, 3);
            this.btClearFilter.Name = "btClearFilter";
            this.btClearFilter.Size = new System.Drawing.Size(24, 23);
            this.btClearFilter.TabIndex = 2;
            this.btClearFilter.Text = "×";
            this.btClearFilter.UseVisualStyleBackColor = true;
            this.btClearFilter.Click += new System.EventHandler(this.btClearFilter_Click);
            // 
            // cbOnlyDisplayOverriddenSpecies
            // 
            this.cbOnlyDisplayOverriddenSpecies.AutoSize = true;
            this.cbOnlyDisplayOverriddenSpecies.Location = new System.Drawing.Point(415, 3);
            this.cbOnlyDisplayOverriddenSpecies.Name = "cbOnlyDisplayOverriddenSpecies";
            this.cbOnlyDisplayOverriddenSpecies.Size = new System.Drawing.Size(164, 17);
            this.cbOnlyDisplayOverriddenSpecies.TabIndex = 1;
            this.cbOnlyDisplayOverriddenSpecies.Text = "Display only adjusted species";
            this.cbOnlyDisplayOverriddenSpecies.UseVisualStyleBackColor = true;
            this.cbOnlyDisplayOverriddenSpecies.CheckedChanged += new System.EventHandler(this.cbOnlyDisplayOverriddenSpecies_CheckedChanged);
            // 
            // groupBox2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox2, 2);
            this.groupBox2.Controls.Add(this.flowLayoutPanelOverrideEdits);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 35);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(812, 692);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Overrides";
            // 
            // flowLayoutPanelOverrideEdits
            // 
            this.flowLayoutPanelOverrideEdits.AutoScroll = true;
            this.flowLayoutPanelOverrideEdits.Controls.Add(this.panel1);
            this.flowLayoutPanelOverrideEdits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelOverrideEdits.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanelOverrideEdits.Name = "flowLayoutPanelOverrideEdits";
            this.flowLayoutPanelOverrideEdits.Size = new System.Drawing.Size(806, 673);
            this.flowLayoutPanelOverrideEdits.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.flowLayoutPanelOverrideEdits.SetFlowBreak(this.panel1, true);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(797, 22);
            this.panel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(655, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "imprinting multiplier";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(536, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "tame affinity";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(436, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "tame add";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(336, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "increase dom";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "increase wild";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "base";
            // 
            // btSaveOverride
            // 
            this.btSaveOverride.Location = new System.Drawing.Point(3, 3);
            this.btSaveOverride.Name = "btSaveOverride";
            this.btSaveOverride.Size = new System.Drawing.Size(154, 23);
            this.btSaveOverride.TabIndex = 3;
            this.btSaveOverride.Text = "Save Override";
            this.btSaveOverride.UseVisualStyleBackColor = true;
            this.btSaveOverride.Click += new System.EventHandler(this.btSaveOverride_Click);
            // 
            // btRemoveOverride
            // 
            this.btRemoveOverride.Location = new System.Drawing.Point(412, 3);
            this.btRemoveOverride.Name = "btRemoveOverride";
            this.btRemoveOverride.Size = new System.Drawing.Size(136, 23);
            this.btRemoveOverride.TabIndex = 4;
            this.btRemoveOverride.Text = "Remove Override";
            this.btRemoveOverride.UseVisualStyleBackColor = true;
            this.btRemoveOverride.Click += new System.EventHandler(this.btRemoveOverride_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btSaveOverride, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btRemoveOverride, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(818, 706);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1432, 706);
            this.splitContainer1.SplitterDistance = 610;
            this.splitContainer1.TabIndex = 7;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadOverrideFileToolStripMenuItem,
            this.addOverrideFileToolStripMenuItem,
            this.exportOverrideFileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1432, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadOverrideFileToolStripMenuItem
            // 
            this.loadOverrideFileToolStripMenuItem.Name = "loadOverrideFileToolStripMenuItem";
            this.loadOverrideFileToolStripMenuItem.Size = new System.Drawing.Size(110, 20);
            this.loadOverrideFileToolStripMenuItem.Text = "Load override file";
            this.loadOverrideFileToolStripMenuItem.Click += new System.EventHandler(this.loadOverrideFileToolStripMenuItem_Click);
            // 
            // addOverrideFileToolStripMenuItem
            // 
            this.addOverrideFileToolStripMenuItem.Name = "addOverrideFileToolStripMenuItem";
            this.addOverrideFileToolStripMenuItem.Size = new System.Drawing.Size(106, 20);
            this.addOverrideFileToolStripMenuItem.Text = "Add override file";
            this.addOverrideFileToolStripMenuItem.Click += new System.EventHandler(this.addOverrideFileToolStripMenuItem_Click);
            // 
            // exportOverrideFileToolStripMenuItem
            // 
            this.exportOverrideFileToolStripMenuItem.Name = "exportOverrideFileToolStripMenuItem";
            this.exportOverrideFileToolStripMenuItem.Size = new System.Drawing.Size(118, 20);
            this.exportOverrideFileToolStripMenuItem.Text = "Export override file";
            this.exportOverrideFileToolStripMenuItem.Click += new System.EventHandler(this.exportOverrideFileToolStripMenuItem_Click);
            // 
            // CustomStatOverridesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1432, 730);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CustomStatOverridesEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Custom Stat Overrides Editor";
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanelOverrideEdits.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvSpecies;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelOverrideEdits;
        private System.Windows.Forms.Button btSaveOverride;
        private System.Windows.Forms.Button btRemoveOverride;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadOverrideFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addOverrideFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportOverrideFileToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox tbFilterSpecies;
        private System.Windows.Forms.Button btClearFilter;
        private System.Windows.Forms.CheckBox cbOnlyDisplayOverriddenSpecies;
        private System.Windows.Forms.Label label6;
    }
}
namespace ARKBreedingStats
{
    partial class Pedigree
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lbPedigreeEmpty = new System.Windows.Forms.Label();
            this.listViewCreatures = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TextBoxFilter = new System.Windows.Forms.TextBox();
            this.ButtonClearFilter = new System.Windows.Forms.Button();
            this.pedigreeCreatureHeaders = new ARKBreedingStats.PedigreeCreature();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(394, 180);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(256, 256);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // lbPedigreeEmpty
            // 
            this.lbPedigreeEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPedigreeEmpty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPedigreeEmpty.Location = new System.Drawing.Point(0, 0);
            this.lbPedigreeEmpty.Name = "lbPedigreeEmpty";
            this.lbPedigreeEmpty.Size = new System.Drawing.Size(881, 520);
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
            this.listViewCreatures.Location = new System.Drawing.Point(3, 29);
            this.listViewCreatures.MultiSelect = false;
            this.listViewCreatures.Name = "listViewCreatures";
            this.listViewCreatures.Size = new System.Drawing.Size(152, 488);
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
            this.splitContainer1.Panel2.Controls.Add(this.pedigreeCreatureHeaders);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox);
            this.splitContainer1.Panel2.Controls.Add(this.lbPedigreeEmpty);
            this.splitContainer1.Size = new System.Drawing.Size(1043, 520);
            this.splitContainer1.SplitterDistance = 158;
            this.splitContainer1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.Controls.Add(this.listViewCreatures, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TextBoxFilter, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ButtonClearFilter, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(158, 520);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // TextBoxFilter
            // 
            this.TextBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxFilter.Location = new System.Drawing.Point(3, 3);
            this.TextBoxFilter.Name = "TextBoxFilter";
            this.TextBoxFilter.Size = new System.Drawing.Size(129, 20);
            this.TextBoxFilter.TabIndex = 4;
            this.TextBoxFilter.TextChanged += new System.EventHandler(this.TextBoxFilterTextChanged);
            // 
            // ButtonClearFilter
            // 
            this.ButtonClearFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonClearFilter.Location = new System.Drawing.Point(136, 1);
            this.ButtonClearFilter.Margin = new System.Windows.Forms.Padding(1);
            this.ButtonClearFilter.Name = "ButtonClearFilter";
            this.ButtonClearFilter.Size = new System.Drawing.Size(21, 24);
            this.ButtonClearFilter.TabIndex = 5;
            this.ButtonClearFilter.Text = "×";
            this.ButtonClearFilter.UseVisualStyleBackColor = true;
            this.ButtonClearFilter.Click += new System.EventHandler(this.ButtonClearFilter_Click);
            // 
            // pedigreeCreatureHeaders
            // 
            this.pedigreeCreatureHeaders.Creature = null;
            this.pedigreeCreatureHeaders.Location = new System.Drawing.Point(375, 19);
            this.pedigreeCreatureHeaders.Name = "pedigreeCreatureHeaders";
            this.pedigreeCreatureHeaders.OnlyLevels = false;
            this.pedigreeCreatureHeaders.Size = new System.Drawing.Size(325, 35);
            this.pedigreeCreatureHeaders.TabIndex = 2;
            this.pedigreeCreatureHeaders.TotalLevelUnknown = false;
            // 
            // Pedigree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Pedigree";
            this.Size = new System.Drawing.Size(1043, 520);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label lbPedigreeEmpty;
        private PedigreeCreature pedigreeCreatureHeaders;
        private System.Windows.Forms.ListView listViewCreatures;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox TextBoxFilter;
        private System.Windows.Forms.Button ButtonClearFilter;
    }
}

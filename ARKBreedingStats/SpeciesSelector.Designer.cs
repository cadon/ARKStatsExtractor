namespace ARKBreedingStats
{
    partial class SpeciesSelector
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
            lvLastSpecies = new System.Windows.Forms.ListView();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            btCancel = new System.Windows.Forms.Button();
            cbDisplayUntameable = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            BtVariantFilter = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            label2 = new System.Windows.Forms.Label();
            lvSpeciesInLibrary = new System.Windows.Forms.ListView();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            lvSpeciesList = new System.Windows.Forms.ListView();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            BtClearLastUsed = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // lvLastSpecies
            // 
            tableLayoutPanel2.SetColumnSpan(lvLastSpecies, 3);
            lvLastSpecies.Dock = System.Windows.Forms.DockStyle.Fill;
            lvLastSpecies.Location = new System.Drawing.Point(4, 98);
            lvLastSpecies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvLastSpecies.Name = "lvLastSpecies";
            lvLastSpecies.Size = new System.Drawing.Size(594, 170);
            lvLastSpecies.TabIndex = 1;
            lvLastSpecies.UseCompatibleStateImageBehavior = false;
            lvLastSpecies.SelectedIndexChanged += lvOftenUsed_SelectedIndexChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tableLayoutPanel3);
            splitContainer1.Size = new System.Drawing.Size(602, 546);
            splitContainer1.SplitterDistance = 271;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 7;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(lvLastSpecies, 0, 3);
            tableLayoutPanel2.Controls.Add(btCancel, 2, 0);
            tableLayoutPanel2.Controls.Add(cbDisplayUntameable, 0, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 2);
            tableLayoutPanel2.Controls.Add(BtVariantFilter, 0, 1);
            tableLayoutPanel2.Controls.Add(button1, 1, 1);
            tableLayoutPanel2.Controls.Add(BtClearLastUsed, 2, 2);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(602, 271);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // btCancel
            // 
            btCancel.Location = new System.Drawing.Point(234, 3);
            btCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btCancel.Name = "btCancel";
            btCancel.Size = new System.Drawing.Size(96, 27);
            btCancel.TabIndex = 6;
            btCancel.Text = "Cancel";
            btCancel.UseVisualStyleBackColor = true;
            btCancel.Click += btCancel_Click;
            // 
            // cbDisplayUntameable
            // 
            cbDisplayUntameable.AutoSize = true;
            tableLayoutPanel2.SetColumnSpan(cbDisplayUntameable, 2);
            cbDisplayUntameable.Location = new System.Drawing.Point(4, 3);
            cbDisplayUntameable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbDisplayUntameable.Name = "cbDisplayUntameable";
            cbDisplayUntameable.Size = new System.Drawing.Size(204, 19);
            cbDisplayUntameable.TabIndex = 7;
            cbDisplayUntameable.Text = "display non-domesticable species";
            cbDisplayUntameable.UseVisualStyleBackColor = true;
            cbDisplayUntameable.CheckedChanged += cbDisplayUntameable_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            tableLayoutPanel2.SetColumnSpan(label1, 2);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(4, 66);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(77, 16);
            label1.TabIndex = 5;
            label1.Text = "Last Used";
            // 
            // BtVariantFilter
            // 
            BtVariantFilter.Location = new System.Drawing.Point(4, 36);
            BtVariantFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtVariantFilter.Name = "BtVariantFilter";
            BtVariantFilter.Size = new System.Drawing.Size(88, 27);
            BtVariantFilter.TabIndex = 8;
            BtVariantFilter.Text = "Variant Filter";
            BtVariantFilter.UseVisualStyleBackColor = true;
            BtVariantFilter.Click += BtVariantFilter_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(100, 36);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(126, 27);
            button1.TabIndex = 9;
            button1.Text = "Variants to default";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel3.Controls.Add(label2, 0, 0);
            tableLayoutPanel3.Controls.Add(lvSpeciesInLibrary, 0, 1);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new System.Drawing.Size(602, 270);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(4, 0);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(71, 16);
            label2.TabIndex = 6;
            label2.Text = "In Library";
            // 
            // lvSpeciesInLibrary
            // 
            lvSpeciesInLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
            lvSpeciesInLibrary.Location = new System.Drawing.Point(4, 26);
            lvSpeciesInLibrary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvSpeciesInLibrary.Name = "lvSpeciesInLibrary";
            lvSpeciesInLibrary.Size = new System.Drawing.Size(594, 241);
            lvSpeciesInLibrary.TabIndex = 4;
            lvSpeciesInLibrary.UseCompatibleStateImageBehavior = false;
            lvSpeciesInLibrary.SelectedIndexChanged += lvSpeciesInLibrary_SelectedIndexChanged;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(lvSpeciesList);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(splitContainer1);
            splitContainer2.Size = new System.Drawing.Size(999, 546);
            splitContainer2.SplitterDistance = 392;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 4;
            // 
            // lvSpeciesList
            // 
            lvSpeciesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
            lvSpeciesList.Dock = System.Windows.Forms.DockStyle.Fill;
            lvSpeciesList.FullRowSelect = true;
            lvSpeciesList.GridLines = true;
            lvSpeciesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvSpeciesList.Location = new System.Drawing.Point(0, 0);
            lvSpeciesList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvSpeciesList.MultiSelect = false;
            lvSpeciesList.Name = "lvSpeciesList";
            lvSpeciesList.ShowItemToolTips = true;
            lvSpeciesList.Size = new System.Drawing.Size(392, 546);
            lvSpeciesList.TabIndex = 0;
            lvSpeciesList.UseCompatibleStateImageBehavior = false;
            lvSpeciesList.View = System.Windows.Forms.View.Details;
            lvSpeciesList.SelectedIndexChanged += lvSpeciesList_SelectedIndexChanged;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Species";
            columnHeader2.Width = 236;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Variant";
            columnHeader3.Width = 108;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Domesticable";
            columnHeader4.Width = 59;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Mod";
            columnHeader5.Width = 74;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "";
            columnHeader1.Width = 204;
            // 
            // BtClearLastUsed
            // 
            BtClearLastUsed.Location = new System.Drawing.Point(233, 69);
            BtClearLastUsed.Name = "BtClearLastUsed";
            BtClearLastUsed.Size = new System.Drawing.Size(176, 23);
            BtClearLastUsed.TabIndex = 10;
            BtClearLastUsed.Text = "Clear last used species";
            BtClearLastUsed.UseVisualStyleBackColor = true;
            BtClearLastUsed.Click += BtClearLastUsed_Click;
            // 
            // SpeciesSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainer2);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "SpeciesSelector";
            Size = new System.Drawing.Size(999, 546);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView lvLastSpecies;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvSpeciesInLibrary;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.CheckBox cbDisplayUntameable;
        private System.Windows.Forms.ListView lvSpeciesList;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button BtVariantFilter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button BtClearLastUsed;
    }
}

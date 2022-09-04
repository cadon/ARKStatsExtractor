namespace ARKBreedingStats.uiControls
{
    partial class LibraryFilter
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.CbMaturationAll = new System.Windows.Forms.CheckBox();
            this.LbMaturation = new System.Windows.Forms.Label();
            this.CbTagsAll = new System.Windows.Forms.CheckBox();
            this.CbServersAll = new System.Windows.Forms.CheckBox();
            this.CbTribesAll = new System.Windows.Forms.CheckBox();
            this.LbStatus = new System.Windows.Forms.Label();
            this.LbTags = new System.Windows.Forms.Label();
            this.LbServers = new System.Windows.Forms.Label();
            this.LbTribes = new System.Windows.Forms.Label();
            this.ClbTags = new System.Windows.Forms.CheckedListBox();
            this.ClbServers = new System.Windows.Forms.CheckedListBox();
            this.ClbTribes = new System.Windows.Forms.CheckedListBox();
            this.ClbOwners = new System.Windows.Forms.CheckedListBox();
            this.LbOwners = new System.Windows.Forms.Label();
            this.CbOwnersAll = new System.Windows.Forms.CheckBox();
            this.FlpStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.LbColors = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.BtColorFilter = new System.Windows.Forms.Button();
            this.BtClearColorFilters = new System.Windows.Forms.Button();
            this.BtClearFlagFilter = new System.Windows.Forms.Button();
            this.ClbMaturationFilters = new System.Windows.Forms.CheckedListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CbUseFilterInTopStatCalculation = new System.Windows.Forms.CheckBox();
            this.BtApply = new System.Windows.Forms.Button();
            this.BtCancel = new System.Windows.Forms.Button();
            this.CbLibraryGroupSpecies = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(874, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel2.Controls.Add(this.CbMaturationAll, 6, 1);
            this.tableLayoutPanel2.Controls.Add(this.LbMaturation, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.CbTagsAll, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.CbServersAll, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.CbTribesAll, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.LbStatus, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.LbTags, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.LbServers, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.LbTribes, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.ClbTags, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.ClbServers, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.ClbTribes, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.ClbOwners, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.LbOwners, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.CbOwnersAll, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.FlpStatus, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.LbColors, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 5, 2);
            this.tableLayoutPanel2.Controls.Add(this.BtClearColorFilters, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.BtClearFlagFilter, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.ClbMaturationFilters, 6, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(870, 408);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // CbMaturationAll
            // 
            this.CbMaturationAll.AutoSize = true;
            this.CbMaturationAll.Location = new System.Drawing.Point(760, 19);
            this.CbMaturationAll.Name = "CbMaturationAll";
            this.CbMaturationAll.Size = new System.Drawing.Size(36, 17);
            this.CbMaturationAll.TabIndex = 21;
            this.CbMaturationAll.Text = "all";
            this.CbMaturationAll.UseVisualStyleBackColor = true;
            this.CbMaturationAll.CheckedChanged += new System.EventHandler(this.CbMaturationAll_CheckedChanged);
            // 
            // LbMaturation
            // 
            this.LbMaturation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbMaturation.AutoSize = true;
            this.LbMaturation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbMaturation.Location = new System.Drawing.Point(778, 3);
            this.LbMaturation.Name = "LbMaturation";
            this.LbMaturation.Size = new System.Drawing.Size(67, 13);
            this.LbMaturation.TabIndex = 19;
            this.LbMaturation.Text = "Maturation";
            // 
            // CbTagsAll
            // 
            this.CbTagsAll.AutoSize = true;
            this.CbTagsAll.Location = new System.Drawing.Point(405, 19);
            this.CbTagsAll.Name = "CbTagsAll";
            this.CbTagsAll.Size = new System.Drawing.Size(36, 17);
            this.CbTagsAll.TabIndex = 12;
            this.CbTagsAll.Text = "all";
            this.CbTagsAll.UseVisualStyleBackColor = true;
            this.CbTagsAll.CheckedChanged += new System.EventHandler(this.CbTagsAll_CheckedChanged);
            // 
            // CbServersAll
            // 
            this.CbServersAll.AutoSize = true;
            this.CbServersAll.Location = new System.Drawing.Point(272, 19);
            this.CbServersAll.Name = "CbServersAll";
            this.CbServersAll.Size = new System.Drawing.Size(36, 17);
            this.CbServersAll.TabIndex = 11;
            this.CbServersAll.Text = "all";
            this.CbServersAll.UseVisualStyleBackColor = true;
            this.CbServersAll.CheckedChanged += new System.EventHandler(this.CbServersAll_CheckedChanged);
            // 
            // CbTribesAll
            // 
            this.CbTribesAll.AutoSize = true;
            this.CbTribesAll.Location = new System.Drawing.Point(139, 19);
            this.CbTribesAll.Name = "CbTribesAll";
            this.CbTribesAll.Size = new System.Drawing.Size(36, 17);
            this.CbTribesAll.TabIndex = 10;
            this.CbTribesAll.Text = "all";
            this.CbTribesAll.UseVisualStyleBackColor = true;
            this.CbTribesAll.CheckedChanged += new System.EventHandler(this.CbTribesAll_CheckedChanged);
            // 
            // LbStatus
            // 
            this.LbStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbStatus.AutoSize = true;
            this.LbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbStatus.Location = new System.Drawing.Point(580, 3);
            this.LbStatus.Name = "LbStatus";
            this.LbStatus.Size = new System.Drawing.Size(19, 13);
            this.LbStatus.TabIndex = 8;
            this.LbStatus.Text = "St";
            // 
            // LbTags
            // 
            this.LbTags.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbTags.AutoSize = true;
            this.LbTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbTags.Location = new System.Drawing.Point(463, 3);
            this.LbTags.Name = "LbTags";
            this.LbTags.Size = new System.Drawing.Size(11, 13);
            this.LbTags.TabIndex = 7;
            this.LbTags.Text = "t";
            // 
            // LbServers
            // 
            this.LbServers.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbServers.AutoSize = true;
            this.LbServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbServers.Location = new System.Drawing.Point(327, 3);
            this.LbServers.Name = "LbServers";
            this.LbServers.Size = new System.Drawing.Size(17, 13);
            this.LbServers.TabIndex = 6;
            this.LbServers.Text = "sr";
            // 
            // LbTribes
            // 
            this.LbTribes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbTribes.AutoSize = true;
            this.LbTribes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbTribes.Location = new System.Drawing.Point(195, 3);
            this.LbTribes.Name = "LbTribes";
            this.LbTribes.Size = new System.Drawing.Size(15, 13);
            this.LbTribes.TabIndex = 5;
            this.LbTribes.Text = "tr";
            // 
            // ClbTags
            // 
            this.ClbTags.CheckOnClick = true;
            this.ClbTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClbTags.FormattingEnabled = true;
            this.ClbTags.Location = new System.Drawing.Point(405, 48);
            this.ClbTags.Name = "ClbTags";
            this.ClbTags.Size = new System.Drawing.Size(127, 354);
            this.ClbTags.TabIndex = 3;
            // 
            // ClbServers
            // 
            this.ClbServers.CheckOnClick = true;
            this.ClbServers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClbServers.FormattingEnabled = true;
            this.ClbServers.Location = new System.Drawing.Point(272, 48);
            this.ClbServers.Name = "ClbServers";
            this.ClbServers.Size = new System.Drawing.Size(127, 354);
            this.ClbServers.TabIndex = 2;
            // 
            // ClbTribes
            // 
            this.ClbTribes.CheckOnClick = true;
            this.ClbTribes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClbTribes.FormattingEnabled = true;
            this.ClbTribes.Location = new System.Drawing.Point(139, 48);
            this.ClbTribes.Name = "ClbTribes";
            this.ClbTribes.Size = new System.Drawing.Size(127, 354);
            this.ClbTribes.TabIndex = 1;
            // 
            // ClbOwners
            // 
            this.ClbOwners.CheckOnClick = true;
            this.ClbOwners.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClbOwners.FormattingEnabled = true;
            this.ClbOwners.Location = new System.Drawing.Point(6, 48);
            this.ClbOwners.Name = "ClbOwners";
            this.ClbOwners.Size = new System.Drawing.Size(127, 354);
            this.ClbOwners.TabIndex = 0;
            // 
            // LbOwners
            // 
            this.LbOwners.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbOwners.AutoSize = true;
            this.LbOwners.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbOwners.Location = new System.Drawing.Point(46, 3);
            this.LbOwners.Name = "LbOwners";
            this.LbOwners.Size = new System.Drawing.Size(47, 13);
            this.LbOwners.TabIndex = 4;
            this.LbOwners.Text = "owners";
            // 
            // CbOwnersAll
            // 
            this.CbOwnersAll.AutoSize = true;
            this.CbOwnersAll.Location = new System.Drawing.Point(6, 19);
            this.CbOwnersAll.Name = "CbOwnersAll";
            this.CbOwnersAll.Size = new System.Drawing.Size(36, 17);
            this.CbOwnersAll.TabIndex = 9;
            this.CbOwnersAll.Text = "all";
            this.CbOwnersAll.UseVisualStyleBackColor = true;
            this.CbOwnersAll.CheckedChanged += new System.EventHandler(this.CbOwnersAll_CheckedChanged);
            // 
            // FlpStatus
            // 
            this.FlpStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FlpStatus.Location = new System.Drawing.Point(538, 48);
            this.FlpStatus.Name = "FlpStatus";
            this.FlpStatus.Size = new System.Drawing.Size(104, 354);
            this.FlpStatus.TabIndex = 14;
            // 
            // LbColors
            // 
            this.LbColors.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LbColors.AutoSize = true;
            this.LbColors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbColors.Location = new System.Drawing.Point(692, 3);
            this.LbColors.Name = "LbColors";
            this.LbColors.Size = new System.Drawing.Size(18, 13);
            this.LbColors.TabIndex = 15;
            this.LbColors.Text = "Cl";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.BtColorFilter);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(648, 48);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(106, 354);
            this.flowLayoutPanel1.TabIndex = 16;
            // 
            // BtColorFilter
            // 
            this.BtColorFilter.Location = new System.Drawing.Point(3, 3);
            this.BtColorFilter.Name = "BtColorFilter";
            this.BtColorFilter.Size = new System.Drawing.Size(99, 133);
            this.BtColorFilter.TabIndex = 0;
            this.BtColorFilter.Text = "color";
            this.BtColorFilter.UseVisualStyleBackColor = true;
            this.BtColorFilter.Click += new System.EventHandler(this.BtColorFilter_Click);
            // 
            // BtClearColorFilters
            // 
            this.BtClearColorFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtClearColorFilters.Location = new System.Drawing.Point(648, 19);
            this.BtClearColorFilters.Name = "BtClearColorFilters";
            this.BtClearColorFilters.Size = new System.Drawing.Size(106, 23);
            this.BtClearColorFilters.TabIndex = 17;
            this.BtClearColorFilters.Text = "ClearColorFilter";
            this.BtClearColorFilters.UseVisualStyleBackColor = true;
            this.BtClearColorFilters.Click += new System.EventHandler(this.BtClearColorFilters_Click);
            // 
            // BtClearFlagFilter
            // 
            this.BtClearFlagFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtClearFlagFilter.Location = new System.Drawing.Point(538, 19);
            this.BtClearFlagFilter.Name = "BtClearFlagFilter";
            this.BtClearFlagFilter.Size = new System.Drawing.Size(104, 23);
            this.BtClearFlagFilter.TabIndex = 18;
            this.BtClearFlagFilter.Text = "Clear";
            this.BtClearFlagFilter.UseVisualStyleBackColor = true;
            this.BtClearFlagFilter.Click += new System.EventHandler(this.BtClearFlagFilter_Click);
            // 
            // ClbMaturationFilters
            // 
            this.ClbMaturationFilters.CheckOnClick = true;
            this.ClbMaturationFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClbMaturationFilters.FormattingEnabled = true;
            this.ClbMaturationFilters.Location = new System.Drawing.Point(760, 48);
            this.ClbMaturationFilters.Name = "ClbMaturationFilters";
            this.ClbMaturationFilters.Size = new System.Drawing.Size(104, 354);
            this.ClbMaturationFilters.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CbLibraryGroupSpecies);
            this.panel1.Controls.Add(this.CbUseFilterInTopStatCalculation);
            this.panel1.Controls.Add(this.BtApply);
            this.panel1.Controls.Add(this.BtCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 415);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(864, 30);
            this.panel1.TabIndex = 4;
            // 
            // CbUseFilterInTopStatCalculation
            // 
            this.CbUseFilterInTopStatCalculation.AutoSize = true;
            this.CbUseFilterInTopStatCalculation.Dock = System.Windows.Forms.DockStyle.Left;
            this.CbUseFilterInTopStatCalculation.Location = new System.Drawing.Point(0, 0);
            this.CbUseFilterInTopStatCalculation.Name = "CbUseFilterInTopStatCalculation";
            this.CbUseFilterInTopStatCalculation.Size = new System.Drawing.Size(170, 30);
            this.CbUseFilterInTopStatCalculation.TabIndex = 2;
            this.CbUseFilterInTopStatCalculation.Text = "Use filter in top stat calculation";
            this.CbUseFilterInTopStatCalculation.UseVisualStyleBackColor = true;
            // 
            // BtApply
            // 
            this.BtApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtApply.Location = new System.Drawing.Point(782, 0);
            this.BtApply.Name = "BtApply";
            this.BtApply.Size = new System.Drawing.Size(75, 23);
            this.BtApply.TabIndex = 0;
            this.BtApply.Text = "Apply";
            this.BtApply.UseVisualStyleBackColor = true;
            this.BtApply.Click += new System.EventHandler(this.BtApply_Click);
            // 
            // BtCancel
            // 
            this.BtCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtCancel.Location = new System.Drawing.Point(701, 0);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(75, 23);
            this.BtCancel.TabIndex = 1;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            // 
            // CbLibraryGroupSpecies
            // 
            this.CbLibraryGroupSpecies.AutoSize = true;
            this.CbLibraryGroupSpecies.Location = new System.Drawing.Point(195, 7);
            this.CbLibraryGroupSpecies.Name = "CbLibraryGroupSpecies";
            this.CbLibraryGroupSpecies.Size = new System.Drawing.Size(138, 17);
            this.CbLibraryGroupSpecies.TabIndex = 3;
            this.CbLibraryGroupSpecies.Text = "Group library by species";
            this.CbLibraryGroupSpecies.UseVisualStyleBackColor = true;
            // 
            // LibraryFilter
            // 
            this.AcceptButton = this.BtApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.CancelButton = this.BtCancel;
            this.ClientSize = new System.Drawing.Size(874, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "LibraryFilter";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button BtApply;
        private System.Windows.Forms.Button BtCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckedListBox ClbOwners;
        private System.Windows.Forms.CheckedListBox ClbTags;
        private System.Windows.Forms.CheckedListBox ClbServers;
        private System.Windows.Forms.CheckedListBox ClbTribes;
        private System.Windows.Forms.CheckBox CbTagsAll;
        private System.Windows.Forms.CheckBox CbServersAll;
        private System.Windows.Forms.CheckBox CbTribesAll;
        private System.Windows.Forms.Label LbStatus;
        private System.Windows.Forms.Label LbTags;
        private System.Windows.Forms.Label LbServers;
        private System.Windows.Forms.Label LbTribes;
        private System.Windows.Forms.Label LbOwners;
        private System.Windows.Forms.CheckBox CbOwnersAll;
        private System.Windows.Forms.FlowLayoutPanel FlpStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox CbUseFilterInTopStatCalculation;
        private System.Windows.Forms.Label LbColors;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button BtColorFilter;
        private System.Windows.Forms.Button BtClearColorFilters;
        private System.Windows.Forms.Button BtClearFlagFilter;
        private System.Windows.Forms.Label LbMaturation;
        private System.Windows.Forms.CheckedListBox ClbMaturationFilters;
        private System.Windows.Forms.CheckBox CbMaturationAll;
        private System.Windows.Forms.CheckBox CbLibraryGroupSpecies;
    }
}
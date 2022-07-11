namespace ARKBreedingStats.mods
{
    partial class ModValuesManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModValuesManager));
            this.lbModList = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btRemoveMod = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbAvailableModFiles = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TbModFilter = new System.Windows.Forms.TextBox();
            this.BtClearFilter = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.LbModVersion = new System.Windows.Forms.Label();
            this.lbAvailableForDownload = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.llbSteamPage = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbModId = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbModTag = new System.Windows.Forms.Label();
            this.lbModName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.LlUnofficialModFiles = new System.Windows.Forms.LinkLabel();
            this.linkLabelCustomModManual = new System.Windows.Forms.LinkLabel();
            this.btOpenValuesFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btMoveDown = new System.Windows.Forms.Button();
            this.btMoveUp = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btAddMod = new System.Windows.Forms.Button();
            this.BtRemoveAllMods = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbModList
            // 
            this.lbModList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbModList.FormattingEnabled = true;
            this.lbModList.Location = new System.Drawing.Point(3, 16);
            this.lbModList.Margin = new System.Windows.Forms.Padding(6);
            this.lbModList.Name = "lbModList";
            this.lbModList.Size = new System.Drawing.Size(221, 485);
            this.lbModList.TabIndex = 0;
            this.lbModList.SelectedIndexChanged += new System.EventHandler(this.LbModList_SelectedIndexChanged);
            this.lbModList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LbModList_MouseDoubleClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btRemoveMod, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btClose, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btAddMod, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtRemoveAllMods, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(908, 510);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btRemoveMod
            // 
            this.btRemoveMod.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btRemoveMod.Location = new System.Drawing.Point(236, 240);
            this.btRemoveMod.Name = "btRemoveMod";
            this.btRemoveMod.Size = new System.Drawing.Size(34, 23);
            this.btRemoveMod.TabIndex = 3;
            this.btRemoveMod.Text = "<";
            this.btRemoveMod.UseVisualStyleBackColor = true;
            this.btRemoveMod.Click += new System.EventHandler(this.BtRemoveMod_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbAvailableModFiles);
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.tableLayoutPanel1.SetRowSpan(this.groupBox3, 3);
            this.groupBox3.Size = new System.Drawing.Size(227, 504);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Available mod files";
            // 
            // lbAvailableModFiles
            // 
            this.lbAvailableModFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbAvailableModFiles.FormattingEnabled = true;
            this.lbAvailableModFiles.Location = new System.Drawing.Point(3, 43);
            this.lbAvailableModFiles.Margin = new System.Windows.Forms.Padding(6);
            this.lbAvailableModFiles.Name = "lbAvailableModFiles";
            this.lbAvailableModFiles.Size = new System.Drawing.Size(221, 458);
            this.lbAvailableModFiles.TabIndex = 0;
            this.lbAvailableModFiles.SelectedIndexChanged += new System.EventHandler(this.LbAvailableModFiles_SelectedIndexChanged);
            this.lbAvailableModFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LbAvailableModFiles_MouseDoubleClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.TbModFilter);
            this.panel2.Controls.Add(this.BtClearFilter);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 16);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(221, 27);
            this.panel2.TabIndex = 10;
            // 
            // TbModFilter
            // 
            this.TbModFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbModFilter.Location = new System.Drawing.Point(3, 3);
            this.TbModFilter.Name = "TbModFilter";
            this.TbModFilter.Size = new System.Drawing.Size(193, 20);
            this.TbModFilter.TabIndex = 10;
            this.TbModFilter.TextChanged += new System.EventHandler(this.TbModFilter_TextChanged);
            // 
            // BtClearFilter
            // 
            this.BtClearFilter.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtClearFilter.Location = new System.Drawing.Point(196, 3);
            this.BtClearFilter.Name = "BtClearFilter";
            this.BtClearFilter.Size = new System.Drawing.Size(22, 21);
            this.BtClearFilter.TabIndex = 11;
            this.BtClearFilter.Text = "×";
            this.BtClearFilter.UseVisualStyleBackColor = true;
            this.BtClearFilter.Click += new System.EventHandler(this.BtClearFilter_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.LbModVersion);
            this.groupBox1.Controls.Add(this.lbAvailableForDownload);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.llbSteamPage);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lbModId);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lbModTag);
            this.groupBox1.Controls.Add(this.lbModName);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(509, 240);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(396, 231);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mod info";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 44);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Mod version";
            // 
            // LbModVersion
            // 
            this.LbModVersion.AutoSize = true;
            this.LbModVersion.Location = new System.Drawing.Point(106, 44);
            this.LbModVersion.Margin = new System.Windows.Forms.Padding(3);
            this.LbModVersion.Name = "LbModVersion";
            this.LbModVersion.Size = new System.Drawing.Size(41, 13);
            this.LbModVersion.TabIndex = 9;
            this.LbModVersion.Text = "version";
            // 
            // lbAvailableForDownload
            // 
            this.lbAvailableForDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbAvailableForDownload.AutoSize = true;
            this.lbAvailableForDownload.Location = new System.Drawing.Point(7, 215);
            this.lbAvailableForDownload.Name = "lbAvailableForDownload";
            this.lbAvailableForDownload.Size = new System.Drawing.Size(223, 13);
            this.lbAvailableForDownload.TabIndex = 7;
            this.lbAvailableForDownload.Text = "(DL) value-file can be downloaded if selected.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Mod-name";
            // 
            // llbSteamPage
            // 
            this.llbSteamPage.AutoSize = true;
            this.llbSteamPage.Location = new System.Drawing.Point(6, 101);
            this.llbSteamPage.Margin = new System.Windows.Forms.Padding(3);
            this.llbSteamPage.Name = "llbSteamPage";
            this.llbSteamPage.Size = new System.Drawing.Size(88, 13);
            this.llbSteamPage.TabIndex = 6;
            this.llbSteamPage.TabStop = true;
            this.llbSteamPage.Text = "Mod Steam page";
            this.llbSteamPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlbSteamPage_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 82);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Mod id";
            // 
            // lbModId
            // 
            this.lbModId.AutoSize = true;
            this.lbModId.Location = new System.Drawing.Point(106, 82);
            this.lbModId.Margin = new System.Windows.Forms.Padding(3);
            this.lbModId.Name = "lbModId";
            this.lbModId.Size = new System.Drawing.Size(15, 13);
            this.lbModId.TabIndex = 5;
            this.lbModId.Text = "id";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 63);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Mod tag";
            // 
            // lbModTag
            // 
            this.lbModTag.AutoSize = true;
            this.lbModTag.Location = new System.Drawing.Point(106, 63);
            this.lbModTag.Margin = new System.Windows.Forms.Padding(3);
            this.lbModTag.Name = "lbModTag";
            this.lbModTag.Size = new System.Drawing.Size(22, 13);
            this.lbModTag.TabIndex = 4;
            this.lbModTag.Text = "tag";
            // 
            // lbModName
            // 
            this.lbModName.AutoSize = true;
            this.lbModName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModName.Location = new System.Drawing.Point(106, 25);
            this.lbModName.Margin = new System.Windows.Forms.Padding(3);
            this.lbModName.Name = "lbModName";
            this.lbModName.Size = new System.Drawing.Size(46, 16);
            this.lbModName.TabIndex = 3;
            this.lbModName.Text = "name";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.LlUnofficialModFiles);
            this.panel1.Controls.Add(this.linkLabelCustomModManual);
            this.panel1.Controls.Add(this.btOpenValuesFolder);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btMoveDown);
            this.panel1.Controls.Add(this.btMoveUp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(509, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 231);
            this.panel1.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(59, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(328, 59);
            this.label6.TabIndex = 7;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // LlUnofficialModFiles
            // 
            this.LlUnofficialModFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LlUnofficialModFiles.AutoSize = true;
            this.LlUnofficialModFiles.Location = new System.Drawing.Point(188, 187);
            this.LlUnofficialModFiles.Name = "LlUnofficialModFiles";
            this.LlUnofficialModFiles.Size = new System.Drawing.Size(104, 13);
            this.LlUnofficialModFiles.TabIndex = 6;
            this.LlUnofficialModFiles.TabStop = true;
            this.LlUnofficialModFiles.Text = "More mod value files";
            this.LlUnofficialModFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlUnofficialModFiles_LinkClicked);
            // 
            // linkLabelCustomModManual
            // 
            this.linkLabelCustomModManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelCustomModManual.AutoSize = true;
            this.linkLabelCustomModManual.Location = new System.Drawing.Point(188, 213);
            this.linkLabelCustomModManual.Name = "linkLabelCustomModManual";
            this.linkLabelCustomModManual.Size = new System.Drawing.Size(166, 13);
            this.linkLabelCustomModManual.TabIndex = 5;
            this.linkLabelCustomModManual.TabStop = true;
            this.linkLabelCustomModManual.Text = "How to manually create a mod file";
            this.linkLabelCustomModManual.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCustomModManual_LinkClicked);
            // 
            // btOpenValuesFolder
            // 
            this.btOpenValuesFolder.Location = new System.Drawing.Point(3, 9);
            this.btOpenValuesFolder.Name = "btOpenValuesFolder";
            this.btOpenValuesFolder.Size = new System.Drawing.Size(190, 23);
            this.btOpenValuesFolder.TabIndex = 0;
            this.btOpenValuesFolder.Text = "Open values folder in explorer";
            this.btOpenValuesFolder.UseVisualStyleBackColor = true;
            this.btOpenValuesFolder.Click += new System.EventHandler(this.BtOpenValuesFolder_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(59, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(328, 40);
            this.label1.TabIndex = 4;
            this.label1.Text = "Mod values are loaded from top to bottom. If multiple mods change the same value," +
    " the value of the mod lower in the list will be used.";
            // 
            // btMoveDown
            // 
            this.btMoveDown.Location = new System.Drawing.Point(3, 96);
            this.btMoveDown.Name = "btMoveDown";
            this.btMoveDown.Size = new System.Drawing.Size(50, 23);
            this.btMoveDown.TabIndex = 3;
            this.btMoveDown.Text = "▼";
            this.btMoveDown.UseVisualStyleBackColor = true;
            this.btMoveDown.Click += new System.EventHandler(this.BtMoveDown_Click);
            // 
            // btMoveUp
            // 
            this.btMoveUp.Location = new System.Drawing.Point(3, 67);
            this.btMoveUp.Name = "btMoveUp";
            this.btMoveUp.Size = new System.Drawing.Size(50, 23);
            this.btMoveUp.TabIndex = 2;
            this.btMoveUp.Text = "▲";
            this.btMoveUp.UseVisualStyleBackColor = true;
            this.btMoveUp.Click += new System.EventHandler(this.BtMoveUp_Click);
            // 
            // btClose
            // 
            this.btClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btClose.Location = new System.Drawing.Point(811, 484);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(94, 23);
            this.btClose.TabIndex = 6;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.BtClose_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbModList);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(276, 3);
            this.groupBox2.Name = "groupBox2";
            this.tableLayoutPanel1.SetRowSpan(this.groupBox2, 3);
            this.groupBox2.Size = new System.Drawing.Size(227, 504);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mods loaded in library";
            // 
            // btAddMod
            // 
            this.btAddMod.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btAddMod.Location = new System.Drawing.Point(236, 211);
            this.btAddMod.Name = "btAddMod";
            this.btAddMod.Size = new System.Drawing.Size(34, 23);
            this.btAddMod.TabIndex = 2;
            this.btAddMod.Text = ">";
            this.btAddMod.UseVisualStyleBackColor = true;
            this.btAddMod.Click += new System.EventHandler(this.BtAddMod_Click);
            // 
            // BtRemoveAllMods
            // 
            this.BtRemoveAllMods.Location = new System.Drawing.Point(236, 477);
            this.BtRemoveAllMods.Name = "BtRemoveAllMods";
            this.BtRemoveAllMods.Size = new System.Drawing.Size(34, 23);
            this.BtRemoveAllMods.TabIndex = 7;
            this.BtRemoveAllMods.Text = "≪";
            this.BtRemoveAllMods.UseVisualStyleBackColor = true;
            this.BtRemoveAllMods.Click += new System.EventHandler(this.BtRemoveAllMods_Click);
            // 
            // ModValuesManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 510);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ModValuesManager";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mod Values Manager";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbModList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btMoveDown;
        private System.Windows.Forms.Button btMoveUp;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.LinkLabel llbSteamPage;
        private System.Windows.Forms.Label lbModId;
        private System.Windows.Forms.Label lbModTag;
        private System.Windows.Forms.Label lbModName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btRemoveMod;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbAvailableModFiles;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btAddMod;
        private System.Windows.Forms.Label lbAvailableForDownload;
        private System.Windows.Forms.Button btOpenValuesFolder;
        private System.Windows.Forms.Button BtRemoveAllMods;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label LbModVersion;
        private System.Windows.Forms.LinkLabel linkLabelCustomModManual;
        private System.Windows.Forms.LinkLabel LlUnofficialModFiles;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox TbModFilter;
        private System.Windows.Forms.Button BtClearFilter;
        private System.Windows.Forms.Label label6;
    }
}
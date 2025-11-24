namespace ARKBreedingStats.SpeciesImages
{
    partial class ImagePackSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImagePackSelection));
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtCancel = new System.Windows.Forms.Button();
            this.BtOk = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LbEnabledPacks = new System.Windows.Forms.ListBox();
            this.LbAvailablePacks = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.BtAdd = new System.Windows.Forms.Button();
            this.BtRemove = new System.Windows.Forms.Button();
            this.BtRemoveAll = new System.Windows.Forms.Button();
            this.BtMoveUp = new System.Windows.Forms.Button();
            this.BtMoveDown = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.LbPackName = new System.Windows.Forms.Label();
            this.LbCreators = new System.Windows.Forms.Label();
            this.LbDescription = new System.Windows.Forms.Label();
            this.TbUrl = new System.Windows.Forms.TextBox();
            this.LLFolder = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LbCustomPackInfo = new System.Windows.Forms.Label();
            this.BtOpenPackPreferenceFile = new System.Windows.Forms.Button();
            this.LlImagePackManual = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtCancel);
            this.panel1.Controls.Add(this.BtOk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 407);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(800, 43);
            this.panel1.TabIndex = 0;
            // 
            // BtCancel
            // 
            this.BtCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtCancel.Location = new System.Drawing.Point(575, 5);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(110, 33);
            this.BtCancel.TabIndex = 1;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            this.BtCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // BtOk
            // 
            this.BtOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtOk.Location = new System.Drawing.Point(685, 5);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(110, 33);
            this.BtOk.TabIndex = 0;
            this.BtOk.Text = "OK";
            this.BtOk.UseVisualStyleBackColor = true;
            this.BtOk.Click += new System.EventHandler(this.BtOk_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LbEnabledPacks, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.LbAvailablePacks, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.LlImagePackManual, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 407);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Enabled image packs";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Available image packs";
            // 
            // LbEnabledPacks
            // 
            this.LbEnabledPacks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbEnabledPacks.FormattingEnabled = true;
            this.LbEnabledPacks.Location = new System.Drawing.Point(172, 36);
            this.LbEnabledPacks.Name = "LbEnabledPacks";
            this.LbEnabledPacks.Size = new System.Drawing.Size(120, 368);
            this.LbEnabledPacks.TabIndex = 6;
            this.LbEnabledPacks.SelectedIndexChanged += new System.EventHandler(this.LbEnabledPacks_SelectedIndexChanged);
            this.LbEnabledPacks.DoubleClick += new System.EventHandler(this.LbEnabledPacks_DoubleClick);
            // 
            // LbAvailablePacks
            // 
            this.LbAvailablePacks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbAvailablePacks.FormattingEnabled = true;
            this.LbAvailablePacks.Location = new System.Drawing.Point(3, 36);
            this.LbAvailablePacks.Name = "LbAvailablePacks";
            this.LbAvailablePacks.Size = new System.Drawing.Size(113, 368);
            this.LbAvailablePacks.TabIndex = 7;
            this.LbAvailablePacks.SelectedIndexChanged += new System.EventHandler(this.LbAvailablePacks_SelectedIndexChanged);
            this.LbAvailablePacks.DoubleClick += new System.EventHandler(this.LbAvailablePacks_DoubleClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.BtAdd, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.BtRemove, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.BtRemoveAll, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.BtMoveUp, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.BtMoveDown, 0, 5);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(122, 36);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(44, 368);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // BtAdd
            // 
            this.BtAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtAdd.Location = new System.Drawing.Point(3, 74);
            this.BtAdd.Name = "BtAdd";
            this.BtAdd.Size = new System.Drawing.Size(38, 39);
            this.BtAdd.TabIndex = 0;
            this.BtAdd.Text = "→";
            this.BtAdd.UseVisualStyleBackColor = true;
            this.BtAdd.Click += new System.EventHandler(this.BtAdd_Click);
            // 
            // BtRemove
            // 
            this.BtRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtRemove.Location = new System.Drawing.Point(3, 119);
            this.BtRemove.Name = "BtRemove";
            this.BtRemove.Size = new System.Drawing.Size(38, 39);
            this.BtRemove.TabIndex = 1;
            this.BtRemove.Text = "←";
            this.BtRemove.UseVisualStyleBackColor = true;
            this.BtRemove.Click += new System.EventHandler(this.BtRemove_Click);
            // 
            // BtRemoveAll
            // 
            this.BtRemoveAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtRemoveAll.Location = new System.Drawing.Point(3, 164);
            this.BtRemoveAll.Name = "BtRemoveAll";
            this.BtRemoveAll.Size = new System.Drawing.Size(38, 39);
            this.BtRemoveAll.TabIndex = 2;
            this.BtRemoveAll.Text = "⇇";
            this.BtRemoveAll.UseVisualStyleBackColor = true;
            this.BtRemoveAll.Click += new System.EventHandler(this.BtRemoveAll_Click);
            // 
            // BtMoveUp
            // 
            this.BtMoveUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtMoveUp.Location = new System.Drawing.Point(3, 209);
            this.BtMoveUp.Name = "BtMoveUp";
            this.BtMoveUp.Size = new System.Drawing.Size(38, 39);
            this.BtMoveUp.TabIndex = 3;
            this.BtMoveUp.Text = "↑";
            this.BtMoveUp.UseVisualStyleBackColor = true;
            this.BtMoveUp.Click += new System.EventHandler(this.BtMoveUp_Click);
            // 
            // BtMoveDown
            // 
            this.BtMoveDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtMoveDown.Location = new System.Drawing.Point(3, 254);
            this.BtMoveDown.Name = "BtMoveDown";
            this.BtMoveDown.Size = new System.Drawing.Size(38, 39);
            this.BtMoveDown.TabIndex = 4;
            this.BtMoveDown.Text = "↓";
            this.BtMoveDown.UseVisualStyleBackColor = true;
            this.BtMoveDown.Click += new System.EventHandler(this.BtMoveDown_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.flowLayoutPanel1);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(298, 36);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(499, 368);
            this.panel2.TabIndex = 10;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.LbPackName);
            this.flowLayoutPanel1.Controls.Add(this.LbCreators);
            this.flowLayoutPanel1.Controls.Add(this.LbDescription);
            this.flowLayoutPanel1.Controls.Add(this.TbUrl);
            this.flowLayoutPanel1.Controls.Add(this.LLFolder);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(499, 202);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label1, true);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(422, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Images are taken from the first pack. If it\'s not available there, the next pack " +
    "is checked.";
            // 
            // LbPackName
            // 
            this.LbPackName.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.LbPackName, true);
            this.LbPackName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbPackName.Location = new System.Drawing.Point(3, 30);
            this.LbPackName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.LbPackName.Name = "LbPackName";
            this.LbPackName.Size = new System.Drawing.Size(53, 20);
            this.LbPackName.TabIndex = 0;
            this.LbPackName.Text = "name";
            // 
            // LbCreators
            // 
            this.LbCreators.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.LbCreators, true);
            this.LbCreators.Location = new System.Drawing.Point(3, 60);
            this.LbCreators.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.LbCreators.Name = "LbCreators";
            this.LbCreators.Size = new System.Drawing.Size(45, 13);
            this.LbCreators.TabIndex = 1;
            this.LbCreators.Text = "creators";
            // 
            // LbDescription
            // 
            this.LbDescription.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.LbDescription, true);
            this.LbDescription.Location = new System.Drawing.Point(3, 83);
            this.LbDescription.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.LbDescription.Name = "LbDescription";
            this.LbDescription.Size = new System.Drawing.Size(58, 13);
            this.LbDescription.TabIndex = 3;
            this.LbDescription.Text = "description";
            // 
            // TbUrl
            // 
            this.TbUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.flowLayoutPanel1.SetFlowBreak(this.TbUrl, true);
            this.TbUrl.Location = new System.Drawing.Point(5, 109);
            this.TbUrl.Margin = new System.Windows.Forms.Padding(5, 3, 3, 10);
            this.TbUrl.Name = "TbUrl";
            this.TbUrl.ReadOnly = true;
            this.TbUrl.Size = new System.Drawing.Size(436, 13);
            this.TbUrl.TabIndex = 9;
            // 
            // LLFolder
            // 
            this.LLFolder.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.LLFolder, true);
            this.LLFolder.Location = new System.Drawing.Point(3, 132);
            this.LLFolder.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.LLFolder.Name = "LLFolder";
            this.LLFolder.Size = new System.Drawing.Size(85, 13);
            this.LLFolder.TabIndex = 5;
            this.LLFolder.TabStop = true;
            this.LLFolder.Text = "open local folder";
            this.LLFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LLFolder_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LbCustomPackInfo);
            this.groupBox1.Controls.Add(this.BtOpenPackPreferenceFile);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(499, 166);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Custom pack preferences";
            // 
            // LbCustomPackInfo
            // 
            this.LbCustomPackInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LbCustomPackInfo.Location = new System.Drawing.Point(3, 45);
            this.LbCustomPackInfo.Name = "LbCustomPackInfo";
            this.LbCustomPackInfo.Size = new System.Drawing.Size(493, 118);
            this.LbCustomPackInfo.TabIndex = 7;
            this.LbCustomPackInfo.Text = resources.GetString("LbCustomPackInfo.Text");
            // 
            // BtOpenPackPreferenceFile
            // 
            this.BtOpenPackPreferenceFile.Location = new System.Drawing.Point(6, 19);
            this.BtOpenPackPreferenceFile.Name = "BtOpenPackPreferenceFile";
            this.BtOpenPackPreferenceFile.Size = new System.Drawing.Size(143, 23);
            this.BtOpenPackPreferenceFile.TabIndex = 6;
            this.BtOpenPackPreferenceFile.Text = "Open pack preference file";
            this.BtOpenPackPreferenceFile.UseVisualStyleBackColor = true;
            this.BtOpenPackPreferenceFile.Click += new System.EventHandler(this.BtOpenPreferenceFile_Click);
            // 
            // LlImagePackManual
            // 
            this.LlImagePackManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LlImagePackManual.AutoSize = true;
            this.LlImagePackManual.Location = new System.Drawing.Point(687, 0);
            this.LlImagePackManual.Name = "LlImagePackManual";
            this.LlImagePackManual.Size = new System.Drawing.Size(110, 13);
            this.LlImagePackManual.TabIndex = 11;
            this.LlImagePackManual.TabStop = true;
            this.LlImagePackManual.Text = "Online documentation";
            this.LlImagePackManual.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlImagePackManual_LinkClicked);
            // 
            // ImagePackSelection
            // 
            this.AcceptButton = this.BtOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtCancel;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ImagePackSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Image Pack Selection";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtCancel;
        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox LbEnabledPacks;
        private System.Windows.Forms.ListBox LbAvailablePacks;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label LbPackName;
        private System.Windows.Forms.Label LbCreators;
        private System.Windows.Forms.Label LbDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button BtAdd;
        private System.Windows.Forms.Button BtRemove;
        private System.Windows.Forms.Button BtRemoveAll;
        private System.Windows.Forms.Button BtMoveUp;
        private System.Windows.Forms.Button BtMoveDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel LLFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LbCustomPackInfo;
        private System.Windows.Forms.Button BtOpenPackPreferenceFile;
        private System.Windows.Forms.TextBox TbUrl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.LinkLabel LlImagePackManual;
    }
}
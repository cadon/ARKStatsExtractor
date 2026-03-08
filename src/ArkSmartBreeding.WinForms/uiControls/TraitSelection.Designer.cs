namespace ARKBreedingStats.uiControls
{
    partial class TraitSelection
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
            this.LbTraitsAvailable = new System.Windows.Forms.ListBox();
            this.LbTraitsAssigned = new System.Windows.Forms.ListBox();
            this.GbTiers = new System.Windows.Forms.GroupBox();
            this.RbTier3 = new System.Windows.Forms.RadioButton();
            this.RbTier2 = new System.Windows.Forms.RadioButton();
            this.RbTier1 = new System.Windows.Forms.RadioButton();
            this.BtAddTrait = new System.Windows.Forms.Button();
            this.BtRemoveTrait = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtCancel = new System.Windows.Forms.Button();
            this.BtOk = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PnTraitDescription = new System.Windows.Forms.Panel();
            this.LbTraitDescription = new System.Windows.Forms.Label();
            this.LbTraitName = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.TbTraitFilter = new System.Windows.Forms.TextBox();
            this.BtClearFilter = new System.Windows.Forms.Button();
            this.BtRemoveAll = new System.Windows.Forms.Button();
            this.GbTiers.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.PnTraitDescription.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // LbTraitsAvailable
            // 
            this.LbTraitsAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbTraitsAvailable.FormattingEnabled = true;
            this.LbTraitsAvailable.Location = new System.Drawing.Point(3, 46);
            this.LbTraitsAvailable.Name = "LbTraitsAvailable";
            this.tableLayoutPanel1.SetRowSpan(this.LbTraitsAvailable, 2);
            this.LbTraitsAvailable.Size = new System.Drawing.Size(206, 446);
            this.LbTraitsAvailable.TabIndex = 0;
            this.LbTraitsAvailable.SelectedIndexChanged += new System.EventHandler(this.LbTraitsAvailable_SelectedIndexChanged);
            this.LbTraitsAvailable.DoubleClick += new System.EventHandler(this.LbTraitsAvailable_DoubleClick);
            // 
            // LbTraitsAssigned
            // 
            this.LbTraitsAssigned.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbTraitsAssigned.FormattingEnabled = true;
            this.LbTraitsAssigned.Location = new System.Drawing.Point(266, 46);
            this.LbTraitsAssigned.Name = "LbTraitsAssigned";
            this.tableLayoutPanel1.SetRowSpan(this.LbTraitsAssigned, 2);
            this.LbTraitsAssigned.Size = new System.Drawing.Size(206, 446);
            this.LbTraitsAssigned.TabIndex = 1;
            this.LbTraitsAssigned.SelectedIndexChanged += new System.EventHandler(this.LbTraitsAssigned_SelectedIndexChanged);
            this.LbTraitsAssigned.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LbTraitsAssigned_MouseDoubleClick);
            // 
            // GbTiers
            // 
            this.GbTiers.Controls.Add(this.RbTier3);
            this.GbTiers.Controls.Add(this.RbTier2);
            this.GbTiers.Controls.Add(this.RbTier1);
            this.GbTiers.Location = new System.Drawing.Point(478, 46);
            this.GbTiers.Name = "GbTiers";
            this.GbTiers.Size = new System.Drawing.Size(55, 94);
            this.GbTiers.TabIndex = 2;
            this.GbTiers.TabStop = false;
            this.GbTiers.Text = "Tier";
            // 
            // RbTier3
            // 
            this.RbTier3.AutoSize = true;
            this.RbTier3.Location = new System.Drawing.Point(6, 65);
            this.RbTier3.Name = "RbTier3";
            this.RbTier3.Size = new System.Drawing.Size(31, 17);
            this.RbTier3.TabIndex = 2;
            this.RbTier3.TabStop = true;
            this.RbTier3.Text = "3";
            this.RbTier3.UseVisualStyleBackColor = true;
            this.RbTier3.Click += new System.EventHandler(this.RbTier3_Click);
            // 
            // RbTier2
            // 
            this.RbTier2.AutoSize = true;
            this.RbTier2.Location = new System.Drawing.Point(6, 42);
            this.RbTier2.Name = "RbTier2";
            this.RbTier2.Size = new System.Drawing.Size(31, 17);
            this.RbTier2.TabIndex = 1;
            this.RbTier2.TabStop = true;
            this.RbTier2.Text = "2";
            this.RbTier2.UseVisualStyleBackColor = true;
            this.RbTier2.Click += new System.EventHandler(this.RbTier2_Click);
            // 
            // RbTier1
            // 
            this.RbTier1.AutoSize = true;
            this.RbTier1.Location = new System.Drawing.Point(6, 19);
            this.RbTier1.Name = "RbTier1";
            this.RbTier1.Size = new System.Drawing.Size(31, 17);
            this.RbTier1.TabIndex = 0;
            this.RbTier1.TabStop = true;
            this.RbTier1.Text = "1";
            this.RbTier1.UseVisualStyleBackColor = true;
            this.RbTier1.Click += new System.EventHandler(this.RbTier1_Click);
            // 
            // BtAddTrait
            // 
            this.BtAddTrait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            this.BtAddTrait.Location = new System.Drawing.Point(3, 77);
            this.BtAddTrait.Name = "BtAddTrait";
            this.BtAddTrait.Size = new System.Drawing.Size(39, 69);
            this.BtAddTrait.TabIndex = 4;
            this.BtAddTrait.Text = "→";
            this.BtAddTrait.UseVisualStyleBackColor = false;
            this.BtAddTrait.Click += new System.EventHandler(this.BtAddTrait_Click);
            // 
            // BtRemoveTrait
            // 
            this.BtRemoveTrait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.BtRemoveTrait.Location = new System.Drawing.Point(3, 152);
            this.BtRemoveTrait.Name = "BtRemoveTrait";
            this.BtRemoveTrait.Size = new System.Drawing.Size(39, 69);
            this.BtRemoveTrait.TabIndex = 5;
            this.BtRemoveTrait.Text = "←";
            this.BtRemoveTrait.UseVisualStyleBackColor = false;
            this.BtRemoveTrait.Click += new System.EventHandler(this.BtRemoveTrait_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtCancel);
            this.panel1.Controls.Add(this.BtOk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 495);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(688, 41);
            this.panel1.TabIndex = 6;
            // 
            // BtCancel
            // 
            this.BtCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtCancel.Location = new System.Drawing.Point(479, 3);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(97, 35);
            this.BtCancel.TabIndex = 1;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            this.BtCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // BtOk
            // 
            this.BtOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtOk.Location = new System.Drawing.Point(576, 3);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(109, 35);
            this.BtOk.TabIndex = 0;
            this.BtOk.Text = "OK";
            this.BtOk.UseVisualStyleBackColor = true;
            this.BtOk.Click += new System.EventHandler(this.BtOk_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.LbTraitsAvailable, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.GbTiers, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.LbTraitsAssigned, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.PnTraitDescription, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(688, 495);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.BtRemoveAll);
            this.panel2.Controls.Add(this.BtAddTrait);
            this.panel2.Controls.Add(this.BtRemoveTrait);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(215, 46);
            this.panel2.Name = "panel2";
            this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
            this.panel2.Size = new System.Drawing.Size(45, 446);
            this.panel2.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Available";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(266, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Assigned";
            // 
            // PnTraitDescription
            // 
            this.PnTraitDescription.Controls.Add(this.LbTraitDescription);
            this.PnTraitDescription.Controls.Add(this.LbTraitName);
            this.PnTraitDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnTraitDescription.Location = new System.Drawing.Point(478, 146);
            this.PnTraitDescription.Name = "PnTraitDescription";
            this.PnTraitDescription.Size = new System.Drawing.Size(207, 346);
            this.PnTraitDescription.TabIndex = 11;
            // 
            // LbTraitDescription
            // 
            this.LbTraitDescription.AutoSize = true;
            this.LbTraitDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.LbTraitDescription.Location = new System.Drawing.Point(0, 13);
            this.LbTraitDescription.Name = "LbTraitDescription";
            this.LbTraitDescription.Size = new System.Drawing.Size(30, 13);
            this.LbTraitDescription.TabIndex = 1;
            this.LbTraitDescription.Text = "desc";
            // 
            // LbTraitName
            // 
            this.LbTraitName.AutoSize = true;
            this.LbTraitName.Dock = System.Windows.Forms.DockStyle.Top;
            this.LbTraitName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LbTraitName.Location = new System.Drawing.Point(0, 0);
            this.LbTraitName.Name = "LbTraitName";
            this.LbTraitName.Size = new System.Drawing.Size(37, 13);
            this.LbTraitName.TabIndex = 0;
            this.LbTraitName.Text = "name";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.TbTraitFilter);
            this.panel3.Controls.Add(this.BtClearFilter);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(206, 24);
            this.panel3.TabIndex = 12;
            // 
            // TbTraitFilter
            // 
            this.TbTraitFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TbTraitFilter.Location = new System.Drawing.Point(0, 0);
            this.TbTraitFilter.Name = "TbTraitFilter";
            this.TbTraitFilter.Size = new System.Drawing.Size(182, 20);
            this.TbTraitFilter.TabIndex = 0;
            this.TbTraitFilter.TextChanged += new System.EventHandler(this.TbTraitFilter_TextChanged);
            // 
            // BtClearFilter
            // 
            this.BtClearFilter.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtClearFilter.Location = new System.Drawing.Point(182, 0);
            this.BtClearFilter.Name = "BtClearFilter";
            this.BtClearFilter.Size = new System.Drawing.Size(24, 24);
            this.BtClearFilter.TabIndex = 1;
            this.BtClearFilter.Text = "×";
            this.BtClearFilter.UseVisualStyleBackColor = true;
            this.BtClearFilter.Click += new System.EventHandler(this.BtClearFilter_Click);
            // 
            // BtRemoveAll
            // 
            this.BtRemoveAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.BtRemoveAll.Location = new System.Drawing.Point(3, 227);
            this.BtRemoveAll.Name = "BtRemoveAll";
            this.BtRemoveAll.Size = new System.Drawing.Size(39, 69);
            this.BtRemoveAll.TabIndex = 6;
            this.BtRemoveAll.Text = "⇇";
            this.BtRemoveAll.UseVisualStyleBackColor = false;
            this.BtRemoveAll.Click += new System.EventHandler(this.BtRemoveAll_Click);
            // 
            // TraitSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 536);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TraitSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Trait Selection";
            this.GbTiers.ResumeLayout(false);
            this.GbTiers.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.PnTraitDescription.ResumeLayout(false);
            this.PnTraitDescription.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox LbTraitsAvailable;
        private System.Windows.Forms.ListBox LbTraitsAssigned;
        private System.Windows.Forms.GroupBox GbTiers;
        private System.Windows.Forms.RadioButton RbTier3;
        private System.Windows.Forms.RadioButton RbTier2;
        private System.Windows.Forms.RadioButton RbTier1;
        private System.Windows.Forms.Button BtAddTrait;
        private System.Windows.Forms.Button BtRemoveTrait;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtCancel;
        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel PnTraitDescription;
        private System.Windows.Forms.Label LbTraitDescription;
        private System.Windows.Forms.Label LbTraitName;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox TbTraitFilter;
        private System.Windows.Forms.Button BtClearFilter;
        private System.Windows.Forms.Button BtRemoveAll;
    }
}
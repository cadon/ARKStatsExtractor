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
            LbTraitsAvailable = new System.Windows.Forms.ListBox();
            LbTraitsAssigned = new System.Windows.Forms.ListBox();
            GbTiers = new System.Windows.Forms.GroupBox();
            RbTier3 = new System.Windows.Forms.RadioButton();
            RbTier2 = new System.Windows.Forms.RadioButton();
            RbTier1 = new System.Windows.Forms.RadioButton();
            BtAddTrait = new System.Windows.Forms.Button();
            BtRemoveTrait = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            BtCancel = new System.Windows.Forms.Button();
            BtOk = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel2 = new System.Windows.Forms.Panel();
            BtRemoveAll = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            PnTraitDescription = new System.Windows.Forms.Panel();
            LbTraitDescription = new System.Windows.Forms.Label();
            LbTraitName = new System.Windows.Forms.Label();
            panel3 = new System.Windows.Forms.Panel();
            TbTraitFilter = new System.Windows.Forms.TextBox();
            BtClearFilter = new System.Windows.Forms.Button();
            GbTiers.SuspendLayout();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            PnTraitDescription.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // LbTraitsAvailable
            // 
            LbTraitsAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            LbTraitsAvailable.FormattingEnabled = true;
            LbTraitsAvailable.Location = new System.Drawing.Point(4, 52);
            LbTraitsAvailable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LbTraitsAvailable.Name = "LbTraitsAvailable";
            tableLayoutPanel1.SetRowSpan(LbTraitsAvailable, 2);
            LbTraitsAvailable.Size = new System.Drawing.Size(239, 516);
            LbTraitsAvailable.TabIndex = 0;
            LbTraitsAvailable.SelectedIndexChanged += LbTraitsAvailable_SelectedIndexChanged;
            LbTraitsAvailable.DoubleClick += LbTraitsAvailable_DoubleClick;
            // 
            // LbTraitsAssigned
            // 
            LbTraitsAssigned.Dock = System.Windows.Forms.DockStyle.Fill;
            LbTraitsAssigned.FormattingEnabled = true;
            LbTraitsAssigned.Location = new System.Drawing.Point(311, 52);
            LbTraitsAssigned.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LbTraitsAssigned.Name = "LbTraitsAssigned";
            tableLayoutPanel1.SetRowSpan(LbTraitsAssigned, 2);
            LbTraitsAssigned.Size = new System.Drawing.Size(240, 516);
            LbTraitsAssigned.TabIndex = 1;
            LbTraitsAssigned.SelectedIndexChanged += LbTraitsAssigned_SelectedIndexChanged;
            LbTraitsAssigned.MouseDoubleClick += LbTraitsAssigned_MouseDoubleClick;
            // 
            // GbTiers
            // 
            GbTiers.Controls.Add(RbTier3);
            GbTiers.Controls.Add(RbTier2);
            GbTiers.Controls.Add(RbTier1);
            GbTiers.Location = new System.Drawing.Point(559, 52);
            GbTiers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GbTiers.Name = "GbTiers";
            GbTiers.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GbTiers.Size = new System.Drawing.Size(64, 108);
            GbTiers.TabIndex = 2;
            GbTiers.TabStop = false;
            GbTiers.Text = "Tier";
            // 
            // RbTier3
            // 
            RbTier3.AutoSize = true;
            RbTier3.Location = new System.Drawing.Point(7, 75);
            RbTier3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbTier3.Name = "RbTier3";
            RbTier3.Size = new System.Drawing.Size(31, 19);
            RbTier3.TabIndex = 2;
            RbTier3.TabStop = true;
            RbTier3.Text = "3";
            RbTier3.UseVisualStyleBackColor = true;
            RbTier3.Click += RbTier3_Click;
            // 
            // RbTier2
            // 
            RbTier2.AutoSize = true;
            RbTier2.Location = new System.Drawing.Point(7, 48);
            RbTier2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbTier2.Name = "RbTier2";
            RbTier2.Size = new System.Drawing.Size(31, 19);
            RbTier2.TabIndex = 1;
            RbTier2.TabStop = true;
            RbTier2.Text = "2";
            RbTier2.UseVisualStyleBackColor = true;
            RbTier2.Click += RbTier2_Click;
            // 
            // RbTier1
            // 
            RbTier1.AutoSize = true;
            RbTier1.Location = new System.Drawing.Point(7, 22);
            RbTier1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RbTier1.Name = "RbTier1";
            RbTier1.Size = new System.Drawing.Size(31, 19);
            RbTier1.TabIndex = 0;
            RbTier1.TabStop = true;
            RbTier1.Text = "1";
            RbTier1.UseVisualStyleBackColor = true;
            RbTier1.Click += RbTier1_Click;
            // 
            // BtAddTrait
            // 
            BtAddTrait.BackColor = System.Drawing.Color.FromArgb(210, 255, 240);
            BtAddTrait.Location = new System.Drawing.Point(4, 89);
            BtAddTrait.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtAddTrait.Name = "BtAddTrait";
            BtAddTrait.Size = new System.Drawing.Size(46, 80);
            BtAddTrait.TabIndex = 4;
            BtAddTrait.Text = "→";
            BtAddTrait.UseVisualStyleBackColor = false;
            BtAddTrait.Click += BtAddTrait_Click;
            // 
            // BtRemoveTrait
            // 
            BtRemoveTrait.BackColor = System.Drawing.Color.FromArgb(255, 225, 225);
            BtRemoveTrait.Location = new System.Drawing.Point(4, 175);
            BtRemoveTrait.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtRemoveTrait.Name = "BtRemoveTrait";
            BtRemoveTrait.Size = new System.Drawing.Size(46, 80);
            BtRemoveTrait.TabIndex = 5;
            BtRemoveTrait.Text = "←";
            BtRemoveTrait.UseVisualStyleBackColor = false;
            BtRemoveTrait.Click += BtRemoveTrait_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(BtCancel);
            panel1.Controls.Add(BtOk);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 571);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Size = new System.Drawing.Size(804, 47);
            panel1.TabIndex = 6;
            // 
            // BtCancel
            // 
            BtCancel.Dock = System.Windows.Forms.DockStyle.Right;
            BtCancel.Location = new System.Drawing.Point(560, 3);
            BtCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtCancel.Name = "BtCancel";
            BtCancel.Size = new System.Drawing.Size(113, 41);
            BtCancel.TabIndex = 1;
            BtCancel.Text = "Cancel";
            BtCancel.UseVisualStyleBackColor = true;
            BtCancel.Click += BtCancel_Click;
            // 
            // BtOk
            // 
            BtOk.Dock = System.Windows.Forms.DockStyle.Right;
            BtOk.Location = new System.Drawing.Point(673, 3);
            BtOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtOk.Name = "BtOk";
            BtOk.Size = new System.Drawing.Size(127, 41);
            BtOk.TabIndex = 0;
            BtOk.Text = "OK";
            BtOk.UseVisualStyleBackColor = true;
            BtOk.Click += BtOk_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            tableLayoutPanel1.Controls.Add(LbTraitsAvailable, 0, 2);
            tableLayoutPanel1.Controls.Add(GbTiers, 3, 2);
            tableLayoutPanel1.Controls.Add(LbTraitsAssigned, 2, 2);
            tableLayoutPanel1.Controls.Add(panel2, 1, 2);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 2, 0);
            tableLayoutPanel1.Controls.Add(PnTraitDescription, 3, 3);
            tableLayoutPanel1.Controls.Add(panel3, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(804, 571);
            tableLayoutPanel1.TabIndex = 7;
            // 
            // panel2
            // 
            panel2.Controls.Add(BtRemoveAll);
            panel2.Controls.Add(BtAddTrait);
            panel2.Controls.Add(BtRemoveTrait);
            panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(251, 52);
            panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            tableLayoutPanel1.SetRowSpan(panel2, 2);
            panel2.Size = new System.Drawing.Size(52, 516);
            panel2.TabIndex = 8;
            // 
            // BtRemoveAll
            // 
            BtRemoveAll.BackColor = System.Drawing.Color.FromArgb(255, 225, 225);
            BtRemoveAll.Location = new System.Drawing.Point(4, 262);
            BtRemoveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtRemoveAll.Name = "BtRemoveAll";
            BtRemoveAll.Size = new System.Drawing.Size(46, 80);
            BtRemoveAll.TabIndex = 6;
            BtRemoveAll.Text = "⇇";
            BtRemoveAll.UseVisualStyleBackColor = false;
            BtRemoveAll.Click += BtRemoveAll_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(55, 15);
            label1.TabIndex = 9;
            label1.Text = "Available";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(311, 0);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(55, 15);
            label2.TabIndex = 10;
            label2.Text = "Assigned";
            // 
            // PnTraitDescription
            // 
            PnTraitDescription.Controls.Add(LbTraitDescription);
            PnTraitDescription.Controls.Add(LbTraitName);
            PnTraitDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            PnTraitDescription.Location = new System.Drawing.Point(559, 166);
            PnTraitDescription.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PnTraitDescription.Name = "PnTraitDescription";
            PnTraitDescription.Size = new System.Drawing.Size(241, 402);
            PnTraitDescription.TabIndex = 11;
            // 
            // LbTraitDescription
            // 
            LbTraitDescription.AutoSize = true;
            LbTraitDescription.Dock = System.Windows.Forms.DockStyle.Top;
            LbTraitDescription.Location = new System.Drawing.Point(0, 13);
            LbTraitDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbTraitDescription.Name = "LbTraitDescription";
            LbTraitDescription.Size = new System.Drawing.Size(31, 15);
            LbTraitDescription.TabIndex = 1;
            LbTraitDescription.Text = "desc";
            // 
            // LbTraitName
            // 
            LbTraitName.AutoSize = true;
            LbTraitName.Dock = System.Windows.Forms.DockStyle.Top;
            LbTraitName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            LbTraitName.Location = new System.Drawing.Point(0, 0);
            LbTraitName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbTraitName.Name = "LbTraitName";
            LbTraitName.Size = new System.Drawing.Size(37, 13);
            LbTraitName.TabIndex = 0;
            LbTraitName.Text = "name";
            // 
            // panel3
            // 
            panel3.Controls.Add(TbTraitFilter);
            panel3.Controls.Add(BtClearFilter);
            panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            panel3.Location = new System.Drawing.Point(4, 18);
            panel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(239, 28);
            panel3.TabIndex = 12;
            // 
            // TbTraitFilter
            // 
            TbTraitFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            TbTraitFilter.Location = new System.Drawing.Point(0, 0);
            TbTraitFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TbTraitFilter.Name = "TbTraitFilter";
            TbTraitFilter.Size = new System.Drawing.Size(211, 23);
            TbTraitFilter.TabIndex = 0;
            TbTraitFilter.TextChanged += TbTraitFilter_TextChanged;
            // 
            // BtClearFilter
            // 
            BtClearFilter.Dock = System.Windows.Forms.DockStyle.Right;
            BtClearFilter.Location = new System.Drawing.Point(211, 0);
            BtClearFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtClearFilter.Name = "BtClearFilter";
            BtClearFilter.Size = new System.Drawing.Size(28, 28);
            BtClearFilter.TabIndex = 1;
            BtClearFilter.Text = "×";
            BtClearFilter.UseVisualStyleBackColor = true;
            BtClearFilter.Click += BtClearFilter_Click;
            // 
            // TraitSelection
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(804, 618);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(panel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "TraitSelection";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Trait Selection";
            GbTiers.ResumeLayout(false);
            GbTiers.PerformLayout();
            panel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel2.ResumeLayout(false);
            PnTraitDescription.ResumeLayout(false);
            PnTraitDescription.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);

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
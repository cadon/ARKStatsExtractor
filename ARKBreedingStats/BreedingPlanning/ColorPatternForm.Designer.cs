namespace ARKBreedingStats.BreedingPlanning
{
    partial class ColorPatternForm
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
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            BtDelete = new System.Windows.Forms.Button();
            TbPatternName = new System.Windows.Forms.TextBox();
            BtSaveAsNew = new System.Windows.Forms.Button();
            BtSave = new System.Windows.Forms.Button();
            coloredCreatureImageWithPose1 = new ARKBreedingStats.uiControls.ColoredCreatureImageWithPose();
            LbColorPatterns = new System.Windows.Forms.ListBox();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            CbRegion5 = new System.Windows.Forms.CheckBox();
            CbRegion4 = new System.Windows.Forms.CheckBox();
            CbRegion3 = new System.Windows.Forms.CheckBox();
            CbRegion2 = new System.Windows.Forms.CheckBox();
            CbRegion1 = new System.Windows.Forms.CheckBox();
            CbRegion0 = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            CbRegionAll = new System.Windows.Forms.CheckBox();
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            regionColorChooser1 = new ARKBreedingStats.uiControls.RegionColorChooser();
            BtAllRegionColors = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 3);
            tableLayoutPanel1.Controls.Add(coloredCreatureImageWithPose1, 1, 2);
            tableLayoutPanel1.Controls.Add(LbColorPatterns, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 1, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(575, 421);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel3.Controls.Add(BtDelete, 0, 1);
            tableLayoutPanel3.Controls.Add(TbPatternName, 0, 0);
            tableLayoutPanel3.Controls.Add(BtSaveAsNew, 2, 1);
            tableLayoutPanel3.Controls.Add(BtSave, 1, 1);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(159, 356);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel3.Size = new System.Drawing.Size(413, 62);
            tableLayoutPanel3.TabIndex = 4;
            // 
            // BtDelete
            // 
            BtDelete.AutoSize = true;
            BtDelete.Location = new System.Drawing.Point(3, 32);
            BtDelete.Name = "BtDelete";
            BtDelete.Size = new System.Drawing.Size(75, 25);
            BtDelete.TabIndex = 2;
            BtDelete.Text = "Delete";
            BtDelete.UseVisualStyleBackColor = true;
            BtDelete.Click += BtDelete_Click;
            // 
            // TbPatternName
            // 
            tableLayoutPanel3.SetColumnSpan(TbPatternName, 3);
            TbPatternName.Dock = System.Windows.Forms.DockStyle.Fill;
            TbPatternName.Location = new System.Drawing.Point(3, 3);
            TbPatternName.Name = "TbPatternName";
            TbPatternName.Size = new System.Drawing.Size(407, 23);
            TbPatternName.TabIndex = 1;
            // 
            // BtSaveAsNew
            // 
            BtSaveAsNew.AutoSize = true;
            BtSaveAsNew.Location = new System.Drawing.Point(165, 32);
            BtSaveAsNew.Name = "BtSaveAsNew";
            BtSaveAsNew.Size = new System.Drawing.Size(121, 25);
            BtSaveAsNew.TabIndex = 3;
            BtSaveAsNew.Text = "Save as new pattern";
            BtSaveAsNew.UseVisualStyleBackColor = true;
            BtSaveAsNew.Click += BtSaveAsNewPattern_Click;
            // 
            // BtSave
            // 
            BtSave.AutoSize = true;
            BtSave.Location = new System.Drawing.Point(84, 32);
            BtSave.Name = "BtSave";
            BtSave.Size = new System.Drawing.Size(75, 25);
            BtSave.TabIndex = 0;
            BtSave.Text = "Save";
            BtSave.UseVisualStyleBackColor = true;
            BtSave.Click += BtSave_Click;
            // 
            // coloredCreatureImageWithPose1
            // 
            coloredCreatureImageWithPose1.Location = new System.Drawing.Point(159, 70);
            coloredCreatureImageWithPose1.Name = "coloredCreatureImageWithPose1";
            coloredCreatureImageWithPose1.Size = new System.Drawing.Size(256, 280);
            coloredCreatureImageWithPose1.TabIndex = 4;
            // 
            // LbColorPatterns
            // 
            LbColorPatterns.Dock = System.Windows.Forms.DockStyle.Fill;
            LbColorPatterns.FormattingEnabled = true;
            LbColorPatterns.Location = new System.Drawing.Point(3, 3);
            LbColorPatterns.Name = "LbColorPatterns";
            tableLayoutPanel1.SetRowSpan(LbColorPatterns, 4);
            LbColorPatterns.Size = new System.Drawing.Size(150, 415);
            LbColorPatterns.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.ColumnCount = 8;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel2.Controls.Add(CbRegion5, 5, 0);
            tableLayoutPanel2.Controls.Add(CbRegion4, 4, 0);
            tableLayoutPanel2.Controls.Add(CbRegion3, 3, 0);
            tableLayoutPanel2.Controls.Add(CbRegion2, 2, 0);
            tableLayoutPanel2.Controls.Add(CbRegion1, 1, 0);
            tableLayoutPanel2.Controls.Add(CbRegion0, 0, 0);
            tableLayoutPanel2.Controls.Add(CbRegionAll, 6, 0);
            tableLayoutPanel2.Controls.Add(label1, 7, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(159, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.Size = new System.Drawing.Size(413, 25);
            tableLayoutPanel2.TabIndex = 7;
            // 
            // CbRegion5
            // 
            CbRegion5.AutoSize = true;
            CbRegion5.Location = new System.Drawing.Point(193, 3);
            CbRegion5.Name = "CbRegion5";
            CbRegion5.Size = new System.Drawing.Size(32, 19);
            CbRegion5.TabIndex = 0;
            CbRegion5.Text = "5";
            CbRegion5.UseVisualStyleBackColor = true;
            // 
            // CbRegion4
            // 
            CbRegion4.AutoSize = true;
            CbRegion4.Location = new System.Drawing.Point(155, 3);
            CbRegion4.Name = "CbRegion4";
            CbRegion4.Size = new System.Drawing.Size(32, 19);
            CbRegion4.TabIndex = 1;
            CbRegion4.Text = "4";
            CbRegion4.UseVisualStyleBackColor = true;
            // 
            // CbRegion3
            // 
            CbRegion3.AutoSize = true;
            CbRegion3.Location = new System.Drawing.Point(117, 3);
            CbRegion3.Name = "CbRegion3";
            CbRegion3.Size = new System.Drawing.Size(32, 19);
            CbRegion3.TabIndex = 2;
            CbRegion3.Text = "3";
            CbRegion3.UseVisualStyleBackColor = true;
            // 
            // CbRegion2
            // 
            CbRegion2.AutoSize = true;
            CbRegion2.Location = new System.Drawing.Point(79, 3);
            CbRegion2.Name = "CbRegion2";
            CbRegion2.Size = new System.Drawing.Size(32, 19);
            CbRegion2.TabIndex = 3;
            CbRegion2.Text = "2";
            CbRegion2.UseVisualStyleBackColor = true;
            // 
            // CbRegion1
            // 
            CbRegion1.AutoSize = true;
            CbRegion1.Location = new System.Drawing.Point(41, 3);
            CbRegion1.Name = "CbRegion1";
            CbRegion1.Size = new System.Drawing.Size(32, 19);
            CbRegion1.TabIndex = 4;
            CbRegion1.Text = "1";
            CbRegion1.UseVisualStyleBackColor = true;
            // 
            // CbRegion0
            // 
            CbRegion0.AutoSize = true;
            CbRegion0.Location = new System.Drawing.Point(3, 3);
            CbRegion0.Name = "CbRegion0";
            CbRegion0.Size = new System.Drawing.Size(32, 19);
            CbRegion0.TabIndex = 5;
            CbRegion0.Text = "0";
            CbRegion0.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(275, 3);
            label1.Margin = new System.Windows.Forms.Padding(3);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(107, 15);
            label1.TabIndex = 6;
            label1.Text = "considered regions";
            // 
            // CbRegionAll
            // 
            CbRegionAll.AutoSize = true;
            CbRegionAll.Location = new System.Drawing.Point(231, 3);
            CbRegionAll.Name = "CbRegionAll";
            CbRegionAll.Size = new System.Drawing.Size(38, 19);
            CbRegionAll.TabIndex = 7;
            CbRegionAll.Text = "all";
            CbRegionAll.UseVisualStyleBackColor = true;
            CbRegionAll.CheckedChanged += CbRegionAll_CheckedChanged;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel4.Controls.Add(regionColorChooser1, 0, 0);
            tableLayoutPanel4.Controls.Add(BtAllRegionColors, 1, 0);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(159, 34);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new System.Drawing.Size(413, 30);
            tableLayoutPanel4.TabIndex = 8;
            // 
            // regionColorChooser1
            // 
            regionColorChooser1.Location = new System.Drawing.Point(0, 0);
            regionColorChooser1.Margin = new System.Windows.Forms.Padding(0);
            regionColorChooser1.Name = "regionColorChooser1";
            regionColorChooser1.Size = new System.Drawing.Size(212, 30);
            regionColorChooser1.TabIndex = 3;
            // 
            // BtAllRegionColors
            // 
            BtAllRegionColors.AutoSize = true;
            BtAllRegionColors.Location = new System.Drawing.Point(215, 3);
            BtAllRegionColors.Name = "BtAllRegionColors";
            BtAllRegionColors.Size = new System.Drawing.Size(71, 24);
            BtAllRegionColors.TabIndex = 4;
            BtAllRegionColors.Text = "all regions";
            BtAllRegionColors.UseVisualStyleBackColor = true;
            BtAllRegionColors.Click += BtAllRegionColors_Click;
            // 
            // ColorPatternForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(575, 421);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "ColorPatternForm";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Color Patterns";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private uiControls.RegionColorChooser regionColorChooser1;
        private uiControls.ColoredCreatureImageWithPose coloredCreatureImageWithPose1;
        private System.Windows.Forms.Button BtSaveAsNew;
        private System.Windows.Forms.Button BtSave;
        private System.Windows.Forms.Button BtDelete;
        private System.Windows.Forms.TextBox TbPatternName;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox LbColorPatterns;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox CbRegion5;
        private System.Windows.Forms.CheckBox CbRegion4;
        private System.Windows.Forms.CheckBox CbRegion3;
        private System.Windows.Forms.CheckBox CbRegion2;
        private System.Windows.Forms.CheckBox CbRegion1;
        private System.Windows.Forms.CheckBox CbRegion0;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox CbRegionAll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button BtAllRegionColors;
    }
}
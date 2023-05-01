namespace ARKBreedingStats.uiControls
{
    partial class StatWeighting
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btAllToOne = new System.Windows.Forms.Button();
            this.BtSavePreset = new System.Windows.Forms.Button();
            this.btSavePresetAs = new System.Windows.Forms.Button();
            this.cbbPresets = new System.Windows.Forms.ComboBox();
            this.btDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.lbStatWeightInfo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 253);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stat Weighting";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelMain, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbStatWeightInfo, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(250, 234);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btAllToOne);
            this.groupBox2.Controls.Add(this.BtSavePreset);
            this.groupBox2.Controls.Add(this.btSavePresetAs);
            this.groupBox2.Controls.Add(this.cbbPresets);
            this.groupBox2.Controls.Add(this.btDelete);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(108, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(139, 210);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Presets";
            // 
            // btAllToOne
            // 
            this.btAllToOne.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btAllToOne.Location = new System.Drawing.Point(3, 48);
            this.btAllToOne.Name = "btAllToOne";
            this.btAllToOne.Size = new System.Drawing.Size(133, 23);
            this.btAllToOne.TabIndex = 18;
            this.btAllToOne.Text = "Reset Weightings";
            this.btAllToOne.UseVisualStyleBackColor = true;
            this.btAllToOne.Click += new System.EventHandler(this.btAllToOne_Click);
            // 
            // BtSavePreset
            // 
            this.BtSavePreset.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BtSavePreset.Location = new System.Drawing.Point(3, 71);
            this.BtSavePreset.Name = "BtSavePreset";
            this.BtSavePreset.Size = new System.Drawing.Size(133, 23);
            this.BtSavePreset.TabIndex = 20;
            this.BtSavePreset.Text = "Save";
            this.BtSavePreset.UseVisualStyleBackColor = true;
            this.BtSavePreset.Click += new System.EventHandler(this.BtSavePreset_Click);
            // 
            // btSavePresetAs
            // 
            this.btSavePresetAs.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btSavePresetAs.Location = new System.Drawing.Point(3, 94);
            this.btSavePresetAs.Name = "btSavePresetAs";
            this.btSavePresetAs.Size = new System.Drawing.Size(133, 23);
            this.btSavePresetAs.TabIndex = 15;
            this.btSavePresetAs.Text = "Save as";
            this.btSavePresetAs.UseVisualStyleBackColor = true;
            this.btSavePresetAs.Click += new System.EventHandler(this.btSavePresetAs_Click);
            // 
            // cbbPresets
            // 
            this.cbbPresets.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbbPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbPresets.FormattingEnabled = true;
            this.cbbPresets.Location = new System.Drawing.Point(3, 16);
            this.cbbPresets.Name = "cbbPresets";
            this.cbbPresets.Size = new System.Drawing.Size(133, 21);
            this.cbbPresets.TabIndex = 16;
            this.cbbPresets.SelectedIndexChanged += new System.EventHandler(this.cbbPresets_SelectedIndexChanged);
            // 
            // btDelete
            // 
            this.btDelete.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btDelete.Location = new System.Drawing.Point(3, 117);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(133, 23);
            this.btDelete.TabIndex = 17;
            this.btDelete.Text = "Remove";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(3, 140);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 67);
            this.label1.TabIndex = 19;
            this.label1.Text = "If a preset named \"Default\" exists it will be applied if no species specific pres" +
    "et is available.";
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 3;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 1;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(99, 210);
            this.tableLayoutPanelMain.TabIndex = 14;
            // 
            // lbStatWeightInfo
            // 
            this.lbStatWeightInfo.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lbStatWeightInfo, 2);
            this.lbStatWeightInfo.Location = new System.Drawing.Point(3, 0);
            this.lbStatWeightInfo.Name = "lbStatWeightInfo";
            this.lbStatWeightInfo.Size = new System.Drawing.Size(177, 13);
            this.lbStatWeightInfo.TabIndex = 15;
            this.lbStatWeightInfo.Text = "Increase the weight of desired stats.";
            // 
            // StatWeighting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "StatWeighting";
            this.Size = new System.Drawing.Size(256, 253);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btSavePresetAs;
        private System.Windows.Forms.ComboBox cbbPresets;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btAllToOne;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbStatWeightInfo;
        private System.Windows.Forms.Button BtSavePreset;
    }
}

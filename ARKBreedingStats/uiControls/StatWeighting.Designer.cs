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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btSavePreset = new System.Windows.Forms.Button();
            this.cbbPresets = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btDelete = new System.Windows.Forms.Button();
            this.btAllToOne = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
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
            this.groupBox1.Size = new System.Drawing.Size(256, 201);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stat Weighting";
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 2;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 1;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(99, 182);
            this.tableLayoutPanelMain.TabIndex = 14;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelMain, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(250, 182);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // btSavePreset
            // 
            this.btSavePreset.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btSavePreset.Location = new System.Drawing.Point(3, 133);
            this.btSavePreset.Name = "btSavePreset";
            this.btSavePreset.Size = new System.Drawing.Size(133, 23);
            this.btSavePreset.TabIndex = 15;
            this.btSavePreset.Text = "Save";
            this.btSavePreset.UseVisualStyleBackColor = true;
            this.btSavePreset.Click += new System.EventHandler(this.btSavePreset_Click);
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btAllToOne);
            this.groupBox2.Controls.Add(this.btSavePreset);
            this.groupBox2.Controls.Add(this.cbbPresets);
            this.groupBox2.Controls.Add(this.btDelete);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(108, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(139, 182);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Presets";
            // 
            // btDelete
            // 
            this.btDelete.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btDelete.Location = new System.Drawing.Point(3, 156);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(133, 23);
            this.btDelete.TabIndex = 17;
            this.btDelete.Text = "Remove";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btAllToOne
            // 
            this.btAllToOne.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btAllToOne.Location = new System.Drawing.Point(3, 110);
            this.btAllToOne.Name = "btAllToOne";
            this.btAllToOne.Size = new System.Drawing.Size(133, 23);
            this.btAllToOne.TabIndex = 18;
            this.btAllToOne.Text = "Set all weights to 1";
            this.btAllToOne.UseVisualStyleBackColor = true;
            this.btAllToOne.Click += new System.EventHandler(this.btAllToOne_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(3, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 61);
            this.label1.TabIndex = 19;
            this.label1.Text = "If a preset named \"Default\" exists it will be applied if no species specific pres" +
    "et is available.";
            // 
            // StatWeighting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "StatWeighting";
            this.Size = new System.Drawing.Size(256, 201);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btSavePreset;
        private System.Windows.Forms.ComboBox cbbPresets;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btAllToOne;
        private System.Windows.Forms.Label label1;
    }
}

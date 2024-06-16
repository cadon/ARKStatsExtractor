namespace ARKBreedingStats.uiControls
{
    partial class StatsOptionsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flpStatControls = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.BtNew = new System.Windows.Forms.Button();
            this.BtRemove = new System.Windows.Forms.Button();
            this.CbbOptions = new System.Windows.Forms.ComboBox();
            this.TbOptionsName = new System.Windows.Forms.TextBox();
            this.LbParent = new System.Windows.Forms.Label();
            this.CbbParent = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpStatControls
            // 
            this.flpStatControls.AutoScroll = true;
            this.flpStatControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpStatControls.Location = new System.Drawing.Point(3, 33);
            this.flpStatControls.Name = "flpStatControls";
            this.flpStatControls.Size = new System.Drawing.Size(929, 476);
            this.flpStatControls.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flpStatControls, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(935, 512);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.BtNew);
            this.flowLayoutPanel1.Controls.Add(this.BtRemove);
            this.flowLayoutPanel1.Controls.Add(this.CbbOptions);
            this.flowLayoutPanel1.Controls.Add(this.TbOptionsName);
            this.flowLayoutPanel1.Controls.Add(this.LbParent);
            this.flowLayoutPanel1.Controls.Add(this.CbbParent);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(929, 24);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // BtNew
            // 
            this.BtNew.Location = new System.Drawing.Point(3, 3);
            this.BtNew.Name = "BtNew";
            this.BtNew.Size = new System.Drawing.Size(20, 20);
            this.BtNew.TabIndex = 4;
            this.BtNew.UseVisualStyleBackColor = true;
            this.BtNew.Click += new System.EventHandler(this.BtNew_Click);
            // 
            // BtRemove
            // 
            this.BtRemove.Location = new System.Drawing.Point(29, 3);
            this.BtRemove.Name = "BtRemove";
            this.BtRemove.Size = new System.Drawing.Size(20, 20);
            this.BtRemove.TabIndex = 5;
            this.BtRemove.UseVisualStyleBackColor = false;
            this.BtRemove.Click += new System.EventHandler(this.BtRemove_Click);
            // 
            // CbbOptions
            // 
            this.CbbOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbbOptions.FormattingEnabled = true;
            this.CbbOptions.Location = new System.Drawing.Point(55, 3);
            this.CbbOptions.Name = "CbbOptions";
            this.CbbOptions.Size = new System.Drawing.Size(251, 21);
            this.CbbOptions.TabIndex = 1;
            this.CbbOptions.SelectedIndexChanged += new System.EventHandler(this.CbbOptions_SelectedIndexChanged);
            // 
            // TbOptionsName
            // 
            this.TbOptionsName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TbOptionsName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TbOptionsName.Location = new System.Drawing.Point(312, 3);
            this.TbOptionsName.Name = "TbOptionsName";
            this.TbOptionsName.Size = new System.Drawing.Size(199, 20);
            this.TbOptionsName.TabIndex = 0;
            this.TbOptionsName.Leave += new System.EventHandler(this.TbOptionsName_Leave);
            // 
            // LbParent
            // 
            this.LbParent.AutoSize = true;
            this.LbParent.Location = new System.Drawing.Point(517, 5);
            this.LbParent.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.LbParent.Name = "LbParent";
            this.LbParent.Size = new System.Drawing.Size(37, 13);
            this.LbParent.TabIndex = 3;
            this.LbParent.Text = "parent";
            // 
            // CbbParent
            // 
            this.CbbParent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CbbParent.FormattingEnabled = true;
            this.CbbParent.Location = new System.Drawing.Point(560, 3);
            this.CbbParent.Name = "CbbParent";
            this.CbbParent.Size = new System.Drawing.Size(257, 21);
            this.CbbParent.TabIndex = 2;
            this.CbbParent.SelectedIndexChanged += new System.EventHandler(this.CbbParent_SelectedIndexChanged);
            // 
            // StatsOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StatsOptionsControl";
            this.Size = new System.Drawing.Size(935, 512);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpStatControls;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox TbOptionsName;
        private System.Windows.Forms.ComboBox CbbOptions;
        private System.Windows.Forms.ComboBox CbbParent;
        private System.Windows.Forms.Label LbParent;
        private System.Windows.Forms.Button BtNew;
        private System.Windows.Forms.Button BtRemove;
    }
}

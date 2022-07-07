namespace ARKBreedingStats.uiControls
{
    partial class MyColorPicker
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
            this.checkBoxOnlyNatural = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtNoColor = new System.Windows.Forms.Button();
            this.LbAlternativeColor = new System.Windows.Forms.Label();
            this.BtUndefinedColor = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxOnlyNatural
            // 
            this.checkBoxOnlyNatural.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxOnlyNatural.AutoSize = true;
            this.checkBoxOnlyNatural.Checked = true;
            this.checkBoxOnlyNatural.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOnlyNatural.Location = new System.Drawing.Point(4, 391);
            this.checkBoxOnlyNatural.Name = "checkBoxOnlyNatural";
            this.checkBoxOnlyNatural.Size = new System.Drawing.Size(154, 17);
            this.checkBoxOnlyNatural.TabIndex = 2;
            this.checkBoxOnlyNatural.Text = "Show only natural occuring";
            this.checkBoxOnlyNatural.UseVisualStyleBackColor = true;
            this.checkBoxOnlyNatural.CheckedChanged += new System.EventHandler(this.checkBoxOnlyNatural_CheckedChanged);
            // 
            // label1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 4);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(440, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "title";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(369, 385);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 31);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(1, 32);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(446, 334);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxOnlyNatural, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.BtNoColor, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.LbAlternativeColor, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.BtUndefinedColor, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(448, 412);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // BtNoColor
            // 
            this.BtNoColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtNoColor.Location = new System.Drawing.Point(287, 385);
            this.BtNoColor.Name = "BtNoColor";
            this.BtNoColor.Size = new System.Drawing.Size(75, 23);
            this.BtNoColor.TabIndex = 4;
            this.BtNoColor.Text = "no color";
            this.BtNoColor.UseVisualStyleBackColor = true;
            this.BtNoColor.Click += new System.EventHandler(this.ColorChosen);
            // 
            // LbAlternativeColor
            // 
            this.LbAlternativeColor.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LbAlternativeColor, 4);
            this.LbAlternativeColor.Location = new System.Drawing.Point(4, 367);
            this.LbAlternativeColor.Name = "LbAlternativeColor";
            this.LbAlternativeColor.Size = new System.Drawing.Size(397, 13);
            this.LbAlternativeColor.TabIndex = 5;
            this.LbAlternativeColor.Text = "Hold Ctrl to select an alternative color id. Hold Ctrl and click on No Color to u" +
    "nset it.";
            // 
            // BtUndefinedColor
            // 
            this.BtUndefinedColor.Location = new System.Drawing.Point(205, 384);
            this.BtUndefinedColor.Name = "BtUndefinedColor";
            this.BtUndefinedColor.Size = new System.Drawing.Size(75, 23);
            this.BtUndefinedColor.TabIndex = 6;
            this.BtUndefinedColor.Text = "undefined";
            this.BtUndefinedColor.UseVisualStyleBackColor = true;
            this.BtUndefinedColor.Click += new System.EventHandler(this.ColorChosen);
            // 
            // MyColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(450, 414);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MyColorPicker";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.MyColorPicker_Load);
            this.Leave += new System.EventHandler(this.MyColorPicker_Leave);
            this.MouseLeave += new System.EventHandler(this.MyColorPicker_MouseLeave);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxOnlyNatural;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button BtNoColor;
        private System.Windows.Forms.Label LbAlternativeColor;
        private System.Windows.Forms.Button BtUndefinedColor;
    }
}
namespace ARKBreedingStats.uiControls
{
    partial class ColorPickerControl
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
            checkBoxOnlyNatural = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            buttonCancel = new System.Windows.Forms.Button();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            BtNoColor = new NoPaddingButton();
            LbAlternativeColor = new System.Windows.Forms.Label();
            BtUndefinedColor = new NoPaddingButton();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // checkBoxOnlyNatural
            // 
            checkBoxOnlyNatural.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            checkBoxOnlyNatural.AutoSize = true;
            checkBoxOnlyNatural.Checked = true;
            checkBoxOnlyNatural.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxOnlyNatural.Location = new System.Drawing.Point(4, 454);
            checkBoxOnlyNatural.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkBoxOnlyNatural.Name = "checkBoxOnlyNatural";
            checkBoxOnlyNatural.Size = new System.Drawing.Size(171, 19);
            checkBoxOnlyNatural.TabIndex = 2;
            checkBoxOnlyNatural.Text = "Show only natural occuring";
            checkBoxOnlyNatural.UseVisualStyleBackColor = true;
            checkBoxOnlyNatural.CheckedChanged += checkBoxOnlyNatural_CheckedChanged;
            // 
            // label1
            // 
            tableLayoutPanel1.SetColumnSpan(label1, 4);
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(4, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(535, 35);
            label1.TabIndex = 1;
            label1.Text = "title";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(451, 446);
            buttonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 27);
            buttonCancel.TabIndex = 0;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancelClick;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 31);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 35);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            flowLayoutPanel1.Size = new System.Drawing.Size(543, 391);
            flowLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 1);
            tableLayoutPanel1.Controls.Add(checkBoxOnlyNatural, 0, 3);
            tableLayoutPanel1.Controls.Add(BtNoColor, 2, 3);
            tableLayoutPanel1.Controls.Add(buttonCancel, 3, 3);
            tableLayoutPanel1.Controls.Add(LbAlternativeColor, 0, 2);
            tableLayoutPanel1.Controls.Add(BtUndefinedColor, 1, 3);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            tableLayoutPanel1.Size = new System.Drawing.Size(543, 476);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // BtNoColor
            // 
            BtNoColor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtNoColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtNoColor.Location = new System.Drawing.Point(355, 446);
            BtNoColor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtNoColor.Name = "BtNoColor";
            BtNoColor.Size = new System.Drawing.Size(88, 27);
            BtNoColor.TabIndex = 4;
            BtNoColor.Text = "no color";
            BtNoColor.UseVisualStyleBackColor = true;
            BtNoColor.Click += ColorChosen;
            // 
            // LbAlternativeColor
            // 
            LbAlternativeColor.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(LbAlternativeColor, 4);
            LbAlternativeColor.Location = new System.Drawing.Point(4, 426);
            LbAlternativeColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LbAlternativeColor.Name = "LbAlternativeColor";
            LbAlternativeColor.Size = new System.Drawing.Size(450, 15);
            LbAlternativeColor.TabIndex = 5;
            LbAlternativeColor.Text = "Hold Ctrl to select an alternative color id. Hold Ctrl and click on No Color to unset it.";
            // 
            // BtUndefinedColor
            // 
            BtUndefinedColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtUndefinedColor.Location = new System.Drawing.Point(259, 444);
            BtUndefinedColor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtUndefinedColor.Name = "BtUndefinedColor";
            BtUndefinedColor.Size = new System.Drawing.Size(88, 27);
            BtUndefinedColor.TabIndex = 6;
            BtUndefinedColor.Text = "undefined";
            BtUndefinedColor.UseVisualStyleBackColor = true;
            BtUndefinedColor.Click += ColorChosen;
            // 
            // ColorPickerControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(tableLayoutPanel1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ColorPickerControl";
            Padding = new System.Windows.Forms.Padding(1);
            Size = new System.Drawing.Size(545, 478);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxOnlyNatural;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private NoPaddingButton BtNoColor;
        private System.Windows.Forms.Label LbAlternativeColor;
        private NoPaddingButton BtUndefinedColor;
    }
}
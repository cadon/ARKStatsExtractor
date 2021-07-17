namespace ARKBreedingStats.uiControls
{
    partial class VariantSelector
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ClbVariants = new System.Windows.Forms.CheckedListBox();
            this.CheckBoxAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ButtonCancel);
            this.panel1.Controls.Add(this.ButtonOk);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 524);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(230, 31);
            this.panel1.TabIndex = 2;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ButtonCancel.Location = new System.Drawing.Point(3, 3);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(100, 25);
            this.ButtonCancel.TabIndex = 1;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.ButtonOk.Location = new System.Drawing.Point(127, 3);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(100, 25);
            this.ButtonOk.TabIndex = 0;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ClbVariants
            // 
            this.ClbVariants.CheckOnClick = true;
            this.ClbVariants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClbVariants.FormattingEnabled = true;
            this.ClbVariants.Location = new System.Drawing.Point(0, 57);
            this.ClbVariants.Name = "ClbVariants";
            this.ClbVariants.Size = new System.Drawing.Size(230, 467);
            this.ClbVariants.TabIndex = 0;
            // 
            // CheckBoxAll
            // 
            this.CheckBoxAll.AutoSize = true;
            this.CheckBoxAll.BackColor = System.Drawing.SystemColors.Window;
            this.CheckBoxAll.Dock = System.Windows.Forms.DockStyle.Top;
            this.CheckBoxAll.Location = new System.Drawing.Point(0, 40);
            this.CheckBoxAll.Name = "CheckBoxAll";
            this.CheckBoxAll.Size = new System.Drawing.Size(230, 17);
            this.CheckBoxAll.TabIndex = 1;
            this.CheckBoxAll.Text = "All";
            this.CheckBoxAll.UseVisualStyleBackColor = false;
            this.CheckBoxAll.CheckedChanged += new System.EventHandler(this.CheckBoxAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(230, 40);
            this.label1.TabIndex = 3;
            this.label1.Text = "Unchecked variants will be hidden.";
            // 
            // VariantSelector
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(230, 555);
            this.Controls.Add(this.ClbVariants);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.CheckBoxAll);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "VariantSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "VariantSelector";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckedListBox ClbVariants;
        private System.Windows.Forms.CheckBox CheckBoxAll;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Label label1;
    }
}
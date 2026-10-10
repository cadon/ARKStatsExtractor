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
            panel1 = new System.Windows.Forms.Panel();
            ButtonCancel = new System.Windows.Forms.Button();
            ButtonOk = new System.Windows.Forms.Button();
            ClbVariants = new System.Windows.Forms.CheckedListBox();
            CheckBoxAll = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(ButtonCancel);
            panel1.Controls.Add(ButtonOk);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(0, 604);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Size = new System.Drawing.Size(269, 36);
            panel1.TabIndex = 2;
            // 
            // ButtonCancel
            // 
            ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            ButtonCancel.Dock = System.Windows.Forms.DockStyle.Left;
            ButtonCancel.Location = new System.Drawing.Point(4, 3);
            ButtonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new System.Drawing.Size(117, 30);
            ButtonCancel.TabIndex = 1;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            ButtonOk.Dock = System.Windows.Forms.DockStyle.Right;
            ButtonOk.Location = new System.Drawing.Point(148, 3);
            ButtonOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonOk.Name = "ButtonOk";
            ButtonOk.Size = new System.Drawing.Size(117, 30);
            ButtonOk.TabIndex = 0;
            ButtonOk.Text = "OK";
            ButtonOk.UseVisualStyleBackColor = true;
            ButtonOk.Click += ButtonOk_Click;
            // 
            // ClbVariants
            // 
            ClbVariants.CheckOnClick = true;
            ClbVariants.Dock = System.Windows.Forms.DockStyle.Fill;
            ClbVariants.FormattingEnabled = true;
            ClbVariants.Location = new System.Drawing.Point(0, 65);
            ClbVariants.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ClbVariants.Name = "ClbVariants";
            ClbVariants.Size = new System.Drawing.Size(269, 539);
            ClbVariants.TabIndex = 0;
            // 
            // CheckBoxAll
            // 
            CheckBoxAll.AutoSize = true;
            CheckBoxAll.BackColor = System.Drawing.SystemColors.Window;
            CheckBoxAll.Dock = System.Windows.Forms.DockStyle.Top;
            CheckBoxAll.Location = new System.Drawing.Point(0, 46);
            CheckBoxAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CheckBoxAll.Name = "CheckBoxAll";
            CheckBoxAll.Size = new System.Drawing.Size(269, 19);
            CheckBoxAll.TabIndex = 1;
            CheckBoxAll.Text = "All";
            CheckBoxAll.UseVisualStyleBackColor = false;
            CheckBoxAll.CheckedChanged += CheckBoxAll_CheckedChanged;
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Top;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            label1.Size = new System.Drawing.Size(269, 46);
            label1.TabIndex = 3;
            label1.Text = "Unchecked variants will be hidden.";
            // 
            // VariantSelector
            // 
            AcceptButton = ButtonOk;
            CancelButton = ButtonCancel;
            ClientSize = new System.Drawing.Size(269, 640);
            Controls.Add(ClbVariants);
            Controls.Add(panel1);
            Controls.Add(CheckBoxAll);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "VariantSelector";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "VariantSelector";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

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

namespace ARKBreedingStats.uiControls
{
    partial class LibraryFilterTemplates
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
            LbStrings = new System.Windows.Forms.ListBox();
            BtRemove = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            BtClose = new System.Windows.Forms.Button();
            BtMoveDown = new System.Windows.Forms.Button();
            BtMoveUp = new System.Windows.Forms.Button();
            CbEdit = new System.Windows.Forms.CheckBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // LbStrings
            // 
            LbStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            LbStrings.FormattingEnabled = true;
            LbStrings.Location = new System.Drawing.Point(4, 3);
            LbStrings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LbStrings.Name = "LbStrings";
            LbStrings.Size = new System.Drawing.Size(287, 334);
            LbStrings.TabIndex = 0;
            LbStrings.SelectedIndexChanged += LbStrings_SelectedIndexChanged;
            // 
            // BtRemove
            // 
            BtRemove.Location = new System.Drawing.Point(77, 5);
            BtRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtRemove.Name = "BtRemove";
            BtRemove.Size = new System.Drawing.Size(42, 27);
            BtRemove.TabIndex = 1;
            BtRemove.Text = "╳";
            BtRemove.UseVisualStyleBackColor = true;
            BtRemove.Click += BtRemove_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(BtClose);
            panel1.Controls.Add(BtMoveDown);
            panel1.Controls.Add(BtMoveUp);
            panel1.Controls.Add(CbEdit);
            panel1.Controls.Add(BtRemove);
            panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel1.Location = new System.Drawing.Point(4, 337);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(287, 33);
            panel1.TabIndex = 2;
            // 
            // BtClose
            // 
            BtClose.BackColor = System.Drawing.Color.Salmon;
            BtClose.Location = new System.Drawing.Point(253, 3);
            BtClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtClose.Name = "BtClose";
            BtClose.Size = new System.Drawing.Size(31, 27);
            BtClose.TabIndex = 5;
            BtClose.Text = "×";
            BtClose.UseVisualStyleBackColor = false;
            BtClose.Click += BtCloseClick;
            // 
            // BtMoveDown
            // 
            BtMoveDown.Location = new System.Drawing.Point(164, 5);
            BtMoveDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtMoveDown.Name = "BtMoveDown";
            BtMoveDown.Size = new System.Drawing.Size(31, 27);
            BtMoveDown.TabIndex = 4;
            BtMoveDown.Text = "▼";
            BtMoveDown.UseVisualStyleBackColor = true;
            BtMoveDown.Click += BtMoveDown_Click;
            // 
            // BtMoveUp
            // 
            BtMoveUp.Location = new System.Drawing.Point(126, 5);
            BtMoveUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtMoveUp.Name = "BtMoveUp";
            BtMoveUp.Size = new System.Drawing.Size(31, 27);
            BtMoveUp.TabIndex = 3;
            BtMoveUp.Text = "▲";
            BtMoveUp.UseVisualStyleBackColor = true;
            BtMoveUp.Click += BtMoveUp_Click;
            // 
            // CbEdit
            // 
            CbEdit.Appearance = System.Windows.Forms.Appearance.Button;
            CbEdit.Location = new System.Drawing.Point(4, 3);
            CbEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CbEdit.Name = "CbEdit";
            CbEdit.Size = new System.Drawing.Size(66, 28);
            CbEdit.TabIndex = 2;
            CbEdit.Text = "✎";
            CbEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            CbEdit.UseVisualStyleBackColor = true;
            CbEdit.CheckedChanged += CbEdit_CheckedChanged;
            // 
            // LibraryFilterTemplates
            // 
            ClientSize = new System.Drawing.Size(295, 373);
            Controls.Add(LbStrings);
            Controls.Add(panel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "LibraryFilterTemplates";
            Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Library Filter Templates";
            panel1.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox LbStrings;
        private System.Windows.Forms.Button BtRemove;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtMoveDown;
        private System.Windows.Forms.Button BtMoveUp;
        private System.Windows.Forms.CheckBox CbEdit;
        private System.Windows.Forms.Button BtClose;
    }
}

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
            this.LbStrings = new System.Windows.Forms.ListBox();
            this.BtRemove = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtClose = new System.Windows.Forms.Button();
            this.BtMoveDown = new System.Windows.Forms.Button();
            this.BtMoveUp = new System.Windows.Forms.Button();
            this.CbEdit = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LbStrings
            // 
            this.LbStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LbStrings.FormattingEnabled = true;
            this.LbStrings.Location = new System.Drawing.Point(3, 3);
            this.LbStrings.Name = "LbStrings";
            this.LbStrings.Size = new System.Drawing.Size(247, 288);
            this.LbStrings.TabIndex = 0;
            this.LbStrings.SelectedIndexChanged += new System.EventHandler(this.LbStrings_SelectedIndexChanged);
            // 
            // BtRemove
            // 
            this.BtRemove.Location = new System.Drawing.Point(66, 4);
            this.BtRemove.Name = "BtRemove";
            this.BtRemove.Size = new System.Drawing.Size(36, 23);
            this.BtRemove.TabIndex = 1;
            this.BtRemove.Text = "╳";
            this.BtRemove.UseVisualStyleBackColor = true;
            this.BtRemove.Click += new System.EventHandler(this.BtRemove_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtClose);
            this.panel1.Controls.Add(this.BtMoveDown);
            this.panel1.Controls.Add(this.BtMoveUp);
            this.panel1.Controls.Add(this.CbEdit);
            this.panel1.Controls.Add(this.BtRemove);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 291);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(247, 29);
            this.panel1.TabIndex = 2;
            // 
            // BtClose
            // 
            this.BtClose.BackColor = System.Drawing.Color.Salmon;
            this.BtClose.Location = new System.Drawing.Point(217, 3);
            this.BtClose.Name = "BtClose";
            this.BtClose.Size = new System.Drawing.Size(27, 23);
            this.BtClose.TabIndex = 5;
            this.BtClose.Text = "×";
            this.BtClose.UseVisualStyleBackColor = false;
            this.BtClose.Click += new System.EventHandler(this.BtCloseClick);
            // 
            // BtMoveDown
            // 
            this.BtMoveDown.Location = new System.Drawing.Point(141, 4);
            this.BtMoveDown.Name = "BtMoveDown";
            this.BtMoveDown.Size = new System.Drawing.Size(27, 23);
            this.BtMoveDown.TabIndex = 4;
            this.BtMoveDown.Text = "▼";
            this.BtMoveDown.UseVisualStyleBackColor = true;
            this.BtMoveDown.Click += new System.EventHandler(this.BtMoveDown_Click);
            // 
            // BtMoveUp
            // 
            this.BtMoveUp.Location = new System.Drawing.Point(108, 4);
            this.BtMoveUp.Name = "BtMoveUp";
            this.BtMoveUp.Size = new System.Drawing.Size(27, 23);
            this.BtMoveUp.TabIndex = 3;
            this.BtMoveUp.Text = "▲";
            this.BtMoveUp.UseVisualStyleBackColor = true;
            this.BtMoveUp.Click += new System.EventHandler(this.BtMoveUp_Click);
            // 
            // CbEdit
            // 
            this.CbEdit.Appearance = System.Windows.Forms.Appearance.Button;
            this.CbEdit.Location = new System.Drawing.Point(3, 3);
            this.CbEdit.Name = "CbEdit";
            this.CbEdit.Size = new System.Drawing.Size(57, 24);
            this.CbEdit.TabIndex = 2;
            this.CbEdit.Text = "✎";
            this.CbEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CbEdit.UseVisualStyleBackColor = true;
            this.CbEdit.CheckedChanged += new System.EventHandler(this.CbEdit_CheckedChanged);
            // 
            // LibraryFilterTemplates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 323);
            this.Controls.Add(this.LbStrings);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LibraryFilterTemplates";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Library Filter Templates";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

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
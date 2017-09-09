namespace ARKBreedingStats
{
    partial class NotesControl
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
            this.listViewNoteTitles = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBoxNote = new System.Windows.Forms.RichTextBox();
            this.tbNoteTitle = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewNoteTitles
            // 
            this.listViewNoteTitles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewNoteTitles.FullRowSelect = true;
            this.listViewNoteTitles.HideSelection = false;
            this.listViewNoteTitles.Location = new System.Drawing.Point(3, 3);
            this.listViewNoteTitles.MultiSelect = false;
            this.listViewNoteTitles.Name = "listViewNoteTitles";
            this.tableLayoutPanel1.SetRowSpan(this.listViewNoteTitles, 2);
            this.listViewNoteTitles.Size = new System.Drawing.Size(194, 368);
            this.listViewNoteTitles.TabIndex = 0;
            this.listViewNoteTitles.UseCompatibleStateImageBehavior = false;
            this.listViewNoteTitles.View = System.Windows.Forms.View.List;
            this.listViewNoteTitles.SelectedIndexChanged += new System.EventHandler(this.listViewNoteTitles_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.listViewNoteTitles, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.richTextBoxNote, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbNoteTitle, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(735, 374);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // richTextBoxNote
            // 
            this.richTextBoxNote.AcceptsTab = true;
            this.richTextBoxNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxNote.Location = new System.Drawing.Point(203, 25);
            this.richTextBoxNote.Name = "richTextBoxNote";
            this.richTextBoxNote.Size = new System.Drawing.Size(529, 346);
            this.richTextBoxNote.TabIndex = 1;
            this.richTextBoxNote.Text = "";
            this.richTextBoxNote.Leave += new System.EventHandler(this.richTextBoxNote_Leave);
            // 
            // tbNoteTitle
            // 
            this.tbNoteTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNoteTitle.Location = new System.Drawing.Point(203, 3);
            this.tbNoteTitle.Name = "tbNoteTitle";
            this.tbNoteTitle.Size = new System.Drawing.Size(529, 20);
            this.tbNoteTitle.TabIndex = 2;
            this.tbNoteTitle.Leave += new System.EventHandler(this.tbNoteTitle_Leave);
            // 
            // NotesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NotesControl";
            this.Size = new System.Drawing.Size(735, 374);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewNoteTitles;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox richTextBoxNote;
        private System.Windows.Forms.TextBox tbNoteTitle;
    }
}

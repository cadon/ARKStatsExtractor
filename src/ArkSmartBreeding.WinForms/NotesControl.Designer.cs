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
            this.BAddNote = new System.Windows.Forms.Button();
            this.BDeleteNote = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewNoteTitles
            // 
            this.listViewNoteTitles.CheckBoxes = true;
            this.tableLayoutPanel1.SetColumnSpan(this.listViewNoteTitles, 2);
            this.listViewNoteTitles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewNoteTitles.FullRowSelect = true;
            this.listViewNoteTitles.HideSelection = false;
            this.listViewNoteTitles.Location = new System.Drawing.Point(3, 31);
            this.listViewNoteTitles.MultiSelect = false;
            this.listViewNoteTitles.Name = "listViewNoteTitles";
            this.listViewNoteTitles.Size = new System.Drawing.Size(194, 320);
            this.listViewNoteTitles.TabIndex = 0;
            this.listViewNoteTitles.UseCompatibleStateImageBehavior = false;
            this.listViewNoteTitles.View = System.Windows.Forms.View.List;
            this.listViewNoteTitles.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewNoteTitles_ItemChecked);
            this.listViewNoteTitles.SelectedIndexChanged += new System.EventHandler(this.listViewNoteTitles_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 535F));
            this.tableLayoutPanel1.Controls.Add(this.listViewNoteTitles, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.richTextBoxNote, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbNoteTitle, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.BAddNote, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BDeleteNote, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(735, 374);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // richTextBoxNote
            // 
            this.richTextBoxNote.AcceptsTab = true;
            this.richTextBoxNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxNote.Location = new System.Drawing.Point(203, 31);
            this.richTextBoxNote.Name = "richTextBoxNote";
            this.tableLayoutPanel1.SetRowSpan(this.richTextBoxNote, 2);
            this.richTextBoxNote.Size = new System.Drawing.Size(529, 340);
            this.richTextBoxNote.TabIndex = 1;
            this.richTextBoxNote.Text = "";
            this.richTextBoxNote.Enter += new System.EventHandler(this.richTextBoxNote_Enter);
            this.richTextBoxNote.Leave += new System.EventHandler(this.richTextBoxNote_Leave);
            // 
            // tbNoteTitle
            // 
            this.tbNoteTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNoteTitle.Location = new System.Drawing.Point(203, 3);
            this.tbNoteTitle.Name = "tbNoteTitle";
            this.tbNoteTitle.Size = new System.Drawing.Size(529, 20);
            this.tbNoteTitle.TabIndex = 2;
            this.tbNoteTitle.Enter += new System.EventHandler(this.tbNoteTitle_Enter);
            this.tbNoteTitle.Leave += new System.EventHandler(this.tbNoteTitle_Leave);
            // 
            // BAddNote
            // 
            this.BAddNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BAddNote.Location = new System.Drawing.Point(3, 3);
            this.BAddNote.Name = "BAddNote";
            this.BAddNote.Size = new System.Drawing.Size(94, 22);
            this.BAddNote.TabIndex = 3;
            this.BAddNote.Text = "New Note";
            this.BAddNote.UseVisualStyleBackColor = true;
            this.BAddNote.Click += new System.EventHandler(this.BAddNote_Click);
            // 
            // BDeleteNote
            // 
            this.BDeleteNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BDeleteNote.Location = new System.Drawing.Point(103, 3);
            this.BDeleteNote.Name = "BDeleteNote";
            this.BDeleteNote.Size = new System.Drawing.Size(94, 22);
            this.BDeleteNote.TabIndex = 4;
            this.BDeleteNote.Text = "Delete Note";
            this.BDeleteNote.UseVisualStyleBackColor = true;
            this.BDeleteNote.Click += new System.EventHandler(this.BDeleteNote_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Location = new System.Drawing.Point(3, 354);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Check a note to display it in the overlay";
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
        private System.Windows.Forms.Button BAddNote;
        private System.Windows.Forms.Button BDeleteNote;
        private System.Windows.Forms.Label label1;
    }
}

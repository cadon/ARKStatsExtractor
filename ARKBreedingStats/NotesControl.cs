using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class NotesControl : UserControl
    {
        private List<Note> noteList;
        private Note selectedNote;
        public event Form1.collectionChangedEventHandler changed;

        public NotesControl()
        {
            InitializeComponent();
        }

        public List<Note> NoteList
        {
            //get { return noteList; }
            set
            {
                listViewNoteTitles.Items.Clear();
                richTextBoxNote.Text = "";
                noteList = value;
                if (value != null)
                {
                    foreach (Note n in value)
                    {
                        ListViewItem lvi = new ListViewItem(n.Title);
                        lvi.Tag = n;
                        listViewNoteTitles.Items.Add(lvi);
                    }
                }
            }
        }

        private void listViewNoteTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewNoteTitles.SelectedIndices.Count > 0)
            {
                selectedNote = (Note)listViewNoteTitles.SelectedItems[0].Tag;
                tbNoteTitle.Text = selectedNote.Title;
                richTextBoxNote.Text = selectedNote.Text;
            }
        }

        public void AddNote()
        {
            Note n = new Note("<new note>");
            noteList.Add(n);
            ListViewItem lvi = new ListViewItem(n.Title);
            lvi.Tag = n;
            listViewNoteTitles.Items.Add(lvi);
            listViewNoteTitles.Items[listViewNoteTitles.Items.Count - 1].Selected = true;
            tbNoteTitle.Focus();
            tbNoteTitle.SelectAll();
        }

        public void RemoveSelectedNote()
        {
            if (listViewNoteTitles.SelectedItems.Count > 0
                && MessageBox.Show("Delete note with the title \"" + ((Note)(listViewNoteTitles.SelectedItems[0].Tag)).Title + "\"?", "Delete Note?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Note n = (Note)listViewNoteTitles.SelectedItems[0].Tag;
                noteList.Remove(n);
                listViewNoteTitles.Items.Remove(listViewNoteTitles.SelectedItems[0]);
                if (listViewNoteTitles.Items.Count > 0)
                    listViewNoteTitles.Items[0].Selected = true;
                else
                {
                    richTextBoxNote.Text = "";
                    tbNoteTitle.Text = "";
                }
                changed?.Invoke();
            }
        }

        private void richTextBoxNote_Leave(object sender, EventArgs e)
        {
            if (selectedNote != null)
            {
                selectedNote.Text = richTextBoxNote.Text;
                changed?.Invoke();
            }
        }

        private void tbNoteTitle_Leave(object sender, EventArgs e)
        {
            if (selectedNote != null)
            {
                if (listViewNoteTitles.SelectedIndices.Count > 0)
                {
                    listViewNoteTitles.SelectedItems[0].Text = tbNoteTitle.Text;
                }
                selectedNote.Title = tbNoteTitle.Text;
                changed?.Invoke();
            }
        }
    }
}

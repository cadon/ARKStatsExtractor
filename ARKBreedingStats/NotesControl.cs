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

        public NotesControl()
        {
            InitializeComponent();
        }

        public List<Note> NoteList
        {
            get { return noteList; }
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
                richTextBoxNote.Text = ((Note)(listViewNoteTitles.SelectedItems[0].Tag)).Text;
            }
        }

        public void AddNote()
        {
            Note n = new Note("<new note>");
            ListViewItem lvi = new ListViewItem(n.Title);
            lvi.Tag = n;
            listViewNoteTitles.Items.Add(lvi);
        }

        public void RemoveSelectedNote()
        {
            if (listViewNoteTitles.SelectedItems.Count > 0
                && MessageBox.Show("Delete note with the title \"" + ((Note)(listViewNoteTitles.SelectedItems[0].Tag)).Title + "\"?", "Delete Note?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                listViewNoteTitles.Items.Remove(listViewNoteTitles.SelectedItems[0]);
                Note n = (Note)listViewNoteTitles.SelectedItems[0].Tag;
                noteList.Remove(n);
            }
        }
    }
}

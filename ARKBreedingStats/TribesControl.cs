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
    public partial class TribesControl : UserControl
    {
        private List<Player> players;
        private List<Tribe> tribes;
        private Player selectedPlayer;
        private ListViewItem selectedRow;
        private Tribe selectedTribe;
        private ListViewItem selectedTribeRow;

        public TribesControl()
        {
            InitializeComponent();
        }

        public List<Player> Players
        {
            set
            {
                players = value;
                updatePlayerList();
            }
        }

        public bool playerExists(string name)
        {
            return players.Count(p => p.PlayerName == name) > 0;
        }

        public string[] playerNames
        {
            get
            {
                return players.Select(p => p.PlayerName).ToArray();
            }
        }

        public string[] ownersTribes
        {
            get
            {
                return players.Select(p => p.Tribe).ToArray();
            }
        }

        public string[] tribeNames
        {
            get
            {
                return tribes.Select(t => t.TribeName).ToArray();
            }
        }

        public List<Tribe> Tribes
        {
            set
            {
                tribes = value;
                updateTribeList();
            }
        }

        private void updatePlayerList()
        {
            listViewPlayer.Items.Clear();
            ListViewItem lvi;
            Dictionary<string, Color> tribeRelColors = new Dictionary<string, Color>();
            foreach (Player p in players)
            {
                // check if group of tribe exists
                ListViewGroup g = null;
                foreach (ListViewGroup lvg in listViewPlayer.Groups)
                {
                    if (lvg.Header == p.Tribe)
                    {
                        g = lvg;
                        break;
                    }
                }
                if (g == null)
                {
                    g = new ListViewGroup(p.Tribe);
                    listViewPlayer.Groups.Add(g);
                }
                if (!tribeRelColors.ContainsKey(p.Tribe))
                {
                    Color c = Color.White;
                    foreach (Tribe t in tribes)
                    {
                        if (t.TribeName == p.Tribe)
                        {
                            c = relationColor(t.TribeRelation);
                            break;
                        }
                    }
                    tribeRelColors.Add(p.Tribe, c);
                }
                int notesL = p.Note.Length;
                if (notesL > 40) notesL = 40;
                string rel = "n/a";
                foreach (Tribe t in tribes)
                {
                    if (t.TribeName == p.Tribe)
                    {
                        rel = t.TribeRelation.ToString();
                        break;
                    }
                }
                lvi = new ListViewItem(new string[] { p.PlayerName, p.Level.ToString(), p.Tribe, rel, p.Note.Substring(0, notesL) }, g);
                lvi.UseItemStyleForSubItems = false;
                lvi.Tag = p;
                lvi.SubItems[3].BackColor = tribeRelColors[p.Tribe];
                listViewPlayer.Items.Add(lvi);
            }
        }

        private void updateTribeList()
        {
            listViewTribes.Items.Clear();
            ListViewItem lvi;
            foreach (Tribe t in tribes)
            {
                lvi = new ListViewItem(new string[] { t.TribeName, t.TribeRelation.ToString() });
                lvi.UseItemStyleForSubItems = false;
                lvi.Tag = t;
                lvi.SubItems[1].BackColor = relationColor(t.TribeRelation);
                listViewTribes.Items.Add(lvi);
            }
            updateTribeSuggestions();
        }

        private void listViewPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool playerSelected = listViewPlayer.SelectedItems.Count > 0;
            if (playerSelected)
            {
                panelPlayerSettings.Visible = true;
                panelTribeSettings.Visible = false;
                selectedPlayer = (Player)listViewPlayer.SelectedItems[0].Tag;
                selectedRow = listViewPlayer.SelectedItems[0];
                textBoxPlayerName.Text = selectedPlayer.PlayerName;
                textBoxPlayerNotes.Text = selectedPlayer.Note;
                numericUpDownLevel.Value = selectedPlayer.Level;
                textBoxPlayerTribe.Text = selectedPlayer.Tribe;
            }
            panelPlayerSettings.Enabled = playerSelected;
        }

        private void listViewTribes_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool tribeSelected = listViewTribes.SelectedIndices.Count > 0;
            if (tribeSelected)
            {
                panelPlayerSettings.Visible = false;
                panelTribeSettings.Visible = true;
                selectedTribe = (Tribe)listViewTribes.SelectedItems[0].Tag;
                selectedTribeRow = listViewTribes.SelectedItems[0];
                textBoxTribeName.Text = selectedTribe.TribeName;
                switch (selectedTribe.TribeRelation)
                {
                    case Tribe.Relation.Allied:
                        radioButtonAllied.Checked = true;
                        break;
                    case Tribe.Relation.Friendly:
                        radioButtonFriendly.Checked = true;
                        break;
                    case Tribe.Relation.Neutral:
                        radioButtonNeutral.Checked = true;
                        break;
                    case Tribe.Relation.Hostile:
                        radioButtonHostile.Checked = true;
                        break;
                }
                textBoxTribeNotes.Text = selectedTribe.Note;
            }
            panelTribeSettings.Enabled = tribeSelected;
        }

        private void updateTribeSuggestions()
        {
            var l = new AutoCompleteStringCollection();
            l.AddRange(tribes.Select(t => t.TribeName).ToArray());
            textBoxPlayerTribe.AutoCompleteCustomSource = l;
        }

        private void textBoxPlayerName_TextChanged(object sender, EventArgs e)
        {
            if (selectedPlayer != null)
            {
                selectedPlayer.PlayerName = textBoxPlayerName.Text;
                selectedRow.SubItems[0].Text = textBoxPlayerName.Text;
            }
        }

        private void numericUpDownLevel_ValueChanged(object sender, EventArgs e)
        {
            if (selectedPlayer != null)
            {
                selectedPlayer.Level = (int)numericUpDownLevel.Value;
                selectedRow.SubItems[1].Text = numericUpDownLevel.Value.ToString();
            }
        }

        private void textBoxPlayerTribe_TextChanged(object sender, EventArgs e)
        {
            if (selectedPlayer != null)
            {
                selectedPlayer.Tribe = textBoxPlayerTribe.Text;
                selectedRow.SubItems[2].Text = textBoxPlayerTribe.Text;
            }
        }

        private void textBoxPlayerNotes_TextChanged(object sender, EventArgs e)
        {
            if (selectedPlayer != null)
            {
                selectedPlayer.Note = textBoxPlayerNotes.Text;
                selectedRow.SubItems[4].Text = textBoxPlayerNotes.Text;
            }
        }

        public void addPlayer(string name = "")
        {
            Player p = new Player();
            p.PlayerName = (name.Length > 0 ? name : "<new Player>");
            players.Add(p);
            updatePlayerList();
            int i = listViewPlayer.Items.Count - 1;
            listViewPlayer.Items[i].Selected = true;
            listViewPlayer.Items[i].Focused = true;
            textBoxPlayerName.SelectAll();
            textBoxPlayerName.Focus();
        }

        public void addTribe(string name = "")
        {
            Tribe t = new Tribe();
            t.TribeName = (name.Length > 0 ? name : "<new Tribe>");
            tribes.Add(t);
            updateTribeList();
            int i = listViewTribes.Items.Count - 1;
            listViewTribes.Items[i].Selected = true;
            listViewTribes.Items[i].Focused = true;
            textBoxTribeName.SelectAll();
            textBoxTribeName.Focus();
        }

        private void deleteSelectedPlayer()
        {
            if (listViewPlayer.SelectedIndices.Count > 0 && (MessageBox.Show("Delete selected Players?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                foreach (ListViewItem lvi in listViewPlayer.SelectedItems)
                {
                    players.Remove((Player)lvi.Tag);
                }
                updatePlayerList();
            }
        }

        private void deleteSelectedTribes()
        {
            if (listViewTribes.SelectedIndices.Count > 0 && (MessageBox.Show("Delete selected Tribes?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                foreach (ListViewItem lvi in listViewTribes.SelectedItems)
                {
                    tribes.Remove((Tribe)lvi.Tag);
                }
                updateTribeList();
            }
        }

        private void numericUpDownLevel_Enter(object sender, EventArgs e)
        {
            numericUpDownLevel.Select(0, numericUpDownLevel.Text.Length);
        }

        private void textBoxTribeName_TextChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeName = textBoxTribeName.Text;
                selectedTribeRow.SubItems[0].Text = textBoxTribeName.Text;
            }
        }

        private void radioButtonAllied_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeRelation = Tribe.Relation.Allied;
                updateTribeRowRelation(selectedTribeRow, Tribe.Relation.Allied);
            }
        }

        private void radioButtonNeutral_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeRelation = Tribe.Relation.Neutral;
                updateTribeRowRelation(selectedTribeRow, Tribe.Relation.Neutral);
            }
        }

        private void radioButtonFriendly_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeRelation = Tribe.Relation.Friendly;
                updateTribeRowRelation(selectedTribeRow, Tribe.Relation.Friendly);
            }
        }

        private void radioButtonHostile_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeRelation = Tribe.Relation.Hostile;
                updateTribeRowRelation(selectedTribeRow, Tribe.Relation.Hostile);
            }
        }

        private void textBoxTribeNotes_TextChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
                selectedTribe.Note = textBoxTribeNotes.Text;
        }

        private void updateTribeRowRelation(ListViewItem tribeRow, Tribe.Relation rel)
        {
            string tribe = tribeRow.SubItems[0].Text;
            tribeRow.SubItems[1].Text = rel.ToString();
            Color c = relationColor(rel);
            tribeRow.SubItems[1].BackColor = c;
            updatePlayerList();
        }

        private Color relationColor(Tribe.Relation r)
        {
            switch (r)
            {
                case Tribe.Relation.Allied: return Color.LightBlue;
                case Tribe.Relation.Friendly: return Color.LightGreen;
                case Tribe.Relation.Neutral: return Color.Yellow;
                case Tribe.Relation.Hostile: return Color.LightSalmon;
            }
            return Color.White;
        }

        private void listViewPlayer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteSelectedPlayer();
        }

        private void listViewTribes_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteSelectedTribes();
        }

        public void removeSelected()
        {
            if (listViewPlayer.Focused)
                deleteSelectedPlayer();
            else if (listViewTribes.Focused)
                deleteSelectedTribes();
        }

        private void listViewTribes_Enter(object sender, EventArgs e)
        {
            panelPlayerSettings.Visible = false;
            panelTribeSettings.Visible = true;
        }

        private void listViewPlayer_Enter(object sender, EventArgs e)
        {
            panelPlayerSettings.Visible = true;
            panelTribeSettings.Visible = false;
        }
    }
}

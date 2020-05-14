using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            listViewPlayer.ListViewItemSorter = new ListViewColumnSorter();
            listViewTribes.ListViewItemSorter = new ListViewColumnSorter();
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        public List<Player> Players
        {
            set
            {
                players = value;
                UpdatePlayerList();
            }
        }

        public List<Tribe> Tribes
        {
            set
            {
                tribes = value;
                UpdateTribeList();
            }
        }

        /// <summary>
        /// Checks if a player with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool PlayerExists(string name) =>
            !string.IsNullOrEmpty(name) && players.Any(p => p.PlayerName == name);

        /// <summary>
        /// Checks if a tribe with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool TribeExists(string name) =>
            !string.IsNullOrEmpty(name) && tribes.Any(t => t.TribeName == name);

        public string[] PlayerNames => players.Select(p => p.PlayerName).ToArray();

        public string[] OwnersTribes => players.Select(p => p.Tribe).ToArray();

        public string[] TribeNames => tribes.Select(t => t.TribeName).ToArray();

        /// <summary>
        /// Updates the displayed list of players from the internal list.
        /// </summary>
        private void UpdatePlayerList()
        {
            listViewPlayer.Items.Clear();
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
                            c = RelationColor(t.TribeRelation);
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
                ListViewItem lvi = new ListViewItem(new[] { p.PlayerName, p.Level.ToString(), p.Tribe, rel, p.Note.Substring(0, notesL) }, g)
                {
                    UseItemStyleForSubItems = false,
                    Tag = p
                };
                lvi.SubItems[3].BackColor = tribeRelColors[p.Tribe];
                listViewPlayer.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Updates the displayed list of tribes from the internal list.
        /// </summary>
        private void UpdateTribeList()
        {
            listViewTribes.Items.Clear();
            foreach (Tribe t in tribes)
            {
                ListViewItem lvi = new ListViewItem(new[] { t.TribeName, t.TribeRelation.ToString() })
                {
                    UseItemStyleForSubItems = false,
                    Tag = t
                };
                lvi.SubItems[1].BackColor = RelationColor(t.TribeRelation);
                listViewTribes.Items.Add(lvi);
            }
            UpdateTribeSuggestions();
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

        private void UpdateTribeSuggestions()
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

        /// <summary>
        /// Adds player.
        /// </summary>
        /// <param name="name"></param>
        public void AddPlayer(string name = null)
        {
            Player p = new Player
            {
                PlayerName = string.IsNullOrEmpty(name) ? "<new Player>" : name
            };
            players.Add(p);
            UpdatePlayerList();
            int i = listViewPlayer.Items.Count - 1;
            listViewPlayer.Items[i].Selected = true;
            listViewPlayer.Items[i].Focused = true;
            textBoxPlayerName.SelectAll();
            textBoxPlayerName.Focus();
        }

        /// <summary>
        /// Add tribe to tribe list.
        /// </summary>
        /// <param name="name"></param>
        public void AddTribe(string name = null)
        {
            Tribe t = new Tribe
            {
                TribeName = string.IsNullOrEmpty(name) ? "<new Tribe>" : name
            };
            tribes.Add(t);
            UpdateTribeList();
            int i = listViewTribes.Items.Count - 1;
            listViewTribes.Items[i].Selected = true;
            listViewTribes.Items[i].Focused = true;
            textBoxTribeName.SelectAll();
            textBoxTribeName.Focus();
        }

        private void DeleteSelectedPlayer()
        {
            if (listViewPlayer.SelectedIndices.Count > 0 && (MessageBox.Show("Delete selected Players?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                foreach (ListViewItem lvi in listViewPlayer.SelectedItems)
                {
                    players.Remove((Player)lvi.Tag);
                }
                UpdatePlayerList();
            }
        }

        private void DeleteSelectedTribes()
        {
            if (listViewTribes.SelectedIndices.Count > 0 && (MessageBox.Show("Delete selected Tribes?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                foreach (ListViewItem lvi in listViewTribes.SelectedItems)
                {
                    tribes.Remove((Tribe)lvi.Tag);
                }
                UpdateTribeList();
            }
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
                UpdateTribeRowRelation(selectedTribeRow, Tribe.Relation.Allied);
            }
        }

        private void radioButtonNeutral_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeRelation = Tribe.Relation.Neutral;
                UpdateTribeRowRelation(selectedTribeRow, Tribe.Relation.Neutral);
            }
        }

        private void radioButtonFriendly_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeRelation = Tribe.Relation.Friendly;
                UpdateTribeRowRelation(selectedTribeRow, Tribe.Relation.Friendly);
            }
        }

        private void radioButtonHostile_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
            {
                selectedTribe.TribeRelation = Tribe.Relation.Hostile;
                UpdateTribeRowRelation(selectedTribeRow, Tribe.Relation.Hostile);
            }
        }

        private void textBoxTribeNotes_TextChanged(object sender, EventArgs e)
        {
            if (selectedTribe != null)
                selectedTribe.Note = textBoxTribeNotes.Text;
        }

        private void UpdateTribeRowRelation(ListViewItem tribeRow, Tribe.Relation rel)
        {
            tribeRow.SubItems[1].Text = rel.ToString();
            tribeRow.SubItems[1].BackColor = RelationColor(rel);
            UpdatePlayerList();
        }

        private static Color RelationColor(Tribe.Relation r)
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
                DeleteSelectedPlayer();
        }

        private void listViewTribes_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteSelectedTribes();
        }

        /// <summary>
        /// Removes selected entry, either player or tribe.
        /// </summary>
        public void RemoveSelected()
        {
            if (listViewPlayer.Focused)
                DeleteSelectedPlayer();
            else if (listViewTribes.Focused)
                DeleteSelectedTribes();
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

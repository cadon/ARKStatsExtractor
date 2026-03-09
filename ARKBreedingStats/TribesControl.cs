using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace ARKBreedingStats
{
    public partial class TribesControl : UserControl
    {
        private List<Player> _players;
        private List<Tribe> _tribes;
        private Player _selectedPlayer;
        private ListViewItem _selectedRow;
        private Tribe _selectedTribe;
        private ListViewItem _selectedTribeRow;

        public TribesControl()
        {
            InitializeComponent();

            listViewPlayer.ListViewItemSorter = new ListViewColumnSorter();
            listViewTribes.ListViewItemSorter = new ListViewColumnSorter();
            ListViewColumnSorter.DoSort(listViewPlayer, Properties.Settings.Default.PlayerListSortColumn);
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Player> Players
        {
            set
            {
                _players = value;
                UpdatePlayerList();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Tribe> Tribes
        {
            set
            {
                _tribes = value;
                UpdateTribeList();
            }
        }

        public ListView ListViewPlayers => listViewPlayer;

        /// <summary>
        /// Checks if a player with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool PlayerExists(string name) =>
            !string.IsNullOrEmpty(name) && _players.Any(p => p.PlayerName == name);

        /// <summary>
        /// Checks if a tribe with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool TribeExists(string name) =>
            !string.IsNullOrEmpty(name) && _tribes.Any(t => t.TribeName == name);

        public string[] PlayerNames => _players.Select(p => p.PlayerName).ToArray();

        public string[] OwnersTribes => _players.Select(p => p.Tribe).ToArray();

        public string[] TribeNames => _tribes.Select(t => t.TribeName).ToArray();

        /// <summary>
        /// Updates the displayed list of players from the internal list.
        /// </summary>
        private void UpdatePlayerList()
        {
            listViewPlayer.Items.Clear();
            Dictionary<string, Color> tribeRelColors = new Dictionary<string, Color>();

            var tribeGroups = new Dictionary<string, ListViewGroup>();
            var lviPlayers = new List<ListViewItem>();

            foreach (Player p in _players)
            {
                // check if group of tribe exists
                var tribeName = p.Tribe ?? string.Empty;
                if (!tribeGroups.TryGetValue(tribeName, out var g))
                {
                    g = new ListViewGroup(p.Tribe);
                    tribeGroups[tribeName] = g;
                }

                if (p.Tribe != null && !tribeRelColors.ContainsKey(p.Tribe))
                {
                    Color c = Color.White;
                    foreach (Tribe t in _tribes)
                    {
                        if (t.TribeName == p.Tribe)
                        {
                            c = RelationColor(t.TribeRelation);
                            break;
                        }
                    }
                    tribeRelColors.Add(p.Tribe, c);
                }
                int notesL = p.Note?.Length ?? 0;
                if (notesL > 40) notesL = 40;
                string rel = "n/a";
                foreach (Tribe t in _tribes)
                {
                    if (t.TribeName == p.Tribe)
                    {
                        rel = t.TribeRelation.ToString();
                        break;
                    }
                }
                ListViewItem lvi = new ListViewItem(new[] { p.Rank.ToString(), p.PlayerName, p.Level.ToString(), p.Tribe, rel, p.Note?.Substring(0, notesL) }, g)
                {
                    UseItemStyleForSubItems = false,
                    Tag = p
                };
                if (!string.IsNullOrEmpty(p.Tribe))
                    lvi.SubItems[3].BackColor = tribeRelColors[p.Tribe];
                lviPlayers.Add(lvi);
            }

            listViewPlayer.Groups.AddRange(tribeGroups.Values.ToArray());
            listViewPlayer.Items.AddRange(lviPlayers.ToArray());
        }

        /// <summary>
        /// Updates the displayed list of tribes from the internal list.
        /// </summary>
        private void UpdateTribeList()
        {
            listViewTribes.Items.Clear();
            var tribeList = new List<ListViewItem>();
            foreach (Tribe t in _tribes)
            {
                ListViewItem lvi = new ListViewItem(new[] { t.TribeName, t.TribeRelation.ToString() })
                {
                    UseItemStyleForSubItems = false,
                    Tag = t
                };
                lvi.SubItems[1].BackColor = RelationColor(t.TribeRelation);
                tribeList.Add(lvi);
            }
            listViewTribes.Items.AddRange(tribeList.ToArray());
            UpdateTribeSuggestions();
        }

        private void listViewPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool playerSelected = listViewPlayer.SelectedItems.Count > 0;
            if (playerSelected)
            {
                panelPlayerSettings.Visible = true;
                panelTribeSettings.Visible = false;
                _selectedPlayer = (Player)listViewPlayer.SelectedItems[0].Tag;
                _selectedRow = listViewPlayer.SelectedItems[0];
                nudPlayerRank.Value = _selectedPlayer.Rank;
                textBoxPlayerName.Text = _selectedPlayer.PlayerName;
                textBoxPlayerNotes.Text = _selectedPlayer.Note;
                nudPlayerLevel.Value = _selectedPlayer.Level;
                textBoxPlayerTribe.Text = _selectedPlayer.Tribe;
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
                _selectedTribe = (Tribe)listViewTribes.SelectedItems[0].Tag;
                _selectedTribeRow = listViewTribes.SelectedItems[0];
                textBoxTribeName.Text = _selectedTribe.TribeName;
                switch (_selectedTribe.TribeRelation)
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
                textBoxTribeNotes.Text = _selectedTribe.Note;
            }
            panelTribeSettings.Enabled = tribeSelected;
        }

        private void UpdateTribeSuggestions()
        {
            var l = new AutoCompleteStringCollection();
            l.AddRange(_tribes.Select(t => t.TribeName).ToArray());
            textBoxPlayerTribe.AutoCompleteCustomSource = l;
        }

        private void nudPlayerRank_ValueChanged(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.Rank = (int)nudPlayerRank.Value;
                _selectedRow.SubItems[0].Text = nudPlayerRank.Value.ToString();
            }
        }

        private void textBoxPlayerName_TextChanged(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.PlayerName = textBoxPlayerName.Text;
                _selectedRow.SubItems[1].Text = textBoxPlayerName.Text;
            }
        }

        private void nudPlayerLevel_ValueChanged(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.Level = (int)nudPlayerLevel.Value;
                _selectedRow.SubItems[2].Text = nudPlayerLevel.Value.ToString();
            }
        }

        private void textBoxPlayerTribe_TextChanged(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.Tribe = textBoxPlayerTribe.Text;
                _selectedRow.SubItems[3].Text = textBoxPlayerTribe.Text;
            }
        }

        private void textBoxPlayerNotes_TextChanged(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.Note = textBoxPlayerNotes.Text;
                _selectedRow.SubItems[4].Text = textBoxPlayerNotes.Text;
            }
        }

        /// <summary>
        /// Adds player.
        /// </summary>
        /// <param name="name"></param>
        public void AddPlayer(string name = null)
        {
            var p = new Player
            {
                PlayerName = string.IsNullOrEmpty(name) ? "<new Player>" : name
            };
            if (_players == null) _players = new List<Player>();
            _players.Add(p);
            UpdatePlayerList();
            int i = listViewPlayer.Items.Count - 1;
            listViewPlayer.Items[i].Selected = true;
            listViewPlayer.Items[i].Focused = true;
            textBoxPlayerName.SelectAll();
            textBoxPlayerName.Focus();
        }

        /// <summary>
        /// Add players if they aren't yet in the list.
        /// </summary>
        /// <param name="playerNames"></param>
        public void AddPlayers(HashSet<string> playerNames)
        {
            if (playerNames == null || !playerNames.Any()) return;
            var existingPlayers = _players?.Select(p => p.PlayerName).ToHashSet();
            if (existingPlayers != null)
                playerNames.ExceptWith(existingPlayers);
            var newPlayersArray = playerNames.Where(newPlayer => !string.IsNullOrEmpty(newPlayer))
                .Select(p => new Player { PlayerName = p }).ToArray();
            if (!newPlayersArray.Any()) return;
            if (_players == null) _players = new List<Player>();
            _players.AddRange(newPlayersArray);
            UpdatePlayerList();
        }

        /// <summary>
        /// Add tribe to tribe list.
        /// </summary>
        /// <param name="name"></param>
        public void AddTribe(string name = null)
        {
            var t = new Tribe
            {
                TribeName = string.IsNullOrEmpty(name) ? "<new Tribe>" : name
            };
            if (_tribes == null) _tribes = new List<Tribe>();
            _tribes.Add(t);
            UpdateTribeList();
            int i = listViewTribes.Items.Count - 1;
            listViewTribes.Items[i].Selected = true;
            listViewTribes.Items[i].Focused = true;
            textBoxTribeName.SelectAll();
            textBoxTribeName.Focus();
        }

        /// <summary>
        /// Add tribes if they aren't yet in the list.
        /// </summary>
        /// <param name="tribeNames"></param>
        public void AddTribes(HashSet<string> tribeNames)
        {
            if (tribeNames == null || !tribeNames.Any()) return;
            var existingTribes = _tribes?.Select(t => t.TribeName).ToHashSet();
            if (existingTribes != null)
                tribeNames.ExceptWith(existingTribes);
            var newTribesArray = tribeNames.Distinct().Where(newTribe => !string.IsNullOrEmpty(newTribe))
                .Select(t => new Tribe { TribeName = t }).ToArray();
            if (!newTribesArray.Any()) return;
            if (_tribes == null) _tribes = new List<Tribe>();
            _tribes.AddRange(newTribesArray);
            UpdateTribeList();
        }

        private void DeleteSelectedPlayer()
        {
            if (listViewPlayer.SelectedIndices.Count > 0 && (MessageBox.Show("Delete selected Players?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                foreach (ListViewItem lvi in listViewPlayer.SelectedItems)
                {
                    _players.Remove((Player)lvi.Tag);
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
                    _tribes.Remove((Tribe)lvi.Tag);
                }
                UpdateTribeList();
            }
        }

        private void textBoxTribeName_TextChanged(object sender, EventArgs e)
        {
            if (_selectedTribe != null)
            {
                _selectedTribe.TribeName = textBoxTribeName.Text;
                _selectedTribeRow.SubItems[0].Text = textBoxTribeName.Text;
            }
        }

        private void radioButtonAllied_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedTribe != null)
            {
                _selectedTribe.TribeRelation = Tribe.Relation.Allied;
                UpdateTribeRowRelation(_selectedTribeRow, Tribe.Relation.Allied);
            }
        }

        private void radioButtonNeutral_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedTribe != null)
            {
                _selectedTribe.TribeRelation = Tribe.Relation.Neutral;
                UpdateTribeRowRelation(_selectedTribeRow, Tribe.Relation.Neutral);
            }
        }

        private void radioButtonFriendly_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedTribe != null)
            {
                _selectedTribe.TribeRelation = Tribe.Relation.Friendly;
                UpdateTribeRowRelation(_selectedTribeRow, Tribe.Relation.Friendly);
            }
        }

        private void radioButtonHostile_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedTribe != null)
            {
                _selectedTribe.TribeRelation = Tribe.Relation.Hostile;
                UpdateTribeRowRelation(_selectedTribeRow, Tribe.Relation.Hostile);
            }
        }

        private void textBoxTribeNotes_TextChanged(object sender, EventArgs e)
        {
            if (_selectedTribe != null)
                _selectedTribe.Note = textBoxTribeNotes.Text;
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

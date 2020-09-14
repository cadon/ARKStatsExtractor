using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class PedigreeCreature : UserControl
    {
        public delegate void CreatureChangedEventHandler(Creature creature, int comboId, MouseEventArgs e);

        public event CreatureChangedEventHandler CreatureClicked;

        /// <summary>
        /// Edit the creature. Boolean parameter determines if the creature is virtual.
        /// </summary>
        public event Action<Creature, bool> CreatureEdit;

        /// <summary>
        /// Display the best breeding partners for the given creature.
        /// </summary>
        public event Action<Creature> BestBreedingPartners;

        /// <summary>
        /// Display the creature in the pedigree.
        /// </summary>
        public event Action<Creature> DisplayInPedigree;

        /// <summary>
        /// Recalculate the breeding plan, e.g. if the cooldown was reset.
        /// </summary>
        public event Action RecalculateBreedingPlan;

        public delegate void ExportToClipboardEventHandler(Creature c, bool breedingValues, bool ArkMl);

        public event ExportToClipboardEventHandler ExportToClipboard;
        private readonly List<Label> _labels;
        private readonly ToolTip _tt;
        public int comboId;
        /// <summary>
        /// If set to true, the control will not display sex, status or creature colors.
        /// </summary>
        public bool OnlyLevels { get; set; }
        public bool[] enabledColorRegions;
        private bool _contextMenuAvailable;
        /// <summary>
        /// If set to true, the levelHatched in parenthesis is appended with an '+'.
        /// </summary>
        public bool TotalLevelUnknown { get; set; } = false;

        public static int[] displayedStats = new int[] {
                                                        (int)StatNames.Health,
                                                        (int)StatNames.Stamina,
                                                        (int)StatNames.Oxygen,
                                                        (int)StatNames.Food,
                                                        (int)StatNames.Weight,
                                                        (int)StatNames.MeleeDamageMultiplier,
                                                        (int)StatNames.SpeedMultiplier,
                                                        (int)StatNames.CraftingSpeedMultiplier
                                                        };
        public static int displayedStatsCount = displayedStats.Length;

        public PedigreeCreature()
        {
            InitializeComponent();
            _tt = new ToolTip
            {
                InitialDelay = 100
            };
            _tt.SetToolTip(labelSex, "Sex");
            _tt.SetToolTip(labelMutations, "Mutation-Counter");
            _labels = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp, labelCr };
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Disposed += PedigreeCreature_Disposed;
            comboId = -1;
        }

        public PedigreeCreature(Creature creature, bool[] enabledColorRegions, int comboId = -1, bool displayPedigreeLink = false) : this()
        {
            Cursor = Cursors.Hand;
            this.enabledColorRegions = enabledColorRegions;
            this.comboId = comboId;
            Creature = creature;
            TsMiViewInPedigree.Visible = displayPedigreeLink;
        }

        /// <summary>
        /// Set text of labels for stats. Only used for header control.
        /// </summary>
        public void SetCustomStatNames(Dictionary<string, string> customStatNames = null)
        {
            for (int s = 0; s < displayedStatsCount; s++)
            {
                _labels[s].Text = Utils.StatName(displayedStats[s], true, customStatNames);
                _tt.SetToolTip(_labels[s], Utils.StatName(displayedStats[s], customStatNames: customStatNames));
            }

            labelMutations.Visible = true;
        }

        private void PedigreeCreature_Disposed(object sender, EventArgs e)
        {
            _tt.RemoveAll();
        }

        private Creature _creature;
        /// <summary>
        /// The creature that is displayed in this control.
        /// </summary>
        public Creature Creature
        {
            get => _creature;
            set
            {
                _creature = value;
                if (_creature == null)
                {
                    Clear();
                    return;
                }
                SetTitle();

                if (!OnlyLevels)
                {
                    if (_creature.Status == CreatureStatus.Dead)
                    {
                        groupBox1.ForeColor = SystemColors.GrayText;
                        _tt.SetToolTip(groupBox1, "Creature has passed away");
                    }
                    else if (_creature.Status == CreatureStatus.Unavailable)
                    {
                        groupBox1.ForeColor = SystemColors.GrayText;
                        _tt.SetToolTip(groupBox1, "Creature is currently not available");
                    }
                    else if (_creature.Status == CreatureStatus.Obelisk)
                    {
                        groupBox1.ForeColor = SystemColors.GrayText;
                        _tt.SetToolTip(groupBox1, "Creature is currently uploaded in an obelisk");
                    }
                }

                _tt.SetToolTip(labelSex, "Sex: " + Loc.S(_creature.sex.ToString()));
                for (int s = 0; s < displayedStatsCount; s++)
                {
                    int si = displayedStats[s];
                    if (_creature.valuesBreeding[si] == 0)
                    {
                        // stat not used // TODO hide label?
                        _labels[s].Text = "-";
                        _labels[s].BackColor = Color.WhiteSmoke;
                        _labels[s].ForeColor = Color.LightGray;
                    }
                    else if (_creature.levelsWild[si] < 0)
                    {
                        _labels[s].Text = "?";
                        _labels[s].BackColor = Color.WhiteSmoke;
                        _labels[s].ForeColor = Color.LightGray;
                    }
                    else if (_creature.levelsWild[si] == 0 && (_creature.Species?.stats[si].IncPerTamedLevel ?? -1) == 0)
                    {
                        // stat cannot be leveled, e.g. speed for flyers, and thus it's assumed there are no wild levels applied, i.e. irrelevant for breeding.
                        _labels[s].Text = "0";
                        _labels[s].BackColor = Color.WhiteSmoke;
                        _labels[s].ForeColor = Color.LightGray;
                        _tt.SetToolTip(_labels[s], Utils.StatName(si, false, _creature.Species?.statNames) + ": " + _creature.valuesBreeding[si] * (Utils.Precision(si) == 3 ? 100 : 1) + (Utils.Precision(si) == 3 ? "%" : string.Empty));
                    }
                    else
                    {
                        _labels[s].Text = _creature.levelsWild[si].ToString();
                        _labels[s].BackColor = Utils.GetColorFromPercent((int)(_creature.levelsWild[si] * 2.5), _creature.topBreedingStats[si] ? 0.2 : 0.7);
                        _labels[s].ForeColor = Parent.ForeColor; // needed so text is not transparent on overlay
                        _tt.SetToolTip(_labels[s], Utils.StatName(si, false, _creature.Species?.statNames) + ": " + _creature.valuesBreeding[si] * (Utils.Precision(si) == 3 ? 100 : 1) + (Utils.Precision(si) == 3 ? "%" : string.Empty));
                    }
                    _labels[s].Font = new Font("Microsoft Sans Serif", 8.25F, _creature.topBreedingStats[si] ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point, 0);
                }
                if (OnlyLevels)
                {
                    labelSex.Visible = false;
                    pictureBox1.Visible = false;
                    plainTextcurrentValuesToolStripMenuItem.Visible = false;
                }
                else
                {
                    labelSex.Visible = true;
                    labelSex.Text = Utils.SexSymbol(_creature.sex);
                    labelSex.BackColor = _creature.flags.HasFlag(CreatureFlags.Neutered) ? SystemColors.GrayText : Utils.SexColor(_creature.sex);
                    UpdateColors(_creature.colors);
                    _tt.SetToolTip(pictureBox1, CreatureColored.RegionColorInfo(_creature.Species, _creature.colors));
                    labelSex.Visible = true;
                    pictureBox1.Visible = true;
                    plainTextcurrentValuesToolStripMenuItem.Visible = true;
                }
                int totalMutations = _creature.Mutations;
                if (totalMutations > 0)
                {
                    labelMutations.Text = totalMutations > 9999 ? totalMutations.ToString().Substring(0, 4) + "…" : totalMutations.ToString();
                    if (totalMutations > 19)
                        labelMutations.BackColor = Utils.MutationColorOverLimit;
                    else
                        labelMutations.BackColor = Utils.MutationColor;
                    _tt.SetToolTip(labelMutations, "Mutation-Counter: " + totalMutations.ToString("N0") + "\nMaternal: " + _creature.mutationsMaternal.ToString("N0") + "\nPaternal: " + _creature.mutationsPaternal.ToString("N0"));
                }
                labelMutations.Visible = totalMutations > 0;
                _contextMenuAvailable = true;
            }
        }

        /// <summary>
        /// Update the colors displayed in the wheel.
        /// </summary>
        /// <param name="colorIds"></param>
        internal void UpdateColors(int[] colorIds)
            => pictureBox1.Image = CreatureColored.GetColoredCreature(colorIds, null, enabledColorRegions, 24, 22, true);

        /// <summary>
        /// Sets the displayed title of the control.
        /// </summary>
        private void SetTitle()
        {
            string totalLevel = _creature.LevelHatched > 0 ? _creature.LevelHatched.ToString() : "?";
            groupBox1.Text =
                $"{(!OnlyLevels && _creature.Status != CreatureStatus.Available ? "(" + Utils.StatusSymbol(_creature.Status) + ") " : string.Empty)}{_creature.name} ({totalLevel}{(TotalLevelUnknown ? "+" : string.Empty)})";

            if (_creature.growingUntil > DateTime.Now)
                groupBox1.Text += $" (grown at {Utils.ShortTimeDate(_creature.growingUntil)})";
            else if (_creature.cooldownUntil > DateTime.Now)
                groupBox1.Text += $" (cooldown until {Utils.ShortTimeDate(_creature.cooldownUntil)})";
        }

        public bool Highlight
        {
            set
            {
                panelHighlight.Visible = value;
                HandCursor = !value;
            }
        }

        public bool HandCursor
        {
            set => Cursor = value ? Cursors.Hand : Cursors.Default;
        }

        private void PedigreeCreature_MouseClick(object sender, MouseEventArgs e)
        {
            if (CreatureClicked != null && e.Button == MouseButtons.Left)
                CreatureClicked(_creature, comboId, e);
        }

        private void element_MouseClick(object sender, MouseEventArgs e)
        {
            PedigreeCreature_MouseClick(sender, e);
        }

        public void Clear()
        {
            for (int s = 0; s < displayedStatsCount; s++)
            {
                _labels[s].Text = string.Empty;
                _labels[s].BackColor = SystemColors.Control;
            }
            labelSex.Visible = false;
            labelMutations.Visible = false;
            groupBox1.Text = string.Empty;
            pictureBox1.Visible = false;
        }

        private bool _isVirtual;
        /// <summary>
        /// If a creature is virtual, it is not a creature in the library.
        /// </summary>
        public void SetIsVirtual(bool isVirtual)
        {
            _isVirtual = isVirtual;
            setCooldownToolStripMenuItem.Visible = !isVirtual;
            removeCooldownGrowingToolStripMenuItem.Visible = !isVirtual;
            bestBreedingPartnersToolStripMenuItem.Visible = !isVirtual;
            editToolStripMenuItem.Text = isVirtual ? "Copy values to Tester" : "Edit";
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreatureEdit?.Invoke(_creature, _isVirtual);
        }

        private void setCooldownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _creature.cooldownUntil = DateTime.Now.AddHours(2);
            RecalculateBreedingPlan?.Invoke();
            SetTitle();
        }

        private void removeCooldownGrowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_creature.cooldownUntil > DateTime.Now)
                _creature.cooldownUntil = DateTime.Now;
            if (_creature.growingUntil > DateTime.Now)
                _creature.growingUntil = DateTime.Now;
            SetTitle();
        }

        private void bestBreedingPartnersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BestBreedingPartners?.Invoke(_creature);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (!_contextMenuAvailable)
            {
                e.Cancel = true;
            }
        }

        private void plainTextbreedingValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToClipboard?.Invoke(_creature, true, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToClipboard?.Invoke(_creature, false, false);
        }

        private void OpenWikipageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_creature?.Species != null)
                System.Diagnostics.Process.Start("https://ark.gamepedia.com/" + _creature.Species.name);
        }

        private void TsMiViewInPedigree_Click(object sender, EventArgs e)
        {
            DisplayInPedigree?.Invoke(_creature);
        }

        private void copyNameToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_creature?.name))
                Clipboard.SetText(_creature.name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.Pedigree
{
    public partial class PedigreeCreature : UserControl, IPedigreeCreature
    {
        public const int HorizontalStatDistance = 29;
        public const int XOffsetFirstStat = 38;

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

        private readonly List<Label> _labels;
        private readonly ToolTip _ttMonospaced;
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
        public bool TotalLevelUnknown { get; set; }

        public static readonly int[] DisplayedStats = {
                                                        Stats.Health,
                                                        Stats.Stamina,
                                                        Stats.Oxygen,
                                                        Stats.Food,
                                                        Stats.Weight,
                                                        Stats.MeleeDamageMultiplier,
                                                        Stats.SpeedMultiplier,
                                                        Stats.CraftingSpeedMultiplier
                                                        };
        public static readonly int DisplayedStatsCount = DisplayedStats.Length;

        public PedigreeCreature()
        {
            InitializeComponent();

            _tt = new ToolTip
            {
                InitialDelay = 100,
                AutoPopDelay = 15000
            };
            _ttMonospaced = new ToolTip
            {
                InitialDelay = 100,
                AutoPopDelay = 15000
            };
            _ttMonospaced.OwnerDraw = true;
            // set to monospaced font for better digit alignment
            if (TooltipFont == null)
                TooltipFont = new Font("Consolas", 12);
            _ttMonospaced.Draw += TtMonospacedDraw;
            _ttMonospaced.Popup += TtMonospacedPopup;
            _tt.SetToolTip(labelSex, "Sex");
            _ttMonospaced.SetToolTip(labelMutations, "Mutation-Counter");
            _labels = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp, labelCr };
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Disposed += PedigreeCreature_Disposed;
            comboId = -1;
        }

        #region Tooltip font

        private static Font TooltipFont;

        private void TtMonospacedPopup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = TextRenderer.MeasureText(_ttMonospaced.GetToolTip(e.AssociatedControl), TooltipFont);
        }

        private void TtMonospacedDraw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, TooltipFont, Brushes.Black, 0, 0);
        }

        public PedigreeCreature(Creature creature, bool[] enabledColorRegions, int comboId = -1, bool displayPedigreeLink = false) : this()
        {
            Cursor = Cursors.Hand;
            this.enabledColorRegions = enabledColorRegions;
            this.comboId = comboId;
            Creature = creature;
            TsMiViewInPedigree.Visible = displayPedigreeLink;
        }

        #endregion

        /// <summary>
        /// Set text of labels for stats. Only used for header control.
        /// </summary>
        public void SetCustomStatNames(Dictionary<string, string> customStatNames = null)
        {
            for (int s = 0; s < DisplayedStatsCount; s++)
            {
                _labels[s].Text = Utils.StatName(DisplayedStats[s], true, customStatNames);
                _ttMonospaced.SetToolTip(_labels[s], Utils.StatName(DisplayedStats[s], customStatNames: customStatNames));
            }

            labelMutations.Visible = true;
        }

        private void PedigreeCreature_Disposed(object sender, EventArgs e)
        {
            _ttMonospaced.RemoveAll();
            _ttMonospaced.Dispose();
            _tt.RemoveAll();
            _tt.Dispose();
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
                var minChartLevel = CreatureCollection.CurrentCreatureCollection?.minChartLevel ?? 0;
                var maxChartLevel = CreatureCollection.CurrentCreatureCollection?.maxChartLevel ?? 50;
                var chartLevelRange = maxChartLevel - minChartLevel;
                var hueRangeEven = Properties.Settings.Default.ChartHueEvenMax - Properties.Settings.Default.ChartHueEvenMin;
                var hueRangeOdd = Properties.Settings.Default.ChartHueOddMax - Properties.Settings.Default.ChartHueOddMin;
                for (int s = 0; s < DisplayedStatsCount; s++)
                {
                    int si = DisplayedStats[s];
                    if (_creature.valuesBreeding != null && _creature.valuesBreeding[si] == 0)
                    {
                        // stat not used // TODO hide label?
                        _labels[s].Text = "-";
                        _labels[s].BackColor = Color.WhiteSmoke;
                        _labels[s].ForeColor = Color.LightGray;
                    }
                    else if (_creature.levelsWild == null || _creature.levelsWild[si] < 0)
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
                        _ttMonospaced.SetToolTip(_labels[s], Utils.StatName(si, false, _creature.Species?.statNames) + ": "
                            + $"{_creature.valuesBreeding[si] * (Utils.Precision(si) == 3 ? 100 : 1),7:#,0.0}"
                            + (Utils.Precision(si) == 3 ? "%" : string.Empty));
                    }
                    else
                    {
                        _labels[s].Text = _creature.levelsWild[si].ToString();
                        if (Properties.Settings.Default.Highlight255Level && _creature.levelsWild[si] > 253) // 255 is max, 254 is the highest that allows dom leveling
                            _labels[s].BackColor = Utils.AdjustColorLight(_creature.levelsWild[si] == 254 ? Utils.Level254 : Utils.Level255, _creature.topBreedingStats[si] ? 0.2 : 0.7);
                        else if (Properties.Settings.Default.HighlightEvenOdd)
                        {
                            var levelForColor = Math.Min(maxChartLevel, Math.Max(minChartLevel, _creature.levelsWild[si]));
                            int hue;
                            if (_creature.levelsWild[si] % 2 == 0)
                            {
                                hue = Properties.Settings.Default.ChartHueEvenMin + levelForColor * hueRangeEven / chartLevelRange;
                            }
                            else
                            {
                                hue = Properties.Settings.Default.ChartHueOddMin + levelForColor * hueRangeOdd / chartLevelRange;
                            }
                            _labels[s].BackColor = Utils.ColorFromHue(hue, _creature.topBreedingStats[si] ? 0.4 : 0.7);
                        }
                        else
                            _labels[s].BackColor = Utils.GetColorFromPercent((int)(_creature.levelsWild[si] * 2.5), _creature.topBreedingStats[si] ? 0.2 : 0.7);
                        _labels[s].ForeColor = Parent?.ForeColor ?? Color.Black; // needed so text is not transparent on overlay
                        _ttMonospaced.SetToolTip(_labels[s], Utils.StatName(si, false, _creature.Species?.statNames) + ": "
                            + $"{_creature.valuesBreeding[si] * (Utils.Precision(si) == 3 ? 100 : 1),7:#,0.0}"
                            + (Utils.Precision(si) == 3 ? "%" : string.Empty));
                    }
                    // fonts are strange, and this seems to work. The assigned font-object is probably only used to read out the properties and then not used anymore.
                    using (var font = new Font("Microsoft Sans Serif", 8.25F, (_creature.topBreedingStats?[si]).GetValueOrDefault() ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point, 0))
                        _labels[s].Font = font;
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
                    var totalMutationsString = totalMutations.ToString();
                    labelMutations.Text = totalMutationsString.Length > 4 ? totalMutationsString.Substring(0, 4) + "…" : totalMutationsString;
                    labelMutations.BackColor = totalMutations < Ark.MutationPossibleWithLessThan ? Utils.MutationColor : Utils.MutationColorOverLimit;
                    _ttMonospaced.SetToolTip(labelMutations,
                        $"Mutation-Counter: {totalMutations,13:#,0}\nMaternal: {_creature.mutationsMaternal,21:#,0}\nPaternal: {_creature.mutationsPaternal,21:#,0}");
                }
                labelMutations.Visible = totalMutations > 0;
                _contextMenuAvailable = true;
            }
        }

        /// <summary>
        /// Update the colors displayed in the wheel.
        /// </summary>
        internal void UpdateColors(byte[] colorIds)
            => pictureBox1.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(colorIds, null, enabledColorRegions, 24, 22, true));

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

        /// <summary>
        /// Clears the displayed data.
        /// </summary>
        public void Clear()
        {
            for (int s = 0; s < DisplayedStatsCount; s++)
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
            TsMiViewInPedigree.Visible = !isVirtual;
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
            ExportImportCreatures.ExportToClipboard(_creature, true, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportImportCreatures.ExportToClipboard(_creature, false, false);
        }

        private void OpenWikipageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_creature?.Species == null) return;
            ArkWiki.OpenPage(_creature.Species.name);
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

        private void copyInfoGraphicToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _creature?.ExportInfoGraphicToClipboard(CreatureCollection.CurrentCreatureCollection);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.Traits;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.Pedigree
{
    public partial class PedigreeCreature : UserControl, IPedigreeCreature
    {
        public const int ControlHeightWoMutations = 32;
        public const int ControlHeightWMutations = 46;
        public const int HorizontalStatDistance = 29;
        public const int XOffsetFirstStat = 38;

        /// <summary>
        /// Display the species name after the creature name.
        /// </summary>
        public bool DisplaySpecies;

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

        /// <summary>
        /// Generate name pattern for creature and copy to clipboard.
        /// </summary>
        public static event Action<Creature, int> CopyGeneratedPatternToClipboard;

        private readonly List<Label> _labelsStats;
        private readonly List<Label> _labelsStatsMut;
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
        /// If set to true, the levelHatched in parentheses is appended with an '+'.
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
            _labelsStats = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp, labelCr };
            _labelsStatsMut = new List<Label> { LbHpMut, LbStMut, LbOxMut, LbFoMut, LbWeMut, LbDmMut, LbSpMut, LbCrMut };
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Disposed += PedigreeCreature_Disposed;
            comboId = -1;

            var stat = 0;
            foreach (var l in _labelsStats)
            {
                l.MouseClick += element_MouseClick;
                l.Tag = DisplayedStats[stat++];
                l.Paint += StatLabelPaint;
            }
            foreach (var l in _labelsStatsMut)
                l.MouseClick += element_MouseClick;

            // name patterns menu entries
            const int namePatternCount = 6;
            var libraryContextMenuItemsCopyToClipboard = new ToolStripMenuItem[namePatternCount];
            for (var i = 0; i < namePatternCount; i++)
            {
                // library context menu copy name pattern to clipboard
                var mi = new ToolStripMenuItem { Text = $"Pattern {i + 1}", Tag = i };
                mi.Click += CopyGeneratedNamePatternToClipboard;
                libraryContextMenuItemsCopyToClipboard[i] = mi;
            }
            toolStripMenuItemCopyGeneratedNameToClipboard.DropDownItems.AddRange(libraryContextMenuItemsCopyToClipboard);
        }

        private void StatLabelPaint(object sender, PaintEventArgs e)
        {
            if (Creature?.Traits == null) return;
            var g = e.Graphics;
            var statIndex = (int)((Control)sender).Tag;
            var i = 0;
            using (var p = new Pen(Color.Black))
            using (var b = new SolidBrush(Color.White))
            {
                foreach (var t in Creature.Traits)
                {
                    if (t.TraitDefinition.StatIndex != statIndex) continue;
                    if (t.MutationProbability > 0)
                    {
                        p.Color = Color.DeepPink;
                        b.Color = Color.Pink;
                    }
                    else if (t.MutationProbability < 0)
                    {
                        p.Color = Color.DarkGreen;
                        b.Color = Color.GreenYellow;
                    }
                    else if (t.InheritHigherProbability > 0)
                    {
                        p.Color = Color.DarkBlue;
                        b.Color = Color.DeepSkyBlue;
                    }
                    else if (t.InheritHigherProbability < 0)
                    {
                        p.Color = Color.DarkGoldenrod;
                        b.Color = Color.Yellow;
                    }
                    else continue;

                    const int circleWidth = 3;
                    const int markersPerColumn = 3;
                    var y = (i % markersPerColumn) * (circleWidth + 1);
                    var x = (i / markersPerColumn) * (circleWidth + 1);
                    g.FillEllipse(b, x, y, circleWidth, circleWidth);
                    g.DrawEllipse(p, x, y, circleWidth, circleWidth);
                    i++;
                }
            }
        }

        private void CopyGeneratedNamePatternToClipboard(object sender, EventArgs e)
        {
            CopyGeneratedPatternToClipboard?.Invoke(Creature, (int)((ToolStripMenuItem)sender).Tag);
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

        public PedigreeCreature(Creature creature, bool[] enabledColorRegions, int comboId = -1, bool displayPedigreeLink = false, bool displaySpecies = false, bool cursorHand = true) : this()
        {
            if (cursorHand)
                Cursor = Cursors.Hand;
            this.enabledColorRegions = enabledColorRegions;
            this.comboId = comboId;
            DisplaySpecies = displaySpecies;
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
                _labelsStats[s].Text = Utils.StatName(DisplayedStats[s], true, customStatNames);
                _ttMonospaced.SetToolTip(_labelsStats[s], Utils.StatName(DisplayedStats[s], customStatNames: customStatNames));
                _labelsStatsMut[s].Visible = false;
            }
            Height = ControlHeightWoMutations;
            labelMutations.Visible = true;
        }

        private void PedigreeCreature_Disposed(object sender, EventArgs e)
        {
            _ttMonospaced.RemoveAllAndDispose();
            _tt.RemoveAllAndDispose();
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

                foreach (var l in _labelsStatsMut) l.Visible = _creature.levelsMutated != null;
                Height = PedigreeCreation.PedigreeElementHeight;

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

                var levelColorOptions = Form1.StatsOptionsLevelColors.GetStatsOptions(Creature.Species);

                for (var s = 0; s < DisplayedStatsCount; s++)
                {
                    var si = DisplayedStats[s];
                    string tooltipText = null;
                    _labelsStatsMut[s].Visible = false;
                    if (_creature.valuesBreeding != null && _creature.valuesBreeding[si] == 0)
                    {
                        // stat not used
                        _labelsStats[s].Text = "-";
                        _labelsStats[s].BackColor = Color.WhiteSmoke;
                        _labelsStats[s].ForeColor = Color.LightGray;
                    }
                    else if (_creature.levelsWild == null || _creature.levelsWild[si] < 0)
                    {
                        _labelsStats[s].Text = "?";
                        _labelsStats[s].BackColor = Color.WhiteSmoke;
                        _labelsStats[s].ForeColor = Color.LightGray;
                    }
                    else if (_creature.levelsWild[si] == 0 && (_creature.Species?.stats[si].IncPerTamedLevel ?? -1) == 0)
                    {
                        // stat cannot be leveled, e.g. speed for flyers, and thus it's assumed there are no wild levels applied, i.e. irrelevant for breeding.
                        _labelsStats[s].Text = "0";
                        _labelsStats[s].BackColor = Color.WhiteSmoke;
                        _labelsStats[s].ForeColor = Color.LightGray;
                        tooltipText = Utils.StatName(si, false, _creature.Species?.statNames) + ": "
                            + $"{_creature.valuesBreeding[si] * (Stats.IsPercentage(si) ? 100 : 1),7:#,0.0}"
                            + (Stats.IsPercentage(si) ? "%" : string.Empty);
                    }
                    else
                    {
                        _labelsStats[s].Text = _creature.levelsWild[si].ToString();
                        if (Properties.Settings.Default.Highlight255Level && _creature.levelsWild[si] > 253) // 255 is max, 254 is the highest that allows dom leveling
                            _labelsStats[s].BackColor = Utils.AdjustColorLight(_creature.levelsWild[si] == 254 ? Utils.Level254 : Utils.Level255, _creature.IsTopStat(si) ? 0.2 : 0.7);
                        else
                            _labelsStats[s].BackColor = Utils.AdjustColorLight(levelColorOptions.StatOptions[si].GetLevelColor(_creature.levelsWild[si]),
                                _creature.IsTopStat(si) ? 0.2 : 0.7);

                        _labelsStats[s].ForeColor = Parent?.ForeColor ?? Color.Black; // needed so text is not transparent on overlay
                        var traitList = CreatureTrait.StringList(Creature.Traits?.Where(t => t.TraitDefinition.StatIndex == si), Environment.NewLine);
                        if (!string.IsNullOrEmpty(traitList)) traitList = Environment.NewLine + "Traits:" + Environment.NewLine + traitList;
                        tooltipText = Utils.StatName(si, false, _creature.Species?.statNames) + ": "
                            + $"{_creature.valuesBreeding[si] * (Stats.IsPercentage(si) ? 100 : 1),7:#,0.0}"
                            + (Stats.IsPercentage(si) ? "%" : string.Empty)
                            + (_creature.levelsMutated == null ? string.Empty
                                : Environment.NewLine + Loc.S("Mutation levels") + ": " + _creature.levelsMutated[si]
                                )
                            + traitList;
                    }

                    if (_creature.levelsMutated != null && _creature.levelsMutated[si] > 0)
                    {
                        _labelsStatsMut[s].Text = _creature.levelsMutated[si].ToString();
                        _labelsStatsMut[s].SetBackColorAndAccordingForeColor(Utils.AdjustColorLight(levelColorOptions.StatOptions[si].GetLevelColor(_creature.levelsMutated[si], mutationLevel: true),
                            _creature.IsTopMutationStat(si) ? 0.2 : 0.7));
                        _labelsStatsMut[s].Visible = true;
                    }

                    _ttMonospaced.SetToolTip(_labelsStats[s], tooltipText);
                    _ttMonospaced.SetToolTip(_labelsStatsMut[s], tooltipText);

                    // fonts are strange and this seems to work. The assigned font-object is probably only used to read out the properties and then not used anymore.
                    using (var font = new Font("Microsoft Sans Serif", 8.25F, _creature.IsTopStat(si) ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point, 0))
                        _labelsStats[s].Font = font;
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
            => pictureBox1.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(colorIds, null, enabledColorRegions, 24, 22, true,
                game: CreatureCollection.CurrentCreatureCollection?.Game));

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
            if (DisplaySpecies)
                groupBox1.Text += $" - {_creature.SpeciesName}";
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

        private void element_MouseClick(object sender, MouseEventArgs e) => PedigreeCreature_MouseClick(sender, e);

        /// <summary>
        /// Clears the displayed data.
        /// </summary>
        public void Clear()
        {
            for (int s = 0; s < DisplayedStatsCount; s++)
            {
                _labelsStats[s].Text = string.Empty;
                _labelsStats[s].BackColor = SystemColors.Control;
                _labelsStatsMut[s].Visible = false;
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

        private void plainTextbreedingValuesToolStripMenuItem_Click(object sender, EventArgs e) => ExportImportCreatures.ExportToClipboard(true, false, _creature);

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e) => ExportImportCreatures.ExportToClipboard(false, false, _creature);

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
                utils.ClipboardHandler.SetText(_creature.name);
        }

        private void copyInfoGraphicToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _creature?.ExportInfoGraphicToClipboard(CreatureCollection.CurrentCreatureCollection);
        }

        private void editTraitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TraitSelection.ShowTraitSelectionWindow(Creature.Traits?.ToList(),
                    $"Trait Selection for {Creature.name} ({Creature.Species})",
                    out var appliedTraits))
                return;
            Creature.Traits = appliedTraits?.ToArray();
            RecalculateBreedingPlan?.Invoke();
        }
    }
}

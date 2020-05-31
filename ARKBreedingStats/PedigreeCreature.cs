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
        /// Recalculate the breeding plan, e.g. if the cooldown was reset.
        /// </summary>
        public event Action RecalculateBreedingPlan;

        public delegate void ExportToClipboardEventHandler(Creature c, bool breedingValues, bool ARKml);

        public event ExportToClipboardEventHandler ExportToClipboard;
        private List<Label> labels;
        private readonly ToolTip tt;
        public int comboId;
        /// <summary>
        /// If set to true, the control will not display sex, status or creature colors.
        /// </summary>
        public bool OnlyLevels { get; set; }
        public bool[] enabledColorRegions;
        private bool contextMenuAvailable;
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
            tt = new ToolTip
            {
                InitialDelay = 100
            };
            tt.SetToolTip(labelSex, "Sex");
            tt.SetToolTip(labelMutations, "Mutation-Counter");
            labels = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp, labelCr };
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Disposed += PedigreeCreature_Disposed;
            comboId = -1;
        }

        public PedigreeCreature(Creature creature, bool[] enabledColorRegions, int comboId = -1) : this()
        {
            Cursor = Cursors.Hand;
            this.enabledColorRegions = enabledColorRegions;
            this.comboId = comboId;
            Creature = creature;
        }

        /// <summary>
        /// Set text of labels for stats. Only used for header control.
        /// </summary>
        public bool IsGlowSpecies
        {
            set
            {
                for (int s = 0; s < displayedStatsCount; s++)
                {
                    labels[s].Text = Utils.StatName(displayedStats[s], true, value);
                    tt.SetToolTip(labels[s], Utils.StatName(displayedStats[s], glowSpecies: value));
                }
            }
        }

        private void PedigreeCreature_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        private Creature creature;
        /// <summary>
        /// The creature that is displayed in this control.
        /// </summary>
        public Creature Creature
        {
            get => creature;
            set
            {
                if (value != null)
                {
                    creature = value;
                    SetTitle();

                    if (!OnlyLevels)
                    {
                        if (creature.Status == CreatureStatus.Dead)
                        {
                            groupBox1.ForeColor = SystemColors.GrayText;
                            tt.SetToolTip(groupBox1, "Creature has passed away");
                        }
                        else if (creature.Status == CreatureStatus.Unavailable)
                        {
                            groupBox1.ForeColor = SystemColors.GrayText;
                            tt.SetToolTip(groupBox1, "Creature is currently not available");
                        }
                        else if (creature.Status == CreatureStatus.Obelisk)
                        {
                            groupBox1.ForeColor = SystemColors.GrayText;
                            tt.SetToolTip(groupBox1, "Creature is currently uploaded in an obelisk");
                        }
                    }

                    tt.SetToolTip(labelSex, "Sex: " + Loc.S(creature.sex.ToString()));
                    bool isGlowSpecies = creature.Species?.IsGlowSpecies ?? false;
                    for (int s = 0; s < displayedStatsCount; s++)
                    {
                        int si = displayedStats[s];
                        if (creature.valuesBreeding[si] == 0)
                        {
                            // stat not used // TODO hide label?
                            labels[s].Text = "-";
                            labels[s].BackColor = Color.WhiteSmoke;
                            labels[s].ForeColor = Color.LightGray;
                        }
                        else if (creature.levelsWild[si] < 0)
                        {
                            labels[s].Text = "?";
                            labels[s].BackColor = Color.WhiteSmoke;
                            labels[s].ForeColor = Color.LightGray;
                        }
                        else
                        {
                            labels[s].Text = creature.levelsWild[si].ToString();
                            labels[s].BackColor = Utils.GetColorFromPercent((int)(creature.levelsWild[si] * 2.5), creature.topBreedingStats[si] ? 0.2 : 0.7);
                            labels[s].ForeColor = SystemColors.ControlText;
                            tt.SetToolTip(labels[s], Utils.StatName(si, false, isGlowSpecies) + ": " + creature.valuesBreeding[si] * (Utils.Precision(si) == 3 ? 100 : 1) + (Utils.Precision(si) == 3 ? "%" : string.Empty));
                        }
                        labels[s].Font = new Font("Microsoft Sans Serif", 8.25F, creature.topBreedingStats[si] ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point, 0);
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
                        labelSex.Text = Utils.SexSymbol(creature.sex);
                        labelSex.BackColor = creature.flags.HasFlag(CreatureFlags.Neutered) ? SystemColors.GrayText : Utils.SexColor(creature.sex);
                        // creature Colors
                        pictureBox1.Image = CreatureColored.GetColoredCreature(creature.colors, null, enabledColorRegions, 24, 22, true);
                        tt.SetToolTip(pictureBox1, CreatureColored.RegionColorInfo(creature.Species, creature.colors));
                        labelSex.Visible = true;
                        pictureBox1.Visible = true;
                        plainTextcurrentValuesToolStripMenuItem.Visible = true;
                    }
                    int totalMutations = creature.Mutations;
                    if (totalMutations > 0)
                    {
                        labelMutations.Text = totalMutations.ToString();
                        if (totalMutations > 9999)
                            labelMutations.Text = totalMutations.ToString().Substring(0, 4) + "…";
                        if (totalMutations > 19)
                            labelMutations.BackColor = Utils.MutationColorOverLimit;
                        else
                            labelMutations.BackColor = Utils.MutationColor;
                        tt.SetToolTip(labelMutations, "Mutation-Counter: " + totalMutations.ToString("N0") + "\nMaternal: " + creature.mutationsMaternal.ToString("N0") + "\nPaternal: " + creature.mutationsPaternal.ToString("N0"));
                    }
                    labelMutations.Visible = totalMutations > 0;
                    contextMenuAvailable = true;
                }
            }
        }

        /// <summary>
        /// Sets the displayed title of the control.
        /// </summary>
        private void SetTitle()
        {
            string totalLevel = creature.LevelHatched > 0 ? creature.LevelHatched.ToString() : "?";
            groupBox1.Text = (!OnlyLevels && creature.Status != CreatureStatus.Available ? "(" + Utils.StatusSymbol(creature.Status) + ") " : string.Empty)
                    + creature.name + " (" + totalLevel + (TotalLevelUnknown ? "+" : string.Empty) + ")";

            if (creature.growingUntil > DateTime.Now)
                groupBox1.Text += " (grown at " + Utils.ShortTimeDate(creature.growingUntil) + ")";
            else if (creature.cooldownUntil > DateTime.Now)
                groupBox1.Text += " (cooldown until " + Utils.ShortTimeDate(creature.cooldownUntil) + ")";
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
                CreatureClicked(creature, comboId, e);
        }

        private void element_MouseClick(object sender, MouseEventArgs e)
        {
            PedigreeCreature_MouseClick(sender, e);
        }

        public void Clear()
        {
            for (int s = 0; s < displayedStatsCount; s++)
            {
                labels[s].Text = string.Empty;
                labels[s].BackColor = SystemColors.Control;
            }
            labelSex.Visible = false;
            groupBox1.Text = string.Empty;
            pictureBox1.Visible = false;
        }

        private bool isVirtual;
        /// <summary>
        /// If a creature is virtual, it is not a creature in the library.
        /// </summary>
        public bool IsVirtual
        {
            get => isVirtual;
            set
            {
                isVirtual = value;
                setCooldownToolStripMenuItem.Visible = !value;
                removeCooldownGrowingToolStripMenuItem.Visible = !value;
                bestBreedingPartnersToolStripMenuItem.Visible = !value;
                editToolStripMenuItem.Text = value ? "Copy values to Tester" : "Edit";
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreatureEdit?.Invoke(creature, isVirtual);
        }

        private void setCooldownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            creature.cooldownUntil = DateTime.Now.AddHours(2);
            RecalculateBreedingPlan?.Invoke();
            SetTitle();
        }

        private void removeCooldownGrowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (creature.cooldownUntil > DateTime.Now)
                creature.cooldownUntil = DateTime.Now;
            if (creature.growingUntil > DateTime.Now)
                creature.growingUntil = DateTime.Now;
            SetTitle();
        }

        private void bestBreedingPartnersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BestBreedingPartners?.Invoke(creature);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (!contextMenuAvailable)
            {
                e.Cancel = true;
            }
        }

        private void plainTextbreedingValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToClipboard?.Invoke(creature, true, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToClipboard?.Invoke(creature, false, false);
        }

        private void OpenWikipageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (creature?.Species != null)
                System.Diagnostics.Process.Start("https://ark.gamepedia.com/" + creature.Species.name);
        }
    }
}

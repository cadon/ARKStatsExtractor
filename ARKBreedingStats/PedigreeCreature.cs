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
        private Creature creature;
        private bool isVirtual; // set to true for not existing creatures (e.g. possible offspring)

        public delegate void CreatureChangedEventHandler(Creature creature, int comboId, MouseEventArgs e);

        public event CreatureChangedEventHandler CreatureClicked;

        public delegate void CreatureEditEventHandler(Creature creature, bool isVirtual);

        public event CreatureEditEventHandler CreatureEdit;

        public delegate void CreaturePartnerEventHandler(Creature creature);

        public event CreaturePartnerEventHandler BestBreedingPartners;

        public delegate void BPRecalcEventHandler();

        public event BPRecalcEventHandler BPRecalc;

        public delegate void ExportToClipboardEventHandler(Creature c, bool breedingValues, bool ARKml);

        public event ExportToClipboardEventHandler exportToClipboard;
        private List<Label> labels;
        ToolTip tt = new ToolTip();
        public int comboId;
        public bool onlyLevels; // no sex, status, colors
        public bool[] enabledColorRegions;
        private bool contextMenuAvailable;
        public bool totalLevelUnknown = false; // if set to true, the levelHatched in parenthesis is appended with an '+'

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

        public PedigreeCreature()
        {
            InitC();
            comboId = -1;
        }

        private void InitC()
        {
            InitializeComponent();
            tt.InitialDelay = 100;
            tt.SetToolTip(labelSex, "Sex");
            tt.SetToolTip(labelMutations, "Mutation-Counter");
            labels = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp, labelCr };
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Disposed += PedigreeCreature_Disposed;
        }

        /// <summary>
        /// Set text of labels for stats. Only used for header control.
        /// </summary>
        public bool IsGlowSpecies
        {
            set
            {
                for (int s = 0; s < 8; s++) // only 8 stats are displayed
                {
                    labels[s].Text = Utils.statName(displayedStats[s], true, value);
                }
                tt.SetToolTip(labelHP, Utils.statName(StatNames.Health, glow: value));
                tt.SetToolTip(labelSt, Utils.statName(StatNames.Stamina, glow: value));
                tt.SetToolTip(labelOx, Utils.statName(StatNames.Oxygen, glow: value));
                tt.SetToolTip(labelFo, Utils.statName(StatNames.Food, glow: value));
                tt.SetToolTip(labelWe, Utils.statName(StatNames.Weight, glow: value));
                tt.SetToolTip(labelDm, Utils.statName(StatNames.MeleeDamageMultiplier, glow: value));
                tt.SetToolTip(labelSp, Utils.statName(StatNames.SpeedMultiplier, glow: value));
                tt.SetToolTip(labelCr, Utils.statName(StatNames.CraftingSpeedMultiplier, glow: value));
            }
        }

        private void PedigreeCreature_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        public PedigreeCreature(Creature creature, bool[] enabledColorRegions, int comboId = -1)
        {
            InitC();
            Cursor = Cursors.Hand;
            this.enabledColorRegions = enabledColorRegions;
            this.comboId = comboId;
            Creature = creature;
        }

        public Creature Creature
        {
            get => creature;
            set
            {
                if (value != null)
                {
                    creature = value;
                    bool isGlowSpecies = Values.V.IsGlowSpecies(creature.Species.name);
                    setTitle();

                    if (!onlyLevels)
                    {
                        if (creature.status == CreatureStatus.Dead)
                        {
                            groupBox1.ForeColor = SystemColors.GrayText;
                            tt.SetToolTip(groupBox1, "Creature has passed away");
                        }
                        else if (creature.status == CreatureStatus.Unavailable)
                        {
                            groupBox1.ForeColor = SystemColors.GrayText;
                            tt.SetToolTip(groupBox1, "Creature is currently not available");
                        }
                        else if (creature.status == CreatureStatus.Obelisk)
                        {
                            groupBox1.ForeColor = SystemColors.GrayText;
                            tt.SetToolTip(groupBox1, "Creature is currently uploaded in an obelisk");
                        }
                    }

                    tt.SetToolTip(labelSex, "Sex: " + Loc.s(creature.sex.ToString()));
                    for (int s = 0; s < 8; s++)
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
                            labels[s].BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[si] * 2.5), creature.topBreedingStats[si] ? 0.2 : 0.7);
                            labels[s].ForeColor = SystemColors.ControlText;
                            tt.SetToolTip(labels[s], Utils.statName(si, false, isGlowSpecies) + ": " + creature.valuesBreeding[si] * (Utils.precision(si) == 3 ? 100 : 1) + (Utils.precision(si) == 3 ? "%" : ""));
                        }
                        labels[s].Font = new Font("Microsoft Sans Serif", 8.25F, creature.topBreedingStats[si] ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point, 0);
                    }
                    if (onlyLevels)
                    {
                        labelSex.Visible = false;
                        pictureBox1.Visible = false;
                        plainTextcurrentValuesToolStripMenuItem.Visible = false;
                    }
                    else
                    {
                        labelSex.Visible = true;
                        labelSex.Text = Utils.sexSymbol(creature.sex);
                        labelSex.BackColor = creature.neutered ? SystemColors.GrayText : Utils.sexColor(creature.sex);
                        // creature Colors
                        pictureBox1.Image = CreatureColored.getColoredCreature(creature.colors, null, enabledColorRegions, 24, 22, true);
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

        private void setTitle()
        {
            string totalLevel = creature.levelHatched > 0 ? creature.levelHatched.ToString() : "?";
            groupBox1.Text = (!onlyLevels && creature.status != CreatureStatus.Available ? "(" + Utils.statusSymbol(creature.status) + ") " : "")
                    + creature.name + " (" + totalLevel + (totalLevelUnknown ? "+" : "") + ")";

            if (creature.growingUntil > DateTime.Now)
                groupBox1.Text += " (grown at " + Utils.shortTimeDate(creature.growingUntil) + ")";
            else if (creature.cooldownUntil > DateTime.Now)
                groupBox1.Text += " (cooldown until " + Utils.shortTimeDate(creature.cooldownUntil) + ")";
        }

        public bool highlight
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
            for (int s = 0; s < 7; s++)
            {
                labels[s].Text = "";
                labels[s].BackColor = SystemColors.Control;
            }
            labelSex.Visible = false;
            groupBox1.Text = "";
            pictureBox1.Visible = false;
        }

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
            BPRecalc?.Invoke();
            setTitle();
        }

        private void removeCooldownGrowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (creature.cooldownUntil > DateTime.Now)
                creature.cooldownUntil = DateTime.Now;
            if (creature.growingUntil > DateTime.Now)
                creature.growingUntil = DateTime.Now;
            setTitle();
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
            exportToClipboard?.Invoke(creature, true, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportToClipboard?.Invoke(creature, false, false);
        }

        private void OpenWikipageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (creature?.Species != null)
                System.Diagnostics.Process.Start("https://ark.gamepedia.com/" + creature.Species.name);
        }
    }
}

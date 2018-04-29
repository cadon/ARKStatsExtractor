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

    public partial class PedigreeCreature : UserControl
    {
        private Creature creature;
        private bool isVirtual = false; // set to true for not existing creatures (e.g. possible offspring)
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
        private bool contextMenuAvailable = false;
        public bool totalLevelUnknown = false; // if set to true, the levelHatched in parenthesis is appended with an '+'

        public PedigreeCreature()
        {
            InitC();
            this.comboId = -1;
        }
        private void InitC()
        {
            InitializeComponent();
            tt.InitialDelay = 100;
            tt.SetToolTip(labelHP, "Health");
            tt.SetToolTip(labelSt, "Stamina");
            tt.SetToolTip(labelOx, "Oxygen");
            tt.SetToolTip(labelFo, "Food");
            tt.SetToolTip(labelWe, "Weight");
            tt.SetToolTip(labelDm, "Melee Damage");
            tt.SetToolTip(labelSp, "Speed");
            tt.SetToolTip(labelSex, "Sex");
            tt.SetToolTip(labelMutations, "Mutation-Counter");
            labels = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp };
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Disposed += PedigreeCreature_Disposed;
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
            set
            {
                if (value != null)
                {
                    creature = value;
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

                    for (int s = 0; s < 7; s++)
                    {
                        if (creature.levelsWild[s] < 0)
                        {
                            labels[s].Text = "?";
                            labels[s].BackColor = Color.WhiteSmoke;
                            labels[s].ForeColor = Color.LightGray;
                        }
                        else
                        {
                            labels[s].Text = creature.levelsWild[s].ToString();
                            labels[s].BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[s] * 2.5), (creature.topBreedingStats[s] ? 0.2 : 0.7));
                            labels[s].ForeColor = SystemColors.ControlText;
                            tt.SetToolTip(labels[s], Utils.statName(s) + ": " + (creature.valuesBreeding[s] * (Utils.precision(s) == 3 ? 100 : 1)).ToString() + (Utils.precision(s) == 3 ? "%" : ""));
                        }
                        labels[s].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, (creature.topBreedingStats[s] ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    }
                    if (onlyLevels)
                    {
                        labelSex.Visible = false;
                        pictureBox1.Visible = false;
                        plainTextcurrentValuesToolStripMenuItem.Visible = false;
                        aRKChatcurrentValuesToolStripMenuItem.Visible = false;
                    }
                    else
                    {
                        labelSex.Visible = true;
                        labelSex.Text = Utils.sexSymbol(creature.sex);
                        labelSex.BackColor = creature.neutered ? SystemColors.GrayText : Utils.sexColor(creature.sex);
                        // creature Colors
                        pictureBox1.Image = CreatureColored.getColoredCreature(creature.colors, "", enabledColorRegions, 24, 22, true);
                        labelSex.Visible = true;
                        pictureBox1.Visible = true;
                        plainTextcurrentValuesToolStripMenuItem.Visible = true;
                        aRKChatcurrentValuesToolStripMenuItem.Visible = true;
                    }
                    labelMutations.BackColor = Color.FromArgb(225, 192, 255);
                    labelMutations.Text = (creature.mutationsMaternal + creature.mutationsPaternal).ToString();
                    labelMutations.Visible = (creature.mutationsMaternal + creature.mutationsPaternal) > 0;
                    contextMenuAvailable = true;
                }
            }
            get { return creature; }
        }

        private void setTitle()
        {
            groupBox1.Text = (!onlyLevels && creature.status != CreatureStatus.Available ? "(" + Utils.statusSymbol(creature.status) + ") " : "")
                + creature.name + " (" + creature.levelHatched + (totalLevelUnknown ? "+" : "") + ")";

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

        public bool HandCursor { set { Cursor = (value ? Cursors.Hand : Cursors.Default); } }

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
            set
            {
                isVirtual = value;
                setCooldownToolStripMenuItem.Visible = !value;
                bestBreedingPartnersToolStripMenuItem.Visible = !value;
                if (value)
                {
                    editToolStripMenuItem.Text = "Copy values to Tester";
                }
                else
                {
                    editToolStripMenuItem.Text = "Edit";
                }
            }
            get { return isVirtual; }
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

        private void aRKChatbreedingValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportToClipboard?.Invoke(creature, true, true);
        }

        private void aRKChatcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportToClipboard?.Invoke(creature, false, true);
        }

        private void plainTextbreedingValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportToClipboard?.Invoke(creature, true, false);
        }

        private void plainTextcurrentValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportToClipboard?.Invoke(creature, false, false);
        }
    }
}

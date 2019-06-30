using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class Pedigree : UserControl
    {
        public delegate void EditCreatureEventHandler(Creature creature, bool virtualCreature);

        public event EditCreatureEventHandler EditCreature;
        public event PedigreeCreature.CreaturePartnerEventHandler BestBreedingPartners;
        public event PedigreeCreature.ExportToClipboardEventHandler exportToClipboard;
        public List<Creature> creatures;
        public Creature creature;
        private List<Creature> children = new List<Creature>();
        private readonly List<List<int[]>> lines = new List<List<int[]>>();
        private List<PedigreeCreature> pcs = new List<PedigreeCreature>();
        private bool[] enabledColorRegions = { true, true, true, true, true, true };

        public Pedigree()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            lines.Add(new List<int[]>());
            lines.Add(new List<int[]>());
            noCreatureSelected();
            listViewCreatures.ListViewItemSorter = new ListViewColumnSorter();
            splitContainer1.Panel2.Paint += Panel2_Paint;
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(splitContainer1.Panel2.AutoScrollPosition.X, splitContainer1.Panel2.AutoScrollPosition.Y);
            if (creature != null)
                drawLines(e.Graphics);
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    // paintBase
        //    base.OnPaint(e);

        //    e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
        //    if (creature != null)
        //        drawLines(e.Graphics);
        //}

        public void drawLines(Graphics g)
        {
            // lines contains all the coordinates the arrows should be drawn: x1,y1,x2,y2,red/green,mutated/equal
            using (Pen myPen = new Pen(Color.Green, 3))
            {
                myPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (int[] line in lines[0])
                {
                    if (line[4] == 1)
                        myPen.Color = Color.DarkRed;
                    else if (line[4] == 2)
                        myPen.Color = Color.Green;
                    else
                        myPen.Color = Color.LightGray;
                    if (line[5] > 0)
                    {
                        // if stat is mutated
                        int mutationBoxWidth = 14;
                        g.FillEllipse(Brushes.LightGreen, (line[0] + line[2] - mutationBoxWidth) / 2, (line[1] + line[3] - mutationBoxWidth) / 2, mutationBoxWidth, mutationBoxWidth);
                    }
                    g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
                }
                myPen.Color = Color.DarkGray;
                myPen.Width = 1;
                foreach (int[] line in lines[1])
                {
                    g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
                }
                if (children.Count > 0)
                    g.DrawString("Descendants", new Font("Arial", 14), new SolidBrush(Color.Black), 210, 170);
            }
        }

        public void Clear()
        {
            creature = null;
            ClearControls();
            noCreatureSelected();
        }

        public void ClearControls()
        {
            // clear pedigree   
            SuspendLayout();
            foreach (PedigreeCreature pc in pcs)
                pc.Dispose();
            lines.Clear();
            lines.Add(new List<int[]>());
            lines.Add(new List<int[]>());
            pictureBox.Image = null;
            ResumeLayout();
        }

        /// <summary>
        /// call this function to create the pedigreeCreature-Elements
        /// </summary>
        public void createPedigree()
        {
            // clear old pedigreeCreatures
            ClearControls();
            if (creature != null)
            {
                SuspendLayout();

                bool isGlowSpecies = Values.V.IsGlowSpecies(creature.Species?.name);
                pedigreeCreature1.IsGlowSpecies = isGlowSpecies;

                int leftBorder = 40;
                int pedigreeElementWidth = 325;
                int margin = 10;

                lbPedigreeEmpty.Visible = false;

                // create ancestors
                createParentsChild(creature, leftBorder + pedigreeElementWidth + margin, 60, true, true);
                if (creature.Mother != null)
                {
                    if (createParentsChild(creature.Mother, leftBorder, 20))
                        lines[1].Add(new[] { leftBorder + pedigreeElementWidth, 79, leftBorder + pedigreeElementWidth + margin, 79 });
                }
                if (creature.Father != null)
                {
                    if (createParentsChild(creature.Father, leftBorder + 2 * (pedigreeElementWidth + margin), 20))
                        lines[1].Add(new[] { leftBorder + 2 * pedigreeElementWidth + 2 * margin, 79, leftBorder + 2 * pedigreeElementWidth + margin, 159 });
                }

                // create descendants
                int row = 0;
                // scrolloffsets
                int xS = AutoScrollPosition.X;
                int yS = AutoScrollPosition.Y;
                foreach (Creature c in children)
                {
                    PedigreeCreature pc = new PedigreeCreature(c, enabledColorRegions)
                    {
                        Location = new Point(leftBorder + xS, 200 + 35 * row + yS)
                    };
                    for (int s = 0; s < PedigreeCreature.displayedStats.Length; s++)
                    {
                        int si = PedigreeCreature.displayedStats[s];
                        if (creature.valuesDom[si] > 0 && creature.levelsWild[si] >= 0 && creature.levelsWild[si] == c.levelsWild[si])
                            lines[0].Add(new[] { leftBorder + 38 + 29 * s, 200 + 35 * row + 6, leftBorder + 38 + 29 * s, 200 + 35 * row + 15, 0, 0 });
                    }
                    pc.CreatureClicked += CreatureClicked;
                    pc.CreatureEdit += CreatureEdit;
                    pc.BestBreedingPartners += BestBreedingPartners;
                    pc.exportToClipboard += exportToClipboard;
                    splitContainer1.Panel2.Controls.Add(pc);
                    pcs.Add(pc);
                    row++;
                }

                pictureBox.Image = CreatureColored.getColoredCreature(creature.colors, creature.Species, enabledColorRegions, 256);

                Invalidate();
                ResumeLayout();
            }
            else
            {
                noCreatureSelected();
            }
        }

        private bool createParentsChild(Creature creature, int x, int y, bool drawWithNoParents = false, bool highlightCreature = false)
        {
            if (creature != null && (drawWithNoParents || creature.Mother != null || creature.Father != null))
            {
                // scrolloffset for control-locations (not for lines)
                int xS = AutoScrollPosition.X;
                int yS = AutoScrollPosition.Y;
                // creature
                PedigreeCreature pc = new PedigreeCreature(creature, enabledColorRegions);
                if (highlightCreature)
                    pc.highlight = true;
                pc.Location = new Point(x + xS, y + yS + 40);
                splitContainer1.Panel2.Controls.Add(pc);
                pc.CreatureClicked += CreatureClicked;
                pc.CreatureEdit += CreatureEdit;
                pc.BestBreedingPartners += BestBreedingPartners;
                pc.exportToClipboard += exportToClipboard;
                pcs.Add(pc);
                // mother
                if (creature.Mother != null)
                {
                    pc = new PedigreeCreature(creature.Mother, enabledColorRegions)
                    {
                        Location = new Point(x + xS, y + yS)
                    };
                    splitContainer1.Panel2.Controls.Add(pc);
                    pc.CreatureClicked += CreatureClicked;
                    pc.CreatureEdit += CreatureEdit;
                    pc.BestBreedingPartners += BestBreedingPartners;
                    pc.exportToClipboard += exportToClipboard;
                    pcs.Add(pc);
                }
                // father
                if (creature.Father != null)
                {
                    pc = new PedigreeCreature(creature.Father, enabledColorRegions)
                    {
                        Location = new Point(x + xS, y + yS + 80)
                    };
                    splitContainer1.Panel2.Controls.Add(pc);
                    pc.CreatureClicked += CreatureClicked;
                    pc.CreatureEdit += CreatureEdit;
                    pc.BestBreedingPartners += BestBreedingPartners;
                    pc.exportToClipboard += exportToClipboard;
                    pcs.Add(pc);
                }
                // gene-inheritance-lines
                // better: if father < mother: 1, if mother < father: -1
                for (int s = 0; s < PedigreeCreature.displayedStats.Length; s++)
                {
                    int si = PedigreeCreature.displayedStats[s];
                    if (creature.valuesDom[si] <= 0) continue; // don't display arrows for non used stats
                    int better = 0;
                    if (creature.Mother != null && creature.Father != null)
                    {
                        if (creature.Mother.levelsWild[si] < creature.Father.levelsWild[si])
                            better = -1;
                        else if (creature.Mother.levelsWild[si] > creature.Father.levelsWild[si])
                            better = 1;
                    }
                    // offspring can have stats that are up to 2 levels higher due to mutations. currently there are no decreasing levels due to mutations
                    if (creature.Mother != null && creature.levelsWild[si] >= 0 && (creature.levelsWild[si] == creature.Mother.levelsWild[si] || creature.levelsWild[si] == creature.Mother.levelsWild[si] + 2))
                    {
                        lines[0].Add(new[] { 38 + x + 29 * s, y + 33, 38 + x + 29 * s, y + 42, (better == -1 ? 1 : 2), (creature.levelsWild[si] > creature.Mother.levelsWild[si] ? 1 : 0) });
                    }
                    if (creature.Father != null && creature.levelsWild[si] >= 0 && (creature.levelsWild[si] == creature.Father.levelsWild[si] || creature.levelsWild[si] == creature.Father.levelsWild[si] + 2))
                    {
                        lines[0].Add(new[] { 38 + x + 29 * s, y + 83, 38 + x + 29 * s, y + 74, (better == 1 ? 1 : 2), (creature.levelsWild[si] > creature.Father.levelsWild[si] ? 1 : 0) });
                    }
                }
                return true;
            }
            return false;
        }

        private void CreatureClicked(Creature c, int comboIndex, MouseEventArgs e)
        {
            setCreature(c);
        }

        private void CreatureEdit(Creature c, bool isVirtual)
        {
            EditCreature?.Invoke(c, isVirtual);
        }

        public void setCreature(Creature centralCreature, bool forceUpdate = false)
        {
            if (centralCreature == null)
            {
                creature = null;
                ClearControls();
            }
            else if (creatures != null && (centralCreature != creature || forceUpdate))
            {
                creature = centralCreature;
                // set children
                children = creatures.Where(cr => cr.motherGuid == creature.guid || cr.fatherGuid == creature.guid)
                        .OrderBy(cr => cr.name)
                        .ToList();

                // select creature in listView
                if (listViewCreatures.SelectedItems.Count == 0 || (Creature)listViewCreatures.SelectedItems[0].Tag != centralCreature)
                {
                    int index = -1;
                    for (int i = 0; i < listViewCreatures.Items.Count; i++)
                    {
                        if ((Creature)listViewCreatures.Items[i].Tag == centralCreature)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index >= 0)
                    {
                        listViewCreatures.Items[index].Selected = true;
                        listViewCreatures.EnsureVisible(index);
                    }
                }

                createPedigree();
            }
        }

        private void noCreatureSelected()
        {
            lbPedigreeEmpty.Visible = true;
        }

        public bool[] EnabledColorRegions
        {
            set
            {
                if (value != null && value.Length == 6)
                {
                    enabledColorRegions = value;
                }
                else
                {
                    enabledColorRegions = new[] { true, true, true, true, true, true };
                }
            }
        }

        private void listViewCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCreatures.SelectedIndices.Count > 0)
            {
                setCreature((Creature)listViewCreatures.SelectedItems[0].Tag);
            }
        }

        public void updateListView()
        {
            listViewCreatures.BeginUpdate();

            // clear ListView
            listViewCreatures.Items.Clear();
            listViewCreatures.Groups.Clear();

            // add groups for each species (so they are sorted alphabetically)
            foreach (string s in Values.V.speciesNames)
            {
                listViewCreatures.Groups.Add(new ListViewGroup(s));
            }

            foreach (Creature cr in creatures)
            {
                // check if group of species exists
                ListViewGroup g = null;
                foreach (ListViewGroup lvg in listViewCreatures.Groups)
                {
                    if (lvg.Header == cr.Species.name)
                    {
                        g = lvg;
                        break;
                    }
                }
                if (g == null)
                {
                    g = new ListViewGroup(cr.Species.name);
                    listViewCreatures.Groups.Add(g);
                }
                string crLevel = cr.levelHatched > 0 ? cr.levelHatched.ToString() : "?";
                ListViewItem lvi = new ListViewItem(new[] { cr.name, crLevel }, g);
                lvi.Tag = cr;
                lvi.UseItemStyleForSubItems = false;
                if (cr.IsPlaceholder)
                    lvi.SubItems[0].ForeColor = Color.LightGray;
                if (crLevel == "?")
                    lvi.SubItems[1].ForeColor = Color.LightGray;
                listViewCreatures.Items.Add(lvi);
            }
            listViewCreatures.EndUpdate();
        }

        private void listViewCreatures_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.doSort((ListView)sender, e.Column);
        }

        public void SetLocalizations()
        {
            Loc.ControlText(lbPedigreeEmpty);
        }
    }
}

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
    public partial class Pedigree : UserControl
    {
        public delegate void EditCreatureEventHandler(Creature creature, bool virtualCreature);
        public event EditCreatureEventHandler EditCreature;
        public event PedigreeCreature.CreaturePartnerEventHandler BestBreedingPartners;
        public event PedigreeCreature.ExportToClipboardEventHandler exportToClipboard;
        public List<Creature> creatures;
        public Creature creature;
        public List<Creature> children = new List<Creature>();
        private List<List<int[]>> lines = new List<List<int[]>>();
        private List<PedigreeCreature> pcs = new List<PedigreeCreature>();
        private bool[] enabledColorRegions = new bool[] { true, true, true, true, true, true };

        public Pedigree()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            lines.Add(new List<int[]>());
            lines.Add(new List<int[]>());
            noCreatureSelected();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // paintBase
            base.OnPaint(e);

            e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
            if (creature != null)
                drawLines(e.Graphics);
        }

        public void drawLines(Graphics g)
        {
            // lines contains all the coordinates the arrows should be drawn: x1,y1,x2,y2,red/green,mutated/equal
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Green, 3);
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
                g.DrawString("Descendants", new System.Drawing.Font("Arial", 14), new System.Drawing.SolidBrush(System.Drawing.Color.Black), 210, 170);
            myPen.Dispose();
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
            this.SuspendLayout();
            foreach (PedigreeCreature pc in pcs)
                pc.Dispose();
            lines.Clear();
            lines.Add(new List<int[]>());
            lines.Add(new List<int[]>());
            pictureBox.Image = null;
            this.ResumeLayout();
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
                this.SuspendLayout();

                int leftBorder = 200;

                labelEmptyInfo.Visible = false;

                // create ancestors
                createParentsChild(creature, leftBorder + 325, 60, true, true);
                if (creature.Mother != null)
                {
                    if (createParentsChild(creature.Mother, leftBorder + 10, 20, false))
                        lines[1].Add(new int[] { leftBorder + 306, 79, leftBorder + 325, 79 });
                }
                if (creature.Father != null)
                {
                    if (createParentsChild(creature.Father, leftBorder + 640, 20, false))
                        lines[1].Add(new int[] { leftBorder + 640, 79, leftBorder + 621, 159 });
                }

                // create descendants
                int row = 0;
                // scrolloffsets
                int xS = AutoScrollPosition.X;
                int yS = AutoScrollPosition.Y;
                foreach (Creature c in children)
                {
                    PedigreeCreature pc = new PedigreeCreature(c, enabledColorRegions);
                    pc.Location = new Point(leftBorder + 10 + xS, 200 + 35 * row + yS);
                    for (int s = 0; s < 7; s++)
                    {
                        if (creature.levelsWild[s] >= 0 && creature.levelsWild[s] == c.levelsWild[s])
                            lines[0].Add(new int[] { leftBorder + 10 + 38 + 28 * s, 200 + 35 * row + 6, leftBorder + 10 + 38 + 28 * s, 200 + 35 * row + 15, 0, 0 });
                    }
                    pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                    pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                    pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                    pc.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
                    Controls.Add(pc);
                    pcs.Add(pc);
                    row++;
                }

                pictureBox.Image = CreatureColored.getColoredCreature(creature.colors, creature.species, enabledColorRegions, 256);

                this.Invalidate();
                this.ResumeLayout();
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
                Controls.Add(pc);
                pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                pc.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
                pcs.Add(pc);
                // mother
                if (creature.Mother != null)
                {
                    pc = new PedigreeCreature(creature.Mother, enabledColorRegions);
                    pc.Location = new Point(x + xS, y + yS);
                    Controls.Add(pc);
                    pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                    pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                    pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                    pc.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
                    pcs.Add(pc);
                }
                // father
                if (creature.Father != null)
                {
                    pc = new PedigreeCreature(creature.Father, enabledColorRegions);
                    pc.Location = new Point(x + xS, y + yS + 80);
                    Controls.Add(pc);
                    pc.CreatureClicked += new PedigreeCreature.CreatureChangedEventHandler(CreatureClicked);
                    pc.CreatureEdit += new PedigreeCreature.CreatureEditEventHandler(CreatureEdit);
                    pc.BestBreedingPartners += new PedigreeCreature.CreaturePartnerEventHandler(BestBreedingPartners);
                    pc.exportToClipboard += new PedigreeCreature.ExportToClipboardEventHandler(exportToClipboard);
                    pcs.Add(pc);
                }
                // gene-inheritance-lines
                // better: if father < mother: 1, if mother < father: -1
                int better;
                for (int s = 0; s < 7; s++)
                {
                    better = 0;
                    if (creature.Mother != null && creature.Father != null)
                    {
                        if (creature.Mother.levelsWild[s] < creature.Father.levelsWild[s])
                            better = -1;
                        else if (creature.Mother.levelsWild[s] > creature.Father.levelsWild[s])
                            better = 1;
                    }
                    // offspring can have stats that are up to 2 levels higher due to mutations. currently there are no decreasing levels due to mutations
                    if (creature.Mother != null && creature.levelsWild[s] >= 0 && (creature.levelsWild[s] == creature.Mother.levelsWild[s] || creature.levelsWild[s] == creature.Mother.levelsWild[s] + 2))
                    {
                        lines[0].Add(new int[] { 38 + x + 28 * s, y + 33, 38 + x + 28 * s, y + 42, (better == -1 ? 1 : 2), (creature.levelsWild[s] > creature.Mother.levelsWild[s] ? 1 : 0) });
                    }
                    if (creature.Father != null && creature.levelsWild[s] >= 0 && (creature.levelsWild[s] == creature.Father.levelsWild[s] || creature.levelsWild[s] == creature.Father.levelsWild[s] + 2))
                    {
                        lines[0].Add(new int[] { 38 + x + 28 * s, y + 83, 38 + x + 28 * s, y + 74, (better == 1 ? 1 : 2), (creature.levelsWild[s] > creature.Father.levelsWild[s] ? 1 : 0) });
                    }
                }
                return true;
            }
            return false;
        }

        private void CreatureClicked(Creature c, int comboIndex, MouseEventArgs e)
        {
            setCreature(c, false);
        }

        private void CreatureEdit(Creature c, bool isVirtual)
        {
            if (EditCreature != null)
                EditCreature(c, isVirtual);
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
                var children = from cr in creatures
                               where cr.motherGuid == creature.guid
                               || cr.fatherGuid == creature.guid
                               orderby cr.name ascending
                               select cr;
                this.children = children.ToList();

                // select creature in listView
                if (listViewCreatures.SelectedItems.Count == 0 || ((Creature)listViewCreatures.SelectedItems[0].Tag) != centralCreature)
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
            labelEmptyInfo.Visible = true;
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
                    enabledColorRegions = new bool[] { true, true, true, true, true, true };
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
                    if (lvg.Header == cr.species)
                    {
                        g = lvg;
                        break;
                    }
                }
                if (g == null)
                {
                    g = new ListViewGroup(cr.species);
                    listViewCreatures.Groups.Add(g);
                }
                ListViewItem lvi = new ListViewItem(new string[] { cr.name, cr.levelHatched.ToString() }, g);
                lvi.Tag = cr;
                listViewCreatures.Items.Add(lvi);
            }
            listViewCreatures.EndUpdate();
        }

    }
}

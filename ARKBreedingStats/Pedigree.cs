using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
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
        public event Action<Creature> BestBreedingPartners;
        public event PedigreeCreature.ExportToClipboardEventHandler ExportToClipboard;
        private List<Creature> _creatures;
        private Creature _selectedCreature;
        private List<Creature> _creatureChildren = new List<Creature>();
        private readonly List<List<int[]>> _lines = new List<List<int[]>>();
        private readonly List<PedigreeCreature> _pcs = new List<PedigreeCreature>();
        private bool[] _enabledColorRegions = { true, true, true, true, true, true };

        public Pedigree()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            _lines.Add(new List<int[]>());
            _lines.Add(new List<int[]>());
            NoCreatureSelected();
            listViewCreatures.ListViewItemSorter = new ListViewColumnSorter();
            splitContainer1.Panel2.Paint += Panel2_Paint;
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(splitContainer1.Panel2.AutoScrollPosition.X, splitContainer1.Panel2.AutoScrollPosition.Y);
            if (_selectedCreature != null)
                DrawLines(e.Graphics);
        }

        /// <summary>
        /// Draws the lines that connect ancestors.
        /// </summary>
        /// <param name="g"></param>
        private void DrawLines(Graphics g)
        {
            // lines contains all the coordinates the arrows should be drawn: x1,y1,x2,y2,red/green,mutated/equal
            using (Pen myPen = new Pen(Color.Green, 3))
            {
                myPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (int[] line in _lines[0])
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
                foreach (int[] line in _lines[1])
                {
                    g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
                }
                if (_creatureChildren.Any())
                    g.DrawString(Loc.S("Descendants"), new Font("Arial", 14), new SolidBrush(Color.Black), 210, 170);
            }
        }

        public void Clear()
        {
            _selectedCreature = null;
            ClearControls();
            NoCreatureSelected();
        }

        public void ClearControls()
        {
            // clear pedigree   
            SuspendLayout();
            foreach (PedigreeCreature pc in _pcs)
                pc.Dispose();
            _lines.Clear();
            _lines.Add(new List<int[]>());
            _lines.Add(new List<int[]>());
            pictureBox.Image = null;
            pictureBox.Visible = false;
            ResumeLayout();
        }

        /// <summary>
        /// Creates the pedigree with creature controls.
        /// </summary>
        private void CreatePedigree()
        {
            // clear old pedigreeCreatures
            ClearControls();
            if (_selectedCreature == null)
            {
                NoCreatureSelected();
                return;
            }

            SuspendLayout();

            pedigreeCreature1.IsGlowSpecies = _selectedCreature.Species?.IsGlowSpecies ?? false;

            const int leftBorder = 40;
            const int pedigreeElementWidth = 325;
            const int margin = 10;

            lbPedigreeEmpty.Visible = false;

            // create ancestors
            CreateParentsChild(_selectedCreature, leftBorder + pedigreeElementWidth + margin, 60, true, true);
            if (_selectedCreature.Mother != null)
            {
                if (CreateParentsChild(_selectedCreature.Mother, leftBorder, 20))
                    _lines[1].Add(new[] { leftBorder + pedigreeElementWidth, 79, leftBorder + pedigreeElementWidth + margin, 79 });
            }
            if (_selectedCreature.Father != null)
            {
                if (CreateParentsChild(_selectedCreature.Father, leftBorder + 2 * (pedigreeElementWidth + margin), 20))
                    _lines[1].Add(new[] { leftBorder + 2 * pedigreeElementWidth + 2 * margin, 79, leftBorder + 2 * pedigreeElementWidth + margin, 159 });
            }

            // create descendants
            int row = 0;
            // scrolloffsets
            int xS = AutoScrollPosition.X;
            int yS = AutoScrollPosition.Y;
            foreach (Creature c in _creatureChildren)
            {
                PedigreeCreature pc = new PedigreeCreature(c, _enabledColorRegions)
                {
                    Location = new Point(leftBorder + xS, 200 + 35 * row + yS)
                };
                for (int s = 0; s < PedigreeCreature.displayedStatsCount; s++)
                {
                    int si = PedigreeCreature.displayedStats[s];
                    if (_selectedCreature.valuesDom[si] > 0 && _selectedCreature.levelsWild[si] >= 0 && _selectedCreature.levelsWild[si] == c.levelsWild[si])
                        _lines[0].Add(new[] { leftBorder + 38 + 29 * s, 200 + 35 * row + 6, leftBorder + 38 + 29 * s, 200 + 35 * row + 15, 0, 0 });
                }
                pc.CreatureClicked += CreatureClicked;
                pc.CreatureEdit += CreatureEdit;
                pc.BestBreedingPartners += BestBreedingPartners;
                pc.ExportToClipboard += ExportToClipboard;
                splitContainer1.Panel2.Controls.Add(pc);
                _pcs.Add(pc);
                row++;
            }

            pictureBox.Image = CreatureColored.GetColoredCreature(_selectedCreature.colors, _selectedCreature.Species, _enabledColorRegions, 256, creatureSex: _selectedCreature.sex);
            pictureBox.Visible = true;

            Invalidate();
            ResumeLayout();
        }

        /// <summary>
        /// Creates the controls that display a creature and its parents.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="drawWithNoParents"></param>
        /// <param name="highlightCreature"></param>
        /// <returns></returns>
        private bool CreateParentsChild(Creature creature, int x, int y, bool drawWithNoParents = false, bool highlightCreature = false)
        {
            if (creature == null || (!drawWithNoParents && creature.Mother == null && creature.Father == null))
                return false;

            // scrolloffset for control-locations (not for lines)
            int xS = AutoScrollPosition.X;
            int yS = AutoScrollPosition.Y;
            // creature
            AddCreatureControl(new PedigreeCreature(creature, _enabledColorRegions)
            {
                Location = new Point(x + xS, y + yS + 40),
                Highlight = highlightCreature
            });

            void AddCreatureControl(PedigreeCreature _pc)
            {
                splitContainer1.Panel2.Controls.Add(_pc);
                _pc.CreatureClicked += CreatureClicked;
                _pc.CreatureEdit += CreatureEdit;
                _pc.BestBreedingPartners += BestBreedingPartners;
                _pc.ExportToClipboard += ExportToClipboard;
                _pcs.Add(_pc);
            }

            // mother
            if (creature.Mother != null)
            {
                AddCreatureControl(new PedigreeCreature(creature.Mother, _enabledColorRegions)
                {
                    Location = new Point(x + xS, y + yS)
                });
            }
            // father
            if (creature.Father != null)
            {
                AddCreatureControl(new PedigreeCreature(creature.Father, _enabledColorRegions)
                {
                    Location = new Point(x + xS, y + yS + 80)
                });
            }
            // gene-inheritance-lines
            // better: if father < mother: 1, if mother < father: -1
            for (int s = 0; s < PedigreeCreature.displayedStatsCount; s++)
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
                    _lines[0].Add(new[] { 38 + x + 29 * s, y + 33, 38 + x + 29 * s, y + 42, (better == -1 ? 1 : 2), (creature.levelsWild[si] > creature.Mother.levelsWild[si] ? 1 : 0) });
                }
                if (creature.Father != null && creature.levelsWild[si] >= 0 && (creature.levelsWild[si] == creature.Father.levelsWild[si] || creature.levelsWild[si] == creature.Father.levelsWild[si] + 2))
                {
                    _lines[0].Add(new[] { 38 + x + 29 * s, y + 83, 38 + x + 29 * s, y + 74, (better == 1 ? 1 : 2), (creature.levelsWild[si] > creature.Father.levelsWild[si] ? 1 : 0) });
                }
            }
            return true;
        }

        private void CreatureClicked(Creature c, int comboIndex, MouseEventArgs e)
        {
            SetCreature(c);
        }

        private void CreatureEdit(Creature c, bool isVirtual)
        {
            EditCreature?.Invoke(c, isVirtual);
        }

        public void SetCreatures(List<Creature> creatures) => _creatures = creatures;

        /// <summary>
        /// Creates the pedigree with the given creature in the center.
        /// </summary>
        /// <param name="centralCreature"></param>
        /// <param name="forceUpdate"></param>
        public void SetCreature(Creature centralCreature, bool forceUpdate = false)
        {
            if (centralCreature == null)
            {
                _selectedCreature = null;
                ClearControls();
            }
            else if (_creatures != null && (centralCreature != _selectedCreature || forceUpdate))
            {
                _selectedCreature = centralCreature;
                // set children
                _creatureChildren = _creatures.Where(cr => cr.motherGuid == _selectedCreature.guid || cr.fatherGuid == _selectedCreature.guid)
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

                CreatePedigree();
            }
        }

        /// <summary>
        /// Displays text that no creature is selected.
        /// </summary>
        private void NoCreatureSelected()
        {
            lbPedigreeEmpty.Visible = true;
        }

        public bool[] EnabledColorRegions
        {
            set
            {
                if (value != null && value.Length == 6)
                {
                    _enabledColorRegions = value;
                }
                else
                {
                    _enabledColorRegions = new[] { true, true, true, true, true, true };
                }
            }
        }

        private void listViewCreatures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCreatures.SelectedIndices.Count > 0)
            {
                SetCreature((Creature)listViewCreatures.SelectedItems[0].Tag);
            }
        }

        /// <summary>
        /// Updates the list of available creatures in the pedigree list.
        /// </summary>
        public void UpdateListView()
        {
            listViewCreatures.BeginUpdate();

            // clear ListView
            listViewCreatures.Items.Clear();
            listViewCreatures.Groups.Clear();

            // add groups for each species (so they are sorted alphabetically)
            foreach (Species s in Values.V.species)
            {
                listViewCreatures.Groups.Add(new ListViewGroup(s.DescriptiveNameAndMod));
            }

            foreach (Creature cr in _creatures)
            {
                // if species is unknown, don't display creature
                if (cr.Species == null) continue;

                // check if group of species exists
                ListViewGroup g = null;
                foreach (ListViewGroup lvg in listViewCreatures.Groups)
                {
                    if (lvg.Header == cr.Species.DescriptiveNameAndMod)
                    {
                        g = lvg;
                        break;
                    }
                }
                if (g == null)
                {
                    g = new ListViewGroup(cr.Species.DescriptiveNameAndMod);
                    listViewCreatures.Groups.Add(g);
                }
                string crLevel = cr.LevelHatched > 0 ? cr.LevelHatched.ToString() : "?";
                ListViewItem lvi = new ListViewItem(new[] { cr.name, crLevel }, g);
                lvi.Tag = cr;
                lvi.UseItemStyleForSubItems = false;
                if (cr.flags.HasFlag(CreatureFlags.Placeholder))
                    lvi.SubItems[0].ForeColor = Color.LightGray;
                if (crLevel == "?")
                    lvi.SubItems[1].ForeColor = Color.LightGray;
                listViewCreatures.Items.Add(lvi);
            }
            listViewCreatures.EndUpdate();
        }

        private void listViewCreatures_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        public void SetLocalizations()
        {
            Loc.ControlText(lbPedigreeEmpty);
        }

        public Species SelectedSpecies => _selectedCreature?.Species;
    }
}

using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class Pedigree : UserControl
    {
        public delegate void EditCreatureEventHandler(Creature creature, bool virtualCreature);

        public event EditCreatureEventHandler EditCreature;
        public event Action<Creature> BestBreedingPartners;
        public event PedigreeCreature.ExportToClipboardEventHandler ExportToClipboard;

        /// <summary>
        /// All creatures of the current collection.
        /// </summary>
        private List<Creature> _creatures;

        private Creature[] _prefilteredCreatures;
        private Creature _selectedCreature;
        private List<Creature> _creatureChildren = new List<Creature>();
        private readonly List<int[]>[] _lines;
        private readonly List<PedigreeCreature> _pcs = new List<PedigreeCreature>();
        private bool[] _enabledColorRegions = { true, true, true, true, true, true };
        internal bool PedigreeNeedsUpdate;
        private readonly Debouncer _filterDebouncer = new Debouncer();

        private const int HorizontalStatDistance = 29;
        private const int XOffsetFirstStat = 38;

        public Pedigree()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            _lines = new[] { new List<int[]>(), new List<int[]>() };
            NoCreatureSelected();
            listViewCreatures.ListViewItemSorter = new ListViewColumnSorter();
            splitContainer1.Panel2.Paint += Panel2_Paint;
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(splitContainer1.Panel2.AutoScrollPosition.X,
                splitContainer1.Panel2.AutoScrollPosition.Y);
            if (_selectedCreature != null)
            {
                DrawLines(e.Graphics, _lines);
                if (_creatureChildren.Any())
                    e.Graphics.DrawString(Loc.S("Descendants"), new Font("Arial", 14), new SolidBrush(Color.Black), 210, 170);
            }
        }

        /// <summary>
        /// Draws the lines that connect ancestors.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lines">Array of arrow coordinates. lines[0] contains stat inheritance arrows, lines[1] parent-offspring-connections.</param>
        internal static void DrawLines(Graphics g, List<int[]>[] lines)
        {
            // lines contains all the coordinates the arrows should be drawn: x1,y1,x2,y2,red/green,mutated/equal
            using (Pen myPen = new Pen(Color.Green, 3))
            {
                myPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                if (lines[0] != null)
                {
                    foreach (int[] line in lines[0])
                    {
                        switch (line[4])
                        {
                            case 1:
                                myPen.Color = Color.DarkRed; break;
                            case 2:
                                myPen.Color = Color.Green; break;
                            default:
                                myPen.Color = Color.LightGray; break;
                        }
                        if (line[5] > 0)
                        {
                            // if stat is mutated
                            const int mutationBoxWidth = 14;
                            g.FillEllipse(Brushes.LightGreen, (line[0] + line[2] - mutationBoxWidth) / 2,
                                (line[1] + line[3] - mutationBoxWidth) / 2, mutationBoxWidth, mutationBoxWidth);
                        }

                        g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
                    }

                }

                myPen.Color = Color.DarkGray;
                myPen.Width = 1;
                if (lines[1] != null)
                {
                    foreach (int[] line in lines[1])
                    {
                        g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
                    }
                }
            }
        }

        public void Clear()
        {
            _selectedCreature = null;
            ClearControls();
            NoCreatureSelected();
        }

        /// <summary>
        /// Redraws the pedigree after the creature objects were reloaded.
        /// </summary>
        /// <param name="isActiveControl">if true, the data is updated immediately.</param>
        public void RecreateAfterLoading(bool isActiveControl = false)
        {
            if (_selectedCreature == null)
                return;

            _selectedCreature = _creatures.FirstOrDefault(c => c.guid == _selectedCreature.guid);

            if (_selectedCreature == null)
                Clear();
            else if (isActiveControl)
                CreatePedigree();
            else PedigreeNeedsUpdate = true;
        }

        private void ClearControls()
        {
            // clear pedigree   
            SuspendLayout();
            foreach (PedigreeCreature pc in _pcs)
                pc.Dispose();
            _lines[0].Clear();
            _lines[1].Clear();
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

            pedigreeCreatureHeaders.SetCustomStatNames(_selectedCreature?.Species?.statNames);

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
            // scroll offsets
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
                        _lines[0].Add(new[] { leftBorder + XOffsetFirstStat + HorizontalStatDistance * s, 200 + 35 * row + 6, leftBorder + XOffsetFirstStat + HorizontalStatDistance * s, 200 + 35 * row + 15, 0, 0 });
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
        /// <returns></returns>
        private bool CreateParentsChild(Creature creature, int x, int y, bool drawWithNoParents = false, bool highlightCreature = false)
        {
            if (creature == null || (!drawWithNoParents && creature.Mother == null && creature.Father == null))
                return false;

            // scroll offset for control-locations (not for lines)
            int xS = AutoScrollPosition.X;
            int yS = AutoScrollPosition.Y;
            // creature
            AddCreatureControl(new PedigreeCreature(creature, _enabledColorRegions)
            {
                Location = new Point(x + xS, y + yS + 40),
                Highlight = highlightCreature
            });

            void AddCreatureControl(PedigreeCreature pc)
            {
                splitContainer1.Panel2.Controls.Add(pc);
                pc.CreatureClicked += CreatureClicked;
                pc.CreatureEdit += CreatureEdit;
                pc.BestBreedingPartners += BestBreedingPartners;
                pc.ExportToClipboard += ExportToClipboard;
                _pcs.Add(pc);
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

            CreateGeneInheritanceLines(creature, creature.Mother, creature.Father, _lines, x, y);
            return true;
        }

        internal static void CreateGeneInheritanceLines(Creature offspring, Creature mother, Creature father, List<int[]>[] lines, int x, int y)
        {
            for (int s = 0; s < PedigreeCreature.displayedStatsCount; s++)
            {
                int si = PedigreeCreature.displayedStats[s];
                if (offspring.valuesDom[si] <= 0) continue; // don't display arrows for non used stats
                int better = 0; // if father < mother: 1, if mother < father: -1
                if (mother != null && father != null)
                {
                    if (mother.levelsWild[si] < father.levelsWild[si])
                        better = -1;
                    else if (mother.levelsWild[si] > father.levelsWild[si])
                        better = 1;
                }
                // offspring can have stats that are up to 2 levels higher due to mutations. currently there are no decreasing levels due to mutations
                if (mother != null && offspring.levelsWild[si] >= 0 && (offspring.levelsWild[si] == mother.levelsWild[si] || offspring.levelsWild[si] == mother.levelsWild[si] + 2))
                {
                    lines[0].Add(new[] { XOffsetFirstStat + x + HorizontalStatDistance * s, y + 33, XOffsetFirstStat + x + HorizontalStatDistance * s, y + 42, (better == -1 ? 1 : 2), (offspring.levelsWild[si] > mother.levelsWild[si] ? 1 : 0) });
                }
                if (father != null && offspring.levelsWild[si] >= 0 && (offspring.levelsWild[si] == father.levelsWild[si] || offspring.levelsWild[si] == father.levelsWild[si] + 2))
                {
                    lines[0].Add(new[] { XOffsetFirstStat + x + HorizontalStatDistance * s, y + 83, XOffsetFirstStat + x + HorizontalStatDistance * s, y + 74, (better == 1 ? 1 : 2), (offspring.levelsWild[si] > father.levelsWild[si] ? 1 : 0) });
                }
            }
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
            PedigreeNeedsUpdate = false;
            if (centralCreature == null)
            {
                if (forceUpdate && _selectedCreature != null)
                {
                    centralCreature = _selectedCreature;
                }
                else
                {
                    _selectedCreature = null;
                    ClearControls();
                    return;
                }
            }

            if (_creatures == null || (centralCreature == _selectedCreature && !forceUpdate)) return;

            if (centralCreature.Species != _selectedCreature?.Species)
                EnabledColorRegions = centralCreature.Species?.EnabledColorRegions;
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

        /// <summary>
        /// Displays text that no creature is selected.
        /// </summary>
        private void NoCreatureSelected()
        {
            lbPedigreeEmpty.Visible = true;
        }

        private bool[] EnabledColorRegions
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
            listViewCreatures.Groups.Clear();

            // add groups for each species (so they are sorted alphabetically)
            foreach (Species s in Values.V.species)
            {
                listViewCreatures.Groups.Add(new ListViewGroup(s.DescriptiveNameAndMod));
            }

            _prefilteredCreatures = _creatures.Where(c => c.Species != null).ToArray();
            listViewCreatures.EndUpdate();

            DisplayFilteredCreatureList();
        }

        private void DisplayFilteredCreatureList()
        {
            listViewCreatures.BeginUpdate();
            listViewCreatures.Items.Clear();

            var filterStrings = TextBoxFilter.Text.Split(',').Select(f => f.Trim())
                .Where(f => !string.IsNullOrEmpty(f)).ToArray();
            if (!filterStrings.Any()) filterStrings = null;

            foreach (Creature cr in _prefilteredCreatures)
            {
                if (filterStrings != null
                   && !filterStrings.All(f =>
                       cr.name.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) != -1
                       || (cr.Species?.name.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                       || (cr.owner?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                       || (cr.tribe?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                       || (cr.note?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                       || (cr.server?.IndexOf(f, StringComparison.InvariantCultureIgnoreCase) ?? -1) != -1
                       || (cr.tags?.Any(t => string.Equals(t, f, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                   ))
                    continue;

                // species group of creature
                ListViewGroup g = null;
                foreach (ListViewGroup lvg in listViewCreatures.Groups)
                {
                    if (lvg.Header == cr.Species.DescriptiveNameAndMod)
                    {
                        g = lvg;
                        break;
                    }
                }
                string crLevel = cr.LevelHatched > 0 ? cr.LevelHatched.ToString() : "?";
                ListViewItem lvi = new ListViewItem(new[] { cr.name, crLevel }, g)
                {
                    Tag = cr,
                    UseItemStyleForSubItems = false
                };
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

        private void TextBoxFilterTextChanged(object sender, EventArgs e)
        {
            _filterDebouncer.Debounce(TextBoxFilter.Text == string.Empty ? 0 : 500, DisplayFilteredCreatureList, Dispatcher.CurrentDispatcher);
        }

        private void ButtonClearFilter_Click(object sender, EventArgs e)
        {
            TextBoxFilter.Clear();
            TextBoxFilter.Focus();
        }
    }
}

using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.library;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class Pedigree : UserControl
    {
        public delegate void EditCreatureEventHandler(Creature creature, bool virtualCreature);

        public event EditCreatureEventHandler EditCreature;
        public event Action<Creature> BestBreedingPartners;

        /// <summary>
        /// All creatures of the current collection.
        /// </summary>
        private List<Creature> _creatures;

        private Species _selectedSpecies;

        private Creature[] _prefilteredCreatures;
        private Creature _selectedCreature;
        private Creature[] _creatureChildren;

        /// <summary>
        /// Array of arrow coordinates. lines[0] contains stat inheritance arrows, lines[1] parent-offspring arrows, lines[2] plain lines.
        /// In the inner arrays the elements represent x0, y0, x1, y1, extra info. The extra info can contain info about the color or line width.
        /// </summary>
        private readonly List<int[]>[] _lines;

        private readonly List<PedigreeCreature> _pcs = new List<PedigreeCreature>();
        private readonly List<PedigreeCreatureCompact> _pccs = new List<PedigreeCreatureCompact>();
        private bool[] _enabledColorRegions = { true, true, true, true, true, true };
        internal bool PedigreeNeedsUpdate;
        private readonly Debouncer _filterDebouncer = new Debouncer();
        private readonly ToolTip _tt;
        private bool _useCompactDisplay;
        private int _compactGenerations;
        private int _highlightInheritanceStatIndex = -1;

        private const int HorizontalStatDistance = 29;
        private const int XOffsetFirstStat = 38;

        public Pedigree()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer,
                true);
            _lines = new[] { new List<int[]>(), new List<int[]>(), new List<int[]>() };
            NoCreatureSelected();
            listViewCreatures.ListViewItemSorter = new ListViewColumnSorter();
            splitContainer1.Panel2.Paint += Panel2_Paint;
            _tt = new ToolTip();
            CbCompactView.Checked = Properties.Settings.Default.PedigreeCompactView;
            nudGenerations.ValueSave = Properties.Settings.Default.PedigreeCompactViewGenerations;
            statSelector1.StatIndexSelected += StatSelector1_StatIndexSelected;
        }

        private void StatSelector1_StatIndexSelected(int statIndex)
        {
            _highlightInheritanceStatIndex = statIndex;
            CreatePedigree();
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(splitContainer1.Panel2.AutoScrollPosition.X,
                splitContainer1.Panel2.AutoScrollPosition.Y);
            if (_selectedCreature != null)
            {
                DrawLines(e.Graphics, _lines);
                if (_creatureChildren.Any())
                    e.Graphics.DrawString(Loc.S("Descendants"), new Font("Arial", 14), new SolidBrush(Color.Black), 50,
                        _useCompactDisplay ? (_compactGenerations + 2) * PedigreeCreatureCompact.ControlHeight : 170);
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
                myPen.EndCap = LineCap.ArrowAnchor;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // stat inheritance lines. index 4 contains info about the color.
                if (lines[0] != null)
                {
                    foreach (int[] line in lines[0])
                    {
                        switch (line[4])
                        {
                            case 1:
                                myPen.Color = Color.DarkRed;
                                break;
                            case 2:
                                myPen.Color = Color.Green;
                                break;
                            default:
                                myPen.Color = Color.LightGray;
                                break;
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

                // simple arrow lines. index 4 contains info about the width: 0: default, 1: bold.
                if (lines[1] != null)
                {
                    foreach (int[] line in lines[1])
                    {
                        SetPenProperty(myPen, line[4]);
                        g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
                    }
                }

                // simple lines (generation lines for the compact mode). index 4 contains info about the width: 0: default, 1: bold.
                if (lines[2] != null)
                {
                    myPen.EndCap = LineCap.Flat;
                    foreach (int[] line in lines[2])
                    {
                        SetPenProperty(myPen, line[4]);
                        g.DrawLine(myPen, line[0], line[1], line[2], line[3]);
                    }
                }

                void SetPenProperty(Pen p, int style)
                {
                    switch (style)
                    {
                        case 1:
                            p.Color = Color.Black;
                            p.Width = 3;
                            break;
                        case 2:
                            p.Color = Color.Fuchsia;
                            p.Width = 3;
                            break;
                        default:
                            p.Color = Color.DarkGray;
                            p.Width = 1;
                            break;
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
            foreach (var pc in _pcs)
                pc.Dispose();
            _pcs.Clear();
            foreach (var pc in _pccs)
                pc.Dispose();
            _pccs.Clear();
            _lines[0].Clear();
            _lines[1].Clear();
            _lines[2].Clear();
            pictureBox.Image = null;
            pictureBox.Visible = false;
            ResumeLayout();
        }

        private void SetViewMode(bool compact)
        {
            if (_useCompactDisplay == compact) return;

            pedigreeCreatureHeaders.Visible = !compact;
            nudGenerations.Visible = compact;
            LbCreatureName.Visible = compact;
            statSelector1.Visible = compact;

            _useCompactDisplay = compact;
            Properties.Settings.Default.PedigreeCompactView = compact;
            SetCompactGenerationDisplay(compact ? _compactGenerations : 0);
        }

        private void SetCompactGenerationDisplay(int generations)
        {
            pictureBox.Top = generations == 0 ? 300 : (generations + 2) * PedigreeCreatureCompact.ControlHeight;
            LbCreatureName.Top = pictureBox.Top - LbCreatureName.Height;
            if (generations != 0)
            {
                _compactGenerations = generations;
                Properties.Settings.Default.PedigreeCompactViewGenerations = _compactGenerations;
            }

            CreatePedigree();
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

            pedigreeCreatureHeaders.SetCustomStatNames(_selectedCreature.Species?.statNames);
            statSelector1.SetStatNames(_selectedCreature.Species);

            const int leftBorder = 40;
            const int pedigreeElementWidth = 325;
            const int margin = 10;
            const int yCenterOfCreatureParent = 79;
            const int minXPosCreature = 300;

            lbPedigreeEmpty.Visible = false;

            if (_useCompactDisplay)
            {
                // each extra generation adds one control width
                var xOffsetStart = 4 * margin + (_compactGenerations < 2 ? 0 : (PedigreeCreatureCompact.ControlWidth + 3) * (1 << (_compactGenerations - 2)));
                if (xOffsetStart < minXPosCreature) xOffsetStart = minXPosCreature;
                var yOffsetStart = 6 * margin + (PedigreeCreatureCompact.ControlHeight + YMarginCreatureCompact) * (_compactGenerations - 1);
                var leftMargin = 2 * margin;
                var xLowest = CreateOffspringParentsCompact(_selectedCreature, xOffsetStart, yOffsetStart, false, _compactGenerations, xOffsetStart / 2, int.MaxValue, true, _highlightInheritanceStatIndex);
                var moveToLeft = xLowest - leftMargin;
                var maxMoveToLeft = Math.Max(0, xOffsetStart - minXPosCreature);
                if (moveToLeft > maxMoveToLeft) moveToLeft = maxMoveToLeft;
                if (moveToLeft > 0)
                    CompactViewLeftAlign(_pccs, _lines, moveToLeft);
            }
            else
            {
                // draw creature
                CreateParentsChild(_selectedCreature, leftBorder + pedigreeElementWidth + margin, 60, true, true);

                // create ancestors
                if (_selectedCreature.Mother != null
                    && CreateParentsChild(_selectedCreature.Mother, leftBorder, 20))
                {
                    _lines[1].Add(new[]
                    {
                        leftBorder + pedigreeElementWidth, yCenterOfCreatureParent,
                        leftBorder + pedigreeElementWidth + margin, yCenterOfCreatureParent
                    });
                }
                if (_selectedCreature.Father != null
                    && CreateParentsChild(_selectedCreature.Father, leftBorder + 2 * (pedigreeElementWidth + margin), 20))
                {
                    _lines[1].Add(new[]
                    {
                        leftBorder + 2 * pedigreeElementWidth + 2 * margin, yCenterOfCreatureParent,
                        leftBorder + 2 * pedigreeElementWidth + margin, yCenterOfCreatureParent + 80
                    });
                }
            }

            // create descendants
            int row = 0;
            // scroll offsets
            int xS = AutoScrollPosition.X;
            int yS = AutoScrollPosition.Y;
            var yDescendants = _useCompactDisplay ? (_compactGenerations + 3) * PedigreeCreatureCompact.ControlHeight : 200;
            foreach (Creature c in _creatureChildren)
            {
                PedigreeCreature pc = new PedigreeCreature(c, _enabledColorRegions)
                {
                    Location = new Point(leftBorder + xS, yDescendants + 35 * row + yS)
                };
                if (c.levelsWild != null && _selectedCreature.levelsWild != null)
                {
                    for (int s = 0; s < PedigreeCreature.DisplayedStatsCount; s++)
                    {
                        int si = PedigreeCreature.DisplayedStats[s];
                        if (_selectedCreature.valuesDom[si] > 0 && _selectedCreature.levelsWild[si] >= 0 &&
                            _selectedCreature.levelsWild[si] == c.levelsWild[si])
                            _lines[0].Add(new[]
                            {
                            leftBorder + XOffsetFirstStat + HorizontalStatDistance * s, yDescendants + 35 * row + 6,
                            leftBorder + XOffsetFirstStat + HorizontalStatDistance * s, yDescendants + 35 * row + 15, 0, 0
                        });
                    }
                }

                pc.CreatureClicked += CreatureClicked;
                pc.CreatureEdit += CreatureEdit;
                pc.BestBreedingPartners += BestBreedingPartners;
                splitContainer1.Panel2.Controls.Add(pc);
                _pcs.Add(pc);
                row++;
            }

            pictureBox.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_selectedCreature.colors,
                _selectedCreature.Species, _enabledColorRegions, 256, creatureSex: _selectedCreature.sex));
            pictureBox.Visible = true;

            Invalidate();
            ResumeLayout();
        }

        /// <summary>
        /// Some pedigrees don't use all the controls at the left, so move the existing controls to the left.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="lines"></param>
        private void CompactViewLeftAlign(List<PedigreeCreatureCompact> controls, List<int[]>[] lines, int moveToLeft)
        {
            foreach (var c in controls) c.Left -= moveToLeft;
            foreach (var ls in lines)
                foreach (var l in ls)
                {
                    l[0] -= moveToLeft;
                    l[2] -= moveToLeft;
                }
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
            var xLine = x;
            var yLine = y;
            x += AutoScrollPosition.X;
            y += AutoScrollPosition.Y;
            // creature
            AddCreatureControl(new PedigreeCreature(creature, _enabledColorRegions)
            {
                Location = new Point(x, y + 40),
                Highlight = highlightCreature
            });

            void AddCreatureControl(PedigreeCreature pc)
            {
                splitContainer1.Panel2.Controls.Add(pc);
                pc.CreatureClicked += CreatureClicked;
                pc.CreatureEdit += CreatureEdit;
                pc.BestBreedingPartners += BestBreedingPartners;
                _pcs.Add(pc);
            }

            // mother
            if (creature.Mother != null)
            {
                AddCreatureControl(new PedigreeCreature(creature.Mother, _enabledColorRegions)
                {
                    Location = new Point(x, y)
                });
            }
            // father
            if (creature.Father != null)
            {
                AddCreatureControl(new PedigreeCreature(creature.Father, _enabledColorRegions)
                {
                    Location = new Point(x, y + 80)
                });
            }

            CreateGeneInheritanceLines(creature, creature.Mother, creature.Father, _lines, xLine, yLine);
            return true;
        }

        internal static void CreateGeneInheritanceLines(Creature offspring, Creature mother, Creature father, List<int[]>[] lines, int x, int y)
        {
            if (offspring.levelsWild == null || offspring.valuesDom == null) return;

            for (int s = 0; s < PedigreeCreature.DisplayedStatsCount; s++)
            {
                int si = PedigreeCreature.DisplayedStats[s];
                if (offspring.valuesDom[si] <= 0) continue; // don't display arrows for non used stats
                int better = 0; // if father < mother: 1, if mother < father: -1
                if (mother?.levelsWild != null && father?.levelsWild != null)
                {
                    if (mother.levelsWild[si] < father.levelsWild[si])
                        better = -1;
                    else if (mother.levelsWild[si] > father.levelsWild[si])
                        better = 1;
                }

                // offspring can have stats that are up to 2 levels higher due to mutations. currently there are no decreasing levels due to mutations
                if (mother?.levelsWild != null && offspring.levelsWild[si] >= 0 &&
                    (offspring.levelsWild[si] == mother.levelsWild[si] ||
                     offspring.levelsWild[si] == mother.levelsWild[si] + 2))
                {
                    lines[0].Add(new[]
                    {
                            XOffsetFirstStat + x + HorizontalStatDistance * s, y + 33,
                            XOffsetFirstStat + x + HorizontalStatDistance * s, y + 42, (better == -1 ? 1 : 2),
                            (offspring.levelsWild[si] > mother.levelsWild[si] ? 1 : 0)
                        });
                }

                if (father?.levelsWild != null && offspring.levelsWild[si] >= 0 &&
                    (offspring.levelsWild[si] == father.levelsWild[si] ||
                     offspring.levelsWild[si] == father.levelsWild[si] + 2))
                {
                    lines[0].Add(new[]
                    {
                            XOffsetFirstStat + x + HorizontalStatDistance * s, y + 83,
                            XOffsetFirstStat + x + HorizontalStatDistance * s, y + 74, (better == 1 ? 1 : 2),
                            (offspring.levelsWild[si] > father.levelsWild[si] ? 1 : 0)
                        });
                }
            }
        }

        private const int YMarginCreatureCompact = 5;
        private const int YOffsetLineCompact = 30;

        private int CreateOffspringParentsCompact(Creature creature, int x, int y, bool onlyDrawParents, int generations, int xOffsetParent, int xLowest, bool highlightCreature, int highlightStatIndex, int highlightMotherLine = 0, int highlightFatherLine = 0)
        {
            var (motherInheritance, fatherInheritance) = CreateParentsChildCompact(creature, x, y, xOffsetParent, onlyDrawParents, highlightCreature, highlightStatIndex, highlightMotherLine, highlightFatherLine);

            var newXLowest = creature.Mother != null ? x - xOffsetParent : x;
            if (newXLowest < xLowest) xLowest = newXLowest;

            if (--generations < 2) return xLowest;
            var yParents = y - PedigreeCreatureCompact.ControlHeight - YMarginCreatureCompact;
            if (creature.Mother != null)
                xLowest = CreateOffspringParentsCompact(creature.Mother, x - xOffsetParent, yParents,
                    true, generations, xOffsetParent / 2, xLowest, false, highlightStatIndex, motherInheritance.maternalInheritance, motherInheritance.paternalInheritance);
            if (creature.Father != null)
                CreateOffspringParentsCompact(creature.Father, x + xOffsetParent, yParents,
                    true, generations, xOffsetParent / 2, xLowest, false, highlightStatIndex, fatherInheritance.maternalInheritance, fatherInheritance.paternalInheritance);

            return xLowest;
        }

        /// <summary>
        /// Creates the controls that display a creature and its parents.
        /// </summary>
        /// <returns>True if stat inheritance line is continued (maternal, paternal).</returns>
        private ((int maternalInheritance, int paternalInheritance) motherInheritance,
            (int maternalInheritance, int paternalInheritance) fatherInheritance)
            CreateParentsChildCompact(Creature creature, int x, int y, int xOffsetParents, bool onlyDrawParents, bool highlightCreature, int highlightStatIndex, int highlightMotherLine = 0, int highlightFatherLine = 0)
        {
            if (creature == null) return ((0, 0), (0, 0));

            // scroll offset for control-locations (not for lines)
            var xLine = x;
            var yLine = y;
            x += AutoScrollPosition.X;
            y += AutoScrollPosition.Y;

            if (!onlyDrawParents)
            {
                // creature
                var c = new PedigreeCreatureCompact(creature, highlightCreature, highlightStatIndex)
                {
                    Location = new Point(x, y)
                };
                AddCreatureControl(c);
                if (highlightStatIndex != -1)
                    (highlightMotherLine, highlightFatherLine) = c.PossibleStatInheritance(highlightStatIndex);
            }

            if (creature.Mother == null && creature.Father == null) return ((0, 0), (0, 0));

            var statInheritanceMother = (0, 0);
            var statInheritanceFather = (0, 0);

            var yParents = y - PedigreeCreatureCompact.ControlHeight - YMarginCreatureCompact;
            // mother
            if (creature.Mother != null)
            {
                var c = new PedigreeCreatureCompact(creature.Mother, highlightStatIndex: highlightMotherLine != 0 ? highlightStatIndex : -1)
                {
                    Location = new Point(x - xOffsetParents, yParents)
                };
                AddCreatureControl(c);
                if (highlightMotherLine != 0 && highlightStatIndex != -1)
                    statInheritanceMother = c.PossibleStatInheritance(highlightStatIndex);
            }
            // father
            if (creature.Father != null)
            {
                var c = new PedigreeCreatureCompact(creature.Father, highlightStatIndex: highlightFatherLine != 0 ? highlightStatIndex : -1)
                {
                    Location = new Point(x + xOffsetParents, yParents)
                };
                AddCreatureControl(c);
                if (highlightFatherLine != 0 && highlightStatIndex != -1)
                    statInheritanceFather = c.PossibleStatInheritance(highlightStatIndex);
            }

            void AddCreatureControl(PedigreeCreatureCompact pc)
            {
                splitContainer1.Panel2.Controls.Add(pc);
                pc.CreatureClicked += CreatureClicked;
                pc.CreatureEdit += CreatureEdit;
                pc.BestBreedingPartners += BestBreedingPartners;
                _pccs.Add(pc);
            }

            // lines
            //  M──┬──F
            //     O
            var yLineHorizontal = yLine - YOffsetLineCompact;
            var xCenterOffspring = xLine + PedigreeCreatureCompact.ControlWidth / 2;
            _lines[2].Add(new[] { xLine - xOffsetParents + PedigreeCreatureCompact.ControlWidth, yLineHorizontal, xCenterOffspring, yLineHorizontal, highlightMotherLine });
            _lines[2].Add(new[] { xLine + xOffsetParents, yLineHorizontal, xCenterOffspring, yLineHorizontal, highlightFatherLine });
            var combinedStyle = highlightMotherLine == 2 || highlightFatherLine == 2 ? 2
                : highlightMotherLine == 1 ? 1
                : highlightFatherLine;
            _lines[1].Add(new[] { xCenterOffspring, yLineHorizontal, xCenterOffspring, yLine, combinedStyle });

            return (statInheritanceMother, statInheritanceFather);
        }

        private void CreatureClicked(Creature c, int comboIndex, MouseEventArgs e)
        {
            SetCreature(c);
        }

        private void CreatureClicked(Creature c)
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
            LbCreatureName.Text = _selectedCreature.name;

            // set children
            _creatureChildren = _creatures.Where(cr => cr.motherGuid == _selectedCreature.guid || cr.fatherGuid == _selectedCreature.guid)
                .OrderBy(cr => cr.name)
                .ToArray();

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

        public void SetSpeciesIfNotSet(Species species)
        {
            if (_selectedSpecies == null)
                SetSpecies(species);
        }

        /// <summary>
        /// Updates the list of available creatures in the pedigree list and sets the species.
        /// </summary>
        public void SetSpecies(Species species = null, bool forceUpdate = false)
        {
            if (!forceUpdate && (species == null || species == _selectedSpecies))
                return;

            if (species != null)
                _selectedSpecies = species;
            else if (_selectedCreature == null)
                return;

            _prefilteredCreatures = _creatures.Where(c => c.Species == _selectedSpecies).ToArray();
            DisplayFilteredCreatureList();
        }

        private void DisplayFilteredCreatureList()
        {
            listViewCreatures.BeginUpdate();
            var filterStrings = TextBoxFilter.Text.Split(',').Select(f => f.Trim())
                .Where(f => !string.IsNullOrEmpty(f)).ToArray();
            if (!filterStrings.Any()) filterStrings = null;

            var items = new List<ListViewItem>();

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

                string crLevel = cr.LevelHatched > 0 ? cr.LevelHatched.ToString() : "?";
                ListViewItem lvi = new ListViewItem(new[] { cr.name, crLevel })
                {
                    Tag = cr,
                    UseItemStyleForSubItems = false
                };
                if (cr.flags.HasFlag(CreatureFlags.Placeholder))
                    lvi.SubItems[0].ForeColor = Color.LightGray;
                if (crLevel == "?")
                    lvi.SubItems[1].ForeColor = Color.LightGray;
                items.Add(lvi);
            }

            listViewCreatures.Items.Clear();
            listViewCreatures.Items.AddRange(items.ToArray());
            listViewCreatures.EndUpdate();
        }

        private void listViewCreatures_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter.DoSort((ListView)sender, e.Column);
        }

        public void SetLocalizations()
        {
            Loc.ControlText(lbPedigreeEmpty);
            _tt.SetToolTip(pictureBox, Loc.S("copyInfoGraphicToClipboard"));
            _tt.SetToolTip(nudGenerations, Loc.S("generations"));
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

        private void pictureBox_Click(object sender, EventArgs e)
        {
            _selectedCreature?.ExportInfoGraphicToClipboard(CreatureCollection.CurrentCreatureCollection);
        }

        private void CbCompactView_CheckedChanged(object sender, EventArgs e)
        {
            SetViewMode(CbCompactView.Checked);
        }

        private void nudGenerations_ValueChanged(object sender, EventArgs e)
        {
            _filterDebouncer.Debounce(300, () => SetCompactGenerationDisplay((int)nudGenerations.Value), Dispatcher.CurrentDispatcher);
        }
    }
}

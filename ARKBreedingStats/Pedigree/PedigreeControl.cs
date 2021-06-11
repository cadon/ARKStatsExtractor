using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.Pedigree
{
    public partial class PedigreeControl : UserControl
    {
        public delegate void EditCreatureEventHandler(Creature creature, bool virtualCreature);

        public event EditCreatureEventHandler EditCreature;
        public event Action<Creature> BestBreedingPartners;

        /// <summary>
        /// All creatures of the current collection.
        /// </summary>
        private List<Creature> _creatures;
        /// <summary>
        /// All creatures of the currently shown species.
        /// </summary>
        private Creature[] _creaturesOfSpecies;

        private Species _selectedSpecies;
        private Creature _selectedCreature;
        private Creature[] _creatureChildren;

        /// <summary>
        /// Array of arrow coordinates. lines[0] contains stat inheritance arrows, lines[1] parent-offspring arrows, lines[2] plain lines.
        /// In the inner arrays the elements represent x0, y0, x1, y1, extra info. The extra info can contain info about the color or line width.
        /// </summary>
        private readonly List<int[]>[] _lines;

        private readonly List<Control> _pedigreeControls = new List<Control>();
        private bool[] _enabledColorRegions = { true, true, true, true, true, true };
        internal bool PedigreeNeedsUpdate;
        private readonly Debouncer _filterDebouncer = new Debouncer();
        private readonly ToolTip _tt;
        private bool _useCompactDisplay;
        private int _compactGenerations;
        private int _displayedGenerations;
        private int _highlightInheritanceStatIndex = -1;

        public PedigreeControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            _lines = new[] { new List<int[]>(), new List<int[]>(), new List<int[]>() };
            NoCreatureSelected();
            listViewCreatures.ListViewItemSorter = new ListViewColumnSorter();
            splitContainer1.Panel2.Paint += Panel2_Paint;
            _tt = new ToolTip();
            _compactGenerations = Properties.Settings.Default.PedigreeCompactViewGenerations;
            nudGenerations.ValueSave = _compactGenerations;
            CbCompactView.Checked = Properties.Settings.Default.PedigreeCompactView;
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
                        _useCompactDisplay ? (_displayedGenerations + 2) * PedigreeCreatureCompact.ControlHeight : 170);
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
                            p.Color = Utils.MutationMarkerColor;
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
            _selectedSpecies = null;
            ClearControls();
            listViewCreatures.Items.Clear();
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

        private void ClearControls(bool suspendLayout = true)
        {
            // clear pedigree   
            if (suspendLayout)
                SuspendLayout();
            foreach (var pc in _pedigreeControls)
                pc.Dispose();
            _pedigreeControls.Clear();
            _lines[0].Clear();
            _lines[1].Clear();
            _lines[2].Clear();
            pictureBox.Image = null;
            pictureBox.Visible = false;
            LbCreatureName.Text = null;
            if (suspendLayout)
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
            pictureBox.Top = 300;
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
            this.SuspendDrawing();
            // clear old pedigreeCreatures
            ClearControls();
            if (_selectedCreature == null)
            {
                NoCreatureSelected();
                this.ResumeDrawing();
                return;
            }
            SuspendLayout();

            pedigreeCreatureHeaders.SetCustomStatNames(_selectedCreature.Species?.statNames);
            statSelector1.SetStatNames(_selectedCreature.Species);

            lbPedigreeEmpty.Visible = false;

            if (_useCompactDisplay)
            {
                _displayedGenerations = Math.Min(_compactGenerations, _selectedCreature.generation + 1);
                var yOffsetStart = PedigreeCreation.CreateCompactView(_selectedCreature, _lines, _pedigreeControls, _displayedGenerations, AutoScrollPosition.X, AutoScrollPosition.Y, _highlightInheritanceStatIndex);

                pictureBox.Top = yOffsetStart + PedigreeCreatureCompact.ControlHeight + PedigreeCreation.YMarginCreatureCompact + LbCreatureName.Height + 3 * PedigreeCreation.Margin;
                LbCreatureName.Top = pictureBox.Top - LbCreatureName.Height;
                LbCreatureName.Text = _selectedCreature.name;
            }
            else
            {
                PedigreeCreation.CreateDetailedView(_selectedCreature, _lines, _pedigreeControls, AutoScrollPosition.X, AutoScrollPosition.Y, _enabledColorRegions);
            }

            // create descendants
            int row = 0;
            // scroll offsets
            int xS = AutoScrollPosition.X;
            int yS = AutoScrollPosition.Y;
            var yDescendants = _useCompactDisplay ? (_displayedGenerations + 3) * PedigreeCreatureCompact.ControlHeight : 200;
            foreach (Creature c in _creatureChildren)
            {
                PedigreeCreature pc = new PedigreeCreature(c, _enabledColorRegions)
                {
                    Location = new Point(PedigreeCreation.LeftBorder + xS, yDescendants + 35 * row + yS)
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
                                PedigreeCreation.LeftBorder + PedigreeCreature.XOffsetFirstStat + PedigreeCreature.HorizontalStatDistance * s, yDescendants + 35 * row + 6,
                                PedigreeCreation.LeftBorder + PedigreeCreature.XOffsetFirstStat + PedigreeCreature.HorizontalStatDistance * s, yDescendants + 35 * row + 15, 0, 0
                        });
                    }
                }
                _pedigreeControls.Add(pc);
                row++;
            }

            // add controls
            foreach (var pc in _pedigreeControls)
            {
                splitContainer1.Panel2.Controls.Add(pc);
                var ipc = (IPedigreeCreature)pc;
                ipc.CreatureClicked += CreatureClicked;
                ipc.CreatureEdit += CreatureEdit;
                ipc.BestBreedingPartners += BestBreedingPartners;
            }

            pictureBox.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_selectedCreature.colors,
                _selectedCreature.Species, _enabledColorRegions, 256, creatureSex: _selectedCreature.sex));
            pictureBox.Visible = true;

            ResumeLayout();
            this.ResumeDrawing();
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

            if (centralCreature.Species != _selectedSpecies)
            {
                SetSpecies(centralCreature.Species);
            }
            _selectedCreature = centralCreature;

            // set children
            _creatureChildren = _creaturesOfSpecies.Where(cr => cr.motherGuid == _selectedCreature.guid || cr.fatherGuid == _selectedCreature.guid)
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

            EnabledColorRegions = _selectedSpecies.EnabledColorRegions;
            _creaturesOfSpecies = _creatures.Where(c => c.Species == _selectedSpecies).ToArray();
            DisplayFilteredCreatureList();
        }

        private void DisplayFilteredCreatureList()
        {
            listViewCreatures.BeginUpdate();
            var filterStrings = TextBoxFilter.Text.Split(',').Select(f => f.Trim())
                .Where(f => !string.IsNullOrEmpty(f)).ToArray();
            if (!filterStrings.Any()) filterStrings = null;

            var items = new List<ListViewItem>();

            foreach (Creature cr in _creaturesOfSpecies)
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

        public ListView ListViewCreatures => listViewCreatures;

        public int LeftColumnWidth
        {
            set => splitContainer1.SplitterDistance = value;
            get => splitContainer1.SplitterDistance;
        }
    }
}

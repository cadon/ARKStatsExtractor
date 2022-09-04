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
        private PedigreeViewMode _pedigreeViewMode;
        private int _compactGenerations;
        private int _displayedGenerations;
        private int _highlightInheritanceStatIndex = -1;
        private int _yBottomOfPedigree; // used for descendents
        private readonly PedigreeCreature _pedigreeHeader, _pedigreeHeaderMaternal, _pedigreeHeaderPaternal;

        public PedigreeControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            _pedigreeHeader = new PedigreeCreature
            {
                Left = PedigreeCreation.LeftMargin + PedigreeCreation.PedigreeElementWidth + PedigreeCreation.Margin,
                Top = PedigreeCreation.TopMargin
            };
            splitContainer1.Panel2.Controls.Add(_pedigreeHeader);
            _pedigreeHeaderMaternal = new PedigreeCreature
            {
                Left = PedigreeCreation.LeftMargin,
                Top = PedigreeCreation.TopMargin
            };
            splitContainer1.Panel2.Controls.Add(_pedigreeHeaderMaternal);
            _pedigreeHeaderPaternal = new PedigreeCreature
            {
                Left = PedigreeCreation.LeftMargin + 2 * (PedigreeCreation.PedigreeElementWidth + PedigreeCreation.Margin),
                Top = PedigreeCreation.TopMargin
            };
            splitContainer1.Panel2.Controls.Add(_pedigreeHeaderPaternal);

            _lines = new[] { new List<int[]>(), new List<int[]>(), new List<int[]>() };
            NoCreatureSelected();
            listViewCreatures.ListViewItemSorter = new ListViewColumnSorter();
            splitContainer1.Panel2.Paint += Panel2_Paint;
            _tt = new ToolTip { AutoPopDelay = 10000 };
            _compactGenerations = Properties.Settings.Default.PedigreeCompactViewGenerations;
            switch ((PedigreeViewMode)Properties.Settings.Default.PedigreeViewMode)
            {
                case PedigreeViewMode.Compact: RbViewCompact.Checked = true; break;
                case PedigreeViewMode.HView: RbViewH.Checked = true; break;
                default: RbViewClassic.Checked = true; break;
            }
            TbZoom.Value = (int)(10 * Properties.Settings.Default.PedigreeZoomFactor);
            PedigreeCreatureCompact.SetSizeFactor(Properties.Settings.Default.PedigreeZoomFactor);
            nudGenerations.ValueSave = _compactGenerations;
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
                DrawLines(e.Graphics, _lines, _pedigreeViewMode == PedigreeViewMode.Classic ? 1 : PedigreeCreatureCompact.PedigreeLineWidthFactor);
                if (_creatureChildren.Any())
                    e.Graphics.DrawString(Loc.S("Descendants"), new Font("Arial", 14), new SolidBrush(Color.Black), 50, _yBottomOfPedigree);
            }
        }

        /// <summary>
        /// Draws the lines that connect ancestors.
        /// </summary>
        /// <param name="lines">Array of arrow coordinates. lines[0] contains stat inheritance arrows, lines[1] parent-offspring-connections.</param>
        internal static void DrawLines(Graphics g, List<int[]>[] lines, float lineWidthFactor = 1)
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

                var fineLineWidth = lineWidthFactor;
                var boldLineWidth = lineWidthFactor * 3;

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
                            p.Width = fineLineWidth;
                            break;
                        case 2:
                            p.Color = Color.Black;
                            p.Width = boldLineWidth;
                            break;
                        case 3:
                            p.Color = Utils.MutationMarkerColor;
                            p.Width = boldLineWidth;
                            break;
                        default:
                            p.Color = Color.DarkGray;
                            p.Width = fineLineWidth;
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

        private void ClearControls(bool suspendDrawingAndLayout = true)
        {
            // clear pedigree
            if (suspendDrawingAndLayout)
            {
                splitContainer1.Panel2.SuspendDrawing();
                SuspendLayout();
            }

            foreach (var pc in _pedigreeControls)
                pc.Dispose();
            _pedigreeControls.Clear();
            _lines[0].Clear();
            _lines[1].Clear();
            _lines[2].Clear();
            if (PbRegionColors.Image != null)
                PbRegionColors.SetImageAndDisposeOld(null);
            PbRegionColors.Visible = false;
            LbCreatureName.Text = null;
            if (suspendDrawingAndLayout)
            {
                ResumeLayout();
                splitContainer1.Panel2.ResumeDrawing();
            }
        }

        private void SetViewMode(PedigreeViewMode viewMode)
        {
            if (_pedigreeViewMode == viewMode) return;
            _pedigreeViewMode = viewMode;

            var classicViewMode = viewMode == PedigreeViewMode.Classic;

            _pedigreeHeader.Visible = classicViewMode;
            _pedigreeHeaderMaternal.Visible = classicViewMode;
            _pedigreeHeaderPaternal.Visible = classicViewMode;
            nudGenerations.Visible = !classicViewMode;
            TbZoom.Visible = !classicViewMode;
            LbCreatureName.Visible = !classicViewMode;
            statSelector1.Visible = !classicViewMode;
            PbKeyExplanations.Visible = !classicViewMode;

            Properties.Settings.Default.PedigreeViewMode = (int)viewMode;
            SetCompactGenerationDisplay(classicViewMode ? 0 : _compactGenerations);
        }

        private void SetCompactGenerationDisplayWithInput() => SetCompactGenerationDisplay((int)nudGenerations.Value);

        private void SetCompactGenerationDisplay(int generations)
        {
            if (generations != 0)
            {
                _compactGenerations = generations;
                Properties.Settings.Default.PedigreeCompactViewGenerations = _compactGenerations;
            }
            else
            {
                PbRegionColors.Top = 300;
            }

            CreatePedigree();
        }

        /// <summary>
        /// Creates the pedigree with creature controls.
        /// </summary>
        private void CreatePedigree()
        {
            splitContainer1.Panel2.SuspendDrawing();
            SuspendLayout();
            // clear old pedigreeCreatures
            ClearControls(false);
            if (_selectedCreature == null)
            {
                NoCreatureSelected();
                ResumeLayout();
                splitContainer1.Panel2.ResumeDrawing();
                return;
            }

            _pedigreeHeader.SetCustomStatNames(_selectedCreature.Species?.statNames);
            _pedigreeHeaderMaternal.SetCustomStatNames(_selectedCreature.Species?.statNames);
            _pedigreeHeaderPaternal.SetCustomStatNames(_selectedCreature.Species?.statNames);
            statSelector1.SetStatNames(_selectedCreature.Species);

            lbPedigreeEmpty.Visible = false;

            // scroll offsets
            splitContainer1.Panel2.AutoScrollPosition = Point.Empty; // if not reset there are still offset issues sometimes, just reset the scrolling when creating a pedigree

            if (_pedigreeViewMode == PedigreeViewMode.Classic)
            {
                PedigreeCreation.CreateDetailedView(_selectedCreature, _lines, _pedigreeControls, _enabledColorRegions);
                _yBottomOfPedigree = PedigreeCreation.TopMargin + 4 * PedigreeCreation.PedigreeElementHeight;
            }
            else
            {
                _displayedGenerations = Math.Min(_compactGenerations, _selectedCreature.generation + 1);
                _yBottomOfPedigree = PedigreeCreation.CreateCompactView(_selectedCreature, _lines, _pedigreeControls, _tt, _displayedGenerations, _highlightInheritanceStatIndex, _pedigreeViewMode == PedigreeViewMode.HView);

                var creatureColorsTop = _yBottomOfPedigree + LbCreatureName.Height;
                PbRegionColors.Top = creatureColorsTop;
                PbKeyExplanations.Top = creatureColorsTop;
                LbCreatureName.Top = creatureColorsTop - LbCreatureName.Height;
                LbCreatureName.Text = _selectedCreature.name;

                if (PbKeyExplanations.Image == null)
                    DrawKey(PbKeyExplanations, _selectedSpecies);

                _pedigreeControls.Add(new PedigreeCreature(_selectedCreature, _enabledColorRegions)
                {
                    Location = new Point(PedigreeCreation.LeftMargin, _yBottomOfPedigree + PedigreeCreation.Margin)
                });
                _yBottomOfPedigree += 50;
            }

            // create descendants
            int row = 0;
            var yDescendants = _yBottomOfPedigree + 3 * PedigreeCreation.Margin;
            foreach (Creature c in _creatureChildren)
            {
                PedigreeCreature pc = new PedigreeCreature(c, _enabledColorRegions)
                {
                    Location = new Point(PedigreeCreation.LeftMargin, yDescendants + 35 * row)
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
                                PedigreeCreation.LeftMargin + PedigreeCreature.XOffsetFirstStat + PedigreeCreature.HorizontalStatDistance * s, yDescendants + 35 * row + 6,
                                PedigreeCreation.LeftMargin + PedigreeCreature.XOffsetFirstStat + PedigreeCreature.HorizontalStatDistance * s, yDescendants + 35 * row + 15, 0, 0
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

            PbRegionColors.SetImageAndDisposeOld(CreatureColored.GetColoredCreature(_selectedCreature.colors,
                _selectedCreature.Species, _enabledColorRegions, 256, creatureSex: _selectedCreature.sex));
            PbRegionColors.Visible = true;

            ResumeLayout();
            splitContainer1.Panel2.ResumeDrawing();
        }

        private static void DrawKey(PictureBox pb, Species species)
        {
            if (species == null) return;

            var w = pb.Width;
            var h = pb.Height;

            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            using (var font = new Font("Microsoft Sans Serif", 8.25f))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var pen = new Pen(Color.Black))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // border
                g.DrawRectangle(pen, 0, 0, w - 1, h - 1);

                // stats
                const int padding = 4;
                const int statCircleSize = PedigreeCreatureCompact.DefaultStatSize * 3 / 2;
                const int statRadius = statCircleSize / 2;
                const int radiusInnerCircle = statRadius / 7;
                var statLeftTopCoords = new Point(padding, padding);
                var center = new Point(statLeftTopCoords.X + statRadius, statLeftTopCoords.Y + statRadius);
                brush.Color = Color.White;
                g.FillEllipse(brush, statLeftTopCoords.X, statLeftTopCoords.Y, statRadius * 2, statRadius * 2);
                brush.Color = Color.Black;

                var usedStats = Enumerable.Range(0, Stats.StatsCount).Where(si => si != Stats.Torpidity && species.UsesStat(si)).ToArray();
                var anglePerStat = 360f / usedStats.Length;
                var i = 0;
                foreach (var si in usedStats)
                {
                    var angle = PedigreeCreatureCompact.AngleOffset + anglePerStat * i++;
                    g.DrawPie(pen, statLeftTopCoords.X, statLeftTopCoords.Y, statCircleSize, statCircleSize, angle, anglePerStat);

                    // text
                    const int radiusPosition = statRadius * 7 / 10;
                    var anglePosition = Math.PI * 2 / 360 * (angle + anglePerStat / 2);
                    const int statTexSizeHalf = 15;
                    var x = (int)Math.Round(radiusPosition * Math.Cos(anglePosition) + center.X - statTexSizeHalf);
                    var y = (int)Math.Round(radiusPosition * Math.Sin(anglePosition) + center.Y - statTexSizeHalf);
                    g.DrawString(Utils.StatName(si, true, species.statNames), font, brush,
                            new RectangleF(x, y, statTexSizeHalf * 2, statTexSizeHalf * 2),
                            format);
                }
                brush.Color = Color.Gray;
                g.FillEllipse(brush, center.X - radiusInnerCircle, center.Y - radiusInnerCircle, 2 * radiusInnerCircle, 2 * radiusInnerCircle);

                // circles
                const int textX = 3 * padding + 6;
                const int lineHeight = 15;
                void CircleExplanation(Color circleColor, string text, int y, int circleSize, int circleOffset = 0)
                {
                    PedigreeCreatureCompact.DrawFilledCircle(g, brush, pen, circleColor, padding + circleOffset, y + lineHeight / 4 + circleOffset, circleSize);
                    brush.Color = Color.Black;
                    g.DrawString(text, font, brush, textX, y);
                }

                void RectangleExplanation(Color rectangleColor, string text, int y, int size)
                {
                    pen.Color = rectangleColor;
                    pen.Width = 2;
                    g.DrawRectangle(pen, padding, y, size, size);
                    brush.Color = Color.Black;
                    g.DrawString(text, font, brush, textX, y);
                }

                void ArrowExplanation(List<int[]> linesList, int lineStyle, string text, int y, int size)
                {
                    var yLine = y + lineHeight / 2;
                    linesList.Add(new[] { padding, yLine, padding + size, yLine, lineStyle });
                    brush.Color = Color.Black;
                    g.DrawString(text, font, brush, textX, y);
                }

                int yText = statRadius * 2 + 4 * padding;
                CircleExplanation(Utils.MutationMarkerColor, "mutation in stat", yText, 6);
                yText += lineHeight;
                CircleExplanation(Utils.MutationMarkerPossibleColor, "possible mutation in stat", yText, 6);
                yText += lineHeight;
                CircleExplanation(Color.Yellow, "mutation in color", yText, 4, 1);
                yText += lineHeight;
                CircleExplanation(Color.GreenYellow, "creature without mutations", yText, 6);
                yText += lineHeight;
                CircleExplanation(Utils.MutationColor, "creature mutations < limit", yText, 6);
                yText += lineHeight;
                CircleExplanation(Color.DarkRed, "creature mutations ≥ limit", yText, 6);
                yText += lineHeight;
                // rectangles
                RectangleExplanation(Color.DodgerBlue, "selected creature", yText, 10);
                yText += lineHeight;
                RectangleExplanation(Utils.MutationMarkerColor, "creature with mutation", yText, 10);
                yText += lineHeight;
                // arrows
                var lines = new[] { null, new List<int[]>(), null };
                ArrowExplanation(lines[1], 1, "offspring", yText, 10);
                yText += lineHeight;
                ArrowExplanation(lines[1], 2, "stat inheritance", yText, 10);
                yText += lineHeight;
                ArrowExplanation(lines[1], 3, "stat inheritance with", yText, 10);
                g.DrawString("possible mutation", font, brush, textX, yText + lineHeight);

                DrawLines(g, lines);
            }

            pb.SetImageAndDisposeOld(bmp);
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
                if (value?.Length == Ark.ColorRegionCount)
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

            if (PbKeyExplanations.Image != null)
                PbKeyExplanations.SetImageAndDisposeOld(null);

            if (species != null)
                _selectedSpecies = species;
            else if (_selectedCreature == null)
                return;

            EnabledColorRegions = _selectedSpecies.EnabledColorRegions;
            _creaturesOfSpecies = _creatures.Where(c => c.Species == _selectedSpecies).ToArray();
            DisplayFilteredCreatureList();
            _selectedCreature = null;
            ClearControls();
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
            _tt.SetToolTip(PbRegionColors, Loc.S("copyInfoGraphicToClipboard"));
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

        private void RbViewClassic_CheckedChanged(object sender, EventArgs e)
        {
            if (RbViewClassic.Checked)
                SetViewMode(PedigreeViewMode.Classic);
        }

        private void RbViewCompact_CheckedChanged(object sender, EventArgs e)
        {
            if (RbViewCompact.Checked)
                SetViewMode(PedigreeViewMode.Compact);
        }

        private void RbViewH_CheckedChanged(object sender, EventArgs e)
        {
            if (RbViewH.Checked)
                SetViewMode(PedigreeViewMode.HView);
        }

        private enum PedigreeViewMode
        {
            Unknown,
            Classic,
            Compact,
            /// <summary>
            /// H-shaped fractal arrangement, most compact.
            /// </summary>
            HView
        };

        private void TbZoom_Scroll(object sender, EventArgs e)
        {
            Properties.Settings.Default.PedigreeZoomFactor = TbZoom.Value * 0.1f;
            PedigreeCreatureCompact.SetSizeFactor(TbZoom.Value * 0.1);
            _filterDebouncer.Debounce(300, SetCompactGenerationDisplayWithInput, Dispatcher.CurrentDispatcher);
        }
    }
}

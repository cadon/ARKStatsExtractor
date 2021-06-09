using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

namespace ARKBreedingStats.uiControls
{
    /// <summary>
    /// Compact representation of a creature with its stats, used for compact display in a pedigree.
    /// </summary>
    public class PedigreeCreatureCompact : PictureBox
    {
        private const int StatSize = 50;
        private const int ColorSize = 10;
        private const int BottomTextHeight = 9;
        public const int ControlWidth = StatSize + ColorSize;
        public const int ControlHeight = StatSize + BottomTextHeight;
        private const int AngleOffset = -90; // start at 12 o'clock
        private readonly ToolTip _tt;
        private Creature _creature;

        /// <summary>
        /// Edit the creature. Boolean parameter determines if the creature is virtual.
        /// </summary>
        public event Action<Creature, bool> CreatureEdit;

        /// <summary>
        /// Display the best breeding partners for the given creature.
        /// </summary>
        public event Action<Creature> BestBreedingPartners;

        /// <summary>
        /// Display the creature in the pedigree.
        /// </summary>
        public event Action<Creature> CreatureClicked;

        /// <summary>
        /// Recalculate the breeding plan, e.g. if the cooldown was reset.
        /// </summary>
        public event Action RecalculateBreedingPlan;

        /// <summary>
        /// If not null, a possible mutation occurred in the stat index where true;
        /// </summary>
        private bool[] _possibleMutationInStat;


        public PedigreeCreatureCompact()
        {
            Width = ControlWidth;
            Height = ControlHeight;
            _tt = new ToolTip
            {
                AutoPopDelay = 10000
            };
            Disposed += (sender, args) => _tt.RemoveAll();
            Click += PedigreeCreatureCompact_Click;
        }

        public PedigreeCreatureCompact(Creature creature, bool highlight = false, int highlightStatIndex = -1) : this()
        {
            _creature = creature;
            DrawData(creature, highlight, highlightStatIndex);
        }

        private void DrawData(Creature creature, bool highlight, int highlightStatIndex)
        {
            if (creature?.Species == null) return;

            var displayedStats = Enumerable.Range(0, Values.STATS_COUNT).Where(si => si != (int)StatNames.Torpidity && creature.Species.UsesStat(si)).ToArray();
            var anglePerStat = 360 / displayedStats.Length;

            const int borderWidth = 1;

            // used for the tooltip text
            var colors = new ArkColor[Species.ColorRegionCount];

            bool mutationHappened = creature.mutationsMaternalNew != 0 || creature.mutationsPaternalNew != 0;
            _possibleMutationInStat = mutationHappened ? new bool[Values.STATS_COUNT] : null;

            Bitmap bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            using (var font = new Font("Microsoft Sans Serif", 7f))
            using (var pen = new Pen(Color.Black))
            using (var brush = new SolidBrush(Color.Black))
            {
                var borderColor = Color.FromArgb(219, 219, 219);
                if (highlight)
                {
                    Highlight = true;
                    borderColor = Color.CornflowerBlue;
                }

                pen.Color = borderColor;
                pen.Width = borderWidth;
                g.DrawRectangle(pen, 0, 0, Width - borderWidth, Height - borderWidth);

                // stats
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var chartMax = CreatureCollection.CurrentCreatureCollection?.maxChartLevel ?? 50;
                const int radiusInnerCircle = (StatSize - 2 * borderWidth) / 7;
                const int centerCoord = StatSize / 2 - 1;

                var i = 0;
                if (creature.levelsWild != null)
                {
                    pen.Color = Color.Black;
                    foreach (var si in displayedStats)
                    {
                        var level = creature.levelsWild[si];

                        var statSize = Math.Min((double)level / chartMax, 1);
                        var pieRadius = (int)(radiusInnerCircle + (centerCoord - radiusInnerCircle - borderWidth) * statSize);
                        var leftTop = centerCoord - pieRadius;
                        var angle = AngleOffset + anglePerStat * i++;
                        brush.Color = Utils.GetColorFromPercent((int)(100 * statSize), creature.topBreedingStats[si] ? 0 : 0.7);
                        g.FillPie(brush, leftTop, leftTop, 2 * pieRadius, 2 * pieRadius, angle, anglePerStat);

                        pen.Width = highlightStatIndex == si ? 2 : 1;
                        g.DrawPie(pen, leftTop, leftTop, 2 * pieRadius, 2 * pieRadius, angle, anglePerStat);


                        var possibleMutation = mutationHappened && (level == (creature.Mother?.levelsWild[si] ?? 0) + Creature.LevelsAddedPerMutation
                                                                    || level == (creature.Father?.levelsWild[si] ?? 0) + Creature.LevelsAddedPerMutation);
                        if (possibleMutation)
                        {
                            const int radius = 3;
                            var radiusPosition = centerCoord - radius - 1;
                            var anglePosition = Math.PI * 2 / 360 * (angle + anglePerStat / 2);
                            var x = (int)Math.Round(radiusPosition * Math.Cos(anglePosition) + radiusPosition);
                            var y = (int)Math.Round(radiusPosition * Math.Sin(anglePosition) + radiusPosition);
                            DrawFilledCircle(Color.Yellow, x, y, 2 * radius);
                            _possibleMutationInStat[si] = true;
                        }
                    }
                }

                if (_possibleMutationInStat != null && !_possibleMutationInStat.Any(m => m))
                {
                    _possibleMutationInStat = null; // not needed, no possible mutations
                }

                // draw sex in the center
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                brush.Color = Utils.AdjustColorLight(Utils.SexColor(creature.sex), 0.2);
                g.FillEllipse(brush, centerCoord - radiusInnerCircle, centerCoord - radiusInnerCircle, 2 * radiusInnerCircle, 2 * radiusInnerCircle);
                pen.Width = 1;
                g.DrawEllipse(pen, centerCoord - radiusInnerCircle, centerCoord - radiusInnerCircle, 2 * radiusInnerCircle, 2 * radiusInnerCircle);

                brush.Color = Color.Black;
                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    g.DrawString(Utils.SexSymbol(creature.sex), font, brush,
                        new RectangleF(centerCoord - radiusInnerCircle + 1, centerCoord - radiusInnerCircle + 2, 2 * radiusInnerCircle, 2 * radiusInnerCircle),
                        format);
                    g.DrawString(creature.name, font, brush,
                        new RectangleF(borderWidth, StatSize + borderWidth - 1, ControlWidth - borderWidth, BottomTextHeight),
                        format);
                }

                // colors
                if (creature.colors != null)
                {
                    var displayedColorRegions = Enumerable.Range(0, Species.ColorRegionCount)
                        .Where(ci => creature.Species.EnabledColorRegions[ci]).ToArray();
                    const int margin = 1;
                    var colorSize = new Size(ColorSize - 3 * margin - borderWidth,
                        (StatSize - 2 * borderWidth) / displayedColorRegions.Length - 2 * margin);
                    i = 0;
                    var left = StatSize + 2 * margin;
                    foreach (var ci in displayedColorRegions)
                    {
                        var color = CreatureColors.CreatureArkColor(creature.colors[ci]);
                        colors[ci] = color;
                        brush.Color = color.Color;
                        g.FillRectangle(brush, left, borderWidth + i * (colorSize.Height + 2 * margin), colorSize.Width,
                            colorSize.Height);
                        g.DrawRectangle(pen, left, borderWidth + i++ * (colorSize.Height + 2 * margin), colorSize.Width,
                            colorSize.Height);
                    }
                }

                // mutation indicator
                if (!creature.flags.HasFlag(CreatureFlags.Placeholder))
                {
                    const int mutationIndicatorSize = 6;
                    const int topLeft = StatSize - mutationIndicatorSize - 1 - borderWidth;
                    Color mutationColor = creature.Mutations == 0 ? Color.GreenYellow
                        : creature.Mutations < BreedingPlan.MutationPossibleWithLessThan ? Utils.MutationColor
                        : Color.DarkRed;

                    DrawFilledCircle(mutationColor, topLeft, topLeft, mutationIndicatorSize);
                }

                void DrawFilledCircle(Color color, int x, int y, int size)
                {
                    brush.Color = color;
                    g.FillEllipse(brush, x, y, size, size);
                    pen.Width = 1;
                    pen.Color = Utils.AdjustColorLight(color, -.6);
                    g.DrawEllipse(pen, x, y, size, size);
                }
            }

            Image?.Dispose();
            Image = bmp;

            var statNames = creature.Species?.statNames;

            var toolTipText = $"{creature.name} ({Utils.SexSymbol(creature.sex)})";
            if (creature.flags.HasFlag(CreatureFlags.Placeholder))
            {
                toolTipText += "\nThis creature is not yet imported. This entry is a placeholder and contains no more info";
            }
            else
            {
                if (creature.levelsWild != null)
                    toolTipText +=
                        $"\n{string.Join("\n", displayedStats.Select(si => $"{Utils.StatName(si, true, statNames)}:\t{creature.levelsWild[si],3}{((_possibleMutationInStat?[si] ?? false) ? " (possible mutation)" : null)}"))}";
                toolTipText +=
                    $"\n{Loc.S("Mutations")}: {creature.Mutations} = {creature.mutationsMaternal} (♀) + {creature.mutationsPaternal} (♂)";
                if (creature.colors != null)
                    toolTipText +=
                        $"\n{Loc.S("Colors")}\n{string.Join("\n", colors.Select((c, i) => c == null ? null : $"[{i}]:\t{c.Id} ({c.Name})").Where(s => s != null))}";
            }

            _tt.SetToolTip(this, toolTipText);
        }

        private bool Highlight
        {
            set => Cursor = !value ? Cursors.Hand : Cursors.Default;
        }

        private void PedigreeCreatureCompact_Click(object sender, EventArgs e)
        {
            CreatureClicked?.Invoke(_creature);
        }

        /// <summary>
        /// Returns 1 if stat was possibly inherited from (mother,father), 2 if it was a possible mutation.
        /// </summary>
        public (int maternalInheritance, int paternalInheritance) PossibleStatInheritance(int statIndex)
        {
            if (statIndex == -1) return (0, 0);

            bool mutationHappened = _possibleMutationInStat?[statIndex] ?? false;

            int levelMother = _creature.Mother?.levelsWild?[statIndex] ?? -1;
            var motherInheritance = _creature.levelsWild[statIndex] == levelMother ? 1
                : (mutationHappened && _creature.levelsWild[statIndex] == levelMother + Creature.LevelsAddedPerMutation) ? 2
                : 0;
            int levelFather = _creature.Father?.levelsWild?[statIndex] ?? -1;
            var fatherInheritance = _creature.levelsWild[statIndex] == levelFather ? 1
                : (mutationHappened && _creature.levelsWild[statIndex] == levelFather + Creature.LevelsAddedPerMutation) ? 2
                : 0;
            return (motherInheritance, fatherInheritance);
        }
    }
}

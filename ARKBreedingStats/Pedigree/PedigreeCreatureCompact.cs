using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.Pedigree;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

namespace ARKBreedingStats.uiControls
{
    /// <summary>
    /// Compact representation of a creature with its stats, used for compact display in a pedigree.
    /// </summary>
    public class PedigreeCreatureCompact : PictureBox, IPedigreeCreature
    {
        public static void SetSizeFactor(double factor = 1)
        {
            _statSize = (int)Math.Round(DefaultStatSize * factor);
            _colorSize = (int)Math.Round(DefaultColorSize * factor);
            _mutationIndicatorSize = (int)Math.Round(DefaultMutationIndicatorSize * factor);
            _fontSize = _colorSize * 0.7f;
            _mutationMarkerRadius = _colorSize * 3 / 10;
            _colorMutationMarkerRadius = _colorSize * 2 / 10;
            ControlWidth = _statSize + _colorSize;
            ControlHeight = _statSize + _colorSize;
            PedigreeLineWidthFactor = Math.Max(1, (float)(1 * factor));
        }

        private const int DefaultColorSize = 10;
        public const int DefaultStatSize = 5 * DefaultColorSize;
        private const int DefaultMutationIndicatorSize = DefaultColorSize * 6 / 10;

        private static int _statSize = DefaultStatSize;
        private static int _colorSize = DefaultColorSize;
        private static int _mutationIndicatorSize = DefaultMutationIndicatorSize;
        private static float _fontSize = DefaultColorSize * 0.7f;
        private static int _mutationMarkerRadius = DefaultColorSize * 3 / 10;
        private static int _colorMutationMarkerRadius = DefaultColorSize * 2 / 10;
        public static float PedigreeLineWidthFactor = 1;
        public static int ControlWidth = _statSize + _colorSize;
        public static int ControlHeight = _statSize + _colorSize;

        public const int AngleOffset = -90; // start at 12 o'clock
        private ToolTip _tt;
        private readonly Creature _creature;

        public event PedigreeCreature.CreatureChangedEventHandler CreatureClicked;

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
        public event Action<Creature> DisplayInPedigree;

        /// <summary>
        /// Recalculate the breeding plan, e.g. if the cooldown was reset.
        /// </summary>
        public event Action RecalculateBreedingPlan;

        /// <summary>
        /// Each byte is used as 8 bit flags.
        /// The 4 least significant bits for mutations from the mother, the 4 most significant bits for mutations from the father.
        /// 0: no inheritance. 1-7: number of certain mutations. 8: inheritance without mutation; 9-15: inheritance with possible mutations, get mutation count by  & 7.
        /// </summary>
        private byte[] _statInheritances;

        /// <summary>
        /// If not null, a possible mutation occurred in the color region index where true;
        /// </summary>
        private bool[] _mutationInColor;


        public PedigreeCreatureCompact()
        {
            Width = ControlWidth;
            Height = ControlHeight;
            Disposed += PedigreeCreatureCompact_Disposed;
            MouseClick += PedigreeCreatureCompact_MouseClick;
        }

        private void PedigreeCreatureCompact_Disposed(object sender, EventArgs e) => _tt?.SetToolTip(this, null);

        public PedigreeCreatureCompact(Creature creature, bool highlight = false, int highlightStatIndex = -1, ToolTip tt = null) : this()
        {
            _creature = creature;
            DrawData(creature, highlight, highlightStatIndex, tt);
        }

        private void DrawData(Creature creature, bool highlight, int highlightStatIndex, ToolTip tt)
        {
            if (creature?.Species == null) return;

            var usedStats = Enumerable.Range(0, Stats.StatsCount).Where(si => si != Stats.Torpidity && creature.Species.UsesStat(si)).ToArray();
            var anglePerStat = 360f / usedStats.Length;

            const int borderWidth = 1;

            // used for the tooltip text
            var colors = new ArkColor[Ark.ColorRegionCount];

            (_statInheritances, _mutationInColor) = DetermineInheritanceAndMutations(creature, usedStats);

            var mutationOccurred = _mutationInColor != null;

            Bitmap bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            using (var font = new Font("Microsoft Sans Serif", _fontSize))
            using (var pen = new Pen(Color.Black))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var borderColor = Color.FromArgb(219, 219, 219);
                float drawnBorderWidth = borderWidth;
                if (highlight)
                {
                    borderColor = Color.DodgerBlue;
                    drawnBorderWidth = 1.5f;
                }
                else
                {
                    Cursor = Cursors.Hand;
                    if (highlightStatIndex != -1)
                        borderColor = Color.Black;
                }

                if (mutationOccurred)
                {
                    borderColor = Utils.MutationMarkerColor;
                    drawnBorderWidth = 1.5f;
                }

                pen.Color = borderColor;
                pen.Width = drawnBorderWidth;
                g.DrawRectangle(pen, drawnBorderWidth, drawnBorderWidth, Width - 2 * drawnBorderWidth, Height - 2 * drawnBorderWidth);

                // stats
                var chartMax = CreatureCollection.CurrentCreatureCollection?.maxChartLevel ?? 50;
                int radiusInnerCircle = (_statSize - 2 * borderWidth) / 7;
                int centerCoord = _statSize / 2 - 1;

                var i = 0;
                if (creature.levelsWild != null)
                {
                    pen.Color = Color.Black;
                    foreach (var si in usedStats)
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

                        var mutationStatus = _statInheritances[si];
                        const int anyMutationMask = 0b01110111;
                        if ((mutationStatus & anyMutationMask) == 0) continue;

                        const int mutationIsNotGuaranteedMask = 0b10001000;
                        var guaranteedMutation = (mutationStatus & mutationIsNotGuaranteedMask) == 0;

                        var anglePosition = Math.PI * 2 / 360 * (angle + anglePerStat / 2);
                        var x = (int)Math.Round(pieRadius * Math.Cos(anglePosition) + centerCoord - _mutationMarkerRadius - 1);
                        var y = (int)Math.Round(pieRadius * Math.Sin(anglePosition) + centerCoord - _mutationMarkerRadius - 1);
                        DrawFilledCircle(g, brush, pen, guaranteedMutation ? Utils.MutationMarkerColor : Utils.MutationMarkerPossibleColor, x, y, 2 * _mutationMarkerRadius);
                    }
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
                        new RectangleF(borderWidth, _statSize + borderWidth - 2, ControlWidth - borderWidth, _colorSize),
                        format);
                }

                // colors
                if (creature.colors != null)
                {
                    var displayedColorRegions = Enumerable.Range(0, Ark.ColorRegionCount)
                        .Where(ci => creature.Species.EnabledColorRegions[ci]).ToArray();

                    var usedColorRegionCount = displayedColorRegions.Length;
                    if (usedColorRegionCount != 0)
                    {
                        const int margin = 1;
                        var colorSize = new Size(_colorSize - 3 * margin - borderWidth,
                            (_statSize - 2 * borderWidth) / usedColorRegionCount - 3 * margin);

                        // only check for color mutations if the colors of both parents are available
                        mutationOccurred = mutationOccurred && creature.Mother?.colors != null &&
                                          creature.Father?.colors != null;

                        i = 0;
                        var left = _statSize + 2 * margin;
                        foreach (var ci in displayedColorRegions)
                        {
                            var color = CreatureColors.CreatureArkColor(creature.colors[ci]);
                            colors[ci] = color;
                            brush.Color = color.Color;
                            var y = borderWidth + margin + i++ * (colorSize.Height + 2 * margin);
                            g.FillRectangle(brush, left, y, colorSize.Width,
                                colorSize.Height);
                            g.DrawRectangle(pen, left, y, colorSize.Width,
                                colorSize.Height);

                            var colorMutationOccurred =
                                mutationOccurred && creature.colors[ci] != creature.Mother.colors[ci]
                                                && creature.colors[ci] != creature.Father.colors[ci];
                            if (colorMutationOccurred)
                            {
                                var x = left - _colorMutationMarkerRadius - 2;
                                y = y + colorSize.Height / 2 - _colorMutationMarkerRadius;
                                DrawFilledCircle(g, brush, pen, Color.Yellow, x, y, 2 * _colorMutationMarkerRadius);
                                _mutationInColor[ci] = true;
                            }
                        }
                    }
                }

                if (_mutationInColor != null && !_mutationInColor.Any(m => m))
                    _mutationInColor = null; // not needed, no possible mutations

                // mutation indicator
                if (!creature.flags.HasFlag(CreatureFlags.Placeholder))
                {
                    int yMarker = _statSize - _mutationIndicatorSize - 1 - borderWidth;
                    Color mutationColor = creature.Mutations == 0 ? Color.GreenYellow
                        : creature.Mutations < Ark.MutationPossibleWithLessThan ? Utils.MutationColor
                        : Color.DarkRed;

                    DrawFilledCircle(g, brush, pen, mutationColor, borderWidth + 1, yMarker, _mutationIndicatorSize);
                }
            }

            var oldImage = Image;
            Image = bmp;
            oldImage?.Dispose();

            var statNames = creature.Species?.statNames;

            var toolTipText = $"{creature.name} ({Utils.SexSymbol(creature.sex)})";
            if (creature.flags.HasFlag(CreatureFlags.Placeholder))
            {
                toolTipText += "\nThis creature is not yet in this library. This entry is a placeholder and contains no more info";
            }
            else
            {
                string InheritanceExplanation(int statIndex)
                {
                    var mutationStatus = _statInheritances[statIndex];
                    if (mutationStatus == 0) return null;
                    var resultMother = Mutation(true);
                    var resultFather = Mutation(false);
                    if (resultMother == null && resultFather == null) return null;

                    return $" ({resultMother}{(resultMother != null && resultFather != null ? " or " : null)}{resultFather})";

                    string Mutation(bool mother)
                    {
                        var status = (mutationStatus >> (!mother ? 4 : 0)) & 0xf;
                        if (status == 0) return null;
                        var sex = mother ? "♀" : "♂";
                        if (status == 8) return sex;
                        if (status > 8)
                        {
                            var mutationCount = status & 7;
                            return $"{sex} with possible {mutationCount} mutation{(mutationCount > 1 ? "s" : null)}";
                        }
                        return $"{sex} with {status} mutation{(status > 1 ? "s" : null)}";
                    }
                }

                if (creature.levelsWild != null)
                    toolTipText +=
                        $"\n{string.Join("\n", usedStats.Select(si => $"{Utils.StatName(si, true, statNames)}:\t{creature.levelsWild[si],3}{InheritanceExplanation(si)}"))}";
                toolTipText +=
                    $"\n{Loc.S("Mutations")}: {creature.Mutations} = {creature.mutationsMaternal} (♀) + {creature.mutationsPaternal} (♂)";
                if (creature.colors != null)
                    toolTipText +=
                        $"\n{Loc.S("Colors")}\n{string.Join("\n", colors.Select((c, i) => c == null ? null : $"[{i}]:\t{c.Id} ({c.Name}){((_mutationInColor?[i] ?? false) ? " (mutated color)" : null)}").Where(s => s != null))}";
            }

            _tt?.SetToolTip(this, null);
            _tt = tt;
            _tt?.SetToolTip(this, toolTipText);
        }

        public static void DrawFilledCircle(Graphics g, SolidBrush brush, Pen pen, Color color, int x, int y, int size)
        {
            brush.Color = color;
            g.FillEllipse(brush, x, y, size, size);
            pen.Width = 1;
            pen.Color = Utils.AdjustColorLight(color, -.6);
            g.DrawEllipse(pen, x, y, size, size);
        }

        private static (byte[] statInheritances, bool[] mutationInColor) DetermineInheritanceAndMutations(Creature creature, int[] usedStats)
        {
            var mutationsOccurredCount = creature.mutationsMaternalNew + creature.mutationsPaternalNew;
            var mutationOccurred = mutationsOccurredCount != 0;

            var statInheritances = new byte[Stats.StatsCount];
            var mutationInColor = mutationOccurred ? new bool[Ark.ColorRegionCount] : null;

            bool levelsKnownMother = creature.Mother?.levelsWild != null;
            bool levelsKnownFather = creature.Father?.levelsWild != null;
            if (!levelsKnownMother && !levelsKnownFather)
                return (statInheritances, mutationInColor);

            var leftMutations = mutationsOccurredCount;
            var statIndicesWithPossibleMutations = usedStats.ToArray();
            var statIndicesWithPossibleMutationsCount = statIndicesWithPossibleMutations.Length;

            // check if some mutations are guaranteed in a specific stat
            // this can only be done if levels of both parents are known
            if (levelsKnownMother && levelsKnownFather)
            {
                bool loopAgain = true;
                while (loopAgain)
                {
                    loopAgain = false;
                    for (int i = 0; i < statIndicesWithPossibleMutationsCount; i++)
                    {
                        var si = statIndicesWithPossibleMutations[i];
                        if (si == -1) continue;

                        var levelMother = creature.Mother.levelsWild[si];
                        var levelFather = creature.Father.levelsWild[si];

                        var possibleMutationsMother = -1;
                        var possibleMutationsFather = -1;
                        for (int m = 0; m <= leftMutations; m++)
                        {
                            if (possibleMutationsMother == -1 && creature.levelsWild[si] == levelMother + m * Ark.LevelsAddedPerMutation)
                                possibleMutationsMother = m;

                            if (possibleMutationsFather == -1 && creature.levelsWild[si] == levelFather + m * Ark.LevelsAddedPerMutation)
                                possibleMutationsFather = m;
                        }

                        // if only one parent can be the stat donor, a possible mutation is guaranteed
                        if (possibleMutationsMother == -1 && possibleMutationsFather == -1)
                        {
                            // the level are not valid
                            break;
                        }

                        if (possibleMutationsMother != -1 && possibleMutationsFather != -1)
                        {
                            // both parents are possible stat donors
                            statInheritances[si] = (byte)(InheritanceFlag(possibleMutationsMother, true)
                                                              | InheritanceFlag(possibleMutationsFather, false));
                            continue;
                        }

                        if (possibleMutationsFather == -1)
                        {
                            // stat was inherited from mother
                            loopAgain = possibleMutationsMother != 0;
                            leftMutations -= possibleMutationsMother;
                            statInheritances[si] = InheritanceFlag(possibleMutationsMother, true);
                        }
                        else if (possibleMutationsMother == -1)
                        {
                            // stat was inherited from father
                            loopAgain = possibleMutationsFather != 0;
                            leftMutations -= possibleMutationsFather;
                            statInheritances[si] = InheritanceFlag(possibleMutationsFather, false);
                        }
                        // this stat now has a known inheritance and doesn't need to be checked again
                        statIndicesWithPossibleMutations[i] = -1;
                        // start loop again if possible mutation count was changed
                        if (loopAgain) break;
                    }
                }
            }
            else
            {
                // one parent is not known, set flags for possible (not guaranteed) inheritance / mutations
                for (int i = 0; i < statIndicesWithPossibleMutationsCount; i++)
                {
                    var si = statIndicesWithPossibleMutations[i];
                    if (si == -1) continue;

                    var levelMother = creature.Mother?.levelsWild?[si] ?? -1;
                    var levelFather = creature.Father?.levelsWild?[si] ?? -1;
                    var possibleMutationsMother = -1;
                    var possibleMutationsFather = -1;
                    for (int m = 0; m <= leftMutations; m++)
                    {
                        if (possibleMutationsMother == -1 && levelMother != -1 && creature.levelsWild[si] ==
                            levelMother + m * Ark.LevelsAddedPerMutation)
                            possibleMutationsMother = m;
                        if (possibleMutationsFather == -1 && levelFather != -1 && creature.levelsWild[si] ==
                            levelFather + m * Ark.LevelsAddedPerMutation)
                            possibleMutationsFather = m;
                    }
                    statInheritances[si] = (byte)((possibleMutationsMother == -1 ? 0 : InheritanceFlag(possibleMutationsMother + 8, true))
                                                | (possibleMutationsFather == -1 ? 0 : InheritanceFlag(possibleMutationsFather + 8, false)));
                }
            }

            byte InheritanceFlag(int mutationCount, bool fromMother)
            {
                // 8 means no mutation but possible inheritance
                return (byte)((mutationCount == 0 ? 8 : mutationCount) << (fromMother ? 0 : 4));
            }

            return (statInheritances, mutationInColor);
        }


        private void PedigreeCreatureCompact_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                CreatureClicked?.Invoke(_creature, -1, e);
        }

        /// <summary>
        /// Returns 2 if stat was possibly inherited from (mother,father), 3 if it was a possible mutation.
        /// </summary>
        public (int maternalInheritance, int paternalInheritance) PossibleStatInheritance(int statIndex)
        {
            if (statIndex == -1) return (0, 0);

            var inheritance = _statInheritances[statIndex];
            var motherInheritance = inheritance & 0xf;
            var fatherInheritance = inheritance >> 4;

            int InheritanceStatus(int status)
            {
                if (status == 0) return 0;
                if (status == 8) return 2;
                return 3;
            }

            return (InheritanceStatus(motherInheritance), InheritanceStatus(fatherInheritance));
        }
    }
}

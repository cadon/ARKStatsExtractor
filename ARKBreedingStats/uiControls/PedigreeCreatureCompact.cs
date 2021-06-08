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
        public const int ControlWidth = StatSize + ColorSize;
        public const int ControlHeight = StatSize;
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


        public PedigreeCreatureCompact()
        {
            Width = StatSize + ColorSize;
            Height = StatSize;
            _tt = new ToolTip
            {
                AutoPopDelay = 10000
            };
            Disposed += (sender, args) => _tt.RemoveAll();
            Click += PedigreeCreatureCompact_Click;
        }

        public PedigreeCreatureCompact(Creature creature) : this()
        {
            SetCreature(creature);
        }

        public void SetCreature(Creature creature)
        {
            _creature = creature;
            DrawData(creature);
        }

        public void SetCreature(int[] levelsWild, int[] colors, Species species, int mutations)
        {
            DrawData(new Creature(species, null, null, null, Sex.Female, levelsWild) { colors = colors, mutationsMaternal = mutations });
        }

        private void DrawData(Creature creature)
        {
            if (creature?.Species == null) return;

            var displayedStats = Enumerable.Range(0, Values.STATS_COUNT).Where(si => si != (int)StatNames.Torpidity && creature.Species.UsesStat(si)).ToArray();
            var anglePerStat = 360 / displayedStats.Length;

            // used for the tooltip text
            var colors = new ArkColor[Species.ColorRegionCount];

            bool mutationHappened = creature.mutationsMaternalNew != 0 || creature.mutationsPaternalNew != 0;
            bool[] possibleMutationInStat = mutationHappened ? new bool[Values.STATS_COUNT] : null;

            Bitmap bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            using (var font = new Font("Microsoft Sans Serif", 8f))
            {
                // stats
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var chartMax = CreatureCollection.CurrentCreatureCollection?.maxChartLevel ?? 50;
                const int radiusInnerCircle = StatSize / 7;
                const int centerCoord = StatSize / 2 - 1;

                var i = 0;
                foreach (var si in displayedStats)
                {
                    var level = creature.levelsWild[si];

                    var statSize = Math.Min((double)level / chartMax, 1);
                    var pieRadius = (int)(radiusInnerCircle + (centerCoord - radiusInnerCircle) * statSize);
                    var leftTop = centerCoord - pieRadius;
                    var angle = AngleOffset + anglePerStat * i++;
                    using (var statBrush = new SolidBrush(Utils.GetColorFromPercent((int)(100 * statSize))))
                        g.FillPie(statBrush, leftTop, leftTop, 2 * pieRadius, 2 * pieRadius, angle, anglePerStat);
                    using (var pen = new Pen(Color.Black))
                        g.DrawPie(pen, leftTop, leftTop, 2 * pieRadius, 2 * pieRadius, angle, anglePerStat);


                    var possibleMutation = mutationHappened && (level == (creature.Mother?.levelsWild[si] ?? 0) + 2
                                                                || level == (creature.Father?.levelsWild[si] ?? 0) + 2);
                    if (possibleMutation)
                    {
                        const int radius = 3;
                        var radiusPosition = centerCoord - radius - 1;
                        var anglePosition = Math.PI * 2 / 360 * (angle + anglePerStat / 2);
                        var x = (int)(radiusPosition * Math.Cos(anglePosition) + radiusPosition);
                        var y = (int)(radiusPosition * Math.Sin(anglePosition) + radiusPosition);
                        DrawFilledCircle(Color.Yellow, x, y, 2 * radius);
                        possibleMutationInStat[si] = true;
                    }
                }

                // draw sex in the center
                using (var brush = new SolidBrush(Utils.AdjustColorLight(Utils.SexColor(creature.sex), 0.2)))
                    g.FillEllipse(brush, centerCoord - radiusInnerCircle, centerCoord - radiusInnerCircle, 2 * radiusInnerCircle, 2 * radiusInnerCircle);
                using (var pen = new Pen(Color.Black))
                    g.DrawEllipse(pen, centerCoord - radiusInnerCircle, centerCoord - radiusInnerCircle, 2 * radiusInnerCircle, 2 * radiusInnerCircle);

                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                using (var brush = new SolidBrush(Color.Black))
                    g.DrawString(Utils.SexSymbol(creature.sex), font, brush,
                        new RectangleF(centerCoord - radiusInnerCircle + 1, centerCoord - radiusInnerCircle + 2, 2 * radiusInnerCircle, 2 * radiusInnerCircle),
                        format);

                // colors
                if (creature.colors != null)
                {
                    var displayedColorRegions = Enumerable.Range(0, Species.ColorRegionCount)
                        .Where(ci => creature.Species.EnabledColorRegions[ci]).ToArray();
                    const int margin = 1;
                    var colorSize = new Size(ColorSize - 3 * margin,
                        StatSize / displayedColorRegions.Length - 2 * margin);
                    i = 0;
                    var left = StatSize + 2 * margin;
                    foreach (var ci in displayedColorRegions)
                    {
                        var color = CreatureColors.CreatureArkColor(creature.colors[ci]);
                        colors[ci] = color;
                        using (var brush = new SolidBrush(color.Color))
                            g.FillRectangle(brush, left, i * (colorSize.Height + 2 * margin), colorSize.Width,
                                colorSize.Height);
                        using (var pen = new Pen(Color.Black))
                            g.DrawRectangle(pen, left, i++ * (colorSize.Height + 2 * margin), colorSize.Width,
                                colorSize.Height);
                    }
                }

                // mutation indicator
                if (creature.Mutations < BreedingPlan.MutationPossibleWithLessThan)
                {
                    const int mutationIndicatorSize = 6;
                    const int topLeft = StatSize - mutationIndicatorSize - 1;
                    DrawFilledCircle(creature.Mutations == 0 ? Color.GreenYellow : Color.Orange, topLeft, topLeft, mutationIndicatorSize);
                }

                void DrawFilledCircle(Color color, int x, int y, int size)
                {
                    using (var brush = new SolidBrush(color))
                        g.FillEllipse(brush, x, y, size, size);
                    using (var pen = new Pen(Color.Black))
                        g.DrawEllipse(pen, x, y, size, size);
                }
            }

            Image?.Dispose();
            Image = bmp;

            var statNames = creature.Species?.statNames;
            _tt.SetToolTip(this, $"{creature.name} ({Utils.SexSymbol(creature.sex)})"
                                 + $"\n{string.Join("\n", displayedStats.Select(si => $"{Utils.StatName(si, true, statNames)}:\t{creature.levelsWild[si],3}{((possibleMutationInStat?[si] ?? false) ? " (possible mutation)" : null)}"))}"
                                 + $"\n{Loc.S("Mutations")}: {creature.mutationsMaternal} (♀) + {creature.mutationsPaternal} (♂) = {creature.Mutations}"
                                 + $"\n{Loc.S("Colors")}\n{string.Join("\n", colors.Select((c, i) => c == null ? null : $"[{i}]:\t{c.Id} ({c.Name})").Where(s => s != null))}");
        }
        public bool Highlight
        {
            set
            {
                // TODO
                //panelHighlight.Visible = value;
                HandCursor = !value;
            }
        }

        public bool HandCursor
        {
            set => Cursor = value ? Cursors.Hand : Cursors.Default;
        }

        private void PedigreeCreatureCompact_Click(object sender, EventArgs e)
        {
            CreatureClicked?.Invoke(_creature);
        }
    }
}

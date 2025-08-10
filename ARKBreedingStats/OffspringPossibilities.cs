using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class OffspringPossibilities : UserControl
    {
        private readonly List<Panel> barPanels;
        private readonly ToolTip tt;
        public int maxWildLevel;
        private int _graphMinLevel;
        private int _graphMaxLevel;
        private double _graphMaxProbability;

        public OffspringPossibilities()
        {
            InitializeComponent();
            barPanels = new List<Panel>();
            tt = new ToolTip
            {
                InitialDelay = 50
            };
            maxWildLevel = 150;
            Paint += PaintLinesAndLabels;
        }

        public void Calculate(Species species, Creature parent1, Creature parent2)
        {
            var levelProbabilities = new Dictionary<int, double>();
            _graphMaxProbability = 0;

            if (parent1?.levelsWild == null || parent2?.levelsWild == null || parent1.levelsWild.Length != Stats.StatsCount || parent2.levelsWild.Length != Stats.StatsCount)
            {
                Clear();
                return;
            }

            List<int> usedStatIndicesTest = new List<int>(Stats.StatsCount);
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (species.CanLevelUpWildOrHaveMutations(s) && s != Stats.Torpidity)
                    usedStatIndicesTest.Add(s);
            }
            int usedStatsCount = usedStatIndicesTest.Count;

            List<int> usedStatIndices = new List<int>(usedStatsCount);

            var levelsHigh = new int[Stats.StatsCount];
            var levelsLow = new int[Stats.StatsCount];

            // first check for equal levels, these can be skipped in the all-combinations loop
            int minimumLevel = 1; // includes the base level and all levels that are equal in both parents
            foreach (var s in usedStatIndicesTest)
            {
                levelsHigh[s] = Math.Max(parent1.levelsWild[s], parent2.levelsWild[s]) +
                                Math.Max(parent1.levelsMutated?[s] ?? 0, parent2.levelsMutated?[s] ?? 0);
                levelsLow[s] = Math.Min(parent1.levelsWild[s], parent2.levelsWild[s]) +
                               Math.Min(parent1.levelsMutated?[s] ?? 0, parent2.levelsMutated?[s] ?? 0);
                if (levelsHigh[s] == levelsLow[s])
                    minimumLevel += levelsHigh[s];
                else
                    usedStatIndices.Add(s);
            }
            usedStatsCount = usedStatIndices.Count;
            int totalLevelCombinations = 1 << usedStatsCount;

            // loop through all combinations the offspring can inherit stat-levels
            // each used stat multiplies the combinations by two
            for (int p = 0; p < totalLevelCombinations; p++)
            {
                int totalLevel = minimumLevel;
                double probability = 1;
                for (int s = 0; s < usedStatsCount; s++)
                {
                    var si = usedStatIndices[s];
                    var probabilityHigherLevel = Ark.ProbabilityInheritHigherLevel +
                                                 parent1.ProbabilityOffsetInheritingHigherLevel(si) +
                                                 parent2.ProbabilityOffsetInheritingHigherLevel(si);
                    // determine if high or low stat-level of parents should be used
                    if ((p & (1 << s)) != 0)
                    {
                        // use high level
                        totalLevel += levelsHigh[si];
                        probability *= probabilityHigherLevel;
                    }
                    else
                    {
                        // use low level
                        totalLevel += levelsLow[si];
                        probability *= 1 - probabilityHigherLevel;
                    }
                }
                if (!levelProbabilities.ContainsKey(totalLevel))
                    levelProbabilities[totalLevel] = probability;
                else levelProbabilities[totalLevel] += probability;

                if (levelProbabilities[totalLevel] > _graphMaxProbability) _graphMaxProbability = levelProbabilities[totalLevel];
            }
            DrawBars(levelProbabilities, _graphMaxProbability);
        }

        private void DrawBars(Dictionary<int, double> levelProbabilities, double maxProbability)
        {
            if (maxProbability == 0)
            {
                Clear();
                return;
            }

            this.SuspendDrawingAndLayout();
            Clear(false);

            int totalWidth = Width;
            int totalHeight = Height - 14;
            double heightMultiplier = totalHeight / maxProbability;

            int barNumber = levelProbabilities.Count;

            if (barNumber > 0)
            {
                int barWidth = totalWidth / barNumber;
                int i = 0;
                foreach (KeyValuePair<int, double> prob in levelProbabilities.OrderBy(l => l.Key))
                {
                    Panel p = new Panel
                    {
                        Width = barWidth + 1,
                        Height = (int)Math.Round(prob.Value * heightMultiplier) + 1
                    };
                    tt.SetToolTip(p, $"Level {prob.Key} ({prob.Value:P})");
                    p.Left = i * barWidth;
                    p.Top = totalHeight - p.Height + 1;
                    p.BackColor = Utils.GetColorFromPercent(100 * (prob.Key - maxWildLevel / 2) / (2 * maxWildLevel)); // color range from maxWildLevel/2 up to 2*maxWildLevel
                    p.BorderStyle = BorderStyle.FixedSingle;
                    Controls.Add(p);
                    barPanels.Add(p);

                    // min/max-labels
                    if (i == 0) _graphMinLevel = prob.Key;
                    if (i == barNumber - 1) _graphMaxLevel = prob.Key;

                    i++;
                }
            }
            this.ResumeDrawingAndLayout();
        }

        private void PaintLinesAndLabels(object sender, PaintEventArgs e)
        {
            const int bottomMargin = 14;

            using (var p = new Pen(Color.Black))
            {
                var g = e.Graphics;
                g.DrawLines(p, new Point[]
                {
                    new Point(0, 0), new Point(0, Height-bottomMargin), new Point(Width, Height-bottomMargin)
                });
                using (var f = new Font(Font, FontStyle.Regular))
                {
                    g.DrawString(_graphMinLevel.ToString(), f, Brushes.Black, 0, Height - bottomMargin);
                    g.DrawString(_graphMaxLevel.ToString(), f, Brushes.Black, Width - 30, Height - bottomMargin);
                    g.DrawString(_graphMaxProbability.ToString("P"), f, Brushes.Black, 0, 0);

                    var stringLevel = Loc.S("Level");
                    var textSize = g.MeasureString(stringLevel, f);
                    g.DrawString(stringLevel, f, Brushes.Black, (Width - textSize.Width) / 2, Height - 14);

                    var stringProb = Loc.S("Probability");
                    var drawFormat = new StringFormat(StringFormatFlags.DirectionVertical);
                    textSize = g.MeasureString(stringProb, f);
                    g.DrawString(stringProb, f, Brushes.Black, 0, textSize.Height + 3, drawFormat);
                }
            }
        }

        public void Clear(bool suspendLayout = true)
        {
            if (suspendLayout)
                this.SuspendDrawingAndLayout();

            tt.RemoveAll();
            foreach (Panel pnl in barPanels)
                pnl.Dispose();
            barPanels.Clear();
            if (suspendLayout)
                this.ResumeDrawingAndLayout();
        }
    }
}

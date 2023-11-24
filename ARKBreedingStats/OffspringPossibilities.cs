using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class OffspringPossibilities : UserControl
    {
        private readonly List<Panel> barPanels;
        private readonly ToolTip tt;
        public int maxWildLevel;

        public OffspringPossibilities()
        {
            InitializeComponent();
            barPanels = new List<Panel>();
            tt = new ToolTip
            {
                InitialDelay = 50
            };
            maxWildLevel = 150;
        }

        public void Calculate(Species species, int[] wildLevels1, int[] wildLevels2)
        {
            var levelProbabilities = new Dictionary<int, double>();
            double maxProbability = 0;

            if (wildLevels1 == null || wildLevels2 == null || wildLevels1.Length != Stats.StatsCount || wildLevels2.Length != Stats.StatsCount)
            {
                Clear(true);
                return;
            }

            List<int> usedStatIndicesTest = new List<int>(Stats.StatsCount);
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (species.UsesStat(s) && s != Stats.Torpidity)
                    usedStatIndicesTest.Add(s);
            }
            int usedStatsCount = usedStatIndicesTest.Count;

            List<int> usedStatIndices = new List<int>(usedStatsCount);
            // first check for equal levels, these can be skipped in the all-combinations loop
            int minimumLevel = 1; // includes the base level and all levels that are equal in both parents
            for (int s = 0; s < usedStatsCount; s++)
            {
                if (wildLevels1[usedStatIndicesTest[s]] == wildLevels2[usedStatIndicesTest[s]])
                {
                    minimumLevel += wildLevels1[usedStatIndicesTest[s]];
                }
                else
                    usedStatIndices.Add(usedStatIndicesTest[s]);
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
                    // determine if stat-level of creature one or two should be used
                    if ((p & (1 << s)) != 0)
                    {
                        // use the stat level of creature 1
                        totalLevel += wildLevels1[usedStatIndices[s]];
                        probability *= wildLevels1[usedStatIndices[s]] > wildLevels2[usedStatIndices[s]] ? Ark.ProbabilityInheritHigherLevel : Ark.ProbabilityInheritLowerLevel;
                    }
                    else
                    {
                        // use the stat level of creature 2
                        totalLevel += wildLevels2[usedStatIndices[s]];
                        probability *= wildLevels1[usedStatIndices[s]] < wildLevels2[usedStatIndices[s]] ? Ark.ProbabilityInheritHigherLevel : Ark.ProbabilityInheritLowerLevel;
                    }
                }
                if (!levelProbabilities.ContainsKey(totalLevel))
                    levelProbabilities[totalLevel] = 0;
                levelProbabilities[totalLevel] += probability;

                if (levelProbabilities[totalLevel] > maxProbability) maxProbability = levelProbabilities[totalLevel];
            }
            DrawBars(levelProbabilities, maxProbability);
        }

        private void DrawBars(Dictionary<int, double> levelProbabilities, double maxProbability)
        {
            if (maxProbability == 0)
            {
                Clear();
                return;
            }

            SuspendLayout();
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
                        Width = barWidth,
                        Height = (int)Math.Ceiling(prob.Value * heightMultiplier)
                    };
                    tt.SetToolTip(p, $"Level {prob.Key} ({Math.Round(prob.Value * 100, 2)}%)");
                    p.Left = i * barWidth;
                    p.Top = totalHeight - p.Height;
                    p.BackColor = Utils.GetColorFromPercent(100 * (prob.Key - maxWildLevel / 2) / (2 * maxWildLevel)); // color range from maxWildLevel/2 up to 2*maxWildLevel
                    p.BorderStyle = BorderStyle.FixedSingle;
                    Controls.Add(p);
                    barPanels.Add(p);

                    // min/max-labels
                    if (i == 0) labelMin.Text = prob.Key.ToString();
                    if (i == barNumber - 1) labelMax.Text = prob.Key.ToString();

                    i++;
                }
                labelMax.Left = i * barWidth - labelMax.Width;
                labelMaxProb.Text = Math.Round(maxProbability * 100, 2) + "%";
            }
            ResumeLayout();
        }

        public void Clear(bool suspendLayout = true)
        {
            if (suspendLayout)
                SuspendLayout();
            panelLine.Height = Height - 14;

            tt.RemoveAll();
            foreach (Panel pnl in barPanels)
                pnl.Dispose();
            barPanels.Clear();
            if (suspendLayout)
                ResumeLayout();
            labelMin.Text = string.Empty;
            labelMax.Text = string.Empty;
            labelMaxProb.Text = string.Empty;
        }
    }
}

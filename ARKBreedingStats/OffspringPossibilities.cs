using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class OffspringPossibilities : UserControl
    {
        public int[] wildLevels1, wildLevels2;
        private Dictionary<int, double> levelProbabilities; // level, probability
        private readonly List<Panel> barPanels;
        private readonly ToolTip tt;
        private double maxProbability;
        public int maxWildLevel;
        private const double probabilityHigherStat = 0.55;
        private readonly double probabilityLowerStat;

        public OffspringPossibilities()
        {
            InitializeComponent();
            barPanels = new List<Panel>();
            tt = new ToolTip
            {
                    InitialDelay = 50
            };
            maxWildLevel = 150;
            probabilityLowerStat = 1 - probabilityHigherStat;
        }

        public void calculate()
        {
            levelProbabilities = new Dictionary<int, double>();
            maxProbability = 0;

            if (wildLevels1 != null && wildLevels2 != null && wildLevels1.Length > 7 && wildLevels2.Length > 7)
            {
                for (int p = 0; p < 128; p++)
                {
                    int totalLevel = 1;
                    double probability = 1;
                    for (int s = 0; s < 7; s++)
                    {
                        if (wildLevels1[s] == wildLevels2[s])
                        {
                            totalLevel += wildLevels1[s];
                            probability *= 0.5;
                        }
                        else
                        {
                            if ((p & (1 << s)) > 0)
                            {
                                totalLevel += wildLevels1[s];
                                probability *= wildLevels1[s] > wildLevels2[s] ? probabilityHigherStat : probabilityLowerStat;
                            }
                            else
                            {
                                totalLevel += wildLevels2[s];
                                probability *= wildLevels1[s] < wildLevels2[s] ? probabilityHigherStat : probabilityLowerStat;
                            }
                        }
                    }
                    if (!levelProbabilities.ContainsKey(totalLevel))
                        levelProbabilities[totalLevel] = 0;
                    levelProbabilities[totalLevel] += probability;

                    if (levelProbabilities[totalLevel] > maxProbability) maxProbability = levelProbabilities[totalLevel];
                }
            }
            drawBars();
        }

        private void drawBars()
        {
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
                    p.BackColor = Utils.getColorFromPercent(100 * (prob.Key - maxWildLevel / 2) / (2 * maxWildLevel)); // color range from maxWildLevel/2 up to 2*maxWildLevel
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

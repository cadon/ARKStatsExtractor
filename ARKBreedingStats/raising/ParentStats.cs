using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.raising
{
    public partial class ParentStats : UserControl
    {
        private readonly List<ParentStatValues> parentStatValues;
        private readonly Label lbLevel;
        public int maxChartLevel;

        public ParentStats()
        {
            InitializeComponent();

            parentStatValues = new List<ParentStatValues>();
            for (int s = 0; s < values.Values.STATS_COUNT; s++)
            {
                ParentStatValues psv = new ParentStatValues();
                psv.StatName = Utils.statName(s, true) + (Utils.precision(s) == 1 ? "" : " %");
                parentStatValues.Add(psv);
                flowLayoutPanel1.SetFlowBreak(psv, true);
            }
            for (int s = 0; s < values.Values.STATS_COUNT; s++)
                flowLayoutPanel1.Controls.Add(parentStatValues[values.Values.statsDisplayOrder[s]]);

            lbLevel = new Label
            {
                Location = new Point(6, 215),
                AutoSize = true
            };
            groupBox1.Controls.Add(lbLevel);

            Clear();
        }

        public void Clear()
        {
            for (int s = 0; s < values.Values.STATS_COUNT; s++)
                parentStatValues[s].setValues();
            lbLevel.Text = "";
        }

        public void setParentValues(Creature mother, Creature father)
        {
            if (mother == null && father == null)
            {
                labelMother.Text = "unknown";
                labelFather.Text = "unknown";
                for (int s = 0; s < values.Values.STATS_COUNT; s++)
                {
                    parentStatValues[s].setValues();
                }
                return;
            }

            Species species = mother?.Species ?? father.Species;

            for (int s = 0; s < values.Values.STATS_COUNT; s++)
            {
                // only display used stats and don't display torpidity
                bool statDisplayed = s != (int)StatNames.Torpidity
                                     && (species.usedStats & (1 << s)) != 0;

                parentStatValues[s].Visible = statDisplayed;
                if (!statDisplayed)
                    continue;

                int bestLevel = -1;
                int bestLevelPercent = 0;
                if (mother != null && father != null)
                {
                    bestLevel = Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                    if (maxChartLevel > 0)
                        bestLevelPercent = (100 * bestLevel) / maxChartLevel;
                }
                parentStatValues[s].setValues(
                    mother == null ? -1 : (mother.valuesBreeding[s] * (Utils.precision(s) == 1 ? 1 : 100)),
                    father == null ? -1 : (father.valuesBreeding[s] * (Utils.precision(s) == 1 ? 1 : 100)),
                    mother != null && father != null ? (mother.valuesBreeding[s] > father.valuesBreeding[s] ? 1 : 2) : 0,
                    bestLevel,
                    bestLevelPercent
                    );
            }
            labelMother.Text = mother == null ? "unknown" : mother.name;
            labelFather.Text = father == null ? "unknown" : (labelMother.Width > 78 ? "\n" : "") + father.name;
            if (mother != null && father != null)
            {
                int minLv = 1, maxLv = 1;
                for (int s = 0; s < 7; s++)
                {
                    if (mother.levelsWild[s] < father.levelsWild[s])
                    {
                        minLv += mother.levelsWild[s];
                        maxLv += father.levelsWild[s];
                    }
                    else
                    {
                        maxLv += mother.levelsWild[s];
                        minLv += father.levelsWild[s];
                    }
                }
                lbLevel.Text = $"Possible Level-Range: {minLv} - {maxLv}";
            }
            else
                lbLevel.Text = "";
        }
    }
}

using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.raising
{
    public partial class ParentStats : UserControl
    {
        private readonly ParentStatValues[] _parentStatValues;
        private readonly Label _lbLevel;
        public int MaxChartLevel;

        public ParentStats()
        {
            InitializeComponent();

            _parentStatValues = new ParentStatValues[Stats.StatsCount];
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var psv = new ParentStatValues
                {
                    StatName = Utils.StatName(s, true) + (Utils.Precision(s) == 1 ? string.Empty : " %")
                };
                _parentStatValues[s] = psv;
                flowLayoutPanel1.SetFlowBreak(psv, true);
            }
            for (int s = 0; s < Stats.StatsCount; s++)
                flowLayoutPanel1.Controls.Add(_parentStatValues[Stats.DisplayOrder[s]]);

            _lbLevel = new Label
            {
                Location = new Point(6, 215),
                AutoSize = true
            };
            groupBox1.Controls.Add(_lbLevel);

            Clear();
        }

        public void Clear()
        {
            for (int s = 0; s < Stats.StatsCount; s++)
                _parentStatValues[s].SetValues();
            _lbLevel.Text = string.Empty;
        }

        public void SetParentValues(Creature mother, Creature father)
        {
            if (mother?.Species == null && father?.Species == null)
            {
                labelMother.Text = Loc.S("Unknown");
                labelFather.Text = Loc.S("Unknown");
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    _parentStatValues[s].SetValues();
                }
                return;
            }

            Species species = mother?.Species ?? father.Species;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                // only display used stats and don't display torpidity
                bool statDisplayed = s != Stats.Torpidity
                                     && species.UsesStat(s);

                _parentStatValues[s].Visible = statDisplayed;
                if (!statDisplayed)
                    continue;

                int bestLevel = -1;
                int bestLevelPercent = 0;
                if (mother?.levelsWild != null && father?.levelsWild != null)
                {
                    bestLevel = Math.Max(mother.levelsWild[s], father.levelsWild[s]);
                    if (MaxChartLevel > 0)
                        bestLevelPercent = (100 * bestLevel) / MaxChartLevel;
                }
                _parentStatValues[s].SetValues(
                    mother?.valuesBreeding == null ? -1 : (mother.valuesBreeding[s] * (Utils.Precision(s) == 1 ? 1 : 100)),
                    father?.valuesBreeding == null ? -1 : (father.valuesBreeding[s] * (Utils.Precision(s) == 1 ? 1 : 100)),
                    mother?.valuesBreeding != null && father?.valuesBreeding != null ? (mother.valuesBreeding[s] > father.valuesBreeding[s] ? 1 : 2) : 0,
                    bestLevel,
                    bestLevelPercent
                    );
            }
            labelMother.Text = mother == null ? Loc.S("Unknown") : mother.name;
            labelFather.Text = father == null ? Loc.S("Unknown") : (labelMother.Width > 78 ? "\n" : string.Empty) + father.name;
            if (mother?.levelsWild != null && father?.levelsWild != null)
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
                _lbLevel.Text = string.Format(Loc.S("possibleLevelRange"), minLv, maxLv);
            }
            else
                _lbLevel.Text = string.Empty;
        }

        public void SetLocalizations()
        {
            Loc.ControlText(label1, "Mother");
            Loc.ControlText(label2, "Father");
            if (_parentStatValues == null) return;

            for (int s = Math.Min(_parentStatValues.Length, Stats.StatsCount) - 1; s >= 0; s--)
                _parentStatValues[s].StatName =
                        Utils.StatName(s, true) + (Utils.Precision(s) == 1 ? string.Empty : " %");
        }
    }
}

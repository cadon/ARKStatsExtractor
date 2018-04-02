using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.raising
{
    public partial class ParentStats : UserControl
    {
        private List<raising.ParentStatValues> parentStatValues;
        private Label lbLevel;

        public ParentStats()
        {
            InitializeComponent();

            parentStatValues = new List<ParentStatValues>();
            for (int s = 0; s < 7; s++)
            {
                ParentStatValues psv = new ParentStatValues();
                psv.Location = new Point(6, 63 + s * 21);
                groupBox1.Controls.Add(psv);
                psv.StatName = Utils.statName(s, true) + (Utils.precision(s) == 1 ? "" : " %");
                parentStatValues.Add(psv);
            }
            lbLevel = new Label();
            lbLevel.Location = new Point(6, 215);
            lbLevel.AutoSize = true;
            groupBox1.Controls.Add(lbLevel);

            Clear();
        }

        public void Clear()
        {
            for (int s = 0; s < 7; s++)
                parentStatValues[s].setValues("-", "-", 0);
            lbLevel.Text = "";
        }

        public void setParentValues(Creature mother, Creature father)
        {
            if (mother == null && father == null)
            {
                labelMother.Text = "unknown";
                labelFather.Text = "unknown";
                for (int s = 0; s < 7; s++)
                {
                    parentStatValues[s].setValues("-", "-", 0);
                }
            }
            else
            {
                for (int s = 0; s < 7; s++)
                {
                    parentStatValues[s].setValues(
                        mother == null ? "-" : (mother.valuesBreeding[s] * (Utils.precision(s) == 1 ? 1 : 100)).ToString("N1"),
                        father == null ? "-" : (father.valuesBreeding[s] * (Utils.precision(s) == 1 ? 1 : 100)).ToString("N1"),
                        mother != null && father != null ? (mother.valuesBreeding[s] > father.valuesBreeding[s] ? 1 : 2) : 0
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
                    lbLevel.Text = "Possible Level-Range: " + minLv.ToString() + " - " + maxLv.ToString();
                }
                else
                    lbLevel.Text = "";
            }
        }
    }
}

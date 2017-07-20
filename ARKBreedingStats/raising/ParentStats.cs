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

        public ParentStats()
        {
            InitializeComponent();

            parentStatValues = new List<ParentStatValues>();
            for (int s = 0; s < 7; s++)
            {
                ParentStatValues psv = new ParentStatValues();
                psv.Location = new Point(6, 42 + s * 21);
                groupBox1.Controls.Add(psv);
                psv.StatName = Utils.statName(s, true) + (Utils.precision(s) == 1 ? "" : " %");
                parentStatValues.Add(psv);
            }

            Clear();
        }

        public void Clear()
        {
            for (int s = 0; s < 7; s++)
                parentStatValues[s].setValues("-", "-", 0);
        }

        public void setParentValues(Creature mother, Creature father)
        {
            for (int s = 0; s < 7; s++)
            {
                parentStatValues[s].setValues(
                    mother == null ? "-" : (mother.valuesBreeding[s] * (Utils.precision(s) == 1 ? 1 : 100)).ToString("N1"),
                    father == null ? "-" : (father.valuesBreeding[s] * (Utils.precision(s) == 1 ? 1 : 100)).ToString("N1"),
                    mother != null && father != null ? (mother.valuesBreeding[s] > father.valuesBreeding[s] ? 1 : 2) : 0
                    );
            }
        }
    }
}

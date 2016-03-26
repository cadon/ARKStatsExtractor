using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class Settings : Form
    {

        private MultiplierSetting[] multSetter;
        private double[][] mainMultis;


        public Settings()
        {
            initStuff();
        }
        public Settings(double[][] multipliers)
        {
            initStuff();
            setMultipliers(multipliers);
        }

        private void initStuff()
        {
            InitializeComponent();
            multSetter = new MultiplierSetting[] { multiplierSettingHP, multiplierSettingSt, multiplierSettingOx, multiplierSettingFo, multiplierSettingWe, multiplierSettingDm, multiplierSettingSp, multiplierSettingTo };
            for (int s = 0; s < 8; s++)
            {
                multSetter[s].StatName = Utils.statName(s);
            }
        }

        private void setMultipliers(double[][] m)
        {
            if (m.Length > 7)
            {
                mainMultis = m;
                for (int s = 0; s < 8; s++)
                {
                    if (m[s].Length > 3)
                    {
                        multSetter[s].Multipliers = m[s];
                    }
                }
            }
        }

        private void saveMultipliers()
        {
            for (int s = 0; s < 8; s++)
            {
                mainMultis[s] = multSetter[s].Multipliers;

            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            saveMultipliers();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAllToOne_Click(object sender, EventArgs e)
        {
            for (int s = 0; s < 8; s++)
            {
                multSetter[s].Multipliers = new double[] { 1, 1, 1, 1 };

            }
        }

        private void buttonSetToOfficial_Click(object sender, EventArgs e)
        {
            //TODO
        }
    }
}

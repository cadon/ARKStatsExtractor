using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class BreedingInfo : UserControl
    {
        public BreedingInfo()
        {
            InitializeComponent();
        }

        public void displayData(int speciesIndex)
        {
            if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count)
            {
                BreedingData breeding = Values.V.species[speciesIndex].breeding;
                string breedingInfo = "";

                string firstTime = "Pregnancy";
                if (breeding.pregnancyTimeAdjusted <= 0)
                    firstTime = "Incubation";


                int babyTime = (int)Math.Ceiling(breeding.maturationTimeAdjusted * .1);
                double fullTime = breeding.maturationTimeAdjusted;

                string[] rowNames = new string[] { firstTime, "Baby", "Maturation" };
                for (int k = 0; k < 3; k++)
                {
                    int t1, totalTime = 0;
                    switch (k)
                    {
                        default:
                        case 0: t1 = breeding.pregnancyTimeAdjusted == 0 ? breeding.incubationTimeAdjusted : breeding.pregnancyTimeAdjusted; totalTime = t1; break;
                        case 1: t1 = (int)(.1f * breeding.maturationTimeAdjusted); totalTime = t1; break;
                        case 2: t1 = breeding.maturationTimeAdjusted; totalTime = breeding.pregnancyTimeAdjusted + breeding.incubationTimeAdjusted + breeding.maturationTimeAdjusted; break;
                    }

                    string[] subitems = new string[] { rowNames[k],
                                                        new TimeSpan(0, 0, t1).ToString("d':'hh':'mm':'ss"),
                                                            new TimeSpan(0, 0, totalTime).ToString("d':'hh':'mm':'ss"),
                                                            DateTime.Now.AddSeconds(totalTime).ToShortTimeString() + ", " + DateTime.Now.AddSeconds(totalTime).ToShortDateString()
                                                    };
                    //listView1.Items.Add(new ListViewItem(subitems));
                    breedingInfo += string.Join(", ", subitems) + "\n";
                }

                breedingInfo += "\n";

                TimeSpan incubation = new TimeSpan(0, 0, breeding.pregnancyTimeAdjusted + breeding.incubationTimeAdjusted);
                TimeSpan growing = new TimeSpan(0, 0, breeding.maturationTimeAdjusted);
                buttonHatching.Text = firstTime;

                // further info
                if (breeding.eggTempMin > 0)
                    breedingInfo += "Egg-Temperature:\n"
                        + (Properties.Settings.Default.celsius ? breeding.eggTempMin : Math.Round(breeding.eggTempMin * 1.8 + 32, 1)) + " - "
                        + (Properties.Settings.Default.celsius ? breeding.eggTempMax : Math.Round(breeding.eggTempMax * 1.8 + 32, 1))
                        + (Properties.Settings.Default.celsius ? " °C" : " °F");
                if (breeding.eggTempMin > 0 && breeding.matingCooldownMinAdjusted > 0)
                    breedingInfo += "\n\n";
                if (breeding.matingCooldownMinAdjusted > 0)
                    breedingInfo += "Time until next mating is possible:\n" + new TimeSpan(0, 0, breeding.matingCooldownMinAdjusted).ToString("d':'hh':'mm") + " - " + new TimeSpan(0, 0, breeding.matingCooldownMaxAdjusted).ToString("d':'hh':'mm");
                labelBreedingInfos.Text = breedingInfo;
            }
        }
    }
}

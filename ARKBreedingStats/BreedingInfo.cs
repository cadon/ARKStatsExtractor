﻿using System;
using System.Windows.Forms;
using ARKBreedingStats.species;

namespace ARKBreedingStats
{
    public partial class BreedingInfo : UserControl
    {
        public BreedingInfo()
        {
            InitializeComponent();
        }

        public void displayData(Species species)
        {
            if (species?.breeding == null) return;
            string breedingInfo = "";

            string firstTime = "Gestation";
            if (species.breeding.gestationTimeAdjusted <= 0)
                firstTime = "Incubation";

            string[] rowNames = { firstTime, "Baby", "Maturation" };
            for (int k = 0; k < 3; k++)
            {
                int t1, totalTime = 0;
                switch (k)
                {
                    default:
                    case 0:
                        t1 = (int)(species.breeding.gestationTimeAdjusted == 0 ? species.breeding.incubationTimeAdjusted : species.breeding.gestationTimeAdjusted);
                        totalTime = t1;
                        break;
                    case 1:
                        t1 = (int)(.1f * species.breeding.maturationTimeAdjusted);
                        totalTime = t1;
                        break;
                    case 2:
                        t1 = (int)species.breeding.maturationTimeAdjusted;
                        totalTime = (int)(species.breeding.gestationTimeAdjusted + species.breeding.incubationTimeAdjusted + species.breeding.maturationTimeAdjusted);
                        break;
                }

                string[] subitems = {
                            rowNames[k],
                            new TimeSpan(0, 0, t1).ToString("d':'hh':'mm':'ss"),
                            new TimeSpan(0, 0, totalTime).ToString("d':'hh':'mm':'ss"),
                            DateTime.Now.AddSeconds(totalTime).ToShortTimeString() + ", " + DateTime.Now.AddSeconds(totalTime).ToShortDateString()
                    };
                //listView1.Items.Add(new ListViewItem(subitems));
                breedingInfo += string.Join(", ", subitems) + "\n";
            }

            breedingInfo += "\n";

            buttonHatching.Text = firstTime;

            // further info
            if (species.breeding.eggTempMin > 0)
            {
                breedingInfo += raising.Raising.eggTemperature(species);
            }
            if (species.breeding.eggTempMin > 0 && species.breeding.matingCooldownMinAdjusted > 0)
                breedingInfo += "\n\n";
            if (species.breeding.matingCooldownMinAdjusted > 0)
                breedingInfo += "Time until next mating is possible:\n" + new TimeSpan(0, 0, (int)species.breeding.matingCooldownMinAdjusted).ToString("d':'hh':'mm")
                              + " - " + new TimeSpan(0, 0, (int)species.breeding.matingCooldownMaxAdjusted).ToString("d':'hh':'mm");
            labelBreedingInfos.Text = breedingInfo;
        }
    }
}

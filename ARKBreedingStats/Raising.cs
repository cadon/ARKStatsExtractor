using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public static class Raising
    {
        public delegate void createIncubationEventHandler(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted);

        public static bool getRaisingTimes(int speciesIndex, out string incubationMode, out TimeSpan incubation, out TimeSpan baby, out TimeSpan maturation, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax)
        {
            incubation = new TimeSpan();
            baby = new TimeSpan();
            maturation = new TimeSpan();
            nextMatingMin = new TimeSpan();
            nextMatingMax = new TimeSpan();
            incubationMode = "";

            if (speciesIndex < 0 || speciesIndex > Values.V.species.Count || Values.V.species[speciesIndex].breeding == null)
                return false;

            BreedingData breeding = Values.V.species[speciesIndex].breeding;

            nextMatingMin = new TimeSpan(0, 0, (int)(breeding.matingCooldownMinAdjusted));
            nextMatingMax = new TimeSpan(0, 0, (int)(breeding.matingCooldownMaxAdjusted));

            incubationMode = "Gestation";
            if (breeding.gestationTimeAdjusted == 0)
                incubationMode = "Incubation";

            incubation = new TimeSpan(0, 0, (int)(breeding.incubationTimeAdjusted + breeding.gestationTimeAdjusted));
            baby = new TimeSpan(0, 0, (int)(.1f * breeding.maturationTimeAdjusted));
            maturation = new TimeSpan(0, 0, (int)(breeding.maturationTimeAdjusted));
            return true;
        }

        public static string eggTemperature(int speciesIndex)
        {
            if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count && Values.V.species[speciesIndex].breeding != null && Values.V.species[speciesIndex].breeding.eggTempMin > 0)
            {
                BreedingData breeding = Values.V.species[speciesIndex].breeding;
                return "Egg-Temperature: "
                    + (Values.V.celsius ? breeding.eggTempMin : Math.Round(breeding.eggTempMin * 1.8 + 32, 1)) + " - "
                    + (Values.V.celsius ? breeding.eggTempMax : Math.Round(breeding.eggTempMax * 1.8 + 32, 1))
                    + (Values.V.celsius ? " °C" : " °F");
            }
            return "";
        }

    }
}

using System;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

namespace ARKBreedingStats.raising
{
    public static class Raising
    {
        public delegate void createIncubationEventHandler(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted);

        public static bool getRaisingTimes(Species species, out string incubationMode, out TimeSpan incubation, out TimeSpan baby, out TimeSpan maturation, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax)
        {
            incubation = new TimeSpan();
            baby = new TimeSpan();
            maturation = new TimeSpan();
            nextMatingMin = new TimeSpan();
            nextMatingMax = new TimeSpan();
            incubationMode = "";

            if (species == null || species.breeding == null)
                return false;

            nextMatingMin = new TimeSpan(0, 0, (int)(species.breeding.matingCooldownMinAdjusted));
            nextMatingMax = new TimeSpan(0, 0, (int)(species.breeding.matingCooldownMaxAdjusted));

            incubationMode = "Gestation";
            if (species.breeding.gestationTimeAdjusted == 0)
                incubationMode = "Incubation";

            incubation = new TimeSpan(0, 0, (int)(species.breeding.incubationTimeAdjusted + species.breeding.gestationTimeAdjusted));
            baby = new TimeSpan(0, 0, (int)(.1f * species.breeding.maturationTimeAdjusted));
            maturation = new TimeSpan(0, 0, (int)(species.breeding.maturationTimeAdjusted));
            return true;
        }

        public static string eggTemperature(Species species)
        {
            if (species != null && species.breeding != null && species.breeding.eggTempMin > 0)
            {
                bool celsius = Properties.Settings.Default.celsius;
                return "Egg-Temperature: "
                    + (celsius ? species.breeding.eggTempMin : Math.Round(species.breeding.eggTempMin * 1.8 + 32, 1)) + " - "
                    + (celsius ? species.breeding.eggTempMax : Math.Round(species.breeding.eggTempMax * 1.8 + 32, 1))
                    + (celsius ? " °C" : " °F");
            }
            return "";
        }

    }
}

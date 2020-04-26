using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;

namespace ARKBreedingStats.raising
{
    public static class Raising
    {
        public delegate void createIncubationEventHandler(Creature mother, Creature father, TimeSpan incubationDuration, bool incubationStarted);

        /// <summary>
        /// Retrieves the times for raising, i.e. for incubation, baby-phase, total maturation, and mating interval.
        /// </summary>
        /// <returns></returns>
        public static bool GetRaisingTimes(Species species, out TimeSpan matingTime, out string incubationMode, out TimeSpan incubation, out TimeSpan baby, out TimeSpan maturation, out TimeSpan nextMatingMin, out TimeSpan nextMatingMax)
        {
            matingTime = new TimeSpan();
            incubation = new TimeSpan();
            baby = new TimeSpan();
            maturation = new TimeSpan();
            nextMatingMin = new TimeSpan();
            nextMatingMax = new TimeSpan();
            incubationMode = null;

            if (species?.breeding == null)
                return false;

            nextMatingMin = new TimeSpan(0, 0, (int)(species.breeding.matingCooldownMinAdjusted));
            nextMatingMax = new TimeSpan(0, 0, (int)(species.breeding.matingCooldownMaxAdjusted));

            incubationMode = species.breeding.gestationTimeAdjusted == 0 ? "Incubation" : "Gestation";

            matingTime = new TimeSpan(0, 0, (int)(species.breeding.matingTimeAdjusted));
            incubation = new TimeSpan(0, 0, (int)(species.breeding.incubationTimeAdjusted + species.breeding.gestationTimeAdjusted));
            baby = new TimeSpan(0, 0, (int)(.1f * species.breeding.maturationTimeAdjusted));
            maturation = new TimeSpan(0, 0, (int)(species.breeding.maturationTimeAdjusted));
            return true;
        }

        /// <summary>
        /// Retrieves the needed temperature range for the eggs of a species.
        /// </summary>
        /// <param name="species"></param>
        /// <returns></returns>
        public static string EggTemperature(Species species)
        {
            if (species?.breeding == null
                || (species.breeding.eggTempMin == 0
                    && species.breeding.eggTempMax == 0)
                )
                return null;

            bool celsius = Properties.Settings.Default.celsius;
            return "Egg-Temperature: "
                + (celsius ? species.breeding.eggTempMin : Math.Round(species.breeding.eggTempMin * 1.8 + 32, 1)) + " – "
                + (celsius ? species.breeding.eggTempMax : Math.Round(species.breeding.eggTempMax * 1.8 + 32, 1))
                + (celsius ? " °C" : " °F");
        }

    }
}

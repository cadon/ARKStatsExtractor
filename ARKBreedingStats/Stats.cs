using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public static class Stats
    {
        public static double calculateValue(int speciesIndex, int stat, int levelWild, int levelDom, bool dom, double tamingEff, double imprintingBonus)
        {
            if (speciesIndex >= 0)
            {
                double add = 0, domMult = 1, imprintingM = 1;
                if (dom)
                {
                    add = Values.V.species[speciesIndex].stats[stat].AddWhenTamed;
                    domMult = (tamingEff >= 0 ? (1 + tamingEff * Values.V.species[speciesIndex].stats[stat].MultAffinity) : 1) * (1 + levelDom * Values.V.species[speciesIndex].stats[stat].IncPerTamedLevel);
                    if (stat != 1 && stat != 2)
                        imprintingM = 1 + 0.2 * imprintingBonus*Values.V.imprintingMultiplier;
                }
                return Math.Round((Values.V.species[speciesIndex].stats[stat].BaseValue * (1 + Values.V.species[speciesIndex].stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) * domMult, Utils.precision(stat), MidpointRounding.AwayFromZero);
            }
            else
                return 0;
        }
    }
}

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
            if (levelWild < 0)
                return -1;
            if (speciesIndex >= 0)
            {
                double add = 0, domMultAffinity = 0, domMult = 1, imprintingM = 1, tamedBaseHP = 1;
                if (dom)
                {
                    add = Values.V.species[speciesIndex].stats[stat].AddWhenTamed;
                    domMultAffinity = Values.V.species[speciesIndex].stats[stat].MultAffinity;
                    // the multiplicative bonus is only multiplied with the TE if it is positive (i.e. negative boni won't get less bad if the TE is low)
                    if (domMultAffinity >= 0)
                        domMultAffinity *= tamingEff;
                    domMult = (tamingEff >= 0 ? (1 + domMultAffinity) : 1) * (1 + levelDom * Values.V.species[speciesIndex].stats[stat].IncPerTamedLevel);
                    if (imprintingBonus > 0 && stat != 1 && stat != 2 && (stat != 6 || Values.V.species[speciesIndex].NoImprintingForSpeed == false))
                        imprintingM = 1 + 0.2 * imprintingBonus * Values.V.imprintingStatScaleMultiplier;
                    if (stat == 0)
                        tamedBaseHP = (float)Values.V.species[speciesIndex].TamedBaseHealthMultiplier;
                }
                double result = Math.Round((Values.V.species[speciesIndex].stats[stat].BaseValue * tamedBaseHP * (1 + Values.V.species[speciesIndex].stats[stat].IncPerWildLevel * levelWild) * imprintingM + add) * domMult, Utils.precision(stat), MidpointRounding.AwayFromZero);
                return result >= 0 ? result : 0;
            }
            else
                return 0;
        }
    }
}

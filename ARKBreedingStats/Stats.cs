using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public class Stats
    {
        private static Stats _Stats;
        public List<List<CreatureStat>> stats = new List<List<CreatureStat>>();
        public List<List<CreatureStat>> statsRaw = new List<List<CreatureStat>>(); // without multipliers

        public static Stats S
        {
            get
            {
                if (_Stats == null)
                {
                    _Stats = new Stats();
                }
                return _Stats;
            }
        }

        public CreatureStat statValue(int species, int stat)
        {
            return stats[species][stat];
        }

        public double calculateValue(int speciesIndex, int stat, int levelWild, int levelDom, bool dom, double tamingEff, double imprintingBonus)
        {
            if (speciesIndex >= 0)
            {
                double add = 0, domMult = 1, imprintingM = 1;
                if (dom)
                {
                    add = Stats.S.stats[speciesIndex][stat].AddWhenTamed;
                    domMult = (tamingEff >= 0 ? (1 + tamingEff * Stats.S.stats[speciesIndex][stat].MultAffinity) : 1) * (1 + levelDom * Stats.S.stats[speciesIndex][stat].IncPerTamedLevel);
                    if (stat != 1 && stat != 2)
                        imprintingM = 1 + 0.2 * imprintingBonus;
                }
                return Math.Round((Stats.S.stats[speciesIndex][stat].BaseValue * (1 + Stats.S.stats[speciesIndex][stat].IncPerWildLevel * levelWild) * imprintingM + add) * domMult, Utils.precision(stat), MidpointRounding.AwayFromZero);
            }
            else
                return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    class Extraction
    {
        public List<StatResult>[] results = new List<StatResult>[8]; // stores the possible results of all stats as array (wildlevel, domlevel, tamingEff)
        public int[] chosenResults = new int[8];
        public bool[] fixedResults = new bool[8];
        public List<int> statsWithEff = new List<int>();
        public bool validResults;
        public bool postTamed;
        public int[] levelDomFromTorporAndTotalRange = new int[] { 0, 0 }, levelWildFromTorporRange = new int[] { 0, 0 }; // 0: min, 1: max
        public int[] lowerBoundWilds = new int[8], lowerBoundDoms = new int[8], upperBoundDoms = new int[8];
        public int wildFreeMax = 0, domFreeMin = 0, domFreeMax = 0; // unassigned levels
        public double imprintingBonusMin, imprintingBonusMax;

        public Extraction()
        {
            for (int s = 0; s < 8; s++)
            {
                chosenResults[s] = 0;
                fixedResults[s] = false;
                results[s] = new List<StatResult>();
            }
        }

        public void Clear()
        {
            for (int s = 0; s < 8; s++)
            {
                chosenResults[s] = 0;
                fixedResults[s] = false;
                results[s].Clear();
            }
            validResults = false;
            statsWithEff.Clear();
            imprintingBonusMax = 0;
            imprintingBonusMin = 0;
        }

        public double uniqueTE()
        {
            double eff = -2;
            if (statsWithEff.Count > 0 && results[statsWithEff[0]].Count > chosenResults[statsWithEff[0]])
            {
                eff = results[statsWithEff[0]][chosenResults[statsWithEff[0]]].TE;
                for (int s = 1; s < statsWithEff.Count; s++)
                {
                    // effectiveness-calculation can be a bit off due to ingame-rounding
                    if (results[statsWithEff[s]].Count <= chosenResults[statsWithEff[s]] || Math.Abs(results[statsWithEff[s]][chosenResults[statsWithEff[s]]].TE - eff) > 0.0025)
                    {
                        return -1; // no unique TE
                    }
                }
            }
            return eff;
        }

        /// <summary>
        /// Marks the results as invalid that violate the given bounds assuming the fixedResults are true
        /// </summary>
        public int filterResultsByFixed(int dontFix = -1)
        {
            int[] lowBoundWs = (int[])lowerBoundWilds.Clone();
            int[] lowBoundDs = (int[])lowerBoundDoms.Clone();
            int[] uppBoundDs = (int[])upperBoundDoms.Clone();
            int wildMax = wildFreeMax, domMin = domFreeMin, domMax = domFreeMax;

            // set all results to non-valid that are in a fixed stat and not the chosen one
            for (int s = 0; s < 7; s++)
            {
                for (int r = 0; r < results[s].Count; r++)
                {
                    results[s][r].currentlyNotValid = (fixedResults[s] && dontFix != s && r != chosenResults[s]);
                }
                // subtract fixed stat-levels, but not from the current stat
                if (fixedResults[s] && dontFix != s)
                {
                    wildMax -= results[s][chosenResults[s]].levelWild;
                    domMin -= results[s][chosenResults[s]].levelDom;
                    domMax -= results[s][chosenResults[s]].levelDom;
                    lowBoundWs[s] = 0;
                    lowBoundDs[s] = 0;
                    uppBoundDs[s] = 0;
                }
            }


            // mark all results as invalid that are not possible with the current fixed chosen results
            // loop as many times as necessary to remove results that depends on the invalidation of results in a later stat
            bool loopAgain = true;
            int validResults, uniqueR;
            while (loopAgain)
            {
                loopAgain = false;
                for (int s = 0; s < 7; s++)
                {
                    validResults = 0;
                    uniqueR = -1;
                    for (int r = 0; r < results[s].Count; r++)
                    {
                        if (!results[s][r].currentlyNotValid)
                            validResults++;
                    }
                    if (validResults > 1)
                    {
                        for (int r = 0; r < results[s].Count; r++)
                        {
                            if (!results[s][r].currentlyNotValid && (results[s][r].levelWild > wildMax - lowBoundWs.Sum() + lowBoundWs[s] || results[s][r].levelDom > domMax - lowBoundDs.Sum() + lowBoundDs[s] || results[s][r].levelDom < domMin - uppBoundDs.Sum() + uppBoundDs[s]))
                            {
                                results[s][r].currentlyNotValid = true;
                                validResults--;
                                // if result gets unique due to this, check if remaining result doesn't violate for max level
                                if (validResults == 1)
                                {
                                    // find unique valid result
                                    for (int rr = 0; rr < results[s].Count; rr++)
                                    {
                                        if (!results[s][rr].currentlyNotValid)
                                        {
                                            uniqueR = rr;
                                            break;
                                        }
                                    }
                                    loopAgain = true;
                                    wildMax -= results[s][uniqueR].levelWild;
                                    domMin -= results[s][uniqueR].levelDom;
                                    domMax -= results[s][uniqueR].levelDom;
                                    lowBoundWs[s] = 0;
                                    lowBoundDs[s] = 0;
                                    uppBoundDs[s] = 0;
                                    if (wildMax < 0 || domMax < 0)
                                    {
                                        return s;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return -1; // -1 is good for this function. A value >=0 means that stat is faulty
        }

        public double ImprintingBonus
        {
            set
            {
                imprintingBonusMin = Math.Round(value) - .5;
                imprintingBonusMax = Math.Round(value) + .5;
            }
        }

        public void extractLevels(int speciesI, int level, List<StatIO> statIOs, double lowerTEBound, double upperTEBound, bool autoDetectTamed, bool tamed, bool justTamed, bool bred, double imprintingBonus, double imprintingBonusMultiplier)
        {
            if (autoDetectTamed)
            {
                // torpor is directly proportional to wild level. Check if creature is wild or tamed (doesn't work with Giganotosaurus because it has no additional bonus on torpor)
                postTamed = (Math.Round(Values.stats[speciesI][7].BaseValue * (1 + Values.stats[speciesI][7].IncPerWildLevel * Math.Round((statIOs[7].Input - Values.stats[speciesI][7].BaseValue) / (Values.stats[speciesI][7].BaseValue * Values.stats[speciesI][7].IncPerWildLevel))), 3) != statIOs[7].Input);
            }
            else
            {
                postTamed = tamed;
            }
            imprintingBonusMin = 0;// imprintingBonus - .005; //TODO add handling of imprinting
            imprintingBonusMax = 0;// imprintingBonus + .005;
            // Torpor-bug: if bonus levels are added due to taming-effectiveness, torpor is too high
            // instead of giving only the TE-bonus, the original wild levels W are added a second time to the torporlevels
            // the game does this after taming: toLvl = (Math.Floor(W*TE/2) > 0 ? 2*W + Math.Min(W*TE/2) : W);
            // max level for wild according to torpor (possible bug ingame: torpor is depending on taming effectiveness 5/3 - 2 times "too high" for level after taming until server-restart (not only the bonus levels are added, but also the existing levels again)
            double torporLevelTamingMultMax = 1, torporLevelTamingMultMin = 1;
            if (postTamed && justTamed)
            {
                torporLevelTamingMultMax = (2 + upperTEBound) / (4 + upperTEBound);
                torporLevelTamingMultMin = (2 + lowerTEBound) / (4 + lowerTEBound);
            }
            levelWildFromTorporRange[0] = (int)Math.Round((statIOs[7].Input - (postTamed ? Values.stats[speciesI][7].AddWhenTamed : 0) - Values.stats[speciesI][7].BaseValue) * torporLevelTamingMultMin / (Values.stats[speciesI][7].BaseValue * Values.stats[speciesI][7].IncPerWildLevel), 0);
            levelWildFromTorporRange[1] = (int)Math.Round((statIOs[7].Input - (postTamed ? Values.stats[speciesI][7].AddWhenTamed : 0) - Values.stats[speciesI][7].BaseValue) * torporLevelTamingMultMax / (Values.stats[speciesI][7].BaseValue * Values.stats[speciesI][7].IncPerWildLevel), 0);
            domFreeMin = 0;
            domFreeMax = 0;
            // lower/upper Bound of each stat (wild has no upper bound as wild-speed and sometimes oxygen is unknown)
            if (postTamed)
            {
                domFreeMin = Math.Max(0, level - levelWildFromTorporRange[1] - 1 - (Values.speciesNames[speciesI] == "Plesiosaur" ? 34 : 0)); // creatures starts with level 1, Plesiosaur starts at level 35
                domFreeMax = Math.Max(0, level - levelWildFromTorporRange[0] - 1 - (Values.speciesNames[speciesI] == "Plesiosaur" ? 34 : 0)); // creatures starts with level 1, Plesiosaur starts at level 35
            }
            levelDomFromTorporAndTotalRange[0] = domFreeMin;
            levelDomFromTorporAndTotalRange[1] = domFreeMax;

            if (bred)
            {
                // bred creatures always have 100% TE
                lowerTEBound = 1;
                upperTEBound = 1;
            }

            // check all possible level-combinations
            for (int s = 0; s < 8; s++)
            {
                if (Values.stats[speciesI][s].BaseValue > 0) // if stat is used (oxygen sometimes is not)
                {
                    statIOs[s].postTame = postTamed;
                    double inputValue = statIOs[s].Input;
                    double tamingEffectiveness = -1;
                    double valueWODom = 0; // value without domesticated levels

                    bool withTEff = (postTamed && Values.stats[speciesI][s].MultAffinity > 0);
                    if (withTEff) { statsWithEff.Add(s); }
                    double maxLW = 0;
                    if (Values.stats[speciesI][s].BaseValue > 0 && Values.stats[speciesI][s].IncPerWildLevel > 0)
                    {
                        maxLW = Math.Round(((inputValue / (postTamed ? 1 + lowerTEBound * Values.stats[speciesI][s].MultAffinity : 1) - (postTamed ? Values.stats[speciesI][s].AddWhenTamed : 0)) / Values.stats[speciesI][s].BaseValue - 1) / Values.stats[speciesI][s].IncPerWildLevel); // floor is too unprecise
                    }
                    if (s != 7 && maxLW > levelWildFromTorporRange[1]) { maxLW = levelWildFromTorporRange[1]; } // torpor level can be too high right after taming (bug ingame?)

                    double maxLD = 0;
                    if (!statIOs[s].DomLevelZero && postTamed && Values.stats[speciesI][s].BaseValue > 0 && Values.stats[speciesI][s].IncPerTamedLevel > 0)
                    {
                        maxLD = Math.Round((inputValue / ((Values.stats[speciesI][s].BaseValue + Values.stats[speciesI][s].AddWhenTamed) * (1 + lowerTEBound * Values.stats[speciesI][s].MultAffinity)) - 1) / Values.stats[speciesI][s].IncPerTamedLevel); //floor is sometimes too unprecise
                    }
                    if (maxLD > domFreeMax) { maxLD = domFreeMax; }

                    for (int w = 0; w < maxLW + 1; w++)
                    {
                        valueWODom = Values.stats[speciesI][s].BaseValue * (1 + Values.stats[speciesI][s].IncPerWildLevel * w) * (1 + imprintingBonusMin * imprintingBonusMultiplier * .2) + (postTamed ? Values.stats[speciesI][s].AddWhenTamed : 0);
                        for (int d = 0; d < maxLD + 1; d++)
                        {
                            if (withTEff)
                            {
                                // taming bonus is dependant on taming-effectiveness
                                // get tamingEffectiveness-possibility
                                // rounding errors need to increase error-range
                                tamingEffectiveness = Math.Round((inputValue / (1 + Values.stats[speciesI][s].IncPerTamedLevel * d) - valueWODom) / (valueWODom * Values.stats[speciesI][s].MultAffinity), 3, MidpointRounding.AwayFromZero);
                                if (tamingEffectiveness < 1.005 && tamingEffectiveness > 1) { tamingEffectiveness = 1; }
                                if (tamingEffectiveness > lowerTEBound - 0.008)
                                {
                                    if (tamingEffectiveness <= upperTEBound)
                                    {
                                        results[s].Add(new StatResult(w, d, tamingEffectiveness));
                                    }
                                    else { continue; }
                                }
                                else
                                {
                                    // if tamingEff < lowerBound: break, in this loop it's getting only smaller
                                    break;
                                }
                            }
                            else if (Math.Abs((valueWODom * (1 + Values.stats[speciesI][s].IncPerTamedLevel * d) - inputValue) * (Utils.precision(s) == 3 ? 100 : 1)) < 0.2)
                            {
                                results[s].Add(new StatResult(w, d));
                                break; // no other solution possible
                            }
                        }
                    }
                }
                else
                {
                    results[s].Add(new StatResult(0, 0));
                }
            }
        }
    }
}

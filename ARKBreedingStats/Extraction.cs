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
        public int[] chosenResults;
        public bool[] fixedResults;
        public List<int> statsWithEff;
        public bool validResults;
        public bool postTamed;
        public bool justTamed;
        public int[] levelDomFromTorporAndTotalRange = new int[] { 0, 0 }, levelWildFromTorporRange = new int[] { 0, 0 }; // 0: min, 1: max
        public int[] lowerBoundWilds, lowerBoundDoms, upperBoundDoms;
        public int wildFreeMax = 0, domFreeMin = 0, domFreeMax = 0; // unassigned levels
        public double imprintingBonus;
        public bool[] activeStats;
        public bool lastTEUnique;

        public Extraction()
        {
            fixedResults = new bool[8];
            chosenResults = new int[8];
            statsWithEff = new List<int>();
            lowerBoundWilds = new int[8];
            lowerBoundDoms = new int[8];
            upperBoundDoms = new int[8];
            activeStats = new bool[8];

            for (int s = 0; s < 8; s++)
            {
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
                lowerBoundWilds[s] = 0;
                lowerBoundDoms[s] = 0;
                upperBoundDoms[s] = 0;
            }
            validResults = false;
            statsWithEff.Clear();
            imprintingBonus = 0;
            lastTEUnique = false;
        }

        public double uniqueTE()
        {
            double eff = -2;
            if (statsWithEff.Count > 0 && results[statsWithEff[0]].Count > chosenResults[statsWithEff[0]])
            {
                eff = -1;
                for (int s = 0; s < statsWithEff.Count; s++)
                {
                    for (int ss = s + 1; ss < statsWithEff.Count; ss++)
                    {
                        // effectiveness-calculation can be a bit off due to ingame-rounding
                        if (results[statsWithEff[ss]].Count <= chosenResults[statsWithEff[ss]]
                            || results[statsWithEff[s]][chosenResults[statsWithEff[s]]].TEMin > results[statsWithEff[ss]][chosenResults[statsWithEff[ss]]].TEMax
                            || results[statsWithEff[s]][chosenResults[statsWithEff[s]]].TEMax < results[statsWithEff[ss]][chosenResults[statsWithEff[ss]]].TEMin)
                        {
                            lastTEUnique = false;
                            return -1; // no unique TE
                        }
                    }
                }
                // calculate most probable real TE
                // TODO do a forward calculation-check instead of just the mean
                double effSum = 0;
                for (int s = 0; s < statsWithEff.Count; s++)
                    effSum += results[statsWithEff[s]][chosenResults[statsWithEff[s]]].TE;
                eff = Math.Round(effSum / statsWithEff.Count, 3);
            }
            lastTEUnique = eff >= 0;
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
            int validResultsNr, uniqueR;
            while (loopAgain)
            {
                loopAgain = false;
                for (int s = 0; s < 7; s++)
                {
                    validResultsNr = 0;
                    uniqueR = -1;
                    for (int r = 0; r < results[s].Count; r++)
                    {
                        if (!results[s][r].currentlyNotValid)
                            validResultsNr++;
                    }
                    if (validResultsNr > 1)
                    {
                        for (int r = 0; r < results[s].Count; r++)
                        {
                            if (!results[s][r].currentlyNotValid && (results[s][r].levelWild > wildMax - lowBoundWs.Sum() + lowBoundWs[s] || results[s][r].levelDom > domMax - lowBoundDs.Sum() + lowBoundDs[s] || results[s][r].levelDom < domMin - uppBoundDs.Sum() + uppBoundDs[s]))
                            {
                                results[s][r].currentlyNotValid = true;
                                validResultsNr--;
                                // if result gets unique due to this, check if remaining result doesn't violate for max level
                                if (validResultsNr == 1)
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
            return -1; // -1 is good for this function. A value >=0 means the stat with that index is faulty
        }

        public void extractLevels(int speciesI, int level, List<StatIO> statIOs, double lowerTEBound, double upperTEBound, bool autoDetectTamed,
            bool tamed, bool justTamed, bool bred, double imprintingBonusRounded, double imprintingBonusMultiplier, double cuddleIntervalMultiplier,
            bool considerWildLevelSteps, int wildLevelSteps, bool adjustToPossibleImprinting, out bool imprintingChanged)
        {
            validResults = true;
            imprintingChanged = false;
            considerWildLevelSteps = considerWildLevelSteps && !bred;

            if (bred)
                postTamed = true;
            else if (autoDetectTamed && Values.V.species[speciesI].stats[7].AddWhenTamed > 0)
            {
                // torpor is directly proportional to wild level. Check if creature is wild or tamed (doesn't work with creatures that have no additive bonus on torpor, e.g. the Giganotosaurus)
                postTamed = (Math.Round(Values.V.species[speciesI].stats[7].BaseValue * (1 + Values.V.species[speciesI].stats[7].IncPerWildLevel * Math.Round((statIOs[7].Input - Values.V.species[speciesI].stats[7].BaseValue) / (Values.V.species[speciesI].stats[7].BaseValue * Values.V.species[speciesI].stats[7].IncPerWildLevel))), 3) != statIOs[7].Input);
            }
            else
                postTamed = tamed;

            imprintingBonus = 0;
            if (bred)
            {
                if (!adjustToPossibleImprinting)
                {
                    imprintingBonus = imprintingBonusRounded;
                }
                else if (Values.V.species[speciesI].breeding != null && Values.V.species[speciesI].breeding.maturationTimeAdjusted > 0)
                {
                    imprintingBonus = Math.Round(Math.Round(imprintingBonusRounded * Values.V.species[speciesI].breeding.maturationTimeAdjusted / (14400 * cuddleIntervalMultiplier))
                        * 14400 * cuddleIntervalMultiplier / Values.V.species[speciesI].breeding.maturationTimeAdjusted, 5);
                    if (imprintingBonus > 1)
                        imprintingBonus = 1;
                    if (Math.Abs(imprintingBonusRounded - imprintingBonus) > 0.01)
                        imprintingChanged = true;
                }
            }
            double imprintingMultiplier = (1 + imprintingBonus * imprintingBonusMultiplier * .2);

            // needed to handle Torpor-bug
            this.justTamed = justTamed;

            // Torpor-bug: if bonus levels are added due to taming-effectiveness, torpor is too high
            // instead of giving only the TE-bonus, the original wild levels W are added a second time to the torporlevels
            // the game does this after taming: toLvl = (Math.Floor(W*TE/2) > 0 ? 2*W + Math.Min(W*TE/2) : W);
            // the game should do (and does after some while, maybe a server-restart): toLvl = W + Math.Min(W*TE/2);
            // max level for wild according to torpor (possible bug ingame: torpor is depending on taming effectiveness 5/3 - 2 times "too high" for level after taming until server-restart (not only the bonus levels are added, but also the existing levels again)
            double torporLevelTamingMultMax, torporLevelTamingMultMin;
            bool runTorporRangeAgain;

            do
            {
                runTorporRangeAgain = false;
                torporLevelTamingMultMax = 1;
                torporLevelTamingMultMin = 1;
                if (postTamed && justTamed && !bred)
                {
                    torporLevelTamingMultMax = (2 + upperTEBound) / (4 + upperTEBound);
                    torporLevelTamingMultMin = (2 + lowerTEBound) / (4 + lowerTEBound);
                }
                levelWildFromTorporRange[0] = (int)Math.Round((statIOs[7].Input / imprintingMultiplier - (postTamed ? Values.V.species[speciesI].stats[7].AddWhenTamed : 0) - Values.V.species[speciesI].stats[7].BaseValue) * torporLevelTamingMultMin / (Values.V.species[speciesI].stats[7].BaseValue * Values.V.species[speciesI].stats[7].IncPerWildLevel), 0);
                levelWildFromTorporRange[1] = (int)Math.Round((statIOs[7].Input / imprintingMultiplier - (postTamed ? Values.V.species[speciesI].stats[7].AddWhenTamed : 0) - Values.V.species[speciesI].stats[7].BaseValue) * torporLevelTamingMultMax / (Values.V.species[speciesI].stats[7].BaseValue * Values.V.species[speciesI].stats[7].IncPerWildLevel), 0);

                // if level of torpor is higher than the total-level, the torporBug displayed a too high torpor. If the user didn't check the justTamed-checkbox, do it for them and recalculate the true torpor-level
                if (!runTorporRangeAgain && !justTamed && levelWildFromTorporRange[0] > level)
                {
                    justTamed = true;
                    this.justTamed = true;
                    runTorporRangeAgain = true;
                }
                // if levelWildFromTorporRange[0] > level, then justTamed has to be true, then run the previous calculation again
            } while (runTorporRangeAgain);

            domFreeMin = 0;
            domFreeMax = 0;
            // lower/upper Bound of each stat (wild has no upper bound as wild-speed and sometimes oxygen is unknown)
            if (postTamed)
            {
                domFreeMin = Math.Max(0, level - levelWildFromTorporRange[1] - 1 - (Values.V.speciesNames[speciesI] == "Plesiosaur" ? 34 : 0)); // creatures starts with level 1, Plesiosaur starts at level 35
                domFreeMax = Math.Max(0, level - levelWildFromTorporRange[0] - 1 - (Values.V.speciesNames[speciesI] == "Plesiosaur" ? 34 : 0)); // creatures starts with level 1, Plesiosaur starts at level 35
            }
            levelDomFromTorporAndTotalRange[0] = domFreeMin;
            levelDomFromTorporAndTotalRange[1] = domFreeMax;
            wildFreeMax = levelWildFromTorporRange[1];

            if (bred)
            {
                // bred creatures always have 100% TE
                lowerTEBound = 1;
                upperTEBound = 1;
            }

            // check all possible level-combinations
            for (int s = 7; s >= 0; s--)
            {
                if (Values.V.species[speciesI].stats[s].BaseValue > 0 && activeStats[s]) // if stat is used (oxygen sometimes is not)
                {
                    statIOs[s].postTame = postTamed;
                    double inputValue = statIOs[s].Input;
                    double statBaseValueWild = Values.V.species[speciesI].stats[s].BaseValue;
                    double statBaseValueTamed = statBaseValueWild * (s == 0 ? (double)Values.V.species[speciesI].TamedBaseHealthMultiplier : 1);

                    double tamingEffectiveness = -1;
                    double valueWODom = 0; // value without domesticated levels

                    bool withTEff = (postTamed && Values.V.species[speciesI].stats[s].MultAffinity > 0);
                    if (withTEff) { statsWithEff.Add(s); }
                    double maxLW = 0;
                    if (Values.V.species[speciesI].stats[s].BaseValue > 0 && Values.V.species[speciesI].stats[s].IncPerWildLevel > 0)
                    {
                        maxLW = Math.Round(((inputValue / (postTamed ? 1 + lowerTEBound * Values.V.species[speciesI].stats[s].MultAffinity : 1) - (postTamed ? Values.V.species[speciesI].stats[s].AddWhenTamed : 0)) / (postTamed ? statBaseValueTamed : statBaseValueWild) - 1) / Values.V.species[speciesI].stats[s].IncPerWildLevel); // floor is too unprecise
                    }
                    if (s != 7 && maxLW > levelWildFromTorporRange[1]) { maxLW = levelWildFromTorporRange[1]; } // torpor level can be too high right after taming (torpor bug in the game)

                    double maxLD = 0;
                    if (!statIOs[s].DomLevelZero && postTamed && Values.V.species[speciesI].stats[s].BaseValue > 0 && Values.V.species[speciesI].stats[s].IncPerTamedLevel > 0)
                    {
                        maxLD = Math.Round((inputValue / ((statBaseValueTamed + Values.V.species[speciesI].stats[s].AddWhenTamed) * (1 + lowerTEBound * Values.V.species[speciesI].stats[s].MultAffinity)) - 1) / Values.V.species[speciesI].stats[s].IncPerTamedLevel); //floor is sometimes too unprecise
                    }
                    if (maxLD > domFreeMax) { maxLD = domFreeMax; }

                    for (int w = 0; w < maxLW + 1; w++)
                    {
                        // imprinting bonus is applied to all stats except stamina (s==1) and oxygen (s==2) and speed (s==6)
                        valueWODom = (postTamed ? statBaseValueTamed : statBaseValueWild) * (1 + Values.V.species[speciesI].stats[s].IncPerWildLevel * w) * (s == 1 || s == 2 || (s == 6 && Values.V.species[speciesI].NoImprintingForSpeed == true) ? 1 : imprintingMultiplier) + (postTamed ? Values.V.species[speciesI].stats[s].AddWhenTamed : 0);
                        for (int d = 0; d < maxLD + 1; d++)
                        {
                            if (withTEff)
                            {
                                // taming bonus is dependant on taming-effectiveness
                                // get tamingEffectiveness-possibility
                                tamingEffectiveness = Math.Round((inputValue / (1 + Values.V.species[speciesI].stats[s].IncPerTamedLevel * d) - valueWODom) / (valueWODom * Values.V.species[speciesI].stats[s].MultAffinity), 4);

                                // calculate rounding-error thresholds. Here it's assumed that the displayed ingame value is maximal 0.6 off of the true ingame value
                                double tamingEffectivenessMax = Math.Round(((inputValue + (Utils.precision(s) == 3 ? 0.0006 : 0.06)) / (1 + Values.V.species[speciesI].stats[s].IncPerTamedLevel * d) - valueWODom) / (valueWODom * Values.V.species[speciesI].stats[s].MultAffinity), 4);
                                double tamingEffectivenessMin = Math.Round(((inputValue - (Utils.precision(s) == 3 ? 0.0006 : 0.06)) / (1 + Values.V.species[speciesI].stats[s].IncPerTamedLevel * d) - valueWODom) / (valueWODom * Values.V.species[speciesI].stats[s].MultAffinity), 4);

                                if (tamingEffectivenessMin <= 1 && tamingEffectiveness > 1) tamingEffectiveness = 1;
                                if (tamingEffectivenessMax >= lowerTEBound)
                                {
                                    if (tamingEffectivenessMin <= upperTEBound)
                                    {
                                        // test if TE with torpor-level of tamed-creatures results in a valid wild-level
                                        double ttttt = trueTorporLevel(tamingEffectiveness);
                                        double ttt = (trueTorporLevel(tamingEffectiveness) + 1) / (1 + tamingEffectiveness / 2);
                                        if (considerWildLevelSteps && s != 7 && tamingEffectiveness > 0)
                                        {
                                            int preTameLevelMin = (int)((trueTorporLevel(tamingEffectiveness) + 1) / (1 + tamingEffectivenessMax / 2));
                                            int preTameLevelMax = (int)Math.Ceiling((trueTorporLevel(tamingEffectiveness) + 1) / (1 + tamingEffectivenessMax / 2));
                                            bool validWildLevel = false;
                                            for (int wl = preTameLevelMin; wl <= preTameLevelMax; wl++)
                                            {
                                                if (wl % wildLevelSteps == 0)
                                                {
                                                    validWildLevel = true;
                                                    break;
                                                }
                                            }
                                            if (!validWildLevel) continue;
                                        }

                                        results[s].Add(new StatResult(w, d, tamingEffectiveness, tamingEffectivenessMin, tamingEffectivenessMax));
                                    }
                                    else continue;
                                }
                                else
                                {
                                    // if tamingEff < lowerBound: break, in this loop it's getting only smaller
                                    break;
                                }
                            }
                            else if (Math.Abs((valueWODom * (1 + Values.V.species[speciesI].stats[s].IncPerTamedLevel * d) - inputValue) * (Utils.precision(s) == 3 ? 100 : 1)) < 0.15)
                            {
                                results[s].Add(new StatResult(w, d));
                                break; // no other solution with this w possible
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

        public bool setStatLevelBounds()
        {
            // substract all uniquely solved stat-levels from possible max and min of sum
            for (int s = 0; s < 7; s++)
            {
                if (results[s].Count == 1)
                {
                    // result is uniquely solved
                    wildFreeMax -= results[s][0].levelWild;
                    domFreeMin -= results[s][0].levelDom;
                    domFreeMax -= results[s][0].levelDom;
                    upperBoundDoms[s] = results[s][0].levelDom;
                }
                else if (results[s].Count > 1)
                {
                    // get the smallest and larges value
                    int minW = results[s][0].levelWild, minD = results[s][0].levelDom, maxD = results[s][0].levelDom;
                    for (int r = 1; r < results[s].Count; r++)
                    {
                        if (results[s][r].levelWild < minW) { minW = results[s][r].levelWild; }
                        if (results[s][r].levelDom < minD) { minD = results[s][r].levelDom; }
                        if (results[s][r].levelDom > maxD) { maxD = results[s][r].levelDom; }
                    }
                    // save min/max-possible value
                    lowerBoundWilds[s] = minW;
                    lowerBoundDoms[s] = minD;
                    upperBoundDoms[s] = maxD;
                }
            }
            if (wildFreeMax < lowerBoundWilds.Sum() || domFreeMax < lowerBoundDoms.Sum())
            {
                Clear();
                validResults = false;
            }
            return validResults;
        }

        /// <summary>
        /// removes all results that violate the stat-level-bounds
        /// </summary>
        /// <returns>-1 on success, else index of stat with error</returns>
        public int removeOutOfBoundsResults()
        {
            // remove all results that violate restrictions
            // loop as many times as necessary to remove results that depends on the removal of results in a later stat
            bool loopAgain = true;
            while (loopAgain)
            {
                loopAgain = false;
                for (int s = 0; s < 7; s++)
                {
                    for (int r = 0; r < results[s].Count; r++)
                    {
                        if (results[s].Count > 1 && (results[s][r].levelWild > wildFreeMax - lowerBoundWilds.Sum() + lowerBoundWilds[s] || results[s][r].levelDom > domFreeMax - lowerBoundDoms.Sum() + lowerBoundDoms[s] || results[s][r].levelDom < domFreeMin - upperBoundDoms.Sum() + upperBoundDoms[s]))
                        {
                            results[s].RemoveAt(r--);
                            // if result gets unique due to this, check if remaining result doesn't violate for max level
                            if (results[s].Count == 1)
                            {
                                loopAgain = true;
                                wildFreeMax -= results[s][0].levelWild;
                                domFreeMin -= results[s][0].levelDom;
                                domFreeMax -= results[s][0].levelDom;
                                lowerBoundWilds[s] = 0;
                                lowerBoundDoms[s] = 0;
                                upperBoundDoms[s] = 0;
                                if (wildFreeMax < 0 || domFreeMax < 0)
                                {
                                    results[s].Clear();
                                    validResults = false;
                                    return s; // this stat has an issue (no valid results)
                                }
                            }
                        }
                    }
                }
            }
            // if more than one parameter is affected by tamingEffectiveness filter all numbers that occure only in one
            if (statsWithEff.Count > 1)
            {
                for (int es = 0; es < statsWithEff.Count; es++)
                {
                    for (int et = es + 1; et < statsWithEff.Count; et++)
                    {
                        List<int> equalEffs1 = new List<int>();
                        List<int> equalEffs2 = new List<int>();
                        for (int ere = 0; ere < results[statsWithEff[es]].Count; ere++)
                        {
                            for (int erf = 0; erf < results[statsWithEff[et]].Count; erf++)
                            {
                                // effectiveness-calculation can be a bit off due to rounding-ingame, use the TE-ranges
                                if (results[statsWithEff[es]][ere].TEMin <= results[statsWithEff[et]][erf].TEMax
                                    && results[statsWithEff[es]][ere].TEMax >= results[statsWithEff[et]][erf].TEMin)
                                {
                                    // if entry is not yet in whitelist, add it
                                    if (equalEffs1.IndexOf(ere) == -1) equalEffs1.Add(ere);
                                    if (equalEffs2.IndexOf(erf) == -1) equalEffs2.Add(erf);
                                }
                            }
                        }
                        // copy all results that have an effectiveness that occurs more than once and replace the others
                        List<StatResult> validResults1 = new List<StatResult>();
                        for (int ev = 0; ev < equalEffs1.Count; ev++)
                        {
                            validResults1.Add(results[statsWithEff[es]][equalEffs1[ev]]);
                        }
                        // replace long list with (hopefully) shorter list with valid entries
                        results[statsWithEff[es]] = validResults1;
                        List<StatResult> validResults2 = new List<StatResult>();
                        for (int ev = 0; ev < equalEffs2.Count; ev++)
                        {
                            validResults2.Add(results[statsWithEff[et]][equalEffs2[ev]]);
                        }
                        results[statsWithEff[et]] = validResults2;
                    }
                    if (es >= statsWithEff.Count - 2)
                    {
                        // only one stat left, not enough to compare it
                        break;
                    }
                }
            }
            return -1;
        }

        public int trueTorporLevel(double te)
        {
            // set Torpor-level (depends on TE due to torpor-bug)
            int torporWildLevel = 0;
            if (results[7].Count > 0)
            {
                torporWildLevel = results[7][0].levelWild;
                if (justTamed)
                {
                    if (te >= 0)
                    {
                        // Torpor-bug: if bonus levels are added due to taming-effectiveness, torpor is too high
                        // instead of giving only the TE-bonus, the original wild levels W are added a second time
                        // the game does this after taming: W = (Math.Floor(W*TE/2) > 0 ? 2*W + Math.Floor(W*TE/2) : W);
                        // First check, if bonus levels are given
                        int bonuslevel = (int)Math.Floor(te * (4 + 2 * torporWildLevel) / (8 + 2 * te));
                        if (bonuslevel > 0)
                        {
                            // now substract the wrongly added levels of torpor
                            torporWildLevel = (torporWildLevel - bonuslevel) / 2 + bonuslevel;
                        }
                    }
                }
            }
            return torporWildLevel;
        }
    }
}

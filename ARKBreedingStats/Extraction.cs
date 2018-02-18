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
        private bool bred;
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
                eff = Math.Round(effSum / statsWithEff.Count, 4);
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
            bool tamed, bool bred, double imprintingBonusRounded, bool adjustImprinting, bool allowMoreThanHundredImprinting, bool extractImprintingFromTorpor, double imprintingBonusMultiplier, double cuddleIntervalMultiplier,
            bool considerWildLevelSteps, int wildLevelSteps, out bool imprintingChanged)
        {
            List<CreatureStat> stats = Values.V.species[speciesI].stats;
            validResults = true;
            imprintingChanged = false;
            considerWildLevelSteps = considerWildLevelSteps && !bred;

            this.bred = bred;
            if (bred)
                postTamed = true;
            else if (autoDetectTamed && stats[7].AddWhenTamed > 0)
            {
                // torpor is directly proportional to wild level. Check if creature is wild or tamed (doesn't work with creatures that have no additive bonus on torpor, e.g. the Giganotosaurus)
                postTamed = (Math.Round(stats[7].BaseValue * (1 + stats[7].IncPerWildLevel * Math.Round((statIOs[7].Input - stats[7].BaseValue) / (stats[7].BaseValue * stats[7].IncPerWildLevel))), 3) != statIOs[7].Input);
            }
            else
                postTamed = tamed;

            imprintingBonus = 0;
            if (bred)
            {
                if (!adjustImprinting)
                {
                    imprintingBonus = imprintingBonusRounded;
                }
                else if (extractImprintingFromTorpor)
                {
                    int wildLevelsFromImprintedTorpor = (int)Math.Round(((((statIOs[7].Input / (1 + stats[7].MultAffinity)) - stats[7].AddWhenTamed) / ((1 + imprintingBonusRounded * 0.2 * imprintingBonusMultiplier) * stats[7].BaseValue)) - 1) / stats[7].IncPerWildLevel);
                    imprintingBonus = ((statIOs[7].Input / (1 + stats[7].MultAffinity) - stats[7].AddWhenTamed) / Stats.calculateValue(speciesI, 7, wildLevelsFromImprintedTorpor, 0, false, 0, 0) - 1) / (0.2 * imprintingBonusMultiplier);

                    // assuming food has no dom-levels, extract the exact imprinting from this stat. If the difference is less than 0.01, take this (probably more precise) value for the imprinting. (food has higher values and yields more precise results)
                    int wildLevelsFromImprintedFood = (int)Math.Round(((((statIOs[3].Input / (1 + stats[3].MultAffinity)) - stats[3].AddWhenTamed) / ((1 + imprintingBonusRounded * 0.2 * imprintingBonusMultiplier) * stats[3].BaseValue)) - 1) / stats[3].IncPerWildLevel);
                    double imprintingBonusFromFood = ((statIOs[3].Input / (1 + stats[3].MultAffinity) - stats[3].AddWhenTamed) / Stats.calculateValue(speciesI, 3, wildLevelsFromImprintedFood, 0, false, 0, 0) - 1) / (0.2 * imprintingBonusMultiplier);
                    if (Math.Abs(imprintingBonus - imprintingBonusFromFood) < 0.01)
                        imprintingBonus = imprintingBonusFromFood;
                }
                else if (Values.V.species[speciesI].breeding != null && Values.V.species[speciesI].breeding.maturationTimeAdjusted > 0)
                {
                    double imprintingGainPerCuddle = Utils.imprintingGainPerCuddle(Values.V.species[speciesI].breeding.maturationTimeAdjusted, cuddleIntervalMultiplier);
                    imprintingBonus = Math.Round(Math.Round(imprintingBonusRounded / imprintingGainPerCuddle) * imprintingGainPerCuddle, 7);
                }
                if (!allowMoreThanHundredImprinting && imprintingBonus > 1)
                    imprintingBonus = 1;
                if (imprintingBonus < 0)
                    imprintingBonus = 0;
                if (Math.Abs(imprintingBonusRounded - imprintingBonus) > 0.01)
                    imprintingChanged = true;
            }
            double imprintingMultiplier = (1 + imprintingBonus * imprintingBonusMultiplier * .2);
            double torporLevelTamingMultMax, torporLevelTamingMultMin;

            torporLevelTamingMultMax = 1;
            torporLevelTamingMultMin = 1;

            levelWildFromTorporRange[0] = (int)Math.Round((statIOs[7].Input / imprintingMultiplier - (postTamed ? stats[7].AddWhenTamed : 0) - stats[7].BaseValue) * torporLevelTamingMultMin / (stats[7].BaseValue * stats[7].IncPerWildLevel), 0);
            levelWildFromTorporRange[1] = (int)Math.Round((statIOs[7].Input / imprintingMultiplier - (postTamed ? stats[7].AddWhenTamed : 0) - stats[7].BaseValue) * torporLevelTamingMultMax / (stats[7].BaseValue * stats[7].IncPerWildLevel), 0);

            domFreeMin = 0;
            domFreeMax = 0;
            // lower/upper Bound of each stat (wild has no upper bound as wild-speed and sometimes oxygen is unknown)
            if (postTamed)
            {
                domFreeMin = Math.Max(0, level - levelWildFromTorporRange[1] - 1);
                domFreeMax = Math.Max(0, level - levelWildFromTorporRange[0] - 1);
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
                if (stats[s].BaseValue > 0 && activeStats[s]) // if stat is used (oxygen sometimes is not)
                {
                    statIOs[s].postTame = postTamed;
                    double inputValue = statIOs[s].Input;
                    double statBaseValue = stats[s].BaseValue;
                    if (postTamed) statBaseValue *= (s == 0 ? (double)Values.V.species[speciesI].TamedBaseHealthMultiplier : 1);

                    double tamingEffectiveness = -1;
                    double valueWODom = 0; // value without domesticated levels

                    bool withTEff = (postTamed && stats[s].MultAffinity > 0);
                    if (withTEff) { statsWithEff.Add(s); }
                    double maxLW = 0;
                    if (stats[s].BaseValue > 0 && stats[s].IncPerWildLevel > 0)
                    {
                        double multAffinityFactor = stats[s].MultAffinity;
                        if (postTamed)
                        {
                            // the multiplicative bonus is only multiplied with the TE if it is positive (i.e. negative boni won't get less bad if the TE is low)
                            if (multAffinityFactor > 0)
                                multAffinityFactor *= lowerTEBound;
                            multAffinityFactor += 1;
                        }
                        else
                            multAffinityFactor = 1;
                        maxLW = Math.Round(((inputValue / multAffinityFactor - (postTamed ? stats[s].AddWhenTamed : 0)) / statBaseValue - 1) / stats[s].IncPerWildLevel); // floor is too unprecise
                    }
                    if (s != 7 && maxLW > levelWildFromTorporRange[1]) { maxLW = levelWildFromTorporRange[1]; } // torpor level can be too high right after taming (torpor bug in the game)

                    double maxLD = 0;
                    if (!statIOs[s].DomLevelZero && postTamed && stats[s].BaseValue > 0 && stats[s].IncPerTamedLevel > 0)
                    {
                        maxLD = Math.Round((inputValue / ((statBaseValue + stats[s].AddWhenTamed) * (1 + lowerTEBound * stats[s].MultAffinity)) - 1) / stats[s].IncPerTamedLevel); //floor is sometimes too unprecise
                    }
                    if (maxLD > domFreeMax) maxLD = domFreeMax;
                    if (maxLD < 0) maxLD = 0;

                    for (int w = 0; w < maxLW + 1; w++)
                    {
                        // imprinting bonus is applied to all stats except stamina (s==1) and oxygen (s==2) and speed (s==6)
                        valueWODom = statBaseValue * (1 + stats[s].IncPerWildLevel * w) * (s == 1 || s == 2 || (s == 6 && Values.V.species[speciesI].NoImprintingForSpeed == true) ? 1 : imprintingMultiplier) + (postTamed ? stats[s].AddWhenTamed : 0);
                        for (int d = 0; d < maxLD + 1; d++)
                        {
                            if (withTEff)
                            {
                                // taming bonus is dependant on taming-effectiveness
                                // get tamingEffectiveness-possibility
                                tamingEffectiveness = Math.Round((inputValue / (1 + stats[s].IncPerTamedLevel * d) - valueWODom) / (valueWODom * stats[s].MultAffinity), 4);

                                // calculate rounding-error thresholds. Here it's assumed that the displayed ingame value is maximal 0.6 off of the true ingame value
                                double tamingEffectivenessMax = Math.Round(((inputValue + (Utils.precision(s) == 3 ? 0.0006 : 0.06)) / (1 + stats[s].IncPerTamedLevel * d) - valueWODom) / (valueWODom * stats[s].MultAffinity), 4);
                                double tamingEffectivenessMin = Math.Round(((inputValue - (Utils.precision(s) == 3 ? 0.0006 : 0.06)) / (1 + stats[s].IncPerTamedLevel * d) - valueWODom) / (valueWODom * stats[s].MultAffinity), 4);

                                if (tamingEffectivenessMin <= 1 && tamingEffectiveness > 1) tamingEffectiveness = 1;
                                if (tamingEffectivenessMax >= lowerTEBound)
                                {
                                    if (tamingEffectivenessMin <= upperTEBound)
                                    {
                                        // test if TE with torpor-level of tamed-creatures results in a valid wild-level
                                        if (considerWildLevelSteps && s != 7 && tamingEffectiveness > 0)
                                        {
                                            int preTameLevelMin = (int)((trueTorporLevel() + 1) / (1 + tamingEffectivenessMax / 2));
                                            int preTameLevelMax = (int)Math.Ceiling((trueTorporLevel() + 1) / (1 + tamingEffectivenessMax / 2));
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
                            else if (Math.Abs((valueWODom * (postTamed ? 1 + stats[s].MultAffinity : 1) * (1 + stats[s].IncPerTamedLevel * d) - inputValue) * (Utils.precision(s) == 3 ? 100 : 1)) < 0.15)
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
            // if creature is bred, all TE is 1 anyway, no need to filter then
            if (!bred && statsWithEff.Count > 1)
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

        public int trueTorporLevel()
        {
            int torporWildLevel = 0;
            if (results[7].Count > 0)
                torporWildLevel = results[7][0].levelWild;
            return torporWildLevel;
        }

        public bool EveryStatHasAtLeastOneResult
        {
            get
            {
                bool r = true;
                for (int s = 0; s < 8; s++)
                {
                    if (results[s].Count == 0)
                    {
                        r = false;
                        break;
                    }
                }
                return r;
            }
        }
    }
}

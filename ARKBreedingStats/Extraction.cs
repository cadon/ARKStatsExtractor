using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats
{
    public class Extraction
    {
        public readonly List<StatResult>[] results = new List<StatResult>[Values.STATS_COUNT]; // stores the possible results of all stats as array (wildlevel, domlevel, tamingEff)
        public readonly int[] chosenResults;
        public readonly bool[] fixedResults;
        public readonly List<int> statsWithTE;
        public bool validResults;
        public bool uniqueResults;
        public bool postTamed;
        private bool bred;
        // lower/upper possible Bound of each stat (wild has no upper bound as wild-speed and sometimes oxygen is unknown,
        // and could be up to levelWildSum, so no results could be filtered out)
        private readonly int[] lowerBoundWilds;
        private readonly int[] lowerBoundDoms;
        private readonly int[] upperBoundDoms;
        private int levelsUndeterminedWild, levelsUndeterminedDom;
        public int levelWildSum, levelDomSum;
        public bool lastTEUnique;
        private MinMaxDouble imprintingBonusRange;
        public IssueNotes.Issue possibleIssues; // possible issues during the extraction, will be shown if extraction failed

        public Extraction()
        {
            fixedResults = new bool[Values.STATS_COUNT];
            chosenResults = new int[Values.STATS_COUNT];
            statsWithTE = new List<int>();
            lowerBoundWilds = new int[Values.STATS_COUNT];
            lowerBoundDoms = new int[Values.STATS_COUNT];
            upperBoundDoms = new int[Values.STATS_COUNT];

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                results[s] = new List<StatResult>();
            }
        }

        public void Clear()
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                chosenResults[s] = 0;
                fixedResults[s] = false;
                results[s].Clear();
                lowerBoundWilds[s] = 0;
                lowerBoundDoms[s] = 0;
                upperBoundDoms[s] = 0;
            }
            validResults = false;
            uniqueResults = false;
            statsWithTE.Clear();
            imprintingBonusRange = new MinMaxDouble(0);
            levelWildSum = 0;
            levelDomSum = 0;
            lastTEUnique = false;
            possibleIssues = IssueNotes.Issue.None;
        }


        public void extractLevels(Species species, int level, List<StatIO> statIOs, double lowerTEBound, double upperTEBound, bool autoDetectTamed,
            bool tamed, bool bred, double imprintingBonusRounded, bool adjustImprinting, bool allowMoreThanHundredImprinting, double imprintingBonusMultiplier, double cuddleIntervalMultiplier,
            bool considerWildLevelSteps, int wildLevelSteps, bool highPrecisionInputs, out bool imprintingChanged)
        {
            List<CreatureStat> stats = species.stats;
            validResults = true;
            imprintingChanged = false;
            considerWildLevelSteps = considerWildLevelSteps
                && !bred
                && species.name.Substring(0, 3) != "Tek"
                && species.name != "Jerboa"
                ;

            this.bred = bred;
            postTamed = bred || tamed;

            // double precision makes it necessary to give a bit more tolerance (hence 0.050001 instead of just 0.05 etc.)
            // the rounding still makes issues, trying +-0.1 in the input often helps. setting the tolerance from the expected 0.05 to 0.06
            // if creatures are imported the precision is much higher and the tolerance is set lower (the numericInput control accepts a higher precision than displayed)
            double statValueInputTolerance = highPrecisionInputs ? 0.0001 : 0.00060001;

            List<MinMaxDouble> imprintingBonusList = new List<MinMaxDouble> { new MinMaxDouble(0) };
            if (bred)
            {
                if (!adjustImprinting)
                {
                    imprintingBonusList[0] = new MinMaxDouble(imprintingBonusRounded);
                }
                else
                {
                    imprintingBonusList = CalculateImprintingBonus(species, imprintingBonusRounded, imprintingBonusMultiplier, cuddleIntervalMultiplier, statIOs[(int)StatNames.Torpidity].Input, statIOs[(int)StatNames.Food].Input);
                }
            }

            for (int IBi = 0; IBi < imprintingBonusList.Count; IBi++)
            {
                imprintingBonusRange = imprintingBonusList[IBi];
                imprintingBonusRange.SetToIntersectionWith(0, (allowMoreThanHundredImprinting ? 5 : 1)); // it's assumed that a valid IB will not be larger than 500%

                var imprintingMultiplierRanges = new List<MinMaxDouble>();
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    imprintingMultiplierRanges.Add(species.statImprintMult[s] != 0
                        ? new MinMaxDouble(1 + imprintingBonusRange.Min * imprintingBonusMultiplier * species.statImprintMult[s],
                                           1 + imprintingBonusRange.Max * imprintingBonusMultiplier * species.statImprintMult[s])
                        : new MinMaxDouble(1));
                }

                var levelWildSumRange = new MinMaxInt((int)Math.Round((statIOs[(int)StatNames.Torpidity].Input / imprintingMultiplierRanges[(int)StatNames.Torpidity].Max - (postTamed ? stats[(int)StatNames.Torpidity].AddWhenTamed : 0) - stats[(int)StatNames.Torpidity].BaseValue) / (stats[(int)StatNames.Torpidity].BaseValue * stats[(int)StatNames.Torpidity].IncPerWildLevel)),
                                                      (int)Math.Round((statIOs[(int)StatNames.Torpidity].Input / imprintingMultiplierRanges[(int)StatNames.Torpidity].Min - (postTamed ? stats[(int)StatNames.Torpidity].AddWhenTamed : 0) - stats[(int)StatNames.Torpidity].BaseValue) / (stats[(int)StatNames.Torpidity].BaseValue * stats[(int)StatNames.Torpidity].IncPerWildLevel)));
                var levelDomSumRange = new MinMaxInt(Math.Max(0, level - 1 - levelWildSumRange.Max),
                                                     Math.Max(0, level - 1 - levelWildSumRange.Min));

                levelWildSum = levelWildSumRange.Min;
                levelDomSum = levelDomSumRange.Min; // TODO implement range-mechanic

                levelsUndeterminedWild = levelWildSum;
                levelsUndeterminedDom = levelDomSum;

                if (bred)
                {
                    // bred creatures always have 100% TE
                    lowerTEBound = 1;
                    upperTEBound = 1;
                }
                else
                {
                    // sometimes it fails due to double-precision errors, e.g.
                    // Pteranodon (Lvl 34, TE: 80%): HP: 415.9 (6, 0); St: 195 (6, 0); Ox: 240 (6, 0); Fo: 2150.4 (6, 0); We: 134.4 (6, 0); Dm: 141.6% (3, 0); Sp: 135% (0, 0); To: 358.1 (33);
                    // will fail the extraction with a lowerTEBound of 0.8, it only extracts with a lowerTEBound of 0.79, then displays 0.8 as result for the TE. Adding these margins make it work as expected.
                    lowerTEBound -= 0.0006;
                    upperTEBound += 0.0006;
                }

                // check all possible level-combinations
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (stats[s].BaseValue > 0 && statIOs[s].Input > 0) // if stat is used (oxygen sometimes is not)
                    {
                        statIOs[s].postTame = postTamed;
                        MinMaxDouble inputValue = new MinMaxDouble(statIOs[s].Input - (Utils.precision(s) == 3 ? statValueInputTolerance : statValueInputTolerance * 100), statIOs[s].Input + (Utils.precision(s) == 3 ? statValueInputTolerance : statValueInputTolerance * 100));
                        double statBaseValue = stats[s].BaseValue;
                        if (postTamed && s == (int)StatNames.Health) statBaseValue *= (double)species.TamedBaseHealthMultiplier;// + 0.00000000001; // todo double-precision handling

                        bool withTEff = (postTamed && stats[s].MultAffinity > 0);
                        if (withTEff) { statsWithTE.Add(s); }
                        int minLW = 0, maxLW = 0;
                        if (stats[s].IncPerWildLevel > 0)
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
                            maxLW = (int)Math.Round(((inputValue.Max / multAffinityFactor - (postTamed ? stats[s].AddWhenTamed : 0)) / statBaseValue - 1) / stats[s].IncPerWildLevel); // floor is too unprecise
                        }
                        else
                        {
                            minLW = -1;
                            maxLW = -1;
                        }
                        if (maxLW > levelWildSum) { maxLW = levelWildSum; }

                        double maxLD = 0;
                        if (!statIOs[s].DomLevelLockedZero && postTamed && species.UsesStat(s) && species.DisplaysStat(s) && stats[s].IncPerTamedLevel > 0)
                        {
                            int ww = 0; // base wild level for the tamed creature needed to be alive
                            if (statBaseValue + stats[s].AddWhenTamed < 0)
                            {
                                // e.g. Griffin
                                // get lowest wild level at which the creature is alive
                                while (Stats.calculateValue(species, s, ww, 0, true, lowerTEBound, 0) <= 0)
                                {
                                    ww++;
                                }
                            }
                            maxLD = Math.Round((inputValue.Max / ((statBaseValue * (1 + stats[s].IncPerWildLevel * ww) + stats[s].AddWhenTamed) * (1 + lowerTEBound * stats[s].MultAffinity)) - 1) / stats[s].IncPerTamedLevel); //floor is sometimes too low
                        }
                        if (maxLD > levelsUndeterminedDom) maxLD = levelsUndeterminedDom;
                        if (maxLD < 0) maxLD = 0;

                        MinMaxDouble statImprintingMultiplierRange = new MinMaxDouble(1);
                        // only use imprintingMultiplier for stats that use them. Stamina and Oxygen don't use ist. Sometimes speed neither.
                        if (bred && species.statImprintMult[s] != 0)
                            statImprintingMultiplierRange = imprintingMultiplierRanges[s].Clone();

                        // if dom levels have no effect, just calculate the wild level
                        // for flyers (without mods) this means for speed, no wild levels at all (i.e. not unknown, but 0)
                        // for the Diplodocus this means 0 wild levels in melee
                        if (stats[s].IncPerTamedLevel == 0)
                        {
                            if (minLW == -1)
                                //results[s].Add(new StatResult(-1, 0, inputValue.Mean));
                                results[s].Add(new StatResult(0, 0, inputValue.Mean));
                            else
                            {
                                MinMaxDouble lwRange = new MinMaxDouble(((inputValue.Min / (postTamed ? 1 + stats[s].MultAffinity : 1) - (postTamed ? stats[s].AddWhenTamed : 0)) / (statBaseValue * statImprintingMultiplierRange.Max) - 1) / stats[s].IncPerWildLevel,
                                                                        ((inputValue.Max / (postTamed ? 1 + stats[s].MultAffinity : 1) - (postTamed ? stats[s].AddWhenTamed : 0)) / (statBaseValue * statImprintingMultiplierRange.Min) - 1) / stats[s].IncPerWildLevel);
                                int lw = (int)Math.Round(lwRange.Mean);
                                if (lwRange.Includes(lw) && lw >= 0 && lw <= maxLW)
                                {
                                    results[s].Add(new StatResult(lw, 0, inputValue.Mean));
                                }
                            }
                            // even if no result was found, there is no other valid
                            continue;
                        }

                        for (int lw = minLW; lw < maxLW + 1; lw++)
                        {
                            // imprinting bonus is applied to all stats except stamina (s==1) and oxygen (s==2) and speed (s==6)
                            MinMaxDouble valueWODomRange = new MinMaxDouble(statBaseValue * (1 + stats[s].IncPerWildLevel * lw) * statImprintingMultiplierRange.Min + (postTamed ? stats[s].AddWhenTamed : 0),
                                                                            statBaseValue * (1 + stats[s].IncPerWildLevel * lw) * statImprintingMultiplierRange.Max + (postTamed ? stats[s].AddWhenTamed : 0)); // value without domesticated levels
                            if (!withTEff)
                            {
                                // calculate the only possible Ld, if it's an integer, take it.
                                if (stats[s].IncPerTamedLevel > 0)
                                {
                                    MinMaxDouble ldRange = new MinMaxDouble((inputValue.Min / (valueWODomRange.Max * (postTamed ? 1 + stats[s].MultAffinity : 1)) - 1) / stats[s].IncPerTamedLevel,
                                                                            (inputValue.Max / (valueWODomRange.Min * (postTamed ? 1 + stats[s].MultAffinity : 1)) - 1) / stats[s].IncPerTamedLevel);
                                    int ld = (int)Math.Round(ldRange.Mean);
                                    if (ldRange.Includes(ld) && ld >= 0 && ld <= maxLD)
                                    {
                                        results[s].Add(new StatResult(lw, ld, inputValue.Mean));
                                    }
                                }
                                else
                                {
                                    results[s].Add(new StatResult(lw, 0, inputValue.Mean));
                                }
                            }
                            else
                            {
                                for (int ld = 0; ld <= maxLD; ld++)
                                {
                                    // taming bonus is dependant on taming-effectiveness
                                    // get tamingEffectiveness-possibility
                                    // calculate rounding-error thresholds. Here it's assumed that the displayed ingame value is maximal 0.5 off of the true ingame value
                                    MinMaxDouble tamingEffectiveness = new MinMaxDouble((inputValue.Min / (1 + stats[s].IncPerTamedLevel * ld) - valueWODomRange.Max) / (valueWODomRange.Max * stats[s].MultAffinity),
                                                                                        (inputValue.Max / (1 + stats[s].IncPerTamedLevel * ld) - valueWODomRange.Min) / (valueWODomRange.Min * stats[s].MultAffinity));

                                    if (tamingEffectiveness.Min > upperTEBound)
                                        continue;
                                    if (tamingEffectiveness.Max < lowerTEBound)
                                        break; // if tamingEff < lowerBound: break, in this d-loop it's getting only smaller

                                    // here it's ensured the TE overlaps the bounds, so we can clamp it to the bounds
                                    if (tamingEffectiveness.Min < lowerTEBound) tamingEffectiveness.Min = lowerTEBound;
                                    if (tamingEffectiveness.Max > upperTEBound) tamingEffectiveness.Max = upperTEBound;

                                    if (!bred)
                                    {
                                        // check if the totalLevel and the TE is possible by using the TE-levelbonus (credits for this check which sorts out more impossible results: https://github.com/VolatilePulse , thanks!)
                                        int levelPostTame = levelWildSum + 1;
                                        MinMaxInt levelPreTameRange = new MinMaxInt((int)Math.Ceiling(levelPostTame / (1 + tamingEffectiveness.Max / 2)),
                                                                               (int)Math.Ceiling(levelPostTame / (1 + tamingEffectiveness.Min / 2)));

                                        bool impossibleTE = true;
                                        for (int wildLevel = levelPreTameRange.Min; wildLevel <= levelPreTameRange.Max; wildLevel++)
                                        {
                                            MinMaxInt levelPostTameRange = new MinMaxInt((int)Math.Floor(wildLevel * (1 + tamingEffectiveness.Min / 2)),
                                                                                    (int)Math.Floor(wildLevel * (1 + tamingEffectiveness.Max / 2)));
                                            if (levelPostTameRange.Includes(levelPostTame))
                                            {
                                                impossibleTE = false;
                                                break;
                                            }
                                        }
                                        if (impossibleTE) continue;

                                        // test if TE with torpor-level of tamed-creatures results in a valid wild-level according to the possible levelSteps
                                        if (considerWildLevelSteps)
                                        {
                                            bool validWildLevel = false;
                                            for (int wildLevel = levelPreTameRange.Min; wildLevel <= levelPreTameRange.Max; wildLevel++)
                                            {
                                                if (wildLevel % wildLevelSteps == 0)
                                                {
                                                    validWildLevel = true;
                                                    break;
                                                }
                                            }
                                            if (!validWildLevel) continue;
                                        }

                                        // if another stat already is dependant on TE, check if this TE overlaps any of their TE-ranges. If not, TE is not possible (a creature can only have the same TE for all TE-dependant stats)
                                        if (statsWithTE.Count > 1)
                                        {
                                            bool TEExistant = false;
                                            for (int er = 0; er < results[statsWithTE[0]].Count; er++)
                                            {
                                                if (tamingEffectiveness.Overlaps(results[statsWithTE[0]][er].TE))
                                                {
                                                    TEExistant = true;
                                                    break;
                                                }
                                            }
                                            if (!TEExistant) continue;
                                        }
                                    }

                                    results[s].Add(new StatResult(lw, ld, inputValue.Mean, tamingEffectiveness));
                                }
                            }
                        }
                    }
                    else
                    {
                        results[s].Add(new StatResult(-1, 0));
                    }
                }
                if (bred)
                {
                    // if each stat has at least one result, assume the extraction was valid with the chosen IB
                    if (EveryStatHasAtLeastOneResult)
                    {
                        // all stats have a result, don't test the other possible IBs
                        imprintingChanged = (Math.Abs(imprintingBonusRounded - imprintingBonus) > 0.01);
                        break;
                    }
                    else if (IBi < imprintingBonusList.Count - 1)
                    {
                        // not all stats got a result, clear results for the next round
                        Clear();
                        validResults = true;
                    }
                }
            }
        }

        private List<MinMaxDouble> CalculateImprintingBonus(Species species, double imprintingBonusRounded, double imprintingBonusMultiplier, double cuddleIntervalMultiplier, double torpor, double food)
        {
            List<MinMaxDouble> imprintingBonusList = new List<MinMaxDouble>();
            if (species.stats[(int)StatNames.Torpidity].BaseValue == 0 || species.stats[(int)StatNames.Torpidity].IncPerWildLevel == 0) return imprintingBonusList; // invalid species-data

            // classic way to calculate the ImprintingBonus, this is the most exact value, but will not work if the imprinting-gain was different (e.g. events, mods (S+Nanny))
            double imprintingBonusFromGainPerCuddle = 0;
            if (species.breeding != null)
            {
                double imprintingGainPerCuddle = Utils.imprintingGainPerCuddle(species.breeding.maturationTimeAdjusted, cuddleIntervalMultiplier);
                imprintingBonusFromGainPerCuddle = Math.Round(imprintingBonusRounded / imprintingGainPerCuddle) * imprintingGainPerCuddle;
            }

            MinMaxInt wildLevelsFromImprintedTorpor = new MinMaxInt(
                (int)Math.Round(((((torpor / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity)) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / ((1 + (imprintingBonusRounded + 0.005) * species.statImprintMult[(int)StatNames.Torpidity] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Torpidity].BaseValue)) - 1) / species.stats[(int)StatNames.Torpidity].IncPerWildLevel),
                (int)Math.Round(((((torpor / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity)) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / ((1 + (imprintingBonusRounded - 0.005) * species.statImprintMult[(int)StatNames.Torpidity] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Torpidity].BaseValue)) - 1) / species.stats[(int)StatNames.Torpidity].IncPerWildLevel));

            // assuming food has no dom-levels, extract the exact imprinting from this stat. If the range is in the range of the torpor-dependant IB, take this more precise value for the imprinting. (food has higher values and yields more precise results)
            MinMaxInt wildLevelsFromImprintedFood = new MinMaxInt(
                (int)Math.Round(((((food / (1 + species.stats[(int)StatNames.Food].MultAffinity)) - species.stats[(int)StatNames.Food].AddWhenTamed) / ((1 + (imprintingBonusRounded + 0.005) * species.statImprintMult[(int)StatNames.Food] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Food].BaseValue)) - 1) / species.stats[(int)StatNames.Food].IncPerWildLevel),
                (int)Math.Round(((((food / (1 + species.stats[(int)StatNames.Food].MultAffinity)) - species.stats[(int)StatNames.Food].AddWhenTamed) / ((1 + (imprintingBonusRounded - 0.005) * species.statImprintMult[(int)StatNames.Food] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Food].BaseValue)) - 1) / species.stats[(int)StatNames.Food].IncPerWildLevel));

            List<int> otherStatsSupportIB = new List<int>(); // the number of other stats that support this IB-range
                                                             // for high-level creatures the bonus from imprinting is so high, that a displayed and rounded value of the imprinting bonus can be possible with multiple torpor-levels, i.e. 1 %point IB results in a larger change than a level in torpor.
            for (int torporLevel = wildLevelsFromImprintedTorpor.Min; torporLevel <= wildLevelsFromImprintedTorpor.Max; torporLevel++)
            {
                int support = 0;
                double imprintingProductTorpor = species.statImprintMult[(int)StatNames.Torpidity] * imprintingBonusMultiplier;
                double imprintingProductFood = species.statImprintMult[(int)StatNames.Food] * imprintingBonusMultiplier;
                if (imprintingProductTorpor == 0 || imprintingProductFood == 0) break;
                MinMaxDouble imprintingBonusRange = new MinMaxDouble(
                    (((torpor - 0.05) / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / Stats.calculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, false, 0, 0) - 1) / imprintingProductTorpor,
                    (((torpor + 0.05) / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / Stats.calculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, false, 0, 0) - 1) / imprintingProductTorpor);

                // check for each possible food-level the IB-range and if it can narrow down the range derived from the torpor (deriving from food is more precise, due to the higher values)
                for (int foodLevel = wildLevelsFromImprintedFood.Min; foodLevel <= wildLevelsFromImprintedFood.Max; foodLevel++)
                {
                    MinMaxDouble imprintingBonusFromFood = new MinMaxDouble(
                        (((food - 0.05) / (1 + species.stats[(int)StatNames.Food].MultAffinity) - species.stats[(int)StatNames.Food].AddWhenTamed) / Stats.calculateValue(species, (int)StatNames.Food, foodLevel, 0, false, 0, 0) - 1) / imprintingProductFood,
                        (((food + 0.05) / (1 + species.stats[(int)StatNames.Food].MultAffinity) - species.stats[(int)StatNames.Food].AddWhenTamed) / Stats.calculateValue(species, (int)StatNames.Food, foodLevel, 0, false, 0, 0) - 1) / imprintingProductFood);


                    // NOTE. it's assumed if the IB-food is in the range of IB-torpor, the values are correct. This doesn't have to be true, but is very probable. If extraction-issues appear, this assumption could be the reason.
                    //if (imprintingBonusFromTorpor.Includes(imprintingBonusFromFood)
                    if (imprintingBonusRange.Overlaps(imprintingBonusFromFood))
                    {
                        MinMaxDouble intersectionIB = new MinMaxDouble(imprintingBonusRange);
                        intersectionIB.SetToIntersectionWith(imprintingBonusFromFood);
                        if (Stats.calculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, true, 1, intersectionIB.Min) <= torpor
                            && Stats.calculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, true, 1, intersectionIB.Max) >= torpor)
                        {
                            //imprintingBonusFromTorpor = imprintingBonusFromFood;
                            imprintingBonusRange.SetToIntersectionWith(imprintingBonusFromFood);
                            support++;
                        }
                    }
                }

                // if classic method results in value in the possible range, take this, probably most exact value
                if (imprintingBonusRange.Includes(imprintingBonusFromGainPerCuddle)
                    && Stats.calculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, true, 1, imprintingBonusFromGainPerCuddle) == torpor)
                {
                    imprintingBonusRange.MinMax = imprintingBonusFromGainPerCuddle;
                    support++;
                }

                // TODO check if this range has already been added to avoid double loops in the extraction. if existant, update support
                imprintingBonusList.Add(imprintingBonusRange);
                otherStatsSupportIB.Add(support);
            }

            // sort IB according to the support they got by other stats, then return the distinct means of the possible ranges.
            return imprintingBonusList.OrderByDescending(i => otherStatsSupportIB[imprintingBonusList.IndexOf(i)]).ToList();
        }

        public void RemoveImpossibleTEsAccordingToMaxWildLevel(int maxWildLevel)
        {
            if (!bred
                && maxWildLevel > 0
                && levelWildSum + 1 > maxWildLevel)
            {
                double minTECheck = 2d * (levelWildSum + 1 - maxWildLevel) / maxWildLevel;

                // if min TE is equal or greater than 1, that indicates it can't possibly be anything but bred, and there cannot be any results that should be sorted out
                if (minTECheck < 1)
                {
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (results[s].Count == 0 || results[s][0].TE.Max < 0)
                            continue;
                        for (int r = 0; r < results[s].Count; r++)
                        {
                            if (results[s][r].TE.Max < minTECheck)
                                results[s].RemoveAt(r--);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// sets bounds to all stats and removes all results that violate the stat-level-bounds. also filter results by TE.
        /// </summary>
        /// <param name="statError">stat-index when failed. If equal to -1 and filtering failed, it's probably an issue of the total level</param>
        /// <returns>Success</returns>
        public bool setStatLevelBoundsAndFilter(out int statError)
        {
            statError = -1;

            levelsUndeterminedWild = levelWildSum;
            levelsUndeterminedDom = levelDomSum;
            // substract all uniquely solved stat-levels from possible max and min of sum
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                adjustBoundsToStatResults(s);
            }
            if (levelsUndeterminedWild < lowerBoundWilds.Sum() || levelsUndeterminedDom < lowerBoundDoms.Sum())
            {
                Clear();
                validResults = false;
                return validResults;
            }

            // remove all results that violate restrictions
            // loop as many times as necessary to remove results that depends on the removal of results in a later stat
            bool filterBoundsAgain, filterBoundsAndTEAgain;
            do
            {
                filterBoundsAndTEAgain = false;
                do
                {
                    filterBoundsAgain = false;
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (s == (int)StatNames.Torpidity) continue;
                        for (int r = 0; r < results[s].Count; r++)
                        {
                            if (results[s].Count > 1
                                && (results[s][r].levelWild > levelsUndeterminedWild - lowerBoundWilds.Sum() + lowerBoundWilds[s]
                                 || results[s][r].levelDom > levelsUndeterminedDom - lowerBoundDoms.Sum() + lowerBoundDoms[s]
                                 || results[s][r].levelDom < levelsUndeterminedDom - upperBoundDoms.Sum() + upperBoundDoms[s]))
                            {
                                // if the sorted out result could affect the bounds, set the bounds again
                                bool adjustBounds = results[s][r].levelWild == lowerBoundWilds[s]
                                                   || results[s][r].levelDom == lowerBoundDoms[s]
                                                   || results[s][r].levelDom == upperBoundDoms[s];

                                // remove result that violated the restrictions
                                results[s].RemoveAt(r--);

                                // if result gets unique due to this, check if remaining result doesn't violate for max level
                                if (results[s].Count == 1 || adjustBounds)
                                {
                                    filterBoundsAgain = adjustBoundsToStatResults(s) || filterBoundsAgain;

                                    // check if undeterminedLevels are too low
                                    if (levelsUndeterminedWild < 0 || levelsUndeterminedDom < 0)
                                    {
                                        results[s].Clear();
                                        validResults = false;
                                        statError = s; // this stat has an issue (no valid results)
                                        return validResults;
                                    }
                                }
                            }
                        }
                    }
                } while (filterBoundsAgain);

                // if more than one parameter is affected by tamingEffectiveness filter out all numbers that occure not in all
                // if creature is bred, all TE is 1 anyway, no need to filter then
                if (!bred && statsWithTE.Count > 1)
                {
                    for (int es = 0; es < statsWithTE.Count - 1; es++)
                    {
                        for (int et = es + 1; et < statsWithTE.Count; et++)
                        {
                            List<int> equalEffs1 = new List<int>();
                            List<int> equalEffs2 = new List<int>();
                            for (int ere = 0; ere < results[statsWithTE[es]].Count; ere++)
                            {
                                for (int erf = 0; erf < results[statsWithTE[et]].Count; erf++)
                                {
                                    // test if the TE-ranges overlap each other, if yes, add them to whitelist
                                    if (results[statsWithTE[es]][ere].TE.Overlaps(results[statsWithTE[et]][erf].TE))
                                    {
                                        if (!equalEffs1.Contains(ere)) equalEffs1.Add(ere);
                                        if (!equalEffs2.Contains(erf)) equalEffs2.Add(erf);
                                    }
                                }
                            }

                            // copy all results that have an effectiveness that occurs more than once and replace the others
                            List<StatResult> validResults1 = new List<StatResult>();
                            foreach (int ev in equalEffs1)
                            {
                                validResults1.Add(results[statsWithTE[es]][ev]);
                            }
                            // replace long list with (hopefully) shorter list with valid entries
                            int oldResultCount = results[statsWithTE[es]].Count;
                            results[statsWithTE[es]] = validResults1;
                            bool resultsRemoved1 = (oldResultCount != results[statsWithTE[es]].Count);
                            if (resultsRemoved1)
                            {
                                filterBoundsAndTEAgain = adjustBoundsToStatResults(statsWithTE[es]);
                            }

                            List<StatResult> validResults2 = new List<StatResult>();
                            foreach (int ev in equalEffs2)
                            {
                                validResults2.Add(results[statsWithTE[et]][ev]);
                            }
                            oldResultCount = results[statsWithTE[et]].Count;
                            results[statsWithTE[et]] = validResults2;
                            bool resultsRemoved2 = (oldResultCount != results[statsWithTE[et]].Count);
                            if (resultsRemoved2)
                            {
                                filterBoundsAndTEAgain = adjustBoundsToStatResults(statsWithTE[et]) || filterBoundsAndTEAgain;
                            }

                            if ((resultsRemoved1 || resultsRemoved2) && et > 1)
                            {
                                // if  results were removed after the comparison of the first two statsWithTE, start over.
                                // This case doesn't happen for now, because only food and melee are dependant on TE, i.e. statsWithTE.Count <= 2.
                                es = -1;
                                et = statsWithTE.Count;
                            }
                        }
                    }
                }
            } while (filterBoundsAndTEAgain);
            return validResults;
        }

        /// <summary>
        /// Adjusts wild and dom levelsUndetermined. Don't call this function more than once for a stat that has only one result!
        /// </summary>
        /// <param name="statIndex">stat index whose bounds will be adjusted</param>
        private bool adjustBoundsToStatResults(int statIndex)
        {
            bool boundsWhereChanged = false;
            if (results[statIndex].Count == 1)
            {
                // if stat is unknown, ignore in bounds (speed, sometimes oxygen is unknown (==-1))
                if (results[statIndex][0].levelWild > 0)
                    levelsUndeterminedWild -= results[statIndex][0].levelWild;
                if (results[statIndex][0].levelDom > 0)
                    levelsUndeterminedDom -= results[statIndex][0].levelDom;
                // bounds only contain the bounds of not unique stats
                lowerBoundWilds[statIndex] = 0;
                lowerBoundDoms[statIndex] = 0;
                upperBoundDoms[statIndex] = 0;
                boundsWhereChanged = true;
            }
            else if (results[statIndex].Count > 1)
            {
                // get the smallest and largest value
                int minW = results[statIndex][0].levelWild, minD = results[statIndex][0].levelDom, maxD = results[statIndex][0].levelDom;
                for (int r = 1; r < results[statIndex].Count; r++)
                {
                    if (results[statIndex][r].levelWild < minW) { minW = results[statIndex][r].levelWild; }
                    if (results[statIndex][r].levelDom < minD) { minD = results[statIndex][r].levelDom; }
                    if (results[statIndex][r].levelDom > maxD) { maxD = results[statIndex][r].levelDom; }
                }

                boundsWhereChanged = lowerBoundWilds[statIndex] != minW
                                  || lowerBoundDoms[statIndex] != minD
                                  || upperBoundDoms[statIndex] != maxD;

                // save min/max-possible value
                lowerBoundWilds[statIndex] = minW;
                lowerBoundDoms[statIndex] = minD;
                upperBoundDoms[statIndex] = maxD;
            }
            return boundsWhereChanged;
        }

        /// <summary>
        /// Marks the results as invalid that violate the given bounds assuming the fixedResults are true
        /// </summary>
        public int filterResultsByFixed(int dontFix = -1)
        {
            int[] lowBoundWs = (int[])lowerBoundWilds.Clone();
            int[] lowBoundDs = (int[])lowerBoundDoms.Clone();
            int[] uppBoundDs = (int[])upperBoundDoms.Clone();
            int wildMax = levelsUndeterminedWild, dom = levelsUndeterminedDom;

            // set all results to non-valid that are in a fixed stat and not the chosen one
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                for (int r = 0; r < results[s].Count; r++)
                {
                    results[s][r].currentlyNotValid = (fixedResults[s] && dontFix != s && r != chosenResults[s]);
                }
                // subtract fixed stat-levels, but not from the current stat
                if (fixedResults[s] && dontFix != s)
                {
                    wildMax -= results[s][chosenResults[s]].levelWild;
                    dom -= results[s][chosenResults[s]].levelDom;
                    lowBoundWs[s] = 0;
                    lowBoundDs[s] = 0;
                    uppBoundDs[s] = 0;
                }
            }

            // mark all results as invalid that are not possible with the current fixed chosen results
            // loop as many times as necessary to remove results that depends on the invalidation of results in a later stat
            bool loopAgain = true;
            while (loopAgain)
            {
                loopAgain = false;
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (s == (int)StatNames.Torpidity) continue;
                    int validResultsNr = 0;
                    int uniqueR = -1;
                    for (int r = 0; r < results[s].Count; r++)
                    {
                        if (!results[s][r].currentlyNotValid)
                            validResultsNr++;
                    }
                    if (validResultsNr > 1)
                    {
                        for (int r = 0; r < results[s].Count; r++)
                        {
                            if (!results[s][r].currentlyNotValid && (results[s][r].levelWild > wildMax - lowBoundWs.Sum() + lowBoundWs[s] || results[s][r].levelDom > dom - lowBoundDs.Sum() + lowBoundDs[s] || results[s][r].levelDom < dom - uppBoundDs.Sum() + uppBoundDs[s]))
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
                                    dom -= results[s][uniqueR].levelDom;
                                    lowBoundWs[s] = 0;
                                    lowBoundDs[s] = 0;
                                    uppBoundDs[s] = 0;
                                    if (wildMax < 0 || dom < 0)
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

        public bool EveryStatHasAtLeastOneResult
        {
            get
            {
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (results[s].Count == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public double uniqueTE()
        {
            double eff = -2;
            if (statsWithTE.Count > 0 && results[statsWithTE[0]].Count > chosenResults[statsWithTE[0]])
            {
                for (int s = 0; s < statsWithTE.Count; s++)
                {
                    for (int ss = s + 1; ss < statsWithTE.Count; ss++)
                    {
                        // effectiveness-calculation can be a bit off due to ingame-rounding
                        if (results[statsWithTE[ss]].Count <= chosenResults[statsWithTE[ss]]
                            || !MinMaxDouble.Overlaps(results[statsWithTE[s]][chosenResults[statsWithTE[s]]].TE,
                                                      results[statsWithTE[ss]][chosenResults[statsWithTE[ss]]].TE))
                        {
                            lastTEUnique = false;
                            return -1; // no unique TE
                        }
                    }
                }
                // calculate most probable real TE
                // get intersection of all TE-ranges

                MinMaxDouble te = results[statsWithTE[0]][chosenResults[statsWithTE[0]]].TE.Clone();
                for (int s = 1; s < statsWithTE.Count; s++)
                {
                    // the overlap is ensured at this point
                    te.SetToIntersectionWith(results[statsWithTE[s]][chosenResults[statsWithTE[s]]].TE);
                }

                eff = te.Mean;
            }
            lastTEUnique = eff >= 0;
            return eff;
        }

        public double imprintingBonus => imprintingBonusRange.Mean;
    }
}

using ARKBreedingStats.Library;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats
{
    public class Extraction
    {
        public readonly List<StatResult>[] Results = new List<StatResult>[Values.STATS_COUNT]; // stores the possible results of all stats as array (wildlevel, domlevel, tamingEff)
        public readonly int[] ChosenResults;
        public readonly bool[] FixedResults;
        public readonly List<int> StatsWithTE;
        /// <summary>
        /// The selected results are valid regarding their level sums.
        /// </summary>
        public bool ValidResults;
        /// <summary>
        /// All stats have only 1 possible combination.
        /// </summary>
        public bool UniqueResults;
        public bool PostTamed;
        private bool _bred;
        // lower/upper possible Bound of each stat (wild has no upper bound as wild-speed and sometimes oxygen is unknown,
        // and could be up to levelWildSum, so no results could be filtered out)
        private readonly int[] _lowerBoundWilds;
        private readonly int[] _lowerBoundDoms;
        private readonly int[] _upperBoundDoms;
        private int _levelsUndeterminedWild;
        private int _levelsUndeterminedDom;
        public int LevelWildSum;
        public int LevelDomSum;
        private MinMaxDouble _imprintingBonusRange;

        public Extraction()
        {
            FixedResults = new bool[Values.STATS_COUNT];
            ChosenResults = new int[Values.STATS_COUNT];
            StatsWithTE = new List<int>();
            _lowerBoundWilds = new int[Values.STATS_COUNT];
            _lowerBoundDoms = new int[Values.STATS_COUNT];
            _upperBoundDoms = new int[Values.STATS_COUNT];

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                Results[s] = new List<StatResult>();
            }
        }

        public void Clear()
        {
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                ChosenResults[s] = 0;
                FixedResults[s] = false;
                Results[s].Clear();
                _lowerBoundWilds[s] = 0;
                _lowerBoundDoms[s] = 0;
                _upperBoundDoms[s] = 0;
            }
            ValidResults = false;
            UniqueResults = false;
            StatsWithTE.Clear();
            _imprintingBonusRange = new MinMaxDouble(0);
            LevelWildSum = 0;
            LevelDomSum = 0;
        }

        /// <summary>
        /// Extracts possible level combinations for the given values.
        /// </summary>
        /// <param name="species"></param>
        /// <param name="level">Total level of the creature.</param>
        /// <param name="statIOs">Controls that display the stats</param>
        /// <param name="lowerTEBound">Lowest possible taming effectiveness</param>
        /// <param name="upperTEBound">Highest possible taming effectiveness</param>
        /// <param name="tamed"></param>
        /// <param name="bred"></param>
        /// <param name="imprintingBonusRounded"></param>
        /// <param name="adjustImprinting"></param>
        /// <param name="allowMoreThanHundredImprinting"></param>
        /// <param name="imprintingBonusMultiplier"></param>
        /// <param name="cuddleIntervalMultiplier"></param>
        /// <param name="considerWildLevelSteps"></param>
        /// <param name="wildLevelSteps"></param>
        /// <param name="highPrecisionInputs">If true, the input is expected to be a float value from an export file.
        /// If false, it's assumed to be a displayed value from the game with one decimal digit.</param>
        /// <param name="imprintingChanged"></param>
        public void ExtractLevels(Species species, int level, List<StatIO> statIOs, double lowerTEBound, double upperTEBound,
            bool tamed, bool bred, double imprintingBonusRounded, bool adjustImprinting, bool allowMoreThanHundredImprinting, double imprintingBonusMultiplier, double cuddleIntervalMultiplier,
            bool considerWildLevelSteps, int wildLevelSteps, bool highPrecisionInputs, out bool imprintingChanged)
        {
            List<CreatureStat> stats = species.stats;
            ValidResults = true;
            imprintingChanged = false;
            considerWildLevelSteps = considerWildLevelSteps
                && !bred
                && species.name.Substring(0, 3) != "Tek"
                && species.name != "Jerboa"
                ;

            this._bred = bred;
            PostTamed = bred || tamed;

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
                _imprintingBonusRange = imprintingBonusList[IBi];
                _imprintingBonusRange.SetToIntersectionWith(0, (allowMoreThanHundredImprinting ? 5 : 1)); // it's assumed that a valid IB will not be larger than 500%


                var imprintingMultiplierRanges = new MinMaxDouble[Values.STATS_COUNT];
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    double statImprintingMultiplier = species.StatImprintMultipliers[s];
                    imprintingMultiplierRanges[s] = statImprintingMultiplier != 0
                        ? new MinMaxDouble(1 + _imprintingBonusRange.Min * imprintingBonusMultiplier * statImprintingMultiplier,
                                           1 + _imprintingBonusRange.Max * imprintingBonusMultiplier * statImprintingMultiplier)
                        : new MinMaxDouble(1);
                }

                var levelWildSumRange = new MinMaxInt((int)Math.Round((statIOs[(int)StatNames.Torpidity].Input / imprintingMultiplierRanges[(int)StatNames.Torpidity].Max - (PostTamed ? stats[(int)StatNames.Torpidity].AddWhenTamed : 0) - stats[(int)StatNames.Torpidity].BaseValue) / (stats[(int)StatNames.Torpidity].BaseValue * stats[(int)StatNames.Torpidity].IncPerWildLevel)),
                                                      (int)Math.Round((statIOs[(int)StatNames.Torpidity].Input / imprintingMultiplierRanges[(int)StatNames.Torpidity].Min - (PostTamed ? stats[(int)StatNames.Torpidity].AddWhenTamed : 0) - stats[(int)StatNames.Torpidity].BaseValue) / (stats[(int)StatNames.Torpidity].BaseValue * stats[(int)StatNames.Torpidity].IncPerWildLevel)));
                var levelDomSumRange = new MinMaxInt(Math.Max(0, level - 1 - levelWildSumRange.Max),
                                                     Math.Max(0, level - 1 - levelWildSumRange.Min));

                LevelWildSum = levelWildSumRange.Min;
                LevelDomSum = levelDomSumRange.Min; // TODO implement range-mechanic

                _levelsUndeterminedWild = LevelWildSum;
                _levelsUndeterminedDom = LevelDomSum;

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
                    if (lowerTEBound < 0) lowerTEBound = 0;
                    upperTEBound += 0.0006;
                }

                // check all possible level-combinations
                for (int s = 0; s < Values.STATS_COUNT; s++)
                {
                    if (!species.UsesStat(s))
                    {
                        Results[s].Add(new StatResult(0, 0));
                        continue;
                    }
                    if (statIOs[s].Input <= 0) // if stat is unknown (e.g. oxygen sometimes is not shown)
                    {
                        Results[s].Add(new StatResult(-1, 0));
                        continue;
                    }

                    statIOs[s].postTame = PostTamed;

                    // determine the precision of the input value
                    float toleranceForThisStat = StatValueCalculation.DisplayedAberration(statIOs[s].Input, Utils.Precision(s), highPrecisionInputs);
                    //Console.WriteLine($"Precision stat {s}: {toleranceForThisStat}");

                    MinMaxDouble inputValue = new MinMaxDouble(statIOs[s].Input - toleranceForThisStat, statIOs[s].Input + toleranceForThisStat);
                    double statBaseValue = stats[s].BaseValue;
                    if (PostTamed && s == (int)StatNames.Health) statBaseValue *= (double)species.TamedBaseHealthMultiplier;// + 0.00000000001; // todo double-precision handling

                    bool withTEff = (PostTamed && stats[s].MultAffinity > 0);
                    if (withTEff) { StatsWithTE.Add(s); }

                    int minLW = 0;
                    int maxLW;
                    if (stats[s].IncPerWildLevel > 0)
                    {
                        double multAffinityFactor = stats[s].MultAffinity;
                        if (PostTamed)
                        {
                            // the multiplicative bonus is only multiplied with the TE if it is positive (i.e. negative boni won't get less bad if the TE is low)
                            if (multAffinityFactor > 0)
                                multAffinityFactor *= lowerTEBound;
                            multAffinityFactor += 1;
                        }
                        else
                            multAffinityFactor = 1;
                        maxLW = (int)Math.Round(((inputValue.Max / multAffinityFactor - (PostTamed ? stats[s].AddWhenTamed : 0)) / statBaseValue - 1) / stats[s].IncPerWildLevel); // floor is too unprecise
                    }
                    else
                    {
                        minLW = -1;
                        maxLW = -1;
                    }
                    if (maxLW > LevelWildSum) { maxLW = LevelWildSum; }

                    double maxLD = 0;
                    if (!statIOs[s].DomLevelLockedZero && PostTamed && species.DisplaysStat(s) && stats[s].IncPerTamedLevel > 0)
                    {
                        int ww = 0; // base wild level for the tamed creature needed to be alive
                        if (statBaseValue + stats[s].AddWhenTamed < 0)
                        {
                            // e.g. Griffin
                            // get lowest wild level at which the creature is alive
                            while (StatValueCalculation.CalculateValue(species, s, ww, 0, true, lowerTEBound, 0, false) <= 0)
                            {
                                ww++;
                            }
                        }
                        maxLD = Math.Round((inputValue.Max / ((statBaseValue * (1 + stats[s].IncPerWildLevel * ww) + stats[s].AddWhenTamed) * (1 + lowerTEBound * stats[s].MultAffinity)) - 1) / stats[s].IncPerTamedLevel); //floor is sometimes too low
                    }
                    if (maxLD > _levelsUndeterminedDom) maxLD = _levelsUndeterminedDom;
                    if (maxLD < 0) maxLD = 0;

                    MinMaxDouble statImprintingMultiplierRange = new MinMaxDouble(1);
                    // only use imprintingMultiplier for stats that use them. Stamina and Oxygen don't use ist. Sometimes speed neither.
                    if (bred && species.StatImprintMultipliers[s] != 0)
                        statImprintingMultiplierRange = imprintingMultiplierRanges[s].Clone();

                    // if dom levels have no effect, just calculate the wild level
                    // for flyers (without mods) this means for speed no wild levels at all (i.e. not unknown, but 0)
                    // for the Diplodocus this means 0 wild levels in melee
                    if (stats[s].IncPerTamedLevel == 0)
                    {
                        if (stats[s].IncPerWildLevel == 0)
                        {
                            // check if the input value is valid
                            MinMaxDouble possibleStatValues = new MinMaxDouble(StatValueCalculation.CalculateValue(species, s, 0, 0, PostTamed, lowerTEBound, _imprintingBonusRange.Min, false),
                                StatValueCalculation.CalculateValue(species, s, 0, 0, PostTamed, upperTEBound, _imprintingBonusRange.Max, false));
                            if (inputValue.Overlaps(possibleStatValues))
                                Results[s].Add(new StatResult(0, 0, inputValue.Mean));
                        }
                        else
                        {
                            MinMaxDouble lwRange = new MinMaxDouble(((inputValue.Min / (PostTamed ? 1 + stats[s].MultAffinity : 1) - (PostTamed ? stats[s].AddWhenTamed : 0)) / (statBaseValue * statImprintingMultiplierRange.Max) - 1) / stats[s].IncPerWildLevel,
                                                                    ((inputValue.Max / (PostTamed ? 1 + stats[s].MultAffinity : 1) - (PostTamed ? stats[s].AddWhenTamed : 0)) / (statBaseValue * statImprintingMultiplierRange.Min) - 1) / stats[s].IncPerWildLevel);
                            int lw = (int)Math.Round(lwRange.Mean);
                            if (lwRange.Includes(lw) && lw >= 0 && lw <= maxLW)
                            {
                                Results[s].Add(new StatResult(lw, 0, inputValue.Mean));
                            }
                        }
                        // even if no result was found, there is no other valid
                        continue;
                    }

                    for (int lw = minLW; lw < maxLW + 1; lw++)
                    {
                        // imprinting bonus is applied to all stats except stamina (s==1) and oxygen (s==2) and speed (s==6)
                        MinMaxDouble valueWODomRange = new MinMaxDouble(statBaseValue * (1 + stats[s].IncPerWildLevel * lw) * statImprintingMultiplierRange.Min + (PostTamed ? stats[s].AddWhenTamed : 0),
                                                                        statBaseValue * (1 + stats[s].IncPerWildLevel * lw) * statImprintingMultiplierRange.Max + (PostTamed ? stats[s].AddWhenTamed : 0)); // value without domesticated levels
                        if (!withTEff)
                        {
                            // calculate the only possible Ld, if it's an integer, take it.
                            if (stats[s].IncPerTamedLevel > 0)
                            {
                                MinMaxDouble ldRange = new MinMaxDouble((inputValue.Min / (valueWODomRange.Max * (PostTamed ? 1 + stats[s].MultAffinity : 1)) - 1) / stats[s].IncPerTamedLevel,
                                                                        (inputValue.Max / (valueWODomRange.Min * (PostTamed ? 1 + stats[s].MultAffinity : 1)) - 1) / stats[s].IncPerTamedLevel);
                                int ld = (int)Math.Round(ldRange.Mean);
                                if (ldRange.Includes(ld) && ld >= 0 && ld <= maxLD)
                                {
                                    Results[s].Add(new StatResult(lw, ld, inputValue.Mean));
                                }
                            }
                            else
                            {
                                Results[s].Add(new StatResult(lw, 0, inputValue.Mean));
                            }
                        }
                        else
                        {
                            for (int ld = 0; ld <= maxLD; ld++)
                            {
                                // taming bonus is dependent on taming-effectiveness
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
                                    int levelPostTame = LevelWildSum + 1;
                                    MinMaxInt levelPreTameRange = new MinMaxInt(Creature.CalculatePreTameWildLevel(levelPostTame, tamingEffectiveness.Max),
                                                                           Creature.CalculatePreTameWildLevel(levelPostTame, tamingEffectiveness.Min));

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

                                    // if another stat already is dependent on TE, check if this TE overlaps any of their TE-ranges. If not, TE is not possible (a creature can only have the same TE for all TE-dependent stats)
                                    if (StatsWithTE.Count > 1)
                                    {
                                        bool teExistent = false;
                                        for (int er = 0; er < Results[StatsWithTE[0]].Count; er++)
                                        {
                                            if (tamingEffectiveness.Overlaps(Results[StatsWithTE[0]][er].TE))
                                            {
                                                teExistent = true;
                                                break;
                                            }
                                        }
                                        if (!teExistent) continue;
                                    }
                                }

                                Results[s].Add(new StatResult(lw, ld, inputValue.Mean, tamingEffectiveness));
                            }
                        }
                    }
                }
                if (bred)
                {
                    // if each stat has at least one result, assume the extraction was valid with the chosen IB
                    if (EveryStatHasAtLeastOneResult)
                    {
                        // all stats have a result, don't test the other possible IBs
                        imprintingChanged = (Math.Abs(imprintingBonusRounded - ImprintingBonus) > 0.01);
                        break;
                    }
                    else if (IBi < imprintingBonusList.Count - 1)
                    {
                        // not all stats got a result, clear results for the next round
                        Clear();
                        ValidResults = true;
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
                double imprintingGainPerCuddle = Utils.ImprintingGainPerCuddle(species.breeding.maturationTimeAdjusted, cuddleIntervalMultiplier);
                imprintingBonusFromGainPerCuddle = Math.Round(imprintingBonusRounded / imprintingGainPerCuddle) * imprintingGainPerCuddle;
            }

            double[] statImprintMultipliers = species.StatImprintMultipliers;

            MinMaxInt wildLevelsFromImprintedTorpor = new MinMaxInt(
                (int)Math.Round(((((torpor / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity)) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / ((1 + (imprintingBonusRounded + 0.005) * statImprintMultipliers[(int)StatNames.Torpidity] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Torpidity].BaseValue)) - 1) / species.stats[(int)StatNames.Torpidity].IncPerWildLevel),
                (int)Math.Round(((((torpor / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity)) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / ((1 + (imprintingBonusRounded - 0.005) * statImprintMultipliers[(int)StatNames.Torpidity] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Torpidity].BaseValue)) - 1) / species.stats[(int)StatNames.Torpidity].IncPerWildLevel));

            // assuming food has no dom-levels, extract the exact imprinting from this stat. If the range is in the range of the torpor-dependent IB, take this more precise value for the imprinting. (food has higher values and yields more precise results)
            MinMaxInt wildLevelsFromImprintedFood = new MinMaxInt(
                (int)Math.Round(((((food / (1 + species.stats[(int)StatNames.Food].MultAffinity)) - species.stats[(int)StatNames.Food].AddWhenTamed) / ((1 + (imprintingBonusRounded + 0.005) * statImprintMultipliers[(int)StatNames.Food] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Food].BaseValue)) - 1) / species.stats[(int)StatNames.Food].IncPerWildLevel),
                (int)Math.Round(((((food / (1 + species.stats[(int)StatNames.Food].MultAffinity)) - species.stats[(int)StatNames.Food].AddWhenTamed) / ((1 + (imprintingBonusRounded - 0.005) * statImprintMultipliers[(int)StatNames.Food] * imprintingBonusMultiplier) * species.stats[(int)StatNames.Food].BaseValue)) - 1) / species.stats[(int)StatNames.Food].IncPerWildLevel));

            List<int> otherStatsSupportIB = new List<int>(); // the number of other stats that support this IB-range
                                                             // for high-level creatures the bonus from imprinting is so high, that a displayed and rounded value of the imprinting bonus can be possible with multiple torpor-levels, i.e. 1 %point IB results in a larger change than a level in torpor.
            for (int torporLevel = wildLevelsFromImprintedTorpor.Min; torporLevel <= wildLevelsFromImprintedTorpor.Max; torporLevel++)
            {
                int support = 0;
                double imprintingProductTorpor = statImprintMultipliers[(int)StatNames.Torpidity] * imprintingBonusMultiplier;
                double imprintingProductFood = statImprintMultipliers[(int)StatNames.Food] * imprintingBonusMultiplier;
                if (imprintingProductTorpor == 0 || imprintingProductFood == 0) break;
                MinMaxDouble imprintingBonusRange = new MinMaxDouble(
                    (((torpor - 0.05) / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / StatValueCalculation.CalculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, false, 0, 0) - 1) / imprintingProductTorpor,
                    (((torpor + 0.05) / (1 + species.stats[(int)StatNames.Torpidity].MultAffinity) - species.stats[(int)StatNames.Torpidity].AddWhenTamed) / StatValueCalculation.CalculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, false, 0, 0) - 1) / imprintingProductTorpor);

                // check for each possible food-level the IB-range and if it can narrow down the range derived from the torpor (deriving from food is more precise, due to the higher values)
                for (int foodLevel = wildLevelsFromImprintedFood.Min; foodLevel <= wildLevelsFromImprintedFood.Max; foodLevel++)
                {
                    MinMaxDouble imprintingBonusFromFood = new MinMaxDouble(
                        (((food - 0.05) / (1 + species.stats[(int)StatNames.Food].MultAffinity) - species.stats[(int)StatNames.Food].AddWhenTamed) / StatValueCalculation.CalculateValue(species, (int)StatNames.Food, foodLevel, 0, false, 0, 0) - 1) / imprintingProductFood,
                        (((food + 0.05) / (1 + species.stats[(int)StatNames.Food].MultAffinity) - species.stats[(int)StatNames.Food].AddWhenTamed) / StatValueCalculation.CalculateValue(species, (int)StatNames.Food, foodLevel, 0, false, 0, 0) - 1) / imprintingProductFood);


                    // NOTE: it's assumed if the IB-food is in the range of IB-torpor, the values are correct. This doesn't have to be true, but is very probable. If extraction-issues appear, this assumption could be the reason.
                    //if (imprintingBonusFromTorpor.Includes(imprintingBonusFromFood)
                    if (imprintingBonusRange.Overlaps(imprintingBonusFromFood))
                    {
                        MinMaxDouble intersectionIB = new MinMaxDouble(imprintingBonusRange);
                        intersectionIB.SetToIntersectionWith(imprintingBonusFromFood);
                        if (StatValueCalculation.CalculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, true, 1, intersectionIB.Min) <= torpor
                            && StatValueCalculation.CalculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, true, 1, intersectionIB.Max) >= torpor)
                        {
                            //imprintingBonusFromTorpor = imprintingBonusFromFood;
                            imprintingBonusRange.SetToIntersectionWith(imprintingBonusFromFood);
                            support++;
                        }
                    }
                }

                // if classic method results in value in the possible range, take this, probably most exact value
                if (imprintingBonusRange.Includes(imprintingBonusFromGainPerCuddle)
                    && StatValueCalculation.CalculateValue(species, (int)StatNames.Torpidity, torporLevel, 0, true, 1, imprintingBonusFromGainPerCuddle) == torpor)
                {
                    imprintingBonusRange.MinMax = imprintingBonusFromGainPerCuddle;
                    support++;
                }

                // TODO check if this range has already been added to avoid double loops in the extraction. if existent, update support
                imprintingBonusList.Add(imprintingBonusRange);
                otherStatsSupportIB.Add(support);
            }

            // sort IB according to the support they got by other stats, then return the distinct means of the possible ranges.
            return imprintingBonusList.OrderByDescending(i => otherStatsSupportIB[imprintingBonusList.IndexOf(i)]).ToList();
        }

        public void RemoveImpossibleTEsAccordingToMaxWildLevel(int maxWildLevel)
        {
            if (!_bred
                && maxWildLevel > 0
                && LevelWildSum + 1 > maxWildLevel)
            {
                double minTECheck = 2d * (LevelWildSum + 1 - maxWildLevel) / maxWildLevel;

                // if min TE is equal or greater than 1, that indicates it can't possibly be anything but bred, and there cannot be any results that should be sorted out
                if (minTECheck < 1)
                {
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (Results[s].Count == 0 || Results[s][0].TE.Max < 0)
                            continue;
                        for (int r = 0; r < Results[s].Count; r++)
                        {
                            if (Results[s][r].TE.Max < minTECheck)
                                Results[s].RemoveAt(r--);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets bounds to all stats and removes all results that violate the stat-level-bounds. also filter results by TE.
        /// </summary>
        /// <param name="statError">stat-index when failed. If equal to -1 and filtering failed, it's probably an issue of the total level</param>
        /// <returns>Success</returns>
        public bool SetStatLevelBoundsAndFilter(out int statError)
        {
            statError = -1;

            _levelsUndeterminedWild = LevelWildSum;
            _levelsUndeterminedDom = LevelDomSum;
            // subtract all uniquely solved stat-levels from possible max and min of sum
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                AdjustBoundsToStatResults(s);
            }
            if (_levelsUndeterminedWild < _lowerBoundWilds.Sum() || _levelsUndeterminedDom < _lowerBoundDoms.Sum())
            {
                Clear();
                ValidResults = false;
                return ValidResults;
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
                        for (int r = 0; r < Results[s].Count; r++)
                        {
                            if (Results[s].Count > 1
                                && (Results[s][r].levelWild > _levelsUndeterminedWild - _lowerBoundWilds.Sum() + _lowerBoundWilds[s]
                                 || Results[s][r].levelDom > _levelsUndeterminedDom - _lowerBoundDoms.Sum() + _lowerBoundDoms[s]
                                 || Results[s][r].levelDom < _levelsUndeterminedDom - _upperBoundDoms.Sum() + _upperBoundDoms[s]))
                            {
                                // if the sorted out result could affect the bounds, set the bounds again
                                bool adjustBounds = Results[s][r].levelWild == _lowerBoundWilds[s]
                                                   || Results[s][r].levelDom == _lowerBoundDoms[s]
                                                   || Results[s][r].levelDom == _upperBoundDoms[s];

                                // remove result that violated the restrictions
                                Results[s].RemoveAt(r--);

                                // if result gets unique due to this, check if remaining result doesn't violate for max level
                                if (Results[s].Count == 1 || adjustBounds)
                                {
                                    filterBoundsAgain = AdjustBoundsToStatResults(s) || filterBoundsAgain;

                                    // check if undeterminedLevels are too low
                                    if (_levelsUndeterminedWild < 0 || _levelsUndeterminedDom < 0)
                                    {
                                        Results[s].Clear();
                                        ValidResults = false;
                                        statError = s; // this stat has an issue (no valid results)
                                        return ValidResults;
                                    }
                                }
                            }
                        }
                    }
                } while (filterBoundsAgain);

                // if more than one parameter is affected by tamingEffectiveness filter out all numbers that occur not in all
                // if creature is bred, all TE is 1 anyway, no need to filter then
                if (!_bred && StatsWithTE.Count > 1)
                {
                    for (int es = 0; es < StatsWithTE.Count - 1; es++)
                    {
                        for (int et = es + 1; et < StatsWithTE.Count; et++)
                        {
                            List<int> equalEffs1 = new List<int>();
                            List<int> equalEffs2 = new List<int>();
                            for (int ere = 0; ere < Results[StatsWithTE[es]].Count; ere++)
                            {
                                for (int erf = 0; erf < Results[StatsWithTE[et]].Count; erf++)
                                {
                                    // test if the TE-ranges overlap each other, if yes, add them to whitelist
                                    if (Results[StatsWithTE[es]][ere].TE.Overlaps(Results[StatsWithTE[et]][erf].TE))
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
                                validResults1.Add(Results[StatsWithTE[es]][ev]);
                            }
                            // replace long list with (hopefully) shorter list with valid entries
                            int oldResultCount = Results[StatsWithTE[es]].Count;
                            Results[StatsWithTE[es]] = validResults1;
                            bool resultsRemoved1 = (oldResultCount != Results[StatsWithTE[es]].Count);
                            if (resultsRemoved1)
                            {
                                filterBoundsAndTEAgain = AdjustBoundsToStatResults(StatsWithTE[es]);
                            }

                            List<StatResult> validResults2 = new List<StatResult>();
                            foreach (int ev in equalEffs2)
                            {
                                validResults2.Add(Results[StatsWithTE[et]][ev]);
                            }
                            oldResultCount = Results[StatsWithTE[et]].Count;
                            Results[StatsWithTE[et]] = validResults2;
                            bool resultsRemoved2 = (oldResultCount != Results[StatsWithTE[et]].Count);
                            if (resultsRemoved2)
                            {
                                filterBoundsAndTEAgain = AdjustBoundsToStatResults(StatsWithTE[et]) || filterBoundsAndTEAgain;
                            }

                            if ((resultsRemoved1 || resultsRemoved2) && et > 1)
                            {
                                // if  results were removed after the comparison of the first two statsWithTE, start over.
                                // This case doesn't happen for now, because only food and melee are dependent on TE, i.e. statsWithTE.Count <= 2.
                                es = -1;
                                et = StatsWithTE.Count;
                            }
                        }
                    }
                }
            } while (filterBoundsAndTEAgain);
            return ValidResults;
        }

        /// <summary>
        /// Adjusts wild and dom levelsUndetermined. Don't call this function more than once for a stat that has only one result!
        /// </summary>
        /// <param name="statIndex">stat index whose bounds will be adjusted</param>
        private bool AdjustBoundsToStatResults(int statIndex)
        {
            bool boundsWhereChanged = false;
            if (Results[statIndex].Count == 1)
            {
                // if stat is unknown, ignore in bounds (speed, sometimes oxygen is unknown (==-1))
                if (Results[statIndex][0].levelWild > 0)
                    _levelsUndeterminedWild -= Results[statIndex][0].levelWild;
                if (Results[statIndex][0].levelDom > 0)
                    _levelsUndeterminedDom -= Results[statIndex][0].levelDom;
                // bounds only contain the bounds of not unique stats
                _lowerBoundWilds[statIndex] = 0;
                _lowerBoundDoms[statIndex] = 0;
                _upperBoundDoms[statIndex] = 0;
                boundsWhereChanged = true;
            }
            else if (Results[statIndex].Count > 1)
            {
                // get the smallest and largest value
                int minW = Results[statIndex][0].levelWild, minD = Results[statIndex][0].levelDom, maxD = Results[statIndex][0].levelDom;
                for (int r = 1; r < Results[statIndex].Count; r++)
                {
                    if (Results[statIndex][r].levelWild < minW) { minW = Results[statIndex][r].levelWild; }
                    if (Results[statIndex][r].levelDom < minD) { minD = Results[statIndex][r].levelDom; }
                    if (Results[statIndex][r].levelDom > maxD) { maxD = Results[statIndex][r].levelDom; }
                }

                boundsWhereChanged = _lowerBoundWilds[statIndex] != minW
                                  || _lowerBoundDoms[statIndex] != minD
                                  || _upperBoundDoms[statIndex] != maxD;

                // save min/max-possible value
                _lowerBoundWilds[statIndex] = minW;
                _lowerBoundDoms[statIndex] = minD;
                _upperBoundDoms[statIndex] = maxD;
            }
            return boundsWhereChanged;
        }

        /// <summary>
        /// Marks the results as invalid that violate the given bounds assuming the fixedResults are true
        /// </summary>
        public int FilterResultsByFixed(int dontFix = -1)
        {
            int[] lowBoundWs = (int[])_lowerBoundWilds.Clone();
            int[] lowBoundDs = (int[])_lowerBoundDoms.Clone();
            int[] uppBoundDs = (int[])_upperBoundDoms.Clone();
            int wildMax = _levelsUndeterminedWild, dom = _levelsUndeterminedDom;

            // set all results to non-valid that are in a fixed stat and not the chosen one
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (s == (int)StatNames.Torpidity) continue;
                for (int r = 0; r < Results[s].Count; r++)
                {
                    Results[s][r].currentlyNotValid = (FixedResults[s] && dontFix != s && r != ChosenResults[s]);
                }
                // subtract fixed stat-levels, but not from the current stat
                if (FixedResults[s] && dontFix != s)
                {
                    wildMax -= Results[s][ChosenResults[s]].levelWild;
                    dom -= Results[s][ChosenResults[s]].levelDom;
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
                    for (int r = 0; r < Results[s].Count; r++)
                    {
                        if (!Results[s][r].currentlyNotValid)
                            validResultsNr++;
                    }
                    if (validResultsNr > 1)
                    {
                        for (int r = 0; r < Results[s].Count; r++)
                        {
                            if (!Results[s][r].currentlyNotValid && (Results[s][r].levelWild > wildMax - lowBoundWs.Sum() + lowBoundWs[s] || Results[s][r].levelDom > dom - lowBoundDs.Sum() + lowBoundDs[s] || Results[s][r].levelDom < dom - uppBoundDs.Sum() + uppBoundDs[s]))
                            {
                                Results[s][r].currentlyNotValid = true;
                                validResultsNr--;
                                // if result gets unique due to this, check if remaining result doesn't violate for max level
                                if (validResultsNr == 1)
                                {
                                    // find unique valid result
                                    for (int rr = 0; rr < Results[s].Count; rr++)
                                    {
                                        if (!Results[s][rr].currentlyNotValid)
                                        {
                                            uniqueR = rr;
                                            break;
                                        }
                                    }
                                    loopAgain = true;
                                    wildMax -= Results[s][uniqueR].levelWild;
                                    dom -= Results[s][uniqueR].levelDom;
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
                    if (Results[s].Count == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public double UniqueTE()
        {
            double eff = -2;
            if (StatsWithTE.Any() && Results[StatsWithTE[0]].Count > ChosenResults[StatsWithTE[0]])
            {
                for (int s = 0; s < StatsWithTE.Count; s++)
                {
                    for (int ss = s + 1; ss < StatsWithTE.Count; ss++)
                    {
                        // effectiveness-calculation can be a bit off due to ingame-rounding
                        if (Results[StatsWithTE[ss]].Count <= ChosenResults[StatsWithTE[ss]]
                            || !MinMaxDouble.Overlaps(Results[StatsWithTE[s]][ChosenResults[StatsWithTE[s]]].TE,
                                                      Results[StatsWithTE[ss]][ChosenResults[StatsWithTE[ss]]].TE))
                        {
                            return -1; // no unique TE
                        }
                    }
                }
                // calculate most probable real TE
                // get intersection of all TE-ranges

                MinMaxDouble te = Results[StatsWithTE[0]][ChosenResults[StatsWithTE[0]]].TE.Clone();
                for (int s = 1; s < StatsWithTE.Count; s++)
                {
                    // the overlap is ensured at this point
                    te.SetToIntersectionWith(Results[StatsWithTE[s]][ChosenResults[StatsWithTE[s]]].TE);
                }

                eff = te.Mean;
            }
            return eff;
        }

        /// <summary>
        /// The mean of the calculated imprinting bonus range.
        /// </summary>
        public double ImprintingBonus => _imprintingBonusRange.Mean;
    }
}

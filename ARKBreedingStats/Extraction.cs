using ARKBreedingStats.Library;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats
{
    public class Extraction
    {
        public readonly List<StatResult>[] Results = new List<StatResult>[Stats.StatsCount]; // stores the possible results of all stats as array (wildlevel, domlevel, tamingEff)
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
        /// <summary>
        /// Sum of all wild and mutated levels combined.
        /// </summary>
        public int LevelWildMutSum;
        public int LevelDomSum;
        private MinMaxDouble _imprintingBonusRange;
        public bool ResultWasSortedOutBecauseOfImpossibleTe { private set; get; }
        /// <summary>
        /// At least one stat has multiple possibilities where at least one has a non-zero mutated level, i.e. the user needs to be able to select it.
        /// </summary>
        public bool MutationLevelPossibilities;

        public Extraction()
        {
            FixedResults = new bool[Stats.StatsCount];
            ChosenResults = new int[Stats.StatsCount];
            StatsWithTE = new List<int>();
            _lowerBoundWilds = new int[Stats.StatsCount];
            _lowerBoundDoms = new int[Stats.StatsCount];
            _upperBoundDoms = new int[Stats.StatsCount];

            for (var s = 0; s < Stats.StatsCount; s++)
            {
                Results[s] = new List<StatResult>();
            }
        }

        public void Clear()
        {
            for (var s = 0; s < Stats.StatsCount; s++)
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
            ResultWasSortedOutBecauseOfImpossibleTe = false;
            StatsWithTE.Clear();
            _imprintingBonusRange = new MinMaxDouble(0);
            ImprintingBonus = 0;
            LevelWildMutSum = 0;
            LevelDomSum = 0;
        }

        /// <summary>
        /// Extracts possible level combinations for the given values.
        /// </summary>
        /// <param name="species"></param>
        /// <param name="level">Total level of the creature.</param>
        /// <param name="statValues"></param>
        /// <param name="domLevelsLockedToZero">True for stat indices where no domestic level is expected</param>
        /// <param name="te">Lowest and highest possible taming effectiveness</param>
        /// <param name="tamed"></param>
        /// <param name="bred"></param>
        /// <param name="imprintingBonusRounded"></param>
        /// <param name="adjustImprinting"></param>
        /// <param name="allowMoreThanHundredImprinting"></param>
        /// <param name="imprintingBonusMultiplier"></param>
        /// <param name="considerWildLevelSteps"></param>
        /// <param name="wildLevelSteps"></param>
        /// <param name="highPrecisionInputs">If true, the input is expected to be a float value from an export file.
        /// If false, it's assumed to be a displayed value from the game with one decimal digit.</param>
        /// <param name="imprintingChanged"></param>
        /// <param name="fixedImprinting">If >= 0 this will be assumed to be the set imprinting.</param>
        public void ExtractLevels(Species species, int level, double[] statValues, bool[] domLevelsLockedToZero, MinMaxDouble te,
            bool tamed, bool bred, double imprintingBonusRounded, bool adjustImprinting, bool allowMoreThanHundredImprinting, double imprintingBonusMultiplier,
            bool considerWildLevelSteps, int wildLevelSteps, bool highPrecisionInputs, bool mutagenApplied, out bool imprintingChanged, Troodonism.AffectedStats troodonismStats,
            double fixedImprinting = -1)
        {
            var stats = troodonismStats == Troodonism.AffectedStats.None
                ? species.stats
                : Troodonism.SelectStats(species.stats, species.altStats, troodonismStats);

            ValidResults = true;
            imprintingChanged = false;
            considerWildLevelSteps = considerWildLevelSteps
                && !bred
                && !mutagenApplied
                && species.name.Substring(0, 3) != "Tek"
                && species.name != "Jerboa"
                ;

            _bred = bred;
            PostTamed = bred || tamed;

            List<MinMaxDouble> imprintingBonusList;
            if (fixedImprinting >= 0)
                imprintingBonusList = new List<MinMaxDouble> { new MinMaxDouble(fixedImprinting) };
            else if (bred)
                imprintingBonusList = adjustImprinting
                    ? CalculateImprintingBonus(species, stats, imprintingBonusRounded, imprintingBonusMultiplier, statValues[Stats.Torpidity], statValues[Stats.Food], troodonismStats)
                    : new List<MinMaxDouble> { new MinMaxDouble(imprintingBonusRounded) };
            else
                imprintingBonusList = new List<MinMaxDouble> { new MinMaxDouble(0) };

            double[] statImprintMultipliers = species.StatImprintMultipliers;

            for (int IBi = 0; IBi < imprintingBonusList.Count; IBi++)
            {
                _imprintingBonusRange = imprintingBonusList[IBi];
                // don't cut off too much possible values, consider a margin of 0.01 to not sort out possible correct values
                _imprintingBonusRange.SetToIntersectionWith(-.01, allowMoreThanHundredImprinting ? 5 : 1.01); // it's assumed that a valid IB will not be larger than 500%
                ImprintingBonus = Math.Max(0, Math.Min(allowMoreThanHundredImprinting ? 5 : 1, _imprintingBonusRange.Mean));

                var imprintingMultiplierRanges = new MinMaxDouble[Stats.StatsCount];
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    // multiply possible tbhm factor in this, it's always linked with ib so far
                    var tbhm = PostTamed && s == Stats.Health && species.TamedBaseHealthMultiplier != null
                        ? species.TamedBaseHealthMultiplier.Value
                        : 1;

                    double statImprintingMultiplier = statImprintMultipliers[s];
                    imprintingMultiplierRanges[s] = statImprintingMultiplier != 0
                        ? new MinMaxDouble((1 + _imprintingBonusRange.Min * imprintingBonusMultiplier * statImprintingMultiplier) * tbhm,
                                           (1 + _imprintingBonusRange.Max * imprintingBonusMultiplier * statImprintingMultiplier) * tbhm)
                        : new MinMaxDouble(tbhm);
                }

                var additiveTamingBonusTorpidity = PostTamed ? stats[Stats.Torpidity].AddWhenTamed : 0;
                var levelWildSumRange = new MinMaxInt((int)Math.Round(((statValues[Stats.Torpidity] - additiveTamingBonusTorpidity) / (imprintingMultiplierRanges[Stats.Torpidity].Max * stats[Stats.Torpidity].BaseValue) - 1) / stats[Stats.Torpidity].IncPerWildLevel),
                                                      (int)Math.Round(((statValues[Stats.Torpidity] - additiveTamingBonusTorpidity) / (imprintingMultiplierRanges[Stats.Torpidity].Min * stats[Stats.Torpidity].BaseValue) - 1) / stats[Stats.Torpidity].IncPerWildLevel));
                var levelDomSumRange = new MinMaxInt(Math.Max(0, level - 1 - levelWildSumRange.Max),
                                                     Math.Max(0, level - 1 - levelWildSumRange.Min));

                LevelWildMutSum = levelWildSumRange.Min;
                LevelDomSum = levelDomSumRange.Min; // TODO implement range mechanic

                _levelsUndeterminedWild = LevelWildMutSum;
                _levelsUndeterminedDom = LevelDomSum;

                if (bred)
                {
                    // bred creatures always have 100% TE
                    te = new MinMaxDouble(1);
                }
                else
                {
                    // sometimes it fails due to double-precision errors, e.g.
                    // Pteranodon (Lvl 34, TE: 80%): HP: 415.9 (6, 0); St: 195 (6, 0); Ox: 240 (6, 0); Fo: 2150.4 (6, 0); We: 134.4 (6, 0); Dm: 141.6% (3, 0); Sp: 135% (0, 0); To: 358.1 (33);
                    // will fail the extraction with a lowerTEBound of 0.8, it only extracts with a lowerTEBound of 0.79, then displays 0.8 as result for the TE. Adding these margins make it work as expected.
                    te = new MinMaxDouble(Math.Max(0, te.Min - 0.0006), te.Max + 0.0006);
                }

                // check all possible level-combinations
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (!species.UsesStat(s))
                    {
                        Results[s].Add(new StatResult(0, 0));
                        continue;
                    }

                    if (statValues[s] <= 0) // if stat is unknown (e.g. oxygen sometimes is not shown)
                    {
                        Results[s].Add(new StatResult(-1, 0));
                        continue;
                    }

                    // determine the precision of the input value
                    float toleranceForThisStat =
                        StatValueCalculation.DisplayedAberration(statValues[s], Stats.Precision(s),
                            highPrecisionInputs);
                    //Console.WriteLine($"Precision stat {s}: {toleranceForThisStat}");

                    MinMaxDouble statValue = new MinMaxDouble(statValues[s] - toleranceForThisStat,
                        statValues[s] + toleranceForThisStat);
                    var statBaseValue = stats[s].BaseValue;

                    bool teAffectsStatValue = PostTamed && stats[s].MultAffinity > 0;
                    if (teAffectsStatValue)
                    {
                        StatsWithTE.Add(s);
                    }

                    var minLw = 0;
                    var maxLw = 0;

                    var minLm = 0; // mutation levels
                    var maxLm = 0;

                    var increaseStatAsPercentage = stats[s].IncreaseStatAsPercentage;

                    var increasePerWildLevel = stats[s].IncPerWildLevel *
                                               (increaseStatAsPercentage ? stats[s].BaseValue : 1);
                    var increasePerMutLevel = stats[s].IncPerMutatedLevel *
                                              (increaseStatAsPercentage ? stats[s].BaseValue : 1);
                    var customMutationLevelIncrease =
                        increasePerMutLevel > 0 && increasePerMutLevel != increasePerWildLevel;

                    var additiveTameBonus = PostTamed ? stats[s].AddWhenTamed : 0;
                    var multAffinityFactor = new MinMaxDouble(1);

                    if (species.CanLevelUpWildOrHaveMutations(s))
                    {
                        if (stats[s].IncPerWildLevel > 0)
                        {
                            if (PostTamed)
                            {
                                var multAffinityBaseFactor = stats[s].MultAffinity;
                                // the multiplicative bonus is only multiplied with the TE if it is positive (i.e. negative boni won't get less bad if the TE is low)
                                if (multAffinityBaseFactor > 0)
                                    multAffinityFactor = new MinMaxDouble(multAffinityBaseFactor * te.Min + 1,
                                        multAffinityBaseFactor * te.Max + 1);
                                else multAffinityFactor = new MinMaxDouble(multAffinityBaseFactor + 1);
                            }

                            maxLw = (int)Math.Round(
                                ((statValue.Max / multAffinityFactor.Min - additiveTameBonus) /
                                 imprintingMultiplierRanges[s].Min
                                 - statBaseValue) / increasePerWildLevel); // floor is too imprecise

                            if (customMutationLevelIncrease)
                                maxLm = (int)Math.Round(
                                    ((statValue.Max / multAffinityFactor.Min - additiveTameBonus) /
                                     imprintingMultiplierRanges[s].Min
                                     - statBaseValue) / increasePerMutLevel);
                        }
                        else
                        {
                            minLw = -1;
                            maxLw = -1;
                        }
                    }

                    if (maxLw > LevelWildMutSum)
                    {
                        maxLw = LevelWildMutSum;
                    }

                    var maxLd = 0;
                    if (!domLevelsLockedToZero[s] && PostTamed && species.DisplaysStat(s) &&
                        stats[s].IncPerTamedLevel > 0)
                    {
                        var minWildLevelAlive = 0; // min wild level for the tamed creature needed to be alive
                        if (statBaseValue + stats[s].AddWhenTamed < 0)
                        {
                            // e.g. Griffin
                            // get the lowest wild level at which the creature is alive
                            while (StatValueCalculation.CalculateValue(species, s, minWildLevelAlive, 0, 0, true,
                                       te.Min, 0, false, troodonismStats) <= 0)
                            {
                                minWildLevelAlive++;
                            }
                        }

                        var postTameValue =
                            ((statBaseValue + increasePerWildLevel * minWildLevelAlive) *
                                imprintingMultiplierRanges[s].Min + additiveTameBonus) * multAffinityFactor.Min;
                        if (postTameValue == 0 && increaseStatAsPercentage)
                            maxLd = 0;
                        else
                            // floor is sometimes too low, so using round
                            maxLd = (int)Math.Round((statValue.Max - postTameValue) / (increaseStatAsPercentage
                                ? postTameValue * stats[s].IncPerTamedLevel
                                : stats[s].IncPerTamedLevel));
                    }

                    if (maxLd > _levelsUndeterminedDom) maxLd = _levelsUndeterminedDom;
                    if (maxLd < 0) maxLd = 0;

                    // if dom levels have no effect, just calculate the wild level
                    // for flyers (without enabled flyerSpeedLevelling) this means for speed no wild levels at all (i.e. not unknown, but 0)
                    // for the Diplodocus this means 0 wild levels in melee
                    if (stats[s].IncPerTamedLevel == 0)
                    {
                        if (stats[s].IncPerWildLevel == 0)
                        {
                            // This branch doesn't handle species that has no increase per wild level but for mutation levels. So far a species with a stat like that doesn't exist.
                            // check if the input value is valid
                            MinMaxDouble possibleStatValues = new MinMaxDouble(
                                StatValueCalculation.CalculateValue(species, s, 0, 0, 0, PostTamed, te.Min,
                                    _imprintingBonusRange.Min, false, troodonismStats),
                                StatValueCalculation.CalculateValue(species, s, 0, 0, 0, PostTamed, te.Max,
                                    _imprintingBonusRange.Max, false, troodonismStats));
                            if (statValue.Overlaps(possibleStatValues))
                                Results[s].Add(new StatResult(0, 0));
                        }
                        else
                        {
                            var lwRange = new MinMaxInt(
                                ((statValue.Min / multAffinityFactor.Max - additiveTameBonus) /
                                    imprintingMultiplierRanges[s].Max - statBaseValue) / increasePerWildLevel,
                                ((statValue.Max / multAffinityFactor.Min - additiveTameBonus) /
                                    imprintingMultiplierRanges[s].Min - statBaseValue) / increasePerWildLevel);

                            if (customMutationLevelIncrease)
                            {
                                var lmRange = new MinMaxInt(
                                    ((statValue.Min / multAffinityFactor.Max - additiveTameBonus) /
                                        imprintingMultiplierRanges[s].Max - statBaseValue) / increasePerMutLevel,
                                    ((statValue.Max / multAffinityFactor.Min - additiveTameBonus) /
                                        imprintingMultiplierRanges[s].Min - statBaseValue) / increasePerMutLevel);

                                if (!lmRange.ValidRange) continue;

                                var tmNotZero = stats[s].MultAffinity != 0;

                                for (var lw = lwRange.Min; lw < lwRange.Max; lw++)
                                {
                                    for (var lm = lmRange.Min; lw < lmRange.Max; lw++)
                                    {
                                        var valueBeforeTe = new MinMaxDouble(
                                            (statBaseValue + increasePerWildLevel * lw + increasePerMutLevel * lm) *
                                            imprintingMultiplierRanges[s].Min + additiveTameBonus,
                                            (statBaseValue + increasePerWildLevel * lw + increasePerMutLevel * lm) *
                                            imprintingMultiplierRanges[s].Max + additiveTameBonus);
                                        if (teAffectsStatValue)
                                        {
                                            var neededTe = new MinMaxDouble(
                                                (statValue.Min / valueBeforeTe.Max - 1) / stats[s].MultAffinity,
                                                (statValue.Max / valueBeforeTe.Min - 1) / stats[s].MultAffinity);

                                            if (neededTe.SetToIntersectionWith(te))
                                                Results[s].Add(new StatResult(lw, 0, neededTe, lm));
                                        }
                                        else if (tmNotZero)
                                        {
                                            // tm is < 0
                                            if (statValue.Includes(valueBeforeTe * (1 + stats[s].MultAffinity)))
                                                Results[s].Add(new StatResult(lw, 0, levelMut: lm));
                                        }
                                        else if (statValue.Includes(valueBeforeTe))
                                            Results[s].Add(new StatResult(lw, 0, levelMut: lm));
                                    }
                                }
                            }
                            else
                            {
                                int lw = (int)Math.Round(lwRange.Mean);
                                if (lwRange.Includes(lw) && lw >= 0 && lw <= maxLw)
                                    Results[s].Add(new StatResult(lw, 0));
                            }
                        }

                        // even if no result was found, there is no other valid
                        continue;
                    }

                    bool resultWasSortedOutBecauseOfImpossibleTe = false;
                    var multBonusWoTe = PostTamed ? 1 + stats[s].MultAffinity : 1;
                    for (var lw = minLw; lw <= maxLw; lw++)
                    {
                        for (var lm = minLm; lm <= maxLm; lm++)
                        {
                            // imprinting bonus is applied to all stats except stamina (s==1) and oxygen (s==2) and speed (s==6)
                            var valueWoTeWoDomRange = new MinMaxDouble((statBaseValue + increasePerWildLevel * lw + increasePerMutLevel * lm) * imprintingMultiplierRanges[s].Min + additiveTameBonus,
                                                                            (statBaseValue + increasePerWildLevel * lw + increasePerMutLevel * lm) * imprintingMultiplierRanges[s].Max + additiveTameBonus);
                            if (!teAffectsStatValue)
                            {
                                // calculate the only possible Ld, if it's an integer, take it.
                                var valueWoDomRange = valueWoTeWoDomRange * multBonusWoTe;
                                if (stats[s].IncPerTamedLevel > 0)
                                {
                                    var effectiveIncPerDomLevel = (increaseStatAsPercentage ? valueWoDomRange : new MinMaxDouble(1)) * stats[s].IncPerTamedLevel;

                                    var ldRange = new MinMaxInt(Math.Max(0, (statValue.Min - valueWoDomRange.Max) / effectiveIncPerDomLevel.Max),
                                                                            (statValue.Max - valueWoDomRange.Min) / effectiveIncPerDomLevel.Min);
                                    var ld = (int)Math.Round(ldRange.Mean);
                                    if (ldRange.Includes(ld) && ld >= 0 && ld <= maxLd)
                                        Results[s].Add(new StatResult(lw, ld, levelMut: lm));
                                }
                                else if (statValue.Includes(valueWoDomRange))
                                {
                                    Results[s].Add(new StatResult(lw, 0, levelMut: lm));
                                }
                            }
                            else
                            {
                                for (var ld = 0; ld <= maxLd; ld++)
                                {
                                    // taming bonus is dependent on taming-effectiveness
                                    // get tamingEffectiveness-possibility
                                    // calculate rounding-error thresholds. Here it's assumed that the displayed in game value is maximal 0.5 off of the true in game value

                                    var calculatedTe = increaseStatAsPercentage
                                        ? new MinMaxDouble((statValue.Min / ((1 + ld * stats[s].IncPerTamedLevel) * valueWoTeWoDomRange.Max) - 1) / stats[s].MultAffinity,
                                                           (statValue.Max / ((1 + ld * stats[s].IncPerTamedLevel) * valueWoTeWoDomRange.Min) - 1) / stats[s].MultAffinity)
                                        : new MinMaxDouble(((statValue.Min - ld * stats[s].IncPerTamedLevel) / valueWoTeWoDomRange.Max - 1) / stats[s].MultAffinity,
                                                           ((statValue.Max - ld * stats[s].IncPerTamedLevel) / valueWoTeWoDomRange.Min - 1) / stats[s].MultAffinity);

                                    if (calculatedTe.Min > te.Max)
                                        continue;
                                    if (calculatedTe.Max < te.Min)
                                        break; // if tamingEff < lowerBound: break, in this d-loop it's getting only smaller

                                    // here it's ensured the TE overlaps the bounds, so we can clamp it to the bounds
                                    calculatedTe.SetToIntersectionWith(te);

                                    if (!bred)
                                    {
                                        // check if the total level and the TE is possible by using the TE-level bonus (credits for this check which sorts out more impossible results: https://github.com/VolatilePulse , thanks!)
                                        // if mutagen is applied, a fixed number of wild levels is added to specific stats
                                        var levelPostTame = LevelWildMutSum + 1 - (mutagenApplied ? Ark.MutagenTotalLevelUpsNonBred : 0);
                                        var levelPreTameRange = new MinMaxInt(
                                            Creature.CalculatePreTameWildLevel(levelPostTame, calculatedTe.Max),
                                            Creature.CalculatePreTameWildLevel(levelPostTame, calculatedTe.Min));

                                        var impossibleTe = true;
                                        for (var wildLevel = levelPreTameRange.Min; wildLevel <= levelPreTameRange.Max; wildLevel++)
                                        {
                                            var levelPostTameRange = new MinMaxInt(
                                                (int)Math.Floor(wildLevel * (1 + calculatedTe.Min / 2)),
                                                (int)Math.Floor(wildLevel * (1 + calculatedTe.Max / 2)));
                                            if (levelPostTameRange.Includes(levelPostTame))
                                            {
                                                impossibleTe = false;
                                                break;
                                            }
                                        }

                                        if (impossibleTe)
                                        {
                                            resultWasSortedOutBecauseOfImpossibleTe = true;
                                            continue;
                                        }

                                        // test if TE with torpor-level of tamed-creatures results in a valid wild-level according to the possible levelSteps
                                        if (considerWildLevelSteps)
                                        {
                                            var validWildLevel = false;
                                            for (var wildLevel = levelPreTameRange.Min; wildLevel <= levelPreTameRange.Max; wildLevel++)
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
                                            var teExistent = false;
                                            for (var er = 0; er < Results[StatsWithTE[0]].Count; er++)
                                            {
                                                if (calculatedTe.Overlaps(Results[StatsWithTE[0]][er].Te))
                                                {
                                                    teExistent = true;
                                                    break;
                                                }
                                            }

                                            if (!teExistent) continue;
                                        }
                                    }

                                    Results[s].Add(new StatResult(lw, ld, calculatedTe, lm));
                                }
                            }
                        }
                    }

                    if (resultWasSortedOutBecauseOfImpossibleTe && !Results[s].Any())
                        ResultWasSortedOutBecauseOfImpossibleTe = true;
                }
                if (bred)
                {
                    // if each stat has at least one result, assume the extraction was valid with the chosen IB
                    if (EveryStatHasAtLeastOneResult)
                    {
                        // all stats have a result, don't test the other possible IBs
                        imprintingChanged = (Math.Abs(imprintingBonusRounded - ImprintingBonus) > 0.01);
                        MutationLevelPossibilities = Results.Any(sr => sr.Count > 1 && sr.Any(r => r.LevelMut > 0));
                        break;
                    }
                    if (IBi < imprintingBonusList.Count - 1)
                    {
                        // not all stats got a result, clear results for the next round
                        Clear();
                        ValidResults = true;
                    }
                }
            }
        }

        /// <summary>
        /// Calculate the exact imprinting bonus, based on the rounded integer value the game displays
        /// and using the bonus on the Torpidity stat (this cannot be leveled, so the exact bonus is known).
        /// Due to the high values of the food stat, which is often not leveled, this stat can be used to further improve the precision of the imprinting bonus.
        /// </summary>
        private static List<MinMaxDouble> CalculateImprintingBonus(Species species, SpeciesStat[] stats, double imprintingBonusRounded, double imprintingBonusMultiplier, double torpor, double food, Troodonism.AffectedStats useTroodonismStats)
        {
            if (imprintingBonusMultiplier == 0)
            {
                return new List<MinMaxDouble> { new MinMaxDouble(imprintingBonusRounded) };
            }

            if (stats[Stats.Torpidity].BaseValue == 0 || stats[Stats.Torpidity].IncPerWildLevel == 0)
            {
                // invalid species-data
                return new List<MinMaxDouble> { new MinMaxDouble(imprintingBonusRounded - 0.005, imprintingBonusRounded + 0.005) };
            }

            var statImprintMultipliers = species.StatImprintMultipliers;

            var anyStatAffectedByImprinting = false;
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                if (statImprintMultipliers[si] != 0)
                {
                    anyStatAffectedByImprinting = true;
                    break;
                }
            }

            if (!anyStatAffectedByImprinting)
            {
                return new List<MinMaxDouble> { new MinMaxDouble(imprintingBonusRounded) };
            }


            if (statImprintMultipliers[Stats.Torpidity] == 0)
            {
                // torpidity is not affected by imprinting, the exact value cannot be calculated
                return new List<MinMaxDouble> { new MinMaxDouble(imprintingBonusRounded - 0.005, imprintingBonusRounded + 0.005) };
            }

            var imprintingBonusList = new List<MinMaxDouble>();

            // if the imprinting bonus was only achieved by cuddles etc. without event bonus, it will increase by fixed steps
            // this is the most exact value, but will not work if the imprinting-gain was different (e.g. events, mods (S+Nanny))
            double imprintingBonusFromGainPerCuddle = 0;
            if (species.breeding != null)
            {
                double imprintingGainPerCuddle = Ark.ImprintingGainPerCuddle(species.breeding.maturationTimeAdjusted);
                if (imprintingGainPerCuddle > 0)
                    imprintingBonusFromGainPerCuddle = Math.Round(imprintingBonusRounded / imprintingGainPerCuddle) * imprintingGainPerCuddle;
            }

            var wildLevelsFromImprintedTorpor = new MinMaxInt(
                ((((torpor / (1 + stats[Stats.Torpidity].MultAffinity)) - stats[Stats.Torpidity].AddWhenTamed) / ((1 + (imprintingBonusRounded + 0.005) * statImprintMultipliers[Stats.Torpidity] * imprintingBonusMultiplier) * stats[Stats.Torpidity].BaseValue)) - 1) / stats[Stats.Torpidity].IncPerWildLevel,
                ((((torpor / (1 + stats[Stats.Torpidity].MultAffinity)) - stats[Stats.Torpidity].AddWhenTamed) / ((1 + (imprintingBonusRounded - 0.005) * statImprintMultipliers[Stats.Torpidity] * imprintingBonusMultiplier) * stats[Stats.Torpidity].BaseValue)) - 1) / stats[Stats.Torpidity].IncPerWildLevel);

            // this assumes the food mutation levels have no custom increasePerLevelMultiplier
            // assuming food has no dom-levels, extract the exact imprinting from this stat. If the range is in the range of the torpor-dependent IB, take this more precise value for the imprinting. (food has higher values and yields more precise results)
            var wildLevelsFromImprintedFood = new MinMaxInt(
                ((((food / (1 + stats[Stats.Food].MultAffinity)) - stats[Stats.Food].AddWhenTamed) / ((1 + (imprintingBonusRounded + 0.005) * statImprintMultipliers[Stats.Food] * imprintingBonusMultiplier) * stats[Stats.Food].BaseValue)) - 1) / stats[Stats.Food].IncPerWildLevel,
                ((((food / (1 + stats[Stats.Food].MultAffinity)) - stats[Stats.Food].AddWhenTamed) / ((1 + (imprintingBonusRounded - 0.005) * statImprintMultipliers[Stats.Food] * imprintingBonusMultiplier) * stats[Stats.Food].BaseValue)) - 1) / stats[Stats.Food].IncPerWildLevel);

            var otherStatsSupportIB = new List<int>(); // the number of other stats that support this IB-range
                                                       // for high-level creatures the bonus from imprinting is so high, that a displayed and rounded value of the imprinting bonus can be possible with multiple torpor-levels, i.e. 1 %point IB results in a larger change than a level in torpor.

            var imprintingBonusTorpidityFinal = statImprintMultipliers[Stats.Torpidity] * imprintingBonusMultiplier;
            var imprintingBonusFoodFinal = statImprintMultipliers[Stats.Food] * imprintingBonusMultiplier;

            for (var torporLevel = wildLevelsFromImprintedTorpor.Min; torporLevel <= wildLevelsFromImprintedTorpor.Max; torporLevel++)
            {
                var support = 0;
                var imprintingBonusRange = new MinMaxDouble(
                    (((torpor - 0.05) / (1 + stats[Stats.Torpidity].MultAffinity) - stats[Stats.Torpidity].AddWhenTamed) / StatValueCalculation.CalculateValue(species, Stats.Torpidity, torporLevel, 0, 0, false, 0, 0, useTroodonismStats: useTroodonismStats) - 1) / imprintingBonusTorpidityFinal,
                    (((torpor + 0.05) / (1 + stats[Stats.Torpidity].MultAffinity) - stats[Stats.Torpidity].AddWhenTamed) / StatValueCalculation.CalculateValue(species, Stats.Torpidity, torporLevel, 0, 0, false, 0, 0, useTroodonismStats: useTroodonismStats) - 1) / imprintingBonusTorpidityFinal);

                // check for each possible food-level the IB-range and if it can narrow down the range derived from the torpor (deriving from food is more precise, due to the higher values)

                if (imprintingBonusFoodFinal > 0)
                {
                    for (var foodLevel = wildLevelsFromImprintedFood.Min;
                         foodLevel <= wildLevelsFromImprintedFood.Max;
                         foodLevel++)
                    {
                        var imprintingBonusFromFood = new MinMaxDouble(
                            (((food - 0.05) / (1 + stats[Stats.Food].MultAffinity) -
                              stats[Stats.Food].AddWhenTamed) /
                                StatValueCalculation.CalculateValue(species, Stats.Food, foodLevel, 0, 0, false, 0, 0, useTroodonismStats: useTroodonismStats) -
                                1) /
                            imprintingBonusFoodFinal,
                            (((food + 0.05) / (1 + stats[Stats.Food].MultAffinity) -
                              stats[Stats.Food].AddWhenTamed) /
                                StatValueCalculation.CalculateValue(species, Stats.Food, foodLevel, 0, 0, false, 0, 0, useTroodonismStats: useTroodonismStats) -
                                1) /
                            imprintingBonusFoodFinal);

                        // NOTE: it's assumed if the IB-food is in the range of IB-torpor, the values are correct.
                        // This doesn't have to be true, but is very probable.
                        // It can be wrong if food was leveled, but happens to result in a value that is also possible with a different amount of wild levels without dom levels and the resulting possible range for the exact IB is in the range of the IB from the torpor
                        // If extraction-issues appear, this assumption could be the reason.

                        //if (imprintingBonusFromTorpor.Includes(imprintingBonusFromFood)
                        if (imprintingBonusRange.Overlaps(imprintingBonusFromFood))
                        {
                            MinMaxDouble intersectionIB = new MinMaxDouble(imprintingBonusRange);
                            intersectionIB.SetToIntersectionWith(imprintingBonusFromFood);
                            if (StatValueCalculation.CalculateValue(species, Stats.Torpidity, torporLevel, 0, 0, true, 1,
                                    intersectionIB.Min, useTroodonismStats: useTroodonismStats) <= torpor
                                && StatValueCalculation.CalculateValue(species, Stats.Torpidity, torporLevel, 0, 0, true,
                                    1, intersectionIB.Max, useTroodonismStats: useTroodonismStats) >= torpor)
                            {
                                //imprintingBonusFromTorpor = imprintingBonusFromFood;
                                imprintingBonusRange.SetToIntersectionWith(imprintingBonusFromFood);
                                support++;
                            }
                        }
                    }
                }

                // if the imprinting bonus value considering only the fixed imprinting gain by cuddles results in a value in the possible range, take this, probably most exact value
                if (imprintingBonusRange.Includes(imprintingBonusFromGainPerCuddle)
                    && Math.Abs(StatValueCalculation.CalculateValue(species, Stats.Torpidity, torporLevel, 0, 0, true, 1, imprintingBonusFromGainPerCuddle, useTroodonismStats: useTroodonismStats) - torpor) <= 0.5)
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
            if (_bred
                || maxWildLevel <= 0
                || LevelWildMutSum + 1 <= maxWildLevel) return;

            var minTeCheck = 2d * (LevelWildMutSum + 1 - maxWildLevel) / maxWildLevel;

            // if min TE is equal or greater than 1, that indicates it can't possibly be anything but bred, and there cannot be any results that should be sorted out
            if (!(minTeCheck < 1)) return;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (Results[s].Count == 0 || Results[s][0].Te.Max < 0)
                    continue;
                for (int r = 0; r < Results[s].Count; r++)
                {
                    if (Results[s][r].Te.Max < minTeCheck)
                        Results[s].RemoveAt(r--);
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

            _levelsUndeterminedWild = LevelWildMutSum;
            _levelsUndeterminedDom = LevelDomSum;
            // subtract all uniquely solved stat-levels from possible max and min of sum
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity) continue;
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
                    for (int s = 0; s < Stats.StatsCount; s++)
                    {
                        if (s == Stats.Torpidity) continue;
                        for (int r = 0; r < Results[s].Count; r++)
                        {
                            if (Results[s].Count > 1
                                && (Results[s][r].LevelWild > _levelsUndeterminedWild - _lowerBoundWilds.Sum() + _lowerBoundWilds[s]
                                 || Results[s][r].LevelDom > _levelsUndeterminedDom - _lowerBoundDoms.Sum() + _lowerBoundDoms[s]
                                 || Results[s][r].LevelDom < _levelsUndeterminedDom - _upperBoundDoms.Sum() + _upperBoundDoms[s]))
                            {
                                // if the sorted out result could affect the bounds, set the bounds again
                                bool adjustBounds = Results[s][r].LevelWild == _lowerBoundWilds[s]
                                                   || Results[s][r].LevelDom == _lowerBoundDoms[s]
                                                   || Results[s][r].LevelDom == _upperBoundDoms[s];

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
                                    if (Results[StatsWithTE[es]][ere].Te.Overlaps(Results[StatsWithTE[et]][erf].Te))
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
                if (Results[statIndex][0].LevelWild > 0)
                    _levelsUndeterminedWild -= Results[statIndex][0].LevelWild;
                if (Results[statIndex][0].LevelDom > 0)
                    _levelsUndeterminedDom -= Results[statIndex][0].LevelDom;
                // bounds only contain the bounds of not unique stats
                _lowerBoundWilds[statIndex] = 0;
                _lowerBoundDoms[statIndex] = 0;
                _upperBoundDoms[statIndex] = 0;
                boundsWhereChanged = true;
            }
            else if (Results[statIndex].Count > 1)
            {
                // get the smallest and largest value
                int minW = Results[statIndex][0].LevelWild, minD = Results[statIndex][0].LevelDom, maxD = Results[statIndex][0].LevelDom;
                for (int r = 1; r < Results[statIndex].Count; r++)
                {
                    if (Results[statIndex][r].LevelWild < minW) { minW = Results[statIndex][r].LevelWild; }
                    if (Results[statIndex][r].LevelDom < minD) { minD = Results[statIndex][r].LevelDom; }
                    if (Results[statIndex][r].LevelDom > maxD) { maxD = Results[statIndex][r].LevelDom; }
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
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (s == Stats.Torpidity) continue;
                for (int r = 0; r < Results[s].Count; r++)
                {
                    Results[s][r].CurrentlyNotValid = (FixedResults[s] && dontFix != s && r != ChosenResults[s]);
                }
                // subtract fixed stat-levels, but not from the current stat
                if (FixedResults[s] && dontFix != s)
                {
                    wildMax -= Results[s][ChosenResults[s]].LevelWild;
                    dom -= Results[s][ChosenResults[s]].LevelDom;
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
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (s == Stats.Torpidity) continue;
                    int validResultsNr = 0;
                    int uniqueR = -1;
                    for (int r = 0; r < Results[s].Count; r++)
                    {
                        if (!Results[s][r].CurrentlyNotValid)
                            validResultsNr++;
                    }
                    if (validResultsNr > 1)
                    {
                        for (int r = 0; r < Results[s].Count; r++)
                        {
                            if (!Results[s][r].CurrentlyNotValid && (Results[s][r].LevelWild > wildMax - lowBoundWs.Sum() + lowBoundWs[s] || Results[s][r].LevelDom > dom - lowBoundDs.Sum() + lowBoundDs[s] || Results[s][r].LevelDom < dom - uppBoundDs.Sum() + uppBoundDs[s]))
                            {
                                Results[s][r].CurrentlyNotValid = true;
                                validResultsNr--;
                                // if result gets unique due to this, check if remaining result doesn't violate for max level
                                if (validResultsNr == 1)
                                {
                                    // find unique valid result
                                    for (int rr = 0; rr < Results[s].Count; rr++)
                                    {
                                        if (!Results[s][rr].CurrentlyNotValid)
                                        {
                                            uniqueR = rr;
                                            break;
                                        }
                                    }
                                    loopAgain = true;
                                    wildMax -= Results[s][uniqueR].LevelWild;
                                    dom -= Results[s][uniqueR].LevelDom;
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
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    if (Results[s].Count == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Returns the taming effectiveness for the selected stat levels.
        /// A value =&gt;0 is a valid value.
        /// -1 indicates the TE cannot be determined.
        /// -2 indicates the TE of the stats that use it is different, i.e. there's an issue with the stat values.
        /// </summary>
        /// <returns></returns>
        public double UniqueTamingEffectiveness()
        {
            double eff = -1;
            if (StatsWithTE.Any() && Results[StatsWithTE[0]].Count > ChosenResults[StatsWithTE[0]])
            {
                for (int s = 0; s < StatsWithTE.Count; s++)
                {
                    for (int ss = s + 1; ss < StatsWithTE.Count; ss++)
                    {
                        // effectiveness-calculation can be a bit off due to ingame-rounding
                        if (Results[StatsWithTE[ss]].Count <= ChosenResults[StatsWithTE[ss]]
                            || !MinMaxDouble.Overlaps(Results[StatsWithTE[s]][ChosenResults[StatsWithTE[s]]].Te,
                                                      Results[StatsWithTE[ss]][ChosenResults[StatsWithTE[ss]]].Te))
                        {
                            return -2; // no unique TE
                        }
                    }
                }
                // calculate most probable real TE
                // get intersection of all TE-ranges

                MinMaxDouble te = Results[StatsWithTE[0]][ChosenResults[StatsWithTE[0]]].Te.Clone();
                for (int s = 1; s < StatsWithTE.Count; s++)
                {
                    // the overlap is ensured at this point
                    te.SetToIntersectionWith(Results[StatsWithTE[s]][ChosenResults[StatsWithTE[s]]].Te);
                }

                eff = te.Mean;
            }
            return eff;
        }

        /// <summary>
        /// The mean of the calculated imprinting bonus range, then clamped to the allowed range.
        /// </summary>
        public double ImprintingBonus;
    }
}

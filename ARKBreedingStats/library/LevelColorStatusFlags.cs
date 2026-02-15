using System;
using System.Collections.Generic;
using System.Text;
using ARKBreedingStats.species;
using ARKBreedingStats.SpeciesOptions.TopStatsSettings;
using ARKBreedingStats.uiControls;
using static ARKBreedingStats.uiControls.StatWeighting;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Level and color status flags of the current extracted or to be added creature.
    /// E.g. if a level is a new top stat or has a new mutations.
    /// </summary>
    public static class LevelColorStatusFlags
    {
        /// <summary>
        /// The level status of the currently to be added creature.
        /// </summary>
        public static readonly LevelStatus[] LevelStatusFlagsCurrentNewCreature = new LevelStatus[Stats.StatsCount];
        /// <summary>
        /// Stat level flags of all stats merged.
        /// </summary>
        public static LevelStatus StatLevelStatusFlagsCombined;
        public static ColorStatus[] ColorFlags = new ColorStatus[Ark.ColorRegionCount];
        /// <summary>
        /// Color flags of all regions merged.
        /// </summary>
        public static ColorStatus ColorFlagsCombined;
        public static string LevelInfoText;

        /// <summary>
        /// Determines if the wild and mutated levels of a creature are equal or higher than the current top levels of that species.
        /// </summary>
        public static void DetermineLevelStatus(Species species, TopLevels topLevels,
            (double[], StatValueEvenOdd[]) statWeights, ConsiderTopStats[] considerTopStats, int[] levelsWild, int[] levelsMutated, double[] valuesBreeding,
            out List<string> topStatsText, out List<string> newTopStatsText)
        {
            // if there are no creatures of the species yet, assume 0 levels to be the current best and worst
            if (topLevels == null) topLevels = new TopLevels();
            var highSpeciesLevels = topLevels.WildLevelsHighest;
            var lowSpeciesLevels = topLevels.WildLevelsLowest;
            var highSpeciesMutationLevels = topLevels.MutationLevelsHighest;

            newTopStatsText = new List<string>();
            topStatsText = new List<string>();
            var sbStatInfoText = new StringBuilder();
            StatLevelStatusFlagsCombined = LevelStatus.Neutral;

            foreach (var s in Stats.DisplayOrder)
            {
                LevelStatusFlagsCurrentNewCreature[s] = LevelStatus.Neutral;

                if (s == Stats.Torpidity
                    || levelsWild[s] < 0
                    || !species.UsesStat(s)
                    || !species.CanLevelUpWildOrHaveMutations(s)
                    || !considerTopStats[s].ConsiderStat
                    )
                    continue;

                var statName = Utils.StatName(s, false, species.statNames);
                var statNameAbb = Utils.StatName(s, true, species.statNames);
                var statWeight = statWeights.Item1[s];
                var weighting = statWeight == 0
                    ? StatWeighting.StatValuePreference.Indifferent
                    : statWeight > 0 ? StatWeighting.StatValuePreference.High
                    : StatWeighting.StatValuePreference.Low;

                sbStatInfoText.Append(levelsMutated != null
                    ? $"{statNameAbb}: {levelsWild[s]} | {levelsMutated[s]} ({valuesBreeding[s]})"
                    : $"{statNameAbb}: {levelsWild[s]} ({valuesBreeding[s]})");

                if (weighting == StatWeighting.StatValuePreference.High)
                {
                    // higher stats are considered to be good. If no custom weightings are available, consider higher levels to be better.

                    // check if higher level is only considered if even or odd
                    if (statWeights.Item2[s] == StatValueEvenOdd.Indifferent // even/odd doesn't matter
                        || (statWeights.Item2[s] == StatValueEvenOdd.Odd && levelsWild[s] % 2 == 1)
                        || (statWeights.Item2[s] == StatValueEvenOdd.Even && levelsWild[s] % 2 == 0)
                        )
                    {
                        if (levelsWild[s] > highSpeciesLevels[s])
                        {
                            LevelStatusFlagsCurrentNewCreature[s] = LevelStatus.NewTopLevel;
                            StatLevelStatusFlagsCombined |= LevelStatus.NewTopLevel;
                            newTopStatsText.Add(statName);
                            sbStatInfoText.Append($" {Loc.S("newTopLevel")}");
                        }
                        else if (levelsWild[s] > 0 && levelsWild[s] == highSpeciesLevels[s])
                        {
                            LevelStatusFlagsCurrentNewCreature[s] = LevelStatus.TopLevel;
                            StatLevelStatusFlagsCombined |= LevelStatus.TopLevel;
                            topStatsText.Add(statName);
                            sbStatInfoText.Append($" {Loc.S("topLevel")}");
                        }
                    }
                }
                else if (weighting == StatWeighting.StatValuePreference.Low)
                {
                    // lower stats are considered to be good
                    if (levelsWild[s] == lowSpeciesLevels[s])
                    {
                        LevelStatusFlagsCurrentNewCreature[s] = LevelStatus.TopLevel;
                        StatLevelStatusFlagsCombined |= LevelStatus.TopLevel;
                        topStatsText.Add(statName);
                        sbStatInfoText.Append($" {Loc.S("topLevel")}");
                    }
                    else if (levelsWild[s] < lowSpeciesLevels[s])
                    {
                        LevelStatusFlagsCurrentNewCreature[s] = LevelStatus.NewTopLevel;
                        StatLevelStatusFlagsCombined |= LevelStatus.NewTopLevel;
                        newTopStatsText.Add(statName);
                        sbStatInfoText.Append($" {Loc.S("newTopLevel")}");
                    }
                }

                if (weighting == StatWeighting.StatValuePreference.High
                    && levelsMutated[s] > highSpeciesMutationLevels[s])
                {
                    LevelStatusFlagsCurrentNewCreature[s] |= LevelStatus.NewMutation;
                    StatLevelStatusFlagsCombined |= LevelStatus.NewMutation;
                    sbStatInfoText.Append($" {Loc.S("new mutation")}");
                }
                sbStatInfoText.AppendLine();
            }

            LevelInfoText = sbStatInfoText.ToString();
        }

        public static void Clear()
        {
            for (var s = 0; s < Stats.StatsCount; s++)
                LevelStatusFlagsCurrentNewCreature[s] = LevelStatus.Neutral;
            StatLevelStatusFlagsCombined = LevelStatus.Neutral;
            for (var c = 0; c < Ark.ColorRegionCount; c++) ColorFlags[c] = ColorStatus.None;
            ColorFlagsCombined = ColorStatus.None;
            LevelInfoText = null;
        }

        /// <summary>
        /// Status of wild levels, e.g. top level, max level.
        /// </summary>
        [Flags]
        public enum LevelStatus
        {
            Neutral = 0,
            /// <summary>
            /// wild level is equal to the current top-level
            /// </summary>
            TopLevel = 1,
            /// <summary>
            /// wild level is higher than the current top-level
            /// </summary>
            NewTopLevel = 2,
            /// <summary>
            /// Max level to apply domesticated levels.
            /// </summary>
            MaxLevelForLevelUp = 4,
            /// <summary>
            /// Max level that can be saved.
            /// </summary>
            MaxLevel = 8,
            /// <summary>
            /// Level too high to be saved, rollover will happen.
            /// </summary>
            UltraMaxLevel = 16,
            /// <summary>
            /// Stat has new mutation.
            /// </summary>
            NewMutation = 32
        }

        [Flags]
        public enum ColorStatus
        {
            None = 0,
            /// <summary>
            /// The color is already available in that region on a creature of that species.
            /// </summary>
            ExistsInRegion = 1,
            /// <summary>
            /// The color is new in this region, but already available in a different region on a creature of that species.
            /// </summary>
            NewRegionColor = 2,
            /// <summary>
            /// The color does not exist on any region on any creature of that species.
            /// </summary>
            NewColor = 4,
            /// <summary>
            /// The color is set as desired for this species.
            /// </summary>
            DesiredColor = 8
        }
    }
}

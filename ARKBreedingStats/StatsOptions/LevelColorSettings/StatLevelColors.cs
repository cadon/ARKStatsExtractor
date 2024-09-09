using System.Drawing;
using ARKBreedingStats.StatsOptions.LevelColorSettings;
using Newtonsoft.Json;

namespace ARKBreedingStats.StatsOptions
{
    /// <summary>
    /// Options for a stat regarding breeding weights, top stat calculation and graph representation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class StatLevelColors : StatOptionsBase
    {
        /// <summary>
        /// Use for all levels, or only for even levels if LevelGraphRepresentationOdd is not null.
        /// </summary>
        [JsonProperty("lvl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public LevelGraphRepresentation LevelGraphRepresentation;

        /// <summary>
        /// If not null use this for odd levels and LevelGraphRepresentation only for even levels.
        /// </summary>
        [JsonProperty("lvlOdd", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public LevelGraphRepresentation LevelGraphRepresentationOdd;

        /// <summary>
        /// If not null use this for mutation levels.
        /// </summary>
        [JsonProperty("lvlMut", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public LevelGraphRepresentation LevelGraphRepresentationMutation;

        public bool UseDifferentColorsForOddLevels;
        public bool UseDifferentColorsForMutationLevels;

        public StatLevelColors Copy()
        {
            var c = new StatLevelColors
            {
                LevelGraphRepresentation = LevelGraphRepresentation?.Copy(),
                LevelGraphRepresentationOdd = LevelGraphRepresentationOdd?.Copy(),
                LevelGraphRepresentationMutation = LevelGraphRepresentationMutation?.Copy(),
                OverrideParent = OverrideParent
            };
            return c;
        }

        private LevelGraphRepresentation GetLevelGraphRepresentation(int level, bool useCustomOdd, bool mutationLevel)
        {
            if (mutationLevel
                && UseDifferentColorsForMutationLevels
                && LevelGraphRepresentationMutation != null)
                return LevelGraphRepresentationMutation;

            if (useCustomOdd
                && UseDifferentColorsForOddLevels
                && LevelGraphRepresentationOdd != null
                && level % 2 == 1)
                return LevelGraphRepresentationOdd;

            return LevelGraphRepresentation ?? LevelGraphRepresentation.GetDefaultValue;
        }

        public Color GetLevelColor(int level, bool useCustomOdd = true, bool mutationLevel = false)
            => GetLevelGraphRepresentation(level, useCustomOdd, mutationLevel).GetLevelColor(level);

        public int GetLevelRange(int level, out int lowerBound, bool useCustomOdd = true, bool mutationLevel = false)
        {
            var levelRepresentations = GetLevelGraphRepresentation(level, useCustomOdd, mutationLevel);
            lowerBound = levelRepresentations.LowerBound;
            return levelRepresentations.UpperBound - lowerBound;
        }

        /// <summary>
        /// Call after loading.
        /// </summary>
        public override void Initialize()
        {
            if (LevelGraphRepresentation != null || LevelGraphRepresentationOdd != null)
                OverrideParent = true;
            if (LevelGraphRepresentationOdd != null)
                UseDifferentColorsForOddLevels = true;
            if (LevelGraphRepresentationMutation != null)
                UseDifferentColorsForMutationLevels = true;
        }

        /// <summary>
        /// Call before saving. Sets unused settings to null.
        /// </summary>
        public override void PrepareForSaving(bool isRoot)
        {
            if (!OverrideParent && !isRoot)
            {
                LevelGraphRepresentation = null;
                LevelGraphRepresentationOdd = null;
                LevelGraphRepresentationMutation = null;
                return;
            }
            if (!UseDifferentColorsForOddLevels)
                LevelGraphRepresentationOdd = null;
            if (!UseDifferentColorsForMutationLevels)
                LevelGraphRepresentationMutation = null;
        }

        public override bool DefinesData() => LevelGraphRepresentation != null;

        public static StatLevelColors GetDefault() => new StatLevelColors
        {
            LevelGraphRepresentation = LevelGraphRepresentation.GetDefaultValue,
            LevelGraphRepresentationMutation = LevelGraphRepresentation.GetDefaultMutationLevelValue
        };
    }
}

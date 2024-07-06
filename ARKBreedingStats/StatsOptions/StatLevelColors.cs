using System.Drawing;
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

        public bool UseDifferentColorsForOddLevels;

        public StatLevelColors Copy()
        {
            var c = new StatLevelColors
            {
                LevelGraphRepresentation = LevelGraphRepresentation.Copy(),
                LevelGraphRepresentationOdd = LevelGraphRepresentationOdd.Copy(),
                OverrideParent = OverrideParent
            };
            return c;
        }

        public Color GetLevelColor(int level)
        {
            var levelRepresentations =
                UseDifferentColorsForOddLevels
                && LevelGraphRepresentationOdd != null
                && level % 2 == 1
                    ? LevelGraphRepresentationOdd
                    : LevelGraphRepresentation;
            return levelRepresentations.GetLevelColor(level);
        }

        public int GetLevelRange(int level, out int lowerBound)
        {
            var levelRepresentations =
                UseDifferentColorsForOddLevels
                && LevelGraphRepresentationOdd != null
                && level % 2 == 1
                    ? LevelGraphRepresentationOdd
                    : LevelGraphRepresentation;
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
        }

        /// <summary>
        /// Call before saving.
        /// </summary>
        public override void PrepareForSaving()
        {
            if (!OverrideParent)
            {
                LevelGraphRepresentation = null;
                LevelGraphRepresentationOdd = null;
            }
            else if (!UseDifferentColorsForOddLevels)
                LevelGraphRepresentationOdd = null;
        }

        public override bool DefinesData() => LevelGraphRepresentation != null;

        public static StatLevelColors GetDefault() => new StatLevelColors
        {
            LevelGraphRepresentation = LevelGraphRepresentation.GetDefaultValue
        };
    }
}

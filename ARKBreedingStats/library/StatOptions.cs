using System.Drawing;
using Newtonsoft.Json;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Options for a stat regarding breeding weights, top stat calculation and graph representation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class StatOptions
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
        /// If true don't use values of parent but overrides of this object.
        /// </summary>
        public bool OverrideParent;
        public bool UseDifferentColorsForOddLevels;

        public StatOptions Copy()
        {
            var c = new StatOptions
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

        // TODO stat weights
    }
}

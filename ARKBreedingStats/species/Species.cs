using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ARKBreedingStats.species
{
    [DataContract]
    public class Species
    {
        /// <summary>
        /// The name as it is displayed for the user in most controls.
        /// </summary>
        [DataMember]
        public string name;
        /// <summary>
        /// The name used for sorting in lists.
        /// </summary>
        [IgnoreDataMember]
        public string SortName;
        /// <summary>
        /// The name suffixed by possible additional infos like cave, minion.
        /// </summary>
        [IgnoreDataMember]
        public string DescriptiveName;
        /// <summary>
        /// The name of the species suffixed by the mod it comes from.
        /// </summary>
        [IgnoreDataMember]
        public string NameAndMod { get; private set; }
        [DataMember]
        public string blueprintPath;
        [DataMember]
        public double?[][] fullStatsRaw; // without multipliers
        public List<CreatureStat> stats;
        /// <summary>
        /// Indicates if a stat is shown ingame.
        /// </summary>
        public bool[] displayedStats;
        /// <summary>
        /// Indicates if a species uses a stat.
        /// </summary>
        public bool[] usedStats;
        public int usedStatCount;
        [DataMember]
        public float? TamedBaseHealthMultiplier;
        [DataMember]
        public bool? NoImprintingForSpeed;
        [DataMember]
        public List<ColorRegion> colors; // each creature has up to 6 colorregions
        [DataMember]
        public TamingData taming;
        [DataMember]
        public BreedingData breeding;
        [DataMember]
        public bool doesNotUseOxygen;
        [DataMember]
        public Dictionary<string, double> boneDamageAdjusters;
        [DataMember]
        public List<string> immobilizedBy;
        /// <summary>
        /// Information about the mod. If this value equals null, the species is probably from the base-game.
        /// </summary>
        private Mod _mod;

        private static int COLOR_REGION_COUNT = 6;

        /// <summary>
        /// creates properties that are not created during deserialization. They are set later with the raw-values with the multipliers applied.
        /// </summary>
        [OnDeserialized]
        private void Initialize(StreamingContext context)
        {
            Regex rSuffixes = new Regex(@"(Cave(?!Wolf)|Minion|Hard|Medium|Easy)"); // some default species start with the word Cave. don't append the suffix there.
            List<string> foundSuffixes = new List<string>();
            var ms = rSuffixes.Matches(blueprintPath);
            foreach (Match m in ms)
            {
                if (!foundSuffixes.Contains(m.Value))
                    foundSuffixes.Add(m.Value);
            }

            DescriptiveName = name + (foundSuffixes.Count > 0 ? " (" + string.Join(", ", foundSuffixes) + ")" : string.Empty);
            SortName = DescriptiveName;
            NameAndMod = DescriptiveName + (string.IsNullOrEmpty(_mod?.title) ? "" : " (" + _mod.title + ")");
            stats = new List<CreatureStat>();
            usedStats = new bool[Values.STATS_COUNT];
            usedStatCount = 0;
            double?[][] completeRaws = new double?[Values.STATS_COUNT][];
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                stats.Add(new CreatureStat((StatNames)s));
                completeRaws[s] = new double?[] { 0, 0, 0, 0, 0 };
                if (fullStatsRaw.Length > s && fullStatsRaw[s] != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (fullStatsRaw[s].Length > i)
                        {
                            completeRaws[s][i] = fullStatsRaw[s][i] != null ? fullStatsRaw[s][i] : 0;
                            if (i == 0 && fullStatsRaw[s][0] > 0)
                            {
                                usedStats[s] = true;
                                usedStatCount++;
                            }
                        }
                    }
                }
            }
            fullStatsRaw = completeRaws;
            if (TamedBaseHealthMultiplier == null)
                TamedBaseHealthMultiplier = 1;
            if (NoImprintingForSpeed == null)
                NoImprintingForSpeed = false;

            if (colors == null)
                colors = new List<ColorRegion>();
            for (int c = 0; c < COLOR_REGION_COUNT; c++)
            {
                if (colors.Count <= c)
                {
                    colors.Add(null);
                }
            }
            if (string.IsNullOrEmpty(blueprintPath))
                blueprintPath = string.Empty;

            if (boneDamageAdjusters != null && boneDamageAdjusters.Count > 0)
            {
                // cleanup boneDamageMultipliers. Remove duplicates. Improve names.
                var boneDamageAdjustersCleanedUp = new Dictionary<string, double>();
                Regex rCleanBoneDamage = new Regex(@"(^r_|^l_|^c_|Cnt_|JNT|\d+|SKL)");
                foreach (KeyValuePair<string, double> bd in boneDamageAdjusters)
                {
                    string boneName = rCleanBoneDamage.Replace(bd.Key, "").Replace("_", "");
                    if (boneName.Length < 2) continue;
                    boneName = boneName.Substring(0, 1).ToUpper() + boneName.Substring(1);
                    if (!boneDamageAdjustersCleanedUp.ContainsKey(boneName))
                        boneDamageAdjustersCleanedUp.Add(boneName, Math.Round(bd.Value, 2));
                }
                boneDamageAdjusters = boneDamageAdjustersCleanedUp;
            }
        }

        public override string ToString()
        {
            return NameAndMod ?? name;
        }

        public override int GetHashCode()
        {
            return blueprintPath.GetHashCode();
        }

        public bool Equals(Species other)
        {
            return !string.IsNullOrEmpty(blueprintPath) && other.blueprintPath == blueprintPath;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj is Species speciesObj && Equals(speciesObj);
        }

        public Mod mod
        {
            set
            {
                _mod = value;
                NameAndMod = name + (string.IsNullOrEmpty(_mod?.title) ? "" : " (" + _mod.title + ")");
            }
        }
    }
}

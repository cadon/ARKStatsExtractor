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
        /// The name suffixed by possible additional infos like cave, minion, etc.
        /// </summary>
        [IgnoreDataMember]
        public string DescriptiveName { get; private set; }
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
        /// Indicates if a stat is shown ingame represented by bit-flags
        /// </summary>
        [DataMember]
        private int displayedStats;
        [IgnoreDataMember]
        public const int displayedStatsDefault = 927;
        /// <summary>
        /// Indicates if a species uses a stat represented by bit-flags
        /// </summary>
        [IgnoreDataMember]
        private int usedStats;
        [DataMember]
        public float? TamedBaseHealthMultiplier;
        /// <summary>
        /// Indicates the multipliers for each stat applied to the imprinting-bonus
        /// </summary>
        [DataMember]
        public double[] statImprintMult;
        /// <summary>
        /// Default values of the statImprintMult if not specified else
        /// </summary>
        [IgnoreDataMember]
        public static double[] statImprintMultDefault = new double[] { 0.2, 0, 0.2, 0, 0.2, 0.2, 0, 0.2, 0.2, 0.2, 0, 0 };
        [DataMember]
        public List<ColorRegion> colors; // each creature has up to 6 colorregions
        [DataMember]
        public TamingData taming;
        [DataMember]
        public BreedingData breeding;
        [DataMember]
        public Dictionary<string, double> boneDamageAdjusters;
        [DataMember]
        public List<string> immobilizedBy;
        /// <summary>
        /// Information about the mod. If this value equals null, the species is probably from the base-game.
        /// </summary>
        private Mod _mod;
        /// <summary>
        /// Determines if species has different stat-names.
        /// </summary>
        public bool IsGlowSpecies;

        private static int COLOR_REGION_COUNT = 6;

        /// <summary>
        /// creates properties that are not created during deserialization. They are set later with the raw-values with the multipliers applied.
        /// </summary>
        [OnDeserialized]
        private void Initialize(StreamingContext context)
        {
            // TODO: Base species are maybe not used ingame and may only lead to confusion (e.g. Giganotosaurus). Some base-species are the only ones used, though (e.g. Glowbug).
            // LL refers to LifeLabyrint-variants in Ragnarok
            // Chalk is used for the Valguero variant of the Rock Golem
            // Ocean is used for the Coelacanth
            Regex rSuffixes = new Regex(@"((?:Tek)?Cave(?!Wolf)|Minion|Surface|Boss|Hard|Med(?:ium)?|Easy|Aggressive|EndTank|Base|LL|Chalk|Ocean|Polar|Ice|Zombie|TheCenter)"); // some default species start with the word Cave. don't append the suffix there.
            List<string> foundSuffixes = new List<string>();
            var ms = rSuffixes.Matches(blueprintPath);
            foreach (Match m in ms)
            {
                string suffix = m.Value;
                if (suffix == "Med") suffix = "Medium";
                if (!name.Contains(suffix)
                    && !foundSuffixes.Contains(suffix))
                    foundSuffixes.Add(suffix);
            }

            // some creatures have variants on the different official DLC-maps (e.g. Ice Wyvern on Ragnarok and Valguero). Add suffix to distinguish them.
            var officialDLCs = new List<string> { "Ragnarok", "Valguero" };
            Regex rOfficialDLCs = new Regex(@"^\/Game\/Mods\/([^\/]+)\/");
            var mDLC = rOfficialDLCs.Match(blueprintPath);
            string variantTag = mDLC.Success && officialDLCs.Contains(mDLC.Groups[1].Value) ? mDLC.Groups[1].Value : "";

            var officialVariants = new List<string> { "EndGame" };
            Regex rOfficialVariants = new Regex(@"^\/Game\/([^\/]+)\/");
            var mVariant = rOfficialVariants.Match(blueprintPath);
            if (mVariant.Success && officialVariants.Contains(mVariant.Groups[1].Value))
                variantTag = mVariant.Groups[1].Value;

            DescriptiveName = name + (foundSuffixes.Count > 0 ? " (" + string.Join(", ", foundSuffixes) + ")" : string.Empty);
            SortName = DescriptiveName;
            string modSuffix = string.IsNullOrEmpty(_mod?.title) ?
                (string.IsNullOrEmpty(variantTag)
                    ? ""
                    : variantTag)
                : _mod.title;
            NameAndMod = DescriptiveName + (string.IsNullOrEmpty(modSuffix) ? "" : " (" + modSuffix + ")");
            stats = new List<CreatureStat>();
            usedStats = 0;
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
                            completeRaws[s][i] = fullStatsRaw[s]?[i] ?? 0;
                            if (i == 0 && fullStatsRaw[s][0] > 0)
                            {
                                usedStats |= (1 << s);
                            }
                        }
                    }
                }
            }
            fullStatsRaw = completeRaws;
            if (TamedBaseHealthMultiplier == null)
                TamedBaseHealthMultiplier = 1;

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

            IsGlowSpecies = new List<string> { "Bulbdog", "Featherlight", "Glowbug", "Glowtail", "Shinehorn" }.Contains(name);

            if (statImprintMult == null) statImprintMult = statImprintMultDefault;
        }

        public void InitializeColors(ARKColors arkColors)
        {
            for (int i = 0; i < COLOR_REGION_COUNT; i++)
                colors[i]?.Initialize(arkColors);
        }

        /// <summary>
        /// Returns if a stat is used (i.e. has a base value > 0) by a species.
        /// </summary>
        /// <param name="statIndex"></param>
        /// <returns></returns>
        public bool UsesStat(int statIndex) => (usedStats & 1 << statIndex) != 0;

        /// <summary>
        /// Returns if a stat is displayed ingame (e.g. can be leveled) by a species.
        /// </summary>
        /// <param name="statIndex"></param>
        /// <returns></returns>
        public bool DisplaysStat(int statIndex) => (displayedStats & 1 << statIndex) != 0;

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
                NameAndMod = DescriptiveName + (string.IsNullOrEmpty(_mod?.title) ? "" : " (" + _mod.title + ")");
            }
        }
    }
}

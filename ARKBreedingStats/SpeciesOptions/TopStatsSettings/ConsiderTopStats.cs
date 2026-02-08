using Newtonsoft.Json;
using System.Linq;

namespace ARKBreedingStats.SpeciesOptions.TopStatsSettings
{
    /// <summary>
    /// Setting which stats are considered in top stats calculation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ConsiderTopStats : SpeciesOptionBase
    {
        [JsonProperty("top", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ConsiderStat;

        /// <summary>
        /// Override parent setting.
        /// This property is saved explicitly for top stats, for other options it might be implicit without the need to save it.
        /// </summary>
        [JsonProperty("ovr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool OverrideParentBool;

        public override void Initialize()
        {
            OverrideParent = OverrideParentBool;
        }

        public override void PrepareForSaving(bool isRoot)
        {
            OverrideParentBool = OverrideParent || isRoot;
        }

        public override bool DefinesData() => true;

        public static ConsiderTopStats GetDefault() => new ConsiderTopStats
        {
            OverrideParent = true,
            ConsiderStat = true
        };

        public static ConsiderTopStats[] GetDefaultOptions()
        {
            var statIndicesToConsiderDefault = new[] { Stats.Health, Stats.Stamina, Stats.Weight, Stats.MeleeDamageMultiplier };
            return Enumerable.Range(0, Stats.StatsCount)
                .Select(si => new ConsiderTopStats { OverrideParent = true, ConsiderStat = statIndicesToConsiderDefault.Contains(si) }).ToArray();
        }
    }
}

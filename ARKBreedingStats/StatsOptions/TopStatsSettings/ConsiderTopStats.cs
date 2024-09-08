using Newtonsoft.Json;

namespace ARKBreedingStats.StatsOptions.TopStatsSettings
{
    /// <summary>
    /// Setting which stats are considered in top stats calculation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class ConsiderTopStats : StatOptionsBase
    {
        [JsonProperty("top", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ConsiderStat;

        /// <summary>
        /// Override parent setting.
        /// </summary>
        [JsonProperty("ovr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool OverrideParentBool;

        public override void Initialize()
        {
            OverrideParent = OverrideParentBool;
        }

        public override void PrepareForSaving()
        {
            OverrideParentBool = OverrideParent;
        }

        public override bool DefinesData() => true;

        public static ConsiderTopStats GetDefault() => new ConsiderTopStats
        {
            OverrideParent = true,
            ConsiderStat = true
        };
    }
}

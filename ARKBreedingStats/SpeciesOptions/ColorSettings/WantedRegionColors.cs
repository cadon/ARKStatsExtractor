using Newtonsoft.Json;

namespace ARKBreedingStats.SpeciesOptions.ColorSettings
{
    /// <summary>
    /// List of color ids wanted for a region.
    /// </summary>
    internal class WantedRegionColors : SpeciesOptionBase
    {
        [JsonProperty("colorIds", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public byte[] WantedColors;

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

        public static WantedRegionColors GetDefault() => new WantedRegionColors { OverrideParentBool = true };
    }
}

using Newtonsoft.Json;

namespace ARKBreedingStats.importExportGun
{
    /// <summary>
    /// Server multipliers as exported by the export gun mod.
    /// </summary>
    [JsonObject]
    internal class ExportGunServerFile
    {
        public int Version { get; set; }
        public double[] WildLevel { get; set; }
        public double[] TameLevel { get; set; }
        public double[] TameAdd { get; set; }
        public double[] TameAff { get; set; }
        public double WildLevelStepSize { get; set; }
        public int MaxWildLevel { get; set; }
        public int DestroyTamesOverLevelClamp { get; set; }
        public double TamingSpeedMultiplier { get; set; }
        public double WildDinoTorporDrainMultiplier { get; set; }
        public double DinoCharacterFoodDrainMultiplier { get; set; }
        public double MatingSpeedMultiplier { get; set; }
        public double MatingIntervalMultiplier { get; set; }
        public double EggHatchSpeedMultiplier { get; set; }
        public double BabyMatureSpeedMultiplier { get; set; }
        public double BabyCuddleIntervalMultiplier { get; set; }
        public double BabyImprintAmountMultiplier { get; set; }
        public double BabyImprintingStatScaleMultiplier { get; set; }
        public double BabyFoodConsumptionSpeedMultiplier { get; set; }
        public double TamedDinoCharacterFoodDrainMultiplier { get; set; }
        public bool AllowSpeedLeveling { get; set; }
        public bool AllowFlyerSpeedLeveling { get; set; }
        public bool UseSingleplayerSettings { get; set; }
        public string SessionName { get; set; }

        /// <summary>
        /// ASE or ASA
        /// </summary>
        public string Game { get; set; }
    }
}

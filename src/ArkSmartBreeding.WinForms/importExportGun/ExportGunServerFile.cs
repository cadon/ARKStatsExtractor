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
        public double MaxWildLevel { get; set; }
        public int DestroyTamesOverLevelClamp { get; set; }
        public double TamingSpeedMultiplier { get; set; } = 1;
        public double WildDinoTorporDrainMultiplier { get; set; } = 1;
        public double DinoCharacterFoodDrainMultiplier { get; set; } = 1;
        public double WildDinoCharacterFoodDrainMultiplier { get; set; } = 1;
        public double TamedDinoCharacterFoodDrainMultiplier { get; set; } = 1;
        public double MatingSpeedMultiplier { get; set; } = 1;
        public double MatingIntervalMultiplier { get; set; } = 1;
        public double EggHatchSpeedMultiplier { get; set; } = 1;
        public double BabyMatureSpeedMultiplier { get; set; } = 1;
        public double BabyCuddleIntervalMultiplier { get; set; } = 1;
        public double BabyImprintAmountMultiplier { get; set; } = 1;
        public double BabyImprintingStatScaleMultiplier { get; set; } = 1;
        public double BabyFoodConsumptionSpeedMultiplier { get; set; } = 1;
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

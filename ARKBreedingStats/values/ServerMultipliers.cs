using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ARKBreedingStats.values
{
    /// <summary>
    /// Contains the multipliers of a server for stats, taming and breeding and levels
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerMultipliers
    {
        /// <summary>
        /// statMultipliers[statIndex][m], m: 0: Stats.IndexTamingAdd, 1: Stats.IndexTamingMult, 2: Stats.IndexLevelDom, 3: Stats.IndexLevelWild
        /// </summary>
        [JsonProperty]
        public double[][] statMultipliers;

        [JsonProperty]
        public double TamingSpeedMultiplier { get; set; } = 1;
        [JsonProperty]
        public double WildDinoTorporDrainMultiplier { get; set; } = 1;
        [JsonProperty]
        public double DinoCharacterFoodDrainMultiplier { get; set; } = 1;
        [JsonProperty]
        public double TamedDinoCharacterFoodDrainMultiplier { get; set; } = 1;

        [JsonProperty]
        public double MatingSpeedMultiplier { get; set; } = 1;
        [JsonProperty]
        public double MatingIntervalMultiplier { get; set; } = 1;
        [JsonProperty]
        public double EggHatchSpeedMultiplier { get; set; } = 1;

        [JsonProperty]
        public double BabyMatureSpeedMultiplier { get; set; } = 1;
        [JsonProperty]
        public double BabyFoodConsumptionSpeedMultiplier { get; set; } = 1;
        [JsonProperty]
        public double BabyCuddleIntervalMultiplier { get; set; } = 1;
        [JsonProperty]
        public double BabyImprintingStatScaleMultiplier { get; set; } = 1;
        [JsonProperty]
        public double BabyImprintAmountMultiplier { get; set; } = 1;

        /// <summary>
        /// Setting introduced in ASA, for ASE it's always true.
        /// </summary>
        [JsonProperty]
        public bool AllowSpeedLeveling { get; set; }
        [JsonProperty]
        public bool AllowFlyerSpeedLeveling { get; set; }

        /// <summary>
        /// Fix any null values
        /// </summary>
        [OnDeserialized]
        private void DefineNullValues(StreamingContext _)
        {
            if (statMultipliers == null) return;
            int l = statMultipliers.Length;
            for (int s = 0; s < l; s++)
            {
                if (statMultipliers[s] == null)
                    statMultipliers[s] = new double[] { 1, 1, 1, 1 };
            }
        }

        /// <summary>
        /// Returns a copy of the server multipliers
        /// </summary>
        /// <returns></returns>
        public ServerMultipliers Copy(bool withStatMultipliers)
        {
            var sm = new ServerMultipliers
            {
                TamingSpeedMultiplier = TamingSpeedMultiplier,
                WildDinoTorporDrainMultiplier = WildDinoTorporDrainMultiplier,
                DinoCharacterFoodDrainMultiplier = DinoCharacterFoodDrainMultiplier,
                TamedDinoCharacterFoodDrainMultiplier = TamedDinoCharacterFoodDrainMultiplier,
                MatingIntervalMultiplier = MatingIntervalMultiplier,
                EggHatchSpeedMultiplier = EggHatchSpeedMultiplier,
                MatingSpeedMultiplier = MatingSpeedMultiplier,
                BabyMatureSpeedMultiplier = BabyMatureSpeedMultiplier,
                BabyFoodConsumptionSpeedMultiplier = BabyFoodConsumptionSpeedMultiplier,
                BabyCuddleIntervalMultiplier = BabyCuddleIntervalMultiplier,
                BabyImprintingStatScaleMultiplier = BabyImprintingStatScaleMultiplier,
                BabyImprintAmountMultiplier = BabyImprintAmountMultiplier,
                AllowFlyerSpeedLeveling = AllowFlyerSpeedLeveling
            };

            if (withStatMultipliers && statMultipliers != null)
            {
                sm.statMultipliers = new double[Stats.StatsCount][];
                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    sm.statMultipliers[s] = new double[4];
                    for (int si = 0; si < 4; si++)
                        sm.statMultipliers[s][si] = statMultipliers[s][si];
                }
            }

            return sm;
        }

        /// <summary>
        /// Checks if critical values are zero and then sets them to one directly before they are used.
        /// This cannot be done directly after deserialization because these values can be multiplied later and can become zero.
        /// </summary>
        public void FixZeroValues()
        {
            if (TamingSpeedMultiplier == 0) TamingSpeedMultiplier = 1;
            if (WildDinoTorporDrainMultiplier == 0) WildDinoTorporDrainMultiplier = 1;
            if (MatingIntervalMultiplier == 0) MatingIntervalMultiplier = 1;
            if (EggHatchSpeedMultiplier == 0) EggHatchSpeedMultiplier = 1;
            if (MatingSpeedMultiplier == 0) MatingSpeedMultiplier = 1;
            if (BabyMatureSpeedMultiplier == 0) BabyMatureSpeedMultiplier = 1;
            if (BabyCuddleIntervalMultiplier == 0) BabyCuddleIntervalMultiplier = 1;
            if (BabyImprintAmountMultiplier == 0) BabyImprintAmountMultiplier = 1;
        }
    }
}

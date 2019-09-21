using System.Runtime.Serialization;

namespace ARKBreedingStats.values
{
    /// <summary>
    /// Contains the multipliers of a server for stats, taming and breeding and levels
    /// </summary>
    [DataContract]
    public class ServerMultipliers
    {
        /// <summary>
        /// statMultipliers[statIndex][m], m: 0:tamingadd, 1:tamingmult, 2:levelupdom, 3:levelupwild
        /// </summary>
        [DataMember]
        public double[][] statMultipliers;

        [DataMember]
        public double TamingSpeedMultiplier { get; set; }
        [DataMember]
        public double DinoCharacterFoodDrainMultiplier { get; set; }

        [DataMember]
        public double MatingIntervalMultiplier { get; set; }
        [DataMember]
        public double EggHatchSpeedMultiplier { get; set; }

        [DataMember]
        public double BabyMatureSpeedMultiplier { get; set; }
        [DataMember]
        public double BabyFoodConsumptionSpeedMultiplier { get; set; }
        [DataMember]
        public double BabyCuddleIntervalMultiplier { get; set; }
        [DataMember]
        public double BabyImprintingStatScaleMultiplier { get; set; }

        [OnDeserializing]
        private void SetDefaultValues(StreamingContext context)
        {
            TamingSpeedMultiplier = 1;
            DinoCharacterFoodDrainMultiplier = 1;
            MatingIntervalMultiplier = 1;
            EggHatchSpeedMultiplier = 1;
            BabyMatureSpeedMultiplier = 1;
            BabyFoodConsumptionSpeedMultiplier = 1;
            BabyCuddleIntervalMultiplier = 1;
            BabyImprintingStatScaleMultiplier = 1;
        }

        /// <summary>
        /// fix any null values
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void DefineNullValues(StreamingContext context)
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
                DinoCharacterFoodDrainMultiplier = DinoCharacterFoodDrainMultiplier,
                MatingIntervalMultiplier = MatingIntervalMultiplier,
                EggHatchSpeedMultiplier = EggHatchSpeedMultiplier,
                BabyMatureSpeedMultiplier = BabyMatureSpeedMultiplier,
                BabyFoodConsumptionSpeedMultiplier = BabyFoodConsumptionSpeedMultiplier,
                BabyCuddleIntervalMultiplier = BabyCuddleIntervalMultiplier,
                BabyImprintingStatScaleMultiplier = BabyImprintingStatScaleMultiplier
            };

            if (withStatMultipliers && statMultipliers != null)
            {
                sm.statMultipliers = new double[Values.STATS_COUNT][];
                for (int s = 0; s < Values.STATS_COUNT; s++)
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
            if (MatingIntervalMultiplier == 0) MatingIntervalMultiplier = 1;
            if (EggHatchSpeedMultiplier == 0) EggHatchSpeedMultiplier = 1;
            if (BabyMatureSpeedMultiplier == 0) BabyMatureSpeedMultiplier = 1;
            if (BabyCuddleIntervalMultiplier == 0) BabyCuddleIntervalMultiplier = 1;
            if (TamingSpeedMultiplier == 0) TamingSpeedMultiplier = 1;
        }
    }
}

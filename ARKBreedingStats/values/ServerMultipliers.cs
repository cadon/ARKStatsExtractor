using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
        /// Returns a copy of the server multipliers without the stat-multipliers
        /// </summary>
        /// <returns></returns>
        public ServerMultipliers Copy()
        {
            return new ServerMultipliers
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
        }

        /// <summary>
        /// Checks if critical values are zero and then sets them to one. Values can be multiplied later and can change.
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

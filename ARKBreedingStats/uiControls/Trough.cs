using System;
using ARKBreedingStats.species;

namespace ARKBreedingStats.uiControls
{
    public class Trough
    {
        //public readonly Dictionary<string, List<int>> foodStacks = new Dictionary<string, List<int>>();
        //public readonly Dictionary<string, int> spoilTimers = new Dictionary<string, int>();
        //private int ticks;
        //
        //public void Tick()
        //{
        //    ticks++;
        //    // subtract one item from each stack if spoiled
        //    foreach (KeyValuePair<string, int> s in spoilTimers)
        //    {
        //        if (s.Value % ticks == 0 && foodStacks.ContainsKey(s.Key))
        //        {
        //            for (int i = 0; i < foodStacks[s.Key].Count; i++)
        //            {
        //                foodStacks[s.Key][i]--;
        //                if (foodStacks[s.Key][i] == 0)
        //                    foodStacks[s.Key].RemoveAt(i--);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Calculates the needed food from a specific maturation to a specific maturation.
        /// </summary>
        public static bool FoodAmountFromUntil(Species species, double babyFoodConsumptionSpeedMultiplier, double dinoFoodDrainMultiplier, double tamedDinoFoodDrainMultiplier, double fromMaturation, double untilMaturation, out double totalFood)
        {
            totalFood = 0;
            if (fromMaturation == untilMaturation) return true;
            if (species?.taming == null || species.breeding == null || fromMaturation > untilMaturation || untilMaturation > 1) return false;

            // food rate in hunger units/s
            // min food rate at maturation 100 %
            var minFoodRate = species.taming.foodConsumptionBase * dinoFoodDrainMultiplier * tamedDinoFoodDrainMultiplier;
            // max food rate at maturation 0 %
            var maxFoodRate = minFoodRate * species.taming.babyFoodConsumptionMult * babyFoodConsumptionSpeedMultiplier;
            var foodRateDecay = minFoodRate - maxFoodRate;

            // to get the current food rate for a maturation value: maxFoodRate + maturation * foodRateDecay
            var foodRateStart = maxFoodRate + fromMaturation * foodRateDecay;
            var foodRateEnd = maxFoodRate + untilMaturation * foodRateDecay;

            // calculate area of rectangle and triangle on top to get the total food needed
            totalFood = species.breeding.maturationTimeAdjusted * ((untilMaturation - fromMaturation) * (foodRateEnd + 0.5 * Math.Abs(foodRateStart - foodRateEnd)));

            return true;
        }
    }
}

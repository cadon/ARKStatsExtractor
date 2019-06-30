using ARKBreedingStats.species;
using System.Collections.Generic;

namespace ARKBreedingStats.uiControls
{
    public class Trough
    {
        public readonly Dictionary<string, List<int>> foodStacks = new Dictionary<string, List<int>>();
        public readonly Dictionary<string, int> spoilTimers = new Dictionary<string, int>();
        private int ticks;

        public void Tick()
        {
            ticks++;
            // subtract one item from each stack if spoilt
            foreach (KeyValuePair<string, int> s in spoilTimers)
            {
                if (s.Value % ticks == 0 && foodStacks.ContainsKey(s.Key))
                {
                    for (int i = 0; i < foodStacks[s.Key].Count; i++)
                    {
                        foodStacks[s.Key][i]--;
                        if (foodStacks[s.Key][i] == 0)
                            foodStacks[s.Key].RemoveAt(i--);
                    }
                }
            }
        }

        public static bool foodAmount(Species species, double babyFoodConsumptionSpeedMultiplier, out double babyFood, out double totalFood)
        {
            babyFood = 0;
            totalFood = 0;
            if (species != null)
            {
                // maxfoodrate = 2.7, minfoodrate = 0.1
                totalFood = 1.4 * species.breeding.maturationTimeAdjusted;
                // babyfood = 0.1*growing * (2.7 - 0.1*1.3)
                babyFood = 0.1 * species.breeding.maturationTimeAdjusted * 2.57;

                totalFood *= babyFoodConsumptionSpeedMultiplier;
                babyFood *= babyFoodConsumptionSpeedMultiplier;

                // roughly the spoiling
                totalFood *= 1.1;
                babyFood *= 1.1;
                return true;
            }
            return false;
        }

        public static bool foodAmountFromUntil(Species species, double babyFoodConsumptionSpeedMultiplier, double currentMaturation, double untilMaturation, out double totalFood)
        {
            totalFood = 0;
            if (species != null && currentMaturation < untilMaturation && untilMaturation <= 1)
            {
                // maxfoodrate = 2.7, minfoodrate = 0.1
                totalFood = species.breeding.maturationTimeAdjusted * (1.4
                        - currentMaturation * (2.7 - 1.3 * currentMaturation)
                        - (1.4 - untilMaturation * (2.7 - 1.3 * untilMaturation)));

                totalFood *= babyFoodConsumptionSpeedMultiplier;

                // roughly the spoiling
                totalFood *= 1.1;
                return true;
            }
            return false;
        }
    }
}

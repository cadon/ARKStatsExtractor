using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.uiControls
{
    public class Trough
    {
        public Dictionary<string, List<int>> foodStacks;
        public Dictionary<string, int> spoilTimers;
        private int ticks;

        public Trough()
        {
            foodStacks = new Dictionary<string, List<int>>();
            spoilTimers = new Dictionary<string, int>();
        }

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

        public static bool foodAmount(int speciesIndex, double babyFoodConsumptionSpeedMultiplier, out double babyFood, out double totalFood)
        {
            babyFood = 0;
            totalFood = 0;
            if (speciesIndex >= 0)
            {
                // maxfoodrate = 2.7, minfoodrate = 0.1
                totalFood = 1.4 * Values.V.species[speciesIndex].breeding.maturationTimeAdjusted;
                // babyfood = 0.1*growing * (2.7 - 0.1*1.3)
                babyFood = 0.1 * Values.V.species[speciesIndex].breeding.maturationTimeAdjusted * 2.57;

                totalFood *= babyFoodConsumptionSpeedMultiplier;
                babyFood *= babyFoodConsumptionSpeedMultiplier;

                // roughly the spoiling
                totalFood *= 1.1;
                babyFood *= 1.1;
                return true;
            }
            return false;
        }

        public static bool foodAmountFromUntil(int speciesIndex, double babyFoodConsumptionSpeedMultiplier, double currentMaturation, double untilMaturation, out double totalFood)
        {
            totalFood = 0;
            if (speciesIndex >= 0 && currentMaturation < untilMaturation && untilMaturation <= 1)
            {
                // maxfoodrate = 2.7, minfoodrate = 0.1
                totalFood = Values.V.species[speciesIndex].breeding.maturationTimeAdjusted * (1.4
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

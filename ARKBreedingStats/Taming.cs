using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public class Taming
    {
        public static void tamingTimes(int speciesI, int level, List<string> usedFood, List<int> foodAmount, out List<int> foodAmountUsed, out TimeSpan duration, out int neededNarcoberries, out int neededNarcotics, out double te, out bool enoughFood)
        {
            double affinityNeeded = 0, totalTorpor = 0, torporDeplPS = 0, foodAffinity, foodValue, torporNeeded = 0;
            string food;
            int seconds = 0, totalSeconds = 0, foodPiecesNeeded;

            te = 1;
            duration = new TimeSpan(0);
            neededNarcoberries = 0;
            neededNarcotics = 0;
            enoughFood = false;

            foodAmountUsed = new List<int>();
            foreach (int i in foodAmount)
                foodAmountUsed.Add(0);

            if (speciesI >= 0 && speciesI < Values.V.species.Count)
            {
                te = 100;
                TamingData taming = Values.V.species[speciesI].taming;
                // test if(creature is tamend non-violently, then use wakeTame multiplicators
                bool nonViolent = false;
                if (taming.nonViolent)
                    nonViolent = true;

                affinityNeeded = taming.affinityNeeded0 + taming.affinityIncreasePL * level;

                if (!nonViolent)
                {
                    //total torpor for level
                    totalTorpor = Values.V.species[speciesI].stats[7].BaseValue * (1 + Values.V.species[speciesI].stats[7].IncPerWildLevel * (level - 1));
                    // torpor depletion per second for level
                    // here the linear approach of 0.01819 * baseTorporDepletion / level is used.Data shows, it's actual an exponential increase
                    torporDeplPS = taming.torporDepletionPS0 * (1 + 0.01819 * level);
                }

                // how much food / resources of the different kinds that this creature eats is needed
                for (int f = 0; f < usedFood.Count; f++)
                {
                    if (foodAmount[f] > 0)
                    {
                        food = usedFood[f];
                        bool specialFood = (taming.specialFoodValues != null && taming.specialFoodValues.ContainsKey(food));
                        if (specialFood || Values.V.foodData.ContainsKey(food))
                        {
                            foodAffinity = 0;
                            foodValue = 0;
                            // check if (creature handles this food in a special way (e.g. scorpions not liking raw meat as much)
                            if (specialFood)
                            {
                                foodAffinity = taming.specialFoodValues[food].affinity;
                                foodValue = taming.specialFoodValues[food].foodValue;
                            }
                            else
                            {
                                foodAffinity = Values.V.foodData[food].affinity;
                                foodValue = Values.V.foodData[food].foodValue;
                            }

                            if (nonViolent)
                            {
                                // consider wake taming multiplicators (non - violent taming)
                                foodAffinity *= taming.wakeAffinityMult;
                                foodValue = foodValue * taming.wakeFoodDeplMult;
                            }

                            foodAffinity *= Values.V.tamingSpeedMultiplier;

                            if (foodAffinity > 0 && foodValue > 0)
                            {

                                // amount of food needed for the left affinity
                                foodPiecesNeeded = (int)Math.Ceiling(affinityNeeded / foodAffinity);

                                if (foodPiecesNeeded > foodAmount[f])
                                    foodPiecesNeeded = foodAmount[f];

                                foodAmountUsed[f] = foodPiecesNeeded;

                                // time to eat needed food
                                seconds = (int)Math.Ceiling(foodPiecesNeeded * foodValue / (taming.foodConsumptionBase * taming.foodConsumptionMult * Values.V.tamingFoodRateMultiplier));
                                affinityNeeded -= foodPiecesNeeded * foodAffinity;

                                if (te > 0)
                                {
                                    double factor = taming.tamingIneffectiveness / (100 * foodAffinity);
                                    for (int i = 0; i < foodPiecesNeeded; i++)
                                        te -= Math.Pow(te, 2) * factor;
                                }

                                if (!nonViolent)
                                {
                                    //extra needed torpor to eat needed food
                                    torporNeeded += (torporDeplPS * seconds);
                                }
                                totalSeconds += seconds;
                            }
                            if (affinityNeeded <= 0)
                                break;
                        }
                    }
                }
                torporNeeded -= totalTorpor;

                if (torporNeeded < 0)
                    torporNeeded = 0;
                // amount of Narcoberries(give 7.5 torpor each over 3s)
                neededNarcoberries = (int)Math.Ceiling(torporNeeded / (7.5 + 3 * torporDeplPS));
                // amount of Narcotics(give 40 each over 5s)
                neededNarcotics = (int)Math.Ceiling(torporNeeded / (40 + 5 * torporDeplPS));

                enoughFood = affinityNeeded <= 0;

                // needed Time to eat
                duration = new TimeSpan(0, 0, totalSeconds);

                if (te < 0)
                    te = 0;
                te *= .01;

                if (!nonViolent)
                {
                    // print needed tranq arrows needed to ko creature.
                    // Bow - arrow causes 20 dmg, tranqMultiplier are 2 + 2.5 = 4.5 ==> 90 Torpor / arrow.
                    // crossbow 35 dmg * 4.5 ==> 157.5 torpor
                    // slingshot: 14 dmg, stone - tranq - mult: 1.75 ==> 24.5 torpor
                    // wooden club: 10 torpor
                    // longneck dart: 26 * 8.5 = 221
                    // string info = "Wooden Club Hits × " + Math.Ceiling(totalTorpor / 10) + "; Slingshot Hits ×" + Math.Ceiling(totalTorpor / 24.5) + "; Tranquilizer Arrows with a Bow × " + Math.Ceiling(totalTorpor / 90) + "; Tranquilizer Arrows with a Crossbow × " + Math.Ceiling(totalTorpor / 157.5) + "; Tranquilizer Dart Shots × " + Math.Ceiling(totalTorpor / 221);

                    // print torpor depletion per s
                    // string info = "Torpor-depletion: " + Math.Round(torporDeplPS, 2) + " / s; Time until all torpor is depleted: " + (totalTorpor / torporDeplPS) + " s, That is approx.one Narcoberry every " + (Math.Floor(75 / torporDeplPS) / 10 + 3) + " s or one Narcotic every " + (Math.Floor(400 / torporDeplPS) / 10 + 5) + " s";
                }
            }
        }

        public static int foodAmountNeeded(int speciesI, int level, string food, bool nonViolent = false)
        {
            if (speciesI >= 0 && speciesI < Values.V.species.Count)
            {
                TamingData taming = Values.V.species[speciesI].taming;
                double affinityNeeded = taming.affinityNeeded0 + taming.affinityIncreasePL * level;

                bool specialFood = (taming.specialFoodValues != null && taming.specialFoodValues.ContainsKey(food));
                if (specialFood || Values.V.foodData.ContainsKey(food))
                {
                    double foodAffinity = 0;
                    if (specialFood)
                        foodAffinity = taming.specialFoodValues[food].affinity;
                    else
                        foodAffinity = Values.V.foodData[food].affinity;

                    if (nonViolent)
                        foodAffinity *= taming.wakeAffinityMult;

                    foodAffinity *= Values.V.tamingSpeedMultiplier;

                    if (foodAffinity > 0)
                    {
                        // amount of food needed for the affinity
                        return (int)Math.Ceiling(affinityNeeded / foodAffinity);
                    }
                }
            }
            return 0;
        }
    }
}

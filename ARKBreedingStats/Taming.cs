using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public class Taming
    {
        public static void tamingTimes(int speciesI, int level, List<string> usedFood, List<int> foodAmount, out TimeSpan duration, out int neededNarcoberries, out int neededNarcotics, out bool enoughFood)
        {
            double affinityNeeded = 0, totalTorpor = 0, torporDeplPS = 0, foodAffinity, foodValue, wakeAffinityMult = 1, wakeFoodDeplMult = 1, torporNeeded = 0, totalAffinity = 0;
            string food;
            int seconds = 0, totalSeconds = 0;
            enoughFood = false;

            // test if(creature is tamend non-violently, then use wakeTame multiplicators
            bool nonViolent = false;
            if (Values.V.species[speciesI].taming.nonViolent)
                nonViolent = true;
            wakeAffinityMult = Values.V.species[speciesI].taming.wakeAffinityMult;
            wakeFoodDeplMult = Values.V.species[speciesI].taming.wakeFoodDeplMult;

            affinityNeeded = Values.V.species[speciesI].taming.affinityNeeded0 + Values.V.species[speciesI].taming.affinityIncreasePL * level;

            if (!nonViolent)
            {
                //total torpor for level
                totalTorpor = Values.V.species[speciesI].taming.torpor1 + Values.V.species[speciesI].taming.torporIncrease * (level - 1);
                // torpor depletion per second for level
                // here the linear approach of 0.01819 * baseTorporDepletion / level is used.Data shows, it's actual an exponential increase
                torporDeplPS = Values.V.species[speciesI].taming.torporDepletionPS0 * (1 + 0.01819 * level);
            }

            // how much food / resources of the different kinds that this creature eats is needed
            for (int f = 0; f < usedFood.Count; f++)
            {
                if (foodAmount[f] > 0)
                {
                    food = usedFood[f];
                    bool specialFood = (Values.V.species[speciesI].taming.specialFoodValues != null && Values.V.species[speciesI].taming.specialFoodValues.ContainsKey(food));
                    if (specialFood || Values.V.foodData.ContainsKey(food))
                    {
                        foodAffinity = 0;
                        foodValue = 0;
                        // for display (food == "Kibble"?"Kibble (" + Values.V.species[speciesI].taming.favoriteKibble + " Egg)": food) ;
                        // check if (creature handles this food in a special way (e.g. scorpions not liking raw meat as much)
                        if (specialFood)
                        {
                            foodAffinity = Values.V.species[speciesI].taming.specialFoodValues[food].affinity;
                            foodValue = Values.V.species[speciesI].taming.specialFoodValues[food].foodValue;
                        }

                        if (foodAffinity == 0)
                            foodAffinity = Values.V.foodData[food].affinity;
                        if (foodValue == 0)
                            foodValue = Values.V.foodData[food].foodValue;

                        if (foodAffinity > 0 && foodValue > 0)
                        {
                            if (nonViolent)
                            {
                                // consider wake taming multiplicators (non - violent taming)
                                foodAffinity = foodAffinity * wakeAffinityMult;
                                foodValue = foodValue * wakeFoodDeplMult;
                            }

                            // amount of food
                            // foodPiecesNeeded = Math.Ceiling(affinityNeeded / foodAffinity); // TODO remove or use as suggestion?

                            // time to eat needed food
                            seconds = (int)Math.Ceiling(foodAmount[f] * foodValue / (Values.V.species[speciesI].taming.foodConsumptionBase * Values.V.species[speciesI].taming.foodConsumptionMult));
                            totalAffinity += foodAmount[f] * foodAffinity;

                            if (nonViolent)
                            {
                                // feeding intervall (only approximately(mean), exact numbers seem to be more complicated because of inital longer pause)
                                // the last feeded food grants the tame instantly, so subtract one of the needed pieces for the time
                                double feedingInterval = 0;
                                if (foodAmount[f] > 1)
                                    feedingInterval = seconds / (foodAmount[f] - 1);
                            }
                            else
                            {
                                //extra needed torpor to eat needed food
                                torporNeeded += Math.Ceiling(torporDeplPS * seconds - totalTorpor);
                            }
                        }
                        totalSeconds += seconds;
                    }
                }
            }

            if (torporNeeded < 0)
                torporNeeded = 0;
            // amount of Narcoberries(give 7.5 torpor each over 3s)
            neededNarcoberries = (int)Math.Ceiling(torporNeeded / (7.5 + 3 * torporDeplPS));
            // amount of Narcotics(give 40 each over 5s)
            neededNarcotics = (int)Math.Ceiling(torporNeeded / (40 + 5 * torporDeplPS));

            enoughFood = affinityNeeded <= totalAffinity;

            duration = new TimeSpan(0, 0, seconds);

            // needed Time to eat
            // time is stored in seconds

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
}

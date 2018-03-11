using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    static public class Taming
    {
        public static void tamingTimes(int speciesI, int level, double tamingSpeedMultiplier, double tamingFoodRateMultiplier,
            List<string> usedFood, List<int> foodAmount, out List<int> foodAmountUsed, out TimeSpan duration,
            out int neededNarcoberries, out int neededNarcotics, out int neededBioToxines, out double te, out double hunger, out int bonusLevel, out bool enoughFood)
        {
            double affinityNeeded = 0, totalTorpor = 0, torporDeplPS = 0, foodAffinity, foodValue, torporNeeded = 0;
            string food;
            int seconds = 0, totalSeconds = 0, foodPiecesNeeded;

            bonusLevel = 0;
            te = 1;
            duration = TimeSpan.Zero;
            neededNarcoberries = 0;
            neededNarcotics = 0;
            neededBioToxines = 0;
            hunger = 0;
            enoughFood = false;

            foodAmountUsed = new List<int>();
            foreach (int i in foodAmount)
                foodAmountUsed.Add(0);

            if (speciesI >= 0 && speciesI < Values.V.species.Count)
            {
                TamingData taming = Values.V.species[speciesI].taming;
                // test if creature is tamend non-violently, then use wakeTame multiplicators
                bool nonViolent = false;
                if (taming.nonViolent)
                    nonViolent = true;

                affinityNeeded = taming.affinityNeeded0 + taming.affinityIncreasePL * level;

                if (!nonViolent)
                {
                    //total torpor for level
                    totalTorpor = Values.V.species[speciesI].stats[7].BaseValue * (1 + Values.V.species[speciesI].stats[7].IncPerWildLevel * (level - 1));
                    // torpor depletion per second for level
                    torporDeplPS = torporDepletionPS(taming.torporDepletionPS0, level);
                }

                double foodByAffinity = 0; // needed for the effectiveness calculation

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

                            foodAffinity *= specialFood ? taming.specialFoodValues[food].quantity : 1;

                            if (nonViolent)
                            {
                                // consider wake taming multiplicators (non - violent taming)
                                foodAffinity *= taming.wakeAffinityMult;
                                foodValue = foodValue * taming.wakeFoodDeplMult;
                            }

                            foodAffinity *= tamingSpeedMultiplier * 2; // *2 in accordance with the permament 2x taming-bonus that was introduced in the game on 2016-12-12

                            if (foodAffinity > 0 && foodValue > 0)
                            {

                                // amount of food needed for the left affinity.
                                foodPiecesNeeded = (int)Math.Ceiling(affinityNeeded / foodAffinity);

                                if (foodPiecesNeeded > foodAmount[f])
                                    foodPiecesNeeded = foodAmount[f];

                                foodAmountUsed[f] = foodPiecesNeeded;

                                // time to eat needed food
                                seconds = (int)Math.Ceiling(foodPiecesNeeded * foodValue / (taming.foodConsumptionBase * taming.foodConsumptionMult * tamingFoodRateMultiplier));
                                affinityNeeded -= foodPiecesNeeded * foodAffinity;

                                // new approach with 1/(1 + IM*IA*N/AO + ID*D) from https://forums.unrealengine.com/development-discussion/modding/ark-survival-evolved/56959-tutorial-dinosaur-taming-parameters?85457-Tutorial-Dinosaur-Taming-Parameters=
                                foodByAffinity += (foodPiecesNeeded / foodAffinity);

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
                // add tamingIneffectivenessMultiplier? Needs settings?
                te = 1 / (1 + taming.tamingIneffectiveness * foodByAffinity); // ignores damage, which has no input

                torporNeeded -= totalTorpor;

                if (torporNeeded < 0)
                    torporNeeded = 0;
                // amount of Narcoberries(give 7.5 torpor each over 3s)
                neededNarcoberries = (int)Math.Ceiling(torporNeeded / (7.5 + 3 * torporDeplPS));
                // amount of Narcotics(give 40 each over 8s)
                neededNarcotics = (int)Math.Ceiling(torporNeeded / (40 + 8 * torporDeplPS));
                // amount of BioToxines (give 80 each over 16s)
                neededBioToxines = (int)Math.Ceiling(torporNeeded / (80 + 16 * torporDeplPS));

                enoughFood = affinityNeeded <= 0;

                // needed Time to eat
                duration = new TimeSpan(0, 0, totalSeconds);

                if (te < 0) // TODO correct? <0 possible?
                    te = 0;

                bonusLevel = (int)Math.Floor(level * te / 2);

                for (int i = 0; i < usedFood.Count; i++)
                {
                    if (taming.specialFoodValues != null && taming.specialFoodValues.ContainsKey(usedFood[i]))
                        hunger += foodAmountUsed[i] * taming.specialFoodValues[usedFood[i]].foodValue;
                    else if (Values.V.foodData.ContainsKey(usedFood[i]))
                        hunger += foodAmountUsed[i] * Values.V.foodData[usedFood[i]].foodValue;
                }
            }
        }


        /// <summary>
        /// Use this function if only one kind of food is fed
        /// </summary>
        public static void tamingTimes(int speciesI, int level, double tamingSpeedMultiplier, double tamingFoodRateMultiplier, string usedFood, int foodAmount, out List<int> foodAmountUsed, out TimeSpan duration, out int neededNarcoberries, out int neededNarcotics, out int neededBioToxines, out double te, out double hunger, out int bonusLevel, out bool enoughFood)
        {
            tamingTimes(speciesI, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, new List<string> { usedFood }, new List<int> { foodAmount }, out foodAmountUsed, out duration, out neededNarcoberries, out neededNarcotics, out neededBioToxines, out te, out hunger, out bonusLevel, out enoughFood);
        }

        public static int foodAmountNeeded(int speciesI, int level, double tamingSpeedMultiplier, string food, bool nonViolent = false)
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

                    foodAffinity *= tamingSpeedMultiplier * 2; // *2 in accordance with the permament 2x taming-bonus that was introduced in the game on 2016-12-12

                    if (foodAffinity > 0)
                    {
                        // amount of food needed for the affinity
                        return (int)Math.Ceiling(affinityNeeded / (foodAffinity * (specialFood ? taming.specialFoodValues[food].quantity : 1)));
                    }
                }
            }
            return 0;
        }

        public static int secondsUntilWakingUp(int speciesI, int level, double currentTorpor)
        {
            int seconds = 0;
            if (speciesI >= 0 && speciesI < Values.V.species.Count && Values.V.species[speciesI].taming.torporDepletionPS0 > 0)
            {
                // torpor depletion per second for level
                // here the linear approach of 0.01819 * baseTorporDepletion / level is used. Data shows, it's actual an exponential increase
                seconds = (int)Math.Floor(currentTorpor / torporDepletionPS(Values.V.species[speciesI].taming.torporDepletionPS0, level));
            }
            return seconds;
        }

        private static double torporDepletionPS(double torporDepletionPS0, int level)
        {
            // torpor depletion per second for level

            // here the linear approach of 0.01819 * baseTorporDepletion / level is used. Data shows, it's actual an exponential increase
            // torporDeplPS = taming.torporDepletionPS0 * (1 + 0.01819 * level);

            // using a more precise approach with an exponential increase, based on http://ark.crumplecorn.com/taming/controller.js?d=20160821
            if (torporDepletionPS0 > 0)
                return torporDepletionPS0 + Math.Pow(level - 1, 0.800403041) / (22.39671632 / torporDepletionPS0);
            return 0;
        }

        public static TimeSpan tamingDuration(int speciesI, int foodQuantity, string food, double tamingFoodRateMultiplier, bool nonViolent = false)
        {
            double foodValue = 0;
            var taming = Values.V.species[speciesI].taming;
            bool specialFood = (taming.specialFoodValues != null && taming.specialFoodValues.ContainsKey(food));
            // check if (creature handles this food in a special way (e.g. scorpions not liking raw meat as much)
            if (specialFood)
                foodValue = taming.specialFoodValues[food].foodValue;
            else
                foodValue = Values.V.foodData[food].foodValue;

            if (nonViolent)
                foodValue = foodValue * taming.wakeFoodDeplMult;

            // time to eat needed food
            return new TimeSpan(0, 0, (int)Math.Ceiling(foodQuantity * foodValue / (taming.foodConsumptionBase * taming.foodConsumptionMult * tamingFoodRateMultiplier)));
        }

        public static string knockoutInfo(int speciesIndex, int level, double longneck, double crossbow, double bow, double slingshot, double club, double prod, double boneDamageAdjuster, out bool knockoutNeeded, out string koNumbers)
        {
            koNumbers = "";
            knockoutNeeded = false;
            if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count)
            {
                //total torpor for level
                double totalTorpor = Values.V.species[speciesIndex].stats[7].BaseValue * (1 + Values.V.species[speciesIndex].stats[7].IncPerWildLevel * (level - 1));
                // torpor depletion per second for level
                double torporDeplPS = torporDepletionPS(Values.V.species[speciesIndex].taming.torporDepletionPS0, level);

                knockoutNeeded = Values.V.species[speciesIndex].taming.violent;
                string warning = "";
                if (!knockoutNeeded)
                    warning = "+++ Creature must not be knocked out for taming! +++\n\n";

                // print needed tranq arrows needed to ko creature.
                // wooden club: 10 torpor
                // slingshot: 14 dmg, stone - tranq - mult: 1.75 ==> 24.5 torpor
                // Bow - arrow causes 20 dmg, tranqMultiplier are 2 + 2.5 = 4.5 ==> 90 Torpor / arrow.
                // crossbow 35 dmg * 4.5 ==> 157.5 torpor
                // longneck dart: 26 * 8.5 = 221
                // shocking tranq dart: 26*17 = 442
                // electric prod: 226

                koNumbers = (longneck > 0 ? Math.Ceiling(totalTorpor / (442 * boneDamageAdjuster * longneck)) + " × Shocking Tranq Darts\n" : "")
                     + (longneck > 0 ? Math.Ceiling(totalTorpor / (221 * boneDamageAdjuster * longneck)) + " × Tranquilizer Darts\n" : "")
                     + (prod > 0 ? Math.Ceiling(totalTorpor / (226 * boneDamageAdjuster * prod)) + " × Electric Prod Hits\n" : "")
                     + (crossbow > 0 ? Math.Ceiling(totalTorpor / (157.5 * boneDamageAdjuster * crossbow)) + " × Tranquilizer Arrows (Crossbow)\n" : "")
                     + (bow > 0 ? Math.Ceiling(totalTorpor / (90 * boneDamageAdjuster * bow)) + " × Tranquilizer Arrows (Bow)\n" : "")
                     + (slingshot > 0 ? Math.Ceiling(totalTorpor / (24.5 * boneDamageAdjuster * slingshot)) + " × Slingshot Hits\n" : "")
                     + (club > 0 ? Math.Ceiling(totalTorpor / (10 * boneDamageAdjuster * club)) + " × Wooden Club Hits\n" : "");

                // torpor depletion per s
                string torporDepletion = "";
                if (torporDeplPS > 0)
                    torporDepletion = "\nTime until max torpor is depleted: " + Utils.durationUntil(new TimeSpan(0, 0, (int)Math.Round(totalTorpor / torporDeplPS)))
                         + "\nTorpor-depletion: " + Math.Round(torporDeplPS, 2)
                         + " / s;\nThat is approx. one Narcoberry every " + Math.Round(7.5 / torporDeplPS + 3, 1)
                         + " s or one Narcotic every " + Math.Round(40 / torporDeplPS + 8, 1)
                         + " s or one Bio Toxin every " + Math.Round(80 / torporDeplPS + 16, 1) + " s";

                return warning + koNumbers + torporDepletion;
            }
            return "";
        }


        public static string quickInfoOneFood(int speciesIndex, int level, double tamingSpeedMultiplier, double tamingFoodRateMultiplier, string foodName, int foodAmount, string foodDisplayName)
        {
            List<int> foodAmountUsed;
            bool enoughFood;
            TimeSpan duration;
            int narcoBerries, narcotics, bioToxines, bonusLevel;
            double te, hunger;
            tamingTimes(speciesIndex, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, foodName, foodAmount, out foodAmountUsed, out duration, out narcoBerries, out narcotics, out bioToxines, out te, out hunger, out bonusLevel, out enoughFood);
            return "With " + foodAmountUsed[0] + " × " + foodDisplayName + " taming takes " + Utils.durationUntil(duration)
                + "\nNarcotics: " + narcotics
                + "\nTE: " + Math.Round(100 * te, 1) + " %"
                + $"\nBonus-Level: +{bonusLevel} (⇒ {(level + bonusLevel)})"
                + $"\nFood has to drop by {hunger:F1} units.";
        }

        public static string boneDamageAdjustersImmobilization(int speciesIndex, out Dictionary<double, string> boneDamageAdjusters)
        {
            string text = "";
            boneDamageAdjusters = new Dictionary<double, string>();
            if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count)
            {
                if (Values.V.species[speciesIndex].boneDamageAdjusters != null)
                {
                    boneDamageAdjusters = Values.V.species[speciesIndex].boneDamageAdjusters;
                    foreach (KeyValuePair<double, string> bd in Values.V.species[speciesIndex].boneDamageAdjusters)
                    {
                        text += (text.Length > 0 ? "\n" : "") + bd.Value + ": × " + bd.Key;
                    }
                }
                if (Values.V.species[speciesIndex].immobilizedBy != null && Values.V.species[speciesIndex].immobilizedBy.Count > 0)
                    text += (text.Length > 0 ? "\n" : "") + "Immobilized by: " + string.Join(", ", Values.V.species[speciesIndex].immobilizedBy);
            }
            return text;
        }

        public static int durationAfterFirstFeeding(int speciesIndex, int level, double foodDepletion)
        {
            int s = 0;
            if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count && Values.V.species[speciesIndex].taming != null && Values.V.species[speciesIndex].taming.nonViolent)
            {
                s = (int)(0.1 * Stats.calculateValue(speciesIndex, 3, (int)Math.Ceiling(level / 7d), 0, false, 0, 0) / foodDepletion);
            }
            return s;
        }
    }
}

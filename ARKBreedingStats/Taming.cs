using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats
{
    public static class Taming
    {
        public static void TamingTimes(Species species, int level, double tamingSpeedMultiplier, double tamingFoodRateMultiplier,
                List<string> usedFood, List<int> foodAmount, out List<int> foodAmountUsed, out TimeSpan duration,
                out int neededNarcoberries, out int neededAscerbicMushrooms, out int neededNarcotics, out int neededBioToxines, out double te, out double hunger, out int bonusLevel, out bool enoughFood)
        {
            double totalTorpor = 0, torporDeplPS = 0, torporNeeded = 0;
            int totalSeconds = 0;

            bonusLevel = 0;
            te = 1;
            duration = TimeSpan.Zero;
            neededNarcoberries = 0;
            neededAscerbicMushrooms = 0;
            neededNarcotics = 0;
            neededBioToxines = 0;
            hunger = 0;
            enoughFood = false;

            foodAmountUsed = new List<int>();
            foreach (int i in foodAmount)
                foodAmountUsed.Add(0);

            if (species != null && species.taming != null)
            {

                double affinityNeeded = species.taming.affinityNeeded0 + species.taming.affinityIncreasePL * level;

                // test if creature is tamend non-violently, then use wakeTame multiplicators
                if (!species.taming.nonViolent)
                {
                    //total torpor for level
                    totalTorpor = species.stats[(int)StatNames.Torpidity].BaseValue * (1 + species.stats[(int)StatNames.Torpidity].IncPerWildLevel * (level - 1));
                    // torpor depletion per second for level
                    torporDeplPS = TorporDepletionPS(species.taming.torporDepletionPS0, level);
                }

                double foodByAffinity = 0; // needed for the effectiveness calculation

                // how much food / resources of the different kinds that this creature eats is needed
                for (int f = 0; f < usedFood.Count; f++)
                {
                    if (foodAmount[f] > 0)
                    {
                        string food = usedFood[f];
                        bool specialFood = species.taming.specialFoodValues != null && species.taming.specialFoodValues.ContainsKey(food);
                        if (specialFood || Values.V.defaultFoodData.ContainsKey(food))
                        {
                            double foodAffinity;
                            double foodValue;

                            // check if (creature handles this food in a special way (e.g. scorpions not liking raw meat as much)
                            if (specialFood)
                            {
                                foodAffinity = species.taming.specialFoodValues[food].affinity;
                                foodValue = species.taming.specialFoodValues[food].foodValue;
                            }
                            else
                            {
                                foodAffinity = Values.V.defaultFoodData[food].affinity;
                                foodValue = Values.V.defaultFoodData[food].foodValue;
                            }

                            foodAffinity *= specialFood ? species.taming.specialFoodValues[food].quantity : 1;

                            if (species.taming.nonViolent)
                            {
                                // consider wake taming multiplicators (non - violent taming)
                                foodAffinity *= species.taming.wakeAffinityMult;
                                foodValue = foodValue * species.taming.wakeFoodDeplMult;
                            }

                            // *2 in accordance with the hardcoded 2x taming-bonus that was introduced in the game in patch 253.0 on 2016-12-23
                            // https://ark.gamepedia.com/253.0
                            foodAffinity *= tamingSpeedMultiplier * 2;

                            if (foodAffinity > 0 && foodValue > 0)
                            {
                                // amount of food needed for the left affinity.
                                int foodPiecesNeeded = (int)Math.Ceiling(affinityNeeded / foodAffinity);

                                if (foodPiecesNeeded > foodAmount[f])
                                    foodPiecesNeeded = foodAmount[f];

                                foodAmountUsed[f] = foodPiecesNeeded;

                                // time to eat needed food
                                // mantis eats every 3 minutes, regardless of level
                                int seconds;
                                if (species.name == "Mantis")
                                    seconds = foodPiecesNeeded * 180;
                                else
                                    seconds = (int)Math.Ceiling(foodPiecesNeeded * foodValue / (species.taming.foodConsumptionBase * species.taming.foodConsumptionMult * tamingFoodRateMultiplier));
                                affinityNeeded -= foodPiecesNeeded * foodAffinity;

                                // new approach with 1/(1 + IM*IA*N/AO + ID*D) from https://forums.unrealengine.com/development-discussion/modding/ark-survival-evolved/56959-tutorial-dinosaur-taming-parameters?85457-Tutorial-Dinosaur-Taming-Parameters=
                                foodByAffinity += foodPiecesNeeded / foodAffinity;

                                if (!species.taming.nonViolent)
                                {
                                    //extra needed torpor to eat needed food
                                    torporNeeded += torporDeplPS * seconds;
                                }
                                totalSeconds += seconds;
                            }
                            if (affinityNeeded <= 0)
                                break;
                        }
                    }
                }
                // add tamingIneffectivenessMultiplier? Needs settings?
                te = 1 / (1 + species.taming.tamingIneffectiveness * foodByAffinity); // ignores damage, which has no input
                if (te < 0)
                    te = 0;

                torporNeeded -= totalTorpor;

                if (torporNeeded < 0)
                    torporNeeded = 0;
                // amount of Narcoberries(give 7.5 torpor each over 3s)
                neededNarcoberries = (int)Math.Ceiling(torporNeeded / (7.5 + 3 * torporDeplPS));
                // amount of Ascerbic Mushrooms (give 25 torpor each over 3s)
                neededAscerbicMushrooms = (int)Math.Ceiling(torporNeeded / (25 + 3 * torporDeplPS));
                // amount of Narcotics(give 40 each over 8s)
                neededNarcotics = (int)Math.Ceiling(torporNeeded / (40 + 8 * torporDeplPS));
                // amount of BioToxines (give 80 each over 16s)
                neededBioToxines = (int)Math.Ceiling(torporNeeded / (80 + 16 * torporDeplPS));

                enoughFood = affinityNeeded <= 0;

                // needed Time to eat
                duration = new TimeSpan(0, 0, totalSeconds);

                bonusLevel = (int)Math.Floor(level * te / 2);

                for (int i = 0; i < usedFood.Count; i++)
                {
                    if (species.taming.specialFoodValues != null && species.taming.specialFoodValues.ContainsKey(usedFood[i]))
                        hunger += foodAmountUsed[i] * species.taming.specialFoodValues[usedFood[i]].foodValue;
                    else if (Values.V.defaultFoodData.ContainsKey(usedFood[i]))
                        hunger += foodAmountUsed[i] * Values.V.defaultFoodData[usedFood[i]].foodValue;
                }
            }
        }

        /// <summary>
        /// Use this function if only one kind of food is fed
        /// </summary>
        public static void TamingTimes(Species species, int level, double tamingSpeedMultiplier, double tamingFoodRateMultiplier,
                string usedFood, int foodAmount,
                out List<int> foodAmountUsed, out TimeSpan duration, out int neededNarcoberries, out int neededAscerbicMushrooms, out int neededNarcotics,
                out int neededBioToxines, out double te, out double hunger, out int bonusLevel, out bool enoughFood)
        {
            TamingTimes(species, level, tamingSpeedMultiplier, tamingFoodRateMultiplier,
                    new List<string> { usedFood }, new List<int> { foodAmount },
                    out foodAmountUsed, out duration, out neededNarcoberries, out neededAscerbicMushrooms, out neededNarcotics, out neededBioToxines,
                    out te, out hunger, out bonusLevel, out enoughFood);
        }

        public static int FoodAmountNeeded(Species species, int level, double tamingSpeedMultiplier, string foodName, bool nonViolent = false)
        {
            if (species != null)
            {
                double affinityNeeded = species.taming.affinityNeeded0 + species.taming.affinityIncreasePL * level;

                bool specialFood = species.taming.specialFoodValues != null && species.taming.specialFoodValues.ContainsKey(foodName);

                if (!specialFood && !Values.V.defaultFoodData.ContainsKey(foodName))
                    return 0;

                TamingFood food = specialFood ? species.taming.specialFoodValues[foodName] : Values.V.defaultFoodData[foodName];

                double foodAffinity;
                foodAffinity = food.affinity;

                if (nonViolent)
                    foodAffinity *= species.taming.wakeAffinityMult;

                foodAffinity *= tamingSpeedMultiplier * 2; // *2 in accordance with the permament 2x taming-bonus that was introduced in the game on 2016-12-12

                if (foodAffinity > 0)
                {
                    // amount of food needed for the affinity
                    int quantity = food.quantity;
                    if (quantity < 1) quantity = 1;
                    return (int)Math.Ceiling(affinityNeeded / (foodAffinity * quantity));
                }
            }
            return 0;
        }

        public static int SecondsUntilWakingUp(Species species, int level, double currentTorpor)
        {
            int seconds = 0;
            if (species != null && species.taming.torporDepletionPS0 > 0)
            {
                // torpor depletion per second for level
                // here the linear approach of 0.01819 * baseTorporDepletion / level is used. Data shows, it's actual an exponential increase
                seconds = (int)Math.Floor(currentTorpor / TorporDepletionPS(species.taming.torporDepletionPS0, level));
            }
            return seconds;
        }

        private static double TorporDepletionPS(double torporDepletionPS0, int level)
        {
            // torpor depletion per second for level

            // here the linear approach of 0.01819 * baseTorporDepletion / level is used. Data shows, it's actual an exponential increase
            // torporDeplPS = taming.torporDepletionPS0 * (1 + 0.01819 * level);

            // using a more precise approach with an exponential increase, based on http://ark.crumplecorn.com/taming/controller.js?d=20160821
            if (torporDepletionPS0 > 0)
                return torporDepletionPS0 + Math.Pow(level - 1, 0.800403041) / (22.39671632 / torporDepletionPS0);
            return 0;
        }

        public static TimeSpan TamingDuration(Species species, int foodQuantity, string food, double tamingFoodRateMultiplier, bool nonViolent = false)
        {
            // time to eat needed food
            int seconds = 0;
            if (species != null && species.taming != null)
            {
                // check if (creature handles this food in a special way (e.g. scorpions don't like raw meat as much as most others)
                bool specialFood = species.taming.specialFoodValues != null && species.taming.specialFoodValues.ContainsKey(food);

                // if no info for the food exists, return 0
                if (!specialFood && !Values.V.defaultFoodData.ContainsKey(food))
                    return new TimeSpan();

                double foodValue = specialFood ? species.taming.specialFoodValues[food].foodValue : Values.V.defaultFoodData[food].foodValue;

                if (nonViolent)
                    foodValue = foodValue * species.taming.wakeFoodDeplMult;

                // mantis eats every 3 minutes, regardless of level
                if (species.name == "Mantis")
                    seconds = foodQuantity * 180;
                else
                    seconds = (int)Math.Ceiling(foodQuantity * foodValue / (species.taming.foodConsumptionBase * species.taming.foodConsumptionMult * tamingFoodRateMultiplier));
            }
            return new TimeSpan(0, 0, seconds);
        }

        public static string KnockoutInfo(Species species, int level, double longneck, double crossbow, double bow, double slingshot,
                double club, double prod, double harpoon, double boneDamageAdjuster, out bool knockoutNeeded, out string koNumbers)
        {
            koNumbers = string.Empty;
            knockoutNeeded = false;
            if (species?.taming != null)
            {
                //total torpor for level
                double totalTorpor = species.stats[(int)StatNames.Torpidity].BaseValue * (1 + species.stats[(int)StatNames.Torpidity].IncPerWildLevel * (level - 1));
                // torpor depletion per second for level
                double torporDeplPS = TorporDepletionPS(species.taming.torporDepletionPS0, level);

                knockoutNeeded = species.taming.violent;
                string warning = string.Empty;
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

                koNumbers = (harpoon > 0 ? Math.Ceiling(totalTorpor / (306 * boneDamageAdjuster * harpoon)) + " × " + Loc.S("TranqSpearBolts") + "\n" : string.Empty)
                        + (longneck > 0 ? Math.Ceiling(totalTorpor / (442 * boneDamageAdjuster * longneck)) + " × " + Loc.S("ShockingTranqDarts") + "\n" : string.Empty)
                        + (longneck > 0 ? Math.Ceiling(totalTorpor / (221 * boneDamageAdjuster * longneck)) + " × " + Loc.S("TranqDarts") + "\n" : string.Empty)
                        + (prod > 0 ? Math.Ceiling(totalTorpor / (226 * boneDamageAdjuster * prod)) + " × " + Loc.S("ElectricProdHits") + "\n" : string.Empty)
                        + (crossbow > 0 ? Math.Ceiling(totalTorpor / (157.5 * boneDamageAdjuster * crossbow)) + " × " + Loc.S("TranqArrowsCrossBow") + "\n" : string.Empty)
                        + (bow > 0 ? Math.Ceiling(totalTorpor / (90 * boneDamageAdjuster * bow)) + " × " + Loc.S("TranqArrowsBow") + "\n" : string.Empty)
                        + (slingshot > 0 ? Math.Ceiling(totalTorpor / (24.5 * boneDamageAdjuster * slingshot)) + " × " + Loc.S("SlingshotHits") + "\n" : string.Empty)
                        + (club > 0 ? Math.Ceiling(totalTorpor / (10 * boneDamageAdjuster * club)) + " × " + Loc.S("WoodenClubHits") + "\n" : string.Empty);

                // torpor depletion per s
                string torporDepletion = string.Empty;
                if (torporDeplPS > 0)
                    torporDepletion = "\n" + Loc.S("TimeUntilTorporDepleted") + ": " + Utils.DurationUntil(new TimeSpan(0, 0, (int)Math.Round(totalTorpor / torporDeplPS)))
                            + "\n" + Loc.S("TorporDepletion") + ": " + Math.Round(torporDeplPS, 2)
                            + " / s;\n" + Loc.S("ApproxOneNarcoberryEvery") + " " + Math.Round(7.5 / torporDeplPS + 3, 1)
                            + " s " + Loc.S("OrOneAscerbicMushroom") + " " + Math.Round(25 / torporDeplPS + 3, 1)
                            + " s " + Loc.S("OrOneNarcoticEvery") + " " + Math.Round(40 / torporDeplPS + 8, 1)
                            + " s " + Loc.S("OrOneBioToxinEvery") + " " + Math.Round(80 / torporDeplPS + 16, 1) + " s";

                return warning + koNumbers + torporDepletion;
            }
            return string.Empty;
        }

        public static string QuickInfoOneFood(Species species, int level, double tamingSpeedMultiplier, double tamingFoodRateMultiplier,
                string foodName, int foodAmount, string foodDisplayName)
        {
            TamingTimes(species, level, tamingSpeedMultiplier, tamingFoodRateMultiplier, foodName, foodAmount,
                    out List<int> foodAmountUsed, out TimeSpan duration, out _, out _, out int narcotics, out _, out double te,
                    out double hunger, out int bonusLevel, out _);
            return $"{string.Format(Loc.S("WithXFoodTamingTakesTime"), foodAmountUsed[0], foodDisplayName, Utils.DurationUntil(duration))}\n" +
                    $"{Loc.S("Narcotics")}: {narcotics}\n" +
                    $"{Loc.S("TamingEffectiveness_Abb")}: {Math.Round(100 * te, 1)} %\n" +
                    $"{Loc.S("BonusLevel")}: +{(level + bonusLevel)}\n" +
                    $"{string.Format(Loc.S("FoodHasToDropUnits"), Math.Round(hunger, 1))}";
        }

        public static string BoneDamageAdjustersImmobilization(Species species, out Dictionary<string, double> boneDamageAdjusters)
        {
            string text = string.Empty;
            boneDamageAdjusters = new Dictionary<string, double>();
            if (species != null)
            {
                if (species.boneDamageAdjusters != null)
                {
                    boneDamageAdjusters = species.boneDamageAdjusters;
                    foreach (KeyValuePair<string, double> bd in boneDamageAdjusters)
                    {
                        text += (text.Length > 0 ? "\n" : string.Empty) + bd.Key + ": × " + bd.Value.ToString();
                    }
                }
                if (species.immobilizedBy != null && species.immobilizedBy.Any())
                    text += $"{(text.Length > 0 ? "\n" : string.Empty)}{Loc.S("ImmobilizedBy")}: " +
                            $"{string.Join(", ", species.immobilizedBy)}";
            }
            return text;
        }

        public static int DurationAfterFirstFeeding(Species species, int level, double foodDepletion)
        {
            int s = 0;
            if (species != null &&
                species.taming != null &&
                species.taming.nonViolent)
            {
                s = (int)(0.1 * StatValueCalculation.CalculateValue(species, (int)StatNames.Food, (int)Math.Ceiling(level / 7d), 0, false, 0, 0) / foodDepletion);
            }
            return s;
        }
    }
}

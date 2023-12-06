using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARKBreedingStats
{
    public static class Taming
    {
        /// <summary>
        /// *2 in accordance with the hardcoded 2x taming-bonus that was introduced in the game in patch 253.0 on 2016-12-23
        /// https://ark.fandom.com/253.0
        /// and again *2 in patch https://ark.fandom.com/313.5
        /// </summary>
        private const int HardCodedTamingMultiplier = 4;

        public static void TamingTimes(Species species, int level, ServerMultipliers serverMultipliers,
                List<string> usedFood, List<int> foodAmount, out List<int> foodAmountUsed, out TimeSpan duration,
                out int neededNarcoberries, out int neededAscerbicMushrooms, out int neededNarcotics, out int neededBioToxins, out double te, out double hunger, out int bonusLevel, out bool enoughFood, bool useSanguineElixir = false)
        {
            double totalTorpor = 0;
            double torporDepletionPerSecond = 0;
            double torporNeeded = 0;
            int totalSeconds = 0;

            bonusLevel = 0;
            te = 1;
            duration = TimeSpan.Zero;
            neededNarcoberries = 0;
            neededAscerbicMushrooms = 0;
            neededNarcotics = 0;
            neededBioToxins = 0;
            hunger = 0;
            enoughFood = false;

            foodAmountUsed = new List<int>();
            foreach (int i in foodAmount)
                foodAmountUsed.Add(0);

            if (species != null && species.taming != null)
            {

                double affinityNeeded = (species.taming.affinityNeeded0 + species.taming.affinityIncreasePL * level) * (useSanguineElixir ? 0.7 : 1);

                // test if creature is tamed non-violently, then use wakeTame multipliers
                if (!species.taming.nonViolent)
                {
                    //total torpor for level
                    totalTorpor = species.stats[Stats.Torpidity].BaseValue * (1 + species.stats[Stats.Torpidity].IncPerWildLevel * (level - 1));
                    // torpor depletion per second for level
                    torporDepletionPerSecond = TorporDepletionPerSecond(species.taming.torporDepletionPS0, level, serverMultipliers.WildDinoTorporDrainMultiplier);
                }

                double foodByAffinity = 0; // needed for the effectiveness calculation

                // how much food / resources of the different kinds that this creature eats is needed
                for (int f = 0; f < usedFood.Count; f++)
                {
                    if (foodAmount[f] <= 0) continue;
                    string foodName = usedFood[f];
                    var food = Values.V.GetTamingFood(species, foodName);
                    if (food == null) continue;


                    double foodAffinity = food.affinity * food.quantity;
                    double foodValue = food.foodValue; // TODO is the food value also affected by the quantity?

                    if (species.taming.nonViolent)
                    {
                        // consider wake taming multiplicators (non - violent taming)
                        foodAffinity *= species.taming.wakeAffinityMult;
                        foodValue = foodValue * species.taming.wakeFoodDeplMult;
                    }

                    foodAffinity *= serverMultipliers.TamingSpeedMultiplier * HardCodedTamingMultiplier;

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
                            seconds = (int)Math.Ceiling(foodPiecesNeeded * foodValue / (species.taming.foodConsumptionBase * species.taming.foodConsumptionMult * serverMultipliers.DinoCharacterFoodDrainMultiplier));
                        affinityNeeded -= foodPiecesNeeded * foodAffinity;

                        // new approach with 1/(1 + IM*IA*N/AO + ID*D) from https://forums.unrealengine.com/development-discussion/modding/ark-survival-evolved/56959-tutorial-dinosaur-taming-parameters?85457-Tutorial-Dinosaur-Taming-Parameters=
                        foodByAffinity += foodPiecesNeeded / foodAffinity;

                        if (!species.taming.nonViolent)
                        {
                            //extra needed torpor to eat needed food
                            torporNeeded += torporDepletionPerSecond * seconds;
                        }
                        totalSeconds += seconds;
                    }
                    if (affinityNeeded <= 0)
                        break;
                }
                // add tamingIneffectivenessMultiplier? Needs settings?
                te = 1 / (1 + species.taming.tamingIneffectiveness * foodByAffinity); // ignores damage, which has no input
                if (te < 0)
                    te = 0;

                torporNeeded -= totalTorpor;

                if (torporNeeded < 0)
                    torporNeeded = 0;
                // amount of Narcoberries(give 7.5 torpor each over 3s)
                neededNarcoberries = (int)Math.Ceiling(torporNeeded / (7.5 + 3 * torporDepletionPerSecond));
                // amount of Ascerbic Mushrooms (give 25 torpor each over 3s)
                neededAscerbicMushrooms = (int)Math.Ceiling(torporNeeded / (25 + 3 * torporDepletionPerSecond));
                // amount of Narcotics(give 40 each over 8s)
                neededNarcotics = (int)Math.Ceiling(torporNeeded / (40 + 8 * torporDepletionPerSecond));
                // amount of BioToxines (give 80 each over 16s)
                neededBioToxins = (int)Math.Ceiling(torporNeeded / (80 + 16 * torporDepletionPerSecond));

                enoughFood = affinityNeeded <= 0;

                // needed Time to eat
                duration = new TimeSpan(0, 0, totalSeconds);

                bonusLevel = (int)Math.Floor(level * te / 2);

                for (int i = 0; i < usedFood.Count; i++)
                {
                    var food = Values.V.GetTamingFood(species, usedFood[i]);
                    if (food != null)
                        hunger += foodAmountUsed[i] * food.foodValue;
                }
            }
        }

        /// <summary>
        /// Use this function if only one kind of food is fed
        /// </summary>
        public static void TamingTimes(Species species, int level, ServerMultipliers serverMultipliers,
                string usedFood, int foodAmount,
                out List<int> foodAmountUsed, out TimeSpan duration, out int neededNarcoberries, out int neededAscerbicMushrooms, out int neededNarcotics,
                out int neededBioToxins, out double te, out double hunger, out int bonusLevel, out bool enoughFood, bool useSanguineElixir = false)
        {
            TamingTimes(species, level, serverMultipliers,
                    new List<string> { usedFood }, new List<int> { foodAmount },
                    out foodAmountUsed, out duration, out neededNarcoberries, out neededAscerbicMushrooms, out neededNarcotics, out neededBioToxins,
                    out te, out hunger, out bonusLevel, out enoughFood, useSanguineElixir);
        }

        public static int FoodAmountNeeded(Species species, int level, double tamingSpeedMultiplier, string foodName, bool nonViolent = false, bool useSanguineElixir = false)
        {
            if (species != null)
            {
                double affinityNeeded = (species.taming.affinityNeeded0 + species.taming.affinityIncreasePL * level) * (useSanguineElixir ? 0.7 : 1);

                var food = Values.V.GetTamingFood(species, foodName);
                if (food == null) return 0;

                var foodAffinity = food.affinity;

                if (nonViolent)
                    foodAffinity *= species.taming.wakeAffinityMult;

                foodAffinity *= tamingSpeedMultiplier * HardCodedTamingMultiplier;

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

        public static int SecondsUntilWakingUp(Species species, ServerMultipliers serverMultipliers, int level, double currentTorpor)
        {
            int seconds = 0;
            if (species != null && species.taming.torporDepletionPS0 > 0)
            {
                // torpor depletion per second for level
                // here the linear approach of 0.01819 * baseTorporDepletion / level is used. Data shows, it's actual an exponential increase
                seconds = (int)Math.Floor(currentTorpor / TorporDepletionPerSecond(species.taming.torporDepletionPS0, level, serverMultipliers.WildDinoTorporDrainMultiplier));
            }
            return seconds;
        }

        private static double TorporDepletionPerSecond(double torporDepletionPS0, int level, double wildDinoTorporDrainMultiplier)
        {
            // torpor depletion per second for level

            // here the linear approach of 0.01819 * baseTorporDepletion / level is used. Data shows, it's actual an exponential increase
            // torporDeplPS = taming.torporDepletionPS0 * (1 + 0.01819 * level);

            // using a more precise approach with an exponential increase, based on http://ark.crumplecorn.com/taming/controller.js?d=20160821
            if (torporDepletionPS0 > 0)
                return (torporDepletionPS0 + Math.Pow(level - 1, 0.800403041) / (22.39671632 / torporDepletionPS0)) * wildDinoTorporDrainMultiplier;
            return 0;
        }

        public static TimeSpan TamingDuration(Species species, int foodQuantity, string foodName, double tamingFoodRateMultiplier, bool nonViolent = false)
        {
            if (species?.taming == null) return TimeSpan.Zero;

            // calculate time to eat needed food
            var food = Values.V.GetTamingFood(species, foodName);
            if (food == null) return TimeSpan.Zero;

            double foodValue = food.foodValue;

            if (nonViolent)
                foodValue *= species.taming.wakeFoodDeplMult;

            int seconds;
            // mantis eats every 3 minutes, regardless of level
            if (species.name == "Mantis")
                seconds = foodQuantity * 180;
            else
                seconds = (int)Math.Ceiling(foodQuantity * foodValue / (species.taming.foodConsumptionBase * species.taming.foodConsumptionMult * tamingFoodRateMultiplier));

            return new TimeSpan(0, 0, seconds);
        }

        public static string KnockoutInfo(Species species, ServerMultipliers serverMultipliers, int level, double longneck, double crossbow, double bow, double slingshot,
                double club, double prod, double harpoon, double boneDamageAdjuster, out bool knockoutNeeded, out string koNumbers)
        {
            koNumbers = string.Empty;
            knockoutNeeded = false;
            if (species?.taming != null)
            {
                //total torpor for level
                double totalTorpor = species.stats[Stats.Torpidity].BaseValue * (1 + species.stats[Stats.Torpidity].IncPerWildLevel * (level - 1));
                // torpor depletion per second for level
                double torporDeplPS = TorporDepletionPerSecond(species.taming.torporDepletionPS0, level, serverMultipliers.WildDinoTorporDrainMultiplier);

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

        public static string QuickInfoOneFood(Species species, int level, ServerMultipliers serverMultipliers,
                string foodName, int foodAmount, string foodDisplayName)
        {
            TamingTimes(species, level, serverMultipliers, foodName, foodAmount,
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
                        text += (text.Length > 0 ? "\n" : string.Empty) + bd.Key + ": × " + bd.Value;
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
                s = (int)(0.1 * StatValueCalculation.CalculateValue(species, Stats.Food, (int)Math.Ceiling(level / 7d), 0, 0, false, 0, 0) / foodDepletion);
            }
            return s;
        }
    }
}

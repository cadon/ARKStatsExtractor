using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ARKBreedingStats.importExported
{
    static class ImportExported
    {
        public static CreatureValues ImportExportedCreature(string filePath)
        {
            CreatureValues cv = new CreatureValues
            {
                domesticatedAt = File.GetLastWriteTime(filePath),
                isTamed = true,
                tamingEffMax = 1,
                tamingEffMin = Properties.Settings.Default.ImportLowerBoundTE
            };
            string[] iniLines = File.ReadAllLines(filePath);
            string id = null;
            int statIndex = -1;
            // this is the order how the stats appear in the ini-file; field names in the file are localized and cannot be used directly
            string[] statParameterNames =
            {
                "Health",
                "Stamina",
                "Torpidity",
                "Oxygen",
                "Food",
                "Water",
                "Temperature",
                "Weight",
                "Melee Damage",
                "Movement Speed",
                "Fortitude",
                "Crafting Skill"
            };

            const NumberStyles numberStyle = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
            var dotSeparatorCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            bool inStatSection = false;
            foreach (string line in iniLines)
            {
                if (line.Contains("[Max Character Status Values]"))
                {
                    inStatSection = true;
                    continue;
                }

                int i = line.IndexOf("=", StringComparison.Ordinal);
                if (i == -1) continue;

                string parameterName;
                string text = line.Substring(i + 1);
                double.TryParse(text, numberStyle, dotSeparatorCulture, out double value);
                if (inStatSection)
                {
                    statIndex++;
                    if (statIndex > 11)
                        inStatSection = false;
                }
                if (inStatSection)
                    parameterName = statParameterNames[statIndex];
                else
                {
                    parameterName = line.Substring(0, i);
                    if (parameterName.Contains("DinoAncestorsMale"))
                        parameterName = "DinoAncestorsMale"; // only the last entry contains the parents
                }

                if (string.IsNullOrEmpty(parameterName))
                    continue;

                switch (parameterName)
                {
                    case "DinoID1":
                        if (string.IsNullOrEmpty(id))
                        {
                            id = text;
                        }
                        else
                        {
                            cv.ARKID = BuildArkId(text, id);
                            cv.guid = Utils.ConvertArkIdToGuid(cv.ARKID);
                        }
                        break;
                    case "DinoID2":
                        if (string.IsNullOrEmpty(id))
                        {
                            id = text;
                        }
                        else
                        {
                            cv.ARKID = BuildArkId(id, text);
                            cv.guid = Utils.ConvertArkIdToGuid(cv.ARKID);
                        }
                        break;
                    case "DinoClass":
                        // despite the property is called DinoClass it contains the complete blueprint-path
                        cv.Species = Values.V.SpeciesByBlueprint(text,true);
                        if (cv.Species == null)
                            cv.speciesBlueprint = text; // species is unknown, check the needed mods later
                        break;
                    //case "DinoNameTag":
                    //    // get name if blueprintpath is not available (in this case a custom values_mod.json should be created, this is just a fallback
                    //    if (cv.Species == null &&
                    //        Values.V.TryGetSpeciesByName(text, out Species species))
                    //    {
                    //        cv.Species = species;
                    //    }
                    //    break;
                    case "bIsFemale":
                        cv.sex = text == "True" ? Sex.Female : Sex.Male;
                        break;
                    case "bNeutered":
                        if (text != "False")
                            cv.flags |= CreatureFlags.Neutered;
                        break;
                    case "TamerString":
                        if (Properties.Settings.Default.ImportExportUseTamerStringForOwner)
                            cv.owner = text;
                        else
                            cv.tribe = text;
                        break;
                    case "TamedName":
                        cv.name = text;
                        break;
                    case "ImprinterName":
                        cv.imprinterName = text;
                        if (string.IsNullOrEmpty(cv.owner))
                            cv.owner = text;
                        if (!string.IsNullOrWhiteSpace(text))
                            cv.isBred = true;
                        break;
                    case "RandomMutationsMale":
                        cv.mutationCounterFather = (int)value;
                        break;
                    case "RandomMutationsFemale":
                        cv.mutationCounterMother = (int)value;
                        break;
                    case "BabyAge":
                        if (cv.Species?.breeding != null)
                            cv.growingUntil = DateTime.Now.AddSeconds((int)(cv.Species.breeding.maturationTimeAdjusted * (1 - value)));
                        break;
                    case "CharacterLevel":
                        cv.level = (int)value;
                        break;
                    case "DinoImprintingQuality":
                        cv.imprintingBonus = value;
                        if (value > 0) cv.isBred = true;
                        break;
                    // Colorization
                    case "ColorSet[0]":
                        cv.colorIDs[0] = ParseColorId(text);
                        break;
                    case "ColorSet[1]":
                        cv.colorIDs[1] = ParseColorId(text);
                        break;
                    case "ColorSet[2]":
                        cv.colorIDs[2] = ParseColorId(text);
                        break;
                    case "ColorSet[3]":
                        cv.colorIDs[3] = ParseColorId(text);
                        break;
                    case "ColorSet[4]":
                        cv.colorIDs[4] = ParseColorId(text);
                        break;
                    case "ColorSet[5]":
                        cv.colorIDs[5] = ParseColorId(text);
                        break;
                    case "Health":
                        cv.statValues[Stats.Health] = value;
                        break;
                    case "Stamina":
                        cv.statValues[Stats.Stamina] = value;
                        break;
                    case "Torpidity":
                        cv.statValues[Stats.Torpidity] = value;
                        break;
                    case "Oxygen":
                        cv.statValues[Stats.Oxygen] = value;
                        break;
                    case "Food":
                        cv.statValues[Stats.Food] = value;
                        break;
                    case "Water":
                        cv.statValues[Stats.Water] = value;
                        break;
                    case "Temperature":
                        cv.statValues[Stats.Temperature] = value;
                        break;
                    case "Weight":
                        cv.statValues[Stats.Weight] = value;
                        break;
                    case "Melee Damage":
                        cv.statValues[Stats.MeleeDamageMultiplier] = 1 + value;
                        break;
                    case "Movement Speed":
                        cv.statValues[Stats.SpeedMultiplier] = 1 + value;
                        break;
                    case "Fortitude":
                        cv.statValues[Stats.TemperatureFortitude] = 1 + value;
                        break;
                    case "Crafting Skill":
                        cv.statValues[Stats.CraftingSpeedMultiplier] = 1 + value;
                        break;
                    case "DinoAncestorsMale":
                        Regex r = new Regex(@"MaleName=([^;]+);MaleDinoID1=([^;]+);MaleDinoID2=([^;]+);FemaleName=([^;]+);FemaleDinoID1=([^;]+);FemaleDinoID2=([^;]+)");
                        Match m = r.Match(text);
                        if (m.Success)
                        {
                            cv.motherArkId = BuildArkId(m.Groups[5].Value, m.Groups[6].Value);
                            cv.fatherArkId = BuildArkId(m.Groups[2].Value, m.Groups[3].Value);
                            cv.isBred = true;
                        }
                        break;
                }
            }

            // if file was not recognized, return null
            if (string.IsNullOrEmpty(cv.speciesBlueprint)) return null;

            cv.ColorIdsAlsoPossible = ArkColors.GetAlternativeColorIds(cv.colorIDs);

            return cv;
        }

        /// <summary>
        /// Determines the ARK color id represented by the given text in the format
        /// (R=0.000000,G=0.000000,B=0.000000,A=1.000000)
        /// </summary>
        private static byte ParseColorId(string text)
        {
            if (text.Length < 33) return 0;

            var numberStyle = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
            var dotSeparatorCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            if (double.TryParse(text.Substring(3, 8), numberStyle, dotSeparatorCulture, out double r)
                && double.TryParse(text.Substring(14, 8), numberStyle, dotSeparatorCulture, out double g)
                && double.TryParse(text.Substring(25, 8), numberStyle, dotSeparatorCulture, out double b)
                && double.TryParse(text.Substring(36, 8), numberStyle, dotSeparatorCulture, out double a)
               )
            {
                if (r == 0 && g == 0 && b == 0 && a == 1) // no color
                    return 0;
                if (r == 1 && g == 1 && b == 1 && a == 1) // undefined color
                    return Ark.UndefinedColorId;

                return Values.V.Colors.ClosestColorId(r, g, b, a);
            }

            // color is invisible or parsing failed
            return 0;
        }

        /// <summary>
        /// Returns the true ARK-Id from two strings in a long.
        /// ARK just concatenates the strings ingame, resulting in non unique displayed IDs.
        /// </summary>
        private static long BuildArkId(string id1, string id2)
        {
            if (int.TryParse(id1, out int id1Int)
                && int.TryParse(id2, out int id2Int))
                return Utils.ConvertArkIdsToLongArkId(id1Int, id2Int);
            return 0;
        }
    }
}

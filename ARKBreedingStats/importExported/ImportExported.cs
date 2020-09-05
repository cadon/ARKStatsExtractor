using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
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

            var numberStyle = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
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
                        if (text.Length > 2 && text.Substring(text.Length - 2) == "_C")
                            text = text.Substring(0, text.Length - 2); // the last two characters are "_C"

                        cv.Species = Values.V.SpeciesByBlueprint(text);
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
                    case "bIsNeutered":
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
                        cv.colorIDs[0] = ParseColor(text);
                        break;
                    case "ColorSet[1]":
                        cv.colorIDs[1] = ParseColor(text);
                        break;
                    case "ColorSet[2]":
                        cv.colorIDs[2] = ParseColor(text);
                        break;
                    case "ColorSet[3]":
                        cv.colorIDs[3] = ParseColor(text);
                        break;
                    case "ColorSet[4]":
                        cv.colorIDs[4] = ParseColor(text);
                        break;
                    case "ColorSet[5]":
                        cv.colorIDs[5] = ParseColor(text);
                        break;
                    case "Health":
                        cv.statValues[(int)StatNames.Health] = value;
                        break;
                    case "Stamina":
                        cv.statValues[(int)StatNames.Stamina] = value;
                        break;
                    case "Torpidity":
                        cv.statValues[(int)StatNames.Torpidity] = value;
                        break;
                    case "Oxygen":
                        cv.statValues[(int)StatNames.Oxygen] = value;
                        break;
                    case "Food":
                        cv.statValues[(int)StatNames.Food] = value;
                        break;
                    case "Water":
                        cv.statValues[(int)StatNames.Water] = value;
                        break;
                    case "Temperature":
                        cv.statValues[(int)StatNames.Temperature] = value;
                        break;
                    case "Weight":
                        cv.statValues[(int)StatNames.Weight] = value;
                        break;
                    case "Melee Damage":
                        cv.statValues[(int)StatNames.MeleeDamageMultiplier] = 1 + value;
                        break;
                    case "Movement Speed":
                        cv.statValues[(int)StatNames.SpeedMultiplier] = 1 + value;
                        break;
                    case "Fortitude":
                        cv.statValues[(int)StatNames.TemperatureFortitude] = 1 + value;
                        break;
                    case "Crafting Skill":
                        cv.statValues[(int)StatNames.CraftingSpeedMultiplier] = 1 + value;
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

            // if parent ArkIds are set, create creature placeholder
            if (cv.motherArkId != 0)
            {
                cv.Mother = new Creature(cv.motherArkId)
                {
                    Species = cv.Species
                };
            }
            if (cv.fatherArkId != 0)
            {
                cv.Father = new Creature(cv.fatherArkId)
                {
                    Species = cv.Species
                };
            }

            // if file was not recognized, return null
            if (string.IsNullOrEmpty(cv.speciesBlueprint)) return null;

            return cv;
        }

        /// <summary>
        /// Determines the ARK color id represented by the given text in the format
        /// (R=0.000000,G=0.000000,B=0.000000,A=1.000000)
        /// </summary>
        private static int ParseColor(string text)
        {
            if (text.Length < 33) return 0;

            var numberStyle = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
            var dotSeparatorCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            if (double.TryParse(text.Substring(3, 8), numberStyle, dotSeparatorCulture, out double r)
                && double.TryParse(text.Substring(14, 8), numberStyle, dotSeparatorCulture, out double g)
                && double.TryParse(text.Substring(25, 8), numberStyle, dotSeparatorCulture, out double b)
                && double.TryParse(text.Substring(36, 8), numberStyle, dotSeparatorCulture, out double a)
                && !(r == 0 && g == 0 && b == 0 && a == 1) // no color
                && !(r == 1 && g == 1 && b == 1 && a == 1) // no color
                )
                return Values.V.Colors.ClosestColorId(r, g, b, a);

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
                return ((long)id1Int << 32) | (id2Int & 0xFFFFFFFFL);
            return 0;
        }
    }
}

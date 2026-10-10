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
        /// <summary>
        /// Reads export file created by the game.
        /// </summary>
        public static CreatureValues ReadExportedCreature(string filePath)
        {
            CreatureValues cv = new CreatureValues
            {
                domesticatedAt = File.GetLastWriteTime(filePath),
                isTamed = true,
                tamingEffMax = 1,
                tamingEffMin = Properties.Settings.Default.ImportLowerBoundTE
            };
            var iniLines = File.ReadAllLines(filePath);
            string id = null;
            var statIndex = -1;

            const NumberStyles numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent;
            var dotSeparatorCulture = CultureInfo.GetCultureInfo("en-US");

            var inStatSection = false;
            const string statSectionLabel = "StatSection";
            foreach (string line in iniLines)
            {
                if (line.TrimStart().StartsWith(";")) continue; // comment
                if (line.Contains("[Max Character Status Values]"))
                {
                    inStatSection = true;
                    continue;
                }

                var i = line.IndexOf("=", StringComparison.Ordinal);
                if (i == -1) continue;

                string parameterName;
                var text = line[(i + 1)..];
                double.TryParse(text, numberStyle, dotSeparatorCulture, out var value);
                if (inStatSection)
                {
                    statIndex++;
                    if (statIndex >= Stats.StatsCount)
                        inStatSection = false;
                }

                if (inStatSection)
                    parameterName = statSectionLabel;
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
                        cv.Species = Values.V.SpeciesByBlueprint(text, true);
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
                        {
                            cv.growingUntil = DateTime.Now.AddSeconds((int)(cv.Species.breeding.maturationTimeAdjusted * (1 - value)));
                            if (value < 1) cv.isBred = true;
                        }
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
                    case statSectionLabel:
                        cv.statValues[statIndex] = value + (Stats.IsPercentage(statIndex) ? 1 : 0);
                        break;
                    case "DinoAncestorsMale":
                        Regex r = new Regex(@"MaleName=([^;]+) - Lvl \d+;MaleDinoID1=([^;]+);MaleDinoID2=([^;]+);FemaleName=([^;]+) - Lvl \d+;FemaleDinoID1=([^;]+);FemaleDinoID2=([^;]+)");
                        Match m = r.Match(text);
                        if (m.Success)
                        {
                            cv.motherArkId = BuildArkId(m.Groups[5].Value, m.Groups[6].Value);
                            cv.fatherArkId = BuildArkId(m.Groups[2].Value, m.Groups[3].Value);
                            cv.motherName = m.Groups[4].Value;
                            cv.fatherName = m.Groups[1].Value;
                            cv.isBred = true;
                        }
                        break;
                }
            }

            // if file was not recognized, return null
            if (string.IsNullOrEmpty(cv.speciesBlueprint)) return null;

            if (cv.Species?.NoGender == true) cv.sex = Sex.Unknown;
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

            const NumberStyles numberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
            var dotSeparatorCulture = CultureInfo.GetCultureInfo("en-US");
            if (double.TryParse(text.AsSpan(3, 8), numberStyle, dotSeparatorCulture, out var r)
                && double.TryParse(text.AsSpan(14, 8), numberStyle, dotSeparatorCulture, out var g)
                && double.TryParse(text.AsSpan(25, 8), numberStyle, dotSeparatorCulture, out var b)
                && double.TryParse(text.AsSpan(36, 8), numberStyle, dotSeparatorCulture, out var a)
               )
            {
                if (r == 0 && g == 0 && b == 0 && a == 1) // no color
                    return 0;
                if (r == 1 && g == 1 && b == 1 && a == 1)
                {
                    // in ASE and ASA this is the undefined color. In ASA it's also the white coloring.
                    // return undefined id for ASE, use color matching for ASA
                    // this will result in the white coloring, then the undefined color is added as alternative possible color
                    if (Ark.UndefinedColorId == Ark.UndefinedColorIdAse)
                        return Ark.UndefinedColorId;
                }

                return Values.V.Colors.ClosestColorId(r, g, b, a);
            }

            // color is invisible or parsing failed
            return 0;
        }

        /// <summary>
        /// Returns the true ARK-Id from two strings in a long.
        /// ARK just concatenates the strings in game, resulting in non-unique displayed IDs.
        /// </summary>
        private static long BuildArkId(string id1, string id2)
        {
            if (int.TryParse(id1, out var id1Int)
                && int.TryParse(id2, out var id2Int))
                return Utils.ConvertArkIdsToLongArkId(id1Int, id2Int);
            return 0;
        }
    }
}

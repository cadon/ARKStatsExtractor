﻿using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ARKBreedingStats.importExported
{
    static class ImportExported
    {
        public static CreatureValues importExportedCreature(string filePath)
        {
            CreatureValues cv = new CreatureValues
            {
                domesticatedAt = File.GetLastWriteTime(filePath),
                isTamed = true,
                tamingEffMax = 1
            };
            string[] iniLines = File.ReadAllLines(filePath);
            string id = "";
            int statIndexIngame = -1;
            // this is the order how the stats appear in the ini-file; field names in the file are localized
            string[] statIndices =
            {
                    "Health",
                    "Stamina",
                    "Torpidity",
                    "Oxygen",
                    "Food",
                    "Water" /*ignored*/,
                    "Temperature" /*ignored*/,
                    "Weight",
                    "Melee Damage",
                    "Movement Speed",
                    "Fortitude" /*ignored*/,
                    "Crafting Skill" /*ignored*/
            };
            bool inStatSection = false;
            foreach (string line in iniLines)
            {
                if (line.Contains("="))
                {
                    string parameterName;
                    int i = line.IndexOf("=");
                    string text = line.Substring(i + 1);
                    double.TryParse(text, System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out double value);
                    if (inStatSection)
                    {
                        statIndexIngame++;
                        if (statIndexIngame > 11)
                            inStatSection = false;
                    }
                    if (inStatSection)
                        parameterName = statIndices[statIndexIngame];
                    else
                    {
                        parameterName = line.Substring(0, i);
                        if (parameterName.Contains("DinoAncestorsMale"))
                            parameterName = "DinoAncestorsMale"; // only the last entry contains the parents
                    }

                    if (parameterName.Length <= 0)
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
                                cv.ARKID = buildARKID(text, id);
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
                                cv.ARKID = buildARKID(id, text);
                                cv.guid = Utils.ConvertArkIdToGuid(cv.ARKID);
                            }
                            break;
                        case "DinoClass":
                            if (text.Length > 2 && text.Substring(text.Length - 2) == "_C")
                                text = text.Substring(0, text.Length - 2); // the last two characters are "_C"

                            cv.Species = Values.V.speciesByBlueprint(text);
                            break;
                        case "DinoNameTag":
                            // get name if blueprintpath is not available (in this case a custom values_mod.json should be created, this is just a fallback
                            if (cv.Species == null &&
                                Values.V.TryGetSpeciesByName(text, out Species species))
                            {
                                cv.Species = species;
                            }
                            break;
                        case "bIsFemale":
                            cv.sex = text == "True" ? Sex.Female : Sex.Male;
                            break;
                        case "bIsNeutered":
                            cv.neutered = text != "False";
                            break;
                        case "TamerString":
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
                                cv.isBred = true; // TODO is this a correct assumption?
                            break;
                        // todo mutations for mother and father
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
                            cv.colorIDs[0] = parseColor(text);
                            break;
                        case "ColorSet[1]":
                            cv.colorIDs[1] = parseColor(text);
                            break;
                        case "ColorSet[2]":
                            cv.colorIDs[2] = parseColor(text);
                            break;
                        case "ColorSet[3]":
                            cv.colorIDs[3] = parseColor(text);
                            break;
                        case "ColorSet[4]":
                            cv.colorIDs[4] = parseColor(text);
                            break;
                        case "ColorSet[5]":
                            cv.colorIDs[5] = parseColor(text);
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
                                cv.motherArkId = buildARKID(m.Groups[5].Value, m.Groups[6].Value);
                                cv.fatherArkId = buildARKID(m.Groups[2].Value, m.Groups[3].Value);
                                cv.isBred = true;
                            }
                            break;
                    }
                }
                else if (line.Contains("[Max Character Status Values]"))
                {
                    inStatSection = true;
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
            return cv;
        }

        private static int parseColor(string text)
        {
            if (text.Length < 33) return 0;
            double.TryParse(text.Substring(3, 8), System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out double r);
            double.TryParse(text.Substring(14, 8), System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out double g);
            double.TryParse(text.Substring(25, 8), System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out double b);
            if (r == 0 && g == 0 && b == 0) return 0;
            return CreatureColors.closestColorIDFromRGB(LinearColorComponentToColorComponent(r),
                    LinearColorComponentToColorComponent(g),
                    LinearColorComponentToColorComponent(b));
        }

        // this is a definition from the unreal engine
        private static int LinearColorComponentToColorComponent(double lc)
        {
            return (int)(255.999f * (lc <= 0.0031308f ? lc * 12.92f : Math.Pow(lc, 1.0f / 2.4f) * 1.055f - 0.055f));
        }

        private static long buildARKID(string id1, string id2)
        {
            int.TryParse(id1, out int id1int);
            int.TryParse(id2, out int id2int);
            return ((long)id1int << 32) | (id2int & 0xFFFFFFFFL);
        }
    }
}

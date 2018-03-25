using ARKBreedingStats.species;
using System;
using System.IO;

namespace ARKBreedingStats
{
    static class ImportExported
    {
        static public CreatureValues importExportedCreature(string filePath)
        {
            CreatureValues cv = new CreatureValues();
            cv.domesticatedAt = File.GetLastWriteTime(filePath);
            cv.isTamed = true;
            cv.tamingEffMax = 100;
            string[] iniLines = File.ReadAllLines(filePath);
            string id = "";
            foreach (string line in iniLines)
            {
                if (line.Contains("="))
                {
                    int i = line.IndexOf("=");
                    string text = line.Substring(i + 1);
                    double.TryParse(text, out double value);
                    switch (line.Substring(0, i))
                    {
                        // todo ID1, ID2
                        case "DinoID1":
                            if (string.IsNullOrEmpty(id))
                            {
                                id = text;
                            }
                            else
                            {
                                cv.guid = builtGuid(text, id);
                            }
                            break;
                        case "DinoID2":
                            if (string.IsNullOrEmpty(id))
                            {
                                id = text;
                            }
                            else
                            {
                                cv.guid = builtGuid(id, text);
                            }
                            break;
                        case "DinoNameTag":
                            cv.species = text;
                            break;
                        case "bIsFemale":
                            cv.gender = (text == "True" ? Sex.Female : Sex.Male);
                            break;
                        case "bIsNeutered":
                            cv.neutered = (text == "False" ? false : true);
                            break;
                        case "TamerString":
                            cv.owner = text;
                            break;
                        case "TamedName":
                            cv.name = text;
                            break;
                        case "ImprinterName":
                            cv.imprinterName = text;
                            if (string.IsNullOrEmpty(cv.owner))
                                cv.owner = text;
                            break;
                        // todo mutations for mother and father
                        case "RandomMutationsMale":
                            break;
                        case "RandomMutationsFemale":
                            break;
                        // todo age
                        case "BabyAge":
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
                            cv.statValues[0] = value;
                            break;
                        case "Stamina":
                            cv.statValues[1] = value;
                            break;
                        case "Torpidity":
                            cv.statValues[7] = value;
                            break;
                        case "Oxygen":
                            cv.statValues[2] = value;
                            break;
                        case "food":
                            cv.statValues[3] = value;
                            break;
                        case "Weight":
                            cv.statValues[4] = value;
                            break;
                        case "Melee Damage":
                            cv.statValues[5] = 1 + value;
                            break;
                        case "Movement Speed":
                            cv.statValues[6] = 1 + value;
                            break;
                    }
                }
            }
            return cv;
        }

        private static int parseColor(string text)
        {
            double.TryParse(text.Substring(3, 8), out double r);
            double.TryParse(text.Substring(14, 8), out double g);
            double.TryParse(text.Substring(25, 8), out double b);
            return CreatureColors.closestColorIDFromRGB(LinearColorComponentToColorComponent(r),
                                                        LinearColorComponentToColorComponent(g),
                                                        LinearColorComponentToColorComponent(b));
        }

        private static int LinearColorComponentToColorComponent(double lc)
        {
            return (int)(255.999f * (lc <= 0.0031308f ? lc * 12.92f : Math.Pow(lc, 1.0f / 2.4f) * 1.055f - 0.055f));
        }

        private static Guid builtGuid(string id1, string id2)
        {
            int.TryParse(id1, out int id1int);
            int.TryParse(id2, out int id2int);
            return Utils.ConvertIdToGuid((((long)id1int) << 32) | (id2int & 0xFFFFFFFFL));
        }

    }
}

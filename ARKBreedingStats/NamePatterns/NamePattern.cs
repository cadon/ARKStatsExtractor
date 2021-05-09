using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats.NamePatterns
{
    public static class NamePattern
    {
        /// <summary>
        /// The pipe character is used as separator in functions, so it needs to be escaped when used literally.
        /// </summary>
        private const string PipeEscapeSequence = @"\pipe";

        /// <summary>
        /// Generate a creature name with the naming pattern.
        /// </summary>
        public static string GenerateCreatureName(Creature creature, Creature[] sameSpecies, int[] speciesTopLevels, int[] speciesLowestLevels, Dictionary<string, string> customReplacings, bool showDuplicateNameWarning, int namingPatternIndex, bool showTooLongWarning = true, string pattern = null, bool displayError = true, Dictionary<string, string> tokenDictionary = null)
        {
            var creatureNames = sameSpecies?.Where(c => c.guid != creature.guid).Select(x => x.name).ToArray() ?? new string[0];
            if (pattern == null)
            {
                if (namingPatternIndex == -1)
                    pattern = string.Empty;
                else
                    pattern = Properties.Settings.Default.NamingPatterns?[namingPatternIndex] ?? string.Empty;
            }

            if (creature.topness == 0)
            {
                if (speciesTopLevels == null)
                {
                    creature.topness = 1000;
                }
                else
                {
                    int topLevelSum = 0;
                    int creatureLevelSum = 0;
                    for (int s = 0; s < Values.STATS_COUNT; s++)
                    {
                        if (s != (int)StatNames.Torpidity
                            && creature.Species.UsesStat(s)
                            && (Properties.Settings.Default.consideredStats & (1 << s)) != 0
                            )
                        {
                            int creatureLevel = Math.Max(0, creature.levelsWild[s]);
                            topLevelSum += Math.Max(creatureLevel, speciesTopLevels[s]);
                            creatureLevelSum += creatureLevel;
                        }
                    }
                    if (topLevelSum != 0)
                        creature.topness = (short)(creatureLevelSum * 1000f / topLevelSum);
                    else creature.topness = 1000;
                }

                if (tokenDictionary != null)
                    tokenDictionary["topPercent"] = (creature.topness / 10f).ToString();
            }

            if (tokenDictionary == null)
                tokenDictionary = CreateTokenDictionary(creature, sameSpecies, speciesTopLevels, speciesLowestLevels);
            // first resolve keys, then functions
            string name = ResolveFunctions(
                ResolveKeysToValues(tokenDictionary, pattern.Replace("\r", string.Empty).Replace("\n", string.Empty)),
                creature, customReplacings, displayError, false);

            if (name.Contains("{n}"))
            {
                // replace the unique number key with the lowest possible positive number >= 1 to get a unique name.
                string numberedUniqueName;
                string lastNumberedUniqueName = null;

                int n = 1;
                do
                {
                    numberedUniqueName = ResolveFunctions(
                        ResolveKeysToValues(tokenDictionary, name, n++),
                        creature, customReplacings, displayError, true);

                    // check if numberedUniqueName actually is different, else break the potentially infinite loop. E.g. it is not different if {n} is an unreached if branch or was altered with other functions
                    if (numberedUniqueName == lastNumberedUniqueName) break;

                    lastNumberedUniqueName = numberedUniqueName;
                } while (creatureNames.Contains(numberedUniqueName, StringComparer.OrdinalIgnoreCase));
                name = numberedUniqueName;
            }

            // evaluate escaped characters
            name = name.Replace(PipeEscapeSequence, "|");

            if (showDuplicateNameWarning && creatureNames.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show($"The generated name for the creature\n{name}\nalready exists in the library.\n\nConsider adding {{n}} or {{sn}} in the pattern to generate unique names.", "Name already exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (showTooLongWarning && name.Length > 24)
            {
                MessageBox.Show("The generated name is longer than 24 characters, the name will look like this in game:\n" + name.Substring(0, 24), "Name too long for game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return name;
        }

        /// <summary>
        /// Resolves functions in the pattern.
        /// A function expression looks like {{#function_name:{xxx}|2|3}}, e.g. {{#substring:{HP}|2|3}}
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="creature"></param>
        /// <param name="customReplacings">Dictionary of user defined replacings</param>
        /// <param name="displayError">If true, a MessageBox with the error will be displayed.</param>
        /// <param name="processNumberField">If true, the {n} will be processed</param>
        /// <returns></returns>
        private static string ResolveFunctions(string pattern, Creature creature, Dictionary<string, string> customReplacings, bool displayError, bool processNumberField)
        {
            int nrFunctions = 0;
            int nrFunctionsAfterResolving = NrFunctions(pattern);
            // the second and third parameter are optional
            Regex r = new Regex(@"\{\{ *#(\w+) *: *([^\|\{\}]*?) *(?:\| *([^\|\{\}]*?) *)?(?:\| *([^\|\{\}]*?) *)?\}\}", RegexOptions.IgnoreCase);
            var parameters = new NamePatternParameters
            {
                Creature = creature,
                CustomReplacings = customReplacings,
                DisplayError = displayError,
                ProcessNumberField = processNumberField
            };
            // resolve nested functions
            while (nrFunctions != nrFunctionsAfterResolving)
            {
                nrFunctions = nrFunctionsAfterResolving;
                pattern = r.Replace(pattern, (m) => ResolveFunction(m, parameters));
                nrFunctionsAfterResolving = NrFunctions(pattern);
            }
            return pattern;

            int NrFunctions(string p)
            {
                int nr = 0;
                foreach (char c in p) if (c == '#') nr++;
                return nr;
            }
        }

        /// <summary>
        /// Resolves the naming-pattern functions
        /// </summary>
        /// <returns></returns>
        private static string ResolveFunction(Match m, NamePatternParameters parameters)
        {
            // function parameters can be non numeric if numbers are parsed
            try
            {
                if (!parameters.ProcessNumberField && m.Groups[2].Value.Contains("{n}")) return m.Groups[0].Value;

                return NamePatternFunctions.ResolveFunction(m, parameters);
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, $"The syntax of the following pattern function\n{m.Groups[0].Value}\ncannot be processed and will be ignored.", "Naming pattern function error");
            }
            return string.Empty;
        }

        private static readonly string[] StatAbbreviationFromIndex = {
            "hp", // (int)StatNames.Health;
            "st", // (int)StatNames.Stamina;
            "to", // (int)StatNames.Torpidity;
            "ox", // (int)StatNames.Oxygen;
            "fo", // (int)StatNames.Food;
            "wa", // (int)StatNames.Water;
            "te", // (int)StatNames.Temperature;
            "we", // (int)StatNames.Weight;
            "dm", // (int)StatNames.MeleeDamageMultiplier;
            "sp", // (int)StatNames.SpeedMultiplier;
            "fr", // (int)StatNames.TemperatureFortitude;
            "cr"  // (int)StatNames.CraftingSpeedMultiplier;
        };


        /// <summary>        
        /// This method creates the token dictionary for the dynamic creature name generation.
        /// </summary>
        /// <param name="creature">Creature with the data</param>
        /// <param name="speciesCreatures">A list of all currently stored creatures of the species</param>
        /// <param name="speciesTopLevels">top levels of that species</param>
        /// <param name="speciesLowestLevels">lowest levels of that species</param>
        /// <returns>A dictionary containing all tokens and their replacements</returns>
        public static Dictionary<string, string> CreateTokenDictionary(Creature creature, Creature[] speciesCreatures, int[] speciesTopLevels, int[] speciesLowestLevels)
        {
            var creatureInLibrary = creature.guid != Guid.Empty ? speciesCreatures.FirstOrDefault(c => c.guid == creature.guid) : null;

            string dom = creature.isBred ? "B" : "T";

            double imp = creature.imprintingBonus * 100;
            double eff = creature.tamingEff * 100;

            Random rand = new Random(DateTime.Now.Millisecond);
            string randStr = rand.Next(0, 999999).ToString("000000");

            string effImp = "Z";
            string prefix = string.Empty;
            if (creature.isBred)
            {
                prefix = "I";
                effImp = Math.Round(imp).ToString();
            }
            else if (eff > 1)
            {
                prefix = "E";
                effImp = Math.Round(eff).ToString();
            }

            while (effImp.Length < 3 && effImp != "Z")
            {
                effImp = "0" + effImp;
            }

            string effImpShort = effImp;
            effImp = prefix + effImp;

            int generation = creature.generation;
            if (generation <= 0)
                generation = Math.Max(
                    creature.Mother?.generation + 1 ?? 0,
                    creature.Father?.generation + 1 ?? 0
                );

            int mutasn = creature.Mutations;
            string mutas = mutasn > 99 ? "99" : mutasn.ToString();

            string oldName = creature.name;

            string firstWordOfOldest = string.Empty;
            if (speciesCreatures?.Any() ?? false)
            {
                firstWordOfOldest = speciesCreatures.Where(c => c.addedToLibrary != null && !c.flags.HasFlag(CreatureFlags.Placeholder)).OrderBy(c => c.addedToLibrary).FirstOrDefault()?.name;
                if (!string.IsNullOrEmpty(firstWordOfOldest) && firstWordOfOldest.Contains(" "))
                {
                    firstWordOfOldest = firstWordOfOldest.Substring(0, firstWordOfOldest.IndexOf(" "));
                }

                if (creature.guid != Guid.Empty)
                {
                    oldName = creatureInLibrary?.name ?? creature.name;
                }
                else if (creature.ArkId != 0)
                {
                    oldName = speciesCreatures.FirstOrDefault(c => c.ArkId == creature.ArkId)?.name ?? creature.name;
                }
            }
            // escape special characters
            oldName = oldName.Replace("|", PipeEscapeSequence);

            string spcsNm = creature.Species.name;
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            while (spcsNm.LastIndexOfAny(vowels) > 0)
                spcsNm = spcsNm.Remove(spcsNm.LastIndexOfAny(vowels), 1); // remove last vowel (not the first letter)

            // for counting, add 1 if the creature is not yet in the library
            var addOne = creatureInLibrary == null ? 1 : 0;
            int speciesCount = (speciesCreatures?.Length ?? 0) + addOne;
            // the index of the creature in its generation, ordered by addedToLibrary
            int nrInGeneration = (speciesCreatures?.Count(c => c.guid != creature.guid && c.addedToLibrary != null && c.generation == generation && (creature.addedToLibrary == null || c.addedToLibrary < creature.addedToLibrary)) ?? 0) + addOne;
            int nrInGenerationAndSameSex = (speciesCreatures?.Count(c => c.guid != creature.guid && c.sex == creature.sex && c.addedToLibrary != null && c.generation == generation && (creature.addedToLibrary == null || c.addedToLibrary < creature.addedToLibrary)) ?? 0) + addOne;
            int speciesSexCount = (speciesCreatures?.Count(c => c.guid != creature.guid && c.sex == creature.sex) ?? 0) + addOne;

            string arkid = string.Empty;
            if (creature.ArkId != 0)
            {
                if (creature.ArkIdImported)
                {
                    arkid = Utils.ConvertImportedArkIdToIngameVisualization(creature.ArkId);
                }
                else
                {
                    arkid = creature.ArkId.ToString();
                }
            }

            string indexStr = string.Empty;
            if (creature.guid != Guid.Empty && (speciesCreatures?.Any() ?? false))
            {
                for (int i = 0; i < speciesCreatures.Length; i++)
                {
                    if (creature.guid == speciesCreatures[i].guid)
                    {
                        indexStr = (i + 1).ToString();
                        break;
                    }
                }
            }

            // stat index and according level
            var levelOrder = new List<Tuple<int, int>>(Values.STATS_COUNT);
            for (int si = 0; si < Values.STATS_COUNT; si++)
            {
                if (si != (int)StatNames.Torpidity && creature.Species.UsesStat(si))
                    levelOrder.Add(new Tuple<int, int>(si, creature.levelsWild[si]));
            }
            levelOrder = levelOrder.OrderByDescending(l => l.Item2).ToList();

            // replace tokens in user configured pattern string
            var dict = new Dictionary<string, string>
            {
                { "species", creature.Species.name },
                { "spcsNm", spcsNm },
                { "firstWordOfOldest", firstWordOfOldest },

                { "owner", creature.owner },
                { "tribe", creature.tribe },
                { "server", creature.server },

                { "sex", creature.sex.ToString() },
                { "sex_short", creature.sex.ToString().Substring(0, 1) },

                { "effImp_short", effImpShort},
                { "index", indexStr},
                { "oldname", oldName },
                { "sex_lang",   Loc.S(creature.sex.ToString()) },
                { "sex_lang_short", Loc.S(creature.sex.ToString()).Substring(0, 1) },
                { "sex_lang_gen",   Loc.S(creature.sex.ToString() + "_gen") },
                { "sex_lang_short_gen", Loc.S(creature.sex.ToString() + "_gen").Substring(0, 1) },

                { "topPercent" , (creature.topness / 10f).ToString() },
                { "baselvl" , creature.LevelHatched.ToString() },
                { "effImp" , effImp },
                { "muta", mutas},
                { "gen", generation.ToString()},
                { "gena", Dec2Hexvig(generation)},
                { "genn", (speciesCreatures?.Count(c=>c.generation==generation) ?? 0 + 1).ToString()},
                { "nr_in_gen", nrInGeneration.ToString()},
                { "nr_in_gen_sex", nrInGenerationAndSameSex.ToString()},
                { "rnd", randStr },
                { "tn", speciesCount.ToString()},
                { "sn", speciesSexCount.ToString()},
                { "dom", dom},
                { "arkid", arkid },
                { "alreadyExists", speciesCreatures.Contains(creature) ? "1" : string.Empty },
                { "highest1l", levelOrder[0].Item2.ToString() },
                { "highest2l", levelOrder[1].Item2.ToString() },
                { "highest3l", levelOrder[2].Item2.ToString() },
                { "highest4l", levelOrder[3].Item2.ToString() },
                { "highest5l", levelOrder[4].Item2.ToString() },
                { "highest6l", levelOrder[5].Item2.ToString() },
                { "highest1s", Utils.StatName(levelOrder[0].Item1, true, creature.Species.statNames) },
                { "highest2s", Utils.StatName(levelOrder[1].Item1, true, creature.Species.statNames) },
                { "highest3s", Utils.StatName(levelOrder[2].Item1, true, creature.Species.statNames) },
                { "highest4s", Utils.StatName(levelOrder[3].Item1, true, creature.Species.statNames) },
                { "highest5s", Utils.StatName(levelOrder[4].Item1, true, creature.Species.statNames) },
                { "highest6s", Utils.StatName(levelOrder[5].Item1, true, creature.Species.statNames) },
            };

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                dict.Add(StatAbbreviationFromIndex[s], creature.levelsWild[s].ToString());
                dict.Add($"{StatAbbreviationFromIndex[s]}_vb", (creature.valuesBreeding[s] * (Utils.Precision(s) == 3 ? 100 : 1)).ToString());
                dict.Add($"isTop{StatAbbreviationFromIndex[s]}", speciesTopLevels == null ? (creature.levelsWild[s] > 0 ? "1" : string.Empty) :
                    creature.levelsWild[s] >= speciesTopLevels[s] ? "1" : string.Empty);
                dict.Add($"isNewTop{StatAbbreviationFromIndex[s]}", speciesTopLevels == null ? (creature.levelsWild[s] > 0 ? "1" : string.Empty) :
                    creature.levelsWild[s] > speciesTopLevels[s] ? "1" : string.Empty);
                dict.Add($"isLowest{StatAbbreviationFromIndex[s]}", speciesLowestLevels == null ? (creature.levelsWild[s] == 0 ? "1" : string.Empty) :
                    speciesLowestLevels[s] != -1 && creature.levelsWild[s] != -1 && creature.levelsWild[s] <= speciesLowestLevels[s] ? "1" : string.Empty);
                dict.Add($"isNewLowest{StatAbbreviationFromIndex[s]}", speciesLowestLevels == null ? (creature.levelsWild[s] == 0 ? "1" : string.Empty) :
                    speciesLowestLevels[s] != -1 && creature.levelsWild[s] != -1 && creature.levelsWild[s] < speciesLowestLevels[s] ? "1" : string.Empty);
            }

            return dict;
        }

        /// <summary>
        /// Converts an integer to a hexavigesimal representation using letters.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string Dec2Hexvig(int number)
        {
            string r = string.Empty;
            number++;
            while (number > 0)
            {
                number--;
                r = (char)(number % 26 + 'A') + r;
                number /= 26;
            }
            return r;
        }

        /// <summary>
        /// Assembles a string representing the desired creature name with the set token
        /// </summary>
        /// <param name="tokenDictionary">a collection of token and their replacements</param>
        /// <returns>The patterned name</returns>
        private static string ResolveKeysToValues(Dictionary<string, string> tokenDictionary, string pattern, int uniqueNumber = 0)
        {
            string regularExpression = "\\{(?<key>" + string.Join("|", tokenDictionary.Keys.Select(x => Regex.Escape(x))) + ")\\}";
            const RegexOptions regularExpressionOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
            Regex r = new Regex(regularExpression, regularExpressionOptions);
            if (uniqueNumber != 0)
            {
                pattern = pattern.Replace("{n}", uniqueNumber.ToString());
            }

            return r.Replace(pattern, m => tokenDictionary.TryGetValue(m.Groups["key"].Value, out string replacement) ? replacement : m.Value);
        }
    }
}

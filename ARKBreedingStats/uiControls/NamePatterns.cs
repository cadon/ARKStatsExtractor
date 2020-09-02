using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public static class NamePatterns
    {
        /// <summary>
        /// Generate a creature name with the naming pattern.
        /// </summary>
        public static string GenerateCreatureName(Creature creature, Creature[] sameSpecies, int[] speciesTopLevels, int[] speciesLowestLevels, Dictionary<string, string> customReplacings, bool showDuplicateNameWarning, int namingPatternIndex, bool showTooLongWarning = true, string pattern = null, bool displayError = true, Dictionary<string, string> tokenDictionary = null)
        {
            var creatureNames = sameSpecies?.Select(x => x.name).ToArray() ?? new string[0];
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
            }

            if (tokenDictionary == null)
                tokenDictionary = CreateTokenDictionary(creature, sameSpecies, speciesTopLevels, speciesLowestLevels);
            // first resolve keys, then functions
            string name = ResolveFunctions(
                ResolveKeysToValues(tokenDictionary, pattern.Replace("\r", string.Empty).Replace("\n", string.Empty), 0),
                creature, customReplacings, displayError, false);

            if (name.Contains("{n}"))
            {
                // replace the unique number key with the lowest possible positive number to get a unique name.
                string numberedUniqueName;
                int n = 1;
                do
                {
                    numberedUniqueName = ResolveFunctions(
                        ResolveKeysToValues(tokenDictionary, name, n++),
                        creature, customReplacings, displayError, true);
                } while (creatureNames.Contains(numberedUniqueName, StringComparer.OrdinalIgnoreCase));
                name = numberedUniqueName;
            }

            if (showDuplicateNameWarning && creatureNames.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("WARNING: The generated name for the creature already exists in the database.");
            }
            else if (showTooLongWarning && name.Length > 24)
            {
                MessageBox.Show("WARNING: The generated name is longer than 24 characters, ingame-preview:\n" + name.Substring(0, 24), "Name too long for game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        /// <param name="displayError"></param>
        /// <returns></returns>
        private static string ResolveFunctions(string pattern, Creature creature, Dictionary<string, string> customReplacings, bool displayError, bool processNumberField)
        {
            int nrFunctions = 0;
            int nrFunctionsAfterResolving = NrFunctions(pattern);
            // the second and third parameter are optional
            Regex r = new Regex(@"\{\{ *#(\w+) *: *([^\|\{\}]*?) *(?:\| *([^\|\{\}]*?) *)?(?:\| *([^\|\{\}]*?) *)?\}\}", RegexOptions.IgnoreCase);
            // resolve nested functions
            while (nrFunctions != nrFunctionsAfterResolving)
            {
                nrFunctions = nrFunctionsAfterResolving;
                pattern = r.Replace(pattern, (m) => ResolveFunction(m, creature, customReplacings, displayError, processNumberField));
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
        /// <param name="m"></param>
        /// <param name="creature"></param>
        /// <param name="customReplacings"></param>
        /// <param name="displayError"></param>
        /// <param name="processNumberField">The number field {n} will add the lowest possible positive integer for the name to be unique. It has to be processed after all other functions.</param>
        /// <returns></returns>
        private static string ResolveFunction(Match m, Creature creature, Dictionary<string, string> customReplacings, bool displayError, bool processNumberField)
        {
            // function parameters can be non numeric if numbers are parsed
            try
            {
                // first parameter value
                string p1 = m.Groups[2].Value;

                if (!processNumberField && p1.Contains("{n}")) return m.Groups[0].Value;

                // switch function name
                switch (m.Groups[1].Value.ToLower())
                {
                    case "if":
                        // check if Group2 !isNullOrWhiteSpace
                        // Group3 contains the result if true
                        // Group4 (optional) contains the result if false
                        return string.IsNullOrWhiteSpace(p1) ? m.Groups[4].Value : m.Groups[3].Value;
                    case "ifexpr":
                        // tries to evaluate the expression
                        // possible operators are ==, !=, <, >, =<, =>
                        var match = Regex.Match(p1, @"\A\s*(\d+(?:\.\d*)?)\s*(==|!=|<|<=|>|>=)\s*(\d+(?:\.\d*)?)\s*\Z");
                        if (match.Success
                            && double.TryParse(match.Groups[1].Value, out double d1)
                            && double.TryParse(match.Groups[3].Value, out double d2)
                            )
                        {
                            switch (match.Groups[2].Value)
                            {
                                case "==": return d1 == d2 ? m.Groups[3].Value : m.Groups[4].Value;
                                case "!=": return d1 != d2 ? m.Groups[3].Value : m.Groups[4].Value;
                                case "<": return d1 < d2 ? m.Groups[3].Value : m.Groups[4].Value;
                                case "<=": return d1 <= d2 ? m.Groups[3].Value : m.Groups[4].Value;
                                case ">": return d1 > d2 ? m.Groups[3].Value : m.Groups[4].Value;
                                case ">=": return d1 >= d2 ? m.Groups[3].Value : m.Groups[4].Value;
                            }
                        }
                        else
                        {
                            // compare the values as strings
                            match = Regex.Match(p1, @"\A\s*(.*?)\s*(==|!=|<=|<|>=|>)\s*(.*?)\s*\Z");
                            if (match.Success)
                            {
                                int stringComparingResult = match.Groups[1].Value.CompareTo(match.Groups[3].Value);
                                switch (match.Groups[2].Value)
                                {
                                    case "==": return stringComparingResult == 0 ? m.Groups[3].Value : m.Groups[4].Value;
                                    case "!=": return stringComparingResult != 0 ? m.Groups[3].Value : m.Groups[4].Value;
                                    case "<": return stringComparingResult < 0 ? m.Groups[3].Value : m.Groups[4].Value;
                                    case "<=": return stringComparingResult <= 0 ? m.Groups[3].Value : m.Groups[4].Value;
                                    case ">": return stringComparingResult > 0 ? m.Groups[3].Value : m.Groups[4].Value;
                                    case ">=": return stringComparingResult >= 0 ? m.Groups[3].Value : m.Groups[4].Value;
                                }
                            }
                        }
                        return ParametersInvalid($"The expression for ifexpr invalid: \"{p1}\"");
                    case "substring":
                        // check param number: 1: substring, 2: p1, 3: pos, 4: length

                        int pos = Convert.ToInt32(m.Groups[3].Value);
                        bool fromEnd = pos < 0;
                        pos = Math.Min(Math.Abs(pos), p1.Length);
                        if (string.IsNullOrEmpty(m.Groups[4].Value))
                        {
                            if (fromEnd)
                                return p1.Substring(p1.Length - pos);
                            else
                                return p1.Substring(pos);
                        }
                        else
                        {
                            int length = Math.Min(Convert.ToInt32(Convert.ToInt32(m.Groups[4].Value)), fromEnd ? pos : p1.Length - pos);
                            if (fromEnd)
                                return p1.Substring(p1.Length - pos, length);
                            else
                                return p1.Substring(pos, length);
                        }
                    case "format":
                        // check param number: 1: format, 2: p1, 3: formatString

                        // only use last param
                        string formatString = m.Groups[3].Value;
                        if (!string.IsNullOrEmpty(formatString))
                        {
                            // convert to double
                            double value = Convert.ToDouble(p1);
                            // format it
                            return value.ToString(formatString);
                        }
                        else
                            return ParametersInvalid("No Format string given");
                    case "padleft":
                        // check param number: 1: padleft, 2: p1, 3: desired length, 4: padding char

                        int padLen = Convert.ToInt32(m.Groups[3].Value);
                        string padChar = m.Groups[4].Value;
                        if (!string.IsNullOrEmpty(padChar))
                        {
                            return p1.PadLeft(padLen, padChar[0]);
                        }
                        else
                        {
                            ParametersInvalid($"No padding char given.");
                            return p1;
                        }
                    case "padright":
                        // check param number: 1: padright, 2: p1, 3: desired length, 4: padding char

                        padLen = Convert.ToInt32(m.Groups[3].Value);
                        padChar = m.Groups[4].Value;
                        if (!string.IsNullOrEmpty(padChar))
                        {
                            return p1.PadRight(padLen, padChar[0]);
                        }
                        else
                            return ParametersInvalid($"No padding char given.");
                    case "float_div":
                        // returns an float after dividing the parsed number
                        // parameter: 1: div, 2: number, 3: divided by 4: format string
                        double dividend = double.Parse(p1);
                        double divisor = double.Parse(m.Groups[3].Value);
                        if (divisor > 0)
                            return ((dividend / divisor)).ToString(m.Groups[4].Value);
                        else
                            return ParametersInvalid("Division by 0");
                    case "div":
                        // returns an integer after dividing the parsed number
                        // parameter: 1: div, 2: number, 3: divided by
                        double number = double.Parse(p1);
                        double div = double.Parse(m.Groups[3].Value);
                        if (div > 0)
                            return ((int)(number / div)).ToString();
                        else
                            return ParametersInvalid("Division by 0");
                    case "casing":
                        // parameter: 1: casing, 2: text, 3: U for UPPER, L for lower, T for Title
                        switch (m.Groups[3].Value.ToLower())
                        {
                            case "u": return p1.ToUpperInvariant();
                            case "l": return p1.ToLowerInvariant();
                            case "t": return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(p1);
                        }
                        return ParametersInvalid($"casing expects 'U', 'L' or 'T', given is '{m.Groups[3].Value}'");
                    case "replace":
                        // parameter: 1: replace, 2: text, 3: find, 4: replace
                        if (string.IsNullOrEmpty(p1)
                            || string.IsNullOrEmpty(m.Groups[3].Value))
                            return p1;
                        return p1.Replace(m.Groups[3].Value.Replace("&nbsp;", " "), m.Groups[4].Value.Replace("&nbsp;", " "));
                    case "customreplace":
                        // parameter: 1: customreplace, 2: key, 3: return if key not available
                        if (customReplacings == null
                            || string.IsNullOrEmpty(p1)
                            || !customReplacings.ContainsKey(p1))
                            return m.Groups[3].Value;
                        return customReplacings[p1];
                    case "time":
                        // parameter: 1: time, 2: format
                        return DateTime.Now.ToString(p1);
                    case "color":
                        // parameter 1: region id (0,...,5), 2: if not empty, the color name instead of the numerical id is returned
                        if (!int.TryParse(p1, out int regionId)
                        || regionId < 0 || regionId > 5) return ParametersInvalid("color region id has to be a number in the range 0 - 5");

                        if ((creature.colors?[regionId] ?? 0) == 0) return string.Empty; // no color info
                        if (string.IsNullOrWhiteSpace(m.Groups[3].Value))
                            return creature.colors[regionId].ToString();
                        return CreatureColors.CreatureColorName(creature.colors[regionId]);
                    case "indexof":
                        // parameter: 1: source string, 2: string to find
                        if (string.IsNullOrEmpty(p1) || string.IsNullOrEmpty(m.Groups[3].Value))
                            return string.Empty;
                        int index = p1.IndexOf(m.Groups[3].Value);
                        return index >= 0 ? index.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The syntax of the following pattern function\n{m.Groups[0].Value}\ncannot be processed and will be ignored.\n\nSpecific error-message:\n{ex.Message}",
                    $"Naming pattern function error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;

            string ParametersInvalid(string specificError)
            {
                if (displayError)
                    MessageBox.Show($"The syntax of the following pattern function\n{m.Groups[0].Value}\ncannot be processed and will be ignored."
                        + (string.IsNullOrEmpty(specificError) ? string.Empty : $"\n\nSpecific error:\n{specificError}"),
                        $"Naming pattern function error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return displayError ? m.Groups[2].Value : specificError;
            }
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
            string dom = creature.isBred ? "B" : "T";

            double imp = creature.imprintingBonus * 100;
            double eff = creature.tamingEff * 100;

            Random rand = new Random(DateTime.Now.Millisecond);
            string randStr = rand.Next(100000, 999999).ToString();

            string effImp = "Z";
            string prefix = string.Empty;
            if (imp > 0)
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

            // the index of the creature in its generation, ordered by addedToLibrary
            int nrInGeneration = (speciesCreatures?.Count(c => c.guid != creature.guid && c.addedToLibrary != null && c.generation == generation && (creature.addedToLibrary == null || c.addedToLibrary < creature.addedToLibrary)) ?? 0) + 1;

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
                    oldName = speciesCreatures.FirstOrDefault(c => c.guid == creature.guid)?.name ?? creature.name;
                }
                else if (creature.ArkId != 0)
                {
                    oldName = speciesCreatures.FirstOrDefault(c => c.ArkId == creature.ArkId)?.name ?? creature.name;
                }
            }

            string spcsNm = creature.Species.name;
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            while (spcsNm.LastIndexOfAny(vowels) > 0)
                spcsNm = spcsNm.Remove(spcsNm.LastIndexOfAny(vowels), 1); // remove last vowel (not the first letter)

            int speciesCount = speciesCreatures?.Length ?? 0 + 1;
            int speciesSexCount = speciesCreatures?.Count(c => c.sex == creature.sex) ?? 0 + 1;
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
                { "rnd", randStr },
                { "tn", speciesCount.ToString()},
                { "sn", speciesSexCount.ToString()},
                { "dom", dom},
                { "arkid", arkid },
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
                r = ((char)(number % 26 + 'A')).ToString() + r;
                number /= 26;
            }
            return r;
        }

        /// <summary>
        /// Assembles a string representing the desired creature name with the set token
        /// </summary>
        /// <param name="tokenDictionary">a collection of token and their replacements</param>
        /// <returns>The patterned name</returns>
        private static string ResolveKeysToValues(Dictionary<string, string> tokenDictionary, string pattern, int uniqueNumber)
        {
            string regularExpression = "\\{(?<key>" + string.Join("|", tokenDictionary.Keys.Select(x => Regex.Escape(x))) + ")\\}";
            const RegexOptions regularExpressionOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
            Regex r = new Regex(regularExpression, regularExpressionOptions);
            if (uniqueNumber != 0) pattern = pattern.Replace("{n}", uniqueNumber.ToString());

            return r.Replace(pattern, m => tokenDictionary.TryGetValue(m.Groups["key"].Value, out string replacement) ? replacement : m.Value);
        }
    }
}

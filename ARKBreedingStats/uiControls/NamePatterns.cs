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
        public static string GenerateCreatureName(Creature creature, List<Creature> females, List<Creature> males, Dictionary<string, string> customReplacings, bool showDuplicateNameWarning, bool showTooLongWarning = true, string pattern = null, bool displayError = true)
        {
            // collect creatures of the same species
            List<Creature> sameSpecies = (females ?? new List<Creature>()).Concat(males ?? new List<Creature>()).ToList();

            return GenerateCreatureName(creature, sameSpecies, customReplacings, showDuplicateNameWarning, showTooLongWarning, pattern, displayError);
        }

        /// <summary>
        /// Generate a creature name with the naming pattern.
        /// </summary>
        public static string GenerateCreatureName(Creature creature, List<Creature> sameSpecies, Dictionary<string, string> customReplacings, bool showDuplicateNameWarning, bool showTooLongWarning = true, string pattern = null, bool displayError = true)
        {
            List<string> creatureNames = sameSpecies.Select(x => x.name).ToList();
            if (pattern == null)
                pattern = Properties.Settings.Default.sequentialUniqueNamePattern;

            Dictionary<string, string> tokenDictionary = CreateTokenDictionary(creature, sameSpecies);
            // first resolve keys, then functions
            string name = ResolveFunctions(
                ResolveKeysToValues(tokenDictionary, pattern.Replace("\r\n", "")),
                creature, customReplacings, displayError);

            if (name.Contains("{n}"))
            {
                // find the sequence token, and if not, return because the configurated pattern string is invalid without it
                int index = name.IndexOf("{n}", StringComparison.OrdinalIgnoreCase);
                string patternStart = name.Substring(0, index);
                string patternEnd = name.Substring(index + 3);

                // loop until we find a unique name in the sequence which is not taken

                int n = 1;
                do
                {
                    name = string.Concat(patternStart, (n < 10 ? "0" : "") + n, patternEnd);
                    n++;
                } while (creatureNames.Contains(name, StringComparer.OrdinalIgnoreCase));
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
        /// <returns></returns>
        private static string ResolveFunctions(string pattern, Creature creature, Dictionary<string, string> customReplacings, bool displayError)
        {
            int nrFunctions = 0;
            int nrFunctionsAfterResolving = NrFunctions(pattern);
            // the second and third parameter are optional
            Regex r = new Regex(@"\{\{#(\w+) *: *([^\|\{\}]+?) *(?:\| *([^\|\{\}]+?) *)?(?:\| *([^\|\{\}]+?) *)?\}\}", RegexOptions.IgnoreCase);
            // resolve nested functions
            while (nrFunctions != nrFunctionsAfterResolving)
            {
                nrFunctions = nrFunctionsAfterResolving;
                pattern = r.Replace(pattern, (m) => ResolveFunction(m, creature, customReplacings, displayError));
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

        private static string ResolveFunction(Match m, Creature creature, Dictionary<string, string> customReplacings, bool displayError)
        {
            // function parameters can be nonnumeric if numbers are parsed
            try
            {
                string p1 = m.Groups[2].Value;
                // switch function name
                switch (m.Groups[1].Value.ToLower())
                {
                    case "if":
                        // Group3 contains the result if true
                        // Group4 (optional) contains the result if false
                        if (p1.Length != 7)
                            return ParametersInvalid($"The condition-parameter expects exactly 7 characters, e.g. \"isTopHP\", given is \"{p1}\"");
                        string conditional = p1.ToLower();
                        if (conditional.Substring(0, 5) != "istop")
                            return ParametersInvalid($"The condition-parameter \"{p1}\"is invalid. It has to start with \"isTop\" followed by a stat specifier, e.g. \"hp\"");
                        int si = StatIndexFromAbbreviation(conditional.Substring(5, 2));
                        if (si == -1)
                            return ParametersInvalid($"Invalid stat name \"{p1}\".");
                        return m.Groups[creature.topBreedingStats[si] ? 3 : 4].Value;
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
                        string fmt_str = m.Groups[3].Value;
                        if (!string.IsNullOrEmpty(fmt_str))
                        {
                            // convert to double
                            double value = Convert.ToDouble(p1);
                            // format it
                            return value.ToString(fmt_str);
                        }
                        else
                            return ParametersInvalid("No Format string given");
                    case "padleft":
                        // check param number: 1: padleft, 2: p1, 3: desired length, 4: padding char

                        int pad_len = Convert.ToInt32(m.Groups[3].Value);
                        string pad_char = m.Groups[4].Value;
                        if (!string.IsNullOrEmpty(pad_char))
                        {
                            return p1.PadLeft(pad_len, pad_char[0]);
                        }
                        else
                        {
                            ParametersInvalid($"No padding char given.");
                            return p1;
                        }
                    case "padright":
                        // check param number: 1: padright, 2: p1, 3: desired length, 4: padding char

                        pad_len = Convert.ToInt32(m.Groups[3].Value);
                        pad_char = m.Groups[4].Value;
                        if (!string.IsNullOrEmpty(pad_char))
                        {
                            return p1.PadRight(pad_len, pad_char[0]);
                        }
                        else
                            return ParametersInvalid($"No padding char given.");
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
                        // parameter: 1: customreplace, 2: text
                        if (customReplacings == null
                            || string.IsNullOrEmpty(p1)
                            || !customReplacings.ContainsKey(p1))
                            return p1;
                        return customReplacings[p1];
                    case "time":
                        // parameter: 1: time, 2: format
                        return DateTime.Now.ToString(p1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The syntax of the following pattern function\n{m.Groups[0].Value}\ncannot be processed and will be ignored.\n\nSpecific error-message:\n{ex.Message}",
                    "Naming pattern function error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;

            string ParametersInvalid(string specificError)
            {
                if (displayError)
                    MessageBox.Show($"The syntax of the following pattern function\n{m.Groups[0].Value}\ncannot be processed and will be ignored."
                        + (string.IsNullOrEmpty(specificError) ? "" : $"\n\nSpecific error:\n{specificError}"),
                        "Naming pattern function error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return displayError ? m.Groups[2].Value : specificError;
            }
        }

        private static int StatIndexFromAbbreviation(string statAb)
        {
            switch (statAb)
            {
                case "hp": return (int)StatNames.Health;
                case "st": return (int)StatNames.Stamina;
                case "to": return (int)StatNames.Torpidity;
                case "ox": return (int)StatNames.Oxygen;
                case "fo": return (int)StatNames.Food;
                case "wa": return (int)StatNames.Water;
                case "te": return (int)StatNames.Temperature;
                case "we": return (int)StatNames.Weight;
                case "dm": return (int)StatNames.MeleeDamageMultiplier;
                case "sp": return (int)StatNames.SpeedMultiplier;
                case "fr": return (int)StatNames.TemperatureFortitude;
                case "cr": return (int)StatNames.CraftingSpeedMultiplier;
                default: return -1;
            }
        }

        /// <summary>        
        /// This method creates the token dictionary for the dynamic creature name generation.
        /// </summary>
        /// <param name="creature">Creature with the data</param>
        /// <param name="speciesCreatures">A list of all currently stored creatures of the species</param>
        /// <returns>A dictionary containing all tokens and their replacements</returns>
        public static Dictionary<string, string> CreateTokenDictionary(Creature creature, List<Creature> speciesCreatures)
        {
            string[] wildLevels = creature.levelsWild.Select(l => l.ToString().PadLeft(2, '0')).ToArray();
            string[] breedingValues = new string[Values.STATS_COUNT];
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                breedingValues[s] = (creature.valuesBreeding[s] * (Utils.precision(s) == 3 ? 100 : 1)).ToString();
            }

            string baselvl = creature.LevelHatched.ToString().PadLeft(2, '0');
            string dom = creature.isBred ? "B" : "T";

            double imp = creature.imprintingBonus * 100;
            double eff = creature.tamingEff * 100;

            Random rand = new Random(DateTime.Now.Millisecond);
            string randStr = rand.Next(100000, 999999).ToString();

            string effImp = "Z";
            string effImp_short = effImp;
            string prefix = "";
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

            effImp_short = effImp;
            effImp = prefix + effImp;

            int generation = 0;
            if (creature.Mother != null) generation = creature.Mother.generation + 1;
            if (creature.Father != null && creature.Father.generation + 1 > generation)
                generation = creature.Father.generation + 1;

            int mutasn = creature.Mutations;
            string mutas = mutasn > 99 ? "99" : mutasn.ToString().PadLeft(2, '0');

            string old_name = creature.name;

            string firstWordOfOldest = "";
            if (speciesCreatures.Count > 0)
            {
                firstWordOfOldest = speciesCreatures.OrderBy(s => s.addedToLibrary).First().name;
                if (!string.IsNullOrEmpty(firstWordOfOldest) && firstWordOfOldest.Contains(" "))
                {
                    firstWordOfOldest = firstWordOfOldest.Substring(0, firstWordOfOldest.IndexOf(" "));
                }

                if (creature.guid != Guid.Empty)
                {
                    old_name = speciesCreatures.FirstOrDefault(c => c.guid == creature.guid)?.name ?? creature.name;
                }
            }

            string spcsNm = creature.Species.name;
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            while (spcsNm.LastIndexOfAny(vowels) > 0)
                spcsNm = spcsNm.Remove(spcsNm.LastIndexOfAny(vowels), 1); // remove last vowel (not the first letter)

            int speciesCount = speciesCreatures.Count + 1;
            int speciesSexCount = speciesCreatures.Count(c => c.sex == creature.sex) + 1;
            string arkid = "";
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

            string index_str = "";
            if (creature.guid != Guid.Empty)
            {
                for (int i = 0; i < speciesCreatures.Count; i++)
                {
                    if (creature.guid == speciesCreatures[i].guid)
                    {
                        index_str = (i + 1).ToString();
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

            // replace tokens in user configurated pattern string
            return new Dictionary<string, string>
            {
                { "species", creature.Species.name },
                { "spcsNm", spcsNm },
                { "firstWordOfOldest", firstWordOfOldest },

                {"owner", creature.owner },
                {"tribe", creature.tribe },
                {"server", creature.server },

                { "sex", creature.sex.ToString() },
                { "sex_short", creature.sex.ToString().Substring(0, 1) },

                { "hp", wildLevels[(int)StatNames.Health] },
                { "st", wildLevels[(int)StatNames.Stamina] },
                { "to", wildLevels[(int)StatNames.Torpidity] },
                { "ox", wildLevels[(int)StatNames.Oxygen] },
                { "fo", wildLevels[(int)StatNames.Food] },
                { "wa", wildLevels[(int)StatNames.Water] },
                { "te", wildLevels[(int)StatNames.Temperature] },
                { "we", wildLevels[(int)StatNames.Weight] },
                { "dm", wildLevels[(int)StatNames.MeleeDamageMultiplier] },
                { "sp", wildLevels[(int)StatNames.SpeedMultiplier] },
                { "fr", wildLevels[(int)StatNames.TemperatureFortitude] },
                { "cr", wildLevels[(int)StatNames.CraftingSpeedMultiplier] },

                { "hp_vb", breedingValues[(int)StatNames.Health] },
                { "st_vb", breedingValues[(int)StatNames.Stamina] },
                { "to_vb", breedingValues[(int)StatNames.Torpidity] },
                { "ox_vb", breedingValues[(int)StatNames.Oxygen] },
                { "fo_vb", breedingValues[(int)StatNames.Food] },
                { "wa_vb", breedingValues[(int)StatNames.Water] },
                { "te_vb", breedingValues[(int)StatNames.Temperature] },
                { "we_vb", breedingValues[(int)StatNames.Weight] },
                { "dm_vb", breedingValues[(int)StatNames.MeleeDamageMultiplier] },
                { "sp_vb", breedingValues[(int)StatNames.SpeedMultiplier] },
                { "fr_vb", breedingValues[(int)StatNames.TemperatureFortitude] },
                { "cr_vb", breedingValues[(int)StatNames.CraftingSpeedMultiplier] },

                { "effImp_short", effImp_short},
                { "index", index_str},
                { "oldname", old_name },
                { "sex_lang",   Loc.s(creature.sex.ToString()) },
                { "sex_lang_short", Loc.s(creature.sex.ToString()).Substring(0, 1) },
                { "sex_lang_gen",   Loc.s(creature.sex.ToString() + "_gen") },
                { "sex_lang_short_gen", Loc.s(creature.sex.ToString() + "_gen").Substring(0, 1) },

                { "baselvl" , baselvl },
                { "effImp" , effImp },
                { "muta", mutas},
                { "gen", generation.ToString()},
                { "gena", Dec2hexvig(generation)},
                { "rnd", randStr },
                { "tn", (speciesCount < 10 ? "0" : "") + speciesCount},
                { "sn", (speciesSexCount < 10 ? "0" : "") + speciesSexCount},
                { "dom", dom},
                { "arkid", arkid },
                { "highest1l", levelOrder[0].Item2.ToString() },
                { "highest2l", levelOrder[1].Item2.ToString() },
                { "highest3l", levelOrder[2].Item2.ToString() },
                { "highest4l", levelOrder[3].Item2.ToString() },
                { "highest1s", Utils.statName(levelOrder[0].Item1, true, creature.Species.IsGlowSpecies) },
                { "highest2s", Utils.statName(levelOrder[1].Item1, true, creature.Species.IsGlowSpecies) },
                { "highest3s", Utils.statName(levelOrder[2].Item1, true, creature.Species.IsGlowSpecies) },
                { "highest4s", Utils.statName(levelOrder[3].Item1, true, creature.Species.IsGlowSpecies) },
            };
        }

        /// <summary>
        /// Convertes an integer to a hexavigesimal representation using letters.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string Dec2hexvig(int number)
        {
            string r = "";
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
        private static string ResolveKeysToValues(Dictionary<string, string> tokenDictionary, string pattern)
        {
            string regularExpression = "\\{(?<key>" + string.Join("|", tokenDictionary.Keys.Select(x => Regex.Escape(x))) + ")\\}";
            const RegexOptions regularExpressionOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
            Regex r = new Regex(regularExpression, regularExpressionOptions);

            return r.Replace(pattern, m => tokenDictionary.TryGetValue(m.Groups["key"].Value, out string replacement) ? replacement : m.Value);
        }
    }
}

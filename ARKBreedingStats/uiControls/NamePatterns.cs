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
        public static string GenerateCreatureName(Creature creature, List<Creature> females, List<Creature> males, bool showDuplicateNameWarning)
        {
            // collect creatures of the same species
            List<Creature> sameSpecies = (females ?? new List<Creature>()).Concat(males ?? new List<Creature>()).ToList();

            return GenerateCreatureName(creature, sameSpecies, showDuplicateNameWarning);
        }

        /// <summary>
        /// Generate a creature name with the naming pattern.
        /// </summary>
        public static string GenerateCreatureName(Creature creature, List<Creature> sameSpecies, bool showDuplicateNameWarning)
        {
            List<string> creatureNames = sameSpecies.Select(x => x.name).ToList();

            Dictionary<string, string> tokenDictionary = CreateTokenDictionary(creature, sameSpecies);
            string resolvedFunction = ResolveFunction(tokenDictionary, Properties.Settings.Default.sequentialUniqueNamePattern);
            string resolvedPattern = ResolveConditions(creature, resolvedFunction);
            string name = AssemblePatternedName(tokenDictionary, resolvedPattern);

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
            else if (name.Length > 24)
            {
                MessageBox.Show("WARNING: The generated name is longer than 24 characters, ingame-preview:\n" + name.Substring(0, 24), "Name too long for game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return name;
        }

        /// <summary>
        /// If a pattern contains conditional expressions, resolve them.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static string ResolveConditions(Creature creature, string pattern)
        {
            // a conditional expression looks like {{#if:conditional-keyword|result if true|result if false}}, e.g. {{#if:IsTopHP|{HP}H}}

            Regex r = new Regex(@"\{\{#if: *(istop\w+) *\| *([^\|]+?)(?: *\| *(.+?))? *\}\}", RegexOptions.IgnoreCase);
            return r.Replace(pattern, (m) => ResolveCondition(creature, m));
        }
        private static string ResolveFunction(Dictionary<string, string> tokenDictionary, string pattern)
        {
            // a function expression looks like {{#function_name:{xxx}|2|3}}, e.g. {{#substring:{HP}|2|3}}

            Regex r = new Regex(@"\{\{#(\w+):\{(\w+)\}((\|\w+)+)\}\}", RegexOptions.IgnoreCase);
            return r.Replace(pattern, (m) => ResolveFunctionParam(tokenDictionary, m));
        }

        private static string ResolveFunctionParam(Dictionary<string, string> tokenDictionary, Match m)
        {
            if (m.Groups.Count < 2) return string.Empty;

            // function parameters can be nonnumeric if numbers are parsed
            try
            {
                // switch function name
                switch (m.Groups[1].Value.ToLower())
                {
                    case "substring":
                        // check param number: 1: substring, 2: xxx, 3: |0|3, 4: |3
                        if (m.Groups.Count != 5)
                        {
                            return string.Empty;
                        }

                        // find
                        string find_value = FindValueByKey(m.Groups[2].Value);
                        // check it
                        if (string.IsNullOrEmpty(find_value)) return string.Empty;

                        string[] param_list = m.Groups[3].Value.Split('|');
                        if (param_list.Length == 2)
                        {
                            int size = Convert.ToInt32(param_list[1].Trim('|'));
                            return find_value.Substring(0, Min(find_value.Length, size));
                        }
                        if (param_list.Length == 3)
                        {
                            int pos = Min(Convert.ToInt32(param_list[1].Trim('|')), find_value.Length);
                            int size = Min(Convert.ToInt32(param_list[2].Trim('|')), find_value.Length - pos);
                            return find_value.Substring(pos, size);
                        }
                        break;
                    case "format":
                        // check param number: 1: format, 2: xxx, 3: |F2, 4: |F2
                        if (m.Groups.Count != 5)
                        {
                            return string.Empty;
                        }

                        // find
                        find_value = FindValueByKey(m.Groups[2].Value);
                        // check it
                        if (string.IsNullOrEmpty(find_value)) return string.Empty;

                        // only use last param
                        string fmt_str = m.Groups[4].Value.Trim('|');
                        if (!string.IsNullOrEmpty(fmt_str))
                        {
                            // convert to double
                            double value = Convert.ToDouble(find_value);
                            // format it
                            return value.ToString(fmt_str);
                        }
                        else
                        {
                            ParametersInvalid("No Format string given");
                        }
                        break;
                    case "padleft":
                        // check param number: 1: padleft, 2: xxx, 3: |2|0, 4: |0
                        if (m.Groups.Count != 5)
                        {
                            return string.Empty;
                        }

                        // find
                        find_value = FindValueByKey(m.Groups[2].Value);
                        // check it
                        if (string.IsNullOrEmpty(find_value)) return string.Empty;

                        param_list = m.Groups[3].Value.Split('|');
                        if (param_list.Length == 3)
                        {
                            int pad_len = Convert.ToInt32(param_list[1]);
                            string pad_char = param_list[2];
                            return find_value.PadLeft(pad_len, pad_char[0]);
                        }
                        else
                        {
                            ParametersInvalid($"3 parameters expected but {param_list.Length} given.");
                        }
                        break;
                    case "padright":
                        // check param number: 1: padright, 2: xxx, 3: |2|0, 4: |0
                        if (m.Groups.Count != 5)
                        {
                            return string.Empty;
                        }

                        // find
                        find_value = FindValueByKey(m.Groups[2].Value);
                        // check it
                        if (string.IsNullOrEmpty(find_value)) return string.Empty;

                        param_list = m.Groups[3].Value.Split('|');
                        if (param_list.Length == 3)
                        {
                            int pad_len = Convert.ToInt32(param_list[1]);
                            string pad_char = param_list[2];
                            return find_value.PadRight(pad_len, pad_char[0]);
                        }
                        else
                        {
                            ParametersInvalid($"3 parameters expected but {param_list.Length} given.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The syntax of the following pattern function\n{m.Groups[0].Value}\ncannot be processed and will be ignored.\n\nSpecific error-message:\n{ex.Message}",
                    "Naming pattern function error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;

            string FindValueByKey(string key)
            {
                foreach (KeyValuePair<string, string> kv_pair in tokenDictionary)
                {
                    if (kv_pair.Key == key)
                    {
                        return kv_pair.Value;
                    }
                }
                return string.Empty;
            }

            void ParametersInvalid(string specificError = null) => MessageBox.Show($"The syntax of the following pattern function\n{m.Groups[0].Value}\ncannot be processed and will be ignored."
                + (string.IsNullOrEmpty(specificError) ? "" : $"\n\nSpecific error:\n{specificError}"),
                    "Naming pattern function error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static string ResolveCondition(Creature c, Match m)
        {
            // Group1 contains the condition
            // Group2 contains the result if true
            // Group3 (optional) contains the result if false
            string conditional = m.Groups[1].Value.ToLower();
            if (conditional.Substring(0, 5) == "istop")
            {
                int si = StatIndexFromAbbreviation(conditional.Substring(5, 2));
                if (si == -1) return string.Empty;
                return m.Groups[c.topBreedingStats[si] ? 2 : 3].Value;
            }
            return string.Empty;

        }

        private static int StatIndexFromAbbreviation(string statAb)
        {
            switch (statAb)
            {
                case "hp": return (int)StatNames.Health;
                case "st": return (int)StatNames.Stamina;
                case "ox": return (int)StatNames.Oxygen;
                case "fo": return (int)StatNames.Food;
                case "we": return (int)StatNames.Weight;
                case "dm": return (int)StatNames.MeleeDamageMultiplier;
                case "sp": return (int)StatNames.SpeedMultiplier;
                case "to": return (int)StatNames.Torpidity;
                case "wa": return (int)StatNames.Water;
                case "te": return (int)StatNames.Temperature;
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
            string yyyy = DateTime.Now.ToString("yyyy");
            string yy = DateTime.Now.ToString("yy");
            string MM = DateTime.Now.ToString("MM");
            string dd = DateTime.Now.ToString("dd");
            string hh = DateTime.Now.ToString("hh");
            string mm = DateTime.Now.ToString("mm");
            string ss = DateTime.Now.ToString("ss");

            string date = DateTime.Now.ToString("yy-MM-dd");
            string time = DateTime.Now.ToString("hh:mm:ss");

            string[] wildLevels = creature.levelsWild.Select(l => l.ToString().PadLeft(2, '0')).ToArray();
            string[] breedingValues = new string[Values.STATS_COUNT];
            string[] breedingValuesN = new string[Values.STATS_COUNT]; // without decimals
            string[] breedingValuesK = new string[Values.STATS_COUNT]; // values in thousands
            string[] breedingValues10K = new string[Values.STATS_COUNT]; // values in ten-thousands

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                breedingValues[s] = (creature.valuesBreeding[s] * (Utils.precision(s) == 3 ? 100 : 1)).ToString();
                breedingValuesN[s] = Math.Floor(creature.valuesBreeding[s] * (Utils.precision(s) == 3 ? 100 : 1)).ToString();
                breedingValuesK[s] = Math.Floor(creature.valuesBreeding[s] * (Utils.precision(s) == 3 ? 0.1 : 0.001)).ToString();
                breedingValues10K[s] = Math.Floor(creature.valuesBreeding[s] * (Utils.precision(s) == 3 ? 0.01 : 0.0001)).ToString();
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
            if (creature.Father != null && creature.Father.generation + 1 > generation) generation = creature.Father.generation + 1;

            string precompressed =
                creature.sex.ToString().Substring(0, 1) +
                yy + MM + dd +
                wildLevels[(int)StatNames.Health] +
                wildLevels[(int)StatNames.Stamina] +
                wildLevels[(int)StatNames.Oxygen] +
                wildLevels[(int)StatNames.Food] +
                wildLevels[(int)StatNames.Weight] +
                wildLevels[(int)StatNames.MeleeDamageMultiplier] +
                effImp;

            int mutasn = creature.Mutations;
            string mutas = mutasn > 99 ? "99" : mutasn.ToString().PadLeft(2, '0');

            string firstWordOfOldest = "";
            if (speciesCreatures.Count > 0)
            {
                firstWordOfOldest = speciesCreatures.OrderBy(s => s.addedToLibrary).First().name;
                if (!string.IsNullOrEmpty(firstWordOfOldest) && firstWordOfOldest.Contains(" "))
                {
                    firstWordOfOldest = firstWordOfOldest.Substring(0, firstWordOfOldest.IndexOf(" "));
                }
            }

            string old_name = string.Empty;
            if (creature.guid != Guid.Empty)
            {
                old_name = speciesCreatures.FirstOrDefault(c => c.guid == creature.guid).name ?? creature.name;
            }
            else
            {
                old_name = creature.name;
            }

            string speciesShort6 = creature.Species.name.Replace(" ", "").Replace("Aberrant", "Ab");
            string spcShort = speciesShort6;
            char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            while (spcShort.Length > 4 && spcShort.LastIndexOfAny(vowels) > 0)
                spcShort = spcShort.Remove(spcShort.LastIndexOfAny(vowels), 1); // remove last vowel (not the first letter)
            spcShort = spcShort.Substring(0, Math.Min(4, spcShort.Length));

            speciesShort6 = speciesShort6.Substring(0, Math.Min(6, speciesShort6.Length));
            string speciesShort5 = speciesShort6.Substring(0, Math.Min(5, speciesShort6.Length));
            string speciesShort4 = speciesShort6.Substring(0, Math.Min(4, speciesShort6.Length));
            int speciesCount = speciesCreatures.Count + 1;
            int speciesSexCount = speciesCreatures.Count(c => c.sex == creature.sex) + 1;
            string arkidlast4 = "";
            if (creature.ArkId != 0)
            {
                if (creature.ArkIdImported)
                {
                    string arkid = Utils.ConvertImportedArkIdToIngameVisualization(creature.ArkId);
                    arkidlast4 = arkid.Substring(arkid.Length - 4, 4);
                }
                else
                {
                    string arkid = creature.ArkId.ToString();
                    int l = Math.Min(4, arkid.Length);
                    arkidlast4 = arkid.Substring(arkid.Length - l, l);
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
            var levelOrder = new List<Tuple<int, int>>(values.Values.STATS_COUNT);
            for (int si = 0; si < values.Values.STATS_COUNT; si++)
            {
                if (si != (int)StatNames.Torpidity && creature.Species.UsesStat(si))
                    levelOrder.Add(new Tuple<int, int>(si, creature.levelsWild[si]));
            }
            levelOrder = levelOrder.OrderByDescending(l => l.Item2).ToList();

            // replace tokens in user configurated pattern string
            return new Dictionary<string, string>
            {
                { "species", creature.Species.name },
                { "species_short6", speciesShort6 },
                { "species_short6u", speciesShort6.ToUpper() },
                { "species_short5", speciesShort5 },
                { "species_short5u", speciesShort5.ToUpper() },
                { "species_short4", speciesShort4 },
                { "species_short4u", speciesShort4.ToUpper() },
                { "spcs_short4", spcShort },
                { "spcs_short4u", spcShort.ToUpper() },
                { "firstWordOfOldest", firstWordOfOldest },
                { "sex", creature.sex.ToString() },
                { "sex_short", creature.sex.ToString().Substring(0, 1) },
                { "cpr", precompressed },
                { "yyyy", yyyy },
                { "yy", yy },
                { "MM", MM },
                { "dd", dd },
                { "hh", hh },
                { "mm", mm },
                { "ss", ss },
                { "date" ,  date },
                { "times" , time },
                { "hp"     , wildLevels[(int)StatNames.Health] },
                { "stam"   , wildLevels[(int)StatNames.Stamina] },
                { "trp"    , wildLevels[(int)StatNames.Torpidity] },
                { "oxy"    , wildLevels[(int)StatNames.Oxygen] },
                { "food"   , wildLevels[(int)StatNames.Food] },
                { "water"  , wildLevels[(int)StatNames.Water] },
                { "temp"   , wildLevels[(int)StatNames.Temperature] },
                { "weight" , wildLevels[(int)StatNames.Weight] },
                { "dmg"    , wildLevels[(int)StatNames.MeleeDamageMultiplier] },
                { "spd"    , wildLevels[(int)StatNames.SpeedMultiplier] },
                { "fort"   , wildLevels[(int)StatNames.TemperatureFortitude] },
                { "craft"  , wildLevels[(int)StatNames.CraftingSpeedMultiplier] },

                { "hp_vb"     , breedingValues[(int)StatNames.Health] },
                { "stam_vb"   , breedingValues[(int)StatNames.Stamina] },
                { "trp_vb"    , breedingValues[(int)StatNames.Torpidity] },
                { "oxy_vb"    , breedingValues[(int)StatNames.Oxygen] },
                { "food_vb"   , breedingValues[(int)StatNames.Food] },
                { "water_vb"  , breedingValues[(int)StatNames.Water] },
                { "temp_vb"   , breedingValues[(int)StatNames.Temperature] },
                { "weight_vb" , breedingValues[(int)StatNames.Weight] },
                { "dmg_vb"    , breedingValues[(int)StatNames.MeleeDamageMultiplier] },
                { "spd_vb"    , breedingValues[(int)StatNames.SpeedMultiplier] },
                { "fort_vb"   , breedingValues[(int)StatNames.TemperatureFortitude] },
                { "craft_vb"  , breedingValues[(int)StatNames.CraftingSpeedMultiplier] },

                { "hp_vb_k"     , breedingValuesK[(int)StatNames.Health] },
                { "stam_vb_k"   , breedingValuesK[(int)StatNames.Stamina] },
                { "trp_vb_k"    , breedingValuesK[(int)StatNames.Torpidity] },
                { "oxy_vb_k"    , breedingValuesK[(int)StatNames.Oxygen] },
                { "food_vb_k"   , breedingValuesK[(int)StatNames.Food] },
                { "water_vb_k"  , breedingValuesK[(int)StatNames.Water] },
                { "temp_vb_k"   , breedingValuesK[(int)StatNames.Temperature] },
                { "weight_vb_k" , breedingValuesK[(int)StatNames.Weight] },
                { "dmg_vb_k"    , breedingValuesK[(int)StatNames.MeleeDamageMultiplier] },
                { "spd_vb_k"    , breedingValuesK[(int)StatNames.SpeedMultiplier] },
                { "fort_vb_k"   , breedingValuesK[(int)StatNames.TemperatureFortitude] },
                { "craft_vb_k"  , breedingValuesK[(int)StatNames.CraftingSpeedMultiplier] },

                { "hp_vb_10k"     , breedingValues10K[(int)StatNames.Health] },
                { "stam_vb_10k"   , breedingValues10K[(int)StatNames.Stamina] },
                { "trp_vb_10k"    , breedingValues10K[(int)StatNames.Torpidity] },
                { "oxy_vb_10k"    , breedingValues10K[(int)StatNames.Oxygen] },
                { "food_vb_10k"   , breedingValues10K[(int)StatNames.Food] },
                { "water_vb_10k"  , breedingValues10K[(int)StatNames.Water] },
                { "temp_vb_10k"   , breedingValues10K[(int)StatNames.Temperature] },
                { "weight_vb_10k" , breedingValues10K[(int)StatNames.Weight] },
                { "dmg_vb_10k"    , breedingValues10K[(int)StatNames.MeleeDamageMultiplier] },
                { "spd_vb_10k"    , breedingValues10K[(int)StatNames.SpeedMultiplier] },
                { "fort_vb_10k"   , breedingValues10K[(int)StatNames.TemperatureFortitude] },
                { "craft_vb_10k"  , breedingValues10K[(int)StatNames.CraftingSpeedMultiplier] },

                { "hp_vb_n"     , breedingValuesN[(int)StatNames.Health] },
                { "stam_vb_n"   , breedingValuesN[(int)StatNames.Stamina] },
                { "trp_vb_n"    , breedingValuesN[(int)StatNames.Torpidity] },
                { "oxy_vb_n"    , breedingValuesN[(int)StatNames.Oxygen] },
                { "food_vb_n"   , breedingValuesN[(int)StatNames.Food] },
                { "water_vb_n"  , breedingValuesN[(int)StatNames.Water] },
                { "temp_vb_n"   , breedingValuesN[(int)StatNames.Temperature] },
                { "weight_vb_n" , breedingValuesN[(int)StatNames.Weight] },
                { "dmg_vb_n"    , breedingValuesN[(int)StatNames.MeleeDamageMultiplier] },
                { "spd_vb_n"    , breedingValuesN[(int)StatNames.SpeedMultiplier] },
                { "fort_vb_n"   , breedingValuesN[(int)StatNames.TemperatureFortitude] },
                { "craft_vb_n"  , breedingValuesN[(int)StatNames.CraftingSpeedMultiplier] },

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
                { "gen", generation.ToString().PadLeft(3,'0')},
                { "gena", Dec2hexvig(generation).PadLeft(2,'0')},
                { "rnd", randStr },
                { "tn", (speciesCount < 10 ? "0" : "") + speciesCount},
                { "sn", (speciesSexCount < 10 ? "0" : "") + speciesSexCount},
                { "dom", dom},
                { "arkidlast4", arkidlast4 },
                { "highest1l", levelOrder[0].Item2.ToString() },
                { "highest2l", levelOrder[1].Item2.ToString() },
                { "highest3l", levelOrder[2].Item2.ToString() },
                { "highest4l", levelOrder[3].Item2.ToString() },
                { "highest1s", Utils.statName(levelOrder[0].Item1,true,creature.Species.IsGlowSpecies) },
                { "highest2s", Utils.statName(levelOrder[1].Item1,true,creature.Species.IsGlowSpecies) },
                { "highest3s", Utils.statName(levelOrder[2].Item1,true,creature.Species.IsGlowSpecies) },
                { "highest4s", Utils.statName(levelOrder[3].Item1,true,creature.Species.IsGlowSpecies) },
            };
        }
        private static int Min(int a, int b)
        {
            if (a > b)
                return b;
            return a;
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
        private static string AssemblePatternedName(Dictionary<string, string> tokenDictionary, string pattern)
        {
            string regularExpression = "\\{(?<key>" + string.Join("|", tokenDictionary.Keys.Select(x => Regex.Escape(x))) + ")\\}";
            const RegexOptions regularExpressionOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
            Regex r = new Regex(regularExpression, regularExpressionOptions);

            return r.Replace(pattern, m => tokenDictionary.TryGetValue(m.Groups["key"].Value, out string replacement) ? replacement : m.Value);
        }
    }
}

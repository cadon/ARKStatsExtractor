using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public static class NamePatterns
    {
        public static string generateCreatureName(Creature creature, List<Creature> females, List<Creature> males, bool showDuplicateNameWarning)
        {
            //try
            //{
            // collect creatures of the same species
            List<Creature> sameSpecies = (females ?? new List<Creature>()).Concat(males ?? new List<Creature>()).ToList();

            return generateCreatureName2(creature, sameSpecies);
        }

        public static string generateCreatureName2(Creature creature, List<Creature> sameSpecies, bool showDuplicateNameWarning)
        {
            List<string> creatureNames = sameSpecies.Select(x => x.name).ToList();

            Dictionary<string, string> tokenDictionary = CreateTokenDictionary(creature, sameSpecies);
            string resolvedFunction = ResolveFunction(tokenDictionary, Properties.Settings.Default.sequentialUniqueNamePattern);
            string resolvedPattern = ResolveConditions(creature, Properties.Settings.Default.sequentialUniqueNamePattern);            string resolvedPattern = ResolveConditions(creature, resolvedFunction);
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
            //}
            //catch
            //{
            //    MessageBox.Show("There was an error while generating the creature name.");
            //}
            //return "";
        }

        /// <summary>
        /// If a pattern contains conditional expressions, resolve them.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="sequentialUniqueNamePattern"></param>
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
            if (m.Groups.Count > 1)
            {
                string func_name = m.Groups[1].Value.ToLower();
                if (func_name == "substring")
                {
                    // check param number: 1: substring, 2: xxx, 3: |0|3, 4: |3
                    if (m.Groups.Count != 5)
                    {
                        return string.Empty;
                    }

                    // find
                    string find_value = "";
                    string find_key = m.Groups[2].Value;
                    foreach(KeyValuePair<string, string> kv_pair in tokenDictionary)
                    {
                        if (kv_pair.Key == find_key)
                        {
                            find_value = kv_pair.Value;
                            break;
                        }
                    }

                    // check it
                    if (find_value == "")
                    {
                        return string.Empty;
                    }
                    string[] param_list = m.Groups[3].Value.Split('|');
                    if (param_list.Length == 2)
                    {
                        int size = Convert.ToInt32(param_list[1].Trim('|'));
                        return find_value.Substring(0, min(find_value.Length, size));
                    }
                    if (param_list.Length == 3)
                    {
                        int pos = Convert.ToInt32(param_list[1].Trim('|'));
                        int size = Convert.ToInt32(param_list[2].Trim('|'));
                        return find_value.Substring(min(pos, find_value.Length), min(find_value.Length, size));
                    }
                } else if (func_name == "format")
                {
                    // check param number: 1: format, 2: xxx, 3: |F2, 4: |F2
                    if (m.Groups.Count != 5)
                    {
                        return string.Empty;
                    }

                    // find
                    string find_value = "";
                    string find_key = m.Groups[2].Value;
                    foreach (KeyValuePair<string, string> kv_pair in tokenDictionary)
                    {
                        if (kv_pair.Key == find_key)
                        {
                            find_value = kv_pair.Value;
                            break;
                        }
                    }

                    // check it
                    if (find_value == "")
                    {
                        return string.Empty;
                    }
                    // only use last param
                    string fmt_str = m.Groups[4].Value.Trim('|');
                    if (fmt_str != "")
                    {
                        // convert to double
                        double value = Convert.ToDouble(find_value);
                        // format it
                        return value.ToString(fmt_str);
                    }
                } else if (func_name == "padleft")
                {
                    // check param number: 1: padleft, 2: xxx, 3: |2|0, 4: |0
                    if (m.Groups.Count != 5)
                    {
                        return string.Empty;
                    }

                    // find
                    string find_value = "";
                    string find_key = m.Groups[2].Value;
                    foreach (KeyValuePair<string, string> kv_pair in tokenDictionary)
                    {
                        if (kv_pair.Key == find_key)
                        {
                            find_value = kv_pair.Value;
                            break;
                        }
                    }

                    // check it
                    if (find_value == "")
                    {
                        return string.Empty;
                    }
                    string[] param_list = m.Groups[3].Value.Split('|');
                    if (param_list.Length == 3)
                    {
                        int pad_len = Convert.ToInt32(param_list[1]);
                        string pad_char = param_list[2];
                        return find_value.PadLeft(pad_len, pad_char[0]); ;
                    }
                } else if (func_name == "padright")
                {
                    // check param number: 1: padright, 2: xxx, 3: |2|0, 4: |0
                    if (m.Groups.Count != 5)
                    {
                        return string.Empty;
                    }

                    // find
                    string find_value = "";
                    string find_key = m.Groups[2].Value;
                    foreach (KeyValuePair<string, string> kv_pair in tokenDictionary)
                    {
                        if (kv_pair.Key == find_key)
                        {
                            find_value = kv_pair.Value;
                            break;
                        }
                    }

                    // check it
                    if (find_value == "")
                    {
                        return string.Empty;
                    }
                    string[] param_list = m.Groups[3].Value.Split('|');
                    if (param_list.Length == 3)
                    {
                        int pad_len = Convert.ToInt32(param_list[1]);
                        string pad_char = param_list[2];
                        return find_value.PadRight(pad_len, pad_char[0]); ;
                    }
                }
            }
            return string.Empty;
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

            string hp = creature.levelsWild[(int)StatNames.Health].ToString().PadLeft(2, '0');
            string stam = creature.levelsWild[(int)StatNames.Stamina].ToString().PadLeft(2, '0');
            string trp = creature.levelsWild[(int)StatNames.Torpidity].ToString().PadLeft(2, '0');
            string oxy = creature.levelsWild[(int)StatNames.Oxygen].ToString().PadLeft(2, '0');
            string food = creature.levelsWild[(int)StatNames.Food].ToString().PadLeft(2, '0');
            string water = creature.levelsWild[(int)StatNames.Water].ToString().PadLeft(2, '0');
            string temp = creature.levelsWild[(int)StatNames.Temperature].ToString().PadLeft(2, '0');
            string weight = creature.levelsWild[(int)StatNames.Weight].ToString().PadLeft(2, '0');
            string dmg = creature.levelsWild[(int)StatNames.MeleeDamageMultiplier].ToString().PadLeft(2, '0');
            string spd = creature.levelsWild[(int)StatNames.SpeedMultiplier].ToString().PadLeft(2, '0');
            string fort = creature.levelsWild[(int)StatNames.TemperatureFortitude].ToString().PadLeft(2, '0');
            string craft = creature.levelsWild[(int)StatNames.CraftingSpeedMultiplier].ToString().PadLeft(2, '0');

            string hp_vb = (creature.valuesBreeding[(int)StatNames.Health]).ToString();
            string stam_vb = (creature.valuesBreeding[(int)StatNames.Stamina]).ToString();
            string trp_vb = (creature.valuesBreeding[(int)StatNames.Torpidity]).ToString();
            string oxy_vb = (creature.valuesBreeding[(int)StatNames.Oxygen]).ToString();
            string food_vb = (creature.valuesBreeding[(int)StatNames.Food]).ToString();
            string water_vb = (creature.valuesBreeding[(int)StatNames.Water]).ToString();
            string temp_vb = (creature.valuesBreeding[(int)StatNames.Temperature]).ToString();
            string weight_vb = (creature.valuesBreeding[(int)StatNames.Weight]).ToString();
            // dmg need multi 100
            string dmg_vb = ((creature.valuesBreeding[(int)StatNames.MeleeDamageMultiplier]*100)).ToString();
            string spd_vb = (creature.valuesBreeding[(int)StatNames.SpeedMultiplier]).ToString();
            string fort_vb = (creature.valuesBreeding[(int)StatNames.TemperatureFortitude]).ToString();
            string craft_vb = (creature.valuesBreeding[(int)StatNames.CraftingSpeedMultiplier]).ToString();

            string hp_vb_k = (creature.valuesBreeding[(int)StatNames.Health] / 1000).ToString();
            string stam_vb_k = (creature.valuesBreeding[(int)StatNames.Stamina] / 1000).ToString();
            string trp_vb_k = (creature.valuesBreeding[(int)StatNames.Torpidity] / 1000).ToString();
            string oxy_vb_k = (creature.valuesBreeding[(int)StatNames.Oxygen] / 1000).ToString();
            string food_vb_k = (creature.valuesBreeding[(int)StatNames.Food] / 1000).ToString();
            string water_vb_k = (creature.valuesBreeding[(int)StatNames.Water] / 1000).ToString();
            string temp_vb_k = (creature.valuesBreeding[(int)StatNames.Temperature] / 1000).ToString();
            string weight_vb_k = (creature.valuesBreeding[(int)StatNames.Weight] / 1000).ToString();
            // dmg need multi 100
            string dmg_vb_k = ((creature.valuesBreeding[(int)StatNames.MeleeDamageMultiplier] / 10)).ToString();
            string spd_vb_k = (creature.valuesBreeding[(int)StatNames.SpeedMultiplier] / 1000).ToString();
            string fort_vb_k = (creature.valuesBreeding[(int)StatNames.TemperatureFortitude] / 1000).ToString();
            string craft_vb_k = (creature.valuesBreeding[(int)StatNames.CraftingSpeedMultiplier] / 1000).ToString();

            string hp_vb_10k = (creature.valuesBreeding[(int)StatNames.Health] / 10000).ToString();
            string stam_vb_10k = (creature.valuesBreeding[(int)StatNames.Stamina] / 10000).ToString();
            string trp_vb_10k = (creature.valuesBreeding[(int)StatNames.Torpidity] / 10000).ToString();
            string oxy_vb_10k = (creature.valuesBreeding[(int)StatNames.Oxygen] / 10000).ToString();
            string food_vb_10k = (creature.valuesBreeding[(int)StatNames.Food] / 10000).ToString();
            string water_vb_10k = (creature.valuesBreeding[(int)StatNames.Water] / 10000).ToString();
            string temp_vb_10k = (creature.valuesBreeding[(int)StatNames.Temperature] / 10000).ToString();
            string weight_vb_10k = (creature.valuesBreeding[(int)StatNames.Weight] / 10000).ToString();
            // dmg need multi 100
            string dmg_vb_10k = ((creature.valuesBreeding[(int)StatNames.MeleeDamageMultiplier] / 100)).ToString();
            string spd_vb_10k = (creature.valuesBreeding[(int)StatNames.SpeedMultiplier] / 10000).ToString();
            string fort_vb_10k = (creature.valuesBreeding[(int)StatNames.TemperatureFortitude] / 10000).ToString();
            string craft_vb_10k = (creature.valuesBreeding[(int)StatNames.CraftingSpeedMultiplier] / 10000).ToString();

            string hp_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Health] )).ToString();
            string stam_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Stamina])).ToString();
            string trp_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Torpidity])).ToString();
            string oxy_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Oxygen])).ToString();
            string food_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Food] )).ToString();
            string water_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Water] )).ToString();
            string temp_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Temperature])).ToString();
            string weight_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.Weight])).ToString();
            // dmg need multi 100
            string dmg_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.MeleeDamageMultiplier] * 100)).ToString();
            string spd_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.SpeedMultiplier])).ToString();
            string fort_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.TemperatureFortitude])).ToString();
            string craft_vb_n = ((int)(creature.valuesBreeding[(int)StatNames.CraftingSpeedMultiplier])).ToString();

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
                hp +
                stam +
                oxy +
                food +
                weight +
                dmg +
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

            string old_name = "";// creature.name;
            if (creature.ArkId != 0)
            {
                foreach (Creature item in speciesCreatures)
                {
                    if (creature.ArkId == item.ArkId)
                    {
                        old_name = item.name;
                        break;
                    }
                }
            } else {
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
            if (creature.ArkId != 0)
            {
                int index_num = 1;
                foreach (Creature item in speciesCreatures)
                {
                    if (creature.ArkId == item.ArkId)
                    {
                        index_str = index_num.ToString();
                        break;
                    }
                    index_num++;
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
                { "hp" , hp },
                { "stam" ,stam },
                { "trp" , trp },
                { "oxy" , oxy },
                { "food" , food },
                { "water" , water },
                { "temp" , temp },
                { "weight" , weight },
                { "dmg" ,dmg },
                { "spd" , spd },
                { "fort" , fort },
                { "craft" , craft },

                { "hp_vb" , hp_vb },
                { "stam_vb" ,stam_vb },
                { "trp_vb" , trp_vb },
                { "oxy_vb" , oxy_vb },
                { "food_vb" , food_vb },
                { "water_vb" , water_vb },
                { "temp_vb" , temp_vb },
                { "weight_vb" , weight_vb },
                { "dmg_vb" ,dmg_vb },
                { "spd_vb" , spd_vb },
                { "fort_vb" , fort_vb },
                { "craft_vb" , craft_vb },

                { "hp_vb_k" , hp_vb_k },
                { "stam_vb_k" ,stam_vb_k },
                { "trp_vb_k" , trp_vb_k },
                { "oxy_vb_k" , oxy_vb_k },
                { "food_vb_k" , food_vb_k },
                { "water_vb_k" , water_vb_k },
                { "temp_vb_k" , temp_vb_k },
                { "weight_vb_k" , weight_vb_k },
                { "dmg_vb_k" ,dmg_vb_k },
                { "spd_vb_k" , spd_vb_k },
                { "fort_vb_k" , fort_vb_k },
                { "craft_vb_k" , craft_vb_k },

                { "hp_vb_10k" , hp_vb_10k },
                { "stam_vb_10k" ,stam_vb_10k },
                { "trp_vb_10k" , trp_vb_10k },
                { "oxy_vb_10k" , oxy_vb_10k },
                { "food_vb_10k" , food_vb_10k },
                { "water_vb_10k" , water_vb_10k },
                { "temp_vb_10k" , temp_vb_10k },
                { "weight_vb_10k" , weight_vb_10k },
                { "dmg_vb_10k" ,dmg_vb_10k },
                { "spd_vb_10k" , spd_vb_10k },
                { "fort_vb_10k" , fort_vb_10k },
                { "craft_vb_10k" , craft_vb_10k },

                { "hp_vb_n" , hp_vb_n },
                { "stam_vb_n" ,stam_vb_n },
                { "trp_vb_n" , trp_vb_n },
                { "oxy_vb_n" , oxy_vb_n },
                { "food_vb_n" , food_vb_n },
                { "water_vb_n" , water_vb_n },
                { "temp_vb_n" , temp_vb_n },
                { "weight_vb_n" , weight_vb_n },
                { "dmg_vb_n" ,dmg_vb_n },
                { "spd_vb_n" , spd_vb_n },
                { "fort_vb_n" , fort_vb_n },
                { "craft_vb_n" , craft_vb_n },

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
        private static int min(int a, int b)
        {
            if (a > b)
                return b;
            return a;
        }

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

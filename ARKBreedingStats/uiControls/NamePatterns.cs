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
        /// <summary>
        /// Generates a creature name with a given pattern
        /// </summary>
        public static string generateCreatureName(Creature creature, List<Creature> females, List<Creature> males, bool showDuplicateNameWarning)
        {
            //try
            //{
            // collect creatures of the same species
            List<Creature> sameSpecies = (females ?? new List<Creature>()).Concat(males ?? new List<Creature>()).ToList();
            List<string> creatureNames = sameSpecies.Select(x => x.name).ToList();

            Dictionary<string, string> tokenDictionary = CreateTokenDictionary(creature, sameSpecies);
            string resolvedPattern = ResolveConditions(creature, Properties.Settings.Default.sequentialUniqueNamePattern);
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
            string baselvl = creature.LevelHatched.ToString().PadLeft(2, '0');
            string dom = creature.isBred ? "B" : "T";

            double imp = creature.imprintingBonus * 100;
            double eff = creature.tamingEff * 100;

            Random rand = new Random(DateTime.Now.Millisecond);
            string randStr = rand.Next(100000, 999999).ToString();

            string effImp = "Z";
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

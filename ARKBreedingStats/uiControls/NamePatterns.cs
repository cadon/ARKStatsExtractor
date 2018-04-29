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
        static public string generateCreatureName(Creature creature, List<Creature> females, List<Creature> males)
        {
            try
            {
                // collect creatures of the same species
                var sameSpecies = (females ?? new List<Creature> { }).Concat((males ?? new List<Creature> { })).ToList();
                var creatureNames = sameSpecies.Select(x => x.name).ToList();

                var tokenDictionary = createTokenDictionary(creature, creatureNames);
                var name = assemblePatternedName(tokenDictionary);

                if (name.Contains("{n}"))
                {
                    // find the sequence token, and if not, return because the configurated pattern string is invalid without it
                    var index = name.IndexOf("{n}", StringComparison.OrdinalIgnoreCase);
                    var patternStart = name.Substring(0, index);
                    var patternEnd = name.Substring(index + 3);

                    // loop until we find a unique name in the sequence which is not taken

                    var n = 1;
                    do
                    {
                        name = string.Concat(patternStart, (n < 10 ? "0" : "") + n, patternEnd);
                        n++;
                    } while (creatureNames.Contains(name, StringComparer.OrdinalIgnoreCase));
                }

                if (creatureNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    MessageBox.Show("WARNING: The generated name for the creature already exists in the database.");
                }
                else if (name.Length > 24)
                {
                    MessageBox.Show("WARNING: The generated name is longer than 24 characters, ingame-preview:" + name.Substring(0, 24));
                }

                return name;
            }
            catch
            {
                MessageBox.Show("There was an error while generating the creature name.");
            }
            return "";
        }

        /// <summary>        
        /// This method creates the token dictionary for the dynamic creature name generation.
        /// </summary>
        /// <param name="creature">Creature with the data</param>
        /// <param name="creatureNames">A list of all names of the currently stored creatures of the species</param>
        /// <returns>A dictionary containing all tokens and their replacements</returns>
        static public Dictionary<string, string> createTokenDictionary(Creature creature, List<string> creatureNames)
        {
            var date_short = DateTime.Now.ToString("yy-MM-dd");
            var date_compressed = date_short.Replace("-", "");
            var time_short = DateTime.Now.ToString("hh:mm:ss");
            var time_compressed = time_short.Replace(":", "");

            string hp = creature.levelsWild[0].ToString().PadLeft(2, '0');
            string stam = creature.levelsWild[1].ToString().PadLeft(2, '0');
            string oxy = creature.levelsWild[2].ToString().PadLeft(2, '0');
            string food = creature.levelsWild[3].ToString().PadLeft(2, '0');
            string weight = creature.levelsWild[4].ToString().PadLeft(2, '0');
            string dmg = creature.levelsWild[5].ToString().PadLeft(2, '0');
            string spd = creature.levelsWild[6].ToString().PadLeft(2, '0');
            string trp = creature.levelsWild[7].ToString().PadLeft(2, '0');
            string baselvl = (creature.levelsWild[7] + 1).ToString().PadLeft(2, '0');

            double imp = creature.imprintingBonus * 100;
            double eff = creature.tamingEff * 100;

            var rand = new Random(DateTime.Now.Millisecond);
            var randStr = rand.Next(100000, 999999).ToString();

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

            var precompressed =
                creature.sex.ToString().Substring(0, 1) +
                date_compressed +
                hp +
                stam +
                oxy +
                food +
                weight +
                dmg +
                effImp;

            var mutasn = creature.mutationsMaternal + creature.mutationsPaternal;
            string mutas;
            if (mutasn > 99)
                mutas = "99";
            else
                mutas = mutasn.ToString().PadLeft(2, '0');

            var spcShort = creature.species.Replace(" ", "");
            var speciesShort = spcShort;
            var vowels = new string[] { "a", "e", "i", "o", "u" };
            while (spcShort.Length > 4 && spcShort.LastIndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u' }) > 0)
                spcShort = spcShort.Remove(spcShort.LastIndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u' }), 1); // remove last vowel (not the first letter)
            spcShort = spcShort.Substring(0, Math.Min(4, spcShort.Length));

            speciesShort = speciesShort.Substring(0, Math.Min(4, speciesShort.Length));
            int speciesCount = (creatureNames.Count + 1);

            // replace tokens in user configurated pattern string
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "species", creature.species },
                { "spcs_short", spcShort },
                { "spcs_shortu", spcShort.ToUpper() },
                { "species_short", speciesShort },
                { "species_shortu", speciesShort.ToUpper() },
                { "sex", creature.sex.ToString() },
                { "sex_short", creature.sex.ToString().Substring(0, 1) },
                { "cpr" , precompressed },
                { "date_short" ,  date_short },
                { "date_compressed" , date_compressed },
                { "times_short" , time_short },
                { "times_compressed" , time_compressed },
                { "time_short",time_short.Substring(0,5)},
                { "time_compressed",time_compressed.Substring(0,4)},
                { "hp" , hp },
                { "stam" ,stam },
                { "oxy" , oxy },
                { "food" , food },
                { "weight" , weight },
                { "dmg" ,dmg },
                { "spd" , spd },
                { "trp" , trp },
                { "baselvl" , baselvl },
                { "effImp" , effImp },
                { "muta", mutas},
                { "gen",generation.ToString().PadLeft(3,'0')},
                { "gena",dec2hexvig(generation).PadLeft(2,'0')},
                { "rnd", randStr },
                { "tn", ((speciesCount < 10 ? "0" : "") + speciesCount.ToString()) }
            };
        }

        static private string dec2hexvig(int number)
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
        static private string assemblePatternedName(Dictionary<string, string> tokenDictionary)
        {
            var regularExpression = "\\{(?<key>" + string.Join("|", tokenDictionary.Keys.Select(x => Regex.Escape(x))) + ")\\}";
            var regularExpressionOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
            var r = new Regex(regularExpression, regularExpressionOptions);

            string savedPattern = Properties.Settings.Default.sequentialUniqueNamePattern;

            return r.Replace(savedPattern, (m) =>
            {
                string replacement = null;
                return tokenDictionary.TryGetValue(m.Groups["key"].Value, out replacement) ? replacement : m.Value;
            });
        }
    }
}

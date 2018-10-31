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
            //try
            //{
            // collect creatures of the same species
            var sameSpecies = (females ?? new List<Creature> { }).Concat((males ?? new List<Creature> { })).ToList();
            var creatureNames = sameSpecies.Select(x => x.name).ToList();

            var tokenDictionary = CreateTokenDictionary(creature, sameSpecies);
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
            //}
            //catch
            //{
            //    MessageBox.Show("There was an error while generating the creature name.");
            //}
            return "";
        }

        /// <summary>        
        /// This method creates the token dictionary for the dynamic creature name generation.
        /// </summary>
        /// <param name="creature">Creature with the data</param>
        /// <param name="speciesCreatures">A list of all currently stored creatures of the species</param>
        /// <returns>A dictionary containing all tokens and their replacements</returns>
        static public Dictionary<string, string> CreateTokenDictionary(Creature creature, List<Creature> speciesCreatures)
        {
            var yyyy = DateTime.Now.ToString("yyyy");
            var yy = DateTime.Now.ToString("yy");
            var MM = DateTime.Now.ToString("MM");
            var dd = DateTime.Now.ToString("dd");
            var hh = DateTime.Now.ToString("hh");
            var mm = DateTime.Now.ToString("mm");
            var ss = DateTime.Now.ToString("ss");

            var date = DateTime.Now.ToString("yy-MM-dd");
            var time = DateTime.Now.ToString("hh:mm:ss");

            string hp = creature.levelsWild[0].ToString().PadLeft(2, '0');
            string stam = creature.levelsWild[1].ToString().PadLeft(2, '0');
            string oxy = creature.levelsWild[2].ToString().PadLeft(2, '0');
            string food = creature.levelsWild[3].ToString().PadLeft(2, '0');
            string weight = creature.levelsWild[4].ToString().PadLeft(2, '0');
            string dmg = creature.levelsWild[5].ToString().PadLeft(2, '0');
            string spd = creature.levelsWild[6].ToString().PadLeft(2, '0');
            string trp = creature.levelsWild[7].ToString().PadLeft(2, '0');
            string baselvl = (creature.levelsWild[7] + 1).ToString().PadLeft(2, '0');
            string dom = (creature.isBred ? "B" : "T");

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
                yy + MM + dd +
                hp +
                stam +
                oxy +
                food +
                weight +
                dmg +
                effImp;

            var mutasn = creature.Mutations;
            string mutas;
            if (mutasn > 99)
                mutas = "99";
            else
                mutas = mutasn.ToString().PadLeft(2, '0');

            var firstWordOfOldest = "";
            if (speciesCreatures.Count > 0)
            {
                firstWordOfOldest = speciesCreatures.OrderBy(s => s.addedToLibrary).First().name;
                if (!string.IsNullOrEmpty(firstWordOfOldest) && firstWordOfOldest.Contains(" "))
                {
                    firstWordOfOldest = firstWordOfOldest.Substring(0, firstWordOfOldest.IndexOf(" "));
                }
            }

            var speciesShort6 = creature.species.Replace(" ", "");
            var spcShort = speciesShort6;
            var vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            while (spcShort.Length > 4 && spcShort.LastIndexOfAny(vowels) > 0)
                spcShort = spcShort.Remove(spcShort.LastIndexOfAny(vowels), 1); // remove last vowel (not the first letter)
            spcShort = spcShort.Substring(0, Math.Min(4, spcShort.Length));

            speciesShort6 = speciesShort6.Substring(0, Math.Min(6, speciesShort6.Length));
            var speciesShort5 = speciesShort6.Substring(0, Math.Min(5, speciesShort6.Length));
            var speciesShort4 = speciesShort6.Substring(0, Math.Min(4, speciesShort6.Length));
            int speciesCount = speciesCreatures.Count + 1;
            int speciesSexCount = speciesCreatures.Count(c => c.sex == creature.sex) + 1;

            // replace tokens in user configurated pattern string
            return new Dictionary<string, string>
            {
                { "species", creature.species },
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
                { "cpr" , precompressed },
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
                { "tn", ((speciesCount < 10 ? "0" : "") + speciesCount.ToString())},
                { "sn", ((speciesSexCount < 10 ? "0" : "") + speciesSexCount.ToString())},
                { "dom", dom}
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

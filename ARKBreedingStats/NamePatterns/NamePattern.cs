using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

using static ARKBreedingStats.Library.CreatureCollection;

namespace ARKBreedingStats.NamePatterns
{
    public static class NamePattern
    {
        /// <summary>
        /// The pipe character is used as separator in functions, so it needs to be escaped when used literally.
        /// </summary>
        private const string PipeEscapeSequence = @"\pipe";

        public static Random Random = new Random();
        private static readonly Func<TokenModel, StatModel>[] StatAccessors = {
            m => m.hp, // StatNames.Health;
            m => m.st, // StatNames.Stamina;
            m => m.to, // StatNames.Torpidity;
            m => m.ox, // StatNames.Oxygen;
            m => m.fo, // StatNames.Food;
            m => m.wa, // StatNames.Water;
            m => m.te, // StatNames.Temperature;
            m => m.we, // StatNames.Weight;
            m => m.dm, // StatNames.MeleeDamageMultiplier;
            m => m.sp, // StatNames.SpeedMultiplier;
            m => m.fr, // StatNames.TemperatureFortitude;
            m => m.cr  // StatNames.CraftingSpeedMultiplier;
        };

        /// <summary>
        /// Generate a creature name with the naming pattern.
        /// </summary>
        /// <param name="alreadyExistingCreature">If the creature already exists in the library, null if the creature is new.</param>
        public static string GenerateCreatureName(Creature creature, Creature alreadyExistingCreature, Creature[] sameSpecies, TopLevels topLevels, Dictionary<string, string> customReplacings,
            bool showDuplicateNameWarning = false, int namingPatternIndex = -1, bool showTooLongWarning = true, string pattern = null, bool displayError = true, TokenModel tokenModel = null,
            ColorExisting[] colorsExisting = null, int libraryCreatureCount = 0, Action<string> consoleLog = null)
        {
            if (pattern == null)
            {
                if (namingPatternIndex == -1)
                    pattern = string.Empty;
                else
                    pattern = Properties.Settings.Default.NamingPatterns?[namingPatternIndex] ?? string.Empty;
            }

            var levelsWildHighest = topLevels?.WildLevelsHighest;

            if (creature.topness == 0)
            {
                if (levelsWildHighest == null)
                {
                    creature.topness = 1000;
                }
                else
                {
                    int topLevelSum = 0;
                    int creatureLevelSum = 0;
                    for (int s = 0; s < Stats.StatsCount; s++)
                    {
                        if (s != Stats.Torpidity
                            && creature.Species.UsesStat(s)
                            && (Properties.Settings.Default.consideredStats & (1 << s)) != 0
                            )
                        {
                            int creatureLevel = Math.Max(0, creature.levelsWild[s]);
                            topLevelSum += Math.Max(creatureLevel, levelsWildHighest[s]);
                            creatureLevelSum += creatureLevel;
                        }
                    }
                    if (topLevelSum != 0)
                        creature.topness = (short)(creatureLevelSum * 1000f / topLevelSum);
                    else creature.topness = 1000;
                }

                if (tokenModel != null)
                    tokenModel.toppercent = creature.topness / 10f;
            }

            if (tokenModel == null)
                tokenModel = CreateTokenModel(creature, alreadyExistingCreature, sameSpecies, colorsExisting, topLevels, libraryCreatureCount);

            string name;

            string[] creatureNames = null;

            var shebangMatch = JavaScriptNamePattern.JavaScriptShebang.Match(pattern);

            if (showDuplicateNameWarning || pattern.Contains("{n}") || shebangMatch.Success)
                creatureNames = sameSpecies?.Where(c => c.guid != creature.guid).Select(x => x.name).ToArray() ?? Array.Empty<string>();

            if (shebangMatch.Success)
            {
                try
                {
                    name = JavaScriptNamePattern.ResolveJavaScript(pattern.Substring(shebangMatch.Length), creature,
                        tokenModel, customReplacings, colorsExisting, creatureNames, displayError, consoleLog);
                }
                catch (FileNotFoundException ex)
                {
                    // Jint.dll not installed
                    MessageBoxes.ExceptionMessageBox(ex, "Probably a needed module is not installed for using the javascript pattern. You can install it via the menu Settings - Extra data.");
                    return null;
                }
            }
            else
            {
                name = ResolveTemplate(pattern, creature, tokenModel, customReplacings, colorsExisting, creatureNames, displayError);
            }
            if (showDuplicateNameWarning && creatureNames.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show($"The generated name for the creature\n{name}\nalready exists in the library.\n\nConsider adding {{n}} or {{sn}} in the pattern to generate unique names.", "Name already exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (showTooLongWarning && name.Length > Ark.MaxCreatureNameLength)
            {
                MessageBox.Show($"The generated name is longer than {Ark.MaxCreatureNameLength} characters, the name will look like this in game:\n" + name.Substring(0, Ark.MaxCreatureNameLength), "Name too long for game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return name;
        }

        private static string ResolveTemplate(string pattern, Creature creature, TokenModel tokenModel, Dictionary<string, string> customReplacings, ColorExisting[] colorsExisting, string[] creatureNames, bool displayError)
        {
            var tokenDictionary = CreateTokenDictionary(tokenModel);
            // first resolve keys, then functions
            string name = ResolveFunctions(
                ResolveKeysToValues(tokenDictionary, pattern.Replace("\r", string.Empty).Replace("\n", string.Empty)),
                creature, customReplacings, displayError, false, colorsExisting);
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
                        creature, customReplacings, displayError, true, colorsExisting);

                    // check if numberedUniqueName actually is different, else break the potentially infinite loop. E.g. it is not different if {n} is an unreached if branch or was altered with other functions
                    if (numberedUniqueName == lastNumberedUniqueName) break;

                    lastNumberedUniqueName = numberedUniqueName;
                } while (creatureNames.Contains(numberedUniqueName, StringComparer.OrdinalIgnoreCase));
                name = numberedUniqueName;
            }

            // evaluate escaped characters
            name = name != null ? NamePatternFunctions.UnEscapeSpecialCharacters(name.Replace(PipeEscapeSequence, "|")) : string.Empty;
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
        private static string ResolveFunctions(string pattern, Creature creature, Dictionary<string, string> customReplacings, bool displayError, bool processNumberField, ColorExisting[] colorsExisting = null)
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
                ProcessNumberField = processNumberField,
                ColorsExisting = colorsExisting
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

        internal static readonly string[] StatAbbreviationFromIndex = {
            "hp", // StatNames.Health;
            "st", // StatNames.Stamina;
            "to", // StatNames.Torpidity;
            "ox", // StatNames.Oxygen;
            "fo", // StatNames.Food;
            "wa", // StatNames.Water;
            "te", // StatNames.Temperature;
            "we", // StatNames.Weight;
            "dm", // StatNames.MeleeDamageMultiplier;
            "sp", // StatNames.SpeedMultiplier;
            "fr", // StatNames.TemperatureFortitude;
            "cr"  // StatNames.CraftingSpeedMultiplier;
        };

        /// <summary>        
        /// This method creates the token model for the dynamic creature name generation.
        /// </summary>
        /// <param name="creature">Creature with the data</param>
        /// <param name="alreadyExistingCreature">If the creature is already existing in the library, i.e. if the name is created for a creature that is updated</param>
        /// <param name="speciesCreatures">A list of all currently stored creatures of the species</param>
        /// <param name="topLevels">top levels of that species</param>
        /// <returns>A strongly typed model containing all tokens and their values</returns>
        public static TokenModel CreateTokenModel(Creature creature, Creature alreadyExistingCreature, Creature[] speciesCreatures, ColorExisting[] colorExistings, TopLevels topLevels, int libraryCreatureCount)
        {
            string dom = creature.isBred ? "B" : "T";
            double imp = creature.imprintingBonus * 100;
            double eff = creature.tamingEff * 100;

            int? effImp;
            string prefix;
            if (creature.isBred)
            {
                prefix = "I";
                effImp = (int)Math.Round(imp);
            }
            else if (eff > 1)
            {
                prefix = "E";
                effImp = (int)Math.Round(eff);
            }
            else
            {
                prefix = "Z";
                effImp = null;
            }

            int generation = creature.generation;
            if (generation <= 0)
                generation = Math.Max(
                    creature.Mother?.generation + 1 ?? 0,
                    creature.Father?.generation + 1 ?? 0
                );

            string oldName = creature.name;

            speciesCreatures = speciesCreatures?.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder)).ToArray();

            string firstWordOfOldest = string.Empty;
            if (speciesCreatures?.Any() ?? false)
            {
                firstWordOfOldest = speciesCreatures.Where(c => c.addedToLibrary != null).OrderBy(c => c.addedToLibrary).FirstOrDefault()?.name;
                if (!string.IsNullOrEmpty(firstWordOfOldest) && firstWordOfOldest.Contains(" "))
                {
                    firstWordOfOldest = firstWordOfOldest.Substring(0, firstWordOfOldest.IndexOf(" "));
                }

                if (creature.guid != Guid.Empty)
                {
                    oldName = (alreadyExistingCreature != null ? alreadyExistingCreature.name : creature.name) ?? string.Empty;
                }
                else if (creature.ArkId != 0)
                {
                    oldName = speciesCreatures.FirstOrDefault(c => c.ArkId == creature.ArkId)?.name ?? creature.name;
                }
            }
            // escape special characters
            oldName = oldName.Replace("|", PipeEscapeSequence);

            var speciesName = creature.SpeciesName;
            string spcsNm = speciesName;
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            while (spcsNm.LastIndexOfAny(vowels) > 0)
                spcsNm = spcsNm.Remove(spcsNm.LastIndexOfAny(vowels), 1); // remove last vowel (not the first letter)

            // for counting, add 1 if the creature is not yet in the library
            var addOne = alreadyExistingCreature == null ? 1 : 0;
            int speciesCount = (speciesCreatures?.Length ?? 0) + addOne;
            if (addOne == 1) libraryCreatureCount++;
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

            int index = 0;
            if (creature.guid != Guid.Empty && (speciesCreatures?.Any() ?? false))
            {
                for (int i = 0; i < speciesCreatures.Length; i++)
                {
                    if (creature.guid == speciesCreatures[i].guid)
                    {
                        index = i + 1;
                        break;
                    }
                }
            }

            // replace tokens in user configured pattern string
            // keys have to be all lower case

            var model = new TokenModel
            {
                species = speciesName,
                spcsnm = spcsNm,
                firstwordofoldest = firstWordOfOldest,

                owner = creature.owner,
                tribe = creature.tribe,
                server = creature.server,

                sex = creature.sex,
                sex_short = creature.sex.ToString().Substring(0, 1),

                effimp_short = effImp.HasValue ? effImp.ToString() : prefix,
                index = index,
                oldname = oldName,
                sex_lang = Loc.S(creature.sex.ToString()),
                sex_lang_short = Loc.S(creature.sex.ToString()).Substring(0, 1),
                sex_lang_gen = Loc.S(creature.sex.ToString() + "_gen"),
                sex_lang_short_gen = Loc.S(creature.sex.ToString() + "_gen").Substring(0, 1),

                toppercent = (creature.topness / 10f),
                baselvl = creature.LevelHatched,
                levelpretamed = creature.levelFound,
                effimp = $"{prefix}{effImp}",
                effimp_value = effImp,
                muta = creature.Mutations,
                mutam = creature.mutationsMaternal,
                mutap = creature.mutationsPaternal,
                gen = generation,
                gena = Dec2Hexvig(generation),
                genn = (speciesCreatures?.Count(c => c.generation == generation) ?? 0 + 1),
                nr_in_gen = nrInGeneration,
                nr_in_gen_sex = nrInGenerationAndSameSex,
                rnd = Random.Next(0, 999999),
                ln = libraryCreatureCount,
                tn = speciesCount,
                sn = speciesSexCount,
                dom = dom,
                arkid = arkid,
                alreadyexists = speciesCreatures?.Contains(creature) ?? false,
                isflyer = creature.Species.isFlyer,
                status = creature.Status,
            };

            // stat index and according wild and mutation level
            var levelOrderWild = new List<(int, int)>(7);
            var levelOrderMutated = new List<(int, int)>(7);
            for (int si = 0; si < Stats.StatsCount; si++)
            {
                if (si == Stats.Torpidity || !creature.Species.UsesStat(si)) continue;
                levelOrderWild.Add((si, creature.levelsWild[si]));
                levelOrderMutated.Add((si, creature.levelsMutated?[si] ?? 0));
            }
            levelOrderWild = levelOrderWild.OrderByDescending(l => l.Item2).ToList();
            levelOrderMutated = levelOrderMutated.OrderByDescending(l => l.Item2).ToList();
            var usedStatsCount = levelOrderWild.Count;

            if (topLevels == null) topLevels = new TopLevels();
            var wildLevelsHighest = topLevels.WildLevelsHighest;
            var wildLevelsLowest = topLevels.WildLevelsLowest;
            var mutationLevelsHighest = topLevels.MutationLevelsHighest;
            var mutationLevelsLowest = topLevels.MutationLevelsLowest;

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var statSet = StatAccessors[s](model);
                statSet.level = creature.levelsWild[s];
                statSet.level_m = creature.levelsMutated?[s] ?? 0;
                statSet.level_vb = creature.valuesBreeding[s] * (Stats.IsPercentage(s) ? 100 : 1);
                statSet.istop = creature.levelsWild[s] != -1 && creature.levelsWild[s] >= wildLevelsHighest[s];
                statSet.isnewtop = creature.levelsWild[s] != -1 && creature.levelsWild[s] > wildLevelsHighest[s];
                statSet.islowest = creature.levelsWild[s] != -1 && creature.levelsWild[s] <= wildLevelsLowest[s];
                statSet.isnewlowest = creature.levelsWild[s] != -1 && creature.levelsWild[s] < wildLevelsLowest[s];
                statSet.istop_m = creature.levelsMutated[s] >= mutationLevelsHighest[s];
                statSet.isnewtop_m = creature.levelsMutated[s] > mutationLevelsHighest[s];
                statSet.islowest_m = creature.levelsMutated[s] <= mutationLevelsLowest[s];
                statSet.isnewlowest_m = creature.levelsMutated[s] < mutationLevelsLowest[s];

                // highest stats and according levels
                model.highest_l[s] = s < usedStatsCount ? levelOrderWild[s].Item2.ToString() : string.Empty;
                model.highest_s[s] = s < usedStatsCount ? Utils.StatName(levelOrderWild[s].Item1, true, creature.Species.statNames) : string.Empty;
                model.highest_l_m[s] = s < usedStatsCount ? levelOrderMutated[s].Item2.ToString() : string.Empty;
                model.highest_s_m[s] = s < usedStatsCount ? Utils.StatName(levelOrderMutated[s].Item1, true, creature.Species.statNames) : string.Empty;
            }

            for (int i = 0; i < 6; i++)
            {
                var colorId = creature.colors[i];
                ColorExisting colorExisting = colorExistings != null ? colorExistings[i] : ColorExisting.Unknown;

                model.colors[i] = new ColorModel
                {
                    id = colorId,
                    name = CreatureColors.CreatureColorName(colorId),
                    used = creature.Species.EnabledColorRegions[i],
                    @new = colorExisting == ColorExisting.ColorExistingInOtherRegion ? "newInRegion"
                     : colorExisting == ColorExisting.ColorIsNew ? "newInSpecies"
                     : string.Empty
                };
            }

            return model;
        }

        /// <summary>        
        /// This method creates the token dictionary for the dynamic creature name generation.
        /// </summary>
        /// <param name="model">TokenModel containing the data for the token dictionary</param>
        /// <returns>A dictionary containing all tokens and their replacements</returns>
        public static Dictionary<string, string> CreateTokenDictionary(TokenModel model)
        {
            // replace tokens in user configured pattern string
            // keys have to be all lower case
            var dict = new Dictionary<string, string>
            {
                { "species", model.species },
                { "spcsnm", model.spcsnm },
                { "firstwordofoldest", model.firstwordofoldest },

                { "owner", model.owner },
                { "tribe", model.tribe },
                { "server", model.server },

                { "sex", model.sex.ToString() },
                { "sex_short", model.sex_short },

                { "effimp_short", model.effimp_short },
                { "index", model.index.ToString() },
                { "oldname", model.oldname },
                { "sex_lang", model.sex_lang },
                { "sex_lang_short", model.sex_lang_short },
                { "sex_lang_gen", model.sex_lang_gen },
                { "sex_lang_short_gen", model.sex_lang_short_gen },

                { "toppercent", model.toppercent.ToString() },
                { "baselvl", model.baselvl.ToString() },
                { "levelpretamed", model.levelpretamed.ToString() },
                { "effimp", model.effimp },
                { "muta", model.muta.ToString() },
                { "mutam", model.mutam.ToString() },
                { "mutap", model.mutap.ToString() },
                { "gen", model.gen.ToString() },
                { "gena", model.gena },
                { "genn", model.genn.ToString() },
                { "nr_in_gen", model.nr_in_gen.ToString() },
                { "nr_in_gen_sex", model.nr_in_gen_sex.ToString() },
                { "rnd", model.rnd.ToString("000000") },
                { "ln", model.ln.ToString() },
                { "tn", model.tn.ToString() },
                { "sn", model.sn.ToString() },
                { "dom", model.dom },
                { "arkid", model.arkid },
                { "alreadyexists", model.alreadyexists ? "1" : string.Empty },
                { "isflyer", model.isflyer ? "1" : string.Empty },
                { "status", model.status.ToString() },
            };

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                var stat = StatAccessors[s](model);
                var abbreviation = StatAbbreviationFromIndex[s];

                dict.Add(abbreviation, stat.level.ToString());
                dict.Add($"{abbreviation}_vb", stat.level_vb.ToString());
                dict.Add($"istop{abbreviation}", stat.istop ? "1" : string.Empty);
                dict.Add($"isnewtop{abbreviation}", stat.isnewtop ? "1" : string.Empty);
                dict.Add($"islowest{abbreviation}", stat.islowest ? "1" : string.Empty);
                dict.Add($"isnewlowest{abbreviation}", stat.isnewlowest ? "1" : string.Empty);
                dict.Add($"istop{abbreviation}_m", stat.istop_m ? "1" : string.Empty);
                dict.Add($"isnewtop{abbreviation}_m", stat.isnewtop_m ? "1" : string.Empty);
                dict.Add($"islowest{abbreviation}_m", stat.islowest_m ? "1" : string.Empty);
                dict.Add($"isnewlowest{abbreviation}_m", stat.isnewlowest_m ? "1" : string.Empty);

                // highest stats and according levels
                dict.Add("highest" + (s + 1) + "l", model.highest_l[s]);
                dict.Add("highest" + (s + 1) + "s", model.highest_s[s]);
                dict.Add("highest" + (s + 1) + "l_m", model.highest_l_m[s]);
                dict.Add("highest" + (s + 1) + "s_m", model.highest_s_m[s]);

                // mutated levels
                dict.Add(abbreviation + "_m", stat.level_m.ToString());
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

            return r.Replace(pattern, m => tokenDictionary.TryGetValue(m.Groups["key"].Value.ToLowerInvariant(), out string replacement) ? replacement : m.Value);
        }
    }
}

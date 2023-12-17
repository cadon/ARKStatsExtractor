using ARKBreedingStats.Library;
using ARKBreedingStats.mods;
using ARKBreedingStats.species;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ARKBreedingStats.values
{
    public class Values : ValuesFile
    {
        private static Values _V;

        /// <summary>
        /// Colors used by the loaded library, ordered according to the mod order.
        /// </summary>
        public ArkColors Colors;

        public List<string> speciesNames = new List<string>();
        internal Dictionary<string, string> aliases;
        public List<string> speciesWithAliasesList;
        private Dictionary<string, Species> _blueprintToSpecies;
        private Dictionary<string, Species> _nameToSpecies;
        private Dictionary<string, Species> _classNameToSpecies;

        /// <summary>
        /// Some color regions are not visible in game. If a species defines a region as invisible, it can be hidden in the UI.
        /// </summary>
        public bool InvisibleColorRegionsExist;

        /// <summary>
        /// Representing the current server multipliers except statMultipliers. Also considers event-changes.
        /// </summary>
        public ServerMultipliers currentServerMultipliers;
        /// <summary>
        /// List of presets for server multipliers for easier setting. Also contains the singleplayer multipliers.
        /// </summary>
        public ServerMultipliersPresets serverMultipliersPresets;

        /// <summary>
        /// The default food data used for taming. Specific species can override it.
        /// </summary>
        public Dictionary<string, TamingFood> defaultFoodData;

        /// <summary>
        /// The special food data for species used for taming. Saved to use for loaded mods.
        /// </summary>
        public Dictionary<string, TamingData> specialFoodData;

        /// <summary>
        /// Infos about the available mod values
        /// </summary>
        public ModsManifest modsManifest;

        /// <summary>
        /// Contains all species-classes that should be ignored when importing a savegame.
        /// This is e.g. used to filter out rafts which are species in ARK.
        /// </summary>
        private List<string> ignoreSpeciesClassesOnImport;

        /// <summary>
        /// For the main-values object this hash represents the current loaded mods and their order.
        /// </summary>
        public int loadedModsHash;

        /// <summary>
        /// Hash if no mod is loaded.
        /// </summary>
        public static readonly int NoModsHash = CreatureCollection.CalculateModListHash(new List<Mod>());

        /// <summary>
        /// Is true if species lists are ordered, food is assigned and colors are ordered. If not, call InitializeSpeciesAndColors().
        /// </summary>
        private bool _speciesAndColorsInitialized;

        public static Values V => _V ?? (_V = new Values());

        /// <summary>
        /// Loads the values from the default file.
        /// </summary>
        public Values LoadValues(bool forceReload, out string errorMessage, out string errorMessageTitle)
        {
            errorMessage = null;
            errorMessageTitle = null;

            // if everything is already loaded, don't load it again
            if (!forceReload && loadedModsHash == NoModsHash)
            {
                return this;
            }

            _V = LoadBaseValuesFile(FileService.GetJsonPath(FileService.ValuesFolder, FileService.ValuesJson));
            InitializeBaseValues();

            if (_V.serverMultipliersPresets == null)
            {
                if (!ServerMultipliersPresets.TryLoadServerMultipliersPresets(out _V.serverMultipliersPresets))
                {
                    errorMessage = "The file with the server multiplier presets couldn't be loaded. Changed settings, e.g. for the singleplayer will be not available.\nIt's recommended to download the application again.";
                    errorMessageTitle = "Server multiplier file not loaded";
                }
            }

            // load values from official expansions that are part of the base game but saved in different files
            var expansionModValueFiles = _V.modsManifest.modsByFiles.Values.Where(m => m.mod?.expansion == true)
                .Select(m => m.mod.FileName).ToArray();

            var (missingModValueFilesOnlineAvailable, _, modValueFilesWithAvailableUpdate) = CheckAvailabilityAndUpdateModFiles(expansionModValueFiles);
            _V.modsManifest.DownloadModFiles(missingModValueFilesOnlineAvailable.Concat(modValueFilesWithAvailableUpdate));
            _V.LoadModValues(expansionModValueFiles, false, out _, out _);

            if (!_V._speciesAndColorsInitialized)
                _V.InitializeSpeciesAndColors();

            return _V;
        }

        /// <summary>
        /// Initializes the base values, call after base values are loaded.
        /// </summary>
        private void InitializeBaseValues()
        {
            bool setTamingFood = TamingFoodData.TryLoadDefaultFoodData(out specialFoodData);
            if (specialFoodData == null) _V.specialFoodData = new Dictionary<string, TamingData>();
            else _V.specialFoodData = specialFoodData;

            const string defaultFoodNameKey = "default";
            if (setTamingFood && _V.specialFoodData.TryGetValue(defaultFoodNameKey, out var defaultFoodValues))
            {
                _V.defaultFoodData = defaultFoodValues.specialFoodValues;
            }
            else
            {
                _V.defaultFoodData = new Dictionary<string, TamingFood>();
            }

            _V.loadedModsHash = NoModsHash;

            // transfer extra loaded objects from the old object to the new one if values is reloaded
            _V.modsManifest = modsManifest;
            _V.serverMultipliersPresets = serverMultipliersPresets;
            _V.Colors = new ArkColors(_V.ArkColorsDyesParsed);
        }

        /// <summary>
        /// Sets food for species, orders species, orders and initializes colors. Call after all values and mod values are loaded.
        /// </summary>
        private void InitializeSpeciesAndColors()
        {
            //var speciesWoFoodData = new List<string>(); // to determine which species has no food data yet
            if (specialFoodData != null)
            {
                foreach (Species sp in _V.species)
                {
                    if (specialFoodData.ContainsKey(sp.name))
                    {
                        sp.taming.eats = specialFoodData[sp.name].eats;
                        sp.taming.eatsAlsoPostTame = specialFoodData[sp.name].eatsAlsoPostTame;
                        sp.taming.specialFoodValues = specialFoodData[sp.name].specialFoodValues;
                    }
                    //if (sp.IsDomesticable && !specialFoodData.ContainsKey(sp.name)) speciesWoFoodData.Add(sp.name);
                }
                //System.Windows.Forms.Clipboard.SetText(speciesWoFoodData.Any() ? string.Join("\n", speciesWoFoodData) : string.Empty);
            }

            OrderSpeciesAndApplyCustomVariants();
            LoadAndInitializeAliases();
            UpdateSpeciesBlueprintDictionaries();

            InitializeArkColors();
            _speciesAndColorsInitialized = true;
        }

        /// <summary>
        /// Loads extra values-files that can add species values or modify existing ones. Returns true if species were added.
        /// </summary>
        public bool LoadModValues(IEnumerable<string> modFilesToLoad, bool throwExceptionOnFail, out List<Mod> loadedMods, out string resultsMessage)
        {
            loadedModsHash = 0;
            var modifiedValues = new List<ValuesFile>();

            loadedMods = new List<Mod>();
            resultsMessage = null;
            if (modFilesToLoad == null) return false;

            StringBuilder resultsMessageSb = new StringBuilder();
            foreach (var modFileToLoad in modFilesToLoad)
            {
                if (string.IsNullOrEmpty(modFileToLoad))
                {
                    modifiedValues.Add(new ValuesFile { mod = Mod.OtherMod });
                    continue;
                }

                string filename = FileService.GetJsonPath(Path.Combine(FileService.ValuesFolder, modFileToLoad));

                if (TryLoadValuesFile(filename, true, false, out var modValues, out var modFileErrorMessage, true))
                {
                    modifiedValues.Add(modValues);
                }
                else if (!string.IsNullOrEmpty(modFileErrorMessage))
                {
                    resultsMessageSb.AppendLine(modFileErrorMessage);
                }
            }

            int speciesAddedCount = 0;
            var colorsAdded = false;

            var blueprintPathDuplicateChecking = _V.species.ToDictionary(s => s.blueprintPath);

            // update data if existing
            foreach (var modValues in modifiedValues)
            {
                // if mods are loaded multiple times, only keep the first
                if (loadedMods.Contains(modValues.mod)) continue;

                loadedMods.Add(modValues.mod);

                // species
                if (modValues.species != null)
                {
                    foreach (Species sp in modValues.species)
                    {
                        if (string.IsNullOrWhiteSpace(sp.blueprintPath)) continue;

                        speciesAddedCount++;

                        if (blueprintPathDuplicateChecking.TryGetValue(sp.blueprintPath, out var originalSpecies))
                        {
                            originalSpecies.LoadOverrides(sp);
                            originalSpecies.Mod = modValues.mod;
                        }
                        else
                        {
                            blueprintPathDuplicateChecking[sp.blueprintPath] = sp;
                            _V.species.Add(sp);
                            sp.Mod = modValues.mod;
                        }
                    }
                }

                // mod colors (even if the mod doesn't add colors, the order of colors can change)
                if (!modValues.mod.expansion)
                {
                    Colors.AddModArkColors((modValues.ArkColorsDyesParsed, modValues.dyeStartIndex));
                    colorsAdded = true;
                }

                // mod food data TODO
            }

            loadedModsHash = CreatureCollection.CalculateModListHash(loadedMods.Where(m => !m.expansion));

            resultsMessageSb.AppendLine($"The following mods were loaded:\n\n- {string.Join("\n- ", modifiedValues.Select(m => m.mod.title).ToArray())}\n\n"
                                        + $"Species added: {speciesAddedCount}");
            resultsMessage = resultsMessageSb.ToString();

            if (!colorsAdded && speciesAddedCount == 0)
            {
                resultsMessage = resultsMessageSb.ToString();
                // nothing changed
                return false;
            }

            InitializeSpeciesAndColors();

            return true;
        }

        private void InitializeArkColors()
        {
            _V.Colors.InitializeArkColors();
            foreach (var s in _V.species)
                s.InitializeColors(_V.Colors);
            _V.InvisibleColorRegionsExist = _V.species.Any(s => s.colors?.Any(r => r?.invisible == true) == true);
        }

        /// <summary>
        /// Check if all mod files are available and up to date, and download the ones not available locally.
        /// </summary>
        /// <param name="modValueFileNames"></param>
        internal (List<string> missingModValueFilesOnlineAvailable, List<string> missingModValueFilesOnlineNotAvailable, List<string> modValueFilesWithAvailableUpdate)
            CheckAvailabilityAndUpdateModFiles(IEnumerable<string> modValueFileNames)
        {
            if (modsManifest == null) throw new ArgumentNullException(nameof(modsManifest));

            List<string> missingModValueFilesOnlineAvailable = new List<string>();
            List<string> missingModValueFilesOnlineNotAvailable = new List<string>();
            List<string> modValueFilesWithAvailableUpdate = new List<string>();

            string valuesFolder = FileService.GetJsonPath(FileService.ValuesFolder);

            foreach (var mf in modValueFileNames)
            {
                if (string.IsNullOrEmpty(mf)) continue;

                string modFilePath = Path.Combine(valuesFolder, mf);
                modsManifest.modsByFiles.TryGetValue(mf, out var modInfo);

                if (!File.Exists(modFilePath))
                {
                    if (modInfo != null
                        && modInfo.OnlineAvailable
                        && IsValidFormatVersion(modInfo.format))
                        missingModValueFilesOnlineAvailable.Add(mf);
                    else
                        missingModValueFilesOnlineNotAvailable.Add(mf);
                }
                else if (modInfo != null)
                {
                    // check if an update is available
                    if (modInfo.OnlineAvailable
                        && IsValidFormatVersion(modInfo.format)
                        && modInfo.Version != null
                        && (!TryLoadValuesFile(modFilePath, setModFileName: false, throwExceptionOnFail: false,
                            out ValuesFile modValues, errorMessage: out _)
                            || modValues.Version < modsManifest.modsByFiles[mf].Version))
                    {
                        modValueFilesWithAvailableUpdate.Add(mf);
                    }
                }
            }

            return (missingModValueFilesOnlineAvailable,
                    missingModValueFilesOnlineNotAvailable,
                    modValueFilesWithAvailableUpdate);
        }

        private string SpeciesNameSortFilePath => FileService.GetJsonPath("sortNames.txt");

        public void ResetDefaultSpeciesNameSorting()
        {
            string filePath = SpeciesNameSortFilePath;

            try
            {
                File.WriteAllText(filePath, "^(Aberrant |Tek |R\\-|X\\-)(.*)$@$2$1\n");
                ApplySpeciesOrdering();
            }
            catch
            {
                // ignored
            }
        }

        public void ResetSpeciesNameSorting()
        {
            string filePath = SpeciesNameSortFilePath;
            if (FileService.TryDeleteFile(filePath))
                ApplySpeciesOrdering();
        }

        public void OpenSpeciesNameSortingFile()
        {
            string filePath = SpeciesNameSortFilePath;
            if (!File.Exists(filePath))
                File.WriteAllText(filePath, string.Empty);
            if (File.Exists(filePath))
                Process.Start(filePath);
        }

        /// <summary>
        /// If the passed species is not yet set in the species order file an entry is added, if it's present it's removed.
        /// </summary>
        /// <param name="species"></param>
        /// <returns>If the species is a favorite now.</returns>
        public void ToggleSpeciesFavorite(Species species)
        {
            string filePath = SpeciesNameSortFilePath;
            List<string> lines;
            if (!File.Exists(filePath))
                lines = new List<string>();
            else lines = File.ReadAllLines(filePath).ToList();

            const string favoritePrefix = "!fav_";
            // check if species is already a favorite
            var favoriteOrderEntry = species.name + "@" + favoritePrefix + species.name;
            var i = lines.IndexOf(favoriteOrderEntry);
            if (i != -1) lines.RemoveAt(i);
            else lines.Add(favoriteOrderEntry);
            File.WriteAllLines(filePath, lines);
            ApplySpeciesOrdering();
        }

        internal void ApplySpeciesOrdering()
        {
            string filePath = SpeciesNameSortFilePath;

            if (File.Exists(filePath))
            {
                foreach (Species s in _V.species)
                    s.SortName = string.Empty;

                string[] lines = File.ReadAllLines(filePath);
                foreach (string l in lines)
                {
                    if (l.IndexOf("@", StringComparison.Ordinal) <= 0 ||
                        l.IndexOf("@", StringComparison.Ordinal) + 1 >= l.Length)
                        continue;
                    string matchName = l.Substring(0, l.IndexOf("@", StringComparison.Ordinal));
                    string replaceName = l.Substring(l.IndexOf("@", StringComparison.Ordinal) + 1);

                    Regex r = new Regex(matchName);

                    var matchedSpecies = _V.species.Where(s => r.IsMatch(s.name)).ToArray();

                    foreach (Species s in matchedSpecies)
                        s.SortName = r.Replace(s.name, replaceName);
                }

                // set each sortName of species without manual sortName to its speciesName
                foreach (Species s in _V.species)
                {
                    if (string.IsNullOrEmpty(s.SortName))
                        s.SortName = s.DescriptiveNameAndMod;
                }
            }
            else
            {
                foreach (Species s in _V.species)
                {
                    s.SortName = s.DescriptiveNameAndMod;
                }
            }

            _V.species = _V.species.OrderBy(s => s.SortName).ToList();
        }

        private void OrderSpeciesAndApplyCustomVariants()
        {
            ApplySpeciesOrdering();
            _V.speciesNames = _V.species.Select(s => s.name).ToList();

            // apply custom species variants
            var customSpeciesVariantsFilePath = FileService.GetJsonPath(FileService.CustomSpeciesVariants);

            if (File.Exists(customSpeciesVariantsFilePath)
                && FileService.LoadJsonFile(customSpeciesVariantsFilePath,
                    out Dictionary<string, string[]> customSpeciesVariants, out var error))
            {
                if (customSpeciesVariants.Any())
                {
                    foreach (Species sp in _V.species)
                    {
                        if (customSpeciesVariants.TryGetValue(sp.blueprintPath, out var variants))
                        {
                            var spVars = (sp.variants?.ToList() ?? new List<string>());
                            spVars.AddRange(variants);
                            sp.variants = spVars.Any() ? spVars.Distinct().ToArray() : null;
                            sp.InitializeNames();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies the serverMultipliers and creates precalculated species-stats values
        /// </summary>
        public void ApplyMultipliers(CreatureCollection cc, bool eventMultipliers = false, bool applyStatMultipliers = true)
        {
            currentServerMultipliers = (eventMultipliers ? cc.serverMultipliersEvents : cc.serverMultipliers)?.Copy(false);
            if (currentServerMultipliers == null) currentServerMultipliers = V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official);
            if (currentServerMultipliers == null)
            {
                throw new FileNotFoundException("No default server multiplier values found.\nIt's recommend to redownload ARK Smart Breeding.");
            }

            ServerMultipliers singlePlayerServerMultipliers = null;

            if (cc.singlePlayerSettings)
            {
                // The singleplayer multipliers are saved as a regular multiplierpreset, but they work differently
                // in the way they are multiplied on existing multipliers and won't work on their own.
                // The preset name "singleplayer" should only be used for this purpose.
                singlePlayerServerMultipliers = serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Singleplayer);
                if (singlePlayerServerMultipliers == null)
                    throw new FileNotFoundException("No server multiplier values for singleplayer settings found.\nIt's recommend to redownload ARK Smart Breeding.");
            }

            if (singlePlayerServerMultipliers != null)
            {
                currentServerMultipliers.MatingIntervalMultiplier *= singlePlayerServerMultipliers.MatingIntervalMultiplier;
                currentServerMultipliers.EggHatchSpeedMultiplier *= singlePlayerServerMultipliers.EggHatchSpeedMultiplier;
                currentServerMultipliers.BabyMatureSpeedMultiplier *= singlePlayerServerMultipliers.BabyMatureSpeedMultiplier;
                currentServerMultipliers.BabyCuddleIntervalMultiplier *= singlePlayerServerMultipliers.BabyCuddleIntervalMultiplier;
                currentServerMultipliers.TamingSpeedMultiplier *= singlePlayerServerMultipliers.TamingSpeedMultiplier;
            }

            currentServerMultipliers.FixZeroValues();
            double[] defaultMultipliers = new double[] { 1, 1, 1, 1 }; // used if serverMultipliers don't specify non-default values
            var allowSpeedLeveling = cc.serverMultipliers.AllowSpeedLeveling || cc.Game != Ark.Asa;
            var allowFlyerSpeedLeveling = cc.serverMultipliers.AllowFlyerSpeedLeveling;

            foreach (Species sp in species)
            {
                if (applyStatMultipliers)
                {
                    bool customOverrideExists = cc.CustomSpeciesStats?.ContainsKey(sp.blueprintPath) ?? false;
                    double?[][] customFullStatsRaw = customOverrideExists ? cc.CustomSpeciesStats[sp.blueprintPath] : null;
                    bool useSpeedLevelup = currentServerMultipliers.AllowFlyerSpeedLeveling || !sp.isFlyer;

                    // stat-multiplier
                    for (int s = 0; s < Stats.StatsCount; s++)
                    {
                        double[] statMultipliers = cc.serverMultipliers?.statMultipliers?[s] ?? defaultMultipliers;

                        bool customOverrideForThisStatExists = customOverrideExists && customFullStatsRaw[s] != null;

                        sp.stats[s].BaseValue = GetRawStatValue(s, 0, customOverrideForThisStatExists);

                        // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                        double addWhenTamed = GetRawStatValue(s, 3, customOverrideForThisStatExists);
                        sp.stats[s].AddWhenTamed = addWhenTamed * (addWhenTamed > 0 ? statMultipliers[0] : 1);

                        // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                        double multAffinity = GetRawStatValue(s, 4, customOverrideForThisStatExists);
                        sp.stats[s].MultAffinity = multAffinity * (multAffinity > 0 ? statMultipliers[1] : 1);

                        if (useSpeedLevelup || s != Stats.SpeedMultiplier)
                        {
                            sp.stats[s].IncPerTamedLevel = GetRawStatValue(s, 2, customOverrideForThisStatExists) * statMultipliers[2];
                        }
                        else
                        {
                            sp.stats[s].IncPerTamedLevel = 0;
                        }

                        sp.stats[s].IncPerWildLevel = GetRawStatValue(s, 1, customOverrideForThisStatExists) * statMultipliers[3];
                        sp.stats[s].IncPerMutatedLevel = sp.stats[s].IncPerWildLevel; // todo consider adjustments if they're implemented

                        // set troodonism values
                        if (sp.altStats?[s] != null && sp.stats[s].BaseValue != 0)
                        {
                            sp.altStats[s].BaseValue = sp.altBaseStatsRaw[s];

                            // alt / troodonism values depend on the base value
                            var altFactor = sp.altStats[s].BaseValue / sp.stats[s].BaseValue;

                            sp.altStats[s].AddWhenTamed = altFactor * sp.stats[s].AddWhenTamed;
                            sp.altStats[s].MultAffinity = altFactor * sp.stats[s].MultAffinity;
                            sp.altStats[s].IncPerTamedLevel = altFactor * sp.stats[s].IncPerTamedLevel;
                            sp.altStats[s].IncPerWildLevel = altFactor * sp.stats[s].IncPerWildLevel;
                        }

                        // single player adjustments if set and available

                        if (singlePlayerServerMultipliers?.statMultipliers?[s] == null)
                            continue;

                        // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                        sp.stats[s].AddWhenTamed *= sp.stats[s].AddWhenTamed > 0 ? singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexTamingAdd] : 1;
                        // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                        sp.stats[s].MultAffinity *= sp.stats[s].MultAffinity > 0 ? singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexTamingMult] : 1;
                        sp.stats[s].IncPerTamedLevel *= singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexLevelDom];
                        sp.stats[s].IncPerWildLevel *= singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexLevelWild];

                        // troodonism values
                        if (sp.altStats?[s] != null)
                        {
                            sp.altStats[s].AddWhenTamed *= sp.altStats[s].AddWhenTamed > 0
                                ? singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexTamingAdd]
                                : 1;
                            sp.altStats[s].MultAffinity *= sp.altStats[s].MultAffinity > 0
                                ? singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexTamingMult]
                                : 1;
                            sp.altStats[s].IncPerTamedLevel *= singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexLevelDom];
                            sp.altStats[s].IncPerWildLevel *= singlePlayerServerMultipliers.statMultipliers[s][Stats.IndexLevelWild];
                        }

                        double GetRawStatValue(int statIndex, int statValueTypeIndex, bool customOverride)
                        {
                            return customOverride && customFullStatsRaw[statIndex][statValueTypeIndex].HasValue ? customFullStatsRaw[statIndex][statValueTypeIndex].Value : sp.fullStatsRaw[statIndex][statValueTypeIndex];
                        }
                    }

                    // imprinting multiplier override
                    var imprintingMultiplierOverrides =
                        customOverrideExists && cc.CustomSpeciesStats[sp.blueprintPath].Length > Stats.StatsCount
                            ? cc.CustomSpeciesStats[sp.blueprintPath][Stats.StatsCount]
                            : null;

                    sp.SetCustomImprintingMultipliers(imprintingMultiplierOverrides);

                    // ATLAS multipliers

                    if (cc.AtlasSettings)
                    {
                        sp.stats[Stats.Health].BaseValue *= 1.25;
                        sp.stats[Stats.Health].IncPerTamedLevel *= 1.5;
                        sp.stats[Stats.Weight].IncPerTamedLevel *= 1.5;
                        sp.stats[Stats.MeleeDamageMultiplier].IncPerTamedLevel *= 1.5;
                    }
                }

                sp.ApplyCanLevelOptions(allowSpeedLeveling, allowFlyerSpeedLeveling);

                // breeding multiplier
                if (sp.breeding == null)
                    continue;
                if (currentServerMultipliers.EggHatchSpeedMultiplier > 0)
                {
                    sp.breeding.gestationTimeAdjusted = sp.breeding.gestationTime / currentServerMultipliers.EggHatchSpeedMultiplier;
                    sp.breeding.incubationTimeAdjusted = sp.breeding.incubationTime / currentServerMultipliers.EggHatchSpeedMultiplier;
                }
                if (currentServerMultipliers.MatingSpeedMultiplier > 0)
                    sp.breeding.matingTimeAdjusted = sp.breeding.matingTime / currentServerMultipliers.MatingSpeedMultiplier;
                if (currentServerMultipliers.BabyMatureSpeedMultiplier > 0)
                    sp.breeding.maturationTimeAdjusted = sp.breeding.maturationTime / currentServerMultipliers.BabyMatureSpeedMultiplier;

                sp.breeding.matingCooldownMinAdjusted = sp.breeding.matingCooldownMin * currentServerMultipliers.MatingIntervalMultiplier;
                sp.breeding.matingCooldownMaxAdjusted = sp.breeding.matingCooldownMax * currentServerMultipliers.MatingIntervalMultiplier;
            }
        }

        /// <summary>
        /// Loads the species aliases from a file and updates the alias dictionary.
        /// </summary>
        private bool LoadAndInitializeAliases()
        {
            aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            speciesWithAliasesList = new List<string>(speciesNames);

            try
            {
                using (StreamReader reader = FileService.GetJsonFileReader(FileService.AliasesJson))
                {
                    JObject aliasesNode = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    foreach (KeyValuePair<string, JToken> pair in aliasesNode)
                    {
                        string alias = pair.Key;
                        string speciesName = pair.Value.Value<string>();
                        if (speciesNames.Contains(alias, StringComparer.OrdinalIgnoreCase)
                                || !speciesNames.Contains(speciesName, StringComparer.OrdinalIgnoreCase)
                                || aliases.ContainsKey(alias))
                            continue;
                        aliases.Add(alias, speciesName);
                        speciesWithAliasesList.Add(alias);
                    }
                    speciesWithAliasesList.Sort();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // TODO create log-file for this?
                //MessageBox.Show($"Couldn't load {FileService.AliasesJson}\nThe program will continue without it.\n" +
                //        $"Error message:\n\n{e.Message}",
                //        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Creates dictionaries to select species
        /// </summary>
        private void UpdateSpeciesBlueprintDictionaries()
        {
            _blueprintToSpecies = new Dictionary<string, Species>();
            _nameToSpecies = new Dictionary<string, Species>(StringComparer.OrdinalIgnoreCase);
            _classNameToSpecies = new Dictionary<string, Species>(StringComparer.OrdinalIgnoreCase);

            Regex rClassName = new Regex(@"(?<=\.)[^\/\.]+$");

            foreach (Species s in species)
            {
                if (!string.IsNullOrEmpty(s.blueprintPath))
                {
                    _blueprintToSpecies[s.blueprintPath] = s;

                    string speciesName = s.name;
                    if (_nameToSpecies.TryGetValue(speciesName, out var existingSpecies))
                    {
                        if (
                            (!existingSpecies.IsDomesticable && s.IsDomesticable) // prefer species that are domesticable
                            || (existingSpecies.Mod == null && s.Mod != null) // prefer species from mods with the same name
                            || ((existingSpecies.variants?.Length ?? 0) > (s.variants?.Length ?? 0)) // prefer species that are not variants
                        )
                            _nameToSpecies[speciesName] = s;
                    }
                    else
                        _nameToSpecies.Add(speciesName, s);

                    Match classNameMatch = rClassName.Match(s.blueprintPath);
                    if (classNameMatch.Success)
                    {
                        string className = classNameMatch.Value + "_C";
                        if (_classNameToSpecies.ContainsKey(className))
                            _classNameToSpecies[className] = s;
                        else
                            _classNameToSpecies.Add(className, s);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the passed name is an available species name or an alias, then returns the species
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>Available species name or empty, if not available.</returns>
        public string SpeciesName(string alias)
        {
            if (speciesNames.Contains(alias))
                return alias;
            return aliases.TryGetValue(alias, out var speciesName) ? speciesName : string.Empty;
        }

        /// <summary>
        /// Checks species names and loaded aliases for a match and sets the out parameter.
        /// Especially when mods are used, this is not guaranteed to result in the correct species.
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="recognizedSpecies"></param>
        /// <returns>True on success</returns>
        public bool TryGetSpeciesByName(string speciesName, out Species recognizedSpecies)
        {
            recognizedSpecies = null;
            if (string.IsNullOrEmpty(speciesName)) return false;

            if (aliases.TryGetValue(speciesName, out var realSpeciesName))
                speciesName = realSpeciesName;
            if (_nameToSpecies.TryGetValue(speciesName, out var s))
            {
                recognizedSpecies = s;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks species for a matching className.
        /// Especially when mods are used, this is not guaranteed to result in the correct species.
        /// </summary>
        /// <param name="speciesClassName"></param>
        /// <param name="recognizedSpecies"></param>
        /// <returns>True on success</returns>
        public bool TryGetSpeciesByClassName(string speciesClassName, out Species recognizedSpecies)
        {
            recognizedSpecies = null;
            if (string.IsNullOrEmpty(speciesClassName)) return false;

            if (_classNameToSpecies.TryGetValue(speciesClassName, out var s))
            {
                recognizedSpecies = s;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the according species to the passed blueprintPath or null if unknown.
        /// </summary>
        public Species SpeciesByBlueprint(string blueprintPath)
        {
            if (string.IsNullOrEmpty(blueprintPath)) return null;
            if (_blueprintRemapping != null && _blueprintRemapping.TryGetValue(blueprintPath, out var realBlueprintPath))
            {
                blueprintPath = realBlueprintPath;
            }
            return _blueprintToSpecies.TryGetValue(blueprintPath, out var s) ? s : null;
        }

        /// <summary>
        /// Returns the according species to the passed blueprintPath or null if unknown. Removes trailing _C if there.
        /// </summary>
        public Species SpeciesByBlueprint(string blueprintPath, bool removeTrailingC)
        {
            if (removeTrailingC && blueprintPath?.EndsWith("_C") == true)
                return SpeciesByBlueprint(blueprintPath.Substring(0, blueprintPath.Length - 2));
            return SpeciesByBlueprint(blueprintPath);
        }

        /// <summary>
        /// Sets the ModsManifest. If the value is null, a new default object will be created.
        /// </summary>
        /// <param name="mm"></param>
        internal void SetModsManifest(ModsManifest mm)
        {
            modsManifest = mm ?? new ModsManifest();
        }

        private void LoadIgnoreSpeciesClassesFile()
        {
            ignoreSpeciesClassesOnImport = new List<string>();
            try
            {
                using (StreamReader reader = FileService.GetJsonFileReader(FileService.IgnoreSpeciesClasses))
                {
                    JArray aliasesNode = (JArray)JToken.ReadFrom(new JsonTextReader(reader));
                    foreach (string speciesClass in aliasesNode)
                    {
                        if (!ignoreSpeciesClassesOnImport.Contains(speciesClass))
                            ignoreSpeciesClassesOnImport.Add(speciesClass);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // TODO create log file for this?
                //MessageBox.Show($"Couldn't load {FileService.IgnoreSpeciesClasses}\nThe program will continue without it.\n" +
                //        $"Error message:\n\n{e.Message}",
                //        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal List<string> IgnoreSpeciesClassesOnImport
        {
            get
            {
                if (ignoreSpeciesClassesOnImport == null)
                    LoadIgnoreSpeciesClassesFile();
                return ignoreSpeciesClassesOnImport;
            }
        }

        /// <summary>
        /// Checks if a speciesBlueprintpath belongs to a species that should be ignored on import.
        /// </summary>
        /// <param name="speciesBlueprintPath"></param>
        /// <returns></returns>
        internal bool IgnoreSpeciesBlueprint(string speciesBlueprintPath)
        {
            if (string.IsNullOrEmpty(speciesBlueprintPath)) return true;

            // check if species should be ignored (e.g. if it's a raft)
            var m = Regex.Match(speciesBlueprintPath, @"\/([^\/\.]+)\.");
            if (!m.Success) return false;

            string speciesClassString = m.Groups[1].Value;
            if (!speciesClassString.EndsWith("_C")) speciesClassString += "_C";
            return IgnoreSpeciesClassesOnImport.Contains(speciesClassString);
        }

        /// <summary>
        /// Returns the taming food data for a species.
        /// Returns null if no data is found.
        /// </summary>
        internal TamingFood GetTamingFood(Species species, string foodName)
        {
            if (species?.taming?.specialFoodValues != null
                && species.taming.specialFoodValues.TryGetValue(foodName, out var food))
                return food;

            if (defaultFoodData != null
                && defaultFoodData.TryGetValue(foodName, out food))
                return food;
            return null;
        }
    }
}

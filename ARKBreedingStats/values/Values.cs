using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.mods;
using ARKBreedingStats.species;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.values
{
    [DataContract]
    public class Values
    {
        public const int STATS_COUNT = 12;
        public const string CURRENT_FORMAT_VERSION = "1.12";

        private static Values _V;

        [DataMember]
        private string version = "0.0";
        /// <summary>
        /// Must be present and a supported value. Defaults to an invalid value
        /// </summary>
        [DataMember]
        private string format = string.Empty;
        public Version Version = new Version(0, 0);
        public Version modVersion = new Version(0, 0);
        [DataMember]
        public List<Species> species = new List<Species>();
        [DataMember]
        public List<List<object>> colorDefinitions;
        [DataMember]
        public List<List<object>> dyeDefinitions;

        public ARKColors Colors;
        public ARKColors Dyes;

        public List<string> speciesNames = new List<string>();
        internal Dictionary<string, string> aliases;
        public List<string> speciesWithAliasesList;
        private Dictionary<string, Species> blueprintToSpecies;
        private Dictionary<string, Species> nameToSpecies;
        private Dictionary<string, Species> classNameToSpecies;

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
        /// If this represents values for a mod, the mod-infos are found here.
        /// </summary>
        [DataMember]
        public Mod mod;

        /// <summary>
        /// Contains all species-classes that should be ignored when importing a savegame.
        /// This is e.g. used to filter out rafts which are species in ARK.
        /// </summary>
        [IgnoreDataMember]
        public List<string> ignoreSpeciesClassesOnImport;

        /// <summary>
        /// For the main-values object this hash represents the current loaded mods and their order.
        /// </summary>
        public int loadedModsHash;

        public static int[] statsDisplayOrder = new int[] {
            (int)StatNames.Health,
            (int)StatNames.Stamina,
            (int)StatNames.Oxygen,
            (int)StatNames.Food,
            (int)StatNames.Water,
            (int)StatNames.Temperature,
            (int)StatNames.Weight,
            (int)StatNames.MeleeDamageMultiplier,
            (int)StatNames.SpeedMultiplier,
            (int)StatNames.TemperatureFortitude,
            (int)StatNames.CraftingSpeedMultiplier,
            (int)StatNames.Torpidity
            };

        public Values() { }

        public static Values V => _V ?? (_V = new Values());

        public bool loadValues()
        {
            try
            {
                using (FileStream file = FileService.GetJsonFileStream(Path.Combine(FileService.ValuesFolder, FileService.ValuesJson)))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Values)
                        , new DataContractJsonSerializerSettings()
                        {
                            UseSimpleDictionaryFormat = true
                        }
                        );
                    var tmpV = (Values)ser.ReadObject(file);
                    if (tmpV.format != CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
                    _V = tmpV;
                }
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"Values-File {FileService.ValuesJson} not found. " +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show($"File {FileService.ValuesJson} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(NullReferenceException)) throw e;
                MessageBox.Show($"File {FileService.ValuesJson} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            bool setTamingFood = TamingFoodData.TryLoadDefaultFoodData(out specialFoodData);
            if (specialFoodData == null) specialFoodData = new Dictionary<string, TamingData>();
            _V.specialFoodData = specialFoodData;

            if (setTamingFood && specialFoodData.ContainsKey("default"))
            {
                _V.defaultFoodData = specialFoodData["default"].specialFoodValues;
            }
            else
            {
                _V.defaultFoodData = new Dictionary<string, TamingFood>();
            }

            _V.speciesNames = new List<string>();
            foreach (Species sp in _V.species)
            {
                _V.speciesNames.Add(sp.name);
                if (setTamingFood && specialFoodData.ContainsKey(sp.name))
                {
                    sp.taming.eats = specialFoodData[sp.name].eats;
                    sp.taming.specialFoodValues = specialFoodData[sp.name].specialFoodValues;
                }
            }

            OrderSpecies();

            _V.loadAliases();
            _V.updateSpeciesBlueprints();
            loadedModsHash = CreatureCollection.CalculateModListHash(new List<Mod>());

            if (_V.modsManifest == null)
            {
                if (modsManifest != null) _V.modsManifest = modsManifest;
                else _V.LoadModsManifest();
            }

            if (serverMultipliersPresets != null)
                _V.serverMultipliersPresets = serverMultipliersPresets;
            else if (!ServerMultipliersPresets.TryLoadServerMultipliersPresets(out _V.serverMultipliersPresets))
            {
                MessageBox.Show("The file with the server multipliers couldn't be loaded. Changed settings, e.g. for the singleplayer will be not available.\nIt's recommended to download the application again.",
                    "Server multiplier file not loaded.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return true;
        }

        public static bool TryLoadValuesFile(string filePath, bool setModFileName, out Values values)
        {
            values = null;

            try
            {
                using (FileStream file = File.OpenRead(filePath))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Values), new DataContractJsonSerializerSettings()
                    {
                        UseSimpleDictionaryFormat = true
                    });
                    var tmpV = (Values)ser.ReadObject(file);
                    if (tmpV.format != CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
                    values = tmpV;
                    if (setModFileName) values.mod.FileName = Path.GetFileName(filePath);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Additional Values-File '" + filePath + "' not found.\n\n" +
                        "This collection seems to have modified or added values that are saved in a separate file, " +
                        "which couldn't be found at the saved location. You can load it manually via the menu File - Load mod values…",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show($"File {filePath} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"File {filePath} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads extra values-files that can add species values or modify existing ones
        /// </summary>
        public bool LoadModValues(List<string> modValueFileNames, bool showResults, out List<Mod> mods)
        {
            loadedModsHash = 0;
            List<Values> modifiedValues = new List<Values>();
            mods = new List<Mod>();
            if (modValueFileNames == null) return false;

            CheckAndUpdateModFiles(modValueFileNames);

            foreach (string mf in modValueFileNames)
            {
                string filename = FileService.GetJsonPath(Path.Combine(FileService.ValuesFolder, mf));

                if (TryLoadValuesFile(filename, setModFileName: true, out Values modValues))
                    modifiedValues.Add(modValues);
            }

            int speciesUpdated = 0;
            int speciesAdded = 0;
            // update data if existing
            foreach (Values modValues in modifiedValues)
            {
                // if mods are loaded multiple times, only keep the last
                mods.Remove(modValues.mod);
                mods.Add(modValues.mod);

                // species
                if (modValues.species != null)
                {
                    foreach (Species sp in modValues.species)
                    {
                        if (string.IsNullOrWhiteSpace(sp.blueprintPath)) continue;

                        Species originalSpecies = speciesByBlueprint(sp.blueprintPath);
                        if (originalSpecies == null)
                        {
                            _V.species.Add(sp);
                            sp.mod = modValues.mod;
                            speciesAdded++;

                            if (!blueprintToSpecies.ContainsKey(sp.blueprintPath))
                                blueprintToSpecies.Add(sp.blueprintPath, sp);
                        }
                        else
                        {
                            // species already exists, update all values which are not null
                            bool updated = false;
                            if (sp.TamedBaseHealthMultiplier != null)
                            {
                                originalSpecies.TamedBaseHealthMultiplier = sp.TamedBaseHealthMultiplier;
                                updated = true;
                            }
                            if (sp.statImprintMult != null)
                            {
                                originalSpecies.statImprintMult = sp.statImprintMult;
                                updated = true;
                            }
                            if (sp.fullStatsRaw != null && sp.fullStatsRaw.Length > 0)
                            {
                                for (int s = 0; s < STATS_COUNT && s < sp.fullStatsRaw.Length; s++)
                                {
                                    if (sp.fullStatsRaw[s] == null)
                                        continue;
                                    for (int si = 0; si < 5 && si < sp.fullStatsRaw[s].Length; si++)
                                    {
                                        if (sp.fullStatsRaw[s][si] == null)
                                            continue;
                                        originalSpecies.fullStatsRaw[s][si] = sp.fullStatsRaw[s][si];
                                        updated = true;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(sp.blueprintPath))
                            {
                                originalSpecies.blueprintPath = sp.blueprintPath;
                                updated = true;
                            }
                            if (updated) speciesUpdated++;
                        }
                    }
                }

                // TODO support for mod colors
            }

            loadedModsHash = CreatureCollection.CalculateModListHash(mods);

            if (speciesUpdated == 0 && speciesAdded == 0)
                return true; // nothing changed

            // sort new species
            OrderSpecies();

            // mod-fooddata TODO

            _V.loadAliases();
            _V.updateSpeciesBlueprints();

            if (showResults)
                MessageBox.Show($"The following mods were loaded:\n{string.Join(", ", modifiedValues.Select(m => m.mod.title).ToArray())}\n\n"
                    + $"Species with changed stats: {speciesUpdated}\nSpecies added: {speciesAdded}",
                        "Additional Values succesfully added", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        /// <summary>
        /// Check if all mod files are available and uptodate, and download the ones not available locally.
        /// </summary>
        /// <param name="modValueFileNames"></param>
        internal bool CheckAndUpdateModFiles(List<string> modValueFileNames)
        {
            List<string> missingModValueFilesOnlineAvailable = new List<string>();
            List<string> missingModValueFilesOnlineNotAvailable = new List<string>();
            List<string> modValueFilesWithAvailableUpdate = new List<string>();

            string valuesFolder = FileService.GetJsonPath(FileService.ValuesFolder);

            if (modsManifest == null)
                LoadModsManifest();

            foreach (string mf in modValueFileNames)
            {
                string modFilePath = Path.Combine(valuesFolder, mf);
                if (!File.Exists(modFilePath))
                {
                    if (modsManifest.modsByFiles.ContainsKey(mf))
                        missingModValueFilesOnlineAvailable.Add(mf);
                    else
                        missingModValueFilesOnlineNotAvailable.Add(mf);
                }
                else if (modsManifest.modsByFiles.ContainsKey(mf))
                {
                    // check if an update is available
                    bool downloadRecommended = true;
                    try
                    {
                        if (TryLoadValuesFile(modFilePath, false, out Values modValues)
                            && modValues.Version >= modsManifest.modsByFiles[mf].Version)
                        {
                            downloadRecommended = false;
                        }
                    }
                    catch { }
                    if (downloadRecommended)
                        modValueFilesWithAvailableUpdate.Add(mf);
                }
            }

            bool filesDownloaded = false;

            if (modValueFilesWithAvailableUpdate.Count > 0
                && MessageBox.Show("For " + modValueFilesWithAvailableUpdate.Count.ToString() + " value files there is an update available. It is strongly recommended to use the updated versions.\n"
                + "The updated files can be downloaded automatically if you want.\n\n"
                + "The following files can be downloaded\n"
                + string.Join(", ", modValueFilesWithAvailableUpdate)
                + "\n\nDo you want to download these files?",
                "Updates for value files available", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                foreach (var mf in modValueFilesWithAvailableUpdate)
                {
                    if (Updater.DownloadModValuesFile(mf)
                        && Values.V.modsManifest.modsByFiles.ContainsKey(mf))
                    {
                        Values.V.modsManifest.modsByFiles[mf].downloaded = true;
                        filesDownloaded = true;
                    }
                }
            }

            if (missingModValueFilesOnlineAvailable.Count > 0
                && MessageBox.Show(missingModValueFilesOnlineAvailable.Count.ToString() + " mod-value files are not available locally. Without these files the library will not display all creatures.\n"
                + "The missing files can be downloaded automatically if you want.\n\n"
                + "The following files can be downloaded\n"
                + string.Join(", ", missingModValueFilesOnlineAvailable)
                + "\n\nDo you want to download these files?",
                "Missing value files", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                foreach (var mf in missingModValueFilesOnlineAvailable)
                {
                    if (Updater.DownloadModValuesFile(mf)
                        && Values.V.modsManifest.modsByFiles.ContainsKey(mf))
                    {
                        Values.V.modsManifest.modsByFiles[mf].downloaded = true;
                        filesDownloaded = true;
                    }
                }
            }

            if (missingModValueFilesOnlineNotAvailable.Count > 0)
            {
                MessageBox.Show(missingModValueFilesOnlineNotAvailable.Count.ToString() + " mod-value files are neither available locally nor online. The creatures of the missing mod will not be displayed.\n"
                + "The following files are missing\n"
                + string.Join(", ", missingModValueFilesOnlineNotAvailable),
                "Missing value files", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return filesDownloaded;
        }

        [OnDeserialized]
        private void ParseVersion(StreamingContext ct)
        {
            if (!Version.TryParse(version, out Version))
                Version = new Version(0, 0);

            Colors = new ARKColors(colorDefinitions);
            Dyes = new ARKColors(dyeDefinitions);

            Values tmp = this;

            foreach (var s in species)
                s.InitializeColors(Colors);

            return;

            // for debugging, test if there are duplicates in the species-names
            var duplicateSpeciesNames = string.Join(", ", species
                                               //.GroupBy(s => s.DescriptiveName)
                                               .GroupBy(s => s.NameAndMod)
                                               .Where(g => g.Count() > 1)
                                               .Select(x => x.Key)
                                               .ToArray());
            if (!string.IsNullOrEmpty(duplicateSpeciesNames))
                Clipboard.SetText(duplicateSpeciesNames);
        }

        private void OrderSpecies()
        {
            string fileName = FileService.GetJsonPath("sortNames.txt");
            if (File.Exists(fileName))
            {
                foreach (Species s in _V.species)
                    s.SortName = string.Empty;

                string[] lines = File.ReadAllLines(fileName);
                foreach (string l in lines)
                {
                    if (l.IndexOf("@", StringComparison.Ordinal) <= 0 || l.IndexOf("@", StringComparison.Ordinal) + 1 >= l.Length)
                        continue;
                    string matchName = l.Substring(0, l.IndexOf("@", StringComparison.Ordinal)).Trim();
                    string replaceName = l.Substring(l.IndexOf("@", StringComparison.Ordinal) + 1).Trim();

                    Regex r = new Regex(matchName);

                    List<Species> matchedSpecies = _V.species.Where(s => string.IsNullOrEmpty(s.SortName) && r.IsMatch(s.name)).ToList();

                    foreach (Species s in matchedSpecies)
                    {
                        s.SortName = r.Replace(s.name, replaceName);
                    }
                }

                // set each sortname of species without manual sortname to its speciesname
                foreach (Species s in _V.species)
                {
                    if (string.IsNullOrEmpty(s.SortName))
                        s.SortName = s.name;
                }
            }

            _V.species = _V.species.OrderBy(s => s.SortName).ToList();
            _V.speciesNames = _V.species.Select(s => s.name).ToList();
        }

        /// <summary>
        /// Applies the serverMultipliers and creates precalculated species-stats values
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="eventMultipliers"></param>
        /// <param name="applyStatMultipliers"></param>
        public void applyMultipliers(CreatureCollection cc, bool eventMultipliers = false, bool applyStatMultipliers = true)
        {
            currentServerMultipliers = (eventMultipliers ? cc.serverMultipliersEvents : cc.serverMultipliers)?.Copy(false);
            if (currentServerMultipliers == null) currentServerMultipliers = Values.V.serverMultipliersPresets.GetPreset("official");
            if (currentServerMultipliers == null)
            {
                MessageBox.Show("No default server multiplier values found.\nIt's recommend to redownload the application.",
                    "No default multipliers available", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            ServerMultipliers singlePlayerServerMultipliers = null;

            if (cc.singlePlayerSettings)
            {
                /// The singleplayer multipliers are saved as a regular multiplierpreset, but they work differently
                /// in the way they are multiplied on existing multipliers and won't work on their own.
                /// The preset name "singleplayer" should only be used for this purpose.
                singlePlayerServerMultipliers = serverMultipliersPresets.GetPreset("singleplayer");
                if (singlePlayerServerMultipliers == null)
                    MessageBox.Show("No values for the singleplayer multipliers found. The singleplayer multipliers cannot be applied.\nIt's recommend to redownload the application.",
                        "No singleplayer data available", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            foreach (Species sp in species)
            {
                if (applyStatMultipliers)
                {
                    // stat-multiplier
                    for (int s = 0; s < STATS_COUNT; s++)
                    {
                        double[] statMultipliers = cc.serverMultipliers?.statMultipliers?[s] ?? defaultMultipliers;
                        sp.stats[s].BaseValue = (float)sp.fullStatsRaw[s][0];
                        // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                        sp.stats[s].AddWhenTamed = (float)sp.fullStatsRaw[s][3] * (sp.fullStatsRaw[s][3] > 0 ? (float)statMultipliers[0] : 1);
                        // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                        sp.stats[s].MultAffinity = (float)sp.fullStatsRaw[s][4] * (sp.fullStatsRaw[s][4] > 0 ? (float)statMultipliers[1] : 1);
                        sp.stats[s].IncPerTamedLevel = (float)sp.fullStatsRaw[s][2] * (float)statMultipliers[2];
                        sp.stats[s].IncPerWildLevel = (float)sp.fullStatsRaw[s][1] * (float)statMultipliers[3];

                        if (singlePlayerServerMultipliers?.statMultipliers?[s] == null)
                            continue;
                        // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                        sp.stats[s].AddWhenTamed *= sp.stats[s].AddWhenTamed > 0 ? (float)singlePlayerServerMultipliers.statMultipliers[s][0] : 1;
                        // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                        sp.stats[s].MultAffinity *= sp.stats[s].MultAffinity > 0 ? (float)singlePlayerServerMultipliers.statMultipliers[s][1] : 1;
                        sp.stats[s].IncPerTamedLevel *= (float)singlePlayerServerMultipliers.statMultipliers[s][2];
                        sp.stats[s].IncPerWildLevel *= (float)singlePlayerServerMultipliers.statMultipliers[s][3];
                    }
                }
                // breeding multiplier
                if (sp.breeding == null)
                    continue;
                if (currentServerMultipliers.EggHatchSpeedMultiplier > 0)
                {
                    sp.breeding.gestationTimeAdjusted = sp.breeding.gestationTime / currentServerMultipliers.EggHatchSpeedMultiplier;
                    sp.breeding.incubationTimeAdjusted = sp.breeding.incubationTime / currentServerMultipliers.EggHatchSpeedMultiplier;
                }
                if (currentServerMultipliers.BabyMatureSpeedMultiplier > 0)
                    sp.breeding.maturationTimeAdjusted = sp.breeding.maturationTime / currentServerMultipliers.BabyMatureSpeedMultiplier;

                sp.breeding.matingCooldownMinAdjusted = sp.breeding.matingCooldownMin * currentServerMultipliers.MatingIntervalMultiplier;
                sp.breeding.matingCooldownMaxAdjusted = sp.breeding.matingCooldownMax * currentServerMultipliers.MatingIntervalMultiplier;
            }
        }

        private void loadAliases()
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show($"Couldn't load {FileService.AliasesJson}\nThe program will continue without it.\n" +
                        $"Error message:\n\n{e.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            speciesWithAliasesList.Sort();
        }

        private void updateSpeciesBlueprints()
        {
            blueprintToSpecies = new Dictionary<string, Species>();
            nameToSpecies = new Dictionary<string, Species>(StringComparer.OrdinalIgnoreCase);
            classNameToSpecies = new Dictionary<string, Species>(StringComparer.OrdinalIgnoreCase);

            Regex rClassName = new Regex(@"(?<=\.)[^\/\.]+$");

            foreach (Species s in species)
            {
                if (!string.IsNullOrEmpty(s.blueprintPath))
                {
                    if (!blueprintToSpecies.ContainsKey(s.blueprintPath))
                        blueprintToSpecies.Add(s.blueprintPath, s);

                    string name = s.DescriptiveName;
                    if (!nameToSpecies.ContainsKey(name))
                        nameToSpecies.Add(name, s);
                    else nameToSpecies[name] = s; // overwrite earlier entry, keep latest entry

                    Match classNameMatch = rClassName.Match(s.blueprintPath);
                    if (classNameMatch.Success)
                    {
                        string className = classNameMatch.Value + "_C";
                        if (classNameToSpecies.ContainsKey(className))
                            classNameToSpecies[className] = s;
                        else
                            classNameToSpecies.Add(className, s);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the passed name is an available species name or an alias, then returns the species
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>Available species name or empty, if not available.</returns>
        public string speciesName(string alias)
        {
            if (speciesNames.Contains(alias))
                return alias;
            return aliases.ContainsKey(alias) ? aliases[alias] : string.Empty;
        }

        /// <summary>
        /// Checks species names and loaded aliases for a match and sets the out parameter.
        /// Especially when mods are used, this is not garantueed to result in the correct species.
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="species"></param>
        /// <returns>True on success</returns>
        public bool TryGetSpeciesByName(string speciesName, out Species species)
        {
            species = null;
            if (string.IsNullOrEmpty(speciesName)) return false;

            if (aliases.ContainsKey(speciesName))
                speciesName = aliases[speciesName];
            if (nameToSpecies.ContainsKey(speciesName))
            {
                species = nameToSpecies[speciesName];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks species for a matching className.
        /// Especially when mods are used, this is not garantueed to result in the correct species.
        /// </summary>
        /// <param name="speciesClassName"></param>
        /// <param name="species"></param>
        /// <returns>True on success</returns>
        public bool TryGetSpeciesByClassName(string speciesClassName, out Species species)
        {
            species = null;
            if (string.IsNullOrEmpty(speciesClassName)) return false;

            if (classNameToSpecies.ContainsKey(speciesClassName))
            {
                species = classNameToSpecies[speciesClassName];
                return true;
            }

            return false;
        }

        public Species speciesByBlueprint(string blueprintpath)
        {
            if (string.IsNullOrEmpty(blueprintpath)) return null;
            return blueprintToSpecies.ContainsKey(blueprintpath) ? blueprintToSpecies[blueprintpath] : null;
        }

        /// <summary>
        /// Run async method and wait for it to load the manifest-file
        /// </summary>
        /// <param name="forceUpdate"></param>
        internal bool LoadModsManifest(bool forceUpdate = false)
        {
            var task = Task.Run(async () => await LoadModsManifestAsync(forceUpdate));
            return task.Result;
        }

        internal async Task<bool> LoadModsManifestAsync(bool forceUpdate = false)
        {
            modsManifest = await ModsManifest.TryLoadModManifestFile(forceUpdate);
            bool manifestFileLoadingSuccessful = modsManifest != null;
            if (!manifestFileLoadingSuccessful)
                modsManifest = new ModsManifest();

            // UpdateManualModValueFiles(); // TODO

            return manifestFileLoadingSuccessful;
        }

        /// <summary>
        /// add possible mod-value files that are not listed in the manifest-file (manually created)
        /// </summary>
        internal void UpdateManualModValueFiles()
        {
            // TODO loop through modvalue files and check if file is not yet loaded in manifest.
        }

        internal void LoadIgnoreSpeciesClassesFile()
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
                MessageBox.Show($"Couldn't load {FileService.IgnoreSpeciesClasses}\nThe program will continue without it.\n" +
                        $"Error message:\n\n{e.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

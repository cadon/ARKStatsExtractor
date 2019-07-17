using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
        private string ver = "0.0";
        [DataMember]
        private string formatVersion = string.Empty; // must be present and a supported value, so defaults to an invalid value
        public Version version = new Version(0, 0);
        public Version modVersion = new Version(0, 0);
        public string modValuesFile = string.Empty;
        [DataMember]
        public List<Species> species = new List<Species>();

        public List<string> speciesNames = new List<string>();
        internal Dictionary<string, string> aliases;
        public List<string> speciesWithAliasesList;
        private Dictionary<string, Species> blueprintToSpecies;
        private Dictionary<string, Species> nameToSpecies;

        /// <summary>
        /// Representing the current server multipliers except statMultipliers. Also considers event-changes.
        /// </summary>
        [IgnoreDataMember]
        public ServerMultipliers currentServerMultipliers;
        /// <summary>
        /// List of presets for server multipliers for easier setting. Also contains the singleplayer multipliers.
        /// </summary>
        [IgnoreDataMember]
        public ServerMultipliersPresets serverMultipliersPresets;

        [DataMember]
        public Dictionary<string, TamingFood> foodData = new Dictionary<string, TamingFood>();

        public bool celsius = true;
        [DataMember]
        public Mod mod;
        [IgnoreDataMember]
        public int loadedModsHash;

        /// <summary>
        /// This List is used to determine if different stat-names should be displayed
        /// </summary>
        [IgnoreDataMember]
        private List<string> glowSpecies = new List<string>();

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
                using (FileStream file = FileService.GetJsonFileStream(FileService.ValuesJson))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Values)
                        //, new DataContractJsonSerializerSettings()
                        //{
                        //    UseSimpleDictionaryFormat = true
                        //}
                        );
                    var tmpV = (Values)ser.ReadObject(file);
                    if (tmpV.formatVersion != CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
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
                MessageBox.Show($"File {FileService.ValuesJson} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                _V.version = new Version(_V.ver);
            }
            catch
            {
                _V.version = new Version(0, 0);
            }

            _V.speciesNames = new List<string>();
            foreach (Species sp in _V.species)
            {
                sp.Initialize();
                _V.speciesNames.Add(sp.name.ToLower());
            }

            OrderSpecies();

            _V.glowSpecies = new List<string> { "Bulbdog", "Featherlight", "Glowbug", "Glowtail", "Shinehorn" };
            _V.loadAliases();
            _V.updateSpeciesBlueprints();
            _V.modValuesFile = string.Empty; // TODO remove, replace with modList

            if (!ServerMultipliersPresets.TryLoadServerMultipliersPresets(out _V.serverMultipliersPresets))
                MessageBox.Show("The file with the server multipliers couldn't be loaded. Changed settings, e.g. for the singleplayer will be not available.\nIt's recommended to download the application again.",
                    "Server multiplier file not loaded.", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //saveJSON();
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
                    if (tmpV.formatVersion != CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
                    values = tmpV;
                    if (setModFileName) values.mod.FileName = Path.GetFileName(filePath);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Additional Values-File '" + filePath + "' not found.\n" +
                        "This collection seems to have modified or added values that are saved in a separate file, " +
                        "which couldn't be found at the saved location. You can load it manually via the menu File - Load additional values…",
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
        /// load extra values-file that can add values or modify existing ones
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="showResults"></param>
        /// <returns></returns>
        [Obsolete("replaced by LoadModValues()")]
        public bool loadAdditionalValues(string filename, bool showResults)
        {
            if (!TryLoadValuesFile(filename, setModFileName: true, out Values modifiedValues)) return false;

            _V.modValuesFile = Path.GetFileName(filename);
            int speciesUpdated = 0;
            int speciesAdded = 0;
            // update data if existing
            // version
            try
            {
                _V.modVersion = new Version(modifiedValues.ver);
            }
            catch
            {
                _V.modVersion = new Version(0, 0);
            }

            // species
            if (modifiedValues.species != null)
            {
                foreach (Species sp in modifiedValues.species)
                {
                    if (string.IsNullOrWhiteSpace(sp.blueprintPath)) continue;

                    Species originalSpecies = speciesByBlueprint(sp.blueprintPath);
                    if (originalSpecies == null)
                    {
                        _V.species.Add(sp);
                        sp.Initialize();
                        sp.mod = modifiedValues.mod;
                        speciesAdded++;
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
                        if (sp.NoImprintingForSpeed != null)
                        {
                            originalSpecies.NoImprintingForSpeed = sp.NoImprintingForSpeed;
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

                // sort new species
                OrderSpecies();
            }
            // fooddata TODO
            // default-multiplier TODO

            _V.loadAliases();
            _V.updateSpeciesBlueprints();

            if (showResults)
                MessageBox.Show($"Species with changed stats: {speciesUpdated}\nSpecies added: {speciesAdded}",
                        "Additional Values succesfully added", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        public bool LoadModValues(List<string> modValueFiles, bool showResults, out List<Mod> mods)
        {
            loadedModsHash = 0;
            List<Values> modifiedValues = new List<Values>();
            mods = new List<Mod>();
            if (modValueFiles == null) return false;

            foreach (string mf in modValueFiles)
            {
                string filename = FileService.GetJsonPath(Path.Combine("mods", mf));

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
                            sp.Initialize();
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
                            if (sp.NoImprintingForSpeed != null)
                            {
                                originalSpecies.NoImprintingForSpeed = sp.NoImprintingForSpeed;
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
            }

            loadedModsHash = CreatureCollection.CalculateModListId(mods);

            // sort new species
            OrderSpecies();

            // fooddata TODO
            // default-multiplier TODO

            _V.loadAliases();
            _V.updateSpeciesBlueprints();

            if (showResults)
                MessageBox.Show($"Species with changed stats: {speciesUpdated}\nSpecies added: {speciesAdded}",
                        "Additional Values succesfully added", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
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
            currentServerMultipliers = (eventMultipliers ? cc.serverMultipliersEvents : cc.serverMultipliers)?.Copy();
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
            aliases = new Dictionary<string, string>();
            speciesWithAliasesList = new List<string>(speciesNames);

            try
            {
                using (StreamReader reader = FileService.GetJsonFileReader(FileService.AliasesJson))
                {
                    JObject aliasesNode = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    foreach (KeyValuePair<string, JToken> pair in aliasesNode)
                    {
                        string alias = pair.Key.ToLower();
                        string speciesName = pair.Value.Value<string>().ToLower();
                        if (speciesNames.Contains(alias)
                                || !speciesNames.Contains(speciesName)
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
            nameToSpecies = new Dictionary<string, Species>();

            foreach (Species s in species)
            {
                if (!string.IsNullOrEmpty(s.blueprintPath))
                {
                    if (!blueprintToSpecies.ContainsKey(s.blueprintPath))
                        blueprintToSpecies.Add(s.blueprintPath, s);
                    string name = s.name.ToLower();
                    if (!nameToSpecies.ContainsKey(name))
                        nameToSpecies.Add(name, s);
                    else nameToSpecies[name] = s; // overwrite earlier entry, keep latest entry
                }
            }
        }

        /// <summary>
        /// Checks if the passed name is an available species name or an alias, then returns the species
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>Available species name or empty, if not available.</returns>
        [ObsoleteAttribute("Use TryGetSpeciesByName() instead")]
        public string speciesName(string alias)
        {
            alias = alias.ToLower();
            if (speciesNames.Contains(alias))
                return alias;
            return aliases.ContainsKey(alias) ? aliases[alias] : string.Empty;
        }

        /// <summary>
        /// Checks species names and loaded aliases for a match and sets the out parameter.
        /// </summary>
        /// <param name="speciesName"></param>
        /// <param name="species"></param>
        /// <returns>True on success</returns>
        public bool TryGetSpeciesByName(string speciesName, out Species species)
        {
            species = null;
            if (string.IsNullOrEmpty(speciesName)) return false;

            speciesName = speciesName.ToLower();

            if (aliases.ContainsKey(speciesName))
                speciesName = aliases[speciesName];
            if (nameToSpecies.ContainsKey(speciesName))
            {
                species = nameToSpecies[speciesName];
                return true;
            }

            return false;
        }

        public Species speciesByBlueprint(string blueprintpath)
        {
            if (string.IsNullOrEmpty(blueprintpath)) return null;
            return blueprintToSpecies.ContainsKey(blueprintpath) ? blueprintToSpecies[blueprintpath] : null;
        }

        public bool IsGlowSpecies(string species) => !string.IsNullOrEmpty(species) && glowSpecies.Contains(species);
    }
}

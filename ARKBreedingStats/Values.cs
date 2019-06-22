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

namespace ARKBreedingStats
{
    [DataContract]
    public class Values
    {
        private static Values _V;
        private static readonly int statsCount = 12;
        [DataMember]
        private string ver = "0.0";
        [DataMember]
        private string FormatVersion = "1.12"; // represents the format of the file. currently set to 1.12 to represent the supported 12 stats
        public Version version = new Version(0, 0);
        public Version modVersion = new Version(0, 0);
        public string modValuesFile = string.Empty;
        [DataMember]
        public List<Species> species = new List<Species>();

        public List<string> speciesNames = new List<string>();
        private Dictionary<string, string> aliases;
        public List<string> speciesWithAliasesList;
        private Dictionary<string, string> speciesBlueprints;

        [DataMember]
        private double[][] statMultipliers = new double[statsCount][]; // official server stats-multipliers
        [DataMember]
        private double?[][] statMultipliersSP = new double?[statsCount][]; // adjustments for sp
        [DataMember]
        public Dictionary<string, TamingFood> foodData = new Dictionary<string, TamingFood>();

        public double imprintingStatScaleMultiplier = 1;
        public double babyFoodConsumptionSpeedMultiplier = 1;
        public double babyCuddleIntervalMultiplier = 1;
        public double tamingSpeedMultiplier = 1;

        [DataMember]
        public double matingIntervalMultiplierSP = 1;
        [DataMember]
        public double eggHatchSpeedMultiplierSP = 1;
        [DataMember]
        public double babyMatureSpeedMultiplierSP = 1;
        [DataMember]
        public double babyCuddleIntervalMultiplierSP = 1;
        [DataMember]
        public double tamingSpeedMultiplierSP = 1;
        public bool celsius = true;

        private List<string> glowSpecies = new List<string>(); // this List is used to determine if different stat-names should be displayed

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
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Values));
                    _V = (Values)ser.ReadObject(file);
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
                _V.speciesNames.Add(sp.name);
            }

            OrderSpecies(_V.species, _V.speciesNames);

            _V.glowSpecies = new List<string> { "Bulbdog", "Featherlight", "Glowbug", "Glowtail", "Shinehorn" };
            _V.loadAliases();
            _V.updateSpeciesBlueprints();
            _V.modValuesFile = string.Empty;

            //saveJSON();
            return true;
        }

        /// <summary>
        /// load extra values-file that can add values or modify existing ones
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="showResults"></param>
        /// <returns></returns>
        public bool loadAdditionalValues(string filename, bool showResults)
        {
            Values modifiedValues;

            try
            {
                using (FileStream file = File.OpenRead(filename))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Values));
                    modifiedValues = (Values)ser.ReadObject(file);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Additional Values-File '" + filename + "' not found.\n" +
                        "This collection seems to have modified or added values that are saved in a separate file, " +
                        "which couldn't be found at the saved location. You can load it manually via the menu File - Load additional values…",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"File {filename} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

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
                    if (!_V.speciesNames.Contains(sp.name))
                    {
                        _V.species.Add(sp);
                        sp.Initialize();
                        _V.speciesNames.Add(sp.name);
                        speciesAdded++;
                    }
                    else
                    {
                        // species already exists, update all values which are not null
                        Species originalSpecies = _V.species[_V.speciesNames.IndexOf(sp.name)];
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
                        if (sp.statsRaw != null && sp.statsRaw.Length > 0)
                        {
                            for (int s = 0; s < statsCount && s < sp.statsRaw.Length; s++)
                            {
                                if (sp.statsRaw[s] == null)
                                    continue;
                                for (int si = 0; si < 5 && si < sp.statsRaw[s].Length; si++)
                                {
                                    if (sp.statsRaw[s][si] == null)
                                        continue;
                                    originalSpecies.statsRaw[s][si] = sp.statsRaw[s][si];
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
                OrderSpecies(_V.species, _V.speciesNames);
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

        private void OrderSpecies(IList<Species> species, List<string> speciesNames)
        {
            string fileName = FileService.GetJsonPath("sortNames.txt");
            if (File.Exists(fileName))
            {
                foreach (Species s in species)
                    s.SortName = string.Empty;

                string[] lines = File.ReadAllLines(fileName);
                foreach (string l in lines)
                {
                    if (l.IndexOf("@", StringComparison.Ordinal) <= 0 || l.IndexOf("@", StringComparison.Ordinal) + 1 >= l.Length)
                        continue;
                    string matchName = l.Substring(0, l.IndexOf("@", StringComparison.Ordinal)).Trim();
                    string replaceName = l.Substring(l.IndexOf("@", StringComparison.Ordinal) + 1).Trim();

                    Regex r = new Regex(matchName);

                    List<Species> matchedSpecies = species.Where(s => string.IsNullOrEmpty(s.SortName) && r.IsMatch(s.name)).ToList();

                    foreach (Species s in matchedSpecies)
                    {
                        s.SortName = r.Replace(s.name, replaceName);
                    }
                }

                // set each sortname of species without manual sortname to its speciesname
                foreach (Species s in species)
                {
                    if (string.IsNullOrEmpty(s.SortName))
                        s.SortName = s.name;
                }
            }

            _V.species = species.OrderBy(s => s.SortName).ToList();
            _V.speciesNames = _V.species.Select(s => s.name).ToList();
        }

        //// currently not used
        //public void saveJSON()
        //{
        //    try
        //    {
        //        // to create minified json of current values
        //        DataContractJsonSerializer writer = new DataContractJsonSerializer(typeof(Values));
        //        using (FileStream file = File.Create(FileService.GetPath(FileService.ValuesJson)))
        //        {
        //            writer.WriteObject(file, _V);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + e.Message, "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        public void applyMultipliers(CreatureCollection cc, bool eventMultipliers = false, bool applyStatMultipliers = true)
        {
            imprintingStatScaleMultiplier = cc.imprintingMultiplier;
            babyFoodConsumptionSpeedMultiplier = eventMultipliers ? cc.BabyFoodConsumptionSpeedMultiplierEvent : cc.BabyFoodConsumptionSpeedMultiplier;

            double eggHatchSpeedMultiplier = eventMultipliers ? cc.EggHatchSpeedMultiplierEvent : cc.EggHatchSpeedMultiplier;
            double babyMatureSpeedMultiplier = eventMultipliers ? cc.BabyMatureSpeedMultiplierEvent : cc.BabyMatureSpeedMultiplier;
            double matingIntervalMultiplier = eventMultipliers ? cc.MatingIntervalMultiplierEvent : cc.MatingIntervalMultiplier;
            babyCuddleIntervalMultiplier = eventMultipliers ? cc.babyCuddleIntervalMultiplierEvent : cc.babyCuddleIntervalMultiplier;
            tamingSpeedMultiplier = eventMultipliers ? cc.tamingSpeedMultiplierEvent : cc.tamingSpeedMultiplier;

            if (cc.singlePlayerSettings)
            {
                matingIntervalMultiplier *= matingIntervalMultiplierSP;
                eggHatchSpeedMultiplier *= eggHatchSpeedMultiplierSP;
                babyMatureSpeedMultiplier *= babyMatureSpeedMultiplierSP;
                babyCuddleIntervalMultiplier *= babyCuddleIntervalMultiplierSP;
                tamingSpeedMultiplier *= tamingSpeedMultiplierSP;
            }

            // check for 0
            if (matingIntervalMultiplier == 0) matingIntervalMultiplier = 1;
            if (eggHatchSpeedMultiplier == 0) eggHatchSpeedMultiplier = 1;
            if (babyMatureSpeedMultiplier == 0) babyMatureSpeedMultiplier = 1;
            if (babyCuddleIntervalMultiplier == 0) babyCuddleIntervalMultiplier = 1;
            if (tamingSpeedMultiplier == 0) tamingSpeedMultiplier = 1;

            foreach (Species sp in species)
            {
                if (applyStatMultipliers)
                {
                    // stat-multiplier
                    for (int s = 0; s < statsCount; s++)
                    {
                        sp.stats[s].BaseValue = (float)sp.statsRaw[s][0];
                        // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                        sp.stats[s].AddWhenTamed = (float)sp.statsRaw[s][3] * (sp.statsRaw[s][3] > 0 ? (float)cc.multipliers[s][0] : 1);
                        // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                        sp.stats[s].MultAffinity = (float)sp.statsRaw[s][4] * (sp.statsRaw[s][4] > 0 ? (float)cc.multipliers[s][1] : 1);
                        sp.stats[s].IncPerTamedLevel = (float)sp.statsRaw[s][2] * (float)cc.multipliers[s][2];
                        sp.stats[s].IncPerWildLevel = (float)sp.statsRaw[s][1] * (float)cc.multipliers[s][3];

                        if (!cc.singlePlayerSettings || statMultipliersSP[s] == null)
                            continue;
                        // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                        sp.stats[s].AddWhenTamed *= statMultipliersSP[s][0] != null && sp.stats[s].AddWhenTamed > 0 ? (float)statMultipliersSP[s][0] : 1;
                        // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                        sp.stats[s].MultAffinity *= statMultipliersSP[s][1] != null && sp.stats[s].MultAffinity > 0 ? (float)statMultipliersSP[s][1] : 1;
                        sp.stats[s].IncPerTamedLevel *= statMultipliersSP[s][2] != null ? (float)statMultipliersSP[s][2] : 1;
                        sp.stats[s].IncPerWildLevel *= statMultipliersSP[s][3] != null ? (float)statMultipliersSP[s][3] : 1;
                    }
                }
                // breeding multiplier
                if (sp.breeding == null)
                    continue;
                if (eggHatchSpeedMultiplier > 0)
                {
                    sp.breeding.gestationTimeAdjusted = sp.breeding.gestationTime / eggHatchSpeedMultiplier;
                    sp.breeding.incubationTimeAdjusted = sp.breeding.incubationTime / eggHatchSpeedMultiplier;
                }
                if (babyMatureSpeedMultiplier > 0)
                    sp.breeding.maturationTimeAdjusted = sp.breeding.maturationTime / babyMatureSpeedMultiplier;

                sp.breeding.matingCooldownMinAdjusted = sp.breeding.matingCooldownMin * matingIntervalMultiplier;
                sp.breeding.matingCooldownMaxAdjusted = sp.breeding.matingCooldownMax * matingIntervalMultiplier;
            }
        }

        public double[][] getOfficialMultipliers()
        {
            double[][] officialMultipliers = new double[statsCount][];
            for (int s = 0; s < statsCount; s++)
            {
                officialMultipliers[s] = new double[4];
                if (s < statMultipliers.Length && statMultipliers[s] != null)
                {
                    for (int sm = 0; sm < 4; sm++)
                        officialMultipliers[s][sm] = statMultipliers[s][sm];
                }
                else
                {
                    for (int sm = 0; sm < 4; sm++)
                        officialMultipliers[s][sm] = 1;
                }
            }
            return officialMultipliers;
        }

        public double[][] getSinglePlayerMultipliers()
        {
            double[][] singlePlayerMultipliers = new double[statsCount][];
            for (int s = 0; s < statsCount; s++)
            {
                singlePlayerMultipliers[s] = new double[4];
                if (s < statMultipliersSP.Length && statMultipliersSP[s] != null)
                {
                    for (int sm = 0; sm < 4; sm++)
                        singlePlayerMultipliers[s][sm] = statMultipliersSP[s][sm] ?? 1;
                }
                else
                {
                    for (int sm = 0; sm < 4; sm++)
                        singlePlayerMultipliers[s][sm] = 1;
                }
            }
            return singlePlayerMultipliers;
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
                        if (speciesNames.Contains(pair.Key)
                                || !speciesNames.Contains(pair.Value.Value<string>())
                                || aliases.ContainsKey(pair.Key))
                            continue;
                        aliases.Add(pair.Key, pair.Value.Value<string>());
                        speciesWithAliasesList.Add(pair.Key);
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
            speciesBlueprints = new Dictionary<string, string>();

            foreach (Species s in species)
            {
                if (!string.IsNullOrEmpty(s.blueprintPath) && !speciesBlueprints.ContainsKey(s.blueprintPath))
                {
                    speciesBlueprints.Add(s.blueprintPath, s.name);
                }
            }
        }

        /// <summary>
        /// Checks if the passed name is an available species name or an alias, then returns the species name
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>Available species name or empty, if not available.</returns>
        public string speciesName(string alias)
        {
            if (speciesNames.Contains(alias))
                return alias;
            return aliases.ContainsKey(alias) ? aliases[alias] : string.Empty;
        }

        public string speciesNameFromBP(string blueprintpath)
        {
            return speciesBlueprints.ContainsKey(blueprintpath) ? speciesBlueprints[blueprintpath] : string.Empty;
        }

        public int speciesIndex(string species)
        {
            species = speciesName(species);
            return speciesNames.IndexOf(species);
        }

        public bool IsGlowSpecies(string species) => glowSpecies.Contains(species);
    }
}

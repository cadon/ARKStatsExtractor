using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ARKBreedingStats
{
    [DataContract]
    public class Values
    {
        private static Values _V;
        [DataMember]
        private string ver = "0.0";
        public Version version = new Version(0, 0);
        public Version modVersion = new Version(0, 0);
        public string modValuesFile = "";
        [DataMember]
        public List<Species> species = new List<Species>();

        public List<string> speciesNames = new List<string>();
        private Dictionary<string, string> aliases;
        public List<string> speciesWithAliasesList;
        private Dictionary<string, string> speciesBlueprints;

        [DataMember]
        public double[][] statMultipliers = new double[8][]; // official server stats-multipliers
        [DataMember]
        public double?[][] statMultipliersSP = new double?[8][]; // adjustments for sp
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

        public List<string> glowSpecies = new List<string>(); // this List is used to determine if different stat-names should be displayed

        public Values()
        {
        }

        public static Values V
        {
            get
            {
                if (_V == null)
                    _V = new Values();
                return _V;
            }
        }

        public bool loadValues()
        {
            bool loadedSuccessful = true;

            string filename = "json/values.json";

            // check if file exists
            if (!File.Exists(filename))
            {
                if (MessageBox.Show("Values-File '" + filename + "' not found. This tool will not work properly without that file.\n\nDo you want to visit the homepage of the tool to redownload it?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/releases/latest");
                return false;
            }

            _V.version = new Version(0, 0);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Values));
            System.IO.FileStream file = System.IO.File.OpenRead(filename);

            try
            {
                _V = (Values)ser.ReadObject(file);
            }
            catch (Exception e)
            {
                MessageBox.Show("File Couldn't be opened or read.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadedSuccessful = false;
            }
            file.Close();

            if (loadedSuccessful)
            {
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
                    sp.initialize();
                    _V.speciesNames.Add(sp.name);
                }

                _V.glowSpecies = new List<string> { "Bulbdog", "Featherlight", "Glowbug", "Glowtail", "Shinehorn" };
                _V.loadAliases();
                _V.loadSpeciesBlueprints();
                _V.modValuesFile = "";
            }

            //saveJSON();
            return loadedSuccessful;
        }

        public bool loadAdditionalValues(string filename, bool showResults)
        {
            // load extra values-file that can add values or modify existing ones
            bool loadedSuccessful = true;

            // check if file exists
            if (!File.Exists(filename))
            {
                MessageBox.Show("Additional Values-File '" + filename + "' not found.\nThis collection seems to have modified or added values that are saved in a separate file, which couldn't be found at the saved location. You can load it manually via the menu File - Load additional values…", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Values));
            System.IO.FileStream file = System.IO.File.OpenRead(filename);

            Values modifiedValues = new Values();

            try
            {
                modifiedValues = (Values)ser.ReadObject(file);
            }
            catch (Exception e)
            {
                MessageBox.Show("File Couldn't be opened or read.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadedSuccessful = false;
            }
            file.Close();
            if (!loadedSuccessful) return false;

            _V.modValuesFile = Path.GetFileName(file.Name);
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
                        sp.initialize();
                        _V.speciesNames.Add(sp.name);
                        speciesAdded++;
                    }
                    else
                    {
                        int i = _V.speciesNames.IndexOf(sp.name);
                        bool updated = false;
                        if (sp.TamedBaseHealthMultiplier != null)
                        {
                            _V.species[i].TamedBaseHealthMultiplier = sp.TamedBaseHealthMultiplier;
                            updated = true;
                        }
                        if (sp.NoImprintingForSpeed != null)
                        {
                            _V.species[i].NoImprintingForSpeed = sp.NoImprintingForSpeed;
                            updated = true;
                        }
                        if (sp.statsRaw != null && sp.statsRaw.Length > 0)
                        {
                            for (int s = 0; s < 8 && s < sp.statsRaw.Length; s++)
                            {
                                if (sp.statsRaw[s] != null)
                                {
                                    for (int si = 0; si < 5 && si < sp.statsRaw[s].Length; si++)
                                    {
                                        if (sp.statsRaw[s][si] != null)
                                        {
                                            _V.species[i].statsRaw[s][si] = sp.statsRaw[s][si];
                                            updated = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (updated) speciesUpdated++;
                    }
                }
            }
            // fooddata TODO
            // default-multiplier TODO

            _V.loadAliases();

            if (showResults)
                MessageBox.Show("Species with changed stats: " + speciesUpdated + "\nSpecies added: " + speciesAdded, "Additional Values succesfully added", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        // currently not used
        //public void saveJSON()
        //{
        //    // to create minified json of current values
        //    DataContractJsonSerializer writer = new DataContractJsonSerializer(typeof(Values));
        //    try
        //    {
        //        System.IO.FileStream file = System.IO.File.Create("values.json");
        //        writer.WriteObject(file, _V);
        //        file.Close();
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

            for (int sp = 0; sp < species.Count; sp++)
            {
                if (applyStatMultipliers)
                {
                    // stat-multiplier
                    for (int s = 0; s < 8; s++)
                    {
                        species[sp].stats[s].BaseValue = (double)species[sp].statsRaw[s][0];
                        // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                        species[sp].stats[s].AddWhenTamed = (double)species[sp].statsRaw[s][3] * (species[sp].statsRaw[s][3] > 0 ? cc.multipliers[s][0] : 1);
                        // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                        species[sp].stats[s].MultAffinity = (double)species[sp].statsRaw[s][4] * (species[sp].statsRaw[s][4] > 0 ? cc.multipliers[s][1] : 1);
                        species[sp].stats[s].IncPerTamedLevel = (double)species[sp].statsRaw[s][2] * cc.multipliers[s][2];
                        species[sp].stats[s].IncPerWildLevel = (double)species[sp].statsRaw[s][1] * cc.multipliers[s][3];

                        if (cc.singlePlayerSettings && statMultipliersSP[s] != null)
                        {
                            // don't apply the multiplier if AddWhenTamed is negative (e.g. Giganotosaurus, Griffin)
                            species[sp].stats[s].AddWhenTamed *= statMultipliersSP[s][0] != null && species[sp].stats[s].AddWhenTamed > 0 ? (double)statMultipliersSP[s][0] : 1;
                            // don't apply the multiplier if MultAffinity is negative (e.g. Aberration variants)
                            species[sp].stats[s].MultAffinity *= statMultipliersSP[s][1] != null && species[sp].stats[s].MultAffinity > 0 ? (double)statMultipliersSP[s][1] : 1;
                            species[sp].stats[s].IncPerTamedLevel *= statMultipliersSP[s][2] != null ? (double)statMultipliersSP[s][2] : 1;
                            species[sp].stats[s].IncPerWildLevel *= statMultipliersSP[s][3] != null ? (double)statMultipliersSP[s][3] : 1;
                        }
                    }
                }
                // breeding multiplier
                if (species[sp].breeding != null)
                {
                    if (eggHatchSpeedMultiplier > 0)
                    {
                        species[sp].breeding.gestationTimeAdjusted = species[sp].breeding.gestationTime / eggHatchSpeedMultiplier;
                        species[sp].breeding.incubationTimeAdjusted = species[sp].breeding.incubationTime / eggHatchSpeedMultiplier;
                    }
                    if (babyMatureSpeedMultiplier > 0)
                        species[sp].breeding.maturationTimeAdjusted = species[sp].breeding.maturationTime / babyMatureSpeedMultiplier;

                    species[sp].breeding.matingCooldownMinAdjusted = species[sp].breeding.matingCooldownMin * matingIntervalMultiplier;
                    species[sp].breeding.matingCooldownMaxAdjusted = species[sp].breeding.matingCooldownMax * matingIntervalMultiplier;
                }
            }
        }

        public double[][] getOfficialMultipliers()
        {
            double[][] officialMultipliers = new double[8][];
            for (int s = 0; s < 8; s++)
            {
                officialMultipliers[s] = new double[4];
                for (int sm = 0; sm < 4; sm++)
                    officialMultipliers[s][sm] = statMultipliers[s][sm];
            }
            return officialMultipliers;
        }

        private void loadAliases()
        {
            aliases = new Dictionary<string, string>();
            speciesWithAliasesList = new List<string>(speciesNames);

            string fileName = "json/aliases.json";
            if (System.IO.File.Exists(fileName))
            {
                string aliasesRaw = System.IO.File.ReadAllText(fileName);

                Regex r = new Regex(@"""([^""]+)"" ?: ?""([^""]+)""");
                MatchCollection matches = r.Matches(aliasesRaw);
                foreach (Match match in matches)
                {
                    if (!speciesNames.Contains(match.Groups[1].Value)
                        && speciesNames.Contains(match.Groups[2].Value)
                        && !aliases.ContainsKey(match.Groups[1].Value))
                    {
                        aliases.Add(match.Groups[1].Value, match.Groups[2].Value);
                        speciesWithAliasesList.Add(match.Groups[1].Value);
                    }
                }
            }
            speciesWithAliasesList.Sort();
        }

        private void loadSpeciesBlueprints()
        {
            speciesBlueprints = new Dictionary<string, string>();

            string fileName = "json/bps.json";
            if (System.IO.File.Exists(fileName))
            {
                string aliasesRaw = System.IO.File.ReadAllText(fileName);

                Regex r = new Regex(@"""([^""]+)"" ?: ?""([^""]+)""");
                MatchCollection matches = r.Matches(aliasesRaw);
                foreach (Match match in matches)
                {
                    if (speciesNames.Contains(match.Groups[2].Value)
                        && !speciesBlueprints.ContainsKey(match.Groups[1].Value))
                    {
                        speciesBlueprints.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            }
            else MessageBox.Show("The file \"json/bps.json\" which contains the blueprint-paths couldn't be found. Try redownloading the latest release.", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public string speciesName(string alias)
        {
            if (speciesNames.Contains(alias))
                return alias;
            else if (aliases.ContainsKey(alias))
                return aliases[alias];
            else return "";
        }

        public string speciesNameFromBP(string blueprintpath)
        {
            if (speciesBlueprints.ContainsKey(blueprintpath))
                return speciesBlueprints[blueprintpath];
            else return "";
        }

        public int speciesIndex(string species)
        {
            species = speciesName(species);
            return speciesNames.IndexOf(species);
        }
    }
}

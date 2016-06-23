using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{
    [DataContract]
    public class Values
    {
        public static Values V;
        [DataMember]
        public int version = 0;
        [DataMember]
        public List<Species> species = new List<Species>();
        public List<string> speciesNames = new List<string>();
        [DataMember]
        public double[][] statMultipliers = new double[8][]; // official server stats-multipliers
        [DataMember]
        public double imprintingMultiplier = 1;
        [DataMember]
        public Dictionary<string, TamingFood> foodData = new Dictionary<string, TamingFood>();

        public Values()
        {
            V = this;
        }

        public bool loadValuesFile()
        {
            version = 0;
            // read species-stats from file
            string path = "values.txt";

            // check if file exists
            if (!File.Exists(path))
            {
                MessageBox.Show("Values-File '" + path + "' not found. This tool will not work properly without that file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string[] rows, values;
            rows = File.ReadAllLines(path);
            int c = -1;
            int s = 0;
            bool multiplierRows = false;
            int ivalue;
            double dvalue;
            species.Clear();
            speciesNames.Clear();
            for (int st = 0; st < 8; st++)
            {
                statMultipliers[st] = new double[] { 1, 1, 1, 1 };
            }
            foreach (string row in rows)
            {
                if (row.Length > 1 && row.Substring(0, 2) == "//")
                    continue;
                else
                {
                    if (row.Length > 0 && row.Substring(0, 1) == "!")
                    {
                        Int32.TryParse(row.Substring(1), out version);
                    }
                    else
                    {
                        values = row.Split(';');
                        if (values.Length == 1)
                        {
                            if (values[0] == "multipliers")
                            {
                                multiplierRows = true;
                                s = 0;
                            }
                            else if (values[0] != "")
                            {
                                // new creature
                                multiplierRows = false;
                                string speciesName = values[0].Trim();
                                speciesNames.Add(speciesName);
                                var spec = new Species(speciesName);
                                species.Add(spec);
                                s = 0;
                                c++;
                            }
                        }
                        else if (values.Length > 1)
                        {
                            if (multiplierRows && s < 8)
                            {
                                for (int v = 0; v < values.Length && v < 4; v++)
                                {
                                    if (Double.TryParse(values[v], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out dvalue))
                                        statMultipliers[s][v] = dvalue;
                                }
                            }
                            else if (s < 8)
                            {
                                // stats
                                double[] stat = new double[5];
                                for (int v = 0; v < values.Length && v < 5; v++)
                                {
                                    if (v == 0 && (s == 5 || s == 6))
                                    {
                                        // damage and speed are handled as percentage of a hidden base value, this tool uses 100% as base, as seen ingame
                                        stat[0] = 1;
                                    }
                                    else
                                    {
                                        Double.TryParse(values[v], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out dvalue);
                                        stat[v] = dvalue;
                                    }
                                }
                                species[c].statsRaw[s].setValues(stat);
                            }
                            else if (s == 8)
                            {
                                // breeding times
                                for (int v = 0; v < values.Length && v < 3; v++)
                                {
                                    Int32.TryParse(values[v], out ivalue);
                                    species[c].breedingTimesRaw[v] = ivalue;
                                }
                            }
                            else if (s > 8 && s < 15)
                            {
                                // colorregions
                                List<int> cIds = new List<int>();
                                species[c].colors[s - 9].name = values[0];
                                for (int v = 0; v < values.Length; v++)
                                {
                                    Int32.TryParse(values[v], out ivalue);
                                    if (ivalue > 0)
                                        cIds.Add(ivalue);
                                }
                                species[c].colors[s - 9].colorIds = cIds;
                            }
                            s++;
                        }
                    }
                }
            }
            // save json
            DataContractJsonSerializer writer = new DataContractJsonSerializer(typeof(Values));
            try
            {
                System.IO.FileStream file = System.IO.File.Create("values.json");
                writer.WriteObject(file, V);
                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + e.Message, "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return true;
        }

        public void applyMultipliersToStats(double[][] multipliers)
        {
            for (int sp = 0; sp < species.Count; sp++)
            {
                for (int s = 0; s < 8; s++)
                {
                    species[sp].stats[s].BaseValue = species[sp].statsRaw[s].BaseValue;
                    // don't apply the multiplier if AddWhenTamed is negative (currently the only case is the Giganotosaurus, which does not get the subtraction multiplied)
                    species[sp].stats[s].AddWhenTamed = species[sp].statsRaw[s].AddWhenTamed * (species[sp].statsRaw[s].AddWhenTamed > 0 ? multipliers[s][0] : 1);
                    species[sp].stats[s].MultAffinity = species[sp].statsRaw[s].MultAffinity * multipliers[s][1];
                    species[sp].stats[s].IncPerTamedLevel = species[sp].statsRaw[s].IncPerTamedLevel * multipliers[s][2];
                    species[sp].stats[s].IncPerWildLevel = species[sp].statsRaw[s].IncPerWildLevel * multipliers[s][3];
                }
            }
        }

        public void applyMultipliersToBreedingTimes(double[] multipliers)
        {
            for (int sp = 0; sp < species.Count; sp++)
            {
                for (int k = 0; k < 3; k++)
                    species[sp].breedingTimes[k] = (int)Math.Ceiling(species[sp].breedingTimesRaw[k] / multipliers[k == 2 ? 1 : 0]);
            }
        }

    }
}

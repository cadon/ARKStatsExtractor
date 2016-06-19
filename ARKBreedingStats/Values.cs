using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public class Values
    {
        public static List<List<CreatureStat>> stats = new List<List<CreatureStat>>();
        public static List<List<CreatureStat>> statsRaw = new List<List<CreatureStat>>(); // without multipliers
        public static List<string> speciesNames = new List<string>();
        public static List<int[]> breedingTimesRaw = new List<int[]>();
        public static List<int[]> breedingTimes = new List<int[]>();
        public static List<ColorRegions> creatureColors = new List<ColorRegions>(); // each creature has up to 6 colorregions
        public static double[][] statMultipliers = new double[8][]; // official server stats-multipliers
        public static double imprintingMultiplier = 1;

        public static bool loadStatFile(out int localFileVer)
        {
            localFileVer = 0;
            // read species-stats from file
            string path = "values.txt";

            // check if file exists
            if (!File.Exists(path))
            {
                MessageBox.Show("Values-File '" + path + "' not found. This tool will not work properly without that file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                string[] rows, values;
                rows = File.ReadAllLines(path);
                int c = -1;
                int s = 0;
                bool multiplierRows = false;
                int ivalue;
                double dvalue;
                stats.Clear();
                statsRaw.Clear();
                speciesNames.Clear();
                breedingTimes.Clear();
                breedingTimesRaw.Clear();
                creatureColors.Clear();
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
                            if (!Int32.TryParse(row.Substring(1), out localFileVer))
                            {
                                localFileVer = 0; // file-version unknown
                            }
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
                                    List<CreatureStat> cs = new List<CreatureStat>();
                                    List<CreatureStat> csr = new List<CreatureStat>();
                                    for (s = 0; s < 8; s++)
                                    {
                                        cs.Add(new CreatureStat((StatName)s));
                                        csr.Add(new CreatureStat((StatName)s));
                                    }
                                    s = 0;
                                    stats.Add(cs);
                                    statsRaw.Add(csr);
                                    creatureColors.Add(new ColorRegions());
                                    breedingTimes.Add(new int[3]);
                                    breedingTimesRaw.Add(new int[3]);
                                    speciesNames.Add(values[0].Trim());
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
                                    statsRaw[c][s].setValues(stat);
                                }
                                else if (s == 8)
                                {
                                    // breeding times
                                    for (int v = 0; v < values.Length && v < 3; v++)
                                    {
                                        Int32.TryParse(values[v], out ivalue);
                                        breedingTimesRaw[c][v] = ivalue;
                                    }
                                }
                                else if (s > 8 && s < 15)
                                {
                                    // colorregions
                                    List<int> cIds = new List<int>();
                                    creatureColors[c].names[s - 9] = values[0];
                                    for (int v = 0; v < values.Length; v++)
                                    {
                                        Int32.TryParse(values[v], out ivalue);
                                        if (ivalue > 0)
                                            cIds.Add(ivalue);
                                    }
                                    creatureColors[c].colorIds[s - 9] = cIds.ToArray();
                                    creatureColors[c].regionUsed[s - 9] = true;
                                }
                                s++;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static void applyMultipliersToStats(double[][] multipliers)
        {
            for (int sp = 0; sp < statsRaw.Count; sp++)
            {
                for (int s = 0; s < 8; s++)
                {
                    stats[sp][s].BaseValue = statsRaw[sp][s].BaseValue;
                    // don't apply the multiplier if AddWhenTamed is negative (currently the only case is the Giganotosaurus, which does not get the subtraction multiplied)
                    stats[sp][s].AddWhenTamed = statsRaw[sp][s].AddWhenTamed * (statsRaw[sp][s].AddWhenTamed > 0 ? multipliers[s][0] : 1);
                    stats[sp][s].MultAffinity = statsRaw[sp][s].MultAffinity * multipliers[s][1];
                    stats[sp][s].IncPerTamedLevel = statsRaw[sp][s].IncPerTamedLevel * multipliers[s][2];
                    stats[sp][s].IncPerWildLevel = statsRaw[sp][s].IncPerWildLevel * multipliers[s][3];
                }
            }
        }

        public static void applyMultipliersToBreedingTimes(double[] multipliers)
        {
            for (int sp = 0; sp < breedingTimesRaw.Count; sp++)
            {
                for (int k = 0; k < 3; k++)
                    breedingTimes[sp][k] = (int)Math.Ceiling(breedingTimesRaw[sp][k] / multipliers[k == 2 ? 1 : 0]);
            }
        }

    }
}

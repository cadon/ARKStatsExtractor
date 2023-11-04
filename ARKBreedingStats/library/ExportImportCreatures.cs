using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Exports creature infos to the clipboard and imports them.
    /// </summary>
    public static class ExportImportCreatures
    {
        private const string ClipboardCreatureFormat = "ASBCreature";

        /// <summary>
        /// Export info for a spreadsheet.
        /// </summary>
        /// <param name="creatures"></param>
        public static int ExportTable(IList<Creature> creatures)
        {
            var fields = Properties.Settings.Default.CreatureTableExportFields;
            if (fields == null)
            {
                fields = new[] { 0, 2, 3, 4, 5, 6, 16 };
            }

            if (!fields.Any()) return 0;

            var output = new StringBuilder();
            var secondaryLanguage = Loc.UseSecondaryCulture;

            // header
            foreach (TableExportFields f in fields)
            {
                switch (f)
                {
                    case TableExportFields.WildLevels:
                        foreach (var si in Stats.DisplayOrder)
                            output.Append(Utils.StatName(si, true, secondaryLanguage: secondaryLanguage) + "_w\t");
                        break;
                    case TableExportFields.DomLevels:
                        foreach (var si in Stats.DisplayOrder)
                            output.Append(Utils.StatName(si, true, secondaryLanguage: secondaryLanguage) + "_d\t");
                        break;
                    case TableExportFields.BreedingValues:
                        foreach (var si in Stats.DisplayOrder)
                            output.Append(Utils.StatName(si, true, secondaryLanguage: secondaryLanguage) + "_b\t");
                        break;
                    case TableExportFields.CurrentValues:
                        foreach (var si in Stats.DisplayOrder)
                            output.Append(Utils.StatName(si, true, secondaryLanguage: secondaryLanguage) + "_v\t");
                        break;
                    case TableExportFields.ParentIds:
                        output.Append("MotherId\tFatherId\t");
                        break;
                    case TableExportFields.ParentNames:
                        output.Append("MotherName\tFatherName\t");
                        break;
                    case TableExportFields.ColorIds:
                        output.Append(string.Join("\t", Enumerable.Range(0, Ark.ColorRegionCount).Select(i => $"c{i}")) + "\t");
                        break;
                    case TableExportFields.ColorNames:
                        output.Append(string.Join("\t", Enumerable.Range(0, Ark.ColorRegionCount).Select(i => $"c{i}_Name")) + "\t");
                        break;
                    default:
                        output.Append($"{f}\t");
                        break;
                }
            }

            output.Length--; // remove last tab
            output.AppendLine();

            // creature rows
            foreach (var c in creatures)
            {
                foreach (TableExportFields f in fields)
                {
                    switch (f)
                    {
                        case TableExportFields.Species:
                            output.Append(c.Species.name + "\t");
                            break;
                        case TableExportFields.SpeciesLongName:
                            output.Append(c.Species.DescriptiveNameAndMod + "\t");
                            break;
                        case TableExportFields.Name:
                            output.Append(c.name + "\t");
                            break;
                        case TableExportFields.Sex:
                            output.Append(Loc.S(c.sex.ToString(), secondaryCulture: secondaryLanguage) + "\t");
                            break;
                        case TableExportFields.Owner:
                            output.Append(c.owner + "\t");
                            break;
                        case TableExportFields.Tribe:
                            output.Append(c.tribe + "\t");
                            break;
                        case TableExportFields.WildLevels:
                            foreach (var si in Stats.DisplayOrder)
                                output.Append($"{c.levelsWild[si]}\t");
                            break;
                        case TableExportFields.DomLevels:
                            foreach (var si in Stats.DisplayOrder)
                                output.Append($"{c.levelsDom[si]}\t");
                            break;
                        case TableExportFields.BreedingValues:
                            foreach (var si in Stats.DisplayOrder)
                                output.Append($"{c.valuesBreeding[si]}\t");
                            break;
                        case TableExportFields.CurrentValues:
                            foreach (var si in Stats.DisplayOrder)
                                output.Append($"{c.valuesDom[si]}\t");
                            break;
                        case TableExportFields.IdInGame:
                            output.Append(c.ArkIdInGame + "\t");
                            break;
                        case TableExportFields.ParentIds:
                            output.Append((c.Mother?.ArkIdInGame ?? string.Empty) + "\t" + (c.Father?.ArkIdInGame ?? string.Empty) + "\t");
                            break;
                        case TableExportFields.ParentNames:
                            output.Append((c.Mother?.name ?? string.Empty) + "\t" + (c.Father?.name ?? string.Empty) + "\t");
                            break;
                        case TableExportFields.MutationCount:
                            output.Append(c.Mutations + "\t");
                            break;
                        case TableExportFields.Fertility:
                            output.Append((c.flags.HasFlag(CreatureFlags.Neutered) ? Loc.S("neutered", secondaryCulture: secondaryLanguage) : string.Empty) + "\t");
                            break;
                        case TableExportFields.Notes:
                            output.Append(c.note + "\t");
                            break;
                        case TableExportFields.ColorIds:
                            for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
                                output.Append($"{c.colors[ci]}\t");
                            break;
                        case TableExportFields.ColorNames:
                            for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
                                output.Append($"{CreatureColors.CreatureColorName(c.colors[ci])}\t");
                            break;
                        case TableExportFields.ServerName:
                            output.Append(c.server + "\t");
                            break;
                        case TableExportFields.AddedToLibrary:
                            output.Append((c.addedToLibrary?.ToString() ?? string.Empty) + "\t");
                            break;
                        case TableExportFields.CreatureStatus:
                            output.Append(c.Status + "\t");
                            break;
                    }
                }
                output.Length--; // remove last tab
                output.AppendLine();
            }

            try
            {
                Clipboard.SetText(output.ToString());
                return creatures.Count();
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex);
            }

            return 0;
        }

        /// <summary>
        /// The fields that can be included in a creature table export.
        /// </summary>
        public enum TableExportFields
        {
            Species, SpeciesLongName, Name, Sex, Owner, Tribe, WildLevels, DomLevels, BreedingValues, CurrentValues, IdInGame, ParentIds, ParentNames, MutationCount, Fertility, Notes, ColorIds, ColorNames, ServerName, AddedToLibrary, CreatureStatus
        }

        /// <summary>
        /// Export the data of a creature to the clipboard in plain text.
        /// </summary>
        /// <param name="c">Creature to export</param>
        /// <param name="breeding">Stat values that are inherited</param>
        /// <param name="ARKml">True if ARKml markup for coloring should be used. That feature was disabled in the ARK-chat.</param>
        public static void ExportToClipboard(Creature c, bool breeding = true, bool ARKml = false)
        {
            if (c == null) return;

            var creatureString = CreatureStringInfo(c, breeding, ARKml);

            string creatureSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(c);

            DataObject o = new DataObject();
            o.SetData(DataFormats.UnicodeText, creatureString);
            if (!string.IsNullOrEmpty(creatureSerialized))
                o.SetData(ClipboardCreatureFormat, creatureSerialized);

            try
            {
                Clipboard.SetDataObject(o, true);
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex);
            }
        }

        /// <summary>
        /// Creates a string that describes the creature.
        /// </summary>
        private static StringBuilder CreatureStringInfo(Creature c, bool breeding, bool ARKml)
        {
            var maxChartLevel = CreatureCollection.CurrentCreatureCollection?.maxChartLevel ?? 0;
            double colorFactor = maxChartLevel > 0 ? 100d / maxChartLevel : 1;
            string modifierText = string.Empty;
            var secondaryLanguage = Loc.UseSecondaryCulture;
            if (!breeding)
            {
                if (!c.isDomesticated)
                    modifierText = ", wild";
                else if (!c.isBred)
                    modifierText = ", TE: " + (c.tamingEff >= 0 ? Math.Round(100 * c.tamingEff, 1) + " %" : "unknown");
                else if (c.imprintingBonus >= 0)
                    modifierText = ", Impr: " + Math.Round(100 * c.imprintingBonus, 2) + " %";
            }

            var output = new StringBuilder((string.IsNullOrEmpty(c.name) ? "noName" : c.name) + " (" +
                                           (ARKml ? Utils.GetARKml(c.Species.name, 50, 172, 255) : c.Species.name)
                                           + ", Lvl " + (breeding ? c.LevelHatched : c.Level) + modifierText +
                                           (c.sex != Sex.Unknown ? ", " + Loc.S(c.sex.ToString(), secondaryCulture: secondaryLanguage) : string.Empty) + "): ");
            for (int s = 0; s < Stats.StatsCount; s++)
            {
                int si = Stats.DisplayOrder[s];
                if (c.levelsWild[si] >= 0 &&
                    c.valuesBreeding[si] > 0) // ignore unknown levels (e.g. oxygen, speed for some species)
                    output.Append(Utils.StatName(si, true, secondaryLanguage: secondaryLanguage) + ": " +
                                  (breeding ? c.valuesBreeding[si] : c.valuesDom[si]) * (Utils.Precision(si) == 3 ? 100 : 1) +
                                  (Utils.Precision(si) == 3 ? " %" : string.Empty) +
                                  " (" + (ARKml
                                      ? Utils.GetARKmlFromPercent(c.levelsWild[si].ToString(),
                                          (int)(c.levelsWild[si] *
                                                 (si == Stats.Torpidity ? colorFactor / 7 : colorFactor)))
                                      : c.levelsWild[si].ToString()) +
                                  (ARKml ? breeding || si == Stats.Torpidity ? string.Empty :
                                      ", " + Utils.GetARKmlFromPercent(c.levelsDom[si].ToString(),
                                          (int)(c.levelsDom[si] * colorFactor)) :
                                      breeding || si == Stats.Torpidity ? string.Empty : ", " + c.levelsDom[si]) +
                                  "); ");
            }

            output.Length--; // remove last space
            return output;
        }

        public static Creature ImportFromClipboard()
        {
            try
            {
                var creatureSerialized = Clipboard.GetData(ClipboardCreatureFormat) as string;
                if (!string.IsNullOrEmpty(creatureSerialized))
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Creature>(creatureSerialized);
                return ParseCreature(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Unknown data in clipboard. Could not paste creature data.");
            }

            return null;
        }

        /// <summary>
        /// import creature values from plain text
        /// </summary>

        private static Creature ParseCreature(string creatureValues)
        {
            if (string.IsNullOrEmpty(creatureValues)) return null;

            const string statRegex = @"(?: (\w+): [\d.]+(?: ?%)? \((\d+)(?:, (\d+))?\);)?"; // TODO mutated levels

            Regex r = new Regex(
                @"(.*?) \(([^,]+), Lvl \d+(?:, (?:wild|TE: ([\d.]+) ?%|Impr: ([\d.]+) ?%))?(?:, (Female|Male))?\):" + string.Concat(Enumerable.Repeat(statRegex, Stats.StatsCount)));
            Match m = r.Match(creatureValues);
            if (!m.Success) return null;

            if (!Values.V.TryGetSpeciesByName(m.Groups[2].Value, out Species species))
            {
                MessageBoxes.ShowMessageBox($"{Loc.S("unknownSpecies")}:\n" + m.Groups[2].Value);
                return null;
            }

            double.TryParse(m.Groups[3].Value, out double te);
            te *= .01;
            double.TryParse(m.Groups[4].Value, out double ib);
            ib *= .01;

            Sex sex = Sex.Unknown;
            switch (m.Groups[5].Value)
            {
                case "Female":
                    sex = Sex.Female;
                    break;
                case "Male":
                    sex = Sex.Male;
                    break;
            }

            // stat levels start in the match group 6, each stat has 3 groups: 0: stat abbreviation, 1: wild level, 2: dom level
            int[] wl = new int[Stats.StatsCount];
            int[] dl = new int[Stats.StatsCount];
            var statAbToIndex = Enumerable.Range(0, Stats.StatsCount)
                .ToDictionary(si => Utils.StatName(si, true, species.statNames), si => si);

            for (int s = 0; s < Stats.StatsCount; s++)
            {
                if (!statAbToIndex.TryGetValue(m.Groups[6 + 3 * s].Value, out var si)) continue;

                int.TryParse(m.Groups[7 + 3 * s].Value, out wl[si]);
                if (si != Stats.Torpidity)
                    int.TryParse(m.Groups[8 + 3 * s].Value, out dl[si]);
            }

            return new Creature(species, m.Groups[1].Value, sex: sex, levelsWild: wl, levelsDom: dl,
                tamingEff: te, isBred: ib > 0, imprinting: ib);
        }

        public static bool ImportCreaturesFromTsvFile(string filePath, out List<Creature> importedCreatures, out string result)
        {
            importedCreatures = null;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                result = $"The import file \"{filePath}\" was not found";
                return false;
            }

            var lines = File.ReadAllLines(filePath);

            // the field order is fixed
            // speciesName, speciesBlueprintPath, name, owner, imprinter, tribe, server, note, sex [m/w], status, isBred [bool], neutered [bool], mutagen [bool],
            // taming effectiveness [double], imprinting bonus [double], mutations maternal [int], mutations paternal [int], 12 stat levels wild [int], 12 stat levels dom [int], 6 color ids [int]
            // in total: 10 string fields, 3 bool fields, 2 double fields (each two regex groups), 32 int fields
            var displayNeededFormat = false;

            var regex = new Regex(
                string.Join(string.Empty, Enumerable.Repeat(@"([^\t]*)\t", 13))
                + string.Join(string.Empty, Enumerable.Repeat(@" ?([\d,.]*) ?(%)? ?\t", 2))
                + string.Join(@"\t", Enumerable.Repeat(@" ?(\d*) ?", 32))
                );
            importedCreatures = new List<Creature>();
            var lineCount = lines.Length;
            var resultSb = new StringBuilder($"Import result\n{lineCount} lines read from\n{filePath}\n");
            var linesCouldNotBeReadCounter = 0;

            for (var i = 0; i < lineCount; i++)
            {
                var line = lines[i];
                var m = regex.Match(line);
                if (!m.Success)
                {
                    if (++linesCouldNotBeReadCounter < 10)
                    {
                        resultSb.AppendLine($"couldn't read line {i + 1}, format couldn't be recognized.");
                        displayNeededFormat = true;
                    }
                    else
                    {
                        resultSb.AppendLine("couldn't read too many lines, aborting import");
                        break;
                    }
                    continue;
                }

                var blueprintPath = m.Groups[2].Value.Trim();
                var species = Values.V.SpeciesByBlueprint(blueprintPath);
                if (species == null && Values.V.TryGetSpeciesByName(m.Groups[1].Value.Trim(), out var sp))
                    species = sp;
                if (species == null)
                {
                    resultSb.AppendLine($"Species for creature in line {i + 1} couldn't be recognized, skipping this line");
                    continue;
                }

                var sex = Sex.Unknown;
                switch (m.Groups[9].Value.Trim().ToLowerInvariant())
                {
                    case "w":
                    case "♀": sex = Sex.Female; break;
                    case "m":
                    case "♂": sex = Sex.Male; break;
                }

                var status = Enum.TryParse(m.Groups[10].Value.Trim(), true, out CreatureStatus statusParsed) ? statusParsed : CreatureStatus.Available;

                const NumberStyles numberStyle = NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
                var dotSeparatorCulture = CultureInfo.GetCultureInfo("en-US");
                double ParseDouble(int regExGroup)
                {
                    var val = double.TryParse(m.Groups[regExGroup].Value.Trim().Replace(',', '.'), numberStyle, dotSeparatorCulture, out var valParsed) ? valParsed : 0;
                    if (m.Groups[regExGroup + 1].Value.Trim() == "%") val /= 100;
                    return val;
                }

                bool ParseBool(int regExGroup)
                {
                    var trimmedString = m.Groups[regExGroup].Value.Trim();
                    return !(string.IsNullOrEmpty(trimmedString) || trimmedString.ToLowerInvariant() == "false" || trimmedString == "0");
                }

                var isBred = ParseBool(11);
                var isNeutered = ParseBool(12);
                var isMutagenApplied = ParseBool(13);
                var te = isBred ? 1 : ParseDouble(14);
                var imprintingBonus = isBred ? ParseDouble(16) : 0;
                var mutationsCountM = int.TryParse(m.Groups[18].Value.Trim(), out var mutationsParsed) ? mutationsParsed : 0;
                var mutationsCountP = int.TryParse(m.Groups[19].Value.Trim(), out mutationsParsed) ? mutationsParsed : 0;

                const int groupIndexOfFirstWildLevel = 20;

                var levelsWild = new int[Stats.StatsCount];
                var levelsDom = new int[Stats.StatsCount];
                for (int si = 0; si < Stats.StatsCount; si++)
                {
                    levelsWild[si] = int.Parse(m.Groups[groupIndexOfFirstWildLevel + si].Value.Trim());
                    levelsDom[si] = int.Parse(m.Groups[groupIndexOfFirstWildLevel + Stats.StatsCount + si].Value.Trim());
                    // TODO mutated levels
                }

                var colorIds = new byte[Ark.ColorRegionCount];
                for (int ci = 0; ci < Ark.ColorRegionCount; ci++)
                {
                    colorIds[ci] = (byte)int.Parse(m.Groups[groupIndexOfFirstWildLevel + 2 * Stats.StatsCount + ci].Value.Trim());
                }

                var creature = new Creature(species, m.Groups[3].Value.Trim(), m.Groups[4].Value.Trim(), m.Groups[6].Value.Trim(), sex,
                    levelsWild, levelsDom, null, te, isBred, imprintingBonus)
                {
                    imprinterName = m.Groups[5].Value.Trim(),
                    server = m.Groups[7].Value.Trim(),
                    note = m.Groups[8].Value.Trim(),
                    Status = status,
                    flags = (isNeutered ? CreatureFlags.Neutered : CreatureFlags.None)
                            | (isMutagenApplied ? CreatureFlags.MutagenApplied : CreatureFlags.None),
                    mutationsMaternal = mutationsCountM,
                    mutationsPaternal = mutationsCountP,
                    colors = colorIds
                };
                creature.RecalculateCreatureValues(null);

                importedCreatures.Add(creature);
            }

            var importedCreaturesCount = importedCreatures.Count;
            var creaturesWereImported = importedCreaturesCount > 0;
            if (!creaturesWereImported)
            {
                resultSb.AppendLine("No creatures imported.");
            }
            else
            {
                resultSb.AppendLine($"{importedCreaturesCount} creatures imported.");
            }

            if (displayNeededFormat)
            {
                resultSb.AppendLine(@"The expected format is one creature per line, the values separated by a tabulator (\t), in the following order (tab instead of commas):");
                resultSb.AppendLine("speciesName, speciesBlueprintPath, name, owner, imprinter, tribe, server, note, sex [m/w], status, isBred [bool], neutered [bool], mutagen [bool], "
                                    + "taming effectiveness [double], imprinting bonus [double], mutations maternal [int], mutations paternal [int], 12 stat levels wild [int], 12 stat levels dom [int], 6 color ids [int]");
                resultSb.AppendLine("The stat order is: Health, Stamina, Torpidity, Oxygen, Food, Water, Temperature, Weight, MeleeDamageMultiplier, SpeedMultiplier, TemperatureFortitude, CraftingSpeedMultiplier");
                resultSb.AppendLine("If speciesBlueprintPath is given, this is used to determine the species. If that's empty, speciesName is used to determine the species, for some species that may result in the wrong species.");
                resultSb.AppendLine("Boolean values are interpreted as false if empty, whitespace, 0 or false, and true else");
            }

            result = resultSb.ToString();
            return creaturesWereImported;
        }
    }
}

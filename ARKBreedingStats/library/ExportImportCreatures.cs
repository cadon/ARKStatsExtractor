using System;
using System.Collections.Generic;
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
        public static void ExportTable(IEnumerable<Creature> creatures)
        {
            var fields = Properties.Settings.Default.CreatureTableExportFields;
            if (fields == null)
            {
                fields = new[] { 0, 2, 3, 4, 5, 6, 16 };
            }

            if (!fields.Any()) return;

            var output = new StringBuilder();

            // header
            foreach (TableExportFields f in fields)
            {
                switch (f)
                {
                    case TableExportFields.WildLevels:
                        foreach (var si in Values.statsDisplayOrder)
                            output.Append(Utils.StatName(si, true) + "_w\t");
                        break;
                    case TableExportFields.DomLevels:
                        foreach (var si in Values.statsDisplayOrder)
                            output.Append(Utils.StatName(si, true) + "_d\t");
                        break;
                    case TableExportFields.BreedingValues:
                        foreach (var si in Values.statsDisplayOrder)
                            output.Append(Utils.StatName(si, true) + "_b\t");
                        break;
                    case TableExportFields.CurrentValues:
                        foreach (var si in Values.statsDisplayOrder)
                            output.Append(Utils.StatName(si, true) + "_v\t");
                        break;
                    case TableExportFields.ParentIds:
                        output.Append("MotherId\tFatherId\t");
                        break;
                    case TableExportFields.ParentNames:
                        output.Append("MotherName\tFatherName\t");
                        break;
                    case TableExportFields.ColorIds:
                        output.Append(string.Join("\t", Enumerable.Range(0, Species.ColorRegionCount).Select(i => $"c{i}")) + "\t");
                        break;
                    case TableExportFields.ColorNames:
                        output.Append(string.Join("\t", Enumerable.Range(0, Species.ColorRegionCount).Select(i => $"c{i}_Name")) + "\t");
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
                            output.Append(c.sex + "\t");
                            break;
                        case TableExportFields.Owner:
                            output.Append(c.owner + "\t");
                            break;
                        case TableExportFields.Tribe:
                            output.Append(c.tribe + "\t");
                            break;
                        case TableExportFields.WildLevels:
                            foreach (var si in Values.statsDisplayOrder)
                                output.Append($"{c.levelsWild[si]}\t");
                            break;
                        case TableExportFields.DomLevels:
                            foreach (var si in Values.statsDisplayOrder)
                                output.Append($"{c.levelsDom[si]}\t");
                            break;
                        case TableExportFields.BreedingValues:
                            foreach (var si in Values.statsDisplayOrder)
                                output.Append($"{c.valuesBreeding[si]}\t");
                            break;
                        case TableExportFields.CurrentValues:
                            foreach (var si in Values.statsDisplayOrder)
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
                            output.Append((c.flags.HasFlag(CreatureFlags.Neutered) ? "neutered" : string.Empty) + "\t");
                            break;
                        case TableExportFields.Notes:
                            output.Append(c.note + "\t");
                            break;
                        case TableExportFields.ColorIds:
                            for (int ci = 0; ci < Species.ColorRegionCount; ci++)
                                output.Append($"{c.colors[ci]}\t");
                            break;
                        case TableExportFields.ColorNames:
                            for (int ci = 0; ci < Species.ColorRegionCount; ci++)
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
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex);
            }
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
            bool wild = c.tamingEff == -2;
            string modifierText = string.Empty;
            if (!breeding)
            {
                if (wild)
                    modifierText = ", wild";
                else if (c.tamingEff < 1)
                    modifierText = ", TE: " + Math.Round(100 * c.tamingEff, 1) + " %";
                else if (c.imprintingBonus > 0)
                    modifierText = ", Impr: " + Math.Round(100 * c.imprintingBonus, 2) + " %";
            }

            var output = new StringBuilder((string.IsNullOrEmpty(c.name) ? "noName" : c.name) + " (" +
                                           (ARKml ? Utils.GetARKml(c.Species.name, 50, 172, 255) : c.Species.name)
                                           + ", Lvl " + (breeding ? c.LevelHatched : c.Level) + modifierText +
                                           (c.sex != Sex.Unknown ? ", " + c.sex : string.Empty) + "): ");
            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                int si = Values.statsDisplayOrder[s];
                if (c.levelsWild[si] >= 0 &&
                    c.valuesBreeding[si] > 0) // ignore unknown levels (e.g. oxygen, speed for some species)
                    output.Append(Utils.StatName(si, true) + ": " +
                                  (breeding ? c.valuesBreeding[si] : c.valuesDom[si]) * (Utils.Precision(si) == 3 ? 100 : 1) +
                                  (Utils.Precision(si) == 3 ? " %" : string.Empty) +
                                  " (" + (ARKml
                                      ? Utils.GetARKmlFromPercent(c.levelsWild[si].ToString(),
                                          (int)(c.levelsWild[si] *
                                                 (si == (int)StatNames.Torpidity ? colorFactor / 7 : colorFactor)))
                                      : c.levelsWild[si].ToString()) +
                                  (ARKml ? breeding || si == (int)StatNames.Torpidity ? string.Empty :
                                      ", " + Utils.GetARKmlFromPercent(c.levelsDom[si].ToString(),
                                          (int)(c.levelsDom[si] * colorFactor)) :
                                      breeding || si == (int)StatNames.Torpidity ? string.Empty : ", " + c.levelsDom[si]) +
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

            const string statRegex = @"(?: (\w+): [\d.]+(?: ?%)? \((\d+)(?:, (\d+))?\);)?";

            Regex r = new Regex(
                @"(.*?) \(([^,]+), Lvl \d+(?:, (?:wild|TE: ([\d.]+) ?%|Impr: ([\d.]+) ?%))?(?:, (Female|Male))?\):" + string.Concat(Enumerable.Repeat(statRegex, Values.STATS_COUNT)));
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
            int[] wl = new int[Values.STATS_COUNT];
            int[] dl = new int[Values.STATS_COUNT];
            var statAbToIndex = Enumerable.Range(0, Values.STATS_COUNT)
                .ToDictionary(si => Utils.StatName(si, true, species.statNames), si => si);

            for (int s = 0; s < Values.STATS_COUNT; s++)
            {
                if (!statAbToIndex.TryGetValue(m.Groups[6 + 3 * s].Value, out var si)) continue;

                int.TryParse(m.Groups[7 + 3 * s].Value, out wl[si]);
                if (si != (int)StatNames.Torpidity)
                    int.TryParse(m.Groups[8 + 3 * s].Value, out dl[si]);
            }

            return new Creature(species, m.Groups[1].Value, sex: sex, levelsWild: wl, levelsDom: dl,
                tamingEff: te, isBred: ib > 0, imprinting: ib);
        }

    }
}

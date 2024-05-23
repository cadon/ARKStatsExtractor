using System;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    internal static class LibraryInfo
    {
        private static Species _infoForSpecies;
        private static bool _libraryFilterConsidered;
        private static string _speciesInfo;
        /// <summary>
        /// All color ids existing for this species per region.
        /// </summary>
        public static readonly HashSet<byte>[] ColorsExistPerRegion = new HashSet<byte>[Ark.ColorRegionCount];
        public static readonly HashSet<byte> ColorsExistInAllRegions = new HashSet<byte>();
        public static readonly HashSet<byte> ColorsExistInAllUsedRegions = new HashSet<byte>();

        /// <summary>
        /// Clear the cached information.
        /// </summary>
        internal static void ClearInfo()
        {
            _infoForSpecies = null;
            _speciesInfo = null;
        }

        internal static string GetSpeciesInfo() => _speciesInfo;

        /// <summary>
        /// Returns information about what color ids exist in which regions of the creatures of a species.
        /// </summary>
        internal static bool SetColorInfo(Species species, IList<Creature> creatures, bool libraryFilterConsidered, TableLayoutPanel tlp = null)
        {
            if (species == null || creatures == null) return false;
            if (species == _infoForSpecies
                && _libraryFilterConsidered == libraryFilterConsidered
                && _infoForSpecies != null) return true;

            _infoForSpecies = species;
            _libraryFilterConsidered = libraryFilterConsidered;
            if (tlp != null)
            {
                tlp.SuspendLayout();
                tlp.Controls.Clear();
            }

            var colorsDontExistPerRegion = new HashSet<byte>[Ark.ColorRegionCount];
            var allAvailableColorIds = Values.V.Colors.ColorsList.Select(c => c.Id).ToArray();
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                ColorsExistPerRegion[i] = new HashSet<byte>();
                colorsDontExistPerRegion[i] = new HashSet<byte>(allAvailableColorIds);
            }

            var properties = new Dictionary<CreatureFlags, int>();
            var flags = ((CreatureFlags[])Enum.GetValues(typeof(CreatureFlags))).Where(f => f != CreatureFlags.None).ToArray();
            foreach (var flag in flags)
                properties[flag] = 0;
            var creatureCount = 0;

            var regionsUsed = _infoForSpecies.colors?.Select(r => !string.IsNullOrEmpty(r?.name)).ToArray()
                              ?? Enumerable.Repeat(true, Ark.ColorRegionCount).ToArray();
            var speciesUsesAllRegions = regionsUsed.All(u => u);

            var creaturesEqualColors = new List<(Creature, byte)>();
            var creaturesUsedEqualColors = new List<(Creature, byte)>();

            foreach (var cr in creatures)
            {
                if (cr.speciesBlueprint != species.blueprintPath
                    || cr.flags.HasFlag(CreatureFlags.Placeholder)
                    || cr.flags.HasFlag(CreatureFlags.Dead)
                    || cr.colors == null)
                    continue;

                var allColorsEqual = -1;
                var allUsedRegionColorsEqual = -1;

                creatureCount++;
                for (var ri = 0; ri < Ark.ColorRegionCount; ri++)
                {
                    var co = cr.colors[ri];

                    if (allColorsEqual == -1)
                        allColorsEqual = co;
                    else if (allColorsEqual != co)
                        allColorsEqual = -2;

                    if (!speciesUsesAllRegions && regionsUsed[ri])
                    {
                        if (allUsedRegionColorsEqual == -1)
                            allUsedRegionColorsEqual = co;
                        else if (allUsedRegionColorsEqual != co)
                            allUsedRegionColorsEqual = -2;
                    }

                    if (ColorsExistPerRegion[ri].Contains(co)) continue;
                    ColorsExistPerRegion[ri].Add(co);
                    colorsDontExistPerRegion[ri].Remove(co);
                }

                if (allColorsEqual >= 0)
                    creaturesEqualColors.Add((cr, (byte)allColorsEqual));
                if (allUsedRegionColorsEqual >= 0)
                    creaturesUsedEqualColors.Add((cr, (byte)allUsedRegionColorsEqual));

                foreach (var flag in flags)
                {
                    if (cr.flags.HasFlag(flag))
                        properties[flag]++;
                }
            }
            SetColorsAvailableInAllRegions(allAvailableColorIds, regionsUsed);

            var sb = new StringBuilder();
            var tableRow = 1;

            void AddParagraph(string text, string suffixForPlainText = null, bool bold = false, float relativeFontSize = 1)
            {
                if (tlp != null)
                {
                    while (tlp.RowStyles.Count <= tableRow)
                        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    var l = new Label
                    {
                        Text = text,
                        Font = bold || relativeFontSize != 1
                            ? new Font(Control.DefaultFont.FontFamily, Control.DefaultFont.Size * relativeFontSize,
                                bold ? FontStyle.Bold : FontStyle.Regular)
                            : Control.DefaultFont,
                        Margin = new Padding(3, bold ? 8 : 3, 3, 3),
                        AutoSize = true,
                        MaximumSize = new Size(440, 0)
                    };
                    tlp.Controls.Add(l, 0, tableRow++);
                }

                sb.AppendLine(text + suffixForPlainText);
            }

            AddParagraph($"Information about the {species.DescriptiveNameAndMod} in this library", "\n", true, 1.5f);
            AddParagraph(
                $"{creatureCount} creatures. {string.Join(", ", properties.Where(p => p.Value > 0).Select(p => $"{Loc.S(p.Key.ToString())}: {p.Value}"))}",
                "\n");
            AddParagraph("Color information", null, true, 1.3f);

            var rangeSb = new StringBuilder();
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                if (!species.EnabledColorRegions[i]) continue;
                AddParagraph($"Color region {i}: {species.colors?[i]?.name}", bold: true, relativeFontSize: 1.1f);
                var colorsExist = ColorsExistPerRegion[i].Count;
                AddParagraph($"{colorsExist} color id{(colorsExist != 1 ? "s" : string.Empty)} available in your library:");
                AddParagraph(CreateNumberRanges(ColorsExistPerRegion[i]));
                var colorsDontExist = colorsDontExistPerRegion[i].Count;
                AddParagraph($"{colorsDontExist} color id{(colorsDontExist != 1 ? "s" : string.Empty)} missing in your library:");
                AddParagraph(CreateNumberRanges(colorsDontExistPerRegion[i]), "\n");
            }

            var regionsUsedList = string.Join(", ", regionsUsed.Select((used, ri) => (used, ri)).Where(r => r.used)
                .Select(r => r.ri));
            if (string.IsNullOrEmpty(regionsUsedList))
                regionsUsedList = "species uses no region";
            if (!speciesUsesAllRegions && ColorsExistInAllUsedRegions.Any())
            {
                AddParagraph($"These colors exist in all regions the {_infoForSpecies.name} uses ({regionsUsedList})", bold: true, relativeFontSize: 1.1f);
                AddParagraph($"{ColorsExistInAllUsedRegions.Count} colors (each color not necessarily in one creature combined)");
                AddParagraph(CreateNumberRanges(ColorsExistInAllUsedRegions));
            }

            if (ColorsExistInAllRegions.Any())
            {
                AddParagraph("These colors exist in all regions", bold: true, relativeFontSize: 1.1f);
                AddParagraph($"{ColorsExistInAllRegions.Count} colors (each color not necessarily in one creature combined)");
                AddParagraph(CreateNumberRanges(ColorsExistInAllRegions));
            }

            if (!speciesUsesAllRegions && creaturesUsedEqualColors.Any())
            {
                AddParagraph($"These colors exist in all regions the {_infoForSpecies.name} uses ({regionsUsedList}) in a single creature", bold: true, relativeFontSize: 1.1f);
                AddParagraph("For each of these colors there's a creature that only has that color in the used regions: "
                             + CreateNumberRanges(creaturesUsedEqualColors.Select(cc => cc.Item2).ToHashSet()));
                AddParagraph(string.Join(Environment.NewLine,
                    creaturesUsedEqualColors.GroupBy(cc => cc.Item2).OrderBy(g => g.Key)
                        .Select(g => $"{g.Key}: {string.Join(", ", g.Select(c => c.Item1.name))}")));
            }

            if (creaturesEqualColors.Any())
            {
                AddParagraph("These colors exist in all regions in a single creature", bold: true, relativeFontSize: 1.1f);
                AddParagraph("For each of these colors there's a creature that only has that color in all regions: "
                             + CreateNumberRanges(creaturesEqualColors.Select(cc => cc.Item2).ToHashSet()));
                AddParagraph(string.Join(Environment.NewLine,
                    creaturesEqualColors.GroupBy(cc => cc.Item2).OrderBy(g => g.Key)
                        .Select(g => $"{g.Key}: {string.Join(", ", g.Select(c => c.Item1.name))}")));
            }

            string CreateNumberRanges(HashSet<byte> numbers)
            {
                var count = numbers.Count;
                if (count == 0) return null;
                if (count == 1)
                {
                    return numbers.First().ToString();
                }

                rangeSb.Clear();
                int lastNumber = -2;
                bool currentlyInRange = false;
                foreach (var number in numbers.OrderBy(c => c))
                {
                    var lastNumberOfSet = --count == 0;
                    if (lastNumber + 1 == number)
                    {
                        if (lastNumberOfSet)
                        {
                            if (currentlyInRange)
                                rangeSb.Append($"-{number}");
                            else rangeSb.Append($", {number}");
                        }
                        currentlyInRange = true;
                    }
                    else if (currentlyInRange)
                    {
                        // was a number range that now ends
                        rangeSb.Append($"-{lastNumber}, {number}");
                        currentlyInRange = false;
                    }
                    else
                    {
                        if (lastNumber == -2)
                            rangeSb.Append($"{number}"); // first number of row
                        else
                            rangeSb.Append($", {number}");
                    }
                    lastNumber = number;
                }

                return rangeSb.ToString();
            }

            _speciesInfo = sb.ToString();
            tlp?.ResumeLayout();
            return true;
        }

        /// <summary>
        /// Stores color ids that are available in all (used) regions in creatures of the species.
        /// </summary>
        /// <param name="allAvailableColorIds"></param>
        /// <param name="regionsUsed"></param>
        private static void SetColorsAvailableInAllRegions(byte[] allAvailableColorIds, bool[] regionsUsed)
        {
            ColorsExistInAllRegions.Clear();
            ColorsExistInAllUsedRegions.Clear();

            if (!ColorsExistPerRegion.Any(r => r.Any())) return;

            foreach (var colorId in allAvailableColorIds)
            {
                var inAllRegions = true;
                var inAllUsedRegions = true;
                var speciesUsesAnyRegion = false;
                for (int r = 0; r < Ark.ColorRegionCount; r++)
                {
                    var inThisRegion = ColorsExistPerRegion[r].Contains(colorId);
                    inAllRegions = inAllRegions && inThisRegion;
                    if (regionsUsed[r])
                    {
                        speciesUsesAnyRegion = true;
                        if (inThisRegion) continue;
                        inAllUsedRegions = false;
                        break;
                    }
                }

                if (inAllRegions)
                    ColorsExistInAllRegions.Add(colorId);
                if (inAllUsedRegions && speciesUsesAnyRegion)
                    ColorsExistInAllUsedRegions.Add(colorId);
            }
        }
    }
}

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
                // remove all controls except copy to clipboard button and filter checkbox
                for (int i = tlp.Controls.Count - 1; i > 1; i--)
                    tlp.Controls.RemoveAt(i);
            }

            var colorsExistPerRegion = new HashSet<byte>[Ark.ColorRegionCount];
            var colorsDontExistPerRegion = new HashSet<byte>[Ark.ColorRegionCount];
            var allAvailableColorIds = Values.V.Colors.ColorsList.Select(c => c.Id).ToArray();
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                colorsExistPerRegion[i] = new HashSet<byte>();
                colorsDontExistPerRegion[i] = new HashSet<byte>(allAvailableColorIds);
            }

            var properties = new Dictionary<CreatureFlags, int>();
            var flags = ((CreatureFlags[])Enum.GetValues(typeof(CreatureFlags))).Where(f => f != CreatureFlags.None).ToArray();
            foreach (var flag in flags)
                properties[flag] = 0;
            var creatureCount = 0;

            foreach (var cr in creatures)
            {
                if (cr.speciesBlueprint != species.blueprintPath
                    || cr.flags.HasFlag(CreatureFlags.Placeholder)
                    || cr.flags.HasFlag(CreatureFlags.Dead)
                    || cr.colors == null)
                    continue;

                creatureCount++;
                for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
                {
                    var co = cr.colors[ci];
                    if (colorsExistPerRegion[ci].Contains(co)) continue;
                    colorsExistPerRegion[ci].Add(co);
                    colorsDontExistPerRegion[ci].Remove(co);
                }

                foreach (var flag in flags)
                {
                    if (cr.flags.HasFlag(flag))
                        properties[flag]++;
                }
            }

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
                        AutoSize = true
                    };
                    tlp.Controls.Add(l, 0, tableRow++);
                    tlp.SetColumnSpan(l, 2);
                }

                sb.AppendLine(text + suffixForPlainText);
            }

            AddParagraph($"Information about the {species.DescriptiveNameAndMod} in this library", "\n", true, 1.5f);
            AddParagraph(
                $"{creatureCount} creatures. {string.Join(", ", properties.Where(p => p.Value > 0).Select(p => $"{Loc.S(p.Key.ToString())}: {p.Value}"))}",
                "\n");
            AddParagraph($"Color information", null, true, 1.3f);

            var rangeSb = new StringBuilder();
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                if (!species.EnabledColorRegions[i]) continue;
                AddParagraph($"Color region {i}: {species.colors[i]?.name}", bold: true, relativeFontSize: 1.1f);
                var colorsExist = colorsExistPerRegion[i].Count;
                AddParagraph($"{colorsExist} color id{(colorsExist != 1 ? "s" : string.Empty)} available in your library:");
                AddParagraph(CreateNumberRanges(colorsExistPerRegion[i]));
                var colorsDontExist = colorsDontExistPerRegion[i].Count;
                AddParagraph($"{colorsDontExist} color id{(colorsDontExist != 1 ? "s" : string.Empty)} missing in your library:");
                AddParagraph(CreateNumberRanges(colorsDontExistPerRegion[i]), "\n");
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

    }
}

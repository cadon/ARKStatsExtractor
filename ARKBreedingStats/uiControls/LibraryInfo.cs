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
        private static string _speciesInfo;

        internal static void ClearInfo()
        {
            _infoForSpecies = null;
            _speciesInfo = null;
        }

        internal static string GetSpeciesInfo() => _speciesInfo;

        /// <summary>
        /// Returns information about what color ids exist in which regions of the creatures of a species.
        /// </summary>
        internal static bool SetColorInfo(Species species, IList<Creature> creatures, TableLayoutPanel tlp = null)
        {
            if (species == null || creatures == null) return false;
            if (species == _infoForSpecies) return true;

            _infoForSpecies = species;
            if (tlp != null)
            {
                // remove all controls except copy to clipboard button
                for (int i = tlp.Controls.Count - 1; i > 0; i--)
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

            foreach (var cr in creatures)
            {
                if (cr.speciesBlueprint != species.blueprintPath
                   || cr.colors == null)
                    continue;

                for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
                {
                    var co = cr.colors[ci];
                    if (colorsExistPerRegion[ci].Contains(co)) continue;
                    colorsExistPerRegion[ci].Add(co);
                    colorsDontExistPerRegion[ci].Remove(co);
                }
            }

            var sb = new StringBuilder();
            var tableRow = 1;

            void AddParagraph(string text, string appendToPlainText = null, bool bold = false, float relativeFontSize = 1)
            {
                tlp?.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tlp?.Controls.Add(new Label
                {
                    Text = text,
                    Font = bold || relativeFontSize != 1 ? new Font(Control.DefaultFont.FontFamily, Control.DefaultFont.Size * relativeFontSize, bold ? FontStyle.Bold : FontStyle.Regular) : Control.DefaultFont,
                    Margin = new Padding(3, bold ? 8 : 3, 3, 3),
                    AutoSize = true
                }, 0, tableRow++);
                sb.AppendLine(text + appendToPlainText);
            }

            AddParagraph($"Color information about {species.DescriptiveNameAndMod} ({species.blueprintPath})", "\n", true, 1.5f);

            var rangeSb = new StringBuilder();
            for (int i = 0; i < Ark.ColorRegionCount; i++)
            {
                if (!species.EnabledColorRegions[i]) continue;
                AddParagraph($"Color region {i}: {species.colors[i].name}", bold: true, relativeFontSize: 1.1f);
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
            return true;
        }

    }
}

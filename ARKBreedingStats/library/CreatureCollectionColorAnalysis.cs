using ARKBreedingStats.species;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ARKBreedingStats.library.LevelColorStatusFlags;

namespace ARKBreedingStats.Library
{
    /// <summary>
    /// App-layer extension methods on CreatureCollection that depend on WinForms/UI types.
    /// </summary>
    internal static class CreatureCollectionColorAnalysis
    {
        /// <summary>
        /// Returns a tuple that indicates if a color id is already available in that species
        /// (inTheRegion, inAnyRegion).
        /// </summary>
        /// <param name="creaturesWithColorsInRegion">For each region an array with creature count with this color, i.e. int[regionId][colorId]</param>
        internal static ColorStatus[] DetermineColorStatus(this CreatureCollection cc, Species species, byte[] colorIds, out string infoText, out int[][] creaturesWithColorsInRegion, out bool[] desiredColors)
        {
            infoText = null;
            creaturesWithColorsInRegion = null;
            desiredColors = null;
            if (string.IsNullOrEmpty(species?.blueprintPath) || colorIds == null) return null;

            var usedColorRegionIndices = Enumerable.Range(0, Ark.ColorRegionCount).Where(i => species.EnabledColorRegions[i]).ToArray();
            var usedColorRegionsCount = usedColorRegionIndices.Length;

            // create data if not available in the cache
            if (!cc._existingColors.TryGetValue(species.blueprintPath, out creaturesWithColorsInRegion))
            {
                // count of each color id in each region. The last index contains the count of color ids of all regions
                creaturesWithColorsInRegion = new int[Ark.ColorRegionCount + 1][];
                foreach (var ri in usedColorRegionIndices)
                    creaturesWithColorsInRegion[ri] = new int[byte.MaxValue + 1];
                creaturesWithColorsInRegion[Ark.ColorRegionCount] = new int[byte.MaxValue + 1];

                foreach (var c in cc.creatures)
                {
                    if (c.flags.HasFlag(CreatureFlags.Placeholder)
                        || c.flags.HasFlag(CreatureFlags.Dead)
                        || c.speciesBlueprint != species.blueprintPath
                        || c.Species == null)
                        continue;

                    foreach (var ri in usedColorRegionIndices)
                    {
                        var cColorId = c.colors[ri];
                        creaturesWithColorsInRegion[ri][cColorId]++;
                        creaturesWithColorsInRegion[Ark.ColorRegionCount][cColorId]++;
                    }
                }

                cc._existingColors[species.blueprintPath] = creaturesWithColorsInRegion;
            }

            var newSpeciesColorsString = new List<string>(usedColorRegionsCount);
            var newRegionColorsStrings = new List<string>(usedColorRegionsCount);

            var regionsColorStatus = new ColorStatus[Ark.ColorRegionCount];
            var anyColorNewInRegion = false;
            var anyColorNew = false;
            foreach (var ri in usedColorRegionIndices)
            {
                var colorId = colorIds[ri];
                var creaturesWithColorIdInRegion = creaturesWithColorsInRegion[ri][colorId];
                var creaturesWithColorIdInAnyRegion = creaturesWithColorsInRegion[Ark.ColorRegionCount][colorId];
                var colorStatus = creaturesWithColorIdInRegion > 0 ? ColorStatus.ExistsInRegion
                               : creaturesWithColorIdInAnyRegion > 0 ? ColorStatus.NewRegionColor
                               : ColorStatus.NewColor;
                regionsColorStatus[ri] = colorStatus;
                switch (colorStatus)
                {
                    case ColorStatus.NewColor:
                        var description = ColorDescription();
                        if (!newSpeciesColorsString.Contains(description))
                            newSpeciesColorsString.Add(description);
                        anyColorNew = true;
                        break;
                    case ColorStatus.NewRegionColor:
                        newRegionColorsStrings.Add($"{ColorDescription()} in region {ri}");
                        anyColorNewInRegion = true;
                        break;
                }

                string ColorDescription()
                {
                    var color = CreatureColors.CreatureArkColor(colorId);
                    return $"{color.Name} ({color.Id})";
                }
            }

            //LevelColorStatusFlags.ColorFlags

            // desired colors
            desiredColors = new bool[Ark.ColorRegionCount];
            var colorSpeciesOptions = Form1.ColorOptionsWantedRegions.GetOptions(species);
            for (var ci = 0; ci < Ark.ColorRegionCount; ci++)
                desiredColors[ci] = colorSpeciesOptions.Options[ci].IsColorWanted(colorIds[ci]);

            ColorFlagsCombined = ColorStatus.None;
            if (anyColorNew) ColorFlagsCombined |= ColorStatus.NewColor;
            if (anyColorNewInRegion) ColorFlagsCombined |= ColorStatus.NewRegionColor;
            if (desiredColors.Any(ci => ci)) ColorFlagsCombined |= ColorStatus.DesiredColor;

            // text output
            var infoTextSb = new StringBuilder();
            if (newSpeciesColorsString.Any())
            {
                infoTextSb.AppendLine($"These colors are new for the {species.name}: {string.Join(", ", newSpeciesColorsString)}.");
            }
            if (newRegionColorsStrings.Any())
            {
                infoTextSb.AppendLine($"These colors are new in their region: {string.Join(", ", newRegionColorsStrings)}.");
            }

            infoTextSb.AppendLine();
            infoTextSb.AppendLine("Library analysis");
            infoText = infoTextSb.ToString();
            return regionsColorStatus;
        }
    }
}

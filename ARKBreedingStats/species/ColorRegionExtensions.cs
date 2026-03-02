using ARKBreedingStats.Core;
using System.Collections.Generic;

namespace ARKBreedingStats.species;

/// <summary>
/// Extension methods for Core ColorRegion class.
/// </summary>
internal static class ColorRegionExtensions
{
    /// <summary>
    /// Sets the ARKColor objects for the natural occurring colors.
    /// </summary>
    internal static void Initialize(this ColorRegion colorRegion, ArkColors arkColors)
    {
        if (colorRegion.colors == null) return;
        colorRegion.naturalColors = new List<ArkColor>();
        foreach (var c in colorRegion.colors)
        {
            ArkColor cl = arkColors.ByName(c);
            if (cl.Id != 0 && !colorRegion.naturalColors.Contains(cl))
                colorRegion.naturalColors.Add(cl);
        }
    }
}

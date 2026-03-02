using ARKBreedingStats.Core;

namespace ARKBreedingStats.species;

/// <summary>
/// Extension methods for Core ColorRegion class.
/// </summary>
public static class ColorRegionExtensions
{
    /// <summary>
    /// Sets the ARKColor objects for the natural occurring colors.
    /// </summary>
    public static void Initialize(this ColorRegion colorRegion, ArkColors arkColors)
    {
        if (colorRegion.colors == null) return;
        colorRegion.naturalColors = new System.Collections.Generic.List<ArkColor>();
        foreach (var c in colorRegion.colors)
        {
            ArkColor cl = arkColors.ByName(c);
            if (cl.Id != 0 && !colorRegion.naturalColors.Contains(cl))
                colorRegion.naturalColors.Add(cl);
        }
    }
}

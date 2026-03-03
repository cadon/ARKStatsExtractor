using Newtonsoft.Json;
using System.Collections.Generic;

namespace ARKBreedingStats.Models;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class ColorRegion
{
    [JsonProperty]
    public string? name;

    /// <summary>
    /// List of natural occurring color names.
    /// </summary>
    [JsonProperty]
    public List<string>? colors;

    /// <summary>
    /// This region is not visible in game if true.
    /// </summary>
    [JsonProperty]
    public bool invisible;

    /// <summary>
    /// List of natural occurring ARKColors.
    /// </summary>
    public List<ArkColor>? naturalColors;

    public ColorRegion()
    {
        name = "Unknown";
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace ARKBreedingStats.Models;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class ColorRegion
{
    [JsonProperty]
    public string? name { get; set; }

    /// <summary>
    /// List of natural occurring color names.
    /// </summary>
    [JsonProperty]
    public List<string>? colors { get; set; }

    /// <summary>
    /// This region is not visible in game if true.
    /// </summary>
    [JsonProperty]
    public bool invisible { get; set; }

    /// <summary>
    /// List of natural occurring ARKColors.
    /// </summary>
    public List<ArkColor>? naturalColors { get; set; }

    public ColorRegion()
    {
        name = "Unknown";
    }
}

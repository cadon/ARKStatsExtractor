using Newtonsoft.Json;

namespace ARKBreedingStats.Models;

/// <summary>
/// Food-specific taming values for a species.
/// </summary>
[JsonObject]
public class TamingFood
{
    /// <summary>
    /// Amount of affinity raise if one piece of this food is eaten.
    /// </summary>
    [JsonProperty("a")]
    public double affinity { get; set; }
    
    /// <summary>
    /// Amount of food one of this food gives.
    /// </summary>
    [JsonProperty("f")]
    public double foodValue { get; set; }
    
    /// <summary>
    /// When taming, some foods can only be feed in higher quantities, this indicates that amount.
    /// </summary>
    [JsonProperty("q")]
    public int quantity { get; set; } = 1;

    /// <summary>
    /// If the food data is not completely confirmed or tested, this is true.
    /// </summary>
    [JsonProperty("u")]
    public bool Unconfirmed { get; set; }
}

namespace ARKBreedingStats.Library;

/// <summary>
/// Represents a player or tribe member in ARK.
/// </summary>
public class Player
{
    public string? PlayerName { get; set; }
    public string? Tribe { get; set; }
    public int Level { get; set; }
    public int Rank { get; set; }
    public string? Note { get; set; }
}

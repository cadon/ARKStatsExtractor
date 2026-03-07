namespace ARKBreedingStats.Library;

public class Tribe
{
    public string TribeName { get; set; } = "";
    public Relation TribeRelation { get; set; } = Tribe.Relation.Neutral;
    public string Note { get; set; } = "";

    public enum Relation
    {
        Neutral,
        Allied,
        Friendly,
        Hostile
    }
}

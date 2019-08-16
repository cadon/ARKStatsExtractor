namespace ARKBreedingStats.Library
{
    public class Tribe
    {
        public string TribeName = "";
        public Relation TribeRelation = Tribe.Relation.Neutral;
        public string Note = "";

        public enum Relation
        {
            Neutral,
            Allied,
            Friendly,
            Hostile
        }
    }
}

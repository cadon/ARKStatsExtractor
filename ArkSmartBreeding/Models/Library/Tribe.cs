namespace ArkSmartBreeding.Models.Library
{
    public class Tribe
    {
        public string TribeName = "";
        public Relation TribeRelation = Relation.Neutral;
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

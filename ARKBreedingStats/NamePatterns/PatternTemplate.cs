using Newtonsoft.Json;

namespace ARKBreedingStats.NamePatterns
{
    /// <summary>
    /// A name pattern for a specific usage with explanation, can be selected by the user from a template list.
    /// </summary>
    [JsonObject]
    public class PatternTemplate
    {
        public string Pattern;
        public string Title;
        public string Description;
    }
}

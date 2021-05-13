using Newtonsoft.Json;

namespace ARKBreedingStats.NamePatterns
{
    [JsonObject]
    public struct PatternTemplates
    {
        public string Format;
        public string Version;
        public PatternTemplate[] Patterns;
    }
}

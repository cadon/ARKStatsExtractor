using System.Collections.Generic;
using Newtonsoft.Json;

namespace ARKBreedingStats.SpeciesImages
{
    [JsonObject]
    internal class FileHashList
    {
        [JsonProperty("files")]
        public Dictionary<string, SpeciesImageFileInfo> Files;
    }

    [JsonObject]
    internal class SpeciesImageFileInfo
    {
        [JsonProperty("hash")]
        public string Hash;
    }
}

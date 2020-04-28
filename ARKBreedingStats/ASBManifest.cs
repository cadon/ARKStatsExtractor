using Newtonsoft.Json;
using System.Collections.Generic;

namespace ARKBreedingStats
{
    /// <summary>
    /// Contains infos about the versions of different parts of this application
    /// </summary>
    [JsonObject]
    public class ASBManifest
    {
        /// <summary>
        /// Must be present and a supported value.
        /// </summary>
        private string format;

        /// <summary>
        /// Dictionary of Versions.
        /// Expected is at least the key "version" in each entry.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> versions;
    }
}

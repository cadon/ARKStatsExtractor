using System;
using Newtonsoft.Json;

namespace ARKBreedingStats.Updater
{
    /// <summary>
    /// Generic class for data in a module with format and version.
    /// </summary>
    [JsonObject]
    public class ValueModule<T> where T : class
    {
        [JsonProperty("format")]
        public int Format;
        [JsonProperty("version")]
        public Version Version;

        /// <summary>
        /// Data of the module.
        /// </summary>
        [JsonProperty("data")]
        public T Data;
    }
}

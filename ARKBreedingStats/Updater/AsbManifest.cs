using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ARKBreedingStats.Updater
{
    /// <summary>
    /// Contains infos about the versions of different parts of this application
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class AsbManifest
    {
        /// <summary>
        /// Must be present and a supported value.
        /// </summary>
        [JsonProperty("format")]
        private string _format;

        /// <summary>
        /// Returns true if the format of the manifest file is supported by this application.
        /// </summary>
        internal static bool IsFormatSupported(string format) => format == "1.0";

        /// <summary>
        /// Dictionary of app-modules. The key is the id of the module.
        /// </summary>
        [JsonProperty("modules")]
        public Dictionary<string, AsbModule> Modules;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext _)
        {
            if (Modules == null) return;
            foreach (var kv in Modules) kv.Value.Id = kv.Key;
        }

        //internal static AsbManifest FromJsonString(string json) => JsonConvert.DeserializeObject<AsbManifest>(json);
        internal static AsbManifest FromJsonFile(string filePath) => FileService.LoadJsonFile(filePath, out AsbManifest manifest, out _) ? manifest : null;
    }
}

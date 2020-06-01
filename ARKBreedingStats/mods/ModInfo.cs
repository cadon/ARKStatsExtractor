using ARKBreedingStats.species;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace ARKBreedingStats.mods
{
    /// <summary>
    /// Contains infos about a mod and its version
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ModInfo
    {
        [JsonProperty]
        public string version;
        public Version Version;
        [JsonProperty]
        public Mod mod;
        /// <summary>
        /// Indicates if the according json-file is downloaded.
        /// </summary>
        public bool locallyAvailable;
        /// <summary>
        /// If true the modInfo is available online. If not it's probably manually created.
        /// </summary>
        public bool onlineAvailable;
        public bool currentlyInLibrary;

        [OnDeserialized]
        private void SetVersion(StreamingContext context)
        {
            Version.TryParse(version, out Version);
        }

        public override string ToString()
        {
            return (mod?.title ?? "unknown mod")
                + (onlineAvailable
                    ? (!locallyAvailable ? " (DL)" : string.Empty)
                    : " (Custom)");
        }
    }
}

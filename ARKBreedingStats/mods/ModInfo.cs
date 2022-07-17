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
        public string format;
        [JsonProperty]
        public Mod mod;
        /// <summary>
        /// Indicates if the according json-file is downloaded.
        /// </summary>
        public bool LocallyAvailable;
        /// <summary>
        /// If true the modInfo is available online. If not it's probably manually created.
        /// </summary>
        public bool OnlineAvailable;
        /// <summary>
        /// Only used in the mod selector, is not reliable else.
        /// </summary>
        public bool CurrentlyInLibrary;

        [OnDeserialized]
        private void SetVersion(StreamingContext context)
        {
            Version.TryParse(version, out Version);
        }

        public override string ToString()
        {
            return (mod?.title ?? "unknown mod")
                + (OnlineAvailable
                    ? (!LocallyAvailable ? " (DL)" : string.Empty)
                    : string.IsNullOrEmpty(mod?.FileName) ? string.Empty : " (Custom)");
        }
    }
}

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
        [JsonProperty("version")]
        private string _version;
        public Version Version;
        [JsonProperty("format")]
        public string Format;
        [JsonProperty("mod")]
        public Mod Mod;
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
        private void SetVersion(StreamingContext _) => Version.TryParse(_version, out Version);

        public override string ToString() =>
            (Mod?.Title ?? "unknown mod")
            + (OnlineAvailable
                ? (!LocallyAvailable ? " (DL)" : string.Empty)
                : string.IsNullOrEmpty(Mod?.FileName) ? string.Empty : " (Custom)");
    }
}

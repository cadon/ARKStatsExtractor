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

        /// <summary>
        /// True if the file is not in the official mods manifest, i.e. a file only available locally, usually custom created.
        /// If this is true, the mod info can be reloaded. This allows the user to edit the file and have the updated version available in the mod manager.
        /// </summary>
        public bool ManuallyLoaded;

        [OnDeserialized]
        private void SetVersion(StreamingContext _) => Version = Utils.TryParseVersionAlsoWithOnlyMajor(_version);

        public override string ToString() =>
            (Mod?.Title ?? "unknown mod")
            + (OnlineAvailable
                ? (!LocallyAvailable ? " (DL)" : string.Empty)
                : string.IsNullOrEmpty(Mod?.FileName) ? string.Empty : " (Custom)");
    }
}

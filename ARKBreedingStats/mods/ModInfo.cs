using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.mods
{
    /// <summary>
    /// Contains infos about a mod and its version
    /// </summary>
    [DataContract]
    public class ModInfo
    {
        [DataMember]
        public string version;
        [IgnoreDataMember]
        public Version Version;
        [DataMember]
        public Mod mod;
        /// <summary>
        /// Indicates if the according json-file is downloaded.
        /// </summary>
        [IgnoreDataMember]
        public bool downloaded;
        /// <summary>
        /// If true the modInfo is available online. If not it's probably customly created.
        /// </summary>
        [IgnoreDataMember]
        public bool onlineAvailable;
        [IgnoreDataMember]
        public bool currentlyInLibrary;

        [OnDeserialized]
        private void SetVersion(StreamingContext context)
        {
            Version.TryParse(version, out Version);
        }

        public override string ToString()
        {
            return (mod?.title ?? "unknown mod")
                + (!downloaded && onlineAvailable ? " (DL)" : "");
        }
    }
}

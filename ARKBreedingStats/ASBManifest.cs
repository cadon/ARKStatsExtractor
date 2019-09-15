using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    /// <summary>
    /// Contains infos about the versions of different parts of this application
    /// </summary>
    [DataContract]
    class ASBManifest
    {
        /// <summary>
        /// Must be present and a supported value.
        /// </summary>
        [DataMember]
        private string format;

        /// <summary>
        /// Dictionary of Versions.
        /// Expected is at least the key "version" in each entry.
        /// </summary>
        [DataMember]
        public Dictionary<string, Dictionary<string, string>> versions;
    }
}

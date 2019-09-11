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
        /// Dictionary of Versions. The key is the part of the application.
        /// The main application has the key "ASB"
        /// </summary>
        [DataMember]
        public Dictionary<string, Version> versions;
    }
}

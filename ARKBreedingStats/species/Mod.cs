using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Information about a mod which contains new species
    /// </summary>
    [DataContract]
    public class Mod
    {
        /// <summary>
        /// The id used by steam
        /// </summary>
        [DataMember]
        public string id;
        /// <summary>
        /// The tag used by ARK in the blueprints
        /// </summary>
        [DataMember]
        public string tag;
        /// <summary>
        /// Commonly used name to describe the mod
        /// </summary>
        [DataMember]
        public string title;
        /// <summary>
        /// Filename of the mod-values
        /// </summary>
        [IgnoreDataMember]
        public string FileName;

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public bool Equals(Mod other)
        {
            return !string.IsNullOrEmpty(id) && other.id == id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj is Mod speciesObj && Equals(speciesObj);
        }

        public override string ToString()
        {
            return title;
        }
    }
}

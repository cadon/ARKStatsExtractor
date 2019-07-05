using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Information about a mod which contains new species
    /// </summary>
    public class Mod
    {
        /// <summary>
        /// The id used by steam
        /// </summary>
        public string id;
        /// <summary>
        /// The tag used by ARK in the blueprints
        /// </summary>
        public string tag;
        /// <summary>
        /// Commonly used name to describe the mod
        /// </summary>
        public string title;
    }
}

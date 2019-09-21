using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ARKBreedingStats.species
{
    [DataContract]
    public class ColorRegion
    {
        [DataMember]
        public string name;
        [DataMember]
        public List<string> colors = new List<string>();
        [IgnoreDataMember]
        public List<ARKColor> naturalColors;

        public ColorRegion()
        {
            name = Loc.s("unknown");
        }

        /// <summary>
        /// Sets the ARKColor objects
        /// </summary>
        internal void Initialize(ARKColors arkColors)
        {
            naturalColors = new List<ARKColor>();
            if (colors == null) return;
            foreach (var c in colors)
            {
                ARKColor cl = arkColors.ByName(c);
                if (cl.hash != 0 && !naturalColors.Contains(cl))
                    naturalColors.Add(cl);
            }
        }
    }
}

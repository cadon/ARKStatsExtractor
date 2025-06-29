using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArkSmartBreeding.Models.Species
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ColorRegion
    {
        [JsonProperty]
        public string name;

        /// <summary>
        /// List of natural occurring color names.
        /// </summary>
        [JsonProperty]
        public List<string> colors;

        /// <summary>
        /// This region is not visible in game if true.
        /// </summary>
        [JsonProperty]
        public bool invisible;

        /// <summary>
        /// List of natural occurring ARKColors.
        /// </summary>
        public List<ArkColor> naturalColors;

        public ColorRegion(string missingColorName)
        {
            name = missingColorName;
        }

        /// <summary>
        /// Sets the ARKColor objects for the natural occurring colors.
        /// </summary>
        public void Initialize(ArkColors arkColors)
        {
            if (colors == null) return;
            naturalColors = new List<ArkColor>();
            foreach (var c in colors)
            {
                ArkColor cl = arkColors.ByName(c);
                if (cl.Id != 0 && !naturalColors.Contains(cl))
                    naturalColors.Add(cl);
            }
        }
    }
}

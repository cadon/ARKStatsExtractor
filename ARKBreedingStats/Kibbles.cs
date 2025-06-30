using ArkSmartBreeding.Models.Species;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ARKBreedingStats
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Kibbles
    {
        [JsonProperty]
        private string ver = "0.0";

        [JsonProperty]
        public Dictionary<string, Kibble> kibble = new Dictionary<string, Kibble>();

        public Version version = new Version(0, 0);
        private static Kibbles _K;
        public static Kibbles K
        {
            get => _K ?? (K = new Kibbles());
            set
            {
                _K = value;

                if (value is null)
                    return;

                value.version = new Version(value.ver);
            }
        }
    }

}

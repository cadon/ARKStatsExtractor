using Newtonsoft.Json;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Information about a mod which contains new species
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Mod
    {
        /// <summary>
        /// The id used by steam
        /// </summary>
        [JsonProperty]
        public string id;

        /// <summary>
        /// The tag used by ARK in the blueprints
        /// </summary>
        [JsonProperty]
        public string tag;

        /// <summary>
        /// Commonly used name to describe the mod
        /// </summary>
        [JsonProperty]
        public string title;

        /// <summary>
        /// Game expansions are usually maps. The species of these expansion are usually included in the vanilla game and thus these files are loaded automatically by this application.
        /// These mod files are not listed explicitely in the mod list of a collection, they're expected to be loaded always.
        /// Also these mods usually cannot contain mod colors and must be ignored in the color stacking of possible other mods.
        /// </summary>
        [JsonProperty]
        public bool expansion;

        /// <summary>
        /// Filename of the mod-values
        /// </summary>
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

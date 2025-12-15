using Newtonsoft.Json;

namespace ARKBreedingStats.mods
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
        [JsonProperty("id")]
        public string Id;

        /// <summary>
        /// The tag used by ARK in the blueprints
        /// </summary>
        [JsonProperty("tag")]
        public string Tag;

        /// <summary>
        /// Mod tag prefixed with game identifier (ASA or ASE).
        /// </summary>
        public string TagWithGamePrefix => (IsAsa ? Ark.Asa : Ark.Ase) + Tag;

        /// <summary>
        /// Commonly used name to describe the mod
        /// </summary>
        [JsonProperty("title")]
        public string Title;

        /// <summary>
        /// Commonly used short name to describe the mod, is preferred over title for species suffix if available.
        /// </summary>
        [JsonProperty("shortTitle")]
        public string ShortTitle;

        /// <summary>
        /// Game expansions are usually maps. The species of these expansion are usually included in the vanilla game and thus these files are loaded automatically by this application.
        /// These mod files are not listed explicitly in the mod list of a collection, they're expected to be loaded always.
        /// Also, these mods usually cannot contain mod colors and must be ignored in the color stacking of possible other mods.
        /// </summary>
        [JsonProperty("expansion")]
        public bool IsExpansion;

        [JsonProperty("author")]
        public string Author;

        [JsonProperty("official")]
        public bool IsOfficial;

        [JsonProperty("ASA")]
        public bool IsAsa;

        /// <summary>
        /// Curse forge mod page name (ASA mods).
        /// </summary>
        [JsonProperty("cfPage")]
        public string CfPage;

        /// <summary>
        /// Filename of the mod-values
        /// </summary>
        public string FileName;

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(Mod other)
        {
            return !string.IsNullOrEmpty(Id) && other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj is Mod speciesObj && Equals(speciesObj);
        }

        public override string ToString()
        {
            return Title;
        }

        #region Other Mod

        /// <summary>
        /// Name of an entry representing another mod, not available in this application. This entry may be needed to correctly determine the available colors.
        /// </summary>
        public const string OtherModName = "[other mod]";

        private static Mod _otherMod;

        /// <summary>
        /// Generic entry for not available mods. Can be important for correctly determining the available colors.
        /// </summary>
        public static Mod OtherMod
        {
            get
            {
                if (_otherMod == null)
                    _otherMod = new Mod { FileName = string.Empty, Id = Mod.OtherModName, Tag = Mod.OtherModName, Title = Mod.OtherModName };
                return _otherMod;
            }
        }

        #endregion
    }
}

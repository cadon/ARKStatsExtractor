using ARKBreedingStats.Models;
using Newtonsoft.Json;

namespace ARKBreedingStats.Mods
{
    /// <summary>
    /// Information about a mod which contains new species.
    /// Represents static mod metadata loaded from JSON.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Mod
    {
        /// <summary>
        /// The id used by steam
        /// </summary>
        [JsonProperty("id")]
        public string? Id { get; set; }

        /// <summary>
        /// The tag used by ARK in the blueprints
        /// </summary>
        [JsonProperty("tag")]
        public string? Tag { get; set; }

        /// <summary>
        /// Mod tag prefixed with game identifier (ASA or ASE).
        /// </summary>
        public string TagWithGamePrefix => (IsAsa ? GameConstants.Asa : GameConstants.Ase) + Tag;

        /// <summary>
        /// Commonly used name to describe the mod
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Commonly used short name to describe the mod, is preferred over title for species suffix if available.
        /// </summary>
        [JsonProperty("shortTitle")]
        public string? ShortTitle { get; set; }

        /// <summary>
        /// Game expansions are usually maps. The species of these expansion are usually included in the vanilla game and thus these files are loaded automatically by this application.
        /// These mod files are not listed explicitly in the mod list of a collection, they're expected to be loaded always.
        /// Also, these mods usually cannot contain mod colors and must be ignored in the color stacking of possible other mods.
        /// </summary>
        [JsonProperty("expansion")]
        public bool IsExpansion { get; set; }

        [JsonProperty("author")]
        public string? Author { get; set; }

        [JsonProperty("official")]
        public bool IsOfficial { get; set; }

        [JsonProperty("ASA")]
        public bool IsAsa { get; set; }

        /// <summary>
        /// Curse forge mod page name (ASA mods).
        /// </summary>
        [JsonProperty("cfPage")]
        public string? CfPage { get; set; }

        /// <summary>
        /// Filename of the mod-values
        /// </summary>
        public string? FileName { get; set; }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public bool Equals(Mod? other)
        {
            return other != null && !string.IsNullOrEmpty(Id) && other.Id == Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is Mod mod && Equals(mod);
        }

        public override string ToString()
        {
            return Title ?? string.Empty;
        }

        #region Other Mod

        /// <summary>
        /// Name of an entry representing another mod, not available in this application. This entry may be needed to correctly determine the available colors.
        /// </summary>
        public const string OtherModName = "[other mod]";

        private static Mod? _otherMod;

        /// <summary>
        /// Generic entry for not available mods. Can be important for correctly determining the available colors.
        /// </summary>
        public static Mod OtherMod
        {
            get
            {
                if (_otherMod == null)
                {
                    _otherMod = new Mod { FileName = string.Empty, Id = OtherModName, Tag = OtherModName, Title = OtherModName };
                }

                return _otherMod;
            }
        }

        #endregion
    }
}

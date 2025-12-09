using ARKBreedingStats.Library;
using System.Collections.Generic;
using ARKBreedingStats.species;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Properties to uniquely identify a creature image.
    /// </summary>
    internal struct CreatureImageParameters
    {
        public Species Species;
        public string SpeciesName;
        /// <summary>
        /// Mod id or game name (ASA or ASE).
        /// </summary>
        public string ModName;
        public string CreatureSex;
        public string Pattern;
        public string Pose;
        /// <summary>
        /// Parameters combined. The file names are based on this.
        /// </summary>
        public string BaseParameters;

        public CreatureImageParameters(Species species, string speciesName, string game, Sex sex, int patternId, int pose)
        {
            Species = species;
            SpeciesName = speciesName;
            ModName = species.Mod?.Id;
            if (string.IsNullOrEmpty(ModName))
                ModName = game;
            if (!string.IsNullOrEmpty(ModName))
                ModName = "_" + ModName;
            CreatureSex = sex == Sex.Female ? "_sf" : sex == Sex.Male ? "_sm" : string.Empty;
            Pattern = patternId >= 0 ? "_p" + patternId : string.Empty;
            Pose = pose >= 0 ? "_v" + pose : string.Empty; // v like variant
            // the file names are based on this.
            // it needs to have the following pattern <species>[_<game|modId>][_s(m|f)][_<pattern>]
            BaseParameters = $"{SpeciesName}{ModName}{CreatureSex}{Pattern}{Pose}";
        }

        /// <summary>
        /// Possible file names for this creature parameters without extension.
        /// </summary>
        public List<string> GetPossibleSpeciesImageNames(string speciesName) => new List<string>
        {
            speciesName + ModName + CreatureSex + Pattern + Pose,
            speciesName + ModName + Pattern + Pose,
            speciesName + CreatureSex + Pattern + Pose,
            speciesName + Pattern + Pose,
            speciesName + ModName + CreatureSex + Pattern,
            speciesName + ModName + Pattern,
            speciesName + CreatureSex + Pattern,
            speciesName + Pattern
        };

    }
}

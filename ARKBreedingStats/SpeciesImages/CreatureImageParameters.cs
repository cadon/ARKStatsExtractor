using System;
using ARKBreedingStats.Library;
using ARKBreedingStats.ocr.PatternMatching;
using ARKBreedingStats.species;
using System.Collections.Generic;
using System.Linq;
using static ARKBreedingStats.Ark;

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
        public int PoseId;
        public string Pose;
        /// <summary>
        /// Parameters combined. The file names are based on this.
        /// </summary>
        public string BaseParameters;

        public CreatureImageParameters(Species species, string game, Sex sex, int patternId, int pose)
        {
            Species = species;
            SpeciesName = species.name;
            ModName = species.Mod?.Id;
            if (string.IsNullOrEmpty(ModName))
                ModName = game;
            if (!string.IsNullOrEmpty(ModName))
                ModName = "_" + ModName;
            CreatureSex = sex == Sex.Female ? "_sf" : sex == Sex.Male ? "_sm" : string.Empty;
            Pattern = patternId >= 0 ? "_p" + patternId : string.Empty;
            PoseId = pose;
            Pose = pose > 0 ? "_v" + pose : string.Empty; // v like variant
            // the file names are based on this.
            // it needs to have the following pattern <species>[_<game|modId>][_s(m|f)][_<pattern>]
            BaseParameters = $"{SpeciesName}{ModName}{CreatureSex}{Pattern}{Pose}";
        }

        /// <summary>
        /// Creates image parameters using base parameters with a specific pose.
        /// </summary>
        public CreatureImageParameters(CreatureImageParameters baseParameters, int poseId)
        {
            Species = baseParameters.Species;
            SpeciesName = baseParameters.SpeciesName;
            ModName = baseParameters.ModName;
            CreatureSex = baseParameters.CreatureSex;
            Pattern = baseParameters.Pattern;
            PoseId = poseId;
            Pose = poseId > 0 ? "_v" + poseId : string.Empty;
            BaseParameters = $"{SpeciesName}{ModName}{CreatureSex}{Pattern}{Pose}";
        }

        /// <summary>
        /// Possible file names for this creature parameters without extension.
        /// </summary>
        public List<string> GetPossibleSpeciesImageNames(string speciesName = null, bool onlyWithPose = false)
        {
            var speciesNameUsed = speciesName ?? SpeciesName;
            if (onlyWithPose)
                return new List<string>
                {
                    speciesNameUsed + ModName + CreatureSex + Pattern + Pose,
                    speciesNameUsed + ModName + Pattern + Pose,
                    speciesNameUsed + CreatureSex + Pattern + Pose,
                    speciesNameUsed + Pattern + Pose
                };

            return new List<string>
            {
                speciesNameUsed + ModName + CreatureSex + Pattern + Pose,
                speciesNameUsed + ModName + Pattern + Pose,
                speciesNameUsed + CreatureSex + Pattern + Pose,
                speciesNameUsed + Pattern + Pose,
                speciesNameUsed + ModName + CreatureSex + Pattern,
                speciesNameUsed + ModName + Pattern,
                speciesNameUsed + CreatureSex + Pattern,
                speciesNameUsed + Pattern
            };
        }

        /// <summary>
        /// Possible file names for this creature parameters without extension also including fallbacks for some variants.
        /// </summary>
        public string[] GetPossibleSpeciesImageNamesWithFallbacks(bool onlyWithPose = false)
        {
            var possibleFileNames = GetPossibleSpeciesImageNames(onlyWithPose: onlyWithPose);
            // fallback for some variant species to use the vanilla one if no aberrant image is available (they're pretty similar)
            if (SpeciesName.StartsWith("Aberrant "))
                possibleFileNames.AddRange(GetPossibleSpeciesImageNames(SpeciesName.Replace("Aberrant ", string.Empty), onlyWithPose));
            if (SpeciesName.Contains("Brute "))
                possibleFileNames.AddRange(GetPossibleSpeciesImageNames(SpeciesName.Replace("Brute ", string.Empty), onlyWithPose));
            if (SpeciesName.Contains("Polar "))
                possibleFileNames.AddRange(GetPossibleSpeciesImageNames(SpeciesName.Replace("Polar Bear", "Dire Bear").Replace("Polar ", string.Empty), onlyWithPose));

            return possibleFileNames.Distinct().ToArray();
        }
    }
}

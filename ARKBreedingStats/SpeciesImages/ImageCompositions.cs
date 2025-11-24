using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.SpeciesImages
{
    internal static class ImageCompositions
    {
        /// <summary>
        /// Key is simple species name or blueprint path.
        /// </summary>
        private static Dictionary<string, ImageComposition> _speciesImageCompositions;

        public static ImageComposition GetComposition(Species species)
            => _speciesImageCompositions == null ? null
                : _speciesImageCompositions.TryGetValue(species.blueprintPath, out var comp) ? comp
                : _speciesImageCompositions.TryGetValue(species.name, out comp) ? comp
                : null;

        public static void LoadCompositions()
        {
            var filePath = FileService.GetJsonPath("imageCompositions.json");
            if (!File.Exists(filePath)) return;
            if (!FileService.LoadJsonFile(filePath, out Dictionary<string, ImageComposition> comps,
                    out var errorMessage))
            {
                _speciesImageCompositions = null;
                MessageBoxes.ShowMessageBox(errorMessage, "Error when loading species image compositions.");
                return;
            }

            _speciesImageCompositions = comps?.Any() == true ? comps : null;
        }
    }
}

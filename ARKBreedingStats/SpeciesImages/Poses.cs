using System.Collections.Generic;
using System.Linq;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Pose id of species images.
    /// </summary>
    internal static class Poses
    {
        /// <summary>
        /// Key is blueprint path of species, value is pose number.
        /// </summary>
        internal static Dictionary<string, int> SelectedPoses;

        private const string Filename = "selectedPoses.json";

        internal static void LoadPoses()
        {
            if (FileService.LoadJsonFileIfAvailable(FileService.GetJsonPath(Filename),
                    out Dictionary<string, int> ps, out var errorMessage)
                && ps.Any())
                SelectedPoses = ps;
            if (!string.IsNullOrEmpty(errorMessage))
                MessageBoxes.ShowMessageBox("Error when loading species image poses.\n\n" + errorMessage);
        }

        internal static void SavePoses()
        {
            var filePath = FileService.GetJsonPath(Filename);
            var keys = SelectedPoses?.Keys.ToArray();
            if (keys?.Any() != true)
            {
                FileService.TryDeleteFile(filePath);
                return;
            }

            foreach (var bp in keys)
            {
                if (SelectedPoses.TryGetValue(bp, out var p) && p == 0)
                    SelectedPoses.Remove(bp);
            }

            if (SelectedPoses?.Any() != true)
            {
                FileService.TryDeleteFile(filePath);
                return;
            }

            if (!FileService.SaveJsonFile(filePath, SelectedPoses, out var errorMessage))
                MessageBoxes.ShowMessageBox("Error when saving species image poses.\n\n" + errorMessage);
        }

        internal static void SetPose(Species species, int pose)
        {
            if (SelectedPoses == null) SelectedPoses = new Dictionary<string, int>();
            SelectedPoses[species.blueprintPath] = pose;
        }

        internal static int GetPose(Species species) =>
            SelectedPoses?.TryGetValue(species.blueprintPath, out var p) == true ? p : 0;
    }
}

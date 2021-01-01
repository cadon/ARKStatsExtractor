using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ARKBreedingStats.species;

namespace ARKBreedingStats.mods
{
    /// <summary>
    /// Handling unknown species that are from mods not loaded.
    /// </summary>
    public static class HandleUnknownMods
    {
        /// <summary>
        /// Check if mod files for the missing species are available.
        /// </summary>
        /// <param name="unknownSpeciesBlueprints"></param>
        public static (List<string> locallyAvailableModFiles, List<string> onlineAvailableModFiles, List<string> unavailableModFiles, List<string> alreadyLoadedModFilesWithoutNeededClass)
            CheckForMissingModFiles(List<string> unknownSpeciesBlueprints, List<Mod> loadedMods)
        {
            List<string> unknownModTags = unknownSpeciesBlueprints.Select(bp => Regex.Replace(bp, @"^\/Game\/Mods\/([^\/]+)\/.*", "$1"))
                                                     .Where(bp => !string.IsNullOrEmpty(bp))
                                                     .Distinct()
                                                     .ToList();
            if (!unknownModTags.Any())
                return (null, null, null, null);

            // check if the needed mod-values can be downloaded automatically.
            List<string> locallyAvailableModFiles = new List<string>();
            List<string> onlineAvailableModFiles = new List<string>();
            List<string> unavailableModFiles = new List<string>();
            List<string> alreadyLoadedModFilesWithoutNeededClass = new List<string>();

            foreach (var modTag in unknownModTags)
            {
                if (Values.V.modsManifest.modsByTag.ContainsKey(modTag))
                {
                    if (loadedMods.Contains(Values.V.modsManifest.modsByTag[modTag].mod))
                        alreadyLoadedModFilesWithoutNeededClass.Add(modTag);
                    else if (Values.V.modsManifest.modsByTag[modTag].LocallyAvailable)
                        locallyAvailableModFiles.Add(modTag);
                    else if (Values.V.modsManifest.modsByTag[modTag].OnlineAvailable)
                        onlineAvailableModFiles.Add(modTag);
                    else
                        unavailableModFiles.Add(modTag);
                }
                else
                    unavailableModFiles.Add(modTag);
            }

            return (locallyAvailableModFiles, onlineAvailableModFiles, unavailableModFiles, alreadyLoadedModFilesWithoutNeededClass);
        }

        /// <summary>
        /// Adds the mods found by CheckForMissingModFiles to the collection file.
        /// </summary>
        /// <param name="creatureCollection"></param>
        /// <param name="modTags">List of the mod tags. Each entry must be loaded.</param>
        public static void AddModsToCollection(CreatureCollection creatureCollection, List<string> modTags)
        {
            if (creatureCollection.modIDs == null) creatureCollection.modIDs = new List<string>();
            creatureCollection.modIDs.AddRange(modTags.Select(mt => Values.V.modsManifest.modsByTag[mt].mod.id));
            creatureCollection.modListHash = 0; // indicates a reload of the mod-values is needed
        }
    }
}

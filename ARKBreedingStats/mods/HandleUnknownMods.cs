using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ARKBreedingStats.mods
{
    /// <summary>
    /// Handling unknown species that are from mods not loaded.
    /// </summary>
    public static class HandleUnknownMods
    {
        /// <summary>
        /// Matches blueprint path of an ASE species, group 1 contains the mod tag.
        /// </summary>
        private static readonly Regex regexBpAse = new Regex(@"^\/Game\/Mods\/([^\/]+)\/.*");

        /// <summary>
        /// Matches blueprint path of an ASA species, group 1 contains the mod tag.
        /// </summary>
        private static readonly Regex regexBpAsa = new Regex(@"^\/(?!Game)([^\/]+)\/.*");

        /// <summary>
        /// Check if mod files for the missing species are available.
        /// </summary>
        public static (List<string> locallyAvailableModFiles, List<string> onlineAvailableModFiles, List<string> unavailableModFiles, List<string> alreadyLoadedModFilesWithoutNeededClass)
            CheckForMissingModFiles(List<string> unknownSpeciesBlueprints, List<Mod> loadedMods, string game)
        {
            var regexModTag = game == Ark.Asa ? regexBpAsa : regexBpAse;

            var unknownModTags = unknownSpeciesBlueprints.Select(bp => regexModTag.Match(bp).Groups[1].Value)
                .Where(modTag => !string.IsNullOrEmpty(modTag))
                .Distinct()
                .ToArray();

            if (!unknownModTags.Any())
                return (null, null, null, null);

            // check if the needed mod-values can be downloaded automatically.
            var locallyAvailableModFiles = new List<string>();
            var onlineAvailableModFiles = new List<string>();
            var unavailableModFiles = new List<string>();
            var alreadyLoadedModFilesWithoutNeededClass = new List<string>();

            foreach (var modTag in unknownModTags)
            {
                if (Values.V.modsManifest.ModsByTag.TryGetValue(game + modTag, out var modInfo))
                {
                    if (loadedMods.Contains(modInfo.Mod))
                        alreadyLoadedModFilesWithoutNeededClass.Add(modTag);
                    else if (modInfo.LocallyAvailable)
                        locallyAvailableModFiles.Add(modTag);
                    else if (modInfo.OnlineAvailable)
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
            creatureCollection.modIDs.AddRange(modTags
                .Select(mt => Values.V.modsManifest.ModsByTag.TryGetValue(creatureCollection.Game + mt, out var modInfo) ? modInfo.Mod.Id : null)
                .Where(id => !string.IsNullOrEmpty(id))
            );
            creatureCollection.modListHash = 0; // indicates a reload of the mod-values is needed
        }
    }
}

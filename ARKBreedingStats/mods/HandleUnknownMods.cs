using ARKBreedingStats.Library;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static void CheckForMissingModFiles(CreatureCollection creatureCollection, List<string> unknownSpeciesBlueprints)
        {
            List<string> unknownModTags = unknownSpeciesBlueprints.Select(bp => Regex.Replace(bp, @"^\/Game\/Mods\/([^\/]+)\/.*", "$1"))
                                                     .Where(bp => !string.IsNullOrEmpty(bp))
                                                     .Distinct()
                                                     .ToList();
            if (!unknownModTags.Any())
                return;

            // check if the needed mod-values can be downloaded automatically.
            List<string> locallyAvailableModFiles = new List<string>();
            List<string> onlineAvailableModFiles = new List<string>();
            List<string> unavailableModFiles = new List<string>();

            foreach (var modTag in unknownModTags)
            {
                if (Values.V.modsManifest.modsByTag.ContainsKey(modTag))
                {
                    if (Values.V.modsManifest.modsByTag[modTag].downloaded)
                        locallyAvailableModFiles.Add(modTag);
                    else
                        onlineAvailableModFiles.Add(modTag);
                }
                else
                    unavailableModFiles.Add(modTag);
            }

            MessageBox.Show("Some of the creatures to be imported have an unknown species, most likely because a mod is used.\n"
                + "To import these creatures, this application needs additional informations about these mods."
                + (locallyAvailableModFiles.Any() ?
                    "\n\nThe value files for the following mods are already locally available and just need to be added to the library:\n"
                    + string.Join("\n", locallyAvailableModFiles)
                    : "")
                + (onlineAvailableModFiles.Any() ?
                    "\n\nThe value files for the following mods can be downloaded automatically if you want:\n"
                    + string.Join("\n", onlineAvailableModFiles)
                    : "")
                + (unavailableModFiles.Any() ?
                    "\n\nThe value files for the following mods are unknown. You probably manually need to create a mod-file to import the creatures depending on it.\n"
                    + string.Join("\n", unavailableModFiles)
                    : ""),
                "Unknown species", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if ((onlineAvailableModFiles.Any() || locallyAvailableModFiles.Any())
                && MessageBox.Show("Do you want to " + (onlineAvailableModFiles.Any() ? "download and " : "") + "add the values-files for the following mods to the library?\n\n"
                                   + string.Join("\n", onlineAvailableModFiles) + "\n"
                                   + string.Join("\n", locallyAvailableModFiles),
                                   "Add value-files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    ) == DialogResult.Yes)
            {
                if (creatureCollection.modIDs == null) creatureCollection.modIDs = new List<string>();
                creatureCollection.modIDs.AddRange(onlineAvailableModFiles.Select(mt => Values.V.modsManifest.modsByTag[mt].mod.id));
                creatureCollection.modIDs.AddRange(locallyAvailableModFiles.Select(mt => Values.V.modsManifest.modsByTag[mt].mod.id));
                creatureCollection.modListHash = 0; // indicates a reload of the mod-values is needed
            }
        }
    }
}

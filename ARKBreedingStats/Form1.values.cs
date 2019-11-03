using ARKBreedingStats.Library;
using ARKBreedingStats.mods;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    /// <summary>
    /// Methods for handling values files. Mainly contains methods that display MessageBoxes.
    /// </summary>

    public partial class Form1
    {
        private bool LoadModValueFiles(List<string> modValueFileNames, bool showResult, bool applySettings, out List<Mod> mods)
        {
            if (modValueFileNames == null) throw new ArgumentNullException();

            // first ensure that all mod-files are available
            CheckAvailabilityAndUpdateModFiles(modValueFileNames, Values.V);

            if (Values.V.LoadModValues(modValueFileNames, throwExceptionOnFail: true, out mods, out string resultsMessage))
            {
                if (!string.IsNullOrEmpty(resultsMessage))
                    MessageBox.Show(resultsMessage, "Mod Values loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (applySettings)
                    ApplySettingsToValues();
                speciesSelector1.SetSpeciesLists(Values.V.species, Values.V.aliases);
                UpdateStatusBar();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if mod files for the missing species are available.
        /// </summary>
        /// <param name="unknownSpeciesBlueprints"></param>
        public static void CheckForMissingModFiles(CreatureCollection creatureCollection, List<string> unknownSpeciesBlueprints)
        {
            var (locallyAvailableModFiles, onlineAvailableModFiles, unavailableModFiles) = mods.HandleUnknownMods.CheckForMissingModFiles(unknownSpeciesBlueprints);

            bool locallyAvailableModsExist = locallyAvailableModFiles != null && locallyAvailableModFiles.Any();
            bool onlineAvailableModsExist = onlineAvailableModFiles != null && onlineAvailableModFiles.Any();
            bool unavailableModsExist = unavailableModFiles != null && unavailableModFiles.Any();

            MessageBox.Show("Some of the creatures to be imported have an unknown species, most likely because a mod is used.\n"
                + "To import these creatures, this application needs additional informations about these mods."
                + (locallyAvailableModsExist ?
                    "\n\nThe value files for the following mods are already locally available and just need to be added to the library:\n"
                    + string.Join("\n", locallyAvailableModFiles)
                    : "")
                + (onlineAvailableModsExist ?
                    "\n\nThe value files for the following mods can be downloaded automatically if you want:\n"
                    + string.Join("\n", onlineAvailableModFiles)
                    : "")
                + (unavailableModsExist ?
                    "\n\nThe value files for the following mods are unknown. You probably manually need to create a mod-file to import the creatures depending on it.\n"
                    + string.Join("\n", unavailableModFiles)
                    : ""),
                "Unknown species", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if ((locallyAvailableModsExist || onlineAvailableModsExist)
                && MessageBox.Show("Do you want to " + (onlineAvailableModsExist ? "download and " : "") + "add the values-files for the following mods to the library?\n\n"
                                   + string.Join("\n", onlineAvailableModFiles) + "\n"
                                   + string.Join("\n", locallyAvailableModFiles),
                                   "Add value-files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    ) == DialogResult.Yes)
            {
                List<string> modTagsToAdd = new List<string>();
                if (locallyAvailableModsExist) modTagsToAdd.AddRange(locallyAvailableModFiles);
                if (onlineAvailableModsExist) modTagsToAdd.AddRange(onlineAvailableModFiles);
                mods.HandleUnknownMods.AddModsToCollection(creatureCollection, modTagsToAdd);
            }
        }

        /// <summary>
        /// Returns true if files were downloaded.
        /// </summary>
        /// <param name="modValueFileNames"></param>
        /// <returns></returns>
        private static bool CheckAvailabilityAndUpdateModFiles(List<string> modValueFileNames, Values values)
        {
            var (missingModValueFilesOnlineAvailable, missingModValueFilesOnlineNotAvailable, modValueFilesWithAvailableUpdate) = values.CheckAvailabilityAndUpdateModFiles(modValueFileNames);

            bool filesDownloaded = false;

            if (modValueFilesWithAvailableUpdate.Count > 0
                && MessageBox.Show("For " + modValueFilesWithAvailableUpdate.Count.ToString() + " value files there is an update available. It is strongly recommended to use the updated versions.\n"
                + "The updated files can be downloaded automatically if you want.\n\n"
                + "The following files can be downloaded\n"
                + string.Join(", ", modValueFilesWithAvailableUpdate)
                + "\n\nDo you want to download these files?",
                "Updates for value files available", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                filesDownloaded |= values.modsManifest.DownloadModFiles(modValueFilesWithAvailableUpdate);
            }

            if (missingModValueFilesOnlineAvailable.Count > 0
                && MessageBox.Show(missingModValueFilesOnlineAvailable.Count.ToString() + " mod-value files are not available locally. Without these files the library will not display all creatures.\n"
                + "The missing files can be downloaded automatically if you want.\n\n"
                + "The following files can be downloaded\n"
                + string.Join(", ", missingModValueFilesOnlineAvailable)
                + "\n\nDo you want to download these files?",
                "Missing value files", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                filesDownloaded |= values.modsManifest.DownloadModFiles(missingModValueFilesOnlineAvailable);
            }

            if (missingModValueFilesOnlineNotAvailable.Count > 0)
            {
                MessageBox.Show(missingModValueFilesOnlineNotAvailable.Count.ToString() + " mod-value files are neither available locally nor online. The creatures of the missing mod will not be displayed.\n"
                + "The following files are missing\n"
                + string.Join(", ", missingModValueFilesOnlineNotAvailable),
                "Missing value files", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return filesDownloaded;
        }

        private async static Task<bool> LoadModsManifestAsync(Values values, bool forceUpdate = false)
        {
            bool success = false;
            ModsManifest modsManifest = null;
            try
            {
                modsManifest = await ModsManifest.TryLoadModManifestFile(forceUpdate);
                success = true;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Mods manifest file {Path.Combine(FileService.ValuesFolder, FileService.ModsManifest)} not found " +
                    "and downloading it failed. You can try it later or try to update your application.",
                    "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                FormatExceptionMessageBox(Path.Combine(FileService.ValuesFolder, FileService.ModsManifest));
            }

            values.SetModsManifest(modsManifest);
            return success;
        }

        private static void LoadServerMultiplierPresets(Values values)
        {
            if (!ServerMultipliersPresets.TryLoadServerMultipliersPresets(out values.serverMultipliersPresets))
            {
                MessageBox.Show("The file with the server multipliers couldn't be loaded. Changed settings, e.g. for the singleplayer will be not available.\nIt's recommended to download the application again.",
                    "Server multiplier file not loaded.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads the default stat values. Returns true if successful.
        /// </summary>
        /// <returns></returns>
        private bool LoadStatValues(Values values)
        {
            bool success = false;

            try
            {
                values = values.LoadValues();

                if (values.modsManifest == null)
                    _ = Task.Run(async () => await LoadModsManifestAsync(values));
                if (values.serverMultipliersPresets == null)
                    LoadServerMultiplierPresets(values);

                success = true;
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"Values-File {FileService.ValuesJson} not found. " +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
            }
            catch (FormatException)
            {
                FormatExceptionMessageBox(FileService.ValuesJson);
            }
            catch (SerializationException e)
            {
                DeserializeExceptionMessageBox(FileService.ValuesJson, e.Message);
            }

            return success;
        }

        public static void FormatExceptionMessageBox(string filePath)
        {
            MessageBox.Show($"File {filePath} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void DeserializeExceptionMessageBox(string filePath, string eMessage)
        {
            MessageBox.Show($"File {filePath} couldn't be deserialized.\nErrormessage:\n\n" + eMessage,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

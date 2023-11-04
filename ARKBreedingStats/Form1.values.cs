using ARKBreedingStats.Library;
using ARKBreedingStats.mods;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    /// <summary>
    /// Methods for handling values files. Mainly contains methods that display MessageBoxes.
    /// </summary>
    public partial class Form1
    {
        /// <summary>
        /// Loads the mod value files for the creatureCollection. If a file is not available locally, it's tried to download it.
        /// </summary>
        /// <param name="modFilesToLoad"></param>
        /// <param name="showResult"></param>
        /// <param name="applySettings"></param>
        /// <param name="mods"></param>
        /// <returns></returns>
        private bool LoadModValueFiles(List<string> modFilesToLoad, bool showResult, bool applySettings, out List<Mod> mods)
        {
            if (modFilesToLoad == null) throw new ArgumentNullException();

            // first ensure that all mod-files are available
            CheckAvailabilityAndUpdateModFiles(modFilesToLoad, Values.V);

            bool modFilesLoaded = Values.V.LoadModValues(modFilesToLoad, true, out mods, out string resultsMessage);

            if (modFilesLoaded)
            {
                speciesSelector1.SetSpeciesLists(Values.V.species, Values.V.aliases);
                if (applySettings)
                    ApplySettingsToValues();
            }
            if (showResult && !string.IsNullOrEmpty(resultsMessage))
                MessageBox.Show(resultsMessage, "Loading Mod Values", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _creatureCollection.ModList = mods;
            UpdateStatusBar();
            return modFilesLoaded;
        }

        /// <summary>
        /// Check if mod files for the missing species are available.
        /// </summary>
        private static void CheckForMissingModFiles(CreatureCollection creatureCollection, List<string> unknownSpeciesBlueprints, string exportFilePath = null, string creatureName = null)
        {
            var (locallyAvailableModFiles, onlineAvailableModFiles, unavailableModFiles, alreadyLoadedModFilesWithoutNeededClass) = HandleUnknownMods.CheckForMissingModFiles(unknownSpeciesBlueprints, creatureCollection.ModList);

            bool locallyAvailableModsExist = locallyAvailableModFiles != null && locallyAvailableModFiles.Any();
            bool onlineAvailableModsExist = onlineAvailableModFiles != null && onlineAvailableModFiles.Any();
            bool unavailableModsExist = unavailableModFiles != null && unavailableModFiles.Any();
            bool alreadyLoadedModFilesWithoutNeededClassExist = alreadyLoadedModFilesWithoutNeededClass != null && alreadyLoadedModFilesWithoutNeededClass.Any();

            MessageBoxes.ShowMessageBox("Some of the creatures to be imported have an unknown species, most likely because a mod is used.\n"
                + "To import these creatures, this application needs additional information about these mods."
                + (locallyAvailableModsExist ?
                    "\n\nThe value files for the following mods are already locally available and just need to be added to the library:\n\n- "
                    + string.Join("\n- ", locallyAvailableModFiles)
                    : string.Empty)
                + (onlineAvailableModsExist ?
                    "\n\nThe value files for the following mods can be downloaded automatically if you want:\n\n- "
                    + string.Join("\n- ", onlineAvailableModFiles)
                    : string.Empty)
                + (unavailableModsExist ?
                    "\n\nThe values for species for the following mods are unknown.\nYou can create a mod values file manually if you have all the needed stat values, see the manual for more info.\nYou can also check on the discord server of ASB the #mod-requests channel and ask for that mod, maybe we'll add support for it in the future.\n\n- "
                    + string.Join("\n- ", unavailableModFiles)
                    : string.Empty)
                + (alreadyLoadedModFilesWithoutNeededClassExist ?
                    "\n\nThe values for species for the following mods are unknown even though an according mod file was already loaded, i.e. the species blueprint path is not in the mod file. If it is a manual mod file, make sure the blueprint path is correct.\n\n- "
                    + string.Join("\n- ", alreadyLoadedModFilesWithoutNeededClass) + "\n\nThe following blueprint paths were not found in the mod file:\n\n"
                    + string.Join("\n", unknownSpeciesBlueprints.Where(bp => alreadyLoadedModFilesWithoutNeededClass.Any(m => bp.StartsWith($"/Game/Mods/{m}/"))))
                    : string.Empty)
                + (string.IsNullOrEmpty(exportFilePath) ? null : $"\n\nThe according export file is located at\n{exportFilePath}")
                + (string.IsNullOrEmpty(creatureName) ? null : $"\n\nThe creature is named\n{creatureName}")
                ,
                "Unknown species", MessageBoxIcon.Information);

            if ((locallyAvailableModsExist || onlineAvailableModsExist)
                && MessageBox.Show("Do you want to " + (onlineAvailableModsExist ? "download and " : string.Empty) + "add the values-files for the following mods to the library?\n\n- "
                                   + string.Join("\n- ", onlineAvailableModFiles) + (locallyAvailableModsExist && onlineAvailableModsExist ? "\n\n- " : string.Empty)
                                   + string.Join("\n- ", locallyAvailableModFiles),
                                   "Add value-files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    ) == DialogResult.Yes)
            {
                List<string> modTagsToAdd = new List<string>();
                if (locallyAvailableModsExist) modTagsToAdd.AddRange(locallyAvailableModFiles);
                if (onlineAvailableModsExist) modTagsToAdd.AddRange(onlineAvailableModFiles);
                HandleUnknownMods.AddModsToCollection(creatureCollection, modTagsToAdd);
            }
        }

        /// <summary>
        /// Returns true if files were downloaded.
        /// </summary>
        private static bool CheckAvailabilityAndUpdateModFiles(List<string> modFilesToCheck, Values values)
        {
            var (missingModValueFilesOnlineAvailable, missingModValueFilesOnlineNotAvailable, modValueFilesWithAvailableUpdate) = values.CheckAvailabilityAndUpdateModFiles(modFilesToCheck);

            bool filesDownloaded = false;

            if (modValueFilesWithAvailableUpdate.Any()
                && MessageBox.Show("For " + modValueFilesWithAvailableUpdate.Count + " value files there is an update available. It is strongly recommended to use the updated versions.\n"
                + "The updated files can be downloaded automatically if you want.\n"
                + "The following files can be downloaded\n\n"
                + string.Join("\n", modValueFilesWithAvailableUpdate)
                + "\n\nDo you want to download these files?",
                "Updates for value files available", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                filesDownloaded |= values.modsManifest.DownloadModFiles(modValueFilesWithAvailableUpdate);
            }

            if (missingModValueFilesOnlineAvailable.Any()
                && MessageBox.Show(missingModValueFilesOnlineAvailable.Count + " mod-value files are not available locally. Without these files the library will not display all creatures.\n"
                + "The missing files can be downloaded automatically if you want.\n"
                + "The following files can be downloaded\n\n"
                + string.Join("\n", missingModValueFilesOnlineAvailable)
                + "\n\nDo you want to download these files?",
                "Missing value files", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                filesDownloaded |= values.modsManifest.DownloadModFiles(missingModValueFilesOnlineAvailable);
            }

            if (missingModValueFilesOnlineNotAvailable.Any())
            {
                MessageBoxes.ShowMessageBox(missingModValueFilesOnlineNotAvailable.Count + " mod-value files are neither available locally nor online. The creatures of the missing mod will not be displayed.\n"
                                             + "The following files are missing\n\n"
                                             + string.Join("\n", missingModValueFilesOnlineNotAvailable), "Missing value files");
            }

            return filesDownloaded;
        }

        private static async Task<bool> LoadModsManifestAsync(Values values, bool forceUpdate = false)
        {
            ModsManifest modsManifest = null;

            try
            {
                try
                {
                    modsManifest = await ModsManifest.TryLoadModManifestFile(forceUpdate);
                    // assume all officially supported mods are online available
                    foreach (var m in modsManifest.modsByFiles) m.Value.OnlineAvailable = true;
                }
                catch (FileNotFoundException ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex, $"Mods manifest file {Path.Combine(FileService.ValuesFolder, FileService.ManifestFileName)} not found " +
                                                        "and downloading it failed. You can try it later or try to update your application.");
                    return false;
                }
                catch (FormatException)
                {
                    FormatExceptionMessageBox(Path.Combine(FileService.ValuesFolder, FileService.ManifestFileName));
                    return false;
                }

                // load custom manifest file for manually created mod value files
                if (ModsManifest.TryLoadCustomModManifestFile(out var customModsManifest))
                {
                    modsManifest = ModsManifest.MergeModsManifest(modsManifest, customModsManifest);
                }
            }
            catch (SerializationException serEx)
            {
                MessageBoxes.ExceptionMessageBox(serEx, "Serialization exception while trying to load the mods-manifest file.");
            }

            modsManifest?.Initialize();
            values.SetModsManifest(modsManifest);
            return true;
        }

        /// <summary>
        /// Loads the default stat values. Returns true if successful.
        /// </summary>
        /// <returns></returns>
        private bool LoadStatValues(Values values, bool forceReload)
        {
            bool success = false;

            try
            {
                if (values.modsManifest == null)
                    LoadModsManifestAsync(values).Wait();

                values.LoadValues(forceReload, out var errorMessage, out var errorMessageTitle);
                if (!string.IsNullOrEmpty(errorMessage))
                    MessageBoxes.ShowMessageBox(errorMessage, errorMessageTitle);

                success = true;
            }
            catch (DirectoryNotFoundException)
            {
                if (MessageBox.Show($"One of the following folders where the values-file is expected was not found.\n{FileService.GetJsonPath(FileService.ValuesFolder, FileService.ValuesJson)}\n\n" +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.Updater.ReleasesUrl);
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"Values-File {FileService.ValuesJson} not found. " +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.Updater.ReleasesUrl);
            }
            catch (FormatException ex)
            {
                FormatExceptionMessageBox(FileService.ValuesJson, ex.Message);
                if ((DateTime.Now - Properties.Settings.Default.lastUpdateCheck).TotalMinutes < 10)
                    CheckForUpdates();
            }
            catch (SerializationException ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, $"File {FileService.ValuesJson} couldn't be deserialized.");
            }

            return success;
        }

        private static void FormatExceptionMessageBox(string filePath, string customMessage = null) =>
            MessageBoxes.ShowMessageBox($"The file {filePath} is in a format that is not supported in this version of ARK Smart Breeding." + (string.IsNullOrEmpty(customMessage) ? string.Empty : $"\n{customMessage}")
                                         + "\n\nTry updating to a newer version of this application, either by using the updater or downloading it again.");
    }
}

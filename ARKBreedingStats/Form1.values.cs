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
        /// <summary>
        /// Loads the mod value files for the creatureCollection. If a file is not available locally, it's tried to download it.
        /// </summary>
        /// <param name="modValueFileNames"></param>
        /// <param name="showResult"></param>
        /// <param name="applySettings"></param>
        /// <param name="mods"></param>
        /// <returns></returns>
        private bool LoadModValueFiles(List<string> modValueFileNames, bool showResult, bool applySettings, out List<Mod> mods)
        {
            if (modValueFileNames == null) throw new ArgumentNullException();

            // first ensure that all mod-files are available
            CheckAvailabilityAndUpdateModFiles(modValueFileNames, Values.V);

            bool modFilesLoaded = Values.V.LoadModValues(modValueFileNames, throwExceptionOnFail: true, out mods, out string resultsMessage);

            if (modFilesLoaded)
            {
                if (showResult && !string.IsNullOrEmpty(resultsMessage))
                    MessageBox.Show(resultsMessage, "Mod Values loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (applySettings)
                    ApplySettingsToValues();
                speciesSelector1.SetSpeciesLists(Values.V.species, Values.V.aliases);
            }

            _creatureCollection.ModList = mods;
            UpdateStatusBar();
            return modFilesLoaded;
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
                    "\n\nThe value files for the following mods are already locally available and just need to be added to the library:\n\n- "
                    + string.Join("\n- ", locallyAvailableModFiles)
                    : "")
                + (onlineAvailableModsExist ?
                    "\n\nThe value files for the following mods can be downloaded automatically if you want:\n\n- "
                    + string.Join("\n- ", onlineAvailableModFiles)
                    : "")
                + (unavailableModsExist ?
                    "\n\nThe value files for the following mods are unknown. Currently you cannot create a mod-values file manually. Check on the discord server of ASB the #mod-requests channel and ask for that mod, maybe we'll add support for it in the future.\n\n- "
                    + string.Join("\n- ", unavailableModFiles)
                    : ""),
                "ASB: Unknown species", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if ((locallyAvailableModsExist || onlineAvailableModsExist)
                && MessageBox.Show("Do you want to " + (onlineAvailableModsExist ? "download and " : "") + "add the values-files for the following mods to the library?\n\n- "
                                   + string.Join("\n- ", onlineAvailableModFiles) + (locallyAvailableModsExist && onlineAvailableModsExist ? "\n\n- " : string.Empty)
                                   + string.Join("\n- ", locallyAvailableModFiles),
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

            if (modValueFilesWithAvailableUpdate.Any()
                && MessageBox.Show("For " + modValueFilesWithAvailableUpdate.Count.ToString() + " value files there is an update available. It is strongly recommended to use the updated versions.\n"
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
                && MessageBox.Show(missingModValueFilesOnlineAvailable.Count.ToString() + " mod-value files are not available locally. Without these files the library will not display all creatures.\n"
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
                MessageBox.Show(missingModValueFilesOnlineNotAvailable.Count.ToString() + " mod-value files are neither available locally nor online. The creatures of the missing mod will not be displayed.\n"
                + "The following files are missing\n\n"
                + string.Join("\n", missingModValueFilesOnlineNotAvailable),
                $"Missing value files - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    foreach (var m in modsManifest.modsByFiles) m.Value.onlineAvailable = true;
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(
                        $"Mods manifest file {Path.Combine(FileService.ValuesFolder, FileService.ModsManifest)} not found " +
                        "and downloading it failed. You can try it later or try to update your application.",
                        $"File not found - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (FormatException)
                {
                    FormatExceptionMessageBox(Path.Combine(FileService.ValuesFolder, FileService.ModsManifest));
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
                MessageBox.Show(
                    $"Serialization exception while trying to load the mods-manifest file.\n\n{serEx.Message}",
                    $"File loading error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            modsManifest?.Initialize();
            values.SetModsManifest(modsManifest);
            return true;
        }

        private static void LoadServerMultiplierPresets(Values values)
        {
            if (!ServerMultipliersPresets.TryLoadServerMultipliersPresets(out values.serverMultipliersPresets))
            {
                MessageBox.Show("The file with the server multipliers couldn't be loaded. Changed settings, e.g. for the singleplayer will be not available.\nIt's recommended to download the application again.",
                    $"Server multiplier file not loaded - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch (DirectoryNotFoundException)
            {
                if (MessageBox.Show($"One of the following folders where the values-file is expected was not found.\n{FileService.GetJsonPath(FileService.ValuesFolder, FileService.ValuesJson)}\n\n" +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"Values-File {FileService.ValuesJson} not found. " +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
            }
            catch (FormatException)
            {
                FormatExceptionMessageBox(FileService.ValuesJson);
                if ((DateTime.Now - Properties.Settings.Default.lastUpdateCheck).TotalMinutes < 10)
                    CheckForUpdates();
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
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void DeserializeExceptionMessageBox(string filePath, string eMessage)
        {
            MessageBox.Show($"File {filePath} couldn't be deserialized.\nErrormessage:\n\n" + eMessage,
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

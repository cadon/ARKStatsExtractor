using ARKBreedingStats.Library;
using ARKBreedingStats.mods;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        private const string CollectionFileExtension = ".asb";

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="resetCollection">If true, the user is not asked and a new collection is created. This can be used if something went wrong while loading a file and a clean collection is needed.</param>
        private void NewCollection(bool resetCollection = false)
        {
            if (!resetCollection
                && UnsavedChanges()
                && CustomMessageBox.Show(Loc.S("Collection changed discard and new?"),
                    Loc.S("Discard changes?"), Loc.S("Discard changes and new"), buttonCancel: Loc.S("Cancel"),
                    icon: MessageBoxIcon.Warning) != DialogResult.Yes
               )
            {
                return;
            }

            if (_creatureCollection.modIDs?.Any() ?? false)
            {
                // if old collection had additionalValues, load the original ones to reset all modded values
                var (statValuesLoaded, _) = LoadStatAndKibbleValues(applySettings: false);
                if (!statValuesLoaded)
                {
                    MessageBoxes.ShowMessageBox("Couldn't load stat values. Please redownload the application.",
                        $"{Loc.S("error")} while loading the stat-values");
                }
            }

            // use previously used multipliers again in the new file
            var oldMultipliers = _creatureCollection.serverMultipliers;
            var asaMode = _creatureCollection.Game == Ark.Asa;

            if (!Properties.Settings.Default.KeepMultipliersForNewLibrary)
            {
                oldMultipliers = null;
                asaMode = true; // default to ASA
            }

            if (oldMultipliers == null)
                oldMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official);

            _creatureCollection = new CreatureCollection
            {
                serverMultipliers = oldMultipliers,
                ModList = new List<Mod>()
            };
            _currentFileName = null;
            _fileSync?.ChangeFile(_currentFileName);

            if (asaMode)
            {
                _creatureCollection.Game = Ark.Asa;
                ReloadModValuesOfCollectionIfNeeded(true, false, false, false);
            }

            pedigree1.Clear();
            breedingPlan1.Clear();
            creatureInfoInputExtractor.Clear(true);
            creatureInfoInputTester.Clear(true);
            ApplySettingsToValues();
            InitializeCollection();

            UpdateCreatureListings();
            creatureBoxListView.Clear();
            Properties.Settings.Default.LastSaveFile = null;
            SetCollectionChanged(false);
        }

        delegate void collectionChangedCallback();

        /// <summary>
        /// This method is called when the collection file was changed. This is used when the file is shared via a cloud service.
        /// </summary>
        private void CollectionChanged()
        {
            if (creatureBoxListView.InvokeRequired)
            {
                collectionChangedCallback d = CollectionChanged;
                Invoke(d);
            }
            else
            {
                LoadCollectionFile(_currentFileName, true, true, true);
            }
        }

        /// <summary>
        /// Recalculate all the stat values of all creatures. Should be done after multipliers were changed or creatures are loaded.
        /// </summary>
        private void RecalculateAllCreaturesValues()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = _creatureCollection.creatures.Count;
            toolStripProgressBar1.Visible = true;
            int? levelStep = _creatureCollection.getWildLevelStep();
            foreach (Creature c in _creatureCollection.creatures)
            {
                c.RecalculateCreatureValues(levelStep);
                toolStripProgressBar1.Value++;
            }
            toolStripProgressBar1.Visible = false;
        }

        /// <summary>
        /// Displays a file selector dialog and loads a collection file.
        /// </summary>
        /// <param name="add">If true, the current loaded creatures will be kept and the ones of the loaded file are added</param>
        private void LoadCollection(bool add = false)
        {
            if (!add && !DiscardChangesAndLoadNewLibrary())
            {
                return;
            }

            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = $"ASB Collection Files (*{CollectionFileExtension}; *.xml)|*{CollectionFileExtension};*.xml"
                                + $"|ASB Collection File (*{CollectionFileExtension})|*{CollectionFileExtension}"
                                + "|Old ASB Collection File(*.xml)| *.xml",
                InitialDirectory = InitialDirectoryForLoadSave
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LoadCollectionFile(dlg.FileName, add);
                }
            }
        }

        /// <summary>
        /// Returns true if there are no unsaved changes or the user wants to discard the changes of the currently loaded library.
        /// </summary>
        private bool DiscardChangesAndLoadNewLibrary()
        {
            return !UnsavedChanges()
                    || CustomMessageBox.Show(Loc.S("Collection changed discard and load?"),
                    Loc.S("Discard changes?"), Loc.S("Discard changes and load file"), buttonCancel: Loc.S("Cancel"), icon: MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        /// <summary>
        /// Returns the directory of the currently used file or the last used directory when loading a file.
        /// </summary>
        private string InitialDirectoryForLoadSave => !string.IsNullOrEmpty(_currentFileName)
            ? Path.GetDirectoryName(_currentFileName)
            : !string.IsNullOrEmpty(Properties.Settings.Default.LastUsedCollectionFolder)
              && Directory.Exists(Properties.Settings.Default.LastUsedCollectionFolder)
                ? Properties.Settings.Default.LastUsedCollectionFolder
                : null
            ;

        /// <summary>
        /// Save the current collection under its file. If it has no file, use saveAs.
        /// </summary>
        private void SaveCollection()
        {
            if (string.IsNullOrEmpty(_currentFileName))
            {
                SaveNewCollection();
                if (!string.IsNullOrEmpty(_currentFileName))
                    Properties.Settings.Default.LastUsedCollectionFolder = Path.GetDirectoryName(_currentFileName);
            }
            else
            {
                SaveCollectionToFileName(_currentFileName);
            }
        }

        private void SaveNewCollection()
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = $"Creature Collection File (*{CollectionFileExtension})|*{CollectionFileExtension}",
                InitialDirectory = InitialDirectoryForLoadSave
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _currentFileName = dlg.FileName;
                    SaveCollectionToFileName(_currentFileName);
                    AddPathToRecentlyUsed(_currentFileName);
                    _fileSync?.ChangeFile(_currentFileName);
                }
            }
        }

        private void SaveCollectionToFileName(string filePath)
        {
            // remove expired timers if setting is set
            if (Properties.Settings.Default.DeleteExpiredTimersOnSaving)
                timerList1.DeleteAllExpiredTimers(false, false);

            notesControl1.CheckForUnsavedChanges();

            // Wait until the file is writable
            const int numberOfRetries = 5;
            const int delayOnRetryBase = 500;
            bool fileSaved = false;

            var tempSavePath = filePath + ".tmp";

            _fileSync?.SavingStarts();
            for (int i = 0; i < numberOfRetries; ++i)
            {
                try
                {
                    using (StreamWriter file = File.CreateText(tempSavePath))
                    {
                        JsonSerializer serializer = new JsonSerializer
                        {
                            Formatting = Properties.Settings.Default.prettifyCollectionJson ? Formatting.Indented : Formatting.None,
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc // save all date-times as UTC, so synced files don't change the timezones
                        };
                        serializer.Serialize(file, _creatureCollection);
                    }

                    if (new FileInfo(tempSavePath).Length == 0)
                        throw new IOException("Saved file is empty and contains no data.");

                    // if saving was successful, keep old file as backup if set or remove it, then move successfully saved temp file to correct
                    var backupEveryMinutes = Properties.Settings.Default.BackupEveryMinutes;
                    var keepBackupFilesCount = Properties.Settings.Default.BackupFileCount;

                    if (keepBackupFilesCount != 0
                        && (backupEveryMinutes == 0 ||
                            (DateTime.Now - _lastAutoSaveBackup).TotalMinutes > backupEveryMinutes)
                        && FileService.IsValidJsonFile(filePath))
                    {
                        if (!KeepBackupFile(filePath, keepBackupFilesCount))
                            File.Delete(filePath); // outdated file is not needed anymore
                    }
                    else
                        File.Delete(filePath); // outdated file is not needed anymore

                    File.Move(tempSavePath, filePath);

                    fileSaved = true;
                    Properties.Settings.Default.LastSaveFile = filePath;

                    break; // when file is saved, break
                }
                catch (IOException)
                {
                    // if file is not saveable wait a bit, each time longer
                    Thread.Sleep(delayOnRetryBase * (1 << i));
                }
                catch (System.Runtime.Serialization.SerializationException ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex, "Error during serialization.");
                    break;
                }
                catch (InvalidOperationException ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex, "Error during serialization.");
                    break;
                }
            }
            _fileSync?.SavingEnds();

            if (fileSaved)
                SetCollectionChanged(false);
            else
                MessageBoxes.ShowMessageBox($"This file couldn't be saved:\n{filePath}\nMaybe the file is used by another application.");
        }

        /// <summary>
        /// Creates a backup file of the current library file, then removes old backup files to keep number to setting.
        /// Returns true if the currentSaveFile was moved as a backup, false if it's still existing.
        /// </summary>
        private bool KeepBackupFile(string currentSaveFilePath, int keepBackupFilesCount)
        {
            string fileNameWoExt = Path.GetFileNameWithoutExtension(currentSaveFilePath);
            string backupFileName = $"{fileNameWoExt}_backup_{new FileInfo(currentSaveFilePath).LastWriteTime:yyyy-MM-dd_HH-mm-ss}{CollectionFileExtension}";

            var backupFolderPath = Properties.Settings.Default.BackupFolder;

            try
            {
                if (string.IsNullOrEmpty(backupFolderPath))
                    backupFolderPath = Path.GetDirectoryName(currentSaveFilePath);
                else
                    Directory.CreateDirectory(backupFolderPath);

                string backupFilePath = Path.Combine(backupFolderPath, backupFileName);
                if (File.Exists(backupFilePath))
                {
                    return false; // backup file of that timestamp already exists, no extra backup needed.
                }

                File.Move(currentSaveFilePath, backupFilePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error while creating backup files of the save file.\nMaybe the backup folder is protected or an antivirus application blocks the file moving.");
                return false;
            }

            _lastAutoSaveBackup = DateTime.Now;

            // delete oldest backup file if more than a certain number

            var directory = new DirectoryInfo(backupFolderPath);
            var oldBackupFiles = directory.GetFiles($"{fileNameWoExt}_backup_*{CollectionFileExtension}")
                .OrderByDescending(f => f.Name)
                .Skip(keepBackupFilesCount)
                .ToArray();
            foreach (FileInfo f in oldBackupFiles)
            {
                try
                {
                    f.Delete();
                }
                catch
                {
                    // ignored
                }
            }

            return true;
        }

        /// <summary>
        /// Loads the given creature collection file.
        /// </summary>
        /// <param name="filePath">File that contains the collection</param>
        /// <param name="keepCurrentCreatures">add the creatures of the loaded file to the current ones</param>
        /// <param name="keepCurrentSelections">don't change the species selection or tab, use if a synchronized library is loaded</param>
        /// <returns></returns>
        private bool LoadCollectionFile(string filePath, bool keepCurrentCreatures = false, bool keepCurrentSelections = false, bool triggeredByFileWatcher = false)
        {
            Species selectedSpecies = speciesSelector1.SelectedSpecies;
            Species selectedLibrarySpecies = listBoxSpeciesLib.SelectedItem as Species;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBoxes.ShowMessageBox($"Save file with name \"{filePath}\" does not exist!", $"File not found");
                return false;
            }

            CreatureCollection previouslyLoadedCreatureCollection = _creatureCollection;

            // Wait until the file is readable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;

            FileStream fileStream = null;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                // sometimes a synchronized file has only 0 bytes, i.e. it's not yet synchronized fully. In this case wait a bit and try again
                if (Properties.Settings.Default.syncCollection && new FileInfo(filePath).Length == 0)
                {
                    Thread.Sleep(delayOnRetry);
                    continue;
                }
                try
                {
                    if (Path.GetExtension(filePath).ToLower() == ".xml")
                    {
                        // old format for backwards compatibility
                        using (fileStream = File.OpenRead(filePath))
                        {
                            // use xml-serializer for old library-format
                            XmlSerializer reader = new XmlSerializer(typeof(oldLibraryFormat.CreatureCollectionOld));
                            var creatureCollectionOld = (oldLibraryFormat.CreatureCollectionOld)reader.Deserialize(fileStream);

                            List<Mod> mods = null;
                            // first check if additional values are used, and if the according values-file is already available.
                            // if not, abort conversion and first make sure the file is available, e.g. downloaded
                            if (!string.IsNullOrEmpty(creatureCollectionOld.additionalValues))
                            {
                                // usually the old filename is equal to the mod-tag
                                bool modFound = false;
                                string modTag = Path.GetFileNameWithoutExtension(creatureCollectionOld.additionalValues).Replace(" ", "").ToLower().Replace("gaiamod", "gaia");
                                foreach (KeyValuePair<string, ModInfo> tmi in Values.V.modsManifest.modsByTag)
                                {
                                    if (tmi.Key.ToLower() == modTag)
                                    {
                                        modFound = true;

                                        MessageBox.Show("The library contains creatures of modded species. For a correct file-conversion the correct mod-values file is needed.\n\n"
                                            + "If the mod-value file is not loaded, the conversion may assign wrong species to your creatures.\n"
                                            + "If the mod-value file is not available locally, it will be tried to download it.",
                                            $"Mod values needed - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                        if (Values.V.loadedModsHash != Values.NoModsHash)
                                            LoadStatAndKibbleValues(false); // reset values to default
                                        LoadModValueFiles(new List<string> { tmi.Value.mod.FileName }, true, true, out mods);
                                        break;
                                    }
                                }
                                if (!modFound
                                    && MessageBox.Show("The additional-values file in the library you're loading is unknown. You should first get a values-file in the new format for that mod.\n"
                                        + "If you're loading the library the conversion of some modded species to the new format may fail and the according creatures have to be imported again later.\n\n"
                                        + $"File:\n{filePath}\n"
                                        + $"unknown mod-file: {modTag}\n\n"
                                        + "Do you want to load the library and risk losing creatures?", $"Unknown mod-file - {Utils.ApplicationNameVersion}",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                    return false;
                            }

                            _creatureCollection = oldLibraryFormat.FormatConverter.ConvertXml2Asb(creatureCollectionOld, filePath);
                            _creatureCollection.ModList = mods ?? new List<Mod>(0);

                            if (_creatureCollection == null) throw new Exception("Conversion failed");

                            string fileNameWOExt = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                            // check if new fileName is not yet existing
                            filePath = fileNameWOExt + CollectionFileExtension;
                            if (File.Exists(filePath))
                            {
                                int fi = 2;
                                while (File.Exists(fileNameWOExt + "_" + fi + CollectionFileExtension)) fi++;
                                filePath = fileNameWOExt + "_" + fi + CollectionFileExtension;
                            }

                            // save converted library
                            SaveCollectionToFileName(filePath);
                        }
                    }
                    else
                    {
                        // new json-format
                        if (FileService.LoadJsonFile(filePath, out CreatureCollection readCollection, out string errorMessage))
                        {
                            if (!Version.TryParse(readCollection.FormatVersion, out Version ccVersion)
                               || !Version.TryParse(CreatureCollection.CurrentLibraryFormatVersion, out Version currentVersion)
                               || ccVersion > currentVersion)
                            {
                                throw new FormatException($"Unsupported format version: {readCollection.FormatVersion ?? "null"}");
                            }
                            _creatureCollection = readCollection;
                        }
                        else
                        {
                            MessageBoxes.ShowMessageBox($"Error while trying to read the library-file\n{filePath}\n\n{errorMessage}");
                            return false;
                        }
                    }

                    break;
                }
                catch (IOException)
                {
                    // if file is not readable
                    Thread.Sleep(delayOnRetry);
                }
                catch (FormatException ex)
                {
                    // This FormatVersion is not understood, abort
                    MessageBoxes.ShowMessageBox("This library format is unsupported in this version of ARK Smart Breeding." +
                                                 $"\n\n{ex.Message}\n\nTry updating to a newer version.");
                    if ((DateTime.Now - Properties.Settings.Default.lastUpdateCheck).TotalMinutes < 10)
                        CheckForUpdates();
                    return false;
                }
                catch (InvalidOperationException ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex, $"The library-file\n{filePath}\ncouldn't be opened, we thought you should know.");
                    return false;
                }
                finally
                {
                    fileStream?.Close();
                }
            }

            if (_creatureCollection.ModValueReloadNeeded)
            {
                // load original multipliers if they were changed
                if (!LoadStatAndKibbleValues(false).statValuesLoaded)
                {
                    NewCollection(true);
                    return false;
                }
            }
            if (_creatureCollection.ModValueReloadNeeded
                && !LoadModValuesOfCollection(_creatureCollection, false, false))
            {
                MessageBoxes.ShowMessageBox("Mod values of the library file couldn't be loaded.", icon: MessageBoxIcon.Error);
                NewCollection(true);
                return false;
            }

            if (_creatureCollection.serverMultipliers == null)
            {
                _creatureCollection.serverMultipliers = previouslyLoadedCreatureCollection.serverMultipliers ?? Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.Official);
            }

            if (speciesSelector1.LastSpecies != null && speciesSelector1.LastSpecies.Length > 0)
            {
                tamingControl1.SetSpecies(Values.V.SpeciesByBlueprint(speciesSelector1.LastSpecies[0]));
            }

            _creatureCollection.FormatVersion = CreatureCollection.CurrentLibraryFormatVersion;

            ApplySettingsToValues();

            bool creatureWasAdded = false;

            if (keepCurrentCreatures)
            {
                creatureWasAdded = previouslyLoadedCreatureCollection.MergeCreatureList(_creatureCollection.creatures, removeCreatures: _creatureCollection.DeletedCreatureGuids);
                _creatureCollection = previouslyLoadedCreatureCollection;
            }
            else
            {
                _currentFileName = filePath;
                _fileSync?.ChangeFile(_currentFileName);
                creatureBoxListView.Clear();
            }

            _creatureCollection.DeletedCreatureGuids = null; // the info was processed and is no longer needed.

            // remove creature entries without species information. some outdated and invisible entries can exist with that. the blueprintInfo is not deleted with the current version, so no new such entries should appear.
            _creatureCollection.creatures = _creatureCollection.creatures
                .Where(c => !string.IsNullOrEmpty(c.speciesBlueprint)).ToList();

            var duplicatesWereRemoved = InitializeCollection(keepCurrentSelections);

            _filterListAllowed = false;

            SetCollectionChanged(creatureWasAdded || duplicatesWereRemoved, triggeredByFileWatcher: triggeredByFileWatcher); // setCollectionChanged only if there really were creatures added from the old library to the just opened one

            ///// creatures loaded.

            // calculate creature values
            RecalculateAllCreaturesValues();

            foreach (var c in _creatureCollection.creatures)
            {
                c.InitializeFlags();
                if (c.ArkIdImported && c.ArkIdInGame == null)
                    c.ArkIdInGame = Utils.ConvertImportedArkIdToIngameVisualization(c.ArkId);
            }

            if (!keepCurrentSelections && _creatureCollection.creatures.Any())
                tabControlMain.SelectedTab = tabPageLibrary;

            creatureBoxListView.CreatureCollection = _creatureCollection;

            UpdateCreatureListings();

            // set global species that was set before loading
            selectedSpecies = Values.V.SpeciesByBlueprint(selectedSpecies?.blueprintPath);
            if (selectedSpecies != null
                && _creatureCollection.creatures.Any(c => c.Species != null && c.Species.Equals(selectedSpecies))
                )
            {
                speciesSelector1.SetSpecies(selectedSpecies);
            }
            else if (_creatureCollection.creatures.Any())
                speciesSelector1.SetSpecies(_creatureCollection.creatures[0].Species);

            // set library species to what it was before loading
            selectedLibrarySpecies = Values.V.SpeciesByBlueprint(selectedLibrarySpecies?.blueprintPath);
            if (selectedLibrarySpecies != null)
                listBoxSpeciesLib.SelectedItem = selectedLibrarySpecies;
            else if (Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad)
                listBoxSpeciesLib.SelectedItem = speciesSelector1.SelectedSpecies;

            _filterListAllowed = true;
            FilterLibRecalculate();

            // apply last sorting
            SortLibrary();

            UpdateTempCreatureDropDown();

            Properties.Settings.Default.LastSaveFile = filePath;
            Properties.Settings.Default.LastUsedCollectionFolder = Path.GetDirectoryName(filePath);
            AddPathToRecentlyUsed(filePath);

            _lastAutoSaveBackup = DateTime.Now.AddMinutes(-10);

            return true;
        }

        /// <summary>
        /// Returns true if there are unsaved changes.
        /// </summary>
        private bool UnsavedChanges()
        {
            notesControl1.CheckForUnsavedChanges();
            return _collectionDirty;
        }

        /// <summary>
        /// Call if the collection has changed and needs to be saved.
        /// </summary>
        /// <param name="changed">is the collection changed?</param>
        /// <param name="species">set to a specific species if only this species needs updates in the pedigree / breeding-planner. Set to null if no species needs updates</param>
        /// <param name="triggeredByFileWatcher">If true, the call was invoked by the fileWatcher, a file save should not performed then.</param>
        private void SetCollectionChanged(bool changed, Species species = null, bool triggeredByFileWatcher = false)
        {
            if (changed)
            {
                if (species == null || pedigree1.SelectedSpecies == species)
                    pedigree1.PedigreeNeedsUpdate = true;
                if (species == null || breedingPlan1.CurrentSpecies == species)
                    breedingPlan1.BreedingPlanNeedsUpdate = true;
            }

            if (triggeredByFileWatcher) return;

            if (changed && Properties.Settings.Default.autosave)
            {
                // save changes automatically
                SaveCollection();
                // function is called soon again from SaveCollectionToFileName(string filePath) to perform title text update
                return;
            }

            var currentFileNotEmpty = !string.IsNullOrEmpty(_currentFileName);
            _collectionDirty = changed;
            string fileName = currentFileNotEmpty ? Path.GetFileName(_currentFileName) : null;
            Text = $"{Utils.ApplicationNameVersion}{(currentFileNotEmpty ? " - " + fileName : string.Empty)}{(changed ? " *" : string.Empty)}";
            openFolderOfCurrentFileToolStripMenuItem.Enabled = currentFileNotEmpty;
        }

        /// <summary>
        /// Saves a file with the current library and the currently entered values in the extractor for issue reporting.
        /// </summary>
        private void SaveDebugFile()
        {
            // get temp folder for zipping
            var tempFolder = FileService.GetTempDirectory();
            var timeStamp = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            string tempFilePath = Path.Combine(tempFolder, $"ASB_issue_{timeStamp}.asb");
            string tempZipFilePath = Path.Combine(Path.GetDirectoryName(tempFolder), $"ASB_issue_{timeStamp}.zip");

            // add currently set values in the extractor to the saved values
            var debugCreatureValues = GetCreatureValuesFromExtractor();
            debugCreatureValues.name = "DebugValues_" + debugCreatureValues.name;
            _creatureCollection.creaturesValues.Add(debugCreatureValues);

            using (StreamWriter file = File.CreateText(tempFilePath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc // save all date-times as UTC, so synced files don't change the timezones
                };
                serializer.Serialize(file, _creatureCollection);
            }
            // remove debug creature
            _creatureCollection.creaturesValues.Remove(debugCreatureValues);

            // zip file
            //FileService.TryDeleteFile(tempZipFilePath);
            try
            {
                ZipFile.CreateFromDirectory(tempFolder, tempZipFilePath);
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "The debug file couldn't be saved.");
            }

            // remove temp library file and folder
            FileService.TryDeleteFile(tempFilePath);
            FileService.TryDeleteDirectory(tempFolder);

            // copy zip file to clipboard
            Clipboard.SetFileDropList(new StringCollection { tempZipFilePath });

            // display info that debug file is in clipboard
            SetMessageLabelText("A File with the current library and the values in the extractor has been created and copied to the clipboard. You can paste this file to a folder to add it to an issue report.", MessageBoxIcon.Information, tempZipFilePath);
        }

        /// <summary>
        /// Zipped library files are often error reports.
        /// </summary>
        private bool OpenZippedLibrary(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            try
            {
                // get temp folder for zipping
                var tempFolder = FileService.GetTempDirectory();
                // unzip file
                ZipFile.ExtractToDirectory(filePath, tempFolder);

                var tempLibPath = Directory.GetFiles(tempFolder)[0];
                LoadCollectionFile(tempLibPath);

                // delete temp extracted file
                FileService.TryDeleteFile(tempLibPath);
                FileService.TryDeleteDirectory(tempFolder);

                Properties.Settings.Default.LastSaveFile = null;
                _currentFileName = null;

                // select last creature values
                var tempCreatureCount = toolStripCBTempCreatures.Items.Count;
                if (tempCreatureCount > 0)
                {
                    toolStripCBTempCreatures.SelectedIndex = tempCreatureCount - 1;
                    ExtractLevels();
                }
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error while loading zipped debug library dump");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a file path to the recently used list for libraries.
        /// </summary>
        /// <param name="filePath"></param>
        private void AddPathToRecentlyUsed(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var files = Properties.Settings.Default.LastUsedLibraryFiles;
            if (files == null)
            {
                Properties.Settings.Default.LastUsedLibraryFiles = new[] { filePath };
                UpdateRecentlyUsedFileMenu();
                return;
            }

            if (files.FirstOrDefault() == filePath)
            {
                if (recentlyUsedToolStripMenuItem.DropDownItems.Count == 0)
                    UpdateRecentlyUsedFileMenu();
                return;
            }

            // add filePath to the first position
            Properties.Settings.Default.LastUsedLibraryFiles =
                files.Where(f => f != filePath).Prepend(filePath).Take(10).ToArray();
            UpdateRecentlyUsedFileMenu();
        }

        /// <summary>
        /// Updates the menu items for the last used files.
        /// </summary>
        private void UpdateRecentlyUsedFileMenu()
        {
            recentlyUsedToolStripMenuItem.DropDownItems.Clear();

            if (!(Properties.Settings.Default.LastUsedLibraryFiles?.Any() ?? false)) return;

            recentlyUsedToolStripMenuItem.DropDownItems.AddRange(
                Properties.Settings.Default.LastUsedLibraryFiles.Select(f => new ToolStripMenuItem(f, null, OpenRecentlyUsedFile)).ToArray()
            );
        }

        private void RemoveNonExistingFilesInRecentlyUsedFiles()
        {
            var files = Properties.Settings.Default.LastUsedLibraryFiles;
            if (files?.Any() != true) return;

            Properties.Settings.Default.LastUsedLibraryFiles = files.Where(File.Exists).ToArray();
        }

        private void OpenRecentlyUsedFile(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem mi
                && !string.IsNullOrEmpty(mi.Text)
                && DiscardChangesAndLoadNewLibrary()
                )
                LoadCollectionFile(mi.Text);
        }

        /// <summary>
        /// Imports creature from file created by the export gun mod.
        /// Returns already existing Creature or null if it's a new creature.
        /// </summary>
        private Creature ImportExportGunFiles(string[] filePaths, out bool creatureAdded, out Creature lastAddedCreature, out bool copiedNameToClipboard)
        {
            creatureAdded = false;
            copiedNameToClipboard = false;
            var newCreatures = new List<Creature>();

            var importedCounter = 0;
            var importFailedCounter = 0;
            string lastError = null;
            string lastCreatureFilePath = null;
            string serverMultipliersHash = null;
            bool? multipliersImportSuccessful = null;
            string serverImportResult = null;
            Creature alreadyExistingCreature = null;
            var gameSettingBefore = _creatureCollection.Game;

            foreach (var filePath in filePaths)
            {
                var c = ImportExportGun.ImportCreature(filePath, out lastError, out serverMultipliersHash);
                if (c != null)
                {
                    newCreatures.Add(c);
                    importedCounter++;
                    lastCreatureFilePath = filePath;
                }
                else if (lastError != null)
                {
                    // file could be a server multiplier file, try to read it that way
                    var esm = ImportExportGun.ReadServerMultipliers(filePath, out var serverImportResultTemp);
                    if (esm != null)
                    {
                        multipliersImportSuccessful = ImportExportGun.SetServerMultipliers(_creatureCollection, esm, Path.GetFileNameWithoutExtension(filePath));
                        serverImportResult = serverImportResultTemp;
                        continue;
                    }

                    importFailedCounter++;
                    MessageBoxes.ShowMessageBox(lastError);
                }
            }

            if (lastCreatureFilePath != null && !string.IsNullOrEmpty(serverMultipliersHash) && _creatureCollection.ServerMultipliersHash != serverMultipliersHash)
            {
                // current server multipliers might be outdated, import them again
                // for ASE the export gun create a .sav file containing a json, for ASA directly a .json file
                var serverMultiplierFilePath = Path.Combine(Path.GetDirectoryName(lastCreatureFilePath), "Servers", serverMultipliersHash + ".json");
                if (!File.Exists(serverMultiplierFilePath))
                    serverMultiplierFilePath = Path.Combine(Path.GetDirectoryName(lastCreatureFilePath), "Servers", serverMultipliersHash + ".sav");

                multipliersImportSuccessful = ImportExportGun.ImportServerMultipliers(_creatureCollection, serverMultiplierFilePath, serverMultipliersHash, out serverImportResult);
            }

            if (multipliersImportSuccessful == true)
            {
                if (_creatureCollection.Game != gameSettingBefore)
                {
                    // ASA setting changed
                    var loadAsa = gameSettingBefore != Ark.Asa;
                    ReloadModValuesOfCollectionIfNeeded(loadAsa, false, false);
                }

                ApplySettingsToValues();
            }

            lastAddedCreature = newCreatures.LastOrDefault();
            if (lastAddedCreature != null)
            {
                creatureAdded = true;
            }

            var totalCreatureCount = _creatureCollection.GetTotalCreatureCount();
            // select creature objects that will be in the library (i.e. new creature, or existing creature), and the old name
            var persistentCreaturesAndOldName = newCreatures.Select(c => (creature:
                IsCreatureAlreadyInLibrary(c.guid, c.ArkId, out alreadyExistingCreature)
                    ? alreadyExistingCreature
                    : c, oldName: alreadyExistingCreature?.name)).ToArray();

            _creatureCollection.MergeCreatureList(newCreatures, true);
            UpdateCreatureParentLinkingSort(false);

            // apply naming pattern if needed. This can only be done after parent linking to get correct name pattern values related to parents
            Species lastSpecies = null;
            Creature[] creaturesOfSpecies = null;
            foreach (var c in persistentCreaturesAndOldName)
            {
                copiedNameToClipboard = SetNameOfImportedCreature(c.creature, lastSpecies == c.creature.Species ? creaturesOfSpecies : null, out creaturesOfSpecies, new Creature(c.creature.Species, c.oldName), totalCreatureCount);
                lastSpecies = c.creature.Species;
                if (c.oldName == null) totalCreatureCount++; // if creature was added, increase total count for name pattern
            }

            UpdateListsAfterCreaturesAdded();

            var resultText = (importedCounter > 0 || importFailedCounter > 0
                                 ? $"Imported {importedCounter} creatures successfully.{(importFailedCounter > 0 ? $"Failed to import {importFailedCounter} files. Last error:{Environment.NewLine}{lastError}" : $"{Environment.NewLine}Last file: {lastCreatureFilePath}")}"
                                 : string.Empty)
                             + (string.IsNullOrEmpty(serverImportResult)
                                 ? string.Empty
                                 : (importedCounter > 0 || importFailedCounter > 0 ? Environment.NewLine : string.Empty)
                                  + serverImportResult);

            SetMessageLabelText(resultText, importFailedCounter > 0 || multipliersImportSuccessful == false ? MessageBoxIcon.Error : MessageBoxIcon.Information, lastCreatureFilePath);

            if (lastAddedCreature != null)
            {
                tabControlMain.SelectedTab = tabPageLibrary;
                if (listBoxSpeciesLib.SelectedItem != null &&
                    listBoxSpeciesLib.SelectedItem != lastAddedCreature.Species)
                    listBoxSpeciesLib.SelectedItem = lastAddedCreature.Species;
                _ignoreNextMessageLabel = true; // keep import message
                SelectCreatureInLibrary(lastAddedCreature);
            }

            return alreadyExistingCreature;
        }

        /// <summary>
        /// Call after creatures were added (imported) to the library. Updates parent linkings, creature lists, set collection as changed
        /// </summary>
        private void UpdateCreatureParentLinkingSort(bool updateLists = true)
        {
            UpdateParents(_creatureCollection.creatures);

            foreach (var creature in _creatureCollection.creatures)
            {
                creature.RecalculateAncestorGenerations();
            }

            UpdateIncubationParents(_creatureCollection);

            if (updateLists)
                UpdateListsAfterCreaturesAdded();
        }


        private void UpdateListsAfterCreaturesAdded()
        {
            // update UI
            SetCollectionChanged(true);
            UpdateCreatureListings();

            if (_creatureCollection.creatures.Any())
                tabControlMain.SelectedTab = tabPageLibrary;

            // reapply last sorting
            SortLibrary();

            UpdateTempCreatureDropDown();
        }

        /// <summary>
        /// Imports a creature when listening to a server.
        /// </summary>
        private void AsbServerDataSent((string jsonData, string serverHash, string message) data)
        {
            if (!string.IsNullOrEmpty(data.message))
            {
                SetMessageLabelText(data.message, MessageBoxIcon.Error);
                return;
            }

            string resultText;
            if (string.IsNullOrEmpty(data.serverHash))
            {
                // import creature
                var creature = ImportExportGun.ImportCreatureFromJson(data.jsonData, null, out resultText, out _);
                if (creature == null)
                {
                    SetMessageLabelText(resultText, MessageBoxIcon.Error);
                    return;
                }

                _creatureCollection.MergeCreatureList(new[] { creature }, true);
                UpdateCreatureParentLinkingSort();

                if (resultText == null)
                    resultText = $"Received creature from server: {creature}";

                SetMessageLabelText(resultText, MessageBoxIcon.Information);

                tabControlMain.SelectedTab = tabPageLibrary;
                if (listBoxSpeciesLib.SelectedItem != null &&
                    listBoxSpeciesLib.SelectedItem != creature.Species)
                    listBoxSpeciesLib.SelectedItem = creature.Species;
                _ignoreNextMessageLabel = true;
                SelectCreatureInLibrary(creature);
                return;
            }

            // import server settings
            var success = ImportExportGun.ImportServerMultipliersFromJson(_creatureCollection, data.jsonData, data.serverHash, out resultText);
            SetMessageLabelText(resultText, success ? MessageBoxIcon.Information : MessageBoxIcon.Error, resultText);
        }
    }
}

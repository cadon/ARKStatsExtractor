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

namespace ARKBreedingStats
{
    public partial class Form1
    {
        private const string CollectionFileExtension = ".asb";

        private void NewCollection()
        {
            if (_collectionDirty
                && MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                        "are you sure you want to discard your changes and create a new Library without saving?",
                        "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (_creatureCollection.modIDs?.Any() ?? false)
            {
                // if old collection had additionalValues, load the original ones to reset all modded values
                var (statValuesLoaded, _) = LoadStatAndKibbleValues(applySettings: false);
                if (!statValuesLoaded)
                {
                    MessageBox.Show("Couldn't load stat values. Please redownload the application.", $"{Loc.S("error")} while loading the stat-values - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (_creatureCollection.serverMultipliers == null)
                _creatureCollection.serverMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL);
            // use previously used multipliers again in the new file
            ServerMultipliers oldMultipliers = _creatureCollection.serverMultipliers;

            _creatureCollection = new CreatureCollection
            {
                serverMultipliers = oldMultipliers,
                ModList = new List<Mod>()
            };
            _creatureCollection.FormatVersion = CreatureCollection.CURRENT_FORMAT_VERSION;
            pedigree1.Clear();
            breedingPlan1.Clear();
            creatureInfoInputExtractor.Clear(true);
            creatureInfoInputTester.Clear(true);
            ApplySettingsToValues();
            InitializeCollection();

            UpdateCreatureListings();
            creatureBoxListView.Clear();
            Properties.Settings.Default.LastSaveFile = null;
            _currentFileName = null;
            _fileSync.ChangeFile(_currentFileName);
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
                LoadCollectionFile(_currentFileName, true, true);
            }
        }

        /// <summary>
        /// Recalculate all the stat values of all creatures. Should be done after multipliers were changed or creatures are loaded.
        /// </summary>
        private void RecalculateAllCreaturesValues()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = _creatureCollection.creatures.Count();
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
            if (!add && _collectionDirty)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to load without saving first?", "Discard Changes?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = $"ASB Collection Files (*{CollectionFileExtension}; *.xml)|*{CollectionFileExtension};*.xml"
                        + $"|ASB Collection File (*{CollectionFileExtension})|*{CollectionFileExtension}"
                        + "|Old ASB Collection File(*.xml)| *.xml"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LoadCollectionFile(dlg.FileName, add);
                }
            }
        }

        /// <summary>
        /// Save the current collection under its file. If it has no file, use saveAs.
        /// </summary>
        private void SaveCollection()
        {
            if (string.IsNullOrEmpty(_currentFileName))
            {
                SaveNewCollection();
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
                Filter = $"Creature Collection File (*{CollectionFileExtension})|*{CollectionFileExtension}"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _currentFileName = dlg.FileName;
                    _fileSync.ChangeFile(_currentFileName);
                    SaveCollectionToFileName(_currentFileName);
                }
            }
        }

        private void SaveCollectionToFileName(string filePath)
        {
            // remove expired timers if setting is set
            if (Properties.Settings.Default.DeleteExpiredTimersOnSaving)
                timerList1.DeleteAllExpiredTimers(false, false);

            // Wait until the file is writable
            const int numberOfRetries = 5;
            const int delayOnRetryBase = 500;
            bool fileSaved = false;

            var tempSavePath = filePath + ".tmp";

            for (int i = 0; i < numberOfRetries; ++i)
            {
                try
                {
                    _fileSync.JustSaving();
                    using (StreamWriter file = File.CreateText(tempSavePath))
                    {
                        JsonSerializer serializer = new JsonSerializer()
                        {
                            Formatting = Properties.Settings.Default.prettifyCollectionJson ? Formatting.Indented : Formatting.None,
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc // save all date-times as UTC, so synced files don't change the timezones
                        };
                        serializer.Serialize(file, _creatureCollection);
                    }

                    if (new FileInfo(tempSavePath).Length == 0)
                        throw new IOException("Saved file is empty and contains no data.");

                    // if saving was successful, remove outdated library file and move successfully saved file
                    File.Delete(filePath);
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
                catch (System.Runtime.Serialization.SerializationException e)
                {
                    MessageBox.Show($"Error during serialization.\nErrormessage:\n\n{e.Message}" + (e.InnerException == null ? string.Empty : $"\n\nInnerException:{e.InnerException.Message}"),
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show($"Error during serialization.\nErrormessage:\n\n{e.Message}" + (e.InnerException == null ? string.Empty : $"\n\nInnerException:{e.InnerException.Message}"),
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }

            if (fileSaved)
                SetCollectionChanged(false);
            else
                MessageBox.Show($"This file couldn\'t be saved:\n{filePath}\nMaybe the file is used by another application.", $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Loads the given creature collection file.
        /// </summary>
        /// <param name="filePath">File that contains the collection</param>
        /// <param name="keepCurrentCreatures">add the creatures of the loaded file to the current ones</param>
        /// <param name="keepCurrentSelections">don't change the species selection or tab, use if a synchronized library is loaded</param>
        /// <returns></returns>
        private bool LoadCollectionFile(string filePath, bool keepCurrentCreatures = false, bool keepCurrentSelections = false)
        {
            Species selectedSpecies = speciesSelector1.SelectedSpecies;
            Species selectedLibrarySpecies = listBoxSpeciesLib.SelectedItem as Species;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show($"Save file with name \"{filePath}\" does not exist!", $"File not found - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            _creatureCollection.ModList = mods;

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
                               || !Version.TryParse(CreatureCollection.CURRENT_FORMAT_VERSION, out Version currentVersion)
                               || ccVersion > currentVersion)
                            {
                                throw new FormatException("Unhandled format version");
                            }
                            _creatureCollection = readCollection;
                        }
                        else
                        {
                            MessageBox.Show($"Error while trying to read the library-file\n{filePath}\n\n{errorMessage}",
                                    $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                catch (FormatException)
                {
                    // This FormatVersion is not understood, abort
                    MessageBox.Show($"This library format is unsupported in this version of ARK Smart Breeding." +
                            "\n\nTry updating to a newer version.",
                            $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if ((DateTime.Now - Properties.Settings.Default.lastUpdateCheck).TotalMinutes < 10)
                        CheckForUpdates();
                    return false;
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show($"The library-file\n{filePath}\ncouldn\'t be opened, we thought you should know.\nErrormessage:\n\n{e.Message}" + (e.InnerException == null ? string.Empty : $"\n\nInnerException:\n\n{e.InnerException.Message}"),
                            $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    _creatureCollection = new CreatureCollection();
                    return false;
                }
            }
            if (_creatureCollection.ModValueReloadNeeded
                && !LoadModValuesOfCollection(_creatureCollection, false, false))
            {
                _creatureCollection = new CreatureCollection();
                return false;
            }

            if (_creatureCollection.serverMultipliers == null)
            {
                _creatureCollection.serverMultipliers = previouslyLoadedCreatureCollection.serverMultipliers ?? Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL);
            }

            if (speciesSelector1.LastSpecies != null && speciesSelector1.LastSpecies.Length > 0)
            {
                tamingControl1.SetSpecies(Values.V.SpeciesByBlueprint(speciesSelector1.LastSpecies[0]));
            }

            _creatureCollection.FormatVersion = CreatureCollection.CURRENT_FORMAT_VERSION;

            ApplySettingsToValues();

            bool creatureWasAdded = false;

            if (keepCurrentCreatures)
            {
                creatureWasAdded = previouslyLoadedCreatureCollection.MergeCreatureList(_creatureCollection.creatures);
                _creatureCollection = previouslyLoadedCreatureCollection;
            }
            else
            {
                _currentFileName = filePath;
                _fileSync.ChangeFile(_currentFileName);
                creatureBoxListView.Clear();
            }

            _creatureCollection.DeletedCreatureGuids = null; // the info was processed and is no longer needed.

            // remove creature entries without species information. some outdated and invisible entries can exist with that. the blueprintInfo is not deleted with the current version, so no new such entries should appear.
            _creatureCollection.creatures = _creatureCollection.creatures
                .Where(c => !string.IsNullOrEmpty(c.speciesBlueprint)).ToList();

            InitializeCollection(keepCurrentSelections);

            _filterListAllowed = false;

            SetCollectionChanged(creatureWasAdded); // setCollectionChanged only if there really were creatures added from the old library to the just opened one

            ///// creatures loaded.

            // calculate creature values
            RecalculateAllCreaturesValues();

            // set flags for all creatures. this is needed for backwards compatibility (added 05/2020)
            foreach (Creature c in _creatureCollection.creatures) c.InitializeFlags();

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
            if (selectedLibrarySpecies != null)
                listBoxSpeciesLib.SelectedItem = selectedLibrarySpecies;
            else if (Properties.Settings.Default.LibrarySelectSelectedSpeciesOnLoad)
                listBoxSpeciesLib.SelectedItem = speciesSelector1.SelectedSpecies;

            _filterListAllowed = true;
            FilterLibRecalculate();

            // apply last sorting
            listViewLibrary.Sort();

            UpdateTempCreatureDropDown();

            Properties.Settings.Default.LastSaveFile = filePath;
            _lastAutoSaveBackup = DateTime.Now.AddMinutes(-10);

            return true;
        }

        /// <summary>
        /// Call if the collection has changed and needs to be saved.
        /// </summary>
        /// <param name="changed">is the collection changed?</param>
        /// <param name="species">set to a specific species if only this species needs updates in the pedigree / breeding-planner. Set to null if no species needs updates</param>
        private void SetCollectionChanged(bool changed, Species species = null)
        {
            if (changed)
            {
                if (species == null || pedigree1.SelectedSpecies == species)
                    pedigree1.PedigreeNeedsUpdate = true;
                if (species == null || breedingPlan1.CurrentSpecies == species)
                    breedingPlan1.breedingPlanNeedsUpdate = true;
            }

            if (_autoSave && changed)
            {
                // save changes automatically
                if (!string.IsNullOrEmpty(_currentFileName) && _autoSaveMinutes > 0 && (DateTime.Now - _lastAutoSaveBackup).TotalMinutes > _autoSaveMinutes && FileService.IsValidJsonFile(_currentFileName))
                {
                    string filenameWOExt = Path.GetFileNameWithoutExtension(_currentFileName);
                    string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    string backupFileName = filenameWOExt + "_backup_" + timeStamp + CollectionFileExtension;
                    string backupFilePath = Path.Combine(Path.GetDirectoryName(_currentFileName), backupFileName);
                    File.Copy(_currentFileName, backupFilePath);
                    _lastAutoSaveBackup = DateTime.Now;
                    // delete oldest backupfile if more than a certain number
                    var directory = new DirectoryInfo(Path.GetDirectoryName(_currentFileName));
                    var oldBackupfiles = directory.GetFiles()
                            .Where(f => f.Name.Length == backupFileName.Length &&
                                    f.Name.Substring(0, filenameWOExt.Length + 8) == filenameWOExt + "_backup_")
                            .OrderByDescending(f => f.LastWriteTime)
                            .Skip(3)
                            .ToList();
                    foreach (FileInfo f in oldBackupfiles)
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
                }

                // save changes
                SaveCollection();
                return; // function is called soon again from savecollection()
            }
            _collectionDirty = changed;
            string fileName = string.IsNullOrEmpty(_currentFileName) ? null : Path.GetFileName(_currentFileName);
            Text = $"ARK Smart Breeding{(string.IsNullOrEmpty(fileName) ? string.Empty : " - " + fileName)}{(changed ? " *" : "")}";
            openFolderOfCurrentFileToolStripMenuItem.Enabled = !string.IsNullOrEmpty(_currentFileName);
        }

        /// <summary>
        /// Saves a file with the current library and the currently entered values in the extractor for issue reporting.
        /// </summary>
        private void SaveDebugFile()
        {
            // get temp folder for zipping
            var tempFolder = FileService.GetTempDirectory();
            var timeStamp = $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss}";
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
            ZipFile.CreateFromDirectory(tempFolder, tempZipFilePath);

            // remove temp library file and folder
            FileService.TryDeleteFile(tempFilePath);
            FileService.TryDeleteDirectory(tempFolder);

            // copy zip file to clipboard
            Clipboard.SetFileDropList(new StringCollection { tempZipFilePath });

            // display info that debug file is in clipboard
            SetMessageLabelText("File with the current library and the values in the extractor has been copied to the clipboard. You can add this file to an issue report.", MessageBoxIcon.Information);
        }

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
                toolStripCBTempCreatures.SelectedIndex = toolStripCBTempCreatures.Items.Count - 1;
                ExtractLevels();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading debug library dump\n{ex.Message}", $"Error loading zipped library dump - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}

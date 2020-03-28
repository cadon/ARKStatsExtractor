﻿using ARKBreedingStats.Library;
using ARKBreedingStats.mods;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        private const string COLLECTION_FILE_EXTENSION = ".asb";

        private void NewCollection()
        {
            if (collectionDirty
                && MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                        "are you sure you want to discard your changes and create a new Library without saving?",
                        "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (creatureCollection.modIDs?.Any() ?? false)
            {
                // if old collection had additionalValues, load the original ones to reset all modded values
                var (statValuesLoaded, _) = LoadStatAndKibbleValues(applySettings: false);
                if (!statValuesLoaded)
                {
                    MessageBox.Show("Couldn't load stat values. Please redownload the application.", "Error while loading the stat-values", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (creatureCollection.serverMultipliers == null)
                creatureCollection.serverMultipliers = Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL);
            // use previously used multipliers again in the new file
            ServerMultipliers oldMultipliers = creatureCollection.serverMultipliers;

            creatureCollection = new CreatureCollection
            {
                serverMultipliers = oldMultipliers,
                ModList = new List<Mod>()
            };
            creatureCollection.FormatVersion = CreatureCollection.CURRENT_FORMAT_VERSION;
            pedigree1.Clear();
            breedingPlan1.Clear();
            ApplySettingsToValues();
            InitializeCollection();

            UpdateCreatureListings();
            creatureBoxListView.Clear();
            Properties.Settings.Default.LastSaveFile = "";
            Properties.Settings.Default.LastImportFile = "";
            currentFileName = "";
            fileSync.ChangeFile(currentFileName);
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
                LoadCollectionFile(currentFileName, true, true);
            }
        }

        /// <summary>
        /// Recalculate all the stat values of all creatures. Should be done after multipliers were changed or creatures are loaded.
        /// </summary>
        private void RecalculateAllCreaturesValues()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = creatureCollection.creatures.Count();
            toolStripProgressBar1.Visible = true;
            int? levelStep = creatureCollection.getWildLevelStep();
            foreach (Creature c in creatureCollection.creatures)
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
            if (!add && collectionDirty)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to load without saving first?", "Discard Changes?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = $"ASB Collection Files (*{COLLECTION_FILE_EXTENSION}; *.xml)|*{COLLECTION_FILE_EXTENSION};*.xml"
                        + $"|ASB Collection File (*{COLLECTION_FILE_EXTENSION})|*{COLLECTION_FILE_EXTENSION}"
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
            if (string.IsNullOrEmpty(currentFileName))
            {
                SaveNewCollection();
            }
            else
            {
                SaveCollectionToFileName(currentFileName);
            }
        }

        private void SaveNewCollection()
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = $"Creature Collection File (*{COLLECTION_FILE_EXTENSION})|*{COLLECTION_FILE_EXTENSION}"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    currentFileName = dlg.FileName;
                    fileSync.ChangeFile(currentFileName);
                    SaveCollectionToFileName(currentFileName);
                }
            }
        }

        private void SaveCollectionToFileName(string filePath)
        {
            // remove expired timers if setting is set
            if (Properties.Settings.Default.DeleteExpiredTimersOnSaving)
                timerList1.DeleteAllExpiredTimers(false, false);

            // Wait until the file is writeable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;
            bool fileSaved = false;
            FileStream fileStream = null;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    fileSync.JustSaving();
                    using (StreamWriter file = File.CreateText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer()
                        {
                            Formatting = Properties.Settings.Default.prettifyCollectionJson ? Formatting.Indented : Formatting.None,
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc // save all date-times as UTC, so synced files don't change the timezones
                        };
                        serializer.Serialize(file, creatureCollection);
                    }

                    fileSaved = true;
                    Properties.Settings.Default.LastSaveFile = filePath;

                    break; // when file is saved, break
                }
                catch (IOException)
                {
                    // if file is not saveable
                    Thread.Sleep(delayOnRetry);
                }
                catch (System.Runtime.Serialization.SerializationException e)
                {
                    MessageBox.Show($"Error during serialization.\nErrormessage:\n\n{e.Message}" + (e.InnerException == null ? string.Empty : $"\n\nInnerException:{e.InnerException.Message}"),
                        "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show($"Error during serialization.\nErrormessage:\n\n{e.Message}" + (e.InnerException == null ? string.Empty : $"\n\nInnerException:{e.InnerException.Message}"),
                        "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                finally
                {
                    fileStream?.Close();
                }
            }

            if (fileSaved)
                SetCollectionChanged(false);
            else
                MessageBox.Show($"This file couldn\'t be saved:\n{filePath}\nMaybe the file is used by another application.", "Error during saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Loads the given creature collection file.
        /// </summary>
        /// <param name="filePath">File that contains the collection</param>
        /// <param name="keepCurrentCreatures">add the creatures of the loaded file to the current ones</param>
        /// <param name="keepCurrentSelections">don't change the species selection or tab</param>
        /// <returns></returns>
        private bool LoadCollectionFile(string filePath, bool keepCurrentCreatures = false, bool keepCurrentSelections = false)
        {
            Species selectedSpecies = speciesSelector1.SelectedSpecies;
            Species selectedlibrarySpecies = listBoxSpeciesLib.SelectedItem as Species;

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Save file with name \"{filePath}\" does not exist!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            List<Creature> oldCreatures = null;
            if (keepCurrentCreatures)
                oldCreatures = creatureCollection.creatures;

            // for the case the collectionfile has no multipliers, keep the current ones
            ServerMultipliers oldMultipliers = creatureCollection.serverMultipliers;

            // Wait until the file is readable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;

            FileStream fileStream = null;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
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
                                            "Mod values needed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                        if (Values.V.loadedModsHash != CreatureCollection.CalculateModListHash(new List<Mod>()))
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
                                        + "Do you want to load the library and risk losing creatures?", "Unknown mod-file",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                    return false;
                            }

                            creatureCollection = oldLibraryFormat.FormatConverter.ConvertXml2Asb(creatureCollectionOld, filePath);
                            creatureCollection.ModList = mods;

                            if (creatureCollection == null) throw new Exception("Conversion failed");

                            string fileNameWOExt = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                            // check if new fileName is not yet existing
                            filePath = fileNameWOExt + COLLECTION_FILE_EXTENSION;
                            if (File.Exists(filePath))
                            {
                                int fi = 2;
                                while (File.Exists(fileNameWOExt + "_" + fi + COLLECTION_FILE_EXTENSION)) fi++;
                                filePath = fileNameWOExt + "_" + fi + COLLECTION_FILE_EXTENSION;
                            }

                            // save converted library
                            SaveCollectionToFileName(filePath);
                        }
                    }
                    else
                    {
                        // new json-format
                        CreatureCollection tmpCC;
                        using (StreamReader file = File.OpenText(filePath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            tmpCC = (CreatureCollection)serializer.Deserialize(file, typeof(CreatureCollection));
                        }
                        if (!Version.TryParse(tmpCC.FormatVersion, out Version ccVersion)
                           || !Version.TryParse(CreatureCollection.CURRENT_FORMAT_VERSION, out Version currentVersion)
                           || ccVersion > currentVersion)
                        {
                            throw new FormatException("Unhandled format version");
                        }
                        creatureCollection = tmpCC;
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
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show($"The library-file\n{filePath}\ncouldn\'t be opened, we thought you should know.\nErrormessage:\n\n{e.Message}" + (e.InnerException == null ? string.Empty : $"\n\nInnerException:\n\n{e.InnerException.Message}"),
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                finally
                {
                    fileStream?.Close();
                }
            }

            if (creatureCollection.ModValueReloadNeeded)
            {
                // load original multipliers if they were changed
                if (!LoadStatAndKibbleValues(false).statValuesLoaded)
                {
                    creatureCollection = new CreatureCollection();
                    return false;
                }
            }
            if (creatureCollection.ModValueReloadNeeded
                && !LoadModValuesOfCollection(creatureCollection, false, false))
            {
                creatureCollection = new CreatureCollection();
                return false;
            }

            if (creatureCollection.serverMultipliers == null)
            {
                creatureCollection.serverMultipliers = oldMultipliers ?? Values.V.serverMultipliersPresets.GetPreset(ServerMultipliersPresets.OFFICIAL);
            }

            if (speciesSelector1.LastSpecies != null && speciesSelector1.LastSpecies.Length > 0)
            {
                tamingControl1.SetSpecies(Values.V.SpeciesByBlueprint(speciesSelector1.LastSpecies[0]));
            }

            creatureCollection.FormatVersion = CreatureCollection.CURRENT_FORMAT_VERSION;

            ApplySettingsToValues();

            bool creatureWasAdded = false;

            if (keepCurrentCreatures)
                creatureWasAdded = creatureCollection.MergeCreatureList(oldCreatures);
            else
            {
                currentFileName = filePath;
                fileSync.ChangeFile(currentFileName);
                creatureBoxListView.Clear();
            }

            creatureCollection.DeletedCreatureGuids = null; // no longer needed

            InitializeCollection();

            filterListAllowed = false;
            SetLibraryFilter("Dead", creatureCollection.showFlags.HasFlag(CreatureFlags.Dead));
            SetLibraryFilter("Unavailable", creatureCollection.showFlags.HasFlag(CreatureFlags.Unavailable));
            SetLibraryFilter("Neutered", creatureCollection.showFlags.HasFlag(CreatureFlags.Neutered));
            SetLibraryFilter("Obelisk", creatureCollection.showFlags.HasFlag(CreatureFlags.Obelisk));
            SetLibraryFilter("Cryopod", creatureCollection.showFlags.HasFlag(CreatureFlags.Cryopod));
            SetLibraryFilter("Mutated", creatureCollection.showFlags.HasFlag(CreatureFlags.Mutated));
            checkBoxUseFiltersInTopStatCalculation.Checked = creatureCollection.useFiltersInTopStatCalculation;

            SetCollectionChanged(creatureWasAdded); // setCollectionChanged only if there really were creatures added from the old library to the just opened one

            ///// creatures loaded.

            // calculate creature values
            RecalculateAllCreaturesValues();

            if (!keepCurrentSelections && creatureCollection.creatures.Any())
                tabControlMain.SelectedTab = tabPageLibrary;

            creatureBoxListView.CreatureCollection = creatureCollection;

            UpdateCreatureListings();

            // set global species that was set before loading
            if (selectedSpecies != null
                && creatureCollection.creatures.Any(c => c.Species != null && c.Species.Equals(selectedSpecies))
                )
            {
                speciesSelector1.SetSpecies(selectedSpecies);
            }
            else if (creatureCollection.creatures.Any())
                speciesSelector1.SetSpecies(creatureCollection.creatures[0].Species);

            // set library species to what it was before loading
            if (selectedlibrarySpecies == null
                || !creatureCollection.creatures.Any(c => c.Species != null && c.Species.Equals(selectedlibrarySpecies))
                )
                selectedlibrarySpecies = speciesSelector1.SelectedSpecies;
            if (selectedlibrarySpecies != null)
                listBoxSpeciesLib.SelectedIndex = listBoxSpeciesLib.Items.IndexOf(selectedlibrarySpecies);

            filterListAllowed = true;
            FilterLib();

            // apply last sorting
            listViewLibrary.Sort();

            UpdateTempCreatureDropDown();

            Properties.Settings.Default.LastSaveFile = filePath;
            lastAutoSaveBackup = DateTime.Now.AddMinutes(-10);

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
                if (species == null || pedigree1.creature != null && pedigree1.creature.Species == species)
                    pedigreeNeedsUpdate = true;
                if (species == null || breedingPlan1.CurrentSpecies == species)
                    breedingPlan1.breedingPlanNeedsUpdate = true;
            }

            if (autoSave && changed)
            {
                // save changes automatically
                if (!string.IsNullOrEmpty(currentFileName) && autoSaveMinutes > 0 && (DateTime.Now - lastAutoSaveBackup).TotalMinutes > autoSaveMinutes)
                {
                    string filenameWOExt = Path.GetFileNameWithoutExtension(currentFileName);
                    string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    string backupFileName = filenameWOExt + "_backup_" + timeStamp + COLLECTION_FILE_EXTENSION;
                    string backupFilePath = Path.Combine(Path.GetDirectoryName(currentFileName), backupFileName);
                    File.Copy(currentFileName, backupFilePath);
                    lastAutoSaveBackup = DateTime.Now;
                    // delete oldest backupfile if more than a certain number
                    var directory = new DirectoryInfo(Path.GetDirectoryName(currentFileName));
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
            collectionDirty = changed;
            string fileName = Path.GetFileName(currentFileName);
            Text = $"ARK Smart Breeding{(string.IsNullOrEmpty(fileName) ? "" : " - " + fileName)}{(changed ? " *" : "")}";
            openFolderOfCurrentFileToolStripMenuItem.Enabled = !string.IsNullOrEmpty(currentFileName);
        }
    }
}

using ARKBreedingStats.species;
using ARKBreedingStats.values;

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
        private void newCollection()
        {
            if (collectionDirty)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, " +
                        "are you sure you want to discard your changes and create a new Library without saving?",
                        "Discard Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }

            if (creatureCollection.additionalValues?.Length > 0)
            {
                // if old collection had additionalValues, load the original ones.
                Values.V.loadValues();
                if (speechRecognition != null)
                    speechRecognition.updateNeeded = true;
            }

            if (creatureCollection.serverMultipliers == null)
                creatureCollection.serverMultipliers = Values.V.serverMultipliersPresets.GetPreset("official");
            // use previously used multipliers again in the new file
            ServerMultipliers oldMultipliers = creatureCollection.serverMultipliers;

            creatureCollection = new CreatureCollection
            {
                serverMultipliers = oldMultipliers
            };
            creatureCollection.FormatVersion = CreatureCollection.CURRENT_FORMAT_VERSION;
            pedigree1.Clear();
            breedingPlan1.Clear();
            applySettingsToValues();
            initializeCollection();

            updateCreatureListings();
            creatureBoxListView.Clear();
            Properties.Settings.Default.LastSaveFile = "";
            Properties.Settings.Default.LastImportFile = "";
            currentFileName = "";
            fileSync.changeFile(currentFileName);
            setCollectionChanged(false);
        }

        delegate void collectionChangedCallback();

        private void collectionChanged()
        {
            if (creatureBoxListView.InvokeRequired)
            {
                collectionChangedCallback d = collectionChanged;
                Invoke(d);
            }
            else
            {
                loadCollectionFile(currentFileName, true, true);
            }
        }

        private void recalculateAllCreaturesValues()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = creatureCollection.creatures.Count();
            toolStripProgressBar1.Visible = true;
            int? levelStep = creatureCollection.getWildLevelStep();
            foreach (Creature c in creatureCollection.creatures)
            {
                c.recalculateCreatureValues(levelStep);
                toolStripProgressBar1.Value++;
            }
            toolStripProgressBar1.Visible = false;
        }

        private void loadCollection(bool add = false)
        {
            if (!add && collectionDirty)
            {
                if (MessageBox.Show("Your Creature Collection has been modified since it was last saved, are you sure you want to load without saving first?", "Discard Changes?", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Creature Collection File (*.xml)|*.xml"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Prevents the ".xml.old" files from being loaded.
                if (dlg.FileName.EndsWith(".xml"))
                    loadCollectionFile(dlg.FileName, add);
                else
                    MessageBox.Show("Invalid Library file selected. Please select a valid Library file.");
            }
        }

        private void saveCollection()
        {
            if (currentFileName == "")
                saveNewCollection();
            else
            {
                saveCollectionToFileName(currentFileName);
            }
        }

        private void saveNewCollection()
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Creature Collection File (*.xml)|*.xml"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                currentFileName = dlg.FileName;
                fileSync.changeFile(currentFileName);
                saveCollectionToFileName(currentFileName);
            }
        }

        private void saveCollectionToFileName(string fileName)
        {
            // Wait until the file is writeable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;
            bool fileSaved = false;
            FileStream file = null;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    file = File.Create(fileName);
                    XmlSerializer writer = new XmlSerializer(typeof(CreatureCollection));
                    fileSync.justSaving();
                    writer.Serialize(file, creatureCollection);
                    fileSaved = true;
                    Properties.Settings.Default.LastSaveFile = fileName;

                    break; // when file is saved, break
                }
                catch (IOException)
                {
                    // if file is not saveable
                    Thread.Sleep(delayOnRetry);
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show($"Error during serialization.\nErrormessage:\n\n{e.Message}", "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    file?.Close();
                }
            }

            if (fileSaved)
                setCollectionChanged(false);
            else
                MessageBox.Show($"This file couldn\'t be saved:\n{fileName}\nMaybe the file is used by another application.", "Error during saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Loads the given creatureCollection file.
        /// </summary>
        /// <param name="fileName">File that contains the collection</param>
        /// <param name="keepCurrentCreatures">add the creatures of the loaded file to the current ones</param>
        /// <param name="keepCurrentSelections">don't change the species selection or tab</param>
        /// <returns></returns>
        private bool loadCollectionFile(string fileName, bool keepCurrentCreatures = false, bool keepCurrentSelections = false)
        {
            string selectedSpeciesInLibrary = listBoxSpeciesLib.SelectedItem.ToString();

            XmlSerializer reader = new XmlSerializer(typeof(CreatureCollection));

            if (!File.Exists(fileName))
            {
                MessageBox.Show($"Save file with name \"{fileName}\" does not exist!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            List<Creature> oldCreatures = null;
            if (keepCurrentCreatures)
                oldCreatures = creatureCollection.creatures;

            FileStream file = null;

            // for the case the collectionfile has no multipliers, keep the current ones
            ServerMultipliers oldMultipliers = creatureCollection.serverMultipliers;

            // Wait until the file is readable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    file = File.OpenRead(fileName);
                    creatureCollection = (CreatureCollection)reader.Deserialize(file);
                    break;
                }
                catch (IOException)
                {
                    // if file is not readable
                    Thread.Sleep(delayOnRetry);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"The library-file\n{fileName}\ncouldn\'t be opened, we thought you should know.\nErrormessage:\n\n{e.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                finally
                {
                    file?.Close();
                }
            }

            if (Values.V.loadedModsHash != 0 && Values.V.loadedModsHash != creatureCollection.modListHash)
            {
                // load original multipliers if they were changed
                if (!Values.V.loadValues())
                {
                    creatureCollection = new CreatureCollection();
                    return false;
                }

                if (speechRecognition != null)
                    speechRecognition.updateNeeded = true;
            }
            if (creatureCollection.ModValueReloadNeeded
                && !loadModValuesOfLibrary(creatureCollection, false, false))
            {
                creatureCollection = new CreatureCollection();
                return false;
            }

            if (creatureCollection.serverMultipliers == null)
            {
                creatureCollection.serverMultipliers = oldMultipliers ?? Values.V.serverMultipliersPresets.GetPreset("official");
            }

            if (speciesSelector1.LastSpecies != null && speciesSelector1.LastSpecies.Length > 0)
            {
                tamingControl1.SetSpecies(Values.V.speciesByBlueprint(speciesSelector1.LastSpecies[0]));
            }

            if (!CheckLibraryVersionAndConvert(creatureCollection, libraryFilePath: fileName))
            {
                creatureCollection = new CreatureCollection();
                return false; // format unknown
            }

            creatureCollection.FormatVersion = CreatureCollection.CURRENT_FORMAT_VERSION;

            applySettingsToValues();

            bool creatureWasAdded = false;

            if (keepCurrentCreatures)
                creatureWasAdded = creatureCollection.mergeCreatureList(oldCreatures);
            else
            {
                currentFileName = fileName;
                fileSync.changeFile(currentFileName);
                creatureBoxListView.Clear();
            }

            initializeCollection();

            filterListAllowed = false;
            setLibraryFilter("Dead", creatureCollection.showDeads);
            setLibraryFilter("Unavailable", creatureCollection.showUnavailable);
            setLibraryFilter("Neutered", creatureCollection.showNeutered);
            setLibraryFilter("Obelisk", creatureCollection.showObelisk);
            setLibraryFilter("Cryopod", creatureCollection.showCryopod);
            setLibraryFilter("Mutated", creatureCollection.showMutated);
            checkBoxUseFiltersInTopStatCalculation.Checked = creatureCollection.useFiltersInTopStatCalculation;
            filterListAllowed = true;

            setCollectionChanged(creatureWasAdded); // setCollectionChanged only if there really were creatures added from the old library to the just opened one

            ///// creatures loaded.

            // calculate creature values
            recalculateAllCreaturesValues();

            if (!keepCurrentSelections && creatureCollection.creatures.Count > 0)
                tabControlMain.SelectedTab = tabPageLibrary;

            creatureBoxListView.maxDomLevel = creatureCollection.maxDomLevel;

            updateCreatureListings();

            // apply last sorting
            listViewLibrary.Sort();

            updateTempCreatureDropDown();

            Properties.Settings.Default.LastSaveFile = fileName;
            lastAutoSaveBackup = DateTime.Now.AddMinutes(-10);

            return true;
        }

        /// <summary>
        /// Call if the collection has changed and needs to be saved.
        /// </summary>
        /// <param name="changed">is the collection changed?</param>
        /// <param name="species">set to a specific species if only this species needs updates in the pedigree / breeding-planner. Set to null if no species needs updates</param>
        private void setCollectionChanged(bool changed, Species species = null)
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
                if (currentFileName != "" && autoSaveMinutes > 0 && (DateTime.Now - lastAutoSaveBackup).TotalMinutes > autoSaveMinutes)
                {
                    string filenameWOExt = Path.GetFileNameWithoutExtension(currentFileName);
                    File.Copy(currentFileName, Path.GetDirectoryName(currentFileName) + "\\" + filenameWOExt + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml");
                    lastAutoSaveBackup = DateTime.Now;
                    // delete oldest backupfile if more than a certain number
                    var directory = new DirectoryInfo(Path.GetDirectoryName(currentFileName));
                    var oldBackupfiles = directory.GetFiles()
                            .Where(f => f.Name.Length > filenameWOExt.Length + 8 &&
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
                saveCollection();
                return; // function is called soon again from savecollection()
            }
            collectionDirty = changed;
            string fileName = Path.GetFileName(currentFileName);
            Text = $"ARK Smart Breeding{(fileName.Length > 0 ? " - " + fileName : "")}{(changed ? " *" : "")}";
        }
    }
}

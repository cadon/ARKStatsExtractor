using ARKBreedingStats.Library;
using ARKBreedingStats.settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARKBreedingStats.NamePatterns;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class Form1
    {
        private void OpenImportExportForm(object sender, EventArgs e)
        {
            var loc = (ATImportExportedFolderLocation)((ToolStripMenuItem)sender).Tag;
            if (string.IsNullOrWhiteSpace(loc.FolderPath))
            {
                if (MessageBox.Show("There is no valid folder set in the settings.\n\nOpen the settings-page?",
                        "No valid export-folder set", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    OpenSettingsDialog(Settings.SettingsTabPages.ExportedImport);
                }
            }
            else
            {
                ShowExportedCreatureListControl();
                _exportedCreatureList.ownerSuffix = loc.OwnerSuffix;
                _exportedCreatureList.LoadFilesInFolder(loc.FolderPath);
            }
        }

        /// <summary>
        /// Show window with exported creatures of the default folder.
        /// If no default window is set, ask to open the according settings-page.
        /// </summary>
        private void ImportExportedCreaturesDefaultFolder()
        {
            if (Utils.GetFirstImportExportFolder(out string folder))
            {
                ShowExportedCreatureListControl();
                _exportedCreatureList.LoadFilesInFolder(folder);
            }
            else if (
                MessageBox.Show("There is no valid folder set where the exported creatures are located. Set this folder in the settings.\n\nOpen the settings-page?",
                        "No default export-folder set", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                OpenSettingsDialog(Settings.SettingsTabPages.ExportedImport);
            }
        }

        private void ImportAllCreaturesInSelectedFolder(object sender, EventArgs e)
        {
            ShowExportedCreatureListControl();
            _exportedCreatureList.chooseFolderAndImport();
        }

        private void btImportLastExported_Click(object sender, EventArgs e)
        {
            ImportLastExportedCreature();
        }

        /// <summary>
        /// Imports the latest file in the default export-folder.
        /// </summary>
        private void ImportLastExportedCreature()
        {
            if (!Utils.GetFirstImportExportFolder(out string folder))
            {
                if (MessageBox.Show("There is no folder set where the exported creatures are located, or the set folder does not exist. Set this folder in the settings. " +
                                    "Usually the folder path ends with\n" + @"…\ARK\ShooterGame\Saved\DinoExports\<ID>" + "\n\nOpen the settings-page?",
                        $"No default export-folder set - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    OpenSettingsDialog(Settings.SettingsTabPages.ExportedImport);
                }
                return;
            }

            var files = Directory.GetFiles(folder);
            if (files.Length == 0)
            {
                // some users forget to select the id folder where the export files are located. Check if that's the case
                FileInfo lastExportFile = null;
                if (Path.GetFileName(folder) == "DinoExports")
                {
                    // check subfolders for export files
                    var subFolders = Directory.GetDirectories(folder);
                    foreach (var sf in subFolders)
                    {
                        var d = new DirectoryInfo(sf);
                        var fs = d.GetFiles("*.ini");
                        if (!fs.Any()) continue;
                        var expFile = fs.OrderByDescending(f => f.LastWriteTime).First();
                        if (lastExportFile == null || expFile.LastWriteTime > lastExportFile.LastWriteTime)
                            lastExportFile = expFile;
                    }
                }

                if (lastExportFile == null)
                {
                    MessageBoxes.ShowMessageBox(
                        $"No exported creature-file found in the set folder\n{folder}\nYou have to export a creature first ingame.\n\n" +
                        "You may also want to check the set folder in the settings. Usually the folder path ends with\n" +
                        @"…\ARK\ShooterGame\Saved\DinoExports\<ID>",
                        $"No files found");
                    return;
                }

                if (MessageBox.Show(
                        $"No exported creature-file found in the set folder\n{folder}\n\nThere seems to be an export file in a subfolder, do you want to use this folder instead?\n{lastExportFile.DirectoryName}",
                        $"Use subfolder with export file? - {Utils.ApplicationNameVersion}",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    var exportFolders = Properties.Settings.Default.ExportCreatureFolders;
                    var firstExportFolder = ATImportExportedFolderLocation.CreateFromString(exportFolders[0]);
                    firstExportFolder.FolderPath = lastExportFile.DirectoryName;
                    exportFolders[0] = firstExportFolder.ToString();

                    ExtractExportedFileInExtractor(lastExportFile.FullName, out _, out _);
                }
                return;
            }

            var newestExportFile = files.OrderByDescending(File.GetLastWriteTime).First();

            switch (Path.GetExtension(newestExportFile))
            {
                case ".ini":
                    // ini files need to be processed by the extractor
                    ExtractExportedFileInExtractor(newestExportFile, out _, out _);
                    return;
                case ".sav":
                case ".json":
                    // export gun mod creature exports can be just added
                    var creature = ImportExportedAddIfPossible(newestExportFile);
                    // display imported creature in the extractor to allow adjustments
                    _ignoreNextMessageLabel = true;
                    EditCreatureInTester(creature);
                    return;
            }
        }

        private void ExportedCreatureList_CopyValuesToExtractor(importExported.ExportedCreatureControl exportedCreatureControl, bool addToLibraryIfUnique, bool goToLibraryTab)
        {
            tabControlMain.SelectedTab = tabPageExtractor;

            ExtractExportedFileInExtractor(exportedCreatureControl, updateParentVisuals: !addToLibraryIfUnique);

            // add to library automatically if batch-extracting exportedImported values and uniqueLevels
            if (addToLibraryIfUnique)
            {
                if (_extractor.UniqueResults)
                    AddCreatureToCollection(true, exportedCreatureControl.creatureValues.motherArkId, exportedCreatureControl.creatureValues.fatherArkId, goToLibraryTab);
                else
                    exportedCreatureControl.setStatus(importExported.ExportedCreatureControl.ImportStatus.NeedsLevelChoosing, DateTime.Now);
            }
            else
            {
                // bring main-window to front to work with the data
                BringToFront();
            }
        }

        private void ImportExportedAddIfPossible_WatcherThread(string filePath, importExported.FileWatcherExports fwe)
        {
            fwe.Watching = false;
            // wait a moment until the file is readable. why is this necessary? blocked by fileWatcher?
            System.Threading.Thread.Sleep(200);

            // moving to the archived folder can trigger another fileWatcherEvent, first check if the file is still there
            if (File.Exists(filePath))
                // fileWatcher is on another thread, invoke ui-thread to work with ui
                Invoke(new Action(delegate { ImportExportedAddIfPossible(filePath); }));

            fwe.Watching = true;
        }

        /// <summary>
        /// Import exported file. Used by a fileWatcher. Returns creature if added successfully.
        /// </summary>
        private Creature ImportExportedAddIfPossible(string filePath)
        {
            bool alreadyExists;
            bool addedToLibrary = false;
            bool uniqueExtraction = false;
            Creature creature = null;
            Creature alreadyExistingCreature = null;
            bool copiedNameToClipboard = false;
            Creature[] creaturesOfSpecies = null;

            switch (Path.GetExtension(filePath))
            {
                case ".ini":
                    var loadResult = ExtractExportedFileInExtractor(filePath, out copiedNameToClipboard, out alreadyExistingCreature);
                    if (loadResult == null) return null;
                    alreadyExists = loadResult.Value;

                    uniqueExtraction = _extractor.UniqueResults
                                           || (alreadyExists && _extractor.ValidResults);
                    Species species = speciesSelector1.SelectedSpecies;

                    if (uniqueExtraction
                        && Properties.Settings.Default.OnAutoImportAddToLibrary)
                    {
                        creature = AddCreatureToCollection(true, goToLibraryTab: Properties.Settings.Default.AutoImportGotoLibraryAfterSuccess);
                        SetMessageLabelText($"Successful {(alreadyExists ? "updated" : "added")} {creature.name} ({species.name}) of the exported file" + Environment.NewLine + filePath, MessageBoxIcon.Information, filePath);
                        addedToLibrary = true;
                    }
                    break;
                case ".sav":
                case ".json":
                    alreadyExistingCreature = ImportExportGunFiles(new[] { filePath }, out addedToLibrary, out creature, out copiedNameToClipboard);
                    alreadyExists = alreadyExistingCreature != null;
                    if (!addedToLibrary || creature == null) return null;
                    uniqueExtraction = true;
                    break;
                default: return null;
            }

            if (creature == null)
            {
                // extraction did not work automatically, user input needed, create creature file for error message
                var levelStep = _creatureCollection.getWildLevelStep();
                var species = speciesSelector1.SelectedSpecies;
                creature = GetCreatureFromInput(true, species, levelStep);
            }

            OverlayFeedbackForImport(creature, uniqueExtraction, alreadyExists, addedToLibrary, copiedNameToClipboard, out bool hasTopLevels, out bool hasNewTopLevels);

            if (addedToLibrary)
            {
                if (Properties.Settings.Default.DeleteAutoImportedFile)
                {
                    FileService.TryDeleteFile(filePath);
                }
                else if (Properties.Settings.Default.MoveAutoImportedFileToSubFolder || Properties.Settings.Default.AutoImportedExportFileRename)
                {
                    string newPath = Properties.Settings.Default.MoveAutoImportedFileToSubFolder
                        ? (string.IsNullOrEmpty(Properties.Settings.Default.ImportExportedArchiveFolder)
                            ? Path.Combine(Path.GetDirectoryName(filePath), "imported")
                            : Properties.Settings.Default.ImportExportedArchiveFolder)
                        : Path.GetDirectoryName(filePath);

                    if (Properties.Settings.Default.MoveAutoImportedFileToSubFolder
                        && !FileService.TryCreateDirectory(newPath, out string errorMessage))
                    {
                        MessageBoxes.ShowMessageBox($"Subfolder\n{newPath}\ncould not be created.\n{errorMessage}");
                        return null;
                    }

                    string namePattern = Properties.Settings.Default.AutoImportedExportFileRenamePattern;

                    string newFileName = Properties.Settings.Default.AutoImportedExportFileRename && !string.IsNullOrWhiteSpace(namePattern)
                        ? NamePattern.GenerateCreatureName(creature, alreadyExistingCreature,
                            creaturesOfSpecies ?? _creatureCollection.creatures.Where(c => c.Species == creature.Species).ToArray(),
                            null, null,
                            _customReplacingNamingPattern, false, -1, false, namePattern, libraryCreatureCount: _creatureCollection.GetTotalCreatureCount())
                        : Path.GetFileName(filePath);

                    // remove invalid characters
                    var invalidCharacters = Path.GetInvalidFileNameChars();
                    foreach (var invalidChar in invalidCharacters)
                        newFileName = newFileName.Replace(invalidChar, '_');

                    string newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);
                    string newFileNameExtension = Path.GetExtension(newFileName);
                    string newFilePath = Path.Combine(newPath, newFileName);
                    int fileSuffix = 1;
                    while (File.Exists(newFilePath))
                    {
                        newFilePath = Path.Combine(newPath, $"{newFileNameWithoutExtension}_{++fileSuffix}{newFileNameExtension}");
                    }

                    if (FileService.TryMoveFile(filePath, newFilePath))
                    {
                        _messageLabelPath = newFilePath;
                        SetMessageLabelLink(newFilePath);
                    }
                }
            }
            else if (!uniqueExtraction && copiedNameToClipboard)
            {
                // extraction failed, user might expect the name of the new creature in the clipboard
                Clipboard.SetText("Automatic extraction was not possible");
            }

            if (Properties.Settings.Default.PlaySoundOnAutoImport)
            {
                if (uniqueExtraction)
                {
                    if (alreadyExists)
                        SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Indifferent);
                    if (hasNewTopLevels)
                        SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Great);
                    else if (hasTopLevels)
                        SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Good);
                    else
                        SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Success);
                }
                else
                {
                    SoundFeedback.BeepSignal(SoundFeedback.FeedbackSounds.Failure);
                }
            }

            if (!uniqueExtraction && Properties.Settings.Default.ImportExportedBringToFrontOnIssue)
            {
                TopMost = true;
                TopMost = false;
            }

            return addedToLibrary ? creature : null;
        }

        /// <summary>
        /// Sets the name of an imported creature and copies it to the clipboard depending on the user settings.
        /// </summary>
        /// <returns>True if name was copied to clipboard</returns>
        private bool SetNameOfImportedCreature(Creature creature, Creature[] creaturesOfSpeciesIn, out Creature[] creaturesOfSpecies, Creature alreadyExistingCreature, int totalCreatureCount)
        {
            creaturesOfSpecies = creaturesOfSpeciesIn;
            if (ApplyNamingPattern(creature, alreadyExistingCreature))
            {
                // don't overwrite existing ASB creature name with empty ingame name
                if (!string.IsNullOrEmpty(alreadyExistingCreature?.name) && string.IsNullOrEmpty(creature.name))
                {
                    creature.name = alreadyExistingCreature.name;
                }
                else
                {
                    if (creaturesOfSpecies == null)
                        creaturesOfSpecies = _creatureCollection.creatures.Where(c => c.Species == creature.Species)
                            .ToArray();
                    creature.name = NamePattern.GenerateCreatureName(creature, alreadyExistingCreature, creaturesOfSpecies,
                        _topLevels.TryGetValue(creature.Species, out var topLevels) ? topLevels : null,
                        _lowestLevels.TryGetValue(creature.Species, out var lowestLevels) ? lowestLevels : null,
                        _customReplacingNamingPattern, false, 0, libraryCreatureCount: totalCreatureCount);
                    if (alreadyExistingCreature != null)
                        alreadyExistingCreature.name = creature.name; // if alreadyExistingCreature was already updated and creature is not used anymore make sure name is not lost
                }

                return CopyCreatureNameToClipboardOnImportIfSetting(creature.name);
            }

            return false;
        }

        /// <summary>
        /// Returns true if the naming pattern should be applied according to the settings.
        /// </summary>
        private bool ApplyNamingPattern(Creature creature, Creature alreadyExistingCreature) =>
            Properties.Settings.Default.applyNamePatternOnAutoImportAlways
            || (Properties.Settings.Default.applyNamePatternOnImportIfEmptyName
                && string.IsNullOrEmpty(creature.name))
            || (alreadyExistingCreature == null
                && Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures);

        /// <summary>
        /// Copies name to clipboard if the according setting is enabled. Returns true if copied.
        /// </summary>
        private bool CopyCreatureNameToClipboardOnImportIfSetting(string creatureName)
        {
            if (!Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied) return false;
            Clipboard.SetText(string.IsNullOrEmpty(creatureName)
                ? "<no name>"
                : creatureName);
            return true;
        }

        /// <summary>
        /// Give feedback in overlay for imported creature.
        /// </summary>
        private void OverlayFeedbackForImport(Creature creature, bool uniqueExtraction, bool alreadyExists, bool addedToLibrary, bool copiedNameToClipboard, out bool topLevels, out bool newTopLevels)
        {
            topLevels = false;
            newTopLevels = false;
            string infoText;
            Color textColor;
            const int colorSaturation = 200;
            if (uniqueExtraction)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{creature.Species.name} \"{creature.name}\" {(alreadyExists ? "updated in " : "added to")} the library.");
                if (addedToLibrary && copiedNameToClipboard)
                    sb.AppendLine("Name copied to clipboard.");

                for (int s = 0; s < Stats.StatsCount; s++)
                {
                    int statIndex = Stats.DisplayOrder[s];
                    if (!creature.Species.UsesStat(statIndex)) continue;

                    sb.Append($"{Utils.StatName(statIndex, true, creature.Species.statNames)}: {creature.levelsWild[statIndex]} ({creature.valuesBreeding[statIndex]})");
                    if (_statIOs[statIndex].TopLevel.HasFlag(LevelStatus.NewTopLevel))
                    {
                        sb.Append($" {Loc.S("newTopLevel")}");
                        newTopLevels = true;
                    }
                    else if (creature.topBreedingStats[statIndex])
                    {
                        sb.Append($" {Loc.S("topLevel")}");
                        topLevels = true;
                    }
                    sb.AppendLine();
                }

                infoText = sb.ToString();
                textColor = Color.FromArgb(colorSaturation, 255, colorSaturation);
            }
            else
            {
                infoText = $"Creature \"{creature.name}\" couldn't be extracted uniquely, manual level selection is necessary.";
                textColor = Color.FromArgb(255, colorSaturation, colorSaturation);
            }

            if (_overlay != null)
            {
                _overlay.SetInfoText(infoText, textColor);
                if (Properties.Settings.Default.DisplayInheritanceInOverlay)
                    _overlay.SetInheritanceCreatures(creature, creature.Mother, creature.Father);
            }
        }

        private void ExportedCreatureList_CheckGuidInLibrary(importExported.ExportedCreatureControl exportedCreatureControl)
        {
            Creature cr = _creatureCollection.creatures.FirstOrDefault(c => c.guid == exportedCreatureControl.creatureValues.guid);
            if (cr != null && !cr.flags.HasFlag(CreatureFlags.Placeholder))
                exportedCreatureControl.setStatus(importExported.ExportedCreatureControl.ImportStatus.OldImported, cr.addedToLibrary);
            else
                exportedCreatureControl.setStatus(importExported.ExportedCreatureControl.ImportStatus.NotImported, DateTime.Now);
        }

        private void llOnlineHelpExtractionIssues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Extraction-issues");
        }

        /// <summary>
        /// Displays a window with exported creature-files for import.
        /// </summary>
        private void ShowExportedCreatureListControl()
        {
            if (_exportedCreatureList == null || _exportedCreatureList.IsDisposed)
            {
                _exportedCreatureList = new importExported.ExportedCreatureList();
                _exportedCreatureList.CopyValuesToExtractor += ExportedCreatureList_CopyValuesToExtractor;
                _exportedCreatureList.CheckArkIdInLibrary += ExportedCreatureList_CheckGuidInLibrary;
                _exportedCreatureList.UpdateVisualData += UpdateVisualDataInExtractor;
                _exportedCreatureList.AddFolderToPresets += AddExportFolderToMenu;
                Utils.SetWindowRectangle(_exportedCreatureList, Properties.Settings.Default.ImportExportedFormRectangle);
                _exportedCreatureList.CheckForUnknownMods += ExportedCreatureList_CheckForUnknownMods;
            }
            _exportedCreatureList.ownerSuffix = string.Empty;
            _exportedCreatureList.Show();
            _exportedCreatureList.BringToFront();
        }

        private void AddExportFolderToMenu(string folderPath)
        {
            var folders = Properties.Settings.Default.ExportCreatureFolders?.ToList() ?? new List<string>();
            bool alreadyExists = false;
            foreach (var f in folders)
            {
                var impExpFolder = ATImportExportedFolderLocation.CreateFromString(f);
                if (impExpFolder.FolderPath == folderPath)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists) return;

            folders.Add(new ATImportExportedFolderLocation(Path.GetFileName(folderPath), null, folderPath).ToString());
            Properties.Settings.Default.ExportCreatureFolders = folders.ToArray();
            CreateImportExportedMenu();
        }

        private void UpdateVisualDataInExtractor(bool updateData)
        {
            if (!updateData)
            {
                creatureInfoInputExtractor.DontUpdateVisuals = true;
                _dontUpdateExtractorVisualData = true;
            }
            else
            {
                creatureInfoInputExtractor.DontUpdateVisuals = true;
                var colors = creatureInfoInputExtractor.RegionColors;
                creatureInfoInputExtractor.DontUpdateVisuals = false;
                _dontUpdateExtractorVisualData = false;
                creatureInfoInputExtractor.RegionColors = colors;
            }
        }

        private void ExportedCreatureList_CheckForUnknownMods(List<string> unknownSpeciesBlueprintPaths)
        {
            CheckForMissingModFiles(_creatureCollection, unknownSpeciesBlueprintPaths);
            // if mods were added, try to import the creature values again
            if (_creatureCollection.ModValueReloadNeeded
                && LoadModValuesOfCollection(_creatureCollection, true, true))
                _exportedCreatureList.LoadFilesInFolder();
        }

        /// <summary>
        /// Recreates the menu entries to import exported creatures.
        /// </summary>
        private void CreateImportExportedMenu()
        {
            importExportedCreaturesToolStripMenuItem.DropDownItems.Clear();
            if (Properties.Settings.Default.ExportCreatureFolders?.Any() == true)
            {
                foreach (string f in Properties.Settings.Default.ExportCreatureFolders)
                {
                    ATImportExportedFolderLocation aTImportExportedFolderLocation =
                        ATImportExportedFolderLocation.CreateFromString(f);
                    string menuItemHeader = string.IsNullOrEmpty(aTImportExportedFolderLocation.ConvenientName)
                        ? "<unnamed>"
                        : aTImportExportedFolderLocation.ConvenientName;
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(menuItemHeader
                                                                   + (string.IsNullOrEmpty(
                                                                       aTImportExportedFolderLocation.OwnerSuffix)
                                                                       ? string.Empty
                                                                       : " - " + aTImportExportedFolderLocation.OwnerSuffix))
                    {
                        Tag = aTImportExportedFolderLocation
                    };
                    tsmi.Click += OpenImportExportForm;
                    importExportedCreaturesToolStripMenuItem.DropDownItems.Add(tsmi);
                }

                importExportedCreaturesToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            }

            // open folder for importExport
            ToolStripMenuItem tsmif = new ToolStripMenuItem("Open folder for importing exported files");
            tsmif.Click += ImportAllCreaturesInSelectedFolder;
            importExportedCreaturesToolStripMenuItem.DropDownItems.Add(tsmif);
        }
    }
}

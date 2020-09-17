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
using ARKBreedingStats.species;

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
            if (Utils.GetFirstImportExportFolder(out string folder))
            {
                var files = Directory.GetFiles(folder);
                if (files.Length > 0)
                {
                    ExtractExportedFileInExtractor(files.OrderByDescending(f => File.GetLastWriteTime(f)).First());
                    return;
                }

                MessageBox.Show($"No exported creature-file found in the set folder\n{folder}\nYou have to export a creature first ingame.\n\n" +
                                "You may also want to check the set folder in the settings. Usually the folder is\n" +
                                @"…\Steam\steamapps\common\ARK\ShooterGame\Saved\DinoExports\<ID>",
                    $"No files found - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("There is no folder set where the exported creatures are located. Set this folder in the settings. " +
                                "Usually the folder is\n" + @"…\Steam\steamapps\common\ARK\ShooterGame\Saved\DinoExports\<ID>" + "\n\nOpen the settings-page?",
                                $"No default export-folder set - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                OpenSettingsDialog(Settings.SettingsTabPages.ExportedImport);
            }
        }

        private void ExportedCreatureList_CopyValuesToExtractor(importExported.ExportedCreatureControl exportedCreatureControl, bool addToLibraryIfUnique, bool goToLibraryTab)
        {
            tabControlMain.SelectedTab = tabPageExtractor;

            bool updateExtractorVisualKeeper = _updateExtractorVisualData;
            if (addToLibraryIfUnique)
                _updateExtractorVisualData = false;

            ExtractExportedFileInExtractor(exportedCreatureControl, updateParentVisuals: !addToLibraryIfUnique);

            if (addToLibraryIfUnique)
                _updateExtractorVisualData = updateExtractorVisualKeeper;

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
                // filewatcher is on another thread, invoke ui-thread to work with ui
                Invoke(new Action(delegate () { ImportExportedAddIfPossible(filePath); }));

            fwe.Watching = true;
        }

        /// <summary>
        /// Import exported file. Used by a fileWatcher.
        /// </summary>
        /// <param name="filePath"></param>
        private void ImportExportedAddIfPossible(string filePath)
        {
            var loadResult = ExtractExportedFileInExtractor(filePath);
            if (!loadResult.HasValue) return;

            bool alreadyExists = loadResult.Value;
            bool added = false;
            bool copyNameToClipboard = Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied
                && (Properties.Settings.Default.applyNamePatternOnImportIfEmptyName ||
                   (!alreadyExists && Properties.Settings.Default.applyNamePatternOnAutoImportForNewCreatures));
            Species species = speciesSelector1.SelectedSpecies;
            Creature creature = null;

            if (_extractor.UniqueResults
                || (alreadyExists && _extractor.ValidResults))
            {
                creature = AddCreatureToCollection(true, goToLibraryTab: false);
                SetMessageLabelText($"Successful {(alreadyExists ? "updated" : "added")} {creature.name} ({species.name}) of the exported file\n" + filePath, MessageBoxIcon.Information);
                added = true;
            }

            bool topLevels = false;
            bool newTopLevels = false;

            // give feedback in overlay
            string infoText;
            Color textColor;
            const int colorSaturation = 200;
            if (added)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{species.name} \"{creature.name}\" {(alreadyExists ? "updated in " : "added to")} the library.");
                if (copyNameToClipboard)
                    sb.AppendLine("Name copied to clipboard.");

                for (int s = 0; s < values.Values.STATS_COUNT; s++)
                {
                    int statIndex = values.Values.statsDisplayOrder[s];
                    if (!species.UsesStat(statIndex)) continue;

                    sb.Append($"{Utils.StatName(statIndex, true, species.statNames)}: { _statIOs[statIndex].LevelWild} ({_statIOs[statIndex].BreedingValue})");
                    if (_statIOs[statIndex].TopLevel == StatIOStatus.NewTopLevel)
                    {
                        sb.Append($" {Loc.S("newTopLevel")}");
                        newTopLevels = true;
                    }
                    else if (_statIOs[statIndex].TopLevel == StatIOStatus.TopLevel)
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
                infoText = $"Creature \"{creatureInfoInputExtractor.CreatureName}\" couldn't be extracted uniquely, manual level selection is necessary.";
                textColor = Color.FromArgb(255, colorSaturation, colorSaturation);
            }

            if (_overlay != null)
            {
                _overlay.SetInfoText(infoText, textColor);
                if (Properties.Settings.Default.DisplayInheritanceInOverlay && creature != null)
                    _overlay.SetInheritanceCreatures(creature, creature.Mother, creature.Father);
            }

            if (added)
            {
                if (Properties.Settings.Default.MoveAutoImportedFileToSubFolder)
                {
                    string importedPath = string.IsNullOrEmpty(Properties.Settings.Default.ImportExportedArchiveFolder)
                            ? Path.Combine(Path.GetDirectoryName(filePath), "imported")
                            : Properties.Settings.Default.ImportExportedArchiveFolder;
                    if (!FileService.TryCreateDirectory(importedPath, out string errorMessage))
                    {
                        MessageBox.Show($"Subfolder\n{importedPath}\ncould not be created.\n{errorMessage}", $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    FileService.TryMoveFile(filePath, Path.Combine(importedPath, Path.GetFileName(filePath)));
                }
                else if (Properties.Settings.Default.DeleteAutoImportedFile)
                {
                    FileService.TryDeleteFile(filePath);
                }
            }
            else if (copyNameToClipboard)
            {
                // extraction failed, user might expect the name of the new creature in the clipboard
                Clipboard.SetText("Automatic extraction was not possible");
            }

            if (Properties.Settings.Default.PlaySoundOnAutoImport)
            {
                if (added)
                {
                    if (newTopLevels)
                        Utils.BeepSignal(3);
                    else if (topLevels)
                        Utils.BeepSignal(2);
                    else
                        Utils.BeepSignal(1);
                }
                else
                {
                    Utils.BeepSignal(0);
                }
            }
        }

        private void ExportedCreatureList_CheckGuidInLibrary(importExported.ExportedCreatureControl exportedCreatureControl)
        {
            Creature cr = _creatureCollection.creatures.SingleOrDefault(c => c.guid == exportedCreatureControl.creatureValues.guid);
            if (cr != null && !cr.flags.HasFlag(CreatureFlags.Placeholder))
                exportedCreatureControl.setStatus(importExported.ExportedCreatureControl.ImportStatus.OldImported, cr.addedToLibrary);
            else
                exportedCreatureControl.setStatus(importExported.ExportedCreatureControl.ImportStatus.NotImported, DateTime.Now);
        }

        private void llOnlineHelpExtractionIssues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Extraction-issues");
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
                Utils.SetWindowRectangle(_exportedCreatureList, Properties.Settings.Default.ImportExportedFormRectangle);
                _exportedCreatureList.CheckForUnknownMods += ExportedCreatureList_CheckForUnknownMods;
            }
            _exportedCreatureList.ownerSuffix = "";
            _exportedCreatureList.Show();
            _exportedCreatureList.BringToFront();
        }

        private void ExportedCreatureList_CheckForUnknownMods(List<string> unknownSpeciesBlueprintPaths)
        {
            CheckForMissingModFiles(_creatureCollection, unknownSpeciesBlueprintPaths);
            // if mods were added, try to import the creature values again
            if (_creatureCollection.ModValueReloadNeeded
                && LoadModValuesOfCollection(_creatureCollection, true, true))
                _exportedCreatureList.LoadFilesInFolder();
        }
    }
}

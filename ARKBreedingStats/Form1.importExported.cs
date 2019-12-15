using ARKBreedingStats.Library;
using ARKBreedingStats.settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
                    OpenSettingsDialog(3);
                }
            }
            else
            {
                ShowExportedCreatureListControl();
                exportedCreatureList.ownerSuffix = loc.OwnerSuffix;
                exportedCreatureList.loadFilesInFolder(loc.FolderPath);
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
                exportedCreatureList.loadFilesInFolder(folder);
            }
            else if (
                MessageBox.Show("There is no valid folder set where the exported creatures are located. Set this folder in the settings.\n\nOpen the settings-page?",
                        "No default export-folder set", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                OpenSettingsDialog(3);
            }
        }

        private void ImportAllCreaturesInSelectedFolder(object sender, EventArgs e)
        {
            ShowExportedCreatureListControl();
            exportedCreatureList.chooseFolderAndImport();
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
                else
                    MessageBox.Show($"No exported creature-file found in the set folder\n{folder}\nYou have to export a creature first ingame.\n\n" +
                            "You may also want to check the set folder in the settings. Usually the folder is\n" +
                            @"…\Steam\steamapps\common\ARK\ShooterGame\Saved\DinoExports\<ID>",
                            "No files found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("There is no folder set where the exported creatures are located. Set this folder in the settings. " +
                                "Usually the folder is\n" + @"…\Steam\steamapps\common\ARK\ShooterGame\Saved\DinoExports\<ID>" + "\n\nOpen the settings-page?",
                                "No default export-folder set", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                OpenSettingsDialog(3);
            }
        }

        private void ExportedCreatureList_CopyValuesToExtractor(importExported.ExportedCreatureControl exportedCreatureControl, bool addToLibraryIfUnique, bool goToLibraryTab)
        {
            tabControlMain.SelectedTab = tabPageExtractor;
            ExtractExportedFileInExtractor(exportedCreatureControl, updateParentVisuals: !addToLibraryIfUnique);

            // add to library automatically if batch-extracting exportedImported values and uniqueLevels
            if (addToLibraryIfUnique)
            {
                if (extractor.uniqueResults)
                    AddCreatureToCollection(true, exportedCreatureControl.creatureValues.motherArkId, exportedCreatureControl.creatureValues.fatherArkId, goToLibraryTab);
                else
                    exportedCreatureControl.setStatus(importExported.ExportedCreatureControl.ImportStatus.NeedsLevelChosing, DateTime.Now);
            }
            else
            {
                // bring main-window to front to work with the data
                BringToFront();
            }
        }

        private void ImportExportedAddIfPossible_WatcherThread(string filePath)
        {
            // wait a moment until the file is readable. why is this necessary? blocked by fileWatcher?
            System.Threading.Thread.Sleep(100);

            // filewatcher is on another thread, invoke ui-thread to work with ui
            Invoke(new Action(delegate () { ImportExportedAddIfPossible(filePath); }));
        }

        private void ImportExportedAddIfPossible(string filePath)
        {
            bool alreadyExists = ExtractExportedFileInExtractor(filePath);
            bool added = false;

            if (extractor.uniqueResults)
            {
                AddCreatureToCollection(true, goToLibraryTab: false);
                added = true;
            }

            // give feedback in overlay
            if (overlay != null)
            {
                string infoText;
                Color textColor;
                const int colorSaturation = 200;
                if (added)
                {
                    infoText = $"Creature \"{creatureInfoInputExtractor.CreatureName}\" {(alreadyExists ? "updated in " : "added to")} the library."
                    + (Properties.Settings.Default.applyNamePatternOnImportIfEmptyName &&
                      Properties.Settings.Default.copyNameToClipboardOnImportWhenAutoNameApplied ? "\nName copied to clipboard." : "");
                    textColor = Color.FromArgb(colorSaturation, 255, colorSaturation);
                }
                else
                {
                    infoText = $"Creature \"{creatureInfoInputExtractor.CreatureName}\" couldn't be extracted uniquely, manual level selection is necessary.";
                    textColor = Color.FromArgb(255, colorSaturation, colorSaturation);
                }

                overlay.SetInfoText(infoText, textColor);
            }
        }

        private void ExportedCreatureList_CheckGuidInLibrary(importExported.ExportedCreatureControl exportedCreatureControl)
        {
            Creature cr = creatureCollection.creatures.SingleOrDefault(c => c.guid == exportedCreatureControl.creatureValues.guid);
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
            if (exportedCreatureList == null || exportedCreatureList.IsDisposed)
            {
                exportedCreatureList = new importExported.ExportedCreatureList();
                exportedCreatureList.CopyValuesToExtractor += ExportedCreatureList_CopyValuesToExtractor;
                exportedCreatureList.CheckArkIdInLibrary += ExportedCreatureList_CheckGuidInLibrary;
                exportedCreatureList.Location = Properties.Settings.Default.importExportedLocation;
                exportedCreatureList.CheckForUnknownMods += ExportedCreatureList_CheckForUnknownMods;
            }
            exportedCreatureList.ownerSuffix = "";
            exportedCreatureList.Show();
            exportedCreatureList.BringToFront();
        }

        private void ExportedCreatureList_CheckForUnknownMods(List<string> unknownSpeciesBlueprintPaths)
        {
            CheckForMissingModFiles(creatureCollection, unknownSpeciesBlueprintPaths);
            // if mods were added, try to import the creature values again
            if (creatureCollection.ModValueReloadNeeded
                && LoadModValuesOfCollection(creatureCollection, true, true))
                exportedCreatureList.loadFilesInFolder();
        }
    }
}

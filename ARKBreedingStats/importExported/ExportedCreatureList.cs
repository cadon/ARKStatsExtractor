using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static ARKBreedingStats.importExported.EccComparer;

namespace ARKBreedingStats.importExported
{
    public partial class ExportedCreatureList : Form
    {
        public event ExportedCreatureControl.CopyValuesToExtractorEventHandler CopyValuesToExtractor;
        public event ExportedCreatureControl.CheckArkIdInLibraryEventHandler CheckArkIdInLibrary;
        public delegate void CheckForUnknownModsEventHandler(List<string> unknownSpeciesBlueprintPaths);
        public event CheckForUnknownModsEventHandler CheckForUnknownMods;

        private List<ExportedCreatureControl> eccs;
        private string selectedFolder;
        public string ownerSuffix;
        private List<string> hiddenSpecies;
        private List<ToolStripMenuItem> speciesHideItems;
        private bool allowFiltering;
        private EccComparer eccComparer;

        public ExportedCreatureList()
        {
            InitializeComponent();
            eccs = new List<ExportedCreatureControl>();
            hiddenSpecies = new List<string>();
            speciesHideItems = new List<ToolStripMenuItem>();
            allowFiltering = true;

            FormClosing += ExportedCreatureList_FormClosing;

            eccComparer = new EccComparer();

            // TODO implement
            loadServerSettingsOfFolderToolStripMenuItem.Visible = false;
        }

        private void ExportedCreatureList_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if window is not minimized
            if (this.WindowState == FormWindowState.Normal)
            {
                (Properties.Settings.Default.ImportExportedFormRectangle, _) = Utils.GetWindowRectangle(this);
            }
        }

        private void chooseFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chooseFolderAndImport();
        }

        public void chooseFolderAndImport()
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                if (Utils.GetFirstImportExportFolder(out string exportFolder))
                {
                    dlg.SelectedPath = exportFolder;
                }
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LoadFilesInFolder(dlg.SelectedPath);
                }
            }
        }

        /// <summary>
        /// Reads all compatible files in the stated folder. If the folder is nullOrEmpty, the previous used folder is used.
        /// </summary>
        /// <param name="folderPath"></param>
        public void LoadFilesInFolder(string folderPath = null)
        {
            if (string.IsNullOrEmpty(folderPath)) folderPath = selectedFolder;
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) return;

            selectedFolder = folderPath;

            string[] files = Directory.GetFiles(folderPath, "*dinoexport*.ini");
            // check if there are many files to import, then ask because that can take time
            if (Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan > 0
                && files.Length > Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan
                && MessageBox.Show($"There are more than {Properties.Settings.Default.WarnWhenImportingMoreCreaturesThan}"
                            + $" files to import ({files.Length}) which can take some time.\n" +
                            "Do you really want to read all these files?",
                            "Many files to import", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            SuspendLayout();
            ClearControls();
            hiddenSpecies.Clear();
            foreach (var i in speciesHideItems) i.Dispose();
            speciesHideItems.Clear();

            List<string> unknownSpeciesBlueprintPaths = new List<string>();
            List<string> ignoreSpeciesBlueprintPaths = new List<string>();

            foreach (string f in files)
            {
                ExportedCreatureControl ecc = new ExportedCreatureControl(f);
                if (ecc.validValues)
                {
                    ecc.Dock = DockStyle.Top;
                    ecc.CopyValuesToExtractor += CopyValuesToExtractor;
                    ecc.CheckArkIdInLibrary += CheckArkIdInLibrary;
                    ecc.DisposeThis += Ecc_DisposeIt;
                    ecc.DoCheckArkIdInLibrary();
                    eccs.Add(ecc);
                    if (!string.IsNullOrEmpty(ecc.creatureValues.Species?.name) && !hiddenSpecies.Contains(ecc.creatureValues.Species.name))
                        hiddenSpecies.Add(ecc.creatureValues.Species.name);
                }
                else if (!string.IsNullOrEmpty(ecc.speciesBlueprintPath))
                {
                    if (unknownSpeciesBlueprintPaths.Contains(ecc.speciesBlueprintPath)
                        || ignoreSpeciesBlueprintPaths.Contains(ecc.speciesBlueprintPath))
                        continue;

                    // check if species should be ignored (e.g. if it's a raft)
                    if (Values.V.IgnoreSpeciesBlueprint(ecc.speciesBlueprintPath))
                    {
                        ignoreSpeciesBlueprintPaths.Add(ecc.speciesBlueprintPath);
                        continue;
                    }

                    // species should not be ignored and is not yet in the unknown species list
                    unknownSpeciesBlueprintPaths.Add(ecc.speciesBlueprintPath);
                }
            }

            OrderList();

            hiddenSpecies.Sort();
            foreach (var s in hiddenSpecies)
            {
                var item = new ToolStripMenuItem(s)
                {
                    CheckOnClick = true,
                    Checked = true
                };
                item.Click += ItemHideSpecies_Click;
                filterToolStripMenuItem.DropDownItems.Add(item);
                speciesHideItems.Add(item);
            }
            hiddenSpecies.Clear();

            Text = "Exported creatures in " + Utils.ShortPath(folderPath, 100);
            UpdateStatusBarLabelAndControls();
            ResumeLayout();

            // check for unsupported species
            if (unknownSpeciesBlueprintPaths.Any()) CheckForUnknownMods?.Invoke(unknownSpeciesBlueprintPaths);
        }

        private void Ecc_DisposeIt(object sender, EventArgs e)
        {
            ((ExportedCreatureControl)sender).Dispose();
            UpdateStatusBarLabelAndControls();
        }

        private void ItemHideSpecies_Click(object sender, EventArgs e)
        {
            var i = (ToolStripMenuItem)sender;
            if (i.Checked)
            {
                hiddenSpecies.Remove(i.Text);
            }
            else
            {
                if (!hiddenSpecies.Contains(i.Text))
                    hiddenSpecies.Add(i.Text);
            }
            FilterList();
        }

        private void ClearControls()
        {
            eccs.Clear();
            SuspendLayout();
            try
            {
                while (panel1.Controls.Count > 0)
                    panel1.Controls[0].Dispose();
            }
            finally
            {
                ResumeLayout();
            }
        }

        private void UpdateStatusBarLabelAndControls()
        {
            int justImported = 0,
                oldImported = 0,
                notImported = 0,
                issuesWhileImporting = 0,
                hiddenCreatures = 0,
                totalFiles = 0;

            eccs = eccs.Where(ecc => !ecc.IsDisposed).ToList();

            foreach (var ecc in eccs)
            {
                totalFiles++;
                if (!ecc.Visible) hiddenCreatures++;
                switch (ecc.Status)
                {
                    case ExportedCreatureControl.ImportStatus.JustImported: justImported++; break;
                    case ExportedCreatureControl.ImportStatus.NeedsLevelChoosing: issuesWhileImporting++; break;
                    case ExportedCreatureControl.ImportStatus.NotImported: notImported++; break;
                    case ExportedCreatureControl.ImportStatus.OldImported: oldImported++; break;
                }
            }

            toolStripStatusLabel1.Text = totalFiles.ToString() + " total files"
                + (hiddenCreatures > 0 ? " (" + hiddenCreatures.ToString() + " of them hidden)" : "") + ". "
                + (notImported + issuesWhileImporting).ToString() + " not imported, "
                + issuesWhileImporting.ToString() + " of these need manual level selection. "
                + justImported.ToString() + " just imported. "
                + oldImported.ToString() + " old imported.";

        }

        private void loadServerSettingsOfFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // check if a game.ini and or gameuser.ini is available and set the settings accordingly
        }

        private void ImportAllUnimportedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportAll(true);
        }

        private void ImportUpdateAllListedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportAll(false);
        }

        /// <summary>
        /// Tries to import all listed creatures and adds them to the library if the extraction is unique.
        /// </summary>
        private void ImportAll(bool onlyUnimported)
        {
            // check if there are many creatures to import, then ask because that can take time
            if (eccs.Count(c => c.Visible) > 50 &&
                    MessageBox.Show($"There are many creature-files to import ({eccs.Count}) which can take some time.\n" +
                            "Do you really want to import all these creature at once?",
                            "Many creatures to import", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            foreach (var ecc in eccs)
            {
                if (ecc.Visible
                    && (!onlyUnimported || ecc.Status == ExportedCreatureControl.ImportStatus.NotImported))
                    ecc.extractAndAddToLibrary(goToLibrary: false);
            }
            UpdateStatusBarLabelAndControls();
        }

        private void deleteAllImportedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllImportedFiles((ModifierKeys & Keys.Shift) != 0);
        }

        private void moveAllImportedFilesToimportedSubfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            bool suppressMessages = (ModifierKeys & Keys.Shift) != 0;
            if (suppressMessages || MessageBox.Show("Move all exported files in the current folder that are already imported in this library to the subfolder \"imported\"?",
                    "Move imported files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string importedPath = Path.Combine(selectedFolder, "imported");
                if (!FileService.TryCreateDirectory(importedPath, out string errorMessage))
                {
                    MessageBox.Show($"Subfolder\n{importedPath}\ncould not be created.\n{errorMessage}", $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int movedFilesCount = 0;
                foreach (var ecc in eccs)
                {
                    if (ecc.Status == ExportedCreatureControl.ImportStatus.JustImported || ecc.Status == ExportedCreatureControl.ImportStatus.OldImported)
                    {
                        try
                        {
                            File.Move(ecc.exportedFile, Path.Combine(importedPath, Path.GetFileName(ecc.exportedFile)));
                            movedFilesCount++;
                            ecc.Dispose();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"The file\n{ecc.exportedFile}\ncould not be moved. The following files will not be moved either.\n\nException:\n{ex.Message}", $"Error moving file - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                }

                if (!suppressMessages)
                {
                    if (movedFilesCount > 0)
                        MessageBox.Show($"{movedFilesCount} imported files moved to\n{importedPath}", "Files moved",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No files were moved.", "No files moved",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            UpdateStatusBarLabelAndControls();
            ResumeLayout();
        }

        private void DeleteAllImportedFiles(bool dontDisplayAnyWarnings)
        {
            SuspendLayout();
            if (dontDisplayAnyWarnings || MessageBox.Show("Delete all exported files in the current folder that are already imported in this library?\nThis cannot be undone!",
                    "Delete imported files?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int deletedFilesCount = 0;
                foreach (var ecc in eccs)
                {
                    if (ecc.Status == ExportedCreatureControl.ImportStatus.JustImported || ecc.Status == ExportedCreatureControl.ImportStatus.OldImported)
                    {
                        if (ecc.removeFile(false))
                        {
                            deletedFilesCount++;
                            ecc.Dispose();
                        }
                    }
                }
                if (!dontDisplayAnyWarnings && deletedFilesCount > 0)
                    MessageBox.Show(deletedFilesCount + " imported files deleted.", "Deleted Files",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdateStatusBarLabelAndControls();
            ResumeLayout();
        }

        private void deleteAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllFiles((ModifierKeys & Keys.Shift) != 0);
        }

        private void DeleteAllFiles(bool dontDisplayAnyWarnings)
        {
            SuspendLayout();
            if (dontDisplayAnyWarnings || MessageBox.Show("Delete all files in the current folder, regardless if they are imported or not imported?\nThis cannot be undone!",
                    "Delete ALL files?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int deletedFilesCount = 0;
                foreach (ExportedCreatureControl ecc in eccs)
                {
                    if (ecc.removeFile(false))
                    {
                        deletedFilesCount++;
                        ecc.Dispose();
                    }
                }
                if (!dontDisplayAnyWarnings && deletedFilesCount > 0)
                    MessageBox.Show(deletedFilesCount + " imported files deleted.", "Deleted Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdateStatusBarLabelAndControls();
            ResumeLayout();
        }

        private void ExportedCreatureList_DragEnter(object sender, DragEventArgs e)
        {
            DragDropEffects effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                    effects = DragDropEffects.Copy;
            }
            e.Effect = effects;
        }

        private void ExportedCreatureList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                    LoadFilesInFolder(path);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFolder) && Directory.Exists(selectedFolder))
                System.Diagnostics.Process.Start(selectedFolder);
        }

        private void toolStripCbHideImported_Click(object sender, EventArgs e)
        {
            FilterList();
        }

        private void FilterList()
        {
            if (!allowFiltering) return;

            SuspendLayout();
            foreach (var ecc in eccs)
            {
                if ((!showImportedCreaturesToolStripMenuItem.Checked &&
                    (ecc.Status == ExportedCreatureControl.ImportStatus.JustImported || ecc.Status == ExportedCreatureControl.ImportStatus.OldImported))
                    || (!string.IsNullOrEmpty(ecc.creatureValues.Species?.name) && hiddenSpecies.Contains(ecc.creatureValues.Species.name)))
                {
                    ecc.Hide();
                }
                else
                {
                    ecc.Show();
                }
            }
            UpdateStatusBarLabelAndControls();
            ResumeLayout();
        }

        /// <summary>
        /// Orders the controls that display the exported creatures.
        /// </summary>
        private void OrderList()
        {
            eccs.Sort(eccComparer);

            foreach (var ecc in eccs)
                panel1.Controls.Add(ecc);

            // update button order index
            for (int i = 0; i < 5; i++)
            {
                string text = (i + 1).ToString() + ". " + (eccComparer.OrderOrderList[i] ? "⯅" : "⯆") + " ";
                switch (eccComparer.OrderPropertyList[i])
                {
                    case OrderProperties.CreatureName:
                        creatureNameToolStripMenuItem1.Text = text + "Creature name";
                        break;
                    case OrderProperties.ExportTime:
                        exportTimeToolStripMenuItem1.Text = text + "Export time";
                        break;
                    case OrderProperties.ImportStatus:
                        importStatusToolStripMenuItem.Text = text + "Import status";
                        break;
                    case OrderProperties.OwnerName:
                        ownerNameToolStripMenuItem1.Text = text + "Owner name";
                        break;
                    case OrderProperties.Species:
                        speciesToolStripMenuItem1.Text = text + "Species";
                        break;
                }
            }
        }

        private void setUserSuffixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utils.ShowTextInput("Owner suffix (will be appended to the owner)", out string ownerSfx, "Owner Suffix", ownerSuffix))
                ownerSuffix = ownerSfx;
        }

        private void filterAllSpeciestoolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowFiltering = false;
            hiddenSpecies.Clear();
            bool check = ((ToolStripMenuItem)sender).Checked;
            foreach (var i in speciesHideItems)
            {
                i.Checked = check;
                if (!check) hiddenSpecies.Add(i.Text);
            }
            allowFiltering = true;
            FilterList();
        }

        /// <summary>
        /// Updates the order lists. If the order-property is already the first item, the order is reversed.
        /// </summary>
        /// <param name="exportTime"></param>
        private void UpdateOrderLists(OrderProperties orderProperty)
        {
            eccComparer.UpdateOrderList(orderProperty);
            OrderList();
        }

        private void exportTimeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateOrderLists(OrderProperties.ExportTime);
        }

        private void creatureNameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateOrderLists(OrderProperties.CreatureName);
        }

        private void speciesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateOrderLists(OrderProperties.Species);
        }

        private void ownerNameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateOrderLists(OrderProperties.OwnerName);
        }

        private void importStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateOrderLists(OrderProperties.ImportStatus);
        }
    }

    internal class EccComparer : Comparer<ExportedCreatureControl>
    {
        public readonly List<OrderProperties> OrderPropertyList;
        public readonly List<bool> OrderOrderList;

        public EccComparer()
        {
            OrderPropertyList = new List<OrderProperties> { OrderProperties.ImportStatus, OrderProperties.ExportTime, OrderProperties.Species, OrderProperties.OwnerName, OrderProperties.CreatureName };
            OrderOrderList = new List<bool> { true, true, true, true, true };
        }

        public enum OrderProperties
        {
            ExportTime,
            CreatureName,
            Species,
            OwnerName,
            ImportStatus
        }

        public override int Compare(ExportedCreatureControl a, ExportedCreatureControl b)
        {
            int result = 0;
            int orderPropertyIndex = 0;
            while (result == 0 && orderPropertyIndex < 5)
            {
                result = CompareByProperty(a, b, OrderPropertyList[orderPropertyIndex], OrderOrderList[orderPropertyIndex]);
                orderPropertyIndex++;
            }
            return result;
        }

        private int CompareByProperty(ExportedCreatureControl a, ExportedCreatureControl b, OrderProperties property, bool ascending)
        {
            int result = 0;
            switch (property)
            {
                case OrderProperties.ExportTime:
                    result = DateTime.Compare(a.creatureValues.domesticatedAt ?? new DateTime(2000, 1, 1), b.creatureValues.domesticatedAt ?? new DateTime(2000, 1, 1)); break;
                case OrderProperties.CreatureName:
                    result = string.Compare(a.creatureValues.name, b.creatureValues.name); break;
                case OrderProperties.Species:
                    result = string.Compare(a.creatureValues.speciesName, b.creatureValues.speciesName); break;
                case OrderProperties.OwnerName:
                    result = string.Compare(a.creatureValues.owner, b.creatureValues.owner); break;
                case OrderProperties.ImportStatus:
                    result = (int)a.Status - (int)b.Status; break;
            }
            if (!ascending) return -result;
            return result;
        }

        internal void UpdateOrderList(OrderProperties orderProperty)
        {
            if (OrderPropertyList[0] == orderProperty)
                OrderOrderList[0] = !OrderOrderList[0];
            else
            {
                int index = OrderPropertyList.IndexOf(orderProperty);
                if (index != -1)
                {
                    OrderPropertyList.RemoveAt(index);
                    bool asc = OrderOrderList[index];
                    OrderOrderList.RemoveAt(index);
                    OrderPropertyList.Insert(0, orderProperty);
                    OrderOrderList.Insert(0, asc);
                }
            }
        }
    }
}

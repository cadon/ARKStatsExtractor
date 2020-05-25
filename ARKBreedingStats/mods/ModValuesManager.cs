using ARKBreedingStats.Library;
using ARKBreedingStats.mods;
using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class ModValuesManager : Form
    {
        private CreatureCollection cc;
        private List<ModInfo> modInfos;

        public ModValuesManager()
        {
            InitializeComponent();
            lbAvailableModFiles.Sorted = true;
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                cc = value;

                if (Values.V.modsManifest?.modsByFiles != null)
                {
                    modInfos = Values.V.modsManifest.modsByFiles.Select(smi => smi.Value).Where(mi => mi.mod != null).ToList();
                }
                UpdateModListBoxes();
            }
        }

        private void BtLoadModFile_Click(object sender, EventArgs e)
        {
            string valuesFolder = FileService.GetJsonPath(FileService.ValuesFolder);
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Additional values-file (*.json)|*.json",
                InitialDirectory = valuesFolder,
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dlg.FileName;
                    // copy to json folder if loaded from somewhere else
                    if (!filePath.StartsWith(valuesFolder))
                    {
                        try
                        {
                            string destination = Path.Combine(valuesFolder, Path.GetFileName(filePath));
                            File.Copy(filePath, destination);
                            filePath = destination;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Trying to copy the file to the application's json folder failed.\n" +
                                    "The program won't be able to load it at its next start.\n\n" +
                                    "Error message:\n\n" + ex.Message, "Copy file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    bool modFileLoaded = false;
                    Values modValues = null;
                    try
                    {
                        modFileLoaded = Values.TryLoadValuesFile(filePath, setModFileName: true, throwExceptionOnFail: true, out modValues, errorMessage: out _);
                    }
                    catch (FormatException)
                    {
                        Form1.FormatExceptionMessageBox(filePath);
                    }

                    if (modFileLoaded && modValues != null)
                    {
                        if (cc.ModList.Contains(modValues.mod))
                        {
                            MessageBox.Show($"The mod\n{modValues.mod.title}\nis already loaded.", "Already loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            cc.ModList.Add(modValues.mod);
                            UpdateModListBoxes();
                        }
                    }
                }
            }
        }

        private void BtRemoveModFile_Click(object sender, EventArgs e)
        {
            RemoveSelectedMod();
        }

        private void BtMoveUp_Click(object sender, EventArgs e)
        {
            MoveSelectedMod(-1);
        }

        private void BtMoveDown_Click(object sender, EventArgs e)
        {
            MoveSelectedMod(1);
        }

        private void MoveSelectedMod(int moveBy)
        {
            if (!(lbModList.SelectedItem is ModInfo selectedLoadedMod) || cc?.ModList == null) return;

            int i = cc.ModList.IndexOf(selectedLoadedMod.mod);
            if (i == -1) return;

            int newPos = i + moveBy;
            if (newPos < 0) newPos = 0;
            if (newPos >= cc.ModList.Count) newPos = cc.ModList.Count - 1;

            cc.ModList.Remove(selectedLoadedMod.mod);
            cc.ModList.Insert(newPos, selectedLoadedMod.mod);
            UpdateModListBoxes();
        }

        /// <summary>
        /// Update entries of the enabled mods for the library and the available mods.
        /// </summary>
        private void UpdateModListBoxes()
        {
            var selectedMiLib = lbModList.SelectedItem as ModInfo;
            var selectedMiAvMod = lbAvailableModFiles.SelectedItem as ModInfo;

            lbModList.Items.Clear();
            lbAvailableModFiles.Items.Clear();

            if (cc?.ModList == null) return;

            var modToModInfo = modInfos.ToDictionary(mi => mi.mod, mi => mi);

            foreach (ModInfo mi in modInfos) mi.currentlyInLibrary = false;

            foreach (Mod m in cc.ModList)
            {
                if (modToModInfo.ContainsKey(m))
                {
                    lbModList.Items.Add(modToModInfo[m]);
                    modToModInfo[m].currentlyInLibrary = true;
                }
            }

            foreach (ModInfo mi in modInfos)
            {
                if (!mi.currentlyInLibrary)
                {
                    lbAvailableModFiles.Items.Add(mi);
                }
            }

            lbModList.SelectedItem = selectedMiLib;
            lbAvailableModFiles.SelectedItem = selectedMiAvMod;

            cc.UpdateModList();
        }

        private void LbAvailableModFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbAvailableModFiles.SelectedItem == null) return;

            DisplayModInfo((ModInfo)lbAvailableModFiles.SelectedItem);
        }

        private void LbModList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbModList.SelectedItem == null) return;

            DisplayModInfo((ModInfo)lbModList.SelectedItem);
        }

        private void DisplayModInfo(ModInfo modInfo)
        {
            if (modInfo?.mod == null) return;
            lbModName.Text = modInfo.mod.title;
            LbModVersion.Text = modInfo.version;
            lbModTag.Text = modInfo.mod.tag;
            lbModId.Text = modInfo.mod.id;
            llbSteamPage.Visible = modInfo.onlineAvailable; // it's assumed that the officially supported mods all have a steam page
        }

        private void BtClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LlbSteamPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrEmpty(lbModId.Text)) return;
            System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=" + lbModId.Text);
        }

        private void BtAddMod_Click(object sender, EventArgs e)
        {
            AddSelectedMod();
        }

        private void BtRemoveMod_Click(object sender, EventArgs e)
        {
            RemoveSelectedMod();
        }

        private void AddSelectedMod()
        {
            ModInfo mi = (ModInfo)lbAvailableModFiles.SelectedItem;
            if (mi?.mod == null || cc?.ModList == null) return;

            cc.ModList.Add(mi.mod);
            UpdateModListBoxes();
            lbAvailableModFiles.SelectedIndex = -1;
            lbModList.SelectedItem = mi;
        }

        private void RemoveSelectedMod()
        {
            ModInfo mi = (ModInfo)lbModList.SelectedItem;
            if (mi?.mod == null || cc?.ModList == null) return;

            if (cc.ModList.Remove(mi.mod))
            {
                UpdateModListBoxes();
                lbModList.SelectedIndex = -1;
                lbAvailableModFiles.SelectedItem = mi;
            }
        }

        private void BtOpenValuesFolder_Click(object sender, EventArgs e)
        {
            string valuesFolderPath = FileService.GetJsonPath(FileService.ValuesFolder);
            if (Directory.Exists(valuesFolderPath))
                System.Diagnostics.Process.Start(valuesFolderPath);
        }

        private void LbAvailableModFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddSelectedMod();
        }

        private void LbModList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RemoveSelectedMod();
        }

        private void BtRemoveAllMods_Click(object sender, EventArgs e)
        {
            ModInfo mi = (ModInfo)lbModList.SelectedItem;
            if (mi?.mod == null || cc?.ModList == null) return;

            cc.ModList.Clear();

            UpdateModListBoxes();
            lbModList.SelectedIndex = -1;
            lbAvailableModFiles.SelectedItem = null;
        }
    }
}

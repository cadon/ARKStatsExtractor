using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats.mods
{
    public partial class ModValuesManager : Form
    {
        private CreatureCollection _cc;
        private ModInfo[] _modInfos;
        private readonly ToolTip _tt = new ToolTip();

        public ModValuesManager()
        {
            InitializeComponent();
            lbAvailableModFiles.Sorted = true;
            llbSteamPage.Visible = false;
            Disposed += (s, a) => _tt?.Dispose();
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                _cc = value;

                if (Values.V.modsManifest?.modsByFiles != null)
                {
                    _modInfos = Values.V.modsManifest.modsByFiles.Select(smi => smi.Value).Where(mi => mi.mod != null && !mi.mod.expansion).ToArray();
                }
                UpdateModListBoxes();
            }
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
            if (!(lbModList.SelectedItem is ModInfo selectedLoadedMod) || _cc?.ModList == null) return;

            int i = _cc.ModList.IndexOf(selectedLoadedMod.mod);
            if (i == -1) return;

            int newPos = i + moveBy;
            if (newPos < 0) newPos = 0;
            if (newPos >= _cc.ModList.Count) newPos = _cc.ModList.Count - 1;

            if (newPos == i) return;

            _cc.ModList.Remove(selectedLoadedMod.mod);
            _cc.ModList.Insert(newPos, selectedLoadedMod.mod);
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

            if (_cc?.ModList == null) return;

            var modToModInfo = _modInfos.ToDictionary(mi => mi.mod, mi => mi);

            foreach (ModInfo mi in _modInfos) mi.CurrentlyInLibrary = false;

            foreach (Mod m in _cc.ModList)
            {
                if (modToModInfo.ContainsKey(m))
                {
                    lbModList.Items.Add(modToModInfo[m]);
                    modToModInfo[m].CurrentlyInLibrary = true;
                }
            }

            FilterMods();

            lbModList.SelectedItem = selectedMiLib;
            lbAvailableModFiles.SelectedItem = selectedMiAvMod;

            _cc.UpdateModList();
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
            llbSteamPage.Visible = modInfo.OnlineAvailable; // it's assumed that the officially supported mods all have a steam page
            if (!string.IsNullOrEmpty(modInfo.mod.id))
                _tt.SetToolTip(llbSteamPage, $"Open this page in your browser:\n{GetSteamModPageUrlById(lbModId.Text)}");
        }

        private void BtClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string GetSteamModPageUrlById(string modId) => "https://steamcommunity.com/sharedfiles/filedetails/?id=" + modId;

        private void LlbSteamPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrEmpty(lbModId.Text)) return;
            System.Diagnostics.Process.Start(GetSteamModPageUrlById(lbModId.Text));
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
            if (mi?.mod == null || _cc?.ModList == null) return;

            _cc.ModList.Add(mi.mod);
            UpdateModListBoxes();
            lbAvailableModFiles.SelectedIndex = -1;
            lbModList.SelectedItem = mi;
        }

        private void RemoveSelectedMod()
        {
            ModInfo mi = (ModInfo)lbModList.SelectedItem;
            if (mi?.mod == null || _cc?.ModList == null) return;

            if (_cc.ModList.Remove(mi.mod))
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
            _cc.ModList.Clear();

            UpdateModListBoxes();
            lbModList.SelectedIndex = -1;
            lbAvailableModFiles.SelectedItem = null;
        }

        private void linkLabelCustomModManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Mod-Values");
        }

        private void LlUnofficialModFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Unsupported-Mod-Values");
        }

        private readonly Debouncer _modFilterDebouncer = new Debouncer();
        private void TbModFilter_TextChanged(object sender, EventArgs e)
        {
            _modFilterDebouncer.Debounce(300, FilterMods, Dispatcher.CurrentDispatcher);
        }

        private void BtClearFilter_Click(object sender, EventArgs e)
        {
            TbModFilter.Text = string.Empty;
        }

        private void FilterMods()
        {
            var filter = string.IsNullOrWhiteSpace(TbModFilter.Text) ? null : TbModFilter.Text.Trim();

            lbAvailableModFiles.BeginUpdate();
            lbAvailableModFiles.Items.Clear();

            lbAvailableModFiles.Items.AddRange(
                _modInfos.Where(mi => !mi.CurrentlyInLibrary
                                      && (filter == null
                                          || mi.mod.title.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1
                                          || mi.mod.tag.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1
                                          )
                ).ToArray());

            lbAvailableModFiles.EndUpdate();

            TbModFilter.BackColor = filter == null ? SystemColors.Window : Color.LightYellow;
        }
    }
}

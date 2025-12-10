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
            LlModWebPage.Visible = false;
            Disposed += (s, a) => _tt?.Dispose();
        }

        public CreatureCollection CreatureCollection
        {
            set
            {
                _cc = value;

                if (Values.V.modsManifest?.ModsByFiles != null)
                {
                    _modInfos = Values.V.modsManifest.ModsByFiles.Select(smi => smi.Value).Where(mi => mi.Mod != null && !mi.Mod.IsExpansion).ToArray();
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

            int i = _cc.ModList.IndexOf(selectedLoadedMod.Mod);
            if (i == -1) return;

            int newPos = i + moveBy;
            if (newPos < 0) newPos = 0;
            if (newPos >= _cc.ModList.Count) newPos = _cc.ModList.Count - 1;

            if (newPos == i) return;

            _cc.ModList.Remove(selectedLoadedMod.Mod);
            _cc.ModList.Insert(newPos, selectedLoadedMod.Mod);
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

            var modToModInfo = _modInfos.ToDictionary(mi => mi.Mod, mi => mi);

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
            if (modInfo?.Mod == null) return;
            lbModName.Text = modInfo.Mod.Title;
            SetLabelInfo(LbGameLabel, LbGame, modInfo.Mod.IsAsa ? Ark.Asa : Ark.Ase);
            LbModVersion.Text = modInfo.Version?.ToString();
            lbModTag.Text = modInfo.Mod.Tag;
            lbModId.Text = modInfo.Mod.Id;
            SetLabelInfo(LbAuthorLabel, LbAuthor, modInfo.Mod.Author);

            // ASA mods need property CfPage set. ASE mods use id for steam link.
            var modUrl = !modInfo.OnlineAvailable || !int.TryParse(modInfo.Mod.Id, out _)
                ? null
                : modInfo.Mod.IsAsa
                    ? GetCurseForgeUrl(modInfo.Mod.CfPage)
                    : GetSteamModPageUrlById(modInfo.Mod.Id);

            var modUrlAvailable = !string.IsNullOrEmpty(modUrl);

            LlModWebPage.Visible = modUrlAvailable;
            if (modUrlAvailable)
            {
                LlModWebPage.Tag = modUrl;
                _tt.SetToolTip(LlModWebPage, $"Open this page in a web browser:\n{modUrl}");
            }
        }

        /// <summary>
        /// Sets value on label. If value is empty, both labels are hidden.
        /// </summary>
        private void SetLabelInfo(Label lbLabel, Label lbValue, string value)
        {
            var show = !string.IsNullOrEmpty(value);
            lbLabel.Visible = show;
            lbValue.Visible = show;
            lbValue.Text = value;
        }

        private void BtClose_Click(object sender, EventArgs e) => Close();

        private static string GetSteamModPageUrlById(string modId) => string.IsNullOrEmpty(modId) ? null : "https://steamcommunity.com/sharedfiles/filedetails/?id=" + modId;
        private static string GetCurseForgeUrl(string modPageName) => string.IsNullOrEmpty(modPageName) ? null : "https://www.curseforge.com/ark-survival-ascended/mods/" + modPageName;

        private void LlbSteamPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!(sender is LinkLabel ll && ll.Tag is string link) || string.IsNullOrEmpty(link)) return;
            System.Diagnostics.Process.Start(link);
        }

        private void BtAddMod_Click(object sender, EventArgs e) => AddSelectedMod();

        private void BtRemoveMod_Click(object sender, EventArgs e) => RemoveSelectedMod();

        private void AddSelectedMod()
        {
            ModInfo mi = (ModInfo)lbAvailableModFiles.SelectedItem;
            if (mi?.Mod == null || _cc?.ModList == null) return;

            _cc.ModList.Add(mi.Mod);
            UpdateModListBoxes();
            lbAvailableModFiles.SelectedIndex = -1;
            lbModList.SelectedItem = mi;
        }

        private void RemoveSelectedMod()
        {
            ModInfo mi = (ModInfo)lbModList.SelectedItem;
            if (mi?.Mod == null || _cc?.ModList == null) return;

            if (_cc.ModList.Remove(mi.Mod))
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

        private void LbAvailableModFiles_MouseDoubleClick(object sender, MouseEventArgs e) => AddSelectedMod();

        private void LbModList_MouseDoubleClick(object sender, MouseEventArgs e) => RemoveSelectedMod();

        private void BtRemoveAllMods_Click(object sender, EventArgs e)
        {
            _cc.ModList.Clear();

            UpdateModListBoxes();
            lbModList.SelectedIndex = -1;
            lbAvailableModFiles.SelectedItem = null;
        }

        private void linkLabelCustomModManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => RepositoryInfo.OpenWikiPage("Mod-Values");

        private void LlUnofficialModFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => RepositoryInfo.OpenWikiPage("Unsupported-Mod-Values");

        private readonly Debouncer _modFilterDebouncer = new Debouncer();
        private void TbModFilter_TextChanged(object sender, EventArgs e)
            => _modFilterDebouncer.Debounce(300, FilterMods, Dispatcher.CurrentDispatcher);

        private void BtClearFilter_Click(object sender, EventArgs e) => TbModFilter.Text = string.Empty;

        private void FilterMods()
        {
            var filter = string.IsNullOrWhiteSpace(TbModFilter.Text) ? null : TbModFilter.Text.Trim();

            lbAvailableModFiles.BeginUpdate();
            lbAvailableModFiles.Items.Clear();

            lbAvailableModFiles.Items.AddRange(
                _modInfos.Where(mi => !mi.CurrentlyInLibrary
                                      && (filter == null
                                          || mi.Mod.Title.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1
                                          || mi.Mod.Tag.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1
                                          )
                ).ToArray());

            lbAvailableModFiles.EndUpdate();

            TbModFilter.BackColor = filter == null ? SystemColors.Window : Color.LightYellow;
        }
    }
}

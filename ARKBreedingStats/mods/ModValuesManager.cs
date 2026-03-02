using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;
using System.ComponentModel;

namespace ARKBreedingStats.mods
{
    public partial class ModValuesManager : Form
    {
        private CreatureCollection _cc;
        private ModInfo[] _modInfos;
        private (ListViewItem lvi, ModInfo mi)[] _lviAvailableMods;
        private readonly ToolTip _tt = new ToolTip();

        public ModValuesManager()
        {
            InitializeComponent();
            LlModWebPage.Visible = false;
            Disposed += (s, a) =>
            {
                _tt?.RemoveAll();
                _tt?.Dispose();
            };
            LvAvailableModFiles.Groups.AddRange(new[]
            {
                new ListViewGroup("Official"),
                new ListViewGroup("Unofficial")
            });
            LvAvailableModFiles.DoubleBuffered(true);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CreatureCollection CreatureCollection
        {
            set
            {
                _cc = value;

                if (Values.V.modsManifest?.ModsByFiles != null)
                {
                    // reload manual mod values files. This avoids needing an app restart if a manual mod value file was added to the folder
                    if (ModsManifest.LoadManualValueFiles(Values.V.modsManifest, out var customModsManifest)
                        && customModsManifest?.ModsByFiles.Any() == true)
                    {
                        Values.V.modsManifest = ModsManifest.MergeModsManifest(Values.V.modsManifest, customModsManifest);
                        Values.V.modsManifest.Initialize();
                    }

                    _modInfos = Values.V.modsManifest.ModsByFiles.Select(smi => smi.Value).Where(mi => mi.Mod != null && !mi.Mod.IsExpansion).ToArray();
                    _lviAvailableMods = _modInfos.Select(mi => (CreateLvi(mi), mi)).OrderBy(m => m.mi.Mod?.Title).ToArray();
                }
                UpdateModListBoxes();
                if (_cc.Game == Ark.Asa)
                    RbAsa.Checked = true;
                else RbAse.Checked = true;
            }
        }

        private ListViewItem CreateLvi(ModInfo mi)
        {
            var lvi = new ListViewItem(new[]
                {
                    mi.Mod.Title,
                    mi.OnlineAvailable?"⤓":string.Empty
                })
            { Tag = mi };
            if (mi.OnlineAvailable && mi.LocallyAvailable)
            {
                lvi.UseItemStyleForSubItems = false;
                lvi.SubItems[1].SetBackColorAndAccordingForeColor(Color.LightGreen);
            }
            lvi.Group = mi.Mod.IsOfficial ? LvAvailableModFiles.Groups[0] : LvAvailableModFiles.Groups[1];
            lvi.ToolTipText = mi.Mod?.Title + (!mi.OnlineAvailable ? string.Empty : mi.LocallyAvailable ? " (downloaded)" : " (will be downloaded if needed)");
            return lvi;
        }

        private void BtMoveUp_Click(object sender, EventArgs e) => MoveSelectedMod(-1);

        private void BtMoveDown_Click(object sender, EventArgs e) => MoveSelectedMod(1);

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
            var selectedMiAvMod = GetSelectedModInfoAvailable();

            lbModList.BeginUpdate();
            lbModList.Items.Clear();

            if (_cc?.ModList == null)
            {
                lbModList.EndUpdate();
                return;
            }

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
            lbModList.EndUpdate();

            FilterMods();

            lbModList.SelectedItem = selectedMiLib;
            SetSelectedModInfoAvailable(selectedMiAvMod);

            _cc.UpdateModList();
        }

        private void LvAvailableModFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mi = GetSelectedModInfoAvailable();
            if (mi != null)
                DisplayModInfo(mi);
        }

        private ModInfo GetSelectedModInfoAvailable()
            => LvAvailableModFiles.SelectedItems.Count > 0
               && LvAvailableModFiles.SelectedItems[0].Tag is ModInfo mi ? mi : null;

        private void SetSelectedModInfoAvailable(ModInfo modInfo)
        {
            LvAvailableModFiles.SelectedIndices.Clear();
            if (modInfo == null) return;
            for (var i = 0; i < LvAvailableModFiles.Items.Count; i++)
            {
                var lvi = LvAvailableModFiles.Items[i];
                if (lvi.Tag is ModInfo mi && mi == modInfo)
                {
                    LvAvailableModFiles.SelectedIndices.Add(i);
                    break;
                }
            }
        }

        private void LbModList_SelectedIndexChanged(object sender, EventArgs e) => DisplayModInfo((ModInfo)lbModList.SelectedItem);

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
            Utils.OpenUri(link);
        }

        private void BtAddMod_Click(object sender, EventArgs e) => AddSelectedMod();

        private void BtRemoveMod_Click(object sender, EventArgs e) => RemoveSelectedMod();

        private void AddSelectedMod()
        {
            var mi = GetSelectedModInfoAvailable();
            if (mi?.Mod == null || _cc?.ModList == null) return;

            _cc.ModList.Add(mi.Mod);
            UpdateModListBoxes();
            LvAvailableModFiles.SelectedIndices.Clear();
            lbModList.SelectedItem = mi;
        }

        private void RemoveSelectedMod()
        {
            var mi = (ModInfo)lbModList.SelectedItem;
            if (mi?.Mod == null || _cc?.ModList == null) return;

            if (_cc.ModList.Remove(mi.Mod))
            {
                UpdateModListBoxes();
                lbModList.SelectedIndex = -1;
                SetSelectedModInfoAvailable(mi);
            }
        }

        private void BtOpenValuesFolder_Click(object sender, EventArgs e)
        {
            string valuesFolderPath = FileService.GetJsonPath(FileService.ValuesFolder);
            if (Directory.Exists(valuesFolderPath))
                Utils.OpenUri(valuesFolderPath);
        }

        private void LvAvailableModFiles_DoubleClick(object sender, EventArgs e) => AddSelectedMod();

        private void LbModList_MouseDoubleClick(object sender, MouseEventArgs e) => RemoveSelectedMod();

        private void BtRemoveAllMods_Click(object sender, EventArgs e)
        {
            _cc.ModList.Clear();

            UpdateModListBoxes();
            lbModList.SelectedIndex = -1;
            LvAvailableModFiles.SelectedIndices.Clear();
        }

        private void linkLabelCustomModManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => RepositoryInfo.OpenWikiPage("Mod-Values");

        private void LlUnofficialModFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => RepositoryInfo.OpenWikiPage("Unsupported-Mod-Values");

        private readonly Debouncer _modFilterDebouncer = new Debouncer();
        private void TbModFilter_TextChanged(object sender, EventArgs e) => FilterModsDebounced();

        private void BtClearFilter_Click(object sender, EventArgs e) => TbModFilter.Text = string.Empty;

        private void FilterModsDebounced()
            => _modFilterDebouncer.Debounce(300, FilterMods, Dispatcher.CurrentDispatcher);

        private void FilterMods()
        {
            var filter = string.IsNullOrWhiteSpace(TbModFilter.Text) ? null : TbModFilter.Text.Trim();

            var filterIsAsa = RbAsa.Checked;

            LvAvailableModFiles.BeginUpdate();
            LvAvailableModFiles.Items.Clear();

            LvAvailableModFiles.Items.AddRange(
                _lviAvailableMods.Where(m => !m.mi.CurrentlyInLibrary
                                             && m.mi.Mod.IsAsa == filterIsAsa
                                             && (filter == null
                                                 || m.mi.Mod.Title.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1
                                                 || m.mi.Mod.Tag.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1
                                                 )
                                             )
                    .Select(m => m.lvi)
                    .ToArray());

            // for some reason the groups are removed each time items are added, so add them here
            foreach (ListViewItem lvi in LvAvailableModFiles.Items)
                lvi.Group = ((ModInfo)lvi.Tag).Mod.IsOfficial ? LvAvailableModFiles.Groups[0] : LvAvailableModFiles.Groups[1];

            LvAvailableModFiles.EndUpdate();

            TbModFilter.BackColor = filter == null ? SystemColors.Window : Color.LightYellow;
        }

        private void RbGameCheckedChanged(object sender, EventArgs e) => FilterModsDebounced();
    }
}

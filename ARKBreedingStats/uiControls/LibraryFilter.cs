using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;

namespace ARKBreedingStats.uiControls
{
    public partial class LibraryFilter : Form
    {
        private CreatureCollection _cc;
        private List<CheckBox> _statusCheckBoxes;
        private int _selectedColorFilter;
        private MyColorPicker _colorPicker;

        public LibraryFilter()
        {
            InitializeComponent();
        }

        public LibraryFilter(CreatureCollection cc) : this()
        {
            _cc = cc;

            Localization();

            _colorPicker = new MyColorPicker();

            SetColorFilter(Properties.Settings.Default.FilterOnlyIfColorId);
            CbUseFilterInTopStatCalculation.Checked = Properties.Settings.Default.useFiltersInTopStatCalculation;
            CbLibraryGroupSpecies.Checked = Properties.Settings.Default.LibraryGroupBySpecies;

            UpdateOwnerServerTagLists();
        }

        /// <summary>
        /// Updates the list of set owners, servers, tribes and tags of all creatures.
        /// </summary>
        private void UpdateOwnerServerTagLists()
        {
            // status list
            var statusList = new CreatureFlags[]
            {
                CreatureFlags.Available,
                CreatureFlags.Unavailable,
                CreatureFlags.Dead,
                CreatureFlags.Obelisk,
                CreatureFlags.Cryopod,
                CreatureFlags.Mutated,
                CreatureFlags.Neutered,
                CreatureFlags.Female,
                CreatureFlags.Male,
            };
            _statusCheckBoxes = new List<CheckBox>(statusList.Length);
            int statusButtonWidth = FlpStatus.Width - 6;
            foreach (var s in statusList)
            {
                bool isChecked = (Properties.Settings.Default.FilterHideCreaturesFlags & (int)s) != 0;
                var cb = new CheckBox()
                {
                    Text = s.ToString(),
                    Appearance = Appearance.Button,
                    Tag = s,
                    Checked = isChecked,
                    BackColor = CheckBoxColor(isChecked),
                    Width = statusButtonWidth
                };
                FlpStatus.Controls.Add(cb);
                FlpStatus.SetFlowBreak(cb, true);
                _statusCheckBoxes.Add(cb);
                cb.CheckedChanged += CbStatusCheckedChanged;
            }

            //// lists with number of according creatures
            var ownerList = new Dictionary<string, int>();
            var tribesList = new Dictionary<string, int>();
            var serversList = new Dictionary<string, int>();
            var tagList = new Dictionary<string, int>();

            // clear checkBoxLists
            ClbOwners.Items.Clear();
            ClbTribes.Items.Clear();
            ClbServers.Items.Clear();
            ClbTags.Items.Clear();

            //// check all creature for info
            var creaturesToCheck = _cc.creatures.Where(c => !c.flags.HasFlag(CreatureFlags.Placeholder)).ToArray();
            foreach (Creature c in creaturesToCheck)
            {
                SetListValue(c.owner, ownerList);
                SetListValue(c.tribe, tribesList);
                SetListValue(c.server, serversList);

                void SetListValue(string _value, Dictionary<string, int> _list)
                {
                    if (string.IsNullOrEmpty(_value))
                    {
                        if (!_list.ContainsKey(string.Empty))
                            _list.Add(string.Empty, 1);
                        else
                            _list[string.Empty]++;
                    }
                    else if (!_list.ContainsKey(_value))
                    {
                        _list.Add(_value, 1);
                    }
                    else
                    {
                        _list[_value]++;
                    }
                }

                // tags
                if (!(c.tags?.Any() ?? false))
                {
                    if (!tagList.ContainsKey(string.Empty))
                        tagList.Add(string.Empty, 1);
                    else
                        tagList[string.Empty]++;
                }
                else
                {
                    for (int t = 0; t < c.tags.Count; t++)
                    {
                        if (!tagList.ContainsKey(c.tags[t]))
                        {
                            tagList.Add(c.tags[t], 1);
                        }
                        else
                        {
                            tagList[c.tags[t]]++;
                        }
                    }
                }
            }

            CbOwnersAll.Checked = CreateCheckboxes(ClbOwners, ownerList, Properties.Settings.Default.FilterHideOwners);
            CbTribesAll.Checked = CreateCheckboxes(ClbTribes, tribesList, Properties.Settings.Default.FilterHideTribes);
            CbServersAll.Checked = CreateCheckboxes(ClbServers, serversList, Properties.Settings.Default.FilterHideServers);
            CbTagsAll.Checked = CreateCheckboxes(ClbTags, tagList, Properties.Settings.Default.FilterHideTags);

            bool IsChecked(string[] list, string name) => list == null || !list.Contains(name);

            bool CreateCheckboxes(CheckedListBox clb, Dictionary<string, int> dict, string[] hiddenList)
            {
                bool allChecked = true;
                foreach (var entry in dict.OrderBy(o => o.Key))
                {
                    bool isChecked = IsChecked(hiddenList, entry.Key);
                    clb.Items.Add($"{entry.Key} ({entry.Value})", isChecked);
                    allChecked = allChecked && isChecked;
                }

                return allChecked;
            }
        }

        private void CbStatusCheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox cb)) return;

            cb.BackColor = CheckBoxColor(cb.Checked);
        }

        private static Color CheckBoxColor(bool isChecked) => isChecked ? Color.LightSalmon : Color.LightGreen;

        private void SetAllChecked(CheckedListBox clb, bool isChecked)
        {
            int count = clb.Items.Count;
            for (int i = 0; i < count; i++)
                clb.SetItemChecked(i, isChecked);
        }

        private void CbOwnersAll_CheckedChanged(object sender, EventArgs e)
        {
            SetAllChecked(ClbOwners, CbOwnersAll.Checked);
        }

        private void CbTribesAll_CheckedChanged(object sender, EventArgs e)
        {
            SetAllChecked(ClbTribes, CbTribesAll.Checked);
        }

        private void CbServersAll_CheckedChanged(object sender, EventArgs e)
        {
            SetAllChecked(ClbServers, CbServersAll.Checked);
        }

        private void CbTagsAll_CheckedChanged(object sender, EventArgs e)
        {
            SetAllChecked(ClbTags, CbTagsAll.Checked);
        }

        private void CbStatusAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = (sender as CheckBox)?.Checked ?? false;
            foreach (var cb in _statusCheckBoxes)
                cb.Checked = isChecked;
        }

        /// <summary>
        /// Returns the list of items that is not checked, i.e. that should be hidden.
        /// </summary>
        /// <param name="cbList"></param>
        /// <returns></returns>
        private string[] GetCheckedStrings(CheckedListBox cbList)
        {
            var list = new List<string>();
            var count = cbList.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (!cbList.GetItemChecked(i))
                {
                    list.Add(Regex.Match(cbList.Items[i].ToString(), @"^(.*?)(?: \(\d+\))?$").Groups[1].Value);
                }
            }

            return list.ToArray();
        }

        private void BtApply_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FilterHideOwners = GetCheckedStrings(ClbOwners);
            Properties.Settings.Default.FilterHideTribes = GetCheckedStrings(ClbTribes);
            Properties.Settings.Default.FilterHideServers = GetCheckedStrings(ClbServers);
            Properties.Settings.Default.FilterHideTags = GetCheckedStrings(ClbTags);

            var hiddenCreatureFlags = CreatureFlags.None;
            foreach (var cb in _statusCheckBoxes)
            {
                if (cb.Checked)
                    hiddenCreatureFlags |= (CreatureFlags)cb.Tag;
            }

            Properties.Settings.Default.FilterHideCreaturesFlags = (int)hiddenCreatureFlags;
            Properties.Settings.Default.useFiltersInTopStatCalculation = CbUseFilterInTopStatCalculation.Checked;
            Properties.Settings.Default.FilterOnlyIfColorId = _selectedColorFilter;
            Properties.Settings.Default.LibraryGroupBySpecies = CbLibraryGroupSpecies.Checked;
        }

        private void BtClearColorFilters_Click(object sender, EventArgs e)
        {
            SetColorFilter(0);
        }

        private void BtColorFilter_Click(object sender, EventArgs e)
        {
            _colorPicker.SetColors(_selectedColorFilter, Loc.s("onlyCreaturesWithThisColor"));
            if (_colorPicker.ShowDialog() == DialogResult.OK)
            {
                SetColorFilter(_colorPicker.SelectedColorId);
            }
        }

        private void SetColorFilter(int colorId)
        {
            _selectedColorFilter = colorId;
            CreatureColors.CreatureColorName(colorId);
            var color = CreatureColors.CreatureArkColor(colorId);
            BtColorFilter.BackColor = color.color;
            BtColorFilter.ForeColor = Utils.ForeColor(color.color);
            BtColorFilter.Text = $"{colorId} - {color.name}";
        }

        private void Localization()
        {
            Text = Loc.s("libraryFilter");
            BtApply.Text = Loc.s("apply");
            BtCancel.Text = Loc.s("Cancel");

            LbOwners.Text = Loc.s("owners");
            LbTribes.Text = Loc.s("tribes");
            LbServers.Text = Loc.s("servers");
            LbTags.Text = Loc.s("tags");
            LbStatus.Text = Loc.s("hideStatus");
            LbColors.Text = Loc.s("Colors");
            BtClearColorFilters.Text = Loc.s("clearColorsFilters");
            CbUseFilterInTopStatCalculation.Text = Loc.s("useFilterInTopStatCalculation");
            CbLibraryGroupSpecies.Text = Loc.s("groupLibraryBySpecies");

            string allString = Loc.s("All");

            CbOwnersAll.Text = allString;
            CbTribesAll.Text = allString;
            CbServersAll.Text = allString;
            CbTagsAll.Text = allString;
            CbStatusAll.Text = allString;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.uiControls
{
    public partial class LibraryFilter : Form
    {
        private readonly CreatureCollection _cc;
        private List<Button> _statusButtons;
        private byte _selectedColorFilter;
        private readonly MyColorPicker _colorPicker;
        private readonly (string, string)[] _maturationFilter = new (string, string)[]
        {
            ("FilterHideAdults", Loc.S("mature")),
            ("FilterHideNonAdults", Loc.S("non mature")),
            ("FilterHideCooldowns", Loc.S("cooldown")),
            ("FilterHideNonCooldowns", Loc.S("non cooldown"))
        };

        public LibraryFilter()
        {
            InitializeComponent();
        }

        public LibraryFilter(CreatureCollection cc) : this()
        {
            _cc = cc;

            _colorPicker = new MyColorPicker();

            SetColorFilter(Properties.Settings.Default.FilterOnlyIfColorId);
            CbUseFilterInTopStatCalculation.Checked = Properties.Settings.Default.useFiltersInTopStatCalculation;
            CbLibraryGroupSpecies.Checked = Properties.Settings.Default.LibraryGroupBySpecies;

            UpdateOwnerServerTagLists();

            Localization();
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
            _statusButtons = new List<Button>(statusList.Length);
            int statusButtonWidth = FlpStatus.Width - 6;
            ButtonState buttonState;
            Button b;
            foreach (var s in statusList)
            {
                buttonState = ButtonState.Neutral;
                if ((Properties.Settings.Default.FilterFlagsOneNeeded & (int)s) != 0)
                    buttonState = ButtonState.OneNeeded;
                else if ((Properties.Settings.Default.FilterFlagsAllNeeded & (int)s) != 0)
                    buttonState = ButtonState.AllNeeded;
                else if ((Properties.Settings.Default.FilterFlagsExclude & (int)s) != 0)
                    buttonState = ButtonState.Exclude;

                b = new Button
                {
                    Text = s.ToString(),
                    Tag = (s, buttonState),
                    BackColor = ColorButtonState(buttonState),
                    Width = statusButtonWidth
                };
                FlpStatus.Controls.Add(b);
                FlpStatus.SetFlowBreak(b, true);
                _statusButtons.Add(b);
                b.Click += BtStatusClicked;
            }

            // maturation filter
            var maturationCheckBoxAll = true;
            foreach (var mf in _maturationFilter)
            {
                var isChecked = !(Properties.Settings.Default[mf.Item1] as bool? ?? false);
                if (!isChecked) maturationCheckBoxAll = false;
                ClbMaturationFilters.Items.Add(mf.Item2, isChecked);
            }

            CbMaturationAll.Checked = maturationCheckBoxAll;

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

                void SetListValue(string stringValue, Dictionary<string, int> list)
                {
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        if (!list.ContainsKey(string.Empty))
                            list.Add(string.Empty, 1);
                        else
                            list[string.Empty]++;
                    }
                    else if (!list.ContainsKey(stringValue))
                    {
                        list.Add(stringValue, 1);
                    }
                    else
                    {
                        list[stringValue]++;
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

        private void BtStatusClicked(object sender, EventArgs e)
        {
            if (!(sender is Button b)) return;

            (var flag, var state) = ((CreatureFlags, ButtonState))b.Tag;
            state = NextState(state);
            b.BackColor = ColorButtonState(state);
            b.Tag = (flag, state);
        }

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

        private void CbMaturationAll_CheckedChanged(object sender, EventArgs e)
        {
            SetAllChecked(ClbMaturationFilters, CbMaturationAll.Checked);
        }

        private void BtClearFlagFilter_Click(object sender, EventArgs e)
        {
            ButtonState state = ButtonState.Neutral;
            foreach (var b in _statusButtons)
            {
                b.Tag = ((((CreatureFlags, ButtonState))b.Tag).Item1, state);
                b.BackColor = ColorButtonState(state);
            }
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
            Properties.Settings.Default.LibraryGroupBySpecies = CbLibraryGroupSpecies.Checked;

            var flagsOneNeeded = CreatureFlags.None;
            var flagsAllNeeded = CreatureFlags.None;
            var flagsExclude = CreatureFlags.None;
            foreach (var b in _statusButtons)
            {
                switch ((((CreatureFlags, ButtonState))b.Tag).Item2)
                {
                    case ButtonState.OneNeeded:
                        flagsOneNeeded |= (((CreatureFlags, ButtonState))b.Tag).Item1;
                        break;
                    case ButtonState.AllNeeded:
                        flagsAllNeeded |= (((CreatureFlags, ButtonState))b.Tag).Item1;
                        break;
                    case ButtonState.Exclude:
                        flagsExclude |= (((CreatureFlags, ButtonState))b.Tag).Item1;
                        break;
                }
            }

            Properties.Settings.Default.FilterFlagsOneNeeded = (int)flagsOneNeeded;
            Properties.Settings.Default.FilterFlagsAllNeeded = (int)flagsAllNeeded;
            Properties.Settings.Default.FilterFlagsExclude = (int)flagsExclude;
            Properties.Settings.Default.useFiltersInTopStatCalculation = CbUseFilterInTopStatCalculation.Checked;
            Properties.Settings.Default.FilterOnlyIfColorId = _selectedColorFilter;

            var i = 0;
            foreach (var mf in _maturationFilter)
                Properties.Settings.Default[mf.Item1] = !ClbMaturationFilters.GetItemChecked(i++);

        }

        private void BtClearColorFilters_Click(object sender, EventArgs e)
        {
            SetColorFilter(0);
        }

        private void BtColorFilter_Click(object sender, EventArgs e)
        {
            _colorPicker.PickColor(_selectedColorFilter, Loc.S("onlyCreaturesWithThisColor"));
            if (_colorPicker.ShowDialog() == DialogResult.OK)
            {
                SetColorFilter(_colorPicker.SelectedColorId);
            }
        }

        private void SetColorFilter(byte colorId)
        {
            _selectedColorFilter = colorId;
            CreatureColors.CreatureColorName(colorId);
            var color = CreatureColors.CreatureArkColor(colorId);
            BtColorFilter.SetBackColorAndAccordingForeColor(color.Color);
            BtColorFilter.Text = $"{colorId} - {color.Name}";
        }

        private void Localization()
        {
            Text = Loc.S("libraryFilter");
            BtApply.Text = Loc.S("apply");
            BtCancel.Text = Loc.S("Cancel");

            LbOwners.Text = Loc.S("owners");
            LbTribes.Text = Loc.S("tribes");
            LbServers.Text = Loc.S("servers");
            LbTags.Text = Loc.S("tags");
            LbStatus.Text = Loc.S("Status");
            LbColors.Text = Loc.S("Colors");
            LbMaturation.Text = Loc.S("Maturation");
            BtClearColorFilters.Text = Loc.S("clearColorsFilters");
            CbUseFilterInTopStatCalculation.Text = Loc.S("useFilterInTopStatCalculation");
            CbLibraryGroupSpecies.Text = Loc.S("groupLibraryBySpecies");
            BtClearFlagFilter.Text = Loc.S("clear");

            string allString = Loc.S("All");

            CbOwnersAll.Text = allString;
            CbTribesAll.Text = allString;
            CbServersAll.Text = allString;
            CbTagsAll.Text = allString;

            FlpStatus.Controls.Add(new Label { Text = Loc.S("filterOneNeededInfo"), BackColor = ColorButtonState(ButtonState.OneNeeded), AutoSize = true, Padding = new Padding(5), Margin = new Padding(3) });
            FlpStatus.Controls.Add(new Label { Text = Loc.S("filterAllNeededInfo"), BackColor = ColorButtonState(ButtonState.AllNeeded), AutoSize = true, Padding = new Padding(5), Margin = new Padding(3) });
            FlpStatus.Controls.Add(new Label { Text = Loc.S("filterExcludeInfo"), BackColor = ColorButtonState(ButtonState.Exclude), AutoSize = true, Padding = new Padding(5), Margin = new Padding(3) });
        }

        private enum ButtonState
        {
            Neutral, OneNeeded, AllNeeded, Exclude
        }

        private ButtonState NextState(ButtonState currentState)
        {
            switch (currentState)
            {
                case ButtonState.OneNeeded: return ButtonState.AllNeeded;
                case ButtonState.AllNeeded: return ButtonState.Exclude;
                case ButtonState.Exclude: return ButtonState.Neutral;
                default: return ButtonState.OneNeeded;
            }
        }

        private Color ColorButtonState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.OneNeeded: return Color.LightGreen;
                case ButtonState.AllNeeded: return Color.LightSkyBlue;
                case ButtonState.Exclude: return Color.LightSalmon;
                default: return Color.LightGray;
            }
        }
    }
}

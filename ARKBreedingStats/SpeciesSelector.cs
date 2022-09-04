using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class SpeciesSelector : UserControl
    {
        /// <summary>
        /// Is invoked if a species was selected. The parameter is true if the species was changed.
        /// </summary>
        public event Action<bool> OnSpeciesSelected;

        /// <summary>
        /// Toggles the visibility of this control.
        /// </summary>
        public event Action<bool> ToggleVisibility;

        /// <summary>
        /// The currently selected species
        /// </summary>
        public Species SelectedSpecies { get; private set; }

        /// <summary>
        /// Items for the species list.
        /// </summary>
        private List<SpeciesListEntry> _entryList;

        /// <summary>
        /// The TextBox control for the species searching which is outside of this control.
        /// </summary>
        private TextBoxSuggest _textBox;

        private bool _ignoreTextBoxChange;

        /// <summary>
        /// List of species-blueprintPaths last used by the user
        /// </summary>
        private List<string> _lastSpeciesBPs;
        private List<string> _iconIndices;
        private readonly Debouncer _speciesChangeDebouncer = new Debouncer();

        internal readonly VariantSelector VariantSelector;

        public SpeciesSelector()
        {
            InitializeComponent();
            _lastSpeciesBPs = new List<string>();
            SplitterDistance = Properties.Settings.Default.SpeciesSelectorVerticalSplitterDistance;
            VariantSelector = new VariantSelector();
        }

        /// <summary>
        /// Initializes the species lists.
        /// </summary>
        /// <param name="species"></param>
        /// <param name="aliases"></param>
        public void SetSpeciesLists(List<Species> species, Dictionary<string, string> aliases)
        {
            if (SelectedSpecies != null)
            {
                SelectedSpecies = Values.V.SpeciesByBlueprint(SelectedSpecies.blueprintPath);
            }

            if (SelectedSpecies == null)
            {
                // if after loading a new file the previously selected species is not available (e.g. previous species is from a now not loaded mod), select first available species
                SetSpecies(species.FirstOrDefault(), ignoreInRecent: true);
            }

            InitializeSpeciesImages(species);

            _entryList = CreateSpeciesList(species, aliases);

            // autocomplete for species-input
            var al = new AutoCompleteStringCollection();
            al.AddRange(_entryList.Select(e => e.SearchName).ToArray());
            _textBox.AutoCompleteCustomSource = al;

            VariantSelector.SetVariants(species);
            cbDisplayUntameable.Checked = Properties.Settings.Default.DisplayNonDomesticableSpecies;

            TextBoxTextChanged(null, null);
        }

        private static List<SpeciesListEntry> CreateSpeciesList(List<Species> species, Dictionary<string, string> aliases)
        {
            Dictionary<string, Species> speciesNameToSpecies = new Dictionary<string, Species>();

            var entryList = new List<SpeciesListEntry>();

            foreach (var s in species)
            {
                if (!speciesNameToSpecies.ContainsKey(s.DescriptiveNameAndMod))
                    speciesNameToSpecies.Add(s.DescriptiveNameAndMod, s);

                entryList.Add(new SpeciesListEntry
                {
                    DisplayName = s.name,
                    SearchName = s.name,
                    ModName = s.Mod?.title ?? string.Empty,
                    Species = s
                });
            }

            foreach (var a in aliases)
            {
                if (speciesNameToSpecies.TryGetValue(a.Value, out var aliasSpecies))
                {
                    entryList.Add(new SpeciesListEntry
                    {
                        DisplayName = a.Key + " (→" + aliasSpecies.name + ")",
                        SearchName = a.Key,
                        Species = aliasSpecies,
                        ModName = aliasSpecies.Mod?.title ?? string.Empty,
                    });
                }
            }

            entryList = entryList.OrderBy(s => s.DisplayName).ToList();
            return entryList;
        }

        public void InitializeSpeciesImages(List<Species> species)
        {
            var creatureColors = new byte[] { 44, 42, 57, 10, 26, 78 }; // uniform color pattern that is used for all species in the selector
            var creatureColorsPolar = new byte[] { 18, 18, 18, 18, 18, 18 }; // uniform color pattern that is used for all polar species in the selector
            var lImgList = new ImageList();
            _iconIndices = new List<string>();
            bool imageFolderExist = !string.IsNullOrEmpty(CreatureColored.ImageFolder) && Directory.Exists(CreatureColored.ImageFolder);
            //var rand = new Random();

            //var speciesWOImage = new List<string>();// to determine which species have no image yet
            if (imageFolderExist)
            {
                foreach (Species s in species)
                {
                    //var colors = s.RandomSpeciesColors(rand);

                    var (imgExists, imagePath, speciesListName) = CreatureColored.SpeciesImageExists(s,
                        s.name.Contains("Polar") ? creatureColorsPolar : creatureColors
                        //colors
                        );
                    //if (!imgExists && !speciesWOImage.Contains(s.name)) speciesWOImage.Add(s.name);
                    if (!imgExists || _iconIndices.Contains(speciesListName)) continue;

                    try
                    {
                        lImgList.Images.Add(Image.FromFile(imagePath));
                        _iconIndices.Add(speciesListName);
                    }
                    catch (OutOfMemoryException)
                    {
                        // usually this exception occurs if the image file is corrupted
                        if (FileService.TryDeleteFile(imagePath))
                        {
                            (imgExists, imagePath, speciesListName) = CreatureColored.SpeciesImageExists(s,
                                s.name.Contains("Polar") ? creatureColorsPolar : creatureColors);
                            if (imgExists)
                            {
                                try
                                {
                                    lImgList.Images.Add(Image.FromFile(imagePath));
                                    _iconIndices.Add(speciesListName);
                                }
                                catch
                                {
                                    // ignore image if it failed a second time
                                }
                            }
                        }
                    }
                }
            }
            //Clipboard.SetText(speciesWOImage.Any() ? string.Join("\n", speciesWOImage) : string.Empty);

            lImgList.ImageSize = new Size(64, 64);
            lvLastSpecies.LargeImageList = lImgList;
            lvSpeciesInLibrary.LargeImageList = lImgList;
            UpdateLastSpecies();
            UpdateImagesLibraryList();
        }

        /// <summary>
        /// Fills the species listed as appearing in the library
        /// </summary>
        /// <param name="librarySpeciesList"></param>
        public void SetLibrarySpecies(IList<Species> librarySpeciesList)
        {
            lvSpeciesInLibrary.BeginUpdate();
            lvSpeciesInLibrary.Items.Clear();
            var newItems = new List<ListViewItem>();
            foreach (Species s in librarySpeciesList)
            {
                ListViewItem lvi = new ListViewItem
                {
                    Text = s.DescriptiveNameAndMod,
                    Tag = s
                };
                int ii = SpeciesImageIndex(s.name);
                if (ii != -1)
                    lvi.ImageIndex = ii;
                newItems.Add(lvi);
            }
            lvSpeciesInLibrary.Items.AddRange(newItems.ToArray());
            lvSpeciesInLibrary.EndUpdate();
        }

        /// <summary>
        /// Updates the images of the list that displays species of the library.
        /// </summary>
        private void UpdateImagesLibraryList()
        {
            foreach (ListViewItem lvi in lvSpeciesInLibrary.Items)
            {
                int ii = SpeciesImageIndex((lvi.Tag as Species)?.name);
                if (ii != -1)
                    lvi.ImageIndex = ii;
            }
        }

        /// <summary>
        /// Updates the list displaying the last selected species. Also sets the images.
        /// </summary>
        private void UpdateLastSpecies()
        {
            lvLastSpecies.BeginUpdate();
            lvLastSpecies.Items.Clear();
            var newItems = new List<ListViewItem>();
            foreach (string s in _lastSpeciesBPs)
            {
                var species = Values.V.SpeciesByBlueprint(s);
                if (species != null)
                {
                    ListViewItem lvi = new ListViewItem
                    {
                        Text = species.DescriptiveNameAndMod,
                        Tag = species
                    };
                    int ii = SpeciesImageIndex(species.name);
                    if (ii != -1)
                        lvi.ImageIndex = ii;
                    newItems.Add(lvi);
                }
            }
            lvLastSpecies.Items.AddRange(newItems.ToArray());
            lvLastSpecies.EndUpdate();
        }

        private void FilterListWithUnselectedText() => FilterList(_textBox.Text.Substring(0, _textBox.SelectionStart));

        private void FilterList(string part = null)
        {
            if (_entryList == null) return;

            bool noVariantFiltering = VariantSelector.DisabledVariants == null || !VariantSelector.DisabledVariants.Any();
            lvSpeciesList.BeginUpdate();
            lvSpeciesList.Items.Clear();
            var newItems = new List<ListViewItem>();
            bool inputIsEmpty = string.IsNullOrWhiteSpace(part);
            foreach (var s in _entryList)
            {
                if ((Properties.Settings.Default.DisplayNonDomesticableSpecies || s.Species.IsDomesticable)
                    && (inputIsEmpty
                       || s.SearchName.ToLower().Contains(part.ToLower())
                       )
                    && (noVariantFiltering
                        || (string.IsNullOrEmpty(s.Species.VariantInfo) ? !VariantSelector.DisabledVariants.Contains(string.Empty)
                        : !VariantSelector.DisabledVariants.Intersect(s.Species.variants).Any()))
                   )
                {
                    newItems.Add(new ListViewItem(new[] { s.DisplayName, s.Species.VariantInfo, s.Species.IsDomesticable ? "✓" : string.Empty, s.ModName })
                    {
                        Tag = s.Species,
                        BackColor = !s.Species.IsDomesticable ? Color.FromArgb(255, 245, 230)
                        : !string.IsNullOrEmpty(s.ModName) ? Color.FromArgb(230, 245, 255)
                        : SystemColors.Window,
                        ToolTipText = s.Species.blueprintPath,
                    });
                }
            }
            lvSpeciesList.Items.AddRange(newItems.ToArray());
            lvSpeciesList.EndUpdate();

            if (!Visible && !inputIsEmpty)
                ToggleVisibility?.Invoke(true);
        }

        private void lvSpeciesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpeciesList.SelectedItems.Count > 0)
                SetSpecies((Species)lvSpeciesList.SelectedItems[0].Tag, true);
        }

        private void lvOftenUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvLastSpecies.SelectedItems.Count > 0)
                SetSpecies((Species)lvLastSpecies.SelectedItems[0].Tag, true);
        }

        private void lvSpeciesInLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpeciesInLibrary.SelectedItems.Count > 0)
                SetSpecies((Species)lvSpeciesInLibrary.SelectedItems[0].Tag, true);
        }

        /// <summary>
        /// Sets the species with the speciesName. This may not be unique.
        /// </summary>
        /// <param name="speciesName"></param>
        /// <returns>True if the species was recognized and was already or is set.</returns>
        public bool SetSpeciesByName(string speciesName)
        {
            if (Values.V.TryGetSpeciesByName(speciesName, out Species species))
            {
                var speciesWasSet = SetSpecies(species);
                if (speciesWasSet)
                {
                    _ignoreTextBoxChange = true;
                    _textBox.Text = species.name;
                    _ignoreTextBoxChange = false;
                }
                return speciesWasSet;
            }

            return false;
        }

        /// <summary>
        /// Set the current species.
        /// </summary>
        /// <returns>True if the species was recognized and was or is set.</returns>
        public bool SetSpecies(Species species, bool alsoTriggerOnSameSpecies = false, bool ignoreInRecent = false)
        {
            if (species == null) return false;
            if (SelectedSpecies == species)
            {
                if (alsoTriggerOnSameSpecies)
                    OnSpeciesSelected?.Invoke(false);
                return true;
            }

            if (!ignoreInRecent)
            {
                _lastSpeciesBPs.Remove(species.blueprintPath);
                _lastSpeciesBPs.Insert(0, species.blueprintPath);
                if (_lastSpeciesBPs.Count > Properties.Settings.Default.SpeciesSelectorCountLastSpecies
                ) // only keep keepNrLastSpecies of the last species in this list
                    _lastSpeciesBPs.RemoveRange(Properties.Settings.Default.SpeciesSelectorCountLastSpecies,
                        _lastSpeciesBPs.Count - Properties.Settings.Default.SpeciesSelectorCountLastSpecies);
                UpdateLastSpecies();
            }

            SelectedSpecies = species;

            OnSpeciesSelected?.Invoke(true);
            return true;
        }

        public void SetTextBox(TextBoxSuggest textBox)
        {
            _textBox = textBox;
            textBox.TextChanged += TextBoxTextChanged;
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (!_ignoreTextBoxChange)
                _speciesChangeDebouncer.Debounce(300, FilterListWithUnselectedText, Dispatcher.CurrentDispatcher);
        }

        public string[] LastSpecies
        {
            get => _lastSpeciesBPs.ToArray();
            set
            {
                if (value == null)
                    _lastSpeciesBPs.Clear();
                else
                {
                    _lastSpeciesBPs = value.ToList();
                    UpdateLastSpecies();
                }
            }
        }

        private int SpeciesImageIndex(string speciesName = null)
        {
            if (_iconIndices == null) return -1;

            if (string.IsNullOrWhiteSpace(speciesName))
                speciesName = SelectedSpecies.name;
            else speciesName = Values.V.SpeciesName(speciesName);
            speciesName = CreatureColored.SpeciesImageName(speciesName, false);
            return _iconIndices.IndexOf(speciesName);
        }

        public Image SpeciesImage(string speciesName = null)
        {
            if (lvLastSpecies.LargeImageList == null) return null;
            int ii = SpeciesImageIndex(speciesName);
            if (ii != -1 && ii < lvLastSpecies.LargeImageList.Images.Count)
                return lvLastSpecies.LargeImageList.Images[ii];
            return null;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            OnSpeciesSelected?.Invoke(false);
        }

        private void cbDisplayUntameable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DisplayNonDomesticableSpecies = ((CheckBox)sender).Checked;
            TextBoxTextChanged(null, null);
        }

        public int SplitterDistance
        {
            get => splitContainer2.SplitterDistance;
            set => splitContainer2.SplitterDistance = value;
        }

        private void BtVariantFilter_Click(object sender, EventArgs e)
        {
            VariantSelector.InitializeCheckStates();
            if (VariantSelector.ShowDialog() == DialogResult.OK)
                TextBoxTextChanged(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VariantSelector.FilterToDefault();
            TextBoxTextChanged(null, null);
        }
    }

    class SpeciesListEntry
    {
        internal string SearchName;
        internal string DisplayName;
        internal string ModName;
        internal Species Species;
        public override string ToString() => DisplayName;
    }
}

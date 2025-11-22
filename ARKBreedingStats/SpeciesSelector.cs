using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.SpeciesImages;
using ARKBreedingStats.uiControls;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

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
        /// <summary>
        /// Key is the blueprint path of the species. Value is the index of the according image in the ListView.Images.
        /// </summary>
        private readonly Dictionary<string, int> _iconIndices = new Dictionary<string, int>();
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
        /// <param name="game">Can be ASE or ASA (some species use different images depending on the game)</param>
        public void SetSpeciesLists(List<Species> species, Dictionary<string, string> aliases, string game = null)
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

            InitializeSpeciesImages();

            _entryList = CreateSpeciesList(species, aliases);

            // autocomplete for species-input
            var al = new AutoCompleteStringCollection();
            al.AddRange(_entryList.Select(e => e.DisplayName).Distinct().ToArray());
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
                    SearchName = (s.name + " " + s.blueprintPath).ToLowerInvariant(),
                    ModName = s.Mod?.title ?? string.Empty,
                    Species = s
                });
                if (!string.IsNullOrEmpty(s.nameFemale) && s.name != s.nameFemale)
                    entryList.Add(new SpeciesListEntry
                    {
                        DisplayName = s.nameFemale + " (→" + s.name + ")",
                        SearchName = s.nameFemale.ToLowerInvariant(),
                        ModName = s.Mod?.title ?? string.Empty,
                        Species = s
                    });
                if (!string.IsNullOrEmpty(s.nameMale) && s.name != s.nameMale)
                    entryList.Add(new SpeciesListEntry
                    {
                        DisplayName = s.nameMale + " (→" + s.name + ")",
                        SearchName = s.nameMale.ToLowerInvariant(),
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
                        SearchName = a.Key.ToLowerInvariant(),
                        ModName = aliasSpecies.Mod?.title ?? string.Empty,
                        Species = aliasSpecies
                    });
                }
            }

            entryList = entryList.OrderBy(s => s.DisplayName)
                .ThenBy(s => !s.Species.IsDomesticable) // prefer domesticable species variants
                .ThenBy(s => s.Species.Mod == null) // prefer loaded mod species
                .ThenBy(s => s.Species.variants?.Length ?? 0) // prefer species that are not variants
                .ToList();
            return entryList;
        }

        /// <summary>
        /// Resets the species images.
        /// </summary>
        public void InitializeSpeciesImages()
        {
            var lvLargeImageList = lvLastSpecies.LargeImageList ?? new ImageList();
            ClearListImages();

            lvLargeImageList.ImageSize = new Size(64, 64);
            lvLargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            lvLastSpecies.LargeImageList = lvLargeImageList;
            lvSpeciesInLibrary.LargeImageList = lvLargeImageList;
            UpdateLastSpecies();
            UpdateImagesLibraryList();
        }

        /// <summary>
        /// Clear list images. Call when image packs were changed.
        /// </summary>
        private void ClearListImages()
        {
            var lvLargeImageList = lvLastSpecies.LargeImageList;
            if (lvLargeImageList == null) return;
            foreach (Image i in lvLargeImageList.Images)
                i.Dispose();
            lvLargeImageList.Images.Clear();
            _iconIndices.Clear();
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
            var game = CreatureCollection.CurrentCreatureCollection?.Game ?? Ark.Asa;
            foreach (Species s in librarySpeciesList)
            {
                ListViewItem lvi = new ListViewItem
                {
                    Text = s.DescriptiveNameAndMod,
                    Tag = s
                };
                SetListViewItemImageIndex(lvi, s);
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
                SetListViewItemImageIndex(lvi, lvi.Tag as Species);
        }

        /// <summary>
        /// Updates the list displaying the last selected species. Also sets the images.
        /// </summary>
        private void UpdateLastSpecies()
        {
            lvLastSpecies.BeginUpdate();
            lvLastSpecies.Items.Clear();
            var newItems = new List<ListViewItem>();
            foreach (var s in _lastSpeciesBPs)
            {
                var species = Values.V.SpeciesByBlueprint(s);
                if (species == null) continue;
                var lvi = new ListViewItem
                {
                    Text = species.DescriptiveNameAndMod,
                    Tag = species
                };
                SetListViewItemImageIndex(lvi, species);
                newItems.Add(lvi);
            }
            lvLastSpecies.Items.AddRange(newItems.ToArray());
            lvLastSpecies.EndUpdate();
        }

        private void FilterListWithUnselectedText() => FilterList(_textBox.Text.Substring(0, _textBox.SelectionStart));

        private void FilterList(string part = null)
        {
            if (_entryList == null) return;

            bool noVariantFiltering = VariantSelector.DisabledVariants == null || !VariantSelector.DisabledVariants.Any();
            var newItems = new List<ListViewItem>();
            part = part?.ToLowerInvariant();
            bool inputIsEmpty = string.IsNullOrWhiteSpace(part);
            foreach (var s in _entryList)
            {
                if ((Properties.Settings.Default.DisplayNonDomesticableSpecies || s.Species.IsDomesticable)
                    && (inputIsEmpty
                       || s.SearchName.Contains(part)
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
                        : !string.IsNullOrEmpty(s.ModName) && s.ModName != "Ark: Survival Ascended" ? Color.FromArgb(230, 245, 255)
                        : SystemColors.Window,
                        ToolTipText = s.Species.blueprintPath,
                    });
                }
            }
            lvSpeciesList.BeginUpdate();
            lvSpeciesList.Items.Clear();
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
        /// Return species by entry string.
        /// </summary>
        public bool SetSpeciesByEntryName(string entryString)
        {
            if (_entryList == null || string.IsNullOrEmpty(entryString)) return false;
            var species = _entryList.FirstOrDefault(e => e.DisplayName.Equals(entryString, StringComparison.OrdinalIgnoreCase))?.Species;
            if (species == null) return false;
            SetSpeciesByName(null, species);
            return true;
        }

        /// <summary>
        /// Sets the species with the speciesName. This may not be unique.
        /// </summary>
        /// <returns>True if the species was recognized and was already or is set.</returns>
        public bool SetSpeciesByName(string speciesName, Species species = null)
        {
            if (species != null
                || (!string.IsNullOrEmpty(speciesName) && Values.V.TryGetSpeciesByName(speciesName, out species)))
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

            if (!ignoreInRecent && _lastSpeciesBPs.FirstOrDefault() != species.blueprintPath)
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

        /// <summary>
        /// Asynchronously retrieves the index of the image associated with the specified species in the species selector.
        /// </summary>
        /// <param name="species">The species for which the image index is to be retrieved.</param>
        /// <param name="game">The game context (e.g. ASA). If not provided, the current game in the creature collection is used.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the index of the species image in the image list.
        /// Returns -1 if the species is null, the image list is unavailable, or the image could not be loaded.
        /// </returns>
        private static async Task<Image> SpeciesListImage(Species species, string game = null)
        {
            if (species == null) return null;

            if (string.IsNullOrEmpty(game))
                game = CreatureCollection.CurrentCreatureCollection?.Game ?? Ark.Asa;
            var colorIds = species.name.Contains("Polar")
                ? new byte[] { 18, 18, 18, 18, 18, 18 } // uniform color pattern that is used for all polar species in the selector
                //: new byte[] { 44, 42, 57, 10, 26, 78 }
                : species.RandomSpeciesColors()
                ;
            var imagePath = await CreatureImageFile.GetSpeciesImageForSpeciesList(species, colorIds, game);
            if (imagePath == null) return null;

            try
            {
                // use temp bitmap to avoid persistent file locking
                using (var bmpTemp = new Bitmap(imagePath))
                    return new Bitmap(bmpTemp);
            }
            catch (OutOfMemoryException)
            {
                // usually this exception occurs if the image file is corrupted
                if (FileService.TryDeleteFile(imagePath))
                {
                    imagePath =
                        await CreatureImageFile.GetSpeciesImageForSpeciesList(species, colorIds, game);
                    if (imagePath == null) return null;

                    try
                    {
                        // use temp bitmap to avoid persistent file locking
                        using (var bmpTemp = new Bitmap(imagePath))
                            return new Bitmap(bmpTemp);
                    }
                    catch
                    {
                        // ignore image if it failed a second time
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            return null;
        }

        private void SetListViewItemImageIndex(ListViewItem lvi, Species species)
        {
            if (lvi == null) return;
            if (species == null) species = SelectedSpecies;
            if (species == null) return;

            if (_iconIndices.TryGetValue(species.blueprintPath, out var index))
            {
                lvi.ImageIndex = index;
                return;
            }

            Task.Run(async () =>
            {
                var img = await SpeciesListImage(species);
                this.Invoke(new Action(() =>
                {
                    var imageList = lvLastSpecies.LargeImageList.Images;
                    index = img == null ? -1 : imageList.Count;
                    if (img != null)
                        imageList.Add(img);
                    lvi.ImageIndex = index;
                    _iconIndices[species.blueprintPath] = index;
                }));
            });
        }

        public Image SpeciesImage(Species species = null)
        {
            species = species ?? SelectedSpecies;
            if (lvLastSpecies.LargeImageList != null
                && !string.IsNullOrEmpty(species?.blueprintPath)
                && _iconIndices.TryGetValue(species.blueprintPath, out var i)
                && i >= 0 && i < lvLastSpecies.LargeImageList.Images.Count)
                return lvLastSpecies.LargeImageList.Images[i];
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

        /// <summary>
        /// Selects the last user selected species.
        /// </summary>
        internal bool SetToLastSetSpecies() => SetSpecies(Values.V.SpeciesByBlueprint(LastSpecies?.FirstOrDefault()));

        /// <summary>
        /// If no species is selected, set to last selected species, or first available species.
        /// </summary>
        public void EnsureSelectedSpecies()
        {
            if (SelectedSpecies != null) return;
            if (SetToLastSetSpecies()) return;
            if (Values.V.species.Any())
                SetSpecies(Values.V.species[0]);
        }
    }

    class SpeciesListEntry
    {
        /// <summary>
        /// Used for search filter, expected lower case.
        /// </summary>
        internal string SearchName;
        internal string DisplayName;
        internal string ModName;
        internal Species Species;
        public override string ToString() => DisplayName;
    }
}

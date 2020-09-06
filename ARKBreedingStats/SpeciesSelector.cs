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
            _iconIndices = new List<string>();
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

            ImageList imageList;
            (_entryList, imageList, _iconIndices) = LoadSpeciesImagesAndCreateSpeciesList(species, aliases);

            imageList.ImageSize = new Size(64, 64);
            lvLastSpecies.LargeImageList = imageList;
            lvSpeciesInLibrary.LargeImageList = imageList;

            // autocomplete for species-input
            var al = new AutoCompleteStringCollection();
            al.AddRange(_entryList.Select(e => e.SearchName).ToArray());
            _textBox.AutoCompleteCustomSource = al;

            VariantSelector.SetVariants(species);
            cbDisplayUntameable.Checked = Properties.Settings.Default.DisplayNonDomesticableSpecies;

            Textbox_TextChanged(null, null);
        }

        private static (List<SpeciesListEntry>, ImageList, List<string>) LoadSpeciesImagesAndCreateSpeciesList(List<Species> species, Dictionary<string, string> aliases)
        {
            Dictionary<string, Species> speciesNameToSpecies = new Dictionary<string, Species>();

            var creatureColors = new int[]
                {44, 42, 57, 10, 26, 78}; // uniform color pattern that is used for all species in the selector
            var creatureColorsPolar = new int[]
                {18, 18, 18, 18, 18, 18}; // uniform color pattern that is used for all polar species in the selector
            ImageList lImgList = new ImageList();
            var iconIndices = new List<string>();
            bool imageFolderExist = Directory.Exists(FileService.GetPath(FileService.ImageFolderName));

            //var speciesWOImage = new List<string>();// to determine which species have no image yet
            foreach (Species ss in species)
            {
                if (!speciesNameToSpecies.ContainsKey(ss.DescriptiveNameAndMod))
                    speciesNameToSpecies.Add(ss.DescriptiveNameAndMod, ss);

                if (imageFolderExist)
                {
                    var (imgExists, imagePath, speciesListName) = CreatureColored.SpeciesImageExists(ss,
                        ss.name.Contains("Polar") ? creatureColorsPolar : creatureColors);
                    if (imgExists && !iconIndices.Contains(speciesListName))
                    {
                        lImgList.Images.Add(Image.FromFile(imagePath));
                        iconIndices.Add(speciesListName);
                    }

                    //if (!imgExists && !speciesWOImage.Contains(ss.name)) speciesWOImage.Add(ss.name);
                }
            }
            //Clipboard.SetText(string.Join("\n", speciesWOImage));

            var entryList = new List<SpeciesListEntry>();

            foreach (var s in species)
            {
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
                if (speciesNameToSpecies.ContainsKey(a.Value))
                {
                    entryList.Add(new SpeciesListEntry
                    {
                        DisplayName = a.Key + " (→" + speciesNameToSpecies[a.Value].name + ")",
                        SearchName = a.Key,
                        Species = speciesNameToSpecies[a.Value],
                        ModName = speciesNameToSpecies[a.Value].Mod?.title ?? string.Empty,
                    });
                }
            }

            entryList = entryList.OrderBy(s => s.DisplayName).ToList();
            return (entryList, lImgList, iconIndices);
        }

        /// <summary>
        /// Fills the species listed as appearing in the library
        /// </summary>
        /// <param name="librarySpeciesList"></param>
        public void SetLibrarySpecies(List<Species> librarySpeciesList)
        {
            lvSpeciesInLibrary.Items.Clear();
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
                lvSpeciesInLibrary.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Updates the list displaying the last selected species.
        /// </summary>
        private void UpdateLastSpecies()
        {
            lvLastSpecies.Items.Clear();
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
                    lvLastSpecies.Items.Add(lvi);
                }
            }
        }

        private void FilterListWithUnselectedText() => FilterList(_textBox.Text.Substring(0, _textBox.SelectionStart));

        private void FilterList(string part = null)
        {
            if (_entryList == null) return;

            bool noVariantFiltering = VariantSelector.DisabledVariants == null || !VariantSelector.DisabledVariants.Any();
            lvSpeciesList.BeginUpdate();
            lvSpeciesList.Items.Clear();
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
                    lvSpeciesList.Items.Add(new ListViewItem(new[] { s.DisplayName, s.Species.VariantInfo, s.Species.IsDomesticable ? "✓" : string.Empty, s.ModName })
                    {
                        Tag = s.Species,
                        BackColor = !s.Species.IsDomesticable ? Color.FromArgb(255, 245, 230)
                        : !string.IsNullOrEmpty(s.ModName) ? Color.FromArgb(230, 245, 255)
                        : SystemColors.Window,
                        ToolTipText = s.Species.blueprintPath,
                    });
                }
            }
            lvSpeciesList.EndUpdate();
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
        public void SetSpeciesByName(string speciesName)
        {
            if (Values.V.TryGetSpeciesByName(speciesName, out Species species))
            {
                SetSpecies(species);
            }
        }

        public void SetSpecies(Species species, bool alsoTriggerOnSameSpecies = false)
        {
            if (species == null) return;
            if (SelectedSpecies == species)
            {
                if (alsoTriggerOnSameSpecies)
                    OnSpeciesSelected?.Invoke(false);
                return;
            }

            _lastSpeciesBPs.Remove(species.blueprintPath);
            _lastSpeciesBPs.Insert(0, species.blueprintPath);
            if (_lastSpeciesBPs.Count > Properties.Settings.Default.SpeciesSelectorCountLastSpecies) // only keep keepNrLastSpecies of the last species in this list
                _lastSpeciesBPs.RemoveRange(Properties.Settings.Default.SpeciesSelectorCountLastSpecies, _lastSpeciesBPs.Count - Properties.Settings.Default.SpeciesSelectorCountLastSpecies);
            UpdateLastSpecies();
            SelectedSpecies = species;

            OnSpeciesSelected?.Invoke(true);
        }

        public void SetTextBox(TextBoxSuggest textbox)
        {
            _textBox = textbox;
            textbox.TextChanged += Textbox_TextChanged;
        }

        private void Textbox_TextChanged(object sender, EventArgs e)
        {
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
            Textbox_TextChanged(null, null);
        }

        public int SplitterDistance
        {
            get => splitContainer2.SplitterDistance;
            set => splitContainer2.SplitterDistance = value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VariantSelector.InitializeCheckStates();
            if (VariantSelector.ShowDialog() == DialogResult.OK)
                Textbox_TextChanged(null, null);
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

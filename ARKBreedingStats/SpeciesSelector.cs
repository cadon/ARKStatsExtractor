using ARKBreedingStats.species;
using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private List<SpeciesListEntry> entryList;

        /// <summary>
        /// The TextBox control for the species searching which is outside of this control.
        /// </summary>
        private uiControls.TextBoxSuggest textbox;

        /// <summary>
        /// List of species-blueprintpaths last used by the user
        /// </summary>
        private List<string> lastSpeciesBPs;
        private readonly List<string> iconIndices;
        private CancellationTokenSource cancelSource;

        public SpeciesSelector()
        {
            InitializeComponent();
            lastSpeciesBPs = new List<string>();
            iconIndices = new List<string>();
            SplitterDistance = Properties.Settings.Default.SpeciesSelectorVerticalSplitterDistance;
        }

        /// <summary>
        /// Initializes the species lists.
        /// </summary>
        /// <param name="species"></param>
        /// <param name="aliases"></param>
        public void SetSpeciesLists(List<Species> species, Dictionary<string, string> aliases)
        {
            Dictionary<string, Species> speciesNameToSpecies = new Dictionary<string, Species>();

            var creatureColors = new int[] { 44, 42, 57, 10, 26, 78 }; // uniform color pattern that is used for all species in the selector
            ImageList lImgList = new ImageList();

            //var speciesWOImage = new List<string>();// TODO debug
            foreach (Species ss in species)
            {
                if (!speciesNameToSpecies.ContainsKey(ss.DescriptiveNameAndMod))
                    speciesNameToSpecies.Add(ss.DescriptiveNameAndMod, ss);

                var (imgExists, imagePath, speciesListName) = CreatureColored.SpeciesImageExists(ss, ss.name.Contains("Polar") ? new[] { 18, 18, 18, 18, 18, 18 } : creatureColors);
                if (imgExists)
                {
                    lImgList.Images.Add(Image.FromFile(imagePath));
                    iconIndices.Add(speciesListName);
                }
                //else if (!speciesWOImage.Contains(ss.name)) speciesWOImage.Add(ss.name);
            }
            //Clipboard.SetText(string.Join("\n", speciesWOImage));

            lImgList.ImageSize = new Size(64, 64);
            lvLastSpecies.LargeImageList = lImgList;
            lvSpeciesInLibrary.LargeImageList = lImgList;

            entryList = new List<SpeciesListEntry>();

            foreach (var s in species)
            {
                entryList.Add(new SpeciesListEntry
                {
                    displayName = s.name,
                    searchName = s.name,
                    modName = s.Mod?.title ?? string.Empty,
                    species = s
                });
            }

            foreach (var a in aliases)
            {
                if (speciesNameToSpecies.ContainsKey(a.Value))
                {
                    entryList.Add(new SpeciesListEntry
                    {
                        displayName = a.Key + " (→" + speciesNameToSpecies[a.Value].name + ")",
                        searchName = a.Key,
                        species = speciesNameToSpecies[a.Value],
                        modName = speciesNameToSpecies[a.Value].Mod?.title ?? string.Empty,
                    });
                }
            }

            entryList = entryList.OrderBy(s => s.displayName).ToList();

            // autocomplete for species-input
            var al = new AutoCompleteStringCollection();
            al.AddRange(entryList.Select(e => e.searchName).ToArray());
            textbox.AutoCompleteCustomSource = al;

            cbDisplayUntameable.Checked = Properties.Settings.Default.DisplayNonDomesticableSpecies;
            FilterList();
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
            foreach (string s in lastSpeciesBPs)
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

        private void FilterList(string part = null)
        {
            if (entryList == null) return;

            lvSpeciesList.BeginUpdate();
            lvSpeciesList.Items.Clear();
            bool inputIsEmpty = string.IsNullOrWhiteSpace(part);
            foreach (var s in entryList)
            {
                if ((Properties.Settings.Default.DisplayNonDomesticableSpecies || s.species.IsDomesticable)
                    && (inputIsEmpty
                       || s.searchName.ToLower().Contains(part.ToLower())
                       )
                   )
                {
                    lvSpeciesList.Items.Add(new ListViewItem(new[] { s.displayName, s.species.VariantInfo, s.species.IsDomesticable ? "✓" : string.Empty, s.modName })
                    {
                        Tag = s.species,
                        BackColor = !s.species.IsDomesticable ? Color.FromArgb(255, 245, 230)
                        : !string.IsNullOrEmpty(s.modName) ? Color.FromArgb(230, 245, 255)
                        : SystemColors.Window,
                        ToolTipText = s.species.blueprintPath,
                    });
                }
            }
            lvSpeciesList.EndUpdate();
        }

        private void lvSpeciesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpeciesList.SelectedItems.Count > 0)
                SetSpecies((Species)lvSpeciesList.SelectedItems[0].Tag);
        }

        private void lvOftenUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvLastSpecies.SelectedItems.Count > 0)
                SetSpecies((Species)lvLastSpecies.SelectedItems[0].Tag);
        }

        private void lvSpeciesInLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpeciesInLibrary.SelectedItems.Count > 0)
                SetSpecies((Species)lvSpeciesInLibrary.SelectedItems[0].Tag);
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

        public void SetSpecies(Species species)
        {
            if (species == null
            || SelectedSpecies == species) return;

            lastSpeciesBPs.Remove(species.blueprintPath);
            lastSpeciesBPs.Insert(0, species.blueprintPath);
            if (lastSpeciesBPs.Count > Properties.Settings.Default.SpeciesSelectorCountLastSpecies) // only keep keepNrLastSpecies of the last species in this list
                lastSpeciesBPs.RemoveRange(Properties.Settings.Default.SpeciesSelectorCountLastSpecies, lastSpeciesBPs.Count - Properties.Settings.Default.SpeciesSelectorCountLastSpecies);
            UpdateLastSpecies();
            SelectedSpecies = species;

            OnSpeciesSelected?.Invoke(true);
        }

        public void SetTextBox(uiControls.TextBoxSuggest textbox)
        {
            this.textbox = textbox;
            textbox.TextChanged += Textbox_TextChanged;
        }

        private async void Textbox_TextChanged(object sender, EventArgs e)
        {
            cancelSource?.Cancel();
            using (cancelSource = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(200, cancelSource.Token); // give the textbox time to apply the selection for the appended text
                    FilterList(textbox.Text.Substring(0, textbox.SelectionStart));
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            cancelSource = null;
        }

        public string[] LastSpecies
        {
            get => lastSpeciesBPs.ToArray();
            set
            {
                if (value == null)
                    lastSpeciesBPs.Clear();
                else
                {
                    lastSpeciesBPs = value.ToList();
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
            return iconIndices.IndexOf(speciesName);
        }

        public Image SpeciesImage(string speciesName = null)
        {
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
    }

    class SpeciesListEntry
    {
        internal string searchName;
        internal string displayName;
        internal string modName;
        internal Species species;
        public override string ToString()
        {
            return displayName;
        }
    }
}

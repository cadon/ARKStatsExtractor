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
        public delegate void speciesChangedEventHandler(bool speciesChanged = true);

        public event speciesChangedEventHandler onSpeciesChanged;
        /// <summary>
        /// The currently selected species
        /// </summary>
        public Species SelectedSpecies { get; private set; }
        /// <summary>
        /// Items for the species list.
        /// </summary>
        private List<SpeciesListEntry> entryList;
        public TabPage lastTabPage;
        private uiControls.TextBoxSuggest textbox;
        /// <summary>
        /// List of species-blueprintpaths last used by the user
        /// </summary>
        private List<string> lastSpeciesBPs;
        private readonly List<string> iconIndices;
        public readonly int keepNrLastSpecies;
        private CancellationTokenSource cancelSource;

        public SpeciesSelector()
        {
            InitializeComponent();
            lastSpeciesBPs = new List<string>();
            iconIndices = new List<string>();
            keepNrLastSpecies = 20;
            SplitterDistance = Properties.Settings.Default.SpeciesSelectorVerticalSplitterDistance;

            // imageList
            ImageList lImgList = new ImageList();
            if (Directory.Exists("img"))
            {
                string[] speciesImageFiles = Directory.GetFiles("img", "*.png", SearchOption.TopDirectoryOnly);
                foreach (string icon in speciesImageFiles)
                {
                    int i = icon.IndexOf("_");
                    if (i == -1)
                    {
                        lImgList.Images.Add(Image.FromFile(icon));
                        iconIndices.Add(Path.GetFileNameWithoutExtension(icon));
                    }
                }

                lImgList.ImageSize = new Size(64, 64);
                lvLastSpecies.LargeImageList = lImgList;
            }
        }

        /// <summary>
        /// Initializes the species lists.
        /// </summary>
        /// <param name="species"></param>
        /// <param name="aliases"></param>
        public void SetSpeciesLists(List<Species> species, Dictionary<string, string> aliases)
        {
            Dictionary<string, Species> speciesNameToSpecies = new Dictionary<string, Species>();

            foreach (Species ss in species)
            {
                if (!speciesNameToSpecies.ContainsKey(ss.DescriptiveNameAndMod))
                    speciesNameToSpecies.Add(ss.DescriptiveNameAndMod, ss);
            }
            entryList = new List<SpeciesListEntry>();

            foreach (var s in species)
            {
                entryList.Add(new SpeciesListEntry
                {
                    displayName = s.name,
                    searchName = s.name,
                    modName = s.Mod?.title ?? string.Empty,
                    tameable = s.taming.nonViolent || s.taming.violent,
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
                        tameable = speciesNameToSpecies[a.Value].taming.nonViolent || speciesNameToSpecies[a.Value].taming.violent,
                    });
                }
            }

            entryList = entryList.OrderBy(s => s.displayName).ToList();

            // autocomplete for species-input
            var al = new AutoCompleteStringCollection();
            al.AddRange(entryList.Select(e => e.searchName).ToArray());
            textbox.AutoCompleteCustomSource = al;

            cbDisplayUntameable.Checked = Properties.Settings.Default.DisplayUntameableSpecies;
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
                lvSpeciesInLibrary.Items.Add(new ListViewItem
                {
                    Text = s.DescriptiveNameAndMod,
                    Tag = s
                });
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
                if ((Properties.Settings.Default.DisplayUntameableSpecies || s.tameable)
                    && (inputIsEmpty
                       || s.searchName.ToLower().Contains(part.ToLower())
                       )
                   )
                {
                    lvSpeciesList.Items.Add(new ListViewItem(new[] { s.displayName, s.species.VariantInfo, s.tameable ? "✓" : string.Empty, s.modName })
                    {
                        Tag = s.species,
                        BackColor = !s.tameable ? Color.FromArgb(255, 245, 230)
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
                SetSpecies((Species)((ListViewItem)lvLastSpecies.SelectedItems[0]).Tag);
        }

        private void lvSpeciesInLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpeciesInLibrary.SelectedItems.Count > 0)
                SetSpecies((Species)((ListViewItem)lvSpeciesInLibrary.SelectedItems[0]).Tag);
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
            if (species == null) return;

            lastSpeciesBPs.Remove(species.blueprintPath);
            if (lastSpeciesBPs.Count > keepNrLastSpecies) // only keep keepNrLastSpecies of the last species in this list
                lastSpeciesBPs.RemoveRange(keepNrLastSpecies, lastSpeciesBPs.Count - keepNrLastSpecies);
            lastSpeciesBPs.Insert(0, species.blueprintPath);
            UpdateLastSpecies();
            SelectedSpecies = species;

            onSpeciesChanged?.Invoke();
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

        private int SpeciesImageIndex(string speciesName = "")
        {
            if (string.IsNullOrWhiteSpace(speciesName))
                speciesName = SelectedSpecies.name;
            else speciesName = Values.V.SpeciesName(speciesName);
            if (speciesName.IndexOf("Aberrant ") != -1)
                speciesName = speciesName.Substring(9);
            return iconIndices.IndexOf(speciesName);
        }

        public Image SpeciesImage(string speciesName = "")
        {
            int ii = SpeciesImageIndex(speciesName);
            if (ii != -1 && ii < lvLastSpecies.LargeImageList.Images.Count)
                return lvLastSpecies.LargeImageList.Images[ii];
            return null;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            onSpeciesChanged?.Invoke(false);
        }

        private void cbDisplayUntameable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DisplayUntameableSpecies = ((CheckBox)sender).Checked;
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
        internal bool tameable;
        internal Species species;
        public override string ToString()
        {
            return displayName;
        }
    }
}

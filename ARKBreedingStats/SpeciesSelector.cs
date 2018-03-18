using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.IO;

namespace ARKBreedingStats
{
    public partial class SpeciesSelector : UserControl
    {
        public delegate void speciesChangedEventHandler();
        public event speciesChangedEventHandler onSpeciesChanged;
        public int speciesIndex;
        public string species;
        private List<string> speciesList;
        private Dictionary<string, string> aliases;
        public List<string> speciesWithAliasesList;
        public TabPage lastTabPage;
        private uiControls.TextBoxSuggest textbox;
        private List<string> lastSpecies;
        private List<string> iconIndices;
        public int keepNrLastSpecies;

        public SpeciesSelector()
        {
            InitializeComponent();
            speciesList = new List<string>();
            aliases = new Dictionary<string, string>();
            speciesWithAliasesList = new List<string>();
            lastSpecies = new List<string>();
            iconIndices = new List<string>();
            keepNrLastSpecies = 20;

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

        public void setSpeciesList(List<string> speciesL)
        {
            speciesList.Clear();
            speciesWithAliasesList.Clear();
            foreach (string s in speciesL)
            {
                speciesList.Add(s);
                speciesWithAliasesList.Add(s);
            }

            loadAliases();
            filterList();

            // autocomplete for species-input
            var al = new AutoCompleteStringCollection();
            al.AddRange(speciesWithAliasesList.ToArray());
            textbox.AutoCompleteCustomSource = al;
        }

        private void loadAliases()
        {
            aliases.Clear();
            string fileName = "json/aliases.json";
            if (System.IO.File.Exists(fileName))
            {
                string aliasesRaw = System.IO.File.ReadAllText(fileName);

                Regex r = new Regex(@"""([^""]+)"" ?: ?""([^""]+)""");
                MatchCollection matches = r.Matches(aliasesRaw);
                foreach (Match match in matches)
                {
                    if (speciesList.IndexOf(match.Groups[1].Value) == -1
                        && speciesList.IndexOf(match.Groups[2].Value) != -1
                        && !aliases.ContainsKey(match.Groups[1].Value))
                    {
                        aliases.Add(match.Groups[1].Value, match.Groups[2].Value);
                        speciesWithAliasesList.Add(match.Groups[1].Value);
                    }
                }
            }
            speciesWithAliasesList.Sort();
        }

        public void setLibrarySpecies(List<string> librarySpeciesList)
        {
            lvSpeciesInLibrary.Items.Clear();
            foreach (string s in librarySpeciesList)
                lvSpeciesInLibrary.Items.Add(s);
        }

        public string speciesName(string alias)
        {
            if (speciesList.IndexOf(alias) != -1)
                return alias;
            else if (aliases.ContainsKey(alias))
                return aliases[alias];
            else return "";
        }

        public void filterList(string part = "")
        {
            lbSpecies.BeginUpdate();
            lbSpecies.Items.Clear();
            if (string.IsNullOrWhiteSpace(part))
            {
                foreach (string s in speciesWithAliasesList)
                {
                    string alias = s;
                    string mainSpecies = speciesName(s);
                    if (mainSpecies != s)
                        alias += " (→" + mainSpecies + ")";
                    lbSpecies.Items.Add(alias);
                }
            }
            else
            {
                foreach (string s in speciesWithAliasesList)
                {
                    if (s.ToLower().Contains(part.ToLower()))
                    {
                        string alias = s;
                        string mainSpecies = speciesName(s);
                        if (mainSpecies != s)
                            alias += " (→" + mainSpecies + ")";
                        lbSpecies.Items.Add(alias);
                    }
                }
            }
            lbSpecies.EndUpdate();
        }

        private void lbSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSpecies.SelectedItems.Count > 0)
            {
                string species = lbSpecies.SelectedItem.ToString();
                if (species.Contains("("))
                    species = species.Substring(0, species.IndexOf("(") - 1);
                setSpecies(species);
            }
        }

        private void lvOftenUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvLastSpecies.SelectedItems.Count > 0)
                setSpecies(lvLastSpecies.SelectedItems[0].Text);
        }

        private void lvSpeciesInLibrary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpeciesInLibrary.SelectedItems.Count > 0)
                setSpecies(lvSpeciesInLibrary.SelectedItems[0].Text);
        }

        public void setSpecies(string species)
        {
            System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
            species = textInfo.ToTitleCase(species.ToLower());
            if (speciesWithAliasesList.Contains(species))
            {
                this.species = speciesName(species);
                speciesIndex = speciesList.IndexOf(this.species);

                lastSpecies.Remove(species);
                if (lastSpecies.Count > keepNrLastSpecies) // only keep keepNrLastSpecies of the last species in this list
                    lastSpecies.RemoveRange(keepNrLastSpecies, lastSpecies.Count - keepNrLastSpecies);
                lastSpecies.Insert(0, species);
                updateLastSpecies();

                onSpeciesChanged?.Invoke();
            }
        }

        public void setSpeciesIndex(int sI)
        {
            if (sI >= 0 && sI < speciesList.Count)
            {
                setSpecies(speciesList[sI]);
            }
        }

        public void SetTextBox(uiControls.TextBoxSuggest textbox)
        {
            this.textbox = textbox;
            textbox.TextChanged += Textbox_TextChanged;
        }

        private async void Textbox_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(200); // give the textbox time to apply the selection for the appended text
            filterList(textbox.Text.Substring(0, textbox.SelectionStart));
        }

        private void updateLastSpecies()
        {
            lvLastSpecies.Items.Clear();
            ListViewItem lvi;
            foreach (string s in lastSpecies)
            {
                lvi = new ListViewItem(s);
                int ii = speciesImageIndex(s);
                if (ii != -1)
                    lvi.ImageIndex = ii;
                lvLastSpecies.Items.Add(lvi);
            }
        }

        public string[] LastSpecies
        {
            set
            {
                if (value == null)
                    lastSpecies.Clear();
                else
                {
                    lastSpecies = value.ToList();
                    updateLastSpecies();
                }
            }
            get
            {
                return lastSpecies.ToArray();
            }
        }

        private int speciesImageIndex(string species = "")
        {
            if (String.IsNullOrWhiteSpace(species))
                species = this.species;
            species = speciesName(species);
            if (species.IndexOf("Aberrant ") != -1)
                species = species.Substring(9);
            return iconIndices.IndexOf(species);
        }

        public Image speciesImage(string species = "")
        {
            int ii = speciesImageIndex(species);
            if (ii != -1 && ii < lvLastSpecies.LargeImageList.Images.Count)
                return lvLastSpecies.LargeImageList.Images[ii];
            return null;
        }
    }
}

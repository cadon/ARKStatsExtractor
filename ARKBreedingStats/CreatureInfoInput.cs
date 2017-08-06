using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ARKBreedingStats
{
    public partial class CreatureInfoInput : UserControl
    {
        public delegate void Add2LibraryClickedEventHandler(CreatureInfoInput sender);
        public event Add2LibraryClickedEventHandler Add2Library_Clicked;
        public delegate void Save2LibraryClickedEventHandler(CreatureInfoInput sender);
        public event Save2LibraryClickedEventHandler Save2Library_Clicked;
        public delegate void RequestParentListEventHandler(CreatureInfoInput sender);
        public event RequestParentListEventHandler ParentListRequested;
        public delegate void RequestCreatureDataEventHandler(CreatureInfoInput sender);
        public event RequestCreatureDataEventHandler CreatureDataRequested;
        public bool extractor;
        private Sex sex;
        private CreatureStatus status;
        public bool parentListValid;
        private int speciesIndex;
        public StatIO weightStat;
        private ToolTip tt = new ToolTip();
        private bool mutationManuallyChanged;
        private bool updateMaturation;
        private List<Creature> _females;
        private List<Creature> _males;

        public CreatureInfoInput()
        {
            InitializeComponent();
            speciesIndex = -1;
            parentComboBoxMother.naLabel = " - Mother n/a";
            parentComboBoxMother.Items.Add(" - Mother n/a");
            parentComboBoxFather.naLabel = " - Father n/a";
            parentComboBoxFather.Items.Add(" - Father n/a");
            parentComboBoxMother.SelectedIndex = 0;
            parentComboBoxFather.SelectedIndex = 0;
            tt.SetToolTip(buttonSex, "Sex");
            tt.SetToolTip(buttonStatus, "Status");
            tt.SetToolTip(dateTimePickerAdded, "Domesticated at");
            tt.SetToolTip(numericUpDownMutations, "Mutation-Counter");
            tt.SetToolTip(btnGenerateUniqueName, "Generate sequential unique name");
            updateMaturation = true;
        }

        private void buttonAdd2Library_Click(object sender, EventArgs e)
        {
            Add2Library_Clicked(this);
        }

        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            Save2Library_Clicked(this);
        }

        public string CreatureName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }
        public string CreatureOwner
        {
            get { return textBoxOwner.Text; }
            set { textBoxOwner.Text = value; }
        }
        public string CreatureTribe
        {
            get { return textBoxTribe.Text; }
            set { textBoxTribe.Text = value; }
        }
        public Sex CreatureSex
        {
            get { return sex; }
            set
            {
                sex = value;
                buttonSex.Text = Utils.sexSymbol(sex);
                buttonSex.BackColor = Utils.sexColor(sex);
                tt.SetToolTip(buttonSex, "Sex: " + sex.ToString());
                if (sex == Sex.Female)
                    checkBoxNeutered.Text = "Spayed";
                else
                    checkBoxNeutered.Text = "Neutered";
            }
        }
        public CreatureStatus CreatureStatus
        {
            get { return status; }
            set
            {
                status = value;
                buttonStatus.Text = Utils.statusSymbol(status);
                tt.SetToolTip(buttonStatus, "Status: " + status.ToString());
            }
        }
        public Creature mother
        {
            get
            {
                return parentComboBoxMother.SelectedParent;
            }
            set
            {
                parentComboBoxMother.preselectedCreatureGuid = (value == null ? Guid.Empty : value.guid);
            }
        }
        public Creature father
        {
            get
            {
                return parentComboBoxFather.SelectedParent;
            }
            set
            {
                parentComboBoxFather.preselectedCreatureGuid = (value == null ? Guid.Empty : value.guid);
            }
        }
        public string CreatureNote
        {
            get { return textBoxNote.Text; }
            set { textBoxNote.Text = value; }
        }

        private void buttonGender_Click(object sender, EventArgs e)
        {
            CreatureSex = Utils.nextSex(sex);
        }

        private void buttonStatus_Click(object sender, EventArgs e)
        {
            CreatureStatus = Utils.nextStatus(status);
        }

        public List<Creature>[] Parents
        {
            set
            {
                if (value != null)
                {
                    _females = parentComboBoxMother.ParentList = value[0];
                    _males = parentComboBoxFather.ParentList = value[1];
                }
            }
        }
        public List<int>[] ParentsSimilarities
        {
            set
            {
                if (value != null)
                {
                    parentComboBoxMother.parentsSimilarity = value[0];
                    parentComboBoxFather.parentsSimilarity = value[1];
                }
            }
        }

        public bool ButtonEnabled { set { buttonAdd2Library.Enabled = value; } }

        public bool ShowSaveButton
        {
            set
            {
                buttonSaveChanges.Visible = value;
                buttonAdd2Library.Location = new Point((value ? 154 : 88), 339);
                buttonAdd2Library.Size = new Size((value ? 68 : 134), 37);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            if (!parentListValid)
                ParentListRequested?.Invoke(this);
        }

        private void dhmInputGrown_TextChanged(object sender, EventArgs e)
        {
            if (updateMaturation && speciesIndex >= 0 && Values.V.species != null && Values.V.species[speciesIndex] != null)
            {
                updateMaturation = false;
                numericUpDownWeight.Value = Values.V.species[speciesIndex].breeding != null && Values.V.species[speciesIndex].breeding.maturationTimeAdjusted > 0 ?
                    (decimal)(weightStat.Input * dhmInputGrown.Timespan.TotalSeconds / Values.V.species[speciesIndex].breeding.maturationTimeAdjusted) : 0; // todo. remove? baby-weight is no more shown?
                updateMaturationPercentage();
                updateMaturation = true;
            }
        }

        private void numericUpDownWeight_ValueChanged(object sender, EventArgs e)
        {
            if (updateMaturation)
            {
                updateMaturation = false;
                if (Values.V.species[speciesIndex].breeding != null && weightStat.Input > 0)
                    dhmInputGrown.Timespan = new TimeSpan(0, 0, (int)(Values.V.species[speciesIndex].breeding.maturationTimeAdjusted * (1 - (double)numericUpDownWeight.Value / weightStat.Input)));
                else dhmInputGrown.Timespan = new TimeSpan(0);
                updateMaturationPercentage();
                updateMaturation = true;
            }
        }

        private void updateMaturationPercentage()
        {
            labelGrownPercent.Text = dhmInputGrown.Timespan.TotalMinutes > 0 && weightStat.Input > 0 ?
                Math.Round(100 * (double)numericUpDownWeight.Value / weightStat.Input, 1) + " %" : "";
        }

        public DateTime Cooldown
        {
            set { dhmInputCooldown.Timespan = value - DateTime.Now; }
            get { return dhmInputCooldown.changed ? DateTime.Now.Add(dhmInputCooldown.Timespan) : DateTime.Now; }
        }

        public DateTime Grown
        {
            set { dhmInputGrown.Timespan = value - DateTime.Now; }
            get { return dhmInputGrown.changed ? DateTime.Now.Add(dhmInputGrown.Timespan) : DateTime.Now; }
        }

        public string[] AutocompleteOwnerList
        {
            set
            {
                var l = new AutoCompleteStringCollection();
                l.AddRange(value);
                textBoxOwner.AutoCompleteCustomSource = l;
            }
        }

        public DateTime domesticatedAt
        {
            set
            {
                if (value < dateTimePickerAdded.MinDate)
                    dateTimePickerAdded.Value = dateTimePickerAdded.MinDate;
                else
                    dateTimePickerAdded.Value = value;
            }
            get { return dateTimePickerAdded.Value; }
        }

        public bool Neutered
        {
            set { checkBoxNeutered.Checked = value; }
            get { return checkBoxNeutered.Checked; }
        }

        public int MutationCounter
        {
            set
            {
                numericUpDownMutations.Value = value;
                mutationManuallyChanged = false;
            }
            get { return (int)numericUpDownMutations.Value; }
        }

        public double babyWeight
        {
            set
            {
                if (value <= (double)numericUpDownWeight.Maximum)
                    numericUpDownWeight.Value = (decimal)value;
            }
        }

        public int SpeciesIndex
        {
            set
            {
                speciesIndex = value;
                bool breedingPossible = Values.V.species.Count > value && Values.V.species[speciesIndex].breeding != null;

                dhmInputCooldown.Visible = breedingPossible;
                dhmInputGrown.Visible = breedingPossible;
                numericUpDownWeight.Visible = breedingPossible;
                label4.Visible = breedingPossible;
                label5.Visible = breedingPossible;
                label6.Visible = breedingPossible;
                labelGrownPercent.Visible = breedingPossible;
                numericUpDownMutations.Visible = breedingPossible;
                labelMutations.Visible = breedingPossible;
                if (!breedingPossible)
                {
                    numericUpDownWeight.Value = 0;
                    dhmInputGrown.Timespan = new TimeSpan(0);
                    dhmInputCooldown.Timespan = new TimeSpan(0);
                }
            }
        }

        private void parentComboBoxMother_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateMutations();
        }

        private void parentComboBoxFather_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateMutations();
        }

        private void updateMutations()
        {
            if (!mutationManuallyChanged)
            {
                numericUpDownMutations.Value = (parentComboBoxMother.SelectedParent != null ? parentComboBoxMother.SelectedParent.mutationCounter : 0) +
                    (parentComboBoxFather.SelectedParent != null ? parentComboBoxFather.SelectedParent.mutationCounter : 0);
                mutationManuallyChanged = false;
            }
        }

        private void numericUpDownMutations_ValueChanged(object sender, EventArgs e)
        {
            mutationManuallyChanged = true;
        }

        private void btnGenerateUniqueName_Click(object sender, EventArgs e)
        {
            if (speciesIndex >= 0 && speciesIndex < Values.V.species.Count)
            {
                CreatureDataRequested?.Invoke(this);
            }
        }

        /// <summary>
        /// Generates a creature name with a given pattern
        /// </summary>
        public void generateCreatureName(Creature creature)
        {
            try
            {
                // collect creatures of the same species
                var sameSpecies = (_females ?? new List<Creature> { }).Concat((_males ?? new List<Creature> { })).ToList();
                var names = sameSpecies.Select(x => x.name).ToArray();

                var tokenDictionary = createTokenDictionary(creature, names);
                var name = assemblePatternedName(tokenDictionary);

                if (name.Contains("{n}"))
                {
                    // find the sequence token, and if not, return because the configurated pattern string is invalid without it
                    var index = name.IndexOf("{n}", StringComparison.OrdinalIgnoreCase);
                    var patternStart = name.Substring(0, index);
                    var patternEnd = name.Substring(index + 3);

                    // loop until we find a unique name in the sequence which is not taken

                    var n = 1;
                    do
                    {
                        name = string.Concat(patternStart, n, patternEnd);
                        n++;
                    } while (names.Contains(name, StringComparer.OrdinalIgnoreCase));
                }

                //TODO SkyDotNET: Add the following notices to the UI instead of showing a messagebox
                if (names.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    MessageBox.Show("WARNING: The generated name for the creature already exists in the database.");
                }
                else if (name.Length > 24)
                {
                    MessageBox.Show("WARNING: The generated name is longer than 24 characters, ingame-preview:" + name.Substring(0, 24));
                }

                CreatureName = name;
            }
            catch
            {
                MessageBox.Show("There was an error while generating the creature name.");
            }
        }

        /// <summary>        
        /// This method creates the token dictionary for the dynamic creature name generation.
        /// </summary>
        /// <param name="creature">Creature with the data</param>
        /// <param name="creatureNames">A list of all name of the currently stored creatures</param>
        /// <returns>A dictionary containing all tokens and their replacements</returns>
        private Dictionary<string, string> createTokenDictionary(Creature creature, string[] creatureNames)
        {
            var date_short = DateTime.Now.ToString("yy-MM-dd");
            var date_compressed = date_short.Replace("-", "");
            var time_short = DateTime.Now.ToString("hh:mm:ss");
            var time_compressed = time_short.Replace(":", "");

            string hp = creature.levelsWild[0].ToString().PadLeft(2, '0');
            string stam = creature.levelsWild[1].ToString().PadLeft(2, '0');
            string oxy = creature.levelsWild[2].ToString().PadLeft(2, '0');
            string food = creature.levelsWild[3].ToString().PadLeft(2, '0');
            string weight = creature.levelsWild[4].ToString().PadLeft(2, '0');
            string dmg = creature.levelsWild[5].ToString().PadLeft(2, '0');
            string spd = creature.levelsWild[6].ToString().PadLeft(2, '0');
            string trp = creature.levelsWild[7].ToString().PadLeft(2, '0');

            double imp = creature.imprintingBonus * 100;
            double eff = creature.tamingEff * 100;

            var rand = new Random(DateTime.Now.Millisecond);
            var randStr = rand.Next(100000, 999999).ToString();

            string effImp = "Z";
            string prefix = "";
            if (imp > 0)
            {
                prefix = "I";
                effImp = Math.Round(imp).ToString();
            }
            else if (eff > 1)
            {
                prefix = "E";
                effImp = Math.Round(eff).ToString();
            }

            while (effImp.Length < 3 && effImp != "Z")
            {
                effImp = "0" + effImp;
            }

            effImp = prefix + effImp;

            var precompressed =
                CreatureSex.ToString().Substring(0, 1) +
                date_compressed +
                hp +
                stam +
                oxy +
                food +
                weight +
                dmg +
                effImp;

            var spcShort = Values.V.species[speciesIndex].name.Replace(" ", "");
            var speciesShort = spcShort;
            var vowels = new string[] { "a", "e", "i", "o", "u" };
            while (spcShort.Length > 4 && spcShort.LastIndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u' }) > 0)
                spcShort = spcShort.Remove(spcShort.LastIndexOfAny(new char[] { 'a', 'e', 'i', 'o', 'u' }), 1); // remove last vowel (not the first letter)
            spcShort = spcShort.Substring(0, Math.Min(4, spcShort.Length));

            speciesShort = speciesShort.Substring(0, Math.Min(4, speciesShort.Length));

            // replace tokens in user configurated pattern string
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "species", Values.V.species[speciesIndex].name },
                { "spcs_short", spcShort },
                { "spcs_shortu", spcShort.ToUpper() },
                { "species_short", speciesShort },
                { "species_shortu", speciesShort.ToUpper() },
                { "sex", CreatureSex.ToString() },
                { "sex_short", CreatureSex.ToString().Substring(0, 1) },
                { "cpr" , precompressed },
                { "date_short" ,  date_short },
                { "date_compressed" , date_compressed },
                { "times_short" , time_short },
                { "times_compressed" , time_compressed },
                { "time_short",time_short.Substring(0,5)},
                { "time_compressed",time_compressed.Substring(0,4)},
                { "hp" , hp },
                { "stam" ,stam },
                { "oxy" , oxy },
                { "food" , food },
                { "weight" , weight },
                { "dmg" ,dmg },
                { "spd" , spd },
                { "trp" , trp },
                { "effImp" , effImp },
                { "rnd", randStr },
                { "tn", (creatureNames.Length + 1).ToString() }
            };
        }

        /// <summary>
        /// Assembles a string representing the desired creature name with the set token
        /// </summary>
        /// <param name="tokenDictionary">a collection of token and their replacements</param>
        /// <returns>The patterned name</returns>
        private string assemblePatternedName(Dictionary<string, string> tokenDictionary)
        {
            var regularExpression = "\\{(?<key>" + string.Join("|", tokenDictionary.Keys.Select(x => Regex.Escape(x))) + ")\\}";
            var regularExpressionOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
            var r = new Regex(regularExpression, regularExpressionOptions);

            string savedPattern = Properties.Settings.Default.sequentialUniqueNamePattern;

            return r.Replace(savedPattern, (m) =>
            {
                string replacement = null;
                return tokenDictionary.TryGetValue(m.Groups["key"].Value, out replacement) ? replacement : m.Value;
            });
        }
    }
}

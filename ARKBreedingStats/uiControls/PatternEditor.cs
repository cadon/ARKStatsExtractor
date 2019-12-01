using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class PatternEditor : Form
    {
        public PatternEditor()
        {
            Initialization();
        }

        public PatternEditor(Creature c, List<Creature> females, List<Creature> males)
        {
            Initialization(c, females, males);
        }

        private void Initialization(Creature creature = null, List<Creature> females = null, List<Creature> males = null)
        {
            InitializeComponent();

            txtboxPattern.Text = Properties.Settings.Default.sequentialUniqueNamePattern;

            Dictionary<string, string> patternList = new Dictionary<string, string>()
            {
                { "species", "species name" },
                { "species_short6", "species name shortened to at most 6 letters" },
                { "species_short6u", "species name shortened to at most 6 letters in uppercase" },
                { "species_short5", "species name shortened to at most 5 letters" },
                { "species_short5u", "species name shortened to at most 5 letters in uppercase" },
                { "species_short4", "species name shortened to at most 4 letters" },
                { "species_short4u", "species name shortened to at most 4 letters in uppercase" },
                { "spcs_short4", "species without vowels and shortened to at most 4 characters" },
                { "spcs_short4u", "like {spcs_short} and in uppercase" },
                { "firstWordOfOldest", "the first word of the name of the first added creature of the species" },
                { "sex", "sex (\"Male\", \"Female\", \"Unknown\")" },
                { "sex_short", "\"M\", \"F\", \"U\"" },
                { "cpr", "{sex_short}{date_short}{hp}{stam}{oxy}{food}{weight}{dmg}{effImp}" },
                { "yyyy", "year with 4 digits" },
                { "yy", "year with 2 digits" },
                { "MM", "month with 2 digits" },
                { "dd", "day of the month with 2 digits" },
                { "hh", "hours (24 h format)" },
                { "mm", "minutes" },
                { "ss", "seconds" },
                { "date", "yy-MM-dd" },
                { "time", "hh:mm:ss" },
                { "n", "if the name is not unique, the smallest possible number is appended (only creatues with a given sex are considered)." },
                { "hp", "Health" },
                { "stam", "Stamina" },
                { "oxy", "Oxygen" },
                { "food", "Food" },
                { "water", "Water" },
                { "temp", "Temperature" },
                { "weight", "Weight" },
                { "dmg", "Damage" },
                { "spd", "Speed" },
                { "fort", "Fortitude" },
                { "craft", "Crafting Speed" },
                { "trp", "Torpor" },
                { "baselvl", "Base-level (level without manually added ones), i.e. level right after taming / hatching" },
                { "effImp", "Taming-effectiveness or Imprinting (if tamed / bred)" },
                { "gen", "Generation" },
                { "gena", "Generation in letters" },
                { "muta", "Mutations. Numbers larger than 99 will be displayed as 99" },
                { "rnd", "6-digit random number in the range 100000 - 999999" },
                { "tn", "number of creatures of the current species in the library + 1" },
                { "sn", "number of creatures of the current species with the same sex in the library + 1" },
                { "dom", "how the creature was domesticated, \"T\" for tamed, \"B\" for bred" },
                { "arkidlast4", "the last 4 digits of the Ark-Id (as entered or seen ingame)"},
                { "highest1l", "the highest stat-level of this creature (excluding torpidity)" },
                { "highest2l", "the second highest stat-level of this creature (excluding torpidity)" },
                { "highest3l", "the third highest stat-level of this creature (excluding torpidity)" },
                { "highest4l", "the fourth highest stat-level of this creature (excluding torpidity)" },
                { "highest1s", "the name of the highest stat-level of this creature (excluding torpidity)" },
                { "highest2s", "the name of the second highest stat-level of this creature (excluding torpidity)" },
                { "highest3s", "the name of the third highest stat-level of this creature (excluding torpidity)" },
                { "highest4s", "the name of the fourth highest stat-level of this creature (excluding torpidity)" },
            };

            // collect creatures of the same species
            var sameSpecies = (females ?? new List<Creature> { }).Concat((males ?? new List<Creature> { })).ToList();
            var creatureNames = sameSpecies.Select(x => x.name).ToList();

            var examples = NamePatterns.CreateTokenDictionary(creature, sameSpecies);

            int i = 0;
            foreach (KeyValuePair<string, string> p in patternList)
            {
                Button btn = new Button
                {
                    Size = new Size(120, 23),
                    Text = $"{{{p.Key}}}"
                };
                flowLayoutPanel1.Controls.Add(btn);
                btn.Click += Btn_Click;

                Label lbl = new Label
                {
                    AutoSize = true,
                    Text = p.Value + (examples.ContainsKey(p.Key) ? ". E.g. \"" + examples[p.Key] + "\"" : "")
                };
                flowLayoutPanel1.Controls.Add(lbl);
                flowLayoutPanel1.SetFlowBreak(lbl, true);
                i++;
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            string insertText = ((Button)sender).Text;
            int selectionIndex = txtboxPattern.SelectionStart;
            txtboxPattern.Text = txtboxPattern.Text.Insert(selectionIndex, insertText);
            txtboxPattern.SelectionStart = selectionIndex + insertText.Length;
            txtboxPattern.Focus();
        }

        public string NamePattern
        {
            get => txtboxPattern.Text;
            set => txtboxPattern.Text = value;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtboxPattern.Text = string.Empty;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Name-Generator");
        }
    }
}

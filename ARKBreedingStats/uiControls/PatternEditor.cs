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
            InitializeComponent();
        }

        public PatternEditor(Creature c, List<Creature> females, List<Creature> males)
        {
            Initialization(c, females, males);
        }

        private void Initialization(Creature creature, List<Creature> females = null, List<Creature> males = null)
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

                { "hp_vb", "Health value at breeding" },
                { "stam_vb", "Stamina value at breeding" },
                { "oxy_vb", "Oxygen value at breeding" },
                { "food_vb", "Food value at breeding" },
                { "water_vb", "Water value at breeding" },
                { "temp_vb", "Temperature value at breeding" },
                { "weight_vb", "Weight value at breeding" },
                { "dmg_vb", "Damage value at breeding" },
                { "spd_vb", "Speed value at breeding" },
                { "fort_vb", "Fortitude value at breeding" },
                { "craft_vb", "Crafting Speed value at breeding" },

                { "hp_vb_k", "Health value at breeding in thousands" },
                { "stam_vb_k", "Stamina value at breeding in thousands" },
                { "oxy_vb_k", "Oxygen value at breeding in thousands" },
                { "food_vb_k", "Food value at breeding in thousands" },
                { "water_vb_k", "Water value at breeding in thousands" },
                { "temp_vb_k", "Temperature value at breeding in thousands" },
                { "weight_vb_k", "Weight value at breeding in thousands" },
                { "dmg_vb_k", "Damage value at breeding in thousands" },
                { "spd_vb_k", "Speed value at breeding in thousands" },
                { "fort_vb_k", "Fortitude value at breeding in thousands" },
                { "craft_vb_k", "Crafting Speed value at breeding in thousands" },

                { "hp_vb_10k", "Health value at breeding in ten thousands" },
                { "stam_vb_10k", "Stamina value at breeding in ten thousands" },
                { "oxy_vb_10k", "Oxygen value at breeding in ten thousands" },
                { "food_vb_10k", "Food value at breeding in ten thousands" },
                { "water_vb_10k", "Water value at breeding in ten thousands" },
                { "temp_vb_10k", "Temperature value at breeding in ten thousands" },
                { "weight_vb_10k", "Weight value at breeding in ten thousands" },
                { "dmg_vb_10k", "Damage value at breeding in ten thousands" },
                { "spd_vb_10k", "Speed value at breeding in ten thousands" },
                { "fort_vb_10k", "Fortitude value at breeding in ten thousands" },
                { "craft_vb_10k", "Crafting Speed value at breeding in ten thousands" },

                { "hp_vb_n", "Health value at breeding as integer" },
                { "stam_vb_n", "Stamina value at breeding as integer" },
                { "oxy_vb_n", "Oxygen value at breeding as integer" },
                { "food_vb_n", "Food value at breeding as integer" },
                { "water_vb_n", "Water value at breeding as integer" },
                { "temp_vb_n", "Temperature value at breeding as integer" },
                { "weight_vb_n", "Weight value at breeding as integer" },
                { "dmg_vb_n", "Damage value at breeding as integer" },
                { "spd_vb_n", "Speed value at breeding as integer" },
                { "fort_vb_n", "Fortitude value at breeding as integer" },
                { "craft_vb_n", "Crafting Speed value at breeding as integer" },

                { "effImp_short", "Short Taming-effectiveness or Imprinting (if tamed / bred)"},
                { "index",        "Index in library(same creature)."},
                { "oldname", "the old name of the creature" },

                { "sex_lang", "sex (\"Male\", \"Female\", \"Unknown\") by loc" },
                { "sex_lang_short", "\"Male\", \"Female\", \"Unknown\" by loc(short)" },

                { "sex_lang_gen", "sex (\"Male_gen\", \"Female_gen\", \"Unknown_gen\") by loc" },
                { "sex_lang_short_gen", "\"Male_gen\", \"Female_gen\", \"Unknown_gen\" by loc(short)" },

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

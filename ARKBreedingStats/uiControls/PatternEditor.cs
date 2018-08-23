using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                { "species", "species name"},
                { "species_short6","species name shortened to at most 6 letters"},
                { "species_short6u","species name shortened to at most 6 letters in uppercase"},
                { "species_short5","species name shortened to at most 5 letters"},
                { "species_short5u","species name shortened to at most 5 letters in uppercase"},
                { "species_short4","species name shortened to at most 4 letters"},
                { "species_short4u","species name shortened to at most 4 letters in uppercase"},
                { "spcs_short4","species without vowels and shortened to at most 4 characters"},
                { "spcs_short4u","like {spcs_short} and in uppercase"},
                { "firstWordOfOldest","the first word of the name of the first added creature of the species"},
                { "sex","sex (\"Male\", \"Female\", \"Unknown\")"},
                { "sex_short","\"M\", \"F\", \"U\""},
                { "cpr","{sex_short}{date_short}{hp}{stam}{oxy}{food}{weight}{dmg}{effImp}"},
                { "yyyy","year with 4 digits"},
                { "yy","year with 2 digits"},
                { "MM","month with 2 digits"},
                { "dd","day of the month with 2 digits"},
                { "hh","hours (24 h format)"},
                { "mm","minutes"},
                { "ss","seconds"},
                { "date","yy-MM-dd"},
                { "time","hh:mm:ss"},
                { "n","if the name is not unique, the smallest possible number is appended (only creatues with a given sex are considered)."},
                { "hp","Health"},
                { "stam","Stamina"},
                { "oxy","Oxygen"},
                { "food","Food"},
                { "weight","Weight"},
                { "dmg","Damage"},
                { "spd","Speed"},
                { "trp","Torpor"},
                { "baselvl","Base-level (level without manually added ones), i.e. level right after taming / hatching"},
                { "effImp","Taming-effectiveness or Imprinting (if tamed / bred)"},
                { "gen","Generation"},
                { "gena","Generation in letters"},
                { "muta","Mutations. Numbers larger than 99 will be displayed as 99"},
                { "rnd","6-digit random number in the range 100000 - 999999"},
                { "tn","number of creatures of the current species in the library + 1"},
                { "sn","number of creatures of the current species with the same sex in the library + 1"},
                { "dom","how the creature was domesticated, \"T\" for tamed, \"B\" for bred"}
            };

            // collect creatures of the same species
            var sameSpecies = (females ?? new List<Creature> { }).Concat((males ?? new List<Creature> { })).ToList();
            var creatureNames = sameSpecies.Select(x => x.name).ToList();

            var examples = uiControls.NamePatterns.CreateTokenDictionary(creature, sameSpecies);

            int i = 0;
            foreach (KeyValuePair<string, string> p in patternList)
            {
                Button btn = new Button();
                btn.Location = new Point(15, 3 + i * 27);
                btn.Size = new Size(120, 23);
                btn.Text = "{" + p.Key + "}";
                panelButtons.Controls.Add(btn);
                btn.Click += Btn_Click;

                Label lbl = new Label();
                lbl.Location = new Point(140, 8 + i * 27);
                lbl.AutoSize = true;
                lbl.Text = p.Value + (examples.ContainsKey(p.Key) ? ". E.g. \"" + examples[p.Key] + "\"" : "");
                panelButtons.Controls.Add(lbl);
                i++;
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            string insertText = ((Button)sender).Text;
            var selectionIndex = txtboxPattern.SelectionStart;
            txtboxPattern.Text = txtboxPattern.Text.Insert(selectionIndex, insertText);
            txtboxPattern.SelectionStart = selectionIndex + insertText.Length;
            txtboxPattern.Focus();
        }

        public string NamePattern
        {
            get { return txtboxPattern.Text; }
            set { txtboxPattern.Text = value; }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtboxPattern.Text = "";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Name-Generator");
        }
    }
}

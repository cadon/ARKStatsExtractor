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
                {"species_short","species name shortened to at most 4 letters"},
                {"species_shortu","species name shortened to at most 4 letters in uppercase"},
                {"spcs_short","species without vowels and shortened to at most 4 characters"},
                {"spcs_shortu","like {spcs_short} and in uppercase"},
                {"sex","sex (\"Male\", \"Female\", \"Unknown\")"},
                {"sex_short","\"M\", \"F\", \"U\""},
                {"cpr","{sex_short}{date_short}{hp}{stam}{oxy}{food}{weight}{dmg}{effImp}"},
                {"date_short","yy-MM-dd"},
                {"date_compressed","yyMMdd"},
                {"times_short","hh:mm:ss"},
                {"times_compressed","hhmmss"},
                {"time_short","hh:mm"},
                {"time_compressed","hhmm"},
                {"n","if the name is not unique, the smallest possible number is appended"},
                {"hp","Health"},
                {"stam","Stamina"},
                {"oxy","Oxygen"},
                {"food","Food"},
                {"weight","Weight"},
                {"dmg","Damage"},
                {"spd","Speed"},
                {"trp","Torpor"},
                {"effImp","Taming-effectiveness or Imprinting (if tamed / bred)"},
                {"gen","Generation"},
                {"gena","Generation in letters"},
                {"muta","Mutations"},
                {"rnd","random number (10000 - 99999"},
                {"tn","number of creatures of the current species in the library + 1"}
            };

            var examples = uiControls.NamePatterns.createTokenDictionary(creature, females, males);

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
    }
}

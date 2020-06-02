using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class PatternEditor : Form
    {
        private CancellationTokenSource cancelSource;
        private Creature _creature;
        private List<Creature> _females;
        private List<Creature> _males;
        private int[] _speciesTopLevels;
        private Dictionary<string, string> _customReplacings;
        public Action<PatternEditor> OnReloadCustomReplacings;

        public PatternEditor()
        {
            InitializeComponent();
        }

        public PatternEditor(Creature creature, List<Creature> females, List<Creature> males, int[] speciesTopLevels, Dictionary<string, string> customReplacings, int namingPatternIndex, Action<PatternEditor> reloadCallback) : this()
        {
            OnReloadCustomReplacings = reloadCallback;
            _creature = creature;
            _females = females;
            _males = males;
            _speciesTopLevels = speciesTopLevels;
            _customReplacings = customReplacings;
            txtboxPattern.Text = Properties.Settings.Default.NamingPatterns?[namingPatternIndex] ?? string.Empty;
            txtboxPattern.SelectionStart = txtboxPattern.Text.Length;

            Text = $"Naming Pattern Editor: pattern {(namingPatternIndex + 1)}";

            // collect creatures of the same species
            var sameSpecies = (females ?? new List<Creature>()).Concat((males ?? new List<Creature>())).ToList();

            var examples = NamePatterns.CreateTokenDictionary(creature, sameSpecies, _speciesTopLevels);

            TableLayoutPanel tlpKeys = new TableLayoutPanel();
            tableLayoutPanel1.Controls.Add(tlpKeys);
            SetControlsToTable(tlpKeys, PatternExplanations(creature.Species.IsGlowSpecies));

            TableLayoutPanel tlpFunctions = new TableLayoutPanel();
            tableLayoutPanel1.Controls.Add(tlpFunctions);
            tableLayoutPanel1.SetColumn(tlpFunctions, 1);
            SetControlsToTable(tlpFunctions, FunctionExplanations(), false, true, 306);

            void SetControlsToTable(TableLayoutPanel tlp, Dictionary<string, string> nameExamples, bool columns = true, bool useExampleAsInput = false, int buttonWidth = 120)
            {
                tlp.Dock = DockStyle.Fill;

                // to deactivate the horizontal scrolling but keep the vertical scrolling,
                // apparently that is the way to go ¯\_(ツ)_/¯
                tlp.HorizontalScroll.Maximum = 0;
                tlp.AutoScroll = false;
                tlp.VerticalScroll.Visible = false;
                tlp.AutoScroll = true;

                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                int i = 0;
                foreach (KeyValuePair<string, string> p in nameExamples)
                {
                    tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    if (!columns)
                        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    Button btn = new Button
                    {
                        Size = new Size(buttonWidth, 23),
                        Text = useExampleAsInput ? p.Key : $"{{{p.Key}}}"
                    };
                    int substringUntil = p.Value.LastIndexOf("\n");
                    btn.Tag = useExampleAsInput ? p.Value.Substring(substringUntil + 1) : $"{{{p.Key}}}";
                    tlp.Controls.Add(btn);
                    tlp.SetCellPosition(btn, new TableLayoutPanelCellPosition(0, i));
                    if (!columns)
                        tlp.SetColumnSpan(btn, 2);
                    btn.Click += Btn_Click;

                    Label lbl = new Label
                    {
                        Dock = DockStyle.Fill,
                        MinimumSize = new Size(50, 40),
                        Text = useExampleAsInput ? p.Value.Substring(0, substringUntil) : p.Value + (examples.ContainsKey(p.Key) ? ". E.g. \"" + examples[p.Key] + "\"" : ""),
                        Margin = new Padding(3, 3, 3, 5)
                    };
                    tlp.Controls.Add(lbl);
                    tlp.SetCellPosition(lbl, new TableLayoutPanelCellPosition(columns ? 1 : 0, columns ? i : ++i));
                    if (!columns)
                        tlp.SetColumnSpan(lbl, 2);

                    if (!columns && p.Value.Contains("#customreplace"))
                    {
                        // button to open custom replacings file
                        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        i++;
                        var btCustomReplacings = new Button() { Text = "Open custom replacings file", Width = 150 };
                        btCustomReplacings.Click += BtCustomReplacings_Click;
                        tlp.Controls.Add(btCustomReplacings);
                        tlp.SetCellPosition(btCustomReplacings, new TableLayoutPanelCellPosition(0, i));
                        var btCustomReplacingsReload = new Button() { Text = "Reload custom replacings", Width = 150 };
                        btCustomReplacingsReload.Click += (sender, eventArgs) => OnReloadCustomReplacings?.Invoke(this);
                        tlp.Controls.Add(btCustomReplacingsReload);
                        tlp.SetCellPosition(btCustomReplacingsReload, new TableLayoutPanelCellPosition(1, i));
                    }

                    // separator
                    if (!columns)
                    {
                        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        var separator = new Label
                        {
                            BorderStyle = BorderStyle.Fixed3D,
                            Height = 2,
                            Dock = DockStyle.Bottom,
                            Margin = new Padding(0, 0, 0, 5)
                        };
                        tlp.Controls.Add(separator);
                        tlp.SetRow(separator, ++i);
                        tlp.SetColumnSpan(separator, 2);
                    }

                    i++;
                }
            }
        }

        internal void SetCustomReplacings(Dictionary<string, string> customReplacings)
        {
            _customReplacings = customReplacings;
            txtboxPattern_TextChanged(null, null);
        }

        private void BtCustomReplacings_Click(object sender, EventArgs e)
        {
            string filePath = FileService.GetJsonPath(FileService.CustomReplacingsNamePattern);
            try
            {
                if (!File.Exists(filePath))
                {
                    // çreate file with example dictionary entries to start with
                    File.WriteAllText(filePath, "{\n  \"Allosaurus\": \"Allo\",\n  \"Snow Owl\": \"Owl\"\n}");
                }
                Process.Start(filePath);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found\n{filePath}\n\nException: {ex.Message}", "ASB - File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // No application is associated with the specified file for this operation
                try
                {
                    // open file with notepad
                    Process.Start("notepad.exe", filePath);
                }
                catch
                {
                    try
                    {
                        // open explorer and display file
                        Process.Start("explorer.exe", @"/select,""" + filePath + "\"");
                    }
                    catch
                    {
                        MessageBox.Show("The file couldn't be opened\n" + filePath, "ASB error while opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            InsertText((string)((Button)sender).Tag);
        }

        private void InsertText(string text)
        {
            int selectionIndex = txtboxPattern.SelectionStart;
            txtboxPattern.Text = txtboxPattern.Text.Insert(selectionIndex, text);
            txtboxPattern.SelectionStart = selectionIndex + text.Length;
            txtboxPattern.Focus();
        }

        public string NamePattern
        {
            get => txtboxPattern.Text;
            set => txtboxPattern.Text = value;
        }

        private static Dictionary<string, string> PatternExplanations(bool isGlowSpecies) => new Dictionary<string, string>()
            {
                { "species", "species name" },
                { "spcsNm", "species name without vowels" },
                { "firstWordOfOldest", "the first word of the name of the first added creature of the species" },

                {"owner", "name of the owner of the creature" },
                {"tribe", "name of the tribe the creature belongs to" },
                {"server", "name of the server the creature is assigned to" },

                { "sex", "sex (\"Male\", \"Female\", \"Unknown\")" },
                { "sex_short", "\"M\", \"F\", \"U\"" },
                { "n", "if the name is not unique, the smallest possible number is appended (only creatues with a given sex are considered)." },

                { "hp", "Level of " + Utils.StatName((int)StatNames.Health, glowSpecies: isGlowSpecies) },
                { "st", "Level of " + Utils.StatName((int)StatNames.Stamina, glowSpecies: isGlowSpecies) },
                { "to", "Level of " + Utils.StatName((int)StatNames.Torpidity, glowSpecies: isGlowSpecies) },
                { "ox", "Level of " + Utils.StatName((int)StatNames.Oxygen, glowSpecies: isGlowSpecies) },
                { "fo", "Level of " + Utils.StatName((int)StatNames.Food, glowSpecies: isGlowSpecies) },
                { "wa", "Level of " + Utils.StatName((int)StatNames.Water, glowSpecies: isGlowSpecies) },
                { "te", "Level of " + Utils.StatName((int)StatNames.Temperature, glowSpecies: isGlowSpecies) },
                { "we", "Level of " + Utils.StatName((int)StatNames.Weight, glowSpecies: isGlowSpecies) },
                { "dm", "Level of " + Utils.StatName((int)StatNames.MeleeDamageMultiplier, glowSpecies: isGlowSpecies) },
                { "sp", "Level of " + Utils.StatName((int)StatNames.SpeedMultiplier, glowSpecies: isGlowSpecies) },
                { "fr", "Level of " + Utils.StatName((int)StatNames.TemperatureFortitude, glowSpecies: isGlowSpecies) },
                { "cr", "Level of " + Utils.StatName((int)StatNames.CraftingSpeedMultiplier, glowSpecies: isGlowSpecies) },

                { "hp_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Health, glowSpecies: isGlowSpecies) },
                { "st_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Stamina, glowSpecies: isGlowSpecies) },
                { "to_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Torpidity, glowSpecies: isGlowSpecies) },
                { "ox_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Oxygen, glowSpecies: isGlowSpecies) },
                { "fo_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Food, glowSpecies: isGlowSpecies) },
                { "wa_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Water, glowSpecies: isGlowSpecies) },
                { "te_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Temperature, glowSpecies: isGlowSpecies) },
                { "we_vb", "Breeding value of "+ Utils.StatName((int)StatNames.Weight, glowSpecies: isGlowSpecies) },
                { "dm_vb", "Breeding value of "+ Utils.StatName((int)StatNames.MeleeDamageMultiplier, glowSpecies: isGlowSpecies) },
                { "sp_vb", "Breeding value of "+ Utils.StatName((int)StatNames.SpeedMultiplier, glowSpecies: isGlowSpecies) },
                { "fr_vb", "Breeding value of "+ Utils.StatName((int)StatNames.TemperatureFortitude, glowSpecies: isGlowSpecies) },
                { "cr_vb", "Breeding value of "+ Utils.StatName((int)StatNames.CraftingSpeedMultiplier, glowSpecies: isGlowSpecies) },

                { "isTophp", "if hp is top, it will return 1 and nothing if it's not top. Combine with the if-function. All stat name abbreviations are possible, e.g. replace hp with st, to, ox etc."},
                { "isNewTophp", "if hp is higher than the current top hp, it will return 1 and nothing else. Combine with the if-function. All stat name abbreviations are possible."},

                { "effImp_short", "Short Taming-effectiveness or Imprinting (if tamed / bred)"},
                { "index",        "Index in library (same species)."},
                { "oldname", "the old name of the creature" },
                { "sex_lang", "sex (\"Male\", \"Female\", \"Unknown\") by loc" },
                { "sex_lang_short", "\"Male\", \"Female\", \"Unknown\" by loc(short)" },
                { "sex_lang_gen", "sex (\"Male_gen\", \"Female_gen\", \"Unknown_gen\") by loc" },
                { "sex_lang_short_gen", "\"Male_gen\", \"Female_gen\", \"Unknown_gen\" by loc(short)" },

                { "topPercent", "Percentage of the considered stat levels compared to the top levels of the species in the library" },
                { "baselvl", "Base-level (level without manually added ones), i.e. level right after taming / hatching" },
                { "effImp", "Taming-effectiveness or Imprinting (if tamed / bred)" },
                { "muta", "Mutations. Numbers larger than 99 will be displayed as 99" },
                { "gen", "Generation" },
                { "gena", "Generation in letters (0=A, 1=B, 26=AA, 27=AB)" },
                { "genn", "The number of creatures with the same species and the same generation plus one" },
                { "nr_in_gen", "The number of the creature in its generation, ordered by added to the library" },
                { "rnd", "6-digit random number in the range 100000 - 999999" },
                { "tn", "number of creatures of the current species in the library + 1" },
                { "sn", "number of creatures of the current species with the same sex in the library + 1" },
                { "dom", "how the creature was domesticated, \"T\" for tamed, \"B\" for bred" },
                { "arkid", "the Ark-Id (as entered or seen ingame)"},
                { "highest1l", "the highest stat-level of this creature (excluding torpidity)" },
                { "highest2l", "the second highest stat-level of this creature (excluding torpidity)" },
                { "highest3l", "the third highest stat-level of this creature (excluding torpidity)" },
                { "highest4l", "the fourth highest stat-level of this creature (excluding torpidity)" },
                { "highest5l", "the fifth highest stat-level of this creature (excluding torpidity)" },
                { "highest6l", "the sixth highest stat-level of this creature (excluding torpidity)" },
                { "highest1s", "the name of the highest stat-level of this creature (excluding torpidity)" },
                { "highest2s", "the name of the second highest stat-level of this creature (excluding torpidity)" },
                { "highest3s", "the name of the third highest stat-level of this creature (excluding torpidity)" },
                { "highest4s", "the name of the fourth highest stat-level of this creature (excluding torpidity)" },
                { "highest5s", "the name of the fifth highest stat-level of this creature (excluding torpidity)" },
                { "highest6s", "the name of the sixth highest stat-level of this creature (excluding torpidity)" },
            };

        private static Dictionary<string, string> FunctionExplanations() => new Dictionary<string, string>()
        {
            {"if", "{{#if: string | if string is not empty | if string is emtpy }}, to check if a string is empty. E.g. you can check if a stat is a top stat of that species (i.e. highest in library).\n{{#if: {isTophp} | bestHP{hp} | notTopHP }}" },
            {"ifexpr", "{{#ifexpr: expression | true | false }}, to check if an expression with two operands and one operator is true or false. Possible operators are ==, !=, <, <=, <, >=.\n{{#ifexpr: {topPercent} > 80 | true | false }}" },
            {"substring","{{#substring: text | start | length }}. Length can be ommited. If start is negative it takes the characters from the end.\n{{#substring: {species} | 0 | 4 }}"},
            {"replace","{{#replace: text | find | replaceBy }}\n{{#replace: {species} | Abberant | Ab }}"},
            {"customreplace","{{#customreplace: text }}. Replaces the text with a value saved in the file customReplacings.json.\nIf a second parameter is given, that is returned if the key is not available.\n{{#customreplace: {species} }}"},
            {"float divide by","{{#float_div: number | divisor | formatString }}, can be used to display stat-values in thousands, e.g. '{{#float_div: {hp_vb} | 1000 | F2 }}kHP'.\n{{#float_div: {hp_vb} | 1000 | F2 }}"},
            {"divide by","{{#div: number | divisor }}, can be used to display stat-values in thousands, e.g. '{{#div: {hp_vb} | 1000 }}kHP'.\n{{#div: {hp_vb} | 1000 }}"},
            {"padleft","{{#padleft: number | length | padding character }}\n{{#padleft: {hp_vb} | 8 | 0 }}"},
            {"padright","{{#padright: number | length | padding character }}\n{{#padright: {hp_vb} | 8 | _ }}"},
            {"casing","{{#casing: text | casingtype (U, L, T) }}. U for UPPER, L for lower, T for Title.\n{{#casing: {species} | U }}"},
            {"time","{{#time: formatString }}\n{{#time: yyyy-MM-dd_HH:mm }}"},
            {"format","{{#format: number | formatString }}\n{{#format: {hp_vb} | 000000 }}"},
            {"color","{{#color: regionId | colorName }}. Returns the colorId of the region. If the second parameter is not empty, the color name will be returned.\n{{#color: 0 | true }}"},
            {"indexof","{{#indexof: source string | string to find }}. Returns the index of the second parameter in the first parameter. If the string is not contained, an empty string will be returned.\n{{#indexof: hello | ll }}"},
        };

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtboxPattern.Text = string.Empty;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/cadon/ARKStatsExtractor/wiki/Name-Generator");
        }

        public int SplitterDistance
        {
            get => splitContainer1.SplitterDistance;
            set => splitContainer1.SplitterDistance = value;
        }

        private async void txtboxPattern_TextChanged(object sender, EventArgs e)
        {
            if (!cbPreview.Checked) return;

            cancelSource?.Cancel();
            using (cancelSource = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(500, cancelSource.Token); // display preview only at interval
                    DisplayPreview();
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            cancelSource = null;
        }

        private void DisplayPreview()
        {
            cbPreview.Text = NamePatterns.GenerateCreatureName(_creature, _females, _males, _speciesTopLevels, _customReplacings, false, -1, false, txtboxPattern.Text, false);
        }
    }
}

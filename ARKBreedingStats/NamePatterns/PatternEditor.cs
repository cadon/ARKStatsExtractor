using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.Library;
using ARKBreedingStats.Properties;
using ARKBreedingStats.Updater;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.NamePatterns
{
    public partial class PatternEditor : Form
    {
        private readonly Creature _creature;
        private readonly Creature[] _creaturesOfSameSpecies;
        private readonly Creature _alreadyExistingCreature;
        private readonly int[] _speciesTopLevels;
        private readonly int[] _speciesLowestLevels;
        private readonly int _libraryCreatureCount;
        private readonly CreatureCollection.ColorExisting[] _colorExistings;
        private Dictionary<string, string> _customReplacings;
        private readonly Dictionary<string, string> _tokenDictionary;
        private readonly Debouncer _updateNameDebouncer = new Debouncer();
        private Action<PatternEditor> _reloadCallback;
        private TableLayoutPanel _tableLayoutPanelKeys;
        private TableLayoutPanel _tableLayoutPanelFunctions;
        private TableLayoutPanel _tableLayoutPanelTemplates;
        private List<Panel> _listKeys;
        private List<Panel> _listFunctions;
        private List<Panel> _listTemplates;
        private Debouncer _keyDebouncer;
        private Debouncer _functionDebouncer;

        public PatternEditor()
        {
            InitializeComponent();
        }

        public PatternEditor(Creature creature, Creature[] creaturesOfSameSpecies, int[] speciesTopLevels, int[] speciesLowestLevels, CreatureCollection.ColorExisting[] colorExistings, Dictionary<string, string> customReplacings, int namingPatternIndex, Action<PatternEditor> reloadCallback, int libraryCreatureCount) : this()
        {
            Utils.SetWindowRectangle(this, Settings.Default.PatternEditorFormRectangle);
            if (Settings.Default.PatternEditorSplitterDistance > 0)
                SplitterDistance = Settings.Default.PatternEditorSplitterDistance;

            InitializeLocalization();

            _creature = creature;
            _creaturesOfSameSpecies = creaturesOfSameSpecies;
            _speciesTopLevels = speciesTopLevels;
            _speciesLowestLevels = speciesLowestLevels;
            _colorExistings = colorExistings;
            _customReplacings = customReplacings;
            _reloadCallback = reloadCallback;
            _libraryCreatureCount = libraryCreatureCount;
            txtboxPattern.Text = Properties.Settings.Default.NamingPatterns?[namingPatternIndex] ?? string.Empty;
            CbPatternNameToClipboardAfterManualApplication.Checked = Properties.Settings.Default.PatternNameToClipboardAfterManualApplication;
            txtboxPattern.SelectionStart = txtboxPattern.Text.Length;

            Text = $"Naming Pattern Editor: pattern {namingPatternIndex + 1}";

            _alreadyExistingCreature = _creaturesOfSameSpecies?.FirstOrDefault(c => c.guid == creature.guid);
            _tokenDictionary = NamePatterns.NamePattern.CreateTokenDictionary(creature, _alreadyExistingCreature, _creaturesOfSameSpecies, _speciesTopLevels, _speciesLowestLevels, _libraryCreatureCount);
            _keyDebouncer = new Debouncer();
            _functionDebouncer = new Debouncer();

            _tableLayoutPanelKeys = CreateTableLayoutPanel();
            TlpKeysFunctions.Controls.Add(_tableLayoutPanelKeys);
            _listKeys = new List<Panel>();
            SetControlsToTable(_tableLayoutPanelKeys, PatternExplanations(creature.Species.statNames), _listKeys);

            _tableLayoutPanelFunctions = CreateTableLayoutPanel();
            TlpKeysFunctions.Controls.Add(_tableLayoutPanelFunctions);
            _listFunctions = new List<Panel>();
            SetControlsToTable(_tableLayoutPanelFunctions, FunctionExplanations(), _listFunctions, false, true, 306);

            void SetControlsToTable(TableLayoutPanel tlp, Dictionary<string, string> nameExamples, List<Panel> entries, bool columns = true, bool useExampleAsInput = false, int buttonWidth = 120)
            {
                int i = 0;
                foreach (KeyValuePair<string, string> p in nameExamples)
                {
                    var entry = new NamePatternEntry { FilterString = p.Key };
                    entries.Add(entry);

                    Button btn = new Button
                    {
                        Size = new Size(buttonWidth, 23),
                        Text = p.Key,
                        Dock = DockStyle.Left
                    };
                    int substringUntil = p.Value.LastIndexOf("\n");
                    btn.Tag = useExampleAsInput ? p.Value.Substring(substringUntil + 1) : $"{{{p.Key}}}";

                    if (!columns)
                        btn.Dock = DockStyle.Top;
                    btn.Click += Btn_Click;

                    Label lbl = new Label
                    {
                        Dock = DockStyle.Fill,
                        //Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                        //MinimumSize = new Size(50, 40),
                        AutoSize = true,
                        Text = useExampleAsInput ? p.Value.Substring(0, substringUntil) : p.Value + (_tokenDictionary.ContainsKey(p.Key) ? ". E.g. \"" + _tokenDictionary[p.Key] + "\"" : ""),
                        Padding = new Padding(3, 3, 3, 5)
                    };
                    entry.Controls.Add(lbl);

                    if (!columns && p.Value.Contains("#customreplace"))
                    {
                        // button to open custom replacings file
                        var panel = new Panel { Dock = DockStyle.Bottom, AutoSize = true, MinimumSize = new Size(0, 27) };

                        const int buttonCustomReplacingWidth = 100;
                        var btCustomReplacings = new Button
                        {
                            Text = "Open file",
                            Height = 23,
                            Width = buttonCustomReplacingWidth,
                            Dock = DockStyle.Left
                        };
                        btCustomReplacings.Click += BtCustomReplacings_Click;
                        panel.Controls.Add(btCustomReplacings);

                        var btCustomReplacingsReload = new Button
                        {
                            Text = "Reload file",
                            Width = buttonCustomReplacingWidth,
                            Dock = DockStyle.Left
                        };
                        btCustomReplacingsReload.Click += (s, e) => _reloadCallback?.Invoke(this);
                        panel.Controls.Add(btCustomReplacingsReload);

                        var btCustomReplacingsFilePath = new Button
                        {
                            Text = "Select file",
                            Width = buttonCustomReplacingWidth,
                            Dock = DockStyle.Left
                        };
                        btCustomReplacingsFilePath.Click += ChangeCustomReplacingsFilePath;
                        panel.Controls.Add(btCustomReplacingsFilePath);
                        entry.Controls.Add(panel);
                    }
                    entry.Controls.Add(btn);

                    tlp.RowCount++;
                    tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    tlp.Controls.Add(entry);

                    //// separator
                    entry.Controls.Add(new Panel
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        Height = 2,
                        Dock = DockStyle.Bottom,
                        Margin = new Padding(0, 3, 0, 5),
                    });

                    i++;
                }
            }

            InitializeTemplates();
        }

        protected override void OnLoad(EventArgs e)
        {
            PatternEditorRecalculateControlSizes();
            base.OnLoad(e);
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            SuspendLayout();
            base.OnResizeBegin(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            ResumeLayout();
            PatternEditorRecalculateControlSizes();
            base.OnResizeEnd(e);
        }

        TableLayoutPanel CreateTableLayoutPanel()
        {
            var newTlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill
            };

            // to deactivate the horizontal scrolling but keep the vertical scrolling,
            // apparently that is the way to go ¯\_(ツ)_/¯
            newTlp.HorizontalScroll.Maximum = 0;
            newTlp.AutoScroll = false;
            newTlp.VerticalScroll.Visible = false;
            newTlp.AutoScroll = true;
            return newTlp;
        }

        private void InitializeTemplates()
        {
            var manifestFilePath = FileService.GetPath(FileService.ManifestFileName);
            if (!File.Exists(manifestFilePath)) return;
            var asbManifest = AsbManifest.FromJsonFile(manifestFilePath);
            var templateFileRelativePath = asbManifest?.modules?.Values.FirstOrDefault(m => m.Category == "Name Pattern Templates")?.LocalPath;
            if (templateFileRelativePath == null) return;
            var templateFilePath = FileService.GetPath(templateFileRelativePath);
            if (!File.Exists(templateFilePath)) return;
            if (!FileService.LoadJsonFile(templateFilePath, out ValueModule<PatternTemplate[]> module, out _)) return;
            var templates = module.Data?.Where(t => !string.IsNullOrEmpty(t.Pattern)).ToArray();
            if (templates == null || !templates.Any()) return;

            _tableLayoutPanelTemplates = CreateTableLayoutPanel();
            TabPagePatternTemplates.Controls.Add(_tableLayoutPanelTemplates);
            _listTemplates = new List<Panel>();

            foreach (var t in templates)
            {
                var panel = new Panel
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                    Padding = new Padding(3)
                };
                _listTemplates.Add(panel);

                var btn = new Button
                {
                    Size = new Size(50, 23),
                    Text = t.Title,
                    Dock = DockStyle.Top,
                    Tag = t.Pattern
                };

                var tbPattern = new TextBox
                {
                    ReadOnly = true,
                    Text = t.Pattern,
                    Dock = DockStyle.Top,
                    Padding = new Padding(3)
                };

                btn.Click += Btn_Click;

                Label lbl = new Label
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Text = t.Description,
                    Padding = new Padding(3, 3, 3, 5)
                };
                panel.Controls.Add(lbl);

                panel.Controls.Add(tbPattern);
                panel.Controls.Add(btn);

                _tableLayoutPanelTemplates.RowCount++;
                _tableLayoutPanelTemplates.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                _tableLayoutPanelTemplates.Controls.Add(panel);

                //// separator
                panel.Controls.Add(new Panel
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Height = 2,
                    Dock = DockStyle.Bottom,
                    Margin = new Padding(0, 3, 0, 5)
                });
            }
        }

        private void PatternEditorRecalculateControlSizes()
        {
            ManuallySetControlSizes(_tableLayoutPanelKeys, _listKeys);
            ManuallySetControlSizes(_tableLayoutPanelFunctions, _listFunctions);
            ManuallySetControlSizes(_tableLayoutPanelTemplates, _listTemplates);
        }

        /// <summary>
        /// It doesn't work automatically.
        /// </summary>
        private static void ManuallySetControlSizes(TableLayoutPanel tlp, List<Panel> entries)
        {
            if (tlp == null || entries == null) return;

            tlp.SuspendLayout();
            var tableWidth = tlp.Width - 25;

            foreach (var entry in entries)
            {
                var extraHeight = 0;
                foreach (Control c in entry.Controls)
                {
                    switch (c)
                    {
                        case Label lbl:
                            lbl.MaximumSize = new Size(tableWidth - lbl.Left, 0);
                            break;
                        case Button bt:
                            if (bt.Right > tableWidth)
                                bt.Width = tableWidth - bt.Left;
                            break;
                        case Panel p:
                            extraHeight += p.Height;
                            break;
                    }
                }

                entry.Height = 30; // min
                int maxBottom = 0;
                foreach (Control ctl in entry.Controls)
                {
                    if (ctl.Bottom + extraHeight > maxBottom)
                        maxBottom = ctl.Bottom + extraHeight + 8; // + margin
                }

                if (entry.Height < maxBottom) entry.Height = maxBottom;
            }
            tlp.ResumeLayout();
        }

        private void InitializeLocalization()
        {
            Loc.ControlText(buttonOK, "OK");
            Loc.ControlText(buttonCancel, "Cancel");
        }

        private void ChangeCustomReplacingsFilePath(object sender, EventArgs e)
        {
            string selectedFilePath = Properties.Settings.Default.CustomReplacingFilePath;
            if (string.IsNullOrEmpty(selectedFilePath))
                selectedFilePath = FileService.GetJsonPath(FileService.CustomReplacingsNamePattern);

            string selectedFolder =
                string.IsNullOrEmpty(selectedFilePath) ? null : Path.GetDirectoryName(selectedFilePath);

            var selectedFileName = string.IsNullOrEmpty(selectedFilePath) ? null : Path.GetFileName(selectedFilePath);

            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = $"ASB Custom Replacings File (*.json)|*.json",
                CheckFileExists = true,
                InitialDirectory = selectedFolder,
                FileName = selectedFileName
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.CustomReplacingFilePath = dlg.FileName;
                    _reloadCallback?.Invoke(this);
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
                    // create file with example dictionary entries to start with
                    File.WriteAllText(filePath, "{\n  \"Allosaurus\": \"Allo\",\n  \"Snow Owl\": \"Owl\"\n}");
                }
                Process.Start(filePath);
            }
            catch (FileNotFoundException ex)
            {
                MessageBoxes.ExceptionMessageBox(ex);
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
                    catch (Exception ex)
                    {
                        MessageBoxes.ExceptionMessageBox(ex, $"The file couldn't be opened\n{filePath}");
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

        public string NamePattern => txtboxPattern.Text;

        public bool PatternNameToClipboardAfterManualApplication => CbPatternNameToClipboardAfterManualApplication.Checked;

        private static Dictionary<string, string> PatternExplanations(Dictionary<string, string> customStatNames) => new Dictionary<string, string>()
            {
                { "species", "species name" },
                { "spcsNm", "species name without vowels" },
                { "firstWordOfOldest", "the first word of the name of the first added creature of the species" },

                { "owner", "name of the owner of the creature" },
                { "tribe", "name of the tribe the creature belongs to" },
                { "server", "name of the server the creature is assigned to" },

                { "sex", "sex (\"Male\", \"Female\", \"Unknown\")" },
                { "sex_short", "\"M\", \"F\", \"U\"" },
                { "n", "will be replaced with the smallest integer >= 1 that makes the name unique in the library. To only display a number if needed use something like {{#ifexpr: {n} > 1 | _{n} }}" },

                { "hp", "Level of " + Utils.StatName(Stats.Health, customStatNames:customStatNames) },
                { "st", "Level of " + Utils.StatName(Stats.Stamina, customStatNames:customStatNames) },
                { "to", "Level of " + Utils.StatName(Stats.Torpidity, customStatNames:customStatNames) },
                { "ox", "Level of " + Utils.StatName(Stats.Oxygen, customStatNames:customStatNames) },
                { "fo", "Level of " + Utils.StatName(Stats.Food, customStatNames:customStatNames) },
                { "wa", "Level of " + Utils.StatName(Stats.Water, customStatNames:customStatNames) },
                { "te", "Level of " + Utils.StatName(Stats.Temperature, customStatNames:customStatNames) },
                { "we", "Level of " + Utils.StatName(Stats.Weight, customStatNames:customStatNames) },
                { "dm", "Level of " + Utils.StatName(Stats.MeleeDamageMultiplier, customStatNames:customStatNames) },
                { "sp", "Level of " + Utils.StatName(Stats.SpeedMultiplier, customStatNames:customStatNames) },
                { "fr", "Level of " + Utils.StatName(Stats.TemperatureFortitude, customStatNames:customStatNames) },
                { "cr", "Level of " + Utils.StatName(Stats.CraftingSpeedMultiplier, customStatNames:customStatNames) },

                { "hp_vb", "Breeding value of "+ Utils.StatName(Stats.Health, customStatNames:customStatNames) },
                { "st_vb", "Breeding value of "+ Utils.StatName(Stats.Stamina, customStatNames:customStatNames) },
                { "to_vb", "Breeding value of "+ Utils.StatName(Stats.Torpidity, customStatNames:customStatNames) },
                { "ox_vb", "Breeding value of "+ Utils.StatName(Stats.Oxygen, customStatNames:customStatNames) },
                { "fo_vb", "Breeding value of "+ Utils.StatName(Stats.Food, customStatNames:customStatNames) },
                { "wa_vb", "Breeding value of "+ Utils.StatName(Stats.Water, customStatNames:customStatNames) },
                { "te_vb", "Breeding value of "+ Utils.StatName(Stats.Temperature, customStatNames:customStatNames) },
                { "we_vb", "Breeding value of "+ Utils.StatName(Stats.Weight, customStatNames:customStatNames) },
                { "dm_vb", "Breeding value of "+ Utils.StatName(Stats.MeleeDamageMultiplier, customStatNames:customStatNames) },
                { "sp_vb", "Breeding value of "+ Utils.StatName(Stats.SpeedMultiplier, customStatNames:customStatNames) },
                { "fr_vb", "Breeding value of "+ Utils.StatName(Stats.TemperatureFortitude, customStatNames:customStatNames) },
                { "cr_vb", "Breeding value of "+ Utils.StatName(Stats.CraftingSpeedMultiplier, customStatNames:customStatNames) },

                { "isTophp", "if hp is top, it will return 1 and nothing if it's not top. Combine with the if-function. All stat name abbreviations are possible, e.g. replace hp with st, to, ox etc."},
                { "isNewTophp", "if hp is higher than the current top hp, it will return 1 and nothing else. Combine with the if-function. All stat name abbreviations are possible."},
                { "isLowesthp", "if hp is the lowest, it will return 1 and nothing if it's not the lowest. Combine with the if-function. All stat name abbreviations are possible, e.g. replace hp with st, to, ox etc."},
                { "isNewLowesthp", "if hp is lower than the current lowest hp, it will return 1 and nothing else. Combine with the if-function. All stat name abbreviations are possible."},

                { "dom", "how the creature was domesticated, \"T\" for tamed, \"B\" for bred" },
                { "effImp", "Taming-effectiveness or Imprinting (if tamed / bred)" },
                { "effImp_short", "Short Taming-effectiveness or Imprinting (if tamed / bred)"},
                { "index",        "Index in library (same species)."},
                { "oldname", "the old name of the creature" },
                { "sex_lang", "sex (\"Male\", \"Female\", \"Unknown\") by loc" },
                { "sex_lang_short", "\"Male\", \"Female\", \"Unknown\" by loc(short)" },
                { "sex_lang_gen", "sex (\"Male_gen\", \"Female_gen\", \"Unknown_gen\") by loc" },
                { "sex_lang_short_gen", "\"Male_gen\", \"Female_gen\", \"Unknown_gen\" by loc(short)" },

                { "topPercent", "Percentage of the considered stat levels compared to the top levels of the species in the library" },
                { "baselvl", "Base-level (level without manually added ones), i.e. level right after taming / hatching" },
                { "muta", "Mutations" },
                { "mutam", "maternal mutations" },
                { "mutap", "paternal mutations" },
                { "gen", "Generation" },
                { "gena", "Generation in letters (0=A, 1=B, 26=AA, 27=AB)" },
                { "genn", "The number of creatures with the same species and the same generation plus one" },
                { "nr_in_gen", "The number of the creature in its generation, ordered by added to the library" },
                { "nr_in_gen_sex", "The number of the creature in its generation with the same sex, ordered by added to the library" },
                { "rnd", "6-digit random number in the range 0 – 999999" },
                { "ln", "number of creatures in the library + 1" },
                { "tn", "number of creatures of the current species in the library + 1" },
                { "sn", "number of creatures of the current species with the same sex in the library + 1" },
                { "arkid", "the Ark-Id (as entered or seen in-game)"},
                { "alreadyExists", "returns 1 if the creature is already in the library, can be used with {{#if: }}"},
                { "isFlyer", "returns 1 if the creature's species is a flyer"},
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

                { "hp_m", "Mutated levels of " + Utils.StatName(Stats.Health, customStatNames:customStatNames) },
                { "st_m", "Mutated levels of " + Utils.StatName(Stats.Stamina, customStatNames:customStatNames) },
                { "to_m", "Mutated levels of " + Utils.StatName(Stats.Torpidity, customStatNames:customStatNames) },
                { "ox_m", "Mutated levels of " + Utils.StatName(Stats.Oxygen, customStatNames:customStatNames) },
                { "fo_m", "Mutated levels of " + Utils.StatName(Stats.Food, customStatNames:customStatNames) },
                { "wa_m", "Mutated levels of " + Utils.StatName(Stats.Water, customStatNames:customStatNames) },
                { "te_m", "Mutated levels of " + Utils.StatName(Stats.Temperature, customStatNames:customStatNames) },
                { "we_m", "Mutated levels of " + Utils.StatName(Stats.Weight, customStatNames:customStatNames) },
                { "dm_m", "Mutated levels of " + Utils.StatName(Stats.MeleeDamageMultiplier, customStatNames:customStatNames) },
                { "sp_m", "Mutated levels of " + Utils.StatName(Stats.SpeedMultiplier, customStatNames:customStatNames) },
                { "fr_m", "Mutated levels of " + Utils.StatName(Stats.TemperatureFortitude, customStatNames:customStatNames) },
                { "cr_m", "Mutated levels of " + Utils.StatName(Stats.CraftingSpeedMultiplier, customStatNames:customStatNames) }
            };

        // list of possible functions, expected format:
        // key: name of function
        // value: [syntax and explanation]\n[example]
        private static Dictionary<string, string> FunctionExplanations() => new Dictionary<string, string>
        {
            {"if", "{{#if: string | if string is not empty | if string is empty }}, to check if a string is empty. E.g. you can check if a stat is a top stat of that species (i.e. highest in library).\n{{#if: {isTophp} | bestHP{hp} | notTopHP }}" },
            {"ifexpr", "{{#ifexpr: expression | true | false }}, to check if an expression with two operands and one operator is true or false. Possible operators are ==, !=, <, <=, <, >=.\n{{#ifexpr: {topPercent} > 80 | true | false }}" },
            {"expr", "{{#expr: expression }}, simple calculation with two operands and one operator. Possible operators are +, -, *, /.\n{{#expr: {hp} * 2 }}" },
            {"len", "{{#len: string }}, returns the length of the passed string.\n{{#len: {isTophp}{isTopdm}{isTopwe} }}" },
            {"substring","{{#substring: text | start | length }}. Length can be omitted. If start is negative it takes the characters from the end. If length is negative it takes the characters until that position from the end\n{{#substring: {species} | 0 | 4 }}"},
            {"replace","{{#replace: text | find | replaceBy }}\n{{#replace: {species} | Aberrant | Ab }}"},
            {"regexreplace","{{#regexreplace: text | pattern | replaceBy }}\nUse &&lcub; instead {, &&vline; instead | and &&rcub; instead of }.\n{{#regexreplace: hp-st-we- | \\-$ | }}"},
            {"customreplace","{{#customreplace: text }}. Replaces the text with a value saved in the file customReplacings.json.\nIf a second parameter is given, that is returned if the key is not available.\n{{#customreplace: {species} }}"},
            {"float divide by","{{#float_div: number | divisor | formatString }}, can be used to display stat-values in thousands, e.g. '{{#float_div: {hp_vb} | 1000 | F2 }}kHP'.\n{{#float_div: {hp_vb} | 1000 | F2 }}"},
            {"divide by","{{#div: number | divisor }}, can be used to display stat-values in thousands, e.g. '{{#div: {hp_vb} | 1000 }}kHP'.\n{{#div: {hp_vb} | 1000 }}"},
            {"padleft","{{#padleft: number | length | padding character }}\n{{#padleft: {hp_vb} | 8 | 0 }}"},
            {"padright","{{#padright: number | length | padding character }}\n{{#padright: {hp_vb} | 8 | _ }}"},
            {"casing","{{#casing: text | case (U, L, T) }}. U for UPPER, L for lower, T for Title.\n{{#casing: {species} | U }}"},
            {"time","{{#time: formatString }}\n{{#time: yyyy-MM-dd_HH:mm }}"},
            {"format","{{#format: number | formatString }}\n{{#format: {hp_vb} | 000000 }}"},
            {"format_int","Like #format, but supports \"x\" in the format for hexadecimal representations. {{#format_int: number | formatString }}\n{{#format_int: {{#color: 0 }} | x2 }}"},
            {"color","{{#color: regionId | return color name | return value even for unused regions }}. Returns the colorId of the region. If the second parameter is not empty, the color name will be returned. Unused regions will only return a value if the third value is not empty.\n{{#color: 0 | true }}"},
            {"colorNew","{{#colorNew: regionId }}. Returns newInRegion if the region contains a color that is not yet available in that species. Returns newInSpecies if that color is not yet available in any region of that species.\n{{#colorNew: 0 }}"},
            {"indexof","{{#indexof: source string | string to find }}. Returns the index of the second parameter in the first parameter. If the string is not contained, an empty string will be returned.\n{{#indexof: hello | ll }}"},
            {"md5", "{{#md5: string }}, returns the md5 hash of a given string\n{{#md5: {hp}{st}{we} }}"}
        };

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtboxPattern.Text = string.Empty;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RepositoryInfo.OpenWikiPage("Name-Generator");
        }

        public int SplitterDistance
        {
            get => splitContainer1.SplitterDistance;
            set => splitContainer1.SplitterDistance = value;
        }

        private void txtboxPattern_TextChanged(object sender, EventArgs e)
        {
            if (cbPreview.Checked)
                _updateNameDebouncer.Debounce(500, DisplayPreview, Dispatcher.CurrentDispatcher);
        }

        private void DisplayPreview()
        {
            cbPreview.Text = NamePatterns.NamePattern.GenerateCreatureName(_creature, _alreadyExistingCreature, _creaturesOfSameSpecies, _speciesTopLevels, _speciesLowestLevels, _customReplacings,
                false, -1, false, txtboxPattern.Text, false, _tokenDictionary, _colorExistings, _libraryCreatureCount);
        }

        private void TbFilterKeys_TextChanged(object sender, EventArgs e)
        {
            _keyDebouncer.Debounce(300, FilterKeys, Dispatcher.CurrentDispatcher);
        }

        private void FilterKeys()
        {
            FilterEntries(_tableLayoutPanelKeys, _listKeys, TbFilterKeys.Text);
        }

        private void TbFilterFunctions_TextChanged(object sender, EventArgs e)
        {
            _functionDebouncer.Debounce(300, FilterFunctions, Dispatcher.CurrentDispatcher);
        }

        private void FilterFunctions()
        {
            FilterEntries(_tableLayoutPanelFunctions, _listFunctions, TbFilterFunctions.Text);
        }

        private static void FilterEntries(TableLayoutPanel tlp, List<Panel> namePatternEntries, string filter)
        {
            filter = string.IsNullOrEmpty(filter) ? null : filter;
            tlp.SuspendLayout();
            foreach (NamePatternEntry npe in namePatternEntries)
                npe.Visible = filter == null
                              || npe.FilterString.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1;
            tlp.ResumeLayout();
            // needed to reevaluate the need of the scrollbar
            tlp.AutoScroll = false;
            tlp.AutoScroll = true;
        }

        private void BtClearFilterKey_Click(object sender, EventArgs e)
        {
            TbFilterKeys.Text = string.Empty;
        }

        private void BtClearFilterFunctions_Click(object sender, EventArgs e)
        {
            TbFilterFunctions.Text = string.Empty;
        }
    }
}

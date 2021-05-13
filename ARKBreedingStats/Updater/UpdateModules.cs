using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.Updater
{
    public partial class UpdateModules : Form
    {
        public UpdateModules()
        {
            InitializeComponent();
            Loc.ControlText(BtOk, "OK");
            Loc.ControlText(BtCancel, "Cancel");

            var manifestFilePath = FileService.GetPath(FileService.ManifestFileName);
            if (!File.Exists(manifestFilePath))
                Task.Run(async () => await Updater.DownloadManifest());

            _asbManifest = AsbManifest.FromJsonFile(manifestFilePath);
            if (_asbManifest?.modules == null) return;

            // Display installed and available modules
            var moduleGroups = _asbManifest.modules.Where(kv => kv.Value.Category != "main").Select(kv => kv.Value)
                .GroupBy(m => m.Category);

            _checkboxesUpdateModule = new List<CheckBox>();
            _checkboxesSelectModule = new List<CheckBox>();

            FlpModules.FlowDirection = FlowDirection.TopDown;
            FlpModules.WrapContents = false;

            foreach (var g in moduleGroups)
            {
                var header = new Label { Text = g.Key, Margin = new Padding(5, 20, 0, 5), AutoSize = true };
                header.Font = new Font(header.Font.FontFamily, header.Font.Size * 2);
                FlpModules.Controls.Add(header);
                var group = g.OrderBy(m => !m.LocallyAvailable).ThenBy(m => m.Name).ToArray();
                foreach (var m in group)
                {
                    var moduleDisplay = CreateModuleControl(m);
                    FlpModules.Controls.Add(moduleDisplay);
                }
            }
        }

        private Control CreateModuleControl(AsbModule module)
        {
            var c = new TableLayoutPanel { AutoSize = true, BorderStyle = BorderStyle.FixedSingle, MinimumSize = new Size(500, 100) };
            c.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            c.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            c.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            c.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            c.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            c.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var header = new Label
            {
                Text = module.Name,
                AutoSize = true,
                Margin = new Padding(5)
            };
            header.Font = new Font(header.Font, FontStyle.Bold);
            c.Controls.Add(header);
            c.SetColumnSpan(header, 2);

            var desc = new Label
            {
                Text = (string.IsNullOrEmpty(module.Author) ? string.Empty : $"Author: {module.Author}\n") + $"{module.Description}",
                AutoSize = true,
                MaximumSize = new Size(300, 0),
                Margin = new Padding(3)
            };
            c.Controls.Add(desc, 0, 1);
            c.SetRowSpan(desc, 2);

            // versions
            var l = new Label
            { Text = $"Version\nLocal: {(module.LocallyAvailable ? module.VersionLocal.ToString() : "not downloaded")}\nOnline: {module.VersionOnline}", AutoSize = true, Margin = new Padding(3) };
            c.Controls.Add(l, 1, 1);

            string checkBoxText = null;
            if (!module.LocallyAvailable)
                checkBoxText = "Download";
            else if (module.UpdateAvailable)
            {
                checkBoxText = "Update";
                UpdateAvailable = true;
            }

            if (checkBoxText != null)
            {
                var cb = new CheckBox { Text = checkBoxText, Tag = module, Padding = new Padding(3) };
                if (module.UpdateAvailable) cb.BackColor = Color.Yellow;
                cb.CheckedChanged += (s, e) => cb.BackColor = cb.Checked ? Color.LightGreen : ((cb.Tag as AsbModule)?.UpdateAvailable ?? false) ? Color.Yellow : SystemColors.Control;

                c.Click += (s, e) => ClickCheckBox(cb);
                foreach (Control cc in c.Controls)
                    cc.Click += (s, e) => ClickCheckBox(cb);

                c.Controls.Add(cb);
                c.SetRow(cb, 2);
                c.SetColumn(cb, 1);
                _checkboxesUpdateModule.Add(cb);
            }

            if (module.Selectable)
            {
                var cb = new CheckBox { Text = "Select", Tag = module, Padding = new Padding(3) };
                cb.CheckedChanged += (s, e) => cb.BackColor = cb.Checked ? Color.LightGreen : SystemColors.Control;

                if (checkBoxText == null)
                {
                    c.Click += (s, e) => ClickCheckBox(cb);
                    foreach (Control cc in c.Controls)
                        cc.Click += (s, e) => ClickCheckBox(cb);
                }

                c.Controls.Add(cb);
                c.SetRow(cb, 3);
                c.SetColumn(cb, 1);
                _checkboxesSelectModule.Add(cb);
            }

            return c;
        }

        private void ClickCheckBox(CheckBox cb) => cb.Checked = !cb.Checked;

        /// <summary>
        /// Is only true if for an already downloaded module an update is available.
        /// </summary>
        internal bool UpdateAvailable { get; private set; }

        private AsbManifest _asbManifest;
        private readonly List<CheckBox> _checkboxesUpdateModule;
        private readonly List<CheckBox> _checkboxesSelectModule;

        internal async Task<string> DownloadRequestedModulesAsync()
        {
            if (_asbManifest == null) return null;
            var downloadModules = _checkboxesUpdateModule.Where(cb => cb.Checked).Select(cb => cb.Tag as AsbModule).ToArray();
            if (!downloadModules.Any()) return null;

            var sb = new StringBuilder();
            foreach (var module in downloadModules)
            {
                var (success, message) = await module.DownloadAsync(true);
                sb.AppendLine();
                sb.AppendLine((success ? "Success: " : "Failed: ") + message);
            }

            return sb.ToString();
        }

        public string GetSpeciesImagesFolder()
        {
            if (!(_checkboxesSelectModule?.Any() ?? false)) return null;

            return _checkboxesSelectModule.Where(cb => cb.Checked).Select(cb => cb.Tag as AsbModule)
                .FirstOrDefault(m => m?.Category == "Species Images")?.LocalPath;
        }
    }
}

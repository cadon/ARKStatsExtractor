using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.Updater
{
    public partial class UpdateModules : Form
    {
        /// <summary>
        /// Task for downloading non optional modules.
        /// </summary>
        public Task TaskDownloadingUpdates;

        public UpdateModules()
        {
            var manifestFilePath = FileService.GetPath(FileService.ManifestFileName);
            if (!File.Exists(manifestFilePath))
                return;

            _asbManifest = AsbManifest.FromJsonFile(manifestFilePath);
            if (_asbManifest?.Modules == null) return;

            InitializeComponent();
            Loc.ControlText(BtOk, "OK");
            Loc.ControlText(BtCancel, "Cancel");

            TaskDownloadingUpdates = DownloadModulesAsync(_asbManifest.Modules.Select(kv => kv.Value).Where(m => !m.Optional && m.UpdateAvailable).ToArray(), true);

            // Display installed and available modules
            var moduleGroups = _asbManifest.Modules.Where(kv => kv.Value.Optional).Select(kv => kv.Value)
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
                var onlyOneEntry = group.Length == 1;
                foreach (var m in group)
                {
                    var moduleDisplay = CreateModuleControl(m, onlyOneEntry);
                    FlpModules.Controls.Add(moduleDisplay);
                }
            }
        }

        private Control CreateModuleControl(AsbModule module, bool onlyOneEntry)
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

            var localPath = module.LocallyAvailable
                ? $"{Environment.NewLine}{Environment.NewLine}Used local path: {Path.GetFullPath(FileService.GetPath(module.LocalPath))}"
                : string.Empty;

            var desc = new Label
            {
                Text = (string.IsNullOrEmpty(module.Author) ? string.Empty : $"Author: {module.Author}\n") + $"{module.Description}{localPath}",
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

            string checkBoxDownloadText = null;
            if (!module.LocallyAvailable)
                checkBoxDownloadText = "Download";
            else if (module.UpdateAvailable)
            {
                checkBoxDownloadText = "Update";
                OptionalUpdateAvailable = true;
            }

            if (checkBoxDownloadText != null)
            {
                var cb = new CheckBox { Text = checkBoxDownloadText, Tag = module, Padding = new Padding(3) };
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

                if (checkBoxDownloadText == null)
                {
                    c.Click += (s, e) => ClickCheckBox(cb);
                    foreach (Control cc in c.Controls)
                        cc.Click += (s, e) => ClickCheckBox(cb);
                }

                // if there's only one option, choose that
                if (onlyOneEntry)
                {
                    cb.Visible = false;
                    cb.Checked = true;
                }

                c.Controls.Add(cb, 1, 3);
                _checkboxesSelectModule.Add(cb);
            }

            return c;
        }

        private void ClickCheckBox(CheckBox cb) => cb.Checked = !cb.Checked;

        /// <summary>
        /// Is only true if for an already downloaded optional module an update is available.
        /// </summary>
        internal bool OptionalUpdateAvailable { get; private set; }

        private readonly AsbManifest _asbManifest;
        private readonly List<CheckBox> _checkboxesUpdateModule;
        private readonly List<CheckBox> _checkboxesSelectModule;

        internal async Task<(string, List<string> idsSuccessfullyDownloaded)> DownloadRequestedModulesAsync()
        {
            if (_asbManifest == null) return (null, null);
            var downloadModules = _checkboxesUpdateModule.Where(cb => cb.Checked).Select(cb => cb.Tag as AsbModule).ToArray();
            if (!downloadModules.Any()) return (null, null);
            var (_, resultMessage, idsSuccessfullyDownloaded) = await DownloadModulesAsync(downloadModules);

            return (resultMessage, idsSuccessfullyDownloaded);
        }

        private static async Task<(bool success, string errorMessage, List<string> idsSuccessfullyDownloaded)> DownloadModulesAsync(AsbModule[] modules, bool displayErrorMessage = false)
        {
            var sb = new StringBuilder();
            var success = true;
            var idsSuccessfullyDownloaded = new List<string>();
            foreach (var module in modules)
            {
                var (successModule, message) = await module.DownloadAsync(true);
                success = success && successModule;
                sb.AppendLine();
                sb.AppendLine((successModule ? "Success: " : "Failed: ") + message);

                if (successModule)
                {
                    module.Initialize();
                    idsSuccessfullyDownloaded.Add(module.Id);
                }
            }

            var resultMessage = sb.ToString();
            if (!success && displayErrorMessage)
                MessageBoxes.ShowMessageBox("Error while downloading ASB modules:\n\n" + resultMessage);

            return (success, resultMessage, idsSuccessfullyDownloaded);
        }
    }
}

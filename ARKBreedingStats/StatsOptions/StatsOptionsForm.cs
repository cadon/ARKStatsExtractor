using ARKBreedingStats.StatsOptions.LevelColorSettings;
using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.StatsOptions.TopStatsSettings;

namespace ARKBreedingStats.StatsOptions
{
    internal class StatsOptionsForm : Form
    {
        private static StatsOptionsForm _displayedForm;
        protected readonly ToolTip Tt = new ToolTip();

        public static void ShowWindow(Form parent,
            StatsOptionsSettings<StatLevelColors> levelColorSettings,
            StatsOptionsSettings<ConsiderTopStats> topStatsSettings,
            int selectTabPageIndex = 0
            )
        {
            if (_displayedForm != null)
            {
                _displayedForm.BringToFront();
                return;
            }

            var f = new StatsOptionsForm
            {
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                Width = Properties.Settings.Default.LevelColorWindowRectangle.Width,
                Height = Properties.Settings.Default.LevelColorWindowRectangle.Height,
                StartPosition = FormStartPosition.Manual,
                Location = new Point(Properties.Settings.Default.LevelColorWindowRectangle.X, Properties.Settings.Default.LevelColorWindowRectangle.Y),
                Text = "Stats options"
            };

            // stat settings tab
            var tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;

            if (levelColorSettings != null)
                AddAndDock(new LevelGraphOptionsControl(levelColorSettings, f.Tt), levelColorSettings.SettingsName);
            if (topStatsSettings != null)
                AddAndDock(new ConsiderTopStatsControl(topStatsSettings, f.Tt), topStatsSettings.SettingsName);

            void AddAndDock(Control c, string tabName)
            {
                var tabPage = new TabPage(tabName);
                c.Dock = DockStyle.Fill;
                tabPage.Controls.Add(c);
                tabs.Controls.Add(tabPage);
            }

            f.Controls.Add(tabs);
            tabs.SelectedIndex = selectTabPageIndex;
            _displayedForm = f;
            f.Closed += F_Closed;
            f.Show(parent);
        }

        private static void F_Closed(object sender, EventArgs e)
        {
            if (_displayedForm == null) return;
            Properties.Settings.Default.LevelColorWindowRectangle =
                new Rectangle(_displayedForm.Left, _displayedForm.Top, _displayedForm.Width, _displayedForm.Height);
            _displayedForm.Tt.RemoveAll();
            _displayedForm.Tt.Dispose();
            _displayedForm = null;
        }
    }
}

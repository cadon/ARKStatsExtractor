using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ASB_Updater
{

    public partial class MainWindow : Window
    {

        // Update 'engine'
        private ASBUpdater updater;

        // Launch delay so users can see final output
        private const int launchDelay = 2000;

        /// <summary>
        /// Path where the application should be installed.
        /// </summary>
        private readonly string applicationPath;
        /// <summary>
        /// If the updater is launched from the main application (ARK Smart Breeding.exe),
        /// the update check was already done and a new version is assumed to be available.
        /// </summary>
        private readonly bool AssumeUpdateAvailable;
        /// <summary>
        /// Use %appdata%\StatsExtractor for the data files (e.g. values.json).
        /// </summary>
        private readonly bool UseLocalAppDataForDataFiles;

        /// <summary>
        /// Name of the application to be updated
        /// </summary>
        private const string APPLICATION_NAME = "ARK Smart Breeding";
        private const string APPLICATIONEXE_NAME = APPLICATION_NAME + ".exe";

        /// <summary>
        /// Initializes the updater window. duh.
        /// </summary>
        public MainWindow()
        {
            // Should contain the caller's filename
            var e = Environment.GetCommandLineArgs();

            //// uncomment for debugging
            //string debugFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ASBUpdaterTest");
            //Directory.CreateDirectory(debugFolder);
            ////e = new string[] { e[0], debugFolder, "doupdate" };
            //e = new string[] { e[0], debugFolder };

            // if the updater was started directly, copy it first to the temp directory and start it from there
            // so it's able to update itself.
            if (e.Length == 1)
            {
                if (AppDomain.CurrentDomain.BaseDirectory == Path.GetTempPath())
                {
                    // this application should not be called from the temp directory without arguments
                    Exit();
                    return;
                }

                string oldLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
                string newLocation = Path.Combine(Path.GetTempPath(), AppDomain.CurrentDomain.FriendlyName);
                File.Copy(oldLocation, newLocation, true);

                // backslashes and quotes in command line arguments are strange. https://stackoverflow.com/questions/9287812/backslash-and-quote-in-command-line-arguments
                string args = "\"" + AppDomain.CurrentDomain.BaseDirectory.Trim(new char[] { '\\' }) + "\"";
                Process.Start(newLocation, args);

                Exit();
                return;
            }

            // the first argument is the path to where the update should be copied
            if (e.Length > 1 && (Directory.Exists(e[1]) || File.Exists(e[1])))
            {
                applicationPath = Directory.Exists(e[1]) ? e[1] : Path.GetDirectoryName(e[1]);
                if (e.Length > 2)
                {
                    AssumeUpdateAvailable = e.Contains("doupdate");
                    UseLocalAppDataForDataFiles = e.Contains("useLocalAppData");
                }
            }
            else
            {
                Exit();
                return;
            }

            InitializeComponent();
            Init();
            Run(new Progress<ProgressReporter>(UpdateProgressBar));
        }

        /// <summary>
        /// Preps the updater for run
        /// </summary>
        private void Init()
        {
            CosturaUtility.Initialize();
            updater = new ASBUpdater();
        }

        /// <summary>
        /// Performs the check/update, launch cycle
        /// </summary>
        private async void Run(IProgress<ProgressReporter> progress)
        {
            bool wasAlreadyUptodate = true;
            bool result = true;
            if (!string.IsNullOrEmpty(applicationPath)
                && Directory.Exists(applicationPath)
                && (AssumeUpdateAvailable
                    || await CheckForUpdates(progress))
                )
            {
                wasAlreadyUptodate = false;
                result = await DoUpdate(progress);
                if (result)
                    updater.Cleanup(progress);
            }

            Launch(wasAlreadyUptodate, result, progress);
        }

        /// <summary>
        /// Checks if an update is available
        /// </summary>
        /// 
        /// <returns>true if update available</returns>
        private async Task<bool> CheckForUpdates(IProgress<ProgressReporter> progress)
        {
            var reporter = new ProgressReporter(-1);
            if (!await updater.Fetch(progress))
            {
                reporter.StatusMessage = "Fetch failed, retrying…";
                progress.Report(reporter);
                if (!await updater.Fetch(progress))
                {
                    return false;
                }
            }
            if (!updater.Parse(progress))
            {
                reporter.StatusMessage = updater.LastError();
                progress.Report(reporter);
                return false;
            }

            return updater.Check(applicationPath, progress);
        }

        /// <summary>
        /// Performs the update
        /// </summary>
        private async Task<bool> DoUpdate(IProgress<ProgressReporter> progress)
        {
            var reporter = new ProgressReporter(0, "Starting the update");
            progress.Report(reporter);
            reporter.Progress = -1;
            if (!await updater.Download(progress))
            {
                if (!await updater.Fetch(progress) || !updater.Parse(progress))
                {
                    reporter.StatusMessage = updater.LastError();
                    progress.Report(reporter);
                    return false;
                }

                reporter.StatusMessage = "Download of update failed, retrying…";
                progress.Report(reporter);
                if (!await updater.Download(progress))
                {
                    return false;
                }
            }

            CloseASB();

            if (!updater.Extract(applicationPath, UseLocalAppDataForDataFiles, progress))
            {
                reporter.StatusMessage = "Extracting update files failed, retrying…";
                progress.Report(reporter);
                if (!updater.Extract(applicationPath, UseLocalAppDataForDataFiles, progress))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Closes ASB so that the files can be updated
        /// </summary>
        private void CloseASB()
        {
            Process[] processes = Process.GetProcessesByName(APPLICATION_NAME);

            foreach (Process proc in processes)
            {
                if (proc.MainModule?.FileName.Equals(APPLICATIONEXE_NAME) ?? false)
                {
                    proc.CloseMainWindow();
                    proc.WaitForExit();
                }
            }
        }

        /// <summary>
        /// Starts ASB
        /// </summary>
        private void Launch(bool wasAlreadyUptodate, bool updateResult, IProgress<ProgressReporter> progress)
        {
            if (updateResult)
            {
                progress.Report(new ProgressReporter(100, "ASB is up to date!", ProgressReporter.State.Success));
                Process.Start(Path.Combine(applicationPath, APPLICATIONEXE_NAME), "cleanupUpdater");

                if (wasAlreadyUptodate)
                {
                    Task.Delay(launchDelay).ContinueWith(_ =>
                    {
                        Exit();
                    });
                }
            }
            else
            {
                UpdateProgressBar(new ProgressReporter(updater.LastError(), ProgressReporter.State.Error));
                UpdateProgressBar(new ProgressReporter("Updating failed.", ProgressReporter.State.Error));
                progressBar.Value = 100;
                progressBar.Foreground = new SolidColorBrush(Colors.DarkRed);
            }
            BtClose.IsEnabled = true;
        }

        /// <summary>
        /// Updates the progress bar and stage message
        /// </summary>
        private void UpdateProgressBar(ProgressReporter progressReport)
        {
            //int progress = updater.GetProgress();
            if (!string.IsNullOrEmpty(progressReport.StatusMessage))
            {
                Brush foregroundBrush;
                switch (progressReport.MessageState)
                {
                    case ProgressReporter.State.Success:
                        foregroundBrush = new SolidColorBrush(Colors.LightGreen);
                        break;
                    case ProgressReporter.State.Error:
                        foregroundBrush = new SolidColorBrush(Colors.LightSalmon);
                        break;
                    default:
                        foregroundBrush = new SolidColorBrush(Colors.Ivory);
                        break;
                }

                StackPanelLog.Children.Add(new TextBlock() { Text = progressReport.StatusMessage, Foreground = foregroundBrush });
                ScrollViewer.ScrollToBottom();
            }

            if (progressReport.Progress >= 0)
            {
                progressBar.Value = progressReport.Progress;
            }
        }

        /// <summary>
        /// Exits the updater
        /// </summary>
        private void Exit()
        {
            try
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(this.Close));
            }
            catch
            {
                // ignored
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void CopyToClipboardClick(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (TextBlock tb in StackPanelLog.Children)
            {
                sb.AppendLine(tb.Text);
            }
            if (sb.Length != 0)
                Clipboard.SetText(sb.ToString());
        }
    }

    public class ProgressReporter
    {
        /// <summary>
        /// Progress in percent.
        /// </summary>
        public int Progress;

        public string StatusMessage;

        public readonly State MessageState;

        public ProgressReporter(int progress, string message = null, State state = State.Default)
        {
            this.Progress = progress;
            StatusMessage = message;
            MessageState = state;
        }

        public ProgressReporter(string message, State state = State.Default)
        {
            this.Progress = -1;
            StatusMessage = message;
            MessageState = state;
        }

        public enum State
        {
            Default, Error, Success
        }
    }
}

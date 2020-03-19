using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ASB_Updater
{

    public partial class MainWindow : Window
    {

        // Update 'engine'
        private IUpdater updater;

        // Launch delay so users can see final output
        private readonly int launchDelay = 2000;

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
            var e = System.Environment.GetCommandLineArgs();

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
                AssumeUpdateAvailable = e.Length > 2 && e[2] == "doupdate";
            }
            else
            {
                Exit();
                return;
            }

            InitializeComponent();
            Init();
            Run();
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
        private void Run()
        {
            bool result = true;
            if (!string.IsNullOrEmpty(applicationPath)
                && Directory.Exists(applicationPath)
                && (AssumeUpdateAvailable
                    || CheckForUpdates())
                )
            {
                result = DoUpdate();
            }

            Launch(result);
        }

        /// <summary>
        /// Checks if an update is available
        /// </summary>
        /// 
        /// <returns>true if update available</returns>
        private bool CheckForUpdates()
        {
            if (!updater.Fetch())
            {
                UpdateProgressBar("Fetch failed, retrying…");
                if (!updater.Fetch())
                {
                    return false;
                }
            }
            if (!updater.Parse())
            {
                UpdateProgressBar(updater.LastError());
                return false;
            }

            return updater.Check(applicationPath);
        }

        /// <summary>
        /// Performs the update
        /// </summary>
        private bool DoUpdate()
        {
            if (!updater.Download())
            {
                if (!updater.Fetch() || !updater.Parse())
                {
                    UpdateProgressBar(updater.LastError());
                    return false;
                }

                UpdateProgressBar("Download of update failed, retrying…");
                if (!updater.Download())
                {
                    return false;
                }
            }

            CloseASB();

            if (!updater.Extract(applicationPath))
            {
                UpdateProgressBar("Extracting update files failed, retrying…");
                if (!updater.Extract(applicationPath))
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

            if (processes == null) return;

            foreach (Process proc in processes)
            {
                if (proc.MainModule.FileName.Equals(APPLICATIONEXE_NAME))
                {
                    proc.CloseMainWindow();
                    proc.WaitForExit();
                }
            }
        }

        /// <summary>
        /// Starts ASB
        /// </summary>
        private void Launch(bool updateResult)
        {
            if (updateResult)
            {
                UpdateProgressBar("ASB up to date!");
            }
            else
            {
                UpdateProgressBar(updater.LastError());
            }

            Task.Delay(launchDelay).ContinueWith(_ =>
            {
                Process.Start(Path.Combine(applicationPath, APPLICATIONEXE_NAME));

                updater.Cleanup();
                Exit();
            });
        }

        /// <summary>
        /// Updates the progress bar and stage message
        /// </summary>
        private void UpdateProgressBar(string message)
        {
            //int progress = updater.GetProgress();

            updateStatus.Content = message;
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
                return;
            }
        }
    }
}

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

        private string executablePath = "";
        private string workingDirectory = Directory.GetCurrentDirectory();

        /// <summary>
        /// Initializes the updater window. duh.
        /// </summary>
        public MainWindow()
        {
            // Should contain the caller's filename
            var e = System.Environment.GetCommandLineArgs();

            if (e.Length == 2 && Path.GetFileName(e[1]) == "ARK Smart Breeding.exe")
                executablePath = e[1];

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
            if (CheckForUpdates())
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
                UpdateProgressBar("Fetch failed, retrying...");
                if (!updater.Fetch())
                {
                    return updater.Cleanup();
                }
            }
            else if (!updater.Parse())
            {
                UpdateProgressBar(updater.LastError());
                return updater.Cleanup();
            }

            return true;
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
                    return updater.Cleanup();
                }

                UpdateProgressBar("Download of update failed, retrying...");
                if (!updater.Download())
                {
                    return updater.Cleanup();
                }
            }

            if (executablePath != "")
            {
                workingDirectory = Path.GetDirectoryName(executablePath);
                CloseASB();
            }

            // Test directory
            else
            {
                workingDirectory = Path.Combine(workingDirectory, "test");
                executablePath = Path.Combine(workingDirectory, "ARK Smart Breeding.exe");
                if (!Directory.Exists(workingDirectory))
                {
                    Directory.CreateDirectory(workingDirectory);
                }
            }

            if (!updater.Extract(workingDirectory))
            {
                UpdateProgressBar("Extracting update files failed, retrying...");
                if (!updater.Extract(workingDirectory))
                {
                    return updater.Cleanup();
                }
            }

            return updater.Cleanup();
        }

        /// <summary>
        /// Closes ASB so that the files can be updated
        /// </summary>
        private void CloseASB()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("ARK Smart Breeding");

                foreach (Process proc in processes)
                {
                    if (proc.MainModule.FileName.Equals(executablePath))
                    {
                        proc.CloseMainWindow();
                        proc.WaitForExit();
                    }
                }
            }
            // No instances were found
            catch (System.NullReferenceException) { }
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
                Process.Start(executablePath);

                updater.Cleanup();
                Exit();
            });
        }

        /// <summary>
        /// Updates the progress bar and stage message
        /// </summary>
        private void UpdateProgressBar(string message)
        {
            int progress = updater.GetProgress();

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

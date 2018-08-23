using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ASB_Updater {

    public partial class MainWindow : Window {

        // Update 'engine'
        private IUpdater updater;

        // Launch delay so users can see final output
        private int launchDelay = 2000;

        /// <summary>
        /// Initializes the updater window. duh.
        /// </summary>
        public MainWindow() {
            InitializeComponent();

            init();
            run();
        }

        /// <summary>
        /// Preps the updater for run
        /// </summary>
        private void init() {
            CosturaUtility.Initialize();
            updater = new ASBUpdater();
        }

        /// <summary>
        /// Performs the check/update, launch cycle
        /// </summary>
        private void run() {
            bool result = true;
            if (isFirstRun() || checkForUpdates()) {
                result = doUpdate();
            }

            launch(result);
        }

        /// <summary>
        /// Checks if this is a 'first run' (no exe)
        /// </summary>
        /// 
        /// <returns>true if first run</returns>
        private bool isFirstRun() {
            return !updater.hasEXE();
        }

        /// <summary>
        /// Checks if an update is available
        /// </summary>
        /// 
        /// <returns>true if update available</returns>
        private bool checkForUpdates() {
            if (!updater.fetch()) {
                updateProgressBar("Fetch failed, retrying...");
                if (!updater.fetch()) {
                    return updater.cleanup();
                }
            }
            else if (!updater.parse()) {
                updateProgressBar(updater.lastError());
                return updater.cleanup();
            }

            return updater.check();
        }

        /// <summary>
        /// Performs the update
        /// </summary>
        private bool doUpdate() {
            if (!updater.download()) {
                if (!updater.fetch() || !updater.parse() || !updater.check()) {
                    updateProgressBar(updater.lastError());
                    return updater.cleanup();
                }

                updateProgressBar("Download of update failed, retrying...");
                if (!updater.download()) {
                    return updater.cleanup();
                }
            }

            if (!updater.extract()) {
                updateProgressBar("Extracting update files failed, retrying...");
                if (!updater.extract()) {
                    return updater.cleanup();
                }
            }

            return updater.cleanup();
        }

        /// <summary>
        /// Starts ASB
        /// </summary>
        private void launch(bool updateResult) {
            if (updateResult) {
                updateProgressBar("ASB up to date!");
            }
            else {
                updateProgressBar(updater.lastError());
            }

            if (!updater.hasEXE()) {
                updateProgressBar("ASB executable not found.");
            }

            Task.Delay(launchDelay).ContinueWith(_ => {
                if (updater.hasEXE()) {
                    Process.Start(updater.getEXE());
                }

                updater.cleanup();
                exit();
            });
        }

        /// <summary>
        /// Updates the progress bar and stage message
        /// </summary>
        private void updateProgressBar(string message) {
            int progress = updater.getProgress();

            updateStatus.Content = message;
        }

        /// <summary>
        /// Exits the updater
        /// </summary>
        private void exit() {
            try {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(this.Close));
            }
            catch {
                return;
            }
        }
    }
}

using System;
using System.IO;
using System.Threading;

namespace ARKBreedingStats
{
    class FileSync
    {
        string currentFile;
        readonly FileSystemWatcher file_watcher;
        DateTime lastUpdated;
        WatcherChangeTypes lastChangeType;
        readonly Action callbackFunction;

        public FileSync(string fileName, Action callback)
        {
            currentFile = fileName;
            callbackFunction = callback;

            file_watcher = new FileSystemWatcher();

            // Add the handler for file changes
            file_watcher.Changed += OnChanged;
            file_watcher.Created += OnChanged;
            file_watcher.Renamed += OnChanged;
            file_watcher.Deleted += OnChanged;

            // Update the file watcher's properties
            UpdateProperties();
        }

        public void ChangeFile(string newFileName)
        {
            currentFile = newFileName;

            // Update the FileSystemWatcher properties
            UpdateProperties();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFile)) return;

            if (e.ChangeType != WatcherChangeTypes.Changed &&                                                    // default || DropBox
                !(e.ChangeType == WatcherChangeTypes.Renamed && lastChangeType == WatcherChangeTypes.Deleted) && // NextCloud
                !(e.ChangeType == WatcherChangeTypes.Created && lastChangeType == WatcherChangeTypes.Deleted))   // CloudStation
            {
                lastChangeType = e.ChangeType;
                return;
            }
            lastChangeType = e.ChangeType;

            // first wait for the time the user has set
            var waitMs = Properties.Settings.Default.WaitBeforeAutoLoadMs;
            if (waitMs > 0)
                Thread.Sleep(waitMs);

            // Wait until the file is writeable
            int numberOfRetries = 5;
            int delayOnRetry = 1000;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    using (Stream unused = File.Open(currentFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        // stream isn't used, if there's no exception, continue with loading the file.
                        break;
                    }
                }
                catch (FileNotFoundException)
                {
                    return;
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                // if file is not saveable
                Thread.Sleep(delayOnRetry);
            }

            // Notify the form that the collection has been changed, but only if it's been
            // at least two seconds since last update
            if ((DateTime.Now - lastUpdated).TotalSeconds > 2)
            {
                callbackFunction(); // load collection
                lastUpdated = DateTime.Now;
            }
        }

        /// <summary>
        /// Call this function just before the tool saves the file, so the fileWatcher ignores the change.
        /// </summary>
        public void SavingStarts()
        {
            file_watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Call when the saving has finished, and the file should be watched again for changes.
        /// </summary>
        public void SavingEnds()
        {
            lastUpdated = DateTime.Now;
            file_watcher.EnableRaisingEvents = true;
        }

        private void UpdateProperties()
        {
            if (!string.IsNullOrEmpty(currentFile) && Properties.Settings.Default.syncCollection)
            {
                // Update the path notify filter and filter of the watcher
                file_watcher.Path = Directory.GetParent(currentFile).ToString();
                file_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
                file_watcher.Filter = Path.GetFileName(currentFile);
                file_watcher.EnableRaisingEvents = true;
            }
            else
            {
                file_watcher.EnableRaisingEvents = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                file_watcher.Dispose();
            }
        }
    }
}

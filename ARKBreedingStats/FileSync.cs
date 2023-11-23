using System;
using System.IO;
using System.Threading;

namespace ARKBreedingStats
{
    internal class FileSync : IDisposable
    {
        private string _currentFile;
        private readonly FileSystemWatcher _fileWatcher;
        private DateTime _lastUpdated;
        private WatcherChangeTypes _lastChangeType;
        private readonly Action _callbackFunction;

        public FileSync(string fileName, Action callback)
        {
            _currentFile = fileName;
            _callbackFunction = callback;

            _fileWatcher = new FileSystemWatcher();

            // Add the handler for file changes
            _fileWatcher.Changed += OnChanged;
            _fileWatcher.Created += OnChanged;
            _fileWatcher.Renamed += OnChanged;
            _fileWatcher.Deleted += OnChanged;

            // Update the file watcher's properties
            UpdateProperties();
        }

        /// <summary>
        /// Update the FileSystemWatcher properties
        /// </summary>
        public void ChangeFile(string newFileName)
        {
            _currentFile = newFileName;
            UpdateProperties();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFile)) return;

            if (e.ChangeType != WatcherChangeTypes.Changed &&                                                    // default || DropBox
                !(e.ChangeType == WatcherChangeTypes.Renamed && _lastChangeType == WatcherChangeTypes.Deleted) && // NextCloud
                !(e.ChangeType == WatcherChangeTypes.Created && _lastChangeType == WatcherChangeTypes.Deleted))   // CloudStation
            {
                _lastChangeType = e.ChangeType;
                return;
            }
            _lastChangeType = e.ChangeType;

            // first wait for the time the user has set
            var waitMs = Properties.Settings.Default.WaitBeforeAutoLoadMs;
            if (waitMs > 0)
                Thread.Sleep(waitMs);

            // Wait until the file is writeable
            const int numberOfRetries = 5;
            const int delayOnRetry = 1000;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    using (Stream unused = File.Open(_currentFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
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
            if ((DateTime.Now - _lastUpdated).TotalSeconds > 2)
            {
                _callbackFunction(); // load collection
                _lastUpdated = DateTime.Now;
            }
        }

        /// <summary>
        /// Call this function just before the tool saves the file, so the fileWatcher ignores the change.
        /// </summary>
        public void SavingStarts()
        {
            _fileWatcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Call when the saving has finished, and the file should be watched again for changes.
        /// </summary>
        public void SavingEnds()
        {
            _lastUpdated = DateTime.Now;
            if (!string.IsNullOrEmpty(_currentFile))
                _fileWatcher.EnableRaisingEvents = true;
        }

        private void UpdateProperties()
        {
            if (!string.IsNullOrEmpty(_currentFile) && Properties.Settings.Default.syncCollection)
            {
                // Update the path notify filter and filter of the watcher
                _fileWatcher.Path = Path.GetDirectoryName(_currentFile);
                _fileWatcher.Filter = Path.GetFileName(_currentFile);
                _fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;
                _fileWatcher.EnableRaisingEvents = true;
            }
            else
            {
                _fileWatcher.EnableRaisingEvents = false;
            }
        }

        #region Disposing

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _fileWatcher.Changed -= OnChanged;
                _fileWatcher.Created -= OnChanged;
                _fileWatcher.Renamed -= OnChanged;
                _fileWatcher.Deleted -= OnChanged;
                _fileWatcher.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}

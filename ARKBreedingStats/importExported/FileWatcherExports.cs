using System;
using System.IO;

namespace ARKBreedingStats.importExported
{
    public class FileWatcherExports : IDisposable
    {
        private readonly FileSystemWatcher _fileWatcherExport;
        private readonly Action<string, FileWatcherExports> _callbackNewFile;

        public FileWatcherExports(string folderToWatch, Action<string, FileWatcherExports> callbackNewFile)
        {
            _callbackNewFile = callbackNewFile;

            _fileWatcherExport = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            _fileWatcherExport.Created += OnChanged;
            _fileWatcherExport.Changed += OnChanged;
            SetWatchFolder(folderToWatch);
        }

        public void SetWatchFolder(string folderToWatch)
        {
            if (Directory.Exists(folderToWatch))
            {
                _fileWatcherExport.Path = folderToWatch;
                Watching = true;
            }
            else { Watching = false; }
        }

        public bool Watching
        {
            set => _fileWatcherExport.EnableRaisingEvents = value;
        }


        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _callbackNewFile?.Invoke(e.FullPath, this);
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
                _fileWatcherExport.Created -= OnChanged;
                _fileWatcherExport.Changed -= OnChanged;
                _fileWatcherExport.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}

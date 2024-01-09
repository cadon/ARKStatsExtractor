using System;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats.importExported
{
    public class FileWatcherExports : IDisposable
    {
        private readonly FileSystemWatcher _fileWatcherExport;
        private readonly Action<string, FileWatcherExports> _callbackNewFile;
        private string _lastFilePath;
        private DateTime _lastChangedTime;

        public FileWatcherExports(string folderToWatch, Action<string, FileWatcherExports> callbackNewFile, Control synchronizingObject)
        {
            _callbackNewFile = callbackNewFile;

            _fileWatcherExport = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite,
                SynchronizingObject = synchronizingObject
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
            if (_callbackNewFile == null) return;
            var filePath = e.FullPath;
            var lastWriteTime = new FileInfo(filePath).LastWriteTimeUtc;
            if (filePath == _lastFilePath && lastWriteTime == _lastChangedTime)
                return; // event was already processed. Some file changes raise multiple events

            _lastFilePath = filePath;
            _lastChangedTime = lastWriteTime;
            _callbackNewFile.Invoke(filePath, this);
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

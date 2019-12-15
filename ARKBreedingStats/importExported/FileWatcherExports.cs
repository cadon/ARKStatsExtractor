using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.importExported
{
    class FileWatcherExports
    {
        private FileSystemWatcher fileWatcherExport;
        public FileWatcherExports(string folderToWatch, Action<string> callbackNewFile, bool watching)
        {
            fileWatcherExport = new FileSystemWatcher()
            {
                Filter = "*.ini"
            };
            fileWatcherExport.Created += (sender, e) => callbackNewFile(e.FullPath);
            fileWatcherExport.Changed += (sender, e) => callbackNewFile(e.FullPath);
            SetWatchFolder(folderToWatch, watching);
        }

        public void SetWatchFolder(string folderToWatch, bool watching = true)
        {
            if (Directory.Exists(folderToWatch))
            {
                fileWatcherExport.Path = folderToWatch;
                Watching = watching;
            }
            else { Watching = false; }
        }

        public bool Watching
        {
            set => fileWatcherExport.EnableRaisingEvents = value;
        }
    }
}

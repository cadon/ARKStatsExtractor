using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ARKBreedingStats
{
    class FileSync
    {
        string currentFile = "";
        FileSystemWatcher file_watcher;
        DateTime lastUpdated;
        Action callbackFunction;

        public FileSync(string fileName, Action callback)
        {
            currentFile = fileName;
            callbackFunction = callback;

            file_watcher = new FileSystemWatcher();

            // Add the handler for file changes
            file_watcher.Changed += new FileSystemEventHandler(onChanged);

            // Update the file watcher's properties
            updateProperties();
        }

        public void changeFile(string newFileName)
        {
            currentFile = newFileName;

            // Update the FileSystemWatcher properties
            updateProperties();
        }

        private void onChanged(object source, FileSystemEventArgs e)
        {
            // Wait until the file is writeable
            int numberOfRetries = 5;
            int delayOnRetry = 1000;

            for (int i = 1; i <= numberOfRetries; ++i)
            {
                try
                {
                    using (Stream stream = File.Open(currentFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (stream != null)
                            break;
                    }
                }
                catch (FileNotFoundException)
                { return; }
                catch (IOException)
                { }
                catch (UnauthorizedAccessException)
                { }
                // if file is not saveable
                Thread.Sleep(delayOnRetry);
            }

            // Notify the form that the collection has been changed, but only if it's been
            // at least two seconds since last update
            if ((lastUpdated == null || (DateTime.Now - lastUpdated).TotalSeconds > 2) && Properties.Settings.Default.syncCollection)
            {
                callbackFunction(); // load collection
                lastUpdated = DateTime.Now;
            }
        }

        public void justSaving()
        {
            // call this function just before the tool saves the file, so the fileWatcher ignores the change
            lastUpdated = DateTime.Now;
        }

        private void updateProperties()
        {
            if (currentFile != "" && Properties.Settings.Default.syncCollection)
            {
                // Update the path notify filter and filter of the watcher
                file_watcher.Path = Directory.GetParent(currentFile).ToString();
                file_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
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

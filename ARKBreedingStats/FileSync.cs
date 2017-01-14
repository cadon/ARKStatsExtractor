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
        String currentFile = "";
        FileSystemWatcher file_watcher;
        DateTime lastUpdated;
        Action callbackFunction;
        
        public FileSync(String fileName, Action callback)
        {
            currentFile = fileName;
            callbackFunction = callback;
            
            file_watcher = new FileSystemWatcher();

            // Add the handler for file changes
            file_watcher.Changed += new FileSystemEventHandler(onChanged);

            // Update the file watcher's properties
            updateProperties();
        }

        public void changeFile(String newFileName)
        {
            currentFile = newFileName;

            // Update the FileSystemWatcher properties
            updateProperties();
        }

        private void onChanged(object source, FileSystemEventArgs e)
        {
            // Wait until the file is writeable
            while (true)
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
                { }
                catch (IOException)
                { }
                catch (UnauthorizedAccessException)
                { }
                Thread.Sleep(500);
            }

            // Notify the form that the collection has been changed, but only if it's been
            // at least two seconds since last update
            if ((lastUpdated == null || (DateTime.Now - lastUpdated).TotalSeconds > 2) && Properties.Settings.Default.syncCollection)
            {
                callbackFunction();
                lastUpdated = DateTime.Now;
            }
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
    }
}

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.Updater
{
    /// <summary>
    /// Info about a module of Asb, e.g. version of the main app, available localizations or other optional parts of the application.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class AsbModule
    {
        /// <summary>
        /// Unique identifier of this module. Must be equal to its key in the module dictionary.
        /// </summary>
        [JsonProperty]
        public string Id;
        [JsonProperty]
        public string Category;
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string Description;
        [JsonProperty]
        public string Author;
        [JsonProperty]
        public string version;
        /// <summary>
        /// The version of the online available module.
        /// </summary>
        public Version VersionOnline;
        /// <summary>
        /// The version of the locally available module.
        /// </summary>
        public Version VersionLocal;

        public bool UpdateAvailable;
        /// <summary>
        /// Timestamp of this version.
        /// </summary>
        [JsonProperty]
        public DateTime Timestamp;
        /// <summary>
        /// Url where the resource can be downloaded from.
        /// </summary>
        [JsonProperty]
        public string Url;
        /// <summary>
        /// If true, the resource is a folder with multiple files. The Folder should contain a file "_ver.txt" with a version string.
        /// </summary>
        [JsonProperty]
        public bool IsFolder;
        /// <summary>
        /// If only one module of a category can be selected.
        /// </summary>
        [JsonProperty]
        public bool Selectable;
        /// <summary>
        /// Relative path of the resource where it should be saved to.
        /// </summary>
        [JsonProperty]
        public string LocalPath;

        /// <summary>
        /// Indicates if the according resource is downloaded.
        /// </summary>
        public bool LocallyAvailable;

        [OnDeserialized]
        private void SetVersion(StreamingContext context)
        {
            Version.TryParse(version, out VersionOnline);

            // local version
            if (string.IsNullOrEmpty(LocalPath)) return;

            if (IsFolder)
            {
                var filePath = FileService.GetPath(LocalPath, "_ver.txt");
                if (File.Exists(filePath) &&
                    Version.TryParse(File.ReadAllText(filePath), out VersionLocal))
                {
                    LocallyAvailable = true;
                    UpdateAvailable = VersionOnline > VersionLocal;
                }
            }
            else
            {
                var filePath = FileService.GetPath(LocalPath);
                if (File.Exists(filePath))
                {
                    try
                    {
                        var json = JObject.Parse(File.ReadAllText(filePath));
                        var ver = json.Value<string>("Version");
                        if (!string.IsNullOrEmpty(ver) && Version.TryParse(ver, out VersionLocal))
                        {
                            LocallyAvailable = true;
                            UpdateAvailable = VersionOnline > VersionLocal;
                        }
                    }
                    catch (JsonReaderException ex)
                    {
                        Description = $"ERROR: Couldn't load json file of this module\n{ex.Message}"
                                      + (string.IsNullOrEmpty(Description) ? string.Empty : "\n\n" + Description);
                    }
                }
            }
        }

        /// <summary>
        /// Downloads the module. Assuming it's a zip file.
        /// </summary>
        /// <returns>Bool if successful, string with error message.</returns>
        public async Task<(bool, string)> DownloadAsync(bool overwrite)
        {
            if (string.IsNullOrEmpty(LocalPath))
                return (false, "LocalPath is empty, aborted.");
            if (string.IsNullOrEmpty(Url))
                return (false, "Url is empty, couldn't download anything.");

            string moduleFolderPath = FileService.GetPath(LocalPath);
            string tempFilePath = Path.GetTempFileName();
            var (success, _) = await Updater.DownloadAsync(Url, tempFilePath);
            if (!success)
                return (false, $"File\n{Url}\ncouldn't be downloaded");

            int fileCountExtracted = 0;
            int fileCountSkipped = 0;

            try
            {
                Directory.CreateDirectory(moduleFolderPath);
                using (var archive = ZipFile.OpenRead(tempFilePath))
                {
                    foreach (ZipArchiveEntry file in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(file.Name)) continue;

                        var filePathUnzipped = Path.Combine(moduleFolderPath, file.Name);
                        if (File.Exists(filePathUnzipped) &&
                            (!overwrite || !FileService.TryDeleteFile(filePathUnzipped)))
                        {
                            fileCountSkipped++;
                            continue;
                        }

                        file.ExtractToFile(filePathUnzipped);
                        fileCountExtracted++;
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error while extracting the files in {moduleFolderPath}\n\n{ex.Message}");
            }
            finally
            {
                FileService.TryDeleteFile(tempFilePath);
            }

            return (true, $"Files of {Name} were downloaded successfully.\n{fileCountExtracted} files extracted{(fileCountSkipped != 0 ? $"\n{fileCountSkipped} already existing files skipped" : string.Empty)}.");
        }
    }
}

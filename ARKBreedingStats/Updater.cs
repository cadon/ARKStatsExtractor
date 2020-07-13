using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public static class Updater
    {
        /// <summary>
        /// Latest release url.
        /// </summary>
        public const string ReleasesUrl = "https://github.com/cadon/ARKStatsExtractor/releases/latest";
        private const string ReleasesFeedUrl = "https://api.github.com/repos/cadon/ARKStatsExtractor/releases";
        internal const string UpdaterExe = "asb-updater.exe";
        private const string ObeliskUrl = "https://raw.githubusercontent.com/arkutils/Obelisk/master/data/asb/";
        private const string MasterRawUrl = "https://github.com/cadon/ARKStatsExtractor/raw/master";
        private const string SpeciesColorRegionZipFileName = "img.zip";

        #region main program

        private static bool? isProgramInstalled;

        /// <summary>
        /// Determines if running .exe is installed or at least running in Program Files folder
        /// </summary>
        public static bool IsProgramInstalled
        {
            get
            {
                if (isProgramInstalled == null)
                {
                    isProgramInstalled = IsInstalled();
                }
                return isProgramInstalled.Value;
            }
        }

        private static bool IsInstalled()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;

            // try to get registry key for installation
            string keyName = (Environment.Is64BitOperatingSystem ?
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" :
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\") + "{8DDA440C-714D-4BE6-AD7B-F549ABB1BB02}_is1";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
            {
                if (key != null)
                {
                    string installLocation = (string)key.GetValue("InstallLocation");
                    if (!string.IsNullOrEmpty(installLocation))
                    {
                        if (assemblyLocation.Replace('/', '\\').StartsWith(installLocation.Replace('/', '\\')))
                        {
                            return true;
                        }
                    }
                }
            }

            // if otherwise located in the programs folder use the installer otherwise the updater
            return assemblyLocation.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)) ||
                    assemblyLocation.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        }

        public static async Task<bool?> CheckForPortableUpdate(bool silentCheck, bool collectionDirty)
        {
            try
            {
                bool? wantsProgramUpdate = await CheckAndAskForUpdate(collectionDirty);
                if (wantsProgramUpdate == null) return null;
                if (!wantsProgramUpdate.Value) return false;

                // Launch the updater and exit this app
                LaunchUpdater();
                return true;
            }
            catch (Exception ex)
            {
                if (!silentCheck
                    && MessageBox.Show("Error while checking for new version.\n\n" +
                        $"{ex.Message}\n\n" +
                        "Try checking for an updated version of ARK Smart Breeding. " +
                        "Do you want to visit the releases page?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    Process.Start(ReleasesUrl);
            }
            return null;
        }

        /// <summary>
        /// Copies the updater to the temp directory and launches it from there
        /// </summary>
        private static void LaunchUpdater()
        {
            string oldLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdaterExe);
            string newLocation = Path.Combine(Path.GetTempPath(), UpdaterExe);

            File.Copy(oldLocation, newLocation, true);

            ProcessStartInfo startInfo = new ProcessStartInfo(newLocation);

            // backslashes and quotes in command line arguments are strange. https://stackoverflow.com/questions/9287812/backslash-and-quote-in-command-line-arguments
            var args = new System.Text.StringBuilder("\"" + AppDomain.CurrentDomain.BaseDirectory.Trim(new char[] { '\\' }) + "\"");
            // the updater doesn't need to check again if an update is available
            args.Append(" doupdate");
            // use the %localAppData%\ARK Smart Breeding folder for the values files
            if (IsProgramInstalled)
                args.Append(" useLocalAppData");

            // check if the application folder is protected, then ask for admin permissions for the updater.
            if (FileService.TestIfFolderIsProtected(AppDomain.CurrentDomain.BaseDirectory))
            {
                startInfo.Verb = "runas";
            }

            startInfo.Arguments = args.ToString();

            Process.Start(startInfo);
        }

        /// <summary>
        /// If new release exists ask to install it
        /// </summary>
        /// <param name="collectionDirty"></param>
        /// <returns>true if new release should be installed, null if it was canceled; download urls or null</returns>
        private static async Task<bool?> CheckAndAskForUpdate(bool collectionDirty)
        {
            string releaseTag = await FetchReleaseFeed();
            (bool? updateAvailable, string localVersion, string remoteVersion) = Updater.UpdateAvailable(releaseTag);

            if (updateAvailable == null)
            {
                throw new Exception($"Parsing version numbers failed. Local: {localVersion}, Remote: {remoteVersion}");
            }

            if (updateAvailable.Value)
            {
                if (MessageBox.Show("A new version of ARK Smart Breeding is available.\n" +
                        $"You have {localVersion}, available is {remoteVersion}.\n\n" +
                        "Do you want to download and install the update now?", "New version available",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    // Ensure there are no unsaved changes
                    if (collectionDirty)
                    {
                        MessageBox.Show("Your Creature Collection has been modified since it was last saved.\nThe update will abort.\n" +
                                "Please either save or discard your changes to proceed with the update.",
                                "Unsaved Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }

                    return true;
                }
            }
            return false;
        }

        #endregion

        #region release check

        /// <summary>
        /// Compares release version with local file version.
        /// </summary>
        /// <param name="releaseTag"></param>
        /// <returns>comparison result and parsed release version, or in case of an error: null and unreadable version numbers</returns>
        private static (bool? updateAvailable, string localVersion, string remoteVersion) UpdateAvailable(string releaseTag)
        {
            string releaseVersion = releaseTag.ToLowerInvariant().Trim().Trim('v', '.');

            Version.TryParse(releaseVersion, out Version tagVersion);
            Version.TryParse(Application.ProductVersion, out Version productVersion);

            if (tagVersion == null || productVersion == null)
                return (null, Application.ProductVersion, releaseVersion);

            return (tagVersion.CompareTo(productVersion) > 0, productVersion.ToString(), tagVersion.ToString());
        }

        public static async Task<string> FetchReleaseFeed()
        {
            string releaseFeed;
            try
            {
                (_, releaseFeed) = await DownloadAsync(ReleasesFeedUrl);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Fetching release info failed: " + e.Message);
                throw;
            }

            try
            {
                return ParseReleaseInfo(releaseFeed);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Parsing release info failed: " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Parses the releases feed for relevant information
        /// </summary>
        /// <param name="releaseFeed"></param>
        /// <returns>Tuple containing tag of the release version and list of urls</returns>
        private static string ParseReleaseInfo(string releaseFeed)
        {
            JArray releaseInfoNode = JArray.Parse(releaseFeed);
            JObject latest = (JObject)releaseInfoNode[0];

            string tag = latest.Value<string>("tag_name");

            Debug.WriteLine("Tag: " + tag);

            return tag;
        }

        #endregion

        /// <summary>
        /// Downloads a file from the given URL, returns as string or writes it to the given destination
        /// </summary>
        /// <param name="url">The URL to download from</param>
        /// <param name="outFileName">File to output contents to</param>
        /// <returns>content or null</returns>
        private static async Task<(bool, string)> DownloadAsync(string url, string outFileName = null)
        {
            using (WebClient client = new WebClient())
            {
                bool successfulDownloaded = true;
                client.Headers.Add("User-Agent", "ASB");

                Debug.WriteLine("URL: " + url);
                Debug.WriteLine("Out File: " + outFileName);

                if (string.IsNullOrEmpty(outFileName))
                {
                    return (successfulDownloaded, await client.DownloadStringTaskAsync(url));
                }

                try
                {
                    string directory = Path.GetDirectoryName(outFileName);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    await client.DownloadFileTaskAsync(url, outFileName);
                }
                catch (Exception e)
                {
                    successfulDownloaded = false;
                    MessageBox.Show($"Error while trying to download the file\n{url}\n\n{e.Message}{(e.InnerException == null ? string.Empty : $"\n\n{e.InnerException.Message}")}",
                        $"Download error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!File.Exists(outFileName))
                    throw new FileNotFoundException($"Downloading file from {url} failed", outFileName);

                return (successfulDownloaded, null);
            }
        }

        /// <summary>
        /// Downloads a file from the given URL and writes it to the given destination
        /// </summary>
        /// <param name="url">The URL to download from</param>
        /// <param name="outFileName">File to output contents to</param>
        private static bool Download(string url, string outFileName)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "ASB");

                Debug.WriteLine("URL: " + url);
                Debug.WriteLine("Out File: " + outFileName);

                if (string.IsNullOrEmpty(outFileName))
                {
                    return false;
                }

                try
                {
                    string directory = Path.GetDirectoryName(outFileName);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    client.DownloadFile(url, outFileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Error while trying to download the file\n{url}\n\n{e.Message}", $"Download error - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!File.Exists(outFileName))
                    throw new FileNotFoundException($"Downloading file from {url} failed", outFileName);

                return true;
            }
        }

        /// <summary>
        /// Downloads the species color region images and saves them in the data folder.
        /// </summary>
        internal static async Task<(bool, string)> DownloadSpeciesImages(bool overwrite)
        {
            string imagesFolderPath = FileService.GetPath(FileService.ImageFolderName);
            string url = MasterRawUrl + "/" + SpeciesColorRegionZipFileName;
            string tempFilePath = Path.GetTempFileName();
            var (downloaded, _) = await DownloadAsync(url, tempFilePath);
            if (!downloaded)
                return (false, $"File {url} couldn't be downloaded");

            int fileCountExtracted = 0;
            int fileCountSkipped = 0;

            try
            {
                Directory.CreateDirectory(imagesFolderPath);
                using (var archive = ZipFile.OpenRead(tempFilePath))
                {
                    foreach (ZipArchiveEntry file in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(file.Name)) continue;

                        var filePathUnzipped = Path.Combine(imagesFolderPath, file.Name);
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
                return (false, $"Error while extracting the files in {imagesFolderPath}\n\n{ex.Message}");
            }
            finally
            {
                FileService.TryDeleteFile(tempFilePath);
            }

            return (true, $"Image files were downloaded successfully.\n{fileCountExtracted} images extracted\n{fileCountSkipped} already existing images skipped");
        }

        #region mod values

        internal static async Task<bool> DownloadModsManifest()
        {
            string tempFilePath = Path.GetTempFileName();
            string valuesFolder = FileService.GetJsonPath(FileService.ValuesFolder);
            string destFilePath = Path.Combine(valuesFolder, FileService.ModsManifest);
            if (!Directory.Exists(valuesFolder))
                Directory.CreateDirectory(valuesFolder);

            try
            {
                if ((await DownloadAsync(ObeliskUrl + FileService.ModsManifest,
                    tempFilePath)).Item1)
                {
                    // if successful downloaded, move tempFile
                    try
                    {
                        if (File.Exists(destFilePath)) File.Delete(destFilePath);
                        File.Move(tempFilePath, destFilePath);
                        return true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error while moving mod-manifest file:\n\n" + e.Message, $"{Loc.S("error")} - {Utils.ApplicationNameVersion}",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while downloading mod-manifest:\n\n" + e.Message, $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                TryDeleteFile(tempFilePath);
            }

            return false;
        }

        /// <summary>
        /// Tries to delete the given file without throwing an error on failing
        /// </summary>
        /// <param name="filePath"></param>
        private static void TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
            catch { }
        }

        internal static async Task<bool> DownloadModValuesFileAsync(string modValuesFileName)
        {
            try
            {
                await DownloadAsync(ObeliskUrl + modValuesFileName,
                    FileService.GetJsonPath(Path.Combine(FileService.ValuesFolder, modValuesFileName)));
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while downloading values file:\n\n" + e.Message, $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        internal static bool DownloadModValuesFile(string modValuesFileName)
        {
            try
            {
                Download(ObeliskUrl + modValuesFileName,
                    FileService.GetJsonPath(Path.Combine(FileService.ValuesFolder, modValuesFileName)));
                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        #endregion
    }
}

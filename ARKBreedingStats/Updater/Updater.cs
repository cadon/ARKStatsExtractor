using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.utils;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.Updater
{
    public static class Updater
    {
        /// <summary>
        /// Latest release url.
        /// </summary>
        public const string ReleasesUrl = RepositoryInfo.RepositoryUrl + "releases/latest";
        private const string MasterRawUrl = RepositoryInfo.RepositoryUrl + "raw/master/";
        private const string ReleasesFeedUrl = "https://api.github.com/repos/cadon/ARKStatsExtractor/releases/latest";
        private const string ManifestUrl = MasterRawUrl + "ARKBreedingStats/_manifest.json";
        internal const string UpdaterExe = "asb-updater.exe";
        private const string ObeliskUrl = "https://raw.githubusercontent.com/arkutils/Obelisk/master/data/asb/";

        #region main program

        private static bool? _isProgramInstalled;

        /// <summary>
        /// Determines if running .exe is installed or at least running in Program Files folder
        /// </summary>
        public static bool IsProgramInstalled
        {
            get
            {
                if (_isProgramInstalled == null)
                {
                    _isProgramInstalled = IsInstalled();
                }
                return _isProgramInstalled.Value;
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
            var args = new System.Text.StringBuilder("\"" + AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + "\"");
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
            string latestVersionString = await FetchReleaseFeed();
            (bool? updateAvailable, string localVersion, string remoteVersion) = UpdateAvailable(latestVersionString);

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
        /// <param name="latestVersionString"></param>
        /// <returns>comparison result and parsed release version, or in case of an error: null and unreadable version numbers</returns>
        private static (bool? updateAvailable, string localVersion, string remoteVersion) UpdateAvailable(string latestVersionString)
        {
            if (Version.TryParse(latestVersionString, out Version tagVersion)
                && Version.TryParse(Application.ProductVersion, out Version productVersion))
            {
                return (tagVersion.CompareTo(productVersion) > 0, productVersion.ToString(), tagVersion.ToString());
            }

            return (null, Application.ProductVersion, latestVersionString);
        }

        /// <summary>
        /// Downloads the application manifest file with the latest available versions.
        /// </summary>
        /// <returns></returns>
        internal static async Task<bool> DownloadManifest()
        {
            return await DownloadManifestFile(ManifestUrl, FileService.GetPath(FileService.ManifestFileName));
        }

        /// <summary>
        /// Downloads the current manifest file with the latest version info. If that fails, it will use the github api and parse the version from there.
        /// </summary>
        /// <returns>The latest version in string format.</returns>
        private static async Task<string> FetchReleaseFeed()
        {
            // download the manifest file
            try
            {
                var manifestFilePath = FileService.GetPath(FileService.ManifestFileName);
                if (await DownloadManifestFile(ManifestUrl, manifestFilePath, false))
                {
                    try
                    {
                        return ParseVersionFromManifest(manifestFilePath);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Parsing release from manifest failed: " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Fetching manifest info failed: " + e.Message);
            }

            // manifest download failed, try the github api
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
        /// Parses the manifest for the latest version info
        /// </summary>
        /// <returns>Latest release version</returns>
        private static string ParseVersionFromManifest(string manifestFilePath)
        {
            var manifest = AsbManifest.FromJsonFile(manifestFilePath);

            if (manifest.modules.TryGetValue("ARK Smart Breeding", out var app))
                return app.version;

            throw new FormatException("version of main app not found in manifest");
        }

        /// <summary>
        /// Parses the releases feed for relevant information
        /// </summary>
        /// <param name="releaseFeed"></param>
        /// <returns>Tuple containing tag of the release version and list of urls</returns>
        private static string ParseReleaseInfo(string releaseFeed)
        {
            var latestRelease = JObject.Parse(releaseFeed);

            string tag = latestRelease.Value<string>("tag_name");

            Debug.WriteLine("Tag: " + tag);

            return tag.Trim('v');
        }

        #endregion

        /// <summary>
        /// Downloads a file from the given URL, returns as string or writes it to the given destination
        /// </summary>
        /// <param name="url">The URL to download from</param>
        /// <param name="outFileName">File to output contents to</param>
        /// <returns>content or null</returns>
        internal static async Task<(bool, string)> DownloadAsync(string url, string outFileName = null, bool showExceptionMessageBox = true)
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
                catch (Exception ex)
                {
                    successfulDownloaded = false;
                    if (showExceptionMessageBox)
                        MessageBoxes.ExceptionMessageBox(ex, $"Error while trying to download the file\n{url}", "Download error");
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
                catch (Exception ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex, $"Error while trying to download the file\n{url}", "Download error");
                }

                if (!File.Exists(outFileName))
                    throw new FileNotFoundException($"Downloading file from {url} failed", outFileName);

                return true;
            }
        }

        #region mod values

        internal static async Task<bool> DownloadModsManifest()
        {
            return await DownloadManifestFile(ObeliskUrl + FileService.ManifestFileName,
                Path.Combine(FileService.GetJsonPath(FileService.ValuesFolder), FileService.ManifestFileName));
        }

        private static async Task<bool> DownloadManifestFile(string url, string destinationPath, bool showDownloadExceptionMessageBox = true)
        {
            string tempFilePath = Path.GetTempFileName();
            string valuesFolder = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(valuesFolder))
                Directory.CreateDirectory(valuesFolder);

            try
            {
                if ((await DownloadAsync(url, tempFilePath, showDownloadExceptionMessageBox)).Item1)
                {
                    // if successful downloaded, move tempFile
                    try
                    {
                        if (File.Exists(destinationPath)) File.Delete(destinationPath);
                        File.Move(tempFilePath, destinationPath);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBoxes.ExceptionMessageBox(ex, "Error while moving manifest file");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex, "Error while downloading manifest");
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
        private static bool TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //internal static async Task<bool> DownloadModValuesFileAsync(string modValuesFileName)
        //{
        //    try
        //    {
        //        await DownloadAsync(ObeliskUrl + modValuesFileName,
        //            FileService.GetJsonPath(Path.Combine(FileService.ValuesFolder, modValuesFileName)));
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBoxes.ExceptionMessageBox(ex, "Error while downloading values file");
        //    }
        //    return false;
        //}

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

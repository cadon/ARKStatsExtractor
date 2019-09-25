using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public class Updater
    {
        public const string MasterBranchUrl = "https://github.com/cadon/ARKStatsExtractor/raw/master/ARKBreedingStats/";

        public const string ReleasesUrl = "https://github.com/cadon/ARKStatsExtractor/releases/latest";
        // Release feed URL
        private const string releasesFeedUrl = "https://api.github.com/repos/cadon/ARKStatsExtractor/releases";
        private const string UPDATER_EXE = "asb-updater.exe";

        private const string OBELISK_URI = "https://raw.githubusercontent.com/arkutils/Obelisk/master/data/asb/";

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
                    isProgramInstalled = isInstalled();
                }
                return isProgramInstalled.Value;
            }
        }

        private static bool isInstalled()
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

        public static async Task<bool> CheckForInstallerUpdate(bool silentCheck, bool collectionDirty)
        {
            try
            {
                (bool wantsProgramUpdate, IList<string> urls) = await checkAndAskForUpdate(collectionDirty);
                if (wantsProgramUpdate)
                {
                    // Launch the installer and exit the app
                    string installerUrl = urls.FirstOrDefault(url => url.EndsWith(".exe"));
                    if (installerUrl != null)
                    {
                        // download installer to temp dir
                        string installerFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(installerUrl));
                        await DownloadAsync(installerUrl, installerFile);

                        ProcessStartInfo startInfo = new ProcessStartInfo(installerFile)
                        {
                            Verb = "runas"
                        };
                        Process.Start(startInfo);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (silentCheck)
                    return false;
                if (MessageBox.Show("Error while checking for new version.\n\n" +
                        $"{ex.Message}\n\n" +
                        "Try checking for an updated version of ARK Smart Breeding. " +
                        "Do you want to visit the releases page?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    Process.Start(ReleasesUrl);
                return false;
            }
            return false;
        }

        public static async Task<bool> CheckForPortableUpdate(bool silentCheck, bool collectionDirty)
        {
            try
            {
                (bool wantsProgramUpdate, IList<string> urls) = await checkAndAskForUpdate(collectionDirty);
                if (wantsProgramUpdate)
                {
                    // Launch the installer and exit the app
                    string zipPackageUrl = urls.FirstOrDefault(url => url.EndsWith(".zip"));
                    if (zipPackageUrl != null)
                    {
                        await LaunchUpdater();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (silentCheck)
                    return false;
                if (MessageBox.Show("Error while checking for new version.\n\n" +
                        $"{ex.Message}\n\n" +
                        "Try checking for an updated version of ARK Smart Breeding. " +
                        "Do you want to visit the releases page?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    Process.Start(ReleasesUrl);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Copies the updater to the temp directory and launches it from there
        /// </summary>
        private static async Task LaunchUpdater()
        {
            string oldLocation = Path.Combine(Directory.GetCurrentDirectory(), UPDATER_EXE);
            string newLocation = Path.Combine(Path.GetTempPath(), UPDATER_EXE);

            // Copy file to temp using async methods
            using (FileStream dst = File.Open(newLocation, FileMode.Create))
            using (FileStream src = File.Open(oldLocation, FileMode.Open))
            {
                await src.CopyToAsync(dst);
            }
            string filePath = "\"" + Process.GetCurrentProcess().MainModule.FileName + "\"";
            Process.Start(newLocation, filePath);
        }

        /// <summary>
        /// If new release exists ask to install it
        /// </summary>
        /// <param name="collectionDirty"></param>
        /// <returns>true if new release should be installed; download urls or null</returns>
        private static async Task<(bool, IList<string> urls)> checkAndAskForUpdate(bool collectionDirty)
        {
            (string releaseTag, IList<string> urls) = await FetchReleaseFeed();
            (bool? updateAvailable, string localVersion, string remoteVersion) = Updater.updateAvailable(releaseTag);

            if (updateAvailable == null)
            {
                throw new Exception($"Reading version numbers failed. Local: {localVersion}, Remote: {remoteVersion}");
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
                        MessageBox.Show("Your Creature Collection has been modified since it was last saved. " +
                                "Please either save or discard your changes to proceed with the update.",
                                "Unsaved Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return (false, null);
                    }

                    return (true, urls);
                }
            }
            return (false, null);
        }

        #endregion

        #region release check

        /// <summary>
        /// Compares release version with local file version.
        /// </summary>
        /// <param name="releaseTag"></param>
        /// <returns>comparison result and parsed release version, or in case of an error: null and unreadable version numbers</returns>
        private static (bool? updateAvailable, string localVersion, string remoteVersion) updateAvailable(string releaseTag)
        {
            string releaseVersion = releaseTag.ToLowerInvariant().Trim().Trim('v', '.');

            Version.TryParse(releaseVersion, out Version tagVersion);
            Version.TryParse(Application.ProductVersion, out Version productVersion);

            if (tagVersion == null || productVersion == null)
                return (null, Application.ProductVersion, releaseVersion);

            return (tagVersion.CompareTo(productVersion) > 0, productVersion.ToString(), tagVersion.ToString());
        }

        public static async Task<(string tag, IList<string> urls)> FetchReleaseFeed()
        {
            string releaseFeed;
            try
            {
                (_, releaseFeed) = await DownloadAsync(releasesFeedUrl);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Fetching release info failed: " + e.Message);
                throw;
            }

            try
            {
                return parseReleaseInfo(releaseFeed);
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
        private static (string tag, IList<string> urls) parseReleaseInfo(string releaseFeed)
        {
            JArray releaseInfoNode = JArray.Parse(releaseFeed);
            JObject latest = (JObject)releaseInfoNode[0];

            string tag = latest.Value<string>("tag_name");

            JArray assets = latest.Value<JArray>("assets");
            IList<string> urls = assets.Select(token => token.Value<string>("browser_download_url")).ToList();

            Debug.WriteLine("Tag: " + tag);
            Debug.WriteLine("Download URLs: " + string.Join(" ", urls));

            return (tag, urls);
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
                    MessageBox.Show("Error while trying to download the file\n" + url + "\n\n" + e.Message, "ASB download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Error while trying to download the file\n" + url + "\n\n" + e.Message, "ASB download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!File.Exists(outFileName))
                    throw new FileNotFoundException($"Downloading file from {url} failed", outFileName);

                return true;
            }
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
                if ((await DownloadAsync(OBELISK_URI + FileService.ModsManifest,
                    tempFilePath)).Item1)
                {
                    // if successful downloaded, move tempFile
                    try
                    {
                        if (File.Exists(destFilePath)) File.Delete(destFilePath);
                        File.Move(tempFilePath, destFilePath);
                        TryDeleteFile(tempFilePath);
                        return true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error while moving mod-manifest file:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while downloading mod-manifest:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            TryDeleteFile(tempFilePath);

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
                await DownloadAsync(OBELISK_URI + modValuesFileName,
                    FileService.GetJsonPath(Path.Combine(FileService.ValuesFolder, modValuesFileName)));
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while downloading values file:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        internal static bool DownloadModValuesFile(string modValuesFileName)
        {
            try
            {
                Download(OBELISK_URI + modValuesFileName,
                    FileService.GetJsonPath(Path.Combine(FileService.ValuesFolder, modValuesFileName)));
                return true;
            }
            catch
            {
                // TODO
            }
            return false;
        }

        #endregion
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace ASB_Updater
{
    public class ASBUpdater
    {

        // Update stages to go through (determines progress bar display %)
        public enum Stages
        {
            FETCH,
            PARSE,
            CHECK,
            DOWNLOAD,
            EXTRACT,
            CLEANUP,
            COMPLETE
        };

        // Messages/Errors order matches Stages order and quantity
        private readonly string[] stageMessages = {
            "Fetching release list…",
            "Checking for updates…",
            "Checking the versions…",
            "Downloading updates…",
            "Extracting files…",
            "Cleaning up…",
            "Update Done!"
        };
        private readonly string[] stageErrors = {
            "Download of release list failed",
            "Failed to read realease list",
            "Failed checking the version number",
            "Download of updates failed",
            "File extraction failed",
            "Could not complete cleanup",
            "ASB is already up to date!"
        };

        // Release feed URL
        private readonly string releasesURL = "https://api.github.com/repos/cadon/ARKStatsExtractor/releases";
        // Temporary download file name
        private const string tempZipName = "ASB_Update.temp.zip";
        private readonly string tempZipNamePath;
        // Temporary release feed file name
        private const string tempReleases = "ASB_Releases.temp.json";
        private readonly string tempReleasesPath;

        /// <summary>
        /// Temporary path for the update related files.
        /// </summary>
        private readonly string tempFolder;

        private string downloadURL { get; set; }
        private string latestVersion { get; set; }
        private string date { get; set; }

        public Stages Stage { get; internal set; }

        public ASBUpdater()
        {
            tempFolder = GetTemporaryDirectory();
            tempZipNamePath = Path.Combine(tempFolder, tempZipName);
            tempReleasesPath = Path.Combine(tempFolder, tempReleases);
        }

        /// <summary>
        /// Calculates the progress made in updating
        /// </summary>
        /// <returns></returns>
        public int GetProgress()
        {
            int max = Enum.GetNames(typeof(Stages)).Length;
            int current = (int)Stage;

            return (current * 100 / max);
        }

        /// <summary>
        /// Calculates the progress made in updating
        /// </summary>
        /// <returns></returns>
        public string GetProgressState()
        {
            return stageMessages[(int)Stage];
        }

        private void SetStatus(Stages stage, IProgress<ProgressReporter> progress)
        {
            Stage = stage;
            progress.Report(new ProgressReporter(GetProgress(), GetProgressState()));
        }

        /// <summary>
        /// Gets the last error (if any)
        /// </summary>
        /// 
        /// <returns>Last logged error</returns>
        public string LastError()
        {
            return stageErrors[(int)Stage];
        }

        /// <summary>
        /// Fetches the releases feed from GitHub
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public async Task<bool> Fetch(IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.FETCH, progress);
            return await DownloadFile(releasesURL, tempReleasesPath, progress);
        }

        /// <summary>
        /// Parses the releases feed for relevant information
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public async Task<bool> Parse(IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.PARSE, progress);

            try
            {
                string json = File.ReadAllText(tempReleasesPath);
                dynamic stuff = JArray.Parse(json);
                dynamic latest = stuff[0];
                dynamic assets = latest.assets[0];

                downloadURL = assets.browser_download_url;

                // get latest version number
                var match = System.Text.RegularExpressions.Regex.Match(downloadURL, @"([\d\.]+)\.zip");
                latestVersion = match.Success ? match.Groups[1].Value : string.Empty;
                date = latest.published_at;

                Debug.WriteLine("Download URL: " + downloadURL);
                Debug.WriteLine("Date: " + date);
            }
            catch (Exception e)
            {
                progress.Report(new ProgressReporter($"Error while parsing download uri: {e.Message}"));
                Debug.Write(e.StackTrace.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the parsed info indicates that a newer version is available
        /// </summary>
        /// <returns>Newer version available for download</returns>
        public async Task<bool> Check(string applicationPath, IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.CHECK, progress);
            if (string.IsNullOrEmpty(applicationPath)
                || !Directory.Exists(applicationPath))
                return false;

            try
            {
                string exePath = Path.Combine(applicationPath, "ARK Smart Breeding.exe");
                // if exe does not exist, an update is needed
                if (!File.Exists(exePath)) return true;

                string installedVersion = FileVersionInfo.GetVersionInfo(exePath).FileVersion;

                progress.Report(new ProgressReporter($"installed version: {installedVersion}\navailable version: {latestVersion}"));

                return Version.TryParse(installedVersion, out Version installedVer)
                    && Version.TryParse(latestVersion, out Version latestVer)
                    && installedVer > new Version(0, 0)
                    && installedVer < latestVer;
            }
            catch (Exception e)
            {
                progress.Report(new ProgressReporter("Exception while checking versions. " + e.Message));
            }

            return false;
        }

        /// <summary>
        /// Retrieves the update from GitHub
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public async Task<bool> Download(IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.DOWNLOAD, progress);
            return await DownloadFile(downloadURL, tempZipNamePath, progress);
        }

        /// <summary>
        /// Extracts the temporary update zip file
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public async Task<bool> Extract(string applicationPath, bool useLocalAppDataForDataFiles, IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.EXTRACT, progress);
            string extractedAppTempPath = Path.Combine(tempFolder, "ASB");
            Directory.CreateDirectory(extractedAppTempPath);
            ZipFile.ExtractToDirectory(tempZipNamePath, extractedAppTempPath);
            if (useLocalAppDataForDataFiles)
            {
                CopyEntireDirectory(new DirectoryInfo(extractedAppTempPath), new DirectoryInfo(applicationPath), overwriteFiles: true, ignoreSubFolder: "json");
                CopyEntireDirectory(new DirectoryInfo(Path.Combine(extractedAppTempPath, "json")), new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ARK Smart Breeding", "json")), overwriteFiles: true);
            }
            else
            {
                CopyEntireDirectory(new DirectoryInfo(extractedAppTempPath), new DirectoryInfo(applicationPath), overwriteFiles: true);
            }

            return true;
        }

        /// <summary>
        /// Cleans up temporary files
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public async Task<bool> Cleanup(IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.CLEANUP, progress);
            bool result = true;
            try
            {
                Directory.Delete(tempFolder, recursive: true);
            }
            catch
            {
                result = false;
            }

            if (result)
            {
                SetStatus(Stages.COMPLETE, progress);
            }
            return result;
        }

        /// <summary>
        /// Downloads a file to the given destination from the given URL
        /// </summary>
        /// 
        /// <param name="url">The URL to download from</param>
        /// <param name="outName">File to output contents to</param>
        /// 
        /// <returns>Success or Fail</returns>
        private async Task<bool> DownloadFile(string url, string outName, IProgress<ProgressReporter> progress)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Anything");

                var reporter = new ProgressReporter(GetProgress());

                if (url == null)
                {
                    await Fetch(progress);
                    await Parse(progress);

                    url = downloadURL;
                    if (url == null)
                    {
                        progress.Report(new ProgressReporter("url for downloading is null due to error while parsing."));
                        return false;
                    }
                }

                progress.Report(new ProgressReporter($"downloading {url} to {outName}"));
                Debug.WriteLine("URL: " + url);
                Debug.WriteLine("Out File: " + outName);

                await client.DownloadFileTaskAsync(url, outName);
            }

            return File.Exists(outName);
        }

        public static void CopyEntireDirectory(DirectoryInfo source, DirectoryInfo target, bool overwriteFiles = true, string ignoreSubFolder = null)
        {
            if (!source.Exists) return;
            if (!target.Exists) target.Create();

            Parallel.ForEach(string.IsNullOrEmpty(ignoreSubFolder) ? source.GetDirectories() : source.GetDirectories().Where(d => d.Name != ignoreSubFolder),
                (sourceChildDirectory) =>
                CopyEntireDirectory(sourceChildDirectory, new DirectoryInfo(Path.Combine(target.FullName, sourceChildDirectory.Name)), overwriteFiles));

            Parallel.ForEach(source.GetFiles(), sourceFile =>
            {
                sourceFile.CopyTo(Path.Combine(target.FullName, sourceFile.Name), overwriteFiles);
            });
        }

        public static string GetTemporaryDirectory()
        {
            string tempFolder = Path.GetTempFileName();
            File.Delete(tempFolder);
            Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }
    }
}

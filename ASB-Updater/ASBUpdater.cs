using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ASB_Updater
{
    public class ASBUpdater
    {
        /// <summary>
        /// Update stages to go through (determines progress bar display %)
        /// </summary>
        private enum Stages
        {
            FETCH,
            PARSE,
            CHECK,
            DOWNLOAD,
            EXTRACT,
            CLEANUP,
            COMPLETE
        }

        // Messages/Errors order matches Stages order and quantity
        private readonly string[] _stageMessages = {
            "Fetching release list…",
            "Checking for updates…",
            "Checking the versions…",
            "Downloading updates…",
            "Extracting files…",
            "Cleaning up…",
            "Update Done!"
        };
        private readonly string[] _stageErrors = {
            "Download of release list failed",
            "Failed to read release list",
            "Failed checking the version number",
            "Download of updates failed",
            "File extraction failed",
            "Could not complete cleanup",
            "ASB is already up to date!"
        };

        // Release feed URL
        private const string ReleasesUrl = "https://api.github.com/repos/cadon/ARKStatsExtractor/releases/latest";
        // Temporary download file name
        private const string TempZipName = "ASB_Update.temp.zip";
        private readonly string _tempZipNamePath;
        // Temporary release feed file name
        private const string TempReleases = "ASB_Releases.temp.json";
        private readonly string _tempReleasesPath;

        /// <summary>
        /// Temporary path for the update related files.
        /// </summary>
        private readonly string _tempFolder;

        private string _downloadUrl;
        private string _latestVersion;
        private string _date;

        private Stages Stage { get; set; }

        public ASBUpdater()
        {
            _tempFolder = GetTemporaryDirectory();
            _tempZipNamePath = Path.Combine(_tempFolder, TempZipName);
            _tempReleasesPath = Path.Combine(_tempFolder, TempReleases);
            // set TLS-protocol (github needs at least TLS 1.2) for update-check
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Calculates the progress made in updating
        /// </summary>
        /// <returns></returns>
        private int GetProgress()
        {
            int max = Enum.GetNames(typeof(Stages)).Length;
            int current = (int)Stage;

            return (current * 100 / max);
        }

        /// <summary>
        /// Calculates the progress made in updating
        /// </summary>
        /// <returns></returns>
        private string GetProgressState() => _stageMessages[(int)Stage];

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
            return _stageErrors[(int)Stage];
        }

        /// <summary>
        /// Fetches the releases feed from GitHub
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public async Task<bool> Fetch(IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.FETCH, progress);
            return await DownloadFile(ReleasesUrl, _tempReleasesPath, progress);
        }

        /// <summary>
        /// Parses the releases feed for relevant information
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool Parse(IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.PARSE, progress);

            try
            {
                string json = File.ReadAllText(_tempReleasesPath);
                dynamic latest = JObject.Parse(json);
                dynamic assets = latest.assets[0];

                _downloadUrl = assets.browser_download_url;

                // get latest version number
                var match = System.Text.RegularExpressions.Regex.Match(_downloadUrl, @"([\d\.]+)\.zip");
                _latestVersion = match.Success ? match.Groups[1].Value : string.Empty;
                _date = latest.published_at;
            }
            catch (Exception e)
            {
                progress.Report(new ProgressReporter($"Error while parsing download uri: {e.Message}", ProgressReporter.State.Error));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the parsed info indicates that a newer version is available
        /// </summary>
        /// <returns>Newer version available for download</returns>
        public bool Check(string applicationPath, IProgress<ProgressReporter> progress)
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

                progress.Report(new ProgressReporter(message: $"installed version: {installedVersion}\navailable version: {_latestVersion}, released at {_date}"));

                return Version.TryParse(installedVersion, out Version installedVer)
                    && Version.TryParse(_latestVersion, out Version latestVer)
                    && installedVer > new Version(0, 0)
                    && installedVer < latestVer;
            }
            catch (Exception e)
            {
                progress.Report(new ProgressReporter(message: "Exception while checking versions. " + e.Message, state: ProgressReporter.State.Error));
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
            return await DownloadFile(_downloadUrl, _tempZipNamePath, progress);
        }

        /// <summary>
        /// Extracts the temporary update zip file
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool Extract(string applicationPath, bool useLocalAppDataForDataFiles,
            IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.EXTRACT, progress);
            string extractedAppTempPath = Path.Combine(_tempFolder, "ASB");

            // assume that if the directory already exists, the files were already extracted to there
            if (!Directory.Exists(extractedAppTempPath))
            {
                Directory.CreateDirectory(extractedAppTempPath);
                try
                {
                    ZipFile.ExtractToDirectory(_tempZipNamePath, extractedAppTempPath);
                }
                catch (Exception ex)
                {
                    progress.Report(new ProgressReporter(
                        message: $"Error while extracting the files to {extractedAppTempPath}: {ex.Message}",
                        state: ProgressReporter.State.Error));
                    return false;
                }
                progress.Report(new ProgressReporter(message: $"Extracted files to {extractedAppTempPath}"));
            }


            bool resultCopyFiles;
            if (useLocalAppDataForDataFiles)
            {
                resultCopyFiles = CopyEntireDirectory(new DirectoryInfo(extractedAppTempPath), new DirectoryInfo(applicationPath),
                    overwriteFiles: true, ignoreSubFolder: "json", progress);

                resultCopyFiles = CopyEntireDirectory(new DirectoryInfo(Path.Combine(extractedAppTempPath, "json")),
                    new DirectoryInfo(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "ARK Smart Breeding", "json")), overwriteFiles: true, progress: progress)
                    && resultCopyFiles;
            }
            else
            {
                resultCopyFiles = CopyEntireDirectory(new DirectoryInfo(extractedAppTempPath), new DirectoryInfo(applicationPath),
                    overwriteFiles: true, progress: progress);
            }

            return resultCopyFiles;
        }

        /// <summary>
        /// Cleans up temporary files
        /// </summary>
        /// <returns>Success or Fail</returns>
        public bool Cleanup(IProgress<ProgressReporter> progress)
        {
            SetStatus(Stages.CLEANUP, progress);
            bool result = true;
            try
            {
                Directory.Delete(_tempFolder, recursive: true);
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
        /// <param name="progress"></param>
        /// <returns>Success or Fail</returns>
        private async Task<bool> DownloadFile(string url, string outName, IProgress<ProgressReporter> progress)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Anything");

                if (url == null)
                {
                    if (!await Fetch(progress) || !Parse(progress))
                        return false;

                    url = _downloadUrl;
                    if (url == null)
                    {
                        progress.Report(new ProgressReporter("url for downloading is null due to error while parsing.", ProgressReporter.State.Error));
                        return false;
                    }
                }

                progress.Report(new ProgressReporter(message: $"downloading {url} to {outName}"));

                try
                {
                    await client.DownloadFileTaskAsync(url, outName);
                }
                catch (WebException ex)
                {
                    progress.Report(new ProgressReporter($"There was an error while trying to download the file {url}: {ex.Message}", ProgressReporter.State.Error));
                    return false;
                }
                catch (InvalidOperationException ex)
                {
                    progress.Report(new ProgressReporter($"There was an error while trying to save the file from {url} to {outName}: {ex.Message}", ProgressReporter.State.Error));
                    return false;
                }
            }

            return File.Exists(outName);
        }

        private static bool CopyEntireDirectory(DirectoryInfo source, DirectoryInfo target, bool overwriteFiles = true, string ignoreSubFolder = null, IProgress<ProgressReporter> progress = null)
        {
            if (!source.Exists)
            {
                progress?.Report(new ProgressReporter(
                    message: $"The path {source.FullName} does not exist, cannot copy files from there",
                    state: ProgressReporter.State.Error));
                return false;
            }
            if (!target.Exists) target.Create();

            try
            {
                Parallel.ForEach(
                    string.IsNullOrEmpty(ignoreSubFolder)
                        ? source.GetDirectories()
                        : source.GetDirectories().Where(d => d.Name != ignoreSubFolder),
                    (sourceChildDirectory) =>
                    {
                        CopyEntireDirectory(sourceChildDirectory,
                            new DirectoryInfo(Path.Combine(target.FullName, sourceChildDirectory.Name)),
                            overwriteFiles, progress: progress);
                    });

                Parallel.ForEach(source.GetFiles(),
                    sourceFile =>
                    {
                        sourceFile.CopyTo(Path.Combine(target.FullName, sourceFile.Name), overwriteFiles);
                    });
            }
            catch (AggregateException exs)
            {
                progress?.Report(new ProgressReporter(
                    message: $"Error while copying files from {source.FullName} to {target.FullName}:\n{string.Join("\n", exs.Flatten().InnerExceptions.Select(ex => ex.Message))}",
                    state: ProgressReporter.State.Error));
                return false;
            }

            return true;
        }

        private static string GetTemporaryDirectory()
        {
            string tempFolder = Path.GetTempFileName();
            File.Delete(tempFolder);
            Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }
    }
}

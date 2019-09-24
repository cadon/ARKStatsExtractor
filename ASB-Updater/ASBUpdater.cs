using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace ASB_Updater
{
    public class ASBUpdater : IUpdater
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
            "Fetching release list...",
            "Checking for updates...",
            "Checking for updates...",
            "Downloading updates...",
            "Extracting files...",
            "Cleaning up...",
            "Done!"
        };
        private readonly string[] stageErrors = {
            "Download of release list failed",
            "Failed to read realease list",
            "Failed to read realease list",
            "Download of updates failed",
            "File extraction failed",
            "Could not complete cleanup",
            "ASB is already up to date!"
        };

        // Release feed URL
        private readonly string releasesURL = "https://api.github.com/repos/cadon/ARKStatsExtractor/releases";
        // Temporary download file name
        private readonly string tempZipName = "ASB_Update.temp.zip";
        // Temporary release feed file name
        private readonly string tempReleases = "ASB_Releases.temp.json";

        private string downloadURL { get; set; }
        private string date { get; set; }

        public Stages Stage { get; internal set; }

        /// <summary>
        /// Calculates the progress made in updating
        /// </summary>
        /// <returns></returns>
        public int GetProgress()
        {
            int max = Enum.GetNames(typeof(Stages)).Length;
            int current = (int)Stage;

            return (current / max) * 100;
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
        public bool Fetch()
        {
            Stage = Stages.FETCH;
            return downloadFile(releasesURL, tempReleases);
        }

        /// <summary>
        /// Parses the releases feed for relevant information
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool Parse()
        {
            Stage = Stages.PARSE;

            try
            {
                string json = File.ReadAllText(tempReleases);
                dynamic stuff = JArray.Parse(json);
                dynamic latest = stuff[0];
                dynamic assets = latest.assets[0];

                downloadURL = assets.browser_download_url;
                date = latest.published_at;

                Debug.WriteLine("Download URL: " + downloadURL);
                Debug.WriteLine("Date: " + date);
            }
            catch (Exception e)
            {
                Debug.Write(e.StackTrace.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves the update from GitHub
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool Download()
        {
            Stage = Stages.DOWNLOAD;
            return downloadFile(downloadURL, tempZipName);
        }

        /// <summary>
        /// Extracts the temporary update zip file
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool Extract(string workingDirectory)
        {
            Stage = Stages.EXTRACT;

            string tmpDir = GetTemporaryDirectory();
            ZipFile.ExtractToDirectory(tempZipName, tmpDir);
            CopyEntireDirectory(new DirectoryInfo(tmpDir), new DirectoryInfo(workingDirectory), overwiteFiles: true);
            Directory.Delete(tmpDir, recursive: true);

            return true;
        }

        /// <summary>
        /// Cleans up temporary files
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool Cleanup()
        {
            Stage = Stages.CLEANUP;
            bool result = true;

            try
            {
                File.Delete(tempReleases);
                File.Delete(tempZipName);
            }
            catch
            {
                result = false;
            }

            if (result)
            {
                Stage = Stages.COMPLETE;
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
        private bool downloadFile(string url, string outName)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Anything");

                if (url == null)
                {
                    Debug.WriteLine("Fetch? " + Fetch());
                    Debug.WriteLine("Parse? " + Parse());

                    url = downloadURL;
                }

                Debug.WriteLine("URL: " + url);
                Debug.WriteLine("Out File: " + outName);

                client.DownloadFile(url, outName);
            }

            return File.Exists(outName);
        }

        public static void CopyEntireDirectory(DirectoryInfo source, DirectoryInfo target, bool overwiteFiles = true)
        {
            if (!source.Exists) return;
            if (!target.Exists) target.Create();

            Parallel.ForEach(source.GetDirectories(), (sourceChildDirectory) =>
                CopyEntireDirectory(sourceChildDirectory, new DirectoryInfo(Path.Combine(target.FullName, sourceChildDirectory.Name))));

            Parallel.ForEach(source.GetFiles(), sourceFile =>
                sourceFile.CopyTo(Path.Combine(target.FullName, sourceFile.Name), overwiteFiles));
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

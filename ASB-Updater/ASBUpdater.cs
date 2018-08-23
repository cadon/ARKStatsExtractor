using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;

namespace ASB_Updater {
    public class ASBUpdater : IUpdater {

        // Update stages to go through (determines progress bar display %)
        public enum Stages {
            FETCH,
            PARSE,
            CHECK,
            DOWNLOAD,
            EXTRACT,
            CLEANUP,
            COMPLETE
        };

        // Messages/Errors order matches Stages order and quantity
        private readonly string[] stageMessages = new string[] {
            "Fetching release list...",
            "Checking for updates...",
            "Checking for updates...",
            "Downloading updates...",
            "Extracting files...",
            "Cleaning up...",
            "Done!"
        };
        private readonly string[] stageErrors = new string[] {
            "Download of release list failed",
            "Failed to read realease list",
            "Failed to read realease list",
            "Download of updates failed",
            "File extraction failed",
            "Could not complete cleanup",
            "ASB is already up to date!"
        };

        // Main program .exe
        private readonly string asb = "ARK Smart Breeding.exe";
        // Github release feed date pattern
        private readonly string datePattern = @"^(\d{2})-(\d{2})-(\d{4}) (\d{2}):(\d{2}):(\d{2})$";
        // Simplified date for easy >= comparison
        private readonly string comparableDatePattern = "yyyyMMddHHmmss";

        // Release feed URL
        private string releasesURL = "https://api.github.com/repos/cadon/ARKStatsExtractor/releases";
        // Temporary download file name
        private string tempZipName = "ASB_Update.temp.zip";
        // Temporary release feed file name
        private string tempReleases = "ASB_Releases.temp.json";

        private string downloadURL { get; set; }
        private string date { get; set; }

        public Stages stage { get; internal set; }

        /// <summary>
        /// Checks if the main exe exists
        /// </summary>
        /// 
        /// <returns>Exists or not</returns>
        public bool hasEXE() {
            return File.Exists(asb);
        }

        /// <summary>
        /// Gets the name of the EXE to launch
        /// </summary>
        /// 
        /// <returns>ASB's exe name</returns>
        public string getEXE() {
            return asb;
        }

        /// <summary>
        /// Calculates the progress made in updating
        /// </summary>
        /// <returns></returns>
        public int getProgress() {
            int max = Enum.GetNames(typeof(Stages)).Length;
            int current = (int) stage;

            return (current / max) * 100;
        }

        /// <summary>
        /// Gets the last error (if any)
        /// </summary>
        /// 
        /// <returns>Last logged error</returns>
        public string lastError() {
            return stageErrors[(int) stage];
        }

        /// <summary>
        /// Fetches the releases feed from GitHub
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool fetch() {
            stage = Stages.FETCH;
            return downloadFile(releasesURL, tempReleases);
        }

        /// <summary>
        /// Parses the releases feed for relevant information
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool parse() {
            stage = Stages.PARSE;

            try {
                string json = File.ReadAllText(tempReleases);
                dynamic stuff = JArray.Parse(json);
                dynamic latest = stuff[0];
                dynamic assets = latest.assets[0];

                downloadURL = assets.browser_download_url;
                date = latest.published_at;

                Debug.WriteLine("Download URL: " + downloadURL);
                Debug.WriteLine("Date: " + date);
            }
            catch (Exception e) {
                Debug.Write(e.StackTrace.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares the last modified date on the main exe file to the last update
        /// </summary>
        /// 
        /// <param name="date">Date of latest update</param>
        /// 
        /// <returns>Whether to download the update.</returns>
        public bool check() {
            stage = Stages.CHECK;

            Match m = Regex.Match(date, datePattern);
            string latest = "";
            while (m.Success) {
                latest += m.Value;
                m = m.NextMatch();
            }

            DateTime lastModified = File.GetLastWriteTime(asb);
            string current = lastModified.ToString(comparableDatePattern);

            if (Int32.TryParse(latest, out int l) && Int32.TryParse(current, out int c)) {
                return c > l;
            }

            return false;
        }

        /// <summary>
        /// Retrieves the update from GitHub
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool download() {
            stage = Stages.DOWNLOAD;
            return downloadFile(downloadURL, tempZipName);
        }

        /// <summary>
        /// Extracts the temporary update zip file
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool extract() {
            stage = Stages.EXTRACT;
            
            ZipFile.ExtractToDirectory(tempZipName, Directory.GetCurrentDirectory());

            return true;
        }

        /// <summary>
        /// Cleans up temporary files
        /// </summary>
        /// 
        /// <returns>Success or Fail</returns>
        public bool cleanup() {
            stage = Stages.CLEANUP;
            bool result = true;

            try {
                File.Delete(tempReleases);
                File.Delete(tempZipName);
            }
            catch {
                result = false;
            }

            if (result) {
                stage = Stages.COMPLETE;
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
        private bool downloadFile(string url, string outName) {
            using (var client = new WebClient()) {
                client.Headers.Add("User-Agent", "Anything");

                if (url == null) {
                    Debug.WriteLine("Fetch? " + fetch());
                    Debug.WriteLine("Parse? " + parse());

                    url = downloadURL;
                } 

                Debug.WriteLine("URL: " + url);
                Debug.WriteLine("Out File: " + outName);

                client.DownloadFile(url, outName);
            }

            return File.Exists(outName);
        }
    }
}

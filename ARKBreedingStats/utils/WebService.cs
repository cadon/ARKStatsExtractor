using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Methods for web services, e.g. downloading.
    /// </summary>
    internal static class WebService
    {
        private static HttpClient _httpClient;

        /// <summary>
        /// Returns a singleton HttpClient. Only use this method to get an HttpClient.
        /// </summary>
        public static HttpClient GetHttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                    _httpClient.Timeout = TimeSpan.FromSeconds(10);
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "ASB");
                }
                return _httpClient;
            }
        }

        /// <summary>
        /// Downloads a file from the given URL, returns as string or writes it to the given destination
        /// </summary>
        /// <param name="url">The URL to download from</param>
        /// <param name="outFilePath">File path to save contents to</param>
        /// <returns>(download successful, content or null)</returns>
        internal static async Task<(bool, string)> DownloadAsync(string url, string outFilePath = null, bool showExceptionMessageBox = true)
        {
            Debug.WriteLine("Downloading from URL: " + url);
            if (!string.IsNullOrEmpty(outFilePath))
                Debug.WriteLine("Saving to: " + outFilePath);

            var httpClient = GetHttpClient;

            if (string.IsNullOrEmpty(outFilePath))
            {
                // download and return string
                using (var result = await httpClient.GetAsync(url).ConfigureAwait(false))
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        MessageBoxes.ShowMessageBox(
                            $"Error when trying to load data from{Environment.NewLine}{url}{Environment.NewLine}StatusCode {(int)result.StatusCode}: {result.ReasonPhrase}");
                        return (false, null);
                    }

                    return (true, await result.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }

            var successfulDownloaded = true;
            try
            {
                var directory = Path.GetDirectoryName(outFilePath);
                if (directory == null)
                {
                    if (showExceptionMessageBox)
                        MessageBoxes.ShowMessageBox($"Error while trying to download\n{url}\n\nInvalid local save path:\n{outFilePath}", "Error while trying to download");
                    return (false, null);
                }
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                // save file

                using (var downloadStream = await httpClient.GetStreamAsync(url).ConfigureAwait(false))
                using (var fileStream = new FileStream(outFilePath, FileMode.Create, FileAccess.Write))
                {
                    await downloadStream.CopyToAsync(fileStream).ConfigureAwait(false);
                    await fileStream.FlushAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                successfulDownloaded = false;
                if (showExceptionMessageBox)
                    MessageBoxes.ExceptionMessageBox(ex, $"Error while trying to download the file\n{url}", "Download error");
            }

            if (!File.Exists(outFilePath))
                throw new FileNotFoundException($"Downloading file from {url} failed", outFilePath);

            return (successfulDownloaded, null);
        }
    }
}

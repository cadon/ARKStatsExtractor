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
                    _httpClient.Timeout = TimeSpan.FromSeconds(30);
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
                using (var result = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
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

            // download and save file
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
                //url = "https://mock.httpstatus.io/401"; // for debugging

                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                           .ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (showExceptionMessageBox)
                        {
                            var status = response.StatusCode;
                            MessageBoxes.ShowMessageBox($@"Error while trying to download the file
{url}

HTTP {(int)status} ({status})
Body:
{await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
                        }
                        return (false, null);
                    }

                    using (var downloadStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var fileStream = new FileStream(outFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await downloadStream.CopyToAsync(fileStream).ConfigureAwait(false);
                        await fileStream.FlushAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                if (showExceptionMessageBox)
                    MessageBoxes.ExceptionMessageBox(ex, $@"Error while trying to download the file
{url}
or while trying do write it to
{outFilePath}", "Download error");
                return (false, null);
            }

            if (!File.Exists(outFilePath))
            {
                MessageBoxes.ShowMessageBox($@"Error when downloading file from
{url}
and writing to
{outFilePath}

The downloaded file does not exist after downloading it without error. The specified folder might be write protected.", "Download error");
                return (false, null);
            }

            return (true, null);
        }
    }
}

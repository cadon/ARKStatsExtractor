using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.Library;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.AsbServer
{
    /// <summary>
    /// Connects to a server to receive creature data, e.g. sent by the export gun mod.
    /// </summary>
    internal static class Connection
    {
        private const string ApiUri = "https://export.arkbreeder.com/api/v1/";

        private static CancellationTokenSource _lastCancellationTokenSource;

        public static async void StartListeningAsync(
            IProgress<ProgressReportAsbServer> progressDataSent, string serverToken = null)
        {
            if (string.IsNullOrEmpty(serverToken)) return;

            // stop previous listening if any
            StopListening();

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                _lastCancellationTokenSource = cancellationTokenSource;

                var reconnectTries = 0;

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    //var requestUri = "https://httpstat.us/429"; // for debugging
                    var requestUri = ApiUri + "listen/" + serverToken;

                    try
                    {
                        var client = FileService.GetHttpClient;
                        using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                var statusCode = (int)response.StatusCode;
                                string serverMessage = await response.Content.ReadAsStringAsync();

                                try
                                {
                                    serverMessage = JObject.Parse(serverMessage).SelectToken("error.message").ToString();
                                }
                                catch
                                {
                                    // server message in unknown format, use raw content string
                                    serverMessage = "unknown response content format, expected json token \"error.message\" not found in content: " + serverMessage;
                                }

                                serverMessage = Environment.NewLine + serverMessage;

                                switch (statusCode)
                                {
                                    case 400: // Bad Request
                                        WriteErrorMessage($"Something went wrong with the server connection.{serverMessage}",
                                            response);
                                        return;
                                    case 429: // Too Many Requests
                                        WriteErrorMessage(
                                            $"The server is currently at the rate limit and cannot process the request. Try again later.{serverMessage}",
                                            response);
                                        return;
                                    case 507: // Insufficient Storage
                                        WriteErrorMessage($"Too many connections to the server. Try again later.{serverMessage}",
                                            response);
                                        return;
                                    default:
                                        var errorMessage = statusCode >= 500
                                            ? "Something went wrong with the server or its proxy." + Environment.NewLine
                                            : null;
                                        WriteErrorMessage($"{errorMessage}{serverMessage}", response);
                                        return;
                                }
                            }

                            reconnectTries = 0;

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var reader = new StreamReader(stream))
                            {
                                var report = await ReadServerSentEvents(reader, progressDataSent, serverToken, cancellationTokenSource.Token);
                                if (report != null)
                                {
                                    if (report.StoppedListening
                                       && cancellationTokenSource == _lastCancellationTokenSource)
                                        _lastCancellationTokenSource = null;
                                    progressDataSent.Report(report);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var tryToReconnect = reconnectTries++ < 3;
                        WriteErrorMessage(
                            $"ASB Server listening {ex.GetType()}: {ex.Message}{Environment.NewLine}{(tryToReconnect ? "Trying to reconnect" + Environment.NewLine : string.Empty)}Stack trace: {ex.StackTrace}",
                            stopListening: !tryToReconnect);

                        if (!tryToReconnect)
                            break;
                        // try to reconnect after some time
                        Thread.Sleep(10_000);
                    }
                    finally
                    {
#if DEBUG
                        Console.WriteLine($"{DateTime.Now}: ASB Server listening stopped using token: {serverToken}");
#endif
                    }
                }

                // Displays an error message in the UI, also logs on the console if in debug mode
                void WriteErrorMessage(string message, HttpResponseMessage response = null, bool stopListening = true)
                {
                    if (response != null)
                    {
                        message = $"{(int)response.StatusCode}: {response.ReasonPhrase}{Environment.NewLine}{message}";
                    }
#if DEBUG
                    Console.WriteLine(message);
#endif
                    if (stopListening)
                        cancellationTokenSource.Cancel();
                    progressDataSent.Report(new ProgressReportAsbServer { Message = message, StoppedListening = stopListening, IsError = true });
                }
            }
        }

        private static readonly Regex ServerEventRegex = new Regex(@"^event: (welcome|ping|replaced|export|server|closing)(?: (\-?\d+))?(?:\ndata:\s(.+))?$");

        private static async Task<ProgressReportAsbServer> ReadServerSentEvents(StreamReader reader, IProgress<ProgressReportAsbServer> progressDataSent, string serverToken, CancellationToken cancellationToken)
        {
#if DEBUG
            Console.WriteLine($"{DateTime.Now}: Now listening using token: {serverToken}");
#endif
            progressDataSent.Report(new ProgressReportAsbServer
            {
                Message = $"Now listening for remote exports.{Environment.NewLine}Enter the following token into the ASB export gun mod:",
                ServerToken = serverToken,
                ClipboardText = serverToken
            });

            while (!cancellationToken.IsCancellationRequested)
            {
                var received = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(received))
                    continue; // empty line marks end of event

#if DEBUG
                Console.WriteLine($"{DateTime.Now}: {received} (token: {serverToken})");
#endif
                switch (received)
                {
                    case "event: welcome":
                        continue;
                    case "event: ping":
                        continue;
                    case "event: replaced":
                        if (cancellationToken.IsCancellationRequested) return null;
                        return new ProgressReportAsbServer
                        {
                            Message = "ASB Server listening stopped. Connection used by a different user",
                            StoppedListening = true,
                            IsError = true
                        };
                    case "event: closing":
                        // only report closing if the user hasn't done this already
                        if (cancellationToken.IsCancellationRequested) return null;
                        return new ProgressReportAsbServer
                        {
                            Message = "ASB Server listening stopped. Connection closed by the server, trying to reconnect",
                            IsError = true
                        };
                }

                if (received != "event: export" && !received.StartsWith("event: server"))
                {
                    Console.WriteLine($"{DateTime.Now}: unknown server event: {received}");
                    continue;
                }

                received += "\n" + await reader.ReadLineAsync();
                if (cancellationToken.IsCancellationRequested)
                    break;
                var m = ServerEventRegex.Match(received);
                if (m.Success)
                {
                    switch (m.Groups[1].Value)
                    {
                        case "export":
                            var report = new ProgressReportAsbServer
                            {
                                JsonText = m.Groups[3].Value,
                                TaskNameGenerated = new TaskCompletionSource<string>()
                            };
                            progressDataSent.Report(report);
                            var nameGeneratedTask = report.TaskNameGenerated.Task;
                            string creatureName = null;
                            if (nameGeneratedTask.Wait(TimeSpan.FromSeconds(5))
                                && nameGeneratedTask.Status == TaskStatus.RanToCompletion)
                            {
                                creatureName = nameGeneratedTask.Result;
                                // TODO send creatureName as response
                            }
                            break;
                        case "server":
                            progressDataSent.Report(new ProgressReportAsbServer { JsonText = m.Groups[3].Value, ServerHash = m.Groups[2].Value });
                            break;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Stops the currently running listener. Returns false if no listener was running.
        /// </summary>
        public static bool StopListening()
        {
            if (_lastCancellationTokenSource == null)
                return false; // nothing to stop
            if (_lastCancellationTokenSource.IsCancellationRequested)
            {
                _lastCancellationTokenSource = null;
                return false; // already stopped
            }

            _lastCancellationTokenSource.Cancel();
            _lastCancellationTokenSource = null;
            return true;
        }

        /// <summary>
        /// Sends creature data to the server, this is done for testing, usually other tools like the export gun mod do this.
        /// </summary>
        public static async void SendCreatureData(Creature creature, string token)
        {
            if (creature == null || string.IsNullOrEmpty(token)) return;

            var client = FileService.GetHttpClient;

            var contentString = Newtonsoft.Json.JsonConvert.SerializeObject(ImportExportGun.ConvertCreatureToExportGunFile(creature, out _));
            var msg = new HttpRequestMessage(HttpMethod.Put, ApiUri + "export/" + token);
            msg.Content = new StringContent(contentString, Encoding.UTF8, "application/json");
            msg.Content.Headers.Add("Content-Length", contentString.Length.ToString());
            Console.WriteLine($"{DateTime.Now}: Sending creature data of {creature} using token: {token}"); //\nContent:\n{contentString}");
            using (var response = await client.SendAsync(msg))
            {
                //Console.WriteLine(msg.ToString());
                Console.WriteLine($"{DateTime.Now}: Response: StatusCode {(int)response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// Returns a new string token that can be used as identifier in combination with the export gun mod.
        /// </summary>
        public static string CreateNewToken()
        {
            // only use characters that are easily distinguishable
            var allowedCharacters = Enumerable.Range(0x31, 9) // 1-9
                .Concat(Enumerable.Range(0x61, 8)) // a-h
                .Concat(new[] { 0x6b, 0x6d, 0x6e }) // k, m, n
                .Concat(Enumerable.Range(0x70, 11)) // p-z
                .Select(i => (char)i)
                .ToArray();
            var l = allowedCharacters.Length;

            var guid = Guid.NewGuid().ToByteArray();
            const int tokenLength = 14; // from these each 5th character is a dash for readability
            var token = new char[tokenLength];
            var checkSum = 0;
            var tokenLengthWithoutCheckDigit = tokenLength - 1;
            for (var i = 0; i < tokenLengthWithoutCheckDigit; i++)
            {
                if ((i + 1) % 5 == 0)
                {
                    token[i] = '-';
                    continue;
                }

                var character = allowedCharacters[guid[i] % l];
                token[i] = character;
                checkSum += character;
            }
            // use last character as check digit
            // checkSum % 15, add 1 (to avoid ambiguous 0), display as hex digit (range [1-9a-f])
            token[tokenLength - 1] = ((checkSum % 15) + 1).ToString("x")[0];

            return new string(token);
        }

        /// <summary>
        /// Returns the passed token string, or wildcards if the streamer mode is enabled.
        /// </summary>
        public static string TokenStringForDisplay(string token) => Properties.Settings.Default.StreamerMode ? "****" : token;

        public static bool IsCurrentlyListening => _lastCancellationTokenSource?.IsCancellationRequested == false;

        /// <summary>
        /// If the token is in the clipboard, remove it from there, when the user is not expecting it to be there.
        /// </summary>
        public static void ClearTokenFromClipboard()
        {
            var clipboardText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clipboardText) && clipboardText == Properties.Settings.Default.ExportServerToken)
            {
                Clipboard.SetText(string.Empty);
            }
        }
    }
}

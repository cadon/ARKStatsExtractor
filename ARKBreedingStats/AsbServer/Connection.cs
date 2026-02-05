using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.AsbServer
{
    /// <summary>
    /// Connects to a server to receive creature data, e.g. sent by the export gun mod.
    /// </summary>
    internal static class Connection
    {
        // api doc: https://github.com/coldino/ASB-export-server/blob/main/docs/API.md
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
                        var client = WebService.GetHttpClient;
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
                        var tryToReconnect = reconnectTries++ < 4;
                        if (tryToReconnect)
                            WriteErrorMessage(
                                $"ASB Server listening error, attempting to reconnect (try {reconnectTries})",
                                stopListening: false);
                        else
                            WriteErrorMessage(
                                $"ASB Server listening error: {ex.GetType()}: {ex.Message}{Environment.NewLine}Stack trace: {ex.StackTrace}",
                                stopListening: true);

                        if (!tryToReconnect)
                            break;
                        // try to reconnect after with increasing delays (10, 20, 40, 80 s)
                        Thread.Sleep(5_000 * (1 << reconnectTries));
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

        private static readonly Regex RgServerHash = new Regex(@"^event: server (\-?\d+)$");
        private static readonly Regex RgEventStatus = new Regex(@"^event: (neuter|dead) (\-?\d+) (\-?\d+)$");
        private static readonly Regex RgEventData = new Regex(@"^(data|id):\s*(.+)$");

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
                    case "event: export":
                        var report = new ProgressReportAsbServer { ServerToken = serverToken };
                        while (true)
                        {
                            var data = await ReadEventData(reader, cancellationToken, report);
                            if (data.cancelled) return null;
                            if (!data.endOfEvent) continue;

                            report.TaskNameGenerated = new TaskCompletionSource<ServerSendName>();
                            progressDataSent.Report(report);
                            var nameGeneratedTask = report.TaskNameGenerated.Task;
                            if (nameGeneratedTask.Wait(TimeSpan.FromSeconds(5))
                                && nameGeneratedTask.Status == TaskStatus.RanToCompletion)
                            {
                                var serverSendName = nameGeneratedTask.Result;
                                if (!string.IsNullOrEmpty(serverSendName.CreatureName)
                                    && !string.IsNullOrEmpty(serverSendName.ConnectionToken)
                                    && !string.IsNullOrEmpty(serverSendName.ExportId)
                                    )
                                {
                                    // send creatureName as response
#if DEBUG
                                    Console.WriteLine($"{DateTime.Now}: sending creature name {serverSendName.CreatureName} to: {ApiUri}respond/{serverSendName.ConnectionToken}/{serverSendName.ExportId}");
#endif
                                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(new { generatedName = serverSendName.CreatureName });
                                    var msg = new HttpRequestMessage(HttpMethod.Post,
                                        $"{ApiUri}respond/{serverSendName.ConnectionToken}/{serverSendName.ExportId}");
                                    msg.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                                    msg.Content.Headers.Add("Content-Length", jsonString.Length.ToString());

                                    var sendResponse = await WebService.GetHttpClient.SendAsync(msg, cancellationToken);
#if DEBUG
                                    Console.WriteLine($"{DateTime.Now}: received send response: {await sendResponse.Content.ReadAsStringAsync()})");
#endif
                                }
                            }
                            break;
                        }
                        break;
                    default:
                        // supported options are: server <hash>, neuter <id1> <id2>, dead <id1> <id2>
                        var matchSetStatus = RgEventStatus.Match(received);
                        if (matchSetStatus.Success)
                        {
                            var creatureStatus = CreatureFlags.None;
                            switch (matchSetStatus.Groups[1].Value)
                            {
                                case ServerCreatureStatusNeuter:
                                    creatureStatus = CreatureFlags.Neutered;
                                    break;
                                case ServerCreatureStatusDead:
                                    creatureStatus = CreatureFlags.Dead;
                                    break;
                            }

                            if (creatureStatus == CreatureFlags.None)
                            {
                                Console.WriteLine($"{DateTime.Now}: unknown server event: {received}");
                                break;
                            }
                            progressDataSent.Report(new ProgressReportAsbServer
                            {
                                creatureId = Utils.ConvertArkIdsToLongArkId(
                                    int.Parse(matchSetStatus.Groups[2].Value),
                                    int.Parse(matchSetStatus.Groups[3].Value)),
                                SetFlag = creatureStatus
                            });
                            break;
                        }

                        var matchServerHash = RgServerHash.Match(received);
                        if (matchServerHash.Success)
                        {
                            report = new ProgressReportAsbServer { ServerHash = matchServerHash.Groups[2].Value };

                            while (true)
                            {
                                var data = await ReadEventData(reader, cancellationToken, report);
                                if (data.cancelled) return null;
                                if (!data.endOfEvent) continue;

                                progressDataSent.Report(report);
                                break;
                            }

                            break;
                        }
                        Console.WriteLine($"{DateTime.Now}: unknown server event: {received}");
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads a server event line.
        /// </summary>
        private static async Task<(bool cancelled, bool endOfEvent)> ReadEventData(StreamReader reader, CancellationToken cancellationToken, ProgressReportAsbServer report)
        {
            var data = await reader.ReadLineAsync();
            if (cancellationToken.IsCancellationRequested)
                return (true, false);

            if (string.IsNullOrEmpty(data))
                return (false, true);

            var match = RgEventData.Match(data);
            if (match.Success)
            {
                switch (match.Groups[1].Value)
                {
                    case "data":
                        report.JsonText = match.Groups[2].Value;
                        break;
                    case "id":
                        report.SendId = match.Groups[2].Value;
                        break;
                }

                return (false, false);
            }
            Console.WriteLine($"{DateTime.Now}: unknown event data: {data}");
            return (false, false);
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

        #region send tests

        /// <summary>
        /// Sends creature data to the server, this is done for testing, usually other tools like the export gun mod do this.
        /// <param name="waitForResponse">If &gt; 0 a response is awaited for that many seconds. Else no waiting for a response.</param>
        /// </summary>
        public static async Task SendCreatureData(Creature creature, string token, int waitForResponse = 5)
        {
            if (creature == null || string.IsNullOrEmpty(token)) return;

            // don't use the static FileService.GetHttpClient here, it will block the other sending connection
            using (var client = new HttpClient())
            {
                var contentString =
                    Newtonsoft.Json.JsonConvert.SerializeObject(
                        ImportExportGun.ConvertCreatureToExportGunFile(creature, out _));
                var msg = new HttpRequestMessage(HttpMethod.Put,
                    ApiUri + "export/" + token + (waitForResponse > 0 ? $"?wait={waitForResponse}" : ""));
                msg.Content = new StringContent(contentString, Encoding.UTF8, "application/json");
                msg.Content.Headers.Add("Content-Length", contentString.Length.ToString());
                Console.WriteLine(
                    $"{DateTime.Now}: [simulating Export gun] Sending creature data of {creature} to {msg.RequestUri}");
                using (var response = await client.SendAsync(msg))
                {
                    //Console.WriteLine(msg.ToString());
                    Console.WriteLine(
                        $"{DateTime.Now}: [simulating Export gun] Response of sending creature: StatusCode {(int)response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}, Content: ");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public const string ServerCreatureStatusNeuter = "neuter";
        public const string ServerCreatureStatusDead = "dead";

        /// <summary>
        /// Sends creature data to the server, this is done for testing, usually other tools like the export gun mod do this.
        /// <param name="creatureId">If of the creature which status should be updated</param>
        /// <param name="status">Status</param>
        /// </summary>
        public static async void SendCreatureStatus(long creatureId, string token, string status)
        {
            if (string.IsNullOrEmpty(token) || (
                    status != ServerCreatureStatusNeuter
                    && status != ServerCreatureStatusDead
                    )
                ) return;

            var client = WebService.GetHttpClient;

            var id1 = (int)(creatureId >> 32);
            var id2 = (int)creatureId;
            var msg = new HttpRequestMessage(HttpMethod.Put,
                ApiUri + status + "/" + token + "/" + id1 + "/" + id2);
            Console.WriteLine(
                $"{DateTime.Now}: [simulating Export gun] Sending creature status for creature id {creatureId} to {msg.RequestUri}");
            using (var response = await client.SendAsync(msg))
            {
                Console.WriteLine(
                    $"{DateTime.Now}: [simulating Export gun] Response of sending creature: StatusCode {(int)response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}, Content: ");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        #endregion

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
                utils.ClipboardHandler.SetText(string.Empty);
            }
        }
    }
}

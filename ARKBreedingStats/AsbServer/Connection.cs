using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            IProgress<ProgressReportAsbServer> progressDataSent, string token = null)
        {
            if (string.IsNullOrEmpty(token)) return;

            // stop previous listening if any
            StopListening();

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                _lastCancellationTokenSource = cancellationTokenSource;

                var reconnectTries = 0;

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var requestUri = ApiUri + "listen/" + token; // "https://httpstat.us/429";

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
                                var report = await ReadServerSentEvents(reader, progressDataSent, token, cancellationTokenSource.Token);
                                if (report != null)
                                {
                                    progressDataSent.Report(report);
                                    if (report.StopListening)
                                    {
                                        StopListening();
                                    }
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
                        Console.WriteLine($"ASB Server listening stopped using token: {token}");
#endif
                    }
                }
            }

            _lastCancellationTokenSource = null;

            return;

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
                progressDataSent.Report(new ProgressReportAsbServer { Message = message, StopListening = stopListening, IsError = true });
            }
        }

        private static Regex _eventRegex = new Regex(@"^event: (welcome|ping|replaced|export|server|closing)(?: (\-?\d+))?(?:\ndata:\s(.+))?$");

        private static async Task<ProgressReportAsbServer> ReadServerSentEvents(StreamReader reader, IProgress<ProgressReportAsbServer> progressDataSent, string token, CancellationToken cancellationToken)
        {
#if DEBUG
            Console.WriteLine($"Now listening using token: {token}");
#endif
            progressDataSent.Report(new ProgressReportAsbServer { Message = $"Now listening to the export server using the token (also copied to clipboard){Environment.NewLine}{token}", ClipboardText = token });

            while (!cancellationToken.IsCancellationRequested)
            {
                var received = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(received))
                    continue; // empty line marks end of event

#if DEBUG
                Console.WriteLine($"{received} (token: {token})");
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
                            StopListening = true,
                            IsError = true
                        };
                    case "event: closing":
                        // only report closing if the user hasn't done this already
                        if (cancellationToken.IsCancellationRequested) return null;
                        return new ProgressReportAsbServer
                        { Message = "ASB Server listening stopped. Connection closed by the server, trying to reconnect", StopListening = false, IsError = true };
                }

                if (received != "event: export" && !received.StartsWith("event: server"))
                {
                    Console.WriteLine($"unknown server event: {received}");
                    continue;
                }

                received += "\n" + await reader.ReadLineAsync();
                if (cancellationToken.IsCancellationRequested)
                    break;
                var m = _eventRegex.Match(received);
                if (m.Success)
                {
                    switch (m.Groups[1].Value)
                    {
                        case "export":
                            progressDataSent.Report(new ProgressReportAsbServer { JsonText = m.Groups[3].Value });
                            break;
                        case "server":
                            progressDataSent.Report(new ProgressReportAsbServer { JsonText = m.Groups[3].Value, ServerHash = m.Groups[2].Value });
                            break;
                    }
                }
            }

            return null;
        }

        public static void StopListening()
        {
            if (_lastCancellationTokenSource == null)
                return; // nothing to stop

            _lastCancellationTokenSource.Cancel();
            _lastCancellationTokenSource = null;
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
            using (var response = await client.SendAsync(msg))
            {
                Console.WriteLine($"Sent creature data of {creature} using token: {token}\nContent:\n{contentString}");
                Console.WriteLine(msg.ToString());
                Console.WriteLine($"Response: StatusCode {(int)response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");
            }
        }

        public static string CreateNewToken()
        {
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
            for (var i = 0; i < tokenLength; i++)
            {
                if ((i + 1) % 5 == 0)
                {
                    token[i] = '-';
                    continue;
                }
                token[i] = allowedCharacters[guid[i] % l];
            }

            return new string(token);
        }

        /// <summary>
        /// Simple replacement of CancellationTokenSource to avoid unnecessary complexities with disposal of CTS when the token is still in use.
        /// </summary>
        private class SimpleCancellationToken
        {
            public bool IsCancellationRequested;
            public void Cancel() => IsCancellationRequested = true;
        }
    }
}

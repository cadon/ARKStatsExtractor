using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using ARKBreedingStats.importExportGun;
using ARKBreedingStats.Library;

namespace ARKBreedingStats.AsbServer
{
    /// <summary>
    /// Connects to a server to receive creature data, e.g. sent by the export gun mod.
    /// </summary>
    internal static class Connection
    {
        private const string ApiUri = "https://export.arkbreeder.com/api/v1/";

        private static SimpleCancellationToken _lastCancellationToken;

        public static async void StartListeningAsync(IProgress<(string jsonText, string serverHash, string message)> progressDataSent, string token = null)
        {
            if (string.IsNullOrEmpty(token)) return;

            // stop previous listening if any
            StopListening();
            var cancellationToken = new SimpleCancellationToken();
            _lastCancellationToken = cancellationToken;

            try
            {
                using (var client = new HttpClient())
                using (var stream = await client.GetStreamAsync(ApiUri + "listen/" + token))
                using (var reader = new StreamReader(stream))
                {
#if DEBUG
                    Console.WriteLine($"Now listening using token: {token}");
#endif
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var received = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(received))
                            continue; // end of event

#if DEBUG
                        Console.WriteLine($"{received} (token: {token})");
#endif
                        switch (received)
                        {
                            case "event: welcome":
                                continue;
                                break;
                            case "event: ping":
                                continue;
                                break;
                            case "event: replaced":
                                progressDataSent.Report((null, null, "ASB Server listening stopped. Connection used by a different user"));
                                return;
                            case "event: closing":
                                progressDataSent.Report((null, null, "ASB Server listening stopped. Connection closed by the server"));
                                return;
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
                                    progressDataSent.Report((m.Groups[3].Value, null, null));
                                    break;
                                case "server":
                                    progressDataSent.Report((m.Groups[3].Value, m.Groups[2].Value, null));
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"ASB Server listening exception:\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}");
#endif
            }
#if DEBUG
            Console.WriteLine($"ASB Server listening stopped using token: {token}");
#endif
        }

        private static Regex _eventRegex = new Regex(@"^event: (welcome|ping|replaced|export|server|closing)(?: (\-?\d+))?(?:\ndata:\s(.+))?$");

        public static void StopListening()
        {
            if (_lastCancellationToken == null)
                return; // nothing to stop

            _lastCancellationToken.Cancel();
            _lastCancellationToken = null;
        }

        /// <summary>
        /// Sends creature data to the server, this is done for testing, usually other tools like the export gun mod do this.
        /// </summary>
        public static async void SendCreatureData(Creature creature, string token)
        {
            if (creature == null || string.IsNullOrEmpty(token)) return;

            using (var client = new HttpClient())
            {
                var contentString = Newtonsoft.Json.JsonConvert.SerializeObject(ImportExportGun.ConvertCreatureToExportGunFile(creature, out _));
                var msg = new HttpRequestMessage(HttpMethod.Put, ApiUri + "export/" + token);
                msg.Content = new StringContent(contentString, Encoding.UTF8, "application/json");
                msg.Content.Headers.Add("Content-Length", contentString.Length.ToString());
                var response = await client.SendAsync(msg);
                Console.WriteLine($"Sent creature data of {creature} using token: {token}\nContent:\n{contentString}");
                Console.WriteLine(msg.ToString());
                Console.WriteLine($"Response: Status: {(int)response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");
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

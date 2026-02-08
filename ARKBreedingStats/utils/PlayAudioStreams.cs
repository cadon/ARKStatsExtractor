using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace ARKBreedingStats.utils
{
    internal class PlayAudioStreams
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Stop() => _cancellationTokenSource?.Cancel();

        public PlayAudioStreams(List<Stream> audioStreams)
        {
            if (audioStreams?.Any() != true || _cancellationTokenSource?.Token.IsCancellationRequested == true) return;

            Task.Run(() =>
            {
                var sp = new SoundPlayer();

                var cancelToken = _cancellationTokenSource.Token;

                foreach (var s in audioStreams)
                {
                    sp.Stream = s;
                    sp.PlaySync();
                    if (cancelToken.IsCancellationRequested)
                        break;
                }
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                sp.Dispose();
            });
        }
    }
}

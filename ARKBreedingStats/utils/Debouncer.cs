using System;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Used to fire an event only if it's not called for a specific amount of time.
    /// </summary>
    public class Debouncer
    {
        private Timer _timer;

        /// <summary>
        /// Call to debounce the action with the given interval.
        /// </summary>
        /// <param name="interval">Interval in ms</param>
        /// <param name="action">Action to perform after interval elapsed without another debounce call</param>
        /// <param name="dispatcher">Dispatcher to use, usually the one of the UI thread</param>
        public void Debounce(int interval, Action action, Dispatcher dispatcher)
        {
            _timer?.Stop();
            _timer = new Timer(interval) { AutoReset = false };

            _timer.Elapsed += (s, o) =>
            {
                if (_timer == null)
                    return;

                _timer.Stop();
                _timer = null;

                dispatcher.BeginInvoke(action, null);
            };
            _timer.Start();
        }
    }
}

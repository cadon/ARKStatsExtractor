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
        private Type _argsType;

        /// <summary>
        /// Call to debounce the action with the given interval.
        /// </summary>
        /// <param name="interval">Interval in ms</param>
        /// <param name="action">Action to perform after interval elapsed without another debounce call</param>
        /// <param name="dispatcher">Dispatcher to use, usually the one of the UI thread</param>
        public void Debounce(int interval, Action action, Dispatcher dispatcher)
        {
            _timer?.Stop();
            if (interval <= 0)
            {
                _timer = null;
                dispatcher.BeginInvoke(action, null);
                return;
            }

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

        /// <summary>
        /// Call to debounce the action with the given interval.
        /// </summary>
        /// <param name="interval">Interval in ms</param>
        /// <param name="action">Action to perform after interval elapsed without another debounce call</param>
        /// <param name="dispatcher">Dispatcher to use, usually the one of the UI thread</param>
        /// <param name="args">Arguments for the action</param>
        public void Debounce<T>(int interval, Action<T> action, Dispatcher dispatcher, T args)
        {
            _timer?.Stop();
            if (interval <= 0)
            {
                _timer = null;
                dispatcher.BeginInvoke(action, args);
                return;
            }

            _timer = new Timer(interval) { AutoReset = false };
            _timer.Elapsed += (s, o) =>
            {
                if (_timer == null)
                    return;

                _timer.Stop();
                _timer = null;

                dispatcher.BeginInvoke(action, args);
            };
            _timer.Start();
        }

        /// <summary>
        /// Cancels the debouncing, the action won't be executed.
        /// </summary>
        public void Cancel()
        {
            _timer?.Stop();
        }
    }
}

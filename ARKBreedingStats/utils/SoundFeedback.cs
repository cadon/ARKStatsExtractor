using System.Media;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Plays sounds to indicate the result of tasks.
    /// </summary>
    internal static class SoundFeedback
    {
        internal enum FeedbackSounds
        {
            Failure,
            Success,
            Good,
            Great,
            Indifferent
        }

        private static readonly SoundPlayer _sp = new SoundPlayer();

        /// <summary>
        /// Beeps.
        /// </summary>
        public static void BeepSignal(FeedbackSounds kind)
        {
            switch (kind)
            {
                case FeedbackSounds.Indifferent:
                    _sp.Stream = Properties.Resources.indifferent;
                    _sp.Play();
                    return;
                case FeedbackSounds.Failure:
                    _sp.Stream = Properties.Resources.failure;
                    _sp.Play();
                    return;
                case FeedbackSounds.Success:
                    _sp.Stream = Properties.Resources.success;
                    _sp.Play();
                    return;
                case FeedbackSounds.Good:
                    _sp.Stream = Properties.Resources.topstat;
                    _sp.Play();
                    return;
                case FeedbackSounds.Great:
                    _sp.Stream = Properties.Resources.newtopstat;
                    _sp.Play();
                    return;
            }
        }
    }
}

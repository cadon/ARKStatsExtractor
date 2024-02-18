using System.Media;
using ARKBreedingStats.library;

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
            Indifferent,
            Updated,
            NewMutation
        }

        private static readonly SoundPlayer Sp = new SoundPlayer();

        /// <summary>
        /// Beeps.
        /// </summary>
        public static void BeepSignal(FeedbackSounds kind)
        {
            switch (kind)
            {
                case FeedbackSounds.Updated:
                    Sp.Stream = Properties.Resources.updated;
                    Sp.Play();
                    return;
                case FeedbackSounds.Indifferent:
                    Sp.Stream = Properties.Resources.indifferent;
                    Sp.Play();
                    return;
                case FeedbackSounds.Failure:
                    Sp.Stream = Properties.Resources.failure;
                    Sp.Play();
                    return;
                case FeedbackSounds.Success:
                    Sp.Stream = Properties.Resources.success;
                    Sp.Play();
                    return;
                case FeedbackSounds.Good:
                    Sp.Stream = Properties.Resources.topstat;
                    Sp.Play();
                    return;
                case FeedbackSounds.Great:
                    Sp.Stream = Properties.Resources.newtopstat;
                    Sp.Play();
                    return;
                case FeedbackSounds.NewMutation:
                    Sp.Stream = Properties.Resources.newMutation;
                    Sp.Play();
                    return;
            }
        }

        /// <summary>
        /// Sound feedback according to current LevelStatusFlags.
        /// </summary>
        public static void BeepSignalCurrentLevelFlags(bool creatureAlreadyExists = false, bool extractionSuccessful = true)
        {
            if (extractionSuccessful)
            {
                if (creatureAlreadyExists)
                    BeepSignal(FeedbackSounds.Updated);
                else if (LevelStatusFlags.CombinedLevelStatusFlags.HasFlag(LevelStatusFlags.LevelStatus.NewMutation))
                    BeepSignal(FeedbackSounds.NewMutation);
                else if (LevelStatusFlags.CombinedLevelStatusFlags.HasFlag(LevelStatusFlags.LevelStatus.NewTopLevel))
                    BeepSignal(FeedbackSounds.Great);
                else if (LevelStatusFlags.CombinedLevelStatusFlags.HasFlag(LevelStatusFlags.LevelStatus.TopLevel))
                    BeepSignal(FeedbackSounds.Good);
                else
                    BeepSignal(FeedbackSounds.Success);
            }
            else
            {
                BeepSignal(FeedbackSounds.Failure);
            }
        }
    }
}

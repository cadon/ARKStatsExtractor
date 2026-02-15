using System.Collections.Generic;
using System.IO;
using ARKBreedingStats.library;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Plays sounds to indicate the result of tasks.
    /// </summary>
    internal static class SoundFeedback
    {
        private static PlayAudioStreams _currentPlayAudioStreams;

        internal enum FeedbackSounds
        {
            None,
            Failure,
            Success,
            Good,
            Great,
            Indifferent,
            Updated,
            NewMutation,
            NewColor,
            NewRegionColor,
            /// <summary>
            /// User can select specific colors, this sound will play if such a color is extracted.
            /// </summary>
            NewDesiredColor
        }

        /// <summary>
        /// Beeps.
        /// </summary>
        public static void BeepSignal(params FeedbackSounds[] sounds)
        {
            var soundList = new List<Stream>();
            foreach (var sound in sounds)
            {
                switch (sound)
                {
                    case FeedbackSounds.Updated:
                        soundList.Add(Properties.Resources.updated);
                        break;
                    case FeedbackSounds.Indifferent:
                        soundList.Add(Properties.Resources.indifferent);
                        break;
                    case FeedbackSounds.Failure:
                        soundList.Add(Properties.Resources.failure);
                        break;
                    case FeedbackSounds.Success:
                        soundList.Add(Properties.Resources.success);
                        break;
                    case FeedbackSounds.Good:
                        soundList.Add(Properties.Resources.topstat);
                        break;
                    case FeedbackSounds.Great:
                        soundList.Add(Properties.Resources.newtopstat);
                        break;
                    case FeedbackSounds.NewMutation:
                        soundList.Add(Properties.Resources.newMutation);
                        break;
                    case FeedbackSounds.NewColor:
                        soundList.Add(Properties.Resources.newColor);
                        break;
                    case FeedbackSounds.NewRegionColor:
                        soundList.Add(Properties.Resources.newRegionColor);
                        break;
                    case FeedbackSounds.NewDesiredColor:
                        soundList.Add(Properties.Resources.newDesiredColor);
                        break;
                }
            }

            _currentPlayAudioStreams?.Stop();
            _currentPlayAudioStreams = new PlayAudioStreams(soundList);
        }

        /// <summary>
        /// Sound feedback according to current LevelStatusFlags.
        /// </summary>
        public static void BeepSignalCurrentLevelFlags(bool creatureAlreadyExists = false, bool extractionSuccessful = true, bool playColorSound = true)
        {
            if (extractionSuccessful)
            {
                FeedbackSounds levelSound;
                if (creatureAlreadyExists)
                    levelSound = FeedbackSounds.Updated;
                else if (LevelColorStatusFlags.StatLevelStatusFlagsCombined.HasFlag(LevelColorStatusFlags.LevelStatus.NewMutation))
                    levelSound = FeedbackSounds.NewMutation;
                else if (LevelColorStatusFlags.StatLevelStatusFlagsCombined.HasFlag(LevelColorStatusFlags.LevelStatus.NewTopLevel))
                    levelSound = FeedbackSounds.Great;
                else if (LevelColorStatusFlags.StatLevelStatusFlagsCombined.HasFlag(LevelColorStatusFlags.LevelStatus.TopLevel))
                    levelSound = FeedbackSounds.Good;
                else
                    levelSound = FeedbackSounds.Success;

                var colorSound = FeedbackSounds.None;
                if (playColorSound)
                {
                    if (LevelColorStatusFlags.ColorFlagsCombined.HasFlag(LevelColorStatusFlags.ColorStatus.DesiredColor))
                        colorSound = FeedbackSounds.NewDesiredColor;
                    else if (LevelColorStatusFlags.ColorFlagsCombined.HasFlag(LevelColorStatusFlags.ColorStatus.NewColor))
                        colorSound = FeedbackSounds.NewColor;
                    else if (LevelColorStatusFlags.ColorFlagsCombined.HasFlag(LevelColorStatusFlags.ColorStatus.NewRegionColor))
                        colorSound = FeedbackSounds.NewRegionColor;
                }

                BeepSignal(levelSound, colorSound);
            }
            else
            {
                BeepSignal(FeedbackSounds.Failure);
            }
        }
    }
}

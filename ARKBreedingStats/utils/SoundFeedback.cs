using System;

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

        /// <summary>
        /// Beeps.
        /// </summary>
        public static void BeepSignal(FeedbackSounds kind)
        {
            switch (kind)
            {
                case FeedbackSounds.Indifferent:
                    Console.Beep(300, 50);
                    Console.Beep(300, 50);
                    Console.Beep(300, 100);
                    break;
                case FeedbackSounds.Failure:
                    Console.Beep(300, 50);
                    Console.Beep(200, 100);
                    break;
                case FeedbackSounds.Success:
                    Console.Beep(300, 50);
                    Console.Beep(400, 100);
                    break;
                case FeedbackSounds.Good:
                    Console.Beep(300, 50);
                    Console.Beep(400, 50);
                    Console.Beep(500, 50);
                    Console.Beep(400, 100);
                    break;
                case FeedbackSounds.Great:
                    Console.Beep(300, 50);
                    Console.Beep(400, 50);
                    Console.Beep(500, 50);
                    Console.Beep(600, 50);
                    Console.Beep(675, 50);
                    Console.Beep(600, 100);
                    break;
            }
        }
    }
}

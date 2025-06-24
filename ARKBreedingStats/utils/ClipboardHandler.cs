using System;
using System.Threading;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Wrapper for windows clipboard to handle exceptions.
    /// </summary>
    internal class ClipboardHandler
    {
        /// <summary>
        /// Attempts to set the specified text to the system clipboard.
        /// </summary>
        /// <param name="text">The text to be copied to the clipboard. If null, an empty string will be used.</param>
        /// <param name="error">
        /// When the operation fails, this parameter will contain an error message describing the failure.
        /// If the operation succeeds, this parameter will be set to <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the text was successfully copied to the clipboard; otherwise, <c>false</c>.
        /// </returns>
        internal static bool SetText(string text, out string error)
        {
            error = null;

            // clipboard operation can throw exception, try again on exception
            const int tries = 3;
            const int delayMs = 300;
            for (var i = tries; i > 0; i--)
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetText(text ?? string.Empty);
                    return true;
                }
                catch (Exception ex)
                {
                    if (i > 1)
                        Thread.Sleep(delayMs);
                    else
                        error = $"Failed to copy text {text} to clipboard, error: {ex.Message}";
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to set the specified text to the system clipboard.
        /// </summary>
        /// <param name="text">The text to be copied to the clipboard. If null, an empty string will be used.</param>
        /// <returns>
        /// <c>true</c> if the text was successfully copied to the clipboard; otherwise, <c>false</c>.
        /// </returns>
        internal static bool SetText(string text) => SetText(text, out _);

        /// <summary>
        /// Clears the content of the Windows clipboard.
        /// </summary>
        /// <remarks>
        /// This method attempts to clear the clipboard content and logs any exceptions that occur during the process.
        /// </remarks>
        internal static void Clear()
        {
            try
            {
                System.Windows.Forms.Clipboard.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to clear clipboard, error: {ex.Message}");
            }
        }
    }
}

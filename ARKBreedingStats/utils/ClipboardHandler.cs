using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

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
        /// <param name="text">The text to be copied to the clipboard. If null or empty the clipboard will be cleared.</param>
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
            // if on non STA thread
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                var staThread = new Thread(() =>
                {
                    try
                    {
                        SetText(text, out _); // error cannot be passed back

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception when trying to set ClipboardText on STA thread: {ex.Message}");
                    }
                });
                staThread.TrySetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();
                return true;
            }

            // clipboard operation can throw exception, try again on exception
            const int tries = 3;
            const int delayMs = 300;
            for (var i = tries; i > 0; i--)
            {
                try
                {
                    if (string.IsNullOrEmpty(text))
                        Clipboard.Clear();
                    else
                        Clipboard.SetText(text);
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

        /// <summary>
        /// Sets an image to the clipboard, trying to use PNG format to preserver the alpha channel.
        /// </summary>
        internal static void SetImageWithAlphaToClipboard(Image img, bool disposeBmp = true)
        {
            if (img == null) return;

            using (var pngStream = new MemoryStream())
            {
                var data = new DataObject();
                data.SetImage(img); // fallback, some applications do not accept the PNG version below

                img.Save(pngStream, ImageFormat.Png);
                data.SetData("PNG", false, pngStream);

                Clipboard.SetDataObject(data, true);
            }
            if (disposeBmp) img.Dispose();
        }
    }
}

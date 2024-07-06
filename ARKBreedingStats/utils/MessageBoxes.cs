using System;
using System.Windows.Forms;
using ARKBreedingStats.uiControls;

namespace ARKBreedingStats.utils
{
    internal static class MessageBoxes
    {
        /// <summary>
        /// Displays a messageBox with the application name and version.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title">If empty, a word depending on the MessageBoxIcon will be used.</param>
        /// <param name="icon"></param>
        internal static void ShowMessageBox(string message, string title = null, MessageBoxIcon icon = MessageBoxIcon.Error, bool displayCopyMessageButton = false)
        {
            if (string.IsNullOrEmpty(title))
            {
                switch (icon)
                {
                    case MessageBoxIcon.Warning:
                        title = "Warning";
                        break;
                    case MessageBoxIcon.Information:
                        title = "Info";
                        break;
                    default:
                        title = Loc.S("error");
                        break;
                }
            }

            if (displayCopyMessageButton)
                CustomMessageBox.Show(message, title, "OK", icon: icon, showCopyToClipboard: true);
            else
                MessageBox.Show(message, $"{title} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Displays an error message with info about the exception and the application name and version.
        /// </summary>
        internal static void ExceptionMessageBox(Exception ex, string messageBeforeException = null, string title = null) =>
            ShowMessageBox((string.IsNullOrEmpty(messageBeforeException) ? string.Empty : messageBeforeException + "\n\n") + ExceptionMessages.WithInner(ex), title, displayCopyMessageButton: true);
    }
}

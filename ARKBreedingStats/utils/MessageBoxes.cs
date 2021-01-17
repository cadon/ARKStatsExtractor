using System;
using System.Windows.Forms;

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
        internal static void ShowMessageBox(string message, string title = null, MessageBoxIcon icon = MessageBoxIcon.Error)
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
            MessageBox.Show(message, $"{title} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Displays an error message with the exception text and the application name and version.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="messageBeforeException"></param>
        /// <param name="title"></param>
        internal static void ExceptionMessageBox(Exception ex, string messageBeforeException = null, string title = null) =>
            ShowMessageBox(
                (string.IsNullOrEmpty(messageBeforeException) ? string.Empty : messageBeforeException + "\n\n")
                + $"Errormessage:\n\n{ex.Message}" + (ex.InnerException == null ? string.Empty : $"\n\nInnerException:\n\n{ex.InnerException.Message}"),
                title);
    }
}

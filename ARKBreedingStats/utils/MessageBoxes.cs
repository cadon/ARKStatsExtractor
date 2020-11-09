using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.utils
{
    internal static class MessageBoxes
    {
        /// <summary>
        /// Displays an error message with the application name and version.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="title"></param>
        internal static void ErrorMessageBox(string error, string title = null, MessageBoxIcon icon = MessageBoxIcon.Error) =>
            MessageBox.Show(error, $"{(string.IsNullOrEmpty(title) ? Loc.S("error") : title)} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, icon);

        /// <summary>
        /// Displays an error message with the exception text and the application name and version.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="messageBeforeException"></param>
        /// <param name="title"></param>
        internal static void ExceptionMessageBox(Exception ex, string messageBeforeException = null, string title = null) =>
            ErrorMessageBox(
                (string.IsNullOrEmpty(messageBeforeException) ? string.Empty : messageBeforeException + "\n\n")
                + $"Errormessage:\n\n{ex.Message}" + (ex.InnerException == null ? string.Empty : $"\n\nInnerException:\n\n{ex.InnerException.Message}"),
                title);
    }
}

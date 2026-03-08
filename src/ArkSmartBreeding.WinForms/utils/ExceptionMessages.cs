using System;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Displays exception message and its inner exceptions if existing.
    /// </summary>
    public static class ExceptionMessages
    {
        /// <summary>
        /// Exception message with inner exceptions
        /// </summary>
        public static string WithInner(Exception ex) =>
            ex.Message
            + "\n\n" + ex.GetType() + " in " + ex.Source
            + "\n\nMethod throwing the error: " + ex.TargetSite.DeclaringType?.FullName + "." +
            ex.TargetSite.Name
            + "\n\nStackTrace:\n" + ex.StackTrace
            + (ex.InnerException != null
                ? "\n\nInner Exception:\n" + WithInner(ex.InnerException)
                : string.Empty);
    }
}

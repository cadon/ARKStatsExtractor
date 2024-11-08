using System;
using System.IO;

namespace ARKBreedingStats.utils
{
    internal class Logging
    {
        internal static void Log(string text, string logFileName = null)
        {
            if (string.IsNullOrEmpty(logFileName))
                logFileName = "log.txt";
            File.AppendAllText(logFileName, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {text}{Environment.NewLine}");
        }
    }
}

using System.Diagnostics;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Links to the ARK wiki.
    /// </summary>
    public static class ArkWiki
    {
        private const string WikiBaseUrl = "https://ark.fandom.com/wiki/";

        /// <summary>
        /// Opens the page in the repository wiki with the default browser.
        /// </summary>
        public static void OpenPage(string pageName)
        {
            if (string.IsNullOrEmpty(pageName)) return;
            Process.Start($"{WikiBaseUrl}{pageName}");
        }
    }
}

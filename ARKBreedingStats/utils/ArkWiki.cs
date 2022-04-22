using System.Diagnostics;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Links to the ARK wiki.
    /// </summary>
    public static class ArkWiki
    {
        private const string WikiBaseUrl = "https://ark.wiki.gg/wiki/";

        public static string WikiUrl(string pageName) => $"{WikiBaseUrl}{pageName}";

        /// <summary>
        /// Opens the page in the Ark wiki with the default browser.
        /// </summary>
        public static void OpenPage(string pageName) => Process.Start(WikiUrl(pageName));
    }
}

using System.Diagnostics;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Information regarding the repository of this project.
    /// </summary>
    internal static class RepositoryInfo
    {
        internal const string RepositoryUrl = "https://github.com/cadon/ARKStatsExtractor/";

        private static string WikiPageLink(string pageName) => $"{RepositoryUrl}wiki/{pageName}";

        /// <summary>
        /// Opens the page in the repository wiki with the default browser.
        /// </summary>
        internal static void OpenWikiPage(string pageName) => Process.Start(WikiPageLink(pageName));

        /// <summary>
        /// Invite link for the ARK Smart Breeding discord link.
        /// </summary>
        internal const string DiscordServerInviteLink = "https://discord.gg/qCYYbQK";
    }
}

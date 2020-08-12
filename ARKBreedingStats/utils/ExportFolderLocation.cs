using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Used to find the folder of exported creature files.
    /// </summary>
    internal static class ExportFolderLocation
    {
        internal static bool GetListOfExportFolders(out string[] exportFolders,
            out (string steamPlayerName, string steamPlayerId)[] steamNamesIds)
        {
            exportFolders = null;
            steamNamesIds = null;

            if (GetSteamInstallationPath(out var steamPath)
                && ReadSteamPlayerIdsAndArkInstallPaths(Path.Combine(steamPath, "config", "config.vdf"),
                    out steamNamesIds, out string[] arkInstallFolders))
            {
                // TODO install folders from config file

                var arkFolder = Path.Combine(steamPath, "steamapps", "common", "ARK");
                if (!Directory.Exists(arkFolder)) return false;

                exportFolders = new[]
                    {Path.Combine(arkFolder, "ShooterGame", "Saved", "DinoExports", steamNamesIds.First().steamPlayerId)};

                return true;
            }

            return false;
        }

        private static bool GetSteamInstallationPath(out string steamPath)
        {
            steamPath = null;

            // try to get registry key for steam installation path
            string keyName = (Environment.Is64BitOperatingSystem ?
                @"SOFTWARE\Wow6432Node\Valve\Steam\" :
                @"SOFTWARE\Valve\Steam\");
            using (var key = Registry.LocalMachine.OpenSubKey(keyName))
            {
                if (key == null) return false;
                steamPath = (string)key.GetValue("InstallPath");
                return !string.IsNullOrEmpty(steamPath);
            }
        }

        /// <summary>
        /// Reads the steam config file (config.vdf) and returns a list of player ids and Ark install locations.
        /// </summary>
        /// <param name="steamConfigFilePath"></param>
        /// <param name="steamPlayerIds"></param>
        /// <param name="ArkInstallPaths"></param>
        /// <returns></returns>
        private static bool ReadSteamPlayerIdsAndArkInstallPaths(string steamConfigFilePath, out (string steamPlayerName, string steamPlayerId)[] steamPlayerIds,
            out string[] ArkInstallPaths)
        {
            steamPlayerIds = null;
            ArkInstallPaths = null; // TODO

            if (!File.Exists(steamConfigFilePath)) return false;

            string configFileContent = File.ReadAllText(steamConfigFilePath);
            if (string.IsNullOrEmpty(configFileContent)) return false;

            var steamAccountRegEx = new Regex(@"""InstallConfigStore"".+""Software"".+""Valve"".+""Steam"".+""Accounts""\s*\{", RegexOptions.Singleline);
            var m = steamAccountRegEx.Match(configFileContent);
            if (!m.Success) return false;

            var playerIdRegEx = new Regex(@"\s*""([^""]+)""\s*\{\s*""SteamID""\s*""(\d+)""\s*\}", RegexOptions.Singleline);
            var mm = playerIdRegEx.Matches(configFileContent, m.Length);

            steamPlayerIds = (from Match mi in mm select (mi.Groups[1].Value, mi.Groups[2].Value)).ToArray();

            return steamPlayerIds.Any();
        }
    }
}

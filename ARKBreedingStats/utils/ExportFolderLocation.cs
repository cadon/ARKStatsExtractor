using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Used to find the folder of exported creature files.
    /// </summary>
    internal static class ExportFolderLocation
    {
        /// <summary>
        /// Extracts possible creature export directories of a Steam or Epic installation of ARK.
        /// </summary>
        internal static bool GetListOfExportFolders(out (string path, string launcherPlayerName)[] exportFolders, out string error)
        {
            if (ExtractSteamExportLocations(out exportFolders, out error)) return true;

            var steamError = error;

            // steam path couldn't be localized. Try Epic
            if (ExtractEpicExportLocations(out exportFolders, out error)) return true;

            error = $"Steam: {steamError}\n\nEpic: {error}";

            exportFolders = null;
            return false;
        }

        /// <summary>
        /// Extracts possible creature export directories of a Steam installation of ARK.
        /// </summary>
        private static bool ExtractSteamExportLocations(out (string path, string launcherPlayerName)[] exportFolders,
            out string error)
        {
            exportFolders = null;

            if (!GetSteamInstallationPath(out var steamPath))
            {
                error = "Steam installation couldn't be found, is it installed?";
                return false;
            }

            var configFilePath = Path.Combine(steamPath, "config", "config.vdf");
            if (!File.Exists(configFilePath))
            {
                error = $"Steam config file {configFilePath} not found.";
                return false;
            }

            if (!ReadSteamPlayerIdsAndArkInstallPaths(configFilePath,
                out (string steamPlayerName, string steamPlayerId)[] steamNamesIds, out string[] arkInstallFolders,
                out error)) return false;

            var relativeArkPath = Path.Combine("steamapps", "common", "ARK");
            var possibleArkPaths = new List<string> { Path.Combine(steamPath, relativeArkPath) };
            possibleArkPaths.AddRange(arkInstallFolders.Select(f => Path.Combine(f, relativeArkPath)));

            var existingArkPaths = possibleArkPaths.Where(Directory.Exists).ToArray();

            if (!existingArkPaths.Any())
            {
                error = "No installation folders with ARK found.";
                return false;
            }

            var relativeExportFolder = RelativeExportFolder();

            // there can be multiple steam users, so list the possible export folder for each user
            exportFolders = new (string, string)[existingArkPaths.Length * steamNamesIds.Length];
            int i = 0;
            foreach (var arkPath in existingArkPaths)
            {
                foreach (var steamNameId in steamNamesIds)
                    exportFolders[i++] = (Path.Combine(arkPath, relativeExportFolder, steamNameId.steamPlayerId),
                        steamNameId.steamPlayerName);
            }

            return true;
        }

        /// <summary>
        /// Extracts possible creature export directories of an Epic installation of ARK.
        /// </summary>
        private static bool ExtractEpicExportLocations(out (string path, string launcherPlayerName)[] exportFolders,
            out string error)
        {
            exportFolders = null;

            var configFilePath = Environment.ExpandEnvironmentVariables(@"%ProgramData%\Epic\UnrealEngineLauncher\LauncherInstalled.dat");

            if (!File.Exists(configFilePath))
            {
                error = $"Epic config file {configFilePath} not found.";
                return false;
            }

            if (!ReadEpicArkInstallPaths(configFilePath, out string[] arkInstallFolders, out error)) return false;

            var existingArkPaths = arkInstallFolders.Where(Directory.Exists).ToArray();

            if (!existingArkPaths.Any())
            {
                error = "No installation folders with ARK found.";
                return false;
            }

            string playerId = string.Empty; // TODO what is the exact folder name of Epic export folders, maybe a player id like for steam?

            exportFolders = new[]
            {
                (Path.Combine(existingArkPaths[0], RelativeExportFolder(), playerId), string.Empty)
            };

            return true;
        }

        private static string RelativeExportFolder() => Path.Combine("ShooterGame", "Saved", "DinoExports");

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
        private static bool ReadSteamPlayerIdsAndArkInstallPaths(string steamConfigFilePath, out (string steamPlayerName, string steamPlayerId)[] steamPlayerIds,
            out string[] arkInstallPaths, out string error)
        {
            steamPlayerIds = null;
            arkInstallPaths = null;
            error = null;

            if (!File.Exists(steamConfigFilePath))
            {
                error = $"Steam config file not found\n{steamConfigFilePath}";
                return false;
            }

            string configFileContent = File.ReadAllText(steamConfigFilePath);
            if (string.IsNullOrEmpty(configFileContent))
            {
                error = $"Steam config file empty\n{steamConfigFilePath}";
                return false;
            }

            var steamAccountRegEx = new Regex(@"""InstallConfigStore"".+""Software"".+""Valve"".+""Steam"".+""Accounts""\s*\{", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var m = steamAccountRegEx.Match(configFileContent);
            if (!m.Success)
            {
                error = $"Steam account info property not found in\n{steamConfigFilePath}";
                return false;
            }

            var playerIdRegEx = new Regex(@"\s*""([^""]+)""\s*\{\s*""SteamID""\s*""(\d+)""\s*\}", RegexOptions.Singleline);
            var mm = playerIdRegEx.Matches(configFileContent, m.Length);

            steamPlayerIds = (from Match mi in mm select (mi.Groups[1].Value, mi.Groups[2].Value)).ToArray();

            // steam library locations
            var libraryRegEx = new Regex(@"""BaseInstallFolder_\d+""\s*""([^""]+)""");
            mm = libraryRegEx.Matches(configFileContent);
            var removeEscapeBackslashes = new Regex(@"\\(.)");
            arkInstallPaths = (from Match mi in mm select removeEscapeBackslashes.Replace(mi.Groups[1].Value, "$1")).ToArray();

            bool anyPlayerIds = steamPlayerIds.Any();
            if (!anyPlayerIds)
                error = "No steam accounts in the steam config file found.";

            return anyPlayerIds;
        }

        /// <summary>
        /// Reads the epic config file (LauncherInstalled.dat) and returns a list of Ark install locations.
        /// </summary>
        private static bool ReadEpicArkInstallPaths(string configFilePath, out string[] arkInstallPaths, out string error)
        {
            arkInstallPaths = null;
            error = null;

            if (!File.Exists(configFilePath))
            {
                error = $"Epic config file not found\n{configFilePath}";
                return false;
            }

            JArray installationList;
            using (StreamReader file = File.OpenText(configFilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                installationList = (JArray)JToken.ReadFrom(reader)["InstallationList"];
            }

            if (installationList == null)
            {
                error = "InstallationList node not found in Epic config file";
                return false;
            }

            string arkPath = null;

            foreach (JObject o in installationList)
            {
                if ((string)o["AppName"] == "aafc587fbf654758802c8e41e4fb3255")
                {
                    arkPath = (string)o["InstallLocation"];
                    break;
                }
            }

            if (string.IsNullOrEmpty(arkPath))
            {
                error = "No Ark installation path in the Epic config file found";
                return false;
            }

            arkInstallPaths = new[] { arkPath };
            return true;
        }
    }
}

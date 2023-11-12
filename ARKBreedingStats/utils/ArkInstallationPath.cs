using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Methods related to paths of an Ark installation.
    /// </summary>
    internal static class ArkInstallationPath
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

            if (!GetSteamLibraryArkInstallationFolders(out var existingArkPaths, out var steamNamesIds, out error))
                return false;

            var relativeExportFolder = RelativeExportPath();

            // there can be multiple steam users, so list the possible export folder for each user
            exportFolders = new (string, string)[existingArkPaths.Length * (steamNamesIds.Length + 1)];
            int i = 0;
            foreach (var arkPath in existingArkPaths)
            {
                foreach (var steamNameId in steamNamesIds)
                    exportFolders[i++] = (
                        arkPath.Game == Ark.Game.ASE
                        ? Path.Combine(arkPath.Path, relativeExportFolder, steamNameId.steamPlayerId)
                        : Path.Combine(arkPath.Path, relativeExportFolder), // ASA doesn't use steam id as subfolder
                        $"{steamNameId.steamPlayerName} ({arkPath.Game})"
                        );
                // for export gun mod
                exportFolders[i++] = (Path.Combine(arkPath.Path, relativeExportFolder, "ASB"),
                    $"ExportGun ({arkPath.Game})");
            }

            return true;
        }

        /// <summary>
        /// Returns a list of possible Ark installation folders
        /// </summary>
        /// <returns></returns>
        public static bool GetSteamLibraryArkInstallationFolders(out (string Path, Ark.Game Game)[] existingArkPaths, out (string steamPlayerName, string steamPlayerId)[] steamNamesIds, out string error)
        {
            existingArkPaths = null;
            steamNamesIds = null;
            if (!GetSteamInstallationPath(out var steamPath))
            {
                error = "Steam installation couldn't be found, is it installed?";
                return false;
            }

            var configFilePath = Path.Combine(steamPath, "config", "config.vdf");
            var libraryFoldersFilePath = Path.Combine(steamPath, "config", "libraryfolders.vdf");
            if (!File.Exists(configFilePath))
            {
                error = $"Steam config file {configFilePath} not found.";
                return false;
            }

            if (!ReadSteamPlayerIdsAndArkInstallPaths(configFilePath, libraryFoldersFilePath,
                    out steamNamesIds, out string[] arkInstallFolders,
                    out error)) return false;

            var relativeAsePath = Path.Combine("steamapps", "common", "ARK");
            var relativeAsaPath = Path.Combine("steamapps", "common", "ARK Survival Ascended");
            var possibleArkPaths = new List<(string Path, Ark.Game Game)>
            {
                (Path.Combine(steamPath, relativeAsePath), Ark.Game.ASE),
                (Path.Combine(steamPath, relativeAsaPath), Ark.Game.ASA)
            }; // use steam folder as default
            possibleArkPaths.AddRange(arkInstallFolders.Select(f => (Path.Combine(f, relativeAsePath), Ase: Ark.Game.ASE)));
            possibleArkPaths.AddRange(arkInstallFolders.Select(f => (Path.Combine(f, relativeAsaPath), Asa: Ark.Game.ASA)));

            existingArkPaths = possibleArkPaths.Distinct().Where(p => Directory.Exists(p.Path)).ToArray();

            if (!existingArkPaths.Any())
            {
                error = "No installation folders with ARK found.";
                return false;
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
                (Path.Combine(existingArkPaths[0], RelativeExportPath(), playerId), string.Empty)
            };

            return true;
        }

        private static string RelativeExportPath() => Path.Combine("ShooterGame", "Saved", "DinoExports");
        public static string RelativeLocalConfigPathAse() => Path.Combine("ShooterGame", "Saved", "Config", "WindowsNoEditor");
        public static string RelativeLocalConfigPathAsa() => Path.Combine("ShooterGame", "Saved", "Config", "Windows");

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
        private static bool ReadSteamPlayerIdsAndArkInstallPaths(string steamConfigFilePath, string steamLibraryFoldersFilePath, out (string steamPlayerName, string steamPlayerId)[] steamPlayerIds,
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

            // steam library locations. TODO maybe not existing anymore and only in libraryfolders.vdf
            var libraryRegEx = new Regex(@"""BaseInstallFolder_\d+""\s*""([^""]+)""");
            mm = libraryRegEx.Matches(configFileContent);
            var removeEscapeBackslashes = new Regex(@"\\(.)");
            arkInstallPaths = (from Match mi in mm select removeEscapeBackslashes.Replace(mi.Groups[1].Value, "$1")).ToArray();

            // using libraryfolders.vdf
            if (!string.IsNullOrEmpty(steamLibraryFoldersFilePath) && File.Exists(steamLibraryFoldersFilePath))
            {
                var pathRegex = new Regex(@"""path""\s*""([^""]+)""");
                mm = pathRegex.Matches(File.ReadAllText(steamLibraryFoldersFilePath));
                if (mm.Count > 0)
                    arkInstallPaths = arkInstallPaths.Concat(from Match mi in mm select removeEscapeBackslashes.Replace(mi.Groups[1].Value, "$1")).ToArray();
            }

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

        /// <summary>
        /// Uses the first element as folder path and orders the elements by the newest file created descending.
        /// </summary>
        public static T[] OrderByNewestFileInFolders<T>(IEnumerable<(string Path, T)> folders)
        {
            return folders?.Select(l => (l, LastWriteOfNewestFileInFolder(l.Path)))
                .OrderByDescending(l => l.Item2).Select(l => l.l.Item2).ToArray();

            DateTime LastWriteOfNewestFileInFolder(string folderPath)
            {
                if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) return new DateTime();
                return new DirectoryInfo(folderPath).GetFiles("*.ini").Select(fi => fi.LastWriteTime).DefaultIfEmpty(new DateTime()).Max();
            }
        }

        /// <summary>
        /// Returns the path to the local config folder, where the files game.ini and gameUserSettings.ini are stored.
        /// </summary>
        public static bool GetLocalArkConfigPaths(out (string, Ark.Game)[] localConfigPaths, out string error)
        {
            localConfigPaths = null;

            if (!GetSteamLibraryArkInstallationFolders(out var existingArkPaths, out _, out error))
                return false;

            var relativeLocalConfigPathAse = RelativeLocalConfigPathAse();
            var relativeLocalConfigPathAsa = RelativeLocalConfigPathAsa();

            localConfigPaths = new (string, Ark.Game)[existingArkPaths.Length];
            int i = 0;
            foreach (var arkPath in existingArkPaths)
            {
                localConfigPaths[i++] = (
                        arkPath.Game == Ark.Game.ASE
                            ? Path.Combine(arkPath.Path, relativeLocalConfigPathAse)
                            : Path.Combine(arkPath.Path, relativeLocalConfigPathAsa),
                        arkPath.Game
                    );
            }

            return true;
        }
    }
}

using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ARKBreedingStats.values;

namespace ARKBreedingStats.mods
{
    /// <summary>
    /// Contains info about available mod files and their version.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ModsManifest
    {
        /// <summary>
        /// Default format version, is valid for all entries that have no specific format version.
        /// </summary>
        [JsonProperty("format")]
        public string DefaultFormatVersion;

        /// <summary>
        /// Dictionary of ModInfos. The key is the mod-filename.
        /// </summary>
        [JsonProperty("files")]
        public Dictionary<string, ModInfo> ModsByFiles = new Dictionary<string, ModInfo>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Dictionary of ModInfos. The key is the modTag prepended by either ASE or ASA.
        /// </summary>
        public Dictionary<string, ModInfo> ModsByTag = new Dictionary<string, ModInfo>();

        /// <summary>
        /// Dictionary of ModInfos. The key is the modID.
        /// </summary>
        public Dictionary<string, ModInfo> ModsById = new Dictionary<string, ModInfo>();

        /// <summary>
        /// Tries to load the manifest file.
        /// </summary>
        /// <param name="forceDownload"></param>
        /// <param name="downloadTry"></param>
        /// <returns></returns>
        public static async Task<ModsManifest> TryLoadModManifestFile(bool forceDownload = false, int downloadTry = 0)
        {
            while (true)
            {
                string modsManifestFilePath = FileService.GetJsonPath(FileService.ValuesFolder, FileService.ManifestFileName);
                if (forceDownload || !File.Exists(modsManifestFilePath)) await TryDownloadFileAsync();

                if (FileService.LoadJsonFile(modsManifestFilePath, out ModsManifest tmpV, out string errorMessage))
                {
                    // set format versions
                    // if an entry has no specific format version, it is the general format version of the manifest file
                    foreach (var mi in tmpV.ModsByFiles)
                    {
                        if (string.IsNullOrEmpty(mi.Value.Format))
                            mi.Value.Format = tmpV.DefaultFormatVersion;
                    }

                    return tmpV;
                }

                if (!forceDownload && downloadTry == 0)
                {
                    // file is probably corrupted, try to redownload it
                    forceDownload = true;
                    downloadTry = 1;
                    continue;
                }

                throw new SerializationException(errorMessage);
            }
        }

        /// <summary>
        /// User can create custom manual values files, e.g. for mods. If there are any available, load them.
        /// The values files of supported mods in the manifest file are ignored.
        /// </summary>
        public static bool LoadManualValueFiles(ModsManifest officialModsManifest, out ModsManifest customModsManifest)
        {
            customModsManifest = null;
            string valuesFolderPath = FileService.GetJsonPath(FileService.ValuesFolder);
            if (!Directory.Exists(valuesFolderPath)) return false;

            var possibleModValueFiles = Directory.GetFiles(valuesFolderPath, "*.json");

            customModsManifest = new ModsManifest();
            var alreadyLoadedOfficialModIds = officialModsManifest.ModsByFiles
                .Where(mf => !string.IsNullOrEmpty(mf.Value.Mod?.Id) && !mf.Value.ManuallyLoaded)
                .Select(mf => mf.Value.Mod?.Id)
                .ToHashSet();

            foreach (var modValuesFilePath in possibleModValueFiles)
            {
                var fileName = Path.GetFileName(modValuesFilePath);
                if (fileName.StartsWith("_") || (officialModsManifest.ModsByFiles.TryGetValue(fileName, out var modInfoAlreadyLoaded) && !modInfoAlreadyLoaded.ManuallyLoaded))
                    continue;

                if (!ValuesFile.TryLoadingModInfoHeader(modValuesFilePath, out var modInfo))
                    continue;

                // if mod is official and already loaded, or already loaded in this loop, ignore file
                if (!alreadyLoadedOfficialModIds.Add(modInfo.Mod.Id))
                    continue;

                modInfo.ManuallyLoaded = true;
                customModsManifest.ModsByFiles.Add(fileName, modInfo);
            }

            return customModsManifest.ModsByFiles.Any();
        }

        /// <summary>
        /// Downloads the current file from the server.
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> TryDownloadFileAsync()
        {
            return await Updater.Updater.DownloadModsManifest();
        }

        /// <summary>
        /// The manifest file only contains a dictionary by fileName. This method initializes the other properties.
        /// </summary>
        internal void Initialize()
        {
            ModsByTag = new Dictionary<string, ModInfo>();
            ModsById = new Dictionary<string, ModInfo>();

            // generic entry for "other mod", this is needed to correctly determine the available color set.
            if (!ModsByFiles.ContainsKey(Mod.OtherMod.FileName))
                ModsByFiles.Add(Mod.OtherMod.FileName, new ModInfo { Mod = Mod.OtherMod });

            string valuesPath = FileService.GetJsonPath(FileService.ValuesFolder);

            foreach (var fmi in ModsByFiles)
            {
                var modInfo = fmi.Value;
                if (modInfo.Mod == null) continue;

                modInfo.Mod.FileName = fmi.Key;
                modInfo.LocallyAvailable = !string.IsNullOrEmpty(modInfo.Mod.FileName) && File.Exists(Path.Combine(valuesPath, modInfo.Mod.FileName));

                if (!string.IsNullOrEmpty(modInfo.Mod.Tag)
                    && !ModsByTag.ContainsKey(modInfo.Mod.TagWithGamePrefix))
                {
                    ModsByTag.Add(modInfo.Mod.TagWithGamePrefix, fmi.Value);
                }

                if (!string.IsNullOrEmpty(modInfo.Mod.Id)
                    && !ModsById.ContainsKey(modInfo.Mod.Id))
                {
                    ModsById.Add(modInfo.Mod.Id, modInfo);
                }
            }
        }

        /// <summary>
        /// Downloads the modFiles. Returns true if a file was downloaded.
        /// </summary>
        public bool DownloadModFiles(IEnumerable<string> modValueFiles)
        {
            bool filesDownloaded = false;
            foreach (var mf in modValueFiles)
            {
                if (Updater.Updater.DownloadModValuesFile(mf)
                    && ModsByFiles.TryGetValue(mf, out var modInfo))
                {
                    modInfo.LocallyAvailable = true;
                    filesDownloaded = true;
                }
            }
            return filesDownloaded;
        }

        /// <summary>
        /// Merges two mod manifests (while for duplicate entries the manifest2 item will be kept) and returns the combined one. This is used to combine the official manifest with the custom created manifest.
        /// </summary>
        /// <param name="manifest1"></param>
        /// <param name="manifest2"></param>
        /// <returns></returns>
        internal static ModsManifest MergeModsManifest(ModsManifest manifest1, ModsManifest manifest2)
        => new ModsManifest
        {
            DefaultFormatVersion = manifest1.DefaultFormatVersion,
            ModsByFiles = manifest2.ModsByFiles
                .Concat(manifest1.ModsByFiles.Where(m1 => !manifest2.ModsByFiles.ContainsKey(m1.Key)))
                .ToDictionary(m => m.Key, m => m.Value)
        };
    }
}

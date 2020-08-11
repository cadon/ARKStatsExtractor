using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ARKBreedingStats.mods
{
    /// <summary>
    /// Contains info about available mod files and their version.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ModsManifest
    {
        /// <summary>
        /// Dictionary of ModInfos. The key is the mod-filename.
        /// </summary>
        [JsonProperty("files")]
        public Dictionary<string, ModInfo> modsByFiles;

        /// <summary>
        /// Dictionary of ModInfos. The key is the modTag.
        /// </summary>
        public Dictionary<string, ModInfo> modsByTag;

        /// <summary>
        /// Dictionary of ModInfos. The key is the modID.
        /// </summary>
        public Dictionary<string, ModInfo> modsByID;

        public ModsManifest()
        {
            modsByFiles = new Dictionary<string, ModInfo>();
            modsByTag = new Dictionary<string, ModInfo>();
            modsByID = new Dictionary<string, ModInfo>();
        }

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
                string modsManifestPath = Path.Combine(FileService.ValuesFolder, FileService.ModsManifest);
                if (forceDownload || !File.Exists(FileService.GetJsonPath(modsManifestPath))) await TryDownloadFileAsync();

                if (FileService.LoadJsonFile(FileService.GetJsonPath(modsManifestPath), out ModsManifest tmpV, out string errorMessage))
                {
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
        /// Users can create an additional custom manifest file for manually created mod files. If available, it's loaded with this method.
        /// </summary>
        /// <returns></returns>
        public static bool TryLoadCustomModManifestFile(out ModsManifest customModsManifest)
        {
            customModsManifest = null;
            string filePath = FileService.GetJsonPath(FileService.ValuesFolder, FileService.ModsManifestCustom);
            if (!File.Exists(filePath)) return false;

            if (FileService.LoadJsonFile(filePath, out ModsManifest tmpV, out string errorMessage))
            {
                customModsManifest = tmpV;
                return true;
            }

            throw new SerializationException(errorMessage);
        }

        /// <summary>
        /// Downloads the current file from the server.
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> TryDownloadFileAsync()
        {
            return await Updater.DownloadModsManifest();
        }

        /// <summary>
        /// The manifest file only contains a dictionary by fileName. This method initializes the other properties.
        /// </summary>
        internal void Initialize()
        {
            modsByTag = new Dictionary<string, ModInfo>();
            modsByID = new Dictionary<string, ModInfo>();

            string valuesPath = FileService.GetJsonPath(FileService.ValuesFolder);

            foreach (KeyValuePair<string, ModInfo> fmi in modsByFiles)
            {
                if (fmi.Value.mod == null) continue;

                fmi.Value.mod.FileName = fmi.Key;
                fmi.Value.locallyAvailable = !string.IsNullOrEmpty(fmi.Value.mod.FileName) && File.Exists(Path.Combine(valuesPath, fmi.Value.mod.FileName));

                if (!string.IsNullOrEmpty(fmi.Value.mod.tag)
                    && !modsByTag.ContainsKey(fmi.Value.mod.tag))
                {
                    modsByTag.Add(fmi.Value.mod.tag, fmi.Value);
                }

                if (!string.IsNullOrEmpty(fmi.Value.mod.id)
                    && !modsByID.ContainsKey(fmi.Value.mod.id))
                {
                    modsByID.Add(fmi.Value.mod.id, fmi.Value);
                }
            }
        }

        /// <summary>
        /// Downloads the modFiles. Returns true if a file was downloaded.
        /// </summary>
        /// <param name="modValueFiles"></param>
        /// <returns></returns>
        public bool DownloadModFiles(List<string> modValueFiles)
        {
            bool filesDownloaded = false;
            foreach (var mf in modValueFiles)
            {
                if (Updater.DownloadModValuesFile(mf)
                    && modsByFiles.ContainsKey(mf))
                {
                    modsByFiles[mf].locallyAvailable = true;
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
        => new ModsManifest()
        {
            modsByFiles = manifest2.modsByFiles.Concat(manifest1.modsByFiles.Where(m1 => !manifest2.modsByFiles.ContainsKey(m1.Key))).ToDictionary(m => m.Key, m => m.Value)
        };

    }
}

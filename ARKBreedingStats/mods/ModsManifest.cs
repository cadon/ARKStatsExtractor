using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.mods
{
    /// <summary>
    /// Contains info about available mod files and their version.
    /// </summary>
    [DataContract]
    public class ModsManifest
    {
        /// <summary>
        /// Must be present and a supported value. Defaults to an invalid value
        /// </summary>
        [DataMember]
        private string format = string.Empty;
        /// <summary>
        /// Dictionary of ModInfos. The key is the mod-filename.
        /// </summary>
        [DataMember(Name = "files")]
        public Dictionary<string, ModInfo> modsByFiles;

        /// <summary>
        /// Dictionary of ModInfos. The key is the modTag.
        /// </summary>
        [IgnoreDataMember]
        public Dictionary<string, ModInfo> modsByTag;

        /// <summary>
        /// Dictionary of ModInfos. The key is the modID.
        /// </summary>
        [IgnoreDataMember]
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
            string modsManifestPath = Path.Combine(FileService.ValuesFolder, FileService.ModsManifest);
            if (forceDownload || !File.Exists(FileService.GetJsonPath(modsManifestPath)))
                await TryDownloadFileAsync();

            try
            {
                using (FileStream file = FileService.GetJsonFileStream(modsManifestPath))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ModsManifest)
                        , new DataContractJsonSerializerSettings()
                        {
                            UseSimpleDictionaryFormat = true
                        }
                        );
                    var tmpV = (ModsManifest)ser.ReadObject(file);
                    if (tmpV.format != Values.CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");

                    tmpV.Initialize();
                    return tmpV;
                }
            }
            catch (SerializationException e)
            {
                if (downloadTry == 0)
                {
                    // file is probably corrupted, try to redownload
                    return await TryLoadModManifestFile(forceDownload: true, downloadTry: 1);
                }
                // redownload didn't solve the issue
                throw e;
            }
        }

        private void Initialize()
        {
            string valuesPath = FileService.GetJsonPath(FileService.ValuesFolder);
            foreach (KeyValuePair<string, ModInfo> mi in modsByFiles)
            {
                if (mi.Value.mod != null)
                {
                    mi.Value.mod.FileName = mi.Key;
                    mi.Value.onlineAvailable = true;
                    mi.Value.downloaded = mi.Value.mod.FileName != null && File.Exists(Path.Combine(valuesPath, mi.Value.mod.FileName));
                }
            }
        }

        /// <summary>
        /// Downloads the current file from the server.
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> TryDownloadFileAsync()
        {
            return await Updater.DownloadModsManifest();
        }

        [OnDeserialized]
        private void SetModDictionaries(StreamingContext c)
        {
            modsByTag = new Dictionary<string, ModInfo>();
            modsByID = new Dictionary<string, ModInfo>();

            foreach (KeyValuePair<string, ModInfo> fmi in modsByFiles)
            {
                if (!string.IsNullOrEmpty(fmi.Value.mod?.tag)
                    && !modsByTag.ContainsKey(fmi.Value.mod.tag))
                {
                    modsByTag.Add(fmi.Value.mod.tag, fmi.Value);
                }
                if (!string.IsNullOrEmpty(fmi.Value.mod?.id)
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
                    modsByFiles[mf].downloaded = true;
                    filesDownloaded = true;
                }
            }
            return filesDownloaded;
        }
    }
}

using ARKBreedingStats.values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
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

        public static async Task<ModsManifest> TryLoadModManifestFile(bool forceUpdate = false, int downloadTry = 0)
        {
            if (forceUpdate || !File.Exists(FileService.GetJsonPath(FileService.ModsManifest)))
                await TryDownloadFileAsync();

            try
            {
                using (FileStream file = FileService.GetJsonFileStream(FileService.ModsManifest))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ModsManifest)
                        , new DataContractJsonSerializerSettings()
                        {
                            UseSimpleDictionaryFormat = true
                        }
                        );
                    var tmpV = (ModsManifest)ser.ReadObject(file);
                    if (tmpV.format != Values.CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
                    if (tmpV != null)
                    {
                        foreach (KeyValuePair<string, ModInfo> mi in tmpV.modsByFiles)
                        {
                            if (mi.Value.mod != null)
                            {
                                mi.Value.mod.FileName = mi.Key;
                                mi.Value.onlineAvailable = true;
                                mi.Value.downloaded = mi.Value.mod.FileName != null && File.Exists(FileService.GetJsonPath(Path.Combine("mods", mi.Value.mod.FileName)));
                            }
                        }
                    }
                    return tmpV;
                }
            }
            catch (FileNotFoundException)
            {
                if (downloadTry > 0)
                    MessageBox.Show($"Mods manifest file { FileService.ModsManifest} not found" +
                        " and downloading it failed. You can try it later or try to update your application.",
                        "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (MessageBox.Show($"Mods manifest file {FileService.ModsManifest} not found." +
                        "Mod infos will not be shown.\n\n" +
                        "Do you want to download this file?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    return await TryLoadModManifestFile(forceUpdate: true, downloadTry: 1);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show($"File {FileService.ModsManifest} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show($"File {FileService.ModsManifest} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        /// <summary>
        /// Downloads the current file from the server.
        /// </summary>
        /// <returns></returns>
        internal static async Task<bool> TryDownloadFileAsync()
        {
            await Updater.DownloadModsManifest();
            return true;
        }

        [OnDeserialized]
        private void SetModTagDictionary(StreamingContext c)
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
    }
}

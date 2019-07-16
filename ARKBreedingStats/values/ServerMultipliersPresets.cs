using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.values
{
    [DataContract]
    public class ServerMultipliersPresets
    {
        [DataMember]
        private string formatVersion = string.Empty; // must be present and a supported value, so defaults to an invalid value
        [DataMember]
        public Dictionary<string, ServerMultipliers> serverMultiplierDictionary;
        [IgnoreDataMember]
        public List<string> PresetNameList;

        public ServerMultipliersPresets()
        {
            serverMultiplierDictionary = new Dictionary<string, ServerMultipliers>();
            PresetNameList = new List<string>();
        }

        /// <summary>
        /// Loads the presets for server multipliers and the changes for the singleplayer. If the loading is not successful, the default values are assumed.
        /// </summary>
        /// <param name="serverMultipliers"></param>
        /// <returns></returns>
        public static bool TryLoadServerMultipliersPresets(out ServerMultipliersPresets serverMultipliersPresets)
        {
            serverMultipliersPresets = new ServerMultipliersPresets();
            try
            {
                using (FileStream file = FileService.GetJsonFileStream(FileService.ValuesServerMultipliers))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ServerMultipliersPresets), new DataContractJsonSerializerSettings()
                    {
                        UseSimpleDictionaryFormat = true
                    });
                    var tmpV = (ServerMultipliersPresets)ser.ReadObject(file);
                    if (tmpV.formatVersion != Values.CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
                    serverMultipliersPresets = tmpV;
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"Servermultipliers-File {FileService.ValuesServerMultipliers} not found. " +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show($"File {FileService.ValuesServerMultipliers} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"File {FileService.ValuesServerMultipliers} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Returns  serverMultipliers with the passed name. Return null if no preset with that name exists.
        /// </summary>
        /// <param name="presetName"></param>
        /// <returns></returns>
        public ServerMultipliers GetPreset(string presetName)
        {
            if (!string.IsNullOrEmpty(presetName)
                && serverMultiplierDictionary.ContainsKey(presetName))
                return serverMultiplierDictionary[presetName];
            return null;
        }

        [OnDeserialized]
        private void CreatePresetNameList(StreamingContext context)
        {
            PresetNameList = serverMultiplierDictionary.Select(sm => sm.Key).ToList();
        }
    }
}

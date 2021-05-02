using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.values
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerMultipliersPresets
    {
        [JsonProperty]
        private string format; // must be present and a supported value
        [JsonProperty]
        public Dictionary<string, ServerMultipliers> serverMultiplierDictionary;
        public List<string> PresetNameList;

        public const string Official = "official";
        public const string Singleplayer = "singleplayer";

        private static bool IsValidFormat(string formatVersion) => formatVersion == "1.13";

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
            if (FileService.LoadJsonFile(FileService.GetJsonPath(FileService.ValuesServerMultipliers), out ServerMultipliersPresets readData, out string errorMessage))
            {
                if (IsValidFormat(readData.format))
                {
                    serverMultipliersPresets = readData;
                    return true;
                }
                MessageBoxes.ShowMessageBox($"The file {FileService.ValuesServerMultipliers} is in the format\n{readData.format}\nwhich is unsupported in this version of ARK Smart Breeding." +
                                             "\n\nTry updating to a newer version.");

            }
            else
            {
                if (MessageBox.Show($"Servermultipliers-File {FileService.ValuesServerMultipliers} couldn't be read.\n{errorMessage}\n\n" +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.Updater.ReleasesUrl);
            }

            return false;
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
                return serverMultiplierDictionary[presetName].Copy(true);
            return null;
        }

        [OnDeserialized]
        private void CreatePresetNameList(StreamingContext context)
        {
            PresetNameList = serverMultiplierDictionary.Select(sm => sm.Key).ToList();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace ARKBreedingStats.values
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServerMultipliersPresets
    {
        [JsonProperty]
        private string format = string.Empty; // must be present and a supported value
        [JsonProperty]
        public Dictionary<string, ServerMultipliers> serverMultiplierDictionary;
        public List<string> PresetNameList;

        public const string OFFICIAL = "official";
        public const string SINGLEPLAYER = "singleplayer";

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
                if (Values.IsValidFormatVersion(readData.format))
                {
                    serverMultipliersPresets = readData;
                    return true;
                }
                MessageBox.Show($"File {FileService.ValuesServerMultipliers} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                if (MessageBox.Show($"Servermultipliers-File {FileService.ValuesServerMultipliers} couldn't be read.\n{errorMessage}\n\n" +
                        "ARK Smart Breeding will not work properly without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
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

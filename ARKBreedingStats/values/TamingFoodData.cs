using ARKBreedingStats.species;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats.values
{
    /// <summary>
    /// Contains the default food data for taming.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class TamingFoodData
    {
        [JsonProperty]
        private string format = string.Empty; // must be present and a supported value
        [JsonProperty]
        private string version = string.Empty; // must be present and a supported value
        /// <summary>
        /// The key is the species name, so it can be used for modded species with the same name, they often have the same taming-data.
        /// The value is a dictionary with the food-name as key and TamingFood info as value.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, TamingData> tamingFoodData = new Dictionary<string, TamingData>();

        /// <summary>
        /// Loads the default food data.
        /// </summary>
        /// <param name="serverMultipliers"></param>
        /// <returns></returns>
        public static bool TryLoadDefaultFoodData(out Dictionary<string, TamingData> tamingFoodData)
        {
            tamingFoodData = null;
            string filePath = FileService.GetJsonPath(FileService.TamingFoodData);
            string errorMessage = $"File not found: {filePath}";

            if (File.Exists(filePath) && FileService.LoadJsonFile(filePath, out TamingFoodData readData, out errorMessage))
            {
                if (Values.IsValidFormatVersion(readData.format))
                {
                    tamingFoodData = readData.tamingFoodData;
                    return tamingFoodData != null;
                }
            }
            else
            {
                if (MessageBox.Show($"The default food data file {FileService.TamingFoodData} couldn't be loaded:\n{errorMessage}\n\n" +
                        "The taming info will be incomplete without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
            }

            return false;
        }
    }
}

using ARKBreedingStats.species;
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
    /// <summary>
    /// Contains the default food data for taming.
    /// </summary>
    [DataContract]
    class TamingFoodData
    {
        [DataMember]
        private string format = string.Empty; // must be present and a supported value
        [DataMember]
        private string version = string.Empty; // must be present and a supported value
        /// <summary>
        /// The key is the species name, so it can be used for modded species with the same name, they often have the same taming-data.
        /// The value is a dictionary with the food-name as key and TamingFood info as value.
        /// </summary>
        [DataMember]
        public Dictionary<string, TamingData> tamingFoodData = new Dictionary<string, TamingData>();

        /// <summary>
        /// Loads the default food data.
        /// </summary>
        /// <param name="serverMultipliers"></param>
        /// <returns></returns>
        public static bool TryLoadDefaultFoodData(out Dictionary<string, TamingData> tamingFoodData)
        {
            tamingFoodData = null;
            try
            {
                using (FileStream file = FileService.GetJsonFileStream(FileService.TamingFoodData))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TamingFoodData), new DataContractJsonSerializerSettings()
                    {
                        UseSimpleDictionaryFormat = true
                    });
                    var tmpV = (TamingFoodData)ser.ReadObject(file);
                    if (tmpV.format != Values.CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
                    tamingFoodData = tmpV.tamingFoodData;
                    return tamingFoodData != null;
                }
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"The default food data file {FileService.TamingFoodData} was not found.\n" +
                        "The taming info will be incomplete without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show($"File {FileService.TamingFoodData} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"File {FileService.TamingFoodData} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

    }
}

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
    class FoodDataCollection
    {
        [DataMember]
        private string format = string.Empty; // must be present and a supported value
        [DataMember]
        public Dictionary<string, TamingFood> foodData = new Dictionary<string, TamingFood>();

        /// <summary>
        /// Loads the default food data.
        /// </summary>
        /// <param name="serverMultipliers"></param>
        /// <returns></returns>
        public static bool TryLoadDefaultFoodData(out Dictionary<string, TamingFood> defaultFoodData)
        {
            defaultFoodData = new Dictionary<string, TamingFood>();
            try
            {
                using (FileStream file = FileService.GetJsonFileStream(FileService.DefaultFoodData))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FoodDataCollection), new DataContractJsonSerializerSettings()
                    {
                        UseSimpleDictionaryFormat = true
                    });
                    var tmpV = (FoodDataCollection)ser.ReadObject(file);
                    if (tmpV.format != Values.CURRENT_FORMAT_VERSION) throw new FormatException("Unhandled format version");
                    defaultFoodData = tmpV.foodData;
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"The default food data file {FileService.DefaultFoodData} was not found.\n" +
                        "The taming info will be incomplete without that file.\n\n" +
                        "Do you want to visit the releases page to redownload it?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show($"File {FileService.DefaultFoodData} is a format that is unsupported in this version of ARK Smart Breeding." +
                        "\n\nTry updating to a newer version.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"File {FileService.DefaultFoodData} couldn't be opened or read.\nErrormessage:\n\n" + e.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

    }
}

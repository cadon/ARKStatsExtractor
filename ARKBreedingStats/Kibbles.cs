using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using ARKBreedingStats.species;

namespace ARKBreedingStats
{
    [DataContract]
    public class Kibbles
    {
        [DataMember]
        private string ver = "0.0";

        [DataMember]
        public Dictionary<string, Kibble> kibble = new Dictionary<string, Kibble>();

        public Version version = new Version(0, 0);
        private static Kibbles _K;
        public static Kibbles K => _K ?? (_K = new Kibbles());

        public bool loadValues()
        {
            _K.version = new Version(0, 0);

            try
            {
                using (FileStream file = FileService.GetJsonFileStream(FileService.KibblesJson))
                {
                    DataContractJsonSerializerSettings s = new DataContractJsonSerializerSettings
                    {
                            UseSimpleDictionaryFormat = true
                    };
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Kibbles), s);

                    _K = (Kibbles)ser.ReadObject(file);

                    _K.version = new Version(_K.ver);
                }
                return true;
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show($"Kibble-File {FileService.KibblesJson} not found. " +
                        "This tool will not show kibble recipes without this file.\n\n" +
                        "Do you want to visit the homepage of the tool to redownload it?",
                        "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
            }
            catch (Exception e)
            {
                MessageBox.Show($"File {FileService.KibblesJson} couldn\'t be opened or read.\nErrormessage:\n\n{e.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }

}

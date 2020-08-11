using ARKBreedingStats.species;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Kibbles
    {
        [JsonProperty]
        private string ver = "0.0";

        [JsonProperty]
        public Dictionary<string, Kibble> kibble = new Dictionary<string, Kibble>();

        public Version version = new Version(0, 0);
        private static Kibbles _K;
        public static Kibbles K => _K ?? (_K = new Kibbles());

        public bool LoadValues()
        {
            _K.version = new Version(0, 0);

            string filePath = FileService.GetJsonPath(FileService.KibblesJson);
            if (!File.Exists(filePath))
            {
                if (MessageBox.Show($"Kibble-File {FileService.KibblesJson} not found. " +
                        "This tool will not show kibble recipes without this file.\n\n" +
                        "Do you want to visit the homepage of the tool to redownload it?",
                        $"{Loc.S("error")} - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(Updater.ReleasesUrl);
            }
            else if (FileService.LoadJsonFile(filePath, out Kibbles tempK, out string errorMessage))
            {
                _K = tempK;
                _K.version = new Version(_K.ver);
                return true;
            }
            else
            {
                MessageBox.Show($"File {FileService.KibblesJson} couldn\'t be opened or read.\nErrormessage:\n\n{errorMessage}", $"{Loc.S("error")} - {Utils.ApplicationNameVersion}",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }

}

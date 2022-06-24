using ARKBreedingStats.species;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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

        public bool LoadValues(out string errorMessage)
        {
            _K.version = new Version(0, 0);

            string filePath = FileService.GetJsonPath(FileService.KibblesJson);
            if (!File.Exists(filePath))
            {
                errorMessage = $"Kibble file {FileService.KibblesJson} not found. This tool will not show kibble recipes without this file.";
            }
            else if (FileService.LoadJsonFile(filePath, out Kibbles tempK, out string errorMessageFileLoading))
            {
                _K = tempK;
                _K.version = new Version(_K.ver);
                errorMessage = null;
                return true;
            }
            else
            {
                errorMessage = $"File {FileService.KibblesJson} couldn\'t be opened or read.\nErrormessage:\n\n{errorMessageFileLoading}";
            }
            return false;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ARKBreedingStats.ocr.PatternMatching;
using ARKBreedingStats.utils;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OcrTemplate
    {
        /// <summary>
        /// Format version of the OCR template.
        /// </summary>
        [JsonProperty]
        public Version Version;
        private const string CurrentVersion = "2.0";
        [JsonProperty]
        public string description;
        [JsonProperty]
        public double resize = 1;
        [JsonProperty]
        public int resolutionWidth;
        [JsonProperty]
        public int resolutionHeight;
        [JsonProperty]
        public int statDistance;
        [JsonProperty]
        public int guiZoom = 100;

        /// <summary>
        /// Contains all the patterns of all the recognizable strings.
        /// </summary>
        [JsonProperty]
        public RecognitionPatterns RecognitionPatterns;

        [JsonProperty]
        public Rectangle[] labelRectangles;


        //public Dictionary<string, int> labelNameIndices; // TODO remove
        //public List<string> labelNames; // TODO remove

        public List<List<int>> reducedIndices = new List<List<int>>(); // indices of letters for reduced set (only [0-9\.,/%:])

        //#region Old file format properties, kept for backwards compatibility

        //[JsonProperty]
        //public List<List<uint[]>> letterArrays;
        //[JsonProperty]
        //public List<List<char>> letters;

        //#endregion


        public OcrTemplate()
        {
            Version = new Version(CurrentVersion);
            InitializeOcrTemplate();
        }

        public void InitializeOcrTemplate()
        {
            if (RecognitionPatterns == null) RecognitionPatterns = new RecognitionPatterns();
            if (labelRectangles == null) labelRectangles = new Rectangle[Enum.GetValues(typeof(OcrLabels)).Length];

            var currentVersion = new Version(CurrentVersion);
            if (Version == null || Version.Major < currentVersion.Major)
            {
                MessageBoxes.ShowMessageBox("The version of this OCR config file is not supported.\nLoad a config file with a supported format.", icon: MessageBoxIcon.Error);
                Version = currentVersion;
            }
            RecognitionPatterns.Save += Save;
        }

        public static OcrTemplate LoadFile(string filePath)
        {
            OcrTemplate ocrConfig = null;

            // check if file exists
            if (!File.Exists(filePath))
            {
                MessageBoxes.ShowMessageBox($"OCR file '{filePath}' not found. OCR is not possible without the config-file.");
                return null;
            }

            if (FileService.LoadJsonFile(filePath, out OcrTemplate data, out var errorMessage, new Newtonsoft.Json.Converters.VersionConverter()))
            {
                ocrConfig = data;
                ocrConfig.InitializeOcrTemplate();
            }
            else
            {
                MessageBoxes.ShowMessageBox(errorMessage, "OCR config File couldn't be opened or read.");
            }

            return ocrConfig;
        }

        private void Save()
        {
            SaveFile(Properties.Settings.Default.ocrFile);
        }

        public bool SaveFile(string filePath)
        {
            if (FileService.SaveJsonFile(filePath, this, out var errorMessage, new Newtonsoft.Json.Converters.VersionConverter()))
                return true;

            MessageBoxes.ShowMessageBox(errorMessage, "OCR config file save error");
            return false;
        }

        /// <summary>
        /// Names of labels used in OCR.
        /// </summary>
        public enum OcrLabels
        {
            Health, Stamina, Oxygen, Food, Weight, MeleeDamage, MovementSpeed, Torpidity, Imprinting, Level, NameSpecies, Tribe, Owner
        }
    }
}

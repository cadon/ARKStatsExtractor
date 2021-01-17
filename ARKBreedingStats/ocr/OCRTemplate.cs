using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.ocr.PatternMatching;
using ARKBreedingStats.utils;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OCRTemplate
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


        public Dictionary<string, int> labelNameIndices; // TODO remove
        public List<string> labelNames; // TODO remove

        public List<List<int>> reducedIndices = new List<List<int>>(); // indices of letters for reduced set (only [0-9\.,/%:])

        //#region Old file format properties, kept for backwards compatibility

        //[JsonProperty]
        //public List<List<uint[]>> letterArrays;
        //[JsonProperty]
        //public List<List<char>> letters;

        //#endregion


        public OCRTemplate()
        {
            InitializeOcrTemplate();
        }

        public void InitializeOcrTemplate()
        {
            if (RecognitionPatterns == null) RecognitionPatterns = new RecognitionPatterns();
            if (labelRectangles == null) labelRectangles = new Rectangle[13]; // TODO use const

            var currentVersion = new Version(CurrentVersion);
            if (Version == null || Version.Major < currentVersion.Major)
            {
                MessageBoxes.ShowMessageBox("The version of this OCR-config file is not supported.\nThe config data needs to be created again.", icon: MessageBoxIcon.Error);
                Version = currentVersion;
            }
            InitializeLabelNames();
            RecognitionPatterns.Save += Save;
        }

        ///// <summary>
        ///// Converts the old ocr format to the new one
        ///// </summary>
        //private bool ConvertOldToNewFormat()
        //{
        //    if (letterArrays == null || letters == null) return false;

        //    int c = Math.Min(letterArrays.Count, letters.Count);
        //    for (int fontSizeIndex = 0; fontSizeIndex < c; fontSizeIndex++)
        //    {
        //        int fsC = Math.Min(letterArrays[fontSizeIndex].Count, letters[fontSizeIndex].Count);
        //        for (int i = 0; i < fsC; i++)
        //        {
        //            // convert old pattern format to new
        //            var oldPattern = letterArrays[fontSizeIndex][i];
        //            // the old pattern has its with stored in the index 0
        //            var width = (int)oldPattern[0];
        //            var height = oldPattern.Length - 1;
        //            if (width == 0 || height == 0) continue;

        //            var patternArray = new bool[width, height];
        //            for (int y = 0; y < height; y++)
        //                for (int x = 0; x < width; x++)
        //                {
        //                    patternArray[x, y] = ((oldPattern[y + 1] >> x) & 1) == 1;
        //                }

        //            var pattern = new Pattern(patternArray);

        //            var text = letters[fontSizeIndex][i].ToString();
        //            var existingText = RecognitionPatterns.Texts.FirstOrDefault(t => t.Text == text);
        //            if (existingText == null)
        //                RecognitionPatterns.Texts.Add(new TextData { Text = text, Patterns = new List<Pattern> { pattern } });
        //            else
        //                existingText.Patterns.Add(pattern);


        //            // TODO debug
        //            Debug.WriteLine("Text: " + text);
        //            Boolean2DimArrayConverter.ToDebugLog(pattern.Data);

        //            // todo remove duplicate patterns
        //        }
        //    }

        //    return true;
        //}

        private void InitializeLabelNames()
        {
            labelNames = new List<string> { "Health", "Stamina", "Oxygen", "Food", "Weight", "MeleeDamage", "MovementSpeed", "Torpor", "Imprinting", "Level", "NameSpecies", "Tribe", "Owner" };

            labelNameIndices = new Dictionary<string, int>();
            for (int i = 0; i < labelNames.Count; i++)
                labelNameIndices.Add(labelNames[i], i);
        }

        public static OCRTemplate LoadFile(string filePath)
        {
            OCRTemplate ocrConfig = null;

            // check if file exists
            if (!File.Exists(filePath))
            {
                MessageBoxes.ShowMessageBox($"OCR-File '{filePath}' not found. OCR is not possible without the config-file.");
                return null;
            }

            if (FileService.LoadJsonFile(filePath, out OCRTemplate data, out var errorMessage, new Newtonsoft.Json.Converters.VersionConverter()))
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
    }
}

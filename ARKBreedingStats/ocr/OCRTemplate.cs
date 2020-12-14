using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Json;
using ARKBreedingStats.ocr.PatternMatching;
using ARKBreedingStats.utils;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OCRTemplate
    {
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
        public int guiZoom = 100; // todo name / float? percentage? decimals?

        /// <summary>
        /// Contains all the patterns of all the recognizable strings.
        /// </summary>
        [JsonProperty]
        public RecognitionPatterns RecognitionPatterns;

        [JsonProperty]
        public Rectangle[] labelRectangles;


        public Dictionary<string, int> labelNameIndices; // TODO remove
        public List<string> labelNames; // TODO remove
        public List<List<char>> letters; // TODO remove
        public List<List<uint[]>> letterArrays; // TODO remove

        public List<List<int>> reducedIndices = new List<List<int>>(); // indices of letters for reduced set (only [0-9\.,/%:])

        public OCRTemplate()
        {
            InitializeOcrTemplate();
        }

        public void InitializeOcrTemplate()
        {
            if (RecognitionPatterns == null) RecognitionPatterns = new RecognitionPatterns();
            if (labelRectangles == null) labelRectangles = new Rectangle[13]; // TODO use const
            InitializeLabelNames();
            RecognitionPatterns.Save += Save;
        }

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

            if (FileService.LoadJsonFile(filePath, out OCRTemplate data, out var errorMessage))
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
            if (FileService.SaveJsonFile(filePath, this, out var errorMessage))
                return true;

            MessageBoxes.ShowMessageBox(errorMessage, "OCR config file save error");
            return false;
        }
    }
}

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
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RecognitionPatterns RecognitionPatterns;

        /// <summary>
        /// Coordinates of the rectangles to read. Kept for backwards compatibility, now uses LabelRectangles.
        /// </summary>
        [JsonProperty]
        public Rectangle[] labelRectangles;

        /// <summary>
        /// Coordinates of the rectangles to read. Multiple sets.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, Rectangle[]> LabelRectangles;

        /// <summary>
        /// Currently used label coordinates.
        /// </summary>
        public Rectangle[] UsedLabelRectangles;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SelectedLabelSetName;

        public const string DefaultLabelsName = "default";

        public List<List<int>> reducedIndices = new List<List<int>>(); // indices of letters for reduced set (only [0-9\.,/%:])

        public OcrTemplate()
        {
            Version = new Version(CurrentVersion);
            InitializeOcrTemplate();
        }

        public void InitializeOcrTemplate()
        {
            if (RecognitionPatterns == null) RecognitionPatterns = new RecognitionPatterns();
            if (LabelRectangles == null) LabelRectangles = new Dictionary<string, Rectangle[]>();

            if (labelRectangles != null)
            {
                if (!LabelRectangles.ContainsKey(DefaultLabelsName))
                    LabelRectangles[DefaultLabelsName] = labelRectangles;
                labelRectangles = null;
            }

            if (string.IsNullOrEmpty(SelectedLabelSetName) ||
                !LabelRectangles.TryGetValue(SelectedLabelSetName, out var currentRecSet))
            {
                if (!LabelRectangles.ContainsKey(DefaultLabelsName))
                    LabelRectangles[DefaultLabelsName] = EmptyLabelSet;
                UsedLabelRectangles = LabelRectangles[DefaultLabelsName];
                SelectedLabelSetName = DefaultLabelsName;
            }
            else
            {
                UsedLabelRectangles = currentRecSet;
            }

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

        private Rectangle[] EmptyLabelSet => new Rectangle[Enum.GetValues(typeof(OcrLabels)).Length];

        public void SetLabelSet(string setName)
        {
            if (string.IsNullOrEmpty(setName)
                || SelectedLabelSetName == setName) return;

            SelectedLabelSetName = setName;
            if (LabelRectangles.TryGetValue(setName, out var set))
            {
                UsedLabelRectangles = set;
            }
            else
            {
                UsedLabelRectangles = EmptyLabelSet;
                LabelRectangles[setName] = UsedLabelRectangles;
            }
        }

        public void DeleteLabelSet(string setName)
        {
            if (string.IsNullOrEmpty(setName)) return;

            LabelRectangles.Remove(setName);
            if (setName == SelectedLabelSetName)
            {
                SelectedLabelSetName = DefaultLabelsName;
                if (LabelRectangles.TryGetValue(SelectedLabelSetName, out var currentRecSet))
                {
                    UsedLabelRectangles = currentRecSet;
                }
                else
                {
                    UsedLabelRectangles = EmptyLabelSet;
                    LabelRectangles[SelectedLabelSetName] = UsedLabelRectangles;
                }
            }
        }

        public string NewLabelSet()
        {
            var newSetNameBase = "new label set";
            string newSetName = newSetNameBase;
            int suffix = 1;
            while (LabelRectangles.ContainsKey(newSetName))
            {
                newSetName = newSetNameBase + " " + (++suffix);
            }

            LabelRectangles[newSetName] = EmptyLabelSet;
            return newSetName;
        }

        public void DeleteCurrentLabelSet()
        {
            DeleteLabelSet(SelectedLabelSetName);
        }

        /// <summary>
        /// Changes the name of the currently selected set. Returns true if the name was changed.
        /// </summary>
        public bool LabelSetChangeName(string newName, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(newName)
                || newName == SelectedLabelSetName) return false;

            if (LabelRectangles.ContainsKey(newName))
            {
                errorMessage = "new name is already used by another set";
                return false;
            }

            LabelRectangles[newName] = UsedLabelRectangles;
            LabelRectangles.Remove(SelectedLabelSetName);
            SelectedLabelSetName = newName;
            return true;
        }
    }
}

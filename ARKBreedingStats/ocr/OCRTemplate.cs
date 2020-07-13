using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace ARKBreedingStats.ocr
{
    [DataContract]
    public class OCRTemplate
    {
        [DataMember]
        public string description = string.Empty;
        [DataMember]
        public double resize = 1;
        [DataMember]
        public int resolutionWidth;
        [DataMember]
        public int resolutionHeight;
        [DataMember]
        public int statDistance;
        [DataMember]
        public int guiZoom = 100; // todo name / float? percentage? decimals?
        [DataMember]
        public List<int> fontSizes = new List<int>();
        [DataMember]
        public List<List<uint[]>> letterArrays = new List<List<uint[]>>();
        [DataMember]
        public List<List<char>> letters = new List<List<char>>();
        [DataMember]
        public List<Rectangle> labelRectangles = new List<Rectangle>();
        public Dictionary<string, int> labelNameIndices = new Dictionary<string, int>();
        public List<string> labelNames = new List<string>();

        public List<List<int>> reducedIndices = new List<List<int>>(); // indices of letters for reduced set (only [0-9\.,/%:])

        public void init()
        {
            initLabelNames();
            initReducedIndices();
        }

        private void initLabelNames()
        {
            labelNames = new List<string> { "Health", "Stamina", "Oxygen", "Food", "Weight", "MeleeDamage", "MovementSpeed", "Torpor", "Imprinting", "Level", "NameSpecies", "Tribe", "Owner" };

            labelNameIndices = new Dictionary<string, int>();
            for (int i = 0; i < labelNames.Count; i++)
                labelNameIndices.Add(labelNames[i], i);
        }

        private void initReducedIndices()
        {
            reducedIndices = new List<List<int>>();
            const string reducedChars = ":0123456789.,%/";
            for (int o = 0; o < fontSizes.Count; o++)
            {
                reducedIndices.Add(new List<int>());
                for (int c = 0; c < letters[o].Count; c++)
                {
                    if (reducedChars.IndexOf(letters[o][c]) != -1)
                        reducedIndices[o].Add(c);
                }
            }
        }

        public int fontSizeIndex(int fontSize, bool createIfNotExisting = false)
        {
            if (fontSizes.IndexOf(fontSize) == -1 && createIfNotExisting)
            {
                fontSizes.Add(fontSize);
                letterArrays.Add(new List<uint[]>());
                letters.Add(new List<char>());
                reducedIndices.Add(new List<int>());
            }
            return fontSizes.IndexOf(fontSize);
        }

        public OCRTemplate LoadFile(string filename)
        {
            OCRTemplate ocrConfig = null;

            // check if file exists
            if (!File.Exists(filename))
            {
                MessageBox.Show($"OCR-File '{filename}' not found. OCR is not possible without the config-file.", $"{Loc.S("error")} - {Utils.ApplicationNameVersion}",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            using (FileStream file = File.OpenRead(filename))
            {
                try
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(OCRTemplate));
                    ocrConfig = (OCRTemplate)ser.ReadObject(file);
                    ocrConfig.init();
                }
                catch (Exception e)
                {
                    MessageBox.Show("File Couldn't be opened or read.\nErrormessage:\n\n" + e.Message, $"{Loc.S("error")} - {Utils.ApplicationNameVersion}",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return ocrConfig;
        }

        public bool SaveFile(string filename)
        {
            try
            {
                using (FileStream file = File.Create(filename))
                {
                    DataContractJsonSerializer writer = new DataContractJsonSerializer(typeof(OCRTemplate));
                    writer.WriteObject(file, ArkOCR.OCR.ocrConfig);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + ex.Message, $"Serialization-Error - {Utils.ApplicationNameVersion}",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }
}

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
        public string description = "";
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
            labelNames = new List<string>() { "Health", "Stamina", "Oxygen", "Food", "Weight", "MeleeDamage", "MovementSpeed", "Torpor", "Imprinting", "Level", "NameSpecies", "Tribe", "Owner" };

            labelNameIndices = new Dictionary<string, int>();
            for (int i = 0; i < labelNames.Count; i++)
                labelNameIndices.Add(labelNames[i], i);

            //if (labelRecs.Count > 0)
            //{
            //    Rectangle r = new Rectangle(labelRecs[0].Left, labelRecs[0].Top, labelRecs[0].Width, labelRecs[0].Height);
            //    for (int i = 0; i < 9 && i < labelRecs.Count; i++)
            //    {
            //        labelRectangles.Add(new Rectangle(r.Left, r.Top, r.Width, r.Height));
            //        r.Offset(0, statDistance);
            //    }

            //    for (int i = 1; i < labelRecs.Count && i + 9 < labelNames.Count; i++)
            //        labelRectangles.Add(labelRecs[i]);
            //}


            //labelNameIndices.Add("Health", 0);
            //labelNameIndices.Add("Stamina", 1);
            //labelNameIndices.Add("Oxygen", 2);
            //labelNameIndices.Add("Food", 3);
            //labelNameIndices.Add("Weight", 4);
            //labelNameIndices.Add("MeleeDamage", 5);
            //labelNameIndices.Add("MovementSpeed", 6);
            //labelNameIndices.Add("Torpor", 7);
            //labelNameIndices.Add("Imprinting", 8);
            //labelNameIndices.Add("Level", 9);
            //labelNameIndices.Add("NameSpecies", 10);
            //labelNameIndices.Add("Tribe", 11);
            //labelNameIndices.Add("Owner", 12);
        }

        private void initReducedIndices()
        {
            reducedIndices = new List<List<int>>();
            string reducedChars = ":0123456789.,%/";
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

        public OCRTemplate loadFile(string filename)
        {
            OCRTemplate ocrConfig = null;

            // check if file exists
            if (!File.Exists(filename))
            {
                MessageBox.Show("OCR-File '" + filename + "' not found. OCR is not possible without the config-file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(OCRTemplate));
            System.IO.FileStream file = System.IO.File.OpenRead(filename);

            try
            {
                ocrConfig = (OCRTemplate)ser.ReadObject(file);
                ocrConfig.init();
                Properties.Settings.Default.ocrFile = filename;
            }
            catch (Exception e)
            {
                MessageBox.Show("File Couldn't be opened or read.\nErrormessage:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            file.Close();

            return ocrConfig;
        }

        public bool saveFile(string filename)
        {
            DataContractJsonSerializer writer = new DataContractJsonSerializer(typeof(OCRTemplate));
            try
            {
                System.IO.FileStream file = System.IO.File.Create(filename);
                writer.WriteObject(file, ArkOCR.OCR.ocrConfig);
                file.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during serialization.\nErrormessage:\n\n" + ex.Message, "Serialization-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }
}

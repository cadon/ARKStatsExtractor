using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.ocr
{
    public partial class OCRControl : UserControl
    {
        public delegate void IntEventHandler(int value);
        public event IntEventHandler updateWhiteThreshold;
        public event DragEventHandler dragEnter;
        public event DragEventHandler dragDrop;
        public FlowLayoutPanel debugPanel;
        public TextBox output;
        private List<uint[]> recognizedLetterArrays;
        private List<char> recognizedLetters;
        private List<int> recognizedFontSizes;
        private Bitmap screenshot;
        private bool updateDrawing, ignoreValueChange;

        public OCRControl()
        {
            InitializeComponent();
            debugPanel = OCRDebugLayoutPanel;
            output = txtOCROutput;
            ocrLetterEditTemplate.drawingEnabled = true;
            recognizedLetterArrays = new List<uint[]>();
            recognizedLetters = new List<char>();
            recognizedFontSizes = new List<int>();
            updateDrawing = true;
            ignoreValueChange = false;
        }

        public void initLabelEntries()
        {
            listBoxLabelRectangles.Items.Clear();
            foreach (KeyValuePair<string, int> rn in ArkOCR.OCR.ocrConfig.labelNameIndices)
                listBoxLabelRectangles.Items.Add(rn.Key);
        }

        private void nudWhiteTreshold_ValueChanged(object sender, EventArgs e)
        {
            updateWhiteThreshold?.Invoke((int)nudWhiteTreshold.Value);
        }

        private void OCRDebugLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            dragEnter?.Invoke(sender, e);
        }

        private void OCRDebugLayoutPanel_DragDrop(object sender, DragEventArgs e)
        {
            dragDrop?.Invoke(sender, e);
        }

        private void listBoxRecognized_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = listBoxRecognized.SelectedIndex;
            if (i >= 0 && i < recognizedLetters.Count)
            {
                textBoxTemplate.Text = recognizedLetters[i].ToString();
                nudFontSize.Value = recognizedFontSizes[i];
                ocrLetterEditRecognized.LetterArray = recognizedLetterArrays[i];
                ocrLetterEditTemplate.LetterArrayComparing = recognizedLetterArrays[i];
                showMatch();
                textBoxTemplate.Focus();
                textBoxTemplate.SelectAll();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ocrLetterEditTemplate.LetterArray = ocrLetterEditRecognized.LetterArray;
        }

        public void ClearLists()
        {
            recognizedLetterArrays.Clear();
            recognizedLetters.Clear();
            recognizedFontSizes.Clear();
            listBoxRecognized.Items.Clear();
        }

        public void addLetterToRecognized(uint[] letterArray, char ch, int fontSize)
        {
            recognizedLetterArrays.Add(letterArray);
            recognizedLetters.Add(ch);
            recognizedFontSizes.Add(fontSize);
            listBoxRecognized.Items.Add(ch);
        }

        private void textBoxTemplate_TextChanged(object sender, EventArgs e)
        {
            if (textBoxTemplate.Text.Length > 0)
            {
                textBoxTemplate.SelectAll();
                loadTemplateLetter();
            }
        }

        private void nudFontSize_ValueChanged(object sender, EventArgs e)
        {
            loadTemplateLetter();
        }

        private void loadTemplateLetter()
        {
            ocrLetterEditTemplate.Clear();
            if (textBoxTemplate.Text.Length > 0)
            {
                char c = textBoxTemplate.Text[0];
                int ocrIndex = ArkOCR.OCR.ocrConfig.fontSizeIndex((int)nudFontSize.Value);
                if (ocrIndex != -1)
                {
                    int lI = ArkOCR.OCR.ocrConfig.letters[ocrIndex].IndexOf(c);
                    if (lI != -1)
                        ocrLetterEditTemplate.LetterArray = ArkOCR.OCR.ocrConfig.letterArrays[ocrIndex][lI];
                }
            }
            showMatch();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveTemplate(textBoxTemplate.Text[0], ocrLetterEditTemplate.LetterArray);
        }

        private void buttonSaveAsTemplate_Click(object sender, EventArgs e)
        {
            saveTemplate(textBoxTemplate.Text[0], ocrLetterEditRecognized.LetterArray);
        }

        private void saveTemplate(char c, uint[] letterArray)
        {
            int ocrIndex = ArkOCR.OCR.ocrConfig.fontSizeIndex((int)nudFontSize.Value, true);
            int lI = ArkOCR.OCR.ocrConfig.letters[ocrIndex].IndexOf(c);
            if (lI == -1)
            {
                ArkOCR.OCR.ocrConfig.letters[ocrIndex].Add(c);
                ArkOCR.OCR.ocrConfig.letterArrays[ocrIndex].Add(letterArray);
            }
            else
                ArkOCR.OCR.ocrConfig.letterArrays[ocrIndex][lI] = letterArray;
            loadTemplateLetter();
        }

        private void textBoxTemplate_Enter(object sender, EventArgs e)
        {
            textBoxTemplate.SelectAll();
        }

        private void showMatch()
        {
            float match = 0;
            uint HammingDiff = 0;
            int maxTestRange = Math.Min(ocrLetterEditTemplate.LetterArray.Length, ocrLetterEditRecognized.LetterArray.Length);
            for (int y = 1; y < maxTestRange; y++)
                HammingDiff += ocr.HammingWeight.HWeight(ocrLetterEditTemplate.LetterArray[y] ^ ocrLetterEditRecognized.LetterArray[y]);
            if (ocrLetterEditTemplate.LetterArray.Length > ocrLetterEditRecognized.LetterArray.Length)
            {
                for (int y = maxTestRange; y < ocrLetterEditTemplate.LetterArray.Length; y++)
                    HammingDiff += ocr.HammingWeight.HWeight(ocrLetterEditTemplate.LetterArray[y]);
            }
            else if (ocrLetterEditTemplate.LetterArray.Length < ocrLetterEditRecognized.LetterArray.Length)
            {
                for (int y = maxTestRange; y < ocrLetterEditRecognized.LetterArray.Length; y++)
                    HammingDiff += ocr.HammingWeight.HWeight(ocrLetterEditRecognized.LetterArray[y]);
            }
            long total = Math.Max(ocrLetterEditTemplate.LetterArray.Length, ocrLetterEditRecognized.LetterArray.Length) * Math.Max(ocrLetterEditTemplate.LetterArray[0], ocrLetterEditRecognized.LetterArray[0]);
            if (total > 10)
                match = ((float)(total - HammingDiff) / total);
            else
                match = 1 - HammingDiff / 10f;

            labelMatching.Text = "matching: " + Math.Round(match * 100, 1) + " %";
        }

        internal void setWhiteThreshold(int oCRWhiteThreshold)
        {
            nudWhiteTreshold.Value = oCRWhiteThreshold;
        }

        private void listBoxLabelRectangles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = listBoxLabelRectangles.SelectedIndex;
            if (i >= 0)
            {
                setLabelControls(i);
                redrawLabelRectangles(i);
            }
        }

        private void setLabelControls(int rectangleIndex)
        {
            if (rectangleIndex >= 0 && rectangleIndex < ArkOCR.OCR.ocrConfig.labelRectangles.Count)
            {
                Rectangle rec = ArkOCR.OCR.ocrConfig.labelRectangles[rectangleIndex];
                ignoreValueChange = true;
                nudX.Value = rec.X;
                nudY.Value = rec.Y;
                nudWidth.Value = rec.Width;
                nudHeight.Value = rec.Height;
                nudWidthL.Value = rec.Width;
                nudHeightT.Value = rec.Height;
                ignoreValueChange = false;
            }
        }

        private void redrawLabelRectangles(int hightlightIndex)
        {
            if (OCRDebugLayoutPanel.Controls.Count > 0 && OCRDebugLayoutPanel.Controls[OCRDebugLayoutPanel.Controls.Count - 1] is PictureBox && screenshot != null)
            {
                PictureBox p = (PictureBox)OCRDebugLayoutPanel.Controls[OCRDebugLayoutPanel.Controls.Count - 1];
                Bitmap b = new Bitmap(screenshot);
                using (Graphics g = Graphics.FromImage(b))
                {
                    Pen penW = new Pen(Color.White, 2);
                    Pen penY = new Pen(Color.Yellow, 2);
                    Pen penB = new Pen(Color.Black, 2);
                    penW.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    penY.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    penB.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    for (int r = 0; r < ArkOCR.OCR.ocrConfig.labelRectangles.Count; r++)
                    {
                        Rectangle rec = ArkOCR.OCR.ocrConfig.labelRectangles[r];
                        if (r == hightlightIndex)
                        {
                            rec.Inflate(2, 2);
                            g.DrawRectangle(penY, rec);
                            rec.Inflate(2, 2);
                            g.DrawRectangle(penB, rec);
                        }
                        else
                        {
                            rec.Inflate(2, 2);
                            g.DrawRectangle(penW, rec);
                            rec.Inflate(2, 2);
                            g.DrawRectangle(penB, rec);
                        }
                    }
                }
                p.Image = b;
            }
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreValueChange)
            {
                updateRectangle();
            }
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreValueChange)
            {
                updateRectangle();
            }
        }

        private void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreValueChange)
            {
                updateDrawing = false;
                nudWidthL.Value = nudWidth.Value;
                updateDrawing = true;
                updateRectangle();
            }
        }

        private void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreValueChange)
            {
                updateDrawing = false;
                nudHeightT.Value = nudHeight.Value;
                updateDrawing = true;
                updateRectangle();
            }
        }

        private void nudWidthL_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreValueChange)
            {
                updateDrawing = false;
                nudX.Value = nudX.Value + nudWidth.Value - nudWidthL.Value;
                updateDrawing = true;
                nudWidth.Value = nudWidthL.Value;
            }
        }

        private void nudHeightT_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreValueChange)
            {
                updateDrawing = false;
                nudY.Value = nudY.Value + nudHeight.Value - nudHeightT.Value;
                updateDrawing = true;
                nudHeight.Value = nudHeightT.Value;
            }
        }

        private void updateRectangle()
        {
            if (updateDrawing)
            {
                int i = listBoxLabelRectangles.SelectedIndex;
                if (i >= 0 && i < ArkOCR.OCR.ocrConfig.labelRectangles.Count)
                {
                    // set all stat-labels if wanted
                    if (chkbSetAllStatLabels.Checked && i < 9)
                    {
                        for (int s = 0; s < 9; s++)
                            if (i != s)
                                ArkOCR.OCR.ocrConfig.labelRectangles[s] = new Rectangle((int)nudX.Value, ArkOCR.OCR.ocrConfig.labelRectangles[s].Y, (int)nudWidth.Value, (int)nudHeight.Value);
                    }
                    ArkOCR.OCR.ocrConfig.labelRectangles[i] = new Rectangle((int)nudX.Value, (int)nudY.Value, (int)nudWidth.Value, (int)nudHeight.Value);
                    redrawLabelRectangles(i);
                }
            }
        }

        private void buttonSaveOCR_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "OCR configuration File (*.json)|*.json";
            dlg.InitialDirectory = Application.StartupPath + "/json";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ArkOCR.OCR.ocrConfig.saveFile(dlg.FileName);
            }
        }

        private void buttonLoadOCRTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "OCR configuration File (*.json)|*.json";
            dlg.InitialDirectory = Application.StartupPath + "/json";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OCRTemplate t = ArkOCR.OCR.ocrConfig.loadFile(dlg.FileName);
                if (t != null)
                {
                    ArkOCR.OCR.ocrConfig = t;
                    labelOCRFile.Text = dlg.FileName;
                }
            }
            initLabelEntries();
        }

        private void buttonLoadCalibrationImage_Click(object sender, EventArgs e)
        {
            ArkOCR.OCR.calibrateFromFontFile((int)nudFontSizeCalibration.Value, textBoxCalibrationText.Text);
        }

        internal void setScreenshot(Bitmap screenshotbmp)
        {
            screenshot = screenshotbmp;
        }

        public void setOCRFile(string ocrFile)
        {
            labelOCRFile.Text = ocrFile;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

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
        private CancellationTokenSource cancelSource;

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

            // resolution
            nudResolutionWidth.Value = ArkOCR.OCR.ocrConfig.resolutionWidth;
            nudResolutionHeight.Value = ArkOCR.OCR.ocrConfig.resolutionHeight;
        }

        private void nudWhiteTreshold_ValueChanged(object sender, EventArgs e)
        {
            updateWhiteThreshold?.Invoke((int)nudWhiteTreshold.Value);
            showPreviewWhiteThreshold((int)nudWhiteTreshold.Value);
        }

        private void nudWhiteTreshold_Leave(object sender, EventArgs e)
        {
            showPreviewWhiteThreshold(-1);
        }

        /// <summary>
        /// Shows a preview of the white-threshold if a screenshot is displayed
        /// </summary>
        /// <param name="value">The white-threshold. -1 disables the preview</param>
        private async void showPreviewWhiteThreshold(int value)
        {
            cancelSource?.Cancel();
            try
            {
                using (cancelSource = new CancellationTokenSource())
                {
                    await Task.Delay(400, cancelSource.Token); // update preview only each interval
                    redrawScreenshot(-1, false, value);
                }
            }
            catch (TaskCanceledException)
            {
                return;
            }
            cancelSource = null;
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

                // for debugging, can be commented out for release
                //ArkOCR.saveLetterArrayToFile(ocrLetterEditRecognized.LetterArray, @"D:\Temp\array" + DateTime.Now.ToString("HHmmss\\-fffffff\\-") + "_recognized.png");
                //ArkOCR.saveLetterArrayToFile(ocrLetterEditTemplate.LetterArray, @"D:\Temp\array" + DateTime.Now.ToString("HHmmss\\-fffffff\\-") + "_template.png");
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
            //ocrLetterEditTemplate.Clear();
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


        private void btnSaveTemplate_Click(object sender, EventArgs e)
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
            float match;
            int offset;
            ArkOCR.letterMatch(ocrLetterEditRecognized.LetterArray, ocrLetterEditTemplate.LetterArray, out match, out offset);
            ocrLetterEditTemplate.recognizedOffset = offset;

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
                redrawScreenshot(i);
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

        /// <summary>
        /// Redraws the screenshot
        /// </summary>
        /// <param name="hightlightIndex">which of the labels should be highlighted</param>
        /// <param name="showLabels">show labels</param>
        /// <param name="whiteThreshold">preview of white-Threshold. -1 to disable</param>
        private void redrawScreenshot(int hightlightIndex, bool showLabels = true, int whiteThreshold = -1)
        {
            if (OCRDebugLayoutPanel.Controls.Count > 0 && OCRDebugLayoutPanel.Controls[OCRDebugLayoutPanel.Controls.Count - 1] is PictureBox && screenshot != null)
            {
                PictureBox p = (PictureBox)OCRDebugLayoutPanel.Controls[OCRDebugLayoutPanel.Controls.Count - 1];
                Bitmap b = new Bitmap((whiteThreshold >= 0 ? ArkOCR.removePixelsUnderThreshold(ArkOCR.GetGreyScale(screenshot), whiteThreshold, true) : screenshot));
                using (Graphics g = Graphics.FromImage(b))
                {
                    if (showLabels)
                    {
                        using (Pen penW = new Pen(Color.White, 2))
                        using (Pen penY = new Pen(Color.Yellow, 2))
                        using (Pen penB = new Pen(Color.Black, 2))
                        {
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
                    }
                }
                Bitmap disp = (Bitmap)p.Image; // take pointer to old image to dispose it soon
                p.Image = b;
                if (disp != null && disp != screenshot)
                    disp.Dispose();
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
                    redrawScreenshot(i);
                }
            }
        }

        private void btnSaveOCRconfig_Click(object sender, EventArgs e)
        {
            ArkOCR.OCR.ocrConfig.saveFile(Properties.Settings.Default.ocrFile);
            updateOCRLabel();
        }

        private void btnSaveOCRConfigAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "OCR configuration File (*.json)|*.json";
            dlg.InitialDirectory = Application.StartupPath + "/json";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ArkOCR.OCR.ocrConfig.saveFile(dlg.FileName);
                loadOCRTemplate(dlg.FileName);
            }
        }

        private void buttonLoadOCRTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "OCR configuration File (*.json)|*.json";
            dlg.InitialDirectory = Application.StartupPath + "\\json";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadOCRTemplate(dlg.FileName);
            }
        }

        public void loadOCRTemplate(string fileName)
        {
            OCRTemplate t = ArkOCR.OCR.ocrConfig.loadFile(fileName);
            if (t != null)
            {
                ArkOCR.OCR.ocrConfig = t;
                updateOCRLabel();
                updateOCRFontSizes();
                initLabelEntries();
                nudResizing.Value = ArkOCR.OCR.ocrConfig.resize == 0 ? 1 : (decimal)ArkOCR.OCR.ocrConfig.resize;
            }
        }

        private void updateOCRLabel()
        {
            if (ArkOCR.OCR.ocrConfig != null)
                labelOCRFile.Text = Properties.Settings.Default.ocrFile + "\n\nResolution: " + ArkOCR.OCR.ocrConfig.resolutionWidth + " × " + ArkOCR.OCR.ocrConfig.resolutionHeight + "\nUI-Scaling: " + ArkOCR.OCR.ocrConfig.guiZoom
                    + "\nScreenshot-Resizing-Factor: " + ArkOCR.OCR.ocrConfig.resize;
        }

        private void buttonLoadCalibrationImage_Click(object sender, EventArgs e)
        {
            if (ArkOCR.OCR.calibrateFromFontFile((int)nudFontSizeCalibration.Value, textBoxCalibrationText.Text))
                updateOCRFontSizes();
        }

        internal void setScreenshot(Bitmap screenshotbmp)
        {
            screenshot?.Dispose();
            screenshot = screenshotbmp;
            OCRDebugLayoutPanel.AutoScrollPosition = new Point(screenshot.Width / 3, screenshot.Height / 4);
        }

        private void cbEnableOutput_CheckedChanged(object sender, EventArgs e)
        {
            ArkOCR.OCR.enableOutput = cbEnableOutput.Checked;
        }

        private void nudResolutionWidth_ValueChanged(object sender, EventArgs e)
        {
            ArkOCR.OCR.ocrConfig.resolutionWidth = (int)nudResolutionWidth.Value;
        }

        private void nudResolutionHeight_ValueChanged(object sender, EventArgs e)
        {
            ArkOCR.OCR.ocrConfig.resolutionHeight = (int)nudResolutionHeight.Value;
        }

        private void buttonGetResFromScreenshot_Click(object sender, EventArgs e)
        {
            if (screenshot != null)
            {
                nudResolutionWidth.Value = screenshot.Width;
                nudResolutionHeight.Value = screenshot.Height;
            }
        }

        private void btnDeleteFontSize_Click(object sender, EventArgs e)
        {
            int fontSize = 0;
            if (Int32.TryParse(cbbFontSizeDelete.SelectedItem.ToString(), out fontSize)
                && MessageBox.Show("Delete all character-templates for the font-size " + fontSize + "?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int ocrIndex = ArkOCR.OCR.ocrConfig.fontSizes.IndexOf(fontSize);
                if (ocrIndex >= 0)
                {
                    ArkOCR.OCR.ocrConfig.fontSizes.RemoveAt(ocrIndex);
                    ArkOCR.OCR.ocrConfig.letterArrays.RemoveAt(ocrIndex);
                    ArkOCR.OCR.ocrConfig.letters.RemoveAt(ocrIndex);
                    updateOCRFontSizes();
                }
            }
        }

        private void nudResizing_ValueChanged(object sender, EventArgs e)
        {
            ArkOCR.OCR.ocrConfig.resize = (double)nudResizing.Value;
            int resizedHeight = (int)(ArkOCR.OCR.ocrConfig.resize * ArkOCR.OCR.ocrConfig.resolutionHeight);
            lbResizeResult.Text = ArkOCR.OCR.ocrConfig.resolutionWidth + " × " + ArkOCR.OCR.ocrConfig.resolutionHeight + " -> " + (int)(ArkOCR.OCR.ocrConfig.resize * ArkOCR.OCR.ocrConfig.resolutionWidth) + " × " + resizedHeight;
            string infoText = "\nKeep in mind, any change of the resizing needs new character templates to be made";
            if (resizedHeight < 1080)
                lbResizeResult.Text += "\nThe size is probably too small for good results, you can try to increse the factor." + infoText;
            if (resizedHeight > 1800) // TODO correct value?
                lbResizeResult.Text += "\nThe size is probably too large for the character-templates (max-size is 31px), you can try to decrese the factor." + infoText;
        }

        private void updateOCRFontSizes()
        {
            cbbFontSizeDelete.Items.Clear();
            foreach (int s in ArkOCR.OCR.ocrConfig.fontSizes)
            {
                cbbFontSizeDelete.Items.Add(s.ToString());
            }
        }
    }
}

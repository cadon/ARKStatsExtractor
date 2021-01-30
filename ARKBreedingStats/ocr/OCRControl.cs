using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using ARKBreedingStats.ocr.PatternMatching;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.ocr
{
    public partial class OCRControl : UserControl
    {
        public event Action<int> UpdateWhiteThreshold;
        public event Action<string, bool> DoOcr;
        public readonly FlowLayoutPanel debugPanel;
        public readonly TextBox output;
        private readonly List<Pattern> _recognizedPatterns = new List<Pattern>();
        private Bitmap _screenshot;
        private bool _updateDrawing = true;
        private bool _ignoreValueChange;
        private readonly Debouncer _redrawingDebouncer = new Debouncer();
        private string _fontFilePath;

        public OCRControl()
        {
            InitializeComponent();
            debugPanel = OCRDebugLayoutPanel;
            output = txtOCROutput;
            ocrLetterEditTemplate.drawingEnabled = true;
        }

        public void Initialize()
        {
            SetWhiteThreshold(Properties.Settings.Default.OCRWhiteThreshold);
            if (!LoadAndInitializeOcrTemplate(Properties.Settings.Default.ocrFile))
                Properties.Settings.Default.ocrFile = null;
        }

        private void InitLabelEntries()
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
            UpdateWhiteThreshold?.Invoke((int)nudWhiteTreshold.Value);
            ShowPreviewWhiteThreshold((int)nudWhiteTreshold.Value);
        }

        private void nudWhiteTreshold_Leave(object sender, EventArgs e)
        {
            ShowPreviewWhiteThreshold(-1);
        }

        /// <summary>
        /// Shows a preview of the white-threshold if a screenshot is displayed
        /// </summary>
        /// <param name="value">The white-threshold. -1 disables the preview</param>
        private async void ShowPreviewWhiteThreshold(int value)
        {
            _redrawingDebouncer.Debounce(500, RedrawScreenshot, Dispatcher.CurrentDispatcher, (-1, false, value));
        }

        private void OCRDebugLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void OCRDebugLayoutPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any()))
                return;

            cbEnableOutput.Checked = true;
            try
            {
                var bmp = new Bitmap(files[0]);

                if (bmp.Width == nudResolutionWidth.Value
                    && bmp.Height == nudResolutionHeight.Value)
                {
                    bmp.Dispose();
                    DoOcr?.Invoke(files[0], true);
                }
                else
                {
                    AddBitmapToDebug(bmp);
                    SetScreenshot(bmp);
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private void LoadTemplateLetter()
        {
            var text = textBoxTemplate.Text;
            if (string.IsNullOrEmpty(text)) return;

            _selectedTextData = ArkOCR.OCR.ocrConfig.RecognitionPatterns.Texts.FirstOrDefault(t => t.Text == text);
            ListBoxPatternsOfString.Items.Clear();

            if (_selectedTextData == null) return;
            int patternCount = _selectedTextData.Patterns.Count;
            ListBoxPatternsOfString.Items.AddRange(Enumerable.Range(1, patternCount).Select(ii => ii.ToString()).ToArray());
            if (patternCount > 0)
                ListBoxPatternsOfString.SelectedIndex = 0;
        }

        private void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            SaveTemplate(textBoxTemplate.Text, ocrLetterEditTemplate.PatternDisplay);
        }

        private void buttonSaveAsTemplate_Click(object sender, EventArgs e)
        {
            SaveTemplate(textBoxTemplate.Text, ocrLetterEditRecognized.PatternDisplay);
        }

        /// <summary>
        /// Saves the pattern.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="letterArray"></param>
        private void SaveTemplate(string text, Pattern pattern)
        {
            var existingTemplate = ArkOCR.OCR.ocrConfig.RecognitionPatterns.Texts.FirstOrDefault(t => t.Text == text);
            if (existingTemplate == null)
            {
                ArkOCR.OCR.ocrConfig.RecognitionPatterns.Texts.Add(new TextData { Text = text, Patterns = new List<Pattern> { pattern } });
            }
            else
            {
                existingTemplate.Patterns.Add(pattern);
            }

            ArkOCR.OCR.ocrConfig.SaveFile(Properties.Settings.Default.ocrFile);

            LoadTemplateLetter();
        }

        private void textBoxTemplate_Enter(object sender, EventArgs e)
        {
            textBoxTemplate.SelectAll();
        }

        private void ShowMatch()
        {
            RecognitionPatterns.PatternMatch(ocrLetterEditTemplate.PatternDisplay.Data, ocrLetterEditRecognized.PatternDisplay?.Data, out float match, out int offset);
            ocrLetterEditTemplate.recognizedOffset = offset;

            labelMatching.Text = $"matching: {Math.Round(match * 100, 1)} %";
        }

        internal void SetWhiteThreshold(int ocrWhiteThreshold)
        {
            nudWhiteTreshold.Value = ocrWhiteThreshold;
        }

        private void listBoxLabelRectangles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = listBoxLabelRectangles.SelectedIndex;
            if (i >= 0)
            {
                SetLabelControls(i);
                RedrawScreenshot(i);
            }
        }

        private void SetLabelControls(int rectangleIndex)
        {
            if (rectangleIndex >= 0 && rectangleIndex < ArkOCR.OCR.ocrConfig.labelRectangles.Length)
            {
                Rectangle rec = ArkOCR.OCR.ocrConfig.labelRectangles[rectangleIndex];
                _ignoreValueChange = true;
                nudX.Value = rec.X;
                nudY.Value = rec.Y;
                nudWidth.Value = rec.Width;
                nudHeight.Value = rec.Height;
                nudWidthL.Value = rec.Width;
                nudHeightT.Value = rec.Height;
                _ignoreValueChange = false;
            }
        }

        internal void AddBitmapToDebug(Bitmap bmp)
        {
            PictureBox b = new PictureBox { SizeMode = PictureBoxSizeMode.AutoSize, Image = bmp };
            OCRDebugLayoutPanel.Controls.Add(b);
            OCRDebugLayoutPanel.Controls.SetChildIndex(b, 0);
            b.Click += PictureBoxClicked;
        }

        private void RedrawScreenshot(object ob)
        {
            var args = ((int, bool, int))ob;
            RedrawScreenshot(args.Item1, args.Item2, args.Item3);
        }

        /// <summary>
        /// Redraws the screenshot
        /// </summary>
        /// <param name="hightlightIndex">which of the labels should be highlighted</param>
        /// <param name="showLabels">show labels</param>
        /// <param name="whiteThreshold">preview of white-Threshold. -1 to disable</param>
        private void RedrawScreenshot(int hightlightIndex, bool showLabels = true, int whiteThreshold = -1)
        {
            if (OCRDebugLayoutPanel.Controls.Count <= 0 ||
                !(OCRDebugLayoutPanel.Controls[OCRDebugLayoutPanel.Controls.Count - 1] is PictureBox) ||
                _screenshot == null) return;

            PictureBox p = (PictureBox)OCRDebugLayoutPanel.Controls[OCRDebugLayoutPanel.Controls.Count - 1];
            Bitmap b = new Bitmap((whiteThreshold >= 0 ? ArkOCR.removePixelsUnderThreshold(ArkOCR.GetGreyScale(_screenshot), whiteThreshold, true) : _screenshot));
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
                        for (int r = 0; r < ArkOCR.OCR.ocrConfig.labelRectangles.Length; r++)
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
            if (disp != null && disp != _screenshot)
                disp.Dispose();
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            if (!_ignoreValueChange)
            {
                UpdateRectangle();
            }
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            if (!_ignoreValueChange)
            {
                UpdateRectangle();
            }
        }

        private void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            if (!_ignoreValueChange)
            {
                _updateDrawing = false;
                nudWidthL.Value = nudWidth.Value;
                _updateDrawing = true;
                UpdateRectangle();
            }
        }

        private void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            if (!_ignoreValueChange)
            {
                _updateDrawing = false;
                nudHeightT.Value = nudHeight.Value;
                _updateDrawing = true;
                UpdateRectangle();
            }
        }

        private void nudWidthL_ValueChanged(object sender, EventArgs e)
        {
            if (!_ignoreValueChange)
            {
                _updateDrawing = false;
                nudX.Value = nudX.Value + nudWidth.Value - nudWidthL.Value;
                _updateDrawing = true;
                nudWidth.Value = nudWidthL.Value;
            }
        }

        private void nudHeightT_ValueChanged(object sender, EventArgs e)
        {
            if (!_ignoreValueChange)
            {
                _updateDrawing = false;
                nudY.Value = nudY.Value + nudHeight.Value - nudHeightT.Value;
                _updateDrawing = true;
                nudHeight.Value = nudHeightT.Value;
            }
        }

        private void UpdateRectangle()
        {
            if (!_updateDrawing) return;
            int i = listBoxLabelRectangles.SelectedIndex;
            if (i >= 0 && i < ArkOCR.OCR.ocrConfig.labelRectangles.Length)
            {
                // set all stat-labels if wanted
                if (chkbSetAllStatLabels.Checked && i < 9)
                {
                    for (int s = 0; s < 9; s++)
                        if (i != s)
                            ArkOCR.OCR.ocrConfig.labelRectangles[s] = new Rectangle((int)nudX.Value, ArkOCR.OCR.ocrConfig.labelRectangles[s].Y, (int)nudWidth.Value, (int)nudHeight.Value);
                }
                ArkOCR.OCR.ocrConfig.labelRectangles[i] = new Rectangle((int)nudX.Value, (int)nudY.Value, (int)nudWidth.Value, (int)nudHeight.Value);

                _redrawingDebouncer.Debounce(100, RedrawScreenshot, Dispatcher.CurrentDispatcher, (i, true, -1));
            }
        }

        private void btnSaveOCRconfig_Click(object sender, EventArgs e)
        {
            string filePath = Properties.Settings.Default.ocrFile;
            if (string.IsNullOrEmpty(filePath))
            {
                SaveOcrFileAs();
                return;
            }
            ArkOCR.OCR.ocrConfig.SaveFile(filePath);
            UpdateOCRLabel(filePath);
        }

        private void btnSaveOCRConfigAs_Click(object sender, EventArgs e)
        {
            SaveOcrFileAs();
        }

        private bool SaveOcrFileAs()
        {
            if (ArkOCR.OCR.ocrConfig == null) return false;

            string path = FileService.GetJsonPath(FileService.OcrFolderName);
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "OCR configuration File (*.json)|*.json",
                InitialDirectory = path
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrWhiteSpace(dlg.FileName))
                    {
                        MessageBox.Show("Can't save, no file name specified.", "Missing file name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    var filePath = dlg.FileName;

                    ArkOCR.OCR.ocrConfig.SaveFile(filePath);
                    Properties.Settings.Default.ocrFile = filePath;
                    LoadAndInitializeOcrTemplate(filePath);
                    return true;
                }
            }

            return false;
        }

        private void buttonLoadOCRTemplate_Click(object sender, EventArgs e)
        {
            string path = FileService.GetJsonPath(FileService.OcrFolderName);

            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "OCR configuration File (*.json)|*.json",
                InitialDirectory = path,
                CheckFileExists = true
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK && LoadAndInitializeOcrTemplate(dlg.FileName))
                {
                    Properties.Settings.Default.ocrFile = dlg.FileName;
                }
            }
        }

        private void btUnloadOCR_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ocrFile = null;
            ArkOCR.OCR.ocrConfig = null;
            UpdateOCRLabel();
        }

        private void BtNewOcrConfig_Click(object sender, EventArgs e)
        {
            var currentOcrConfig = ArkOCR.OCR.ocrConfig;
            ArkOCR.OCR.ocrConfig = new OCRTemplate();
            if (SaveOcrFileAs()) return;

            // user doesn't want to create new config, reset to old one
            ArkOCR.OCR.ocrConfig = currentOcrConfig;
        }

        private bool LoadAndInitializeOcrTemplate(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                UpdateOCRLabel();
                return false;
            }

            var loadedOcrConfig = OCRTemplate.LoadFile(filePath);
            if (loadedOcrConfig == null)
            {
                filePath = null;
            }

            ArkOCR.OCR.ocrConfig = loadedOcrConfig;
            UpdateOCRLabel(filePath);
            if (loadedOcrConfig == null) return false;

            InitLabelEntries();
            nudResizing.Value = ArkOCR.OCR.ocrConfig.resize == 0 ? 1 : (decimal)ArkOCR.OCR.ocrConfig.resize;
            CbTrainRecognition.Checked = ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.IsTrainingEnabled;
            CbSkipNameRecognition.Checked = ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.SkipName;
            CbSkipTribeRecognition.Checked = ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.SkipTribe;
            CbSkipOwnerRecognition.Checked = ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.SkipOwner;
            return true;
        }

        private void UpdateOCRLabel(string fileName = null)
        {
            var ocrAvailable = !string.IsNullOrEmpty(fileName) && ArkOCR.OCR.ocrConfig != null;
            labelOCRFile.Text = !ocrAvailable
                ? "no ocr-File loaded (OCR won't work)"
                : $"{fileName}\n\n" +
                $"Resolution: {ArkOCR.OCR.ocrConfig.resolutionWidth} × {ArkOCR.OCR.ocrConfig.resolutionHeight}\n" +
                $"UI-Scaling: {ArkOCR.OCR.ocrConfig.guiZoom}\n" +
                $"Screenshot-Resizing-Factor: {ArkOCR.OCR.ocrConfig.resize}";

            UpdateResizeResultLabel();

            labelOCRFile.Cursor = ocrAvailable ? Cursors.Hand : null;

            BtSaveOCRConfigAs.Enabled = ocrAvailable;
            BtSaveOCRconfig.Enabled = ocrAvailable;
            BtUnloadOCR.Enabled = ocrAvailable;
        }

        private void BtCreateOcrPatternsFromManualChars_Click(object sender, EventArgs e)
        {
            string characters = textBoxCalibrationText.Text;
            int fontSize = (int)nudFontSizeCalibration.Value;
            if (fontSize < 5)
            {
                MessageBoxes.ShowMessageBox($"Fontsize {fontSize} is too small", "Error");
                return;
            }

            string fontFilePath = null;
            if (ArkOCR.OCR.CreateOcrTemplatesFromFontFile(fontSize, characters, _fontFilePath, ref fontFilePath))
            {
                _fontFilePath = fontFilePath;
                MessageBoxes.ShowMessageBox($"OCR patterns created for\n{characters}\nin font size {fontSize}", "OCR patterns created", MessageBoxIcon.Information);
            }
            else
                MessageBoxes.ShowMessageBox($"Unknown error while creating OCR patterns for\n{characters}\nin font size {fontSize}");
        }

        private void buttonLoadCalibrationImage_Click(object sender, EventArgs e)
        {
            const string statValueChars = "0123456789.,%/";
            const string levelChars = "0123456789:LEVEL";
            const string textChars = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            // get font sizes from label heights
            var fontSizesChars = new Dictionary<int, string>(4)
            {
                {ArkOCR.OCR.ocrConfig.labelRectangles[10].Height, textChars} // name, species
            };

            if (!fontSizesChars.ContainsKey(ArkOCR.OCR.ocrConfig.labelRectangles[0].Height))
                fontSizesChars.Add(ArkOCR.OCR.ocrConfig.labelRectangles[0].Height, statValueChars); // stats
            if (!fontSizesChars.ContainsKey(ArkOCR.OCR.ocrConfig.labelRectangles[9].Height))
                fontSizesChars.Add(ArkOCR.OCR.ocrConfig.labelRectangles[9].Height, levelChars); // level
            if (!fontSizesChars.ContainsKey(ArkOCR.OCR.ocrConfig.labelRectangles[11].Height))
                fontSizesChars.Add(ArkOCR.OCR.ocrConfig.labelRectangles[11].Height, textChars); // owner

            string fontFilePath = null;
            foreach (var c in fontSizesChars)
                ArkOCR.OCR.CreateOcrTemplatesFromFontFile(c.Key, c.Value, _fontFilePath, ref fontFilePath);
            _fontFilePath = fontFilePath;

            MessageBoxes.ShowMessageBox($"OCR patterns created for the set labels", "OCR patterns created", MessageBoxIcon.Information);
        }

        internal void SetScreenshot(Bitmap screenshotbmp)
        {
            _screenshot?.Dispose();
            _screenshot = screenshotbmp;
            OCRDebugLayoutPanel.AutoScrollPosition = new Point(_screenshot.Width / 3, _screenshot.Height / 4);
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
            if (_screenshot != null)
            {
                nudResolutionWidth.Value = _screenshot.Width;
                nudResolutionHeight.Value = _screenshot.Height;
            }
        }

        private void nudResizing_ValueChanged(object sender, EventArgs e)
        {
            ArkOCR.OCR.ocrConfig.resize = (double)nudResizing.Value;
            UpdateResizeResultLabel();
        }

        private void UpdateResizeResultLabel()
        {
            if (ArkOCR.OCR.ocrConfig == null)
            {
                lbResizeResult.Text = string.Empty;
                return;
            }

            int resizedHeight = (int)(ArkOCR.OCR.ocrConfig.resize * ArkOCR.OCR.ocrConfig.resolutionHeight);
            lbResizeResult.Text = $"{ArkOCR.OCR.ocrConfig.resolutionWidth} × {ArkOCR.OCR.ocrConfig.resolutionHeight} -> " +
                    $"{(int)(ArkOCR.OCR.ocrConfig.resize * ArkOCR.OCR.ocrConfig.resolutionWidth)} × {resizedHeight}";
            string infoText = "\nKeep in mind, any change of the resizing needs new character templates to be made";
            if (resizedHeight < 1080)
                lbResizeResult.Text += "\nThe size is probably too small for good results, you can try to increse the factor." + infoText;
        }

        private void CbTrainRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.IsTrainingEnabled, sender);
        }

        private void CbSkipNameRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.SkipName, sender);
        }

        private void CbSkipTribeRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.SkipTribe, sender);
        }

        private void CbSkipOwnerRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOCR.OCR.ocrConfig.RecognitionPatterns.TrainingSettings.SkipOwner, sender);
        }

        private static void SaveOcrSettings(ref bool setting, object sender)
        {
            bool setTo = sender is CheckBox cb && cb.Checked;
            if (setting == setTo) return;

            setting = setTo;
            ArkOCR.OCR.ocrConfig.SaveFile(Properties.Settings.Default.ocrFile);
        }

        private void PictureBoxClicked(object sender, EventArgs e)
        {
            // set position of active label to that position
            if (tabControlManage.SelectedTab != tabPage3
                || listBoxLabelRectangles.SelectedIndex == -1
            )
                return;

            var coords = (MouseEventArgs)e;
            nudX.ValueSave = coords.X;
            nudY.ValueSave = coords.Y;
            if (nudHeight.Value == 0)
                nudHeight.ValueSave = 18;
            if (nudWidth.Value == 0)
                nudWidth.ValueSave = 150;
        }

        #region Pattern editing

        private TextData _selectedTextData;

        /// <summary>
        /// Display the patterns for the recognized text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxRecognized_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = listBoxRecognized.SelectedIndex;
            string s = listBoxRecognized.SelectedItem.ToString();
            if (i >= 0 && i < _recognizedPatterns.Count)
            {
                // TODO implement new multi pattern system, allow editing / removing patterns from string
                _selectedTextData = ArkOCR.OCR.ocrConfig.RecognitionPatterns.Texts.FirstOrDefault(t => t.Text == s) ??
                                    new TextData();

                ocrLetterEditRecognized.PatternDisplay = _recognizedPatterns[i];
                ocrLetterEditTemplate.PatternComparing = _recognizedPatterns[i];
                textBoxTemplate.Text = _selectedTextData.Text; // this also loads the pattern template
            }
        }

        private void ListBoxPatternsOfString_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = ((ListBox)sender).SelectedIndex;
            if (i == -1 || _selectedTextData == null) return;

            ocrLetterEditTemplate.PatternDisplay = _selectedTextData.Patterns[i];

            ShowMatch();
            textBoxTemplate.Focus();
            textBoxTemplate.SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ocrLetterEditTemplate.LetterArray = ocrLetterEditRecognized.LetterArray;
        }

        /// <summary>
        /// Clears the lists of recognized ocr patterns. Call before each ocr.
        /// </summary>
        public void ClearLists()
        {
            _recognizedPatterns.Clear();
            listBoxRecognized.Items.Clear();
        }

        /// <summary>
        /// Adds the recognized characters to a selectable list, where they can be viewed and adjusted / fine tuned.
        /// </summary>
        public void AddLetterToRecognized(string characters, Pattern readPattern)
        {
            _recognizedPatterns.Add(readPattern);
            listBoxRecognized.Items.Add(characters);
        }

        private void textBoxTemplate_TextChanged(object sender, EventArgs e)
        {
            if (textBoxTemplate.Text.Length > 0)
            {
                textBoxTemplate.SelectAll();
                LoadTemplateLetter();
            }
            else
            {
                ListBoxPatternsOfString.Items.Clear();
            }
        }

        private void BtRemovePattern_Click(object sender, EventArgs e)
        {
            if (_selectedTextData == null
                || ListBoxPatternsOfString.SelectedIndex == -1
                || MessageBox.Show($"Remove the selected pattern for the string {_selectedTextData.Text}", "Remove Pattern?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var selectedIndex = ListBoxPatternsOfString.SelectedIndex;
            _selectedTextData.Patterns.RemoveAt(selectedIndex);

            ListBoxPatternsOfString.Items.Clear();

            int patternCount = _selectedTextData.Patterns.Count;
            if (selectedIndex >= patternCount) selectedIndex = patternCount - 1;
            ListBoxPatternsOfString.Items.AddRange(Enumerable.Range(1, patternCount).Select(ii => ii.ToString()).ToArray());
            if (patternCount > 0)
                ListBoxPatternsOfString.SelectedIndex = selectedIndex;
        }

        #endregion

        private void labelOCRFile_Click(object sender, EventArgs e)
        {
            // open explorer with currently loaded ocrConfigFile
            var ocrFile = Properties.Settings.Default.ocrFile;
            if (string.IsNullOrEmpty(ocrFile) || !File.Exists(ocrFile)) return;

            Process.Start("explorer.exe", $"/select,\"{ocrFile}\"");
        }

        private void BtSetStatPositionBasedOnFirstTwo_Click(object sender, EventArgs e)
        {
            var rectangles = ArkOCR.OCR.ocrConfig.labelRectangles;
            int y = rectangles[0].Y;
            int yDiff = rectangles[1].Y - y;
            if (yDiff < 0) return;

            int width = rectangles[0].Width;
            int height = rectangles[0].Height;
            int x = rectangles[0].X;

            for (int i = 2; i < 9; i++)
            {
                rectangles[i] = new Rectangle(x, y + yDiff * i, width, height);
            }

            RedrawScreenshot(0);
        }

        private void BtRemoveSelectedPatterns_Click(object sender, EventArgs e)
        {
            RemovePatterns(TbRemovePatterns.Text);
        }

        private void BtRemoveAllPatterns_Click(object sender, EventArgs e)
        {
            RemovePatterns(null);
        }

        private void RemovePatterns(string patternText)
        {

            if (ArkOCR.OCR.ocrConfig == null || patternText == string.Empty) return;

            if (MessageBox.Show(patternText != null ? $"Remove all the OCR patterns for the text\n\n{patternText}" : "WARNING\nRemove all patterns in this config file?", "Remove patterns?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (patternText == null)
                ArkOCR.OCR.ocrConfig.RecognitionPatterns.Texts.Clear();
            else
                ArkOCR.OCR.ocrConfig.RecognitionPatterns.Texts.RemoveAll(t => t.Text == patternText);

            LoadTemplateLetter();
        }
    }
}

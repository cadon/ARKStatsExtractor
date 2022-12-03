using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
        public event Action<byte> UpdateWhiteThreshold;
        public event Action<string, bool, bool> DoOcr;
        public readonly FlowLayoutPanel debugPanel;
        public readonly TextBox output;
        private readonly List<Pattern> _recognizedPatterns = new List<Pattern>();
        private Bitmap _screenshot;
        private bool _updateDrawing = true;
        private bool _ignoreValueChange;
        private readonly Debouncer _redrawingDebouncer = new Debouncer();
        private string _fontFilePath;
        private readonly ToolTip _tt;

        public OCRControl()
        {
            InitializeComponent();
            debugPanel = OCRDebugLayoutPanel;
            output = txtOCROutput;
            _tt = new ToolTip();
            ocrLetterEditTemplate.drawingEnabled = true;
            ocrLetterEditTemplate.PatternChanged += OcrLetterEditTemplate_PatternChanged;
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
            listBoxLabelRectangles.Items.AddRange(Enum.GetNames(typeof(OcrTemplate.OcrLabels)).ToArray());

            // resolution
            nudResolutionWidth.Value = ArkOcr.Ocr.ocrConfig.resolutionWidth;
            nudResolutionHeight.Value = ArkOcr.Ocr.ocrConfig.resolutionHeight;
        }

        private void nudWhiteTreshold_ValueChanged(object sender, EventArgs e)
        {
            var whiteThreshold = (byte)nudWhiteTreshold.Value;
            UpdateWhiteThreshold?.Invoke(whiteThreshold);
            ShowPreviewWhiteThreshold(whiteThreshold);
        }

        private void nudWhiteTreshold_Leave(object sender, EventArgs e)
        {
            ShowPreviewWhiteThreshold(-1);
        }

        /// <summary>
        /// Shows a preview of the white-threshold if a screenshot is displayed
        /// </summary>
        /// <param name="value">The white-threshold. -1 disables the preview</param>
        private void ShowPreviewWhiteThreshold(int value)
        {
            _redrawingDebouncer.Debounce(500, RedrawScreenshot, Dispatcher.CurrentDispatcher, (-1, false, value, Rectangle.Empty));
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

                if (nudResolutionWidth.Value == 0
                    && nudResolutionHeight.Value == 0)
                {
                    nudResolutionWidth.Value = bmp.Width;
                    nudResolutionHeight.Value = bmp.Height;
                }

                if (bmp.Width == nudResolutionWidth.Value
                    && bmp.Height == nudResolutionHeight.Value)
                {
                    bmp.Dispose();
                    if (!ArkOcr.Ocr.ocrConfig?.RecognitionPatterns?.Texts?.Any() ?? false)
                        ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.IsTrainingEnabled = true;

                    DoOcr?.Invoke(files[0], true, false);
                }
                else
                {
                    DisplayBmpInOcrControl(bmp);
                    txtOCROutput.Text =
                        $"Error: the current ocr configuration is set to an image size of{Environment.NewLine}{nudResolutionWidth.Value} × {nudResolutionHeight.Value} px,{Environment.NewLine}the image has a size of{Environment.NewLine}{bmp.Width} × {bmp.Height} px.";
                }
            }
            catch (Exception ex)
            {
                txtOCROutput.Text =
                    $"Error during the OCR:{Environment.NewLine}{ex.Message}";
            }
        }

        private void LoadTemplateLetter()
        {
            var text = textBoxTemplate.Text;
            if (string.IsNullOrEmpty(text)) return;

            _selectedTextData = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.Texts.FirstOrDefault(t => t.Text == text);

            ListPatternsOfText();
        }

        /// <summary>
        /// Displays all patterns of the selected text. If selectedIndex is -1, the best match will be selected.
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void ListPatternsOfText(int selectedIndex = -1)
        {
            ListBoxPatternsOfString.Items.Clear();

            if (_selectedTextData == null) return;
            int patternCount = _selectedTextData.Patterns.Count;
            if (patternCount == 0) return;

            var matches = new string[patternCount];
            int bestMatchIndex = 0;
            float bestMatchValue = 0;
            for (int i = 0; i < patternCount; i++)
            {
                RecognitionPatterns.PatternMatch(_selectedTextData.Patterns[i].Data, ocrLetterEditRecognized.PatternDisplay?.Data, out float match, out _);
                matches[i] = $"{Math.Round(match * 100, 1)}";
                if (match > bestMatchValue)
                {
                    bestMatchValue = match;
                    bestMatchIndex = i;
                }
            }
            ListBoxPatternsOfString.Items.AddRange(matches);

            ListBoxPatternsOfString.SelectedIndex = selectedIndex == -1 ? bestMatchIndex : selectedIndex;
        }

        private void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            SaveTemplate(textBoxTemplate.Text, ocrLetterEditTemplate.PatternDisplay);
            SaveOcrConfigFile(false);
        }

        private void buttonSaveAsTemplate_Click(object sender, EventArgs e)
        {
            SaveTemplate(textBoxTemplate.Text, ocrLetterEditRecognized.PatternDisplay);
            SaveOcrConfigFile(false);
        }

        /// <summary>
        /// Saves the pattern.
        /// </summary>
        private void SaveTemplate(string text, Pattern pattern)
        {
            if (string.IsNullOrEmpty(text) || pattern == null) return;

            var existingTemplate = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.Texts.FirstOrDefault(t => t.Text == text);
            if (existingTemplate == null)
            {
                ArkOcr.Ocr.ocrConfig.RecognitionPatterns.Texts.Add(new TextData { Text = text, Patterns = new List<Pattern> { pattern } });
            }
            else
            {
                foreach (var p in existingTemplate.Patterns)
                {
                    if (p.Equals(pattern)) return;
                }

                existingTemplate.Patterns.Add(pattern);
            }

            ArkOcr.Ocr.ocrConfig.SaveFile(Properties.Settings.Default.ocrFile);

            LoadTemplateLetter();
        }

        private void textBoxTemplate_Enter(object sender, EventArgs e)
        {
            textBoxTemplate.SelectAll();
        }

        /// <summary>
        /// Calculates and displays the match between the recognized and template pattern.
        /// </summary>
        private void ShowMatch()
        {
            RecognitionPatterns.PatternMatch(ocrLetterEditTemplate.PatternDisplay.Data, ocrLetterEditRecognized.PatternDisplay?.Data, out float match, out int offset);
            ocrLetterEditTemplate.RecognizedOffset = offset;

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
            if (rectangleIndex < 0 || rectangleIndex >= ArkOcr.Ocr.ocrConfig.UsedLabelRectangles.Length) return;

            Rectangle rec = ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[rectangleIndex];
            _ignoreValueChange = true;
            nudX.Value = rec.X;
            nudY.Value = rec.Y;
            nudWidth.Value = rec.Width;
            nudHeight.Value = rec.Height;
            nudWidthL.Value = rec.Width;
            nudHeightT.Value = rec.Height;
            _ignoreValueChange = false;
        }

        /// <summary>
        /// Displays screen shot in OCR control.
        /// </summary>
        /// <param name="bmp"></param>
        internal void DisplayBmpInOcrControl(Bitmap bmp)
        {
            for (int i = OCRDebugLayoutPanel.Controls.Count - 1; i >= 0; i--)
            {
                if (OCRDebugLayoutPanel.Controls[i] is PictureBox pb)
                    pb.Dispose();
            }
            OCRDebugLayoutPanel.Controls.Clear();

            PictureBox b = new PictureBox { SizeMode = PictureBoxSizeMode.AutoSize, Image = bmp };
            OCRDebugLayoutPanel.Controls.Add(b);
            OCRDebugLayoutPanel.Controls.SetChildIndex(b, 0);
            int scrollHorizontal = bmp.Width - OCRDebugLayoutPanel.Width;
            if (scrollHorizontal > 0)
                OCRDebugLayoutPanel.AutoScrollPosition = new Point(scrollHorizontal / 2, 0);
            b.Click += PictureBoxClicked;

            _screenshot?.Dispose();
            _screenshot = bmp;
        }

        private void RedrawScreenshot((int, bool, int, Rectangle) args)
        {
            RedrawScreenshot(args.Item1, args.Item2, args.Item3, args.Item4);
        }

        /// <summary>
        /// Redraws the screenShot
        /// </summary>
        /// <param name="highlightIndex">Which of the labels should be highlighted, -1 for none.</param>
        /// <param name="showLabels">Show set label rectangles.</param>
        /// <param name="whiteThreshold">Preview of white-Threshold. -1 to disable.</param>
        /// <param name="manualRectangle">draw a custom rectangle</param>
        private void RedrawScreenshot(int highlightIndex = -1, bool showLabels = true, int whiteThreshold = -1, Rectangle manualRectangle = default)
        {
            if (_screenshot == null
                || OCRDebugLayoutPanel.Controls.Count == 0
                || !(OCRDebugLayoutPanel.Controls[OCRDebugLayoutPanel.Controls.Count - 1] is PictureBox p)
                ) return;

            _redrawingDebouncer.Cancel();

            Bitmap b = new Bitmap((whiteThreshold >= 0 ? ArkOcr.RemovePixelsUnderThreshold(ArkOcr.GetGreyScale(_screenshot), (byte)whiteThreshold, true) : _screenshot));

            if (showLabels || !manualRectangle.IsEmpty)
            {
                using (Graphics g = Graphics.FromImage(b))
                using (Pen penW = new Pen(Color.White, 2))
                using (Pen penY = new Pen(Color.Yellow, 2))
                using (Pen penB = new Pen(Color.Black, 2))
                {
                    penW.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    penY.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    penB.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                    if (!manualRectangle.IsEmpty)
                    {
                        var rec = manualRectangle;
                        rec.Inflate(2, 2);
                        g.DrawRectangle(penY, rec);
                        rec.Inflate(2, 2);
                        g.DrawRectangle(penB, rec);
                    }
                    else
                    {
                        for (int r = 0; r < ArkOcr.Ocr.ocrConfig.UsedLabelRectangles.Length; r++)
                        {
                            var rec = ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[r];
                            rec.Inflate(2, 2);
                            g.DrawRectangle(r == highlightIndex ? penY : penW, rec);
                            rec.Inflate(2, 2);
                            g.DrawRectangle(penB, rec);
                        }
                    }
                }

                var magnifiedRectangle = highlightIndex != -1
                    ? ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[highlightIndex]
                    : !manualRectangle.IsEmpty
                        ? manualRectangle
                        : Rectangle.Empty;

                if (!magnifiedRectangle.IsEmpty)
                {
                    var currentWhiteThreshold = Properties.Settings.Default.OCRWhiteThreshold;
                    var magnifiedMarginBottom = -1;
                    var magnifiedMarginTop = -1;
                    const int recMargin = 30;

                    if (magnifiedRectangle.Y - OCRDebugLayoutPanel.VerticalScroll.Value >
                        OCRDebugLayoutPanel.Height / 2)
                        magnifiedMarginTop = recMargin + OCRDebugLayoutPanel.VerticalScroll.Value;
                    else
                    {
                        magnifiedMarginBottom = OCRDebugLayoutPanel.Height < b.Height
                            ? b.Height - OCRDebugLayoutPanel.Height + recMargin - OCRDebugLayoutPanel.VerticalScroll.Value
                            : recMargin;
                    }

                    DrawMagnifiedRectangle(b, magnifiedRectangle, currentWhiteThreshold, magnifiedMarginTop, magnifiedMarginBottom);
                }
            }

            Bitmap disp = (Bitmap)p.Image; // take pointer to old image to dispose it soon
            p.Image = b;
            if (disp != null && disp != _screenshot)
                disp.Dispose();
        }

        /// <summary>
        /// Draws a magnified version of the highlighted label for easier positioning.
        /// </summary>
        private static void DrawMagnifiedRectangle(Bitmap bmp, Rectangle labelRectangle, byte whiteThreshold, int magnifiedYMarginTop, int magnifiedYMarginBottom)
        {
            const int margin = 5;
            var bmpWidth = bmp.Width;
            var bmpHeight = bmp.Height;
            int pixelSize = Math.Min(10, bmpWidth / (labelRectangle.Width + 2 * margin)); // cap magnifying

            labelRectangle.Inflate(margin, margin);
            var magnifiedWidth = labelRectangle.Width * pixelSize;
            var magnifiedHeight = labelRectangle.Height * pixelSize;
            var magnifiedRectangle = new Rectangle((bmpWidth - magnifiedWidth) / 2, magnifiedYMarginTop == -1 ? bmpHeight - magnifiedHeight - magnifiedYMarginBottom : magnifiedYMarginTop, magnifiedWidth, magnifiedHeight);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmpWidth, bmpHeight), ImageLockMode.ReadWrite, bmp.PixelFormat);
            var bytes = bmp.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
            unsafe
            {
                var scan0Bmp = (byte*)bmpData.Scan0.ToPointer();
                for (int x = Math.Max(0, -labelRectangle.X); x < labelRectangle.Width; x++)
                {
                    var xBmp = x + labelRectangle.X;
                    if (xBmp >= bmpWidth)
                        break;
                    bool inXBorder = x < margin || x >= labelRectangle.Width - margin;
                    for (int y = Math.Max(0, -labelRectangle.Y); y < labelRectangle.Height; y++)
                    {
                        var yBmp = y + labelRectangle.Y;
                        if (yBmp >= bmpHeight)
                            break; // it gets only bigger
                        bool inBorder = inXBorder || y < margin || y >= labelRectangle.Height - margin;
                        byte* px = scan0Bmp + yBmp * bmpData.Stride + xBmp * bytes;
                        var b = px[0];
                        var g = px[1];
                        var r = px[2];
                        if (!inBorder)
                        {
                            if (ArkOcr.HslLightness(r, g, b) < whiteThreshold)
                            {
                                b /= 2;
                                g /= 2;
                                r /= 2;
                            }
                            else
                            {
                                b = 255; //(byte)(255 - (255 - b) / 2);
                                g = 255; //(byte)(255 - (255 - g) / 2);
                                r = 255; //(byte)(255 - (255 - r) / 2);
                            }
                            //var pxLightness = (byte)(ArkOCR.HslLightness(r, g, b) < whiteThreshold ? 0 : 255);
                        }

                        var xEnd = magnifiedRectangle.X + (x + 1) * pixelSize;
                        var yEnd = magnifiedRectangle.Y + (y + 1) * pixelSize;

                        for (int xm = magnifiedRectangle.X + x * pixelSize; xm < xEnd; xm++)
                        {
                            for (int ym = magnifiedRectangle.Y + y * pixelSize; ym < yEnd; ym++)
                            {
                                byte* mPx = scan0Bmp + ym * bmpData.Stride + xm * bytes;
                                mPx[0] = b;
                                mPx[1] = g;
                                mPx[2] = r;
                            }
                        }
                    }
                }
            }
            bmp.UnlockBits(bmpData);
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
            if (i >= 0 && i < ArkOcr.Ocr.ocrConfig.UsedLabelRectangles.Length)
            {
                // set all stat-labels if wanted
                if (chkbSetAllStatLabels.Checked && i < 9)
                {
                    for (int s = 0; s < 9; s++)
                        if (i != s)
                            ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[s] = new Rectangle((int)nudX.Value, ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[s].Y, (int)nudWidth.Value, (int)nudHeight.Value);
                }
                ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[i] = new Rectangle((int)nudX.Value, (int)nudY.Value, (int)nudWidth.Value, (int)nudHeight.Value);

                _redrawingDebouncer.Debounce(100, RedrawScreenshot, Dispatcher.CurrentDispatcher, (i, true, -1, Rectangle.Empty));
            }
        }

        private void btnSaveOCRconfig_Click(object sender, EventArgs e)
        {
            SaveOcrConfigFile(true);
        }

        private void SaveOcrConfigFile(bool updateOcrLabel = false)
        {
            string filePath = Properties.Settings.Default.ocrFile;
            if (string.IsNullOrEmpty(filePath))
            {
                SaveOcrFileAs();
                return;
            }
            ArkOcr.Ocr.ocrConfig.SaveFile(filePath);
            UpdateOcrLabel(filePath);
        }

        private void btnSaveOCRConfigAs_Click(object sender, EventArgs e)
        {
            SaveOcrFileAs();
        }

        private bool SaveOcrFileAs()
        {
            if (ArkOcr.Ocr.ocrConfig == null) return false;

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

                    ArkOcr.Ocr.ocrConfig.SaveFile(filePath);
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
            ArkOcr.Ocr.ocrConfig = null;
            InitializeComboboxLabelSetNames();
            UpdateOcrLabel();
        }

        private void BtNewOcrConfig_Click(object sender, EventArgs e)
        {
            var currentOcrConfig = ArkOcr.Ocr.ocrConfig;
            ArkOcr.Ocr.ocrConfig = new OcrTemplate();
            if (SaveOcrFileAs()) return;

            // user doesn't want to create new config, reset to old one
            ArkOcr.Ocr.ocrConfig = currentOcrConfig;
        }

        private bool LoadAndInitializeOcrTemplate(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                UpdateOcrLabel();
                return false;
            }

            var loadedOcrConfig = OcrTemplate.LoadFile(filePath);
            if (loadedOcrConfig == null)
            {
                filePath = null;
            }

            ArkOcr.Ocr.ocrConfig = loadedOcrConfig;
            ArkOcr.Ocr.LoadReplacingsFile();
            UpdateOcrLabel(filePath);
            if (loadedOcrConfig == null) return false;

            InitializeComboboxLabelSetNames();
            InitLabelEntries();
            nudResizing.Value = ArkOcr.Ocr.ocrConfig.resize == 0 ? 1 : (decimal)ArkOcr.Ocr.ocrConfig.resize;
            CbTrainRecognition.Checked = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.IsTrainingEnabled;
            CbSkipNameRecognition.Checked = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.SkipName;
            CbSkipTribeRecognition.Checked = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.SkipTribe;
            CbSkipOwnerRecognition.Checked = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.SkipOwner;

            return true;
        }

        private void UpdateOcrLabel(string fileName = null)
        {
            var ocrAvailable = !string.IsNullOrEmpty(fileName) && ArkOcr.Ocr.ocrConfig != null;
            labelOCRFile.Text = !ocrAvailable
                ? "no OCR file loaded (OCR won't work)"
                : $"{fileName}\n\n" +
                $"Resolution: {ArkOcr.Ocr.ocrConfig.resolutionWidth} × {ArkOcr.Ocr.ocrConfig.resolutionHeight}\n" +
                $"UI-Scaling: {ArkOcr.Ocr.ocrConfig.guiZoom}\n" +
                $"Screenshot-Resizing-Factor: {ArkOcr.Ocr.ocrConfig.resize}";

            UpdateResizeResultLabel();
            LbReplacingsFileStatus.Text = ArkOcr.Ocr.RegexReplacingsStatus;

            if (ocrAvailable)
            {
                labelOCRFile.Cursor = Cursors.Hand;
                _tt.SetToolTip(labelOCRFile, "Click to open file in explorer");
            }
            else
            {
                labelOCRFile.Cursor = null;
                _tt.SetToolTip(labelOCRFile, null);
            }

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
                MessageBoxes.ShowMessageBox($"Font size {fontSize} is too small", "Error");
                return;
            }

            string fontFilePath = null;
            if (ArkOcr.Ocr.CreateOcrTemplatesFromFontFile(fontSize, characters, _fontFilePath, ref fontFilePath))
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
                {ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[10].Height, textChars} // name, species
            };

            if (!fontSizesChars.ContainsKey(ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[0].Height))
                fontSizesChars.Add(ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[0].Height, statValueChars); // stats
            if (!fontSizesChars.ContainsKey(ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[9].Height))
                fontSizesChars.Add(ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[9].Height, levelChars); // level
            if (!fontSizesChars.ContainsKey(ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[11].Height))
                fontSizesChars.Add(ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[11].Height, textChars); // owner

            string fontFilePath = null;
            foreach (var c in fontSizesChars)
            {
                if (!ArkOcr.Ocr.CreateOcrTemplatesFromFontFile(c.Key, c.Value, _fontFilePath, ref fontFilePath))
                    return; // user probably cancelled font selection
            }
            _fontFilePath = fontFilePath;

            MessageBoxes.ShowMessageBox("OCR patterns created for the set labels", "OCR patterns created", MessageBoxIcon.Information);

            string filePath = Properties.Settings.Default.ocrFile;
            if (!string.IsNullOrEmpty(filePath))
            {
                ArkOcr.Ocr.ocrConfig.SaveFile(filePath);
            }
        }

        private void cbEnableOutput_CheckedChanged(object sender, EventArgs e)
        {
            ArkOcr.Ocr.enableOutput = cbEnableOutput.Checked;
        }

        private void nudResolutionWidth_ValueChanged(object sender, EventArgs e)
        {
            if (ArkOcr.Ocr.ocrConfig != null)
                ArkOcr.Ocr.ocrConfig.resolutionWidth = (int)nudResolutionWidth.Value;
        }

        private void nudResolutionHeight_ValueChanged(object sender, EventArgs e)
        {
            if (ArkOcr.Ocr.ocrConfig != null)
                ArkOcr.Ocr.ocrConfig.resolutionHeight = (int)nudResolutionHeight.Value;
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
            ArkOcr.Ocr.ocrConfig.resize = (double)nudResizing.Value;
            UpdateResizeResultLabel();
        }

        private void UpdateResizeResultLabel()
        {
            if (ArkOcr.Ocr.ocrConfig == null)
            {
                lbResizeResult.Text = string.Empty;
                return;
            }

            int resizedHeight = (int)(ArkOcr.Ocr.ocrConfig.resize * ArkOcr.Ocr.ocrConfig.resolutionHeight);
            lbResizeResult.Text = $"{ArkOcr.Ocr.ocrConfig.resolutionWidth} × {ArkOcr.Ocr.ocrConfig.resolutionHeight} -> " +
                    $"{(int)(ArkOcr.Ocr.ocrConfig.resize * ArkOcr.Ocr.ocrConfig.resolutionWidth)} × {resizedHeight}";
            string infoText = "\nKeep in mind, any change of the resizing needs new character templates to be made";
            if (resizedHeight < 1080)
                lbResizeResult.Text += "\nThe size is probably too small for good results, you can try to increse the factor." + infoText;
        }

        private void CbTrainRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.IsTrainingEnabled, sender);
        }

        private void CbSkipNameRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.SkipName, sender);
        }

        private void CbSkipTribeRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.SkipTribe, sender);
        }

        private void CbSkipOwnerRecognition_CheckedChanged(object sender, EventArgs e)
        {
            SaveOcrSettings(ref ArkOcr.Ocr.ocrConfig.RecognitionPatterns.TrainingSettings.SkipOwner, sender);
        }

        private static void SaveOcrSettings(ref bool setting, object sender)
        {
            bool setTo = sender is CheckBox cb && cb.Checked;
            if (setting == setTo) return;

            setting = setTo;

            ArkOcr.Ocr.ocrConfig.SaveFile(Properties.Settings.Default.ocrFile);
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
            var recognizedPattern = listBoxRecognized.SelectedItem as RecognizedCharData;
            if (recognizedPattern == null) return;

            _selectedTextData = ArkOcr.Ocr.ocrConfig.RecognitionPatterns.Texts.FirstOrDefault(t => t.Text == recognizedPattern.Text) ??
                                new TextData();

            var recognizedPatternData = new Pattern(recognizedPattern.Pattern, recognizedPattern.YOffset);
            ocrLetterEditRecognized.PatternDisplay = recognizedPatternData;
            ocrLetterEditTemplate.PatternComparing = recognizedPatternData;
            textBoxTemplate.Text = string.Empty;
            textBoxTemplate.Text = _selectedTextData.Text; // this also loads the pattern template

            // draw border around character on the image
            const int padding = 5;
            RedrawScreenshot(showLabels: false,
                manualRectangle: new Rectangle(recognizedPattern.Coords.X - padding, recognizedPattern.Coords.Y - recognizedPattern.YOffset - padding,
                    recognizedPattern.Pattern.GetLength(0) + 2 * padding, recognizedPattern.Pattern.GetLength(1) + recognizedPattern.YOffset + 2 * padding));
        }

        private void ListBoxPatternsOfString_SelectedIndexChanged(object sender, EventArgs e)
        {
            var lb = (ListBox)sender;
            int i = lb.SelectedIndex;
            string match;
            if (i == -1 || _selectedTextData == null)
            {
                ocrLetterEditTemplate.PatternDisplay = null;
                match = "0";
            }
            else
            {
                ocrLetterEditTemplate.PatternDisplay = _selectedTextData.Patterns[i];
                match = lb.SelectedItem.ToString();
            }

            labelMatching.Text = $"matching: {match} %";

            textBoxTemplate.Focus();
            textBoxTemplate.SelectAll();
        }

        private void BtCopyPatternRecognizedToTemplateClick(object sender, EventArgs e)
        {
            ocrLetterEditTemplate.PatternDisplay = ocrLetterEditRecognized.PatternDisplay;
            ShowMatch();
        }

        /// <summary>
        /// Clears the lists of recognized ocr patterns. Call before each ocr.
        /// </summary>
        public void ClearLists()
        {
            _recognizedPatterns.Clear();
            listBoxRecognized.Items.Clear();
            textBoxTemplate.Text = string.Empty;
            ocrLetterEditRecognized.PatternDisplay = null;
        }

        /// <summary>
        /// Adds the recognized characters to a selectable list, where they can be viewed and adjusted / fine tuned.
        /// </summary>
        public void AddLetterToRecognized(string characters, Pattern readPattern)
        {
            _recognizedPatterns.Add(readPattern);
            listBoxRecognized.Items.Add(characters);
        }
        public void AddLetterToRecognized(RecognizedCharData readPattern)
        {
            listBoxRecognized.Items.Add(readPattern);
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
                ListBoxPatternsOfString.SelectedIndex = -1;
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
            int patternCount = _selectedTextData.Patterns.Count;
            if (selectedIndex >= patternCount) selectedIndex = patternCount - 1;

            ListPatternsOfText(selectedIndex);
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
            var rectangles = ArkOcr.Ocr.ocrConfig.UsedLabelRectangles;
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

            RedrawScreenshot();
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

            if (ArkOcr.Ocr.ocrConfig == null || patternText == string.Empty) return;

            if (MessageBox.Show(patternText != null ? $"Remove all the OCR patterns for the text\n\n{patternText}" : "WARNING\nRemove all patterns in this config file?", "Remove patterns?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            if (patternText == null)
                ArkOcr.Ocr.ocrConfig.RecognitionPatterns.Texts.Clear();
            else
                ArkOcr.Ocr.ocrConfig.RecognitionPatterns.Texts.RemoveAll(t => t.Text == patternText);

            LoadTemplateLetter();
        }

        private void OcrLetterEditTemplate_PatternChanged()
        {
            ShowMatch();
        }

        private void LlOcrManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RepositoryInfo.OpenWikiPage("OCR");
        }

        private void BtReplacingOpenFile_Click(object sender, EventArgs e)
        {
            var filePath = FileService.GetJsonPath(FileService.OcrFolderName, FileService.OcrReplacingsFile);
            if (string.IsNullOrEmpty(filePath)) return;


            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            Process.Start(filePath);
        }

        private void BtReplacingLoadFile_Click(object sender, EventArgs e)
        {
            ArkOcr.Ocr.LoadReplacingsFile();
            LbReplacingsFileStatus.Text = ArkOcr.Ocr.RegexReplacingsStatus;
        }

        #region OCR label sets

        public event Action OcrLabelSetsChanged;
        public event Action OcrLabelSelectedSetChanged;

        private void BtNewLabelSet_Click(object sender, EventArgs e)
        {
            if (ArkOcr.Ocr.ocrConfig == null) return;
            ArkOcr.Ocr.ocrConfig.SetLabelSet(ArkOcr.Ocr.ocrConfig.NewLabelSet());
            InitializeComboboxLabelSetNames();
        }

        private void BtDeleteLabelSet_Click(object sender, EventArgs e)
        {
            if (ArkOcr.Ocr.ocrConfig == null) return;
            ArkOcr.Ocr.ocrConfig.DeleteCurrentLabelSet();
            InitializeComboboxLabelSetNames();
        }

        private void TbLabelSetName_Leave(object sender, EventArgs e)
        {
            if (ArkOcr.Ocr.ocrConfig == null) return;

            if (ArkOcr.Ocr.ocrConfig.LabelSetChangeName(TbLabelSetName.Text, out var errorMessage))
                InitializeComboboxLabelSetNames();
            if (!string.IsNullOrEmpty(errorMessage))
                MessageBoxes.ShowMessageBox(errorMessage, "Label set name change error");
        }

        private void InitializeComboboxLabelSetNames()
        {
            CbbLabelSets.Items.Clear();
            OcrLabelSetsChanged?.Invoke();
            if (ArkOcr.Ocr.ocrConfig == null)
            {
                TbLabelSetName.Text = string.Empty;
                return;
            }
            CbbLabelSets.Items.AddRange(ArkOcr.Ocr.ocrConfig.LabelRectangles.Keys.ToArray());
            CbbLabelSets.SelectedItem = ArkOcr.Ocr.ocrConfig.SelectedLabelSetName;
        }

        private void CbbLabelSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ArkOcr.Ocr.ocrConfig == null) return;
            ArkOcr.Ocr.ocrConfig.SetLabelSet(((ComboBox)sender).SelectedItem.ToString());
            TbLabelSetName.Text = ArkOcr.Ocr.ocrConfig.SelectedLabelSetName;
            RedrawScreenshot();
            OcrLabelSelectedSetChanged?.Invoke();
        }

        public void SetOcrLabelSetToCurrent()
        {
            CbbLabelSets.SelectedItem = ArkOcr.Ocr.ocrConfig.SelectedLabelSetName;
        }

        #endregion
    }
}

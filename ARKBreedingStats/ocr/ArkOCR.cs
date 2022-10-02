using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ARKBreedingStats.ocr.PatternMatching;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.ocr
{
    public class ArkOcr
    {
        // Class initially created by Nakram
        public OcrTemplate ocrConfig;
        private static ArkOcr _ocr;
        private static OCRControl _ocrControl;
        public string screenCaptureApplicationName;
        public int waitBeforeScreenCapture;
        public bool enableOutput = false;
        private (string search, string replace)[] regexReplacings;
        public string RegexReplacingsStatus;

        public static ArkOcr Ocr => _ocr ?? (_ocr = new ArkOcr());

        private ArkOcr()
        {
            screenCaptureApplicationName = Properties.Settings.Default.OCRApp;
            waitBeforeScreenCapture = 500;
        }

        public Bitmap GetScreenshotOfProcess() => Win32API.GetScreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture);

        /// <summary>
        /// Checks if the resolution of the default set process is supported by the currently load ocrConfig.
        /// </summary>
        public bool CheckResolutionSupportedByOcr()
        {
            return CheckResolutionSupportedByOcr(GetScreenshotOfProcess());
        }

        /// <summary>
        /// Checks if the screenshot is supported by the currently load ocrConfig.
        /// </summary>
        private bool CheckResolutionSupportedByOcr(Bitmap screenshot)
        {
            if (screenshot == null
                || ocrConfig == null)
                return false;

            return screenshot.Width == ocrConfig.resolutionWidth && screenshot.Height == ocrConfig.resolutionHeight;
        }

        private Bitmap SubImage(Bitmap source, int x, int y, int width, int height)
        {
            //// test if first column only contains very few whites, then ommit this column
            //if (height > 7) // TODO this extra check is better for 'a', but worse for 'l'
            //{
            //    int firstWhites = 0, minNeeded = 2;
            //    for (int i = 0; i < height; i++)
            //        if (source.GetPixel(x, y + i).R != 0)
            //        {
            //            firstWhites++;
            //            if (firstWhites > minNeeded)
            //                break;
            //        }
            //    if (firstWhites <= minNeeded)
            //    {
            //        //// draw uncropped
            //        //Rectangle cropRectL = new Rectangle(x, y, width, height);
            //        //Bitmap targetL = new Bitmap(cropRectL.Width, cropRectL.Height);

            //        //using (Graphics g = Graphics.FromImage(targetL))
            //        //{
            //        //    g.DrawImage(source, new Rectangle(0, 0, targetL.Width, targetL.Height),
            //        //                     cropRectL,
            //        //                     GraphicsUnit.Pixel);
            //        //}
            //        //targetL.Save("D:\\temp\\debug_uncroppedLetter.png");// TODO comment out

            //        x++;
            //        width--;
            //    }
            //}

            Rectangle cropRect = new Rectangle(x, y, width, height);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;
        }

        public static Bitmap GetGreyScale(Bitmap source, bool writingInWhite = false)
        {
            Bitmap dest = (Bitmap)source.Clone();

            const PixelFormat pxf = PixelFormat.Format24bppRgb;

            Rectangle rect = new Rectangle(0, 0, dest.Width, dest.Height);
            BitmapData bmpData = dest.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int numBytes = bmpData.Stride * dest.Height;
            byte[] bgrValues = new byte[numBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, bgrValues, 0, numBytes);

            int extraBytes = bmpData.Stride % 3;
            // convert to greyscale
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    int idx = j * bmpData.Stride + i * 3;
                    byte grey;

                    if (writingInWhite)
                    {
                        grey = Math.Max(bgrValues[idx + 1], bgrValues[idx + 2]); // ignoring the blue-channel, ui is blueish
                    }
                    else
                    {
                        int sum = bgrValues[idx + 1] + bgrValues[idx + 2]; // ignoring the blue-channel, ui is blueish
                        grey = (byte)(sum / 2);
                    }

                    bgrValues[idx] = grey;
                    bgrValues[idx + 1] = grey;
                    bgrValues[idx + 2] = grey;
                }
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(bgrValues, 0, ptr, numBytes);

            dest.UnlockBits(bmpData);

            return dest;
        }

        public static Bitmap RemovePixelsUnderThreshold(Bitmap source, byte threshold, bool disposeSource = false)
        {
            Bitmap dest = (Bitmap)source.Clone();

            const PixelFormat pxf = PixelFormat.Format24bppRgb;

            Rectangle rect = new Rectangle(0, 0, dest.Width, dest.Height);
            BitmapData bmpData = dest.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            IntPtr ptr = bmpData.Scan0;

            int numBytes = bmpData.Stride * dest.Height;
            byte[] rgbValues = new byte[numBytes];

            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            double lowT = threshold * .9, highT = threshold * 1; // threshold for grey

            int w = source.Width * 3;
            int remain = bmpData.Stride - w;
            int pc = 0;

            for (int counter = 0; counter + 2 < numBytes; counter += 3)
            {
                pc += 3;
                if (pc == w)
                {
                    counter += remain - 3;
                    pc = 0;
                    continue;
                }

                //using only black and white
                /*
                if (rgbValues[counter] < threshold)
                    rgbValues[counter] = 0;
                else
                    rgbValues[counter] = 255; // maximize the white

               */
                // using a third grey-value


                // only use blue for the threshold testing. the ui text is blueish.
                if (rgbValues[counter] < lowT)
                {
                    rgbValues[counter] = 0;
                    rgbValues[counter + 1] = 0;
                    rgbValues[counter + 2] = 0;
                }
                else if (rgbValues[counter] > highT)
                {
                    rgbValues[counter] = 255; // maximize the white
                    rgbValues[counter + 1] = 255;
                    rgbValues[counter + 2] = 255;
                }
                else
                {
                    rgbValues[counter] = 150;
                    rgbValues[counter + 1] = 150;
                    rgbValues[counter + 2] = 150;
                    //// if a neighbor-pixel (up, right, down or left) is above the threshold, also set this ambiguous pixel to white
                    //if ((counter % bmpData.Stride > 0 && rgbValues[counter - 3] > threshold)
                    //|| (counter % bmpData.Stride < bmpData.Stride - 3 && rgbValues[counter + 3] > threshold)
                    //|| (counter >= bmpData.Stride && rgbValues[counter - bmpData.Stride] > threshold)
                    //|| (counter < numBytes - bmpData.Stride && rgbValues[counter + bmpData.Stride] > threshold))
                    //    rgbValues[counter] = 255;
                    //else
                    //    rgbValues[counter] = 0;
                }
            }

            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            dest.UnlockBits(bmpData);
            if (disposeSource) source.Dispose();
            return dest;
        }

        /// <summary>
        /// Returns the HSL Lightness of given rgb values.
        /// </summary>
        public static byte HslLightness(byte r, byte g, byte b) => (byte)((Math.Max(r, Math.Max(g, b)) + Math.Min(r, Math.Min(g, b))) / 2);

        public bool CreateOcrTemplatesFromFontFile(int fontPxSize, string calibrationText, string lastUsedFontFile, ref string fontFile)
        {
            var patterns = Ocr.ocrConfig?.RecognitionPatterns;
            if (patterns == null) return false;

            if (string.IsNullOrEmpty(fontFile))
            {
                using (OpenFileDialog dlg = new OpenFileDialog
                {
                    Filter = "Font File (*.ttf)|*.ttf"
                })
                {
                    if (!string.IsNullOrEmpty(lastUsedFontFile) && File.Exists(lastUsedFontFile))
                        dlg.FileName = lastUsedFontFile;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        fontFile = dlg.FileName;
                    }
                }
            }

            if (string.IsNullOrEmpty(fontFile) || !File.Exists(fontFile)) return false;

            using (PrivateFontCollection pfColl = new PrivateFontCollection())
            {
                pfColl.AddFontFile(fontFile);
                FontFamily ff = pfColl.Families[0];
                float fontEmSize = fontPxSize * 100f / 118; // specific ratio for the used font
                int moveUpPx = -fontPxSize * 24 / 100; // specific ratio for the used font
                int maxCharWidth = 2 * fontPxSize;

                ////// debug
                //using (Bitmap bitmap = new Bitmap(500, 150))
                //using (Graphics graphics = Graphics.FromImage(bitmap))
                //using (Font f = new Font(ff, fontEmSize, FontStyle.Regular))
                //{
                //    graphics.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
                //    graphics.DrawString(calibrationText, f, Brushes.White, 0, moveUpPx);
                //    bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ASBFontTesting", $"debugTest_{fontPxSize}px_{fontEmSize:F2}.png"));
                //}

                //using (Bitmap bitmap = new Bitmap(500, 150))
                //using (Graphics graphics = Graphics.FromImage(bitmap))
                //using (Font f = new Font(ff, 100 * 100f / 118, FontStyle.Regular))
                //{
                //    graphics.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
                //    graphics.DrawString("|E|P", f, Brushes.White, 0, -100 * 24f / 100);
                //    bitmap.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ASBFontTesting", "debugTest_100em.png"));
                //}

                using (Bitmap bitmap = new Bitmap(maxCharWidth, fontPxSize))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                using (Font f = new Font(ff, fontEmSize, FontStyle.Regular))
                {
                    foreach (char c in calibrationText)
                    {
                        graphics.FillRectangle(Brushes.Black, 0, 0, maxCharWidth, fontPxSize);
                        graphics.DrawString(c.ToString(), f, Brushes.White, 0, moveUpPx);

                        patterns.AddPattern(c.ToString(), bitmap);
                    }
                }
            }
            return true;
        }

        public double[] DoOcr(out string OcrText, out string dinoName, out string species, out string ownerName, out string tribeName, out Sex sex, string useImageFilePath = null, bool changeForegroundWindow = true, bool screenShotFromClipboard = false)
        {
            string finishedText = string.Empty;
            dinoName = string.Empty;
            species = string.Empty;
            ownerName = string.Empty;
            tribeName = string.Empty;
            sex = Sex.Unknown;
            double[] finalValues = { 0 };
            if (ocrConfig?.UsedLabelRectangles == null)
            {
                OcrText = "Error: OCR not configured.\nYou can configure the OCR in the OCR-tab by loading or creating an OCR config-file.\nFor more details see the online manual.";
                ProcessScreenshot(out _);
                return finalValues;
            }

            // check if there is at least one rectangle not empty
            bool oneLabelNotEmpty = false;
            foreach (var rect in ocrConfig.UsedLabelRectangles)
            {
                if (!rect.IsEmpty)
                {
                    oneLabelNotEmpty = true;
                    break;
                }
            }

            if (!oneLabelNotEmpty)
            {
                OcrText = "Error: The rectangles where to read the text in the image with OCR are not configured.\nYou can configure them by navigating to the OCR-tab then to the Labels tab.\nFor more details see the online manual.";
                ProcessScreenshot(out _);
                return finalValues;
            }

            Bitmap screenShotBmp = ProcessScreenshot(out string outText);
            if (!string.IsNullOrEmpty(outText))
            {
                OcrText = outText;
                return finalValues;
            }

            Bitmap ProcessScreenshot(out string errorText)
            {
                errorText = null;
                _ocrControl.debugPanel.Controls.Clear();
                _ocrControl.ClearLists();

                Bitmap bmp;
                if (screenShotFromClipboard)
                {
                    var im = Clipboard.GetImage();
                    if (im == null)
                    {
                        errorText = "No image in the clipboard, OCR cannot be performed. Press the Print-key to create a screenshot and copy it to the clipboard";
                        return null;
                    }
                    try
                    {
                        bmp = (Bitmap)im;
                        var rec = Properties.Settings.Default.OCRFromRectangle;
                        if (!rec.IsEmpty)
                        {
                            var croppedBmp = bmp.Clone(rec, bmp.PixelFormat);
                            bmp.Dispose();
                            bmp = croppedBmp;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorText = $"Error when trying to load the screenshot from the clipboard for the OCR.\n\n" + ex.Message + (ex.InnerException != null ? $" - InnerException: {ex.InnerException.Message}" : null);
                        return null;
                    }
                }
                else if (!string.IsNullOrEmpty(useImageFilePath) && File.Exists(useImageFilePath))
                {
                    try
                    {
                        bmp = (Bitmap)Image.FromFile(useImageFilePath);
                    }
                    catch (Exception ex)
                    {
                        errorText = $"Error when trying to load the file\n{useImageFilePath}\nfor the OCR.\n\n" + ex.Message + (ex.InnerException != null ? $" - InnerException: {ex.InnerException.Message}" : null);
                        return null;
                    }
                }
                else
                {
                    // grab screen shot from ark
                    bmp = Win32API.GetScreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture, true);
                }

                if (bmp == null)
                {
                    errorText = "Error: no image for OCR. Is ARK running?";
                    return null;
                }

                if (ocrConfig != null)
                {
                    if (!CheckResolutionSupportedByOcr(bmp))
                    {
                        errorText =
                            "Error while calibrating: The game-resolution is not supported by the currently loaded OCR-configuration.\n"
                            + $"The tested image has a resolution of {bmp.Width} × {bmp.Height} px,\n"
                            + $"the resolution of the loaded ocr-config is {ocrConfig.resolutionWidth} × {ocrConfig.resolutionHeight} px.\n\n"
                            + "Load or create a ocr-config file with the resolution of the game to make it work.";
                        return bmp;
                    }

                    // TODO resize image according to resize-factor. used for large screenshots
                    if (ocrConfig.resize != 1 && ocrConfig.resize > 0)
                    {
                        Bitmap resized = new Bitmap((int)(ocrConfig.resize * ocrConfig.resolutionWidth),
                            (int)(ocrConfig.resize * ocrConfig.resolutionHeight));
                        using (var graphics = Graphics.FromImage(resized))
                        {
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            using (var wrapMode = new ImageAttributes())
                            {
                                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                                graphics.DrawImage(bmp, new Rectangle(0, 0, resized.Width, resized.Height), 0, 0,
                                    bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrapMode);
                            }

                            bmp.Dispose();
                            bmp = resized;
                        }
                    }
                }

                if (enableOutput)
                {
                    _ocrControl?.DisplayBmpInOcrControl(bmp);
                }

                return bmp;
            }

            finalValues = new double[ocrConfig.UsedLabelRectangles.Length];
            finalValues[8] = -1; // set imprinting to -1 to mark it as unknown and to set a difference to a creature with 0% imprinting.

            if (changeForegroundWindow)
                Win32API.SetForegroundWindow(Application.OpenForms[0].Handle);

            HammingWeight.InitializeBitCounts();

            var whiteThreshold = Properties.Settings.Default.OCRWhiteThreshold;

            bool wild = false; // todo: set to true and find out if the creature is wild in the first loop
            int stI = -1;

            var labels = (OcrTemplate.OcrLabels[])Enum.GetValues(typeof(OcrTemplate.OcrLabels));

            for (int lbI = 0; lbI < labels.Length; lbI++)
            {
                stI++;
                if (lbI == 8) stI = 8;
                var label = labels[stI];

                switch (label)
                {
                    case OcrTemplate.OcrLabels.NameSpecies:
                        if (ocrConfig.RecognitionPatterns.TrainingSettings.SkipName)
                        {
                            dinoName = string.Empty;
                            continue;
                        }
                        break;
                    case OcrTemplate.OcrLabels.Tribe:
                        if (ocrConfig.RecognitionPatterns.TrainingSettings.SkipTribe)
                        {
                            tribeName = string.Empty;
                            continue;
                        }

                        break;
                    case OcrTemplate.OcrLabels.Owner:
                        if (ocrConfig.RecognitionPatterns.TrainingSettings.SkipOwner)
                        {
                            ownerName = string.Empty;
                            continue;
                        }
                        break;
                }

                Rectangle rec = ocrConfig.UsedLabelRectangles[lbI];
                if (rec.IsEmpty)
                    continue;

                // wild creatures don't have the xp-bar, all stats are moved one row up
                if (wild && stI < 9)
                    rec.Offset(0, ocrConfig.UsedLabelRectangles[0].Top - ocrConfig.UsedLabelRectangles[1].Top);

                Bitmap testbmp = SubImage(screenShotBmp, rec.X, rec.Y, rec.Width, rec.Height);
                //AddBitmapToDebug(testbmp);

                string statOcr;

                try
                {
                    if (label == OcrTemplate.OcrLabels.NameSpecies)
                        statOcr = PatternOcr.ReadImageOcr(testbmp, false, whiteThreshold, rec.X, rec.Y, _ocrControl);
                    else if (label == OcrTemplate.OcrLabels.Level)
                        statOcr = PatternOcr.ReadImageOcr(testbmp, true, whiteThreshold, rec.X, rec.Y, _ocrControl).Replace(".", ": ");
                    else if (label == OcrTemplate.OcrLabels.Tribe || label == OcrTemplate.OcrLabels.Owner)
                        statOcr = PatternOcr.ReadImageOcr(testbmp, false, whiteThreshold, rec.X, rec.Y, _ocrControl);
                    else
                        statOcr = PatternOcr.ReadImageOcr(testbmp, true, whiteThreshold, rec.X, rec.Y, _ocrControl).Trim('.'); // statValues are only numbers
                }
                catch (OperationCanceledException)
                {
                    OcrText = "Canceled";
                    return finalValues;
                }

                if (statOcr == string.Empty &&
                    (label == OcrTemplate.OcrLabels.Health || label == OcrTemplate.OcrLabels.Imprinting || label == OcrTemplate.OcrLabels.Tribe || label == OcrTemplate.OcrLabels.Owner))
                {
                    if (wild && label == OcrTemplate.OcrLabels.Health)
                    {
                        stI--;
                        wild = false;
                    }
                    continue; // these can be missing, it's fine
                }

                finishedText += $"{(finishedText.Length == 0 ? string.Empty : "\r\n")}{label}:\t{statOcr}";

                // parse the OCR String

                var r = new Regex(@"^[_\/\\]*(.*?)[_\/\\]*$");
                statOcr = r.Replace(statOcr, "$1");

                if (label == OcrTemplate.OcrLabels.NameSpecies)
                {
                    r = new Regex(@".*?([♂♀])?[_.,-\/\\]*([^♂♀]+?)(?:[\(\[]([^\[\(\]\)]+)[\)\]]$|$)");
                }
                else if (label == OcrTemplate.OcrLabels.Owner || label == OcrTemplate.OcrLabels.Tribe)
                    r = new Regex(@"(.*)");
                else if (label == OcrTemplate.OcrLabels.Level)
                    r = new Regex(@".*\D(\d+)");
                else
                {
                    r = new Regex(@"(?:[\d.,%\/]*\/)?(\d+[\.,']?\d?)(%)?\.?"); // only the second numbers is interesting after the current weight is not shown anymore
                }

                MatchCollection mc = r.Matches(statOcr);

                if (mc.Count == 0)
                {
                    if (label == OcrTemplate.OcrLabels.NameSpecies || label == OcrTemplate.OcrLabels.Owner || label == OcrTemplate.OcrLabels.Tribe)
                        continue;
                    //if (statName == "Torpor")
                    //{
                    //    // probably it's a wild creature
                    //    // todo
                    //}
                    //else
                    //{
                    finishedText += $"error reading stat {label}";
                    finalValues[stI] = 0;
                    continue;
                    //}
                }

                if (label == OcrTemplate.OcrLabels.NameSpecies || label == OcrTemplate.OcrLabels.Owner || label == OcrTemplate.OcrLabels.Tribe)
                {
                    if (label == OcrTemplate.OcrLabels.NameSpecies && mc[0].Groups.Count > 0)
                    {
                        if (mc[0].Groups[1].Value == "♀")
                            sex = Sex.Female;
                        else if (mc[0].Groups[1].Value == "♂")
                            sex = Sex.Male;
                        dinoName = mc[0].Groups[2].Value;
                        species = mc[0].Groups[3].Value;
                        if (species.Length == 0)
                            species = dinoName;

                        // remove non-letter chars
                        r = new Regex(@"[^a-zA-Z]");
                        species = r.Replace(species, string.Empty);
                        // replace capital I with lower l (common misrecognition)
                        r = new Regex(@"(?<=[a-z])I(?=[a-z])");
                        species = r.Replace(species, "l");
                        // readd spaces before capital letters
                        //r = new Regex("(?<=[a-z])(?=[A-Z])");
                        //species = r.Replace(species, " ");

                        finishedText += $"\t→ {sex}, {species}";
                        dinoName = FinishCleanupOcrText(dinoName, ref finishedText);
                        species = FinishCleanupOcrText(species, ref finishedText);
                    }
                    else if (label == OcrTemplate.OcrLabels.Owner && mc[0].Groups.Count > 0)
                    {
                        ownerName = mc[0].Groups[0].Value;
                        finishedText += $"\t→ {ownerName}";
                        ownerName = FinishCleanupOcrText(ownerName, ref finishedText);
                    }
                    else if (label == OcrTemplate.OcrLabels.Tribe && mc[0].Groups.Count > 0)
                    {
                        tribeName = mc[0].Groups[0].Value;
                        finishedText += $"\t→ {tribeName}";
                        tribeName = FinishCleanupOcrText(tribeName, ref finishedText);
                    }
                    continue;
                }

                if (mc[0].Groups.Count > 2 && mc[0].Groups[2].Value == "%" && label == OcrTemplate.OcrLabels.Weight)
                {
                    // first stat with a '%' is damage, if oxygen is missing, shift all stats by one
                    finalValues[4] = finalValues[3]; // shift food to weight
                    finalValues[3] = finalValues[2]; // shift oxygen to food
                    finalValues[2] = 0; // set oxygen (which wasn't there) to 0
                    stI++;
                }

                var splitRes = statOcr.Split('/', ',', ':');
                var ocrValue = splitRes[splitRes.Length - 1] == "%" ? splitRes[splitRes.Length - 2] : splitRes[splitRes.Length - 1];

                ocrValue = PatternOcr.RemoveNonNumeric(ocrValue);

                double.TryParse(ocrValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out double v); // common substitutions: comma and apostrophe to dot, 

                finishedText += label == OcrTemplate.OcrLabels.Level ? $"\t→ {v:F0}" : $"\t→ {v:F1}";

                // TODO: test here that the read stat name corresponds to the stat supposed to be read
                finalValues[stI] = v;

                string FinishCleanupOcrText(string s, ref string outputText)
                {
                    var t = s.Replace("�", string.Empty);
                    if (regexReplacings == null) return t;

                    var beforeReplacements = t;

                    try
                    {
                        foreach (var regexReplacing in regexReplacings)
                        {
                            t = Regex.Replace(t, regexReplacing.search, regexReplacing.replace);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxes.ShowMessageBox($"Custom OCR replacement error:\n{ex.Message}", "Custom OCR replacement");
                    }

                    if (beforeReplacements != t)
                    {
                        outputText += $"\t→ {t}";
                    }

                    return t;
                }
            }

            OcrText = finishedText;

            // TODO reorder stats to match 12-stats-order

            return finalValues;
        }

        ///// <summary>
        ///// Calculates the match between the test-array and the templateArray, represented in a float from 0 to 1.
        ///// </summary>
        ///// <param name="test">The array to test</param>
        ///// <param name="templateArray">The existing template to which the test will be compared.</param>
        ///// <param name="match">match, 0 no equal pixels, 1 all pixels are identical.</param>
        ///// <param name="offset">0 no shift. 1 the test is shifted one pixel to the right. -1 the test is shifted one pixel to the left.</param>
        //public static void letterMatch(uint[] test, uint[] templateArray, out float match, out int offset)
        //{
        //    match = 0;
        //    offset = 0;
        //    // test letter and also shifted by one pixel (offset)
        //    for (int currentOffset = -1; currentOffset < 2; currentOffset++)
        //    {
        //        int testOffset = currentOffset > 0 ? currentOffset : 0;
        //        int templateOffset = currentOffset < 0 ? -currentOffset : 0;

        //        uint HammingDiff = 0;
        //        int maxTestRange = Math.Min(test.Length, templateArray.Length);
        //        for (int y = 1; y < maxTestRange; y++)
        //            HammingDiff += HammingWeight.HWeight((test[y] << testOffset) ^ templateArray[y] << templateOffset);
        //        if (test.Length > templateArray.Length)
        //        {
        //            for (int y = maxTestRange; y < test.Length; y++)
        //                HammingDiff += HammingWeight.HWeight((test[y] << testOffset));
        //        }
        //        else if (test.Length < templateArray.Length)
        //        {
        //            for (int y = maxTestRange; y < templateArray.Length; y++)
        //                HammingDiff += HammingWeight.HWeight(templateArray[y] << templateOffset);
        //        }
        //        long total = (Math.Max(test.Length, templateArray.Length) - 1) * Math.Max(test[0], templateArray[0]);
        //        float newMatch;
        //        if (total > 10)
        //            newMatch = (float)(total - HammingDiff) / total;
        //        else
        //            newMatch = 1 - HammingDiff / 10f;

        //        if (newMatch > match)
        //        {
        //            match = newMatch;
        //            offset = currentOffset;
        //        }
        //    }
        //}

        internal void SetOcrControl(OCRControl ocrControlObject)
        {
            _ocrControl = ocrControlObject;
        }

        private Process _screenCaptureProcess;
        public bool IsDinoInventoryVisible()
        {
            if (_screenCaptureProcess == null)
            {
                _screenCaptureProcess = Process.GetProcessesByName(screenCaptureApplicationName).FirstOrDefault();
                if (_screenCaptureProcess == null)
                    return false;
            }

            if (Win32API.GetForegroundWindow() != _screenCaptureProcess.MainWindowHandle)
                return false;

            Bitmap screenshotBmp = Win32API.GetScreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture);

            if (screenshotBmp == null
                || !CheckResolutionSupportedByOcr(screenshotBmp))
                return false;

            const OcrTemplate.OcrLabels label = OcrTemplate.OcrLabels.Level;
            Rectangle rec = ocrConfig.UsedLabelRectangles[(int)label];
            Bitmap bmp = SubImage(screenshotBmp, rec.X, rec.Y, rec.Width, rec.Height);
            string statOCR = PatternOcr.ReadImageOcr(bmp, true, Properties.Settings.Default.OCRWhiteThreshold);

            return Regex.IsMatch(statOCR, @":\d+$");
        }

        /// <summary>
        /// Loads the replacings file for manual regex corrections
        /// </summary>
        internal void LoadReplacingsFile()
        {
            var filePath = FileService.GetJsonPath(FileService.OcrFolderName, FileService.OcrReplacingsFile);
            if (ocrConfig == null || string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                RegexReplacingsStatus = null;
                return;
            }

            var lines = File.ReadAllLines(filePath);

            var regex = new Regex(@"([^@]+)@(.*)");

            var replacings = new List<(string, string)>();
            foreach (var l in lines)
            {
                var m = regex.Match(l);
                if (!m.Success) continue;
                replacings.Add((m.Groups[1].Value, m.Groups[2].Value));
            }

            RegexReplacingsStatus = $"{replacings.Count} replacements loaded at {DateTime.Now}";
            regexReplacings = replacings.Any() ? replacings.ToArray() : null;
        }
    }
}

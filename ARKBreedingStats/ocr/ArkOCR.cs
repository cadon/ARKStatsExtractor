using ARKBreedingStats.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ARKBreedingStats.ocr
{
    public class ArkOCR
    {
        // Class initially created by Nakram
        public int whiteThreshold = 155;
        public OCRTemplate ocrConfig;
        private static ArkOCR _OCR;
        private static OCRControl _ocrControl;
        //private readonly Dictionary<string, Point> _lastLetterPositions = new Dictionary<string, Point>(); // TODO remove?
        public string screenCaptureApplicationName;
        private Process _screenCaptureProcess;
        public int waitBeforeScreenCapture;
        public bool enableOutput = false;

        public static ArkOCR OCR => _OCR ?? (_OCR = new ArkOCR());

        public ArkOCR()
        {
            screenCaptureApplicationName = Properties.Settings.Default.OCRApp;

            Process[] p = Process.GetProcessesByName(screenCaptureApplicationName);
            if (p.Length > 0)
                _screenCaptureProcess = p[0];

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
        public bool CheckResolutionSupportedByOcr(Bitmap screenshot)
        {
            if (screenshot == null
                || ocrConfig == null)
                return false;

            if (screenshot.Width == ocrConfig.resolutionWidth && screenshot.Height == ocrConfig.resolutionHeight)
                return true;

            return false; // resolution not supported
        }

        public Bitmap SubImage(Bitmap source, int x, int y, int width, int height)
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
            byte[] rgbValues = new byte[numBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

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
                        int sum = rgbValues[idx] + rgbValues[idx + 1] + rgbValues[idx + 2];

                        //grey = Math.Max(Math.Max(rgbValues[idx], rgbValues[idx + 1]), rgbValues[idx + 2]);
                        grey = Math.Max(rgbValues[idx], rgbValues[idx + 1]); // ignoring the blue-channel, ui is blueish
                    }
                    else
                    {
                        //int sum = rgbValues[idx] + rgbValues[idx + 1] + rgbValues[idx + 2];
                        int sum = rgbValues[idx] + rgbValues[idx + 1]; // ignoring the blue-channel, ui is blueish

                        grey = (byte)(sum / 2);
                    }

                    rgbValues[idx] = grey;
                    rgbValues[idx + 1] = grey;
                    rgbValues[idx + 2] = grey;

                }
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            dest.UnlockBits(bmpData);

            return dest;
        }

        public static Bitmap removePixelsUnderThreshold(Bitmap source, int threshold, bool disposeSource = false)
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
                    //// if a neighbour-pixel (up, right, down or left) is above the threshold, also set this ambigious pixel to white
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

        // function currently unused. ARK seems to be scaling down larger fonts rather than using entire pixel heights
        public bool calibrateFromFontFile(int pixelSize, string calibrationText)
        {
            if (MessageBox.Show("All characters of the following set will replace any existing ocr-templates for the font size " + pixelSize + "px.\n\n"
                + calibrationText + "\n\nAre you sure?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return false;

            bool success = false;
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Font File (*.ttf)|*.ttf"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (PrivateFontCollection pfcoll = new PrivateFontCollection())
                    {
                        pfcoll.AddFontFile(dlg.FileName);
                        FontFamily ff = pfcoll.Families[0];

                        using (Bitmap bitmap = new Bitmap(31, pixelSize))
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        using (Font f = new Font(ff, 72f / 96 * pixelSize, FontStyle.Regular))
                        {
                            foreach (char c in calibrationText)
                            {
                                graphics.FillRectangle(Brushes.Black, 0, 0, 31, pixelSize);
                                graphics.DrawString(c.ToString(), f, Brushes.White, 0, -2);

                                bool foundLetter;
                                int letterStart = -1;
                                // look for the start pixel of the letter
                                do
                                {
                                    letterStart++;
                                    foundLetter = HasWhiteInVerticalLine(bitmap, letterStart, false);
                                }
                                while (!(foundLetter || letterStart >= bitmap.Width));
                                StoreImageInAlphabet(c, bitmap, letterStart, 31);
                            }
                        }
                    }
                    success = true;
                }
            }
            return success;
        }

        public void CalibrateFromImage(Bitmap source, string textInImage)
        {
            int ocrIndex = ocrConfig.fontSizeIndex(source.Height, true);
            int posXInImage = 0;

            // iterate for each letter in the text image
            for (int i = 0; i < textInImage.Length && posXInImage < source.Width; i++)
            {
                char letter = textInImage[i];
                if (letter == ' ')
                    continue;


                bool foundLetter = false;
                int letterStart = 0;
                int letterEnd = 0;

                // look for the start pixel of the letter
                while (!(foundLetter || posXInImage >= source.Width))
                {
                    foundLetter = HasWhiteInVerticalLine(source, posXInImage, false);
                    posXInImage++;
                }

                if (foundLetter)
                {
                    letterStart = posXInImage - 1;
                    // look for the end of the letter
                    do
                    {
                        posXInImage++;
                    } while (HasWhiteInVerticalLine(source, posXInImage, true) && posXInImage < source.Width);
                    letterEnd = posXInImage;
                }

                // store the image in the alphabet
                if (ocrConfig.letters[ocrIndex].IndexOf(letter) == -1 && posXInImage != source.Width)
                    StoreImageInAlphabet(letter, source, letterStart, letterEnd);
            }
        }

        public int lastLetterPosition(Bitmap source)
        {
            for (int i = source.Width; i > 0; i--)
            {
                if (HasWhiteInVerticalLine(source, i, false))
                    return i + 1;
            }
            return 0;
        }

        private void StoreImageInAlphabet(char letter, Bitmap source, int letterStart, int letterEnd)
        {
            int ocrIndex = ocrConfig.fontSizeIndex(source.Height, true);
            Rectangle cropRect = new Rectangle(letterStart, 0, letterEnd - letterStart, source.Height);
            if (cropRect.Width > 0 && cropRect.Height > 0)
            {
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }

                int lI = ocrConfig.letters[ocrIndex].IndexOf(letter);
                if (lI == -1)
                {
                    ocrConfig.letters[ocrIndex].Add(letter);
                    ocrConfig.letterArrays[ocrIndex].Add(letterArray(target));
                }
                else
                {
                    ocrConfig.letters[ocrIndex][lI] = letter;
                    ocrConfig.letterArrays[ocrIndex][lI] = letterArray(target);
                }

                //if ((letter >= 'A' && letter <= 'Z') || (letter >= 'a' && letter <= 'z'))
                //    target.Save(@"D:\arktest\" + source.Height.ToString() + "_" + letter.ToString() + ".png", ImageFormat.Png); // todo remove debug
                //if (letter == '|')
                //    target.Save(@"D:\arktest\" + source.Height.ToString() + "__Pipe.png", ImageFormat.Png); // todo remove debug
            }
        }

        private bool HasWhiteInVerticalLine(Bitmap source, int posXInImage, bool needsPreviousWhite)
        {
            bool hasWhite = false;
            if (posXInImage >= source.Width)
                return false;

            int greys = source.Height / 4;

            for (int h = 0; h < source.Height; h++)
            {
                if (source.GetPixel(posXInImage, h).R == 255)
                {
                    if (!needsPreviousWhite || posXInImage == 0)
                    {
                        hasWhite = true;
                        break;
                    }
                    // check if that white has a connected white previously (for handling kernel)
                    for (int hh = Math.Max(0, h - 1); hh < source.Height && hh < h + 2; hh++)
                    {
                        if (source.GetPixel(posXInImage - 1, hh).R == 255)
                        {
                            hasWhite = true;
                            break;
                        }
                    }
                }
                else if (source.GetPixel(posXInImage, h).R > 0)
                {
                    greys--;
                    if (greys == 0)
                    {
                        hasWhite = true;
                        break;
                    }
                }
            }
            return hasWhite;
        }

        // todo not used, remove?
        //private static Rectangle letterRect(Bitmap source, int hStart, int hEnd)
        //{
        //    int startWhite = -1, endWhite = -1;
        //    for (int j = 0; j < source.Height; j++)
        //    {
        //        for (int i = hStart; i < hEnd; i++)
        //        {
        //            if (startWhite == -1 && source.GetPixel(i, j).R == 255)
        //            {
        //                startWhite = j;
        //            }

        //            if (endWhite == -1 && source.GetPixel(i, (source.Height - j) - 1).R == 255)
        //            {
        //                endWhite = (source.Height - j);
        //            }
        //            if (startWhite != -1 && endWhite != -1)
        //                return new Rectangle(hStart, startWhite, hEnd - hStart, endWhite - startWhite);
        //        }
        //    }


        //    return Rectangle.Empty;
        //}

        public double[] DoOcr(out string OCRText, out string dinoName, out string species, out string ownerName, out string tribeName, out Sex sex, string useImageFilePath = "", bool changeForegroundWindow = true)
        {
            string finishedText = "";
            dinoName = "";
            species = "";
            ownerName = "";
            tribeName = "";
            sex = Sex.Unknown;
            double[] finalValues = new double[1] { 0 };
            if (ocrConfig == null)
            {
                OCRText = "Error: no ocr configured";
                return finalValues;
            }

            Bitmap screenshotbmp;

            _ocrControl.debugPanel.Controls.Clear();
            _ocrControl.ClearLists();

            if (System.IO.File.Exists(useImageFilePath))
            {
                screenshotbmp = (Bitmap)Image.FromFile(useImageFilePath);
            }
            else
            {
                // grab screenshot from ark
                screenshotbmp = Win32API.GetScreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture, true);
            }
            if (screenshotbmp == null)
            {
                OCRText = "Error: no image for OCR. Is ARK running?";
                return finalValues;
            }

            if (!CheckResolutionSupportedByOcr(screenshotbmp))
            {
                OCRText = "Error while calibrating: The game-resolution is not supported by the currently loaded OCR-configuration.\n"
                    + $"The tested image has a resolution of {screenshotbmp.Width.ToString()} × {screenshotbmp.Height.ToString()} px,\n"
                    + $"the resolution of the loaded ocr-config is {ocrConfig.resolutionWidth} × {ocrConfig.resolutionHeight} px.\n\n"
                    + "Load a ocr-config file with the resolution of the game to make it work.";
                return finalValues;
            }

            // TODO resize image according to resize-factor. used for large screenshots
            if (ocrConfig.resize != 1 && ocrConfig.resize > 0)
            {
                Bitmap resized = new Bitmap((int)(ocrConfig.resize * ocrConfig.resolutionWidth), (int)(ocrConfig.resize * ocrConfig.resolutionHeight));
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
                        graphics.DrawImage(screenshotbmp, new Rectangle(0, 0, 1920, 1080), 0, 0, screenshotbmp.Width, screenshotbmp.Height, GraphicsUnit.Pixel, wrapMode);
                    }

                    screenshotbmp?.Dispose();
                    screenshotbmp = resized;
                }
            }

            if (enableOutput && _ocrControl != null)
            {
                _ocrControl.AddBitmapToDebug(screenshotbmp);
                _ocrControl.SetScreenshot(screenshotbmp);
            }

            finalValues = new double[ocrConfig.labelRectangles.Count];
            finalValues[8] = -1; // set imprinting to -1 to mark it as unknown and to set a difference to a creature with 0% imprinting.

            if (changeForegroundWindow)
                Win32API.SetForegroundWindow(Application.OpenForms[0].Handle);


            bool wild = false; // todo: set to true and find out if the creature is wild in the first loop
            int stI = -1;
            for (int lbI = 0; lbI < ocrConfig.labelNames.Count; lbI++)
            {
                stI++;
                if (lbI == 8) stI = 8;
                string statName = ocrConfig.labelNames[stI];

                Rectangle rec = ocrConfig.labelRectangles[lbI];

                // wild creatures don't have the xp-bar, all stats are moved one row up
                if (wild && stI < 9)
                    rec.Offset(0, ocrConfig.labelRectangles[0].Top - ocrConfig.labelRectangles[1].Top);

                Bitmap testbmp = SubImage(screenshotbmp, rec.X, rec.Y, rec.Width, rec.Height);
                //AddBitmapToDebug(testbmp);

                string statOCR = "";

                if (statName == "NameSpecies")
                    statOCR = readImage(testbmp, true, false);
                else if (statName == "Level")
                    statOCR = readImage(testbmp, true, true);
                else if (statName == "Tribe" || statName == "Owner")
                    statOCR = readImage(testbmp, true, false);
                else
                    statOCR = readImage(testbmp, true, true); // statvalues are only numbers

                if (statOCR == "" &&
                    (statName == "Health" || statName == "Imprinting" || statName == "Tribe" || statName == "Owner"))
                {
                    if (wild && statName == "Health")
                    {
                        stI--;
                        wild = false;
                    }
                    continue; // these can be missing, it's fine
                }


                //_lastLetterPositions[statName] = new Point(rec.X + lastLetterPosition(removePixelsUnderThreshold(GetGreyScale(testbmp), whiteThreshold)), rec.Y); // TODO remove?

                finishedText += (finishedText.Length == 0 ? "" : "\r\n") + statName + ":\t" + statOCR;

                // parse the OCR String

                Regex r;
                r = new Regex(@"^[_\/\\]*(.*?)[_\/\\]*$"); // trim. often the background is misinterpreted as underscores or slash/backslash
                statOCR = r.Replace(statOCR, "$1");

                if (statName == "NameSpecies")
                {
                    r = new Regex(@".*?([♂♀])?[_.,-\/\\]*([^♂♀]+?)(?:[\(\[]([^\[\(\]\)]+)[\)\]]$|$)");
                }
                else if (statName == "Owner" || statName == "Tribe")
                    r = new Regex(@"(.*)");
                else if (statName == "Level")
                    r = new Regex(@".*\D(\d+)");
                else
                {
                    r = new Regex(@"(?:[\d.,%\/]*\/)?(\d+[\.,']?\d?)(%)?"); // only the second numbers is interesting after the current weight is not shown anymore

                    //if (onlyNumbers)
                    //r = new Regex(@"((\d*[\.,']?\d?\d?)\/)?(\d*[\.,']?\d?\d?)");
                    //else
                    // r = new Regex(@"([a-zA-Z]*)[:;]((\d*[\.,']?\d?\d?)\/)?(\d*[\.,']?\d?\d?)");
                }

                MatchCollection mc = r.Matches(statOCR);

                if (mc.Count == 0)
                {
                    if (statName == "NameSpecies" || statName == "Owner" || statName == "Tribe")
                        continue;
                    //if (statName == "Torpor")
                    //{
                    //    // probably it's a wild creature
                    //    // todo
                    //}
                    //else
                    //{
                    finishedText += "error reading stat " + statName;
                    finalValues[stI] = 0;
                    continue;
                    //}
                }

                if (statName == "NameSpecies" || statName == "Owner" || statName == "Tribe")
                {
                    if (statName == "NameSpecies" && mc[0].Groups.Count > 0)
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
                        r = new Regex("[^a-zA-Z]");
                        species = r.Replace(species, "");
                        // replace capital I with lower l (common misrecognition)
                        r = new Regex("(?<=[a-z])I(?=[a-z])");
                        species = r.Replace(species, "l");
                        // readd spaces before capital letters
                        r = new Regex("(?<=[a-z])(?=[A-Z])");
                        species = r.Replace(species, " ");

                        finishedText += "\t→ " + sex.ToString() + ", " + species;
                    }
                    else if (statName == "Owner" && mc[0].Groups.Count > 0)
                    {
                        ownerName = mc[0].Groups[0].Value;
                        finishedText += "\t→ " + ownerName;
                    }
                    else if (statName == "Tribe" && mc[0].Groups.Count > 0)
                    {
                        tribeName = mc[0].Groups[0].Value.Replace("Tobe", "Tribe").Replace("Tdbe", "Tribe").Replace("Tribeof", "Tribe of ");
                        finishedText += "\t→ " + tribeName;
                    }
                    continue;
                }

                if (mc[0].Groups.Count > 2 && mc[0].Groups[2].Value == "%" && statName == "Weight")
                {
                    // first stat with a '%' is damage, if oxygen is missing, shift all stats by one
                    finalValues[4] = finalValues[3]; // shift food to weight
                    finalValues[3] = finalValues[2]; // shift oxygen to food
                    finalValues[2] = 0; // set oxygen (which wasn't there) to 0
                    stI++;
                }

                double.TryParse(mc[0].Groups[1].Value.Replace('\'', '.').Replace(',', '.').Replace('O', '0'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out double v); // common substitutions: comma and apostrophe to dot, 

                finishedText += $"\t→ {v}";

                // TODO: test here that the read stat name corresponds to the stat supposed to be read
                finalValues[stI] = v;
            }

            OCRText = finishedText;

            // TODO reorder stats to match 12-stats-order

            return finalValues;

            /*
            Bitmap grab = Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName);
            AddBitmapToDebug(grab);

            //grab.Save("E:\\Temp\\Calibration8.png", ImageFormat.Png);
            if (changeForegroundWindow)
                Win32Stuff.SetForegroundWindow(Application.OpenForms[0].Handle);
            */
        }

        private string readImageAtCoords(Bitmap source, int x, int y, int width, int height, bool onlyMaximalMatches, bool onlyNumbers, bool writingInWhite = true)
        {
            return readImage(SubImage(source, x, y, width, height), onlyMaximalMatches, onlyNumbers, writingInWhite);
        }

        //// for debugging. reads a small image
        //public void debugReadImage(Bitmap source)
        //{
        //    string result = readImage(source, true, false);
        //}

        private string readImage(Bitmap source, bool onlyMaximalMatches, bool onlyNumbers, bool writingInWhite = true)
        {
            string result = "";
            int fontSize = source.Height;
            int ocrIndex = ocrConfig.fontSizeIndex(fontSize);
            if (ocrIndex == -1)
                return "error: font-size is " + fontSize + ", no calibration-data found for that.";

            //Bitmap[] theAlphabet = alphabets[fontSize]; // todo remove
            //uint[][] theAlphabetI = alphabetsI[fontSize];
            List<uint[]> letterArrays = ocrConfig.letterArrays[ocrIndex];
            List<char> letters = ocrConfig.letters[ocrIndex];
            List<int> reducedIndices = ocrConfig.reducedIndices[ocrIndex];

            Bitmap cleanedImage = removePixelsUnderThreshold(GetGreyScale(source, !writingInWhite), whiteThreshold);
            //AddBitmapToDebug(cleanedImage); // todo comment out
            //source.Save(@"D:\Temp\debug.png"); // TODO comment out
            //cleanedImage.Save(@"D:\Temp\debug_cleaned.png"); // save cleaned part of the image that will be read. TODO comment out


            for (int x = 0; x < cleanedImage.Width; x++)
            {
                bool foundLetter = false;
                int letterStart = 0;
                int letterEnd = 0;

                // look for the start pixel of the letter
                while (!(foundLetter || x >= cleanedImage.Width))
                {
                    foundLetter = HasWhiteInVerticalLine(cleanedImage, x, false);
                    x++;
                }

                if (foundLetter)
                {
                    letterStart = x - 1;
                    // look for the end of the letter
                    do
                    {
                        x++;
                    } while (HasWhiteInVerticalLine(cleanedImage, x, true) && x < cleanedImage.Width - 1);
                    letterEnd = x;
                }
                if (letterEnd > cleanedImage.Width)
                    letterEnd = cleanedImage.Width;

                if (letterStart != letterEnd)
                {
                    // found a letter, see if a match can be found
                    Rectangle letterR = new Rectangle(letterStart, 0, letterEnd - letterStart, fontSize);

                    while (letterR.Width > 0 && letterR.Height > 0)
                    {
                        Bitmap testImage = SubImage(cleanedImage, letterR.Left, letterR.Top, letterR.Width, letterR.Height);
                        //testImage.Save(@"D:\Temp\debug_letterfound.png");// TODO comment out
                        Dictionary<int, float> matches = new Dictionary<int, float>();
                        Dictionary<int, int> offsets = new Dictionary<int, int>();
                        float bestMatch = 0;
                        uint[] HWs = letterArray(testImage);

                        int maxLetters = onlyNumbers ? reducedIndices.Count : letterArrays.Count;
                        for (int lI = 0; lI < maxLetters; lI++)
                        {
                            int l = lI;
                            if (onlyNumbers) l = reducedIndices[lI];

                            letterMatch(HWs, letterArrays[l], out float match, out int offset);

                            if (match > 0.5)
                            {
                                // string letter = ocrConfig.letters[ocrIndex][l].ToString(); // TODO comment out debugging
                                matches[l] = match;
                                offsets[l] = offset;
                                if (bestMatch < match)
                                    bestMatch = match;
                                if (match == 1)
                                    break;
                            }
                        }

                        if (matches.Count == 0)
                        {
                            // if no matches were found, try again, but cut off the first two pixel-columns
                            letterStart += 2;
                            if (letterEnd - letterStart > 1)
                                letterR = new Rectangle(letterStart, 0, letterEnd - letterStart, fontSize);
                            else letterR = new Rectangle(0, 0, 0, 0);
                            continue;
                        }

                        Dictionary<int, float> goodMatches = new Dictionary<int, float>();

                        if (matches.Count == 1)
                            goodMatches = matches;
                        else
                        {
                            foreach (KeyValuePair<int, float> kv in matches)
                                if (kv.Value > 0.95 * bestMatch)
                                    goodMatches[kv.Key] = kv.Value; // discard matches that are not at least 95% as good as the best match
                        }

                        //// debugging / TODO
                        //// save recognized image and two best matches with percentage
                        //goodMatches = goodMatches.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

                        //Bitmap debugImg = new Bitmap(75, 36);
                        //using (Graphics g = Graphics.FromImage(debugImg))
                        //using (Font font = new Font("Arial", 8))
                        //{
                        //    g.FillRectangle(Brushes.DarkGray, 0, 0, debugImg.Width, debugImg.Height);
                        //    g.DrawImage(testImage, 1, 1, testImage.Width, testImage.Height);
                        //    int i = testImage.Width + 3;

                        //    foreach (int l in goodMatches.Keys)
                        //    {
                        //        string letter = ocrConfig.letters[ocrIndex][l].ToString();
                        //        for (int y = 0; y < ocrConfig.letterArrays[ocrIndex][l].Length - 1; y++)
                        //        {
                        //            uint row = ocrConfig.letterArrays[ocrIndex][l][y + 1];
                        //            int lx = 0;
                        //            while (row > 0)
                        //            {
                        //                if ((row & 1) == 1)
                        //                    g.FillRectangle(Brushes.White, i + lx, y, 1, 1);
                        //                row = row >> 1;
                        //                lx++;
                        //            }
                        //        }

                        //        g.DrawString(Math.Round(goodMatches[l] * 100).ToString(), font, (bestMatch == goodMatches[l] ? Brushes.Green : Brushes.Red), i, 25);
                        //        i += 22;
                        //    }
                        //    debugImg.Save(@"D:\Temp\debug_letter" + DateTime.Now.ToString("HHmmss\\-fffffff\\-") + x + ".png");
                        //}
                        //// end debugging

                        if (goodMatches.Count == 1)
                        {
                            int l = goodMatches.Keys.ToArray()[0];
                            char c = ocrConfig.letters[ocrIndex][l];
                            result += c;

                            if ((int)letterArrays[l][0] - offsets[l] > 0)
                                letterStart += (int)letterArrays[l][0] - offsets[l];
                            else letterStart += 1;

                            // add letter to list of recognized
                            if (enableOutput)
                                _ocrControl.AddLetterToRecognized(HWs, c, fontSize);
                        }
                        else
                        {
                            if (onlyMaximalMatches)
                            {
                                var letterChars = goodMatches.Select(p => letters[p.Key]).ToList();
                                foreach (int l in goodMatches.Keys)
                                {
                                    if (goodMatches[l] == bestMatch)
                                    {
                                        result += ocrConfig.letters[ocrIndex][l];

                                        // add letter to config
                                        if (enableOutput)
                                            _ocrControl.AddLetterToRecognized(HWs, ocrConfig.letters[ocrIndex][l], fontSize);

                                        if ((int)letterArrays[l][0] - offsets[l] > 0)
                                            letterStart += (int)letterArrays[l][0] - offsets[l];
                                        else letterStart += 1;

                                        break; // if there are multiple best matches take only the first
                                    }
                                }
                            }
                            else
                            {
                                bool bestMatchLetterForwareded = false;
                                result += "[";
                                foreach (int l in goodMatches.Keys)
                                {
                                    result += (char)l + goodMatches[l].ToString("{0.00}") + " ";
                                    if (!bestMatchLetterForwareded && goodMatches[l] == bestMatch)
                                    {
                                        if ((int)letterArrays[l][0] - offsets[l] > 0)
                                            letterStart += (int)letterArrays[l][0] - offsets[l];
                                        else letterStart += 1;
                                        bestMatchLetterForwareded = true;
                                    }
                                }
                                result += "]";
                            }
                        }
                        // check if the image contained another letter that couldn't be separated (kerning)
                        letterR = letterEnd - letterStart > 1 ?
                                new Rectangle(letterStart, 0, letterEnd - letterStart, fontSize) :
                                new Rectangle(0, 0, 0, 0);
                    }
                }
            }

            //addCalibrationImageToDebug(result, fontSize); // debugging

            //// replace half letters. // todo necessary?
            //result = result.Replace((char)15 + "n", "n");
            //result = result.Replace((char)16 + "lK", "K");

            return result;
        }

        /// <summary>
        /// Calculates the match between the test-array and the templateArray, represented in a float from 0 to 1.
        /// </summary>
        /// <param name="test">The array to test</param>
        /// <param name="templateArray">The existing template to which the test will be compared.</param>
        /// <param name="match">match, 0 no equal pixels, 1 all pixels are identical.</param>
        /// <param name="offset">0 no shift. 1 the test is shifted one pixel to the right. -1 the test is shifted one pixel to the left.</param>
        public static void letterMatch(uint[] test, uint[] templateArray, out float match, out int offset)
        {
            match = 0;
            offset = 0;
            // test letter and also shifted by one pixel (offset)
            for (int currentOffset = -1; currentOffset < 2; currentOffset++)
            {
                int testOffset = currentOffset > 0 ? currentOffset : 0;
                int templateOffset = currentOffset < 0 ? -currentOffset : 0;

                uint HammingDiff = 0;
                int maxTestRange = Math.Min(test.Length, templateArray.Length);
                for (int y = 1; y < maxTestRange; y++)
                    HammingDiff += HammingWeight.HWeight((test[y] << testOffset) ^ templateArray[y] << templateOffset);
                if (test.Length > templateArray.Length)
                {
                    for (int y = maxTestRange; y < test.Length; y++)
                        HammingDiff += HammingWeight.HWeight((test[y] << testOffset));
                }
                else if (test.Length < templateArray.Length)
                {
                    for (int y = maxTestRange; y < templateArray.Length; y++)
                        HammingDiff += HammingWeight.HWeight(templateArray[y] << templateOffset);
                }
                long total = (Math.Max(test.Length, templateArray.Length) - 1) * Math.Max(test[0], templateArray[0]);
                float newMatch;
                if (total > 10)
                    newMatch = (float)(total - HammingDiff) / total;
                else
                    newMatch = 1 - HammingDiff / 10f;

                if (newMatch > match)
                {
                    match = newMatch;
                    offset = currentOffset;
                }
            }
        }

        /*
        private void addCalibrationImageToDebug(string text, int fontSize)
        {
            Bitmap b = new Bitmap(200, 30);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
                int x = 0;
                foreach (char c in text)
                {
                    g.DrawImage(alphabets[fontSize][c], x, 5);
                    x += alphabets[fontSize][c].Width + 3;
                }
            }
            AddBitmapToDebug(b);
        }
        */

        /*
        // used for debugging. can be commented out for release
        static public void saveLetterArrayToFile(uint[] array, string file)
        {
            Bitmap debugImg = new Bitmap(36, 36);
            using (Graphics g = Graphics.FromImage(debugImg))
            using (Font font = new Font("Arial", 8))
            {
                g.FillRectangle(Brushes.DarkGray, 0, 0, debugImg.Width, debugImg.Height);

                for (int y = 0; y < array.Length - 1; y++)
                {
                    uint row = array[y + 1];
                    int x = 0;
                    while (row > 0)
                    {
                        if ((row & 1) == 1)
                            g.FillRectangle(Brushes.White, x, y, 1, 1);
                        row = row >> 1;
                        x++;
                    }
                }
                debugImg.Save(file);
            }
        }
        */

        internal void setOCRControl(OCRControl ocrControlObject)
        {
            _ocrControl = ocrControlObject;
        }

        public bool isDinoInventoryVisible()
        {
            if (_screenCaptureProcess == null)
            {
                Process[] p = Process.GetProcessesByName(screenCaptureApplicationName);
                if (p.Length > 0)
                    _screenCaptureProcess = p[0];
                else return false;
            }

            if (Win32API.GetForegroundWindow() != _screenCaptureProcess.MainWindowHandle)
                return false;

            Bitmap screenshotbmp = Win32API.GetScreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture);

            if (screenshotbmp == null)
                return false;
            if (!CheckResolutionSupportedByOcr(screenshotbmp))
                return false;

            const string statName = "Level";
            Rectangle rec = ocrConfig.labelRectangles[ocrConfig.labelNameIndices[statName]];
            Bitmap testbmp = SubImage(screenshotbmp, rec.X, rec.Y, rec.Width, rec.Height);
            string statOCR = readImage(testbmp, true, true);

            Regex r = new Regex(@":\d+$");
            MatchCollection mc = r.Matches(statOCR);

            return mc.Count != 0;
        }

        /// <summary>
        /// returns bit-array that represent a b/w-bitmap. At index 0 is the width
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        private uint[] letterArray(Bitmap letter)
        {
            // determine height
            int height = 0;
            for (int y = letter.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < letter.Width; x++)
                {
                    if (letter.GetPixel(x, y).R != 0)
                    {
                        height = y + 1;
                        break;
                    }
                }
                if (height != 0) break;
            }

            uint[] la = new uint[height + 1];
            la[0] = 0;
            for (int y = 0; y < height; y++)
            {
                uint row = 0;
                for (int x = 0; x < letter.Width && x < 32; x++) // max-width is 31px
                {
                    row += (letter.GetPixel(x, y).R == 0 ? 0 : (uint)(1 << x));
                }
                la[y + 1] = row;

                uint width = (uint)Math.Log(row, 2) + 1;
                if (width > la[0]) la[0] = width;
            }
            return la;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace ARKBreedingStats
{
    public class ArkOCR
    {
        // Class initially created by Nakram
        public int whiteThreshold = 155;
        public ocr.OCRTemplate ocrConfig = new ocr.OCRTemplate();
        private static ArkOCR _OCR;
        private static ocr.OCRControl ocrControl;
        public Dictionary<string, Point> lastLetterPositions = new Dictionary<string, Point>();
        public string screenCaptureApplicationName;
        public Process ScreenCaptureProcess;
        public int waitBeforeScreenCapture;
        public bool enableOutput = false;

        public static ArkOCR OCR
        {
            get
            {
                if (_OCR == null)
                {
                    _OCR = new ArkOCR();
                }
                return _OCR;
            }
        }


        public ArkOCR()
        {
            screenCaptureApplicationName = "ShooterGame";

            Process[] p = Process.GetProcessesByName(screenCaptureApplicationName);
            if (p.Length > 0)
                ScreenCaptureProcess = p[0];

            waitBeforeScreenCapture = 500;

            calibrate();
        }

        public void calibrate()
        {
            //Bitmap origBitmap, bmp;

            //origBitmap = Properties.Resources.ARKCalibration13;
            //bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThresholdCalibration);
            //CalibrateFromImage(13, bmp, "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            //origBitmap = Properties.Resources.ARKCalibration15;
            //bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThresholdCalibration);
            //CalibrateFromImage(15, bmp, "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            //origBitmap = Properties.Resources.ARKCalibration18;
            //bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThresholdCalibration);
            //CalibrateFromImage(18, bmp, "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            //// debugging
            //// save recognized alphabet
            //Bitmap debugImg = new Bitmap(1000, 50);
            //using (Graphics g = Graphics.FromImage(debugImg))
            //{
            //    g.FillRectangle(Brushes.Black, 0, 0, debugImg.Width, debugImg.Height);
            //    Font font = new Font("Arial", 8);
            //    int i = 1;
            //    int c = -1;
            //    foreach (Bitmap b in alphabets[15])
            //    {
            //        c++;
            //        if (b != null)
            //        {
            //            g.DrawImage(b, i, 1, b.Width, b.Height);
            //            g.DrawString(((char)c).ToString(), font, Brushes.White, i, 20);
            //            i += b.Width + 15;
            //        }
            //    }
            //    debugImg.Save(@"D:\Temp\alphabetDebug.png");
            //}
            //// end debugging
        }

        public bool setResolution()
        {
            return setResolution(Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture));
        }

        // figure out the current resolution and positions
        // return true if the calibration was successful
        public bool setResolution(Bitmap screenshot)
        {
            if (screenshot == null)
                return false;

            if (screenshot.Width == ocrConfig.resolutionWidth && screenshot.Height == ocrConfig.resolutionHeight)
                return true;

            return false; // resolution not supported

            // todo remove method-part below this line?
            /*

            //debugPanel.Controls.Clear();

            // positions depend on screen resolution.
            int resolutionW = 0, resolutionH = 0;
            Win32Stuff.Rect res = new Win32Stuff.Rect(); // Win32Stuff.GetWindowRect(screenCaptureApplicationName);

            res.left = 0;
            res.right = screenshot.Width;
            res.top = 0;
            res.bottom = screenshot.Height;

            if (resolutionW == 1920 && resolutionH == 1080)
            {
                int xStats = 950;
                int yStatsD = 43;
                int statWidth = 164;
                int yc = 0;
                // coords for 1920x1080
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(xStats, 508 + (yStatsD * (yc++)), statWidth, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(907, 192, 127, 18));
                ocrConfig.labelRectangles.Add(new Rectangle(846, 158, 228, 18));
                ocrConfig.labelRectangles.Add(new Rectangle(856, 230, 202, 15));
                ocrConfig.labelRectangles.Add(new Rectangle(967, 402, 92, 13));
            }
            //else if (resolutionW == 1680 && resolutionH == 1050)
            //{
            //    // coords for 1680x1050
            //    // 1680/1920 = height-factor; 50 = translation
            //    // not yet correct x_1080 |--> (x_1080+60)*1680/1920
            //    //statPositions["NameAndLevel"] = new Point(1111, 200);
            //    //statPositions["Health"] = new Point(1183, 595);
            //    //statPositions["Stamina"] = new Point(1183, 630);
            //    //statPositions["Oxygen"] = new Point(1183, 665);
            //    //statPositions["Food"] = new Point(1183, 691);
            //    //statPositions["Weight"] = new Point(1183, 755);
            //    //statPositions["Melee Damage"] = new Point(1183, 788);
            //    //statPositions["Movement Speed"] = new Point(1183, 817);
            //    //statPositions["Torpor"] = new Point(1183, 912);

            //    statPositions["NameAndLevel"] = new Point(1111, 200);
            //    statPositions["Health"] = new Point(1260, 595);
            //    statPositions["Stamina"] = new Point(1277, 630);
            //    statPositions["Oxygen"] = new Point(1271, 665);
            //    statPositions["Food"] = new Point(1249, 691);
            //    statPositions["Weight"] = new Point(1264, 755);
            //    statPositions["Melee Damage"] = new Point(1340, 788);
            //    statPositions["Movement Speed"] = new Point(1362, 817);
            //    statPositions["Torpor"] = new Point(1260, 912);
            //    statPositions["CurrentWeight"] = new Point(1, 1); // not correct, TODO
            //}
            else return false; // no supported resolution

            currentResolutionW = resolutionW;
            currentResolutionH = resolutionH;
            

            return true;
            */
        }

        private PictureBox AddBitmapToDebug(Bitmap bmp)
        {
            if (ocrControl.debugPanel != null)
            {
                PictureBox b = new PictureBox();
                b.SizeMode = PictureBoxSizeMode.AutoSize;
                b.Image = bmp;
                ocrControl.debugPanel.Controls.Add(b);
                ocrControl.debugPanel.Controls.SetChildIndex(b, 0);
                return b;
            }
            else
                return null;
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

        public Bitmap GetGreyScale(Bitmap source, bool writingInWhite = false)
        {
            Bitmap dest = (Bitmap)source.Clone();

            PixelFormat pxf = PixelFormat.Format24bppRgb;

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
                        grey = Math.Max(rgbValues[idx], rgbValues[idx + 1]); // ignoring the blue-channel, ui is bluish
                    }
                    else
                    {
                        //int sum = rgbValues[idx] + rgbValues[idx + 1] + rgbValues[idx + 2];
                        int sum = rgbValues[idx] + rgbValues[idx + 1]; // ignoring the blue-channel, ui is bluish

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

        public Bitmap removePixelsUnderThreshold(Bitmap source, int threshold)
        {
            Bitmap dest = (Bitmap)source.Clone();

            PixelFormat pxf = PixelFormat.Format24bppRgb;

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

            return dest;
        }

        // function currently unused. ARK seems to be scaling down larger fonts rather than using entire pixel heights
        public void calibrateFromFontFile(int pixelSize, string calibrationText)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Font File (*.ttf)|*.ttf";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PrivateFontCollection pfcoll = new PrivateFontCollection();
                pfcoll.AddFontFile(dlg.FileName);
                FontFamily ff = pfcoll.Families[0];

                Bitmap bitmap = new Bitmap(20, pixelSize);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Font f = new Font(ff, 72f / 96 * pixelSize, FontStyle.Regular))
                    {
                        foreach (char c in calibrationText)
                        {
                            graphics.FillRectangle(Brushes.Black, 0, 0, 20, pixelSize);
                            graphics.DrawString(c.ToString(), f, Brushes.White, 0, -2);

                            bool foundLetter = false;
                            int letterStart = -1;
                            // look for the start pixel of the letter
                            do
                            {
                                letterStart++;
                                foundLetter = HasWhiteInVerticalLine(bitmap, letterStart, false);
                            }
                            while (!(foundLetter || letterStart >= bitmap.Width));
                            StoreImageInAlphabet(c, bitmap, letterStart, 20);
                        }
                    }
                }
            }
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
                while (!(foundLetter == true || posXInImage >= source.Width))
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
                    else
                    {
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

        public float[] doOCR(out string OCRText, out string dinoName, out string species, out string ownerName, out string tribeName, out Sex sex, string useImageFilePath = "", bool changeForegroundWindow = true)
        {
            string finishedText = "";
            dinoName = "";
            species = "";
            ownerName = "";
            tribeName = "";
            sex = Sex.Unknown;
            float[] finalValues = new float[1] { 0 };

            Bitmap screenshotbmp = null;
            Bitmap testbmp;

            ocrControl.debugPanel.Controls.Clear();
            ocrControl.ClearLists();

            if (System.IO.File.Exists(useImageFilePath))
            {
                screenshotbmp = (Bitmap)Bitmap.FromFile(useImageFilePath);
            }
            else
            {
                // grab screenshot from ark
                screenshotbmp = Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture);
            }
            if (screenshotbmp == null)
            {
                OCRText = "Error: no image for OCR. Is ARK running?";
                return finalValues;
            }

            /*
            // TODO resize image does not work well
            if (screenshotbmp.Width != 1920 && screenshotbmp.Width != 1680)
            {
                Bitmap resized = new Bitmap(1920, 1080);
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
                    screenshotbmp = resized;
                }
            }
            */

            if (!setResolution(screenshotbmp))
            {
                OCRText = "Error while calibrating: probably game-resolution is not supported by this OCR-configuration.\nThe tested image has a resolution of " + screenshotbmp.Width.ToString() + "×" + screenshotbmp.Height.ToString() + " px.";
                return finalValues;
            }
            finalValues = new float[ocrConfig.labelRectangles.Count];

            if (enableOutput)
            {
                AddBitmapToDebug(screenshotbmp);
                ocrControl.setScreenshot(screenshotbmp);
            }

            if (changeForegroundWindow)
                Win32Stuff.SetForegroundWindow(Application.OpenForms[0].Handle);


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

                testbmp = SubImage(screenshotbmp, rec.X, rec.Y, rec.Width, rec.Height);
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


                lastLetterPositions[statName] = new Point(rec.X + lastLetterPosition(removePixelsUnderThreshold(GetGreyScale(testbmp), whiteThreshold)), rec.Y);

                finishedText += (finishedText.Length == 0 ? "" : "\r\n") + statName + ": " + statOCR;

                // parse the OCR String

                Regex r;
                r = new Regex(@"^[_\/\\]*(.*?)[_\/\\]*$"); // trim. often the background is misinterpreted as underscores or slash/backslash
                statOCR = r.Replace(statOCR, "$1");

                if (statName == "NameSpecies")
                {
                    r = new Regex(@"([♂♀])?(.+?)(?:[\(\[]([^\[\(\]\)]+)[\)\]]$|$)");
                }
                else if (statName == "Owner" || statName == "Tribe")
                    r = new Regex(@"(.*)");
                else if (statName == "Level")
                    r = new Regex(@".*:(\d*)");
                else
                {
                    r = new Regex(@"(?:\d+\.\d%?[\/1])?(\d+[\.,']?\d?\d?)(%)?"); // only the second numbers is interesting after the current weight is not shown anymore

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
                    else if (statName == "Torpor" && false)
                    {
                        // probably it's a wild creature
                        // todo
                    }
                    else
                    {
                        finishedText += "error reading stat " + statName;
                        finalValues[stI] = 0;
                        continue;
                    }
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
                        // todo remove this removal, does the new ui show this?
                        /*
                        // remove prefixes Baby, Juvenile and Adolescent
                        r = new Regex("^(?:Ba[bh]y|Juven[il]le|Adolescent) *");
                        dinoName = r.Replace(dinoName, "");
                        */

                        r = new Regex("[^a-zA-Z]");
                        species = r.Replace(species, "");
                        r = new Regex("([^^])([A-Z])");
                        species = r.Replace(species, "$1 $2");
                    }
                    else if (statName == "Owner" && mc[0].Groups.Count > 0)
                        ownerName = mc[0].Groups[0].Value;
                    else if (statName == "Tribe" && mc[0].Groups.Count > 0)
                        tribeName = mc[0].Groups[0].Value.Replace("Tobe", "Tribe");
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

                float v = 0;
                float.TryParse(mc[0].Groups[1].Value.Replace('\'', '.').Replace(',', '.').Replace('O', '0'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out v); // common substitutions: comma and apostrophe to dot, 

                // TODO: test here that the read stat name corresponds to the stat supposed to be read
                finalValues[stI] = v;
            }

            OCRText = finishedText;

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

        private string readImage(Bitmap source, bool onlyMaximalMatches, bool onlyNumbers, bool writingInWhite = true)
        {
            string result = "";
            int fontSize = source.Height;
            int ocrIndex = ocrConfig.fontSizeIndex(fontSize);
            if (ocrIndex == -1)
                return "not calibrated for this font-size!";

            //Bitmap[] theAlphabet = alphabets[fontSize]; // todo remove
            //uint[][] theAlphabetI = alphabetsI[fontSize];
            List<uint[]> letterArrays = ocrConfig.letterArrays[ocrIndex];
            List<char> letters = ocrConfig.letters[ocrIndex];
            List<int> reducedIndices = ocrConfig.reducedIndices[ocrIndex];

            Bitmap cleanedImage = removePixelsUnderThreshold(GetGreyScale(source, !writingInWhite), whiteThreshold);
            //AddBitmapToDebug(cleanedImage); // todo comment out
            //source.Save(@"D:\Temp\debug.png"); // TODO comment out
            //cleanedImage.Save(@"D:\Temp\debug_cleaned.png");// TODO comment out


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
                    //Rectangle letterR = letterRect(cleanedImage, letterStart, letterEnd);
                    //letterR.Y = 0; // todo test
                    //letterR.Height = source.Height; // todo test
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
                            float match;
                            int offset;

                            letterMatch(HWs, letterArrays[l], out match, out offset);

                            if (match > 0.5)
                            {
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
                        //Bitmap debugImg = new Bitmap(75, 36);
                        //using (Graphics g = Graphics.FromImage(debugImg))
                        //{
                        //    g.FillRectangle(Brushes.DarkGray, 0, 0, debugImg.Width, debugImg.Height);
                        //    g.DrawImage(testImage, 1, 1, testImage.Width, testImage.Height);
                        //    int i = testImage.Width + 20;
                        //    Font font = new Font("Arial", 8);

                        //    foreach (int l in goodMatches.Keys)
                        //    {
                        //        g.DrawImage(theAlphabet[l], i, 1, theAlphabet[l].Width, theAlphabet[l].Height);
                        //        g.DrawString(Math.Round(goodMatches[l] * 100).ToString(), font, (bestMatch == goodMatches[l] ? Brushes.Green : Brushes.Red), i, 25);
                        //        i += theAlphabet[l].Width + 15;
                        //    }
                        //    debugImg.Save(@"D:\Temp\debug_letter" + DateTime.Now.ToString("HHmmss\\-fffffff\\-") + x + ".png");
                        //}
                        //// end debugging

                        if (goodMatches.Count == 1)
                        {
                            int l = goodMatches.Keys.ToArray()[0];
                            char c = ocrConfig.letters[ocrIndex][l];
                            result += c;

                            letterStart += (int)letterArrays[l][0] + offsets[l];

                            // add letter to config
                            if (enableOutput)
                                ocrControl.addLetterToRecognized(HWs, c, fontSize);
                        }
                        else
                        {
                            if (onlyMaximalMatches)
                            {
                                foreach (int l in goodMatches.Keys)
                                {
                                    if (goodMatches[l] == bestMatch)
                                    {
                                        result += ocrConfig.letters[ocrIndex][l];

                                        // add letter to config
                                        if (enableOutput)
                                            ocrControl.addLetterToRecognized(HWs, ocrConfig.letters[ocrIndex][l], fontSize);

                                        letterStart += (int)letterArrays[l][0] + offsets[l];

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
                                        letterStart += (int)letterArrays[l][0] + offsets[l];
                                        bestMatchLetterForwareded = true;
                                    }
                                }
                                result += "]";
                            }
                        }
                        // check if the image contained another letter that couldn't be separated (kerning)
                        if (letterEnd - letterStart > 1)
                            letterR = new Rectangle(letterStart, 0, letterEnd - letterStart, fontSize);
                        else letterR = new Rectangle(0, 0, 0, 0);
                    }
                }
            }

            //addCalibrationImageToDebug(result, fontSize); // debugging

            //// replace half letters. // todo necessary?
            //result = result.Replace((char)15 + "n", "n");
            //result = result.Replace((char)16 + "lK", "K");

            return result;

        }

        static public void letterMatch(uint[] HWs, uint[] letterArray, out float match, out int offset)
        {
            match = 0;
            offset = 0;
            float newMatch = 0;
            for (int currentOffset = 0; currentOffset < 2; currentOffset++)
            {
                uint HammingDiff = 0;
                int maxTestRange = Math.Min(HWs.Length, letterArray.Length);
                for (int y = 1; y < maxTestRange; y++)
                    HammingDiff += ocr.HammingWeight.HWeight((HWs[y] >> currentOffset) ^ letterArray[y]);
                if (HWs.Length > letterArray.Length)
                {
                    for (int y = maxTestRange; y < HWs.Length; y++)
                        HammingDiff += ocr.HammingWeight.HWeight((HWs[y] >> currentOffset));
                }
                else if (HWs.Length < letterArray.Length)
                {
                    for (int y = maxTestRange; y < letterArray.Length; y++)
                        HammingDiff += ocr.HammingWeight.HWeight(letterArray[y]);
                }
                long total = (Math.Max(HWs.Length, letterArray.Length) - 1) * Math.Max(HWs[0], letterArray[0]);
                if (total > 10)
                    newMatch = ((float)(total - HammingDiff) / total);
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

        public float PercentageMatch(Bitmap img1, Bitmap img2)
        {
            /*
            // double size of images. todo. better? nope.
            var img1L = new Bitmap(img1.Width * 2, img1.Height * 2);
            using (Graphics g = Graphics.FromImage(img1L))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(img1, 0, 0, img1L.Width, img1L.Height);
            }
            img1 = img1L;
            var img2L = new Bitmap(img2.Width * 2, img2.Height * 2);
            using (Graphics g = Graphics.FromImage(img2L))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(img2, 0, 0, img2L.Width, img2L.Height);
            }
            img2 = img2L;
            */

            if (Math.Abs(img1.Width - img2.Width) > 3 || Math.Abs(img1.Height - img2.Height) > 3)
                return 0;


            double maxMatches = 0;


            for (int s = 0; s < 1; s++) // todo disabled for now, doesn't seem to improve recognition
            {
                // shift one image 1 to the right if width is different // currently not used, does not improve recognition
                int s1 = 0, s2 = 0;
                if (s == 1)
                {
                    if (img1.Width < img2.Width) s1 = 1;
                    else if (img1.Width > img2.Width) s2 = 1;
                    else break;
                }

                int minWidth = Math.Min(img1.Width, img2.Width);
                int minHeight = Math.Min(img1.Height, img2.Height);

                var b1cut = new Bitmap(minWidth, minHeight);
                using (Graphics g = Graphics.FromImage(b1cut))
                {
                    g.DrawImageUnscaled(img1, s1, 0);
                }
                var b2cut = new Bitmap(minWidth, minHeight);
                using (Graphics g = Graphics.FromImage(b2cut))
                {
                    g.DrawImageUnscaled(img2, s2, 0);
                }


                Bitmap b1 = new Bitmap(b1cut);
                Bitmap b2 = new Bitmap(b2cut);

                BitmapData bData1 = b1.LockBits(new Rectangle(0, 0, b1cut.Width, b1cut.Height), ImageLockMode.ReadOnly, b1.PixelFormat);
                BitmapData bData2 = b2.LockBits(new Rectangle(0, 0, b2cut.Width, b2cut.Height), ImageLockMode.ReadOnly, b2.PixelFormat);

                /*the size of the image in bytes */
                int size1 = bData1.Stride * bData1.Height;
                int size2 = bData2.Stride * bData2.Height;

                /*Allocate buffer for image*/
                byte[] data1 = new byte[size1];
                byte[] data2 = new byte[size2];

                /*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
                System.Runtime.InteropServices.Marshal.Copy(bData1.Scan0, data1, 0, size1);
                System.Runtime.InteropServices.Marshal.Copy(bData2.Scan0, data2, 0, size2);

                int sizeMin = Math.Min(size1, size2);

                double matches = 0;

                for (int i = 0; i < sizeMin; i += 3)
                {
                    if (data1[i] == 0)
                    {
                        if (data2[i] == 0)
                            matches++;
                        else if (data2[i] < 255)
                            matches += 0.2;
                    }
                    else if (data1[i] < 255)
                    {
                        if (data2[i] == 255)
                            matches += 0.6;
                        else if (data2[i] != 0)
                            matches += 0.8;
                        else matches += 0.2;
                    }
                    else
                    {
                        if (data2[i] == 255)
                            matches++;
                        else if (data2[i] != 0)
                            matches += 0.6;
                    }
                }
                if (matches > maxMatches) maxMatches = matches;
                b1.UnlockBits(bData1);
                b2.UnlockBits(bData2);
            }


            float percentage = (float)(maxMatches / (Math.Max(img1.Width, img2.Width) * Math.Max(img1.Height, img2.Height)));

            return percentage;
        }

        internal void setOCRControl(ocr.OCRControl ocrControlObject)
        {
            ocrControl = ocrControlObject;
        }

        public bool isDinoInventoryVisible()
        {
            if (ScreenCaptureProcess == null)
            {
                Process[] p = Process.GetProcessesByName(screenCaptureApplicationName);
                if (p.Length > 0)
                    ScreenCaptureProcess = p[0];
                else return false;
            }

            float[] finalValues = new float[1] { 0 };

            Bitmap screenshotbmp = null;// = (Bitmap)Bitmap.FromFile(@"D:\ScreenshotsArk\Clipboard12.png");

            if (Win32Stuff.GetForegroundWindow() != ScreenCaptureProcess.MainWindowHandle)
                return false;

            screenshotbmp = Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName, waitBeforeScreenCapture);

            if (screenshotbmp == null)
                return false;
            if (!setResolution(screenshotbmp))
                return false;

            string statName = "Level";
            Rectangle rec = ocrConfig.labelRectangles[ocrConfig.labelNameIndices[statName]];
            Bitmap testbmp = SubImage(screenshotbmp, rec.X, rec.Y, rec.Width, rec.Height);
            string statOCR = readImage(testbmp, true, false);

            Regex r = new Regex(@"\d+");
            MatchCollection mc = r.Matches(statOCR);

            if (mc.Count != 0)
                return true;

            return false;
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
            uint width = 0;
            for (int y = 0; y < height; y++)
            {
                uint row = 0;
                for (int x = 0; x < letter.Width && x < 32; x++) // max-width is 31px
                {
                    row += (letter.GetPixel(x, y).R == 0 ? 0 : (uint)(1 << x));
                }
                la[y + 1] = row;

                width = (uint)Math.Log(row, 2) + 1;
                if (width > la[0]) la[0] = width;
            }
            return la;
        }

    }
}

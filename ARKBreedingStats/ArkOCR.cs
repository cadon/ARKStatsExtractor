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
        // I'm very sorry for the quality of this code and its "hack"-ish nature.
        // -- Nakram
        int whiteThreshold = 155;
        public Dictionary<int, Bitmap[]> alphabets = new Dictionary<int, Bitmap[]>(); // font-size, then alphabet
        public Dictionary<int, Bitmap[]> reducedAlphabets = new Dictionary<int, Bitmap[]>();
        private double[] charWeighting = new double[255]; // contains weightings for chars
        //public Dictionary<Int64, List<byte>> hashMap = new Dictionary<long, List<byte>>();
        public Dictionary<string, Point> statPositions = new Dictionary<string, Point>();
        private static ArkOCR _OCR;
        public static FlowLayoutPanel debugPanel;
        public Dictionary<string, Point> lastLetterPositions = new Dictionary<string, Point>();
        public string screenCaptureApplicationName;
        public Process ScreenCaptureProcess;
        public int currentFontSizeSmall, currentFontSizeLarge;
        public int currentResolutionW, currentResolutionH;

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
            Bitmap origBitmap, bmp;

            origBitmap = Properties.Resources.ARKCalibration15;
            bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThreshold);
            CalibrateFromImage(15, bmp, "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            origBitmap = Properties.Resources.ARKCalibration18;
            bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThreshold);
            CalibrateFromImage(18, bmp, "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            //// debugging / TODO
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


            // add weightings to chars. More probable chars get higher weighting
            for (int l = 32; l < charWeighting.Length; l++)
            {
                if (l == 37) charWeighting[l] = 1; // %
                else if (l < 44) charWeighting[l] = 0.9;
                else if (l < 58) charWeighting[l] = 1; // numbers ,-./
                else if (l < 65) charWeighting[l] = 0.9; // :;<=>?@
                else if (l < 91) charWeighting[l] = 0.98; // capital letters
                else if (l < 97) charWeighting[l] = 0.9; // [\]^_'
                else if (l < 123) charWeighting[l] = 1; // lowercase letters
                else if (l < 165) charWeighting[l] = 0.97; // letters with accents
                else charWeighting[l] = 0.8; // symbols
            }
            charWeighting[108] = 0.98;// l (i is often mistaken for l)

            screenCaptureApplicationName = "ShooterGame";
        }

        public bool calibrate()
        {
            return calibrate(Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName));
        }

        // figure out the current resolution and positions
        // return true if the calibration was successful
        public bool calibrate(Bitmap screenshot)
        {
            if (screenshot == null)
                return false;

            if (screenshot.Width == currentResolutionW && screenshot.Height == currentResolutionH)
                return true;

            Process[] p = Process.GetProcessesByName(screenCaptureApplicationName);
            if (p.Length > 0)
                ScreenCaptureProcess = p[0];

            //debugPanel.Controls.Clear();
            //alphabet = new Bitmap[3,255];
            //hashMap = new Dictionary<long, List<byte>>(); todo. currently not used. remove?
            statPositions = new Dictionary<string, Point>(12);

            // positions depend on screen resolution.
            int resolutionW = 0, resolutionH = 0;
            Win32Stuff.Rect res = new Win32Stuff.Rect(); // Win32Stuff.GetWindowRect(screenCaptureApplicationName);

            res.left = 0;
            res.right = screenshot.Width;
            res.top = 0;
            res.bottom = screenshot.Height;

            if (res.Width == 1920 && res.Height == 1080)
            {
                resolutionW = 1920;
                resolutionH = 1080;
            }
            else if (res.Width == 1680 && res.Height == 1050)
            {
                resolutionW = 1680;
                resolutionH = 1050;
            }
            else
            {
                resolutionW = -1;
                resolutionH = -1;
                return false; // no supported resolution
            }

            if (currentResolutionW == resolutionW && currentResolutionH == resolutionH)
                return true; // already calibrated

            if (resolutionW == 1920 && resolutionH == 1080)
            {
                currentFontSizeLarge = 18;
                currentFontSizeSmall = 15;

                int xStats = 950;
                int yStatsD = 43;
                int yc = 0;
                // coords for 1920x1080
                statPositions["Species"] = new Point(860, 154);
                statPositions["Level"] = new Point(860, 186);
                statPositions["Health"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Stamina"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Oxygen"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Food"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Weight"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Melee Damage"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Movement Speed"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Torpor"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                statPositions["Imprinting"] = new Point(xStats, 501 + (yStatsD * (yc++)));
                //statPositions["CurrentWeight"] = new Point(805, 231); // central version of weight, gives "temporary maximum", useful for determining baby %age // todo new res
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

            /*
            AddBitmapToDebug(alphabet['µ']);
            AddBitmapToDebug(alphabet['%']);
            AddBitmapToDebug(alphabet['$']);
            AddBitmapToDebug(alphabet['A']);
            AddBitmapToDebug(alphabet['J']);
            AddBitmapToDebug(alphabet['µ']);
            AddBitmapToDebug(alphabet['?']);
            AddBitmapToDebug(alphabet['-']);
            AddBitmapToDebug(alphabet['[']);
            AddBitmapToDebug(alphabet['z']);
            AddBitmapToDebug(alphabet['(']);
            AddBitmapToDebug(alphabet[')']);
            AddBitmapToDebug(alphabet['f']);
            AddBitmapToDebug(alphabet['K']);

            */

            foreach (KeyValuePair<int, Bitmap[]> a in alphabets)
            {
                reducedAlphabets.Add(a.Key, new Bitmap[255]);
                foreach (char c in ":0123456789.,%/")
                    reducedAlphabets[a.Key][c] = alphabets[a.Key][c];
            }
            return true;
        }

        private PictureBox AddBitmapToDebug(Bitmap bmp)
        {
            if (debugPanel != null)
            {
                PictureBox b = new PictureBox();
                b.SizeMode = PictureBoxSizeMode.AutoSize;
                b.Image = bmp;
                debugPanel.Controls.Add(b);
                debugPanel.Controls.SetChildIndex(b, 0);
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

            for (int counter = 0; counter < rgbValues.Length; counter++)
            {
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
                }
                else if (rgbValues[counter] > highT)
                {
                    rgbValues[counter] = 255; // maximize the white
                }
                else
                {
                    rgbValues[counter] = 150;
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

        /*
        // function currently unused. ARK seems to be scaling down larger fonts rather than using entire pixel heights
        private void GenerateLetterImagesFromFont(Int16 pixelSize)
        {

            return;

            string fontName = "Sansation-Bold.ttf"; // font used by ARK

            //ARKBreedingStats.Properties.Resources.Sansation_Bold;
            fontName = System.Reflection.Assembly.GetExecutingAssembly().Location + @"\..\..\Resources\Sansation-Bold.ttf";

            PrivateFontCollection pfcoll = new PrivateFontCollection();
            //put a font file under a Fonts directory within your application root
            pfcoll.AddFontFile(fontName);
            FontFamily ff = pfcoll.Families[0];
            string firstText = "Health: 2660.1 / 2660.1";

            PointF firstLocation = new PointF(10f, 10f);
            PointF secondLocation = new PointF(10f, 50f);
            //put an image file under a Images directory within your application root
            Bitmap bitmap = new Bitmap(300, 200);//load the image file

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font f = new Font(ff, pixelSize, FontStyle.Regular))
                {
                    graphics.DrawString(firstText, f, Brushes.Red, firstLocation);
                }
            }
            //save the new image file within Images directory
            //bitmap.Save(@"D:\Temp\test.png");
        }
        */


        public void CalibrateFromImage(int fontSize, Bitmap source, string textInImage)
        {
            int posXInImage = 0;

            if (!alphabets.ContainsKey(fontSize))
                alphabets.Add(fontSize, new Bitmap[128]);

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
                if (alphabets[fontSize][letter] == null && posXInImage != source.Width)
                    StoreImageInAlphabet(fontSize, letter, source, letterStart, letterEnd);
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

        private void StoreImageInAlphabet(int fontSize, char letter, Bitmap source, int letterStart, int letterEnd)
        {
            Rectangle cropRect = letterRect(source, letterStart, letterEnd); //new Rectangle(x, y, width, height);
            if (cropRect.Width > 0 && cropRect.Height > 0) // todo, '|' has no width
            {
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }

                alphabets[fontSize][letter] = target;
            }

            //// hashmap isn't used
            //int pcount = 0;
            //for (int i = 0; i < target.Width; i++)
            //    for (int j = 0; j < target.Height; j++)
            //        if (target.GetPixel(i, j).R != 0)
            //            pcount++;

            //if (!hashMap.ContainsKey(pcount))
            //    hashMap[pcount] = new List<byte>();

            //hashMap[pcount].Add((byte)letter);
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
                        for (int hh = Math.Max(0, h - 3); hh < source.Height && hh < h + 3; hh++)
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

        private static Rectangle letterRect(Bitmap source, int hStart, int hEnd)
        {
            int startWhite = -1, endWhite = -1;
            for (int j = 0; j < source.Height; j++)
            {
                for (int i = hStart; i < hEnd; i++)
                {
                    if (startWhite == -1 && source.GetPixel(i, j).R == 255)
                    {
                        startWhite = j;
                    }

                    if (endWhite == -1 && source.GetPixel(i, (source.Height - j) - 1).R == 255)
                    {
                        endWhite = (source.Height - j);
                    }
                    if (startWhite != -1 && endWhite != -1)
                        return new Rectangle(hStart, startWhite, hEnd - hStart, endWhite - startWhite);
                }
            }


            return Rectangle.Empty;
        }

        public float[] doOCR(out string OCRText, out string dinoName, out string ownerName, string useImageFilePath = "", bool changeForegroundWindow = true)
        {
            string finishedText = "";
            dinoName = "";
            ownerName = "";
            float[] finalValues = new float[1] { 0 };

            Bitmap screenshotbmp = null;
            Bitmap testbmp;

            debugPanel.Controls.Clear();

            if (System.IO.File.Exists(useImageFilePath))
            {
                screenshotbmp = (Bitmap)Bitmap.FromFile(useImageFilePath);
            }
            else
            {
                // grab screenshot from ark
                screenshotbmp = Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName);
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

            if (!calibrate(screenshotbmp))
            {
                OCRText = "Error while calibrating: probably game-resolution is not supported by this OCR";
                return finalValues;
            }
            finalValues = new float[statPositions.Count];

            AddBitmapToDebug(screenshotbmp);
            if (changeForegroundWindow)
                Win32Stuff.SetForegroundWindow(Application.OpenForms[0].Handle);

            int count = -2;
            foreach (string statName in statPositions.Keys)
            {
                count++;
                testbmp = SubImage(screenshotbmp, statPositions[statName].X, statPositions[statName].Y, 165, 25); // width of 165 is enough
                //AddBitmapToDebug(testbmp);

                string statOCR = "";

                if (statName == "Species")
                    statOCR = readImage(currentFontSizeLarge, testbmp, true, false);
                else if (statName == "Level")
                    statOCR = readImage(currentFontSizeLarge, testbmp, true, true);
                else
                    statOCR = readImage(currentFontSizeSmall, testbmp, true, true); // statvalues are only numbers

                if (statOCR == "" &&
                    (statName == "Oxygen" || statName == "Imprinting" || statName == "CurrentWeight"))
                    continue; // these can be missing, it's fine

                lastLetterPositions[statName] = new Point(statPositions[statName].X + lastLetterPosition(removePixelsUnderThreshold(GetGreyScale(testbmp), whiteThreshold)), statPositions[statName].Y);

                finishedText += "\r\n" + statName + ": " + statOCR;

                // parse the OCR String

                Regex r;
                if (statName == "Species")
                    r = new Regex(@"(.+)");
                else if (statName == "Level")
                    r = new Regex(@".+:(\d*)");
                else
                {
                    r = new Regex(@"(?:\d+\.\d%?[\/1])?(\d+[\.,']?\d?\d?)%?"); // only the second numbers is interesting after the current weight is not shown anymore

                    //if (onlyNumbers)
                    //r = new Regex(@"((\d*[\.,']?\d?\d?)\/)?(\d*[\.,']?\d?\d?)");
                    //else
                    // r = new Regex(@"([a-zA-Z]*)[:;]((\d*[\.,']?\d?\d?)\/)?(\d*[\.,']?\d?\d?)");
                }

                MatchCollection mc = r.Matches(statOCR);

                if (mc.Count == 0)
                {
                    if (statName == "Species" || statName == "Level")
                        continue;
                    else
                    {
                        finishedText += "error reading stat " + statName;
                        finalValues[count] = 0;
                        continue;
                    }
                }

                string testStatName = mc[0].Groups[1].Value;
                float v = 0;
                float.TryParse(mc[0].Groups[mc[0].Groups.Count - 1].Value.Replace('\'', '.').Replace(',', '.').Replace('O', '0'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out v); // common substitutions: comma and apostrophe to dot, 

                if (statName == "Species")
                {
                    dinoName = testStatName;
                    // todo remove this removal, does the new ui show this?
                    // remove prefixes Baby, Juvenile and Adolescent
                    r = new Regex("^(?:Ba[bh]y|Juven[il]le|Adolescent) *");
                    dinoName = r.Replace(dinoName, "");

                    // common OCR-mistake, 'I' is taken instead of 'i' in names
                    r = new Regex("(?<=[a-z])I(?=[a-z])");
                    dinoName = r.Replace(dinoName, "i");
                    continue;
                }
                // todo is the owner / raiser shown?
                /* OCR too bad to do this yet (font-size is smaller)
                else if (statName == "Imprinting")
                {
                    // parse the name of the person that imprinted the creature
                    r = new Regex(@"(?<=RaisedBy)([^,\.]+)");
                    mc = r.Matches(statOCR);
                    if (mc.Count > 0)
                        ownerName = mc[0].Groups[1].Value;
                }
                */

                // TODO: test here that the read stat name corresponds to the stat supposed to be read
                finalValues[count] = v;
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

        private string readImageAtCoords(int fontSize, Bitmap source, int x, int y, int width, int height, bool onlyMaximalMatches, bool onlyNumbers, bool writingInWhite = true)
        {
            return readImage(fontSize, SubImage(source, x, y, width, height), onlyMaximalMatches, onlyNumbers, writingInWhite);
        }

        private string readImage(int fontSize, Bitmap source, bool onlyMaximalMatches, bool onlyNumbers, bool writingInWhite = true)
        {
            string result = "";
            if (!alphabets.ContainsKey(fontSize))
                return "not calibrated!";

            Bitmap[] theAlphabet = alphabets[fontSize];

            if (onlyNumbers)
                theAlphabet = reducedAlphabets[fontSize];


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
                    Rectangle letterR = letterRect(cleanedImage, letterStart, letterEnd);
                    if (letterR.Width > 0 && letterR.Height > 0)
                    {
                        Bitmap testImage = SubImage(cleanedImage, letterR.Left, letterR.Top, letterR.Width, letterR.Height);
                        //testImage.Save(@"D:\Temp\debug_letterfound.png");// TODO comment out
                        Dictionary<int, float> matches = new Dictionary<int, float>();
                        float bestMatch = 0;
                        for (int l = 33; l < theAlphabet.Length; l++) // start at 33, before are no used chars in ascii
                        {
                            float match = 0;
                            if (theAlphabet[l] != null)
                                match = (float)(PercentageMatch(theAlphabet[l], testImage) * charWeighting[l]);
                            else
                                continue;

                            if (match > 0.5)
                            {
                                matches[l] = match;
                                if (bestMatch < match)
                                    bestMatch = match;
                            }
                        }

                        if (matches.Count == 0)
                            continue;

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
                            result += (char)(goodMatches.Keys.ToArray()[0]);
                        else
                        {
                            if (onlyMaximalMatches)
                            {
                                foreach (int l in goodMatches.Keys)
                                {
                                    if (goodMatches[l] == bestMatch)
                                    {
                                        result += (char)l;
                                        break; // if there are multiple best matches take only the first
                                    }
                                }
                            }
                            else
                            {
                                result += "[";
                                foreach (int l in goodMatches.Keys)
                                    result += (char)l + goodMatches[l].ToString("{0.00}") + " ";
                                result += "]";
                            }
                        }
                    }
                }
            }

            //// replace half letters. // todo necessary?
            //result = result.Replace((char)15 + "n", "n");
            //result = result.Replace((char)16 + "lK", "K");

            return result;

        }

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

        internal void setDebugPanel(FlowLayoutPanel OCRDebugLayoutPanel)
        {
            debugPanel = OCRDebugLayoutPanel;
        }

        public bool isDinoInventoryVisible()
        {
            if (ScreenCaptureProcess == null)
                return false;

            float[] finalValues = new float[1] { 0 };

            Bitmap screenshotbmp = null;// = (Bitmap)Bitmap.FromFile(@"D:\ScreenshotsArk\Clipboard12.png");
            Bitmap testbmp;

            if (Win32Stuff.GetForegroundWindow() != ScreenCaptureProcess.MainWindowHandle)
                return false;

            screenshotbmp = Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName);

            if (screenshotbmp == null)
                return false;
            if (!calibrate(screenshotbmp))
                return false;

            string statName = "Level";
            testbmp = SubImage(screenshotbmp, statPositions[statName].X, statPositions[statName].Y, 200, 30);
            string statOCR = readImage(currentFontSizeLarge, testbmp, true, false);

            Regex r = new Regex(@"(\d+)");
            MatchCollection mc = r.Matches(statOCR);

            if (mc.Count != 0)
                return true;

            return false;
        }

    }
}

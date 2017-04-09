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
        public int whiteThreshold = 155;
        public int whiteThresholdCalibration = 155;
        public Dictionary<int, Bitmap[]> alphabets = new Dictionary<int, Bitmap[]>(); // font-size in px, then alphabet
        public Dictionary<int, Bitmap[]> reducedAlphabets = new Dictionary<int, Bitmap[]>();
        private Dictionary<int, uint[][]> alphabetsI = new Dictionary<int, uint[][]>();
        private Dictionary<int, uint[][]> reducedAlphabetsI = new Dictionary<int, uint[][]>();
        private double[] charWeighting = new double[255]; // contains weightings for chars
        //public Dictionary<Int64, List<byte>> hashMap = new Dictionary<long, List<byte>>();
        public Dictionary<string, int[]> statPositions = new Dictionary<string, int[]>(); // int[]: X,Y,Width,Height
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
            statPositions = new Dictionary<string, int[]>(12);

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

            calibrate();
        }

        public void calibrate()
        {
            Bitmap origBitmap, bmp;

            origBitmap = Properties.Resources.ARKCalibration15;
            bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThresholdCalibration);
            CalibrateFromImage(15, bmp, "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            origBitmap = Properties.Resources.ARKCalibration18;
            bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThresholdCalibration);
            CalibrateFromImage(18, bmp, "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

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
            return setResolution(Win32Stuff.GetSreenshotOfProcess(screenCaptureApplicationName));
        }

        // figure out the current resolution and positions
        // return true if the calibration was successful
        public bool setResolution(Bitmap screenshot)
        {
            if (screenshot == null)
                return false;

            if (screenshot.Width == currentResolutionW && screenshot.Height == currentResolutionH)
                return true;

            Process[] p = Process.GetProcessesByName(screenCaptureApplicationName);
            if (p.Length > 0)
                ScreenCaptureProcess = p[0];

            //debugPanel.Controls.Clear();

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
                int statWidth = 164;
                int yc = 0;
                // coords for 1920x1080
                statPositions["Health"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Stamina"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Oxygen"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Food"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Weight"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Melee Damage"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Movement Speed"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Torpor"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["Level"] = new int[] { 907, 192, 127, 18 };
                statPositions["Imprinting"] = new int[] { xStats, 508 + (yStatsD * (yc++)), statWidth, currentFontSizeSmall };
                statPositions["NameSpecies"] = new int[] { 846, 158, 228, 18 };
                statPositions["Tribe"] = new int[] { 856, 230, 202, 15 };
                statPositions["Owner"] = new int[] { 967, 402, 92, 13 };
                //statPositions["CurrentWeight"] = new int[]{805, 231}; // central version of weight, gives "temporary maximum", useful for determining baby %age // todo currently not available in the UI
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

            if (!alphabetsI.ContainsKey(fontSize))
            {
                //alphabets.Add(fontSize, new Bitmap[128]);
                alphabetsI.Add(fontSize, new uint[128][]);
            }

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
                if (alphabetsI[fontSize][letter] == null && posXInImage != source.Width)
                    StoreImageInAlphabet(fontSize, letter, source, letterStart, letterEnd);
            }

            if (!reducedAlphabetsI.ContainsKey(fontSize))
                //reducedAlphabets.Add(fontSize, new Bitmap[128]);
                reducedAlphabetsI.Add(fontSize, new uint[128][]);
            foreach (char c in ":0123456789.,%/")
                reducedAlphabetsI[fontSize][c] = alphabetsI[fontSize][c];
            //reducedAlphabets[fontSize][c] = alphabets[fontSize][c];
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

                //alphabets[fontSize][letter] = target; // todo
                alphabetsI[fontSize][letter] = letterArray(target);
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

        public float[] doOCR(out string OCRText, out string dinoName, out string species, out string ownerName, out string tribeName, string useImageFilePath = "", bool changeForegroundWindow = true)
        {
            string finishedText = "";
            dinoName = "";
            species = "";
            ownerName = "";
            tribeName = "";
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

            if (!setResolution(screenshotbmp))
            {
                OCRText = "Error while calibrating: probably game-resolution is not supported by this OCR.\nThe tested image has a resolution of " + screenshotbmp.Width.ToString() + "×" + screenshotbmp.Height.ToString() + " px.";
                return finalValues;
            }
            finalValues = new float[statPositions.Count];

            AddBitmapToDebug(screenshotbmp);
            if (changeForegroundWindow)
                Win32Stuff.SetForegroundWindow(Application.OpenForms[0].Handle);

            int count = 0;
            foreach (string statName in statPositions.Keys)
            {
                testbmp = SubImage(screenshotbmp, statPositions[statName][0], statPositions[statName][1], statPositions[statName][2], statPositions[statName][3]);
                AddBitmapToDebug(testbmp);

                string statOCR = "";

                if (statName == "NameSpecies")
                    statOCR = readImage(currentFontSizeLarge, testbmp, true, false);
                else if (statName == "Level")
                    statOCR = readImage(currentFontSizeLarge, testbmp, true, true);
                else if (statName == "Tribe" || statName == "Owner")
                    statOCR = readImage(currentFontSizeSmall, testbmp, true, false);
                else
                    statOCR = readImage(currentFontSizeSmall, testbmp, true, true); // statvalues are only numbers

                if (statOCR == "" &&
                    (statName == "Oxygen" || statName == "Imprinting" || statName == "CurrentWeight" || statName == "Tribe" || statName == "Owner"))
                    continue; // these can be missing, it's fine

                lastLetterPositions[statName] = new Point(statPositions[statName][0] + lastLetterPosition(removePixelsUnderThreshold(GetGreyScale(testbmp), whiteThreshold)), statPositions[statName][1]);

                finishedText += "\r\n" + statName + ": " + statOCR;

                // parse the OCR String

                Regex r;
                if (statName == "NameSpecies" || statName == "Owner" || statName == "Tribe")
                    r = new Regex(@"(.*?)(:?\((.+)\))?");
                else if (statName == "Level")
                    r = new Regex(@".+:(\d*)");
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
                    else
                    {
                        finishedText += "error reading stat " + statName;
                        finalValues[count] = 0;
                        continue;
                    }
                }

                if (statName == "NameSpecies" || statName == "Owner" || statName == "Tribe")
                {
                    if (statName == "NameSpecies" && mc[0].Groups.Count > 0)
                    {
                        dinoName = mc[0].Groups[0].Value;
                        if (mc[0].Groups.Count > 1)
                            species = mc[0].Groups[1].Value;
                        // todo remove this removal, does the new ui show this?
                        /*
                        // remove prefixes Baby, Juvenile and Adolescent
                        r = new Regex("^(?:Ba[bh]y|Juven[il]le|Adolescent) *");
                        dinoName = r.Replace(dinoName, "");

                        // common OCR-mistake, 'I' is taken instead of 'i' in names
                        r = new Regex("(?<=[a-z])I(?=[a-z])");
                        dinoName = r.Replace(dinoName, "i");
                        */
                    }
                    else if (statName == "Owner" && mc[0].Groups.Count > 0)
                        ownerName = mc[0].Groups[0].Value;
                    else if (statName == "Tribe" && mc[0].Groups.Count > 0)
                        tribeName = mc[0].Groups[0].Value;
                    continue;
                }

                if (mc[0].Groups.Count > 2 && mc[0].Groups[2].Value == "%" && statName == "Weight")
                {
                    // first stat with a '%' is damage, if oxygen is missing, shift all stats by one
                    finalValues[4] = finalValues[3]; // shift food to weight
                    finalValues[3] = finalValues[2]; // shift oxygen to food
                    finalValues[2] = 0; // set oxygen (which wasn't there) to 0
                    count++;
                }

                float v = 0;
                float.TryParse(mc[0].Groups[1].Value.Replace('\'', '.').Replace(',', '.').Replace('O', '0'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out v); // common substitutions: comma and apostrophe to dot, 

                // TODO: test here that the read stat name corresponds to the stat supposed to be read
                finalValues[count] = v;
                count++;
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
            if (!alphabetsI.ContainsKey(fontSize))
                return "not calibrated!";

            //Bitmap[] theAlphabet = alphabets[fontSize];
            uint[][] theAlphabetI = alphabetsI[fontSize];

            if (onlyNumbers)
                theAlphabetI = reducedAlphabetsI[fontSize];


            Bitmap cleanedImage = removePixelsUnderThreshold(GetGreyScale(source, !writingInWhite), whiteThreshold);
            AddBitmapToDebug(cleanedImage); // todo comment out
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
                        for (int l = 33; l < theAlphabetI.Length; l++) // start at 33, before are no used chars in ascii
                        {
                            float match = 0;
                            if (theAlphabetI[l] != null)
                            {
                                //match = (float)(PercentageMatch(theAlphabet[l], testImage) * charWeighting[l]);
                                uint HammingDiff = 0;
                                uint[] HWs = letterArray(testImage);
                                int maxTestRange = Math.Min(HWs.Length, theAlphabetI[l].Length);
                                for (int y = 0; y < maxTestRange; y++)
                                {
                                    uint t = HWs[y] ^ theAlphabetI[l][y];
                                    HammingDiff += HammingWeight(HWs[y] ^ theAlphabetI[l][y]);
                                }
                                HammingDiff += (uint)(Math.Abs(HWs.Length - theAlphabetI[l].Length) * Math.Max(HWs.Length, theAlphabetI[l].Length));
                                int total = testImage.Width * testImage.Height;
                                if (total > 10)
                                    match = ((float)(total - HammingDiff) / total);
                                else
                                    match = 1 - HammingDiff / 10f;
                            }
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

            //addCalibrationImageToDebug(result, fontSize); // debugging

            //// replace half letters. // todo necessary?
            //result = result.Replace((char)15 + "n", "n");
            //result = result.Replace((char)16 + "lK", "K");

            return result;

        }

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
            if (!setResolution(screenshotbmp))
                return false;

            string statName = "Level";
            testbmp = SubImage(screenshotbmp, statPositions[statName][0], statPositions[statName][1], statPositions[statName][2], statPositions[statName][3]);
            string statOCR = readImage(currentFontSizeLarge, testbmp, true, false);

            Regex r = new Regex(@"(\d+)");
            MatchCollection mc = r.Matches(statOCR);

            if (mc.Count != 0)
                return true;

            return false;
        }

        // Hamming-weight lookup-table, taken from http://www.necessaryandsufficient.net/2009/04/optimising-bit-counting-using-iterative-data-driven-development/

        private readonly byte[] bitCounts = new byte[ushort.MaxValue + 1];

        private bool HammingIsInitialized;  // will be false by default

        private uint BitsSetCountWegner(uint input)
        {
            uint count;
            for (count = 0; input != 0; count++)
            {
                input &= input - 1; // turn off the rightmost 1-bit
            }
            return count;
        }

        private void InitializeBitcounts()
        {
            for (uint i = 0; i < UInt16.MaxValue; i++)
            {
                // Get the bitcount using any method
                bitCounts[i] = (byte)BitsSetCountWegner(i);
            }
            bitCounts[ushort.MaxValue] = 16;
            HammingIsInitialized = true;
        }

        private uint HammingWeight(uint i)
        {
            if (!HammingIsInitialized)
                InitializeBitcounts();

            return (uint)(bitCounts[i & 0xFFFF] + bitCounts[(i >> 16) & 0xFFFF]);
        }

        private uint[] letterArray(Bitmap letter)
        {
            uint[] la = new uint[letter.Height];
            for (int y = 0; y < letter.Height; y++)
            {
                uint row = 0;
                for (int x = 0; x < letter.Width; x++)
                {
                    row += (letter.GetPixel(x, y).R == 0 ? 0 : (uint)(1 << x));
                }
                la[y] = row;
            }
            return la;
        }

    }
}

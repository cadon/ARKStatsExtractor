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
        int whiteThreshold = 230;
        public Bitmap[,] alphabet = new Bitmap[3, 255]; // resolution, then alphabet
        public Bitmap[,] reducedAlphabet = new Bitmap[3, 255];
        private double[] charWeighting = new double[255]; // contains weightings for chars
        public Dictionary<Int64, List<byte>> hashMap = new Dictionary<long, List<byte>>();
        public Dictionary<string, Point> statPositions = new Dictionary<string, Point>();
        private static ArkOCR _OCR;
        public static FlowLayoutPanel debugPanel;
        private int[] calibrationResolution = new int[] { 0, 0 };
        public Dictionary<string, Point> lastLetterPositions = new Dictionary<string, Point>();
        private bool coordsAfterDot = false;
        public string screenCaptureApplicationName;
        public Process ScreenCaptureProcess;
        public int currentResolution = -1;

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

            origBitmap = Properties.Resources.ARKCalibration1080;
            bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThreshold);
            CalibrateFromImage(0, bmp, @"1234567890?,.;/:+=@|#%abcdeghijklm " + @"n opqrstuvwxyz&é'(§è!çà)-ABCDEFGHIJLMNOPQRSTUVWXYZ£µ$[]{}ñ<>/\f lK");

            origBitmap = Properties.Resources.ARKCalibration1050;
            bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThreshold);
            CalibrateFromImage(1, bmp, @"1234567890.,?;.:/=+ù%µ$* ABCDEFGHIJ-LMNOPQRSTUVWXYZabcdeghijklmnopqrstuvwxyz&#'()[]{}!@flK"); // £ missing
            //bmp.Save("D:\\temp\\calibration_threshold_1050.png");// TODO comment out

            origBitmap = Properties.Resources.ARKCalibration1050;
            bmp = removePixelsUnderThreshold(GetGreyScale(origBitmap), whiteThreshold);
            CalibrateFromImage(2, bmp, @"1234567890.,?;.:/=+ù%µ$* ABCDEFGHIJ-LMNOPQRSTUVWXYZabcdeghijklmnopqrstuvwxyz&#'()[]{}!@flK"); // £ missing

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

            if (screenshot.Width == calibrationResolution[0] && screenshot.Height == calibrationResolution[1])
                return true;

            Process[] p = Process.GetProcessesByName(screenCaptureApplicationName);
            if (p.Length > 0)
                ScreenCaptureProcess = p[0];

            //debugPanel.Controls.Clear();
            //alphabet = new Bitmap[3,255];
            hashMap = new Dictionary<long, List<byte>>();
            statPositions = new Dictionary<string, Point>(9);

            // positions depend on screen resolution.
            int resolution = 0;
            Win32Stuff.Rect res = new Win32Stuff.Rect(); // Win32Stuff.GetWindowRect(screenCaptureApplicationName);
            if (screenshot != null)
            {
                res.left = 0;
                res.right = screenshot.Width;
                res.top = 0;
                res.bottom = screenshot.Height;
            }
            else
                return false; // error

            if (res.Width == 1920 && res.Height == 1080)
                resolution = 0;
            else if (res.Width == 1680 && res.Height == 1050)
                resolution = 1;
            else if (res.Width == 1600 && res.Height == 900)
                resolution = 2;
            else
            {
                resolution = -1;
                return false; // no supported resolution
            }

            if (currentResolution == resolution)
                return true;

            switch (resolution)
            {
                case 0:
                    // coords for 1920x1080
                    statPositions["NameAndLevel"] = new Point(1280, 170);
                    statPositions["Health"] = new Point(1355, 630);
                    statPositions["Stamina"] = new Point(1355, 665);
                    statPositions["Oxygen"] = new Point(1355, 705);
                    statPositions["Food"] = new Point(1355, 740);
                    statPositions["Weight"] = new Point(1355, 810);
                    statPositions["Melee Damage"] = new Point(1355, 845);
                    statPositions["Movement Speed"] = new Point(1355, 885);
                    statPositions["Torpor"] = new Point(1355, 990);
                    statPositions["CurrentWeight"] = new Point(805, 231); // central version of weight, gives "temporary maximum", useful for determining baby %age
                    statPositions["Imprinting"] = new Point(1260, 594);
                    break;

                case 1:
                    // coords for 1680x1050
                    // 1680/1920 = height-factor; 50 = translation
                    // not yet correct x_1080 |--> (x_1080+60)*1680/1920
                    //statPositions["NameAndLevel"] = new Point(1111, 200);
                    //statPositions["Health"] = new Point(1183, 595);
                    //statPositions["Stamina"] = new Point(1183, 630);
                    //statPositions["Oxygen"] = new Point(1183, 665);
                    //statPositions["Food"] = new Point(1183, 691);
                    //statPositions["Weight"] = new Point(1183, 755);
                    //statPositions["Melee Damage"] = new Point(1183, 788);
                    //statPositions["Movement Speed"] = new Point(1183, 817);
                    //statPositions["Torpor"] = new Point(1183, 912);

                    // version without the "statName:"
                    coordsAfterDot = true;
                    statPositions["NameAndLevel"] = new Point(1111, 200);
                    statPositions["Health"] = new Point(1260, 595);
                    statPositions["Stamina"] = new Point(1277, 630);
                    statPositions["Oxygen"] = new Point(1271, 665);
                    statPositions["Food"] = new Point(1249, 691);
                    statPositions["Weight"] = new Point(1264, 755);
                    statPositions["Melee Damage"] = new Point(1340, 788);
                    statPositions["Movement Speed"] = new Point(1362, 817);
                    statPositions["Torpor"] = new Point(1260, 912);
                    statPositions["CurrentWeight"] = new Point(1, 1); // not correct, TODO
                    break;

                case 2:
                    // coords for 1600x960
                    statPositions["NameAndLevel"] = new Point(1130, 170);
                    statPositions["Health"] = new Point(1130, 550);
                    statPositions["Stamina"] = new Point(1130, 585);
                    statPositions["Oxygen"] = new Point(1130, 615);
                    statPositions["Food"] = new Point(1130, 645);
                    statPositions["Weight"] = new Point(1130, 705);
                    statPositions["Melee Damage"] = new Point(1130, 735);
                    statPositions["Movement Speed"] = new Point(1130, 765);
                    statPositions["Torpor"] = new Point(1130, 855);
                    statPositions["CurrentWeight"] = new Point(1, 1); // not correct, TODO
                    break;
            }
            calibrationResolution[0] = res.Width;
            calibrationResolution[1] = res.Height;

            currentResolution = resolution;

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

            for (int l = 0; l < alphabet.GetLength(0); l++)
                foreach (char a in ":0123456789.,%/")
                    reducedAlphabet[l, a] = alphabet[l, a];

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

                        grey = Math.Max(Math.Max(rgbValues[idx], rgbValues[idx + 1]), rgbValues[idx + 2]);
                    }
                    else
                    {
                        int sum = rgbValues[idx] + rgbValues[idx + 1] + rgbValues[idx + 2];

                        grey = (byte)(sum / 3);
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

            double lowT = threshold * .85, highT = threshold * 1;

            for (int counter = 0; counter < rgbValues.Length; counter++)
            {
                //if (rgbValues[counter] < threshold)
                //    rgbValues[counter] = 0;
                //else
                //    rgbValues[counter] = 255; // maximize the white

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


        public void CalibrateFromImage(int resolution, Bitmap source, string textInImage)
        {
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
                    foundLetter = HasWhiteInVerticalLine(source, posXInImage);
                    posXInImage++;
                }

                if (foundLetter)
                {
                    letterStart = posXInImage - 1;
                    // look for the end of the letter
                    do
                    {
                        posXInImage++;
                    } while (HasWhiteInVerticalLine(source, posXInImage) && posXInImage < source.Width);
                    letterEnd = posXInImage;
                }

                // store the image in the alphabet
                if (alphabet[resolution, letter] == null && posXInImage != source.Width)
                    StoreImageInAlphabet(resolution, letter, source, letterStart, letterEnd);
            }

        }

        public int lastLetterPosition(Bitmap source)
        {
            for (int i = source.Width; i > 0; i--)
            {
                if (HasWhiteInVerticalLine(source, i))
                    return i + 1;
            }
            return 0;
        }

        private void StoreImageInAlphabet(int resolution, char letter, Bitmap source, int letterStart, int letterEnd)
        {
            Rectangle cropRect = letterRect(source, letterStart, letterEnd); //new Rectangle(x, y, width, height);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            alphabet[resolution, letter] = target;

            int pcount = 0;
            for (int i = 0; i < target.Width; i++)
                for (int j = 0; j < target.Height; j++)
                    if (target.GetPixel(i, j).R != 0)
                        pcount++;

            if (!hashMap.ContainsKey(pcount))
                hashMap[pcount] = new List<byte>();

            hashMap[pcount].Add((byte)letter);
        }

        private static bool HasWhiteInVerticalLine(Bitmap source, int posXInImage)
        {
            bool hasWhite = false;
            if (posXInImage >= source.Width)
                return false;

            int greys = source.Height / 3;

            for (int h = 0; h < source.Height; h++)
            {
                if (source.GetPixel(posXInImage, h).R == 255)
                {
                    hasWhite = true;
                    break;
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

            int count = -1;
            foreach (string statName in statPositions.Keys)
            {
                count++;
                testbmp = SubImage(screenshotbmp, statPositions[statName].X, statPositions[statName].Y, 500, 30); // 300 is enough, except for the name
                AddBitmapToDebug(testbmp);

                bool onlyNumbers = coordsAfterDot; // hack for 1050, position just for coordinates

                string statOCR = "";

                if (statName == "NameAndLevel")
                    statOCR = readImage(currentResolution, testbmp, true, false);
                else if (statName == "Imprinting")
                    statOCR = readImage(currentResolution + 1, testbmp, true, true, false); // imprinting is written in lower letters
                else
                    statOCR = readImage(currentResolution, testbmp, true, onlyNumbers);

                if (statOCR == "" &&
                    (statName == "Oxygen" || statName == "Imprinting"))
                    continue; // these can be missing, it's fine

                lastLetterPositions[statName] = new Point(statPositions[statName].X + lastLetterPosition(removePixelsUnderThreshold(GetGreyScale(testbmp), whiteThreshold)), statPositions[statName].Y);

                finishedText += "\r\n" + statName + ": " + statOCR;

                // parse the OCR String

                Regex r;
                if (statName == "NameAndLevel")
                    r = new Regex(@"(.*)-?Lv[liI](\d*)Eq");
                else
                {
                    if (onlyNumbers)
                        r = new Regex(@"((\d+[\.,']?\d?\d?)%?\/)?(\d+[\.,']?\d?\d?)%?"); //new Regex(@"((\d*[\.,']?\d?\d?)\/)?(\d*[\.,']?\d?\d?)");
                    else
                        r = new Regex(@"([a-zA-Z]*)[:;]((\d*[\.,']?\d?\d?)\/)?(\d*[\.,']?\d?\d?)");
                }

                MatchCollection mc = r.Matches(statOCR);

                if (mc.Count == 0)
                {
                    if (statName == "NameAndLevel")
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

                if (statName == "NameAndLevel")
                {
                    if (testStatName.EndsWith("-"))
                        dinoName = testStatName.Substring(0, testStatName.Length - 1);
                    else
                        dinoName = testStatName;
                    // remove prefixes Baby, Juvenile and Adolescent
                    r = new Regex("^(?:Ba[bh]y|Juven[il]le|Adolescent) *");
                    dinoName = r.Replace(dinoName, "");

                    // common OCR-mistake, 'I' is taken instead of 'i' in names
                    r = new Regex("(?<=[a-z])I(?=[a-z])");
                    dinoName = r.Replace(dinoName, "i");
                }
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

        private string readImageAtCoords(int resolution, Bitmap source, int x, int y, int width, int height, bool onlyMaximalMatches, bool onlyNumbers, bool writingInWhite = true)
        {
            return readImage(resolution, SubImage(source, x, y, width, height), onlyMaximalMatches, onlyNumbers, writingInWhite);
        }

        private string readImage(int resolution, Bitmap source, bool onlyMaximalMatches, bool onlyNumbers, bool writingInWhite = true)
        {
            string result = "";
            Bitmap[,] theAlphabet = alphabet;

            if (onlyNumbers)
                theAlphabet = reducedAlphabet;


            Bitmap cleanedImage = removePixelsUnderThreshold(GetGreyScale(source, !writingInWhite), whiteThreshold);
            AddBitmapToDebug(cleanedImage);
            //source.Save("D:\\temp\\debug.png"); // TODO comment out
            //cleanedImage.Save("D:\\temp\\debug_cleaned.png");// TODO comment out


            for (int x = 0; x < cleanedImage.Width; x++)
            {
                bool foundLetter = false;
                int letterStart = 0;
                int letterEnd = 0;

                // look for the start pixel of the letter
                while (!(foundLetter == true || x >= cleanedImage.Width))
                {
                    foundLetter = HasWhiteInVerticalLine(cleanedImage, x);
                    x++;
                }

                if (foundLetter)
                {
                    letterStart = x - 1;
                    // look for the end of the letter
                    do
                    {
                        x++;
                    } while (HasWhiteInVerticalLine(cleanedImage, x) && x < cleanedImage.Width - 1);
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
                        //testImage.Save("D:\\temp\\debug_letterfound.png");// TODO comment out
                        Dictionary<int, float> matches = new Dictionary<int, float>();
                        float bestMatch = 0;
                        for (int l = 0; l < theAlphabet.GetLength(1); l++)
                        {
                            float match = 0;
                            if (theAlphabet[resolution, l] != null)
                                match = (float)(PercentageMatch(theAlphabet[resolution, l], testImage) * charWeighting[l]);
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
                        //Bitmap debugImg = new Bitmap(200, 50);
                        //using (Graphics g = Graphics.FromImage(debugImg))
                        //{
                        //    g.FillRectangle(Brushes.DarkCyan, 0, 0, debugImg.Width, debugImg.Height);
                        //    g.DrawImage(testImage, 1, 1, testImage.Width, testImage.Height);
                        //    int i = testImage.Width + 25;
                        //    Font font = new Font("Arial", 8);

                        //    foreach (int l in goodMatches.Keys)
                        //    {
                        //        g.DrawImage(theAlphabet[resolution, l], i, 1, theAlphabet[resolution, l].Width, theAlphabet[resolution, l].Height);
                        //        g.DrawString(Math.Round(goodMatches[l] * 100).ToString(), font, (bestMatch == goodMatches[l] ? Brushes.DarkGreen : Brushes.DarkRed), i, 35);
                        //        i += theAlphabet[resolution, l].Width + 15;
                        //    }
                        //    debugImg.Save("D:\\temp\\debug_letter" + DateTime.Now.ToString("HHmmss\\-fffffff\\-") + x + ".png");
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

            // replace half letters.
            result = result.Replace((char)15 + "n", "n");
            result = result.Replace((char)16 + "lK", "K");

            return result;

        }


        public float PercentageMatch(Bitmap img1, Bitmap img2)
        {
            //int whiteInFirstAlsoInSecond = 0;
            //int whiteInFirstNotInSecond = 0;
            //int blackInFirstAlsoInSecond = 0;
            //int blackInFirstNotInSecond = 0;
            double matches = 0;

            int minWidth = Math.Min(img1.Width, img2.Width);
            int minHeight = Math.Min(img1.Height, img2.Height);

            for (int i = 0; i < minWidth; i++)
            {
                for (int j = 0; j < minHeight; j++)
                {
                    if (img1.GetPixel(i, j).R == 0)
                    {
                        if (img2.GetPixel(i, j).R == 0)
                            matches++;
                        else if (img2.GetPixel(i, j).R < 255)
                            matches += 0.2;
                    }
                    else if (img1.GetPixel(i, j).R < 255)
                    {
                        if (img2.GetPixel(i, j).R == 255)
                            matches += 0.6;
                        else if (img2.GetPixel(i, j).R != 0)
                            matches += 0.8;
                        else matches += 0.2;
                    }
                    else
                    {
                        if (img2.GetPixel(i, j).R == 255)
                            matches++;
                        else if (img2.GetPixel(i, j).R != 0)
                            matches += 0.6;
                    }

                    //if (img1.GetPixel(i, j).R != 0)
                    //{
                    //    if (img2.GetPixel(i, j).R != 0)
                    //        whiteInFirstAlsoInSecond++;
                    //    else
                    //        whiteInFirstNotInSecond++;
                    //}
                    //else
                    //{
                    //    if (img2.GetPixel(i, j).R == 0)
                    //        blackInFirstAlsoInSecond++;
                    //    else
                    //        blackInFirstNotInSecond++;
                    //}
                }
            }

            // assume whites count as much as blacks.

            //int totalMatches = whiteInFirstAlsoInSecond + blackInFirstAlsoInSecond;
            //int totalFails = whiteInFirstNotInSecond + blackInFirstNotInSecond;

            //totalFails += (Math.Max(img1.Width, img2.Width) * Math.Max(img1.Height, img2.Height) - minWidth * minHeight);
            //float oldpercentage = (float)totalMatches / (totalMatches + totalFails);

            float percentage = (float)(matches / (Math.Max(img1.Width, img2.Width) * Math.Max(img1.Height, img2.Height)));

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

            string statName = "NameAndLevel";
            testbmp = SubImage(screenshotbmp, statPositions[statName].X, statPositions[statName].Y, 500, 30);
            string statOCR = readImage(currentResolution, testbmp, true, false);

            Regex r = new Regex(@"(.*)-?Lv[liI](\d*)Eq");
            MatchCollection mc = r.Matches(statOCR);

            if (mc.Count != 0)
                return true;

            return false;
        }

    }
}

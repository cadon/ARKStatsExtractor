using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr.PatternMatching
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RecognitionPatterns
    {
        private static readonly int[] OffsetX = { -1, 0, 1, 0 };
        private static readonly int[] OffsetY = { 0, -1, 0, 1 };

        [JsonProperty]
        internal List<TextData> Texts { get; } = new List<TextData>();

        [JsonProperty]
        public TrainingSettings TrainingSettings { get; set; } = new TrainingSettings();

        /// <summary>
        /// Save current ocr settings, e.g. after a new character was added.
        /// </summary>
        public event Action Save;

        private const string OnlyNumbersChars = "0123456789.%/:LEVEL";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="image">Used to display the context when entering a character.</param>
        /// <param name="tolerance"></param>
        /// <param name="onlyNumbers"></param>
        /// <returns></returns>
        public string FindMatchingChar(RecognizedCharData sym, bool[,] image, float tolerance = 0.3f, bool onlyNumbers = false)
        {
            var recognizedPattern = new Pattern(sym.Pattern, sym.YOffset);

            //// debug
            //Boolean2DimArrayConverter.ToDebugLog(recognizedPattern.Data);
            //var letterWasSortedOutByAperture = false;

            //string aperturesString = string.Empty;
            //for (int i = 0; i < 8; i++)
            //{
            //    if (i % 2 == 0)
            //        aperturesString += "|";
            //    aperturesString += ((recognizedPattern.Apertures >> i) & 1) == 1 ? "▬" : " ";
            //}
            //Debug.WriteLine($"Apertures: {aperturesString}");


            var widthRecognized = recognizedPattern.Width;
            var heightRecognized = recognizedPattern.Height;
            var recognizedHeightWithOffset = heightRecognized + sym.Coords.Y;

            float bestMatchDifference = float.MaxValue;
            string bestMatch = null;
            int maxAllowedSet = (int)(recognizedPattern.SetPixels * (1 + tolerance)) + 1;
            int minAllowedSet = (int)(recognizedPattern.SetPixels * (1 - tolerance));

            foreach (var c in Texts)
            {
                // if only numbers are expected, skip non numerical patterns
                if (onlyNumbers && !OnlyNumbersChars.Contains(c.Text))
                    continue;

                foreach (var pattern in c.Patterns)
                {
                    if (HammingWeight.SetBitCount((byte)(pattern.Apertures ^ recognizedPattern.Apertures)) > 1
                        || Math.Abs(pattern.YOffset - recognizedPattern.YOffset) > 3
                    ) continue;

                    //var currentLetterWasSortedOutByAperture = pattern.Apertures != recognizedPattern.Apertures;

                    int minWidth = Math.Min(pattern.Width, widthRecognized);
                    int maxWidth = Math.Max(pattern.Width, widthRecognized);
                    var widthDiff = maxWidth - minWidth;
                    if (pattern.SetPixels > maxAllowedSet
                        || pattern.SetPixels < minAllowedSet
                        || (widthDiff > 2 && widthDiff > maxWidth * 0.2)) continue; // if dimensions is too different ignore pattern

                    int minHeight = Math.Min(pattern.Height, heightRecognized);
                    int maxHeight = Math.Max(pattern.Height, heightRecognized);
                    var heightDiff = maxHeight - minHeight;
                    if (heightDiff > 2 && heightDiff > maxHeight * 0.2) continue;

                    var allowedDifference = pattern.SetPixels * 2 * tolerance;

                    // Attempted to do offset shifting here but got too many false recognitions here, might need some tweaking.
                    //var minOffsetX = xSizeFound > 2 ? -1 : 0;
                    //var maxOffsetX = xSizeFound > 2 ? 1 : 0;
                    //var minOffsetY = xSizeFound > 2 ? -1 : 0;
                    //var maxOffsetY = xSizeFound > 2 ? 1 : 0;

                    //for (var offSetX = minOffsetX; offSetX <= maxOffsetX; offSetX++)
                    //{
                    //    for (var offSetY = minOffsetY; offSetY <= maxOffsetY; offSetY++)
                    //    {
                    var dif = 0f;
                    var fail = false;

                    // y offset. Small character at the baseline like dots have mostly empty pixels.
                    var yStart = Math.Min(sym.Coords.Y, pattern.YOffset);
                    var patternHeightWithOffset = pattern.Height + pattern.YOffset;

                    Pattern overlappingPattern; // the pattern that is more than 1 px larger than the other. testing that margin is simpler.
                    int widthTesting;
                    if (widthRecognized > pattern.Width)
                    {
                        widthTesting = widthRecognized;
                        overlappingPattern = recognizedPattern;
                    }
                    else
                    {
                        widthTesting = pattern.Width;
                        overlappingPattern = pattern;
                    }
                    var widthMinPlusOne = Math.Min(widthRecognized, pattern.Width) + 1;

                    // pixels too far outside of the narrower pattern never have a match or a possible neighbor in the other one, they can be sorted out fast
                    if (widthTesting - widthMinPlusOne > 0)
                    {
                        for (int y = 0; !fail && y < overlappingPattern.Height; y++)
                        {
                            for (var x = widthMinPlusOne; x < widthTesting; x++)
                            {
                                if (overlappingPattern[x, y])
                                {
                                    dif += 1;
                                    if (dif > allowedDifference)
                                    {
                                        fail = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    for (var y = yStart; !fail && y < recognizedHeightWithOffset && y < patternHeightWithOffset; y++)
                    {
                        //var curPatternY = y;// + offSetY;
                        var patternYIndex = y - pattern.YOffset;
                        var recognizedYIndex = y - sym.Coords.Y;

                        for (var x = 0; x < widthMinPlusOne; x++)
                        {
                            //var curPatternX = x;// + offSetX;
                            //if (curPatternX < 0 || curPatternY < 0) continue;
                            //if (y >= heightRecognized || x >= widthRecognized) continue;

                            var cHave = recognizedYIndex >= 0 && x < widthRecognized && recognizedPattern[x, recognizedYIndex];
                            var pHave = patternYIndex >= 0 && x < pattern.Width && pattern[x, patternYIndex];

                            // if the bits are different, check if the total number of different bits is too large for a match and if to ignore this pattern
                            if (cHave != pHave)
                            {
                                // tolerance of difference if a nearby bit is equal
                                dif += IsNearby(cHave ? pattern.Data : recognizedPattern.Data, x, cHave ? patternYIndex : recognizedYIndex) ? 0.4f : 1f;
                                if (dif > allowedDifference)
                                {
                                    fail = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!fail && bestMatchDifference > dif)
                    {
                        if (dif == 0)
                        {
                            //Debug.WriteLine($"matched with {c.Text} (dif: {dif})");
                            //if (currentLetterWasSortedOutByAperture)
                            //    Debug.WriteLine("Would have been sorted out by Aperture");
                            return c.Text; // there is no better match
                        }


                        //letterWasSortedOutByAperture = currentLetterWasSortedOutByAperture;
                        bestMatchDifference = dif;
                        bestMatch = c.Text;
                    }
                    //    }
                    //}
                }
            }

            if (!string.IsNullOrEmpty(bestMatch))
            {
                //Debug.WriteLine($"matched with {bestMatch} (dif: {bestMatchDifference})");
                //if (letterWasSortedOutByAperture)
                //    Debug.WriteLine("Would have been sorted out by Aperture");
                return bestMatch;
            }


            // no match was found

            if (!TrainingSettings.IsTrainingEnabled)
            {
                return "�"; //string.Empty;
            }

            var manualChar = new RecognitionTrainingForm(sym, image).Prompt();

            if (string.IsNullOrEmpty(manualChar))
                return manualChar;

            return AddNewPattern(recognizedPattern, manualChar);
        }

        /// <summary>
        /// Calculates the matching proportion between two patterns. Currently only used in the editor, so it gives different results than the actual OCR algorithm.
        /// </summary>
        internal static void PatternMatch(bool[,] template, bool[,] recognized, out float match, out int offset)
        {
            offset = 0;
            if (template == null || recognized == null || template.Length == 0)
            {
                match = 0;
                return;
            }

            int templateWidth = template.GetLength(0);
            int templateHeight = template.GetLength(1);

            int recognizedWidth = recognized.GetLength(0);
            int recognizedHeight = recognized.GetLength(1);

            int width = Math.Min(templateWidth, recognizedWidth);
            int height = Math.Min(templateHeight, recognizedHeight);

            int maxWidth = Math.Max(templateWidth, recognizedWidth);
            int maxHeight = Math.Max(templateHeight, recognizedHeight);

            int testArea = width * height;
            int maxArea = maxWidth * maxHeight;

            if (testArea == 0 || maxArea / testArea >= 2)
            {
                // match is less than 0.5
                match = 0.5f;
                return;
            }

            float equalPixels = 0;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var curPatternX = x;// + offSetX;
                    var curPatternY = y;// + offSetY;

                    var cHave = recognized[curPatternX, curPatternY];
                    var pHave = template[x, y];

                    // if the bits are different, check if the total number of different bits is too large for a match and if to ignore this pattern
                    if (cHave == pHave)
                    {
                        equalPixels += 1;
                    }
                    else
                    {
                        // tolerance of difference if a nearby bit is equal
                        equalPixels += IsNearby(cHave ? template : recognized, x, y) ? 0.6f : 0;
                    }
                }
            }

            match = equalPixels * 2 / (maxArea + testArea);
        }

        /// <summary>
        /// Returns true if a nearby (8 directions) pixel is set.
        /// </summary>
        private static bool IsNearby(bool[,] pattern, int x, int y)
        {
            var width = pattern.GetLength(0);
            var height = pattern.GetLength(1);
            for (int i = 0; i < OffsetX.Length; i++)
            {
                var nextX = OffsetX[i] + x;
                var nextY = OffsetY[i] + y;

                var isSafe = nextX >= 0 && nextX < width && nextY >= 0 && nextY < height;
                if (!isSafe)
                {
                    continue;
                }

                if (pattern[nextX, nextY])
                {
                    return true;
                }
            }

            return false;
        }

        private string AddNewPattern(Pattern newPattern, string text)
        {
            if (string.IsNullOrEmpty(text) || newPattern == null) return text;

            var pat = Texts.FirstOrDefault(x => x.Text == text);
            if (pat != null)
            {
                pat.Patterns.Add(newPattern);
            }
            else
            {
                Texts.Add(newPattern.CreateTextData(text));
            }

            Save?.Invoke();

            return text;
        }

        public void AddPattern(string text, Bitmap bmp)
        {
            if (string.IsNullOrEmpty(text)) return;

            var newPattern = Pattern.FromBmp(bmp);
            if (newPattern == null) return;

            //// DEBUG
            //Debug.WriteLine(text);
            //Boolean2DimArrayConverter.ToDebugLog(newPattern.Data);

            var textData = Texts.FirstOrDefault(x => x.Text == text);

            if (textData == null)
            {
                Texts.Add(new TextData { Patterns = new List<Pattern> { newPattern }, Text = text });
                return;
            }

            // check if pattern is already added
            bool alreadyAdded = false;
            foreach (var p in textData.Patterns)
            {
                if (p.Equals(newPattern))
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (!alreadyAdded)
                textData.Patterns.Add(newPattern);
        }
    }
}
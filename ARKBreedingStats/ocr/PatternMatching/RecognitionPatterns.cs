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

        public string FindMatchingChar(RecognizedCharData sym, Image originalImg, float tolerance = 0.15f, bool onlyNumbers = false)
        {
            var curPattern = sym.Pattern;
            var xSizeFound = curPattern.GetLength(0);
            var ySizeFound = curPattern.GetLength(1);

            float bestMatchDifference = float.MaxValue;
            string bestMatch = null;

            foreach (var c in Texts)
            {
                // if only numbers are expected, skip non numerical patterns
                if (onlyNumbers && !"0123456789.,%/:LEVEL".Contains(c.Text))
                    continue;

                foreach (var pattern in c.Patterns)
                {
                    var possibleDif = ((pattern.Length + sym.Pattern.Length) / 2) * tolerance;
                    if (Math.Abs(pattern.Length - curPattern.Length) > possibleDif) continue;

                    possibleDif = pattern.SetPixels * 2 * tolerance;


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

                    // TODO sort out small recognized patterns that would match 100 % of their size with a lot of patterns, e.g. dots

                    for (var x = 0; !fail && x < xSizeFound && x < pattern.Width; x++)
                    {
                        for (var y = 0; !fail && y < ySizeFound && y < pattern.Height; y++)
                        {
                            var curPatternX = x;// + offSetX;
                            var curPatternY = y;// + offSetY;
                            if (curPatternX >= 0 && curPatternY >= 0 && curPatternY < ySizeFound && curPatternX < xSizeFound)
                            {
                                var cHave = curPattern[curPatternX, curPatternY];
                                var pHave = pattern[x, y];

                                // if the bits are different, check if the total number of different bits is too large for a match and if to ignore this pattern
                                if (cHave != pHave)
                                {
                                    // tolerance of difference if a nearby bit is equal
                                    dif += IsNearby(cHave ? pattern.Data : curPattern, x, y) ? 0.33f : 1f;
                                    if (dif > possibleDif)
                                    {
                                        fail = true;
                                    }
                                }
                            }
                        }
                    }

                    if (!fail && bestMatchDifference > dif)
                    {
                        if (dif == 0)
                            return c.Text; // there is no better match

                        bestMatchDifference = dif;
                        bestMatch = c.Text;
                    }
                    //    }
                    //}
                }
            }

            if (!string.IsNullOrEmpty(bestMatch))
            {
                return bestMatch;
            }


            // no match was found

            if (!TrainingSettings.IsTrainingEnabled)
            {
                return "�"; //string.Empty;
            }

            var manualChar = new RecognitionTrainingForm(sym, originalImg).Prompt();

            if (string.IsNullOrEmpty(manualChar))
                return manualChar;

            return AddNewPattern(sym, manualChar, curPattern);
        }

        /// <summary>
        /// Calculates the matching proportion between two patterns.
        /// </summary>
        internal static void PatternMatch(bool[,] template, bool[,] recognized, out float match, out int offset)
        {
            offset = 0;
            if (template == null || recognized == null)
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

            if (maxArea / testArea >= 2)
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
        /// Returns true if a nearby bit is set.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool IsNearby(bool[,] pattern, int x, int y)
        {
            var width = pattern.GetLength(0);
            var height = pattern.GetLength(1);
            for (int i = 0; i < OffsetX.Length; i++)
            {
                var nextX = OffsetX[i] + x;
                var nextY = OffsetY[i] + y;

                var isSafe = nextX > 0 && nextX < width && nextY > 0 && nextY < height;
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

        private string AddNewPattern(RecognizedCharData sym, string manualChar, bool[,] curPattern)
        {
            var pat = Texts.FirstOrDefault(x => x.Text == manualChar);
            if (pat != null)
            {
                pat.Patterns.Add(curPattern);
            }
            else
            {
                Texts.Add(sym.ToCharData(manualChar));
            }

            Save?.Invoke();

            return manualChar;
        }

        public void AddPattern(string text, Bitmap bmp)
        {
            var newPattern = Pattern.FromBmp(bmp);
            if (newPattern == null) return;

            var textData = Texts.FirstOrDefault(x => x.Text == text);

            if (textData != null)
            {
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
            else
            {
                Texts.Add(new TextData { Patterns = new List<Pattern> { newPattern }, Text = text });
            }
        }
    }
}
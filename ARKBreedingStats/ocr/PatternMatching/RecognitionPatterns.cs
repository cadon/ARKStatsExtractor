using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using ARKBreedingStats.settings;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class RecognitionPatterns
    {
        private static readonly int[] offsetX = { -1, 0, 1, 0 };
        private static readonly int[] offsetY = { 0, -1, 0, 1 };

        // ReSharper disable once InconsistentNaming
        private static readonly SettingsController<RecognitionPatterns> settingsController;

        public List<TextData> Texts { get; set; } = new List<TextData>();

        public static RecognitionPatterns Settings => settingsController.Settings;

        [DefaultValue(true)]
        public bool IsTrainingEnabled { get; set; } = true;

        public TrainingSettings TrainingSettings { get; set; } = new TrainingSettings();

        static RecognitionPatterns()
        {
            settingsController = new SettingsController<RecognitionPatterns>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        public string FindMatchingChar(RecognizedCharData sym, Image originalImg, float tolerance = 0.1f)
        {
            var curPattern = sym.Pattern;

            foreach (var c in this.Texts)
            {
                foreach (var pattern in c.Patterns)
                {
                    var possibleDif = ((pattern.Length + sym.Pattern.Length) / 2) * tolerance;
                    if (Math.Abs(pattern.Length - curPattern.Length) > possibleDif) continue;

                    var blackCount = pattern.CountBlacks();

                    possibleDif = blackCount * (1.5f * tolerance);

                    var xSizeFound = curPattern.GetLength(0);
                    var ySizeFound = curPattern.GetLength(1);

                    // Attempted to do offset shifting here but got too many false recognitions here, might need some tweaking.
                    //var minOffsetX = xSizeFound > 2 ? -1 : 0;
                    //var maxOffsetX = xSizeFound > 2 ? 1 : 0;
                    //var minOffsetY = xSizeFound > 2 ? -1 : 0;
                    //var maxOffsetY = xSizeFound > 2 ? 1 : 0;
                    var minOffsetX = 0;
                    var maxOffsetX = 0;
                    var minOffsetY = 0;
                    var maxOffsetY = 0;

                    for (var offSetX = minOffsetX; offSetX <= maxOffsetX; offSetX++)
                    {
                        for (var offSetY = minOffsetY; offSetY <= maxOffsetY; offSetY++)
                        {
                            var dif = 0f;
                            var fail = false;
                            for (var x = 0; !fail && x < xSizeFound && x < pattern.Width; x++)
                            {
                                for (var y = 0; !fail && y < ySizeFound && y < pattern.Height; y++)
                                {
                                    var curPatternX = x + offSetX;
                                    var curPatternY = y + offSetY;
                                    if (curPatternX >= 0 && curPatternY >= 0 && curPatternY < ySizeFound && curPatternX < xSizeFound)
                                    {
                                        var cHave = curPattern[curPatternX, curPatternY];
                                        var pHave = pattern[x, y];
                                        if ((cHave || pHave) && curPattern[curPatternX, curPatternY] != pattern[x, y])
                                        {
                                            dif += IsNearby(cHave ? pattern : curPattern, x, y) ? 0.33f : 1f;
                                            if (dif > possibleDif)
                                            {
                                                fail = true;
                                            }
                                        }
                                    }
                                }
                            }

                            if (!fail)
                            {
                                return c.Text.ToUpper();
                            }
                        }
                    }
                }
            }

            if (!this.IsTrainingEnabled)
            {
                return " ";
            }

            var manualChar = new RecognitionTrainingForm(sym, originalImg).Prompt();

            if (manualChar == null)
            {
                return null;
            }

            return this.AddNewPattern(sym, manualChar, curPattern);
        }

        private static bool IsNearby(Pattern pattern, int x, int y)
        {
            var width = pattern.Width;
            var height = pattern.Height;
            for (int i = 0; i < offsetX.Length; i++)
            {
                var nextX = offsetX[i] + x;
                var nextY = offsetY[i] + y;

                var isSafe = nextX > 0 && nextX < width && nextY > 0 && nextY < height;
                if (!isSafe)
                {
                    continue;
                }

                if (pattern[x,y])
                {
                    return true;
                }
            }

            return false;
        }

        private string AddNewPattern(RecognizedCharData sym, string manualChar, bool[,] curPattern)
        {
            var pat = this.Texts.FirstOrDefault(x => x.Text == manualChar);
            if (pat != null)
            {
                pat.Patterns.Add(curPattern);
            }
            else
            {
                this.Texts.Add(sym.ToCharData(manualChar));
            }

            this.Save();

            return manualChar;
        }

        public void Save()
        {
            settingsController.Save();
        }
    }
}
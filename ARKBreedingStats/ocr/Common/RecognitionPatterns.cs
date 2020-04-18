using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using ARKBreedingStats.settings;

namespace ARKBreedingStats.ocr.Common
{
    public class RecognitionPatterns
    {
        private static readonly SettingsController<RecognitionPatterns> SettingsController;
        private bool isTrainingEnabled = true;
        public List<TextData> Texts { get; set; } = new List<TextData>();

        public static RecognitionPatterns Settings => SettingsController.Settings;

        [DefaultValue(true)]
        public bool IsTrainingEnabled
        {
            get => this.isTrainingEnabled;
            set
            {
                this.isTrainingEnabled = value;
            }
        }

        static RecognitionPatterns()
        {
            SettingsController = new SettingsController<RecognitionPatterns>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        public string FindMatchingChar(RecognizedCharData sym, Image originalImg, float tolerance = 0.1f)
        {
            var curPattern = sym.Pattern;

            foreach (var c in this.Texts)
            {
                foreach (var pattern in c.Patterns)
                {
                    var possibleDif = pattern.Length * tolerance;
                    if (Math.Abs(pattern.Length - curPattern.Length) > possibleDif) continue;

                    var xSizeFound = curPattern.GetLength(0);
                    var ySizeFound = curPattern.GetLength(1);
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
                            var dif = 0;
                            var fail = false;
                            for (var x = 0; !fail && x < xSizeFound && x < pattern.Width; x++)
                            {
                                for (var y = 0; !fail && y < ySizeFound && y < pattern.Height; y++)
                                {
                                    var curPatternX = x + offSetX;
                                    var curPatternY = y + offSetY;
                                    if (curPatternX >= 0 && curPatternY >= 0 && curPatternY < ySizeFound && curPatternX < xSizeFound)
                                    {
                                        if (curPattern[curPatternX, curPatternY] != pattern[x, y])
                                        {
                                            dif++;
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
                                return c.Text;
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
            SettingsController.Save();
        }
    }
}
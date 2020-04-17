using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        public List<CharData> Chars { get; set; } = new List<CharData>();

        public int Width { get; set; }

        public int Height { get; set; }

        public static RecognitionPatterns Settings => SettingsController.Settings;

        static RecognitionPatterns()
        {
            SettingsController = new SettingsController<RecognitionPatterns>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        public char FindMatchingChar(CharData sym, Image originalImg, float tolerance = 0.1f)
        {
            var curPattern = sym.Patterns[0];

            foreach (var c in this.Chars)
            {
                foreach (var pattern in c.Patterns)
                {
                    var possibleDif = pattern.Length * tolerance;
                    if (Math.Abs(pattern.Length - curPattern.Length) > possibleDif) continue;

                    int dif = 0;
                    var fail = false;
                    for (int x = 0; !fail && x < curPattern.GetLength(0) && x < pattern.GetLength(0); x++)
                    {
                        for (int y = 0; !fail && y < curPattern.GetLength(1) && y < pattern.GetLength(1); y++)
                        {
                            if (curPattern[x, y] != pattern[x, y])
                            {
                                dif++;
                                if (dif > possibleDif)
                                {
                                    fail = true;
                                }
                            }
                        }
                    }

                    if (!fail)
                    {
                        return c.Char;
                    }
                }
            }

            var manualChar = new RecognitionTrainingForm(curPattern, originalImg).Prompt();

            if (manualChar == '\0')
            {
                return manualChar;
            }

            var pat = this.Chars.FirstOrDefault(x => x.Char == manualChar);
            if (pat != null)
            {
                pat.Patterns.Add(curPattern);
            }
            else
            {
                sym.Char = manualChar;
                this.Chars.Add(sym);
            }

            SettingsController.Save();

            return manualChar;
        }
    }

    public class CharData
    {
        public char Char { get; set; }

        public List<bool[,]> Patterns { get; set; } = new List<bool[,]>();
    }
}
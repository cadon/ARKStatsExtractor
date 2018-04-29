using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    static class Utils
    {
        /// <summary>
        /// This method returns a shade from red over yellow to green, corresponding to the value p (0-100). If light is given, the brightness is adjusted (-1 to 1)
        /// </summary>
        /// <param name="percent">percent that determines the shade between red and green (0 to 100)</param>
        /// <param name="light">double from -1 to 1. Values greater zero make the color brighter, lower than zero make the color darker.</param>
        /// <param name="blue">true if color should fade from violett to green</param>
        /// <returns>the calculated color.</returns>
        public static Color getColorFromPercent(int percent, double light = 0, bool blue = false)
        {
            getRGBFromPercent(out int r, out int g, out int b, percent, light);
            if (blue)
                return Color.FromArgb(b, g, r);
            else
                return Color.FromArgb(r, g, b);
        }

        public static string getARKmlFromPercent(string text, int percent, double light = 0)
        {
            getRGBFromPercent(out int r, out int g, out int b, percent, light);
            return getARKml(text, r, g, b);
        }

        public static string getARKml(string text, int r, int g, int b)
        {
            return "<RichColor Color=\"" + Math.Round(r / 255d, 2).ToString(CultureInfo.InvariantCulture) + "," + Math.Round(g / 255d, 2).ToString(CultureInfo.InvariantCulture) + "," + Math.Round(b / 255d, 2).ToString(CultureInfo.InvariantCulture) + ",1\">" + text + "</>";
        }

        private static void getRGBFromPercent(out int r, out int g, out int b, int percent, double light = 0)
        {
            if (light > 1) { light = 1; }
            if (light < -1) { light = -1; }
            g = (int)(percent * 5.1); // g = percent * 255 / 50 = percent * 5.1
            r = 511 - g;
            b = 0;
            if (r < 0) { r = 0; }
            if (g < 0) { g = 0; }
            if (r > 255) { r = 255; }
            if (g > 255) { g = 255; }
            if (light > 0)
            {
                r = (int)((255 - r) * light + r);
                g = (int)((255 - g) * light + g);
                b = (int)((255 - b) * light + b);
            }
            else
            {
                light += 1;
                r = (int)(r * light);
                g = (int)(g * light);
                //b = (int)(b * light); // b == 0 always
            }
        }

        public static string sexSymbol(Sex g)
        {
            switch (g)
            {
                case Sex.Male:
                    return "♂";
                case Sex.Female:
                    return "♀";
                default:
                    return "?";
            }
        }

        public static Color sexColor(Sex g)
        {
            switch (g)
            {
                case Sex.Male:
                    return Color.FromArgb(220, 235, 255);
                case Sex.Female:
                    return Color.FromArgb(255, 230, 255);
                default:
                    return SystemColors.Control;
            }
        }

        public static string statusSymbol(CreatureStatus s)
        {
            switch (s)
            {
                case CreatureStatus.Dead:
                    return "†";
                case CreatureStatus.Unavailable:
                    return "✗";
                case CreatureStatus.Obelisk:
                    return "⌂";
                default:
                    return "✓";
            }
        }

        public static Sex nextSex(Sex s)
        {
            switch (s)
            {
                case Sex.Female:
                    return Sex.Male;
                case Sex.Male:
                    return Sex.Unknown;
                default:
                    return Sex.Female;
            }
        }

        public static CreatureStatus nextStatus(CreatureStatus s)
        {
            switch (s)
            {
                case CreatureStatus.Available:
                    return CreatureStatus.Unavailable;
                case CreatureStatus.Unavailable:
                    return CreatureStatus.Dead;
                case CreatureStatus.Dead:
                    return CreatureStatus.Obelisk;
                default:
                    return CreatureStatus.Available;
            }
        }

        public static string levelPercentile(int l)
        {
            // percentiles assuming a normal distribution of 180 levels on 7 stats
            double[] prb = new double[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 99.99, 99.98, 99.95, 99.88, 99.72, 99.40, 98.83, 97.85, 96.28, 93.94, 90.62, 86.20, 80.61, 73.93, 66.33, 58.10, 49.59, 41.19, 33.26, 26.08, 19.85, 14.66, 10.50, 7.30, 4.92, 3.21, 2.04, 1.25, 0.75, 0.43, 0.24, 0.13 };
            if (l < 0) l = 0;
            if (l >= prb.Length) l = prb.Length - 1;
            return "This level is in the top " + prb[l].ToString("N2") + "% of what you can find.";
        }

        public static string statName(int s, bool abr = false, bool glow = false)
        {
            if (s >= 0 && s < 8)
            {
                string[] statNames;
                if (glow)
                {
                    if (abr) statNames = new string[] { "HP", "CC", "CRe", "Fo", "We", "CRa", "Sp", "To" };
                    else statNames = new string[] { "Health", "Ch Capacity", "Ch Regen", "Food", "Weight", "Ch Emission Range", "Speed", "Torpidity" };
                }
                else
                {
                    if (abr) statNames = new string[] { "HP", "St", "Ox", "Fo", "We", "Dm", "Sp", "To" };
                    else statNames = new string[] { "Health", "Stamina", "Oxygen", "Food", "Weight", "Damage", "Speed", "Torpidity" };
                }
                return statNames[s];
            }
            return "";
        }

        public static int precision(int s)
        {
            int p = 1;
            if (s == 5 || s == 6)
            {
                p = 3; // damage and speed are percentagevalues, need more precision
            }
            return p;
        }

        public static string duration(TimeSpan ts)
        {
            return ts.ToString("d':'hh':'mm':'ss");
        }

        public static string duration(int seconds)
        {
            return duration(new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// Returns the formated timespan and also the DateTime when the timespan is over
        /// </summary>
        /// <param name="ts">timespan of countdown</param>
        /// <returns>Returns the timespan and the DateTime when the timespan is over</returns>
        public static string durationUntil(TimeSpan ts)
        {
            return ts.ToString("d':'hh':'mm':'ss") + " (until: " + shortTimeDate(DateTime.Now.Add(ts)) + ")";
        }

        public static string shortTimeDate(DateTime dt, bool omitDateIfToday = true)
        {
            return dt.ToShortTimeString() + (omitDateIfToday && DateTime.Today == dt.Date ? "" : " - " + dt.ToShortDateString());
        }

        public static string timeLeft(DateTime dt)
        {
            if (dt < DateTime.Now)
                return "-";
            return duration(dt.Subtract(DateTime.Now));
        }

        public static double imprintingGainPerCuddle(double maturationTime, double cuddleIntervalMultiplier)
        {
            return 1d / Math.Max(1, Math.Floor(maturationTime / (28800 * cuddleIntervalMultiplier)));
        }

        /// <summary>
        /// Returns either black or white, depending on the backcolor, so text can be read well
        /// </summary>
        /// <param name="backColor"></param>
        /// <returns>ForeColor</returns>
        public static Color ForeColor(Color backColor)
        {
            return ((backColor.R * .3f + backColor.G * .59f + backColor.B * .11f) < 110 ? Color.White : SystemColors.ControlText);
        }

        public static bool ShowTextInput(string text, out string input, string title = "", string preInput = "")
        {
            Form inputForm = new Form()
            {
                Width = 250,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false
            };
            Label textLabel = new Label() { Left = 20, Top = 15, Text = text };
            TextBox textBox = new TextBox() { Left = 20, Top = 40, Width = 200 };
            Button buttonOK = new Button() { Text = "OK", Left = 120, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            Button buttonCancel = new Button() { Text = "Cancel", Left = 20, Width = 80, Top = 70, DialogResult = DialogResult.Cancel };
            buttonOK.Click += (sender, e) => { inputForm.Close(); };
            buttonCancel.Click += (sender, e) => { inputForm.Close(); };
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(buttonOK);
            inputForm.Controls.Add(buttonCancel);
            inputForm.Controls.Add(textLabel);
            inputForm.AcceptButton = buttonOK;
            inputForm.CancelButton = buttonCancel;
            textBox.Text = preInput;
            textBox.SelectAll();

            input = "";
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                input = textBox.Text;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Guid ConvertIdToGuid(long id)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(id).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}

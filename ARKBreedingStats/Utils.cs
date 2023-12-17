using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARKBreedingStats.values;

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
        public static Color GetColorFromPercent(int percent, double light = 0, bool blue = false)
        {
            GetRgbFromPercent(out int r, out int g, out int b, percent, light);
            return blue ? Color.FromArgb(b, g, r) : Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// String with ARKml tags.
        /// </summary>
        public static string GetARKmlFromPercent(string text, int percent, double light = 0)
        {
            GetRgbFromPercent(out int r, out int g, out int b, percent, light);
            return GetARKml(text, r, g, b);
        }

        /// <summary>
        /// Returns a string with ARKml tags. Currently that doesn't seem to be supported anymore by the ARK chat.
        /// </summary>
        public static string GetARKml(string text, int r, int g, int b)
        {
            return
                $"<RichColor Color=\"{Math.Round(r / 255d, 2)},{Math.Round(g / 255d, 2)},{Math.Round(b / 255d, 2)},1\">{text}</>";
        }

        /// <summary>
        /// RGB values for a given percentage (0-100). 0 is red, 100 is green. Light can be adjusted (1 bright, 0 default, -1 dark).
        /// </summary>
        private static void GetRgbFromPercent(out int r, out int g, out int b, int percent, double light = 0)
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

        /// <summary>
        /// Adjusts the lightness of a color.
        /// </summary>
        public static Color AdjustColorLight(Color color, double light = 0)
        {
            if (light == 0) return color;

            if (light > 1) { light = 1; }
            if (light < -1) { light = -1; }
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            if (light > 0)
            {
                r = (byte)((255 - r) * light + r);
                g = (byte)((255 - g) * light + g);
                b = (byte)((255 - b) * light + b);
            }
            else
            {
                light += 1;
                r = (byte)(r * light);
                g = (byte)(g * light);
                b = (byte)(b * light);
            }
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Returns a color from a hue value.
        /// </summary>
        /// <param name="hue">red: 0, green: 120, blue: 240</param>
        /// <param name="light">-1 very dark, 0 default, 1 very bright</param>
        public static Color ColorFromHue(int hue, double light = 0)
        {
            hue %= 360;
            if (hue < 0) hue += 360;
            // there are six sections, 0-120, 120-240, 240-360
            // in each section one channel is either ascending, descending, max or 0
            byte sectionPos = (byte)(hue % 60);
            byte asc = (byte)(sectionPos * 4.25); // == sectionPos * 255 / 60;
            byte desc = (byte)(255 - asc);
            const byte zero = 0;
            const byte max = 255;

            byte r, g, b;

            if (hue < 60)
            {
                r = max;
                g = asc;
                b = zero;
            }
            else if (hue < 120)
            {
                r = desc;
                g = max;
                b = zero;
            }
            else if (hue < 180)
            {
                r = zero;
                g = max;
                b = asc;
            }
            else if (hue < 240)
            {
                r = zero;
                g = desc;
                b = max;
            }
            else if (hue < 300)
            {
                r = asc;
                g = zero;
                b = max;
            }
            else
            {
                r = max;
                g = zero;
                b = desc;
            }

            if (light != 0)
            {
                if (light > 1) { light = 1; }
                if (light < -1) { light = -1; }

                if (light > 0)
                {
                    r = (byte)((255 - r) * light + r);
                    g = (byte)((255 - g) * light + g);
                    b = (byte)((255 - b) * light + b);
                }
                else
                {
                    light += 1;
                    r = (byte)(r * light);
                    g = (byte)(g * light);
                    b = (byte)(b * light);
                }
            }
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Used for highlighting critical levels. Level 254 is the highest level that allows dom leveling.
        /// </summary>
        public static Color Level254 => Color.FromArgb(0, 196, 255);

        /// <summary>
        /// Used for highlighting critical levels. Level 255 is the highest level that can be saved.
        /// </summary>
        public static Color Level255 => Color.FromArgb(255, 0, 159);

        /// <summary>
        /// Color that represents a mutation.
        /// </summary>
        public static Color MutationColor => Color.FromArgb(225, 192, 255);
        /// <summary>
        /// Color that represents a mutation number over the limit.
        /// </summary>
        public static Color MutationColorOverLimit => Color.FromArgb(255, 200, 200);
        /// <summary>
        /// Color that represents a mutation marker or line, is more vibrant than the MutationColor.
        /// </summary>
        public static Color MutationMarkerColor => Color.Magenta;
        /// <summary>
        /// Color that represents a possible (not guaranteed) mutation marker or line.
        /// </summary>
        public static Color MutationMarkerPossibleColor => Color.FromArgb(204, 123, 255);

        /// <summary>
        /// String icon that represents a sex.
        /// </summary>
        public static string SexSymbol(Sex s)
        {
            switch (s)
            {
                case Sex.Male:
                    return "♂";
                case Sex.Female:
                    return "♀";
                default:
                    return "?";
            }
        }

        /// <summary>
        /// Color for a sex.
        /// </summary>
        public static Color SexColor(Sex s)
        {
            switch (s)
            {
                case Sex.Male:
                    return Color.FromArgb(220, 235, 255);
                case Sex.Female:
                    return Color.FromArgb(255, 230, 255);
                default:
                    return SystemColors.Control;
            }
        }

        /// <summary>
        /// String icon that represents a creature status.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="defaultReturn"></param>
        /// <returns></returns>
        public static string StatusSymbol(CreatureStatus status, string defaultReturn = "✓")
        {
            switch (status)
            {
                case CreatureStatus.Dead: return "†";
                case CreatureStatus.Unavailable: return "✗";
                case CreatureStatus.Obelisk: return "⌂";
                case CreatureStatus.Cryopod: return "Θ";
                default: return defaultReturn;
            }
        }

        /// <summary>
        /// Localized creature status string.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string StatusText(CreatureStatus status)
        {
            switch (status)
            {
                case CreatureStatus.Available: return Loc.S(CreatureStatus.Available.ToString());
                case CreatureStatus.Dead: return Loc.S(CreatureStatus.Dead.ToString());
                case CreatureStatus.Unavailable: return Loc.S(CreatureStatus.Unavailable.ToString());
                case CreatureStatus.Obelisk: return Loc.S(CreatureStatus.Obelisk.ToString());
                case CreatureStatus.Cryopod: return Loc.S(CreatureStatus.Cryopod.ToString());
                default: return "n/a";
            }
        }

        /// <summary>
        /// Returns the next possible sex.
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        public static Sex NextSex(Sex sex)
        {
            switch (sex)
            {
                case Sex.Female:
                    return Sex.Male;
                case Sex.Male:
                    return Sex.Unknown;
                default:
                    return Sex.Female;
            }
        }

        /// <summary>
        /// Returns the next possible creature status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static CreatureStatus NextStatus(CreatureStatus status)
        {
            switch (status)
            {
                case CreatureStatus.Available: return CreatureStatus.Unavailable;
                case CreatureStatus.Unavailable: return CreatureStatus.Dead;
                case CreatureStatus.Dead: return CreatureStatus.Obelisk;
                case CreatureStatus.Obelisk: return CreatureStatus.Cryopod;
                default: return CreatureStatus.Available;
            }
        }

        /// <summary>
        /// Probability of the occurrence of a stat level, assuming a normal distribution of 180 levels on 7 stats.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string LevelPercentile(int level)
        {
            double[] prb = { 100, 100, 100, 100, 100, 100, 100, 100, 100, 99.99, 99.98, 99.95, 99.88, 99.72, 99.40, 98.83, 97.85, 96.28, 93.94, 90.62, 86.20, 80.61, 73.93, 66.33, 58.10, 49.59, 41.19, 33.26, 26.08, 19.85, 14.66, 10.50, 7.30, 4.92, 3.21, 2.04, 1.25, 0.75, 0.43, 0.24, 0.13 };
            if (level < 0) level = 0;
            if (level >= prb.Length) level = prb.Length - 1;
            return string.Format(Loc.S("topPercentileLevel"), prb[level].ToString("N2"));
        }

        /// <summary>
        /// Default stat names, localized.
        /// </summary>
        private static string[] _statNames;

        /// <summary>
        /// Default stat names, abbreviated and localized.
        /// </summary>
        private static string[] _statNamesAbb;

        /// <summary>
        /// Stat index for a given stat name abbreviation. Localized abbreviations are used, English abbreviations work as well if the localized abbreviations don't have an equal abbreviation already. Case insensitive.
        /// </summary>
        public static Dictionary<string, int> StatAbbreviationToIndex;

        public static void InitializeLocalizations()
        {
            if (_statNames == null) _statNames = new string[Stats.StatsCount];
            if (_statNamesAbb == null) _statNamesAbb = new string[Stats.StatsCount];
            if (StatAbbreviationToIndex == null) StatAbbreviationToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            else StatAbbreviationToIndex.Clear();

            for (int si = 0; si < Stats.StatsCount; si++)
            {
                _statNames[si] = Loc.S(StatNameKeys[si]);
                _statNamesAbb[si] = Loc.S(StatNameKeys[si] + "_Abb");
                StatAbbreviationToIndex.Add(_statNamesAbb[si], si);
            }

            // load abbreviations of invariant culture (here English) as a fallback
            var defaultCulture = CultureInfo.InvariantCulture;
            for (int si = 0; si < Stats.StatsCount; si++)
            {
                var key = Loc.S(StatNameKeys[si] + "_Abb", defaultCulture);
                if (StatAbbreviationToIndex.ContainsKey(key)) continue;
                StatAbbreviationToIndex.Add(key, si);
            }
        }

        /// <summary>
        /// Key names of stats like they're used in the localization files.
        /// </summary>
        private static readonly string[] StatNameKeys = { "Health", "Stamina", "Torpidity", "Oxygen", "Food", "Water", "Temperature", "Weight", "Damage", "Speed", "Fortitude", "Crafting Speed" };

        /// <summary>
        /// Returns a string that represents the localized stat name.
        /// </summary>
        /// <param name="statIndex"></param>
        /// <param name="abbreviation"></param>
        /// <param name="customStatNames">Dictionary with custom stat names</param>
        /// <returns></returns>
        public static string StatName(int statIndex, bool abbreviation = false, Dictionary<string, string> customStatNames = null, bool secondaryLanguage = false)
        {
            if (_statNames == null || statIndex < 0 || statIndex >= _statNames.Length)
                return string.Empty;

            if (customStatNames != null && customStatNames.TryGetValue(statIndex.ToString(), out string statName))
            {
                return Loc.S(abbreviation ? $"{statName}_Abb" : statName, secondaryCulture: secondaryLanguage);
            }
            if (secondaryLanguage)
                return Loc.S(abbreviation ? StatNameKeys[statIndex] + "_Abb" : StatNameKeys[statIndex], secondaryCulture: true);

            // use cached names
            return abbreviation ? _statNamesAbb[statIndex] : _statNames[statIndex];
        }

        /// <summary>
        /// Returns the displayed decimal values of the stat with the given index
        /// </summary>
        public static int Precision(int statIndex)
        {
            // damage and speed are percentage values, need more precision
            return (statIndex == Stats.SpeedMultiplier || statIndex == Stats.MeleeDamageMultiplier || statIndex == Stats.CraftingSpeedMultiplier) ? 3 : 1;
        }

        /// <summary>
        /// String that represents a duration.
        /// </summary>
        public static string Duration(TimeSpan ts)
        {
            return ts.ToString("dd':'hh':'mm':'ss");
        }

        /// <summary>
        /// String that represents a duration, given in seconds.
        /// </summary>
        public static string Duration(int seconds)
        {
            return Duration(new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// Returns the formatted timespan and also the DateTime when the timespan is over
        /// </summary>
        /// <param name="ts">timespan of countdown</param>
        /// <returns>Returns the timespan and the DateTime when the timespan is over</returns>
        public static string DurationUntil(TimeSpan ts)
        {
            return ts.ToString("d':'hh':'mm':'ss") + " (" + Loc.S("until") + ": " + ShortTimeDate(DateTime.Now.Add(ts)) + ")";
        }

        /// <summary>
        /// Returns a string that represents the given DateTime in a short format.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="omitDateIfToday"></param>
        /// <returns></returns>
        public static string ShortTimeDate(DateTime? dt, bool omitDateIfToday = true)
        {
            if (dt == null) return Loc.S("Unknown");
            return dt.Value.ToShortTimeString() + (omitDateIfToday && DateTime.Today == dt.Value.Date ? string.Empty : " - " + dt.Value.ToShortDateString());
        }

        /// <summary>
        /// String that represents the time left.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string TimeLeft(DateTime dt)
        {
            return dt < DateTime.Now ? "-" : Duration(dt.Subtract(DateTime.Now));
        }

        /// <summary>
        /// By default the cuddle interval is 8 hours.
        /// </summary>
        private const int DefaultCuddleIntervalInSeconds = 8 * 60 * 60;

        /// <summary>
        /// Returns the imprinting gain per cuddle, dependent on the maturation time and the cuddle interval multiplier.
        /// </summary>
        /// <param name="maturationTime">Maturation time in seconds</param>
        /// <returns></returns>
        public static double ImprintingGainPerCuddle(double maturationTime)
        {
            var multipliers = Values.V.currentServerMultipliers;
            return Math.Min(1, DefaultCuddleIntervalInSeconds * multipliers.BabyCuddleIntervalMultiplier * multipliers.BabyImprintAmountMultiplier / maturationTime);
        }

        /// <summary>
        /// Returns either black or white, depending on the backColor, so text can be read well.
        /// </summary>
        /// <param name="backColor"></param>
        /// <returns>ForeColor</returns>
        public static Color ForeColor(Color backColor)
        {
            return backColor.R * .3f + backColor.G * .59f + backColor.B * .11f < 110 ? Color.White : Color.Black;
        }

        public static bool ShowTextInput(string text, out string input, string title = null, string preInput = null, params string[] autoCompleteStrings)
        {
            Form inputForm = new Form
            {
                Width = 250,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false
            };
            Label textLabel = new Label { Left = 20, Top = 15, Text = text, AutoSize = true };
            TextBox textBox = new TextBox { Left = 20, Top = 40, Width = 200 };
            Button buttonOk = new Button { Text = Loc.S("OK"), Left = 120, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            Button buttonCancel = new Button { Text = Loc.S("Cancel"), Left = 20, Width = 80, Top = 70, DialogResult = DialogResult.Cancel };
            buttonOk.Click += (sender, e) => { inputForm.Close(); };
            buttonCancel.Click += (sender, e) => { inputForm.Close(); };
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(buttonOk);
            inputForm.Controls.Add(buttonCancel);
            inputForm.Controls.Add(textLabel);
            inputForm.AcceptButton = buttonOk;
            inputForm.CancelButton = buttonCancel;
            textBox.Text = preInput;
            if (autoCompleteStrings?.Any() == true)
            {
                textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                var ac = new AutoCompleteStringCollection();
                ac.AddRange(autoCompleteStrings);
                textBox.AutoCompleteCustomSource = ac;
            }
            textBox.SelectAll();

            input = string.Empty;
            if (inputForm.ShowDialog() != DialogResult.OK)
                return false;
            input = textBox.Text;
            return true;
        }

        /// <summary>
        /// Displays a control with options, where the user can select one of them or cancel.
        /// The index of the selection is returned or -1 when cancelled.
        /// </summary>
        public static int ShowListInput(IList<string> optionTexts, string headerText = null, string windowTitle = null, int buttonHeight = 21)
        {
            const int width = 350;
            const int margin = 15;
            var result = -1;
            Form inputForm = new Form
            {
                Width = width,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                Text = windowTitle,
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false,
                AutoScroll = true
            };
            var y = 10;
            if (!string.IsNullOrEmpty(headerText))
            {
                Label textLabel = new Label { Left = margin, Top = y, Text = headerText, AutoSize = true };
                inputForm.Controls.Add(textLabel);
                y += 30;
            }

            var tt = new ToolTip();

            var i = 0;
            foreach (var option in optionTexts)
            {
                var optionButton = new Button { Text = option, Left = margin, Width = width - 3 * margin, Top = y, DialogResult = DialogResult.OK, Tag = i++ };
                if (buttonHeight > 0) optionButton.Height = buttonHeight;
                y += buttonHeight + 12;
                optionButton.Click += (sender, e) =>
                {
                    result = (int)((Button)sender).Tag;
                    inputForm.Close();
                };
                inputForm.Controls.Add(optionButton);
                tt.SetToolTip(optionButton, option);
            }

            const int cancelButtonWidth = 80;
            Button buttonCancel = new Button { Text = Loc.S("Cancel"), Left = width - cancelButtonWidth - 2 * margin, Width = cancelButtonWidth, Top = y, DialogResult = DialogResult.Cancel };
            buttonCancel.Click += (sender, e) => { inputForm.Close(); };
            inputForm.Controls.Add(buttonCancel);
            y += 30;
            inputForm.CancelButton = buttonCancel;

            inputForm.Height = Math.Min(y + 50, 800);

            var dialogResult = inputForm.ShowDialog();
            tt.RemoveAll();
            tt.Dispose();

            return dialogResult != DialogResult.OK ? -1 : result;
        }

        /// <summary>
        /// This function may only be used if the ArkId is unique (when importing files that have ArkId1 and ArkId2)
        /// </summary>
        /// <param name="arkId">ArkId built from ArkId1 and ArkId2, user input from the ingame-representation is not allowed</param>
        /// <returns>Guid built from the ArkId</returns>
        public static Guid ConvertArkIdToGuid(long arkId)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(arkId).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        /// <summary>
        /// This function may only be used if the Guid is created by an imported Ark id (i.e. two int32)
        /// </summary>
        public static long ConvertCreatureGuidToArkId(Guid guid)
        {
            return BitConverter.ToInt64(guid.ToByteArray(), 0);
        }

        public static bool IsArkIdImported(long arkId, Guid guid)
        {
            return arkId != 0
                     && guid == ConvertArkIdToGuid(arkId);
        }

        /// <summary>
        /// Returns the Ark-Id as seen ingame from the unique representation used in ASB
        /// </summary>
        /// <param name="importedArkId"></param>
        /// <returns>Ingame visualization of the Ark-Id (not unique in rare cases)</returns>
        public static string ConvertImportedArkIdToIngameVisualization(long importedArkId) => $"{(int)(importedArkId >> 32)}{(int)importedArkId}";

        /// <summary>
        /// Converts the two 32 bit Ark id parts into one 64 bit Ark id.
        /// </summary>
        public static long ConvertArkIdsToLongArkId(int id1, int id2) => ((long)id1 << 32) | (id2 & 0xFFFFFFFFL);

        /// <summary>
        /// Converts int64 Ark id to two int32 ids, like used in the game.
        /// </summary>
        public static (int, int) ConvertArkId64ToArkIds32(long id) => ((int)(id >> 32), (int)id);

        /// <summary>
        /// returns a shortened string with an ellipsis in the middle. One third of the beginning is shown and two thirds of then end
        /// </summary>
        /// <param name="longPath">long string</param>
        /// <param name="maxLength">max length of the string</param>
        /// <returns>string with ellipsis in the middle</returns>
        public static string ShortPath(string longPath, int maxLength = 50)
        {
            if (longPath.Length <= maxLength)
                return longPath;
            int begin = maxLength / 3;
            return longPath.Substring(0, begin) + "…" + longPath.Substring(longPath.Length - maxLength + begin + 1);
        }

        /// <summary>
        /// Determines the default import export folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static bool GetFirstImportExportFolder(out string folder)
        {
            folder = null;

            if (Properties.Settings.Default.ExportCreatureFolders != null
                && Properties.Settings.Default.ExportCreatureFolders.Length > 0)
            {
                var loc = settings.ATImportExportedFolderLocation.CreateFromString(Properties.Settings.Default.ExportCreatureFolders[0]);
                if (System.IO.Directory.Exists(loc.FolderPath))
                {
                    folder = loc.FolderPath;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Changes the color of a control briefly.
        /// </summary>
        public static async void BlinkAsync(Control ctlr, Color c, int duration = 500, bool foreColor = true)
        {
            if (foreColor)
            {
                Color original = ctlr.ForeColor;
                ctlr.ForeColor = c;
                await Task.Delay(duration);
                ctlr.ForeColor = original;
            }
            else
            {
                Color original = ctlr.BackColor;
                ctlr.BackColor = c;
                await Task.Delay(duration);
                ctlr.BackColor = original;
            }
        }

        /// <summary>
        /// Sets the borders of the window according to a Rectangle and tries to make sure it's visible on the available screens.
        /// </summary>
        public static void SetWindowRectangle(Form form, Rectangle windowRect, bool maximized = false)
        {
            if (form == null) return;

            if (double.IsInfinity(windowRect.Top)
                || double.IsInfinity(windowRect.Left)
                || double.IsInfinity(windowRect.Height)
                || double.IsInfinity(windowRect.Width)
                || windowRect.Height < 100
                || windowRect.Width < 100
            )
            {
                windowRect = DefaultFormRectangle;
            }
            else
            {
                // check if rectangle is on screen
                bool isOnScreen = false;
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.Contains(windowRect))
                    {
                        isOnScreen = true;
                        break;
                    }
                }
                if (!isOnScreen)
                {
                    windowRect.X = 100;
                    windowRect.Y = 100;
                }
            }

            form.Top = windowRect.Top;
            form.Left = windowRect.Left;
            form.Width = windowRect.Width;
            form.Height = windowRect.Height;

            if (maximized)
            {
                form.WindowState = FormWindowState.Maximized;
            }
        }

        private static Rectangle DefaultFormRectangle => new Rectangle(100, 100, 800, 600);

        /// <summary>
        /// Returns the window rectangle and state for saving.
        /// </summary>
        public static (Rectangle windowRect, bool maximized) GetWindowRectangle(Form form)
        {
            if (form == null)
                return (DefaultFormRectangle, false);

            switch (form.WindowState)
            {
                case FormWindowState.Maximized:
                    return (form.RestoreBounds, true);
                case FormWindowState.Normal:
                    return (new Rectangle(form.Left, form.Top, form.Width, form.Height), false);
                default:
                    return (form.RestoreBounds, false);
            }
        }

        private static string _applicationNameVersion;
        /// <summary>
        /// The name and version of this application.
        /// </summary>
        public static string ApplicationNameVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationNameVersion))
                    _applicationNameVersion = $"{Application.ProductName} v{Application.ProductVersion}";
                return _applicationNameVersion;
            }
        }
    }
}

using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    /// <summary>
    /// static class for localizations
    /// </summary>
    internal static class Loc
    {
        private static ResourceManager _rm;
        /// <summary>
        /// Second language can be used in parts of the app.
        /// </summary>
        private static CultureInfo _secondaryCulture;

        public static void LoadResourceFile(string language, string language2 = null)
        {
            CultureInfo LoadCultureInfo(string l)
            {
                if (string.IsNullOrEmpty(l))
                {
                    return null;
                }
                try
                {
                    return new CultureInfo(l);
                }
                catch (CultureNotFoundException)
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(language2) && language2 != language)
                _secondaryCulture = LoadCultureInfo(language2);
            else _secondaryCulture = null;

            var culture = LoadCultureInfo(language) ?? CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = culture;
            if (_rm == null)
                _rm = new ResourceManager("ARKBreedingStats.local.strings", typeof(Form1).Assembly);
        }

        public static bool UseSecondaryCulture => _secondaryCulture != null;

        /// <summary>
        /// Returns the localized string.
        /// </summary>
        public static string S(string key, bool returnKeyIfValueNa = true, bool secondaryCulture = false)
        {
            if (_rm == null) return null;
            if (secondaryCulture && _secondaryCulture != null)
                return S(key, _secondaryCulture, returnKeyIfValueNa);

            var s = _rm.GetString(key);
            //if (string.IsNullOrEmpty(s) && !key.EndsWith("TT")) System.Console.WriteLine("missing: " + key); // for debugging
            return s ?? (returnKeyIfValueNa ? key : null);
        }

        /// <summary>
        /// Returns the localized string of the given culture.
        /// </summary>
        public static string S(string key, CultureInfo culture, bool returnKeyIfValueNa = true)
        {
            if (_rm == null) return null;
            string s = _rm.GetString(key, culture);
            //if (string.IsNullOrEmpty(s) && !key.EndsWith("TT")) System.Console.WriteLine("missing: " + key); // for debugging
            return s ?? (returnKeyIfValueNa ? key : null);
        }

        public static void ControlText(Control c) => c.Text = S(c.Name);
        public static void ControlText(Control c, string key) => c.Text = S(key);

        /// <summary>
        /// Sets the Text property of the control to the localized string, using the control.Name as key.
        /// If there is a key with an appended TT, a tooltip is set.
        /// </summary>
        public static void ControlText(Control c, ToolTip tt)
        {
            c.Text = S(c.Name);
            tt.SetToolTip(c, S(c.Name + "TT", false));
        }

        /// <summary>
        /// Sets the tooltip of the control using the localization key of the control name with TT appended.
        /// </summary>
        public static void SetToolTip(Control c, ToolTip tt)
        {
            tt.SetToolTip(c, S(c.Name + "TT", false));
        }

        /// <summary>
        /// Sets the tooltip of the control using the custom key.
        /// </summary>
        public static void SetToolTip(Control c, string key, ToolTip tt)
        {
            tt.SetToolTip(c, S(key));
        }

        /// <summary>
        /// Sets the Text of the control according to the key. Sets the tooltip according to the controlName
        /// </summary>
        /// <param name="c"></param>
        /// <param name="key"></param>
        /// <param name="tt"></param>
        public static void ControlText(Control c, string key, ToolTip tt)
        {
            c.Text = S(key);
            tt.SetToolTip(c, S(c.Name + "TT", false));
        }

        public static void ControlText(ToolStripItem i)
        {
            i.Text = S(i.Name);
            string tt = S(i.Name + "TT", false);
            if (!string.IsNullOrEmpty(tt))
                i.ToolTipText = tt;
        }
        public static void ControlText(ToolStripItem i, string key)
        {
            i.Text = S(key);
            string tt = S(key + "TT", false);
            if (!string.IsNullOrEmpty(tt))
                i.ToolTipText = tt;
        }
    }
}

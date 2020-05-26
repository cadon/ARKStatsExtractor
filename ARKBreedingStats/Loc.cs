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
        private static ResourceManager rm;

        public static void LoadResourceFile()
        {
            Thread.CurrentThread.CurrentUICulture = !string.IsNullOrEmpty(Properties.Settings.Default.language) ?
                    new System.Globalization.CultureInfo(Properties.Settings.Default.language) :
                    System.Globalization.CultureInfo.CurrentCulture;

            rm = new ResourceManager("ARKBreedingStats.local.strings", typeof(Form1).Assembly);
        }

        /// <summary>
        /// Returns the localized string.
        /// </summary>
        public static string S(string key, bool returnKeyIfValueNA = true)
        {
            if (rm == null) return null;
            string s = rm.GetString(key);
            //if (string.IsNullOrEmpty(s) && !key.EndsWith("TT")) System.Console.WriteLine("missing: " + key); // TODO debug
            return s ?? (returnKeyIfValueNA ? key : null);
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
        public static void ControlText(ToolStripMenuItem i)
        {
            i.Text = S(i.Name);
            string tt = S(i.Name + "TT", false);
            if (!string.IsNullOrEmpty(tt))
                i.ToolTipText = tt;
        }

        public static void ControlText(ToolStripButton i)
        {
            i.Text = S(i.Name);
            string tt = S(i.Name + "TT", false);
            if (!string.IsNullOrEmpty(tt))
                i.ToolTipText = tt;
        }
    }
}

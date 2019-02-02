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

        public static string s(string key)
        {
            if (rm == null) return string.Empty;
            string s = rm.GetString(key);
            //if (string.IsNullOrEmpty(s)) System.Console.WriteLine("missing: " + key);
            //return string.IsNullOrEmpty(s) ? "MISSING" : s;
            return s ?? key ?? string.Empty;
        }

        public static void ControlText(Control c) => c.Text = s(c.Name);
        public static void ControlText(Control c, string key) => c.Text = s(key);
        public static void ControlText(Control c, ToolTip tt)
        {
            c.Text = s(c.Name);
            tt.SetToolTip(c, s(c.Name + "TT"));
        }
        public static void setToolTip(Control c, ToolTip tt)
        {
            tt.SetToolTip(c, s(c.Name + "TT"));
        }
        public static void setToolTip(Control c, string key, ToolTip tt)
        {
            tt.SetToolTip(c, s(key));
        }
        /// <summary>
        /// sets the Text of the control according to the key. Sets the tooltip according to the controlName
        /// </summary>
        /// <param name="c"></param>
        /// <param name="key"></param>
        /// <param name="tt"></param>
        public static void ControlText(Control c, string key, ToolTip tt)
        {
            c.Text = s(key);
            tt.SetToolTip(c, s(c.Name + "TT"));
        }
        public static void ControlText(ToolStripMenuItem i)
        {
            i.Text = s(i.Name);
            string tt = s(i.Name + "TT");
            if (!string.IsNullOrEmpty(tt))
                i.ToolTipText = tt;
        }

        public static void ControlText(ToolStripButton i)
        {
            i.Text = s(i.Name);
            string tt = s(i.Name + "TT");
            if (!string.IsNullOrEmpty(tt))
                i.ToolTipText = tt;
        }
    }
}

using System;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledExcHandler;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void UnhandledExcHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("Unhandled Exception:\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

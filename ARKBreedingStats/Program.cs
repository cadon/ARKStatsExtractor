using ARKBreedingStats.uiControls;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
#if !DEBUG
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledExceptionHandler;
#endif

            if (CloseIfDifferentInstanceOfAppIsRunning())
                return;

            var args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "cleanupupdater":
                        FileService.TryDeleteFile(Path.Combine(Path.GetTempPath(), Updater.Updater.UpdaterExe));
                        break;
                    case "-setlanguage":
                        var language = args.Length > ++i ? args[i] : null;
                        Properties.Settings.Default.language = language;
                        break;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1
            {
                Font = new Font(Properties.Settings.Default.DefaultFontName, Properties.Settings.Default.DefaultFontSize)
            });
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            if (e is ConfigurationErrorsException ex)
            {
                if (ex.InnerException is ConfigurationErrorsException configEx)
                {
                    if (MessageBox.Show("Error while accessing the configuration file.\n\n" +
                            $"Message:\n{e.Message}\n" +
                            $"{configEx.Message}\n" +
                            $"File: {configEx.Filename}\n\n" +
                            "Ark Smart Breeding cannot continue without valid settings and will stop now.\n" +
                            "You can try to rename the settings file to reset it, this may solve the issue.\n\n" +
                            "Show the settings file in the explorer?",
                            $"Error reading configuration file - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                        == DialogResult.Yes)
                        FileService.OpenFolderInExplorer(configEx.Filename);
                }
                else
                {
                    string settingsFilePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                    if (MessageBox.Show("Error while accessing the configuration file.\n\n" +
                                        $"Message:\n{e.Message}\n\n" +
                                        "Ark Smart Breeding cannot continue without valid settings and will stop now.\n" +
                                        "You can try to rename the settings file to reset it, this may solve the issue.\n" +
                                        settingsFilePath + "\n\n" +
                                        "Show the settings file in the explorer?",
                            $"Error reading configuration file - {Utils.ApplicationNameVersion}", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                        == DialogResult.Yes)
                        FileService.OpenFolderInExplorer(settingsFilePath);
                }
                Environment.Exit(78);
            }
            else
            {
                if (System.Diagnostics.Debugger.IsAttached) throw e;
                string message = e.Message
                    + "\n\n" + e.GetType() + " in " + e.Source + " (" + Utils.ApplicationNameVersion + ")"
                    + "\n\nMethod throwing the error: " + e.TargetSite.DeclaringType?.FullName + "." + e.TargetSite.Name
                    + "\n\nStackTrace:\n" + e.StackTrace
                    + (e.InnerException != null ? "\n\nInner Exception:\n" + e.InnerException.Message : string.Empty)
                    ;

                CustomMessageBox.Show(message, "Unhandled Exception", "OK", icon: MessageBoxIcon.Error,
                    showCopyToClipboard: true);
            }
        }

        private static bool CloseIfDifferentInstanceOfAppIsRunning()
        {
            // Wine might crash when accessing Process
            try
            {
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length < 2) return false;
                return MessageBox.Show($@"{Application.ProductName} seems to be running already.
Starting a second instance of this app could cause issues with synchronization, automatic importing and app settings.

Start another instance?", $"{Application.ProductName} seems to be running already", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) !=
                       DialogResult.Yes;
            }
            catch
            {
                return false;
            }
        }
    }
}

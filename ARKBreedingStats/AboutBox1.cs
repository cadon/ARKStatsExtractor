using System;
using System.Reflection;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            Text = $"Info about {AssemblyTitle}";
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = $"Version {AssemblyVersion}";
            labelCopyright.Text = AssemblyCopyright;
            labelDescription.Text = AssemblyDescription;
            textBoxContributors.Text = Contributors;
        }

        #region Assemblyattributaccessoren

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion => Application.ProductVersion;

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKStatsExtractor");
        }

        private string Contributors => @"Thanks for contributions, help and support to

* NakramR (coding, OCR, library, overlay)
* Flachdachs (savefile extractor, installer-version, style, fixes)
* coldino (ARK-data, support)
* VolatilesPulse (ARK-data, support)
* qowyn (savefile extractor, ARK-data)
* aaron-williamson (file-syncing for cloud-services)
* DelilahEve (updater)
* DodoCooker (fixes, performance)
* Warstone (Kibble recipes)
* tsebring (naming-generator, fixes)
* maxime-paquatte (custom timer sounds)
* hallipr (FTP savefile import)
* EmkioA (Cryopod import, listView tweaks)
* dunger (fixes)
* Myrmecoleon (extra species images)

Translations:
* French by Vykan and Yanuut
* Italian by Zaffira and Spit-Biago
* German by cadon
* Spanish by KRIPT4
* Chinese by MicheaBoab";
    }
}

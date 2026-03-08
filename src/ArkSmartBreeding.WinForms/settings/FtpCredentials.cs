using ARKBreedingStats.miscClasses;
using System.Windows.Forms;

namespace ARKBreedingStats.settings
{
    public partial class FtpCredentialsForm : Form
    {
        public FtpCredentialsForm()
        {
            InitializeComponent();
        }

        public FtpCredentials Credentials => new FtpCredentials
        {
            Username = textBox_Username.Text,
            Password = textBox_Password.Text
        };

        public bool SaveCredentials => checkBox_SaveCredentials.Checked;
    }
}

using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class ArkGameDialog : Form
    {
        public Ark.Game GameVersion;
        public bool UseSelectionAsDefault => CbRememberSelection?.Checked != false;

        public ArkGameDialog()
        {
            InitializeComponent();
            Text = Utils.ApplicationNameVersion;
        }

        public ArkGameDialog(Form owner) : this()
        {
            Owner = owner;
        }

        private void Close(Ark.Game arkGame)
        {
            GameVersion = arkGame;
            Close();
        }

        private void BtAsa_Click(object sender, EventArgs e) => Close(Ark.Game.Asa);

        private void BtAse_Click(object sender, EventArgs e) => Close(Ark.Game.Ase);
    }
}

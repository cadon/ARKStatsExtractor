using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    internal class ArkVersionDialog : Form
    {
        public Ark.Game GameVersion;
        public bool UseSelectionAsDefault => _cbUseAsDefault?.Checked != false;
        private CheckBox _cbUseAsDefault;

        public ArkVersionDialog()
        {
            StartPosition = FormStartPosition.CenterParent;
            Text = Utils.ApplicationNameVersion;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;

            const int margin = 20;
            const int buttonWidth = 160;
            const int buttonHeight = 50;

            Width = 3 * margin + 2 * buttonWidth + 15;
            Height = 5 * margin + buttonHeight + 30;

            var lb = new Label
            {
                Text = "Game version of new library",
                Width = Width,
                TextAlign = ContentAlignment.TopCenter,
                Top = margin
            };

            var btAse = new Button
            {
                Text = "ARK: Survival Evolved",
                Width = buttonWidth,
                Height = buttonHeight,
                Top = 2 * margin,
                Left = margin
            };
            var btAsa = new Button
            {
                Text = "ARK: Survival Ascended",
                Width = buttonWidth,
                Height = buttonHeight,
                Top = 2 * margin,
                Left = 2 * margin + buttonWidth
            };
            btAse.Click += (s, e) => Close(Ark.Game.Ase);
            btAsa.Click += (s, e) => Close(Ark.Game.Asa);

            _cbUseAsDefault = new CheckBox
            {
                Text = "Remember selection (can be changed in the settings)",
                AutoSize = true,
                Left = margin,
                Top = 3 * margin + buttonHeight
            };

            Controls.AddRange(new Control[] { btAse, btAsa, _cbUseAsDefault, lb });
        }

        public ArkVersionDialog(Form owner) : this()
        {
            Owner = owner;
        }

        private void Close(Ark.Game arkGame)
        {
            GameVersion = arkGame;
            Close();
        }
    }
}

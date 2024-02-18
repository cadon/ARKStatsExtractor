using System;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class ColorPickerWindow : Form
    {
        public bool isShown;
        public ColorPickerControl Cp;

        public ColorPickerWindow()
        {
            InitializeComponent();
            Cp = ColorPickerControl1;
            Cp.Window = this;
            Cp.CancelButtonVisible(true);
            Load += ColorPickerWindow_Load;
            Cp.UserMadeSelection += ColorPickerHideWindow;
            Cp.HeightChanged += SetWindowHeight;
            Cp.MouseLeave += ColorPickerWindow_MouseLeave;

            TopMost = true;
        }

        private void SetWindowHeight(int height) => Height = height;

        private void ColorPickerWindow_Load(object sender, EventArgs e)
        {
            int y = Cursor.Position.Y - Height;
            if (y < 20) y = 20;
            SetDesktopLocation(Cursor.Position.X - 20, y);
        }

        private void ColorPickerWindow_MouseLeave(object sender, EventArgs e)
        {
            // mouse left, close
            if (!ClientRectangle.Contains(PointToClient(MousePosition)) || PointToClient(MousePosition).X == 0 || PointToClient(MousePosition).Y == 0)
            {
                ColorPickerHideWindow(false);
            }
        }

        private void ColorPickerHideWindow(bool colorChosen)
        {
            isShown = false;
            DialogResult = colorChosen ? DialogResult.OK : DialogResult.Cancel;
        }
    }
}

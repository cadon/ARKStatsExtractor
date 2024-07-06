using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    /// <summary>
    /// Invisible form for scrolling while holding the left mouse button.
    /// </summary>
    public partial class ScrollForm : Form
    {
        /// <summary>
        /// Mouse was moved to coordinates.
        /// </summary>
        public event Action<int, int> Moved;

        private Point _centerOffset;

        public ScrollForm()
        {
            InitializeComponent();
            Capture = true;
        }

        public void SetLocation(Point p)
        {
            Location = p;
            _centerOffset = PointToScreen(new Point(Width / 2, Height / 2));
            _centerOffset = new Point(-_centerOffset.X, -_centerOffset.Y);
        }

        private void ScrollForm_MouseUp(object sender, MouseEventArgs e) => Close();

        private void ScrollForm_MouseLeave(object sender, EventArgs e) => Close();

        private void ScrollForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (Moved == null) return;
            var p = Cursor.Position;
            p.Offset(_centerOffset);
            Moved(p.X, p.Y);
        }
    }
}

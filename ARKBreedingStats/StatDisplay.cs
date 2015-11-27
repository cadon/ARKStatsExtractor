using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class StatDisplay : UserControl
    {
        private int level;
        private double statValue;
        private bool percent = false;
        public StatDisplay()
        {
            InitializeComponent();
        }

        public string Title
        {
            set { this.labelName.Text = value; }
        }

        public int Level
        {
            set
            {
                level = value;
                this.panelBar.Width = value;
                int r = 511 - value * 255 / 33;
                int g = value * 255 / 33;
                if (r < 0) { r = 0; }
                if (g < 0) { g = 0; }
                if (r > 255) { r = 255; }
                if (g > 255) { g = 255; }
                this.panelBar.BackColor = Color.FromArgb(r, g, 0);
                updateLabel();
            }
        }

        public double StatValue
        {
            set
            {
                statValue = value;
                updateLabel();
            }
        }

        public bool Percent { set { percent = value; } }

        private void updateLabel()
        {
            this.label1.Text = "Lvl " + level + " (" + (percent ? Math.Round(100 * statValue, 1).ToString() + " %" : statValue.ToString()) + ")";
        }
    }
}

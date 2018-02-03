using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats.raising
{
    public partial class ParentStatValues : UserControl
    {
        public ParentStatValues()
        {
            InitializeComponent();
            labelM.TextAlign = ContentAlignment.MiddleRight;
            labelF.TextAlign = ContentAlignment.MiddleRight;
        }

        public string StatName { set { label1.Text = value; } }

        internal void setValues(string sMother, string sFather, int highlight)
        {
            labelM.Text = sMother;
            labelF.Text = sFather;
            switch (highlight)
            {
                case 1:
                    labelM.ForeColor = Color.DarkBlue;
                    labelF.ForeColor = Color.DarkRed;
                    break;
                case 2:
                    labelF.ForeColor = Color.DarkBlue;
                    labelM.ForeColor = Color.DarkRed;
                    break;
                default:
                    labelM.ForeColor = Color.Black;
                    labelF.ForeColor = Color.Black;
                    break;
            }
        }
    }
}

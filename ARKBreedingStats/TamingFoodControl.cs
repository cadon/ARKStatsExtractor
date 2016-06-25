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
    public partial class TamingFoodControl : UserControl
    {
        public delegate void ValueChangedEventHandler();
        public event ValueChangedEventHandler valueChanged;

        public TamingFoodControl()
        {
            InitializeComponent();
        }

        public TamingFoodControl(string name)
        {
            InitializeComponent();
            this.foodName = name;
        }

        public int amount
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }

        public string foodName
        {
            set { groupBox1.Text = value; }
            get { return groupBox1.Text; }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (valueChanged != null)
                valueChanged();
        }
    }
}

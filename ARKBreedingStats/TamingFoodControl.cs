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
        public delegate void ClickedEventHandler(string food);
        public event ClickedEventHandler Clicked;
        public string foodName;
        private int maxFoodAmount;

        public TamingFoodControl()
        {
            InitializeComponent();
        }

        public TamingFoodControl(string name)
        {
            InitializeComponent();
            this.foodName = name;
            foodNameDisplay = name;
        }

        public int amount
        {
            get { return (int)numericUpDown1.Value; }
            set { if (value >= 0) numericUpDown1.Value = value; }
        }

        public string foodNameDisplay
        {
            set { groupBox1.Text = value; }
            get { return groupBox1.Text; }
        }

        public int maxFood
        {
            set
            {
                maxFoodAmount = value;
                labelMax.Text = maxFoodAmount.ToString();
            }
            get { return maxFoodAmount; }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (valueChanged != null)
                valueChanged();
        }

        public int foodUsed { set { labelFoodUsed.Text = value.ToString(); } }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Clicked != null)
                Clicked(foodName);
        }
    }
}

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
        private string _foodName;
        private int maxFoodAmount;

        public TamingFoodControl()
        {
            InitializeComponent();
        }

        public TamingFoodControl(string name)
        {
            InitializeComponent();
            FoodName = name;
        }

        public string FoodName
        {
            set
            {
                _foodName = value;
                foodNameDisplay = _foodName;
            }
            get
            {
                return _foodName;
            }
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
                button1.Text = maxFoodAmount.ToString();
            }
            get { return maxFoodAmount; }
        }

        public TimeSpan tamingDuration
        {
            set { if (value != null) labelDuration.Text = value.ToString("d':'hh':'mm':'ss"); }
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
                Clicked(FoodName);
        }
    }
}

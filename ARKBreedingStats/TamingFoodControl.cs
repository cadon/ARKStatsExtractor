using System;
using System.Drawing;
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
            get => _foodName;
            set
            {
                _foodName = value;
                foodNameDisplay = _foodName;

                // display specific colors of kibbles
                switch (_foodName)
                {
                    case "Basic Kibble": PanelColorIndicator.BackColor = Color.FromArgb(215, 215, 215); break;
                    case "Simple Kibble": PanelColorIndicator.BackColor = Color.FromArgb(0, 255, 0); break;
                    case "Regular Kibble": PanelColorIndicator.BackColor = Color.FromArgb(0, 0, 255); break;
                    case "Superior Kibble": PanelColorIndicator.BackColor = Color.FromArgb(225, 50, 200); break;
                    case "Exceptional Kibble": PanelColorIndicator.BackColor = Color.FromArgb(255, 255, 0); break;
                    case "Extraordinary Kibble": PanelColorIndicator.BackColor = Color.FromArgb(0, 255, 255); break;
                    default: PanelColorIndicator.BackColor = Color.Transparent; break;
                }
            }
        }

        public int amount
        {
            get => (int)numericUpDown1.Value;
            set
            {
                if (value >= 0) numericUpDown1.Value = value;
            }
        }

        public string foodNameDisplay
        {
            get => groupBox1.Text;
            set => groupBox1.Text = value;
        }

        public int maxFood
        {
            get => maxFoodAmount;
            set
            {
                maxFoodAmount = value;
                button1.Text = maxFoodAmount.ToString();
            }
        }

        public TimeSpan tamingDuration
        {
            set
            {
                labelDuration.Text = value.ToString("d':'hh':'mm':'ss");
                TamingSeconds = (int)value.TotalSeconds;
            }
        }

        public int TamingSeconds;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            valueChanged?.Invoke();
        }

        public int foodUsed
        {
            set => labelFoodUsed.Text = value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clicked?.Invoke(FoodName);
        }
    }
}

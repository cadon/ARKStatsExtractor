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
                FoodNameDisplay = _foodName;

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

        /// <summary>
        /// Food amount currently set.
        /// </summary>
        public int Amount
        {
            get => (int)numericUpDown1.Value;
            set => numericUpDown1.Value = Math.Max(0, value);
        }

        public string FoodNameDisplay
        {
            get => groupBox1.Text;
            set => groupBox1.Text = value;
        }

        public int MaxFood
        {
            get => maxFoodAmount;
            set
            {
                maxFoodAmount = value;
                button1.Text = maxFoodAmount.ToString();
            }
        }

        public TimeSpan TamingDuration
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

        /// <summary>
        /// Amount of food used during taming.
        /// </summary>
        public int FoodUsed
        {
            set => labelFoodUsed.Text = value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clicked?.Invoke(FoodName);
        }
    }
}

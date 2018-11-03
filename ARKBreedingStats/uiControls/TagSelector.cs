using System;
using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class TagSelector : UserControl
    {
        private tagStatus status;
        private ToolTip tt;

        public delegate void TagStatusChangedEventHandler();

        public event TagStatusChangedEventHandler OnTagChanged;

        public TagSelector()
        {
            InitializeComponent();
            status = tagStatus.indifferent;
            button1.Text = "○";
            button1.BackColor = SystemColors.Control;
            tt = new ToolTip();
            Disposed += TagSelector_Disposed;
        }

        private void TagSelector_Disposed(object sender, EventArgs e)
        {
            tt.RemoveAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (status == tagStatus.indifferent)
                setStatus(tagStatus.include);
            else if (status == tagStatus.include)
                setStatus(tagStatus.exclude);
            else
                setStatus(tagStatus.indifferent);
            OnTagChanged?.Invoke();
        }

        private void setStatus(tagStatus s)
        {
            if (s == tagStatus.include)
            {
                status = tagStatus.include;
                button1.Text = "✓";
                button1.BackColor = Color.LightGreen;
            }
            else if (s == tagStatus.exclude)
            {
                status = tagStatus.exclude;
                button1.Text = "✕";
                button1.BackColor = Color.LightSalmon;
            }
            else
            {
                status = tagStatus.indifferent;
                button1.Text = "○";
                button1.BackColor = SystemColors.Control;
            }
        }

        public tagStatus Status
        {
            get => status;
            set => setStatus(value);
        }

        public string TagName
        {
            get => label1.Text;
            set
            {
                label1.Text = value;
                tt.SetToolTip(label1, value);
            }
        }

        public enum tagStatus
        {
            indifferent,
            include,
            exclude
        }
    }
}

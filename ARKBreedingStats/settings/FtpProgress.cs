using ARKBreedingStats.miscClasses;
using FluentFTP;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ARKBreedingStats.settings
{
    public partial class FtpProgressForm : Form, IProgress<FtpProgress>
    {
        public FtpProgressForm(CancellationTokenSource cancellationTokenSource)
        {
            InitializeComponent();
            FormClosing += (sender, args) => cancellationTokenSource.Cancel();
        }

        public string StatusText
        {
            get
            {
                return StatusLabel.Text;
            }
            set
            {
                StatusLabel.Text = value;
            }
        }

        public string FileName { get; set; }
        private Stopwatch stopwatch = new Stopwatch();

        public void Report(FtpProgress value)
        {
            if (value.Progress < 100 && stopwatch.IsRunning && stopwatch.ElapsedMilliseconds < 250)
            {
                // only update the progress every 250ms unless setting it to 100%
                return;
            }

            var statusText = $"Downloading {FileName}\r\n{value.Progress:F0}% complete\r\n{value.TransferSpeedToString()}";
            StatusLabel.Invoke(new Action(() => StatusLabel.Text = statusText));
            stopwatch.Restart();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

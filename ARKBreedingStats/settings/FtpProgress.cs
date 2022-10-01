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
            get => StatusLabel.Text;
            set => StatusLabel.Text = value;
        }

        public string FileName { get; set; }
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void Report(FtpProgress value)
        {
            if (value.Progress < 100 && _stopwatch.IsRunning && _stopwatch.ElapsedMilliseconds < 250)
            {
                // only update the progress every 250ms unless setting it to 100%
                return;
            }

            var statusText = $"Downloading {FileName}\r\n{value.Progress:F0}% complete\r\n{value.TransferSpeedToString()}";
            StatusLabel.Invoke(new Action(() => StatusLabel.Text = statusText));
            _stopwatch.Restart();
        }

        private void button_Cancel_Click(object sender, EventArgs e) => Close();
    }
}

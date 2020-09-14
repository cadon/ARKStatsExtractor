using ARKBreedingStats.ocr;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARKBreedingStats.Library;

namespace ARKBreedingStats
{
    public partial class ARKOverlay : Form
    {
        private readonly Label[] _labels;
        private readonly Timer _timerUpdateTimer;
        public Form1 ExtractorForm;
        private bool _ocrPossible;
        private bool _OCRing;
        public List<TimerListEntry> timers;
        private string _notes;
        public static ARKOverlay theOverlay;
        private DateTime _infoShownAt;
        public int InfoDuration;
        private bool _currentlyInInventory;
        public bool checkInventoryStats;
        private bool _toggleInventoryCheck; // check inventory only every other time

        public ARKOverlay()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            parentInheritance1.ForeColor = Color.FromArgb(1, 1, 1); // so it's not transparent (black == TransparencyKey)

            _infoShownAt = DateTime.Now.AddMinutes(-10);
            _labels = new[] { lblHealth, lblStamina, lblOxygen, lblFood, lblWeight, lblMeleeDamage, lblMovementSpeed, lblLevel };

            foreach (Label l in _labels)
                l.Text = string.Empty;
            lblStatus.Text = string.Empty;
            labelTimer.Text = string.Empty;
            labelInfo.Text = string.Empty;

            Size = ArkOCR.OCR.GetScreenshotOfProcess()?.Size ?? default;
            if (Size == default)
                Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            _timerUpdateTimer = new Timer { Interval = 1000 };
            _timerUpdateTimer.Tick += TimerUpdateTimer_Tick;
            theOverlay = this;
            _currentlyInInventory = false;

            _ocrPossible = ArkOCR.OCR.ocrConfig != null && ArkOCR.OCR.CheckResolutionSupportedByOcr();

            SetInfoPositions();
            _notes = string.Empty;
            SetInheritanceCreatures();
            SetLocatlizations();

            InfoDuration = 10;

            Location = new Point(0, 0);
        }

        public void SetInfoPositions()
        {
            labelTimer.Location = Properties.Settings.Default.OverlayTimerPosition;
            labelInfo.Location = new Point(Size.Width - labelInfo.Width - Properties.Settings.Default.OverlayInfoPosition.X, Properties.Settings.Default.OverlayInfoPosition.Y);
        }

        public void InitLabelPositions()
        {
            if (!_ocrPossible) return;

            for (int statIndex = 0; statIndex < _labels.Length; statIndex++)
            {
                Rectangle r = ArkOCR.OCR.ocrConfig.labelRectangles[statIndex];
                _labels[statIndex].Location = new Point(r.Left + r.Width + 6, r.Top - 10); //this.PointToClient(new Point(r.Left + r.Width + 6, r.Top - 10));
            }
            lblStatus.Location = new Point(50, 10);
        }

        /// <summary>
        /// Sets the overlay timer to enabled or disabled.
        /// </summary>
        public bool EnableOverlayTimer
        {
            set => _timerUpdateTimer.Enabled = value;
        }

        private void TimerUpdateTimer_Tick(object sender, EventArgs e)
        {
            SetTimerAndNotesText();

            // info
            if (labelInfo.Text != "" && _infoShownAt.AddSeconds(InfoDuration) < DateTime.Now)
            {
                labelInfo.Text = string.Empty;
                parentInheritance1.Visible = false;
            }

            if (!_ocrPossible) return;

            _toggleInventoryCheck = !_toggleInventoryCheck;
            if (checkInventoryStats && _toggleInventoryCheck)
            {
                if (_OCRing)
                    return;
                lblStatus.Text = "…";
                Application.DoEvents();
                _OCRing = true;
                if (!ArkOCR.OCR.isDinoInventoryVisible())
                {
                    if (_currentlyInInventory)
                    {
                        for (int i = 0; i < _labels.Length; i++)
                            if (_labels[i] != null)
                                _labels[i].Text = string.Empty;
                        _currentlyInInventory = false;
                    }
                }
                else if (_currentlyInInventory)
                {
                    // assuming it's still the same inventory, don't do anything, assuming nothing changed
                }
                else
                {
                    _currentlyInInventory = true;
                    lblStatus.Text = "Reading Values";
                    Application.DoEvents();
                    ExtractorForm?.DoOcr("", false);
                }
                _OCRing = false;
                lblStatus.Text = string.Empty;
                Application.DoEvents();
            }
        }

        public void SetStatLevels(int[] wildValues, int[] tamedValues, int levelWild, int levelDom, Color[] colors = null)
        {
            // only 7 stats are displayed
            var displayIndices = new int[] { (int)StatNames.Health, (int)StatNames.Stamina, (int)StatNames.Oxygen, (int)StatNames.Food, (int)StatNames.Weight, (int)StatNames.MeleeDamageMultiplier, (int)StatNames.SpeedMultiplier };
            for (int s = 0; s < 7; s++)
            {
                int di = displayIndices[s];
                _labels[s].Text = "[w" + wildValues[di];
                if (tamedValues[di] != 0)
                    _labels[s].Text += "+d" + tamedValues[di];
                _labels[s].Text += "]";
                if (colors != null && di < colors.Length)
                    _labels[s].ForeColor = colors[di];
            }

            // total level
            _labels[7].Text = "[w" + levelWild;
            if (levelDom != 0)
                _labels[7].Text += "+d" + levelDom;
            _labels[7].Text += "]";
        }

        /// <summary>
        /// Used to display longer texts at the top right, e.g. taming-info.
        /// </summary>
        /// <param name="infoText"></param>
        internal void SetInfoText(string infoText, Color textColor)
        {
            labelInfo.ForeColor = textColor;
            labelInfo.Visible = true;
            labelInfo.Text = infoText;
            _infoShownAt = DateTime.Now;
        }

        internal void SetInfoText(string infoText) => SetInfoText(infoText, Color.White);

        /// <summary>
        /// Update the text for the timers and notes.
        /// </summary>
        private void SetTimerAndNotesText()
        {
            StringBuilder sb = new StringBuilder();

            if (timers?.Any() ?? false)
            {
                bool timerListChanged = false;
                foreach (TimerListEntry tle in timers)
                {
                    int secLeft = (int)tle.time.Subtract(DateTime.Now).TotalSeconds + 1;
                    if (secLeft < 10)
                    {
                        if (!Properties.Settings.Default.KeepExpiredTimersInOverlay && secLeft < -20)
                        {
                            tle.showInOverlay = false;
                            timerListChanged = true;
                            continue;
                        }
                        sb.Append("expired ");
                    }
                    sb.AppendLine($"{Utils.TimeLeft(tle.time)} : {tle.name}");
                }
                if (timerListChanged)
                    timers = timers.Where(t => t.showInOverlay).ToList();
            }
            sb.Append(_notes);
            labelTimer.Text = sb.ToString();
        }

        internal void SetNotes(string notes)
        {
            _notes = notes;
            SetTimerAndNotesText();
        }

        internal void SetInheritanceCreatures(Creature creature = null, Creature mother = null, Creature father = null) => parentInheritance1.SetCreatures(creature, mother, father);

        internal void SetLocatlizations()
        {
            parentInheritance1.SetLocalizations();
        }
    }
}

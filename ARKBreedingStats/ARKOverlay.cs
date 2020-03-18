using ARKBreedingStats.ocr;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class ARKOverlay : Form
    {
        private const int LABEL_COUNT = 8;
        private readonly Control[] labels = new Control[LABEL_COUNT];
        private readonly Timer timerUpdateTimer;
        public Form1 ExtractorForm;
        private bool ocrPossible;
        public bool OCRing;
        public readonly List<TimerListEntry> timers = new List<TimerListEntry>();
        private string notes;
        public static ARKOverlay theOverlay;
        private DateTime infoShownAt;
        public int InfoDuration;
        private bool currentlyInInventory;
        public bool checkInventoryStats;
        private bool toggleInventoryCheck; // check inventory only every other time

        public ARKOverlay()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;

            infoShownAt = DateTime.Now.AddMinutes(-10);

            labels[0] = lblHealth;
            labels[1] = lblStamina;
            labels[2] = lblOxygen;
            labels[3] = lblFood;
            labels[4] = lblWeight;
            labels[5] = lblMeleeDamage;
            labels[6] = lblMovementSpeed;
            labels[7] = lblLevel;

            foreach (Label l in labels)
                l.Text = string.Empty;
            lblStatus.Text = string.Empty;
            labelTimer.Text = string.Empty;
            labelInfo.Text = string.Empty;

            Size = ArkOCR.OCR.GetScreenshotOfProcess()?.Size ?? default;
            if (Size == default)
                Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            timerUpdateTimer = new Timer { Interval = 1000 };
            timerUpdateTimer.Tick += TimerUpdateTimer_Tick;
            theOverlay = this;
            currentlyInInventory = false;

            ocrPossible = ArkOCR.OCR.setResolution();

            SetInfoPositions();
            notes = string.Empty;

            InfoDuration = 10;
        }

        public void SetInfoPositions()
        {
            labelTimer.Location = Properties.Settings.Default.OverlayTimerPosition;
            labelInfo.Location = new Point(Size.Width - labelInfo.Width - Properties.Settings.Default.OverlayInfoPosition.X, Properties.Settings.Default.OverlayInfoPosition.Y);
        }

        public void InitLabelPositions()
        {
            if (!ocrPossible) return;

            for (int statIndex = 0; statIndex < LABEL_COUNT; statIndex++)
            {
                Rectangle r = ArkOCR.OCR.ocrConfig.labelRectangles[statIndex];
                labels[statIndex].Location = new Point(r.Left + r.Width + 6, r.Top - 10); //this.PointToClient(new Point(r.Left + r.Width + 6, r.Top - 10));
            }
            lblStatus.Location = new Point(50, 10);
        }

        public bool enableOverlayTimer
        {
            set => timerUpdateTimer.Enabled = value;
        }

        private void TimerUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (timers.Any())
                SetTimerText();

            // info
            if (labelInfo.Text != "" && infoShownAt.AddSeconds(InfoDuration) < DateTime.Now)
                labelInfo.Text = string.Empty;

            if (!ocrPossible) return;

            toggleInventoryCheck = !toggleInventoryCheck;
            if (checkInventoryStats && toggleInventoryCheck)
            {
                if (OCRing)
                    return;
                lblStatus.Text = "…";
                Application.DoEvents();
                OCRing = true;
                if (!ArkOCR.OCR.isDinoInventoryVisible())
                {
                    if (currentlyInInventory)
                    {
                        for (int i = 0; i < LABEL_COUNT; i++)
                            if (labels[i] != null)
                                labels[i].Text = string.Empty;
                        currentlyInInventory = false;
                    }
                }
                else if (currentlyInInventory)
                {
                    // assuming it's still the same inventory, don't do anything, assuming nothing changed
                }
                else
                {
                    currentlyInInventory = true;
                    lblStatus.Text = "Reading Values";
                    Application.DoEvents();
                    ExtractorForm?.DoOCR("", false);
                }
                OCRing = false;
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
                labels[s].Text = "[w" + wildValues[di];
                if (tamedValues[di] != 0)
                    labels[s].Text += "+d" + tamedValues[di];
                labels[s].Text += "]";
                if (colors != null && di < colors.Length)
                    labels[s].ForeColor = colors[di];
            }

            // total level
            labels[7].Text = "[w" + levelWild;
            if (levelDom != 0)
                labels[7].Text += "+d" + levelDom;
            labels[7].Text += "]";
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
            infoShownAt = DateTime.Now;
        }

        internal void SetInfoText(string infoText) => SetInfoText(infoText, Color.White);

        private void SetTimerText()
        {
            var sb = new StringBuilder();
            foreach (TimerListEntry tle in timers.ToList()) // .ToList() is used to make a copy, to be able to remove expired elements in the loop
            {
                int secLeft = (int)tle.time.Subtract(DateTime.Now).TotalSeconds + 1;
                if (secLeft < 10)
                {
                    if (!Properties.Settings.Default.KeepExpiredTimersInOverlay && secLeft < -20)
                    {
                        timers.Remove(tle);
                        tle.showInOverlay = false;
                        continue;
                    }
                    sb.Append("!!! ");
                }
                sb.AppendLine($"{Utils.timeLeft(tle.time)} : {tle.name}");
            }
            labelTimer.Text = sb.ToString() + notes;
        }

        internal void SetNotes(string notes)
        {
            this.notes = notes;
            SetTimerText();
        }
    }
}

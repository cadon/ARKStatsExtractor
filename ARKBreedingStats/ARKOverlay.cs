using ARKBreedingStats.ocr;
using ARKBreedingStats.species;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class ARKOverlay : Form
    {
        private readonly Control[] labels = new Control[10];
        private readonly Timer timerUpdateTimer;
        public Form1 ExtractorForm;
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
            labels[8] = lblExtraText;
            labels[9] = lblBreedingProgress;

            foreach (Label l in labels)
                l.Text = string.Empty;
            lblStatus.Text = string.Empty;
            labelTimer.Text = string.Empty;
            labelInfo.Text = string.Empty;

            Size = new Size(ArkOCR.OCR.ocrConfig.resolutionWidth, ArkOCR.OCR.ocrConfig.resolutionHeight);

            timerUpdateTimer = new Timer { Interval = 1000 };
            timerUpdateTimer.Tick += TimerUpdateTimer_Tick;
            theOverlay = this;
            currentlyInInventory = false;

            if (!ArkOCR.OCR.setResolution())
                MessageBox.Show("No calibration-info for this resolution found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            labelInfo.Location = new Point(ArkOCR.OCR.ocrConfig.resolutionWidth - (labelInfo.Width + 30), 40);

            notes = string.Empty;

            InfoDuration = 10;
        }

        public void InitLabelPositions()
        {
            for (int statIndex = 0; statIndex < 8; statIndex++)
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
            if (timers.Count > 0)
                SetTimerText();

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
                        for (int i = 0; i < labels.Count(); i++)
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

            // info
            if (labelInfo.Text != "" && infoShownAt.AddSeconds(InfoDuration) < DateTime.Now)
                labelInfo.Text = string.Empty;
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

            lblExtraText.Location = new Point(labels[0].Location.X - 100, 40);
            lblBreedingProgress.Text = string.Empty;
        }

        internal void SetExtraText(string p)
        {
            lblExtraText.Visible = true;
            labelInfo.Visible = false;
            //Point loc = this.PointToClient(ArkOCR.OCR.lastLetterPositions["NameAndLevel"]);
            //Point loc = this.PointToClient(new Point(ArkOCR.OCR.ocrConfig.labelRectangles[9].X, ArkOCR.OCR.ocrConfig.labelRectangles[9].Y + 30));
            Point loc = new Point(ArkOCR.OCR.ocrConfig.labelRectangles[9].X, ArkOCR.OCR.ocrConfig.labelRectangles[9].Y + 30);

            loc.Offset(0, 30);

            lblExtraText.Text = p;
            lblExtraText.Location = loc;
        }

        /// <summary>
        /// Used to display longer texts at the top right, e.g. taming-info.
        /// </summary>
        /// <param name="infoText"></param>
        internal void SetInfoText(string infoText, Color textColor)
        {
            labelInfo.ForeColor = textColor;
            lblExtraText.Visible = false;
            labelInfo.Visible = true;
            labelInfo.Text = infoText;
            infoShownAt = DateTime.Now;
        }

        private void SetTimerText()
        {
            string timerText = string.Empty;
            foreach (TimerListEntry tle in timers.ToList()) // .ToList() is used to make a copy, to be able to remove expired elements in the loop
            {
                int secLeft = (int)tle.time.Subtract(DateTime.Now).TotalSeconds + 1;
                if (secLeft < 10)
                {
                    if (secLeft < -20)
                    {
                        timers.Remove(tle);
                        tle.showInOverlay = false;
                        continue;
                    }
                    timerText += "!!! ";
                }
                timerText += Utils.timeLeft(tle.time) + ": " + tle.name + "\n";
            }
            labelTimer.Text = timerText + notes;
        }

        internal void SetNotes(string notes)
        {
            this.notes = notes;
            SetTimerText();
        }
    }
}

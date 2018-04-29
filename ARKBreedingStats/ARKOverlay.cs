using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public partial class ARKOverlay : Form
    {
        private Control[] labels = new Control[10];
        private Timer timerUpdateTimer = new Timer();
        public Form1 ExtractorForm;
        public bool OCRing = false;
        public List<TimerListEntry> timers = new List<TimerListEntry>();
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
                l.Text = "";
            lblStatus.Text = "";
            labelTimer.Text = "";
            labelInfo.Text = "";

            Location = Point.Empty;
            Size = new Size(ArkOCR.OCR.ocrConfig.resolutionWidth, ArkOCR.OCR.ocrConfig.resolutionHeight);

            timerUpdateTimer.Interval = 1000;
            timerUpdateTimer.Tick += TimerUpdateTimer_Tick;
            theOverlay = this;
            currentlyInInventory = false;


            if (!ArkOCR.OCR.setResolution())
                MessageBox.Show("No calibration-info for this resolution found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            labelInfo.Location = new Point(ArkOCR.OCR.ocrConfig.resolutionWidth - (labelInfo.Width + 30), 40);

            InfoDuration = 10;
        }

        public void initLabelPositions()
        {
            for (int statIndex = 0; statIndex < 8; statIndex++)
            {
                Rectangle r = ArkOCR.OCR.ocrConfig.labelRectangles[statIndex];
                labels[statIndex].Location = new Point(r.Left + r.Width + 6, r.Top - 10); //this.PointToClient(new Point(r.Left + r.Width + 6, r.Top - 10));
            }
            lblStatus.Location = new Point(50, 10);
        }

        public bool enableOverlayTimer { set { timerUpdateTimer.Enabled = value; } }

        void inventoryCheckTimer_Tick(object sender, EventArgs e)
        {


            return;
        }

        private void TimerUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (timers.Count > 0)
                setTimer();

            toggleInventoryCheck = !toggleInventoryCheck;
            if (checkInventoryStats && toggleInventoryCheck)
            {
                if (OCRing == true)
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
                                labels[i].Text = "";
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
                    if (ExtractorForm != null)
                        ExtractorForm.doOCR("", false);
                }
                OCRing = false;
                lblStatus.Text = "";
                Application.DoEvents();
            }

            // info
            if (labelInfo.Text != "" && infoShownAt.AddSeconds(InfoDuration) < DateTime.Now)
                labelInfo.Text = "";
        }

        public void setStatLevels(float[] wildValues, float[] tamedValues, Color[] colors = null)
        {
            for (int statIndex = 0; statIndex < 8; statIndex++)
            {
                int labelIndex = statIndex;
                if (statIndex == 7)
                    statIndex = 9; // skip torpor and imprinting, index:9: level
                labels[labelIndex].Text = "[w" + wildValues[statIndex];
                if (tamedValues[statIndex] != 0)
                    labels[labelIndex].Text += "+d" + tamedValues[statIndex];
                labels[labelIndex].Text += "]";

                if (statIndex < 8)
                    labels[labelIndex].ForeColor = colors[statIndex];
            }
            lblExtraText.Location = new Point(labels[0].Location.X - 100, 40);
            lblBreedingProgress.Text = "";
        }

        internal void setExtraText(string p)
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

        internal void setInfoText(string p)
        {
            // used to display longer texts, e.g. taming-info
            lblExtraText.Visible = false;
            labelInfo.Visible = true;
            labelInfo.Text = p;
            infoShownAt = DateTime.Now;
        }

        public void setTimer()
        {
            string timerText = "";
            foreach (TimerListEntry tle in timers)
            {
                timerText += Utils.timeLeft(tle.time) + ":" + tle.name + "\n";
            }
            labelTimer.Text = timerText;
        }

        internal void setBreedingProgressValues(float percentage, int maxTime)
        {
            return;
            // current weight cannot be read in the new ui. TODO remove this function when current weight is confirmed to not be shown anymore
            if (percentage >= 1)
            {
                lblBreedingProgress.Text = "";
                return;
            }
            string text = "";
            text = string.Format(@"Progress: {0:P2}", percentage);
            TimeSpan ts;
            string tsformat = "";
            if (percentage <= 0.1)
            {
                ts = new TimeSpan(0, 0, (int)(maxTime * (0.1 - percentage)));
                tsformat = "";
                tsformat += ts.Days > 0 ? "d'd'" : "";
                tsformat += ts.Hours > 0 ? "hh'h'" : "";
                tsformat += ts.Minutes > 0 ? "mm'm'" : "";
                tsformat += "ss's'";

                text += "\r\n[juvenile: " + ts.ToString(tsformat) + "]";
            }
            if (percentage <= 0.5)
            {
                ts = new TimeSpan(0, 0, (int)(maxTime * (0.5 - percentage)));
                tsformat = "";
                tsformat += ts.Days > 0 ? "d'd'" : "";
                tsformat += ts.Hours > 0 ? "hh'h'" : "";
                tsformat += ts.Minutes > 0 ? "mm'm'" : "";
                tsformat += "ss's'";

                text += "\r\n[adolescent: " + ts.ToString(tsformat) + "]";
            }

            ts = new TimeSpan(0, 0, (int)(maxTime * (1 - percentage)));
            tsformat = "";
            tsformat += ts.Days > 0 ? "d'd'" : "";
            tsformat += ts.Hours > 0 ? "hh'h'" : "";
            tsformat += ts.Minutes > 0 ? "mm'm'" : "";
            tsformat += "ss's'";

            text += "\r\n[adult: " + ts.ToString(tsformat) + "]";

            lblBreedingProgress.Text = text;
            //lblBreedingProgress.Location = this.PointToClient(ArkOCR.OCR.lastLetterPositions["CurrentWeight"]);
            lblBreedingProgress.Location = ArkOCR.OCR.lastLetterPositions["CurrentWeight"];
        }
    }
}

using ARKBreedingStats.ocr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.utils;

namespace ARKBreedingStats
{
    public partial class ARKOverlay : Form
    {
        private readonly Label[] _labels;
        private readonly Timer _timerUpdateTimer;
        public Form1 ExtractorForm;
        private bool _ocrPossible;
        private bool _OCRing;
        public TimerListEntry[] timers;
        public List<Creature> CreatureTimers;
        public List<IncubationTimerEntry> IncubationTimers;
        private string _notes;
        public static ARKOverlay theOverlay;
        private DateTime _infoShownAt;
        public int InfoDuration;
        private bool _currentlyInInventory;
        public bool checkInventoryStats;
        private bool _toggleInventoryCheck; // check inventory only every other time
        private Dictionary<Label, float> _initialFontSizes;

        public ARKOverlay()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Win32API.SetHitTestVisibility(this.Handle, false);
            TopMost = true;
            parentInheritance1.ForeColor = Color.FromArgb(1, 1, 1); // so it's not transparent (black == TransparencyKey)

            _infoShownAt = DateTime.Now.AddMinutes(-10);
            _labels = new[] { lblHealth, lblStamina, lblOxygen, lblFood, lblWeight, lblMeleeDamage, lblMovementSpeed, lblLevel };

            // save initial font sizes for later adjustment
            _initialFontSizes = new Dictionary<Label, float>();

            foreach (Label l in _labels)
            {
                l.Text = string.Empty;
                _initialFontSizes[l] = l.Font.Size;
            }
            lblStatus.Text = string.Empty;
            labelTimer.Text = string.Empty;
            labelInfo.Text = string.Empty;

            _initialFontSizes[lblStatus] = lblStatus.Font.Size;
            _initialFontSizes[labelTimer] = labelTimer.Font.Size;
            _initialFontSizes[labelInfo] = labelInfo.Font.Size;


            Size = ArkOcr.Ocr.GetScreenshotOfProcess()?.Size ?? default;
            if (Size == default)
                Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            _timerUpdateTimer = new Timer { Interval = 1000 };
            _timerUpdateTimer.Tick += TimerUpdateTimer_Tick;
            theOverlay = this;
            _currentlyInInventory = false;

            _ocrPossible = ArkOcr.Ocr.ocrConfig != null && ArkOcr.Ocr.CheckResolutionSupportedByOcr();

            SetInfoPositionsAndFontSize();
            _notes = string.Empty;
            SetInheritanceCreatures();
            SetLocalizations();

            InfoDuration = 10;
        }

        public void SetInfoPositionsAndFontSize()
        {
            labelTimer.Location = Properties.Settings.Default.OverlayTimerPosition;
            labelInfo.Location = new Point(Size.Width - labelInfo.Width - Properties.Settings.Default.OverlayInfoPosition.X, Properties.Settings.Default.OverlayInfoPosition.Y);
            SetLabelFontSize(Properties.Settings.Default.OverlayRelativeFontSize);
        }

        public void InitLabelPositions()
        {
            if (!_ocrPossible) return;

            for (int statIndex = 0; statIndex < _labels.Length; statIndex++)
            {
                Rectangle r = ArkOcr.Ocr.ocrConfig.UsedLabelRectangles[statIndex];
                _labels[statIndex].Location = new Point(r.Left + r.Width + 30, r.Top - 10);
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
            if (!string.IsNullOrEmpty(labelInfo.Text) && _infoShownAt.AddSeconds(InfoDuration) < DateTime.Now)
            {
                labelInfo.Text = string.Empty;
                parentInheritance1.Visible = false;
            }

            if (!_ocrPossible) return;

            _toggleInventoryCheck = !_toggleInventoryCheck;
            if (!checkInventoryStats || !_toggleInventoryCheck) return;
            if (_OCRing)
                return;
            lblStatus.Text = "…";
            Application.DoEvents();
            _OCRing = true;
            if (!ArkOcr.Ocr.IsDinoInventoryVisible())
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

        public void SetStatLevels(int[] wildValues, int[] tamedValues, int levelWild, int levelDom, Color[] colors = null)
        {
            // only 7 stats are displayed
            var displayIndices = new int[] { Stats.Health, Stats.Stamina, Stats.Oxygen, Stats.Food, Stats.Weight, Stats.MeleeDamageMultiplier, Stats.SpeedMultiplier };
            for (int s = 0; s < 7; s++)
            {
                int di = displayIndices[s];
                _labels[s].Text = wildValues[di] == -1 ? "?" : wildValues[di].ToString();
                if (tamedValues[di] > 0)
                    _labels[s].Text += $" +{tamedValues[di]}";
                if (colors != null && di < colors.Length)
                    _labels[s].ForeColor = colors[di];
            }

            // total level
            _labels[7].Text = "w" + levelWild;
            if (levelDom != 0)
                _labels[7].Text += "+d" + levelDom;
        }

        /// <summary>
        /// Used to display longer texts at the top right, e.g. taming-info.
        /// </summary>
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
            var sb = new StringBuilder();

            if (timers?.Any() ?? false)
            {
                var timerListChanged = false;
                foreach (TimerListEntry tle in timers)
                {
                    var timeLeft = tle.time.Subtract(DateTime.Now);
                    int secLeft = (int)timeLeft.TotalSeconds + 1;
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
                    sb.AppendLine($"{Utils.Duration(timeLeft)} : {tle.name}");
                }
                if (timerListChanged)
                    timers = timers.Where(t => t.showInOverlay).ToArray();
            }
            if (IncubationTimers?.Any() ?? false)
            {
                sb.AppendLine();
                sb.AppendLine(Loc.S("Incubation"));
                foreach (var it in IncubationTimers)
                {
                    var timeLeft = it.incubationEnd.Subtract(DateTime.Now);
                    int secLeft = (int)timeLeft.TotalSeconds + 1;
                    if (secLeft < 10)
                    {
                        if (!Properties.Settings.Default.KeepExpiredTimersInOverlay && secLeft < -20)
                        {
                            it.ShowInOverlay = false;
                            RemoveTimer(it);
                            continue;
                        }
                        sb.Append("incubated ");
                    }
                    sb.AppendLine($"{Utils.Duration(timeLeft)} : {(it.Mother?.Species ?? it.Father?.Species)?.DescriptiveName ?? "unknown species"}");
                }
            }
            if (CreatureTimers?.Any() ?? false)
            {
                sb.AppendLine();
                sb.AppendLine(Loc.S("Maturation"));
                foreach (var c in CreatureTimers)
                {
                    var timeLeft = c.growingUntil?.Subtract(DateTime.Now);
                    int secLeft = timeLeft == null ? -100 : (int)timeLeft.Value.TotalSeconds + 1;
                    if (secLeft < 10)
                    {
                        if (!Properties.Settings.Default.KeepExpiredTimersInOverlay && secLeft < -20)
                        {
                            c.ShowInOverlay = false;
                            RemoveTimer(c);
                            continue;
                        }

                        timeLeft = null;
                    }
                    sb.AppendLine($"{(timeLeft == null ? "grown" : Utils.Duration(timeLeft.Value))} : {c.name} ({c.Species.DescriptiveName})");
                }
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

        public static void AddTimer(Creature creature)
        {
            creature.ShowInOverlay = true;

            if (theOverlay == null)
                return;

            if (theOverlay.CreatureTimers == null)
                theOverlay.CreatureTimers = new List<Creature> { creature };
            else theOverlay.CreatureTimers.Add(creature);
        }

        public static void RemoveTimer(Creature creature)
        {
            creature.ShowInOverlay = false;
            if (theOverlay?.CreatureTimers == null) return;
            theOverlay.CreatureTimers.Remove(creature);
            if (!theOverlay.CreatureTimers.Any())
                theOverlay.CreatureTimers = null;
        }

        public static void AddTimer(IncubationTimerEntry incubationTimer)
        {
            incubationTimer.ShowInOverlay = true;

            if (theOverlay == null)
                return;

            if (theOverlay.IncubationTimers == null)
                theOverlay.IncubationTimers = new List<IncubationTimerEntry> { incubationTimer };
            else theOverlay.IncubationTimers.Add(incubationTimer);
        }

        public static void RemoveTimer(IncubationTimerEntry incubationTimer)
        {
            incubationTimer.ShowInOverlay = false;
            if (theOverlay?.IncubationTimers == null) return;
            theOverlay.IncubationTimers.Remove(incubationTimer);
            if (!theOverlay.IncubationTimers.Any())
                theOverlay.IncubationTimers = null;
        }

        public void SetLabelFontSize(float relativeSize)
        {
            foreach (var l in _initialFontSizes)
                l.Key.Font = new Font(l.Key.Font.FontFamily, l.Value * relativeSize, l.Key.Font.Style);
        }

        internal void SetLocalizations()
        {
            parentInheritance1.SetLocalizations();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.utils;
using ARKBreedingStats.values;

namespace ARKBreedingStats.uiControls
{
    public partial class CreatureAnalysis : UserControl
    {
        private LevelColorStatusFlags.LevelStatus _statsStatus;
        private LevelColorStatusFlags.LevelStatus _colorStatus;
        public string ColorStatus;
        private readonly Label[] _labelsRegionColors = new Label[Ark.ColorRegionCount * 3]; // for each color region 3 labels: 0: color id; 1: creature count with the color; 2: indicator if color is wanted
        private readonly LinkLabel[] _linklabelsRegionColors = new LinkLabel[Ark.ColorRegionCount];
        private byte[] _colorIdsCurrent;
        public event Action<string> ViewLibraryWithFilter;
        private readonly ToolTip _tt = new ToolTip { InitialDelay = 100 };

        public CreatureAnalysis()
        {
            InitializeComponent();
            TlpRegionInfo.RowCount = Ark.ColorRegionCount + 1;
            for (var ri = 0; ri < Ark.ColorRegionCount; ri++)
            {
                TlpRegionInfo.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var lbRegionId = new Label { AutoSize = true, Text = ri.ToString(), Anchor = AnchorStyles.None };
                var lbColorId = new Label { AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Padding = new Padding(3) };
                var lbCreatureCount = new Label { AutoSize = true, TextAlign = ContentAlignment.MiddleRight, Anchor = AnchorStyles.Right, Padding = new Padding(3) };
                var lbWantedColor = new Label { AutoSize = true, ForeColor = Color.White, BackColor = Color.DarkGreen, Padding = new Padding(3) };
                var llbLibraryLink = new LinkLabel { AutoSize = true, Text = "view", Tag = ri };
                llbLibraryLink.Click += (s, e) => ViewInLibrary((int)((LinkLabel)s).Tag);

                _labelsRegionColors[3 * ri] = lbColorId;
                _labelsRegionColors[3 * ri + 1] = lbCreatureCount;
                _labelsRegionColors[3 * ri + 2] = lbWantedColor;
                _linklabelsRegionColors[ri] = llbLibraryLink;

                TlpRegionInfo.Controls.Add(lbRegionId, 0, ri + 1);
                TlpRegionInfo.Controls.Add(lbColorId, 1, ri + 1);
                TlpRegionInfo.Controls.Add(lbCreatureCount, 2, ri + 1);
                TlpRegionInfo.Controls.Add(llbLibraryLink, 3, ri + 1);
                TlpRegionInfo.Controls.Add(lbWantedColor, 4, ri + 1);
            }
        }

        private void ViewInLibrary(int regionId)
        {
            if (_colorIdsCurrent == null) return;
            ViewLibraryWithFilter?.Invoke($"c{regionId}: {_colorIdsCurrent[regionId]}");
        }

        public void SetStatsAnalysis(LevelColorStatusFlags.LevelStatus statsStatus, string statsAnalysis)
        {
            _statsStatus = statsStatus;
            SetStatus(LbStatsStatus, statsStatus);

            LbStatAnalysis.Text = statsAnalysis;

            var generalStatus = statsStatus;
            if (!generalStatus.HasFlag(LevelColorStatusFlags.LevelStatus.NewTopLevel) && _colorStatus != LevelColorStatusFlags.LevelStatus.Neutral)
            {
                generalStatus = _colorStatus;
            }

            SetStatus(LbIcon, generalStatus, LbConclusion);
        }

        /// <summary>
        /// Set the color status and uses the earlier set statsStatus.
        /// </summary>
        public void SetColorAnalysis(LevelColorStatusFlags.LevelStatus colorStatus, string colorAnalysis, byte[] colorIds, int[][] creaturesWithColorsInRegion, bool[] regionsWithWantedColor)
        {
            _colorStatus = colorStatus;
            _colorIdsCurrent = colorIds;
            SetStatus(LbColorStatus, colorStatus);

            ColorStatus = colorAnalysis;
            LbColorAnalysis.Text = colorAnalysis;
            SetColorTable(colorIds, creaturesWithColorsInRegion, regionsWithWantedColor);

            var generalStatus = _statsStatus;
            if (generalStatus != LevelColorStatusFlags.LevelStatus.NewTopLevel && colorStatus != LevelColorStatusFlags.LevelStatus.Neutral)
            {
                generalStatus = colorStatus;
            }

            SetStatus(LbIcon, generalStatus, LbConclusion);
        }

        private void SetColorTable(byte[] colorIds = null, int[][] creaturesWithColorsInRegion = null, bool[] regionsWithWantedColor = null)
        {
            if (creaturesWithColorsInRegion == null || colorIds == null)
            {
                TlpRegionInfo.Visible = false;
                return;
            }

            for (var ri = 0; ri < Ark.ColorRegionCount; ri++)
            {
                if (creaturesWithColorsInRegion[ri] == null)
                {
                    _labelsRegionColors[3 * ri].Visible = false;
                    _labelsRegionColors[3 * ri + 1].Visible = false;
                    _labelsRegionColors[3 * ri + 2].Visible = false;
                    _linklabelsRegionColors[ri].Visible = false;
                    TlpRegionInfo.RowStyles[ri + 1].SizeType = SizeType.Absolute;
                    TlpRegionInfo.RowStyles[ri + 1].Height = 0;
                }
                else
                {
                    _labelsRegionColors[3 * ri].Visible = true;
                    _labelsRegionColors[3 * ri + 1].Visible = true;
                    _labelsRegionColors[3 * ri].Text = colorIds[ri].ToString();
                    var arkColor = Values.V.Colors.ById(colorIds[ri]);
                    _labelsRegionColors[3 * ri].SetBackColorAndAccordingForeColor(arkColor.Color);
                    _tt.SetToolTip(_labelsRegionColors[3 * ri], $"[{ri}]: {colorIds[ri]} - {arkColor.Name}");
                    var creatureCountWithThisRegionColor = creaturesWithColorsInRegion[ri][colorIds[ri]];
                    _labelsRegionColors[3 * ri + 1].Text = creatureCountWithThisRegionColor.ToString();
                    _labelsRegionColors[3 * ri + 1].BackColor = creatureCountWithThisRegionColor == 0 ? Color.LightYellow : Color.Transparent;

                    var wantedColor = regionsWithWantedColor?[ri] == true;
                    _labelsRegionColors[3 * ri + 2].Text = wantedColor ? "★" : string.Empty;
                    _tt.SetToolTip(_labelsRegionColors[3 * ri + 2], wantedColor ? "Wanted color" : null);
                    _labelsRegionColors[3 * ri + 2].Visible = wantedColor;
                    TlpRegionInfo.RowStyles[ri + 1].SizeType = SizeType.AutoSize;
                    _linklabelsRegionColors[ri].Visible = creatureCountWithThisRegionColor > 0;
                }
            }

            TlpRegionInfo.Visible = true;
        }

        private void SetStatus(Label labelIcon, LevelColorStatusFlags.LevelStatus status, Label labelText = null)
        {
            if (status.HasFlag(LevelColorStatusFlags.LevelStatus.NewTopLevel))
            {
                labelIcon.BackColor = Color.LightYellow;
                labelIcon.ForeColor = Color.Gold;
                labelIcon.Text = "★";
                if (labelText != null)
                    labelText.Text = "Keep this creature, it adds new traits to your library!";
            }
            else if (status.HasFlag(LevelColorStatusFlags.LevelStatus.TopLevel))
            {
                labelIcon.BackColor = Color.LightGreen;
                labelIcon.ForeColor = Color.DarkGreen;
                labelIcon.Text = "✓";
                if (labelText != null)
                    labelText.Text = "Keep this creature!";
            }
            else
            {
                labelIcon.BackColor = Color.LightGray;
                labelIcon.ForeColor = Color.Gray;
                labelIcon.Text = "-";
                if (labelText != null)
                    labelText.Text = "This creature adds nothing new to your library.";
            }
        }

        /// <summary>
        /// Colors are not cleared, they are set independent of the stats if the colors are changed.
        /// </summary>
        public void Clear()
        {
            ColorStatus = null;
            ClearLabel(LbIcon);
            ClearLabel(LbConclusion);
            ClearLabel(LbStatAnalysis);
            ClearLabel(LbStatsStatus);
            //ClearLabel(LbColorAnalysis);
            //ClearLabel(LbColorStatus);
            SetColorTable();
            void ClearLabel(Label l)
            {
                l.BackColor = Color.Transparent;
                l.Text = string.Empty;
            }
        }
    }
}

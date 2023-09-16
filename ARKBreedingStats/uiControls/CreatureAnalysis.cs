using System.Drawing;
using System.Windows.Forms;

namespace ARKBreedingStats.uiControls
{
    public partial class CreatureAnalysis : UserControl
    {
        public CreatureAnalysis()
        {
            InitializeComponent();
        }

        private LevelStatus _statsStatus;
        private LevelStatus _colorStatus;

        public void SetStatsAnalysis(LevelStatus statsStatus, string statsAnalysis)
        {
            _statsStatus = statsStatus;
            SetStatus(LbStatsStatus, statsStatus);

            LbStatAnalysis.Text = statsAnalysis;

            var generalStatus = statsStatus;
            if (generalStatus != LevelStatus.NewTopLevel && _colorStatus != LevelStatus.Neutral)
            {
                generalStatus = _colorStatus;
            }

            SetStatus(LbIcon, generalStatus, LbConclusion);
        }

        /// <summary>
        /// Set the color status and uses the earlier set statsStatus.
        /// </summary>
        public void SetColorAnalysis(LevelStatus colorStatus, string colorAnalysis)
        {
            _colorStatus = colorStatus;
            SetStatus(LbColorStatus, colorStatus);

            LbColorAnalysis.Text = colorAnalysis;

            var generalStatus = _statsStatus;
            if (generalStatus != LevelStatus.NewTopLevel && colorStatus != LevelStatus.Neutral)
            {
                generalStatus = colorStatus;
            }

            SetStatus(LbIcon, generalStatus, LbConclusion);
        }

        private void SetStatus(Label labelIcon, LevelStatus status, Label labelText = null)
        {
            switch (status)
            {
                case LevelStatus.TopLevel:
                    labelIcon.BackColor = Color.LightGreen;
                    labelIcon.ForeColor = Color.DarkGreen;
                    labelIcon.Text = "✓";
                    if (labelText != null)
                        labelText.Text = "Keep this creature!";
                    break;
                case LevelStatus.NewTopLevel:
                    labelIcon.BackColor = Color.LightYellow;
                    labelIcon.ForeColor = Color.Gold;
                    labelIcon.Text = "★";
                    if (labelText != null)
                        labelText.Text = "Keep this creature, it adds new traits to your library!";
                    break;
                default:
                    labelIcon.BackColor = Color.LightGray;
                    labelIcon.ForeColor = Color.Gray;
                    labelIcon.Text = "-";
                    if (labelText != null)
                        labelText.Text = "This creature adds nothing new to your library.";
                    break;
            }
        }

        /// <summary>
        /// Colors are not cleared, they are set independently from the stats if the colors are changed.
        /// </summary>
        public void Clear()
        {
            ClearLabel(LbIcon);
            ClearLabel(LbConclusion);
            ClearLabel(LbStatAnalysis);
            ClearLabel(LbStatsStatus);
            //ClearLabel(LbColorAnalysis);
            //ClearLabel(LbColorStatus);

            void ClearLabel(Label l)
            {
                l.BackColor = Color.Transparent;
                l.Text = string.Empty;
            }
        }
    }
}

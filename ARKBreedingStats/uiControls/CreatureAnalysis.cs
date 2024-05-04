using System.Drawing;
using System.Windows.Forms;
using ARKBreedingStats.library;

namespace ARKBreedingStats.uiControls
{
    public partial class CreatureAnalysis : UserControl
    {
        public CreatureAnalysis()
        {
            InitializeComponent();
        }

        private LevelStatusFlags.LevelStatus _statsStatus;
        private LevelStatusFlags.LevelStatus _colorStatus;
        public string ColorStatus;

        public void SetStatsAnalysis(LevelStatusFlags.LevelStatus statsStatus, string statsAnalysis)
        {
            _statsStatus = statsStatus;
            SetStatus(LbStatsStatus, statsStatus);

            LbStatAnalysis.Text = statsAnalysis;

            var generalStatus = statsStatus;
            if (!generalStatus.HasFlag(LevelStatusFlags.LevelStatus.NewTopLevel) && _colorStatus != LevelStatusFlags.LevelStatus.Neutral)
            {
                generalStatus = _colorStatus;
            }

            SetStatus(LbIcon, generalStatus, LbConclusion);
        }

        /// <summary>
        /// Set the color status and uses the earlier set statsStatus.
        /// </summary>
        public void SetColorAnalysis(LevelStatusFlags.LevelStatus colorStatus, string colorAnalysis)
        {
            _colorStatus = colorStatus;
            SetStatus(LbColorStatus, colorStatus);

            ColorStatus = colorAnalysis;
            LbColorAnalysis.Text = colorAnalysis;

            var generalStatus = _statsStatus;
            if (generalStatus != LevelStatusFlags.LevelStatus.NewTopLevel && colorStatus != LevelStatusFlags.LevelStatus.Neutral)
            {
                generalStatus = colorStatus;
            }

            SetStatus(LbIcon, generalStatus, LbConclusion);
        }

        private void SetStatus(Label labelIcon, LevelStatusFlags.LevelStatus status, Label labelText = null)
        {
            if (status.HasFlag(LevelStatusFlags.LevelStatus.NewTopLevel))
            {
                labelIcon.BackColor = Color.LightYellow;
                labelIcon.ForeColor = Color.Gold;
                labelIcon.Text = "★";
                if (labelText != null)
                    labelText.Text = "Keep this creature, it adds new traits to your library!";
            }
            else if (status.HasFlag(LevelStatusFlags.LevelStatus.TopLevel))
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

            void ClearLabel(Label l)
            {
                l.BackColor = Color.Transparent;
                l.Text = string.Empty;
            }
        }
    }
}

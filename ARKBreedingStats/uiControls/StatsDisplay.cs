using ARKBreedingStats.Library;
using ARKBreedingStats.utils;
using System.Windows.Forms;
using ARKBreedingStats.species;
using ARKBreedingStats.SpeciesOptions.LevelColorSettings;
using System.ComponentModel;

namespace ARKBreedingStats.uiControls
{
    internal class StatsDisplay : FlowLayoutPanel
    {
        private readonly StatDisplay[] _stats = new StatDisplay[Stats.StatsCount];
        private readonly ToolTip _tt = new ToolTip();
        private readonly Label _lbSex;
        private Species _species;
        private StatLevelColors[] _levelColors;

        public StatsDisplay()
        {
            var panelHeader = new Panel { AutoSize = true };
            SetFlowBreak(panelHeader, true);
            Controls.Add(panelHeader);
            _lbSex = new Label { AutoSize = true };
            panelHeader.Controls.Add(_lbSex);
            var lbWildL = new Label { Text = "W", AutoSize = true, Left = 28 };
            panelHeader.Controls.Add(lbWildL);
            var lbMutL = new Label { Text = "M", AutoSize = true, Left = 49 };
            panelHeader.Controls.Add(lbMutL);
            var lbDomL = new Label { Text = "D", AutoSize = true, Left = 72 };
            panelHeader.Controls.Add(lbDomL);
            var lbValueBreed = new Label { Text = "Breed", AutoSize = true, Left = 103 };
            panelHeader.Controls.Add(lbValueBreed);
            var lbValueCurrent = new Label { Text = "Current", AutoSize = true, Left = 140 };
            panelHeader.Controls.Add(lbValueCurrent);

            // tooltips
            _tt.SetToolTip(_lbSex, "Sex of the Creature");
            _tt.SetToolTip(lbWildL, "Wild levels");
            _tt.SetToolTip(lbMutL, "Mutated levels");
            _tt.SetToolTip(lbDomL, "Domestic levels");
            _tt.SetToolTip(lbValueBreed, "Value that is inherited");
            _tt.SetToolTip(lbValueCurrent, "Current Value of the Creature");

            foreach (var si in Stats.DisplayOrder)
            {
                var sd = new StatDisplay(si, Stats.IsPercentage(si), _tt);
                _stats[si] = sd;
                SetFlowBreak(sd, true);
                sd.Visible = false;
                Controls.Add(sd);
            }

            Disposed += (s, e) => _tt.RemoveAllAndDispose();
        }

        public void SetCreatureValues(Creature creature)
        {
            if (creature == null)
            {
                Clear();
                return;
            }

            if (_species != creature.Species)
            {
                _species = creature.Species;
                _levelColors = Form1.StatsOptionsLevelColors.GetOptions(_species).Options;
            }

            this.SuspendDrawingAndLayout();

            for (var s = 0; s < Stats.StatsCount; s++)
            {
                if (_species.UsesStat(s) != true || s == Stats.Torpidity)
                {
                    _stats[s].Visible = false;
                    continue;
                }

                _stats[s].LevelColors = _levelColors[s];
                _stats[s].SetCustomStatNames(_species.statNames);
                _stats[s].SetNumbers(creature.levelsWild[s], creature.levelsMutated?[s] ?? 0, creature.levelsDom[s], creature.valuesBreeding[s], creature.valuesCurrent[s]);
                _stats[s].Visible = true;
            }

            _lbSex.Text = Utils.SexSymbol(creature.sex);

            this.ResumeDrawingAndLayout();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BarMaxLevel
        {
            set
            {
                for (var s = 0; s < Stats.StatsCount; s++)
                    _stats[s].BarMaxLevel = value;
            }
        }

        public void Clear()
        {
            for (var s = 0; s < Stats.StatsCount; s++)
                _stats[s].SetNumbers(0, 0, 0, 0, 0);
            _lbSex.Text = string.Empty;
        }
    }
}

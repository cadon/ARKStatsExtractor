using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ARKBreedingStats.uiControls
{
    class StatGraphs : Chart
    {
        Series serie;

        public StatGraphs()
        {
            serie = new Series("Stat");
            serie.ChartType = SeriesChartType.Column;
            Series.Add(serie);
            ChartAreas.Add("Stat");
            ChartAreas[0].AxisX.Minimum = 0;
        }

        public void setGraph(int sI, int statIndex, int wildLevels, int domLevels, bool tamed, double TE, double imprinting)
        {
            if (sI >= 0 && sI < Values.V.speciesNames.Count && statIndex >= 0 && statIndex < 7)
            {
                CreatureStat stat = Values.V.species[sI].stats[statIndex];
                serie.Points.Clear();
                serie.Points.AddXY("Base", stat.BaseValue);
                serie.Points.AddXY("Wild", Stats.calculateValue(sI, statIndex, wildLevels, 0, false, 0, 0));
                serie.Points.AddXY("Tamed", Stats.calculateValue(sI, statIndex, wildLevels, 0, true, TE, 0));
                serie.Points.AddXY("Dom", Stats.calculateValue(sI, statIndex, wildLevels, domLevels, true, TE, 0));
                serie.Points.AddXY("Impr", Stats.calculateValue(sI, statIndex, wildLevels, domLevels, true, TE, imprinting));
            }

        }
    }
}

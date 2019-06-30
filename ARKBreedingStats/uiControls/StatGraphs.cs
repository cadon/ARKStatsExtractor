using System.Windows.Forms.DataVisualization.Charting;
using ARKBreedingStats.species;

namespace ARKBreedingStats.uiControls
{
    internal class StatGraphs : Chart
    {
        readonly Series serie;

        public StatGraphs()
        {
            serie = new Series("Stat")
            {
                ChartType = SeriesChartType.Column
            };
            Series.Add(serie);
            ChartAreas.Add("Stat");
            ChartAreas[0].AxisX.Minimum = 0;
        }

        public void setGraph(Species species, int statIndex, int wildLevels, int domLevels, bool tamed, double TE, double imprinting)
        {
            if (species != null && statIndex >= 0 && statIndex < 12)
            {
                CreatureStat stat = species.stats[statIndex];
                serie.Points.Clear();
                serie.Points.AddXY("Base", stat.BaseValue);
                serie.Points.AddXY("Wild", Stats.calculateValue(species, statIndex, wildLevels, 0, false, 0, 0));
                serie.Points.AddXY("Tamed", Stats.calculateValue(species, statIndex, wildLevels, 0, true, TE, 0));
                serie.Points.AddXY("Dom", Stats.calculateValue(species, statIndex, wildLevels, domLevels, true, TE, 0));
                serie.Points.AddXY("Impr", Stats.calculateValue(species, statIndex, wildLevels, domLevels, true, TE, imprinting));
            }
        }
    }
}

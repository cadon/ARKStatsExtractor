using ARKBreedingStats.species;
using System.Windows.Forms.DataVisualization.Charting;

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

        public void SetGraph(Species species, int statIndex, int wildLevels, int mutatedLevel, int domLevels, bool tamed, double TE, double imprinting)
        {
            if (species != null && statIndex >= 0 && statIndex < 12)
            {
                CreatureStat stat = species.stats[statIndex];
                serie.Points.Clear();
                serie.Points.AddXY("Base", stat.BaseValue);
                serie.Points.AddXY("Wild", StatValueCalculation.CalculateValue(species, statIndex, wildLevels, mutatedLevel, 0, false, 0, 0));
                serie.Points.AddXY("Tamed", StatValueCalculation.CalculateValue(species, statIndex, wildLevels, mutatedLevel, 0, true, TE, 0));
                serie.Points.AddXY("Dom", StatValueCalculation.CalculateValue(species, statIndex, wildLevels, mutatedLevel, domLevels, true, TE, 0));
                serie.Points.AddXY("Impr", StatValueCalculation.CalculateValue(species, statIndex, wildLevels, mutatedLevel, domLevels, true, TE, imprinting));
            }
        }
    }
}

using System.Text;
using System.Windows.Forms;
using ARKBreedingStats.species;
using ARKBreedingStats.values;

namespace ARKBreedingStats.uiControls
{
    public partial class Hatching : UserControl
    {
        public Hatching()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set a species to display the stats of the current top levels. This can help in determine if a new creature is good.
        /// </summary>
        /// <param name="species"></param>
        /// <param name="topLevels"></param>
        public void SetSpecies(Species species, int[] topLevels)
        {
            if (species == null)
            {
                LbHeader.Text = "no species selected";
                LbStatNames.Text = "";
                LbStatValues.Text = "";
                LbStatLevels.Text = "";
                return;
            }

            if (topLevels == null) topLevels = new int[Values.STATS_COUNT];

            LbHeader.Text = $"Best stat values for bred creatures without imprinting of the species {species.DescriptiveNameAndMod} in this library.";
            var sbNames = new StringBuilder();
            var sbValues = new StringBuilder();
            var sbLevels = new StringBuilder();

            foreach (var si in Values.statsDisplayOrder)
            {
                if (!species.UsesStat(si)) continue;

                var statValue = StatValueCalculation.CalculateValue(species, si, topLevels[si], 0, true, 1, 0);
                var statRepresentation = Utils.Precision(si) == 3 ? $"{statValue * 100:0.0} %" : $"{statValue:0.0}    ";

                sbNames.AppendLine($"{Utils.StatName(si, customStatNames: species.statNames)}:");
                sbValues.AppendLine(statRepresentation);
                sbLevels.AppendLine(topLevels[si].ToString());
            }

            LbStatNames.Text = sbNames.ToString();
            LbStatValues.Text = sbValues.ToString();
            LbStatLevels.Text = sbLevels.ToString();
        }
    }
}

﻿using System.Windows.Forms;
using ARKBreedingStats.library;
using ARKBreedingStats.species;

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
        public void SetSpecies(Species species, TopLevels topLevels)
        {
            if (species == null)
            {
                LbHeader.Text = "no species selected";
                LbStatNames.Text = string.Empty;
                LbStatValues.Text = string.Empty;
                LbStatLevels.Text = string.Empty;
                LbLowestValues.Text = string.Empty;
                LbLowestLevels.Text = string.Empty;
                return;
            }

            if (topLevels == null) topLevels = new TopLevels(true);
            var highLevels = topLevels.WildLevelsHighest;
            var lowLevels = topLevels.WildLevelsLowest;

            LbHeader.Text = $"Best stat values for bred creatures without imprinting of the species {species.DescriptiveNameAndMod} in this library.";
            string sbNames = null;
            string sbValues = null;
            string sbLevels = null;
            string sbLowestValues = null;
            string sbLowestLevels = null;

            foreach (var si in Stats.DisplayOrder)
            {
                if (!species.UsesStat(si)) continue;

                var isPercentage = Stats.IsPercentage(si);
                var statValue = StatValueCalculation.CalculateValue(species, si, highLevels[si], 0, 0, true, 1, 0);
                var statRepresentation = isPercentage ? $"{statValue * 100:0.0} %" : $"{statValue:0.0}    ";

                sbNames += $"{Utils.StatName(si, customStatNames: species.statNames)}\n";
                sbValues += statRepresentation + "\n";
                sbLevels += highLevels[si] + "\n";

                statValue = StatValueCalculation.CalculateValue(species, si, lowLevels[si], 0, 0, true, 1, 0);
                statRepresentation = isPercentage ? $"{statValue * 100:0.0} %" : $"{statValue:0.0}    ";

                sbLowestValues += statRepresentation + "\n";
                sbLowestLevels += lowLevels[si] + "\n";
            }

            LbStatNames.Text = sbNames;
            LbStatValues.Text = sbValues;
            LbStatLevels.Text = sbLevels;
            LbLowestValues.Text = sbLowestValues;
            LbLowestLevels.Text = sbLowestLevels;
        }
    }
}

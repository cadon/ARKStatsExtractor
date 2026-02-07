using System.Windows.Forms;
using ARKBreedingStats.SpeciesOptions.LevelColorSettings;

namespace ARKBreedingStats.utils
{
    internal static class LevelColorBar
    {
        internal static void SetLevelBar(Panel panel, StatLevelColors statColors, int maxBarLength, int level, bool useCustomOdd = true, bool mutationLevel = false)
        {
            const int minBarWidthForNonZeroValues = 3;
            if (statColors == null) return;
            var range = statColors.GetLevelRange(level, out var lowerBound, useCustomOdd, mutationLevel);
            if (range < 1) range = 1;
            var lengthFraction = 1d * (level - lowerBound) / range; // fraction of the max bar width

            var length = 0;
            if (lengthFraction > 0)
            {
                if (lengthFraction > 1) lengthFraction = 1;
                length = (int)((maxBarLength - minBarWidthForNonZeroValues) * lengthFraction + minBarWidthForNonZeroValues);
            }

            panel.Width = length;
            panel.BackColor = statColors.GetLevelColor(level, useCustomOdd, mutationLevel);
        }
    }
}

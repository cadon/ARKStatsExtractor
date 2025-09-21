using ARKBreedingStats.StatsOptions;
using System.Windows.Forms;

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
            var lengthPercentage = 100d * (level - lowerBound) / range; // in percentage of the max bar width

            var length = 0;
            if (lengthPercentage > 0)
            {
                if (lengthPercentage > 100) lengthPercentage = 100;
                length = (int)((maxBarLength - minBarWidthForNonZeroValues) * lengthPercentage / 100 + minBarWidthForNonZeroValues);
            }

            panel.Width = length;
            panel.BackColor = statColors.GetLevelColor(level, useCustomOdd, mutationLevel);
        }
    }
}

namespace ARKBreedingStats.utils
{
    internal static class UiUtils
    {
        /// <summary>
        /// Is supposed to equal DeviceDpi / 96f.
        /// </summary>
        internal static float UiScaling = 1;

        /// <summary>
        /// Returns an int of a scaled length based on the DeviceDpi.
        /// </summary>
        public static int UiLengthInt(int baseLength) => (int)(baseLength * UiScaling);

        /// <summary>
        /// Returns an int of a scaled length based on the DeviceDpi.
        /// </summary>
        public static float UiLength(float baseLength) => baseLength * UiScaling;
    }
}

namespace ArkSmartBreeding.Models.Species
{
    /// <summary>
    /// Info about color region pattern. This is used if a species has multiple color region patterns.
    /// </summary>
    public class ColorPattern
    {
        /// <summary>
        /// Color region that represents a pattern id (and not a color id).
        /// </summary>
        public int selectRegion;
        /// <summary>
        /// Number of patterns.
        /// </summary>
        public int count;
    }
}

using System.Drawing;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Static helper class to get ArkColor properties.
    /// </summary>
    public static class CreatureColors
    {
        /// <summary>
        /// Returns the name of a color by id.
        /// </summary>
        /// <param name="colorId"></param>
        /// <returns></returns>
        public static string CreatureColorName(byte colorId) => values.Values.V.Colors.ById(colorId).Name;

        /// <summary>
        /// Returns the Color struct of an ArkColor by id.
        /// </summary>
        /// <param name="colorId"></param>
        /// <returns></returns>
        public static Color CreatureColor(byte colorId) => values.Values.V.Colors.ById(colorId).Color;

        /// <summary>
        /// Returns the ArkColor by id.
        /// </summary>
        public static ArkColor CreatureArkColor(byte colorId) => values.Values.V.Colors.ById(colorId);
    }
}

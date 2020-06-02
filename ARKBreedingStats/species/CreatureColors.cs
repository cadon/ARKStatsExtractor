using System.Drawing;

namespace ARKBreedingStats.species
{
    static class CreatureColors
    {
        /// <summary>
        /// Returns the name of a color by id.
        /// </summary>
        /// <param name="colorId"></param>
        /// <returns></returns>
        public static string CreatureColorName(int colorId)
        {
            return values.Values.V.Colors.ById(colorId).Name;
        }

        /// <summary>
        /// Returns the Color struct of an ArkColor by id.
        /// </summary>
        /// <param name="colorId"></param>
        /// <returns></returns>
        public static Color CreatureColor(int colorId)
        {
            return values.Values.V.Colors.ById(colorId).Color;
        }

        /// <summary>
        /// Returns the ArkColor by id.
        /// </summary>
        public static ArkColor CreatureArkColor(int colorId)
        {
            return values.Values.V.Colors.ById(colorId);
        }
    }
}

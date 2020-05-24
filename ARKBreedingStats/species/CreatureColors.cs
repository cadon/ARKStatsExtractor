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
            return values.Values.V.Colors.ByID(colorId).name;
        }

        /// <summary>
        /// Returns the Color struct of an ArkColor by id.
        /// </summary>
        /// <param name="colorId"></param>
        /// <returns></returns>
        public static Color CreatureColor(int colorId)
        {
            return values.Values.V.Colors.ByID(colorId).color;
        }

        /// <summary>
        /// Returns the ArkColor by id.
        /// </summary>
        public static ARKColor CreatureArkColor(int colorId)
        {
            return values.Values.V.Colors.ByID(colorId);
        }
    }
}

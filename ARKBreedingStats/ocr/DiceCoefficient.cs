using System;

namespace ARKBreedingStats
{
    public static class DiceCoefficient
    {
        // https://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient

        public static double diceCoefficient(string input, string compareTo)
        {
            string[] ibg = biGrams(input);
            string[] cbg = biGrams(compareTo);
            int matches = 0;

            foreach (string s in ibg)
            {
                if (Array.IndexOf(cbg, s) != -1) matches++;
            }

            return 2d * matches / (ibg.Length + cbg.Length);
        }

        private static string[] biGrams(string input)
        {
            input = "$" + input + "%";
            var bg = new string[input.Length - 1];
            for (int i = 0; i < input.Length - 1; i++)
                bg[i] = input.Substring(i, 2);
            return bg;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace ARKBreedingStats
{
    [Serializable]
    public class Kibble : Dictionary<string, int>
    {
        public string RecipeAsText()
        {
            string result = "";

            foreach (string s in Keys)
            {
                result += "\n " + this[s] + " x " + s;
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats
{
    public class Animal
    {
        public String raceName;
        public String animalName;
        public AnimalGender gender;

        public Int32[] wildLevels = new Int32[] { 1, 1, 1, 1, 1, 1, 1, 1 };
        public Int32[] domLevels = new Int32[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public double[] values = new double[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public double tamingEfficiency;

        public Animal()
        {
            raceName = "Pteranodon";
            animalName = "No Name";
        }
    }

    public enum AnimalGender
    {
        Male,
        Female,
        Neuter
    };
}

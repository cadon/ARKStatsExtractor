using System.Collections.Generic;
using Newtonsoft.Json;

namespace ARKBreedingStats.species
{
    [JsonObject]
    public class CreatureStat
    {
        public double BaseValue;
        public double IncPerWildLevel;
        public double IncPerTamedLevel;
        public double AddWhenTamed;
        public double MultAffinity; // used with taming effectiveness

        [JsonIgnore]
        public static readonly Dictionary<string, int> StatAbbreviationToIndex = new Dictionary<string, int>()
        {
            {"hp",(int)StatNames.Health},
            {"st",(int)StatNames.Stamina},
            {"to",(int)StatNames.Torpidity},
            {"ox",(int)StatNames.Oxygen},
            {"fo",(int)StatNames.Food},
            {"wa",(int)StatNames.Water},
            {"te",(int)StatNames.Temperature},
            {"we",(int)StatNames.Weight},
            {"dm",(int)StatNames.MeleeDamageMultiplier},
            {"sp",(int)StatNames.SpeedMultiplier},
            {"fr",(int)StatNames.TemperatureFortitude},
            {"cr",(int)StatNames.CraftingSpeedMultiplier}
        };
    }

    public enum StatNames
    {
        Health = 0,
        Stamina = 1,
        Torpidity = 2,
        Oxygen = 3,
        Food = 4,
        Water = 5,
        Temperature = 6,
        Weight = 7,
        MeleeDamageMultiplier = 8,
        SpeedMultiplier = 9,
        TemperatureFortitude = 10,
        CraftingSpeedMultiplier = 11
    }
}

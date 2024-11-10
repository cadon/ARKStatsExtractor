using System;
using System.Collections.Generic;
using System.IO;
using ARKBreedingStats.utils;
using Newtonsoft.Json;

namespace ARKBreedingStats.species
{
    /// <summary>
    /// Usually a species has a defined set of stats which it can have wild levels and mutations.
    /// For some species these stats got changed.
    /// This can lead to issues when extracting a creature that has levels in these and the current definition doesn't allow this.
    /// This class contains the stats where creatures can have wild levels in a previous version of the game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal static class CanHaveWildLevelExceptions
    {
        [JsonProperty] public static Dictionary<string, int> SpeciesStatBits;

        public static void LoadDefinitions()
        {
            const string fileName = "canHaveWildLevelExceptions.json";
            var filePath = FileService.GetJsonPath(fileName);
            if (!File.Exists(filePath))
                return;

            try
            {
                FileService.LoadJsonFile(filePath, out SpeciesStatBits, out var errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    MessageBoxes.ShowMessageBox(errorMessage, "error when loading wild level exception file");
            }
            catch (Exception ex)
            {
                {
                    MessageBoxes.ExceptionMessageBox(ex);
                }
            }
        }

        /// <summary>
        /// Returns the bit flags of stats that can have wild levels despite the current definitions doesn't contain them.
        /// </summary>
        public static int GetWildLevelExceptions(string speciesName)
        {
            if (SpeciesStatBits == null || string.IsNullOrEmpty(speciesName)) return 0;
            if (SpeciesStatBits.TryGetValue(speciesName, out var levelBits))
                return levelBits;
            return 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using ARKBreedingStats.mods;
using ARKBreedingStats.species;
using Newtonsoft.Json;

namespace ARKBreedingStats.values
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ValuesFile
    {
        /// <summary>
        /// Checks if the version string is a format version that is supported by the version of this application.
        /// </summary>
        protected static bool IsValidFormatVersion(string version) =>
            version != null
            && (version == "1.12" // format with 12 stats (minimum required format)
                || version == "1.13" // introduced remaps for blueprintPaths
                || version == "1.14-flyerspeed" // introduced isFlyer property for AllowFlyerSpeedLeveling
                || version == "1.15-asa" // for new properties in ARK: Survival Ascended
            );

        [JsonProperty]
        private string version;
        /// <summary>
        /// Must be present and a supported value. Defaults to an invalid value
        /// </summary>
        [JsonProperty]
        protected string format;
        public Version Version;
        [JsonProperty]
        public List<Species> species;

        /// <summary>
        /// If not zero it indicates the values file contains new dye definitions that should overwrite the existing base definitions.
        /// </summary>
        [JsonProperty]
        public int dyeStartIndex;

        [JsonProperty("colorDefinitions")]
        private object[][] _colorDefinitions;

        [JsonProperty("dyeDefinitions")]
        private object[][] _dyeDefinitions;

        internal List<ArkColor> ArkColorsDyesParsed;

        /// <summary>
        /// If a species for a blueprintPath is requested, the blueprintPath will be remapped if an according key is present.
        /// This is needed if species are remapped ingame, e.g. if a variant is removed.
        /// </summary>
        [JsonProperty("remaps")]
        protected Dictionary<string, string> _blueprintRemapping;

        /// <summary>
        /// If this represents values for a mod, the mod-infos are found here.
        /// </summary>
        [JsonProperty]
        public Mod mod;

        [OnDeserialized]
        private void ParseVersionAndColors(StreamingContext ct)
        {
            if (!Version.TryParse(version, out Version))
                Version = new Version(0, 0);

            ArkColorsDyesParsed = ArkColors.ParseColorDefinitions(_colorDefinitions, ArkColorsDyesParsed);
            ArkColorsDyesParsed = ArkColors.ParseColorDefinitions(_dyeDefinitions, ArkColorsDyesParsed, true);

            //// for debugging, test if there are duplicates in the species-names
            //var duplicateSpeciesNames = string.Join("\n", species
            //                                   //.GroupBy(s => s.DescriptiveName)
            //                                   .GroupBy(s => s.NameAndMod)
            //                                   .Where(g => g.Count() > 1)
            //                                   .Select(x => x.Key)
            //                                   .ToArray());
            //if (!string.IsNullOrEmpty(duplicateSpeciesNames))
            //    Clipboard.SetText(duplicateSpeciesNames);
        }

        protected static Values LoadBaseValuesFile(string filePath)
        {
            if (FileService.LoadJsonFile(filePath, out Values readData, out string errorMessage))
            {
                if (!IsValidFormatVersion(readData.format)) throw new FormatException($"Unsupported values format version: {(readData.format ?? "null")}");
                return readData;
            }
            throw new FileLoadException(errorMessage);
        }

        protected static ValuesFile LoadValuesFile(string filePath)
        {
            if (FileService.LoadJsonFile(filePath, out ValuesFile readData, out string errorMessage))
            {
                if (!IsValidFormatVersion(readData.format)) throw new FormatException($"Unsupported values format version: {(readData.format ?? "null")}");
                return readData;
            }
            throw new FileLoadException(errorMessage);
        }

        /// <summary>
        /// Tries to load a mod file.
        /// If the mod-values will be used, setModFileName should be true.
        /// If the file cannot be found or the format is wrong, the file is ignored and no exception is thrown if throwExceptionOnFail is false.
        /// </summary>
        protected static bool TryLoadValuesFile(string filePath, bool setModFileName, bool throwExceptionOnFail, out ValuesFile values, out string errorMessage, bool checkIfModPropertyIsExisting = false)
        {
            values = null;
            errorMessage = null;
            try
            {
                values = LoadValuesFile(filePath);
                if (checkIfModPropertyIsExisting && string.IsNullOrEmpty(values.mod?.id))
                {
                    errorMessage =
                        $"The mod file\n{filePath}\ndoesn't contains an object \"mod\" or that object doesn't contain a valid entry \"id\". The mod file cannot be loaded until this information is added";
                    return false;
                }
                if (setModFileName) values.mod.FileName = Path.GetFileName(filePath);

                return true;
            }
            catch (FileNotFoundException ex)
            {
                errorMessage = "Values-File '" + filePath + "' not found. "
                               + "This collection seems to have modified stat values that are saved in a separate file, "
                               + "which couldn't be found at the saved location.";
                if (throwExceptionOnFail)
                    throw new FileNotFoundException(errorMessage, ex);
            }
            catch (FormatException ex)
            {
                errorMessage = "Values-File '" + filePath + $"' has an invalid version.\n{ex.Message}\nTry updating ARK Smart Breeding.";
                if (throwExceptionOnFail)
                    throw new FormatException(errorMessage);
            }
            return false;
        }
    }
}

using System;
using ARKBreedingStats.utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARKBreedingStats.species;
using Newtonsoft.Json;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Options for stats of species, e.g. breeding stat weights and graph representation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class StatsOptions
    {
        public static Dictionary<string, StatsOptions> StatsOptionsDict;

        /// <summary>
        /// Name of the stats options, usually a species name.
        /// </summary>
        [JsonProperty]
        public string Name;

        public override string ToString() => string.IsNullOrEmpty(Name) ? $"<{Loc.S("default")}>" : Name;

        /// <summary>
        /// Name of the parent setting
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ParentName;

        public StatsOptions ParentOptions;

        [JsonProperty]
        public StatOptions[] StatOptions;

        /// <summary>
        /// Load stats options from the settings file.
        /// </summary>
        public static void LoadSettings()
        {
            var filePath = FileService.GetJsonPath("statOptions.json");

            string errorMessage = null;
            if (!File.Exists(filePath)
                || !FileService.LoadJsonFile(filePath, out StatsOptionsDict, out errorMessage))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    MessageBoxes.ShowMessageBox(errorMessage);
                StatsOptionsDict = new Dictionary<string, StatsOptions>();
            }

            // default value
            if (!StatsOptionsDict.ContainsKey(string.Empty))
                StatsOptionsDict[string.Empty] = GetDefaultStatOptions(string.Empty);
            var rootSettings = StatsOptionsDict[string.Empty];

            foreach (var o in StatsOptionsDict.Values)
            {
                // set parent except for root (root has name string.Empty)
                if (o.Name != string.Empty)
                {
                    if (o.ParentName != null && StatsOptionsDict.TryGetValue(o.ParentName, out var po))
                        o.ParentOptions = po;
                    else
                        o.ParentOptions = rootSettings; // root is default parent
                }

                foreach (var so in o.StatOptions)
                {
                    if (so.LevelGraphRepresentation != null || so.LevelGraphRepresentationOdd != null)
                        so.OverrideParent = true;
                    if (so.LevelGraphRepresentationOdd != null)
                        so.UseDifferentColorsForOddLevels = true;
                }
            }
        }

        /// <summary>
        /// Returns the default stat options.
        /// </summary>
        public static StatsOptions GetDefaultStatOptions(string name) => new StatsOptions
        {
            Name = name,
            StatOptions = Enumerable.Range(0, Stats.StatsCount).Select(si => new StatOptions
            {
                LevelGraphRepresentation = LevelGraphRepresentation.GetDefaultValue
            }).ToArray(),
            ParentOptions = StatsOptionsDict.TryGetValue(string.Empty, out var p) ? p : null
        };

        /// <summary>
        /// Save stats options to the settings file.
        /// </summary>
        public static void SaveSettings()
        {
            var filePath = FileService.GetJsonPath("statOptions.json");

            // set parent names and clear settings not used
            foreach (var o in StatsOptionsDict.Values)
            {
                if (o.ParentOptions?.Name != o.Name)
                    o.ParentName = o.ParentOptions?.Name;
                else
                    o.ParentName = null; // don't save direct loop
                foreach (var so in o.StatOptions)
                {
                    if (!so.OverrideParent)
                    {
                        so.LevelGraphRepresentation = null;
                        so.LevelGraphRepresentationOdd = null;
                    }
                    else if (!so.UseDifferentColorsForOddLevels)
                        so.LevelGraphRepresentationOdd = null;
                }
            }

            FileService.SaveJsonFile(filePath, StatsOptionsDict, out var errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                MessageBoxes.ShowMessageBox(errorMessage);
        }

        /// <summary>
        /// Returns the stats options for a species.
        /// </summary>
        public static StatsOptions GetStatsOptions(Species species)
        {
            if (species == null || StatsOptionsDict == null) return null;

            if (StatsOptionsDict.TryGetValue(species.blueprintPath, out var o)
                || StatsOptionsDict.TryGetValue(species.DescriptiveNameAndMod, out o)
                || StatsOptionsDict.TryGetValue(species.DescriptiveName, out o)
                || StatsOptionsDict.TryGetValue(species.name, out o))
                return GenerateStatsOptions(o);
            if (StatsOptionsDict.TryGetValue(string.Empty, out o)) return o; // default settings
            return null;
        }

        /// <summary>
        /// Generates StatsOptions, using the parent's options if not specified explicitly.
        /// </summary>
        private static StatsOptions GenerateStatsOptions(StatsOptions so)
        {
            var finalStatsOptions = new StatsOptions { StatOptions = new StatOptions[Stats.StatsCount] };
            var parentLine = new HashSet<StatsOptions>(); // to track possible parent loops (i.e. check if setting depends on itself)
            StatsOptions defaultOptions = null;
            for (var si = 0; si < Stats.StatsCount; si++)
            {
                var useStatsOptions = so;
                parentLine.Clear();
                while (useStatsOptions.StatOptions?[si]?.OverrideParent != true
                       && useStatsOptions.ParentOptions != null
                       && !parentLine.Contains(useStatsOptions.ParentOptions))
                {
                    useStatsOptions = useStatsOptions.ParentOptions;
                    parentLine.Add(useStatsOptions);
                }

                var statOptions = useStatsOptions.StatOptions?[si];
                if (statOptions?.LevelGraphRepresentation == null)
                {
                    if (defaultOptions == null && !StatsOptionsDict.TryGetValue(string.Empty, out defaultOptions))
                        throw new Exception("no default stats options found");
                    statOptions = defaultOptions.StatOptions[si];
                }

                finalStatsOptions.StatOptions[si] = statOptions;
            }

            return finalStatsOptions;
        }
    }
}

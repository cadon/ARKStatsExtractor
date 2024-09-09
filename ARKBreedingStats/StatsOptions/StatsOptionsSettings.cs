using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARKBreedingStats.species;
using ARKBreedingStats.StatsOptions.TopStatsSettings;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.StatsOptions
{
    /// <summary>
    /// Base access to all stats options of one kind, e.g. all level color options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StatsOptionsSettings<T> where T : StatOptionsBase
    {
        public Dictionary<string, StatsOptions<T>> StatsOptionsDict;

        public event Action SettingsChanged;

        /// <summary>
        /// List of cached stat options in species.
        /// </summary>
        private readonly Dictionary<string, StatsOptions<T>> _cache = new Dictionary<string, StatsOptions<T>>();

        /// <summary>
        /// Name of the settings file.
        /// </summary>
        private readonly string _settingsFileName;

        /// <summary>
        /// Descriptive name of these settings.
        /// </summary>
        public readonly string SettingsName;

        public string SettingsFilePath => FileService.GetJsonPath(_settingsFileName);

        public StatsOptionsSettings(string settingsFileName, string settingsName)
        {
            _settingsFileName = settingsFileName;
            SettingsName = settingsName;
            LoadSettings(settingsFileName);
        }

        /// <summary>
        /// Load stats options from the settings file.
        /// </summary>
        public void LoadSettings(string settingsFileName)
        {
            if (string.IsNullOrEmpty(settingsFileName)) return;
            var filePath = FileService.GetJsonPath(_settingsFileName);

            string errorMessage = null;
            if (!File.Exists(filePath)
                || !FileService.LoadJsonFile(filePath, out StatsOptionsDict, out errorMessage))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    MessageBoxes.ShowMessageBox(errorMessage);
                StatsOptionsDict = new Dictionary<string, StatsOptions<T>>();
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
                    so.Initialize();
                }
            }
        }

        /// <summary>
        /// Returns the default stat options and set the name according the parameter.
        /// </summary>
        public StatsOptions<T> GetDefaultStatOptions(string name)
        {
            T[] statOptions;

            if (typeof(T) == typeof(StatLevelColors))
            {
                statOptions = Enumerable.Range(0, Stats.StatsCount)
                    .Select(si => StatLevelColors.GetDefault() as T).ToArray();
            }
            else if (typeof(T) == typeof(ConsiderTopStats))
            {
                var statIndicesToConsiderDefault = new[] { Stats.Health, Stats.Stamina, Stats.Weight, Stats.MeleeDamageMultiplier };
                statOptions = Enumerable.Range(0, Stats.StatsCount)
                    .Select(si => new ConsiderTopStats { OverrideParent = true, ConsiderStat = statIndicesToConsiderDefault.Contains(si) } as T).ToArray();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unknown type {typeof(T)}, no default value defined");
            }

            return new StatsOptions<T>
            {
                Name = name,
                StatOptions = statOptions,
                ParentOptions = StatsOptionsDict.TryGetValue(string.Empty, out var p) ? p : null
            };
        }

        /// <summary>
        /// Save stats options to the settings file.
        /// </summary>
        public void SaveSettings()
        {
            if (string.IsNullOrEmpty(_settingsFileName)) return;

            var filePath = FileService.GetJsonPath(_settingsFileName);

            // set parent names and clear settings not used
            foreach (var o in StatsOptionsDict.Values)
            {
                if (o.ParentOptions?.Name != o.Name)
                    o.ParentName = o.ParentOptions?.Name;
                else
                    o.ParentName = null; // don't save direct loop
                foreach (var so in o.StatOptions)
                {
                    so.PrepareForSaving(string.IsNullOrEmpty(o.Name));
                }
            }

            FileService.SaveJsonFile(filePath, StatsOptionsDict, out var errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                MessageBoxes.ShowMessageBox(errorMessage);
        }

        /// <summary>
        /// Returns the stats options for a species.
        /// </summary>
        public StatsOptions<T> GetStatsOptions(Species species)
        {
            if (string.IsNullOrEmpty(species?.blueprintPath) || StatsOptionsDict == null) return null;

            if (_cache.TryGetValue(species.blueprintPath, out var o)) return o;

            StatsOptions<T> speciesStatsOptions;

            var dict = new Dictionary<string, StatsOptions<T>>();
            var list = StatsOptionsDict
                .Where(kv => kv.Value.AffectedSpecies != null)
                .SelectMany(kv => kv.Value.AffectedSpecies.Select(sp => (sp, kv.Value)));
            foreach (var sp in list)
                dict[sp.sp] = sp.Value;

            if (dict.TryGetValue(species.blueprintPath, out o)
                || dict.TryGetValue(species.DescriptiveNameAndMod, out o)
                || dict.TryGetValue(species.DescriptiveName, out o)
                || dict.TryGetValue(species.name, out o))
            {
                speciesStatsOptions = GenerateStatsOptions(o);
            }
            else if (StatsOptionsDict.TryGetValue(string.Empty, out o))
            {
                speciesStatsOptions = o; // default settings
            }
            else
            {
                // error, no default settings available
                return null;
            }

            _cache[species.blueprintPath] = speciesStatsOptions;
            return speciesStatsOptions;
        }

        /// <summary>
        /// Generates StatsOptions, using the parent's options if not specified explicitly.
        /// </summary>
        private StatsOptions<T> GenerateStatsOptions(StatsOptions<T> so)
        {
            var finalStatsOptions = new StatsOptions<T> { StatOptions = new T[Stats.StatsCount] };
            var parentLine = new HashSet<StatsOptions<T>>(); // to track possible parent loops (i.e. check if setting depends on itself)
            StatsOptions<T> defaultOptions = null;
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
                if (statOptions?.DefinesData() != true)
                {
                    if (defaultOptions == null && !StatsOptionsDict.TryGetValue(string.Empty, out defaultOptions))
                        throw new Exception("no default stats options found");
                    statOptions = defaultOptions.StatOptions[si];
                }

                finalStatsOptions.StatOptions[si] = statOptions;
            }

            return finalStatsOptions;
        }

        /// <summary>
        /// Clear species cache when settings were changed.
        /// </summary>
        public void ClearSpeciesCache()
        {
            _cache.Clear();
            SettingsChanged?.Invoke();
        }
    }
}

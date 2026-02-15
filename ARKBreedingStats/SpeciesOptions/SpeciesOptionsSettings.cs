using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARKBreedingStats.species;
using ARKBreedingStats.SpeciesOptions.ColorSettings;
using ARKBreedingStats.SpeciesOptions.LevelColorSettings;
using ARKBreedingStats.SpeciesOptions.TopStatsSettings;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.SpeciesOptions
{
    /// <summary>
    /// Base access to all species options of one kind, e.g. all level color options.
    /// </summary>
    /// <typeparam name="T">Type of single setting element</typeparam>
    /// <typeparam name="U">Type of species settings kind (e.g. stats, colors)</typeparam>
    public class SpeciesOptionsSettings<T, U>
        where T : SpeciesOptionBase
        where U : SpeciesOptionsBase<T>, new()
    {
        public Dictionary<string, U> SpeciesOptionsDict;

        public event Action SettingsChanged;

        /// <summary>
        /// List of cached species options in species.
        /// </summary>
        private readonly Dictionary<string, U> _cache = new Dictionary<string, U>();

        /// <summary>
        /// Name of the settings file.
        /// </summary>
        private readonly string _settingsFileName;

        /// <summary>
        /// Descriptive name of these settings.
        /// </summary>
        public readonly string SettingsName;

        public string SettingsFilePath => FileService.GetJsonPath(_settingsFileName);

        public SpeciesOptionsSettings(string settingsFileName, string settingsName)
        {
            _settingsFileName = settingsFileName;
            SettingsName = settingsName;
            LoadSettings();
        }

        /// <summary>
        /// Load species options from the settings file.
        /// </summary>
        public void LoadSettings()
        {
            if (string.IsNullOrEmpty(_settingsFileName)) return;
            var filePath = FileService.GetJsonPath(_settingsFileName);

            string errorMessage = null;
            if (!File.Exists(filePath)
                || !FileService.LoadJsonFile(filePath, out SpeciesOptionsDict, out errorMessage))
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    MessageBoxes.ShowMessageBox(errorMessage);
                SpeciesOptionsDict = new Dictionary<string, U>();
            }

            // default value
            if (!SpeciesOptionsDict.ContainsKey(string.Empty))
                SpeciesOptionsDict[string.Empty] = GetDefaultSpeciesOptions(string.Empty);
            var rootSettings = SpeciesOptionsDict[string.Empty];

            // ensure root setting has all values
            for (var i = 0; i < rootSettings.Options.Length; i++)
            {
                if (rootSettings.Options[i] == null)
                    rootSettings.Options[i] = GetDefaultOption();
            }

            foreach (var o in SpeciesOptionsDict.Values)
            {
                // set parent except for root (root has name string.Empty)
                if (o.Name != string.Empty)
                {
                    if (o.ParentName != null && SpeciesOptionsDict.TryGetValue(o.ParentName, out var po))
                        o.ParentOptions = po;
                    else
                        o.ParentOptions = rootSettings; // root is default parent
                }

                if (o.Options != null)
                {
                    foreach (var so in o.Options)
                        so?.Initialize();
                }
            }
        }

        /// <summary>
        /// Returns the default species options of type T and set the name according the parameter.
        /// </summary>
        public U GetDefaultSpeciesOptions(string name)
        {
            T[] speciesOptionElements;

            if (typeof(T) == typeof(StatLevelColors))
            {
                speciesOptionElements = StatLevelColors.GetDefaultOptions() as T[];
            }
            else if (typeof(T) == typeof(ConsiderTopStats))
            {
                speciesOptionElements = ConsiderTopStats.GetDefaultOptions() as T[];
            }
            else if (typeof(T) == typeof(WantedRegionColors))
            {
                speciesOptionElements = WantedRegionColors.GetDefaultOptions() as T[];
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unknown type {typeof(T)}, no default value defined");
            }

            return new U
            {
                Name = name,
                Options = speciesOptionElements,
                ParentOptions = SpeciesOptionsDict.TryGetValue(string.Empty, out var p) ? p : null
            };
        }

        public T GetDefaultOption()
        {
            if (typeof(T) == typeof(StatLevelColors))
                return StatLevelColors.GetDefault() as T;

            if (typeof(T) == typeof(ConsiderTopStats))
                return ConsiderTopStats.GetDefault() as T;

            if (typeof(T) == typeof(WantedRegionColors))
                return WantedRegionColors.GetDefault() as T;

            throw new ArgumentOutOfRangeException($"Unknown type {typeof(T)}, no default value defined");
        }

        /// <summary>
        /// Save species options to the settings file.
        /// </summary>
        public void SaveSettings()
        {
            if (string.IsNullOrEmpty(_settingsFileName)) return;

            var filePath = FileService.GetJsonPath(_settingsFileName);

            // set parent names and clear settings not used
            foreach (var o in SpeciesOptionsDict.Values)
            {
                if (o.ParentOptions?.Name != o.Name)
                    o.ParentName = o.ParentOptions?.Name;
                else
                    o.ParentName = null; // don't save direct loop
                foreach (var so in o.Options)
                    so?.PrepareForSaving(string.IsNullOrEmpty(o.Name));
            }

            FileService.SaveJsonFile(filePath, SpeciesOptionsDict, out var errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                MessageBoxes.ShowMessageBox(errorMessage);
        }

        /// <summary>
        /// Returns the options for a species.
        /// </summary>
        public U GetOptions(Species species)
        {
            if (string.IsNullOrEmpty(species?.blueprintPath) || SpeciesOptionsDict == null) return null;

            if (_cache.TryGetValue(species.blueprintPath, out var o)) return o;

            U speciesOptions;

            var dict = new Dictionary<string, U>();
            var list = SpeciesOptionsDict
                .Where(kv => kv.Value.AffectedSpecies != null)
                .SelectMany(kv => kv.Value.AffectedSpecies.Select(sp => (sp, kv.Value)));
            foreach (var sp in list)
                dict[sp.sp] = sp.Value;

            if (dict.TryGetValue(species.blueprintPath, out o)
                || dict.TryGetValue(species.DescriptiveNameAndMod, out o)
                || dict.TryGetValue(species.DescriptiveName, out o)
                || dict.TryGetValue(species.name, out o))
            {
                speciesOptions = GenerateSpeciesOptions(o);
            }
            else if (SpeciesOptionsDict.TryGetValue(string.Empty, out o))
            {
                speciesOptions = o; // default settings
            }
            else
            {
                // error, no default settings available
                return null;
            }

            _cache[species.blueprintPath] = speciesOptions;
            return speciesOptions;
        }

        /// <summary>
        /// Generates SpeciesOptions, using the parent's options if not specified explicitly.
        /// </summary>
        private U GenerateSpeciesOptions(U so)
        {
            var finalSpeciesOptions = new U();
            var optionsCount = finalSpeciesOptions.Options.Length;
            var parentLine = new HashSet<U>(); // to track possible parent loops (i.e. check if setting depends on itself)
            U defaultOptions = null;
            for (var oi = 0; oi < optionsCount; oi++)
            {
                var useSpeciesOptions = so;
                parentLine.Clear();
                while (useSpeciesOptions.Options?[oi]?.OverrideParent != true
                       && useSpeciesOptions.ParentOptions != null
                       && !parentLine.Contains(useSpeciesOptions.ParentOptions))
                {
                    useSpeciesOptions = (U)useSpeciesOptions.ParentOptions;
                    parentLine.Add(useSpeciesOptions);
                }

                var speciesOptions = useSpeciesOptions.Options?[oi];
                if (speciesOptions?.DefinesData() != true)
                {
                    if (defaultOptions == null && !SpeciesOptionsDict.TryGetValue(string.Empty, out defaultOptions))
                        throw new Exception($"no default options found for type {typeof(U)}");
                    speciesOptions = defaultOptions.Options[oi];
                }

                finalSpeciesOptions.Options[oi] = speciesOptions;
            }

            return finalSpeciesOptions;
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

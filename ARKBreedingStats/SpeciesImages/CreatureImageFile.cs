using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Handles species image files, e.g. file names.
    /// </summary>
    internal static class CreatureImageFile
    {
        private const string Extension = ".png";
        private static string _imgCacheFolderPath;

        /// <summary>
        /// Tasks to retrieve images. This avoids simultaneous retrieval of equal images on different threads.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Lazy<Task<(string, string)>>> RetrievalTasks =
            new ConcurrentDictionary<string, Lazy<Task<(string, string)>>>();

        /// <summary>
        /// Cache of color images (file paths, file name without extension) for species including optional parameters game, sex and pattern.
        /// </summary>
        private static readonly Dictionary<string, (string, string)> CachedSpeciesFilePaths = new Dictionary<string, (string, string)>();

        /// <summary>
        /// Returns the (file path,imageBasePath) of the image used for the species. E.g. parts like Brute are removed, they share the same graphics.
        /// There are optional properties (fixed order): game (ASE/ASA), sex (f/m), pattern (id) for the file name.
        /// If the file is not available locally or outdated, it's downloaded.
        /// If the file is not available locally nor remotely, null is returned.
        /// </summary>
        internal static async Task<(string filePath, string fileBaseName)> SpeciesImageFilePath(Species species, string game = null, bool replacePolar = true) => await
            SpeciesImageFilePath(species, game ?? (species?.blueprintPath.StartsWith("/Game/ASA/") == true ? Ark.Asa : null), Sex.Unknown, (species?.patterns?.count ?? 0) > 0 ? 1 : -1, replacePolar);

        /// <summary>
        /// Returns the (file path,imageBasePath) of the image used for the species. Parts like Brute are removed, they share the same graphics.
        /// There are optional properties (fixed order): game (ASE/ASA), sex (f/m), pattern (id) for the file name.
        /// If the file is not available locally or outdated, it's downloaded.
        /// If the file is not available locally nor remotely, null is returned.
        /// </summary>
        internal static async Task<(string filePath, string fileBaseName)> SpeciesImageFilePath(Species species,
            string game = null, Sex creatureSex = Sex.Unknown,
            int patternId = -1, bool replacePolar = true)
        {
            var speciesName = species?.name;
            if (string.IsNullOrEmpty(speciesName)
                ||!ImageCollections.AnyManifests)
                return (null, null);
            speciesName = speciesName.Replace("Brute ", string.Empty);
            if (replacePolar)
                speciesName = speciesName.Replace("Polar Bear", "Dire Bear").Replace("Polar ", string.Empty);

            // the file name has the following pattern <species>[_<game|modId>][_s(m|f)][_<pattern>]

            var modString = string.IsNullOrEmpty(species.Mod?.id) ? string.Empty : "_" + species.Mod.id;
            var gameString = !string.IsNullOrEmpty(modString) ? modString :
                string.IsNullOrEmpty(game) ? string.Empty : "_" + game;
            var sexString = creatureSex == Sex.Female ? "_sf" : creatureSex == Sex.Male ? "_sm" : string.Empty;
            var patternString = patternId >= 0 ? "_" + patternId : string.Empty;
            var keyString = speciesName + gameString + sexString + patternString;
            if (CachedSpeciesFilePaths?.TryGetValue(keyString, out var filePathAndFileNameBase) == true)
                return filePathAndFileNameBase;

            // create ordered list of possible files, take first existing file (most specific). If pattern is given, it must be included.
            var possibleFileNames = GetPossibleSpeciesNames(speciesName);

            // fallback for aberrant species to use the vanilla one if no aberrant image is available (they're pretty similar)
            if (speciesName.StartsWith("Aberrant "))
                possibleFileNames.AddRange(GetPossibleSpeciesNames(speciesName.Replace("Aberrant ", string.Empty)));
            possibleFileNames = possibleFileNames.Distinct().ToList();

            return await GetImagePathAsync(keyString, possibleFileNames);

            List<string> GetPossibleSpeciesNames(string spName) => new List<string>
            {
                spName + gameString + sexString + patternString,
                spName + gameString + patternString,
                spName + sexString + patternString,
                spName + patternString
            };
        }

        private static Task<(string filePath, string fileBaseName)> GetImagePathAsync(
            string speciesPropertiesKeyString, List<string> possibleFileNames)
        {
            var lazyTask = RetrievalTasks.GetOrAdd(speciesPropertiesKeyString, new Lazy<Task<(string, string)>>(() => GetImagePathSingle(speciesPropertiesKeyString, possibleFileNames)));
            return lazyTask.Value;
        }

        private static async Task<(string filePath, string fileBaseName)> GetImagePathSingle(string speciesPropertiesKeyString, List<string> possibleFileNames)
        {
            try
            {
                foreach (var fileNameBase in possibleFileNames)
                {
                    var filePath = await ImageCollections.GetFile(fileNameBase + Extension);

                    if (File.Exists(filePath))
                    {
                        var filePathAndFileNameBase = (filePath, fileNameBase);
                        CachedSpeciesFilePaths[speciesPropertiesKeyString] = filePathAndFileNameBase;
                        // file exists, check if according mask file exists and get it or update it
                        await ImageCollections.GetFile(fileNameBase + "_m" + Extension);

                        return filePathAndFileNameBase;
                    }
                }

                // file for that species properties does not exist
                CachedSpeciesFilePaths.Add(speciesPropertiesKeyString, (null, null));
                return (null, null);
            }
            finally
            {
                RetrievalTasks.TryRemove(speciesPropertiesKeyString, out _);
            }
        }

        /// <summary>
        /// Returns the image file path to the image with the according colorization.
        /// For the listView the color ids will be ignored.
        /// </summary>
        /// <param name="speciesProperties">String unique to a species and possible other properties like game, mod, sex.</param>
        internal static string ColoredCreatureCacheFilePath(string speciesProperties, byte[] colorIds, bool listView = false)
            => Path.Combine(_imgCacheFolderPath, speciesProperties.Substring(0, Math.Min(speciesProperties.Length, 5)) + "_"
                + Convert32.ToBase32String((listView ? new byte[] { 0 } : colorIds.Select(ci => ci)).Concat(Encoding.UTF8.GetBytes(speciesProperties)).ToArray()).Replace('/', '-')
                + (listView ? "_lv" : string.Empty) + Extension);

        /// <summary>
        /// Checks if an according species image exists in the cache folder, if not it tries to create one. Returns false if there's no image.
        /// </summary>
        internal static async Task<(bool imageExists, string imagePathFinal)> GetSpeciesImageForSpeciesList(Species species, byte[] colorIds, string game = null)
        {
            var (speciesImageFilePath, fileBaseName) = await SpeciesImageFilePath(species, game, true);
            if (speciesImageFilePath == null) return (false, null);

            var cacheFilePath = string.IsNullOrEmpty(fileBaseName)
                ? null
                : ColoredCreatureCacheFilePath(fileBaseName, colorIds, true);
            if (!string.IsNullOrEmpty(cacheFilePath) && File.Exists(cacheFilePath))
                return (true, cacheFilePath);

            if (CreatureColored.CreateAndSaveCacheSpeciesFile(colorIds,
                    species?.EnabledColorRegions,
                    speciesImageFilePath, MaskFilePath(speciesImageFilePath), cacheFilePath, 64))
                return (true, cacheFilePath);

            return (false, null);
        }

        /// <summary>
        /// Returns the file path of the mask file of a given base image file path.
        /// The file might not exist.
        /// </summary>
        internal static string MaskFilePath(string baseImageFilePath) =>
            string.IsNullOrEmpty(baseImageFilePath) ? null : Path.Combine(Path.GetDirectoryName(baseImageFilePath),
                Path.GetFileNameWithoutExtension(baseImageFilePath) + "_m" + Path.GetExtension(baseImageFilePath));

        /// <summary>
        /// Deletes all cached species color images with a specific pattern that weren't used for some time.
        /// </summary>
        /// <param name="clearAllCacheFiles">If true all cached images are cleared, use if the image packs were changed. If false, only unused images files are cleared.</param>
        internal static void CleanupCache(bool clearAllCacheFiles = false)
        {
            if (string.IsNullOrEmpty(_imgCacheFolderPath) || !Directory.Exists(_imgCacheFolderPath)) return;

            DirectoryInfo directory = new DirectoryInfo(_imgCacheFolderPath);
            var oldCacheFiles = clearAllCacheFiles ? directory.GetFiles() : directory.GetFiles().Where(f => f.LastAccessTime < DateTime.Now.AddDays(-7)).ToArray();
            foreach (var f in oldCacheFiles)
            {
                FileService.TryDeleteFile(f);
            }
            CachedSpeciesFilePaths?.Clear();
        }

        /// <summary>
        /// If the setting ImgCacheUseLocalAppData is true, the image cache files are saved in the %localAppData% folder instead of the app folder.
        /// This is always true if the app is installed.
        /// Call this method after that setting or the speciesImageFolder where the images are stored are changed.
        /// The reason to use the appData folder is that this folder is used to save files, the portable version can be shared and be write protected.
        /// </summary>
        internal static void InitializeSpeciesImageLocation()
        {
            _imgCacheFolderPath = GetImgCacheFolderPath();
        }

        private static string GetImgCacheFolderPath() => FileService.GetPath(FileService.ImageFolderName, FileService.CacheFolderName,
            useAppData: Updater.Updater.IsProgramInstalled || Properties.Settings.Default.ImgCacheUseLocalAppData);
    }
}

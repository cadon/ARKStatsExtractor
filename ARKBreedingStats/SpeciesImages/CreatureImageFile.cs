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
        private static readonly ConcurrentDictionary<string, Lazy<Task<string>>> RetrievalTasks =
            new ConcurrentDictionary<string, Lazy<Task<string>>>();

        /// <summary>
        /// Tasks to generate composed images. This avoids simultaneous retrieval of equal images on different threads.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Lazy<Task<string>>> CompositionTasks =
            new ConcurrentDictionary<string, Lazy<Task<string>>>();

        /// <summary>
        /// Cache of color images file paths for species including optional parameters game, sex and pattern.
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> CachedSpeciesFilePaths = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Returns the file path of the image used for the species. E.g. parts like Brute are removed, they share the same graphics.
        /// There are optional properties (fixed order): game (ASE/ASA), sex (f/m), pattern (id) for the file name.
        /// If the file is not available locally or outdated, it's downloaded.
        /// If the file is not available locally nor remotely, null is returned.
        /// </summary>
        internal static async Task<string> SpeciesImageFilePath(Species species, string game = null, bool replacePolar = true) => await
            SpeciesImageFilePath(species, game ?? (species?.blueprintPath.StartsWith("/Game/ASA/") == true ? Ark.Asa : null),
                Sex.Unknown, (species?.patterns?.count ?? 0) > 0 ? 1 : -1, replacePolar);

        /// <summary>
        /// Returns the file path of the image used for the species. Parts like Brute are removed, they share the same graphics.
        /// There are optional properties (fixed order): game (ASE/ASA), sex (f/m), pattern (id) for the file name.
        /// If the file is not available locally or outdated, it's downloaded.
        /// If the file is not available locally nor remotely, null is returned.
        /// </summary>
        internal static async Task<string> SpeciesImageFilePath(Species species,
            string game = null, Sex creatureSex = Sex.Unknown,
            int patternId = -1, bool replacePolar = true, string imagePackName = null, string imageName = null,
            bool useComposition = false)
        {
            var speciesName = species?.name;
            if (string.IsNullOrEmpty(speciesName)
                || !ImageCollections.AnyManifests)
                return null;
            speciesName = speciesName.Replace("Brute ", string.Empty);
            if (replacePolar)
                speciesName = speciesName.Replace("Polar Bear", "Dire Bear").Replace("Polar ", string.Empty);
            var creatureImageParameters = new CreatureImageParameters(species, speciesName, game, creatureSex, patternId);

            return await SpeciesImageFilePath(creatureImageParameters, imagePackName, imageName, useComposition).ConfigureAwait(false);
        }

        private static async Task<string> SpeciesImageFilePath(CreatureImageParameters creatureImageParameters,
            string imagePackName = null, string imageName = null, bool useComposition = false)
        {

            var composition = useComposition ? ImageCompositions.GetComposition(creatureImageParameters.Species) : null;
            var compositionHash = "_" + composition?.Hash ?? string.Empty;
            if (composition != null)
            {
                imagePackName = null;
                imageName = null;
            }
            var preferredResources = "_" + imagePackName + "_" + imageName;

            var keyString = creatureImageParameters.BaseParameters + preferredResources + compositionHash;
            if (CachedSpeciesFilePaths?.TryGetValue(keyString, out var filePathAndFileNameBase) == true)
                return filePathAndFileNameBase;

            // if request needs a specific image, only try to get that
            if (!string.IsNullOrEmpty(imageName))
                return await GetImagePathAsync(keyString, new List<string> { imageName }, imagePackName).ConfigureAwait(false);

            if (composition != null)
            {
                var filePath = await CreateCompositionBaseFiles(keyString, composition, creatureImageParameters);
                if (filePath != null)
                    return filePath;
            }

            // create ordered list of possible files, take first existing file (most specific). If pattern is given, it must be included.
            var possibleFileNames = creatureImageParameters.GetPossibleSpeciesImageNames(creatureImageParameters.SpeciesName);

            // fallback for aberrant species to use the vanilla one if no aberrant image is available (they're pretty similar)
            if (creatureImageParameters.SpeciesName.StartsWith("Aberrant "))
                possibleFileNames.AddRange(creatureImageParameters.GetPossibleSpeciesImageNames(creatureImageParameters.SpeciesName.Replace("Aberrant ", string.Empty)));
            possibleFileNames = possibleFileNames.Distinct().ToList();

            return await GetImagePathAsync(keyString, possibleFileNames, imagePackName).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns file path of composed base image.
        /// </summary>
        private static Task<string> CreateCompositionBaseFiles(string speciesPropertiesKeyString, ImageComposition composition,
            CreatureImageParameters creatureImageParameters) =>
            CompositionTasks.GetOrAdd(speciesPropertiesKeyString, new Lazy<Task<string>>(()
                => CreateCompositionBaseFilesOnceAsync(speciesPropertiesKeyString, composition, creatureImageParameters))).Value;

        private static async Task<string> CreateCompositionBaseFilesOnceAsync(string speciesPropertiesKeyString, ImageComposition composition,
            CreatureImageParameters creatureImageParameters)
        {
            if (composition == null || composition.Hash == 0 || composition.Parts?.Any() != true)
                return null;

            var filePathResult = FilePathCombinedSpeciesImage(FileService.ReplaceInvalidCharacters(speciesPropertiesKeyString));
            if (File.Exists(filePathResult))
            {
                CachedSpeciesFilePaths[speciesPropertiesKeyString] = filePathResult;
                return filePathResult;
            }

            var tasks = composition.Parts.Select(async part =>
                await SpeciesImageFilePath(creatureImageParameters, part.ImagePackName, part.ImageName)
            ).ToArray();

            var filePaths = await Task.WhenAll(tasks).ConfigureAwait(false);

            if (filePaths.Any(f => f == null)) return null;
            if (composition.CombineImages(filePaths, filePathResult))
            {
                CachedSpeciesFilePaths[speciesPropertiesKeyString] = filePathResult;
                return filePathResult;
            }

            CachedSpeciesFilePaths[speciesPropertiesKeyString] = null;
            return null;
        }

        private static Task<string> GetImagePathAsync(
            string speciesPropertiesKeyString, List<string> possibleFileNames, string imagePackName = null) =>
            RetrievalTasks.GetOrAdd(speciesPropertiesKeyString, new Lazy<Task<string>>(()
                => GetImagePathOnceAsync(speciesPropertiesKeyString, possibleFileNames, imagePackName))).Value;

        private static async Task<string> GetImagePathOnceAsync(string speciesPropertiesKeyString, List<string> possibleFileNames, string imagePackName = null)
        {
            try
            {
                foreach (var fileNameBase in possibleFileNames)
                {
                    var filePath = await ImageCollections.GetFile(fileNameBase + Extension, imagePackName).ConfigureAwait(false);

                    if (File.Exists(filePath))
                    {
                        CachedSpeciesFilePaths[speciesPropertiesKeyString] = filePath;
                        // file exists, check if according mask file exists and get it or update it
                        await ImageCollections.GetFile(fileNameBase + "_m" + Extension).ConfigureAwait(false);

                        return filePath;
                    }
                }

                // file for that species properties does not exist
                CachedSpeciesFilePaths[speciesPropertiesKeyString] = null;
                return null;
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
        /// File path of combined image.
        /// </summary>
        private static string FilePathCombinedSpeciesImage(string speciesProperties) =>
            Path.Combine(_imgCacheFolderPath, "comp_" + speciesProperties + Extension);

        /// <summary>
        /// Checks if an according species image exists in the cache folder, if not it tries to create one. Returns false if there's no image.
        /// </summary>
        internal static async Task<string> GetSpeciesImageForSpeciesList(Species species, byte[] colorIds, string game = null)
        {
            var speciesImageFilePath = await SpeciesImageFilePath(species, game, true);
            if (speciesImageFilePath == null) return null;

            var cacheFilePath = ColoredCreatureCacheFilePath(Path.GetFileNameWithoutExtension(speciesImageFilePath), colorIds, true);
            if (!string.IsNullOrEmpty(cacheFilePath) && File.Exists(cacheFilePath))
                return cacheFilePath;

            if (CreatureColored.CreateAndSaveCacheSpeciesFile(colorIds,
                    species?.EnabledColorRegions,
                    speciesImageFilePath, MaskFilePath(speciesImageFilePath), cacheFilePath, 64))
                return cacheFilePath;

            return null;
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

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
    public static class CreatureImageFile
    {
        internal const string FileExtension = ".png";
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
        /// Cache of base image file paths for species including optional parameters like game, sex, pattern and pose.
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> CachedSpeciesFilePaths = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Cache of possibly existing neighboring pose images for species including optional parameters like game, sex, pattern and pose.
        /// </summary>
        private static readonly ConcurrentDictionary<string, NeighbourPoseExist> CachedSpeciesNeighborPoseExists = new ConcurrentDictionary<string, NeighbourPoseExist>();

        /// <summary>
        /// Preferred image packs per species. Key is blueprint path or species name.
        /// </summary>
        private static Dictionary<string, string> _preferImagePacks;

        /// <summary>
        /// Returns the file path of the image used for the species.
        /// If the file is not available locally or outdated, it's downloaded.
        /// If the file is not available locally nor remotely, null is returned.
        /// </summary>
        internal static async Task<(string FilePath, NeighbourPoseExist NeighbourPoseExist)> SpeciesImageFilePath(Species species,
            string game = null, Sex creatureSex = Sex.Unknown,
            int patternId = -1, string imagePackName = null, string imageName = null,
            bool useComposition = false, int pose = 0)
        {
            if (string.IsNullOrEmpty(species?.name)
                || !ImageCollections.AnyManifests)
                return (null, NeighbourPoseExist.None);
            var creatureImageParameters = new CreatureImageParameters(species, game, creatureSex, patternId, pose);

            return await SpeciesImageFilePath(creatureImageParameters, imagePackName, imageName, useComposition).ConfigureAwait(false);
        }

        private static async Task<(string FilePath, NeighbourPoseExist NeighbourPoseExist)> SpeciesImageFilePath(CreatureImageParameters creatureImageParameters,
            string imagePackName = null, string imageName = null, bool useComposition = false)
        {
            var composition = useComposition ? ImageCompositions.GetComposition(creatureImageParameters.Species) : null;
            var compositionHash = "_" + (composition?.Hash.ToString() ?? string.Empty);
            if (composition != null)
            {
                imagePackName = null;
                imageName = null;
            }
            var preferredResources = "_" + imagePackName + "_" + imageName;

            var keyString = creatureImageParameters.BaseParameters + preferredResources + compositionHash;
            NeighbourPoseExist neighbourPoseExist;
            if (CachedSpeciesFilePaths.TryGetValue(keyString, out var filePathAndFileNameBase))
                return (filePathAndFileNameBase,
                    CachedSpeciesNeighborPoseExists.TryGetValue(keyString, out neighbourPoseExist) ? neighbourPoseExist : NeighbourPoseExist.None);

            // if request needs a specific image, only try to get that
            if (!string.IsNullOrEmpty(imageName))
                return (await GetImagePathAsync(keyString, new[] { imageName }, imagePackName).ConfigureAwait(false), NeighbourPoseExist.None);

            if (composition != null)
            {
                var filePath = await CreateCompositionBaseFiles(keyString, composition, creatureImageParameters);
                if (filePath != null)
                    return (filePath, NeighbourPoseExist.None);
            }

            // user may prefer an image pack for a species
            if (imagePackName == null)
                imagePackName = UserPreferenceImagePack(creatureImageParameters.Species);

            // create ordered list of possible files, take first existing file (most specific). If pattern is given, it must be included.
            var possibleFileNames = creatureImageParameters.GetPossibleSpeciesImageNamesWithFallbacks();

            // file names for possible pose neighbours
            var possibleFileNamesPreviousPose = creatureImageParameters.PoseId < 1
                ? null
                : new CreatureImageParameters(creatureImageParameters, creatureImageParameters.PoseId - 1).GetPossibleSpeciesImageNamesWithFallbacks(true);
            var possibleFileNamesNextPose = creatureImageParameters.PoseId < 0
                ? null
                : new CreatureImageParameters(creatureImageParameters, creatureImageParameters.PoseId + 1).GetPossibleSpeciesImageNamesWithFallbacks(true);

            return (await GetImagePathAsync(keyString, possibleFileNames, imagePackName, possibleFileNamesPreviousPose, possibleFileNamesNextPose).ConfigureAwait(false),
                CachedSpeciesNeighborPoseExists.TryGetValue(keyString, out neighbourPoseExist) ? neighbourPoseExist : NeighbourPoseExist.None);
        }

        /// <summary>
        /// Check if user prefers an image pack for a species.
        /// </summary>
        private static string UserPreferenceImagePack(Species species) =>
            _preferImagePacks != null
            && (_preferImagePacks.TryGetValue(species.blueprintPath, out var ip)
                || _preferImagePacks.TryGetValue(species.name, out ip))
                ? ip
                : null;

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
                return null; // no composition

            var filePathResult = FilePathCombinedSpeciesImage(speciesPropertiesKeyString);
            if (File.Exists(filePathResult))
            {
                // composition file already exists
                CachedSpeciesFilePaths[speciesPropertiesKeyString] = filePathResult;
                return filePathResult;
            }

            var tasks = composition.Parts.Select(async part =>
                await SpeciesImageFilePath(creatureImageParameters, part.ImagePackName, part.ImageName)
            ).ToArray();

            var filePaths = (await Task.WhenAll(tasks).ConfigureAwait(false)).Select(f => f.FilePath).ToArray();

            if (filePaths.Any(f => f == null)) return null;
            if (composition.CombineImages(filePaths, filePathResult))
            {
                CachedSpeciesFilePaths[speciesPropertiesKeyString] = filePathResult;
                return filePathResult;
            }

            CachedSpeciesFilePaths[speciesPropertiesKeyString] = null;
            return null;
        }

        private static Task<string> GetImagePathAsync(string speciesPropertiesKeyString, string[] possibleFileNames, string imagePackName = null,
            string[] possibleFileNamesPreviousPose = null, string[] possibleFileNamesNextPose = null) =>
            RetrievalTasks.GetOrAdd(speciesPropertiesKeyString, new Lazy<Task<string>>(()
                => GetImagePathOnceAsync(speciesPropertiesKeyString, possibleFileNames, imagePackName, possibleFileNamesPreviousPose, possibleFileNamesNextPose))).Value;

        private static async Task<string> GetImagePathOnceAsync(string speciesPropertiesKeyString, IList<string> possibleFileNames, string imagePackName = null,
            string[] possibleFileNamesPreviousPose = null, string[] possibleFileNamesNextPose = null)
        {
            try
            {
                var (filePath, usedImagePackName) = await ImageCollections.GetFile(possibleFileNames.Select(f => f + FileExtension).ToArray(), imagePackName).ConfigureAwait(false);

                if (File.Exists(filePath))
                {
                    CachedSpeciesFilePaths[speciesPropertiesKeyString] = filePath;
                    // file exists, check if according mask file exists and get it or update it
                    var maskFileName = Path.GetFileNameWithoutExtension(filePath) + MaskFileSuffix + FileExtension;
                    await ImageCollections.GetFile(new[] { maskFileName }, usedImagePackName).ConfigureAwait(false);

                    // check if possible neighbour poses exist
                    var neighbourPosesExist = NeighbourPoseExist.None;
                    if (possibleFileNamesPreviousPose != null
                        && !string.IsNullOrEmpty(
                            (await ImageCollections.GetFile(possibleFileNamesPreviousPose.Select(f => f + FileExtension).ToArray(), imagePackName, true))
                            .FilePath))
                        neighbourPosesExist |= NeighbourPoseExist.Previous;
                    if (possibleFileNamesNextPose != null
                        && !string.IsNullOrEmpty(
                            (await ImageCollections.GetFile(possibleFileNamesNextPose.Select(f => f + FileExtension).ToArray(), imagePackName, true))
                            .FilePath))
                        neighbourPosesExist |= NeighbourPoseExist.Next;
                    CachedSpeciesNeighborPoseExists[speciesPropertiesKeyString] = neighbourPosesExist;

                    return filePath;
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
            => Path.Combine(_imgCacheFolderPath, (listView ? "lv_" : string.Empty)
                + speciesProperties.Substring(0, Math.Min(speciesProperties.Length, 5)) + "_"
                + Convert32.ToBase32String((listView ? new byte[] { 0 } : colorIds.Select(ci => ci)).Concat(Encoding.UTF8.GetBytes(speciesProperties)).ToArray()).Replace('/', '-')
                + FileExtension);

        /// <summary>
        /// File path of combined base image.
        /// </summary>
        private static string FilePathCombinedSpeciesImage(string speciesProperties) =>
            Path.Combine(_imgCacheFolderPath, FileService.ReplaceInvalidCharacters(speciesProperties) + "_comp" + FileExtension);

        /// <summary>
        /// Checks if an according species image exists in the cache folder, if not it tries to create one. Returns false if there's no image.
        /// </summary>
        internal static async Task<string> GetSpeciesImageForSpeciesList(Species species, byte[] colorIds, string game = null)
        {
            var speciesImageFilePath = (await SpeciesImageFilePath(species, game ?? (species?.blueprintPath.StartsWith("/Game/ASA/") == true ? Ark.Asa : null),
                patternId: (species?.patterns?.count ?? 0) > 0 ? 1 : -1)).FilePath;
            if (speciesImageFilePath == null) return null;

            var cacheFilePath = ColoredCreatureCacheFilePath(Path.GetFileNameWithoutExtension(speciesImageFilePath), colorIds, true);
            if (!string.IsNullOrEmpty(cacheFilePath) && File.Exists(cacheFilePath))
                return cacheFilePath;

            if (CreatureColored.CreateAndSaveCacheSpeciesFile(colorIds, species?.EnabledColorRegions,
                    speciesImageFilePath, MaskFilePath(speciesImageFilePath), cacheFilePath, outputSize: 64))
                return cacheFilePath;

            return null;
        }

        private const string MaskFileSuffix = "_m";

        /// <summary>
        /// Returns the file path of the mask file of a given base image file path.
        /// The file might not exist.
        /// </summary>
        internal static string MaskFilePath(string baseImageFilePath) =>
            string.IsNullOrEmpty(baseImageFilePath) ? null : Path.Combine(Path.GetDirectoryName(baseImageFilePath),
                Path.GetFileNameWithoutExtension(baseImageFilePath) + MaskFileSuffix + Path.GetExtension(baseImageFilePath));

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
            ClearCachedSpeciesFiles();
        }

        public const string PreferImagePackFileName = "preferImagePacks.json";

        /// <summary>
        /// Clear cached species base file paths, call after image pack change.
        /// </summary>
        internal static void ClearCachedSpeciesFiles()
        {
            CachedSpeciesFilePaths?.Clear();
            _preferImagePacks = FileService.LoadJsonFileIfAvailable<Dictionary<string, string>>(FileService.GetJsonPath(PreferImagePackFileName));
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

        /// <summary>
        /// Indicator if a neighbor pose exists. E.g. if pose 1-4 exist, pose 3 has both neighbours, pose 4 has only previous neighbour.
        /// </summary>
        [Flags]
        public enum NeighbourPoseExist : byte
        {
            None = 0,
            Previous = 1,
            Next = 2,
            Both = Previous | Next
        }
    }
}

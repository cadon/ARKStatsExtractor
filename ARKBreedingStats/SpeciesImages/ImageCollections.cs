using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ARKBreedingStats.utils;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.SpeciesImages
{
    internal static class ImageCollections
    {
        /// <summary>
        /// Key is imagePack id, value is ImageManifest.
        /// </summary>
        public static readonly Dictionary<string, ImagesManifest> ImageManifests = new Dictionary<string, ImagesManifest>();

        /// <summary>
        /// Enabled image packs, ordered.
        /// </summary>
        private static readonly List<ImageCollection> EnabledImageCollections = new List<ImageCollection>();

        /// <summary>
        /// Cache for image paths, key is fileName.
        /// </summary>
        private static readonly Dictionary<string, string> ImagePaths = new Dictionary<string, string>();

        private static string _imageBasePath;

        private const string ImagePacksFileNameBase = "imagePacks";

        /// <summary>
        /// True if any image manifests are loaded.
        /// </summary>
        public static bool AnyManifests;

        public static string ManifestFilePathOfPack(string folderNameImagePack)
            => string.IsNullOrEmpty(folderNameImagePack)
                ? null
                : Path.Combine(_imageBasePath, folderNameImagePack, ImagesManifest.FileName);

        /// <summary>
        /// Load info about available image packs.
        /// </summary>
        public static void LoadImagePackInfos()
        {
            _imageBasePath = FileService.GetPath(FileService.ImageFolderName);

            ImageManifests.Clear();

            var pathJsonFolder = FileService.GetJsonPath();
            var filesImagePacks = Directory.GetFiles(pathJsonFolder, ImagePacksFileNameBase + "*.json");

            foreach (var p in filesImagePacks)
            {
                if (!FileService.LoadJsonFile(
                        p, out ImagesManifest[] ims,
                        out var errorText))
                {
                    MessageBoxes.ShowMessageBox(
                        $"Error when trying to load image packs manifest file\n{p}\n\n{errorText}",
                        "Error when loading image packs manifest file");
                    continue;
                }

                foreach (var im in ims)
                {
                    if (string.IsNullOrEmpty(im.Id))
                    {
                        MessageBoxes.ShowMessageBox(
                            $"Error when loading the manifest of an image pack in the file\n{p}\n\nNo url and no name is given.\nRemote packs need an url, local packs need a name.",
                            "Error when loading image pack manifest");
                        continue;
                    }

                    if (ImageManifests.ContainsKey(im.Id))
                    {
                        MessageBoxes.ShowMessageBox(
                            $"Error when loading the manifest of the image pack with name\n{im.Name}\nand url\n{im.Url}\nin the file\n{p}\n\nThe id {im.Id} is already used by another image pack.",
                            "Error when loading image pack manifest");
                        continue;
                    }

                    im.FilePathOfManifest = p;
                    ImageManifests[im.Id] = im;
                }
            }

            LoadImagePackManifests();
        }

        /// <summary>
        /// Load species images collections info, call when setting was changed.
        /// </summary>
        public static void LoadImagePackManifests(bool forceUpdate = false) =>
            LoadImagePackManifestsAsync(forceUpdate).ConfigureAwait(false).GetAwaiter().GetResult();

        private static async Task LoadImagePackManifestsAsync(bool forceUpdate)
        {
            EnabledImageCollections.Clear();
            AnyManifests = false;
            ImagePaths.Clear();
            await FetchImageManifests(forceUpdate).ConfigureAwait(false);
            var enabledPacks = Properties.Settings.Default.SpeciesImagesUrls;
            var imagePacksOrdered = ImageManifests.Values
                .OrderBy(im => enabledPacks == null ? 0 : Array.IndexOf(enabledPacks, im.Id) is int i && i >= 0 ? i : int.MaxValue).ToArray();

            foreach (var im in imagePacksOrdered)
            {
                var filePathLocalManifest = ManifestFilePathOfPack(im.FolderName);
                if (string.IsNullOrEmpty(filePathLocalManifest)) continue;

                if (!File.Exists(filePathLocalManifest))
                {
                    MessageBoxes.ShowMessageBox($"Manifest file not found at\n{filePathLocalManifest}\n\nThis image pack ({im.Name}) cannot be used without a valid manifest file.",
                        "Error loading manifest file");
                    continue;
                }

                if (!FileService.LoadJsonObjectFromJsonFile(
                        filePathLocalManifest, out var manifestJson,
                        out var errorText))
                {
                    MessageBoxes.ShowMessageBox($"Error when trying to load manifest file\n{filePathLocalManifest}\n\n{errorText}",
                            "Error loading manifest file");
                    continue;
                }

                var isEnabledPack = Properties.Settings.Default.SpeciesImagesUrls?.Contains(im.Id) == true;
                im.ParseJsonManifest(manifestJson, isEnabledPack);
                if (isEnabledPack)
                {
                    EnabledImageCollections.Add(new ImageCollection(im));
                    AnyManifests = true;
                }
            }
        }

        /// <summary>
        /// Fetch latest version of the manifest files of enabled packs.
        /// </summary>
        public static async Task FetchImageManifests(bool forceUpdate = false)
        {
            if (ImageManifests == null) return;
            var enabledImagePackIds = Properties.Settings.Default.SpeciesImagesUrls;

            foreach (var ip in ImageManifests.Values)
            {
                if (string.IsNullOrEmpty(ip.Url))
                    continue; // assuming it is a local pack

                var filePathManifest = ManifestFilePathOfPack(ip.FolderName);
                if (string.IsNullOrEmpty(filePathManifest))
                {
                    MessageBoxes.ShowMessageBox($"The image pack\n{ip.Name}\ncontains no valid folder.",
                        "Error when updating image pack manifest file");
                    continue;
                }

                var packIsEnabled = enabledImagePackIds?.Contains(ip.Id) == true;

                var manifestFileInfo = new FileInfo(filePathManifest);

                // only download if file does not exist or is outdated
                var downloadFile = !manifestFileInfo.Exists
                                   || manifestFileInfo.Length == 0
                                   || (forceUpdate && packIsEnabled);
                if (!downloadFile && packIsEnabled)
                {
                    if (manifestFileInfo.LastWriteTimeUtc < DateTime.UtcNow - TimeSpan.FromDays(1))
                    {
                        var lastCommit = await LastCommit(ip.Url).ConfigureAwait(false);
                        if (lastCommit == null || manifestFileInfo.LastWriteTimeUtc < lastCommit.Value)
                            downloadFile = true; // there was a recent commit to the repo
                        else
                            File.SetLastWriteTimeUtc(manifestFileInfo.FullName, DateTime.UtcNow);
                    }
                }

                if (!downloadFile)
                    continue;

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePathManifest));
                    await WebService.DownloadAsync(ip.Url + ImagesManifest.FileName, filePathManifest).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    MessageBoxes.ExceptionMessageBox(ex, $"Error when trying to download the manifest file of the pack\n{ip.Name}",
                            "Error when updating image pack manifest file");
                }
            }
        }

        private static readonly Regex RegexGithubManifest = new Regex(@"github\.com\/([^\/]+)\/([^\/]+)\/");

        private static async Task<DateTime?> LastCommit(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            try
            {
                var m = RegexGithubManifest.Match(url);
                if (!m.Success) return null;

                var urlLastCommit =
                    $"https://api.github.com/repos/{m.Groups[1].Value}/{m.Groups[2].Value}/commits?path=_manifest.json&page=1&per_page=1";

                var (success, jsonStringLastCommit) = await WebService.DownloadAsync(urlLastCommit).ConfigureAwait(false);
                if (!success || string.IsNullOrEmpty(jsonStringLastCommit)) return null;

                var json = JArray.Parse(jsonStringLastCommit);
                return json[0]?["commit"]?["committer"]?["date"]?.ToObject<DateTime>();
            }
            catch
            {
                // ignore
            }
            return null;
        }

        /// <summary>
        /// Get file from enabled image packs.
        /// </summary>
        /// <param name="imagePackName">If specified, prefer this pack.</param>
        public static async Task<string> GetFile(string fileName, string imagePackName = null)
        {
            if (string.IsNullOrEmpty(fileName) || ImageManifests == null) return null;

            if (ImagePaths.TryGetValue(fileName, out var filePath)) return filePath;

            var checkImagePacks = string.IsNullOrEmpty(imagePackName)
                ? EnabledImageCollections
                : EnabledImageCollections.OrderBy(c => c.FolderName != imagePackName).ToList();

            foreach (var imageCollection in checkImagePacks)
            {
                var collectionFileName = await imageCollection.GetFileAsync(fileName).ConfigureAwait(false);
                if (string.IsNullOrEmpty(collectionFileName)) continue;

                filePath = Path.Combine(_imageBasePath, imageCollection.FolderName, collectionFileName);
                if (File.Exists(filePath))
                {
                    ImagePaths[fileName] = filePath;
                    return filePath;
                }
            }

            // file not available
            ImagePaths[fileName] = null;
            return null;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Represents one species image pack.
    /// </summary>
    internal class ImageCollection
    {
        private readonly ConcurrentDictionary<string, Lazy<Task<string>>> _currentDownloads =
            new ConcurrentDictionary<string, Lazy<Task<string>>>();

        /// <summary>
        /// Only allow 5 concurrent downloads.
        /// </summary>
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(5);

        /// <summary>
        /// Local folder name, should be unique.
        /// </summary>
        public string FolderName { get; private set; }

        public string Name => _manifestSource?.Name ?? FolderName;

        /// <summary>
        /// Manifest from the source, it's a downloaded file.
        /// </summary>
        private ImagesManifest _manifestSource;

        public ImageCollection(ImagesManifest manifestSource)
        {
            _manifestSource = manifestSource;
            FolderName = _manifestSource?.FolderName;
        }

        /// <summary>
        /// Returns the fileName for a species image.
        /// If it's not available locally, or if there's a newer version remote, it's downloaded.
        /// If it's not in this image collection, null is returned.
        /// </summary>
        public Task<string> GetFileAsync(string fileName)
        {
            var lazyTask = _currentDownloads.GetOrAdd(fileName, fn => new Lazy<Task<string>>(() => GetFileOnceAsync(fn)));
            return lazyTask.Value;
        }

        /// <summary>
        /// Returns the fileName for a species image.
        /// If it's not available locally, or if there's a newer version remote, it's downloaded.
        /// If it's not in this image collection, null is returned.
        /// </summary>
        private async Task<string> GetFileOnceAsync(string fileName)
        {
            try
            {
                if (_manifestSource == null)
                    await LoadManifestAsync();
                if (_manifestSource == null)
                    return null;

                if (_manifestSource.FileHashes?.TryGetValue(fileName, out var hashRemoteFile) != true)
                    return null; // file not in this image pack

                var filePathLocalImage = FileService.GetPath(FileService.ImageFolderName, FolderName, fileName);
                if (string.IsNullOrEmpty(hashRemoteFile) || string.IsNullOrEmpty(_manifestSource.Url))
                    return fileName; // file is assumed to be available locally. No info about possible remote files.
                var hashMatches = miscClasses.Encryption.FileEqualByHash(filePathLocalImage, hashRemoteFile);
                if (hashMatches == true)
                    return fileName; // file available and up to date
                if (hashMatches == null)
                    return null; // error, hash invalid or fileName empty

                // file needs to be downloaded. Limit concurrent downloads.
                Debug.WriteLine($"Waiting to download file, currently {Semaphore.CurrentCount} more downloads are allowed.");
                await Semaphore.WaitAsync();
                try
                {
                    var sw = Stopwatch.StartNew();
                    // file is in image pack but not up to date or not available locally
                    var (downloadSuccessful, _) =
                        await WebService.DownloadAsync(_manifestSource.Url + fileName, filePathLocalImage);
                    sw.Stop();
                    if (downloadSuccessful)
                    {
                        Debug.WriteLine(
                            $"Downloading file {fileName} from image pack {_manifestSource.Name} took {sw.ElapsedMilliseconds} ms. Currently {(Semaphore.CurrentCount + 1)} more concurrent downloads are allowed.");
                        return fileName;
                    }

                    Debug.WriteLine($"downloading file {fileName} from image pack {_manifestSource.Name} failed");
                    // error, file not retrievable
                    return null;
                }
                finally
                {
                    Semaphore.Release();
                }
            }
            finally
            {
                _currentDownloads.TryRemove(fileName, out _);
            }
        }

        private async Task LoadManifestAsync()
        {
            var manifestFilePath = FileService.GetPath(FileService.ImageFolderName, FolderName, ImagesManifest.FileName);
            if (!File.Exists(manifestFilePath))
            {
                await UpdateManifestAsync(manifestFilePath);
            }

            FileService.LoadJsonFile(manifestFilePath, out _manifestSource, out _);
        }

        private async Task UpdateManifestAsync(string manifestFilePath)
        {
            if (string.IsNullOrEmpty(_manifestSource.Url)) return;
            await WebService.DownloadAsync(_manifestSource.Url + ImagesManifest.FileName, manifestFilePath);
        }

        /// <summary>
        /// Returns true if file is listed in file hash list.
        /// </summary>
        public async Task<bool> IsFileInCollection(string fileName)
        {
            if (_manifestSource == null)
                await LoadManifestAsync();
            // return true if file is listed in fileHashes or if it exists locally
            return _manifestSource?.FileHashes?.ContainsKey(fileName) == true
                || File.Exists(FileService.GetPath(FileService.ImageFolderName, FolderName, fileName));
        }
    }
}

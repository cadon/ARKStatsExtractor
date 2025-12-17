using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using ARKBreedingStats.miscClasses;
using ARKBreedingStats.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.SpeciesImages
{
    /// <summary>
    /// Manifest file of a species image pack. The file name is manifest.json.
    /// </summary>
    [JsonObject]
    internal class ImagesManifest
    {
        public const string FileName = "_manifest.json";

        /// <summary>
        /// Used to identify the pack.
        /// This is the Url ?? Name.
        /// </summary>
        public string Id;

        /// <summary>
        /// Descriptive name.
        /// </summary>
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// Name of the creator of the image pack.
        /// </summary>
        [JsonProperty("creator")]
        public string Creator;

        /// <summary>
        /// Source folder of the images.
        /// The images and the manifest file should be directly in this folder.
        /// If no url is given, it is assumed to be a local image pack, the folder name then is based on the Name.
        /// </summary>
        [JsonProperty("url")]
        public string Url;

        /// <summary>
        /// Folder name of the collection.
        /// </summary>
        public string FolderName;

        /// <summary>
        /// Hashes of the species files.
        /// </summary>
        public Dictionary<string, string> FileHashes;

        /// <summary>
        /// Local file path of manifest that defines this image pack.
        /// </summary>
        public string FilePathOfManifest;

        public override string ToString() => string.IsNullOrEmpty(Name) ? Url : Name;

        /// <summary>
        /// Sets the folder name of this pack. It is based on the url for remote packs.
        /// For local packs without url only the name is used.
        /// </summary>
        [OnDeserialized]
        public void Initialize(StreamingContext _)
        {
            if (Url?.EndsWith("/") == false)
                Url += "/";

            Id = string.IsNullOrEmpty(Url) ? Name : Url;

            FolderName = string.IsNullOrEmpty(Url) ? Name : Encryption.Md5(Url);
            FolderName = FileService.ReplaceInvalidCharacters(FolderName)
                .Replace(' ', '_');
            const int maxFolderName = 32;
            if (FolderName.Length > maxFolderName)
                FolderName = FolderName.Substring(0, maxFolderName);
        }

        /// <summary>
        /// Loads local manifest file. For local packs without manifest file only the available files are checked.
        /// </summary>
        /// <returns>False on error.</returns>
        public bool LoadLocalImagePackInfo(string filePathLocalManifest, bool isEnabledPack)
        {
            if (!File.Exists(filePathLocalManifest))
            {
                return !isEnabledPack || ParseFiles(Path.GetDirectoryName(filePathLocalManifest));

                //MessageBoxes.ShowMessageBox($"Manifest file not found at\n{filePathLocalManifest}\n\nThis image pack ({Name}) cannot be used without a valid manifest file.",
                //    "Error loading manifest file");
                //return false;
            }

            if (!FileService.LoadJsonObjectFromJsonFile(
                    filePathLocalManifest, out var manifestJson,
                    out var errorText))
            {
                MessageBoxes.ShowMessageBox($"Error when trying to load manifest file\n{filePathLocalManifest}\n\n{errorText}",
                    "Error loading manifest file");
                return false;
            }

            return ParseJsonManifest(manifestJson, isEnabledPack);
        }

        /// <summary>
        /// Parses manifest, adds file hashes if pack is enabled.
        /// </summary>
        /// <returns>False on error</returns>
        private bool ParseJsonManifest(JObject json, bool addFileHashes)
        {
            try
            {
                // parse meta info
                var metaData = json["files"]?["!info.json"]?["metadata"];
                if (metaData != null)
                {
                    var name = metaData["name"]?.Value<string>();
                    if (!string.IsNullOrEmpty(name)) Name = name;
                    var description = metaData["description"]?.Value<string>();
                    if (!string.IsNullOrEmpty(description)) Description = description;
                    var creator = metaData["creator"]?.Value<string>();
                    if (!string.IsNullOrEmpty(creator)) Creator = creator;
                }

                if (!addFileHashes) return true;

                FileHashes = (json["files"] as JObject)?
                    .Properties().Where(f => f.Name != "!info.json")
                    .Select(p => (p.Name, p.Value["hash"]?.ToString()))
                    .Where(p => !string.IsNullOrEmpty(p.Name))
                    .ToDictionary(p => p.Name, p => p.Item2);
                if (FileHashes != null) return true;

                MessageBoxes.ShowMessageBox(
                       $"Error when parsing manifest file\n{ImageCollections.ManifestFilePathOfPack(FolderName)}\n\n"
                       + "The file structure needs to be a json object with a property \"files\" which contains the file names as property names.\n"
                       + "Each species image json object needs to have the structure:\n"
                       + "\"[species].png\": { \"hash\": \"md5:[fileHash]:[fileLength]\" }"
                   );
            }
            catch (Exception ex)
            {
                MessageBoxes.ExceptionMessageBox(ex,
                    $"Error when parsing manifest file\n{ImageCollections.ManifestFilePathOfPack(FolderName)}\n\n"
                    + "Each species image json object needs to have the structure:\n"
                    + "\"[species].png\": { \"hash\": \"md5:[fileHash]:[fileLength]\" }"
                    );
            }
            return false;
        }

        /// <summary>
        /// If no local manifest is available, just use the available files. No remote update check is available then.
        /// This is the intended method for local image packs.
        /// </summary>
        private bool ParseFiles(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return false;
            var files = Directory.GetFiles(directoryPath, "*" + CreatureImageFile.FileExtension);
            FileHashes = files.ToDictionary(Path.GetFileName, f => default(string));
            return true;
        }
    }
}

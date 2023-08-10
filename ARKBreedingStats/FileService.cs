using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{

    public static class FileService
    {
        private const string JsonFolder = "json";
        public const string ValuesFolder = "values";
        public const string ValuesJson = "values.json";
        public const string ValuesServerMultipliers = "serverMultipliers.json";
        public const string TamingFoodData = "tamingFoodData.json";
        public const string ManifestFileName = "_manifest.json";
        public const string ModsManifestCustom = "_manifestCustom.json";
        public const string KibblesJson = "kibbles.json";
        public const string AliasesJson = "aliases.json";
        public const string IgnoreSpeciesClasses = "ignoreSpeciesClasses.json";
        public const string CustomReplacingsNamePattern = "customReplacings.json";
        public const string CustomSpeciesVariants = "customSpeciesVariants.json";
        public const string DataFolderName = "data";
        public const string OcrReplacingsFile = "ocrReplacings.txt";
        public const string EqualColorIdsFile = "equalColorIds.json";

        /// <summary>
        /// Where the colored species images are cached.
        /// </summary>
        public const string CacheFolderName = "cache";
        public const string OcrFolderName = "ocr";

        public static readonly string ExeFilePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
        private static readonly string ExeLocation = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        /// <summary>
        /// Returns a <see cref="StreamReader"/> of a file located in the json data folder
        /// </summary>
        /// <param name="fileName">name of file to read; use FileService constants</param>
        /// <returns></returns>
        public static StreamReader GetJsonFileReader(string fileName)
        {
            return File.OpenText(GetJsonPath(fileName));
        }

        /// <summary>
        /// Gets the full path for the given filename or the path to the application data folder.
        /// If fileName2 is given, fileName is considered to be the containing folder.
        /// </summary>
        /// <param name="useAppData">If true, the %localAppData%-folder is used regardless of installed or portable version.</param>
        public static string GetPath(string fileName = null, string fileName2 = null, string fileName3 = null, bool useAppData = false)
            => Path.Combine(useAppData || Updater.Updater.IsProgramInstalled ? GetLocalApplicationDataPath() : ExeLocation, fileName ?? string.Empty, fileName2 ?? string.Empty, fileName3 ?? string.Empty);


        /// <summary>
        /// Gets the full path for the given filename or the path to the json folder.
        /// If fileName2 is given, fileName is considered to be the containing folder.
        /// </summary>
        /// <returns></returns>
        public static string GetJsonPath(string fileName = null, string fileName2 = null) =>
            GetPath(JsonFolder, fileName, fileName2);

        private static string GetLocalApplicationDataPath()
        {
            return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // C:\Users\xxx\AppData\Local\
                    Path.GetFileNameWithoutExtension(ExeFilePath) ?? "ARK Smart Breeding"); // ARK Smart Breeding;
        }

        /// <summary>
        /// Saves an object to a json-file.
        /// </summary>
        public static bool SaveJsonFile(string filePath, object data, out string errorMessage, Newtonsoft.Json.JsonConverter converter = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                errorMessage = "File path is empty.\nCouldn't be saved.";
                return false;
            }

            errorMessage = null;
            try
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    var ser = new Newtonsoft.Json.JsonSerializer();
                    if (converter != null)
                        ser.Converters.Add(converter);
                    ser.Serialize(sw, data);
                }
                return true;
            }
            catch (SerializationException ex)
            {
                errorMessage = $"File\n{Path.GetFullPath(filePath)}\ncouldn't be saved.\nError message:\n\n" + ex.Message;
            }
            return false;
        }

        /// <summary>
        /// Loads a serialized object from a json-file.
        /// </summary>
        public static bool LoadJsonFile<T>(string filePath, out T data, out string errorMessage, Newtonsoft.Json.JsonConverter converter = null) where T : class
        {
            errorMessage = null;
            data = null;

            // load json-file of data
            try
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    var ser = new Newtonsoft.Json.JsonSerializer();
                    if (converter != null)
                        ser.Converters.Add(converter);
                    data = (T)ser.Deserialize(sr, typeof(T));
                    if (data != null)
                        return true;

                    errorMessage = $"File\n{Path.GetFullPath(filePath)}\n contains no readable data.";
                    return false;
                }
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                errorMessage = $"File\n{Path.GetFullPath(filePath)}\ncouldn't be opened or read.\nError message:\n\n" + ex.Message;
            }
            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                errorMessage = $"File\n{Path.GetFullPath(filePath)}\ncouldn't be opened or read.\nError message:\n\n" + ex.Message;
            }
            return false;
        }

        /// <summary>
        /// Tries to create a directory if not existing. Returns true if the path exists.
        /// </summary>
        public static bool TryCreateDirectory(string path, out string error)
        {
            error = null;
            if (Directory.Exists(path)) return true;

            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// Tries to delete a file, doesn't throw an exception when failing.
        /// </summary>
        /// <returns>True if the file is not existing after this method ends.</returns>
        public static bool TryDeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return true;
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        /// <summary>
        /// Tries to delete a file, doesn't throw an exception when failing.
        /// </summary>
        public static bool TryDeleteFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists) return false;
            try
            {
                fileInfo.Delete();
                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        /// <summary>
        /// Tries to delete a directory, doesn't throw an exception when failing.
        /// </summary>
        public static bool TryDeleteDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return false;
            try
            {
                Directory.Delete(dirPath);
                return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        /// <summary>
        /// Tries to move a file, doesn't throw an exception.
        /// </summary>
        public static bool TryMoveFile(string filePathFrom, string filePathTo)
        {
            if (!File.Exists(filePathFrom)) return false;
            try
            {
                File.Move(filePathFrom, filePathTo);
                return true;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        /// <summary>
        /// Creates a temporary directory and returns its path.
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory()
        {
            string tempFolder = Path.GetTempFileName();
            File.Delete(tempFolder);
            Directory.CreateDirectory(tempFolder);
            return tempFolder;
        }

        /// <summary>
        /// Tests if a folder is protected and needs admin privileges to copy files over.
        /// This is used for the updater.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>Returns true if elevated privileges are needed.</returns>
        public static bool TestIfFolderIsProtected(string folderPath)
        {
            try
            {
                string testFilePath = Path.Combine(folderPath, "testFile.txt");
                File.WriteAllText(testFilePath, string.Empty);
                TryDeleteFile(testFilePath);
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a file is a valid json file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static bool IsValidJsonFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            string fileContent = File.ReadAllText(filePath);
            // currently very basic test, could be improved
            return fileContent.StartsWith("{") && fileContent.EndsWith("}");

            //try
            //{
            //    Newtonsoft.Json.Linq.JObject.Parse(fileContent);
            //    return true;
            //}
            //catch { return false; }
        }
    }
}

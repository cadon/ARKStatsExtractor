using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace ARKBreedingStats
{

    public static class FileService
    {
        private const string jsonFolder = "json";

        public const string ValuesFolder = "values";
        public const string ValuesJson = "values.json";
        public const string ValuesServerMultipliers = "serverMultipliers.json";
        public const string TamingFoodData = "tamingFoodData.json";
        public const string ModsManifest = "_manifest.json";
        public const string KibblesJson = "kibbles.json";
        public const string AliasesJson = "aliases.json";
        public const string ArkDataJson = "ark_data.json";
        public const string IgnoreSpeciesClasses = "ignoreSpeciesClasses.json";

        public static readonly string ExeFilePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
        public static readonly string ExeLocation = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        /// <summary>
        /// Returns a <see cref="FileStream"/> of a file located in the json data folder
        /// </summary>
        /// <param name="fileName">name of file to read; use FileService constants</param>
        /// <returns></returns>
        public static FileStream GetJsonFileStream(string fileName)
        {
            return File.OpenRead(GetJsonPath(fileName));
        }

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
        /// Gets the full path for the given filename or the path to the application data folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetPath(string fileName = null)
        {
            return Path.Combine(Updater.IsProgramInstalled ? getLocalApplicationDataPath() : ExeLocation, fileName ?? string.Empty);
        }

        /// <summary>
        /// Gets the full path for the given filename or the path to the json folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetJsonPath(string fileName = null, string fileName2 = null)
        {
            return Path.Combine(Updater.IsProgramInstalled ? getLocalApplicationDataPath() : ExeLocation, jsonFolder, fileName ?? string.Empty, fileName2 ?? string.Empty);
        }

        private static string getLocalApplicationDataPath()
        {
            return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // C:\Users\xxx\AppData\Local\
                    Path.GetFileNameWithoutExtension(ExeFilePath) ?? "ARK Smart Breeding"); // ARK Smart Breeding;
        }

        /// <summary>
        /// Saves an object to a json-file.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath">filePath</param>
        public static bool SaveJSONFile(string filePath, object data, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    var ser = new Newtonsoft.Json.JsonSerializer();
                    ser.Serialize(sw, data);
                }
                return true;
            }
            catch (SerializationException ex)
            {
                errorMessage = $"File\n{Path.GetFullPath(filePath)}\ncouldn't be saved.\nErrormessage:\n\n" + ex.Message;
            }
            return false;
        }

        /// <summary>
        /// Loads a serialized object from a json-file.
        /// </summary>
        /// <param name="filePath">filePath</param>
        /// <param name="data"></param>
        public static bool LoadJSONFile<T>(string filePath, out T data, out string errorMessage)
        {
            errorMessage = null;
            data = default;
            if (!File.Exists(filePath))
                return false;

            // load json-file of data
            try
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    var ser = new Newtonsoft.Json.JsonSerializer();
                    data = (T)ser.Deserialize(sr, typeof(T));
                    return true;
                }
            }
            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                errorMessage = $"File\n{Path.GetFullPath(filePath)}\ncouldn't be opened or read.\nErrormessage:\n\n" + ex.Message;
            }
            return false;
        }
    }
}

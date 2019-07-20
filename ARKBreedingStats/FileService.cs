using System;
using System.IO;
using System.Reflection;

namespace ARKBreedingStats
{

    public static class FileService
    {
        private const string jsonFolder = "json";

        public const string ValuesJson = "values.json";
        public const string ValuesServerMultipliers = "serverMultipliers.json";
        public const string DefaultFoodData = "defaultFoodData.json";
        public const string KibblesJson = "kibbles.json";
        public const string AliasesJson = "aliases.json";
        public const string ArkDataJson = "ark_data.json";
        public const string ClassicFlyersJson = "classicFlyers.json";
        public const string GaiaModJson = "gaiamod.json";

        public const string Ocr1680x1050 = "ocr_1680x1050_100.json";
        public const string Ocr1920x1080 = "ocr_1920x1080_100.json";
        public const string Ocr2560x1440 = "ocr_2560x1440_100.json";
        public const string Ocr2680x1080 = "ocr_2680x1080_100.json";
        public const string Ocr3440x1440 = "ocr_3440x1440_100.json";

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
        public static string GetJsonPath(string fileName = null)
        {
            return Path.Combine(Updater.IsProgramInstalled ? getLocalApplicationDataPath() : ExeLocation, jsonFolder, fileName ?? string.Empty);
        }

        private static string getLocalApplicationDataPath()
        {
            return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // C:\Users\xxx\AppData\Local\
                    Path.GetFileNameWithoutExtension(ExeFilePath) ?? "ARK Smart Breeding"); // ARK Smart Breeding;
        }
    }
}

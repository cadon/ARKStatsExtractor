using System;
using System.Collections.Generic;
using System.IO;

namespace ARKBreedingStats.NamePatterns
{
    /// <summary>
    /// Loads a list of names from a file.
    /// </summary>
    internal static class NameList
    {
        /// <summary>
        /// Contains all name lists, key is the fileName suffix.
        /// </summary>
        private static readonly Dictionary<string, string[]> NameLists = new Dictionary<string, string[]>();
        private static readonly Dictionary<string, DateTime> ListFileCheckedAt = new Dictionary<string, DateTime>();

        /// <summary>
        /// Returns a name from a list. If the file wasn't checked recently, it's checked and reloaded.
        /// </summary>
        public static string GetName(int nameIndex = 0, string listSuffix = null)
        {
            if (nameIndex < 0) return null;
            var nameList = GetNameList(listSuffix);
            if (nameList == null || nameList.Length == 0) return null;

            if (nameIndex >= nameList.Length)
                nameIndex %= nameList.Length;
            return nameList[nameIndex];
        }

        /// <summary>
        /// Returns a name list.
        /// </summary>
        public static string[] GetNameList(string listSuffix = null)
        {
            if (listSuffix == null) listSuffix = string.Empty;
            string[] list;
            if (!ListFileCheckedAt.TryGetValue(listSuffix, out var checkedAt)
                || (DateTime.Now - checkedAt).TotalSeconds > 10
                || !NameLists.TryGetValue(listSuffix, out list))
            {
                list = LoadList(listSuffix, checkedAt);
            }
            return list;
        }

        private static string[] LoadList(string listSuffix, DateTime checkedAt)
        {
            var filePath = FileService.GetJsonPath("creatureNames" + listSuffix + ".txt");

            if (!File.Exists(filePath)) return null;
            try
            {
                if (new FileInfo(filePath).LastWriteTime > checkedAt)
                {
                    var list = File.ReadAllLines(filePath);
                    NameLists[listSuffix] = list;
                }
                ListFileCheckedAt[listSuffix] = DateTime.Now;
                return NameLists[listSuffix];
            }
            catch { }
            return null;
        }
    }
}

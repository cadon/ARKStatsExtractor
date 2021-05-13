using System;
using Newtonsoft.Json;

namespace ARKBreedingStats.Updater
{
    /// <summary>
    /// Generic class for data in a module with format and version.
    /// </summary>
    [JsonObject]
    public class ValueModule<T> where T : class
    {
        public Version Format;
        public Version Version;
        /// <summary>
        /// Data of the module.
        /// </summary>
        public T Data;
    }
}

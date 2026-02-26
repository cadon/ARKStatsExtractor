using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace ARKBreedingStats.utils
{
    internal static class JsonUtils
    {
        /// <summary>
        /// Incrementally reads a json file until a specific root child JsonProperty is found, then this is outed and the json file is closed without reading more.
        /// </summary>
        public static bool ReadJsonNode<T>(string filePath, string nodeName, out T nodeValue)
        {
            nodeValue = default;
            if (string.IsNullOrEmpty(filePath)
                || !File.Exists(filePath)) return false;

            using (var fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            using (var jr = new JsonTextReader(sr) { SupportMultipleContent = false })
            {
                // validate root
                if (!jr.Read() || jr.TokenType != JsonToken.StartObject)
                {
                    MessageBoxes.ShowMessageBox("Error while trying to read the json values file, no json root object found in" + Environment.NewLine + filePath);
                    return false;
                }

                string currentProp = null;
                while (jr.Read())
                {
                    if (jr.TokenType == JsonToken.PropertyName)
                    {
                        currentProp = (string)jr.Value;
                        continue; // move to token after property name
                    }
                    if (currentProp != nodeName) continue;

                    nodeValue = JToken.ReadFrom(jr).ToObject<T>();
                    return true;
                }
            }
            return false;
        }
    }
}

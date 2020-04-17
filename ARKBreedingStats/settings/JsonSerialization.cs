using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ARKBreedingStats.settings
{
    public static class JsonSerialization
    {
        public static bool SerializeDataJson<T>(this T data, string path, bool objectHandling = false)
        {
            try
            {
                var p = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(p))
                    Directory.CreateDirectory(p);
                var js = new JsonSerializer();
                js.Converters.Add(new JavaScriptDateTimeConverter());
                js.NullValueHandling = NullValueHandling.Ignore;
                js.Formatting = Formatting.Indented;
                if (objectHandling)
                    js.TypeNameHandling = TypeNameHandling.Objects;

                using (StreamWriter sw = new StreamWriter(path))
                using (JsonWriter writer = new JsonTextWriter(sw))
                    js.Serialize(writer, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T DeserializeDataFromString<T>(this string inputString, bool objectHandling = false)
        {
            try
            {
                T data;
                var serializer = new JsonSerializer();
                if (objectHandling)
                    serializer.TypeNameHandling = TypeNameHandling.Objects;

                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                using (TextReader tr = new StringReader(inputString))
                {
                    using (JsonReader jsR = new JsonTextReader(tr))
                    {
                        data = serializer.Deserialize<T>(jsR);
                    }
                }
                return data;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static string SerializeDataToString<T>(this T data, bool objectHandling = false)
        {
            try
            {
                var js = new JsonSerializer();
                js.Converters.Add(new JavaScriptDateTimeConverter());
                js.NullValueHandling = NullValueHandling.Ignore;
                js.Formatting = Formatting.Indented;
                if (objectHandling)
                    js.TypeNameHandling = TypeNameHandling.Objects;
                var sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                using (JsonWriter writer = new JsonTextWriter(sw))
                    js.Serialize(writer, data);
                return sb.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static T DeserializeDataFromFile<T>(string path, bool objectHandling = false)
        {
            if (!File.Exists(path))
                return default(T);
            try
            {
                using (var file = File.OpenText(path))
                {
                    var serializer = new JsonSerializer();
                    if (objectHandling)
                        serializer.TypeNameHandling = TypeNameHandling.Objects;

                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    var data = (T)serializer.Deserialize(file, typeof(T));
                    return data;
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}

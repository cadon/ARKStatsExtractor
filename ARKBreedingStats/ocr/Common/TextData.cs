using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.ocr.Common
{
    public class TextData
    {
        public string Text { get; set; }

        public List<Pattern> Patterns { get; set; } = new List<Pattern>();
    }

    public class Boolean2DimArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var arr = (bool[,])value;
            var xSize = arr.GetLength(0);
            var ySize = arr.GetLength(1);
            var res = new JArray();

            for (var y = 0; y < ySize; y++)
            {
                var row = string.Empty;
                for (var x = 0; x < xSize; x++)
                {
                    row += arr[x, y] ? '1' : '0';
                }
                res.Add(row);
            }

            res.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jArray = JArray.Load(reader);
            var first = (string)((JValue)jArray[0]).Value;
            var xSize = first.Length;
            var ySize = jArray.Count;

            var ret = new bool[xSize, ySize];
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    var s = (string)((JValue)jArray[y]).Value;
                    for (int i = 0; i < s.Length; i++)
                    {
                        ret[i, y] = s[i] == '1';
                    }
                }
            }


            return ret;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
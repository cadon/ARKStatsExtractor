using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class Boolean2DimArrayConverter : JsonConverter
    {
        private const long Black = 1L;
        private const long White = 0L;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var arr = (bool[,])value;
            //ToDebugLog(arr);
            var xSize = arr.GetLength(0);
            var ySize = arr.GetLength(1);
            var res = new JArray();

            res.Add(xSize);
            res.Add(ySize);

            int counter = 0;
            long currentValue = 0L;
            foreach (var val in arr)
            {
                if (counter > 63)
                {
                    counter = 0;
                    res.Add(currentValue);
                    currentValue = 0L;
                }

                currentValue |= (val ? Black : White) << counter;
                counter++;
            }

            res.Add(currentValue);

            res.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jArray = JArray.Load(reader);
            var width = Convert.ToInt64(((JValue)jArray[0]).Value);
            var height = Convert.ToInt64(((JValue)jArray[1]).Value);
            var ret = new bool[width, height];
            var totalBits = width * height;
            var i = 0;
            var j = 0;
            foreach (var jVal in jArray.Skip(2))
            {
                var val = Convert.ToInt64(((JValue)jVal).Value);
                var cnt = 0;
                while (cnt < 64 && --totalBits >= 0)
                {
                    var bitValue = (val & (Black << cnt++)) != 0;
                    ret[i, j] = bitValue;

                    j++;
                    if (j >= height)
                    {
                        j = 0;
                        i++;
                    }
                }
            }

            /*
             var bytes = BitConverter.GetBytes(val);
             var bArray = new BitArray(bytes);
             while (cnt < 64 && --totalBits >= 0)
             {
                 var bitValue = bArray[cnt++];
             */

            //ToDebugLog(ret);

            return ret;
        }

        public static void ToDebugLog(bool[,] arr)
        {
            var xSize = arr.GetLength(0);
            var ySize = arr.GetLength(1);
            StringBuilder sb = new StringBuilder(xSize * ySize);
            for (var y = 0; y < ySize; y++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    //row += arr[x, y] ? '1' : '0';
                    sb.Append(arr[x, y] ? '♥' : ' ');
                }
                sb.AppendLine();
            }
            Debug.WriteLine(sb);
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class Pattern
    {
        public Pattern(bool[,] arr)
        {
            Data = arr;
        }

        [JsonConverter(typeof(Boolean2DimArrayConverter))]
        public bool[,] Data { get; set; }

        [JsonIgnore]
        public double Length => Data.Length;

        [JsonIgnore]
        public int Width => Data.GetLength(0);

        [JsonIgnore]
        public int Height => Data.GetLength(1);

        public bool this[int x, int y]
        {
            get => Data[x, y];
            set => Data[x, y] = value;
        }

        public static implicit operator Pattern(bool[,] arr) => new Pattern(arr);
        public static explicit operator bool[,](Pattern arr) => (bool[,])arr.Data.Clone();

        public int CountBlacks() => Data.Cast<bool>().Count(b => b);

        public override string ToString() => OcrUtils.BoolArrayToString(Data);

        public Pattern Clone() => new Pattern((bool[,])Data.Clone());
    }
}

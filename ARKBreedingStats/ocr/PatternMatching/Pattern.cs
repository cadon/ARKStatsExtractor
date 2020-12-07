using System.Linq;
using Newtonsoft.Json;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public class Pattern
    {
        public Pattern(bool[,] arr)
        {
            this.Data = arr;
        }

        public Pattern()
        {
        }

        [JsonConverter(typeof(Boolean2DimArrayConverter))]
        public bool[,] Data { get; set; }

        [JsonIgnore] 
        public double Length => this.Data.Length;

        [JsonIgnore]
        public int Width => this.Data.GetLength(0);

        [JsonIgnore] 
        public int Height => this.Data.GetLength(1);

        public bool this[int i, int j] => this.Data[i, j];

        public static implicit operator Pattern(bool[,] arr)
        {
            return new Pattern(arr);

        }
        public int CountBlacks()
        {
            return this.Data.Cast<bool>().Count(b => b);
        }

        public override string ToString()
        {
            return OcrUtils.BoolArrayToString(this.Data);
        }
    }
}
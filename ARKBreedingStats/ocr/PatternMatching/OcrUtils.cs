using System.Text;

namespace ARKBreedingStats.ocr.PatternMatching
{
    // TODO class needed? or remove?
    public static class OcrUtils
    {
        public static string BoolArrayToString(bool[,] arr)
        {
            var xSize = arr.GetLength(0);
            var ySize = arr.GetLength(1);
            StringBuilder sb = new StringBuilder(xSize * ySize);
            for (var y = 0; y < ySize; y++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    sb.Append(arr[x, y] ? '1' : '0');
                }
                sb.AppendLine();
            }

            return $"\r\n{sb}\r\n";
        }
    }
}
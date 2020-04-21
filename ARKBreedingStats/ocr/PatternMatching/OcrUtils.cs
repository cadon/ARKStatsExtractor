using System.Text;

namespace ARKBreedingStats.ocr.PatternMatching
{
    public static class OcrUtils
    {
        public static string BoolArrayToString(bool[,] arr)
        {
            StringBuilder sb = new StringBuilder();
            var xSize = arr.GetLength(0);
            var ySize = arr.GetLength(1);
            for (var y = 0; y < ySize; y++)
            {
                var row = string.Empty;
                for (var x = 0; x < xSize; x++)
                {
                    row += arr[x, y] ? '1' : '0';
                }
                sb.AppendLine(row);
            }

            return "\r\n" + sb.ToString() + "\r\n";
        }
    }
}
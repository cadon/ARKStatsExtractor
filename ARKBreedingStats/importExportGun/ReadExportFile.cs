using System.IO;
using System.Text;

namespace ARKBreedingStats.importExportGun
{
    /// <summary>
    /// Reads the content of an export files created by the export gun mod.
    /// </summary>
    internal static class ReadExportFile
    {
        /// <summary>
        /// Reads the content of an export file and returns the containing json part as string.
        /// </summary>
        public static string ReadFile(string filePath, string expectedStartString, out string error)
        {
            error = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    br.ReadBytes(4);
                    if (Encoding.UTF8.GetString(br.ReadBytes(expectedStartString.Length))
                        != expectedStartString)
                    {
                        error = $"Expected start string {expectedStartString} not found";
                        return null;
                    }

                    const string strProp = "StrProperty";
                    if (!SearchBytes(br, Encoding.ASCII.GetBytes(strProp)))
                    {
                        error = $"Expected property {strProp} not found";
                        return null;
                    }

                    br.ReadBytes(9); // skipping to json string length
                    var jsonLength = br.ReadInt32();
                    if (jsonLength <= 0)
                    {
                        error = $"Json length {jsonLength} at position {(br.BaseStream.Position - 4)} invalid";
                        return null;
                    }
                    return Encoding.UTF8.GetString(br.ReadBytes(jsonLength));
                }
            }
        }

        /// <summary>
        /// Looks for a specific byte pattern sequence, the stream position is set after that pattern.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="bytesToFind"></param>
        /// <returns>True if pattern found.</returns>
        private static bool SearchBytes(BinaryReader br, byte[] bytesToFind)
        {
            if (bytesToFind == null || bytesToFind.Length == 0) return false;
            var pi = 0; // index of pattern currently comparing, indices before already found
            var l = br.BaseStream.Length;
            while (br.BaseStream.Position < l)
            {
                if (br.ReadByte() == bytesToFind[pi])
                {
                    pi++;
                    if (pi == bytesToFind.Length)
                        return true;
                    continue;
                }

                pi = 0;
            }
            return false;
        }
    }
}

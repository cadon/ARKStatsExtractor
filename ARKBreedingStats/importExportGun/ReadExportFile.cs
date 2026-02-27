using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace ARKBreedingStats.importExportGun
{
    /// <summary>
    /// Reads the content of an export file encapsulating a json string, created by the export gun mod (ASE).
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
                    var header = br.ReadBytes(4);
                    // Check for GVAS header to determine ASA format
                    if (header[0] == 'G' && header[1] == 'V' && header[2] == 'A' && header[3] == 'S')
                    {
                        // ASA format is a variant of GVAS - but we ignore most of it

                        // Find classname to confirm correct save object
                        if (!SearchBytes(br, Encoding.ASCII.GetBytes("DinoExportGunSave_C\0")))
                        {
                            error = "Expected start string DinoExportGunSave_C not found";
                            return null;
                        }
                        
                        // Find StrProperty
                        if (!SearchBytes(br, Encoding.ASCII.GetBytes("StrProperty\0")))
                        {
                            error = "Expected property StrProperty not found";
                            return null;
                        }

                        // Skip 25 bytes
                        br.ReadBytes(25);

                        // Read the length of the json string in bytes
                        var jsonByteLength = br.ReadInt32();

                        // Read the json string
                        return Encoding.UTF8.GetString(br.ReadBytes(jsonByteLength));
                    }
                    else
                    {
                        // ASE format
                        if (Encoding.UTF8.GetString(br.ReadBytes(expectedStartString.Length))
                        != expectedStartString)
                        {
                            error = $"Expected start string {expectedStartString} not found";
                            return null;
                        }

                        const string strProp = "StrProperty";
                        if (!SearchBytes(br, Encoding.ASCII.GetBytes(strProp + '\0')))
                        {
                            error = $"Expected property {strProp} not found";
                            return null;
                        }

                        // Assumption of the next 12 bytes:
                        // first the length of the string in bytes including 4 leading bytes (i.e. 4 bytes longer than the actual string)
                        // then four \0 bytes
                        // the next 4 bytes are the length of the actual string, depending on the encoding:
                        // If >0 it's the length in bytes and the string uses utf8, if it's <0 it's the negative length of the string in double bytes
                        var jsonByteLength = br.ReadInt32() - 4; // string length (subtracting the 4 encoding length bytes)
                        br.ReadBytes(4); // skipping \0 bytes
                        var jsonCharLength = br.ReadInt32();
                        var useUtf16 = false;
                        if (jsonCharLength <= 0)
                        {
                            if (jsonCharLength * -2 == jsonByteLength)
                            {
                                useUtf16 = true;
                            }
                            else
                            {
                                error = $"Json length {jsonCharLength} at position {(br.BaseStream.Position - 4)} invalid";
                                return null;
                            }
                        }

                        return useUtf16
                            ? Encoding.Unicode.GetString(br.ReadBytes(jsonByteLength))
                            : Encoding.UTF8.GetString(br.ReadBytes(jsonByteLength));
                    }
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

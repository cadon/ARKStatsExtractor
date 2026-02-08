using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ARKBreedingStats.SpeciesOptions.ColorSettings
{
    /// <summary>
    /// List of color ids wanted for a region.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class WantedRegionColors : SpeciesOptionBase
    {
        private const int BitsPerElement = 32; // uint

        /// <summary>
        /// Bit flag for each wanted colors.
        /// </summary>
        [JsonProperty("colorIdBits", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private uint[] _wantedColorBitFlags;

        /// <summary>
        /// Override parent setting.
        /// </summary>
        [JsonProperty("ovr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool OverrideParentBool;

        /// <summary>
        /// Returns if a color id is set to wanted.
        /// </summary>
        public bool IsColorWanted(byte colorId)
        {
            if (_wantedColorBitFlags == null) return false;
            Indices(colorId, out var arrayIndex, out var bitFlag);
            return (_wantedColorBitFlags[arrayIndex] & bitFlag) != 0;
        }

        /// <summary>
        /// Sets a color id to wanted or not wanted.
        /// </summary>
        public void SetColorWanted(byte colorId, bool setColor)
        {
            if (_wantedColorBitFlags == null) _wantedColorBitFlags = new uint[256 / BitsPerElement];
            Indices(colorId, out var arrayIndex, out var bitFlag);
            if (setColor)
                _wantedColorBitFlags[arrayIndex] |= bitFlag;
            else
                _wantedColorBitFlags[arrayIndex] &= ~bitFlag;
        }

        private static readonly Regex CleanUpColorIdRanges = new Regex(@"[^\d\-,]");

        /// <summary>
        /// Set color ids. Returns false if there was an error.
        /// </summary>
        /// <param name="colorIds">Comma separated string of color ids, ranges like 2-4 are supported.</param>
        /// <param name="clearOthers">Clear color ids not mentioned.</param>
        public bool SetColorsWanted(string colorIds, bool clearOthers, out string errorMessage)
        {
            errorMessage = null;
            if (clearOthers) _wantedColorBitFlags = new uint[256 / BitsPerElement];

            if (string.IsNullOrWhiteSpace(colorIds)) return true;
            colorIds = CleanUpColorIdRanges.Replace(colorIds, string.Empty);

            var colorIdsToSet = new List<byte>();
            try
            {
                var ranges = colorIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var range in ranges)
                {
                    if (range.Contains('-'))
                    {
                        var bounds = range.Split('-').Select(byte.Parse).ToArray();
                        for (var colorId = bounds[0]; colorId <= bounds[1]; colorId++)
                            colorIdsToSet.Add(colorId);
                    }
                    else
                    {
                        colorIdsToSet.Add(byte.Parse(range));
                    }
                }
            }
            catch
            {
                errorMessage = $"Invalid color id ranges: \"{colorIds}\". Expected format is color separated list, optional ranges, e.g. \"2, 14-22, 78\"";
                //MessageBoxes.ExceptionMessageBox(ex, $"Invalid color id ranges: \"{colorIds}\"");
                return false;
            }
            foreach (var colorId in colorIdsToSet)
                SetColorWanted(colorId, true);
            return true;
        }

        /// <summary>
        /// Get comma separated string of color ids, where consecutive ids are combined in a range like 3-5.
        /// </summary>
        /// <returns></returns>
        public string GetColorIdsCsv()
        {
            if (_wantedColorBitFlags == null) return string.Empty;

            var colorIds = new List<byte>();
            for (byte colorId = 0; ; colorId++)
            {
                if (IsColorWanted(colorId))
                    colorIds.Add(colorId);
                if (colorId == byte.MaxValue) break;
            }

            var result = new List<string>();
            for (var i = 0; i < colorIds.Count; i++)
            {
                var start = colorIds[i];
                while (i + 1 < colorIds.Count && colorIds[i + 1] == colorIds[i] + 1) i++;

                var end = colorIds[i];
                result.Add(start == end ? start.ToString() : $"{start}-{end}");
            }

            return string.Join(", ", result);
        }

        /// <summary>
        /// Using an uint array where each uint represents 32 bit flags as color ids. Returns the array index of the uint and the bit flag.
        /// </summary>
        private static void Indices(byte colorId, out int arrayIndex, out uint bitFlag)
        {
            arrayIndex = colorId / BitsPerElement;
            bitFlag = 1U << (colorId % BitsPerElement);
        }

        public override void Initialize()
        {
            OverrideParent = OverrideParentBool;
        }

        public override void PrepareForSaving(bool isRoot)
        {
            OverrideParentBool = OverrideParent || isRoot;
            if (_wantedColorBitFlags?.All(f => f == 0) == true)
                _wantedColorBitFlags = null;
        }

        public override bool DefinesData() => true;

        public static WantedRegionColors GetDefault() => new WantedRegionColors();

        public static WantedRegionColors[] GetDefaultOptions() =>
            Enumerable.Range(0, Ark.ColorRegionCount).Select(si => GetDefault()).ToArray();
    }
}

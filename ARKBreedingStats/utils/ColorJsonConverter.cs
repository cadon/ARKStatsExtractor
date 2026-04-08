using System;
using System.Drawing;
using Newtonsoft.Json;

namespace ARKBreedingStats.utils
{
    /// <summary>
    /// Serializes <see cref="Color"/> as an "R,G,B" string for human-readable JSON palettes.
    /// </summary>
    internal class ColorJsonConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.R},{value.G},{value.B}");
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var s = reader.Value as string;
            if (string.IsNullOrWhiteSpace(s))
                return existingValue;

            var parts = s.Split(',');
            if (parts.Length == 3
                && int.TryParse(parts[0].Trim(), out int r)
                && int.TryParse(parts[1].Trim(), out int g)
                && int.TryParse(parts[2].Trim(), out int b))
            {
                return Color.FromArgb(
                    Math.Clamp(r, 0, 255),
                    Math.Clamp(g, 0, 255),
                    Math.Clamp(b, 0, 255));
            }

            return existingValue;
        }
    }
}

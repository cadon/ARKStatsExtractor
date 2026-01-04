using System.Collections.Generic;
using System.Drawing;
using ARKBreedingStats.utils;
using Newtonsoft.Json;

namespace ARKBreedingStats.StatsOptions.LevelColorSettings
{
    /// <summary>
    /// Representation of a level regarding chart scaling and colour.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class LevelGraphRepresentation
    {
        /// <summary>
        /// Level with the LowerColor color.
        /// </summary>
        [JsonProperty]
        public int LowerBound
        {
            get => _lowerBound;
            set
            {
                _lowerBound = value;
                _colorCache?.Clear();
            }
        }

        private int _lowerBound;

        /// <summary>
        /// Level with the UpperColor color.
        /// </summary>
        [JsonProperty]
        public int UpperBound
        {
            get => _upperBound;
            set
            {
                _upperBound = value;
                _colorCache?.Clear();
            }
        }

        private int _upperBound = 50;

        private Color _lowerColor;
        private float _lowerColorH;
        private float _lowerColorS;
        private float _lowerColorV;
        /// <summary>
        /// Color of the lower bound.
        /// </summary>
        [JsonProperty]
        public Color LowerColor
        {
            get => _lowerColor;
            set
            {
                _lowerColor = value;
                _lowerColorH = _lowerColor.GetHue();
                _lowerColorS = _lowerColor.GetSaturation();
                _lowerColorV = _lowerColor.GetValue();
                if (_lowerColorS < float.Epsilon)
                    _lowerColorH = _upperColorH;

                _colorCache?.Clear();
            }
        }

        private Color _upperColor;
        private float _upperColorH;
        private float _upperColorS;
        private float _upperColorV;
        /// <summary>
        /// Color of the upper bound.
        /// </summary>
        [JsonProperty]
        public Color UpperColor
        {
            get => _upperColor;
            set
            {
                _upperColor = value;
                _upperColorH = _upperColor.GetHue();
                _upperColorS = _upperColor.GetSaturation();
                _upperColorV = _upperColor.GetValue();
                if (_upperColorS == 0)
                    _upperColorH = _lowerColorH;

                _colorCache?.Clear();
            }
        }

        /// <summary>
        /// If false the hue value increases from the lower color to the upper color.
        /// </summary>
        [JsonProperty("hueRev", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ColorGradientReversed
        {
            get => _colorGradientReversed;
            set
            {
                _colorGradientReversed = value;
                _colorCache?.Clear();
            }
        }

        private bool _colorGradientReversed;

        private Dictionary<int, Color> _colorCache;

        /// <summary>
        /// Returns the color of a level.
        /// </summary>
        public Color GetLevelColor(int level)
        {
            if (level <= LowerBound) return LowerColor;
            if (level >= UpperBound) return UpperColor;
            if (_colorCache?.TryGetValue(level, out var c) == true)
                return c;

            var relativePosition = (double)(level - LowerBound) / (UpperBound - LowerBound);

            var h = _lowerColorH + relativePosition * (_upperColorH - _lowerColorH - (ColorGradientReversed ? 360 : 0) + (_upperColorH < _lowerColorH ? 360 : 0));
            var s = _lowerColorS + relativePosition * (_upperColorS - _lowerColorS);
            var v = _lowerColorV + relativePosition * (_upperColorV - _lowerColorV);

            c = Utils.ColorFromHsv(h, s, v);
            if (_colorCache == null) _colorCache = new Dictionary<int, Color>();
            _colorCache[level] = c;
            return c;
        }

        public static Color GetStatLevelColor(int level, int statIndex) =>
            LevelRepresentations[statIndex].GetLevelColor(level);

        public static LevelGraphRepresentation[] LevelRepresentations;

        /// <summary>
        /// Default color range from red to green.
        /// </summary>
        public static LevelGraphRepresentation GetDefault => new LevelGraphRepresentation
        {
            LowerBound = 0,
            UpperBound = 50,
            LowerColor = Color.Red,
            UpperColor = Color.FromArgb(0, 255, 0)
        };

        /// <summary>
        /// Inverted color range from green over yellow to red.
        /// </summary>
        public static LevelGraphRepresentation GetDefaultInverted => new LevelGraphRepresentation
        {
            ColorGradientReversed = true,
            LowerBound = 0,
            UpperBound = 50,
            LowerColor = Color.FromArgb(0, 255, 0),
            UpperColor = Color.Red
        };

        public static LevelGraphRepresentation GetDefaultMutationLevel => new LevelGraphRepresentation
        {
            LowerBound = 0,
            UpperBound = 255,
            LowerColor = Color.Cyan,
            UpperColor = Color.DeepPink
        };

        public static LevelGraphRepresentation GetDefaultMutationLevelInverted => new LevelGraphRepresentation
        {
            ColorGradientReversed = true,
            LowerBound = 0,
            UpperBound = 255,
            LowerColor = Color.DeepPink,
            UpperColor = Color.Cyan
        };

        public LevelGraphRepresentation Copy() =>
            new LevelGraphRepresentation
            {
                LowerBound = LowerBound,
                UpperBound = UpperBound,
                LowerColor = LowerColor,
                UpperColor = UpperColor,
                ColorGradientReversed = ColorGradientReversed
            };

        public static bool operator ==(LevelGraphRepresentation a, LevelGraphRepresentation b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(LevelGraphRepresentation a, LevelGraphRepresentation b) => !(a == b);

        public bool Equals(LevelGraphRepresentation other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.LowerBound == LowerBound
                   && other.UpperBound == UpperBound
                   && other.ColorGradientReversed == ColorGradientReversed
                   && Utils.ColorsEqual(other.LowerColor, LowerColor)
                   && Utils.ColorsEqual(other.UpperColor, UpperColor);
        }

        public override int GetHashCode() => $"{LowerBound}-{UpperBound}_{LowerColor}-{UpperColor}_{ColorGradientReversed}".GetHashCode();

        /// <summary>
        /// Returns true if the colors and gradient direction are equal (ignoring the level range).
        /// </summary>
        public bool ColorEquals(LevelGraphRepresentation other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return ColorGradientReversed == other.ColorGradientReversed
                   && Utils.ColorsEqual(other.LowerColor, LowerColor)
                   && Utils.ColorsEqual(other.UpperColor, UpperColor);
        }
    }
}

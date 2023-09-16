using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;
using ARKBreedingStats.utils;

namespace ARKBreedingStats.NamePatterns
{
    /// <summary>
    /// Functions used in name patterns to name creatures.
    /// </summary>
    internal static class NamePatternFunctions
    {
        internal static string ResolveFunction(Match match, NamePatternParameters parameters)
        {
            var functionName = match.Groups[1].Value.ToLower();

            if (!string.IsNullOrEmpty(functionName) && Functions.TryGetValue(functionName, out var func))
            {
                return func(match, parameters);
            }

            return string.Empty;
        }

        private static string ParametersInvalid(string specificError, string expression, bool displayError)
        {
            if (displayError)
                MessageBoxes.ShowMessageBox($"The syntax of the following pattern function\n{expression}\ncannot be processed and will be ignored."
                                            + (string.IsNullOrEmpty(specificError) ? string.Empty : $"\n\nSpecific error:\n{specificError}"),
                    $"Naming pattern function error");
            return displayError ? expression : specificError;
        }


        private static readonly Dictionary<string, Func<Match, NamePatternParameters, string>> Functions =
            new Dictionary<string, Func<Match, NamePatternParameters, string>>
            {
                {"if", FunctionIf},
                {"ifexpr", FunctionIfExpr},
                {"expr", FunctionExpr},
                {"len", FunctionLen},
                {"substring", FunctionSubString},
                {"format", FunctionFormat},
                {"format_int", FunctionFormatInt},
                {"padleft", FunctionPadLeft},
                {"padright", FunctionPadRight},
                {"float_div", FunctionFloatDiv},
                {"div", FunctionDiv},
                {"casing", FunctionCasing},
                {"replace", FunctionReplace},
                {"regexreplace", FunctionRegExReplace},
                {"customreplace", FunctionCustomReplace},
                {"time", FunctionTime},
                {"color", FunctionColor},
                {"colornew", FunctionColorNew},
                {"indexof", FunctionIndexOf},
                {"md5", FunctionMd5}
            };

        private static string FunctionIf(Match m, NamePatternParameters p)
        {
            // check if Group2 !isNullOrWhiteSpace
            // Group3 contains the result if true
            // Group4 (optional) contains the result if false
            return string.IsNullOrWhiteSpace(m.Groups[2].Value) ? m.Groups[4].Value : m.Groups[3].Value;
        }

        private static string FunctionIfExpr(Match m, NamePatternParameters p)
        {
            // tries to evaluate the expression
            // possible operators are ==, !=, <, >, =<, =>
            var match = Regex.Match(m.Groups[2].Value, @"\A\s*(\d+(?:\.\d*)?)\s*(==|!=|<|<=|>|>=)\s*(\d+(?:\.\d*)?)\s*\Z");
            if (match.Success
                && double.TryParse(match.Groups[1].Value, out double d1)
                && double.TryParse(match.Groups[3].Value, out double d2)
                )
            {
                switch (match.Groups[2].Value)
                {
                    case "==": return d1 == d2 ? m.Groups[3].Value : m.Groups[4].Value;
                    case "!=": return d1 != d2 ? m.Groups[3].Value : m.Groups[4].Value;
                    case "<": return d1 < d2 ? m.Groups[3].Value : m.Groups[4].Value;
                    case "<=": return d1 <= d2 ? m.Groups[3].Value : m.Groups[4].Value;
                    case ">": return d1 > d2 ? m.Groups[3].Value : m.Groups[4].Value;
                    case ">=": return d1 >= d2 ? m.Groups[3].Value : m.Groups[4].Value;
                }
            }
            else
            {
                // compare the values as strings
                match = Regex.Match(m.Groups[2].Value, @"\A\s*(.*?)\s*(==|!=|<=|<|>=|>)\s*(.*?)\s*\Z");
                if (match.Success)
                {
                    int stringComparingResult = match.Groups[1].Value.CompareTo(match.Groups[3].Value);
                    switch (match.Groups[2].Value)
                    {
                        case "==": return stringComparingResult == 0 ? m.Groups[3].Value : m.Groups[4].Value;
                        case "!=": return stringComparingResult != 0 ? m.Groups[3].Value : m.Groups[4].Value;
                        case "<": return stringComparingResult < 0 ? m.Groups[3].Value : m.Groups[4].Value;
                        case "<=": return stringComparingResult <= 0 ? m.Groups[3].Value : m.Groups[4].Value;
                        case ">": return stringComparingResult > 0 ? m.Groups[3].Value : m.Groups[4].Value;
                        case ">=": return stringComparingResult >= 0 ? m.Groups[3].Value : m.Groups[4].Value;
                    }
                }
            }
            return ParametersInvalid($"The expression for ifexpr invalid: \"{m.Groups[2].Value}\"", m.Groups[0].Value, p.DisplayError);
        }

        private static string FunctionExpr(Match m, NamePatternParameters p)
        {
            // tries to calculate the result of the expression
            // possible operators are +, -, *, /
            var match = Regex.Match(m.Groups[2].Value, @"\A\s*(\d+(?:\.\d*)?)\s*(\+|\-|\*|\/)\s*(\d+(?:\.\d*)?)\s*\Z");
            if (match.Success
                && double.TryParse(match.Groups[1].Value, out var d1)
                && double.TryParse(match.Groups[3].Value, out var d2)
            )
            {
                switch (match.Groups[2].Value)
                {
                    case "+": return (d1 + d2).ToString();
                    case "-": return (d1 - d2).ToString();
                    case "*": return (d1 * d2).ToString();
                    case "/": return d2 == 0 ? "divByZero" : (d1 / d2).ToString();
                }
            }
            return ParametersInvalid($"The expression for expr is invalid: \"{m.Groups[2].Value}\"", m.Groups[0].Value, p.DisplayError);
        }

        private static string FunctionLen(Match m, NamePatternParameters p)
        {
            // returns the length of the parameter
            return UnEscapeSpecialCharacters(m.Groups[2].Value).Length.ToString();
        }

        private static string FunctionSubString(Match m, NamePatternParameters p)
        {
            // check param number: 1: substring, 2: p1, 3: pos, 4: length
            if (!int.TryParse(m.Groups[3].Value, out var pos))
                return m.Groups[2].Value;

            var text = m.Groups[2].Value;
            var textLength = text.Length;

            if (pos < 0) pos += textLength;
            if (pos < 0) pos = 0;
            if (pos >= textLength) return string.Empty;

            if (string.IsNullOrEmpty(m.Groups[4].Value))
                return text.Substring(pos);

            var substringLength = int.TryParse(m.Groups[4].Value, out var v) ? v : 0;
            if (substringLength < 0)
                substringLength += textLength - pos;

            if (substringLength <= 0) return string.Empty;
            if (pos + substringLength > textLength)
                substringLength = textLength - pos;

            return text.Substring(pos, substringLength);
        }

        private static string FunctionFormat(Match m, NamePatternParameters p)
        {
            // check param number: 1: format, 2: p1, 3: formatString
            return FormatString(m.Groups[2].Value, m.Groups[3].Value, m, p);
        }

        private static string FunctionFormatInt(Match m, NamePatternParameters p)
        {
            return FormatString(m.Groups[2].Value, m.Groups[3].Value, m, p, true);
        }

        private static string FormatString(string s, string formatString, Match m, NamePatternParameters p, bool isInteger = false)
        {
            if (string.IsNullOrEmpty(formatString))
                return ParametersInvalid("No Format string given", m.Groups[0].Value, p.DisplayError);

            if (string.IsNullOrEmpty(s)) return string.Empty;

            return isInteger ? Convert.ToInt32(s).ToString(formatString) : Convert.ToDouble(s).ToString(formatString);
        }

        private static string FunctionPadLeft(Match m, NamePatternParameters p)
        {
            // check param number: 1: padleft, 2: p1, 3: desired length, 4: padding char

            var padLen = Convert.ToInt32(m.Groups[3].Value);
            var padChar = m.Groups[4].Value;
            if (!string.IsNullOrEmpty(padChar))
            {
                return m.Groups[2].Value.PadLeft(padLen, padChar[0]);
            }
            return ParametersInvalid($"No padding char given.", m.Groups[0].Value, p.DisplayError);
        }

        private static string FunctionPadRight(Match m, NamePatternParameters p)
        {
            // check param number: 1: padright, 2: p1, 3: desired length, 4: padding char

            var padLen = Convert.ToInt32(m.Groups[3].Value);
            var padChar = m.Groups[4].Value;
            if (!string.IsNullOrEmpty(padChar))
            {
                return m.Groups[2].Value.PadRight(padLen, padChar[0]);
            }
            return ParametersInvalid($"No padding char given.", m.Groups[0].Value, p.DisplayError);
        }

        private static string FunctionFloatDiv(Match m, NamePatternParameters p)
        {
            // returns an float after dividing the parsed number
            // parameter: 1: div, 2: number, 3: divided by 4: format string
            double dividend = double.Parse(m.Groups[2].Value);
            double divisor = double.Parse(m.Groups[3].Value);
            if (divisor > 0)
                return ((dividend / divisor)).ToString(m.Groups[4].Value);
            else
                return ParametersInvalid("Division by 0", m.Groups[0].Value, p.DisplayError);
        }

        private static string FunctionDiv(Match m, NamePatternParameters p)
        {
            // returns an integer after dividing the parsed number
            // parameter: 1: div, 2: number, 3: divided by
            double number = double.Parse(m.Groups[2].Value);
            double div = double.Parse(m.Groups[3].Value);
            if (div > 0)
                return ((int)(number / div)).ToString();
            else
                return ParametersInvalid("Division by 0", m.Groups[0].Value, p.DisplayError);
        }

        private static string FunctionCasing(Match m, NamePatternParameters p)
        {
            // parameter: 1: casing, 2: text, 3: U for UPPER, L for lower, T for Title
            switch (m.Groups[3].Value.ToLower())
            {
                case "u": return m.Groups[2].Value.ToUpperInvariant();
                case "l": return m.Groups[2].Value.ToLowerInvariant();
                case "t": return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m.Groups[2].Value);
            }
            return ParametersInvalid($"casing expects 'U', 'L' or 'T', given is '{m.Groups[3].Value}'", m.Groups[0].Value, p.DisplayError);
        }

        private static string FunctionReplace(Match m, NamePatternParameters p)
        {
            // parameter: 1: replace, 2: text, 3: find, 4: replace
            if (string.IsNullOrEmpty(m.Groups[2].Value)
                || string.IsNullOrEmpty(m.Groups[3].Value))
                return m.Groups[2].Value;
            return m.Groups[2].Value.Replace(UnEscapeSpecialCharacters(m.Groups[3].Value), UnEscapeSpecialCharacters(m.Groups[4].Value));
        }

        private static string FunctionRegExReplace(Match m, NamePatternParameters p)
        {
            // parameter: 1: replace, 2: text, 3: regEx pattern, 4: replace

            try
            {
                return Regex.Replace(UnEscapeSpecialCharacters(m.Groups[2].Value), UnEscapeSpecialCharacters(m.Groups[3].Value), UnEscapeSpecialCharacters(m.Groups[4].Value));
            }
            catch (Exception ex)
            {
                return ParametersInvalid($"The regex \"{m.Groups[3].Value}\" caused the exception: {ex.Message}", m.Groups[0].Value, p.DisplayError);
            }
        }

        /// <summary>
        /// Functions cannot process the characters {|} directly, they have to be replaced to be used.
        /// </summary>
        public static string UnEscapeSpecialCharacters(string text) => text?
            .Replace("&lcub;", "{")
            .Replace("&vline;", "|")
            .Replace("&rcub;", "}")
            .Replace("&nbsp;", " ") // for backwards compatibility
            .Replace("&sp;", " ")
        ;

        private static string FunctionCustomReplace(Match m, NamePatternParameters p)
        {
            // parameter: 1: customreplace, 2: key, 3: return if key not available
            if (p.CustomReplacings == null
                || string.IsNullOrEmpty(m.Groups[2].Value)
                || !p.CustomReplacings.ContainsKey(m.Groups[2].Value))
                return m.Groups[3].Value;
            return p.CustomReplacings[m.Groups[2].Value];
        }

        private static string FunctionTime(Match m, NamePatternParameters p)
        {
            // parameter: 1: time, 2: format
            return DateTime.Now.ToString(m.Groups[2].Value);
        }

        private static string FunctionColor(Match m, NamePatternParameters p)
        {
            // parameter 1: region id (0,...,5), 2: if not empty, the color name instead of the numerical id is returned, 3: if not empty, the function will return also a value even if the color region is not used on that species.
            if (!int.TryParse(m.Groups[2].Value, out int regionId)
                || regionId < 0 || regionId > 5) return ParametersInvalid("color region id has to be a number in the range 0 - 5", m.Groups[0].Value, p.DisplayError);

            if (!p.Creature.Species.EnabledColorRegions[regionId] &&
                string.IsNullOrWhiteSpace(m.Groups[4].Value))
                return string.Empty; // species does not use this region and user doesn't want it (param 3 is empty)

            if (p.Creature.colors == null) return string.Empty; // no color info
            if (string.IsNullOrWhiteSpace(m.Groups[3].Value))
                return p.Creature.colors[regionId].ToString();
            return CreatureColors.CreatureColorName(p.Creature.colors[regionId]);
        }

        /// <summary>
        /// Returns new if the color is newInRegion in this region, returns newInSpecies if the color is new in all regions of this species, else returns string.Empty.
        /// </summary>
        private static string FunctionColorNew(Match m, NamePatternParameters p)
        {
            // parameter 1: region id (0,...,5)
            if (!int.TryParse(m.Groups[2].Value, out int regionId)
                || regionId < 0 || regionId > 5) return ParametersInvalid("color region id has to be a number in the range 0 - 5", m.Groups[0].Value, p.DisplayError);

            if (p.ColorsExisting == null) return string.Empty;

            switch (p.ColorsExisting[regionId])
            {
                case CreatureCollection.ColorExisting.ColorExistingInOtherRegion:
                    return "newInRegion";
                case CreatureCollection.ColorExisting.ColorIsNew:
                    return "newInSpecies";
                default:
                    return string.Empty;
            }
        }

        private static string FunctionIndexOf(Match m, NamePatternParameters p)
        {
            // parameter: 1: source string, 2: string to find
            if (string.IsNullOrEmpty(m.Groups[2].Value) || string.IsNullOrEmpty(m.Groups[3].Value))
                return string.Empty;
            int index = m.Groups[2].Value.IndexOf(m.Groups[3].Value);
            return index >= 0 ? index.ToString() : string.Empty;
        }

        private static MD5 _md5;

        private static string FunctionMd5(Match m, NamePatternParameters p)
        {
            if (_md5 == null) _md5 = MD5.Create();

            var inputBytes = Encoding.ASCII.GetBytes(UnEscapeSpecialCharacters(m.Groups[2].Value));
            var hashBytes = _md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static void Dispose()
        {
            _md5?.Dispose();
        }
    }

    internal struct NamePatternParameters
    {
        internal Creature Creature;
        internal Dictionary<string, string> CustomReplacings;
        internal bool DisplayError;
        /// <summary>
        /// The number field {n} will add the lowest possible positive integer >1 for the name to be unique. It has to be processed after all other functions.
        /// </summary>
        internal bool ProcessNumberField;

        internal CreatureCollection.ColorExisting[] ColorsExisting;
    }
}

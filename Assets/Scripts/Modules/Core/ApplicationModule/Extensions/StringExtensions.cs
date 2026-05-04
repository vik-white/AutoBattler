using System.Globalization;
using UnityEngine;

namespace vikwhite
{
    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }
        
        public static float ToFloat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0f;

            value = value.Replace(",", ".");

            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                return result;

            return 0f;
        }

        /// <summary>
        /// Parses a string as either a single integer ("5") or an inclusive range ("5-10").
        /// Trims whitespace and non-breaking spaces. If <paramref name="raw"/> is a single value,
        /// both <paramref name="min"/> and <paramref name="max"/> are set to it.
        /// If max &lt; min, the values are swapped.
        /// </summary>
        public static bool TryParseValueRange(this string raw, out int min, out int max)
        {
            min = 0;
            max = 0;
            if (string.IsNullOrWhiteSpace(raw)) return false;

            var cleaned = raw.Replace(" ", "").Replace("\u00a0", "");

            // Range "min-max". Start search from index 1 to allow leading '-' (negative single value).
            var dash = cleaned.IndexOf('-', 1);
            if (dash > 0)
            {
                var minPart = cleaned.Substring(0, dash);
                var maxPart = cleaned.Substring(dash + 1);

                if (!int.TryParse(minPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out min)) return false;
                if (!int.TryParse(maxPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out max)) return false;

                if (max < min) (min, max) = (max, min);
                return true;
            }

            if (!int.TryParse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture, out var single)) return false;
            min = single;
            max = single;
            return true;
        }
    }
}

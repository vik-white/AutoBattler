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
    }
}

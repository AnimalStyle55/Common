using CuttingEdge.Conditions;
using System;

namespace LD.Common.TextFormatting
{
    /// <summary>
    /// String extensions for fixed file formatting
    /// </summary>
    public static class TextFormatting
    {
        /// <summary>
        /// Makes sure that a string fits in a field of a given length and never goes under or over
        /// </summary>
        /// <param name="s">The string to trim; if the string is null it will be converted to an empty string</param>
        /// <param name="width">The width of the field</param>
        /// <param name="c">The padding character, default is space</param>
        /// <returns>A string trimmed if necessary and padded with the padding string if necessary to ensure a given length</returns>
        public static string TrimAndPadLeft(this string s, int width, char c = ' ')
        {
            Condition.Requires(width, "width").IsGreaterThan(0, "width must be a positive integer");
            string result = s ?? "";
            if (result.Length > width)
                result = result.Substring(result.Length - width);
            return result.PadLeft(width, c);
        }

        /// <summary>
        /// Makes sure that a string fits in a field of a given length and never goes under or over
        /// </summary>
        /// <param name="s">The string to trim; if the string is null it will be converted to an empty string</param>
        /// <param name="width">The width of the field</param>
        /// <param name="c">The padding character, default is space</param>
        /// <returns>A string trimmed if necessary and padded with the padding string if necessary to ensure a given length</returns>
        public static string TrimAndPadRight(this string s, int width, char c = ' ')
        {
            Condition.Requires(width, "width").IsGreaterThan(0, "width must be a positive integer");
            string result = s ?? "";
            if (result.Length > width)
                result = result.Substring(0, width);
            return result.PadRight(width, c);
        }

        /// <summary>
        /// Makes sure that an integer fits in a field of a given length; throws ArgumentOutOfRangeException if it goes over
        /// </summary>
        /// <param name="i">The Int64/long to turn into a string and trim; if it is too long, it will throw ArgumentOutOfRangeException</param>
        /// <param name="width">The width of the field</param>
        /// <param name="c">The padding character, default is zero</param>
        /// <returns>A string trimmed if necessary and padded with the padding string if necessary to ensure a given length</returns>
        public static string CheckAndPadLeft(this Int64 i, int width, char c = '0')
        {
            Condition.Requires(i, "i").IsGreaterOrEqual(0);
            string temp = i.ToString();
            if (temp.Length > width)
                throw new ArgumentOutOfRangeException("Integer value would be lost");
            return temp.TrimAndPadLeft(width, c);
        }

        /// <summary>
        /// Makes sure that an integer fits in a field of a given length and never goes under or over
        /// </summary>
        /// <param name="i">The int to turn into a string and trim; if the int is null it will be converted to an empty string</param>
        /// <param name="width">The width of the field</param>
        /// <param name="c">The padding character, default is zero</param>
        /// <returns>A string trimmed if necessary and padded with the padding string if necessary to ensure a given length</returns>
        public static string CheckAndPadLeft(this int i, int width, char c = '0')
        {
            return ((Int64)i).CheckAndPadLeft(width, c);
        }    
    }
}

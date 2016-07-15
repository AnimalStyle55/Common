using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Extensions
{
    /// <summary>
    /// Extenstions for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to an enumeration of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns>an enumeration of the specified type</returns>
        public static T ToEnum<T>(this string s) where T : struct, IConvertible
        {
            Condition.Requires("T must be an Enum").Evaluate(typeof(T).IsEnum);

            return (T)Enum.Parse(typeof(T), s, true);
        }

        /// <summary>
        /// Converts a string to an enumeration of the specified type, or a default value if null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="defaultVal"></param>
        /// <returns>an enumeration of the specified type, or the defaut</returns>
        public static T ToEnumOrDefault<T>(this string s, T defaultVal) where T : struct, IConvertible
        {
            return ToEnumOrNull<T>(s) ?? defaultVal;
        }

        /// <summary>
        /// Converts a string to an enumeration of the specified type, or to null if null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns>a nullable enumeration of the specified type, or null</returns>
        public static T? ToEnumOrNull<T>(this string s) where T : struct, IConvertible
        {
            Condition.Requires("T must be an Enum").Evaluate(typeof(T).IsEnum);

            T outVal;
            if (!string.IsNullOrEmpty(s) && Enum.TryParse(s, true, out outVal))
                return outVal;

            return null;
        }
    }
}
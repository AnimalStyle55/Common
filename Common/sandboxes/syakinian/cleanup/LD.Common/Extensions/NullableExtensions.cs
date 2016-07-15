using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Extensions
{
    /// <summary>
    /// Extensions for nullable types
    /// </summary>
    public static class NullableExtensions
    {
        /// <summary>
        /// Convert a nullable type to string, or null if the nullable is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static string ToStringOrNull<T>(this T? nullable) where T : struct
        {
            return nullable.HasValue ? nullable.ToString() : null;
        }
    }
}
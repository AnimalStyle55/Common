namespace Common.Extensions
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

        /// <summary>
        /// Get Nullable value without null check.
        /// </summary>
        /// <typeparam name="T">Type of the nullable.</typeparam>
        /// <param name="value">Nullable value to get.</param>
        /// <param name="defaultValue">defaultValue to return if value is null.</param>
        /// <returns>value or defaultValue if value is null</returns>
        public static T ToValueOrDefault<T>(this T? value, T defaultValue = default(T)) where T : struct
        {
            return value == null ? defaultValue : value.Value;
        }
    }
}
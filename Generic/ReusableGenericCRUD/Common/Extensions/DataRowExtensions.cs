using System.Data;

namespace Common.Extensions
{
    /// <summary>
    /// Extensions to ease getting values out of a DataRow
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>Get a value as a string</summary>
        /// <returns>string or default if unknown column or null value</returns>
        public static string AsString(this DataRow dr, string columnName, string defaultValue = "")
        {
            if (!dr.Table.Columns.Contains(columnName))
                return defaultValue;
            var value = dr[columnName];
            return value == null || dr.IsNull(columnName) ? defaultValue : value.ToString();
        }

        /// <summary>Get a value as a bool</summary>
        /// <returns>bool or default if unknown column, null value or invalid value</returns>
        public static bool AsBool(this DataRow dr, string columnName, bool defaultValue = false)
        {
            return AsBoolNull(dr, columnName) ?? defaultValue;
        }

        /// <summary>Get a value as a bool</summary>
        /// <returns>bool or null if unknown column, null value or invalid value</returns>
        public static bool? AsBoolNull(this DataRow dr, string columnName)
        {
            if (!dr.Table.Columns.Contains(columnName))
                return null;
            bool result;
            var value = dr[columnName];
            if (value == null || dr.IsNull(columnName) || !bool.TryParse(value.ToString(), out result))
                return null;
            return result;
        }

        /// <summary>Get a value as a decimal</summary>
        /// <returns>decimal or default if unknown column or null value</returns>
        public static decimal AsDecimal(this DataRow dr, string columnName, decimal defaultValue = 0m)
        {
            return AsDecimalNull(dr, columnName) ?? defaultValue;
        }

        /// <summary>Get a value as a decimal</summary>
        /// <returns>decimal or null if unknown column, null value or invalid value</returns>
        public static decimal? AsDecimalNull(this DataRow dr, string columnName)
        {
            if (!dr.Table.Columns.Contains(columnName))
                return null;
            decimal result;
            var value = dr[columnName];
            if (value == null || dr.IsNull(columnName) || !decimal.TryParse(value.ToString(), out result))
                return null;
            return result;
        }

        /// <summary>Get a value as a int</summary>
        /// <returns>int or default if unknown column, null value or invalid value</returns>
        public static int AsInt(this DataRow dr, string columnName, int defaultValue = 0)
        {
            return AsIntNull(dr, columnName) ?? defaultValue;
        }

        /// <summary>Get a value as a int, treating 0 as null</summary>
        /// <returns>int or null if unknown column, null value, invalid value, or 0</returns>
        public static int? AsIntNullZero(this DataRow dr, string columnName)
        {
            var v = AsIntNull(dr, columnName);
            return (v == 0) ? null : v;
        }

        /// <summary>Get a value as a int</summary>
        /// <returns>int or null if unknown column, null value or invalid value</returns>
        public static int? AsIntNull(this DataRow dr, string columnName)
        {
            if (!dr.Table.Columns.Contains(columnName))
                return null;
            int result;
            var value = dr[columnName];
            if (value == null || dr.IsNull(columnName) || !int.TryParse(value.ToString(), out result))
                return null;
            return result;
        }
    }
}

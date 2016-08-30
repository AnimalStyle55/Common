namespace Common.Database
{
    /// <summary>
    /// Constants for DB Code
    /// </summary>
    public static class SqlConstants
    {
        /// <summary>
        /// Put this in front of the parameter name to prevent auto-trimming
        /// e.g.   new SqlParameter(SqlConstants.DoNotTrimHeader + "@ParamName", valueStr)
        /// </summary>
        public const string DoNotTrimHeader = "NOTRIM-";

        /// <summary>
        /// Put this in front of the parameter name to explicitly set a null value to DBNull.Value
        /// e.g.   new SqlParameter(SqlConstants.NullableHeader + "@ParamName", valueStr)
        /// </summary>
        public const string NullableHeader = "NULLABLE-";
    }
}
using System;
using System.Collections.Generic;

namespace LD.Common.Database
{
    /// <summary>
    /// Represents a single row result from a database.
    /// </summary>
    public interface IResult
    {
        /// <summary>get string column</summary>
        string GetString(string name);

        /// <summary>get int column</summary>
        int GetInt(string name);

        /// <summary>get nullable int column</summary>
        int? GetIntNull(string name);

        /// <summary>get long column</summary>
        long GetLong(string name);

        /// <summary>get nullable long column</summary>
        long? GetLongNull(string name);

        /// <summary>get decimal column</summary>
        decimal GetDecimal(string name);

        /// <summary>get nullable decimal column</summary>
        decimal? GetDecimalNull(string name);

        /// <summary>get bool column</summary>
        bool GetBool(string name);

        /// <summary>get nullable bool column</summary>
        bool? GetBoolNull(string name);

        /// <summary>get DateTime column</summary>
        DateTime GetDateTime(string name, DateTimeKind kind = DateTimeKind.Utc);

        /// <summary>get nullable DateTime column</summary>
        DateTime? GetDateTimeNull(string name, DateTimeKind kind = DateTimeKind.Utc);

        /// <summary>get TimeSpan column</summary>
        TimeSpan GetTimeSpan(string name);

        /// <summary>get nullable TimeSpan column</summary>
        TimeSpan? GetTimeSpanNull(string name);

        /// <summary>get Guid column</summary>
        Guid GetGuid(string name);

        /// <summary>get nullable Guid column</summary>
        Guid? GetGuidNull(string name);

        /// <summary>get Enum column, value must match Enum value name</summary>
        T GetEnum<T>(string name) where T : struct, IConvertible;

        /// <summary>get Enum column, or default value if NULL</summary>
        T GetEnumDefault<T>(string name, T defaultVal) where T : struct, IConvertible;

        /// <summary>get Enum column, null if NULL</summary>
        T? GetEnumNull<T>(string name) where T : struct, IConvertible;

        /// <summary>Get bytes from varbinary</summary>
        byte[] GetBytes(string name);

        /// <summary>check if column is NULL</summary>
        bool IsNull(string name);

        /// <summary>Get a list of column names</summary>
        List<string> GetColumns();
    }
}
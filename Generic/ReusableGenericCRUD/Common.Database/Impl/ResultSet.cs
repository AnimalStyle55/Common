using Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Common.Database.Impl
{
    /// <remarks>
    /// All "pos" variables are 0-based
    /// These are private until a use-case presents itself where you need position access
    /// </remarks>
    internal class ResultSet : IResultSet, IResult
    {
        protected readonly SqlDataReader _reader;

        public ResultSet(SqlDataReader reader)
        {
            _reader = reader;
        }

        public virtual IResult NextResult()
        {
            if (!_reader.Read())
                return null;

            return this;
        }

        /// <summary>
        /// Dispose of the result set
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
        }

        public virtual IEnumerator<IResult> GetEnumerator()
        {
            while (_reader.Read())
            {
                yield return this;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private string GetString(int pos) => IsNull(pos) ? null : _reader.GetString(pos);

        public string GetString(string name) => GetString(_reader.GetOrdinal(name));

        private int GetInt(int pos) => _reader.GetInt32(pos);

        public int GetInt(string name) => GetInt(_reader.GetOrdinal(name));

        private int? GetIntNull(int pos) => IsNull(pos) ? (int?)null : GetInt(pos);

        public int? GetIntNull(string name) => GetIntNull(_reader.GetOrdinal(name));

        private long GetLong(int pos) => _reader.GetInt64(pos);

        public long GetLong(string name) => GetLong(_reader.GetOrdinal(name));

        private long? GetLongNull(int pos) => IsNull(pos) ? (long?)null : GetLong(pos);

        public long? GetLongNull(string name) => GetLongNull(_reader.GetOrdinal(name));

        private decimal GetDecimal(int pos) => _reader.GetDecimal(pos);

        public decimal GetDecimal(string name) => GetDecimal(_reader.GetOrdinal(name));

        private decimal? GetDecimalNull(int pos) => IsNull(pos) ? (decimal?)null : GetDecimal(pos);

        public decimal? GetDecimalNull(string name) => GetDecimalNull(_reader.GetOrdinal(name));

        private bool GetBool(int pos) => _reader.GetBoolean(pos);

        public bool GetBool(string name) => GetBool(_reader.GetOrdinal(name));

        private bool? GetBoolNull(int pos) => IsNull(pos) ? (bool?)null : GetBool(pos);

        public bool? GetBoolNull(string name) => GetBoolNull(_reader.GetOrdinal(name));

        private DateTime GetDateTime(int pos, DateTimeKind kind = DateTimeKind.Utc) => DateTime.SpecifyKind(_reader.GetDateTime(pos), kind);

        public DateTime GetDateTime(string name, DateTimeKind kind = DateTimeKind.Utc) => GetDateTime(_reader.GetOrdinal(name), kind);

        private DateTime? GetDateTimeNull(int pos, DateTimeKind kind = DateTimeKind.Utc) => IsNull(pos) ? (DateTime?)null : GetDateTime(pos, kind);

        public DateTime? GetDateTimeNull(string name, DateTimeKind kind = DateTimeKind.Utc) => GetDateTimeNull(_reader.GetOrdinal(name), kind);

        private Guid GetGuid(int pos) => _reader.GetGuid(pos);

        public Guid GetGuid(string name) => GetGuid(_reader.GetOrdinal(name));

        private Guid? GetGuidNull(int pos) => IsNull(pos) ? (Guid?)null : GetGuid(pos);

        public Guid? GetGuidNull(string name) => GetGuidNull(_reader.GetOrdinal(name));

        private T GetEnum<T>(int pos) where T : struct, IConvertible => GetString(pos).ToEnum<T>();

        public T GetEnum<T>(string name) where T : struct, IConvertible => GetString(name).ToEnum<T>();

        private T GetEnumDefault<T>(int pos, T defaultVal) where T : struct, IConvertible => GetString(pos).ToEnumOrDefault(defaultVal);

        public T GetEnumDefault<T>(string name, T defaultVal) where T : struct, IConvertible => GetString(name).ToEnumOrDefault(defaultVal);

        private T? GetEnumNull<T>(int pos) where T : struct, IConvertible => GetString(pos).ToEnumOrNull<T>();

        public T? GetEnumNull<T>(string name) where T : struct, IConvertible => GetString(name).ToEnumOrNull<T>();

        private byte[] GetBytes(int pos) => (byte[])_reader.GetValue(pos);

        public byte[] GetBytes(string name) => GetBytes(_reader.GetOrdinal(name));

        private bool IsNull(int pos) => _reader.IsDBNull(pos);

        public bool IsNull(string name) => IsNull(_reader.GetOrdinal(name));

        public List<string> GetColumns() => Enumerable.Range(0, _reader.FieldCount)
                 .Select(i => _reader.GetName(i))
                 .ToList();

        private TimeSpan GetTimeSpan(int pos) => (TimeSpan)_reader.GetValue(pos);

        public TimeSpan GetTimeSpan(string name) => GetTimeSpan(_reader.GetOrdinal(name));

        private TimeSpan? GetTimeSpanNull(int pos) => IsNull(pos) ? (TimeSpan?)null : GetTimeSpan(pos);

        public TimeSpan? GetTimeSpanNull(string name) => GetTimeSpanNull(_reader.GetOrdinal(name));
    }
}
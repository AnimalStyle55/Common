using LD.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LD.Common.Database.Impl
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string GetString(int pos)
        {
            return IsNull(pos) ? null : _reader.GetString(pos);
        }

        public string GetString(string name)
        {
            return GetString(_reader.GetOrdinal(name));
        }

        private int GetInt(int pos)
        {
            return _reader.GetInt32(pos);
        }

        public int GetInt(string name)
        {
            return GetInt(_reader.GetOrdinal(name));
        }

        private int? GetIntNull(int pos)
        {
            return IsNull(pos) ? (int?)null : GetInt(pos);
        }

        public int? GetIntNull(string name)
        {
            return GetIntNull(_reader.GetOrdinal(name));
        }

        private long GetLong(int pos)
        {
            return _reader.GetInt64(pos);
        }

        public long GetLong(string name)
        {
            return GetLong(_reader.GetOrdinal(name));
        }

        private long? GetLongNull(int pos)
        {
            return IsNull(pos) ? (long?)null : GetLong(pos);
        }

        public long? GetLongNull(string name)
        {
            return GetLongNull(_reader.GetOrdinal(name));
        }

        private decimal GetDecimal(int pos)
        {
            return _reader.GetDecimal(pos);
        }

        public decimal GetDecimal(string name)
        {
            return GetDecimal(_reader.GetOrdinal(name));
        }

        private decimal? GetDecimalNull(int pos)
        {
            return IsNull(pos) ? (decimal?)null : GetDecimal(pos);
        }

        public decimal? GetDecimalNull(string name)
        {
            return GetDecimalNull(_reader.GetOrdinal(name));
        }

        private bool GetBool(int pos)
        {
            return _reader.GetBoolean(pos);
        }

        public bool GetBool(string name)
        {
            return GetBool(_reader.GetOrdinal(name));
        }

        private bool? GetBoolNull(int pos)
        {
            return IsNull(pos) ? (bool?)null : GetBool(pos);
        }

        public bool? GetBoolNull(string name)
        {
            return GetBoolNull(_reader.GetOrdinal(name));
        }

        private DateTime GetDateTime(int pos, DateTimeKind kind = DateTimeKind.Utc)
        {
            return DateTime.SpecifyKind(_reader.GetDateTime(pos), kind);
        }

        public DateTime GetDateTime(string name, DateTimeKind kind = DateTimeKind.Utc)
        {
            return GetDateTime(_reader.GetOrdinal(name), kind);
        }

        private DateTime? GetDateTimeNull(int pos, DateTimeKind kind = DateTimeKind.Utc)
        {
            return IsNull(pos) ? (DateTime?)null : GetDateTime(pos, kind);
        }

        public DateTime? GetDateTimeNull(string name, DateTimeKind kind = DateTimeKind.Utc)
        {
            return GetDateTimeNull(_reader.GetOrdinal(name), kind);
        }

        private Guid GetGuid(int pos)
        {
            return _reader.GetGuid(pos);
        }

        public Guid GetGuid(string name)
        {
            return GetGuid(_reader.GetOrdinal(name));
        }

        private Guid? GetGuidNull(int pos)
        {
            return IsNull(pos) ? (Guid?)null : GetGuid(pos);
        }

        public Guid? GetGuidNull(string name)
        {
            return GetGuidNull(_reader.GetOrdinal(name));
        }

        private T GetEnum<T>(int pos) where T : struct, IConvertible
        {
            return GetString(pos).ToEnum<T>();
        }

        public T GetEnum<T>(string name) where T : struct, IConvertible
        {
            return GetString(name).ToEnum<T>();
        }

        private T GetEnumDefault<T>(int pos, T defaultVal) where T : struct, IConvertible
        {
            return GetString(pos).ToEnumOrDefault(defaultVal);
        }

        public T GetEnumDefault<T>(string name, T defaultVal) where T : struct, IConvertible
        {
            return GetString(name).ToEnumOrDefault(defaultVal);
        }

        private T? GetEnumNull<T>(int pos) where T : struct, IConvertible
        {
            return GetString(pos).ToEnumOrNull<T>();
        }

        public T? GetEnumNull<T>(string name) where T : struct, IConvertible
        {
            return GetString(name).ToEnumOrNull<T>();
        }

        private byte[] GetBytes(int pos)
        {
            return (byte[])_reader.GetValue(pos);
        }

        public byte[] GetBytes(string name)
        {
            return GetBytes(_reader.GetOrdinal(name));
        }

        private bool IsNull(int pos)
        {
            return _reader.IsDBNull(pos);
        }

        public bool IsNull(string name)
        {
            return IsNull(_reader.GetOrdinal(name));
        }

        public List<string> GetColumns()
        {
            return Enumerable.Range(0, _reader.FieldCount)
                             .Select(i => _reader.GetName(i))
                             .ToList();
        }

        private TimeSpan GetTimeSpan(int pos)
        {
            return (TimeSpan)_reader.GetValue(pos);
        }

        public TimeSpan GetTimeSpan(string name)
        {
            return GetTimeSpan(_reader.GetOrdinal(name));
        }

        private TimeSpan? GetTimeSpanNull(int pos)
        {
            return IsNull(pos) ? (TimeSpan?)null : GetTimeSpan(pos);
        }

        public TimeSpan? GetTimeSpanNull(string name)
        {
            return GetTimeSpanNull(_reader.GetOrdinal(name));
        }
    }
}
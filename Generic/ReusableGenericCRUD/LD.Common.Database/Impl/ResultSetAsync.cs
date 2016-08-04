using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Common.Database.Impl
{
    internal class ResultSetAsync : ResultSet
    {
        public ResultSetAsync(SqlDataReader reader) : base(reader)
        {
        }

        public override IResult NextResult()
        {
            throw new NotSupportedException("NextResult not supported for async");
        }

        public override IEnumerator<IResult> GetEnumerator()
        {
            throw new NotSupportedException("GetEnumerator not supported for async");
        }

        public async Task<bool> NextResultAsync()
        {
            if (!await _reader.ReadAsync())
                return false;

            return true;
        }
    }
}
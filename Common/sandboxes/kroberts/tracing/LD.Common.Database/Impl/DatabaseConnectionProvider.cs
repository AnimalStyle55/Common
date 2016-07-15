using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LD.Common.Database.Impl
{
    internal class DatabaseConnectionProvider : IDatabaseConnectionProvider
    {
        private readonly string _connStr;

        public DatabaseConnectionProvider(string connString)
        {
            _connStr = connString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connStr);
        }
    }
}
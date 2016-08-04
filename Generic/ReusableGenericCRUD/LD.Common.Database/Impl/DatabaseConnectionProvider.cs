using System.Data.SqlClient;

namespace Common.Database.Impl
{
    internal class DatabaseConnectionProvider : IDatabaseConnectionProvider
    {
        private readonly string _connStr;

        public DatabaseConnectionProvider(string connString)
        {
            _connStr = connString;
        }

        public SqlConnection GetConnection() => new SqlConnection(_connStr);
    }
}
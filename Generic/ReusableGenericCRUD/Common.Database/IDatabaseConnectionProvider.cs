using System.Data.SqlClient;

namespace Common.Database
{
    internal interface IDatabaseConnectionProvider
    {
        /// <summary>
        /// Get a SqlConnection from the pool
        /// </summary>
        /// <returns></returns>
        SqlConnection GetConnection();
    }
}
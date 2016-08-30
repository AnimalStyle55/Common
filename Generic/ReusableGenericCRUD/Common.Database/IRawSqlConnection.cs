using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Common.Database
{
    /// <summary>
    /// Interface for executing RAW SQL as well as stored procedures
    /// </summary>
    public interface IRawSqlConnection : ISqlConnection
    {
        /// <summary>
        /// Execute a raw SQL command
        /// </summary>
        /// <param name="sql">The SQL query text</param>
        /// <param name="parameters"></param>
        /// <returns>the row count of modified rows</returns>
        int ExecuteRaw(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a raw SQL query
        /// </summary>
        /// <param name="sql">The SQL query text</param>
        /// <param name="parameters"></param>
        /// <returns>An IEnumerable that can be interated on to get each returned row</returns>
        IEnumerable<IResult> ExecuteRawQuery(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a raw SQL query
        /// </summary>
        /// <param name="sql">The SQL query text</param>
        /// <param name="func">A function that converts a row to an object (only called if a row was returned)</param>
        /// <param name="parameters"></param>
        /// <returns>An IEnumerable that can be interated on to get each returned row</returns>
        IList<T> ExecuteRawQueryList<T>(string sql, Func<IResult, T> func, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a raw SQL command, async
        /// </summary>
        /// <param name="sql">The SQL query text</param>
        /// <param name="parameters"></param>
        /// <returns>the row count of modified rows</returns>
        Task<int> ExecuteRawAsync(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a raw SQL query, async
        /// </summary>
        /// <param name="sql">The SQL query text</param>
        /// <param name="func">A function that converts a row to an object (only called if a row was returned), should be async</param>
        /// <param name="parameters"></param>
        /// <returns>An IEnumerable that can be interated on to get each returned row</returns>
        Task<IList<T>> ExecuteRawQueryListAsync<T>(string sql, Func<IResult, Task<T>> func, params SqlParameter[] parameters);
    }
}
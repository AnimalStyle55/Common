using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Common.Database
{
    /// <summary>
    /// Interface for SQL Connections
    /// </summary>
    public interface ISqlConnection
    {
        /// <summary>
        /// Set this connection to roll back at the end of a transaction
        /// without having to throw an exception
        /// </summary>
        void SetRollback();

        /// <summary>
        /// Determine if this connection/transaction has been set for rollback
        /// </summary>
        bool WillBeRollingBack();

        /// <summary>
        /// The time in seconds to wait for a command to execute before throwing an error.
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// Execute a stored procedure that returns no results
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <exception cref="DatabaseException">thrown with ExpectedRowsMismatch if proc returns 0 rows modified</exception>
        void ExecuteProc(string procName, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns no results, check for a specific # of rows returned
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="expectedRows">number of rows expected</param>
        /// <param name="parameters"></param>
        /// <exception cref="DatabaseException">thrown with ExpectedRowsMismatch if #rows modified does not equal expectedRows</exception>
        void ExecuteProc(string procName, int expectedRows, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns no results, does not check expected rows
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <returns>number of rows affected</returns>
        int ExecuteProcUnchecked(string procName, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure and return results as an enumerable
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <returns>An IEnumerable that can be interated on to get each returned row</returns>
        IEnumerable<IResult> ExecuteQueryProc(string procName, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns a list of results
        /// </summary>
        /// <typeparam name="T">type of the objects in the list</typeparam>
        /// <param name="procName"></param>
        /// <param name="readerFunc">A function that converts a row to an object (only called if a row was returned)</param>
        /// <param name="parameters"></param>
        /// <returns>A List of objects, always non-null</returns>
        List<T> ExecuteQueryProcList<T>(string procName, Func<IResult, T> readerFunc, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns a single row
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="readerFunc">An action that will be called with a result (only called if a row was returned)</param>
        /// <param name="parameters"></param>
        /// <returns>true if a row was found, false otherwise</returns>
        bool ExecuteQueryProcOne(string procName, Action<IResult> readerFunc, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns a single row
        /// </summary>
        /// <typeparam name="T">type of the single value to return</typeparam>
        /// <param name="procName"></param>
        /// <param name="readerFunc">A function that extracts a single value from a result (only called if a row was returned).</param>
        /// <param name="parameters"></param>
        /// <returns>A SqlSingleResult containing the return value from readerFunc, or default(T) if no results returned,
        /// and a bool containing whether a row was returned or not</returns>
        SingleResult<T> ExecuteQueryProcOne<T>(string procName, Func<IResult, T> readerFunc, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns no results, async
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <exception cref="DatabaseException">thrown with ExpectedRowsMismatch if proc returns 0 rows modified</exception>
        Task ExecuteProcAsync(string procName, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns no results, check for a specific # of rows returned, async
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="expectedRows"></param>
        /// <param name="parameters"></param>
        /// <exception cref="DatabaseException">thrown with ExpectedRowsMismatch if proc returns 0 rows modified</exception>
        Task ExecuteProcAsync(string procName, int expectedRows, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns no results, does not check expected rows, async
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <returns>number of rows affected</returns>
        Task<int> ExecuteProcUncheckedAsync(string procName, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns a list of results, async
        /// </summary>
        /// <typeparam name="T">type of the objects in the list</typeparam>
        /// <param name="procName"></param>
        /// <param name="readerFunc">A function that converts a row to an object (only called if a row was returned), should be async</param>
        /// <param name="parameters"></param>
        /// <returns>A List of objects, always non-null</returns>
        Task<List<T>> ExecuteQueryProcListAsync<T>(string procName, Func<IResult, Task<T>> readerFunc, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns a single row, async
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="readerFunc">An action that will be called with a result (only called if a row was returned), should be async</param>
        /// <param name="parameters"></param>
        /// <returns>true if a row was found, false otherwise</returns>
        Task<bool> ExecuteQueryProcOneAsync(string procName, Func<IResult, Task> readerFunc, params SqlParameter[] parameters);

        /// <summary>
        /// Execute a stored procedure that returns a single row, async
        /// </summary>
        /// <typeparam name="T">type of the single value to return</typeparam>
        /// <param name="procName"></param>
        /// <param name="readerFunc">A function that extracts a single value from a result (only called if a row was returned), should be async</param>
        /// <param name="parameters"></param>
        /// <returns>A SqlSingleResult containing the return value from readerFunc, or default(T) if no results returned,
        /// and a bool containing whether a row was returned or not</returns>
        Task<SingleResult<T>> ExecuteQueryProcOneAsync<T>(string procName, Func<IResult, Task<T>> readerFunc, params SqlParameter[] parameters);
    }
}
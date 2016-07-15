using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LD.Common.Database
{
    /// <summary>
    /// A Sql Connection that will defer the execution of database writes
    /// </summary>
    public interface IDeferredSqlConnection : ISqlConnection
    {
        /// <summary>
        /// Add a deferred call to ExecuteProc
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="checkRowsReturned">true to check for >0 rows modified when executed, false to not check</param>
        /// <param name="parameters"></param>
        void DeferProc(string procName, bool checkRowsReturned, params SqlParameter[] parameters);

        /// <summary>
        /// Add a deferred call to ExecuteProc
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="expectedRows"># of expected rows modified</param>
        /// <param name="parameters"></param>
        void DeferProc(string procName, int expectedRows, params SqlParameter[] parameters);

        /// <summary>
        /// Execute all deferred calls
        /// </summary>
        /// <param name="conn"></param>
        /// <exception cref="DatabaseException">thrown if any deferred call doesn't meet expected rows</exception>
        void ExecuteAll(ISqlConnection conn);

        /// <summary>
        /// Execute all deferred calls, async
        /// </summary>
        /// <param name="conn"></param>
        /// <exception cref="DatabaseException">thrown if any deferred call doesn't meet expected rows</exception>
        Task ExecuteAllAsync(ISqlConnection conn);
    }
}
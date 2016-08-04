using System;
using System.Threading.Tasks;

namespace Common.Database
{
    /// <summary>
    /// Provides the boundaries of a transaction
    /// 
    /// NOTE: Any Func or Action that is passed may be recalled if a transient error occurs (such as Deadlock)
    ///       Be cautious about doing anything that cannot be undone inside a DB transaction.
    ///       This can be disabled when registering the transaction factory
    /// </summary>
    public interface ISqlTransactionProvider
    {
        /// <summary>
        /// Execute the Action within a new transaction
        /// </summary>
        /// <param name="action">action to be called with connection</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        void ExecuteTxn(Action<ISqlConnection> action, int? commandTimeout = null);

        /// <summary>
        /// Execute the function within a new transaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">action to be called with connection</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        /// <returns>the value returned from func</returns>
        T ExecuteTxn<T>(Func<ISqlConnection, T> func, int? commandTimeout = null);

        /// <summary>
        /// Execute the Action within a new transaction, suitable for raw sql queries
        /// </summary>
        /// <param name="action">action to be called with connection</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        void ExecuteRawTxn(Action<IRawSqlConnection> action, int? commandTimeout = null);

        /// <summary>
        /// Execute the function within a new transaction, suitable for raw sql queries
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">action to be called with connection</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        /// <returns>the value returned from func</returns>
        T ExecuteRawTxn<T>(Func<IRawSqlConnection, T> func, int? commandTimeout = null);

        /// <summary>
        /// Execute the Action within a new transaction, async
        /// </summary>
        /// <param name="action">action to be called with connection, should be async</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        Task ExecuteTxnAsync(Func<ISqlConnection, Task> action, int? commandTimeout = null);

        /// <summary>
        /// Execute the function within a new transaction, async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">action to be called with connection, should be async</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        /// <returns>the T value returned from func</returns>
        Task<T> ExecuteTxnAsync<T>(Func<ISqlConnection, Task<T>> func, int? commandTimeout = null);

        /// <summary>
        /// Execute the Action within a new transaction, suitable for raw sql queries, async
        /// </summary>
        /// <param name="action">action to be called with connection, should be async</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        Task ExecuteRawTxnAsync(Func<IRawSqlConnection, Task> action, int? commandTimeout = null);

        /// <summary>
        /// Execute the function within a new transaction, suitable for raw sql queries, async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">action to be called with connection, should be async</param>
        /// <param name="commandTimeout">timeout for sql commands (seconds), null for default</param>
        /// <returns>the T value returned from func</returns>
        Task<T> ExecuteRawTxnAsync<T>(Func<IRawSqlConnection, Task<T>> func, int? commandTimeout = null);

        /// <summary>
        /// Get a deferred sql connection
        /// </summary>
        /// <returns></returns>
        IDeferredSqlConnection GetDeferred();

        /// <summary>
        /// Shortcut method to execute all the deferred calls in a deferred connection
        /// </summary>
        /// <param name="deferredConn"></param>
        void ExecuteDeferred(IDeferredSqlConnection deferredConn);

        /// <summary>
        /// Shortcut method to execute all the deferred calls in a deferred connection, async
        /// </summary>
        /// <param name="deferredConn"></param>
        Task ExecuteDeferredAsync(IDeferredSqlConnection deferredConn);
    }
}
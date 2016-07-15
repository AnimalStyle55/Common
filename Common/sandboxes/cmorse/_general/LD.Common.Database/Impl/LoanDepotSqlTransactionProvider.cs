using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LD.Common.Database.Impl
{
    internal class LoanDepotSqlTransactionProvider : ISqlTransactionProvider
    {
        private readonly IDatabaseConnectionProvider _connProv;

        private readonly int _defaultCommandTimeout;
        private readonly RetryPolicy _retryPolicy;

        public LoanDepotSqlTransactionProvider(IDatabaseConnectionProvider prov, int defaultCommandTimeout, RetryPolicy retryPolicy)
        {
            _defaultCommandTimeout = defaultCommandTimeout;
            _connProv = prov;
            _retryPolicy = retryPolicy;
        }

        public IDeferredSqlConnection GetDeferred()
        {
            return new DeferredSqlConnection(_defaultCommandTimeout);
        }

        public void ExecuteTxn(Action<ISqlConnection> action, int? commandTimeout = null)
        {
            ExecuteRawTxn(action, commandTimeout);
        }

        public T ExecuteTxn<T>(Func<ISqlConnection, T> func, int? commandTimeout = null)
        {
            return ExecuteRawTxn(func, commandTimeout);
        }

        public void ExecuteRawTxn(Action<IRawSqlConnection> action, int? commandTimeout = null)
        {
            ExecuteRawTxn(c =>
                {
                    action.Invoke(c);
                    return true;
                },
                commandTimeout);
        }

        public T ExecuteRawTxn<T>(Func<IRawSqlConnection, T> func, int? commandTimeout = null)
        {
            return _retryPolicy.ExecuteAction(() =>
            {
                using (SqlConnection conn = _connProv.GetConnection())
                {
                    conn.Open();

                    using (var ldTxn = new LoanDepotSqlTransaction(conn, commandTimeout ?? _defaultCommandTimeout))
                    {
                        try
                        {
                            return func.Invoke(ldTxn.Connection);
                        }
                        catch (Exception)
                        {
                            ldTxn.Connection.SetRollback();
                            throw;
                        }
                    }
                }
            });
        }

        public async Task ExecuteTxnAsync(Func<ISqlConnection, Task> action, int? commandTimeout = null)
        {
            await ExecuteRawTxnAsync(action, commandTimeout);
        }

        public async Task<T> ExecuteTxnAsync<T>(Func<ISqlConnection, Task<T>> func, int? commandTimeout = null)
        {
            return await ExecuteRawTxnAsync(func, commandTimeout);
        }

        public async Task ExecuteRawTxnAsync(Func<IRawSqlConnection, Task> action, int? commandTimeout = null)
        {
            await ExecuteRawTxnAsync(async c =>
                {
                    await action.Invoke(c);
                    return true;
                },
                commandTimeout);
        }

        public async Task<T> ExecuteRawTxnAsync<T>(Func<IRawSqlConnection, Task<T>> func, int? commandTimeout = null)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using (SqlConnection conn = _connProv.GetConnection())
                {
                    await conn.OpenAsync();

                    using (var ldTxn = new LoanDepotSqlTransaction(conn, commandTimeout ?? _defaultCommandTimeout))
                    {
                        try
                        {
                            return await func.Invoke(ldTxn.Connection);
                        }
                        catch (Exception)
                        {
                            ldTxn.Connection.SetRollback();
                            throw;
                        }
                    }
                }
            });
        }

        public void ExecuteDeferred(IDeferredSqlConnection deferredConn)
        {
            ExecuteRawTxn(c =>
                {
                    deferredConn.ExecuteAll(c);
                    return true;
                }
            );
        }

        public async Task ExecuteDeferredAsync(IDeferredSqlConnection deferredConn)
        {
            await ExecuteRawTxnAsync(async c =>
                {
                    await deferredConn.ExecuteAllAsync(c);
                    return true;
                }
            );
        }
    }
}
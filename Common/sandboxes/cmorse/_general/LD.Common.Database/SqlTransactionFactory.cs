using Common.Logging;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Database
{
    using Impl;
    using System.Data.SqlClient;
    /// <summary>
    /// A factory for getting SQL connections
    /// </summary>
    public class SqlTransactionFactory
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        private class TxnFactory
        {
            public IDatabaseConnectionProvider ConnProvider { get; set; }
            public int TimeoutSeconds { get; set; }
            public bool DisableRetry { get; set; }
        }

        private readonly IDictionary<string, TxnFactory> _connStrMap = new Dictionary<string, TxnFactory>();
        private readonly int _defaultCommandTimeout;
        private readonly RetryPolicy _retryPolicy;
        private readonly RetryPolicy _noopRetryPolicy;

        private string _defaultConnName;

        /// <summary>
        ///
        /// </summary>
        /// <param name="defaultName"></param>
        /// <param name="defaultConnStr"></param>
        /// <param name="defaultCommandTimeout">default timeout for sql commands in seconds</param>
        /// <param name="retryCount">how many times to retry on transient failures</param>
        /// <param name="retryIntervalMs">how often to retry on transient failures</param>
        /// <param name="retryFastFirst">if true, 1st retry will be immediate</param>
        /// <param name="disableTransientRetry">if true, the transaction will not retry on transient errors</param>
        public SqlTransactionFactory(string defaultName,
            string defaultConnStr,
            int defaultCommandTimeout = 30,
            int retryCount = 3,
            int retryIntervalMs = 200,
            bool retryFastFirst = true,
            bool disableTransientRetry = false)
        {
            MapConnectionString(defaultName, defaultConnStr, defaultCommandTimeout, disableTransientRetry);
            _defaultConnName = defaultName;
            _defaultCommandTimeout = defaultCommandTimeout;

            var strategy = new FixedInterval("LD", retryCount, TimeSpan.FromMilliseconds(retryIntervalMs), retryFastFirst);

            _retryPolicy = new RetryPolicy<LDSqlDatabaseTransientErrorDetectionStrategy>(strategy);
            _noopRetryPolicy = new RetryPolicy<NOOPTransientErrorDetectionStrategy>(new FixedInterval(0));
        }

        /// <summary>
        /// Map a connection name to a connection string for later retrieval from the factory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionStr"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="disableTransientRetry">if true, the transaction will not retry on transient errors</param>
        public void MapConnectionString(string name, string connectionStr, int commandTimeout = 30, bool disableTransientRetry = false)
        {
            _connStrMap[name] = new TxnFactory()
            {
                ConnProvider = new DatabaseConnectionProvider(connectionStr),
                TimeoutSeconds = commandTimeout,
                DisableRetry = disableTransientRetry
            };
        }

        /// <summary>
        /// Map the default unnamed connection string for later retrieval from the factory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionStr"></param>
        /// <param name="commandTimeout"></param>
        public void MapDefaultConnectionString(string name, string connectionStr, int commandTimeout = 30)
        {
            MapConnectionString(name, connectionStr, commandTimeout);
            _defaultConnName = name;
        }

        /// <summary>
        /// Get a transaction provider from the factory
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISqlTransactionProvider Get(string name)
        {
            var txnFac = _connStrMap[name];
            return new LoanDepotSqlTransactionProvider(txnFac.ConnProvider, txnFac.TimeoutSeconds, txnFac.DisableRetry ? _noopRetryPolicy : _retryPolicy);
        }

        /// <summary>
        /// Get the default transaction provider from the factory
        /// </summary>
        /// <returns></returns>
        public ISqlTransactionProvider GetDefault()
        {
            return Get(_defaultConnName);
        }

        private class NOOPTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                return false;
            }
        }

        private class LDSqlDatabaseTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
        {
            private readonly SqlDatabaseTransientErrorDetectionStrategy _strategy = new SqlDatabaseTransientErrorDetectionStrategy();

            public bool IsTransient(Exception ex)
            {
                bool isTransient = false;

                SqlException sqex = ex as SqlException;
                if (sqex != null)
                {
                    // Enumerate through all errors found in the exception.
                    foreach (SqlError err in sqex.Errors)
                    {
                        switch (err.Number)
                        {
                            case -2:   // timeout
                            case 1205: // deadlock
                                isTransient = true;
                                break;

                            default:
                                // Intentionally left blank here.
                                break;
                        }
                    }
                }

                isTransient |= _strategy.IsTransient(ex);

                if (isTransient)
                    log.Warn($"Found transient error, will retry transaction {ex.GetType().Name}: {ex.Message}");

                return isTransient;
            }
        }
    }
}
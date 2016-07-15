using Common.Logging;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Database
{
    using Impl;

    /// <summary>
    /// A factory for getting SQL connections
    /// </summary>
    public class SqlTransactionFactory
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        private readonly IDictionary<string, Tuple<IDatabaseConnectionProvider, int>> _connStrMap = new Dictionary<string, Tuple<IDatabaseConnectionProvider, int>>();
        private readonly int _defaultCommandTimeout;
        private readonly RetryPolicy _retryPolicy;

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
        public SqlTransactionFactory(string defaultName,
            string defaultConnStr,
            int defaultCommandTimeout = 30,
            int retryCount = 3,
            int retryIntervalMs = 200,
            bool retryFastFirst = true)
        {
            MapConnectionString(defaultName, defaultConnStr, defaultCommandTimeout);
            _defaultConnName = defaultName;
            _defaultCommandTimeout = defaultCommandTimeout;

            var strategy = new FixedInterval("LD", retryCount, TimeSpan.FromMilliseconds(retryIntervalMs), retryFastFirst);

            _retryPolicy = new RetryPolicy<LDSqlDatabaseTransientErrorDetectionStrategy>(strategy);
        }

        /// <summary>
        /// Map a connection name to a connection string for later retrieval from the factory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionStr"></param>
        /// <param name="commandTimeout"></param>
        public void MapConnectionString(string name, string connectionStr, int commandTimeout = 30)
        {
            _connStrMap[name] = Tuple.Create<IDatabaseConnectionProvider, int>(new DatabaseConnectionProvider(connectionStr), commandTimeout);
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
            var tuple = _connStrMap[name];
            return new LoanDepotSqlTransactionProvider(tuple.Item1, tuple.Item2, _retryPolicy);
        }

        /// <summary>
        /// Get the default transaction provider from the factory
        /// </summary>
        /// <returns></returns>
        public ISqlTransactionProvider GetDefault()
        {
            return Get(_defaultConnName);
        }

        private class LDSqlDatabaseTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                // place transient error detection here
                return false;
            }
        }
    }
}
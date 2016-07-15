using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LD.Common.Database.Impl
{
    internal class LoanDepotSqlTransaction : IDisposable
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        public bool IsOpen { get; private set; }

        public LoanDepotSqlConnection Connection { get; private set; }

        private readonly SqlTransaction _txn;
        private readonly SqlConnection _conn;

        public LoanDepotSqlTransaction(SqlConnection conn, int commandTimeout)
        {
            _conn = conn;
            _txn = _conn.BeginTransaction();
            IsOpen = true;
            Connection = new LoanDepotSqlConnection(_conn, this, commandTimeout);

            log.Debug("BEGIN TRANSACTION");
        }

        internal SqlCommand GetCommand(string commandText, CommandType type, int commandTimeout)
        {
            return new SqlCommand(commandText, _conn)
            {
                CommandTimeout = commandTimeout,
                Transaction = _txn,
                CommandType = type
            };
        }

        public void Dispose()
        {
            if (Connection.WillBeRollingBack())
            {
                log.Info("ROLLBACK TRANSACTION");
                _txn.Rollback();
            }
            else
            {
                log.Debug("COMMIT TRANSACTION");
                _txn.Commit();
            }

            log.Debug("END TRANSACTION");
            IsOpen = false;
        }

        internal void CheckIsOpen()
        {
            if (!IsOpen)
                throw new DatabaseException("Transaction is not open!");
        }
    }
}
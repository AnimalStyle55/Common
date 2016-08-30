using Common.Logging;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Common.Database.Impl
{
    internal class CommonSqlTransaction : IDisposable
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        public bool IsOpen { get; private set; }

        public CommonSqlConnection Connection { get; private set; }

        private readonly SqlTransaction _txn;
        private readonly SqlConnection _conn;

        public CommonSqlTransaction(SqlConnection conn, int commandTimeout)
        {
            _conn = conn;
            _txn = _conn.BeginTransaction();
            IsOpen = true;
            Connection = new CommonSqlConnection(_conn, this, commandTimeout);

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
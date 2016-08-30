using Common.Logging;
using CuttingEdge.Conditions;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Database.Impl
{
    internal class CommonSqlConnection : ISqlConnection, IRawSqlConnection
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        private readonly SqlConnection _conn;
        private readonly CommonSqlTransaction _ldTxn;

        private bool _rollback;

        /// <summary>
        /// The time in seconds to wait for a command to execute before throwing an error.
        /// </summary>
        public int CommandTimeout { get; set; }

        public CommonSqlConnection(SqlConnection conn, CommonSqlTransaction txn, int commandTimeout)
        {
            _conn = conn;
            _ldTxn = txn;
            CommandTimeout = commandTimeout;

            _ldTxn.CheckIsOpen();
        }

        public void SetRollback()
        {
            _rollback = true;
        }

        public bool WillBeRollingBack() => _rollback;

        private SqlCommand PrepareCmd(string commandText, CommandType type, SqlParameter[] parameters)
        {
            Condition.Requires(commandText, nameof(commandText)).IsNotNull();

            _ldTxn.CheckIsOpen();

            SqlCommand cmd = _ldTxn.GetCommand(commandText, type, CommandTimeout);

            foreach (var p in parameters)
            {
                SqlParameterProcessor.ProcessParam(p);
                cmd.Parameters.Add(p);
            }

            if (log.IsDebugEnabled)
            {
                if (cmd.CommandType == CommandType.StoredProcedure)
                    log.Debug(StoredProcLogger.ExecToString(commandText, parameters));
                else
                    log.Debug(StoredProcLogger.RawExecToString(commandText, parameters));
            }

            return cmd;
        }

        #region proc

        public int ExecuteProcUnchecked(string procName, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(procName, CommandType.StoredProcedure, parameters);

            using (new TimeUtil.TimeLogger("ExecuteProc", log))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public void ExecuteProc(string procName, params SqlParameter[] parameters)
        {
            if (ExecuteProcUnchecked(procName, parameters) <= 0)
                throw new DatabaseException("expected #rows > 0", DatabaseExceptionType.ExpectedRowsMismatch);
        }

        public void ExecuteProc(string procName, int expectedRows, params SqlParameter[] parameters)
        {
            int rows = ExecuteProcUnchecked(procName, parameters);
            if (rows != expectedRows)
                throw new DatabaseException($"expected {expectedRows} got {rows}", DatabaseExceptionType.ExpectedRowsMismatch);
        }

        public IEnumerable<IResult> ExecuteQueryProc(string procName, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(procName, CommandType.StoredProcedure, parameters);

            using (new TimeUtil.TimeLogger("ExecuteQueryProc", log))
            using (var rs = new ResultSet(cmd.ExecuteReader()))
            {
                foreach (var r in rs)
                    yield return r;
            }
        }

        public bool ExecuteQueryProcOne(string procName, Action<IResult> readerFunc, params SqlParameter[] parameters)
        {
            foreach (var r in ExecuteQueryProc(procName, parameters))
            {
                readerFunc.Invoke(r);
                return true;
            }

            return false;
        }

        public SingleResult<T> ExecuteQueryProcOne<T>(string procName, Func<IResult, T> readerFunc, params SqlParameter[] parameters)
        {
            foreach (var r in ExecuteQueryProc(procName, parameters))
            {
                return new SingleResult<T>(readerFunc.Invoke(r), true);
            }

            return new SingleResult<T>(default(T), false);
        }

        public List<T> ExecuteQueryProcList<T>(string procName, Func<IResult, T> readerFunc, params SqlParameter[] parameters) => ExecuteQueryProc(procName, parameters).Select(readerFunc).ToList();

        #endregion proc

        #region proc async

        public async Task<int> ExecuteProcUncheckedAsync(string procName, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(procName, CommandType.StoredProcedure, parameters);

            using (new TimeUtil.TimeLogger("ExecuteProc", log))
            {
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task ExecuteProcAsync(string procName, params SqlParameter[] parameters)
        {
            if (await ExecuteProcUncheckedAsync(procName, parameters) <= 0)
                throw new DatabaseException("expected #rows > 0", DatabaseExceptionType.ExpectedRowsMismatch);
        }

        public async Task ExecuteProcAsync(string procName, int expectedRows, params SqlParameter[] parameters)
        {
            int rows = await ExecuteProcUncheckedAsync(procName, parameters);
            if (rows != expectedRows)
                throw new DatabaseException($"expected {expectedRows} got {rows}", DatabaseExceptionType.ExpectedRowsMismatch);
        }

        public async Task<bool> ExecuteQueryProcOneAsync(string procName, Func<IResult, Task> readerFunc, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(procName, CommandType.StoredProcedure, parameters);

            using (new TimeUtil.TimeLogger("ExecuteQueryProcOneAsync", log))
            using (var rs = new ResultSetAsync(await cmd.ExecuteReaderAsync()))
            {
                if (await rs.NextResultAsync())
                {
                    await readerFunc.Invoke(rs);
                    return true;
                }
            }

            return false;
        }

        public async Task<SingleResult<T>> ExecuteQueryProcOneAsync<T>(string procName, Func<IResult, Task<T>> readerFunc, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(procName, CommandType.StoredProcedure, parameters);

            using (new TimeUtil.TimeLogger("ExecuteQueryProcOneAsync", log))
            using (var rs = new ResultSetAsync(await cmd.ExecuteReaderAsync()))
            {
                if (await rs.NextResultAsync())
                    return new SingleResult<T>(await readerFunc.Invoke(rs), true);
            }

            return new SingleResult<T>(default(T), false);
        }

        public async Task<List<T>> ExecuteQueryProcListAsync<T>(string procName, Func<IResult, Task<T>> readerFunc, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(procName, CommandType.StoredProcedure, parameters);

            using (new TimeUtil.TimeLogger("ExecuteQueryProcListAsync", log))
            using (var rs = new ResultSetAsync(await cmd.ExecuteReaderAsync()))
            {
                var list = new List<T>();
                while (await rs.NextResultAsync())
                    list.Add(await readerFunc.Invoke(rs));
                return list;
            }
        }

        #endregion proc async

        #region raw

        public IEnumerable<IResult> ExecuteRawQuery(string sql, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(sql, CommandType.Text, parameters);

            using (new TimeUtil.TimeLogger("ExecuteRawQuery", log))
            using (var rs = new ResultSet(cmd.ExecuteReader()))
            {
                foreach (var r in rs)
                    yield return r;
            }
        }

        public IList<T> ExecuteRawQueryList<T>(string sql, Func<IResult, T> func, params SqlParameter[] parameters) => ExecuteRawQuery(sql, parameters).Select(func).ToList();

        public int ExecuteRaw(string sql, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(sql, CommandType.Text, parameters);

            using (new TimeUtil.TimeLogger("ExecuteRaw", log))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        #endregion raw

        #region raw async

        public async Task<IList<T>> ExecuteRawQueryListAsync<T>(string sql, Func<IResult, Task<T>> func, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(sql, CommandType.Text, parameters);

            using (new TimeUtil.TimeLogger("ExecuteRawQueryListAsync", log))
            using (var rs = new ResultSetAsync(await cmd.ExecuteReaderAsync()))
            {
                var list = new List<T>();
                while (await rs.NextResultAsync())
                    list.Add(await func.Invoke(rs));
                return list;
            }
        }

        public async Task<int> ExecuteRawAsync(string sql, params SqlParameter[] parameters)
        {
            SqlCommand cmd = PrepareCmd(sql, CommandType.Text, parameters);

            using (new TimeUtil.TimeLogger("ExecuteRawAsync", log))
            {
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        #endregion raw async
    }
}
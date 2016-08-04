using Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Common.Database.Impl
{
    internal class DeferredSqlConnection : IDeferredSqlConnection
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        private class DeferredCall
        {
            public string Proc;
            public SqlParameter[] Parameters;
            public int CommandTimeout;
            public bool CheckRows;
            public int? ExpectedRows;
        }

        private readonly ConcurrentQueue<DeferredCall> _deferredCalls = new ConcurrentQueue<DeferredCall>();

        public int CommandTimeout { get; set; }

        private bool _failed;

        public DeferredSqlConnection(int defaultTimeout)
        {
            CommandTimeout = defaultTimeout;
        }

        public void DeferProc(string procName, bool checkRowsReturned, params SqlParameter[] parameters)
        {
            log.DebugFormat("Deferring call to {0}: check = {1}", procName, checkRowsReturned);

            _deferredCalls.Enqueue(new DeferredCall()
            {
                Proc = procName,
                Parameters = parameters,
                CommandTimeout = CommandTimeout,
                CheckRows = checkRowsReturned,
                ExpectedRows = null
            });
        }

        public void DeferProc(string procName, int expectedRows, params SqlParameter[] parameters)
        {
            log.DebugFormat("Deferring call to {0}: expected rows = {1}", procName, expectedRows);

            _deferredCalls.Enqueue(new DeferredCall()
            {
                Proc = procName,
                Parameters = parameters,
                CommandTimeout = CommandTimeout,
                CheckRows = false,
                ExpectedRows = expectedRows
            });
        }

        public void ExecuteAll(ISqlConnection conn)
        {
            if (_failed)
            {
                conn.SetRollback();
                return;
            }

            int origTimeout = conn.CommandTimeout;

            DeferredCall call;
            while (_deferredCalls.TryDequeue(out call))
            {
                conn.CommandTimeout = call.CommandTimeout;
                if (call.CheckRows)
                    conn.ExecuteProc(call.Proc, call.Parameters);
                else if (call.ExpectedRows.HasValue)
                    conn.ExecuteProc(call.Proc, call.ExpectedRows.Value, call.Parameters);
                else
                    conn.ExecuteProcUnchecked(call.Proc, call.Parameters);
            }

            conn.CommandTimeout = origTimeout;
        }

        public async Task ExecuteAllAsync(ISqlConnection conn)
        {
            if (_failed)
            {
                conn.SetRollback();
                return;
            }

            int origTimeout = conn.CommandTimeout;

            DeferredCall call;
            while (_deferredCalls.TryDequeue(out call))
            {
                conn.CommandTimeout = call.CommandTimeout;
                if (call.CheckRows)
                    await conn.ExecuteProcAsync(call.Proc, call.Parameters);
                else if (call.ExpectedRows.HasValue)
                    await conn.ExecuteProcAsync(call.Proc, call.ExpectedRows.Value, call.Parameters);
                else
                    await conn.ExecuteProcUncheckedAsync(call.Proc, call.Parameters);
            }

            conn.CommandTimeout = origTimeout;
        }

        public void SetRollback()
        {
            _failed = true;
        }

        public bool WillBeRollingBack() => _failed;

        public void ExecuteProc(string procName, params SqlParameter[] parameters)
        {
            DeferProc(procName, true, parameters);
        }

        public void ExecuteProc(string procName, int expectedRows, params SqlParameter[] parameters)
        {
            DeferProc(procName, expectedRows, parameters);
        }

        public int ExecuteProcUnchecked(string procName, params SqlParameter[] parameters)
        {
            DeferProc(procName, false, parameters);
            // always return 1, since this is unchecked and we don't know how many will be returned
            return 1;
        }

        public IEnumerable<IResult> ExecuteQueryProc(string procName, params SqlParameter[] parameters)
        {
            throw new NotSupportedException("cannot read on deferred connection");
        }

        public List<T> ExecuteQueryProcList<T>(string procName, Func<IResult, T> readerFunc, params SqlParameter[] parameters)
        {
            throw new NotSupportedException("cannot read on deferred connection");
        }

        public bool ExecuteQueryProcOne(string procName, Action<IResult> readerFunc, params SqlParameter[] parameters)
        {
            throw new NotSupportedException("cannot read on deferred connection");
        }

        public SingleResult<T> ExecuteQueryProcOne<T>(string procName, Func<IResult, T> readerFunc, params SqlParameter[] parameters)
        {
            throw new NotSupportedException("cannot read on deferred connection");
        }

        public Task ExecuteProcAsync(string procName, params SqlParameter[] parameters)
        {
            ExecuteProc(procName, parameters);
            return Task.CompletedTask;
        }

        public Task ExecuteProcAsync(string procName, int expectedRows, params SqlParameter[] parameters)
        {
            ExecuteProc(procName, expectedRows, parameters);
            return Task.CompletedTask;
        }

        public Task<int> ExecuteProcUncheckedAsync(string procName, params SqlParameter[] parameters)
        {
            int ret = ExecuteProcUnchecked(procName, parameters);
            return Task.FromResult(ret);
        }

        public Task<List<T>> ExecuteQueryProcListAsync<T>(string procName, Func<IResult, Task<T>> readerFunc, params SqlParameter[] parameters)
        {
            throw new NotSupportedException("cannot read on deferred connection");
        }

        public Task<bool> ExecuteQueryProcOneAsync(string procName, Func<IResult, Task> readerFunc, params SqlParameter[] parameters)
        {
            throw new NotSupportedException("cannot read on deferred connection");
        }

        public Task<SingleResult<T>> ExecuteQueryProcOneAsync<T>(string procName, Func<IResult, Task<T>> readerFunc, params SqlParameter[] parameters)
        {
            throw new NotSupportedException("cannot read on deferred connection");
        }
    }
}
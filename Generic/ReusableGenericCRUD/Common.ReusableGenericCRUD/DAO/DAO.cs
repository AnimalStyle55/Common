using Common.Database;
using Common.ReusableGenericCRUD.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Common.ReusableGenericCRUD.DAO
{
    public abstract class DAO<T> : IDAO<T> where T : IKeyed
    {
        private string GetTName() => typeof(T).Name;
        public ISqlConnection Connection { get; set; }

        public DAO(ISqlConnection connection)
        {
            Connection = connection;
        }

        public DAO() { }

        public List<T> Get(string userName)
        {
            return Connection.ExecuteQueryProcList(
                $"usp_Get{GetTName()}List",
                (rs) => ConvertResult(rs),
                new SqlParameter("@CallingUser", userName));
        }

        public T Get(string userName, Guid id)
        {
            return Connection.ExecuteQueryProcOne(
                    $"usp_Get{GetTName()}ByGUID",
                    (rs) => ConvertResult(rs),
                    new SqlParameter[] { new SqlParameter("@GUID", id), new SqlParameter("@CallingUser", userName) })
                .Value;
        }

        public T Insert(string userName, T item)
        {
            item.Id = Guid.NewGuid();

            Connection.ExecuteProc($"usp_Insert{GetTName()}", UpsertParams(userName, item));

            return item;
        }

        public void Update(string userName, T item)
        {
            Connection.ExecuteProc($"usp_Update{GetTName()}", UpsertParams(userName, item));
        }

        public void Delete(string userName, Guid id)
        {
            Connection.ExecuteProc($"usp_Delete{GetTName()}", new SqlParameter[] {
                new SqlParameter("@Guid", id),
                new SqlParameter("@CallingUser", userName)
            });
        }

        internal abstract T ConvertResult(IResult result);

        internal abstract SqlParameter[] UpsertParams(string username, T item);
    }
}
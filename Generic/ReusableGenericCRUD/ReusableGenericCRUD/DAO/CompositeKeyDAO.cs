using Common.Database;
using Common.ReusableGenericCRUD.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Common.ReusableGenericCRUD.DAO
{
    public abstract class CompositeKeyDAO<T> : ICompositeKeyDAO<T> where T : ICompositeKeyed
    {
        private string GetTName() => typeof(T).Name;
        public ISqlConnection Connection { get; set; }
        public string FirstItemName { get; set; }
        public string SecondItemName { get; set; }

        public CompositeKeyDAO(ISqlConnection connection)
        {
            Connection = connection;
        }

        public CompositeKeyDAO() { }

        public void Initialize(string firstItemName, string secondItemName)
        {
            FirstItemName = firstItemName;
            SecondItemName = secondItemName;
        }

        public void Insert(string userName, T item)
        {
            Connection.ExecuteProc($"usp_Insert{GetTName()}", UpsertParams(userName, item));
        }

        public void Update(string userName, T item)
        {
            Connection.ExecuteProc($"usp_Update{GetTName()}", UpsertParams(userName, item));
        }
        
        public void Delete(string userName, Guid firstItemId, Guid secondItemId)
        {
            Connection.ExecuteProc(
                $"usp_Delete{GetTName()}", new SqlParameter[] {
                new SqlParameter($"@{FirstItemName}Guid", firstItemId),
                new SqlParameter($"@{SecondItemName}Guid", secondItemId),
                new SqlParameter("@CallingUser", userName)});
        }

        public List<T> GetAll(string userName)
        {
            return Connection.ExecuteQueryProcList(
                $"usp_Get{GetTName()}List",
                (rs) => ConvertResult(rs),
                new SqlParameter("@CallingUser", userName));
        }

        public List<T> GetWithFirstItemId(string userName, Guid firstItemId)
        {
            return Connection.ExecuteQueryProcList(
                $"usp_Get{GetTName()}ListBy{FirstItemName}GUID",
                (rs) => ConvertResult(rs),
                new SqlParameter($"@{FirstItemName}Guid", firstItemId),
                new SqlParameter("@CallingUser", userName));
        }

        public List<T> GetWithSecondItemId(string userName, Guid secondItemId)
        {
            return Connection.ExecuteQueryProcList(
                $"usp_Get{GetTName()}ListBy{SecondItemName}GUID",
                (rs) => ConvertResult(rs),
                new SqlParameter($"@{SecondItemName}Guid", secondItemId),
                new SqlParameter("@CallingUser", userName));
        }

        public T Get(string userName, Guid firstItemId, Guid secondItemId)
        {
            return Connection.ExecuteQueryProcOne(
                $"usp_Get{GetTName()}ListBy{SecondItemName}GUID",
                (rs) => ConvertResult(rs),
                new SqlParameter($"@{FirstItemName}Guid", firstItemId),
                new SqlParameter($"@{SecondItemName}Guid", secondItemId),
                new SqlParameter("@CallingUser", userName)).Value;
        }

        internal abstract T ConvertResult(IResult result);

        internal abstract SqlParameter[] UpsertParams(string username, T item);
    }
}
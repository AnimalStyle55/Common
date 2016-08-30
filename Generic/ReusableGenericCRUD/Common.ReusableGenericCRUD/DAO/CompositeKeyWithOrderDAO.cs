using Common.Database;
using Common.ReusableGenericCRUD.Repository;
using System.Data.SqlClient;

namespace Common.ReusableGenericCRUD.DAO
{
    public abstract class CompositeKeyWithOrderDAO<T> : CompositeKeyDAO<T>, ICompositeKeyWithOrderDAO<T> where T : ICompositeKeyedWithOrder, new()
    {
        private string GetTName() => typeof(T).Name;
        public CompositeKeyWithOrderDAO() { }

        public CompositeKeyWithOrderDAO(ISqlConnection conn)
        {
            Connection = conn;
        }
        
        internal override T ConvertResult(IResult result) => new T()
        {
            FirstItemId = result.GetGuid($"{FirstItemName}GUID"),
            SecondItemId = result.GetGuid($"{SecondItemName}GUID"),
            Order = result.GetInt("Order")
        };

        internal override SqlParameter[] UpsertParams(string username, T item) => new SqlParameter[]
        {
            new SqlParameter($"CallingUser", username),
            new SqlParameter($"@{FirstItemName}GUID", item.FirstItemId),
            new SqlParameter($"@{SecondItemName}GUID", item.SecondItemId),
            new SqlParameter("@Order", item.Order)
        };
    }
}
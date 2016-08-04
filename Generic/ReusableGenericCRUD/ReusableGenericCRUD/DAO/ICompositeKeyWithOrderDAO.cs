namespace Common.ReusableGenericCRUD.Repository
{
    public interface ICompositeKeyWithOrderDAO<T> : ICompositeKeyDAO<T> where T : ICompositeKeyedWithOrder
    {

    }
}
namespace Common.ReusableGenericCRUD
{
    public interface ICompositeKeyedWithOrder : ICompositeKeyed
    {
        int Order { get; set; }
    }
}
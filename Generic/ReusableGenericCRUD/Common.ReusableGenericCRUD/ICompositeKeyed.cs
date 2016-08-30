using System;

namespace Common.ReusableGenericCRUD
{
    public interface ICompositeKeyed
    {
        Guid FirstItemId { get; set; }
        Guid SecondItemId { get; set; }
    }
}
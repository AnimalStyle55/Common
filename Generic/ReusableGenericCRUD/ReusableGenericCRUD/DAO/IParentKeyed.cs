using System;

namespace Common.ReusableGenericCRUD.DAO
{
    public interface IParentKeyed
    {
        Guid ParentId { get; set; }
    }
}
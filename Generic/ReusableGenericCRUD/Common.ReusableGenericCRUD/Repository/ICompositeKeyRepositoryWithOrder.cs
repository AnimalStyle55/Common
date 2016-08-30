using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Repository
{
    public interface ICompositeKeyWithOrderRepository<T> : ICompositeKeyRepository<T>
    {
        void DeleteChildrenAndRecreate(string userName, Guid parentId, List<Guid> orderedChildren);
        void AddToEnd(string callingUser, Guid id, Guid childId);
    }
}
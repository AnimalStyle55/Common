using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Repository
{
    public interface ICompositeKeyRepository<T>
    {
        void Delete(string userName, Guid firstItemId, Guid secondItemId);
        List<T> GetAll(string userName);
        List<T> GetWithFirstItemId(string userName, Guid firstItemId);
        List<T> GetWithSecondItemId(string userName, Guid secondItemId);
        T Get(string userName, Guid firstItemId, Guid secondItemId);
        void Insert(string userName, T item);
        void Update(string userName, T item);
    }
}
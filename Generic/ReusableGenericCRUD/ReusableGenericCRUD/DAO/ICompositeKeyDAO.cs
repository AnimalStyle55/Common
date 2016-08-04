using Common.Database;
using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Repository
{
    public interface ICompositeKeyDAO<T> where T : ICompositeKeyed
    {
        ISqlConnection Connection { get; set; }
        string FirstItemName { get; set; }
        string SecondItemName { get; set; }
        void Initialize(string firstItemName, string secondItemName);

        void Delete(string userName, Guid firstItemId, Guid secondItemId);
        List<T> GetAll(string userName);
        List<T> GetWithFirstItemId(string userName, Guid firstItemId);
        List<T> GetWithSecondItemId(string userName, Guid secondItemId);
        T Get(string userName, Guid firstItemId, Guid secondItemId);
        void Insert(string userName, T item);
        void Update(string userName, T item);
    }
}
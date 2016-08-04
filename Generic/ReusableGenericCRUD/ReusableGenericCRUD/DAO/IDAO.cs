using Common.Database;
using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Repository
{
    public interface IDAO<T> where T : IKeyed
    {
        ISqlConnection Connection { get; set; }
        void Delete(string userName, Guid id);
        List<T> Get(string userName);
        T Get(string userName, Guid id);
        T Insert(string userName, T item);
        void Update(string userName, T item);
    }
}
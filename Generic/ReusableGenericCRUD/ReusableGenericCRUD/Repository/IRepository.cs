using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Repository
{
    public interface IRepository<T>
    {
        List<T> GetAll(string username);
        T Get(string username, Guid id);
        T Create(string username, T item);
        void Update(string username, T item);
        void Delete(string username, Guid id);
    }
}
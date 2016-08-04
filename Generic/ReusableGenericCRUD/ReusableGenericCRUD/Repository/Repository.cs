using Common.Database;
using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Repository
{
    public abstract class Repository<DAOType, T> where DAOType : IDAO<T>, new() where T : IKeyed, new()
    {
        public ISqlTransactionProvider SqlTransactionProvider { get; set; }

        public Repository(ISqlTransactionProvider sqlTransactionProvider)
        {
            SqlTransactionProvider = sqlTransactionProvider;
        }

        public T Get(string username, Guid id)
        {
            T item = new T();
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                DAOType itemDAO = new DAOType();
                itemDAO.Connection = conn;
                item = itemDAO.Get(username, id);
            });

            return item;
        }

        public List<T> GetAll(string username)
        {
            List<T> list = null;

            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                DAOType itemDAO = new DAOType();
                itemDAO.Connection = conn;
                list = itemDAO.Get(username);
            });

            return list;
        }

        public T Create(string username, T item)
        {
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                DAOType itemDAO = new DAOType();
                itemDAO.Connection = conn;
                item = itemDAO.Insert(username, item);
            });

            return item;
        }

        public void Delete(string username, Guid id)
        {
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                DAOType itemDAO = new DAOType();
                itemDAO.Connection = conn;
                itemDAO.Delete(username, id);
            });
        }

        public void Update(string username, T item)
        {
            SqlTransactionProvider.ExecuteTxn(conn => {
                DAOType itemDAO = new DAOType();
                itemDAO.Connection = conn;
                itemDAO.Update(username, item);
            });
        }
    }
}
using Common.Database;
using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Repository
{
    public abstract class CompositeKeyRepository<T, K> : ICompositeKeyRepository<K> where T : ICompositeKeyDAO<K>, new() where K : ICompositeKeyed, new()
    {
        public ISqlTransactionProvider SqlTransactionProvider { get; set; }

        public CompositeKeyRepository(ISqlTransactionProvider sqlTransactionProvider)
        {
            SqlTransactionProvider = sqlTransactionProvider;
        }

        public void Update(string username, K item)
        {
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;
                tDAO.Update(username, item);
            });
        }

        public void Delete(string userName, Guid firstItemId, Guid secondItemId)
        {
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;
                tDAO.Delete(userName, firstItemId, secondItemId);
            });
        }

        public List<K> GetAll(string userName)
        {
            List<K> list = null;

            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;
                list = tDAO.GetAll(userName);
            });

            return list;
        }

        public List<K> GetWithFirstItemId(string userName, Guid firstItemId)
        {
            List<K> list = null;

            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;
                list = tDAO.GetWithFirstItemId(userName, firstItemId);
            });

            return list;
        }

        public List<K> GetWithSecondItemId(string userName, Guid secondItemId)
        {
            List<K> list = null;

            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;
                list = tDAO.GetWithSecondItemId(userName, secondItemId);
            });

            return list;
        }

        public K Get(string userName, Guid firstItemId, Guid secondItemId)
        {
            K item = new K();

            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;
                item = tDAO.Get(userName, firstItemId, secondItemId);
            });

            return item;
        }

        public void Insert(string userName, K item)
        {
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;
                tDAO.Insert(userName, item);
            });
        }
    }
}
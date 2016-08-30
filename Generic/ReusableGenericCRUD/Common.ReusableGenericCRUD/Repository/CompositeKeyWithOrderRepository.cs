using Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.ReusableGenericCRUD.Repository
{
    public abstract class CompositeKeyWithOrderRepository<T, K> : CompositeKeyRepository<T, K>, ICompositeKeyWithOrderRepository<K> where T : ICompositeKeyWithOrderDAO<K>, new() where K : ICompositeKeyedWithOrder, new()
    {
        public CompositeKeyWithOrderRepository(ISqlTransactionProvider sqlTransactionProvider) : base(sqlTransactionProvider) { }

        public void DeleteChildrenAndRecreate(string userName, Guid parentId, List<Guid> orderedChildren)
        {
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;

                //Get List of all with parent.
                List<K> current = tDAO.GetWithFirstItemId(userName, parentId);

                //Delete all for parent.
                foreach (K c in current)
                {
                    tDAO.Delete(userName, c.FirstItemId, c.SecondItemId);
                }

                //Insert new ones according to order.
                for (int i = 0; i < orderedChildren.Count; i++)
                {
                    tDAO.Insert(userName, new K() { FirstItemId = parentId, SecondItemId = orderedChildren[i], Order = i + 1 });
                }
            });
        }

        public void AddToEnd(string userName, Guid parentId, Guid childId)
        {
            SqlTransactionProvider.ExecuteTxn(conn =>
            {
                T tDAO = new T();
                tDAO.Connection = conn;

                //Get List of all with parent.
                List<K> current = tDAO.GetWithFirstItemId(userName, parentId);

                int nextOrderNumber = 1;
                if (current != null && current.Count > 0)
                    nextOrderNumber = current.Max(x => x.Order) + 1;

                //Insert new relation with order
                tDAO.Insert(userName, new K() { FirstItemId = parentId, SecondItemId = childId, Order = nextOrderNumber });
            });
        }
    }
}
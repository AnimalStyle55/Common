using System;
using System.Collections.Generic;

namespace Common.ReusableGenericCRUD.Extensions
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this Guid guid) => guid == null || guid == Guid.Empty;
        public static bool IsNotNullNullOrEmpty(this Guid guid) => guid != null && guid != Guid.Empty;

        public static Dictionary<Guid, IKeyed> ConvertToDictionary(this List<IKeyed> list)
        {
            if (list == null)
                return null;

            Dictionary<Guid, IKeyed> indexedList = new Dictionary<Guid, IKeyed>();

            foreach (IKeyed k in list)
                indexedList.Add(k.Id, k);

            return indexedList;
        }
    }
}
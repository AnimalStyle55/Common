using System;
using System.Collections.Generic;
using System.Linq;

namespace LD.Common.Database
{
    /// <summary>
    /// Represents a single result that might or might not be present
    /// (essentially Optional(T))
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleResult<T>
    {
        /// <summary>
        /// The value returned
        /// </summary>
        public T Value
        {
            get
            {
                if (!RowReturned)
                    return default(T);
                return _value;
            }
        }

        private readonly T _value;

        /// <summary>
        /// True if the query returned a result
        /// </summary>
        public bool RowReturned { get; private set; }

        internal SingleResult(T val, bool hasVal)
        {
            _value = val;
            RowReturned = hasVal;
        }
    }
}
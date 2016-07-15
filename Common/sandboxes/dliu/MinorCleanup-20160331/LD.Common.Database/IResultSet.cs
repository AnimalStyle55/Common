using System;
using System.Collections.Generic;

namespace LD.Common.Database
{
    /// <summary>
    /// A set of results from a query
    /// </summary>
    public interface IResultSet : IDisposable, IEnumerable<IResult>
    {
        /// <summary>
        /// Get the next result from the set
        /// </summary>
        IResult NextResult();
    }
}
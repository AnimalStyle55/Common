using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LD.Common.Database
{
    internal interface IDatabaseConnectionProvider
    {
        /// <summary>
        /// Get a SqlConnection from the pool
        /// </summary>
        /// <returns></returns>
        SqlConnection GetConnection();
    }
}
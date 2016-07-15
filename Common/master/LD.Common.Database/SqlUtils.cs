using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LD.Common.Database
{
    /// <summary>
    /// Utilities for DB Code
    /// </summary>
    public static class SqlUtils
    {
        /// <summary>
        /// Determine if the given exception was caused by timeout
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsTimeoutException(SqlException e)
        {
            // sigh, this appears to be the only way to detect timeout
            // http://stackoverflow.com/questions/29664/how-to-catch-sqlserver-timeout-exceptions
            return e.Number == -2;
        }

        /// <summary>
        /// Convert a nullable timespan to integer milliseconds
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns>int milliseconds or null if timespan is null</returns>
        public static int? TimeSpanToMillis(TimeSpan? timespan)
        {
            return timespan.HasValue ? (int)timespan.Value.TotalMilliseconds : (int?)null;
        }

        /// <summary>
        /// Convert a nullable integer milliseconds to timespan
        /// </summary>
        /// <param name="millis"></param>
        /// <returns>timespan or null if millis is null</returns>
        public static TimeSpan? MillisToTimeSpan(int? millis)
        {
            return millis.HasValue ? TimeSpan.FromMilliseconds(millis.Value) : (TimeSpan?)null;
        }
    }
}
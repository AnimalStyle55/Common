using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LD.Common.Database.Impl
{
    internal static class StoredProcLogger
    {
        /// <summary>
        /// Converts a stored procedure execution to string so it can be logged
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="parameters"></param>
        /// <returns>a string containing executable sql</returns>
        public static string ExecToString(string procName, IEnumerable<SqlParameter> parameters)
        {
            StringBuilder sb = new StringBuilder(128);
            sb.AppendFormat("EXECUTE {0} ", procName);

            sb.Append(string.Join(", ", parameters.Select(paramToString)));

            return sb.ToString();
        }

        /// <summary>
        /// Converts a raw sql call to a string to it can be logged
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>a string containing the sql followed by the parameters</returns>
        public static string RawExecToString(string sql, IEnumerable<SqlParameter> parameters)
        {
            StringBuilder sb = new StringBuilder(128);
            sb.Append(sql);

            int mark = sb.Length;
            sb.Append(" ==> VALUES (");
            string paramStr = string.Join(", ", parameters.Select(paramToString));
            sb.Append(paramStr);
            sb.Append(")");

            if (string.IsNullOrEmpty(paramStr))
                sb.Length = mark;

            return sb.ToString();
        }

        private static string paramToString(SqlParameter p)
        {
            if (p.Value == null)
                return string.Format("{0} = NULL", p.ParameterName);
            else if (p.Value is string || p.Value is Guid)
                return string.Format("{0} = '{1}'", p.ParameterName, p.Value);
            else
                return string.Format("{0} = {1}", p.ParameterName, p.Value);
        }
    }
}
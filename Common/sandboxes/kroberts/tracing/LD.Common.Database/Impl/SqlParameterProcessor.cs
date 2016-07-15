using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LD.Common.Database.Impl
{
    internal static class SqlParameterProcessor
    {
        private static readonly ILog log = LoanDepotLogManager.GetLogger();

        /// <summary>
        /// Process any changes necessary for SqlParameters
        ///
        /// - DateTime => to UTC
        /// - Enums => to string plus warning
        /// - Strings =>
        ///       If starting with "NOTRIM-", value will not be trimmed
        ///       Otherwise, .Trim()
        /// </summary>
        /// <param name="param">[in/out] will be modified if needed</param>
        public static void ProcessParam(SqlParameter param)
        {
            // nullable parameters must explicitly set DBNull.Value for null values
            if (param.ParameterName[0] != '@'
                && param.ParameterName.StartsWith(SqlConstants.NullableHeader))
            {
                param.ParameterName = param.ParameterName.Substring(SqlConstants.NullableHeader.Length);
                param.Value = param.Value ?? DBNull.Value;
            }

            // null values require no processing
            if (param.Value == null || param.Value == DBNull.Value)
            {
                ProcessDoNotTrim(param);
                return;
            }

            // ensure all datetimes are UTC
            if (param.DbType == DbType.DateTime)
            {
                param.Value = ((DateTime)param.Value).ToUniversalTime();
                return;
            }

            // enums should be .ToString()'d before getting here
            if (param.Value.GetType().IsEnum)
            {
                log.WarnFormat("Using an enum type as a SQL command parameter. It has been automatically cast to a string. Consider explicitly using ToString(): {0}.{1}",
                    param.Value.GetType().Name, param.Value.ToString());
                param.Value = param.Value.ToString();
                return;
            }

            // strings are automatically trimmed, unless the parameter name starts with the DoNotTrimHeader
            if (param.Value is string)
            {
                if (param.ParameterName[0] == '@')
                    param.Value = (param.Value as string).Trim();
                else
                    ProcessDoNotTrim(param);
                return;
            }
        }

        private static bool ProcessDoNotTrim(SqlParameter param)
        {
            if (param.ParameterName.StartsWith(SqlConstants.DoNotTrimHeader))
            {
                param.ParameterName = param.ParameterName.Substring(SqlConstants.DoNotTrimHeader.Length);
                return true;
            }
            return false;
        }
    }
}
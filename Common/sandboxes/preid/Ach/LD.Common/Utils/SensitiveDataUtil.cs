using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LD.Common.Utils
{
    /// <summary>
    /// Methods to help protect sensitive date from appearing in logs
    /// </summary>
    public static class SensitiveDataUtil
    {
        /// <summary>
        /// Masks values in the provided JSON string which match the provided key and value regular expressin.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="keyRegEx"></param>
        /// <param name="valueRegEx"></param>
        /// <returns>string with masked specified data</returns>
        public static string MaskJsonValues(string s, string keyRegEx = null, string valueRegEx = null)
        {
            string regexFormat = string.Format("(\"{0}\"\\s*:\\s*\")({1})\"", keyRegEx ?? ".*?", valueRegEx ?? ".*?");
            return Regex.Replace(s, regexFormat, "$1<REDACTED>\"", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Masks values in the provided XML string which match the provided key and value regular expression.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="keyRegEx"></param>
        /// <param name="valueRegEx"></param>
        /// <returns>string with masked specified data</returns>
        public static string MaskXmlValues(string s, string keyRegEx = null, string valueRegEx = null)
        {
            string regexFormat = string.Format("<({0})>({1})</\\1>", keyRegEx ?? "[^>]+", valueRegEx ?? "[^<]*");
            return Regex.Replace(s, regexFormat, "<$1><REDACTED></$1>", RegexOptions.IgnoreCase);
        }
    }
}
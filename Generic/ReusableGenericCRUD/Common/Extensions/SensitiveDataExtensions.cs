using System;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
    /// <summary>
    /// Methods to help protect sensitive data from appearing in logs
    /// </summary>
    public static class SensitiveDataExtensions
    {
        // Note: the insertion text for redacting should be compatible with all supported document
        // types (e.g. JSON, XML, query string)
        private const string _RedactedMark = "_REDACTED_";

        /// <summary>
        /// Masks values in the provided JSON string which match the provided key and value regular expression.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="keyRegEx"></param>
        /// <param name="valueRegEx"></param>
        /// <returns>string with masked specified data</returns>
        public static string MaskJsonValues(this string s, string keyRegEx = null, string valueRegEx = null)
        {
            string regexFormat = $"(\"{keyRegEx ?? ".*?"}\"\\s*:\\s*\")({valueRegEx ?? ".*?"})\"";
            return Regex.Replace(s, regexFormat, $"$1{_RedactedMark}\"", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Masks values in the provided URI query string which match the provided key and value regular expression.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="keyRegEx"></param>
        /// <param name="valueRegEx"></param>
        /// <returns>string with masked specified data</returns>
        public static string MaskQueryStringValues(this string s, string keyRegEx = null, string valueRegEx = null)
        {
            string regexFormat = $"([?&]{keyRegEx ?? ".*?"}=)({valueRegEx ?? "[^&]*"})";
            return Regex.Replace(s, regexFormat, $"$1{_RedactedMark}", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Masks json and xml versions of the ssn field
        /// </summary>
        /// <param name="s"></param>
        /// <returns>string with masked ssn values</returns>
        public static string MaskSsn(this string s)
        {
            int jsonSsnIndex = s.IndexOf("\"ssn\"", 0, StringComparison.OrdinalIgnoreCase);
            if (jsonSsnIndex >= 0)
                s = s.MaskJsonValues("ssn", "\\d{3}[\\-]?\\d{2}[\\-]?\\d{4}");

            int xmlSsnIndex = s.IndexOf("<ssn>", 0, StringComparison.OrdinalIgnoreCase);
            if (xmlSsnIndex >= 0)
                s = s.MaskXmlValues("ssn", "\\d{3}[\\-]?\\d{2}[\\-]?\\d{4}");

            int querySsnIndex = s.IndexOf("ssn=", 0, StringComparison.OrdinalIgnoreCase);
            if (querySsnIndex >= 0)
                s = s.MaskQueryStringValues("ssn", "\\d{3}[\\-]?\\d{2}[\\-]?\\d{4}");

            return s;
        }

        /// <summary>
        /// Masks json and xml versions of the password field
        /// </summary>
        /// <param name="s"></param>
        /// <returns>string with masked password values</returns>
        public static string MaskPassword(this string s)
        {
            int jsonPasswordIndex = s.IndexOf("password\"", 0, StringComparison.OrdinalIgnoreCase);
            if (jsonPasswordIndex >= 0)
                s = s.MaskJsonValues("[a-zA-Z]*[pP]assword");

            int xmlPasswordIndex = s.IndexOf("<password>", 0, StringComparison.OrdinalIgnoreCase);
            if (xmlPasswordIndex >= 0)
                s = s.MaskXmlValues("[a-zA-Z]*[pP]assword");

            int queryPasswordIndex = s.IndexOf("password=", 0, StringComparison.OrdinalIgnoreCase);
            if (queryPasswordIndex >= 0)
                s = s.MaskQueryStringValues("[a-zA-Z]*[pP]assword");

            return s;
        }

        /// <summary>
        /// Masks values in the provided XML string which match the provided key and value regular expression.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="keyRegEx"></param>
        /// <param name="valueRegEx"></param>
        /// <returns>string with masked specified data</returns>
        private static string MaskXmlValues(this string s, string keyRegEx, string valueRegEx = null)
        {
            string regexFormat = $"<({keyRegEx})>({valueRegEx ?? "[^<]*"})</\\1>";
            return Regex.Replace(s, regexFormat, $"<$1>{_RedactedMark}</$1>", RegexOptions.IgnoreCase);
        }
    }
}
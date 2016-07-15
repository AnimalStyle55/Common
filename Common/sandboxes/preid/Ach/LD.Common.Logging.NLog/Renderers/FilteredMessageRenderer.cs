using LD.Common.Utils;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD.Common.Logging.NLog.Renderers
{
    /// <summary>
    /// Custom renderer for filtering sensitive data and truncating on max length
    /// </summary>
    [LayoutRenderer("filteredMessage")]
    [AmbientProperty("MaxLength")]
    public class FilteredMessageRenderer : MessageLayoutRenderer
    {
        /// <summary>
        /// Max line length
        /// </summary>
        [System.ComponentModel.DefaultValue(25000)]
        public int MaxLength { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FilteredMessageRenderer()
        {
            MaxLength = 25000;
        }

        /// <summary>
        /// Called by NLog to append our data
        /// </summary>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            string msg = logEvent.FormattedMessage;

            if (msg.Length > MaxLength)
            {
                msg = new StringBuilder(msg.Substring(0, MaxLength)).Append("<truncated>").ToString();
            }

            msg = maskSsn(msg);
            msg = maskPassword(msg);

            builder.Append(msg);
        }

        private string maskSsn(string msg)
        {
            int jsonSsnIndex = msg.IndexOf("\"ssn\"", 0, StringComparison.OrdinalIgnoreCase);
            if (jsonSsnIndex >= 0)
            {
                msg = SensitiveDataUtil.MaskJsonValues(msg, "ssn", "\\d{3}[\\-]?\\d{2}[\\-]?\\d{4}");
            }

            int xmlSsnIndex = msg.IndexOf("<ssn>", 0, StringComparison.OrdinalIgnoreCase);
            if (xmlSsnIndex >= 0)
            {
                msg = SensitiveDataUtil.MaskXmlValues(msg, "ssn", "\\d{3}[\\-]?\\d{2}[\\-]?\\d{4}");
            }

            return msg;
        }

        private string maskPassword(string msg)
        {
            int jsonPasswordIndex = msg.IndexOf("password\"", 0, StringComparison.OrdinalIgnoreCase);
            if (jsonPasswordIndex >= 0)
            {
                msg = SensitiveDataUtil.MaskJsonValues(msg, "[a-zA-Z]*[pP]assword");
            }

            int xmlPasswordIndex = msg.IndexOf("<password>", 0, StringComparison.OrdinalIgnoreCase);
            if (xmlPasswordIndex >= 0)
            {
                msg = SensitiveDataUtil.MaskXmlValues(msg, "[a-zA-Z]*[pP]assword");
            }

            return msg;
        }
    }
}
using LD.Common.Extensions;
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
                msg = msg.Substring(0, MaxLength) + "<truncated>";

            msg = msg.MaskSsn();
            msg = msg.MaskPassword();

            builder.Append(msg);
        }
    }
}
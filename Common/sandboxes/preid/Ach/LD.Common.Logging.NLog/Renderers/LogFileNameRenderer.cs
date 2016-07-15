using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD.Common.Logging.NLog.Renderers
{
    /// <summary>
    /// Custom log file name renderer to set the log file name
    /// </summary>
    [LayoutRenderer("logFileName")]
    [AmbientProperty("Component")]
    [AmbientProperty("Environment")]
    [AmbientProperty("IncludeMachine")]
    [AmbientProperty("Type")]
    public class LogFileNameRenderer : LayoutRenderer
    {
        /// <summary>Software Component Name</summary>
        [System.ComponentModel.DefaultValue("")]
        public string Component { get; set; }

        /// <summary>Environment Name</summary>
        [System.ComponentModel.DefaultValue("")]
        public string Environment { get; set; }

        /// <summary>whether to include machine name</summary>
        [System.ComponentModel.DefaultValue(true)]
        public bool IncludeMachine { get; set; }

        /// <summary>Type (error or none)</summary>
        [System.ComponentModel.DefaultValue("")]
        public string Type { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public LogFileNameRenderer()
        {
            Component = "";
            Environment = "";
            IncludeMachine = true;
            Type = "";
        }

        /// <summary>
        /// Called by NLog to append our data
        /// </summary>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(string.Join(".", nameItems(logEvent.TimeStamp)));
        }

        private IEnumerable<string> nameItems(DateTime dt)
        {
            yield return "loandepot";

            if (!string.IsNullOrEmpty(Type))
                yield return Type;

            if (!string.IsNullOrEmpty(Component))
                yield return Component;

            if (!string.IsNullOrEmpty(Environment))
                yield return Environment;

            if (IncludeMachine)
                yield return System.Environment.MachineName.ToLower();
        }
    }
}
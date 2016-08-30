using NLog;
using NLog.LayoutRenderers;
using System.Text;

namespace Common.Logging.NLog.Renderers
{
#if !NET40
    /// <summary>
    /// Custom renderer that will log some value for all items in am async context
    /// </summary>
    [LayoutRenderer("contextId")]
    public class AsyncCtxRenderer : LayoutRenderer
    {
        /// <summary>
        /// Renders the specified MDLC item and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var id = CommonLogManager.GetContextId();
            if (string.IsNullOrEmpty(id))
            {
                builder.AppendFormat("     ");
                return;
            }

            builder.AppendFormat("{0,-5}", id);
        }
    }
#endif
}
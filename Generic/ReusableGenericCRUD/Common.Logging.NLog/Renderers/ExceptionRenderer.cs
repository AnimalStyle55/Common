using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Common.Logging.NLog.Renderers
{
    /// <summary>
    /// Custom Renderer for nicer exceptions in the log
    /// </summary>
    [LayoutRenderer("exception")]
    [AmbientProperty("MaxFrames")]
    public class ExceptionRenderer : StackTraceLayoutRenderer
    {
        /// <summary>
        /// Maximum number of stack frames in an exception log
        /// </summary>
        [System.ComponentModel.DefaultValue(20)]
        public int MaxFrames { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExceptionRenderer()
        {
            MaxFrames = 20;
        }

        /// <summary>
        /// Called by NLog to append our data
        /// </summary>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            AppendException(logEvent.Exception, builder, 0);
        }

        /// <summary>
        /// Append an exception to the log
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="builder"></param>
        /// <param name="level">the 0-based depth of this exception</param>
        /// <param name="isParentAggregate">true if this exception is an InnerException of an AggregateException</param>
        private void AppendException(Exception ex, StringBuilder builder, int level, bool isParentAggregate = false)
        {
            if (ex == null)
                return;

            WriteException(ex, builder, level, isParentAggregate);

            foreach (var innerException in GetInnerExceptions(ex))
            {
                AppendException(innerException, builder, level + 1, ex is AggregateException);
            }
        }

        private IEnumerable<Exception> GetInnerExceptions(Exception ex)
        {
            if (ex is AggregateException)
            {
                return (ex as AggregateException).Flatten().InnerExceptions;
            }

            return ex.InnerException != null ? new[] { ex.InnerException } : new Exception[0];
        }

        /// <summary>
        /// Write an exception to the log
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="builder"></param>
        /// <param name="level"></param>
        /// <param name="isParentAggregate">true if this exception is an InnerException of an AggregateException</param>
        private void WriteException(Exception ex, StringBuilder builder, int level, bool isParentAggregate = false)
        {
            string threadName = Thread.CurrentThread.Name ?? ("Thread-" + Thread.CurrentThread.ManagedThreadId);

            builder.Append(Environment.NewLine);

            if (level == 0 || isParentAggregate)
            {
                builder.AppendFormat("{0}{1}Exception in thread \"{2}\" {3}: {4}{5}",
                    LevelPrefix(level),
                    isParentAggregate ? "Aggregated: " : String.Empty,
                    threadName,
                    ex.GetType().FullName,
                    ex.Message,
                    Environment.NewLine);
            }
            else
            {
                builder.AppendFormat("{0}Caused by: {1}: {2}{3}",
                    LevelPrefix(level),
                    ex.GetType().FullName,
                    ex.Message,
                    Environment.NewLine);
            }

            var trace = new StackTrace(ex, true).GetFrames();

            AppendStack(trace, builder);
        }

        /// <summary>
        /// Returns a prefix for the log line based on the level of an exception
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string LevelPrefix(int level)
        {
            if (level < 2)
                return string.Empty;

            return String.Format("L{0}> ", level);
        }

        /// <summary>
        /// Append the stack trace
        /// </summary>
        /// <param name="trace"></param>
        /// <param name="builder"></param>
        private void AppendStack(StackFrame[] trace, StringBuilder builder)
        {
            int mark = 0;

            if (trace == null || trace.Length == 0)
            {
                builder.AppendFormat("    [No stack trace present]");
                return;
            }

            for (int i = 0; i < MaxFrames && i < trace.Length; i++)
            {
                var frame = trace[i];
                var method = frame.GetMethod();
                var declType = method.DeclaringType;

                if (declType == null)
                {
                    builder.AppendFormat("    at <unknown type>({0})", method.Name);
                    AppendFileInfo(frame, builder);
                }
                else
                {
                    builder.AppendFormat("    at {0}.{1}(", method.DeclaringType.FullName, method.Name);
                    AppendFileInfo(frame, builder);
                    builder.Append(')');
                }

                mark = builder.Length;
                builder.Append(Environment.NewLine);
            }

            if (trace.Length > MaxFrames)
            {
                builder.AppendFormat("    ... {0} more", trace.Length - MaxFrames);
                mark = builder.Length;
            }

            builder.Length = mark;
        }

        /// <summary>
        /// Construct the file info (file name and line number)
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="builder"></param>
        private void AppendFileInfo(StackFrame frame, StringBuilder builder)
        {
            if (frame.GetFileName() == null)
            {
                builder.Append("<unknown file>");
                return;
            }

            if (frame.GetFileName().Contains("LD."))
            {
                var fi = new FileInfo(frame.GetFileName());
                filterDir(fi.Directory, builder);
                builder.Append(Path.DirectorySeparatorChar).AppendFormat("{0}:{1}", fi.Name, frame.GetFileLineNumber());
            }
            else
                builder.AppendFormat("{0}:{1}", frame.GetFileName(), frame.GetFileLineNumber());
        }

        /// <summary>
        /// do a post-order traversal to append the path of the file, stopping at the first "LD.Leads"
        /// so it doesn't log the full path that was on the build machine
        /// </summary>
        /// <param name="di"></param>
        /// <param name="sb"></param>
        private void filterDir(DirectoryInfo di, StringBuilder sb)
        {
            if (di.Name.StartsWith("LD."))
            {
                sb.Append(di.Name);
                return;
            }

            filterDir(di.Parent, sb);
            sb.Append(Path.DirectorySeparatorChar).Append(di.Name);
        }
    }
}
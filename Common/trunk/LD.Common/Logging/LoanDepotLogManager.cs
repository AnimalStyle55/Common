using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

/*
 * This is in the Common.Logging namespace to avoid needing to import this
 * namespace to only get this single class, since you must import Common.Logging to use ILog
*/
namespace /*LD.*/Common.Logging
{
    /// <summary>
    /// Workaround for an issue in Common.Logging
    /// https://github.com/net-commons/common-logging/issues/22
    ///
    /// They have deprecated LogManager.GetLogger() because when declaring loggers
    /// in non-static context with dynamic types (and with dependency injection frameworks),
    /// the current class detection doesn't work.
    ///
    /// This lets us do what GetCurrentClassLogger() did without the Obsolete warnings, and without
    /// having to do .GetLogger(typeof(class)) everywhere.  We only declare our loggers statically.
    /// </summary>
    public static class LoanDepotLogManager
    {
#if !NET40
        // this file is linked in the Net40 version of LD.Common, which defines the NET40 constant.
        // any usage of later .NET functionality is removed via #ifs
        private readonly static AsyncLocal<string> _ctxId = new AsyncLocal<string>();
#endif

        /// <summary>
        /// Get the logger for the current class.
        /// It must be declared as private static readonly at the top of the class
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ILog GetLogger()
        {
            var frame = new StackFrame(1, false);
            var method = frame.GetMethod();

            MethodAttributes required =
                MethodAttributes.Private |
                MethodAttributes.PrivateScope |
                MethodAttributes.Static |
                MethodAttributes.SpecialName;

            if (!method.Attributes.HasFlag(required))
            {
                throw new ArgumentException("log variable must be 'private static readonly' declared at the top of the class");
            }

            var declaringType = method.DeclaringType;

            return LogManager.GetLogger(declaringType);
        }

#if !NET40
        /// <summary>
        /// Set the context ID
        /// </summary>
        /// <param name="contextId">a long, but only the bottom 20 bits are used</param>
        public static void SetContextId(long contextId)
        {
            _ctxId.Value = (contextId & 0x0FFFFF).ToString("X5");
        }

        /// <summary>
        /// Get the context ID
        /// </summary>
        /// <returns>a 5 digit hex number for this async context</returns>
        public static string GetContextId()
        {
            return _ctxId.Value;
        }
#endif
    }
}
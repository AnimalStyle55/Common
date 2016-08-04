using System;

namespace Common.Security
{
    /// <summary>
    /// base class for all exceptions thrown in security classes
    /// </summary>
    public class SecurityException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SecurityException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SecurityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SecurityException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
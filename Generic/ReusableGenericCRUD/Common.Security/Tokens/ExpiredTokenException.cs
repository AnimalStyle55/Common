using System;

namespace Common.Security.Tokens
{
    /// <summary>
    /// exception when a token is expired
    /// </summary>
    public class ExpiredTokenException : SecurityException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExpiredTokenException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExpiredTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExpiredTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
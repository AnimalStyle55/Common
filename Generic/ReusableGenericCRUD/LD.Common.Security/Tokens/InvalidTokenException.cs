using System;

namespace Common.Security.Tokens
{
    /// <summary>
    /// exception when a token could not be decrypted or was invalid
    /// </summary>
    public class InvalidTokenException : SecurityException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidTokenException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
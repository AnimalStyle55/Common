using System;

namespace Common.Database
{
    /// <summary>
    /// Database Exception Type
    /// </summary>
    public enum DatabaseExceptionType
    {
        /// <summary>
        /// A Generic SQL Exception
        /// </summary>
        Unknown,

        /// <summary>
        /// When a row could not be inserted due to duplicate key
        /// </summary>
        KeyConstraintVolation,

        /// <summary>
        /// When a row/data value was expected but was not found
        /// </summary>
        DataValueExpected,

        /// <summary>
        /// User access control authorization check failed
        /// </summary>
        AuthorizationFailed,

        /// <summary>
        /// When an optimistic lock assumption fails, transaction should be retried
        /// </summary>
        OptimisticLockFailed,

        /// <summary>
        /// Number of rows modified by proc didn't match what was expected
        /// </summary>
        ExpectedRowsMismatch,
    }

    /// <summary>Database Exception thrown by LDTS DB Code</summary>
    public class DatabaseException : Exception
    {
        /// <summary>
        /// Type of the exception
        /// </summary>
        public DatabaseExceptionType ExceptionType { get; private set; }

        /// <summary>Constructor</summary>
        public DatabaseException()
        {
            ExceptionType = DatabaseExceptionType.Unknown;
        }

        /// <summary>Constructor</summary>
        public DatabaseException(string message)
            : base(message)
        {
            ExceptionType = DatabaseExceptionType.Unknown;
        }

        /// <summary>Constructor</summary>
        public DatabaseException(string message, Exception inner)
            : base(message, inner)
        {
            ExceptionType = DatabaseExceptionType.Unknown;
        }

        /// <summary>Constructor</summary>
        public DatabaseException(DatabaseExceptionType dbExType)
        {
            ExceptionType = dbExType;
        }

        /// <summary>Constructor</summary>
        public DatabaseException(string message, DatabaseExceptionType dbExType)
            : base(message)
        {
            ExceptionType = dbExType;
        }

        /// <summary>Constructor</summary>
        public DatabaseException(string message, Exception inner, DatabaseExceptionType dbExType)
            : base(message, inner)
        {
            ExceptionType = dbExType;
        }
    }
}
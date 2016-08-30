namespace Common.WebApi.Response
{
    /// <summary>
    /// Enumeration of Api Error Codes
    /// </summary>
    public enum ApiErrorCode
    {
        /// <summary>
        /// Equivalent to returning a 500 level status code
        /// </summary>
        ServerError,

        /// <summary>
        /// Indicates that a parameter value was invalid
        /// </summary>
        ValidationError
    }
}
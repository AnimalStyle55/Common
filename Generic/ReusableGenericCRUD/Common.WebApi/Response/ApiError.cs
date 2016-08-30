using Common.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Common.WebApi.Response
{
    /// <summary>
    /// ApiError Model Object
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// The error code.
        /// </summary>
        [Required]
        public Enum ErrorCode { get; set; }

        /// <summary>
        /// A description of the error.
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// The property name that the error applies to. Typically this is used when the error code is ValidationError.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        /// <param name="propertyName"></param>
        public ApiError(Enum errorCode, string errorDescription = null, string propertyName = null)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription ?? ErrorCode.GetDescription();
            PropertyName = propertyName;
        }
    }
}
using LD.Common.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LD.Common.WebApi.Response
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
        /// ErrorDescription is the description of errorCode.
        /// PropertyName is set to null.
        /// </summary>
        /// <param name="errorCode"></param>
        public ApiError(Enum errorCode)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorCode.GetDescription();
            PropertyName = null;
        }

        /// <summary>
        /// Constructor
        /// PropertyName is set to null.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        public ApiError(Enum errorCode, string errorDescription)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
            PropertyName = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        /// <param name="propertyName"></param>
        public ApiError(Enum errorCode, string errorDescription, string propertyName)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
            PropertyName = propertyName;
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Common.WebApi.Response
{
    /// <summary>
    /// ApiResponse Model Object
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Whether or not the operation was successful
        /// </summary>
        [Required]
        public bool Success { get; set; }

        /// <summary>
        /// Errors that prevented successful completion of the operation. Only present when Success = false
        /// </summary>
        public List<ApiError> Errors { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(bool success = true)
        {
            Success = success;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(IEnumerable<ApiError> errors)
        {
            Success = false;
            Errors = errors.ToList();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(params ApiError[] errors)
            : this((IEnumerable<ApiError>)errors)
        {
        }
    }

    /// <summary>
    /// ApiResponse model object with an embedded object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Response data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Additional metadata for the response data
        /// </summary>
        public object Meta { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(bool success = true) : base(success)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(IEnumerable<ApiError> errors) : base(errors)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApiResponse(params ApiError[] errors) : base(errors)
        {
        }
    }
}
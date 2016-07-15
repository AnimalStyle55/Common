using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LD.Common.WebApi.Response
{
    /// <summary>
    /// Easy way to send Web Responses in WebApi Controllers for loanDepot APIs
    /// </summary>
    public static class ApiWebResponses
    {
        #region Success Responses

        /// <summary>
        /// Success message with a custom status code
        /// </summary>
        public static HttpResponseMessage Success(ApiController ctrl, HttpStatusCode status)
        {
            return ctrl.Request.CreateResponse(
                status,
                new ApiResponse(),
                ctrl.Configuration.Formatters.JsonFormatter);
        }

        /// <summary>
        /// Success message with a custom status code and data
        /// </summary>
        public static HttpResponseMessage Success<T>(ApiController ctrl, HttpStatusCode status, T item)
        {
            return ctrl.Request.CreateResponse(
                status,
                new ApiResponse<T>() { Data = item },
                ctrl.Configuration.Formatters.JsonFormatter);
        }

        /// <summary>
        /// 200 OK
        /// </summary>
        public static HttpResponseMessage Ok(ApiController ctrl)
        {
            return Success(ctrl, HttpStatusCode.OK);
        }

        /// <summary>
        /// 200 OK
        /// </summary>
        public static HttpResponseMessage Ok<T>(ApiController ctrl, T item)
        {
            return Success(ctrl, HttpStatusCode.OK, item);
        }

        /// <summary>
        /// 201 Created
        /// </summary>
        public static HttpResponseMessage Created(ApiController ctrl)
        {
            return Success(ctrl, HttpStatusCode.Created);
        }

        /// <summary>
        /// 201 Created
        /// </summary>
        public static HttpResponseMessage Created<T>(ApiController ctrl, T item)
        {
            return Success(ctrl, HttpStatusCode.Created, item);
        }

        /// <summary>
        /// 204 No Content
        /// </summary>
        public static HttpResponseMessage NoContent(ApiController ctrl)
        {
            return Success(ctrl, HttpStatusCode.NoContent);
        }

        #endregion Success Responses

        #region Error Responses

        /// <summary>
        /// Error exception with custom status code and error enum
        /// </summary>
        public static HttpResponseException Error(ApiController ctrl, HttpStatusCode status, Enum error, string errorDescription = null)
        {
            var response = ctrl.Request.CreateResponse(
                status,
                new ApiResponse(new ApiError(error, errorDescription)),
                ctrl.Configuration.Formatters.JsonFormatter);

            return new HttpResponseException(response);
        }

        /// <summary>
        /// 403 Forbidden (throw)
        /// </summary>
        public static HttpResponseException Forbidden(ApiController ctrl, Enum error, string errorDescription = null)
        {
            return Error(ctrl, HttpStatusCode.Forbidden, error, errorDescription);
        }

        /// <summary>
        /// 401 Unauthorized (throw)
        /// </summary>
        public static HttpResponseException Unauthorized(ApiController ctrl, Enum error, string errorDescription = null)
        {
            return Error(ctrl, HttpStatusCode.Unauthorized, error, errorDescription);
        }

        /// <summary>
        /// 404 Not Found (throw)
        /// </summary>
        public static HttpResponseException NotFound(ApiController ctrl, Enum error, string errorDescription = null)
        {
            return Error(ctrl, HttpStatusCode.NotFound, error, errorDescription);
        }

        /// <summary>
        /// 409 Conflict (throw)
        /// </summary>
        public static HttpResponseException Conflict(ApiController ctrl, Enum error, string errorDescription = null)
        {
            return Error(ctrl, HttpStatusCode.Conflict, error, errorDescription);
        }

        /// <summary>
        /// 412 Precondition Failed (throw)
        /// </summary>
        public static HttpResponseException PreconditionFailed(ApiController ctrl, Enum error, string errorDescription = null)
        {
            return Error(ctrl, HttpStatusCode.PreconditionFailed, error, errorDescription);
        }

        /// <summary>
        /// 400 Bad Request (throw)
        /// </summary>
        public static HttpResponseException BadRequest(ApiController ctrl, Enum error, string errorDescription = null)
        {
            return Error(ctrl, HttpStatusCode.BadRequest, error, errorDescription);
        }

        /// <summary>
        /// 500 Internal Server Error (throw)
        /// </summary>
        public static HttpResponseException InternalServerError(ApiController ctrl, Enum error, string errorDescription = null)
        {
            return Error(ctrl, HttpStatusCode.InternalServerError, error, errorDescription);
        }

        #endregion Error Responses
    }
}
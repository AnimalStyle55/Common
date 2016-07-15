using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LD.Common.WebApi.Response
{
    /// <summary>
    /// Easy way to send Web Responses in WebApi Controllers
    /// </summary>
    public static class WebResponses
    {
        /// <summary>
        /// Redirect to another path
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public static HttpResponseMessage Redirect(ApiController ctrl, string route)
        {
            var response = ctrl.Request.CreateResponse(HttpStatusCode.Redirect);
            string fullyQualifiedUrl = ctrl.Request.RequestUri.GetLeftPart(UriPartial.Authority) + '/' + route.TrimStart('/');
            response.Headers.Location = new Uri(fullyQualifiedUrl);

            return response;
        }

        #region Success Responses

        /// <summary>
        /// Success message with a custom status code
        /// </summary>
        public static HttpResponseMessage Success(ApiController ctrl, HttpStatusCode status, string reasonPhrase = null)
        {
            var response = ctrl.Request.CreateResponse(status);
            if (!string.IsNullOrEmpty(reasonPhrase))
            {
                response.ReasonPhrase = reasonPhrase;
                response.Content = new StringContent(reasonPhrase);
            }

            return response;
        }

        /// <summary>
        /// Success message with a custom status code and data
        /// </summary>
        public static HttpResponseMessage Success<T>(ApiController ctrl, HttpStatusCode status, T item, string reasonPhrase = null)
        {
            var response = ctrl.Request.CreateResponse(status, item, ctrl.Configuration.Formatters.JsonFormatter);
            if (!string.IsNullOrEmpty(reasonPhrase))
                response.ReasonPhrase = reasonPhrase;

            return response;
        }

        /// <summary>
        /// 200 OK
        /// </summary>
        public static HttpResponseMessage Ok(ApiController ctrl, string reasonPhrase = null)
        {
            return Success(ctrl, HttpStatusCode.OK, reasonPhrase);
        }

        /// <summary>
        /// 200 OK
        /// </summary>
        public static HttpResponseMessage Ok<T>(ApiController ctrl, T item, string reasonPhrase = null)
        {
            return Success(ctrl, HttpStatusCode.OK, item, reasonPhrase);
        }

        /// <summary>
        /// 201 Created
        /// </summary>
        public static HttpResponseMessage Created(ApiController ctrl, string message = null)
        {
            return Success(ctrl, HttpStatusCode.Created, message);
        }

        /// <summary>
        /// 201 Created
        /// </summary>
        public static HttpResponseMessage Created<T>(ApiController ctrl, T item, string message = null)
        {
            return Success(ctrl, HttpStatusCode.Created, item, message);
        }

        /// <summary>
        /// 204 No Content
        /// </summary>
        public static HttpResponseMessage NoContent(ApiController ctrl, string message = null)
        {
            return Success(ctrl, HttpStatusCode.NoContent, message);
        }

        #endregion Success Responses

        #region Error Responses

        /// <summary>
        /// Error exception with custom status code (throw)
        /// </summary>
        public static HttpResponseException Error(ApiController ctrl, HttpStatusCode status, string message = null)
        {
            var response = ctrl.Request.CreateErrorResponse(status, message ?? status.ToString());
            return new HttpResponseException(response);
        }

        /// <summary>
        /// 403 Forbidden (throw)
        /// </summary>
        public static HttpResponseException Forbidden(ApiController ctrl, string message = null)
        {
            return Error(ctrl, HttpStatusCode.Forbidden, message);
        }

        /// <summary>
        /// 401 Unauthorized (throw)
        /// </summary>
        public static HttpResponseException Unauthorized(ApiController ctrl, string message = null)
        {
            return Error(ctrl, HttpStatusCode.Unauthorized, message);
        }

        /// <summary>
        /// 404 Not Found (throw)
        /// </summary>
        public static HttpResponseException NotFound(ApiController ctrl, string message = null)
        {
            return Error(ctrl, HttpStatusCode.NotFound, message);
        }

        /// <summary>
        /// 409 Conflict (throw)
        /// </summary>
        public static HttpResponseException Conflict(ApiController ctrl, string message = null)
        {
            return Error(ctrl, HttpStatusCode.Conflict, message);
        }

        /// <summary>
        /// 412 Precondition Failed (throw)
        /// </summary>
        public static HttpResponseException PreconditionFailed(ApiController ctrl, string message = null)
        {
            return Error(ctrl, HttpStatusCode.PreconditionFailed, message);
        }

        /// <summary>
        /// 400 Bad Request (throw)
        /// </summary>
        public static HttpResponseException BadRequest(ApiController ctrl, string message = null)
        {
            return Error(ctrl, HttpStatusCode.BadRequest, message);
        }

        /// <summary>
        /// 500 Internal Server Error (throw)
        /// </summary>
        public static HttpResponseException InternalServerError(ApiController ctrl, string message = null)
        {
            return Error(ctrl, HttpStatusCode.InternalServerError, message);
        }

        #endregion Error Responses
    }
}
using Common.WebApi.Response;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Common.WebApi.Filters
{
    /// <summary>
    /// Use this attribute to return model validation errors in the form of an Api response
    /// object, mapping properties to validation error descriptions.
    /// <code>[ValidateModelFilter]</code>
    /// </summary>
    public class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Handle Action Executing
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var response = new ApiResponse(
                   actionContext.ModelState
                       .SelectMany(kvp => kvp.Value.Errors
                           .Select(e => new ApiError(ApiErrorCode.ValidationError, GetErrorDescription(e), kvp.Key))
                       )
                       .GroupBy(e => new { e.ErrorCode, e.ErrorDescription, e.PropertyName })
                       .Select(g => g.First())
                );

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
        }

        /// <summary>
        /// Get Error Description
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetErrorDescription(ModelError error)
        {
            if (!string.IsNullOrEmpty(error.ErrorMessage))
                return error.ErrorMessage;

            if (!string.IsNullOrEmpty(error.Exception?.Message))
                return error.Exception.Message;

            return "";
        }
    }
}

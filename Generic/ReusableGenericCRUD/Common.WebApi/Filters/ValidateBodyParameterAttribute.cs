using Common.WebApi.Response;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Common.WebApi.Filters
{
    /// <summary>
    /// Use this attribute to ensure the parameter derived from the body of a request (if any) is non-null.
    /// </summary>
    public class ValidateBodyParameterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Handle Action Executing
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // At most there is one parameter which is populated from the payload in the body of a request
            var bodyParam = actionContext?.ActionDescriptor?.ActionBinding?.ParameterBindings
                ?.FirstOrDefault(c => c.WillReadBody);

            if (bodyParam != null) // If a body parameter is present..
            {
                var paramName = bodyParam.Descriptor.ParameterName;
                if (actionContext.ActionArguments[paramName] == null) // ..but null then a validation error has occurred
                {
                    var message = $"missing or mis-formatted json for parameter: {paramName}";
                    var response = new ApiResponse(new ApiError(ApiErrorCode.ValidationError, message));
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
        }
    }
}
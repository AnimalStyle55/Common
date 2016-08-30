using System.Web;
using System.Web.Routing;

namespace Common.WebApi.Routing
{
    /// <summary>
    /// This class is referenced in Web.config so that routing to controllers works in all cases.
    /// The main case is when you have an email at the end of a rest endpoint
    ///     e.g.    /api/v1/borrowers/user@domain.com
    /// </summary>
    public class RoutingHandler : UrlRoutingHandler
    {
        /// <summary>
        /// Override VerifyAndProcessRequest
        /// </summary>
        protected override void VerifyAndProcessRequest(IHttpHandler httpHandler, HttpContextBase httpContext)
        {
            // no need to do anything here
        }
    }
}
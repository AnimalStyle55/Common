using System.Web.Http;

namespace Common.ReusableGenericCRUD.Controllers
{
    public abstract class ApiControllerBase : ApiController
    {
        //public string CallingUser => RequestContext.Principal.Identity.Name;
        public string CallingUser => "me";
    }
}
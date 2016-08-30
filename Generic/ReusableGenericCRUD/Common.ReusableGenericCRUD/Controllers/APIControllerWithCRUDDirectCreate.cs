using Common.Logging;
using Common.ReusableGenericCRUD.Extensions;
using Common.ReusableGenericCRUD.Repository;
using Common.WebApi.Response;
using System.Net.Http;
using System.Web.Http;

namespace Common.ReusableGenericCRUD.Controllers
{
    public class APIControllerWithCRUDDirectCreate<Repo, Item> : ApiControllerWithRUD<Repo, Item> where Repo : IRepository<Item> where Item : IKeyed
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        public APIControllerWithCRUDDirectCreate(Repo repository) : base(repository) { }

        [Route("")]
        [HttpPost]
        public HttpResponseMessage Create([FromBody]Item item)
        {
            if (item == null || item.Id.IsNotNullNullOrEmpty())
                throw WebResponses.PreconditionFailed(this);

            Item response = Repository.Create(CallingUser, item);

            return WebResponses.Created(this, response);
        }
    }
}
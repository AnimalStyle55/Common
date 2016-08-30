using Common.Logging;
using Common.ReusableGenericCRUD.Extensions;
using Common.ReusableGenericCRUD.Repository;
using Common.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Common.ReusableGenericCRUD.Controllers
{
    public class ApiControllerWithRUD<Repo, Item> : ApiControllerBase where Repo : IRepository<Item> where Item : IKeyed
    {
        private static readonly ILog log = CommonLogManager.GetLogger();

        public IRepository<Item> Repository;

        public ApiControllerWithRUD(Repo repository)
        {
            Repository = repository;
        }

        [Route("")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            List<Item> items = Repository.GetAll(CallingUser);
            
            if (items.Count < 1)
                return WebResponses.NoContent(this);
            
            return WebResponses.Ok(this, items.Select(k => (IKeyed)k).ToList().ConvertToDictionary());
        }

        [Route("{id}")]
        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            Item item = Repository.Get(CallingUser, id);

            if (item == null)
                throw WebResponses.NotFound(this);

            return WebResponses.Ok(this, item);
        }

        [Route("{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            if (id.IsNullOrEmpty())
                throw WebResponses.BadRequest(this);

            Repository.Delete(CallingUser, id);

            return WebResponses.Ok(this);
        }

        [Route("{id}")]
        [HttpPut]
        public HttpResponseMessage Update([FromUri]Guid id, [FromBody]Item item)
        {
            if (item == null || item.Id.IsNullOrEmpty())
                throw WebResponses.PreconditionFailed(this);

            Repository.Update(CallingUser, item);

            return WebResponses.Ok(this);
        }
    }
}
using Common.ReusableGenericCRUD.DAO;
using Common.ReusableGenericCRUD.Extensions;
using Common.ReusableGenericCRUD.Repository;
using Common.WebApi.Response;
using System.Net.Http;
using System.Web.Http;

namespace Common.ReusableGenericCRUD.Controllers
{
    public class APIControllerWithCRUDAndParentRelationShip<RelationshipRepo, Repo, RelationshipItem, Item, ItemWithParent> : ApiControllerWithRUD<Repo, Item> where RelationshipRepo : ICompositeKeyWithOrderRepository<RelationshipItem> where RelationshipItem : ICompositeKeyedWithOrder where Repo : IRepository<Item> where Item : IKeyed where ItemWithParent : Item, IParentKeyed, IKeyed
    {
        private ICompositeKeyWithOrderRepository<RelationshipItem> RelationshipRepository;
        public APIControllerWithCRUDAndParentRelationShip(RelationshipRepo relationshipRepository, Repo repository) : base(repository)
        {
            RelationshipRepository = relationshipRepository;
        }

        [Route("")]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] ItemWithParent item)
        {
            Item response = Repository.Create(CallingUser, item);

            if(item.ParentId.IsNotNullNullOrEmpty())
                RelationshipRepository.AddToEnd(CallingUser, item.ParentId, item.Id);

            return WebResponses.Created(this, response);
        }
    }
}
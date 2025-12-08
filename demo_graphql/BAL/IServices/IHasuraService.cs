using demo_graphql.Models;

namespace demo_graphql.Controllers
{
  public interface IHasuraService
  {
    Task<Response> Post(GraphQLRequestModel requestModel, IHeaderDictionary additionalHeaders, int LoginPersonId);
  }
}
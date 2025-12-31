using demo_graphql.Models;

namespace demo_graphql.Controllers
{
  public interface IGLService
  {
    Task<Response> Post(GraphQLRequestModel requestModel, int positionId, int userId);
  }
}
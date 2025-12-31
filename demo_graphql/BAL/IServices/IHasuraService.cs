using demo_graphql.Models;

namespace demo_graphql.BAL.IServices
{
  public interface IHasuraService
  {
    Task<Response> Post(GraphQLRequestModel requestModel);
  }
}
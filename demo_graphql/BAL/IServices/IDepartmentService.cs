using demo_graphql.Models;

namespace demo_graphql.Controllers
{
  public interface IDepartmentService
  {
    Task<Response> GetAllAsync();

  }
}
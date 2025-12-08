using demo_graphql.Models;

namespace demo_graphql.Controllers
{
  public interface IEmployeeService
  {
    Task<Response> GetDashboardSummary();

  }
}
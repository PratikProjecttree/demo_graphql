using demo_graphql.Models;

namespace demo_graphql.Controllers
{
  public interface IDashboardService
  {
    Task<Response> GetDashboardSummary();

  }
}
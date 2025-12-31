using demo_graphql.Models;

namespace demo_graphql.BAL.IServices
{
  public interface IDashboardService
  {
    Task<Response> GetDashboardSummary();

  }
}
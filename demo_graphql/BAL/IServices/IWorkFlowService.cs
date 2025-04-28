using demo_graphql.Models;

namespace demo_graphql.Controllers
{
  public interface IWorkFlowService
  {
    Task<Response> Request(WorkflowModel requestModel);
  }
}
using demo_graphql.Models;

namespace demo_graphql.BAL.IServices
{
  public interface IWorkFlowService
  {
    Task<Response> Request(WorkflowModel requestModel, Dictionary<string, object> payload);
  }
}
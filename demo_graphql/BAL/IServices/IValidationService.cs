using demo_graphql.Models;

namespace demo_graphql.BAL.IServices
{
    public interface IValidationService
    {
        Response ValidateInputValidationMeta(GraphQLRequestModel requestModel, GLRoutingModel routingModel);
        List<ResponseMessage> ValidateInputAgainstMeta(GLRoutingModel model, Dictionary<string, object> inputValues);
        List<ResponseMessage> ValidateUpdateMutation(string query);
    }
}

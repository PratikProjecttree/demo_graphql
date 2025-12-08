using System.Text.Json;
using demo_graphql.BAL.IServices;
using demo_graphql.Models;
using static demo_graphql.Controllers.QueryInspector;

namespace demo_graphql.Controllers
{
    public class GLService(IDapperService _dapperService,
        IHasuraService _hasuraService,
        IWorkFlowService _workFlowService,
        IValidationService _validationService
        ) : IGLService
    {

        public async Task<Response> Post(GraphQLRequestModel requestModel)
        {
            Response _response = new();

            //Query validation: if not valid
            if (!GLInspector.IsValidGraphQLSyntax(requestModel.query))
            {
                _response.data = null;
                _response.responseMessages.Add(new ResponseMessage() { message = "Invalid syntax", type = "E" });
                return _response;
            }
            // get all query list
            var (operationType, queryList) = GLInspector.GetOperationTypeAndTopLevelFieldNames(requestModel.query);

            var routingModels = await _dapperService.QueryAsync<GLRoutingModel>(PostGresQuery.Get_request_meta, new { queryList });

            foreach (var routingModel in routingModels ?? new List<GLRoutingModel>())
            {
                var validationResult = ProcessRouting(routingModel, operationType, requestModel);

                if (validationResult.responseMessages.Any(m => m.type == "E"))
                {
                    _response.data = null;
                    _response.responseMessages.AddRange(validationResult.responseMessages);
                    return _response;
                }
            }

            //hasura request
            _response = await _hasuraService.Post(requestModel);

            return _response;
        }
        private Response ProcessRouting(GLRoutingModel routingModel, string operationType, GraphQLRequestModel requestModel)
        {
            // Create a routing key tuple for switch case
            var routingKey = (routingModel.category, operationType);

            switch (routingKey)
            {
                case (Category.Default, QueryType.Mutation):
                    return _validationService.ValidateInputValidationMeta(requestModel, routingModel);

                case (Category.Workflow, QueryType.Mutation):
                    if (routingModel.workflow_meta == null)
                    {
                        return new Response
                        {
                            data = null,
                            responseMessages = new List<ResponseMessage>
                            {
                                new ResponseMessage { message = "Workflow meta is not found", type = "E" }
                            }
                        };
                    }
                    return _validationService.ValidateInputValidationMeta(requestModel, routingModel);

                case (Category.Custom, QueryType.Mutation):
                    if (routingModel.custom_meta == null)
                    {
                        return new Response
                        {
                            data = null,
                            responseMessages = new List<ResponseMessage>
                            {
                                new ResponseMessage { message = "Custom meta is not found", type = "E" }
                            }
                        };
                    }
                    return _validationService.ValidateInputValidationMeta(requestModel, routingModel);

                case (Category.Default, QueryType.Query):
                case (Category.Custom, QueryType.Query):

                    return new Response();
                default:
                    return new Response();
            }
        }
    }
}
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

        public async Task<Response> Post(GraphQLRequestModel requestModel, IHeaderDictionary additionalHeaders, int LoginPersonId)
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

            // Routing to Hasura
            var _getProcessRequest = await _dapperService.QueryFirstOrDefaultAsync<string>(PostGresQuery.fn_process_request, new { LoginPersonId, queryList });
            var routingModels = JsonSerializer.Deserialize<List<GLRoutingModel>>(_getProcessRequest ?? "");

            if (routingModels == null || routingModels?.Count == 0)
            {
                _response.data = null;
                _response.responseMessages.Add(new ResponseMessage() { message = "Request meta not found", type = "E" });
                return _response;
            }

            foreach (var routingModel in routingModels ?? new List<GLRoutingModel>())
            {
                var validationResult = ProcessRouting(routingModel, operationType, requestModel);

                if (validationResult.responseMessages.Any(m => m.type == "E"))
                {
                    _response.data = null;
                    _response.responseMessages.AddRange(validationResult.responseMessages);
                    return _response;
                }
                // For workflow query operations, process and return immediately
                if (routingModel.category == Category.Workflow && operationType == QueryType.Query)
                {
                    if (routingModel.workflow_meta != null)
                    {
                        return await _workFlowService.Request(routingModel.workflow_meta);
                    }
                    else
                    {
                        _response.data = null;
                        _response.responseMessages.Add(new ResponseMessage { message = "Workflow meta is not found", type = "E" });
                        return _response;
                    }
                }

                #region commented
                //if (operationType == QueryType.Query)
                //{
                //    if (routingModel.input_validation_meta != null)
                //    {
                //        var whereConditions = GLInspector.ExtractFilterFieldsWithValues(requestModel.query);

                //        var validationMessages = ValidateInputAgainstMeta(routingModel, whereConditions);

                //        if (validationMessages.Any(m => m.type == "E"))
                //        {
                //            _response.data = null;
                //            _response.responseMessages.AddRange(validationMessages);
                //            return _response;
                //        }
                //    }
                //}
                #endregion
            }

            // Validate update mutation
            if (operationType == QueryType.Mutation && requestModel.query.Contains("update_"))
            {
                var validationMessages = _validationService.ValidateUpdateMutation(requestModel.query);

                if (validationMessages.Any(m => m.type == "E"))
                {
                    _response.data = null;
                    _response.responseMessages.AddRange(validationMessages);
                    return _response;
                }
            }


            //hasura request
            _response = await _hasuraService.Post(requestModel, additionalHeaders, LoginPersonId);

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

                    #region commented
                    //As of now we not validating any thig if its queryType is Query.
                    //if (routingModel.input_validation_meta != null)
                    //{
                    //    var whereConditions = GLInspector.ExtractFilterFieldsWithValues(requestModel.query);
                    //    var validationMessages = _validationService.ValidateInputAgainstMeta(routingModel, whereConditions);

                    //    if (validationMessages.Any(m => m.type == "E"))
                    //    {
                    //        return new Response
                    //        {
                    //            data = null,
                    //            responseMessages = validationMessages
                    //        };
                    //    }
                    //}
                    #endregion
                    return new Response();

                // Case for Workflow + Query is handled separately in the main method

                default:
                    return new Response();
            }
        }
    }
}
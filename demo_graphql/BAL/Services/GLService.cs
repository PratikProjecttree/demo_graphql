using System.Net;
using demo_graphql.BAL.IServices;
using demo_graphql.Models;
using demo_graphql.Models.EmailModels;
using static demo_graphql.BAL.QueryInspector;

namespace demo_graphql.BAL.Services
{
    public class GLService(IDapperService _dapperService,
        IHasuraService _hasuraService,
        IWorkFlowService _workFlowService,
        IValidationService _validationService,
        IAuthenticationService _authenticationService,
        IEmailQueueService _emailQueueService
        ) : IGLService
    {

        public async Task<Response> Post(GraphQLRequestModel requestModel, int positionId, int userId)
        {
            Response _response = new();

            //Query validation: if not valid
            if (!GLInspector.IsValidGraphQLSyntax(requestModel.query))
            {
                _response.data = null;
                _response.responseMessages.Add(new ResponseMessage() { message = "Invalid syntax", type = "E", statusCode = (int)HttpStatusCode.BadRequest });
                return _response;
            }

            // get all query list
            var (operationType, queryList) = GLInspector.GetOperationTypeAndTopLevelFieldNames(requestModel.query);
            var routingModels = await _dapperService.QueryAsync<GLRoutingModel>(PostGresQuery.Get_request_meta, new { queryList });

            foreach (var routingModel in routingModels ?? new List<GLRoutingModel>())
            {
                #region :: Asm module access validation ::
                if (!string.IsNullOrWhiteSpace(routingModel.system_module_access_codes))
                {
                    var moduleAccessCodes = routingModel.system_module_access_codes.Split(',').Select(s => s.Trim()).ToList();
                    var hasAccess = await _authenticationService.HasModulePermission(positionId, userId, moduleAccessCodes, routingModel.object_name ?? "", operationType);
                    if (!hasAccess)
                    {
                        _response.data = null;
                        _response.responseMessages.Add(new ResponseMessage { message = "You do not have permission to perform this action.", type = "E", statusCode = (int)HttpStatusCode.Forbidden });
                        return _response;
                    }
                }
                #endregion

                #region  :: Routing processing and validation ::
                if (operationType != OperationType.Query)
                {
                    var validationResult = ProcessRouting(routingModel, operationType, requestModel);

                    if (validationResult.responseMessages.Any(m => m.type == "E"))
                    {
                        _response.data = null;
                        _response.responseMessages.AddRange(validationResult.responseMessages);
                        return _response;
                    }
                }
                #endregion

                #region :: workflow query operations, process and return immediately ::
                if (routingModel.category == Category.Workflow && operationType == QueryType.Mutation)
                {
                    if (routingModel.workflow_meta != null)
                    {
                        // extract payload and parameters
                        var objectDataList = GLInspector.ExtractObjectsArguments(requestModel.query);
                        Dictionary<string, object>? mergedPayload = objectDataList.Where(x => x.Role == ArgRole.Payload || x.Role == ArgRole.Parameter).SelectMany(x => x.Fields).ToDictionary(k => k.Key, v => (object)v.Value);

                        // 1️⃣ Normalize Hasura "data" pattern
                        var normalized = GLInspector.NormalizeHasuraData(mergedPayload);

                        // 2️⃣ Convert flat → nested JSON
                        var payload = GLInspector.Unflatten(normalized);

                        // 3️⃣ Process workflow request
                        var wfResult = await _workFlowService.Request(routingModel.workflow_meta, payload);
                        if (wfResult.responseMessages.Any(m => m.type == "E"))
                        {
                            _response.data = null;
                            _response.responseMessages.AddRange(wfResult.responseMessages);
                            return _response;
                        }
                        else
                        {
                            #region :: Email Queue processing (Non-blocking) ::

                            if (!string.IsNullOrEmpty(routingModel.email_configuration_raw))
                            {
                                var emailConfig = routingModel.email_configuration;
                                if (emailConfig != null && emailConfig.IsEnableEmail)
                                {
                                    EmailQueue(emailConfig);
                                }
                            }

                            #endregion

                            return wfResult;
                        }
                    }
                    else
                    {
                        _response.data = null;
                        _response.responseMessages.Add(new ResponseMessage { message = "Workflow meta is not found", type = "E" });
                        return _response;
                    }
                }
                #endregion

                #region :: Email Queue processing (Non-blocking) ::

                if (!string.IsNullOrEmpty(routingModel.email_configuration_raw))
                {
                    var emailConfig = routingModel.email_configuration;
                    if (emailConfig != null && emailConfig.IsEnableEmail)
                    {
                        EmailQueue(emailConfig);
                    }
                }

                #endregion

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
        private void EmailQueue(EmailQueueRequest emailConfig)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailQueueService.AddOrUpdateEmailQueue(emailConfig);
                }
                catch (Exception ex)
                {
                }
            });
        }
    }
}
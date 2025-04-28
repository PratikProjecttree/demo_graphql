using System.Text.Json;
using demo_graphql.Models;
using GraphQLParser;
using GraphQLParser.AST;
using static demo_graphql.Controllers.QueryInspector;

namespace demo_graphql.Controllers
{
    public class GLService : IGLService
    {
        private readonly IDapperService _dapperService;
        private readonly IHasuraService _hasuraService;
        private readonly IWorkFlowService _workFlowService;

        public GLService(IDapperService dapperService, IHasuraService hasuraService, IWorkFlowService workFlowService)
        {
            _dapperService = dapperService;
            _hasuraService = hasuraService;
            _workFlowService = workFlowService;
        }

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
            var getProcessRequestList = JsonSerializer.Deserialize<List<GLRoutingModel>>(_getProcessRequest ?? "");

            if (getProcessRequestList == null || getProcessRequestList?.Count == 0)
            {
                _response.data = null;
                _response.responseMessages.Add(new ResponseMessage() { message = "Request meta not found", type = "E" });
                return _response;
            }

            foreach (var getProcessRequest in getProcessRequestList ?? new List<GLRoutingModel>())
            {

                if (getProcessRequest.category == Category.Default && operationType == QueryType.Mutation)
                {
                    var validationResponse = ValidateInputValidationMeta(requestModel, getProcessRequest);
                    if (validationResponse.responseMessages.Any(m => m.type == "E"))
                    {
                        _response.data = null;
                        _response.responseMessages.AddRange(validationResponse.responseMessages);
                        return _response;
                    }
                }

                if (getProcessRequest.category == Category.Workflow && operationType == QueryType.Mutation)
                {
                    if (getProcessRequest.workflow_meta != null)
                    {
                        var validationResponse = ValidateInputValidationMeta(requestModel, getProcessRequest);
                        if (validationResponse.responseMessages.Any(m => m.type == "E"))
                        {
                            _response.data = null;
                            _response.responseMessages.AddRange(validationResponse.responseMessages);
                            return _response;
                        }
                    }
                    else
                    {
                        _response.data = null;
                        _response.responseMessages.Add(new ResponseMessage() { message = "Workflow meta is not found", type = "E" });
                        return _response;
                    }
                }

                if (getProcessRequest.category == Category.Workflow && operationType == QueryType.Query)
                {
                    if (getProcessRequest.workflow_meta != null)
                    {
                        _response = await _workFlowService.Request(getProcessRequest.workflow_meta ?? new WorkflowModel());
                        return _response;
                    }
                    else
                    {
                        _response.data = null;
                        _response.responseMessages.Add(new ResponseMessage() { message = "Workflow meta is not found", type = "E" });
                        return _response;
                    }
                }

                if (getProcessRequest.category == Category.Custom && operationType == QueryType.Mutation)
                {

                    if (getProcessRequest.custom_meta != null)
                    {
                        // Custom validation logic for mutation
                        var validationResponse = ValidateInputValidationMeta(requestModel, getProcessRequest);
                        if (validationResponse.responseMessages.Any(m => m.type == "E"))
                        {
                            _response.data = null;
                            _response.responseMessages.AddRange(validationResponse.responseMessages);
                            return _response;
                        }
                    }
                    else
                    {
                        _response.data = null;
                        _response.responseMessages.Add(new ResponseMessage() { message = "Custom meta is not found", type = "E" });
                        return _response;
                    }
                }

                if (operationType == QueryType.Query)
                {
                    if (getProcessRequest.input_validation_meta != null)
                    {
                        var whereConditions = GLInspector.ExtractFilterFieldsWithValues(requestModel.query);

                        var validationMessages = ValidateInputAgainstMeta(getProcessRequest, whereConditions);

                        if (validationMessages.Any(m => m.type == "E"))
                        {
                            _response.data = null;
                            _response.responseMessages.AddRange(validationMessages);
                            return _response;
                        }
                    }
                }
            }

            // Validate update mutation
            if (operationType == QueryType.Mutation && requestModel.query.Contains("update_"))
            {
                var validationMessages = ValidateUpdateMutation(requestModel.query);

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

        private Response ValidateInputValidationMeta(GraphQLRequestModel requestModel, GLRoutingModel getProcessRequest)
        {
            var response = new Response();

            if (getProcessRequest.input_validation_meta != null)
            {
                var objectDataList = GLInspector.ExtractObjectsArguments(requestModel.query);

                var validationMessages = InputValidationMetaValidateFields(objectDataList, getProcessRequest);

                if (validationMessages.Any(m => m.type == "E"))
                {
                    response.data = null;
                    response.responseMessages.AddRange(validationMessages);
                }
            }
            return response;
        }

        public static List<ResponseMessage> InputValidationMetaValidateFields(List<Dictionary<string, string>> objectDataList, GLRoutingModel getProcessRequest)
        {
            var responseMessages = new List<ResponseMessage>();

            if (string.IsNullOrEmpty(getProcessRequest.object_name) || getProcessRequest.input_validation_meta == null)
                return responseMessages;

            var expectedFields = getProcessRequest.input_validation_meta;

            foreach (var obj in objectDataList)
            {
                foreach (var fieldKey in expectedFields.Keys)
                {
                    var fieldMeta = expectedFields[fieldKey];

                    // 1. If field is marked as isNotAllow (field should not exist at all)
                    if (fieldMeta.isNotAllow)
                    {
                        // Check if the field exists in the request data
                        if (obj.ContainsKey(fieldKey))
                        {
                            responseMessages.Add(new ResponseMessage
                            {
                                message = $"Field '{fieldKey}' should not exist because it is marked as not allowed.",
                                type = "E"
                            });
                            continue;
                        }
                    }

                    // Check required field
                    if (fieldMeta.required && (!obj.TryGetValue(fieldKey, out var value) || string.IsNullOrWhiteSpace(value)))
                    {
                        responseMessages.Add(new ResponseMessage
                        {
                            message = $"Field '{fieldKey}' is required.",
                            type = "E"
                        });
                        continue;
                    }

                    if (obj.TryGetValue(fieldKey, out var val))
                    {
                        // Check min length
                        if (val.Length < fieldMeta.minLength)
                        {
                            responseMessages.Add(new ResponseMessage
                            {
                                message = $"Field '{fieldKey}' should be at least {fieldMeta.minLength} characters long.",
                                type = "E"
                            });
                        }

                        // Check max length
                        if (val.Length > fieldMeta.maxLength)
                        {
                            responseMessages.Add(new ResponseMessage
                            {
                                message = $"Field '{fieldKey}' should be at most {fieldMeta.maxLength} characters long.",
                                type = "E"
                            });
                        }
                    }
                }
            }
            return responseMessages;
        }

        public static List<ResponseMessage> ValidateInputAgainstMeta(GLRoutingModel model, Dictionary<string, object> inputValues)
        {
            var responseMessages = new List<ResponseMessage>();

            if (model.input_validation_meta != null)
            {
                // 1. Check for required fields
                foreach (var fieldEntry in model.input_validation_meta)
                {
                    var fieldKey = fieldEntry.Key;
                    var fieldMeta = fieldEntry.Value;

                    // 1.1. If field is marked as 'isNotAllow' (field should not exist)
                    if (fieldMeta.isNotAllow)
                    {
                        if (inputValues.ContainsKey(fieldKey))
                        {
                            responseMessages.Add(new ResponseMessage
                            {
                                message = $"Field '{fieldKey}' should not exist because it is marked as not allowed.",
                                type = "E"
                            });
                            continue; // Skip further checks for this field
                        }
                    }

                    if (fieldMeta.required &&
                        (!inputValues.TryGetValue(fieldKey, out var value) ||
                         value == null ||
                         string.IsNullOrWhiteSpace(value.ToString())))
                    {
                        responseMessages.Add(new ResponseMessage
                        {
                            message = $"Field '{fieldKey}' is required.",
                            type = "E"
                        });
                        continue;
                    }

                    // 2. Type check (if value exists and is not null)
                    if (inputValues.TryGetValue(fieldKey, out var actualValue) && actualValue != null)
                    {
                        var actualString = actualValue.ToString();

                        // Only validate if type is defined in meta
                        if (!string.IsNullOrWhiteSpace(fieldMeta.type))
                        {
                            var expectedType = fieldMeta.type.ToLower();

                            bool typeMismatch = expectedType switch
                            {
                                "int" => !int.TryParse(actualString, out _),
                                "float" => !float.TryParse(actualString, out _),
                                "bool" or "boolean" => !bool.TryParse(actualString, out _),
                                "string" => false, // Everything is valid as string
                                _ => false
                            };

                            if (typeMismatch)
                            {
                                responseMessages.Add(new ResponseMessage
                                {
                                    message = $"Field '{fieldKey}' must be of type '{fieldMeta.type}'.",
                                    type = "E"
                                });
                            }
                        }
                    }
                }
            }
            return responseMessages;
        }

        public static List<ResponseMessage> ValidateUpdateMutation(string query)
        {
            var responseMessages = new List<ResponseMessage>();

            var document = Parser.Parse(query);

            // Extract operation definitions
            var operationDefinition = document.Definitions.OfType<GraphQLOperationDefinition>().FirstOrDefault();

            if (operationDefinition != null)
            {
                // Extract the selection sets from the operation
                foreach (var selection in operationDefinition.SelectionSet.Selections.OfType<GraphQLField>())
                {
                    // Check if the selection name matches the dynamic operation (e.g., update_mds_departments)
                    if (selection.Name.StringValue.StartsWith("update_"))
                    {
                        // Check if the 'where' argument exists
                        var whereArgument = selection.Arguments.FirstOrDefault(arg => arg.Name.StringValue == "where");
                        if (whereArgument != null && whereArgument.Value is GraphQLObjectValue whereCondition)
                        {
                            // Check for forbidden conditions in the 'where' condition
                            if (ContainsForbiddenConditions(whereCondition))
                            {
                                responseMessages.Add(new ResponseMessage
                                {
                                    message = "The 'where' condition can only contain '_eq', '_in', '_and' operators. Other operators are not allowed in update.",
                                    type = "E"
                                });
                            }
                        }
                    }
                }
            }

            return responseMessages;
        }

        // Helper method to check for forbidden conditions
        private static bool ContainsForbiddenConditions(GraphQLObjectValue whereCondition)
        {
            var allowedOperators = new List<string> { "_eq", "_in", "_and" };

            foreach (var field in whereCondition?.Fields)
            {
                var fieldName = field.Name.StringValue; // Always use StringValue!

                // Check if field is an operator (starts with '_')
                if (fieldName.StartsWith("_"))
                {
                    if (!allowedOperators.Contains(fieldName))
                    {
                        return true; // Found forbidden operator
                    }
                }

                // Recursively check nested objects
                if (field.Value is GraphQLObjectValue nestedObject && ContainsForbiddenConditions(nestedObject))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
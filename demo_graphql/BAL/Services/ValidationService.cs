using demo_graphql.BAL.IServices;
using demo_graphql.Controllers;
using demo_graphql.Models;
using GraphQLParser.AST;
using GraphQLParser;

namespace demo_graphql.BAL.Services
{
    public class ValidationService : IValidationService
    {
        public Response ValidateInputValidationMeta(GraphQLRequestModel requestModel, GLRoutingModel routingModel)
        {
            var response = new Response();

            if (routingModel.input_validation_meta != null)
            {
                var objectDataList = GLInspector.ExtractObjectsArguments(requestModel.query);
                var validationMessages = InputValidationMetaValidateFields(objectDataList, routingModel);

                if (validationMessages.Any(m => m.type == "E"))
                {
                    response.data = null;
                    response.responseMessages.AddRange(validationMessages);
                }
            }
            return response;
        }
        public List<ResponseMessage> ValidateInputAgainstMeta(GLRoutingModel model, Dictionary<string, object> inputValues)
        {
            var responseMessages = new List<ResponseMessage>();

            if (model.input_validation_meta != null)
            {
                // Check each field against validation meta
                foreach (var fieldEntry in model.input_validation_meta)
                {
                    var fieldKey = fieldEntry.Key;
                    var fieldMeta = fieldEntry.Value;

                    // Check if field is not allowed
                    if (fieldMeta.isNotAllow && inputValues.ContainsKey(fieldKey))
                    {
                        responseMessages.Add(new ResponseMessage
                        {
                            message = $"Field '{fieldKey}' should not exist because it is marked as not allowed.",
                            type = "E"
                        });
                        continue;
                    }

                    // Check required field
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

                    // Type validation
                    if (inputValues.TryGetValue(fieldKey, out var actualValue) &&
                        actualValue != null &&
                        !string.IsNullOrWhiteSpace(fieldMeta.type))
                    {
                        var actualString = actualValue.ToString();
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
            return responseMessages;
        }

        public List<ResponseMessage> ValidateUpdateMutation(string query)
        {
            var responseMessages = new List<ResponseMessage>();
            var document = Parser.Parse(query);
            var operationDefinition = document.Definitions.OfType<GraphQLOperationDefinition>().FirstOrDefault();

            if (operationDefinition != null)
            {
                foreach (var selection in operationDefinition.SelectionSet.Selections.OfType<GraphQLField>())
                {
                    if (selection.Name.StringValue.StartsWith("update_"))
                    {
                        var whereArgument = selection.Arguments.FirstOrDefault(arg => arg.Name.StringValue == "where");
                        if (whereArgument != null && whereArgument.Value is GraphQLObjectValue whereCondition)
                        {
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

        private List<ResponseMessage> InputValidationMetaValidateFields(List<Dictionary<string, string>> objectDataList, GLRoutingModel routingModel)
        {
            var responseMessages = new List<ResponseMessage>();

            if (string.IsNullOrEmpty(routingModel.object_name) || routingModel.input_validation_meta == null)
                return responseMessages;

            var expectedFields = routingModel.input_validation_meta;

            foreach (var obj in objectDataList)
            {
                foreach (var fieldKey in expectedFields.Keys)
                {
                    var fieldMeta = expectedFields[fieldKey];

                    // Check if field is not allowed
                    if (fieldMeta.isNotAllow && obj.ContainsKey(fieldKey))
                    {
                        responseMessages.Add(new ResponseMessage
                        {
                            message = $"Field '{fieldKey}' should not exist because it is marked as not allowed.",
                            type = "E"
                        });
                        continue;
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

        // Helper method to check for forbidden conditions
        private bool ContainsForbiddenConditions(GraphQLObjectValue whereCondition)
        {

            foreach (var field in whereCondition?.Fields)
            {
                var fieldName = field.Name.StringValue;

                // Check if field is an operator (starts with '_')
                if (fieldName.StartsWith("_") && !AllowedUpdateOperators.AllowedOperators.Contains(fieldName))
                {
                    return true; // Found forbidden operator
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

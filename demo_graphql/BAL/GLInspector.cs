using GraphQLParser;
using GraphQLParser.AST;
using GraphQLParser.Exceptions;

namespace demo_graphql.Controllers
{
    public class GLInspector
    {

        public static bool IsValidGraphQLSyntax(string query)
        {
            try
            {
                var document = Parser.Parse(query);
                return true;
            }
            catch (GraphQLSyntaxErrorException ex)
            {
                return false;
            }
        }

        public static (string OperationType, List<string> FieldNames) GetOperationTypeAndTopLevelFieldNames(string query)
        {
            var document = Parser.Parse(query);

            var fieldNames = new List<string>();
            string operationType = string.Empty;

            foreach (var definition in document.Definitions)
            {
                if (definition is GraphQLOperationDefinition operation)
                {
                    operationType = operation.Operation.ToString(); // "Query", "Mutation", or "Subscription"

                    var selections = operation.SelectionSet.Selections
                        .OfType<GraphQLField>();

                    foreach (var field in selections)
                    {
                        fieldNames.Add(field.Name.StringValue);
                    }
                }
            }

            return (operationType, fieldNames);
        }


        public static List<Dictionary<string, string>> ExtractObjectsArguments(string query)
        {
            var result = new List<Dictionary<string, string>>();
            var document = Parser.Parse(query);

            foreach (var definition in document.Definitions)
            {
                if (definition is GraphQLOperationDefinition operationDef)
                {
                    foreach (var selection in operationDef.SelectionSet.Selections.OfType<GraphQLField>())
                    {
                        foreach (var argument in selection.Arguments)
                        {
                            if (argument.Name.ToString() == "objects")
                            {
                                if (argument.Value is GraphQLListValue listValue)
                                {
                                    // Iterate over each object in the list of objects
                                    foreach (var item in listValue.Values.OfType<GraphQLObjectValue>())
                                    {
                                        var objectData = new Dictionary<string, string>();

                                        // Extract fields from each object and add to the dictionary
                                        foreach (var field in item.Fields)
                                        {
                                            string key = field.Name.ToString();
                                            string value = ExtractValue(field.Value);
                                            objectData[key] = value;
                                        }

                                        // Add the object data to the result list
                                        result.Add(objectData);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static string ExtractValue(ASTNode value)
        {
            return value switch
            {
                GraphQLStringValue strVal => strVal.Value.ToString(),
                GraphQLIntValue intVal => intVal.Value.ToString(),
                GraphQLBooleanValue boolVal => boolVal.Value.ToString().ToLower(),
                GraphQLFloatValue floatVal => floatVal.Value.ToString(),
                _ => value.ToString()
            };
        }

        public static Dictionary<string, object> ExtractFilterFieldsWithValues(string query)
        {
            var filterFields = new Dictionary<string, object>();

            // Parse the GraphQL query
            var document = Parser.Parse(query);

            // Go through all definitions
            foreach (var definition in document.Definitions)
            {
                if (definition is GraphQLOperationDefinition operation)
                {
                    foreach (var selection in operation.SelectionSet?.Selections ?? Enumerable.Empty<ASTNode>())
                    {
                        if (selection is GraphQLField field && field.Arguments != null)
                        {
                            foreach (var argument in field.Arguments)
                            {
                                if (argument?.Name?.StringValue == "where" &&
                                    argument.Value is GraphQLObjectValue filterValue)
                                {
                                    TraverseObjectValue(filterValue, filterFields);
                                }
                            }
                        }
                    }
                }
            }

            return filterFields;
        }

        // Recursively traverse and collect filter fields and their values
        private static void TraverseObjectValue(GraphQLObjectValue obj, Dictionary<string, object> result)
        {
            foreach (var field in obj.Fields)
            {
                var key = field.Name.StringValue;

                if (field.Value is GraphQLObjectValue nestedObj)
                {
                    // If there are multiple conditions (_eq, _in, etc.), pick the first one
                    foreach (var cond in nestedObj.Fields)
                    {
                        object? value = cond.Value switch
                        {
                            GraphQLIntValue intVal => int.Parse(intVal.Value),
                            GraphQLStringValue strVal => strVal.Value,
                            GraphQLBooleanValue boolVal => boolVal.Value,
                            GraphQLFloatValue floatVal => float.Parse(floatVal.Value),
                            _ => null
                        };
                        result[key] = value;
                        break; // Only take the first condition
                    }
                }
                else
                {
                    // Direct value, if not wrapped in _eq, _in, etc.
                    result[key] = field.Value.ToString();
                }
            }
        }
    }
}
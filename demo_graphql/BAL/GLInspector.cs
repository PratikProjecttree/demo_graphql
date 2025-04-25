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
            // Initialize the filter fields dictionary
            var filterFields = new Dictionary<string, object>();

            // Parse the GraphQL query
            var document = Parser.Parse(query);

            // Iterate over the document definitions (operations or fragments)
            foreach (var definition in document.Definitions)
            {
                if (definition is GraphQLOperationDefinition operation)
                {
                    foreach (var selection in operation.SelectionSet.Selections)
                    {
                        if (selection is GraphQLField field)
                        {
                            foreach (var argument in field?.Arguments)
                            {
                                // Look for the where argument
                                if (argument.Name.StringValue == "where" && argument.Value is GraphQLObjectValue filterValue)
                                {
                                    // Extract filter fields from the GraphQL query
                                    TraverseObjectValue(filterValue, filterFields);
                                }
                            }
                        }
                    }
                }
            }

            // Return the dictionary containing the extracted filter fields and values
            return filterFields;
        }

        // Recursively traverse and collect filter fields and their values
        private static void TraverseObjectValue(GraphQLObjectValue obj, Dictionary<string, object> filterFields)
        {
            foreach (var field in obj.Fields)
            {
                var fieldName = field.Name.StringValue;
                object fieldValue = null;

                // If the field value is another object (e.g., { _eq: value, _in: value })
                if (field.Value is GraphQLObjectValue nestedObject)
                {
                    // Iterate through the nested fields (operations like _eq, _in)
                    foreach (var nestedField in nestedObject.Fields)
                    {
                        var operationName = nestedField.Name.StringValue; // e.g., _eq, _in
                        if (nestedField.Value is GraphQLIntValue intValue)
                        {
                            fieldValue = intValue.Value;
                            break;  // Only take the first operation value
                        }
                        else if (nestedField.Value is GraphQLStringValue stringValue)
                        {
                            fieldValue = stringValue.Value;
                            break;  // Only take the first operation value
                        }
                    }
                }
                else if (field.Value is GraphQLStringValue stringValue)
                {
                    fieldValue = stringValue.Value;
                }
                else if (field.Value is GraphQLIntValue intValue)
                {
                    fieldValue = intValue.Value;
                }
                else if (field.Value is GraphQLBooleanValue boolValue)
                {
                    fieldValue = boolValue.Value;
                }
                else if (field.Value is GraphQLFloatValue floatValue)
                {
                    fieldValue = floatValue.Value;
                }

                // If the field has a valid value, add it to the dictionary
                if (fieldValue != null)
                {
                    filterFields[fieldName] = fieldValue;
                }
            }
        }
    }
}
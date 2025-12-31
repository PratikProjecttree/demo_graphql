using demo_graphql.Models;
using GraphQLParser;
using GraphQLParser.AST;
using GraphQLParser.Exceptions;

namespace demo_graphql.BAL
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

        public static List<ExtractedObject> ExtractObjectsArguments(string query)
        {
            var result = new List<ExtractedObject>();
            var document = Parser.Parse(query);

            foreach (var def in document.Definitions.OfType<GraphQLOperationDefinition>())
            {
                foreach (var selection in def.SelectionSet.Selections.OfType<GraphQLField>())
                {
                    foreach (var argument in selection?.Arguments ?? Enumerable.Empty<GraphQLArgument>())
                    {
                        // For each argument, potentially produce 0..N ExtractedObject entries
                        ExtractFromValue(argument.Value, result, argument?.Name?.ToString());
                    }
                }
            }

            return result;
        }

        private static void ExtractFromValue(GraphQLValue value, List<ExtractedObject> output, string parentArgName)
        {
            if (value == null) return;

            // If it's a single object literal
            if (value is GraphQLObjectValue objVal)
            {
                var eo = new ExtractedObject
                {
                    ArgName = parentArgName,
                    Role = GuessRoleFromArgName(parentArgName)
                };
                FlattenObjectFields(objVal, eo.Fields, prefix: null);
                output.Add(eo);
                return;
            }

            // If it's a list of objects
            if (value is GraphQLListValue listVal)
            {
                foreach (var item in listVal.Values)
                {
                    // only handle object entries here (scalars will be ignored)
                    if (item is GraphQLObjectValue childObj)
                    {
                        var eo = new ExtractedObject
                        {
                            ArgName = parentArgName,
                            Role = GuessRoleFromArgName(parentArgName)
                        };
                        FlattenObjectFields(childObj, eo.Fields, prefix: null);
                        output.Add(eo);
                    }
                    else
                    {
                        // if list items are scalars, you may want to record them under a special key
                    }
                }
                return;
            }

            // If it's a variable reference, record as a single-field ExtractedObject with special marker:
            if (value is GraphQLVariable variable)
            {
                var eo = new ExtractedObject
                {
                    ArgName = parentArgName,
                    Role = GuessRoleFromArgName(parentArgName)
                };
                // single placeholder entry to indicate the variable was provided for this arg
                eo.Fields["$variable"] = $"${variable.Name.StringValue}";
                output.Add(eo);
            }

            // other scalar or nested shapes are ignored here; FlattenObjectFields handles scalar fields inside object literals
        }

        // Flatten fields into dictionary using dotted keys for nested objects and indexed keys for object lists
        private static void FlattenObjectFields(GraphQLObjectValue objVal, Dictionary<string, string> dict, string prefix)
        {
            foreach (var field in objVal.Fields)
            {
                var key = string.IsNullOrEmpty(prefix) ? field.Name.StringValue : $"{prefix}.{field.Name.StringValue}";
                var val = field.Value;

                switch (val)
                {
                    case GraphQLObjectValue nestedObj:
                        FlattenObjectFields(nestedObj, dict, key);
                        break;

                    case GraphQLListValue listVal:
                        // scalar list -> comma string, object list -> indexed keys
                        var scalarValues = new List<string>();
                        int idx = 0;
                        foreach (var item in listVal.Values)
                        {
                            if (item is GraphQLObjectValue itemObj)
                            {
                                var subDict = new Dictionary<string, string>();
                                FlattenObjectFields(itemObj, subDict, null);
                                foreach (var kv in subDict)
                                    dict[$"{key}[{idx}].{kv.Key}"] = kv.Value;
                                idx++;
                            }
                            else
                            {
                                scalarValues.Add(ExtractScalarAsString(item));
                            }
                        }
                        if (scalarValues.Count > 0)
                            dict[key] = string.Join(",", scalarValues);
                        break;

                    case GraphQLVariable variable:
                        dict[key] = $"${variable.Name.StringValue}";
                        break;

                    default:
                        dict[key] = ExtractScalarAsString(val);
                        break;
                }
            }
        }

        private static ArgRole GuessRoleFromArgName(string argName)
        {
            if (string.IsNullOrEmpty(argName)) return ArgRole.Unknown;
            var n = argName.Trim().ToLowerInvariant();

            // common payload arg names
            if (n == "objects" || n == "_set" || n == "input" || n == "patch" || n == "object")
                return ArgRole.Payload;

            // common parameter/filter arg names
            if (n == "where" || n == "filter" || n == "id" || n == "condition")
                return ArgRole.Parameter;

            // otherwise unknown
            return ArgRole.Unknown;
        }

        // --- helper from previous responses: convert ROM->string and scalar extraction ---
        private static string RomToString(ReadOnlyMemory<char> rom) => new string(rom.Span);

        private static string ExtractScalarAsString(GraphQLValue value)
        {
            if (value == null) return null;

            switch (value)
            {
                case GraphQLStringValue s: return RomToString(s.Value);
                case GraphQLIntValue i: return RomToString(i.Value);
                case GraphQLFloatValue f: return RomToString(f.Value);
                case GraphQLBooleanValue b:
                    {
                        var txt = RomToString(b.Value);
                        if (bool.TryParse(txt, out var bv)) return bv ? "true" : "false";
                        return txt;
                    }
                case GraphQLEnumValue e:
                    // safe fallback: try reflection if needed, otherwise ToString()
                    try
                    {
                        // many versions have e.Value as ROM; try via reflection to avoid compile-time issues
                        var p = e.GetType().GetProperty("Value");
                        if (p != null && p.GetValue(e) is ReadOnlyMemory<char> rom) return RomToString(rom);
                    }
                    catch { /* ignore reflection failures */ }
                    return e.ToString();

                case GraphQLNullValue _: return null;
                case GraphQLVariable v: return $"${v.Name.StringValue}";
                default: return value.ToString();
            }
        }

        // public static List<Dictionary<string, string>> ExtractObjectsArguments(string query)
        // {
        //     var result = new List<Dictionary<string, string>>();
        //     var document = Parser.Parse(query);

        //     foreach (var definition in document.Definitions)
        //     {
        //         if (definition is GraphQLOperationDefinition operationDef)
        //         {
        //             foreach (var selection in operationDef.SelectionSet.Selections.OfType<GraphQLField>())
        //             {
        //                 foreach (var argument in selection.Arguments)
        //                 {
        //                     if (argument.Name.ToString() == "objects")
        //                     {
        //                         // Check if the value is a list of objects (GraphQLListValue)
        //                         if (argument.Value is GraphQLListValue listValue)
        //                         {
        //                             // Iterate over each object in the list of objects
        //                             foreach (var item in listValue.Values.OfType<GraphQLObjectValue>())
        //                             {
        //                                 var objectData = new Dictionary<string, string>();

        //                                 // Extract fields from each object and add to the dictionary
        //                                 foreach (var field in item.Fields)
        //                                 {
        //                                     string key = field.Name.ToString();
        //                                     string value = ExtractValue(field.Value);
        //                                     objectData[key] = value;
        //                                 }

        //                                 // Add the object data to the result list
        //                                 result.Add(objectData);
        //                             }
        //                         }
        //                         // Handle the case when the value is a single object (GraphQLObjectValue)
        //                         else if (argument.Value is GraphQLObjectValue objectValue)
        //                         {
        //                             var objectData = new Dictionary<string, string>();

        //                             // Extract fields from the single object and add to the dictionary
        //                             foreach (var field in objectValue?.Fields)
        //                             {
        //                                 string key = field.Name.ToString();
        //                                 string value = ExtractValue(field.Value);
        //                                 objectData[key] = value;
        //                             }

        //                             // Add the object data to the result list
        //                             result.Add(objectData);
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }

        //     return result;
        // }

        // private static string ExtractValue(ASTNode value)
        // {
        //     return value switch
        //     {
        //         GraphQLStringValue strVal => strVal.Value.ToString(),
        //         GraphQLIntValue intVal => intVal.Value.ToString(),
        //         GraphQLBooleanValue boolVal => boolVal.Value.ToString().ToLower(),
        //         GraphQLFloatValue floatVal => floatVal.Value.ToString(),
        //         _ => value.ToString()
        //     };
        // }

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
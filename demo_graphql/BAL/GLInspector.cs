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

        public static List<string> GetTopLevelFieldNames(string query)
        {
            var document = Parser.Parse(query);

            var fieldNames = new List<string>();

            foreach (var definition in document.Definitions)
            {
                if (definition is GraphQLOperationDefinition operation)
                {
                    var selections = operation.SelectionSet.Selections
                        .OfType<GraphQLField>();

                    foreach (var field in selections)
                    {
                        fieldNames.Add(field.Name.StringValue);
                    }
                }
            }

            return fieldNames;
        }
    }
}
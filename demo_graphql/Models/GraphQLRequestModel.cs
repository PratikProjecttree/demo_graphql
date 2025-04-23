
namespace demo_graphql.Models
{

    public class GraphQLRequestModel
    {
        public string query { get; set; }
        public string? variables { get; set; }
        public string operationName { get; set; }
    }
}
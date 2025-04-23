
namespace demo_graphql.Models
{

    public class GraphQLConfigurationModel
    {
        public string url { get; set; }
        public List<Headers> headers { get; set; }
    }

    public class Headers
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
namespace demo_graphql.Models
{
    public class GLRoutingModel
    {
        public string type { get; set; }
        public string category { get; set; }
        public Dictionary<string, string> headers { get; set; }
    }
}
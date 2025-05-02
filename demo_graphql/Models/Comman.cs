
namespace demo_graphql.Controllers
{
    public static class Category
    {
        public const string Default = "default";
        public const string Workflow = "workflow";
        public const string Custom = "custom";
    }
    public static class QueryType
    {
        public const string Query = "Query";
        public const string Mutation = "Mutation";
    }

    public static class AllowedUpdateOperators
    {
        public static readonly List<string> AllowedOperators = new List<string> { "_eq", "_in", "_and" };
    }

}